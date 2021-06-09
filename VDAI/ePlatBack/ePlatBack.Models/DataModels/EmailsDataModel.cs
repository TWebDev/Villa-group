using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.eplatformDataModel;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Collections;
using System.Net.Mail;
using System.Net;

namespace ePlatBack.Models.DataModels
{
    public class EmailsDataModel
    {
        public static UserSession session = new UserSession();
        public class EmailsCatalogs
        {
            public static List<SelectListItem> FillDrpEmailTemplatesByCulture(string culture)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                if (culture != "0")
                {
                    var query = db.tblEmailTemplates.Where(m => m.culture == culture);
                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.emailTemplateID.ToString(),
                            Text = i.emailTemplate
                        });
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpSysEventsPerTerminals(long terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                if (terminalID == 0)
                {
                    list.Add(ListItems.Default("--Select Terminal--"));
                    return list;
                }

                var query = db.tblSysEvents_Terminals.Where(m => m.terminalID == terminalID).Select(m => new { m.tblSysEvents.eventID, m.tblSysEvents.@event }).Distinct();

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.eventID.ToString(),
                        Text = i.@event
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpEmailsByTerminals(long terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                if (terminalID == 0)
                {
                    list.Add(ListItems.Default("--Select Terminal--"));
                    return list;
                }

                var query = db.tblEmails_Terminals.Where(m => m.terminalID == terminalID).Select(m => m.tblEmails);

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.emailID.ToString(),
                        Text = (i.description != null && i.description != "" ? i.description : i.subject) + " - " + i.culture
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpRequiredFields()
            {
                var list = new List<SelectListItem>();
                var query = GeneralFunctions.RequiredFields;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.Key,
                        Text = i.Value
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpFieldGroups()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblFieldGroups;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.fieldGroupID.ToString(),
                        Text = i.fieldGroup
                    });
                }

                return list;
            }

