using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ePlatBack.Models.ViewModels;
using OpenPop.Pop3;
using OpenPop.Common;
using ePlatBack.Models.Utils;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.DataModels
{
    public class MailingDataModel
    {
        public static MailingViewModel GetMails(string folder, string search)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            MailingViewModel Mailing = new MailingViewModel();

            Mailing.Mails = new List<MailingViewModel.MailMessage>();
            List<MailingViewModel.MailMessage> NewMails = new List<MailingViewModel.MailMessage>();

            UserSession session = new UserSession();
            ePlatEntities db = new ePlatEntities();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];

            //obtener mails guardados
            var Mails = (from m in db.tblMailMessages
                         join l in db.tblLeads on m.leadID equals l.leadID
                         into l_m
                         from l in l_m.DefaultIfEmpty()
                         where m.userID == session.UserID
                         && (search == null || (m.subject.Contains(search) || m.emailAddress.Contains(search) || m.sender.Contains(search)))
                         orderby m.sendDate descending
                         select new
                         {
                             m.mailMessageID,
                             m.uid,
                             m.sender,
                             m.sendTo,
                             m.sendCC,
                             m.sendBCC,
                             m.emailAddress,
                             m.sendDate,
                             m.subject,
                             m.body,
                             m.attachments,
                             m.importance,
                             m.replied,
                             m.folder,
                             m.leadID,
                             m.dateDeleted,
                             l.firstName,
                             l.lastName
                         }).Take(50);

            //obtener los 100 más nuevos de la carpeta seleccionada
            foreach (var mail in Mails.OrderByDescending(x => x.sendDate).Where(x => x.folder == folder && x.dateDeleted == null))
            {
                MailingViewModel.MailMessage newMail = new MailingViewModel.MailMessage();
                newMail.MailMessageID = mail.mailMessageID;
                newMail.UID = mail.uid;
                newMail.Sender = mail.sender;
                newMail.To = js.Deserialize<List<MailingViewModel.MailAddress>>(mail.sendTo);
                if (mail.sendCC != null)
                {
                    newMail.CC = js.Deserialize<List<MailingViewModel.MailAddress>>(mail.sendCC);
                }
                if (mail.sendBCC != null)
                {
                    newMail.BCC = js.Deserialize<List<MailingViewModel.MailAddress>>(mail.sendBCC);
                }
                newMail.EmailAddress = mail.emailAddress;
                newMail.SendDate = mail.sendDate;
                newMail.Subject = mail.subject;
                newMail.Body = mail.body;
                if (mail.attachments != null)
                {
                    newMail.Attachments = js.Deserialize<List<MailingViewModel.MailAttachment>>(mail.attachments);
                }
                else
                {
                    newMail.Attachments = new List<MailingViewModel.MailAttachment>();
                }
                newMail.Importance = mail.importance;
                newMail.Replied = mail.replied;
                newMail.Folder = mail.folder;
                if (mail.leadID != null)
                {
                    newMail.LeadID = mail.leadID;
                    newMail.FirstName = mail.firstName;
                    newMail.LastName = mail.lastName;
                }
                if (newMail.Sender == "")
                {
                    newMail.Sender = newMail.EmailAddress;
                }

                Mailing.Mails.Add(newMail);
            }

            if (search == null)
            {
                //obtener mails no guardados
                var settings = (from s in db.tblMailingSettings
                                where s.userID == session.UserID
                                select s).FirstOrDefault();

                using (Pop3Client client = new Pop3Client())
                {
                    // Connect to the server
                    client.Connect(settings.popServer, Int32.Parse(settings.incomingPort), settings.popSsl);

                    // Authenticate ourselves towards the server
                    client.Authenticate(settings.popUsername, settings.popPassword);

                    // Get the number of messages in the inbox
                    int messageCount = client.GetMessageCount();
                    int errorCounter = 1;

                    // Messages are numbered in the interval: [1, messageCount]
                    // Ergo: message numbers are 1-based.
                    // Most servers give the latest message the highest number
                    for (int i = messageCount; i > 0; i--)
                    {
                        try
                        {
                            OpenPop.Mime.Message email = client.GetMessage(i);

                            if ((Mails.Count() == 0 && email.Headers.DateSent > DateTime.Today) || (Mails.Count() > 0 && email.Headers.DateSent > DateTime.Today.AddDays(-2) && Mails.FirstOrDefault(x => x.uid == email.Headers.MessageId) == null))
                            {
                                MailingViewModel.MailMessage newMail = new MailingViewModel.MailMessage();
                                newMail.MailMessageID = Guid.NewGuid();
                                newMail.UID = email.Headers.MessageId;
                                newMail.SendDate = email.Headers.DateSent;
                                newMail.Sender = email.Headers.From.DisplayName;
                                newMail.EmailAddress = email.Headers.From.Address;
                                newMail.To = new List<MailingViewModel.MailAddress>();
                                foreach (var to in email.Headers.To)
                                {
                                    newMail.To.Add(new MailingViewModel.MailAddress()
                                    {
                                        DisplayName = to.DisplayName,
                                        Address = to.Address
                                    });
                                }
                                newMail.CC = new List<MailingViewModel.MailAddress>();
                                foreach (var cc in email.Headers.Cc)
                                {
                                    newMail.CC.Add(new MailingViewModel.MailAddress()
                                    {
                                        DisplayName = cc.DisplayName,
                                        Address = cc.Address
                                    });
                                }
                                newMail.BCC = new List<MailingViewModel.MailAddress>();
                                foreach (var bcc in email.Headers.To)
                                {
                                    newMail.BCC.Add(new MailingViewModel.MailAddress()
                                    {
                                        DisplayName = bcc.DisplayName,
                                        Address = bcc.Address
                                    });
                                }
                                newMail.Subject = email.Headers.Subject;
                                OpenPop.Mime.MessagePart html = email.FindFirstHtmlVersion();
                                if (html != null)
                                {
                                    newMail.Body = html.GetBodyAsText();
                                }
                                else
                                {
                                    OpenPop.Mime.MessagePart plainText = email.FindFirstPlainTextVersion();
                                    if (plainText != null)
                                    {
                                        newMail.Body = plainText.GetBodyAsText();
                                    }
                                }
                                newMail.Attachments = new List<MailingViewModel.MailAttachment>();
                                //PENDIENTE obtener adjuntos
                                newMail.Importance = email.Headers.Importance.ToString();
                                newMail.Replied = false;
                                newMail.Folder = "inbox";

                                NewMails.Add(newMail);
                            }
                            else
                            {
                                //eliminar los que sean mayores a...
                                if (settings.deleteAfterDays > 0)
                                {
                                    if (email.Headers.DateSent < DateTime.Now.AddDays(settings.deleteAfterDays * -1))
                                    {
                                        client.DeleteMessage(i);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Mailing.Message += errorCounter + " - " + e.InnerException + "<br />";
                        }
                    }
                }

                if (NewMails.Count() > 0)
                {
                    List<string> newMailAddress = new List<string>();
                    newMailAddress = NewMails.Select(x => x.EmailAddress).ToList();
                    var Leads = from m in db.tblLeadEmails
                                join l in db.tblLeads on m.leadID equals l.leadID
                                into m_l
                                from l in m_l.DefaultIfEmpty()
                                where newMailAddress.Contains(m.email)
                                && l.terminalID == terminalID
                                orderby l.inputDateTime descending
                                select new
                                {
                                    m.leadID,
                                    m.email,
                                    l.firstName,
                                    l.lastName
                                };

                    foreach (var newMail in NewMails)
                    {
                        //guardar en db
                        tblMailMessages newMessage = new tblMailMessages();
                        newMessage.mailMessageID = newMail.MailMessageID;
                        newMessage.uid = newMail.UID;
                        newMessage.sender = newMail.Sender;
                        newMessage.sendTo = js.Serialize(newMail.To);
                        if (newMail.CC.Count() > 0)
                        {
                            newMessage.sendCC = js.Serialize(newMail.CC);
                        }
                        if (newMail.BCC.Count() > 0)
                        {
                            newMessage.sendBCC = js.Serialize(newMail.BCC);
                        }
                        newMessage.emailAddress = newMail.EmailAddress;
                        newMessage.sendDate = newMail.SendDate;
                        newMessage.subject = newMail.Subject;
                        newMessage.body = newMail.Body;
                        if (newMail.Attachments.Count() > 0)
                        {
                            newMessage.attachments = js.Serialize(newMail.Attachments);
                        }
                        newMessage.importance = newMail.Importance;
                        newMessage.replied = newMail.Replied;
                        newMessage.folder = newMail.Folder;
                        newMessage.userID = session.UserID;
                        //buscar si existe un lead en la terminal con ese email
                        if (Leads.FirstOrDefault(x => x.email == newMail.EmailAddress) != null)
                        {
                            var lead = Leads.FirstOrDefault(x => x.email == newMail.EmailAddress);
                            newMessage.leadID = lead.leadID;
                            newMail.LeadID = lead.leadID;
                            newMail.FirstName = lead.firstName;
                            newMail.LastName = lead.lastName;
                        }
                        if (newMail.Sender == "")
                        {
                            newMail.Sender = newMail.EmailAddress;
                        }

                        db.tblMailMessages.AddObject(newMessage);
                        Mailing.Mails.Add(newMail);
                    }

                    db.SaveChanges();
                }
            }

            Mailing.Mails = Mailing.Mails.OrderByDescending(m => m.SendDate).ToList();

            return Mailing;
        }

        public static List<FollowUpViewModel.LogItem> GetLogs(DateTime date, Guid[] userIDs)
        {
            List<FollowUpViewModel.LogItem> Logs = new List<FollowUpViewModel.LogItem>();
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];
            DateTime toDate = date.AddDays(1);
            int agentsLength = 0;
            List<Guid> agentsArr = new List<Guid>();
            if (userIDs != null)
            {
                agentsArr = userIDs.ToList();
                agentsLength = userIDs.Count();
            }
            ePlatEntities db = new ePlatEntities();
            if (agentsLength == 0)
            {
                //no hay agentes seleccionados, obtener los ids de los "agentes" de la terminal
                agentsArr = (from x in db.tblUsers_Terminals
                             join role in db.tblUsers_SysWorkGroups
                             on x.userID equals role.userID
                             into x_role
                             from role in x_role.DefaultIfEmpty()
                             join rolename in db.aspnet_Roles
                             on role.roleID equals rolename.RoleId
                             into role_name
                             from rolename in role_name.DefaultIfEmpty()
                             join memb in db.aspnet_Membership
                             on x.userID equals memb.UserId
                             into x_memb
                             from memb in x_memb.DefaultIfEmpty()
                             where x.terminalID == terminalID
                             && memb.IsApproved == true
                             && (rolename.RoleName.Contains("agent") || rolename.RoleName.Contains("supervisor"))
                             select x.userID).ToList();
            }

            // agregar para pruebas mi email address manualmente
            agentsArr.Add(new Guid("24B1B490-3796-4354-A5EA-3FA1C085BEF3"));

            var Emails = from m in db.tblMailMessages
                         join i in db.tblInteractions
                         on m.interactionID equals i.interactionID
                         into m_interaction
                         from i in m_interaction.DefaultIfEmpty()
                         join profile in db.tblUserProfiles
                         on m.userID equals profile.userID
                         into m_profile
                         from profile in m_profile.DefaultIfEmpty()
                         join l in db.tblLeads
                         on m.leadID equals l.leadID
                         into m_lead
                         from l in m_lead.DefaultIfEmpty()
                         join qualification in db.tblQualificationStatus
                         on l.qualificationStatusID equals qualification.qualificationStatusID
                         join d in db.tblDestinations
                         on l.interestedInDestinationID equals d.destinationID
                         into lead_destination
                         from d in lead_destination.DefaultIfEmpty()
                         join p in db.tblQuotations
                         on m.leadID equals p.leadID
                         into l_p
                         from p in l_p.DefaultIfEmpty()
                         join c in db.tblCurrencies
                         on p.currencyID equals c.currencyID
                         into p_currency
                         from c in p_currency.DefaultIfEmpty()
                         join il in db.tblInterestLevels
                         on i.interestLevelID equals il.interestLevelID
                         into interaction_il
                         from il in interaction_il.DefaultIfEmpty()
                         join bs in db.tblBookingStatus
                         on i.bookingStatusID equals bs.bookingStatusID
                         into interaction_bs
                         from bs in interaction_bs.DefaultIfEmpty()
                         join dr in db.tblDiscardReasons
                         on i.discardReasonID equals dr.discardReasonID
                         into interaction_dr
                         from dr in interaction_dr.DefaultIfEmpty()
                         where agentsArr.Contains(m.userID)
                         && m.leadID != null
                         && m.sendDate >= date
                         && m.sendDate < toDate
                         select new
                         {
                             m.mailMessageID,
                             m.leadID,
                             m.interactionID,
                             m.sendDate,
                             m.subject,
                             agent = profile.firstName + " " + profile.lastName,
                             l.firstName,
                             l.lastName,
                             l.qualificationStatusID,
                             qualification.qualificationStatus,
                             l.interestedInDestinationID,
                             d.destination,
                             p.total,
                             c.currencyCode,
                             p.currencyID,
                             i.interactionComments,
                             i.interestLevelID,
                             il.interestLevel,
                             i.bookingStatusID,
                             bs.bookingStatus,
                             i.discardReasonID,
                             dr.discardReason,
                             l.terminalID
                         };

            //phones
            var leadIDs = Emails.Where(x => x.leadID != null).Select(x => x.leadID).Distinct();

            var Phones = from p in db.tblPhones
                         where leadIDs.Contains(p.leadID)
                         select p;

            //emails
            var MailEmails = from e in db.tblLeadEmails
                             where leadIDs.Contains(e.leadID)
                             && e.main == true
                             select new
                             {
                                 e.leadID,
                                 e.email,
                                 e.emailID
                             };

            foreach (var email in Emails)
            {
                FollowUpViewModel.LogItem log = new FollowUpViewModel.LogItem();
                log.MailMessageID = email.mailMessageID;
                log.InteractionID = email.interactionID;
                log.InteractionType = "Email";
                log.InteractionTypeID = 2;
                log.Agent = email.agent;
                log.Date = email.sendDate;
                log.LeadID = email.leadID;
                log.FirstName = email.firstName;
                log.LastName = email.lastName;
                log.EmailSubject = email.subject;
                if (email.leadID != null)
                {
                    if (Phones.FirstOrDefault(p => p.leadID == email.leadID) != null)
                    {
                        log.Phone = Phones.FirstOrDefault(p => p.leadID == email.leadID).phone;
                    }

                    if (MailEmails.FirstOrDefault(m => m.leadID == email.leadID) != null)
                    {
                        log.Email = MailEmails.FirstOrDefault(m => m.leadID == email.leadID).email;
                        log.EmailID = MailEmails.FirstOrDefault(m => m.leadID == email.leadID).emailID;
                    }
                }
                log.QualificationStatus = email.qualificationStatus;
                log.QualificationStatusID = email.qualificationStatusID;
                log.InterestedInDestination = email.destination;
                log.InterestedInDestinationID = email.interestedInDestinationID;
                log.Total = email.total;
                log.CurrencyCode = email.currencyCode;
                log.CurrencyID = email.currencyID;
                log.FirstContact = false;
                log.InteractionComments = email.interactionComments;
                log.InterestLevel = email.interestLevel;
                log.InterestLevelID = email.interestLevelID;
                log.BookingStatus = email.bookingStatus;
                log.BookingStatusID = email.bookingStatusID;
                log.DiscardReason = email.discardReason;
                log.DiscardReasonID = email.discardReasonID;
                log.TerminalID = email.terminalID;
                Logs.Add(log);
            }

            return Logs;
        }

        public static AttemptResponse DeleteMail(Guid id)
        {
            AttemptResponse response = new AttemptResponse();

            ePlatEntities db = new ePlatEntities();
            var Mail = (from m in db.tblMailMessages
                        where m.mailMessageID == id
                        select m).FirstOrDefault();

            Mail.folder = "trash";
            db.SaveChanges();

            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "Mail Saved";

            return response;
        }

        public static List<MailingViewModel.GetMailing> GetSentEmails(string type, string typeID)
        {
            ePlatEntities db = new ePlatEntities();
            List<MailingViewModel.GetMailing> emails = new List<MailingViewModel.GetMailing>();

            Guid itemID = typeID != null && typeID != "" ? Guid.Parse(typeID) : Guid.Empty;
            int tourID = type == "tour" ? int.Parse(typeID) : 0;

            //IQueryable<MailingViewModel.GetMailing> emailLogList = (from email in db.tblEmailNotificationLogs
            List<MailingViewModel.GetMailing> emailLogList = (from email in db.tblEmailNotificationLogs
                                                                    join user in db.tblUserProfiles on email.sentByUserID equals user.userID
                                                                    join n in db.tblEmailNotifications on email.emailNotificationID equals n.emailNotificationID
                                                                    where (type == "reservation" && email.reservationID == itemID)
                                                                    || (type == "tour" && email.tourID == tourID)
                                                                         || (type == "invitation" && email.invitationID == itemID)
                                                                         || (type == "lead" && email.leadID == itemID)
                                                                         || (type == "purchase" && email.purchaseID == itemID)
                                                                    select new MailingViewModel.GetMailing()
                                                                    {
                                                                        emailNotificationLogID = email.emailNotificationLogID,
                                                                        emailNotificationID = email.emailNotificationID,
                                                                        emailID = n.emailID,
                                                                        description = n.tblEmails.description,
                                                                        dateSent = email.dateSent,
                                                                        sentByUserID = email.sentByUserID,
                                                                        sentByUser = user.firstName + " " + user.lastName,
                                                                        leadID = itemID,
                                                                        tourID = type == "tour" ? tourID : (int?)null,
                                                                        trackingID = email.trackingID,
                                                                        preview = email.emailPreviewJson,
                                                                        subject = email.subject,
                                                                        lastOpen = (DateTime?)null,
                                                                        lastClick = (DateTime?)null,
                                                                        sysEventID = n.eventID
                                                                    }).OrderByDescending(x => x.dateSent).ToList();
            var logList = emailLogList.Select(m => m.emailNotificationLogID).ToList();
            List<MailingViewModel.MailingTracking> tracks = (from track in db.tblEmailNotificationEvents
                                                             join sysEvent in db.tblSysEvents on track.sysEventID equals sysEvent.eventID
                                                             //where emailLogList.Count(x => x.emailNotificationLogID == track.emailNotificationLogID) > 0
                                                             where logList.Contains(track.emailNotificationLogID)
                                                             select new MailingViewModel.MailingTracking
                                                             {
                                                                 emailNotificationEventID = track.emailNotificationEventID,
                                                                 emailNotificationLogID = track.emailNotificationLogID,
                                                                 sysEventID = track.sysEventID,
                                                                 sysEvent = sysEvent.@event,
                                                                 dateEvent = track.dateEvent,
                                                                 url = track.url
                                                             }).OrderByDescending(x => x.dateEvent).ToList();
            if (emailLogList.Count() > 0)
            {
                foreach (var email in emailLogList)
                {

                    email.trackingInfo = new List<MailingViewModel.MailingTracking>();
                    email.preview = GetEmailPreview(email.preview);
                    if (tracks.Count(x => x.emailNotificationLogID == email.emailNotificationLogID) > 0)
                    {
                        var currentTracking = tracks.Where(x => x.emailNotificationLogID == email.emailNotificationLogID);
                        var open = currentTracking.FirstOrDefault(x => x.sysEventID == 36);//open
                        email.lastOpen = open != null ? open.dateEvent : (DateTime?)null;
                        if (currentTracking.Count(x => x.sysEventID == 37) > 0)//clicked
                        {
                            var click = currentTracking.FirstOrDefault(x => x.sysEventID == 37);
                            email.lastClick = click != null ? click.dateEvent : (DateTime?)null;
                        }
                        email.trackingInfo.AddRange(currentTracking);
                    }
                    emails.Add(email);
                }
            }
            return emails;
        }

        public static string GetEmailPreview(string model)
        {
            if (model != null && model != "" && model != "null")
            {
                //var email = new JavaScriptSerializer().Deserialize<EmailPreViewModel>(model);
                try
                {
                    var email = new JavaScriptSerializer().Deserialize<EmailPreViewModel>(model);
                    return email.Body;
                }
                catch
                {
                    return model;
                }
            }
            return "";
        }

        //public static object PreviewEmail(string itemType, string itemID, int? emailNotificationID)
        //{

        //    return new object { };
        //}

        //public static object SendEmail(MailingViewModel.EmailPreViewModel model)
        //{
        //    return new object { };
        //}

        ///// <summary>
        ///// Redirect urls to method designed for saving action and redirects to original url
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public static MailingViewModel.EmailPreViewModel InsertTracker(MailingViewModel.EmailPreViewModel model)
        //{
        //    var body = model.Body;
        //    var anchors = Regex.Matches(body, @"(<a.*?.*?</a>)", RegexOptions.Singleline);

        //    foreach(Match match in anchors)
        //    {
        //        var before = match.Groups[1].Value;
        //        var current = match.Groups[1].Value;
        //        Match item = Regex.Match(current, @"href=\""(.*?)\""", RegexOptions.Singleline);

        //        if (item.Success)
        //        {
        //            if(item.Groups[1].Value.IndexOf("mailto") == -1)
        //            {
        //                current = current.Replace("href=\"", "href=\"https://eplat.villagroup.com/Public/OpenUrl/" + model.TransactionID + "?url=");
        //                body = body.Replace(before, current);
        //            }
        //        }
        //    }

        //    body += "<img src=\"https://eplat.villagroup.com/Public/GetImage/" + model.TransactionID + "\" style=\"position:absolute;z-index:-10; opacity:0;\" />";

        //    model.Body = body;

        //    return model;
        //}

        //public static object SaveInFieldsValues(EmailPreViewModel model)
        //{
        //    return new object { };
        //}

        //public static object SaveInNotificationLogs(EmailPreViewModel model)
        //{
        //    return new object { };
        //}
    }
}
