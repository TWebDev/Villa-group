using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Models.DataModels
{
    public class FollowUpDataModel
    {
        public static List<FollowUpViewModel.LogItem> GetLogs(DateTime date, Guid[] userID)
        {
            List<FollowUpViewModel.LogItem> Logs = new List<FollowUpViewModel.LogItem>();

            //llamadas
            Logs = NetCenterDataModel.CallsLog.GetLogs(date, userID);

            //correos
            Logs = Logs.Concat(MailingDataModel.GetLogs(date, userID)).ToList();

            //sms

            //conversaciones de chat

            return Logs;
        }

        public static AttemptResponse PhoneAnalysis(string phone)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalid = terminals[0];

            FollowUpViewModel.PhoneAnalysis phoneAnalysis = new FollowUpViewModel.PhoneAnalysis();

            var LeadID = (from p in db.tblPhones
                          join t in db.tblLeads on p.leadID equals t.leadID
                          into p_t
                          from t in p_t.DefaultIfEmpty()
                          orderby t.inputDateTime descending
                          where p.phone == phone
                          && t.terminalID == terminalid
                          select p.leadID).FirstOrDefault();

            if (LeadID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                phoneAnalysis.LeadID = LeadID;
            }
            else
            {
                phoneAnalysis.LeadID = null;
            }

            //ubicacion del teléfono
            string areaCode = "000";
            if (phone.Length >= 10)
            {
                areaCode = phone.Substring(0, 3);
                var Area = (from a in db.tblAreaCodes
                           join ac in db.tblAreas on a.areaID equals ac.areaID
                           into a_ac
                           from ac in a_ac.DefaultIfEmpty()
                           join s in db.tblStates on ac.stateID equals s.stateID
                           into a_s
                           from s in a_s.DefaultIfEmpty()
                           where a.code == areaCode
                           select new
                           {
                               ac.area,
                               ac.stateID,
                               s.state,
                               s.countryID
                           }).FirstOrDefault();

                if(Area != null)
                {
                    phoneAnalysis.CountryID = Area.countryID;
                    phoneAnalysis.StateID = Area.stateID;
                    phoneAnalysis.State = Area.state;
                    phoneAnalysis.City = Area.area;
                }
            }

            response.Type = Attempt_ResponseTypes.Ok;
            response.ObjectID = phoneAnalysis;

            return response;
        }

        public static AttemptResponse SaveLead(string lead)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalid = terminals[0];

            FollowUpViewModel.LeadInfo Lead = js.Deserialize<FollowUpViewModel.LeadInfo>(lead);

            tblLeads objLead = new tblLeads();

            if (Lead.LeadID != null)
            {
                //actualizar
                objLead = (from l in db.tblLeads
                           where l.leadID == Lead.LeadID
                           select l).FirstOrDefault();
            }
            else
            {
                //nuevo
                objLead.leadID = Guid.NewGuid();
                objLead.initialTerminalID = terminalid;
                objLead.terminalID = terminalid;
                objLead.inputByUserID = session.UserID;
                objLead.inputDateTime = DateTime.Now;
                Lead.CustomerInfo.DateSaved = objLead.inputDateTime;
                objLead.inputMethodID = Lead.CustomerInfo.InputMethodID;
            }

            objLead.firstName = Lead.CustomerInfo.FirstName;
            objLead.lastName = Lead.CustomerInfo.LastName;
            objLead.city = Lead.CustomerInfo.City;
            objLead.stateID = Lead.CustomerInfo.StateID;
            objLead.state = Lead.CustomerInfo.State;
            objLead.countryID = Lead.CustomerInfo.CountryID;
            objLead.leadTypeID = Lead.CustomerInfo.LeadTypeID;
            objLead.bookingStatusID = Lead.CustomerInfo.BookingStatusID;
            //objLead.qualificationStatusID
            objLead.leadSourceID = Lead.CustomerInfo.LeadSourceID;
            objLead.leadSourceChannelID = Lead.CustomerInfo.LeadSourceChannelID;
            objLead.assignedToUserID = Lead.CustomerInfo.AssignedToUserID;
            if (Lead.CustomerInfo.AssignedToUserID != null)
            {
                objLead.assignationDate = DateTime.Now;
            }
            objLead.leadStatusID = Lead.CustomerInfo.LeadStatusID;
            if (Lead.LeadID == null)
            {
                db.tblLeads.AddObject(objLead);
                db.SaveChanges();
            }

            //revisar teléfonos y correos para guardar
            if (Lead.LeadID != null)
            {
                //si ya no es nuevo, buscar teléfonos y correos
                //los que ya no vienen del cliente
                var clientPhoneIDs = Lead.ContactInfo.Phones
                    .Where(x => x.PhoneID != null)
                    .Select(x => x.PhoneID).ToList();

                db.tblPhones
                        .Where(x => !clientPhoneIDs.Contains(x.phoneID))
                        .ToList()
                        .ForEach(db.tblPhones.DeleteObject);

                //los que vienen del cliente
                foreach (var phone in Lead.ContactInfo.Phones)
                {
                    if (phone.PhoneID == null)
                    {
                        //nuevo
                        tblPhones newPhone = new tblPhones();
                        newPhone.phone = phone.Phone;
                        newPhone.phoneTypeID = phone.PhoneTypeID;
                        newPhone.doNotCall = phone.DoNotCall;
                        newPhone.leadID = objLead.leadID;
                        newPhone.ext = phone.Ext;
                        newPhone.main = phone.Main;
                        db.tblPhones.AddObject(newPhone);
                    }
                    else if (phone.PhoneID != null)
                    {
                        //actualizar
                        tblPhones cPhone = objLead.tblPhones.FirstOrDefault(x => x.phoneID == phone.PhoneID);
                        if (cPhone != null)
                        {
                            cPhone.phone = phone.Phone;
                            cPhone.phoneTypeID = phone.PhoneTypeID;
                            cPhone.doNotCall = phone.DoNotCall;
                            cPhone.ext = phone.Ext;
                            cPhone.main = phone.Main;
                        }
                    }
                }

                //los que ya no vienen del cliente
                var clientEmailIDs = Lead.ContactInfo.Emails
                    .Where(x => x.EmailID != null)
                    .Select(x => x.EmailID).ToList();

                db.tblLeadEmails
                        .Where(x => !clientEmailIDs.Contains(x.emailID))
                        .ToList()
                        .ForEach(db.tblLeadEmails.DeleteObject);

                //los que vienen del cliente
                foreach (var email in Lead.ContactInfo.Emails)
                {
                    if (email.EmailID == null)
                    {
                        //nuevo
                        tblLeadEmails newEmail = new tblLeadEmails();
                        newEmail.email = email.Email;
                        newEmail.leadID = objLead.leadID;
                        newEmail.main = email.Main;
                        db.tblLeadEmails.AddObject(newEmail);
                    }
                    else if (email.EmailID != null)
                    {
                        //actualizar
                        tblLeadEmails cEmail = objLead.tblLeadEmails.FirstOrDefault(x => x.emailID == email.EmailID);
                        if (cEmail != null)
                        {
                            cEmail.email = email.Email;
                            cEmail.main = email.Main;
                        }
                    }
                }
            } else
            {
                foreach (var phone in Lead.ContactInfo.Phones)
                {
                    tblPhones newPhone = new tblPhones();
                    newPhone.phone = phone.Phone;
                    newPhone.phoneTypeID = phone.PhoneTypeID;
                    newPhone.doNotCall = phone.DoNotCall;
                    newPhone.leadID = objLead.leadID;
                    newPhone.ext = phone.Ext;
                    newPhone.main = phone.Main;
                    db.tblPhones.AddObject(newPhone);
                }

                foreach(var email in Lead.ContactInfo.Emails)
                {
                    tblLeadEmails newEmail = new tblLeadEmails();
                    newEmail.email = email.Email;
                    newEmail.leadID = objLead.leadID;
                    newEmail.main = email.Main;
                    db.tblLeadEmails.AddObject(newEmail);
                }
            }

            db.SaveChanges();

            //asignar leadID
            Lead.LeadID = objLead.leadID;

            //recoger los teléfonos y correos ya con ID
            Lead.ContactInfo.Phones = new List<FollowUpViewModel.PhoneInfo>();
            foreach (var phone in objLead.tblPhones)
            {
                FollowUpViewModel.PhoneInfo newPhone = new FollowUpViewModel.PhoneInfo();
                newPhone.PhoneID = phone.phoneID;
                newPhone.PhoneTypeID = phone.phoneTypeID;
                newPhone.Phone = phone.phone;
                newPhone.Ext = phone.ext;
                newPhone.DoNotCall = phone.doNotCall;
                newPhone.Main = phone.main;
                newPhone.Editing = false;
                Lead.ContactInfo.Phones.Add(newPhone);
            }

            Lead.ContactInfo.Emails = new List<FollowUpViewModel.EmailInfo>();
            foreach (var email in objLead.tblLeadEmails)
            {
                FollowUpViewModel.EmailInfo newEmail = new FollowUpViewModel.EmailInfo();
                newEmail.EmailID = email.emailID;
                newEmail.Email = email.email;
                newEmail.Main = email.main;
                newEmail.Editing = false;
                Lead.ContactInfo.Emails.Add(newEmail);
            }

            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "Log Saved";
            response.ObjectID = Lead;

            return response;
        }

        public static AttemptResponse SaveLog(FollowUpViewModel.LogItem model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            UserSession session = new UserSession();
            //try
            //{
            if (model.InteractionTypeID == 1)
            {
                //obtener llamada
                var CallLog = (from c in db.tblCallLogs
                               where c.callLogID == model.CallLogID
                               select c).FirstOrDefault();

                if (CallLog != null)
                {
                    Guid? LeadID = new Guid?();
                    if (CallLog.leadID != null)
                    {
                        //actualizar el lead
                        LeadID = CallLog.leadID;
                        CallLog.tblLeads.firstName = model.FirstName;
                        CallLog.tblLeads.lastName = model.LastName;
                        CallLog.tblLeads.modifiedByUserID = session.UserID;
                        CallLog.tblLeads.modificationDate = DateTime.Now;
                        if (CallLog.tblLeads.tblPhones.FirstOrDefault(x => x.phone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "") == model.Phone) == null)
                        {
                            tblPhones newPhone = new tblPhones();
                            newPhone.phone = model.Phone;
                            newPhone.phoneTypeID = 4;
                            if (CallLog.tblLeads.tblPhones.Count() == 0)
                            {
                                newPhone.main = true;
                            }
                            CallLog.tblLeads.tblPhones.Add(newPhone);
                        }
                        if (model.Email != null)
                        {
                            if (model.EmailID != null)
                            {
                                CallLog.tblLeads.tblLeadEmails.FirstOrDefault(x => x.emailID == model.EmailID).email = model.Email;
                            }
                            else
                            {
                                tblLeadEmails newEmail = new tblLeadEmails();
                                newEmail.email = model.Email;
                                if (CallLog.tblLeads.tblLeadEmails.Count() == 0)
                                {
                                    newEmail.main = true;
                                }
                                newEmail.leadID = (Guid)LeadID;
                                db.tblLeadEmails.AddObject(newEmail);
                            }
                        }
                    }
                    else
                    {
                        //crear nuevo lead y actualizar relación
                        tblLeads newLead = new tblLeads();
                        newLead.leadID = Guid.NewGuid();
                        LeadID = newLead.leadID;
                        newLead.initialTerminalID = model.TerminalID;
                        newLead.terminalID = model.TerminalID;
                        newLead.firstName = model.FirstName;
                        newLead.lastName = model.LastName;
                        //ubicación
                        string source = "";
                        string sourceAreaCode = "000";
                        if (CallLog.callIO)
                        {
                            //inbound
                            source = CallLog.phoneSource;
                        }
                        else
                        {
                            //outbound
                            source = CallLog.phoneDialed;
                        }
                        if (source.Length >= 10)
                        {
                            source = source.Substring(source.Length - 10);
                            sourceAreaCode = source.Substring(0, 3);
                        }
                        var currentArea = (from c in db.tblAreaCodes
                                           join a in db.tblAreas
                                           on c.areaID equals a.areaID
                                           into c_a
                                           from a in c_a.DefaultIfEmpty()
                                           join s in db.tblStates
                                           on a.stateID equals s.stateID
                                           into c_s
                                           from s in c_s.DefaultIfEmpty()
                                           where c.areaCodeID == CallLog.areaCodeID
                                           select new
                                           {
                                               c.areaCodeID,
                                               c.areaID,
                                               a.area,
                                               a.stateID,
                                               s.state,
                                               s.countryID,
                                               c.timeZoneID,
                                           }).FirstOrDefault();
                        if (currentArea != null)
                        {
                            newLead.countryID = currentArea.countryID;
                            newLead.stateID = currentArea.stateID;
                            newLead.state = currentArea.state;
                            newLead.city = currentArea.area;
                            newLead.timeZoneID = currentArea.timeZoneID;
                        }
                        newLead.leadTypeID = 47;
                        newLead.bookingStatusID = model.BookingStatusID;
                        newLead.qualificationStatusID = model.QualificationStatusID;
                        //leadsource
                        if (CallLog.callIO)
                        {
                            //Inbound
                            var LeadSource = (from l in db.tblTerminalPhones
                                              join c in db.tblLeadSourcesChannels
                                              on l.leadSourceChannelID equals c.leadSourceChannelID
                                              into l_c
                                              from c in l_c.DefaultIfEmpty()
                                              where l.phone == CallLog.phoneDialed
                                              select c.leadSourceID).FirstOrDefault();
                            if (LeadSource != null)
                            {
                                newLead.leadSourceID = LeadSource;
                            }
                        }
                        newLead.inputByUserID = session.UserID;
                        newLead.assignedToUserID = session.UserID;
                        newLead.inputDateTime = DateTime.Now;
                        newLead.assignationDate = DateTime.Now;
                        newLead.inputMethodID = 1;
                        newLead.leadStatusID = 20;
                        newLead.lastInteractionTypeID = 1;
                        if (model.DiscardReasonID != null)
                        {
                            newLead.discardReasonID = model.DiscardReasonID;
                        }
                        newLead.interestLevelID = model.InterestLevelID;
                        newLead.interestedInDestinationID = model.InterestedInDestinationID;

                        db.tblLeads.AddObject(newLead);
                        db.SaveChanges();
                        model.LeadID = newLead.leadID;

                        //phone
                        tblPhones newPhone = new tblPhones();
                        newPhone.phone = model.Phone;
                        newPhone.phoneTypeID = 4;
                        newPhone.main = true;
                        newLead.tblPhones.Add(newPhone);

                        //email
                        if (model.Email != null)
                        {
                            tblLeadEmails newEmail = new tblLeadEmails();
                            newEmail.email = model.Email;
                            newEmail.main = true;
                            newEmail.leadID = (Guid)LeadID;
                            db.tblLeadEmails.AddObject(newEmail);
                            db.SaveChanges();
                            model.EmailID = newEmail.emailID;
                        }

                        CallLog.leadID = LeadID;
                        //si es outbound, buscar llamadas inboun del mismo numero, para relacionarlas al lead
                        if (!CallLog.callIO)
                        {
                            var NotAnswered = from c in db.tblCallLogs
                                              where c.phoneSource == CallLog.phoneDialed
                                              && c.callDateTime < CallLog.callDateTime
                                              select c;

                            foreach (var na in NotAnswered)
                            {
                                na.leadID = LeadID;
                            }
                        }
                    }

                    //revisar si tiene interacción
                    if (CallLog.interactionID != null)
                    {
                        //actualizar la interacción
                        var Interaction = (from i in db.tblInteractions
                                           where i.interactionID == CallLog.interactionID
                                           select i).FirstOrDefault();
                        if (Interaction != null)
                        {
                            Interaction.interactionComments = model.InteractionComments;
                            Interaction.interestLevelID = model.InterestLevelID;
                            Interaction.bookingStatusID = model.BookingStatusID;
                            if (model.DiscardReasonID != null)
                            {
                                Interaction.discardReasonID = model.DiscardReasonID;
                            }
                        }
                    }
                    else
                    {
                        if (model.InteractionComments != null || model.InterestLevelID != null || model.BookingStatusID != null)
                        {
                            //crear nueva interacción
                            tblInteractions newInteraction = new tblInteractions();
                            newInteraction.leadID = LeadID;
                            newInteraction.interactionTypeID = 1;
                            newInteraction.interactionComments = model.InteractionComments;
                            newInteraction.savedByUserID = session.UserID;
                            newInteraction.dateSaved = DateTime.Now;
                            newInteraction.interactedWithUserID = session.UserID;
                            newInteraction.interestLevelID = model.InterestLevelID;
                            newInteraction.bookingStatusID = model.BookingStatusID;
                            if (model.DiscardReasonID != null)
                            {
                                newInteraction.discardReasonID = model.DiscardReasonID;
                            }
                            db.tblInteractions.AddObject(newInteraction);
                            db.SaveChanges();
                            CallLog.interactionID = newInteraction.interactionID;
                            model.InteractionID = newInteraction.interactionID;
                        }
                    }

                    db.SaveChanges();
                }
            }

            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "Log Saved";
            response.ObjectID = model;
            //}
            //catch (Exception ex)
            //{
            //    response.Type = Attempt_ResponseTypes.Error;
            //    response.Message = "Log not Saved";
            //    response.ObjectID = 0;
            //    response.Exception = ex;
            //}

            return response;
        }

        public static DependantFields GetDependentFields()
        {
            ePlatEntities db = new ePlatEntities();
            DependantFields df = new DependantFields();
            df.Fields = new List<DependantFields.DependantField>();
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalID = terminals[0];
            int? sysWorkGroupID = db.tblTerminals.Where(x => x.terminalID == terminalID).FirstOrDefault().sysWorkGroupID;

            DependantFields.FieldValue valUnknown = new DependantFields.FieldValue();
            valUnknown.ParentValue = null;
            valUnknown.Value = "";
            valUnknown.Text = "Unknown";

            DependantFields.FieldValue valSelect = new DependantFields.FieldValue();
            valSelect.ParentValue = null;
            valSelect.Value = "";
            valSelect.Text = "Select One";

            //TerminalID > LeadSourceID
            DependantFields.DependantField LeadSourceID = new DependantFields.DependantField();
            LeadSourceID.Field = "LeadSourceID";
            LeadSourceID.ParentField = "TerminalID";
            LeadSourceID.Values = new List<DependantFields.FieldValue>();

            var LeadSources = from t in db.tblTerminals_LeadSources
                              join ls in db.tblLeadSources on t.leadSourceID equals ls.leadSourceID
                              into t_ls
                              from ls in t_ls.DefaultIfEmpty()
                              where t.terminalID == terminalID
                              orderby t.terminalID, ls.leadSource
                              select new
                              {
                                  t.terminalID,
                                  t.leadSourceID,
                                  ls.leadSource
                              };

            foreach (var leadSource in LeadSources)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = leadSource.leadSourceID.ToString();
                val.Text = leadSource.leadSource;
                LeadSourceID.Values.Add(val);
            }

            LeadSourceID.Values.Insert(0, valUnknown);
            df.Fields.Add(LeadSourceID);

            //LeadSourceID > LeadSourceChannelID
            DependantFields.DependantField LeadSourceChannelID = new DependantFields.DependantField();
            LeadSourceChannelID.Field = "LeadSourceChannelID";
            LeadSourceChannelID.ParentField = "LeadSourceID";
            LeadSourceChannelID.Values = new List<DependantFields.FieldValue>();

            var LeadSourceIDs = LeadSources.Select(x => x.leadSourceID).Distinct().ToList();

            var LeadSourceChannels = from t in db.tblTerminals_LeadSourcesChannels
                                     join c in db.tblLeadSourcesChannels on t.leadSourceChannelID equals c.leadSourceChannelID
                                     into t_c
                                     from c in t_c.DefaultIfEmpty()
                                     where LeadSourceIDs.Contains(c.leadSourceID)
                                     && t.terminalID == terminalID
                                     select new
                                     {
                                         c.leadSourceID,
                                         c.leadSourceChannelID,
                                         c.channel
                                     };

            foreach (var channel in LeadSourceChannels)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = channel.leadSourceID;
                val.Value = channel.leadSourceChannelID.ToString();
                val.Text = channel.channel;
                LeadSourceChannelID.Values.Add(val);
            }
            LeadSourceChannelID.Values.Insert(0, valUnknown);
            df.Fields.Add(LeadSourceChannelID);

            //TerminalID > LeadStatusID
            DependantFields.DependantField LeadStatusID = new DependantFields.DependantField();
            LeadStatusID.Field = "LeadStatusID";
            LeadStatusID.ParentField = "TerminalID";
            LeadStatusID.Values = new List<DependantFields.FieldValue>();

            var LeadStatus = from l in db.tblTerminals_LeadStatus
                             join ls in db.tblLeadStatus on l.leadStatusID equals ls.leadStatusID
                             into l_ls
                             from ls in l_ls.DefaultIfEmpty()
                             where l.terminalID == terminalID
                             select new
                             {
                                 l.terminalID,
                                 l.leadStatusID,
                                 ls.leadStatus
                             };

            foreach (var status in LeadStatus)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = status.leadStatusID.ToString();
                val.Text = status.leadStatus;
                LeadStatusID.Values.Add(val);
            }
            df.Fields.Add(LeadStatusID);

            //TerminalID > BookingStatusID
            DependantFields.DependantField BookingStatusID = new DependantFields.DependantField();
            BookingStatusID.Field = "BookingStatusID";
            BookingStatusID.ParentField = "TerminalID";
            BookingStatusID.Values = new List<DependantFields.FieldValue>();

            var BookingStatusQ = from s in db.tblBookingStatus
                                 where s.terminalID == terminalID
                                 select new
                                 {
                                     s.terminalID,
                                     s.bookingStatus,
                                     s.bookingStatusID
                                 };

            foreach (var bs in BookingStatusQ)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = bs.bookingStatusID.ToString();
                val.Text = bs.bookingStatus;
                BookingStatusID.Values.Add(val);
            }
            df.Fields.Add(BookingStatusID);

            //TerminalID > LeadTypeID
            DependantFields.DependantField LeadTypeID = new DependantFields.DependantField();
            LeadTypeID.Field = "LeadTypeID";
            LeadTypeID.ParentField = "TerminalID";
            LeadTypeID.Values = new List<DependantFields.FieldValue>();

            var LeadTypes = from t in db.tblLeadTypes_SysWorkGroups
                            join lt in db.tblLeadTypes on t.leadTypeID equals lt.leadTypeID
                            into t_lt
                            from lt in t_lt.DefaultIfEmpty()
                            orderby lt.leadType
                            where t.sysWorkGroupID == sysWorkGroupID
                            select new
                            {
                                t.leadTypeID,
                                lt.leadType
                            };

            foreach (var leadType in LeadTypes)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = leadType.leadTypeID.ToString();
                val.Text = leadType.leadType;
                LeadTypeID.Values.Add(val);
            }
            df.Fields.Add(LeadTypeID);

            //TerminalID > InterestLevelID
            DependantFields.DependantField InterestLevelID = new DependantFields.DependantField();

            InterestLevelID.Field = "InterestLevelID";
            InterestLevelID.ParentField = "TerminalID";
            InterestLevelID.Values = new List<DependantFields.FieldValue>();

            var InterestLevels = from i in db.tblInterestLevels
                                 select new
                                 {
                                     i.interestLevelID,
                                     i.interestLevel
                                 };

            foreach (var interest in InterestLevels)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = interest.interestLevelID.ToString();
                val.Text = interest.interestLevel;
                InterestLevelID.Values.Add(val);
            }
            df.Fields.Add(InterestLevelID);

            //TerminalID > DiscardReasonID
            DependantFields.DependantField DiscardReasonID = new DependantFields.DependantField();
            DiscardReasonID.Field = "DiscardReasonID";
            DiscardReasonID.ParentField = "TerminalID";
            DiscardReasonID.Values = new List<DependantFields.FieldValue>();

            var DiscardReasons = from d in db.tblDiscardReasons
                                 select new
                                 {
                                     d.discardReason,
                                     d.discardReasonID
                                 };

            foreach (var reason in DiscardReasons)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = reason.discardReasonID.ToString();
                val.Text = reason.discardReason;
                DiscardReasonID.Values.Add(val);
            }
            DiscardReasonID.Values.Insert(0, valSelect);
            df.Fields.Add(DiscardReasonID);

            //TerminalID > AgentID
            DependantFields.DependantField AgentID = new DependantFields.DependantField();
            AgentID.Field = "AgentID";
            AgentID.ParentField = "TerminalID";
            AgentID.Values = new List<DependantFields.FieldValue>();

            if (!Utils.GeneralFunctions.IsUserInRole("agent", session.UserID, true, db))
            {
                //si no es agente
                var UsersQ = from user in db.tblUsers_Terminals
                             join profile in db.tblUserProfiles on user.userID equals profile.userID
                             join role in db.tblUsers_SysWorkGroups on user.userID equals role.userID
                             join roleInfo in db.aspnet_Roles on role.roleID equals roleInfo.RoleId
                             join membership in db.aspnet_Membership on user.userID equals membership.UserId
                             where user.terminalID == terminalID
                             && (roleInfo.RoleName.Contains("agent") || roleInfo.RoleName.Contains("supervisor"))
                             && membership.IsApproved
                             && profile.phoneEXT != null
                             && profile.phoneEXT != ""
                             orderby profile.firstName
                             select new
                             {
                                 user.userID,
                                 profile.firstName,
                                 profile.lastName,
                                 profile.phoneEXT
                             };

                foreach (var user in UsersQ)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = null;
                    val.Value = user.userID.ToString();
                    val.Text = user.firstName + " " + user.lastName + " (" + user.phoneEXT + ")";
                    if (user.userID == session.UserID)
                    {
                        val.Selected = true;
                    }
                    AgentID.Values.Add(val);
                }
            }

            if (AgentID.Values.Count(x => x.Value == session.UserID.ToString()) == 0)
            {
                tblUserProfiles us = db.tblUserProfiles.Single(x => x.userID == session.UserID);
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = us.userID.ToString();
                val.Text = us.firstName + " " + us.lastName;
                val.Selected = true;
                AgentID.Values.Add(val);
            }


            df.Fields.Add(AgentID);

            //TerminalID > InteractionTypeID
            DependantFields.DependantField InteractionTypeID = new DependantFields.DependantField();
            InteractionTypeID.Field = "InteractionTypeID";
            InteractionTypeID.ParentField = "TerminalID";
            InteractionTypeID.Values = new List<DependantFields.FieldValue>();

            var InteractionTypes = from t in db.tblInteractionTypes
                                   select new
                                   {
                                       t.interactionTypeID,
                                       t.interactionType
                                   };

            foreach (var type in InteractionTypes)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = type.interactionTypeID.ToString();
                val.Text = type.interactionType;
                InteractionTypeID.Values.Add(val);
            }
            df.Fields.Add(InteractionTypeID);

            //TerminalID > QualificationStatusID
            DependantFields.DependantField QualificationStatusID = new DependantFields.DependantField();
            QualificationStatusID.Field = "QualificationStatusID";
            QualificationStatusID.ParentField = "TerminalID";
            QualificationStatusID.Values = new List<DependantFields.FieldValue>();

            var QualificationStatus = from q in db.tblQualificationStatus
                                      select new
                                      {
                                          q.qualificationStatus,
                                          q.qualificationStatusID
                                      };

            foreach (var qs in QualificationStatus)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = qs.qualificationStatusID.ToString();
                val.Text = qs.qualificationStatus;
                QualificationStatusID.Values.Add(val);
            }
            df.Fields.Add(QualificationStatusID);

            //TerminalID > InterestedInDestinationID
            DependantFields.DependantField InterestedInDestinationID = new DependantFields.DependantField();
            InterestedInDestinationID.Field = "InterestedInDestinationID";
            InterestedInDestinationID.ParentField = "TerminalID";
            InterestedInDestinationID.Values = new List<DependantFields.FieldValue>();

            var Destinations = from d in db.tblTerminals_Destinations
                               join destination in db.tblDestinations on d.destinationID equals destination.destinationID
                               into d_destination
                               from destination in d_destination.DefaultIfEmpty()
                               where d.terminalID == terminalID
                               select new
                               {
                                   d.destinationID,
                                   destination.destination
                               };

            foreach (var d in Destinations)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = d.destinationID.ToString();
                val.Text = d.destination;
                InterestedInDestinationID.Values.Add(val);
            }
            df.Fields.Add(InterestedInDestinationID);

            //TerminalID > InputMethodID
            DependantFields.DependantField InputMethodID = new DependantFields.DependantField();
            InputMethodID.Field = "InputMethodID";
            InputMethodID.ParentField = "TerminalID";
            InputMethodID.Values = new List<DependantFields.FieldValue>();

            var InputMethods = from m in db.tblInputMethods
                               select new
                               {
                                   m.inputMethodID,
                                   m.inputMethod
                               };

            foreach (var m in InputMethods)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = m.inputMethodID.ToString();
                val.Text = m.inputMethod;
                InputMethodID.Values.Add(val);
            }
            df.Fields.Add(InputMethodID);

            //TerminalID > CountryID
            DependantFields.DependantField CountryID = new DependantFields.DependantField();
            CountryID.Field = "CountryID";
            CountryID.ParentField = "TerminalID";
            CountryID.Values = new List<DependantFields.FieldValue>();

            var Countries = from c in db.tblCountries
                            select new
                            {
                                c.countryID,
                                c.country
                            };

            foreach (var c in Countries)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = c.countryID.ToString();
                val.Text = c.country;
                CountryID.Values.Add(val);
            }
            df.Fields.Add(CountryID);

            //CountryID > StateID
            DependantFields.DependantField StateID = new DependantFields.DependantField();
            StateID.Field = "StateID";
            StateID.ParentField = "CountryID";
            StateID.Values = new List<DependantFields.FieldValue>();

            var States = from s in db.tblStates
                         select new
                         {
                             s.countryID,
                             s.stateID,
                             s.state
                         };

            foreach (var s in States)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = s.countryID;
                val.Value = s.stateID.ToString();
                val.Text = s.state;
                StateID.Values.Add(val);
            }
            df.Fields.Add(StateID);

            //TerminalID > PhoneTypeID
            DependantFields.DependantField PhoneTypeID = new DependantFields.DependantField();
            PhoneTypeID.Field = "PhoneTypeID";
            PhoneTypeID.ParentField = "TerminalID";
            PhoneTypeID.Values = new List<DependantFields.FieldValue>();

            var PhoneTypes = from t in db.tblPhoneTypes
                             orderby t.phoneType
                             select new
                             {
                                 t.phoneTypeID,
                                 t.phoneType
                             };

            foreach (var t in PhoneTypes)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = null;
                val.Value = t.phoneTypeID.ToString();
                val.Text = t.phoneType;
                PhoneTypeID.Values.Add(val);
            }
            df.Fields.Add(PhoneTypeID);

            return df;
        }

        public static FollowUpViewModel.SearchLeadsResponse SearchLeads(FollowUpViewModel.SearchFilters model)
        {
            FollowUpViewModel.SearchLeadsResponse results = new FollowUpViewModel.SearchLeadsResponse();
            results.Leads = new List<FollowUpViewModel.LeadInfo>();
            results.Summary = new FollowUpViewModel.LeadsSummary();
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            long terminalid = terminals[0];
            ePlatEntities db = new ePlatEntities();

            DateTime? endDate = null;
            if(model.Search_ToDate != null)
            {
                endDate = model.Search_ToDate.Value.AddDays(1);
            }

            int leadStatusLength = 0;
            int?[] leadStatusID = new int?[] { };
            if(model.Search_LeadStatusID != null)
            {
                leadStatusID = model.Search_LeadStatusID;
                leadStatusLength = model.Search_LeadStatusID.Length;
            }

            int bookingStatusLength = 0;
            int?[] bookingStatusID = new int?[] { };
            if(model.Search_BookingStatusID != null)
            {
                bookingStatusID = model.Search_BookingStatusID;
                bookingStatusLength = model.Search_BookingStatusID.Length;
            }

            int leadTypeLength = 0;
            int?[] leadTypeID = new int?[] { };
            if(model.Search_LeadTypeID != null)
            {
                leadTypeID = model.Search_LeadTypeID;
                leadTypeLength = model.Search_LeadTypeID.Length;
            }

            int leadSourceChannelLength = 0;
            int?[] leadSourceChannelID = new int?[] { };
            if(model.Search_LeadSourceChannelID != null)
            {
                leadSourceChannelID = model.Search_LeadSourceChannelID;
                leadSourceChannelLength = model.Search_LeadSourceChannelID.Length;
            }

            var Leads = from l in db.tblLeads
                        join s in db.tblStates on l.stateID equals s.stateID
                        into l_s
                        from s in l_s.DefaultIfEmpty()
                        join ls in db.tblLeadSources on l.leadSourceID equals ls.leadSourceID
                        into l_ls
                        from ls in l_ls.DefaultIfEmpty()
                        join ch in db.tblLeadSourcesChannels on l.leadSourceChannelID equals ch.leadSourceChannelID
                        into l_ch
                        from ch in l_ch.DefaultIfEmpty()
                        join lt in db.tblLeadTypes on l.leadTypeID equals lt.leadTypeID
                        into l_lt
                        from lt in l_lt.DefaultIfEmpty()
                        join st in db.tblLeadStatus on l.leadStatusID equals st.leadStatusID
                        into l_st
                        from st in l_st.DefaultIfEmpty()
                        join d1 in db.tblDestinations on l.interestedInDestinationID equals d1.destinationID
                        into l_d1
                        from d1 in l_d1.DefaultIfEmpty()
                        join im in db.tblInputMethods on l.inputMethodID equals im.inputMethodID
                        into l_im
                        from im in l_im.DefaultIfEmpty()
                        join bs in db.tblBookingStatus on l.bookingStatusID equals bs.bookingStatusID
                        into l_bs
                        from bs in l_bs.DefaultIfEmpty()
                        join il in db.tblInterestLevels on l.interestLevelID equals il.interestLevelID
                        into l_il
                        from il in l_il.DefaultIfEmpty()
                        join ag in db.tblUserProfiles on l.assignedToUserID equals ag.userID
                        into l_ag
                        from ag in l_ag.DefaultIfEmpty()
                        join qs in db.tblQualificationStatus on l.qualificationStatusID equals qs.qualificationStatusID
                        into l_qs
                        from qs in l_qs.DefaultIfEmpty()
                        where l.terminalID == terminalid
                        && (model.Search_FromDate == null || l.inputDateTime > model.Search_FromDate)
                        && (model.Search_ToDate == null || l.inputDateTime < endDate)
                        && (model.Search_GuestName == null || l.firstName.Contains(model.Search_GuestName) || l.lastName.Contains(model.Search_GuestName))
                        && (model.Search_AssignedToUserID == null || l.assignedToUserID == model.Search_AssignedToUserID)
                        && (leadStatusLength == 0 || leadStatusID.Contains(l.leadStatusID))
                        && (bookingStatusLength == 0 || bookingStatusID.Contains(l.bookingStatusID))
                        && (leadTypeLength == 0 || leadTypeID.Contains(l.leadTypeID))
                        && (leadSourceChannelLength == 0 || leadSourceChannelID.Contains(l.leadSourceChannelID))
                        select new
                        {
                            l.leadID,
                            l.firstName,
                            l.lastName,
                            l.countryID,
                            l.stateID,
                            s.state,
                            l.city,
                            l.leadSourceID,
                            ls.leadSource,
                            l.leadSourceChannelID,
                            ch.channel,
                            l.leadTypeID,
                            lt.leadType,
                            l.leadStatusID,
                            st.leadStatus,
                            l.interestedInDestinationID,
                            d1.destination,
                            l.inputMethodID,
                            im.inputMethod,
                            l.bookingStatusID,
                            bs.bookingStatus,
                            l.interestLevelID,
                            il.interestLevel,
                            l.inputDateTime,
                            l.assignedToUserID,
                            agFirstName = ag.firstName,
                            agLastName = ag.lastName,
                            l.mood,
                            l.qualificationStatusID,
                            qs.qualificationStatus,
                            l.tblPhones,
                            l.tblLeadEmails
                        };

            foreach(var lead in Leads)
            {
                FollowUpViewModel.LeadInfo newLead = new FollowUpViewModel.LeadInfo();
                newLead.ContactInfo = new FollowUpViewModel.ContactInfo();
                newLead.CustomerInfo = new FollowUpViewModel.CustomerInfo();
                newLead.QualificationInfo = new FollowUpViewModel.QualificationInfo();

                newLead.LeadID = lead.leadID;
                newLead.CustomerInfo.FirstName = lead.firstName;
                newLead.CustomerInfo.LastName = lead.lastName;
                newLead.CustomerInfo.CountryID = lead.countryID;
                newLead.CustomerInfo.StateID = lead.stateID;
                newLead.CustomerInfo.State = lead.state;
                newLead.CustomerInfo.City = lead.city;
                newLead.CustomerInfo.LeadSourceID = lead.leadSourceID;
                newLead.CustomerInfo.Source = lead.leadSource;
                newLead.CustomerInfo.LeadSourceChannelID = lead.leadSourceChannelID;
                newLead.CustomerInfo.Channel = lead.channel;
                newLead.CustomerInfo.LeadTypeID = lead.leadTypeID;
                newLead.CustomerInfo.FirstContactType = lead.leadType;
                newLead.CustomerInfo.LeadStatusID = lead.leadStatusID;
                newLead.CustomerInfo.LeadStatus = lead.leadStatus;
                newLead.CustomerInfo.InterestedInDestinationID = lead.interestedInDestinationID;
                newLead.CustomerInfo.InterestedInDestination = lead.destination;
                newLead.CustomerInfo.InputMethodID = lead.inputMethodID;
                newLead.CustomerInfo.InputMethod = lead.inputMethod;
                newLead.CustomerInfo.BookingStatusID = lead.bookingStatusID;
                newLead.CustomerInfo.BookingStatus = lead.bookingStatus;
                newLead.CustomerInfo.InterestLevelID = lead.interestLevelID;
                newLead.CustomerInfo.InterestLevel = lead.interestLevel;
                newLead.CustomerInfo.DateSaved = lead.inputDateTime;
                newLead.CustomerInfo.AssignedToUserID = lead.assignedToUserID;
                newLead.CustomerInfo.AssignedToAgent = lead.agFirstName + " " + lead.agLastName;
                newLead.CustomerInfo.Mood = lead.mood;

                newLead.QualificationInfo.QualificationStatusID = lead.qualificationStatusID;
                newLead.QualificationInfo.Qualification = lead.qualificationStatus;

                newLead.ContactInfo.Phones = new List<FollowUpViewModel.PhoneInfo>();
                foreach(var phone in lead.tblPhones)
                {
                    FollowUpViewModel.PhoneInfo newPhone = new FollowUpViewModel.PhoneInfo();
                    newPhone.PhoneID = phone.phoneID;
                    newPhone.PhoneTypeID = phone.phoneTypeID;
                    newPhone.Phone = phone.phone;
                    newPhone.Ext = phone.ext;
                    newPhone.DoNotCall = phone.doNotCall;
                    newPhone.Main = phone.main;
                    newPhone.Editing = false;
                    newLead.ContactInfo.Phones.Add(newPhone);
                }

                newLead.ContactInfo.Emails = new List<FollowUpViewModel.EmailInfo>();
                foreach(var email in lead.tblLeadEmails)
                {
                    FollowUpViewModel.EmailInfo newEmail = new FollowUpViewModel.EmailInfo();
                    newEmail.EmailID = email.emailID;
                    newEmail.Email = email.email;
                    newEmail.Main = email.main;
                    newEmail.Editing = false;
                    newLead.ContactInfo.Emails.Add(newEmail);
                }

                results.Leads.Add(newLead);
            }

            //summary

            return results;
        }
    }
}