            public static List<SelectListItem> FillDrpFieldsPerGroup(int fieldGroupID)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                if (fieldGroupID != 0)
                {
                    var query = db.tblFields.Where(m => m.fieldGroupID == fieldGroupID);
                    if (query.Count() > 0)
                    {
                        foreach (var i in query)
                        {
                            list.Add(new SelectListItem()
                            {
                                Value = i.fieldID.ToString(),
                                Text = i.field
                            });
                        }
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpFieldGuidsPerGroup(int fieldGroupID)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                if (fieldGroupID != 0)
                {
                    var query = db.tblFields.Where(m => m.fieldGroupID == fieldGroupID);
                    if (query.Count() > 0)
                    {
                        foreach (var i in query)
                        {
                            list.Add(new SelectListItem()
                            {
                                Value = i.fieldGuid.ToString(),
                                Text = i.field
                            });
                        }
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpFieldTypes()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblFieldTypes;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.fieldTypeID.ToString(),
                        Text = i.fieldType
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpFieldSubTypes(int fieldTypeID)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                if (fieldTypeID != 0)
                {
                    var query = db.tblFieldSubTypes.Where(m => m.fieldTypeID == fieldTypeID);

                    if (query.Count() > 0)
                    {
                        foreach (var i in query)
                        {
                            list.Add(new SelectListItem()
                            {
                                Value = i.fieldSubTypeID.ToString(),
                                Text = i.subType
                            });
                        }
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpEmailNotifications()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();

                //var query = db.tblEmailNotifications.Where(m => terminals.Contains(m.terminalID) && (m.active != null && (bool)m.active));
                var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
                var query = db.tblEmailNotifications.Where(m => terminals.Contains(m.terminalID) && (isAdmin || (m.active != null && (bool)m.active)));

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.emailNotificationID.ToString(),
                        Text = i.description ?? i.tblEmails.subject
                    });
                }

                return list;
            }

            public static string GetURLDomain(Guid purchaseID)
            {
                ePlatEntities eplat = new ePlatEntities();

                var purchase = eplat.tblPurchases.Single(m => m.purchaseID == purchaseID);
                var pos = purchase.pointOfSaleID;
                var posDomain = eplat.tblPointsOfSale.Single(m => m.pointOfSaleID == pos).domain;
                var domain = "";

                posDomain = posDomain != null ? posDomain : eplat.tblTerminalDomains.FirstOrDefault(m => m.terminalID == purchase.terminalID && m.culture == purchase.culture && m.domain.IndexOf("localhost") == -1 && m.domain.IndexOf("beta") == 1).domain;
                posDomain = posDomain.Replace("//my", "//www.my");
                domain = "https://" + (posDomain.IndexOf("experience.com") != -1 ? posDomain : ((purchase.culture == "es-MX" ? "mx." : "") + "eplatfront.villagroup.com"));

                return domain;
            }

            public static long GetTerminalIDFromEplat(int terminalID)
            {
                switch (terminalID)
                {
                    case 18://Today Getaway
                        {
                            return 9;
                        }
                    case 50://VDP Cancun Reservations
                        {
                            return 44;
                        }
                    case 51://SD Reservations
                        {
                            return 45;
                        }
                    case 62://Travel Option Inc
                        {
                            return 46;
                        }
                    case 68://GB
                        {
                            return 33;
                        }
                    default:
                        {
                            return 0;
                        }
                }
            }

            public static bool CheckRequiredFields(tblReservations rsv, List<string> requiredFields)
            {
                foreach (var field in requiredFields)
                {
                    switch (field)
                    {
                        case "First Name":
                            {
                                if (rsv.tblLeads.firstName == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Last Name":
                            {
                                if (rsv.tblLeads.lastName == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Email":
                            {
                                if (rsv.tblLeads.tblLeadEmails == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Hotel Confirmation Number":
                            {
                                if (rsv.hotelConfirmationNumber == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Arrival Date":
                            {
                                if (rsv.arrivalDate == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Destination":
                            {
                                if (rsv.destinationID == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Resort":
                            {
                                if (rsv.placeID == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Presentation Date":
                            {
                                //if (rsv.tblPresentations == null || rsv.tblPresentations.FirstOrDefault().datePresentation == null)
                                if (rsv.tblPresentations.Count() == 0 || rsv.tblPresentations.FirstOrDefault().datePresentation == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        case "Presentation Time":
                            {
                                if (rsv.tblPresentations == null || rsv.tblPresentations.FirstOrDefault().timePresentation == null)
                                {
                                    return false;
                                }
                                break;
                            }
                        default:
                            {
                                return true;
                            }
                    }
                }
                return true;
            }
        }

        public List<EmailInfoModel> SearchEmails(EmailsSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            List<EmailInfoModel> list = new List<EmailInfoModel>();

            int culturesLength = model.EmailSearch_Culture != null ? model.EmailSearch_Culture.Length : 0;
            var culturesArray = model.EmailSearch_Culture != null ? model.EmailSearch_Culture.ToArray() : new string[] { };
            var terminals = model.EmailSearch_Terminals != null ? model.EmailSearch_Terminals.ToArray() : session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var query = from email in db.tblEmails
                        where email.tblEmails_Terminals.Count(m => terminals.Contains(m.terminalID)) > 0
                        && (culturesLength == 0 || culturesArray.Contains(email.culture))
                        && (model.EmailSearch_Subject == null || email.subject.Contains(model.EmailSearch_Subject))
                        && (model.EmailSearch_Sender == null || email.sender.Contains(model.EmailSearch_Sender))
                        select email;

            foreach (var i in query)
            {
                list.Add(new EmailInfoModel()
                {
                    EmailInfo_EmailID = i.emailID,
                    EmailInfo_TerminalsText = string.Join(", ", i.tblEmails_Terminals.Select(m => m.tblTerminals.terminal)),
                    EmailInfo_Culture = db.tblLanguages.Single(m => m.culture == i.culture).language,
                    EmailInfo_EmailTemplate = i.emailTemplateID,
                    EmailInfo_EmailTemplateText = i.tblEmailTemplates.emailTemplate,
                    EmailInfo_Subject = i.subject,
                    EmailInfo_Content = i.content_,
                    EmailInfo_Sender = i.sender,
                    EmailInfo_Alias = i.alias,
                    EmailInfo_Description = i.description
                });
            }

            return list;
        }

        public EmailInfoModel GetEmailInfo(int EmailID)
        {
            ePlatEntities db = new ePlatEntities();
            EmailInfoModel model = new EmailInfoModel();

            var query = db.tblEmails.Single(m => m.emailID == EmailID);

            model.EmailInfo_EmailID = query.emailID;
            model.EmailInfo_Terminals = query.tblEmails_Terminals.Select(m => m.terminalID).ToArray();
            model.EmailInfo_Culture = query.culture;
            model.EmailInfo_EmailTemplate = query.emailTemplateID;
            model.EmailInfo_Subject = query.subject;
            model.EmailInfo_Sender = query.sender;
            model.EmailInfo_Alias = query.alias;
            model.EmailInfo_Description = query.description;
            model.EmailInfo_Content = query.content_;

            return model;
        }

        public AttemptResponse SaveEmail(EmailInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.EmailInfo_EmailID != null && model.EmailInfo_EmailID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblEmails.Single(m => m.emailID == model.EmailInfo_EmailID);
                    query.culture = model.EmailInfo_Culture;
                    query.subject = model.EmailInfo_Subject;
                    query.content_ = model.EmailInfo_Content;
                    query.emailTemplateID = model.EmailInfo_EmailTemplate;
                    query.sender = model.EmailInfo_Sender ?? "empty@empty.com";
                    query.alias = model.EmailInfo_Alias;
                    query.description = model.EmailInfo_Description;

                    IQueryable<tblEmails_Terminals> savedTerminals = db.tblEmails_Terminals.Where(m => m.emailID == query.emailID);

                    foreach (var i in model.EmailInfo_Terminals)
                    {
                        if (savedTerminals.Count(m => m.terminalID == i) > 0)
                        {
                            savedTerminals = savedTerminals.Where(m => m.terminalID != i);
                        }
                        else
                        {
                            tblEmails_Terminals terminal = new tblEmails_Terminals();
                            terminal.terminalID = i;
                            terminal.emailID = query.emailID;
                            db.tblEmails_Terminals.AddObject(terminal);
                        }
                    }

                    if (savedTerminals.Count() > 0)
                    {
                        foreach (var i in savedTerminals)
                        {
                            db.DeleteObject(i);
                        }
                    }

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Email Updated";
                    response.ObjectID = query.emailID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Email NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    tblEmails query = new tblEmails();
                    query.culture = model.EmailInfo_Culture;
                    query.subject = model.EmailInfo_Subject;
                    query.content_ = model.EmailInfo_Content;
                    query.emailTemplateID = model.EmailInfo_EmailTemplate;
                    query.sender = model.EmailInfo_Sender ?? "empty@empty.com";
                    query.alias = model.EmailInfo_Alias;
                    query.description = model.EmailInfo_Description;

                    foreach (var i in model.EmailInfo_Terminals)
                    {
                        tblEmails_Terminals terminal = new tblEmails_Terminals();
                        terminal.terminalID = i;
                        query.tblEmails_Terminals.Add(terminal);
                    }
                    db.tblEmails.AddObject(query);

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Email Saved";
                    response.ObjectID = query.emailID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Email NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public List<EmailNotificationInfoModel> SearchEmailNotifications(EmailNotificationsSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            List<EmailNotificationInfoModel> list = new List<EmailNotificationInfoModel>();

            //var _terminals = model.EmailNotificationsSearch_Terminal != null ? model.EmailNotificationsSearch_Terminal.Select(m => m).ToArray() : TerminalDataModel.GetActiveTerminalsList().Select(m => (long?)long.Parse(m.Value)).ToArray();
            //var _destinations = model.EmailNotificationsSearch_Destination != null ? model.EmailNotificationsSearch_Destination.Select(m => m).ToArray() : PlaceDataModel.GetDestinationsByCurrentTerminals().Select(m => (long?)long.Parse(m.Value)).ToArray();
            //var _resorts = model.EmailNotificationsSearch_Resort != null ? model.EmailNotificationsSearch_Resort.Select(m => (long)m).ToArray() : PlaceDataModel.GetResortsByDestinations(_destinations).Select(m => long.Parse(m.Value)).ToArray();
            var _terminalsLength = model.EmailNotificationsSearch_Terminal != null ? model.EmailNotificationsSearch_Terminal.Length : 0;
            var _terminals = model.EmailNotificationsSearch_Terminal ?? TerminalDataModel.GetActiveTerminalsList().Select(m => (long?)long.Parse(m.Value)).ToArray();
            var _destinationsLength = model.EmailNotificationsSearch_Destination != null ? model.EmailNotificationsSearch_Destination.Length : 0;
            var _destinations = model.EmailNotificationsSearch_Destination != null ? model.EmailNotificationsSearch_Destination.Select(m => (long)m) : new long[] { };
            var _resortsLength = model.EmailNotificationsSearch_Resort != null ? model.EmailNotificationsSearch_Resort.Length : 0;
            var _resorts = model.EmailNotificationsSearch_Resort != null ? model.EmailNotificationsSearch_Resort.Select(m => (long)m) : new long[] { };

            //var query = db.tblEmailNotifications.Where(m => (_terminalsLength == 0 || _terminals.Contains(m.terminalID)) && (_destinationsLength == 0 || _destinations.Contains(m.destinationID)));
            var query = db.tblEmailNotifications.Where(m => _terminals.Contains(m.terminalID));
            query = query.Where(m => (_destinationsLength == 0 || _destinations.Intersect(db.tblEmailNotifications_Destinations.Where(x => x.emailNotificationID == m.emailNotificationID).Select(x => x.destinationID)).Any()));
            query = query.Where(m => (_resortsLength == 0 || _resorts.Intersect(db.tblEmailNotifications_Places.Where(x => x.emailNotificationID == m.emailNotificationID).Select(x => x.placeID)).Any()));

            foreach (var i in query)
            {
                var fields = "";
                if (i.requiredFields != null)
                {
                    foreach (var a in i.requiredFields.Split(','))
                    {
                        fields += (fields != "" ? ", " : "") + GeneralFunctions.RequiredFields.Single(m => m.Key == a).Value;
                    }
                }
                list.Add(new EmailNotificationInfoModel()
                {
                    EmailNotificationInfo_EmailNotificationID = i.emailNotificationID,
                    EmailNotificationInfo_Email = i.emailID,
                    EmailNotificationInfo_EmailSubject = i.tblEmails.subject,
                    EmailNotificationInfo_Terminal = i.terminalID,
                    EmailNotificationInfo_TerminalText = i.terminalID != null ? i.tblTerminals.terminal : "",
                    EmailNotificationInfo_Event = i.eventID,
                    EmailNotificationInfo_EventText = i.tblSysEvents.@event,
                    EmailNotificationInfo_EmailAccounts = i.emailAccounts,
                    EmailNotificationInfo_DestinationText = string.Join(",", i.tblEmailNotifications_Destinations.Where(m => m.active).Select(m => m.tblDestinations.destination)),
                    EmailNotificationInfo_CopyLead = i.copyLead ?? false,
                    EmailNotificationInfo_CopyLeadText = i.copyLead != null ? (bool)i.copyLead ? "True" : "False" : "False",
                    EmailNotificationInfo_RequiredFieldsText = fields,
                    EmailNotificationInfo_LeadSourcesText = string.Join(", ", i.tblEmailNotifications_LeadSources.Select(m => m.tblLeadSources.leadSource)),
                    EmailNotificationInfo_PointsOfSaleText = string.Join(", ", i.tblEmailNotifications_PointsOfSale.Select(m => m.tblPointsOfSale.pointOfSale)),
                    EmailNotificationInfo_LeadStatusText = string.Join(", ", i.tblEmailNotifications_LeadStatus.Select(m => m.tblLeadStatus.leadStatus)),
                    EmailNotificationInfo_BookingStatusText = string.Join(", ", i.tblEmailNotifications_BookingStatus.Select(m => m.tblBookingStatus.bookingStatus)),
                    EmailNotificationInfo_PreBookingStatusText = string.Join(", ", i.tblEmailNotifications_SecondaryBookingStatus.Select(m => m.tblBookingStatus.bookingStatus)),
                    EmailNotificationInfo_BrokerContractsText = string.Join(", ", i.tblEmailNotifications_BrokerContracts.Select(m => m.tblBrokerContracts.brokerContractFullName)),
                    EmailNotificationInfo_ResortsText = string.Join(", ", i.tblEmailNotifications_Places.Select(m => m.tblPlaces.place)),
                    EmailNotificationInfo_Description = i.description,
                    EmailNotificationInfo_IsActive = (bool)i.active
                });
            }

            return list;
        }

        public EmailNotificationInfoModel GetEmailNotificationInfo(int EmailNotificationID)
        {
            ePlatEntities db = new ePlatEntities();
            EmailNotificationInfoModel model = new EmailNotificationInfoModel();

            var query = db.tblEmailNotifications.Single(m => m.emailNotificationID == EmailNotificationID);

            model.EmailNotificationInfo_EmailNotificationID = query.emailNotificationID;
            model.EmailNotificationInfo_Email = query.emailID;
            model.EmailNotificationInfo_Event = query.eventID;
            model.EmailNotificationInfo_ReplyTo = query.replyTo;
            model.EmailNotificationInfo_EmailAccounts = query.emailAccounts;
            model.EmailNotificationInfo_Terminal = query.terminalID;
            model.EmailNotificationInfo_Destinations = query.tblEmailNotifications_Destinations.Select(m => m.destinationID).ToArray();
            model.EmailNotificationInfo_Resorts = query.tblEmailNotifications_Places.Select(m => m.placeID).ToArray();
            model.EmailNotificationInfo_LeadSources = query.tblEmailNotifications_LeadSources.Select(m => m.leadSourceID).ToArray();
            model.EmailNotificationInfo_PointsOfSale = query.tblEmailNotifications_PointsOfSale.Select(m => m.pointOfSaleID).ToArray();
            model.EmailNotificationInfo_LeadStatus = query.tblEmailNotifications_LeadStatus.Select(m => m.leadStatusID).ToArray();
            model.EmailNotificationInfo_BookingStatus = query.tblEmailNotifications_BookingStatus.Select(m => m.bookingStatusID).ToArray();
            model.EmailNotificationInfo_PreBookingStatus = query.tblEmailNotifications_SecondaryBookingStatus.Select(m => m.secondaryBookingStatusID).ToArray();
            model.EmailNotificationInfo_BrokerContracts = query.tblEmailNotifications_BrokerContracts.Select(m => m.brokerContractID).ToArray();
            model.EmailNotificationInfo_RequiredFields = query.requiredFields != null ? query.requiredFields.Split(',').ToArray() : new string[] { };
            model.EmailNotificationInfo_Description = query.description;
            model.EmailNotificationInfo_CopyLead = query.copyLead ?? false;
            model.EmailNotificationInfo_CopySender = query.copySender ?? false;
            model.EmailNotificationInfo_IsActive = query.active ?? false;

            return model;
        }

        public AttemptResponse SaveEmailNotification(EmailNotificationInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.EmailNotificationInfo_EmailNotificationID != null && model.EmailNotificationInfo_EmailNotificationID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblEmailNotifications.Single(m => m.emailNotificationID == model.EmailNotificationInfo_EmailNotificationID);
                    var queryDestinations = query.tblEmailNotifications_Destinations.ToArray();
                    var queryResorts = query.tblEmailNotifications_Places.ToArray();
                    var queryLeadSources = query.tblEmailNotifications_LeadSources.ToArray();
                    var queryLeadStatus = query.tblEmailNotifications_LeadStatus.ToArray();
                    var queryBookingStatus = query.tblEmailNotifications_BookingStatus.ToArray();
                    var querySecondaryBookingStatus = query.tblEmailNotifications_SecondaryBookingStatus.ToArray();
                    var queryBrokerContracts = query.tblEmailNotifications_BrokerContracts.ToArray();
                    var queryPos = query.tblEmailNotifications_PointsOfSale.ToArray();

                    query.emailID = model.EmailNotificationInfo_Email;
                    query.eventID = model.EmailNotificationInfo_Event;
                    query.replyTo = model.EmailNotificationInfo_ReplyTo;
                    query.emailAccounts = model.EmailNotificationInfo_EmailAccounts != null ? model.EmailNotificationInfo_EmailAccounts.Trim() : null;
                    query.terminalID = model.EmailNotificationInfo_Terminal;
                    query.requiredFields = model.EmailNotificationInfo_RequiredFields != null ? string.Join(",", model.EmailNotificationInfo_RequiredFields) : null;
                    query.description = model.EmailNotificationInfo_Description;
                    query.copyLead = model.EmailNotificationInfo_CopyLead;
                    query.copySender = model.EmailNotificationInfo_CopySender;
                    query.active = model.EmailNotificationInfo_IsActive;

                    #region "destinations"
                    if (model.EmailNotificationInfo_Destinations != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_Destinations)
                        {
                            if (queryDestinations.Where(m => m.destinationID == i).Count() > 0)
                            {
                                queryDestinations = queryDestinations.Where(m => m.destinationID != i).ToArray();
                            }
                            else
                            {
                                var _dest = new tblEmailNotifications_Destinations();
                                _dest.destinationID = i;
                                query.tblEmailNotifications_Destinations.Add(_dest);
                            }
                        }
                        if (queryDestinations.Count() > 0)
                        {
                            foreach (var i in queryDestinations)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (queryDestinations.Count() > 0)
                        {
                            foreach (var i in queryDestinations)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    #region "resorts"
                    if (model.EmailNotificationInfo_Resorts != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_Resorts)
                        {
                            if (queryResorts.Where(m => m.placeID == i).Count() > 0)
                            {
                                queryResorts = queryResorts.Where(m => m.placeID != i).ToArray();
                            }
                            else
                            {
                                var _resort = new tblEmailNotifications_Places();
                                _resort.placeID = i;
                                query.tblEmailNotifications_Places.Add(_resort);
                            }
                        }
                        if (queryResorts.Count() > 0)
                        {
                            foreach (var i in queryResorts)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (queryResorts.Count() > 0)
                        {
                            foreach (var i in queryResorts)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    #region "lead sources"
                    if (model.EmailNotificationInfo_LeadSources != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_LeadSources)
                        {
                            if (queryLeadSources.Where(m => m.leadSourceID == i).Count() > 0)
                            {
                                queryLeadSources = queryLeadSources.Where(m => m.leadSourceID != i).ToArray();
                            }
                            else
                            {
                                var _leadSource = new tblEmailNotifications_LeadSources();
                                _leadSource.leadSourceID = i;
                                query.tblEmailNotifications_LeadSources.Add(_leadSource);
                            }
                        }
                        if (queryLeadSources.Count() > 0)
                        {
                            foreach (var i in queryLeadSources)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (queryLeadSources.Count() > 0)
                        {
                            foreach (var i in queryLeadSources)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    #region "points of sale"
                    if (model.EmailNotificationInfo_PointsOfSale != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_PointsOfSale)
                        {
                            if (queryPos.Where(m => m.pointOfSaleID == i).Count() > 0)
                            {
                                queryPos = queryPos.Where(m => m.pointOfSaleID != i).ToArray();
                            }
                            else
                            {
                                var pos = new tblEmailNotifications_PointsOfSale();
                                pos.pointOfSaleID = i;
                                pos.active = true;
                                query.tblEmailNotifications_PointsOfSale.Add(pos);
                            }
                        }
                        if (queryPos.Count() > 0)
                        {
                            foreach (var i in queryPos)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    #region "lead status
                    if (model.EmailNotificationInfo_LeadStatus != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_LeadStatus)
                        {
                            if (queryLeadStatus.Where(m => m.leadStatusID == i).Count() > 0)
                            {
                                queryLeadStatus = queryLeadStatus.Where(m => m.leadStatusID != i).ToArray();
                            }
                            else
                            {
                                var _leadStatus = new tblEmailNotifications_LeadStatus();
                                _leadStatus.leadStatusID = i;
                                query.tblEmailNotifications_LeadStatus.Add(_leadStatus);
                            }
                        }
                        if (queryLeadStatus.Count() > 0)
                        {
                            foreach (var i in queryLeadStatus)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (queryLeadStatus.Count() > 0)
                        {
                            foreach (var i in queryLeadStatus)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    #region "booking status"
                    if (model.EmailNotificationInfo_BookingStatus != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_BookingStatus)
                        {
                            if (queryBookingStatus.Where(m => m.bookingStatusID == i).Count() > 0)
                            {
                                queryBookingStatus = queryBookingStatus.Where(m => m.bookingStatusID != i).ToArray();
                            }
                            else
                            {
                                var _bookingStatus = new tblEmailNotifications_BookingStatus();
                                _bookingStatus.bookingStatusID = i;
                                query.tblEmailNotifications_BookingStatus.Add(_bookingStatus);
                            }
                        }
                        if (queryBookingStatus.Count() > 0)
                        {
                            foreach (var i in queryBookingStatus)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (queryBookingStatus.Count() > 0)
                        {
                            foreach (var i in queryBookingStatus)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    #region "secondary booking status"
                    if (model.EmailNotificationInfo_PreBookingStatus != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_PreBookingStatus)
                        {
                            if (querySecondaryBookingStatus.Where(m => m.secondaryBookingStatusID == i).Count() > 0)
                            {
                                querySecondaryBookingStatus = querySecondaryBookingStatus.Where(m => m.secondaryBookingStatusID != i).ToArray();
                            }
                            else
                            {
                                var _bookingStatus = new tblEmailNotifications_SecondaryBookingStatus();
                                _bookingStatus.secondaryBookingStatusID = i;
                                query.tblEmailNotifications_SecondaryBookingStatus.Add(_bookingStatus);
                            }
                        }
                        if (querySecondaryBookingStatus.Count() > 0)
                        {
                            foreach (var i in querySecondaryBookingStatus)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (querySecondaryBookingStatus.Count() > 0)
                        {
                            foreach (var i in querySecondaryBookingStatus)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion

                    #region "broker contracts"
                    if (model.EmailNotificationInfo_BrokerContracts != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_BrokerContracts)
                        {
                            if (queryBrokerContracts.Where(m => m.brokerContractID == i).Count() > 0)
                            {
                                queryBrokerContracts = queryBrokerContracts.Where(m => m.brokerContractID != i).ToArray();
                            }
                            else
                            {
                                var _broker = new tblEmailNotifications_BrokerContracts();
                                _broker.brokerContractID = i;
                                query.tblEmailNotifications_BrokerContracts.Add(_broker);
                            }
                        }
                        if (queryBrokerContracts.Count() > 0)
                        {
                            foreach (var i in queryBrokerContracts)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    else
                    {
                        if (queryBrokerContracts.Count() > 0)
                        {
                            foreach (var i in queryBrokerContracts)
                            {
                                db.DeleteObject(i);
                            }
                        }
                    }
                    #endregion
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Email Notification Updated";
                    response.ObjectID = query.emailNotificationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Email Notification NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    tblEmailNotifications query = new tblEmailNotifications();

                    query.emailID = model.EmailNotificationInfo_Email;
                    query.eventID = model.EmailNotificationInfo_Event;
                    query.replyTo = model.EmailNotificationInfo_ReplyTo;
                    query.emailAccounts = model.EmailNotificationInfo_EmailAccounts != null ? model.EmailNotificationInfo_EmailAccounts.Trim() : null;
                    query.terminalID = model.EmailNotificationInfo_Terminal;
                    query.requiredFields = model.EmailNotificationInfo_RequiredFields != null ? string.Join(",", model.EmailNotificationInfo_RequiredFields) : null;
                    query.description = model.EmailNotificationInfo_Description;
                    query.copyLead = model.EmailNotificationInfo_CopyLead;
                    query.copySender = model.EmailNotificationInfo_CopySender;
                    query.active = model.EmailNotificationInfo_IsActive;

                    if (model.EmailNotificationInfo_Destinations != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_Destinations)
                        {
                            var destination = new tblEmailNotifications_Destinations();
                            destination.destinationID = i; ;
                            query.tblEmailNotifications_Destinations.Add(destination);
                        }
                    }
                    if (model.EmailNotificationInfo_Resorts != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_Resorts)
                        {
                            var resort = new tblEmailNotifications_Places();
                            resort.placeID = i;
                            query.tblEmailNotifications_Places.Add(resort);
                        }
                    }
                    if (model.EmailNotificationInfo_LeadSources != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_LeadSources)
                        {
                            var source = new tblEmailNotifications_LeadSources();
                            source.leadSourceID = i;
                            query.tblEmailNotifications_LeadSources.Add(source);
                        }
                    }
                    if (model.EmailNotificationInfo_PointsOfSale != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_PointsOfSale)
                        {
                            var pos = new tblEmailNotifications_PointsOfSale();
                            pos.active = true;
                            pos.pointOfSaleID = i;
                            query.tblEmailNotifications_PointsOfSale.Add(pos);
                        }
                    }
                    if (model.EmailNotificationInfo_LeadStatus != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_LeadStatus)
                        {
                            var status = new tblEmailNotifications_LeadStatus();
                            status.leadStatusID = i;
                            query.tblEmailNotifications_LeadStatus.Add(status);
                        }
                    }
                    if (model.EmailNotificationInfo_BookingStatus != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_BookingStatus)
                        {
                            var status = new tblEmailNotifications_BookingStatus();
                            status.bookingStatusID = i;
                            query.tblEmailNotifications_BookingStatus.Add(status);
                        }
                    }
                    if (model.EmailNotificationInfo_PreBookingStatus != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_PreBookingStatus)
                        {
                            var status = new tblEmailNotifications_SecondaryBookingStatus();
                            status.secondaryBookingStatusID = i;
                            query.tblEmailNotifications_SecondaryBookingStatus.Add(status);
                        }
                    }
                    if (model.EmailNotificationInfo_BrokerContracts != null)
                    {
                        foreach (var i in model.EmailNotificationInfo_BrokerContracts)
                        {
                            var broker = new tblEmailNotifications_BrokerContracts();
                            broker.brokerContractID = i;
                            query.tblEmailNotifications_BrokerContracts.Add(broker);
                        }
                    }
                    db.tblEmailNotifications.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Email Notification Saved";
                    response.ObjectID = query.emailNotificationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Email Notification NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public List<FieldGroupsInfoModel> SearchFieldGroups(FieldGroupsSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<FieldGroupsInfoModel>();
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
            var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
            var users = model.FieldGroupsSearch_SavedByUser != null ? model.FieldGroupsSearch_SavedByUser.ToArray() : new Guid?[] { };
            var usersLength = users.Length;
            var query = from fg in db.tblFieldGroups
                        where (fg.fieldGroup.Contains(model.FieldGroupsSearch_FieldGroup) || model.FieldGroupsSearch_FieldGroup == null)
                        && (usersLength == 0 || users.Contains(fg.savedByUserID))
                        && (terminals.Contains(fg.tblEmailNotifications.terminalID) || isAdmin)
                        select fg;

            foreach (var i in query)
            {
                list.Add(new FieldGroupsInfoModel()
                {
                    FieldGroupsInfo_FieldGroupID = i.fieldGroupID,
                    FieldGroupsInfo_FieldGroup = i.fieldGroup,
                    FieldGroupsInfo_Description = i.description,
                    FieldGroupsInfo_SavedByUser = i.savedByUserID != null ? i.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + i.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName : "System Administrator",
                    FieldGroupsInfo_DateSaved = i.dateSaved != null ? ((DateTime)i.dateSaved).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                    //FieldGroupInfo_Terminal = db.tblTerminals.FirstOrDefault(m => m.terminalID == (i.terminalID ?? i.tblEmailNotifications.terminalID)).terminal
                    FieldGroupInfo_Terminal = i.terminalID != null ? db.tblTerminals.FirstOrDefault(m => m.terminalID == i.terminalID).terminal : i.tblEmailNotifications != null && i.tblEmailNotifications.terminalID != null ? db.tblTerminals.FirstOrDefault(m => m.terminalID == i.tblEmailNotifications.terminalID).terminal : ""
                });
            }

            return list;
        }

        public AttemptResponse SaveFieldGroup(FieldGroupsInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            DateTime _now = DateTime.Now;
            tblFieldGroups query;

            if (model.FieldGroupsInfo_FieldGroupID != null && model.FieldGroupsInfo_FieldGroupID != 0)
            {
                #region "update"
                try
                {
                    query = db.tblFieldGroups.Single(m => m.fieldGroupID == model.FieldGroupsInfo_FieldGroupID);
                    query.fieldGroup = model.FieldGroupsInfo_FieldGroup;
                    query.description = model.FieldGroupsInfo_Description;
                    query.logo = model.FieldGroupsInfo_Logo;
                    query.emailNotificationID = model.FieldGroupsInfo_EmailNotification != 0 ? model.FieldGroupsInfo_EmailNotification : (int?)null;
                    query.terminalID = db.tblEmailNotifications.FirstOrDefault(m => m.emailNotificationID == model.FieldGroupsInfo_EmailNotification).terminalID;
                    query.dateModified = _now;
                    query.modifiedByUserID = session.UserID;
                    db.SaveChanges();

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Fields Group Updated";
                    response.ObjectID = new { fieldGroupID = query.fieldGroupID, savedByUser = session.User, dateSaved = _now.ToString("yyyy-MM-dd hh:mm:ss tt") };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Fields Group NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "create"
                try
                {
                    query = new tblFieldGroups();
                    query.fieldGroup = model.FieldGroupsInfo_FieldGroup;
                    query.fieldGroupGUID = Guid.NewGuid();
                    query.description = model.FieldGroupsInfo_Description;
                    query.logo = model.FieldGroupsInfo_Logo;
                    query.emailNotificationID = model.FieldGroupsInfo_EmailNotification != 0 ? model.FieldGroupsInfo_EmailNotification : (int?)null;
                    query.terminalID = db.tblEmailNotifications.FirstOrDefault(m => m.emailNotificationID == model.FieldGroupsInfo_EmailNotification).terminalID;
                    query.dateSaved = _now;
                    query.savedByUserID = session.UserID;
                    db.tblFieldGroups.AddObject(query);
                    db.SaveChanges();

                    response.Message = "Fields Group Saved";
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = new { fieldGroupID = query.fieldGroupID, savedByUser = session.User, dateSaved = _now.ToString("yyyy-MM-dd hh:mm:ss tt") };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Fields Group NOT Saved";
                    response.Exception = ex;
                    response.ObjectID = 0;
                    return response;
                }
                #endregion
            }
        }

        public FieldGroupsInfoModel GetFieldGroupInfo(int FieldGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            FieldGroupsInfoModel model = new FieldGroupsInfoModel();
            var list = new List<FieldsInfoModel>();

            var query = db.tblFieldGroups.Single(m => m.fieldGroupID == FieldGroupID);

            var _fields = query.tblFields;

            foreach (var i in _fields)
            {
                list.Add(new FieldsInfoModel()
                {
                    FieldsInfo_FieldID = i.fieldID,
                    FieldsInfo_Field = i.field,
                    FieldsInfo_Description = i.description,
                    FieldsInfo_FieldTypeText = i.fieldTypeID != null ? i.tblFieldTypes.fieldType : "",
                    FieldsInfo_FieldSubTypeText = i.fieldSubTypeID != null ? i.tblFieldSubTypes.subType : "",
                    FieldsInfo_ParentFieldText = i.parentFieldID != null ? i.tblFields2.field : "",
                    FieldsInfo_Options = i.options,
                    FieldsInfo_Order = i.order_
                });
            }

            model.FieldGroupsInfo_FieldGroupID = query.fieldGroupID;
            model.FieldGroupsInfo_FieldGroup = query.fieldGroup;
            model.FieldGroupsInfo_FieldGroupGUID = query.fieldGroupGUID;
            model.FieldGroupsInfo_Description = query.description;
            model.FieldGroupsInfo_Logo = query.logo;
            model.FieldGroupsInfo_EmailNotification = query.emailNotificationID ?? 0;
            model.FieldsInGroup = list;
            return model;
        }

        public FieldsInfoModel GetFieldInfo(int FieldID)
        {
            ePlatEntities db = new ePlatEntities();
            FieldsInfoModel model = new FieldsInfoModel();

            var query = db.tblFields.Single(m => m.fieldID == FieldID);

            model.FieldsInfo_FieldID = query.fieldID;
            model.FieldsInfo_FieldGuid = query.fieldGuid;
            model.FieldsInfo_Field = query.field;
            model.FieldsInfo_FieldGroup = query.fieldGroupID;
            model.FieldsInfo_Description = query.description;
            model.FieldsInfo_FieldType = query.fieldTypeID;
            model.FieldsInfo_FieldSubType = query.fieldSubTypeID;
            model.FieldsInfo_ParentField = query.parentFieldID ?? 0;
            model.FieldsInfo_Visibility = query.visibility;
            model.FieldsInfo_VisibleIf = query.visibleIf;
            model.FieldsInfo_VisibleIfGuid = query.visibleIfFieldGuid != null ? query.visibleIfFieldGuid.ToString() : "0";
            model.FieldsInfo_Options = query.options;
            model.FieldsInfo_Order = query.order_;

            return model;
        }

        public AttemptResponse SaveField(FieldsInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.FieldsInfo_FieldID != null && model.FieldsInfo_FieldID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblFields.Single(m => m.fieldID == model.FieldsInfo_FieldID);
                    query.field = model.FieldsInfo_Field;
                    query.description = model.FieldsInfo_Description;
                    query.fieldTypeID = model.FieldsInfo_FieldType;
                    query.fieldSubTypeID = model.FieldsInfo_FieldSubType;
                    query.parentFieldID = model.FieldsInfo_ParentField != 0 ? model.FieldsInfo_ParentField : (long?)null;
                    query.visibility = model.FieldsInfo_Visibility;
                    query.visibleIf = model.FieldsInfo_VisibleIf;
                    query.visibleIfFieldGuid = model.FieldsInfo_VisibleIfGuid != "0" ? Guid.Parse(model.FieldsInfo_VisibleIfGuid) : (Guid?)null;
                    query.options = model.FieldsInfo_Options;
                    query.order_ = model.FieldsInfo_Order;
                    db.SaveChanges();

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Field Updated";
                    response.ObjectID = new { fieldID = query.fieldID };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Field NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "create"
                try
                {
                    tblFields query = new tblFields();
                    query.fieldGuid = Guid.NewGuid();
                    query.field = model.FieldsInfo_Field;
                    query.fieldGroupID = model.FieldsInfo_FieldGroup;
                    query.description = model.FieldsInfo_Description;
                    query.fieldTypeID = model.FieldsInfo_FieldType;
                    query.fieldSubTypeID = model.FieldsInfo_FieldSubType;
                    query.parentFieldID = model.FieldsInfo_ParentField != 0 ? model.FieldsInfo_ParentField : (long?)null;
                    query.visibility = model.FieldsInfo_Visibility;
                    query.visibleIf = model.FieldsInfo_VisibleIf;
                    query.visibleIfFieldGuid = model.FieldsInfo_VisibleIfGuid != "0" ? Guid.Parse(model.FieldsInfo_VisibleIfGuid) : (Guid?)null;
                    query.options = model.FieldsInfo_Options;
                    query.order_ = model.FieldsInfo_Order;
                    db.tblFields.AddObject(query);
                    db.SaveChanges();

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Field Saved";
                    response.ObjectID = new { fieldID = query.fieldID };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Field NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public List<SelectListItem> GetDDLData(string itemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();

            switch (itemType)
            {
                case "activeTerminals":
                    {
                        list = TerminalDataModel.GetActiveTerminalsList();
                        break;
                    }
                case "templatesByCulture":
                    {
                        list = EmailsCatalogs.FillDrpEmailTemplatesByCulture(itemID);
                        break;
                    }
                case "destinationsPerTerminal":
                    {
                        list = PlaceDataModel.GetDestinationsByCurrentTerminals(itemID);
                        break;
                    }
                case "resorts":
                    {
                        var _itemID = itemID != "" ? itemID.Split(',').Select(m => (long?)long.Parse(m)).ToArray() : null;
                        list = PlaceDataModel.GetResortsByDestinations(_itemID, true);
                        break;
                    }
                case "emails":
                    {
                        list = EmailsDataModel.EmailsCatalogs.FillDrpEmailsByTerminals(long.Parse(itemID));
                        break;
                    }
                case "events":
                    {
                        list = EmailsDataModel.EmailsCatalogs.FillDrpSysEventsPerTerminals(long.Parse(itemID));
                        break;
                    }
                case "fieldSubTypes":
                    {
                        list = EmailsDataModel.EmailsCatalogs.FillDrpFieldSubTypes(int.Parse(itemID));
                        break;
                    }
                case "fieldsPerGroup":
                    {
                        list = EmailsDataModel.EmailsCatalogs.FillDrpFieldsPerGroup(int.Parse(itemID));
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "fieldGuidsPerGroup":
                    {
                        list = EmailsDataModel.EmailsCatalogs.FillDrpFieldGuidsPerGroup(int.Parse(itemID));
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return list;
        }

        //public string GetStatusOfRequiredFields(tblEmailNotifications notification, ref object _query)
        public string GetStatusOfRequiredFields(tblEmailNotifications notification, Type type, ref object _query)
        {

            var _fields = GeneralFunctions.RequiredFields.Where(m => notification.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value);
            var _requiredFields = "";
            //var type = _query.GetType();
            var query = Convert.ChangeType(_query, type);
            //var query = _query;
            foreach (var field in _fields)
            {
                switch (field)
                {
                    case "First Name":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("tblLeads");
                            var c = b.GetValue(query, null);
                            if (c.GetType().GetProperty("firstName").GetValue(c, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Last Name":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("tblLeads");
                            var c = b.GetValue(query, null);
                            if (c.GetType().GetProperty("lastName").GetValue(c, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Email":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("tblLeads");
                            var c = b.GetValue(query, null);
                            if (c.GetType().GetProperty("tblLeadEmails").GetValue(c, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Hotel Confirmation Number":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("hotelConfirmationNumber");
                            if (b.GetValue(query, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Arrival Date":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("arrivalDate");
                            if (b.GetValue(query, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Destination":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("destinationID");
                            if (b.GetValue(query, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Resort":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("placeID");
                            if (b.GetValue(query, null) == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Presentation Date":
                        {

                            var a = query.GetType();
                            var b = a.GetProperty("tblPresentations");
                            IEnumerable c = (IEnumerable)b.GetValue(query, null);
                            if (c != null)
                            {
                                var d = c.Cast<object>().FirstOrDefault();

                                if (d.GetType().GetProperty("datePresentation").GetValue(d, null) == null)
                                {
                                    _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                                }
                            }
                            else
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Presentation Time":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("tblPresentations");
                            IEnumerable c = (IEnumerable)b.GetValue(query, null);
                            if (c != null)
                            {
                                var d = c.Cast<object>().FirstOrDefault();

                                if (d.GetType().GetProperty("timePresentation").GetValue(d, null) == null)
                                {
                                    _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                                }
                            }
                            else
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                    case "Optionals":
                        {
                            var a = query.GetType();
                            var b = a.GetProperty("tblOptionsSold");
                            IEnumerable c = (IEnumerable)b.GetValue(query, null);
                            if (c == null)
                            {
                                _requiredFields += (_requiredFields != "" ? ", " : "") + field;
                            }
                            break;
                        }
                }
            }

            return _requiredFields;
        }

        public AttemptResponse EmailSenderByRule(string date = null)
        {
            AttemptResponse response = new AttemptResponse();
            ecommerceEntities ecommerce = new ecommerceEntities();
            ePlatEntities eplat = new ePlatEntities();
            List<KeyValuePair<SmtpClient, MailMessage>> listEmails = new List<KeyValuePair<SmtpClient, MailMessage>>();
            List<KeyValuePair<KeyValuePair<SmtpClient, MailMessage>, List<tblFieldsValues>>> listEmailFieldsValues = new List<KeyValuePair<KeyValuePair<SmtpClient, MailMessage>, List<tblFieldsValues>>>();
            List<Guid> signs = new List<Guid>();

            var today = date != null ? DateTime.Parse(date) : DateTime.Today;
            var todayEnd = today.AddDays(1).AddSeconds(-1);

            var smtpClient = new SmtpClient("smtp.villagroup.com") { Port = 25, EnableSsl = false, Timeout = 100000, DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false };

            try
            {
                //foreach (var rule in ecommerce.tbaEnviosAutomaticos.Where(m => m.active))
                foreach (var rule in ecommerce.tbaEnviosAutomaticos.Where(m => m.idEnvioAutomatico == 32))
                {
                    var ruleType = rule.tipoRegla;
                    var ruleFilters = rule.terminales.Split(',').ToArray();
                    var ruleTerminals = ruleFilters.Select(m => long.Parse(m.Split('|')[0])).Distinct().ToArray();
                    var ruleDestinations = rule.terminales.IndexOf("|") != -1 ? ruleFilters.Select(m => (long?)long.Parse(m.Split('|')[1].Split('_')[0])).Distinct().ToArray() : new long?[] { };
                    var ruleResorts = rule.terminales.IndexOf("_") != -1 ? ruleFilters.Select(m => (m.Split('|')[1].Split('_')[1] == "null" ? 0 : (long?)long.Parse(m.Split('|')[1].Split('_')[1]))).Distinct().ToArray() : new long?[] { };//pendiente agregar exclusión para nulos
                    ruleResorts = ruleResorts.Count(m => m == 0) == ruleResorts.Count() ? new long?[] { } : ruleResorts;

                    var iDate = rule.diasDespues > 0 ? today.AddDays(-rule.diasDespues) : today.AddDays((-1 * rule.diasDespues));
                    var fDate = iDate.AddDays(1).AddSeconds(-1);

                    var groupFields = rule.fieldGroupID != null ? eplat.tblFields.Where(m => m.fieldGroupID == rule.fieldGroupID && (m.fieldTypeID == 2 || m.fieldTypeID == 3) && m.field.ToLower() != "open" && m.field.ToLower() != "submitted") : null;
                    var fieldGroupGUID = rule.fieldGroupID != null ? eplat.tblFieldGroups.Single(m => m.fieldGroupID == rule.fieldGroupID).fieldGroupGUID.ToString() : "";

                    switch (ruleType)
                    {
                        case "reservation":
                            {
                                #region
                                var excludedReservationStatus = new int?[] { 2, 5, 8 };
                                var query = from rsv in ecommerce.tbaReservaciones
                                            join lead in ecommerce.tbaProspectos on rsv.idProspecto equals lead.idProspecto
                                            where !excludedReservationStatus.Contains(rsv.idEstatusReservacion)
                                            && ruleTerminals.Contains(lead.idTerminal)
                                            && ruleDestinations.Contains(rsv.idDestino)
                                            && (ruleResorts.Contains(rsv.idLugar) || ruleResorts.Count() == 0)
                                            && (rule.llegada == false || iDate <= rsv.tbaRegistrosPresentacion.FirstOrDefault().fechaHora && rsv.tbaRegistrosPresentacion.FirstOrDefault().fechaHora <= fDate)
                                            && (rule.llegada == true || iDate <= rsv.salida && rsv.salida <= fDate)
                                            orderby rsv.tbaProspectos.email
                                            select rsv;

                                foreach (var rsv in query)
                                {
                                    var reservationID = rsv.idReservacion.ToString();
                                    var lead = rsv.tbaProspectos;
                                    try
                                    {
                                        if (eplat.tblFieldsValues.Where(m => m.value == reservationID && today <= m.dateSaved && m.dateSaved <= todayEnd).Count() == 0)
                                        {
                                            List<tblFieldsValues> listFieldsValues = new List<tblFieldsValues>();
                                            var now = DateTime.Now;
                                            var transactionID = Guid.NewGuid();
                                            var flightInfo = rsv.tbaDatosProspectos.FirstOrDefault(m => m.idReservacion == rsv.idReservacion);
                                            var body = rule.cuerpoCorreo;

                                            if (groupFields != null)
                                            {
                                                foreach (var field in groupFields)
                                                {
                                                    tblFieldsValues fv = new tblFieldsValues();
                                                    fv.fieldID = field.fieldID;
                                                    fv.terminalID = EmailsCatalogs.GetTerminalIDFromEplat(rsv.tbaProspectos.idTerminal);
                                                    fv.dateSaved = now;
                                                    fv.transactionID = transactionID;

                                                    #region "switch to assign value to field"
                                                    switch (field.field)
                                                    {
                                                        case "$GuestName":
                                                            {
                                                                fv.value = lead.firstName + " " + lead.lastName;
                                                                break;
                                                            }
                                                        case "$Stay":
                                                            {
                                                                fv.value = rsv.llegada.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " / " + rsv.salida.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                                                                break;
                                                            }
                                                        case "$City":
                                                            {
                                                                fv.value = lead.city;
                                                                break;
                                                            }
                                                        case "$Terminal":
                                                            {
                                                                fv.value = lead.tbaTerminales.terminal;
                                                                break;
                                                            }
                                                        case "$ReservationID":
                                                            {
                                                                fv.value = reservationID;
                                                                break;
                                                            }
                                                        case "$DestinationID":
                                                            {
                                                                fv.value = rsv.idDestino.ToString();
                                                                break;
                                                            }
                                                        case "$Destination":
                                                            {
                                                                fv.value = rsv.tbaDestinos.destino;
                                                                break;
                                                            }
                                                        case "$Resort":
                                                            {
                                                                fv.value = rsv.tbaLugares.lugar;
                                                                break;
                                                            }
                                                        case "$CheckIn":
                                                            {
                                                                fv.value = rsv.llegada.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                                                                break;
                                                            }
                                                        case "$CheckOut":
                                                            {
                                                                fv.value = rsv.salida.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                                                                break;
                                                            }
                                                        case "$LeadID":
                                                            {
                                                                fv.value = lead.idProspecto.ToString();
                                                                break;
                                                            }
                                                        case "$MarketingSourceID":
                                                            {
                                                                fv.value = lead.idMarketingSource.ToString();
                                                                break;
                                                            }
                                                        case "$DB":
                                                            {
                                                                fv.value = "ecommerce";
                                                                break;
                                                            }
                                                        case "$Sent":
                                                            {
                                                                fv.value = now.ToString("yyyy-MM-dd HH:mm:ss-fff", CultureInfo.InvariantCulture);
                                                                break;
                                                            }
                                                        case "$MarketingSource":
                                                            {
                                                                fv.value = lead.tbaMarketingSources.marketingSource;
                                                                break;
                                                            }
                                                        case "$VueloLlegada":
                                                            {
                                                                fv.value = flightInfo.vueloLlegada.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                                                                break;
                                                            }
                                                        case "$Travelers":
                                                            {
                                                                fv.value = lead.firstName + " " + lead.lastName + " / " + lead.spouseFirstName + " " + lead.spouseLastName;
                                                                break;
                                                            }
                                                        case "$ResortConfirmation":
                                                            {
                                                                fv.value = rsv.certificateNumber;
                                                                break;
                                                            }
                                                        case "$StayingAt":
                                                            {
                                                                fv.value = rsv.tbaLugares.lugar;
                                                                break;
                                                            }
                                                        case "$Airline":
                                                            {
                                                                fv.value = flightInfo.vueloAerolinea;
                                                                break;
                                                            }
                                                        case "$FlightNumber":
                                                            {
                                                                fv.value = flightInfo.vueloNumero;
                                                                break;
                                                            }
                                                        case "$MeetingTime":
                                                            {
                                                                fv.value = flightInfo.vueloHoraLlegada;
                                                                break;
                                                            }
                                                        case "$PartyOf":
                                                            {
                                                                fv.value = flightInfo.vueloNPersonas;
                                                                break;
                                                            }
                                                    }
                                                    #endregion

                                                    listFieldsValues.Add(fv);
                                                    body = body.Replace(field.field, fv.value);
                                                }
                                            }
                                            body = body.Replace("$LastName", lead.lastName)
                                                .Replace("$FieldGroupGUID", fieldGroupGUID)
                                                .Replace("$TransactionID", transactionID.ToString());

                                            var email = new MailMessage();
                                            email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoRemitente, rule.aliasRemitente) : new MailAddress(rule.correoRemitente));
                                            email.To.Add(lead.email);
                                            email.Subject = rule.asuntoCorreo;
                                            email.IsBodyHtml = true;
                                            email.Body = body;
                                            email.Bcc.Add(rule.cco.IndexOf(",") != -1 ? rule.cco : rule.cco.IndexOf(";") != -1 ? string.Join(",", rule.cco.Split(';').ToArray()) : rule.cco != "" ? rule.cco : "efalcon@villagroup.com");
                                            email.ReplyToList.Add(rule.correoResponderA ?? "");//for many addresses codify as previous line

                                            smtpClient.Credentials = new NetworkCredential(rule.correoRemitente, rule.passwordRemitente);
                                            listEmailFieldsValues.Add(new KeyValuePair<KeyValuePair<SmtpClient, MailMessage>, List<tblFieldsValues>>(
                                                new KeyValuePair<SmtpClient, MailMessage>(smtpClient, email),
                                                listFieldsValues
                                                ));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var body = "<strong>Exception ocurred while trying to send an email for: " + rule.asuntoCorreo + "</strong><br /><br />";
                                        body += "<br />Reservation ID: " + reservationID;
                                        body += "<br />Sender Address: " + rule.correoRemitente;
                                        body += "<br />Target Address(es): " + lead.email;
                                        body += "<br />Exception Message: " + ex.Message ?? "";
                                        body += "<br />Inner Exception Message: " + ex.InnerException ?? "";
                                        body += "<br /><br />";

                                        var email = EmailNotifications.GetSystemEmail(body);
                                        //EmailNotifications.Send(email, false);
                                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                                    }
                                }
                                #endregion
                                break;
                            }
                        case "service":
                            {
                                #region
                                var query = from purchase in eplat.tblPurchases
                                            join ps in eplat.tblPurchases_Services on purchase.purchaseID equals ps.purchaseID
                                            where ruleTerminals.Contains(purchase.terminalID)
                                            && purchase.culture == rule.cultura
                                            && purchase.tblPointsOfSale.online
                                            && ps.serviceStatusID == 3
                                            && ps.serviceDateTime >= iDate && ps.serviceDateTime <= fDate
                                            select purchase;

                                foreach (var p in query)
                                {
                                    var purchaseID = p.purchaseID.ToString();

                                    try
                                    {
                                        var mostRecentCoupon = p.tblPurchases_Services.OrderByDescending(m => m.serviceDateTime).FirstOrDefault().serviceDateTime;
                                        if (iDate <= mostRecentCoupon && mostRecentCoupon <= fDate)
                                        {
                                            var services = p.tblPurchases_Services.Where(m => m.serviceStatusID == 3).Select(m => new { service = m.tblServices.tblServiceDescriptions.Count() > 0 ? m.tblServices.tblServiceDescriptions.FirstOrDefault(x => x.culture == p.culture).service : m.tblServices.service, m.tblServices.tblProviders.comercialName, m.serviceDateTime });
                                            var str = "";

                                            foreach (var i in services)
                                            {
                                                str += (str == "" ? "" : "<br />")
                                                    + "<p><strong>" + i.service + "</strong>"
                                                    + "<br />" + (p.culture == "es-MX" ? "por " : "by ") + i.comercialName
                                                    + "<br />" + (p.culture == "es-MX" ? "visitado el " : "visited on ") + i.serviceDateTime.ToString("D", CultureInfo.GetCultureInfo(p.culture)) + "</p>";
                                            }

                                            if (eplat.tblFieldsValues.Where(m => m.value == purchaseID && today <= m.dateSaved && m.dateSaved <= todayEnd).Count() == 0)
                                            {
                                                List<tblFieldsValues> listFieldsValues = new List<tblFieldsValues>();
                                                var now = DateTime.Now;
                                                var transactionID = Guid.NewGuid();
                                                var domain = EmailsCatalogs.GetURLDomain(p.purchaseID);
                                                var body = rule.cuerpoCorreo;
                                                var firstName = p.tblLeads.firstName;
                                                var lastName = p.tblLeads.lastName;

                                                if (groupFields != null)
                                                {
                                                    foreach (var field in groupFields)
                                                    {
                                                        #region "tblFieldsValues assignation"
                                                        tblFieldsValues fv = new tblFieldsValues();
                                                        fv.fieldID = field.fieldID;
                                                        fv.terminalID = p.terminalID;
                                                        fv.dateSaved = now;
                                                        fv.transactionID = transactionID;

                                                        switch (field.field)
                                                        {
                                                            case "$PurchaseID":
                                                                {
                                                                    fv.value = purchaseID;
                                                                    break;
                                                                }
                                                            case "$FirstName":
                                                                {
                                                                    fv.value = firstName;
                                                                    break;
                                                                }
                                                            case "$LastName":
                                                                {
                                                                    fv.value = lastName;
                                                                    break;
                                                                }
                                                            case "$DB":
                                                                {
                                                                    fv.value = "ePlat";
                                                                    break;
                                                                }
                                                            case "$Sent":
                                                                {
                                                                    fv.value = now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                                                    break;
                                                                }
                                                            case "$Terminal":
                                                                {
                                                                    fv.value = p.tblTerminals.terminal;
                                                                    break;
                                                                }
                                                        }
                                                        listFieldsValues.Add(fv);
                                                        body = body.Replace(field.field, fv.value);
                                                        #endregion
                                                    }
                                                }
                                                body = body.Replace("$FieldGroupGUID", fieldGroupGUID)
                                                    .Replace("$TransactionID", transactionID.ToString())
                                                    .Replace("$Domain", domain)
                                                    .Replace("$BodyMail", str);

                                                var email = new MailMessage();
                                                email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoResponderA, rule.aliasRemitente) : new MailAddress(rule.correoResponderA));
                                                email.To.Add(p.tblLeads.tblLeadEmails.FirstOrDefault(m => m.main).email);
                                                email.Subject = rule.asuntoCorreo.Replace("$FirstName", firstName).Replace("$Domain", domain.Replace("https://", ""));
                                                email.IsBodyHtml = true;
                                                email.Body = body;
                                                email.Bcc.Add(rule.cco.IndexOf(",") != -1 ? rule.cco : rule.cco.IndexOf(";") != -1 ? string.Join(",", rule.cco.Split(';').ToArray()) : rule.cco != "" ? rule.cco : "efalcon@villagroup.com");
                                                email.ReplyToList.Add(rule.correoResponderA ?? "");//for many addresses codify as previous line

                                                smtpClient.Credentials = new NetworkCredential(rule.correoRemitente, rule.passwordRemitente);
                                                listEmailFieldsValues.Add(new KeyValuePair<KeyValuePair<SmtpClient, MailMessage>, List<tblFieldsValues>>(
                                                new KeyValuePair<SmtpClient, MailMessage>(smtpClient, email),
                                                listFieldsValues
                                                ));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        var body = "<strong>Exception ocurred while trying to send an email for: " + rule.asuntoCorreo + "</strong><br /><br />";
                                        body += "<br />Purchase ID: " + purchaseID;
                                        body += "<br />Sender Address: " + rule.correoRemitente;
                                        body += "<br />Target Address(es): " + string.Join(",", p.tblLeads.tblLeadEmails.OrderByDescending(m => m.main).Select(m => m.email).ToArray());
                                        body += "<br />Exception Message: " + ex.Message ?? "";
                                        body += "<br />Inner Exception Message: " + ex.InnerException ?? "";
                                        body += "<br /><br />";

                                        var email = EmailNotifications.GetSystemEmail(body);
                                        //EmailNotifications.Send(email, false);
                                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                                    }
                                }
                                #endregion
                                break;
                            }
                        case "hotelConfReminder":
                            {
                                #region
                                var excludedReservationStatus = new int?[] { 2, 4, 8, 15, 16, 21, 23 };

                                var query = from rsv in ecommerce.tbaReservaciones
                                            join lead in ecommerce.tbaProspectos on rsv.idProspecto equals lead.idProspecto
                                            where ruleTerminals.Contains(lead.idTerminal)
                                            && rsv.cartaConfirmacionEnviadaPor != null && rsv.fechaEnvioCartaConfirmacion != null
                                            && !excludedReservationStatus.Contains(rsv.idEstatusReservacion)
                                            && rsv.llegada > todayEnd
                                            && (ruleDestinations.Contains(rsv.idDestino) || ruleDestinations.Count() == 0)
                                            && (ruleResorts.Contains(rsv.idLugar) || ruleResorts.Count() == 0)
                                            && rsv.tbaFirmas.Count(m => m.idTipodeDocumento == 1 && m.urlDocumento.Contains("hotelAndPresentationConfirmation") && (m.fechaUltimoEnvio == iDate || (m.fechaUltimoEnvio == null && m.fechaCreacion <= iDate))) > 0
                                            select rsv;

                                foreach (var rsv in query)
                                {
                                    var signature = rsv.tbaFirmas.Where(m => m.idTipodeDocumento == 1 && m.urlDocumento.Contains("hotelAndPresentationConfirmation") && (m.fechaUltimoEnvio == iDate || (m.fechaUltimoEnvio == null && m.fechaCreacion <= iDate))).OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                    //var signature = rsv.tbaFirmas.Where(m => m.idTipodeDocumento == 1 && m.fechaFirma == null && m.firma == null && m.urlDocumento.Contains("hotelAndPresentationConfirmation") && (m.fechaUltimoEnvio == iDate || (m.fechaUltimoEnvio == null && m.fechaCreacion <= iDate))).OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                    var signID = signature.firma != null ? (Guid?)null : signature.idFirma;
                                    var prospecto = rsv.tbaProspectos;
                                    try
                                    {
                                        if (signID != null && !signs.Contains((Guid)signID))
                                        {
                                            signs.Add((Guid)signID);
                                            var body = rule.cuerpoCorreo;
                                            body = body.Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;firstName&quot; labeled=&quot;false&quot; /]", prospecto.firstName)
                                                .Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;lastName&quot; labeled=&quot;false&quot; /]", prospecto.lastName)
                                                .Replace("[vg:var table=&quot;tbaLugares&quot; field=&quot;lugar&quot; labeled=&quot;false&quot; /]", rsv.tbaLugares.lugar)
                                                .Replace("here [link]", "<a href=\"http://eplatform.villagroup.com/signit/?signature=" + signID + "\">here</a>");

                                            var email = new MailMessage();
                                            email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoResponderA, rule.aliasRemitente) : new MailAddress(rule.correoResponderA));
                                            email.To.Add(prospecto.email);
                                            email.Subject = rule.asuntoCorreo;
                                            email.IsBodyHtml = true;
                                            email.Body = body;
                                            email.Bcc.Add(rule.cco.IndexOf(",") != -1 ? rule.cco : rule.cco.IndexOf(";") != -1 ? string.Join(",", rule.cco.Split(';').ToArray()) : rule.cco != "" ? rule.cco : "efalcon@villagroup.com");
                                            email.ReplyToList.Add(rule.correoResponderA ?? "");//for many addresses codify as previous line

                                            smtpClient.Credentials = new NetworkCredential(rule.correoRemitente, rule.passwordRemitente);
                                            listEmails.Add(new KeyValuePair<SmtpClient, MailMessage>(smtpClient, email));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        signs = signs.Where(m => m != signID).ToList<Guid>();
                                        var body = "<strong>Exception ocurred while trying to send an email for: " + rule.asuntoCorreo + "</strong><br /><br />";
                                        body += "<br />Signature ID: " + signID;
                                        body += "<br />Reservation ID: " + rsv.idReservacion;
                                        body += "<br />Sender Address: " + rule.correoRemitente;
                                        body += "<br />Target Address: " + prospecto.email;
                                        body += "<br />Exception Message: " + ex.Message ?? "";
                                        body += "<br />Inner Exception Message: " + ex.InnerException ?? "";
                                        body += "<br /><br />";

                                        var email = EmailNotifications.GetSystemEmail(body);
                                        //EmailNotifications.Send(email, false);
                                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                                    }
                                }


                                //foreach (var rsv in query)
                                //{
                                //    var signature = rsv.tbaFirmas.Where(m => m.idReservacion == rsv.idReservacion && m.idTipodeDocumento == 1 && m.urlDocumento.Contains("hotelAndPresentationConfirmation")).OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                //    var signID = signature.firma != null ? (Guid?)null : signature.idFirma;

                                //    try
                                //    {
                                //        if (signID != null && !signs.Contains((Guid)signID))
                                //        {
                                //            signs.Add((Guid)signID);
                                //            var body = rule.cuerpoCorreo;
                                //            body = body.Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;firstName&quot; labeled=&quot;false&quot; /]", rsv.tbaProspectos.firstName)
                                //                .Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;lastName&quot; labeled=&quot;false&quot; /]", rsv.tbaProspectos.lastName)
                                //                .Replace("[vg:var table=&quot;tbaLugares&quot; field=&quot;lugar&quot; labeled=&quot;false&quot; /]", rsv.tbaLugares.lugar)
                                //                .Replace("here [link]", "<a href=\"http://eplatform.villagroup.com/signit/?signature=" + signID + "\">here</a>");

                                //            var email = new MailMessage();
                                //            email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoResponderA, rule.aliasRemitente) : new MailAddress(rule.correoResponderA));
                                //            email.To.Add(rsv.tbaProspectos.email);
                                //            email.Subject = rule.asuntoCorreo;
                                //            email.IsBodyHtml = true;
                                //            email.Body = body;
                                //            email.Bcc.Add(rule.cco.IndexOf(",") != -1 ? rule.cco : rule.cco.IndexOf(";") != -1 ? string.Join(",", rule.cco.Split(';').ToArray()) : rule.cco != "" ? rule.cco : "efalcon@villagroup.com");
                                //            email.ReplyToList.Add(rule.correoResponderA ?? "");//for many addresses codify as previous line

                                //            smtpClient.Credentials = new NetworkCredential(rule.correoRemitente, rule.passwordRemitente);
                                //            listEmails.Add(new KeyValuePair<SmtpClient, MailMessage>(smtpClient, email));
                                //        }
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        signs = signs.Where(m => m != signID).ToList<Guid>();
                                //        var body = "<strong>Exception ocurred while trying to send an email for: " + rule.asuntoCorreo + "</strong><br /><br />";
                                //        body += "<br />Signature ID: " + signID;
                                //        body += "<br />Reservation ID: " + rsv.idReservacion;
                                //        body += "<br />Sender Address: " + rule.correoRemitente;
                                //        body += "<br />Target Address: " + rsv.tbaProspectos.email;
                                //        body += "<br />Exception Message: " + ex.Message ?? "";
                                //        body += "<br />Inner Exception Message: " + ex.InnerException ?? "";
                                //        body += "<br /><br />";

                                //        var email = EmailNotifications.GetSystemEmail(body);
                                //        EmailNotifications.Send(email, false);
                                //    }
                                //}
                                #endregion
                                break;
                            }
                        case "purchaseConfReminder":
                            {
                                #region
                                var excludedReservationStatus = new int?[] { 2, 4, 8, 15, 16, 21, 23 };
                                var query = from rsv in ecommerce.tbaReservaciones
                                            join lead in ecommerce.tbaProspectos on rsv.idProspecto equals lead.idProspecto
                                            join sign in ecommerce.tbaFirmas on rsv.idReservacion equals sign.idReservacion
                                            where !excludedReservationStatus.Contains(rsv.idEstatusReservacion)
                                            && ruleTerminals.Contains(lead.idTerminal)
                                            && (ruleDestinations.Contains(rsv.idDestino) || ruleDestinations.Count() == 0)
                                            && (ruleResorts.Contains(rsv.idLugar) || ruleResorts.Count() == 0)
                                            && rsv.llegada > todayEnd
                                            && (sign.idTipodeDocumento == 4 && sign.fechaFirma == null && sign.firma == null && ((sign.fechaUltimoEnvio == null && sign.fechaCreacion <= iDate) || sign.fechaUltimoEnvio == iDate) && sign.urlDocumento.Contains("purchaseConfirmation"))
                                            select rsv;

                                foreach (var rsv in query)
                                {
                                    //var signature = rsv.tbaFirmas.OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                    var signature = rsv.tbaFirmas.Where(m => m.idReservacion == rsv.idReservacion && m.idTipodeDocumento == 4 && m.urlDocumento.Contains("purchaseConfirmation")).OrderByDescending(m => m.fechaCreacion).FirstOrDefault();
                                    var signID = signature != null ? (Guid?)null : signature.idFirma;

                                    try
                                    {
                                        if (signID != null && !signs.Contains((Guid)signID))
                                        {
                                            signs.Add((Guid)signID);

                                            var body = rule.cuerpoCorreo;
                                            body = body.Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;firstName&quot; labeled=&quot;false&quot; /]", rsv.tbaProspectos.firstName)
                                                .Replace("[vg:var table=&quot;tbaProspectos&quot; field=&quot;lastName&quot; labeled=&quot;false&quot; /]", rsv.tbaProspectos.lastName)
                                                .Replace("here [link]", "<a href=\"http://eplatform.villagroup.com/signit/?signature=" + signID + "\">here</a>");

                                            var email = new MailMessage();
                                            email.From = (rule.aliasRemitente != null ? new MailAddress(rule.correoRemitente, rule.aliasRemitente) : new MailAddress(rule.correoResponderA));
                                            email.To.Add(rsv.tbaProspectos.email);
                                            email.Subject = rule.asuntoCorreo;
                                            email.IsBodyHtml = true;
                                            email.Body = body;
                                            email.Bcc.Add(rule.cco.IndexOf(",") != -1 ? rule.cco : rule.cco.IndexOf(";") != -1 ? string.Join(",", rule.cco.Split(';').ToArray()) : rule.cco != "" ? rule.cco : "efalcon@villagroup.com");
                                            email.ReplyToList.Add(rule.correoResponderA ?? "");//for many addresses codify as previous line

                                            smtpClient.Credentials = new NetworkCredential(rule.correoRemitente, rule.passwordRemitente);
                                            listEmails.Add(new KeyValuePair<SmtpClient, MailMessage>(smtpClient, email));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        signs = signs.Where(m => m != signID).ToList<Guid>();
                                        var body = "<strong>Exception ocurred while trying to send an email for: " + rule.asuntoCorreo + "</strong><br /><br />";
                                        body += "<br />Signature ID: " + signID;
                                        body += "<br />Reservation ID: " + rsv.idReservacion;
                                        body += "<br />Sender Address: " + rule.correoRemitente;
                                        body += "<br />Target Address: " + rsv.tbaProspectos.email;
                                        body += "<br />Exception Message: " + ex.Message ?? "";
                                        body += "<br />Inner Exception Message: " + ex.InnerException ?? "";
                                        body += "<br /><br />";

                                        var email = EmailNotifications.GetSystemEmail(body);
                                        //EmailNotifications.Send(email, false);
                                        EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                                    }
                                }
                                #endregion
                                break;
                            }
                    }

                }

                #region "check and advise for repeated receivers --comented"
                //var duplicatedReminders = listEmails.GroupBy(m => m.Value.To).Select(m => new { group = m, count = m.Count() });
                //var duplicated = listEmailFieldsValues.GroupBy(m => m.Key.Value.To).Select(m => new { group = m.Key, count = m.Count() });

                //foreach (var i in duplicatedReminders.Where(m => m.count > 1))
                //{
                //    var body = "<strong>There was found more than one send attempt on the rule: " + i.group.FirstOrDefault().Value.Subject + "</strong><br /><br />";
                //    body += "<br /><br />";

                //    var email = EmailNotifications.GetSystemEmail(body);
                //    EmailNotifications.Send(email, false);
                //}
                #endregion

                foreach (var sign in signs)
                {
                    ecommerce.tbaFirmas.Single(m => m.idFirma == sign).fechaUltimoEnvio = today;
                }

                foreach (var instance in listEmailFieldsValues)
                {
                    //instance.Key.Key.Send(instance.Key.Value);
                    foreach (var fv in instance.Value)
                    {
                        eplat.tblFieldsValues.AddObject(fv);
                    }
                }

                foreach (var reminder in listEmails)
                {
                    //reminder.Key.Send(reminder.Value);
                }

                //ecommerce.SaveChanges();
                //eplat.SaveChanges();

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Rules Successfully Proccesed";
                response.ObjectID = date;
                return response;
            }
            catch (Exception ex)
            {
                var body = "<strong>Exception ocurred on the main block of " + ex.TargetSite + "</strong><br /><br />";
                body += "<br />Please see the exception details in order to fix the issue.<br />";
                body += "<br />Exception Message: " + ex.Message ?? "";
                body += "<br />Inner Exception Message: " + ex.InnerException ?? "";
                body += "<br />Stack Trace: " + ex.StackTrace ?? "";
                body += "<br />Additional data: " + ex.Data ?? "";
                body += "<br /><br />";

                var email = EmailNotifications.GetSystemEmail(body);
                //EmailNotifications.Send(email, false);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Rules NOT Successfully Proccesed";
                response.ObjectID = date;
                response.Exception = ex;
                return response;
            }
        }


    }
}
