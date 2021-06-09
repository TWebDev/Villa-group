using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Text.RegularExpressions;
using ePlatBack.Models.eplatformDataModel;

namespace ePlatBack.Models.DataModels
{
    public class NetCenterDataModel
    {
        public class NetCenterCatalogs
        {
            public static List<SelectListItem> FillDrpPhones(long terminalid)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var Phones = from p in db.tblTerminalPhones
                             where p.terminalID == terminalid
                             select p;

                foreach (var phone in Phones)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = phone.label + " : " + phone.phone,
                        Value = phone.phone
                    });
                }

                return list;
            }
        }

        public class CallsLog
        {
            public static void SaveCallStarted(NetCenterViewModel.CallLogEvent.CallStarted model)
            {
                if (model.agi_uniqueid != null)
                {
                    ePlatEntities db = new ePlatEntities();
                    //verificar si se trata de una llamada de los números trackeados
                    string accountCode = model.agi_accountcode.Substring(model.agi_accountcode.Length - 10);
                    var trackingPhone = db.tblTerminalPhones.FirstOrDefault(x => x.phone == accountCode);
                    if (trackingPhone != null)
                    {
                        string source = model.agi_callerid;
                        string sourceAreaCode = "000";
                        if (source.Length >= 10)
                        {
                            source = source.Substring(source.Length - 10);
                            sourceAreaCode = source.Substring(0, 3);
                        }

                        //guardar
                        tblCallLogs newCallLog = new tblCallLogs();
                        newCallLog.callIO = true;
                        newCallLog.@event = "Call Started";
                        newCallLog.uniqueID = model.agi_uniqueid;
                        newCallLog.callDateTime = DateTime.Now;
                        newCallLog.phoneSource = source;
                        newCallLog.phoneDialed = accountCode;
                        newCallLog.srcChannel = model.agi_channel;
                        newCallLog.terminalID = trackingPhone.terminalID;
                        //areaCodeID
                        var currentArea = db.tblAreaCodes.FirstOrDefault(x => x.code == sourceAreaCode);
                        if (currentArea != null)
                        {
                            newCallLog.areaCodeID = currentArea.areaCodeID;
                        }

                        db.tblCallLogs.AddObject(newCallLog);
                        db.SaveChanges();

                        //enviar a contexto SignalR - PENDIENTE

                    }
                }
            }

            public static void SaveCallFinished(NetCenterViewModel.CallLogEvent.CallFinished model)
            {
                ePlatEntities db = new ePlatEntities();
                //verificar si es entrante o saliente
                if (model.channel.IndexOf("Incomming") >= 0)
                {
                    //entrante
                    //verificar si se trata de una llamada de los números monitoreados
                    string accountCode = model.accountcode.Substring(model.accountcode.Length - 10);
                    var trackingPhone = db.tblTerminalPhones.FirstOrDefault(x => x.phone == accountCode);
                    if (trackingPhone != null)
                    {
                        string source = model.source;
                        string sourceAreaCode = "000";
                        if (source.Length >= 10)
                        {
                            source = source.Substring(source.Length - 10);
                            sourceAreaCode = source.Substring(0, 3);
                        }

                        //verificar si existe ya registro de la llamada
                        tblCallLogs call = db.tblCallLogs.FirstOrDefault(c => c.uniqueID == model.uniqueid);
                        if (call == null)
                        {
                            //nuevo
                            call = new tblCallLogs();
                            call.uniqueID = model.uniqueid;
                            call.callIO = true;
                            call.phoneSource = source;
                            call.phoneDialed = accountCode;
                            //areaCodeID
                            var currentArea = db.tblAreaCodes.FirstOrDefault(x => x.code == sourceAreaCode);
                            if (currentArea != null)
                            {
                                call.areaCodeID = currentArea.areaCodeID;
                            }
                            call.srcChannel = model.channel;
                            call.terminalID = trackingPhone.terminalID;
                        }
                        call.@event = "Call Finished";
                        call.callDateTime = DateTime.Parse(model.answer);
                        call.duration = model.billsec;
                        call.disposition = model.disposition;
                        call.destination = model.destination;
                        //agentExtension
                        string dchannel = model.dstchannel;
                        call.dstChannel = dchannel;
                        string extension = string.Empty;
                        if (dchannel != null && dchannel != "")
                        {
                            extension = dchannel.Substring(dchannel.IndexOf("/") + 1);
                            if (extension.IndexOf("@") >= 0)
                            {
                                extension = extension.Substring(0, extension.IndexOf("@"));
                            }
                            else if (extension.IndexOf("-") >= 0)
                            {
                                extension = extension.Substring(0, extension.IndexOf("-"));
                            }
                            call.agentExtension = extension;
                        }
                        //callDestinationID
                        var callDestination = db.tblCallDestinationSettings.FirstOrDefault(x => x.terminalID == trackingPhone.terminalID && x.destination == model.destination);
                        if (callDestination != null)
                        {
                            call.callDestinationID = callDestination.callDestinationID;
                        }
                        //firstContact
                        Guid? leadID = null;
                        if (!NetCenterDataModel.CallsLog.IsFollowUp(call.callDateTime, call.phoneSource, ref leadID, trackingPhone.terminalID))
                        {
                            call.firstContact = true;
                        }
                        else
                        {
                            //verificar si el contacto anterior tiene registrado lead
                            if (leadID == null)
                            {
                                //buscar en teléfonos de leads de la terminal
                                var phone = (from p in db.tblPhones
                                             join l in db.tblLeads on p.leadID equals l.leadID
                                             into p_l
                                             from l in p_l.DefaultIfEmpty()
                                             where p.phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "") == call.phoneSource
                                             && l.terminalID == trackingPhone.terminalID
                                             select new
                                             {
                                                 p.phone,
                                                 p.leadID
                                             }).FirstOrDefault();
                                if (phone != null)
                                {
                                    call.leadID = phone.leadID;
                                }
                            }
                            else
                            {
                                call.leadID = leadID;
                            }
                        }

                        //actualizar registro o generar nuevo
                        if (call == null)
                        {
                            //nuevo
                            db.tblCallLogs.AddObject(call);
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    //saliente
                    //verificar si se trata de una llamada de una extensión monitoreada
                    var terminalID = (from x in db.tblTerminalExtensions
                                      where x.extension == model.source
                                      select x.terminalID).FirstOrDefault();

                    if (terminalID != null)
                    {
                        //en llamadas salientes no registramos el start, entonces crear como registro nuevo
                        tblCallLogs call = new tblCallLogs();
                        call.callIO = false;
                        call.@event = "Call Finished";
                        call.uniqueID = model.uniqueid;
                        call.callDateTime = DateTime.Parse(model.answer);
                        call.phoneSource = model.source;
                        string dialed = model.destination;
                        string dialedAreaCode = "000";
                        if (dialed.Length >= 10)
                        {
                            dialed = dialed.Substring(dialed.Length - 10);
                            dialedAreaCode = dialed.Substring(dialed.Length - 10, 3);
                        }
                        call.phoneDialed = dialed;
                        call.agentExtension = model.source;
                        call.duration = model.billsec;
                        call.disposition = model.disposition;
                        //areaCodeID
                        var currentArea = db.tblAreaCodes.FirstOrDefault(x => x.code == dialedAreaCode);
                        if (currentArea != null)
                        {
                            call.areaCodeID = currentArea.areaCodeID;
                        }
                        call.destination = model.destination;
                        call.srcChannel = model.channel;
                        call.dstChannel = model.dstchannel;
                        call.terminalID = terminalID;
                        //firstContact
                        Guid? leadID = null;
                        if (!NetCenterDataModel.CallsLog.IsFollowUp(call.callDateTime, call.phoneDialed, ref leadID, terminalID))
                        {
                            call.firstContact = true;
                        }
                        else
                        {
                            //verificar si el contacto anterior tiene registrado lead
                            if (leadID == null)
                            {
                                //buscar en teléfonos de leads de la terminal
                                var phone = (from p in db.tblPhones
                                             join l in db.tblLeads on p.leadID equals l.leadID
                                             into p_l
                                             from l in p_l.DefaultIfEmpty()
                                             where p.phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "") == call.phoneDialed
                                             && l.terminalID == terminalID
                                             select new
                                             {
                                                 p.phone,
                                                 p.leadID
                                             }).FirstOrDefault();

                                if (phone != null)
                                {
                                    call.leadID = phone.leadID;
                                }
                            }
                            else
                            {
                                call.leadID = leadID;
                            }
                        }

                        db.tblCallLogs.AddObject(call);
                        db.SaveChanges();
                    }
                }

                //enviar a contexto SignalR - PENDIENTE
            }

            public static void ImportFromCSV(string filePath)
            {
                ePlatEntities db = new ePlatEntities();

                //catalogo de areas
                var Areas = from code in db.tblAreaCodes
                            select new
                            {
                                code.code,
                                code.areaCodeID
                            };

                var CallDestinations = from d in db.tblCallDestinationSettings
                                       select new
                                       {
                                           d.destination,
                                           d.callDestinationID,
                                           d.terminalID
                                       };

                var TerminalPhones = from p in db.tblTerminalPhones
                                     select new
                                     {
                                         p.phone,
                                         p.terminalID
                                     };


                using (var reader = new StreamReader(@filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        line = line.Replace("\"", "");
                        var values = line.Split(',');

                        if (values[0].IndexOf("Date") < 0)
                        {
                            if (values[5] != "")
                            {
                                //INBOUND
                                DateTime date = DateTime.Parse(values[0]);
                                string source = values[1];
                                string sourceAreaCode = "000";
                                if (source.Length >= 10)
                                {
                                    source = source.Substring(source.Length - 10);
                                    sourceAreaCode = source.Substring(0, 3);
                                }
                                string destination = values[3];
                                string accountCode = values[5].Substring(values[5].Length - 10);
                                string accountAreaCode = accountCode.Substring(0, 3);
                                string schannel = values[4];
                                string dchannel = values[6];
                                string extension = string.Empty;
                                if (dchannel != "")
                                {
                                    extension = dchannel.Substring(dchannel.IndexOf("/") + 1);
                                    if (extension.IndexOf("@") >= 0)
                                    {
                                        extension = extension.Substring(0, extension.IndexOf("@"));
                                        if(extension.IndexOf("-") >= 0)
                                        {
                                            extension = extension.Substring(extension.IndexOf("-") + 1);
                                        }
                                    }
                                    else if (extension.IndexOf("-") >= 0)
                                    {
                                        extension = extension.Substring(0, extension.IndexOf("-"));
                                    }
                                }
                                string status = values[7];
                                int duration = int.Parse(values[8].Substring(0, values[8].IndexOf("s")));

                                //INBOUND
                                //revisar si el registro ya existe
                                var ExistLog = (from l in db.tblCallLogs
                                                where (l.callDateTime == date || l.srcChannel == schannel)
                                                && l.phoneSource == source
                                                select l).Count();

                                if (ExistLog == 0)
                                {
                                    //si no existe el registro, registrarlo
                                    tblCallLogs newLog = new tblCallLogs();
                                    newLog.callIO = true;
                                    newLog.@event = "Call Finished";
                                    //newLog.uniqueID = "";
                                    newLog.callDateTime = date;
                                    newLog.phoneSource = source;
                                    newLog.phoneDialed = accountCode;
                                    newLog.agentExtension = extension;
                                    newLog.duration = duration;
                                    newLog.disposition = status;
                                    //newLog.interactionID = "";
                                    //newLog.marketingCampaignID = "";
                                    var currentArea = Areas.FirstOrDefault(x => x.code == sourceAreaCode);
                                    if (currentArea != null)
                                    {
                                        newLog.areaCodeID = currentArea.areaCodeID;
                                    }
                                    //call destination
                                    newLog.destination = destination;
                                    long? terminalID = null;
                                    if (TerminalPhones.FirstOrDefault(x => x.phone == accountCode) != null)
                                    {
                                        terminalID = TerminalPhones.FirstOrDefault(x => x.phone == accountCode).terminalID;

                                        if (CallDestinations.Count(x => x.terminalID == terminalID && x.destination == destination) > 0)
                                        {
                                            newLog.callDestinationID = CallDestinations.FirstOrDefault(x => x.terminalID == terminalID && x.destination == destination).callDestinationID;
                                        }
                                    };
                                    newLog.terminalID = terminalID;

                                    db.tblCallLogs.AddObject(newLog);
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }

            public static PictureDataModel.FineUploaderResult UploadCSV(PictureDataModel.FineUpload upload)
            {
                ePlatEntities db = new ePlatEntities();
                AttemptResponse response = new AttemptResponse();
                string path = HttpContext.Current.Server.MapPath("~/");
                path = path.Substring(0, path.LastIndexOf("ePlatBack"));
                path = path + "ePlatBack\\Content\\files\\elastix";
                var fileName = upload.Filename;

                var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
                fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
                fileName = fileName.Replace("+", "");
                for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
                {
                    string encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                    string newFileName = fileName.Replace(encoded, "_");
                    fileName = newFileName;
                }

                string filePath = path + "\\" + fileName;
                //try
                //{
                upload.SaveAs(filePath);

                //PROCESO PARA EXTRAER LA INFORMACIÓN
                ImportFromCSV(filePath);

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "File Imported";
                //response.ObjectID = ;
                return new PictureDataModel.FineUploaderResult(true, new { response = response });
                //}
                //catch (Exception ex)
                //{
                //    response.Type = Attempt_ResponseTypes.Error;
                //    response.Message = "File NOT Imported";
                //    response.ObjectID = 0;
                //    response.Exception = ex;
                //    return new PictureDataModel.FineUploaderResult(false, new { response = response });
                //}
            }

            public static bool IsFollowUp(DateTime? callDateTime, string phone, ref Guid? leadID, long? terminalID = null, int? months = 3, bool? moreThanForty = true)
            {
                bool followUp = false;
                ePlatEntities db = new ePlatEntities();
                DateTime monthsAgo = callDateTime.Value.AddMonths((int)months * (-1));
                var IsFollowUp = from f in db.tblCallLogs
                                 where ((f.phoneSource == phone && f.callIO == true)
                                 || (f.phoneDialed == phone && f.callIO == false))
                                 && f.callDateTime < callDateTime
                                 && f.callDateTime > monthsAgo
                                 && f.disposition == "ANSWERED"
                                 orderby f.callDateTime descending
                                 select new
                                 {
                                     f.callLogID,
                                     f.callDateTime,
                                     f.leadID,
                                     f.terminalID,
                                     f.duration
                                 };

                if (moreThanForty == true)
                {
                    IsFollowUp = IsFollowUp.Where(f => f.duration > 40);
                }
                else
                {
                    IsFollowUp = IsFollowUp.Where(f => f.duration <= 40);
                }

                if (terminalID != null)
                {
                    if (IsFollowUp.Count(x => x.terminalID != null) > 0)
                    {
                        IsFollowUp = IsFollowUp.Where(x => x.terminalID == terminalID).OrderByDescending(x => x.callDateTime);
                    }
                    else
                    {
                        IsFollowUp = IsFollowUp.OrderByDescending(x => x.callDateTime);
                    }
                }

                if (IsFollowUp.FirstOrDefault() == null || phone.Length < 10)
                {
                    followUp = false;
                }
                else
                {
                    if (IsFollowUp.FirstOrDefault() != null)
                    {
                        leadID = IsFollowUp.FirstOrDefault().leadID;
                    }
                    followUp = true;
                }

                return followUp;
            }

            public static List<FollowUpViewModel.LogItem> GetLogs(DateTime date, Guid[] userIDs)
            {
                UserSession session = new UserSession();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                long terminalID = terminals[0];
                DateTime toDate = date.AddDays(1);
                int agentsLength = 0;
                Guid[] agentsArr = new Guid[] { };
                if (userIDs != null)
                {
                    agentsArr = userIDs;
                    agentsLength = userIDs.Count();
                }
                ePlatEntities db = new ePlatEntities();
                List<string> Extensions = new List<string>();
                if (agentsLength > 0)
                {
                    Extensions = (from x in db.tblUserProfiles
                                  where agentsArr.Contains(x.userID)
                                  select x.phoneEXT).ToList();
                }
                else
                {
                    //no hay agentes seleccionados, obtener las extensiones de la terminal
                    Extensions = (from x in db.tblTerminalExtensions
                                  where x.terminalID == terminalID
                                  select x.extension).ToList();
                }

                var TerminalPhones = from t in db.tblTerminalPhones
                                     where t.terminalID == terminalID
                                     select new
                                     {
                                         t.phone,
                                         t.label
                                     };

                List<FollowUpViewModel.LogItem> Logs = new List<FollowUpViewModel.LogItem>();

                var CallLogs = from c in db.tblCallLogs
                               join l in db.tblLeads on c.leadID equals l.leadID
                               into c_l
                               from l in c_l.DefaultIfEmpty()
                               join q in db.tblQualificationStatus on l.qualificationStatusID equals q.qualificationStatusID
                               into l_q
                               from q in l_q.DefaultIfEmpty()
                               join d in db.tblDestinations on l.interestedInDestinationID equals d.destinationID
                               into l_d
                               from d in l_d.DefaultIfEmpty()
                               join p in db.tblQuotations on c.leadID equals p.leadID
                               into l_p
                               from p in l_p.DefaultIfEmpty()
                               join cu in db.tblCurrencies on p.currencyID equals cu.currencyID
                               into p_cu
                               from cu in p_cu.DefaultIfEmpty()
                               join i in db.tblInteractions on c.interactionID equals i.interactionID
                               into c_i
                               from i in c_i.DefaultIfEmpty()
                               join il in db.tblInterestLevels on i.interestLevelID equals il.interestLevelID
                               into i_il
                               from il in i_il.DefaultIfEmpty()
                               join bs in db.tblBookingStatus on i.bookingStatusID equals bs.bookingStatusID
                               into i_bs
                               from bs in i_bs.DefaultIfEmpty()
                               join dr in db.tblDiscardReasons on i.discardReasonID equals dr.discardReasonID
                               into i_dr
                               from dr in i_dr.DefaultIfEmpty()
                               join up in db.tblUserProfiles on l.assignedToUserID equals up.userID
                               into l_up
                               from up in l_up.DefaultIfEmpty()
                               join e in db.tblLeadEmails on l.leadID equals e.leadID
                               into l_e
                               from e in l_e.DefaultIfEmpty()
                               where c.terminalID == terminalID
                               && c.callDateTime > date
                               && c.callDateTime < toDate
                               && (Extensions.Contains(c.agentExtension) || c.agentExtension == null)
                               select new
                               {
                                   c.callLogID,
                                   c.interactionID,
                                   c.callDateTime,
                                   c.phoneDialed,
                                   c.phoneSource,
                                   c.callIO,
                                   c.leadID,
                                   c.duration,
                                   c.disposition,
                                   c.agentExtension,
                                   l.firstName,
                                   l.lastName,
                                   l.qualificationStatusID,
                                   q.qualificationStatus,
                                   l.interestedInDestinationID,
                                   d.destination,
                                   p.total,
                                   p.currencyID,
                                   cu.currencyCode,
                                   c.firstContact,
                                   i.interactionComments,
                                   i.interestLevelID,
                                   il.interestLevel,
                                   i.bookingStatusID,
                                   bs.bookingStatus,
                                   i.discardReasonID,
                                   dr.discardReason,
                                   agent = up.firstName + " " + up.lastName,
                                   e.email
                               };

                var Emails = from e in db.tblLeadEmails
                             where CallLogs.Select(x => x.leadID).Contains(e.leadID)
                             select new
                             {
                                 e.leadID,
                                 e.email,
                                 e.emailID
                             };

                foreach (var call in CallLogs)
                {
                    FollowUpViewModel.LogItem log = new FollowUpViewModel.LogItem();
                    log.InteractionTypeID = 1;
                    log.CallLogID = call.callLogID;
                    log.InteractionID = call.interactionID;
                    log.CallDisposition = call.disposition;
                    log.CallDuration = (call.duration / 60 > 0 ? call.duration / 60 + " m " : "") + (call.duration % 60 > 0 ? call.duration % 60 + " s" : "");
                    if (TerminalPhones.FirstOrDefault(x => x.phone == call.phoneDialed) != null)
                    {
                        log.CallLabel = TerminalPhones.FirstOrDefault(x => x.phone == call.phoneDialed).label;
                    }
                    log.CallExtension = call.agentExtension;
                    if (call.callIO == true)
                    {
                        log.InteractionType += "Inbound Call";
                        log.Phone = call.phoneSource;
                    }
                    else
                    {
                        log.InteractionType = "Outbound Call";
                        log.Phone = call.phoneDialed;
                    }
                    log.Agent = call.agent;
                    log.Date = call.callDateTime;
                    log.LeadID = call.leadID;
                    log.FirstName = call.firstName;
                    log.LastName = call.lastName;
                    log.Email = call.email;
                    if (Emails.FirstOrDefault(x => x.email == call.email && call.leadID == call.leadID) != null)
                    {
                        log.EmailID = Emails.FirstOrDefault(x => x.email == call.email && call.leadID == call.leadID).emailID;
                    }
                    log.QualificationStatus = call.qualificationStatus;
                    log.QualificationStatusID = call.qualificationStatusID;
                    log.InterestedInDestination = call.destination;
                    log.InterestedInDestinationID = call.interestedInDestinationID;
                    log.Total = call.total;
                    log.CurrencyCode = call.currencyCode;
                    log.CurrencyID = call.currencyID;
                    log.FirstContact = call.firstContact;
                    log.InteractionComments = call.interactionComments;
                    log.InterestLevel = call.interestLevel;
                    log.InterestLevelID = call.interestLevelID;
                    log.BookingStatus = call.bookingStatus;
                    log.BookingStatusID = call.bookingStatusID;
                    log.DiscardReason = call.discardReason;
                    log.TerminalID = terminalID;
                    log.DiscardReasonID = call.discardReasonID;

                    Logs.Add(log);
                }

                return Logs;
            }
        }

        public class Reports
        {
            public static AgentsPerformanceViewModel.AgentsPerformanceResults GetAgentsPerformance(CallsByLocationViewModel.SearchCalls model)
            {
                ePlatEntities db = new ePlatEntities();
                ecommerceEntities edb = new ecommerceEntities();
                AgentsPerformanceViewModel.AgentsPerformanceResults results = new AgentsPerformanceViewModel.AgentsPerformanceResults();

                results.AgentsMoreThanForty = new List<AgentsPerformanceViewModel.AgentPerformance>();
                results.TotalMoreThanForty = new AgentsPerformanceViewModel.TotalPerformance();
                results.AgentsFirstContact = new List<AgentsPerformanceViewModel.AgentPerformance>();
                results.TotalFirstContact = new AgentsPerformanceViewModel.TotalPerformance();

                DateTime fromDate = DateTime.Parse(model.Search_I_FromDate);
                DateTime toDate = DateTime.Parse(model.Search_F_ToDate).AddDays(1);
                long? terminalID = model.Search_TerminalID;
                results.FromDate = model.Search_I_FromDate;
                results.ToDate = model.Search_F_ToDate;

                var PhonesCatalog = from p in db.tblTerminalPhones
                                    where p.terminalID == terminalID
                                    select p;

                int phonesLength = 0;
                string[] phones = new string[] { };
                if (model.Search_Phones != null)
                {
                    phones = model.Search_Phones.ToArray();
                    phonesLength = model.Search_Phones.Length;
                }
                else
                {
                    //All
                    phonesLength = PhonesCatalog.Count();
                    phones = PhonesCatalog.Select(x => x.phone).ToArray();
                }

                foreach (var phoneLabel in phones)
                {
                    if (results.PhoneLabels != "")
                    {
                        results.PhoneLabels += "<br />";
                    }
                    results.PhoneLabels += PhonesCatalog.FirstOrDefault(x => x.phone == phoneLabel).label + " : " + phoneLabel;
                }

                //Sales
                int?[] excludedStatus = new int?[] { 2, 6 }; // Cancelled, Quinela
                int?[] destinationIDS = new int?[] { 10, 2, 1, 3, 4, 5, 23 };

                int? ecommerceTerminalID = 0;
                int? ecommerceMarketingSourceID = 0;

                var Terminal = (from t in db.tblTerminals
                                where t.terminalID == model.Search_TerminalID
                                select new
                                {
                                    t.ecommerceMarketingSourceID,
                                    t.ecommerceTerminalID
                                }).FirstOrDefault();

                if (Terminal != null)
                {
                    ecommerceTerminalID = Terminal.ecommerceTerminalID;
                    ecommerceMarketingSourceID = Terminal.ecommerceMarketingSourceID;
                }

                //Sales
                var Sales = from reservation in edb.tbaReservaciones
                            join destination in edb.tbaDestinos on reservation.idDestino equals destination.idDestino
                            into reservationDestination
                            from destination in reservationDestination.DefaultIfEmpty()
                            join lead in edb.tbaProspectos on reservation.idProspecto equals lead.idProspecto
                            into reservationLead
                            from lead in reservationLead.DefaultIfEmpty()
                            join resort in edb.tbaLugares on reservation.idLugar equals resort.idLugar
                            into reservationResort
                            from resort in reservationResort.DefaultIfEmpty()
                            join status in edb.tbaEstatusReservaciones on reservation.idEstatusReservacion equals status.idEstatusReservacion
                            into reservationStatus
                            from status in reservationStatus.DefaultIfEmpty()
                            where reservation.fechaDeVenta >= fromDate
                            && reservation.fechaDeVenta < toDate
                            && (reservation.idEstatusReservacion == null || !excludedStatus.Contains(reservation.idEstatusReservacion))
                            && destinationIDS.Contains(reservation.idDestino)
                            && !reservation.certificateNumber.Contains(".1")
                            && lead.idMarketingSource == ecommerceMarketingSourceID
                            && lead.idTerminal == ecommerceTerminalID
                            orderby reservation.fechaDeVenta
                            select new
                            {
                                reservation.idReservacion,
                                reservation.fechaDeVenta,
                                reservation.certificateNumber,
                                reservation.llegada,
                                reservation.salida,
                                reservation.numeroConfirmacionHotel,
                                reservation.agenteDeVenta,
                                lead.phone,
                                lead.phone2,
                                lead.firstName,
                                lead.lastName,
                                destination.destino,
                                resort.lugar,
                                status.estatusReservacion
                            };

                foreach (var sale in Sales)
                {
                    if (results.AgentsFirstContact.Count(x => x.EcommerceUserID == sale.agenteDeVenta) == 0)
                    {
                        AgentsPerformanceViewModel.AgentPerformance agent = new AgentsPerformanceViewModel.AgentPerformance();

                        var AgentQ = (from a in db.tblUserProfiles
                                      where a.ecommerceUserID == sale.agenteDeVenta
                                      select a).FirstOrDefault();

                        if (AgentQ != null)
                        {
                            agent.UserID = AgentQ.userID;
                            agent.EcommerceUserID = AgentQ.ecommerceUserID;
                            agent.Agent = AgentQ.firstName + " " + AgentQ.lastName;
                            agent.Extension = agent.Extension;
                            agent.Calls = 0;
                            agent.Sales = 0;
                            agent.Closing = 0;
                            agent.TotalSales = 0;
                            agent.AverageSale = 0;
                        }

                        results.AgentsFirstContact.Add(agent);
                        results.AgentsMoreThanForty.Add(agent);
                    }

                    results.AgentsFirstContact.FirstOrDefault(x => x.EcommerceUserID == sale.agenteDeVenta).Sales++;
                    results.TotalFirstContact.Sales++;
                    results.AgentsMoreThanForty.FirstOrDefault(x => x.EcommerceUserID == sale.agenteDeVenta).Sales++;
                    results.TotalMoreThanForty.Sales++;
                }

                //Calls
                var CallsLog = from c in db.tblCallLogs
                               join code in db.tblAreaCodes on c.areaCodeID equals code.areaCodeID
                               into ccode
                               from code in ccode.DefaultIfEmpty()
                               join area in db.tblAreas on code.areaID equals area.areaID
                               into codearea
                               from area in codearea.DefaultIfEmpty()
                               join state in db.tblStates on area.stateID equals state.stateID
                               into areastate
                               from state in areastate.DefaultIfEmpty()
                               join destination in db.tblCallDestinationSettings on c.callDestinationID equals destination.callDestinationID
                               into calldestination
                               from destination in calldestination.DefaultIfEmpty()
                               join destinationname in db.tblDestinations on destination.destinationID equals destinationname.destinationID
                               into dindestinationname
                               from destinationname in dindestinationname.DefaultIfEmpty()
                               where c.callDateTime > fromDate
                               && c.callDateTime < toDate
                               && c.callIO == true
                               && (phonesLength == 0 || phones.Contains(c.phoneDialed))
                               && c.disposition == "ANSWERED"
                               select new CallsByLocationViewModel.CallsLogQuery()
                               {
                                   callLogID = c.callLogID,
                                   callDateTime = c.callDateTime,
                                   phoneSource = c.phoneSource,
                                   phoneDialed = c.phoneDialed,
                                   agentExtension = c.agentExtension,
                                   duration = c.duration,
                                   areaCodeID = c.areaCodeID,
                                   areaID = area.areaID,
                                   stateID = state.stateID,
                                   countryID = state.countryID,
                                   destinationID = destination.destinationID,
                                   destination = destinationname.destination
                               };

                foreach (var call in CallsLog)
                {
                    if (call.duration > 40)
                    {
                        if (results.AgentsFirstContact.Count(x => x.Extension == call.agentExtension) == 0)
                        {
                            AgentsPerformanceViewModel.AgentPerformance agent = new AgentsPerformanceViewModel.AgentPerformance();

                            var AgentQ = (from a in db.tblUserProfiles
                                          where a.phoneEXT == call.agentExtension
                                          select a).FirstOrDefault();

                            if (AgentQ != null)
                            {
                                agent.UserID = AgentQ.userID;
                                agent.EcommerceUserID = AgentQ.ecommerceUserID;
                                agent.Agent = AgentQ.firstName + " " + AgentQ.lastName;
                                agent.Extension = agent.Extension;
                                agent.Calls = 0;
                                agent.Sales = 0;
                                agent.Closing = 0;
                                agent.TotalSales = 0;
                                agent.AverageSale = 0;
                            }

                            results.AgentsFirstContact.Add(agent);
                            results.AgentsMoreThanForty.Add(agent);
                        }

                        //buscar si el número ya está considerado
                        //Guid? leadID = null;
                        //if (!NetCenterDataModel.CallsLog.IsFollowUp(call.callDateTime, call.phoneSource, leadID, model.Search_TerminalID))
                        //{
                        //    //no es follow up
                        //    results.AgentsFirstContact.FirstOrDefault(x => x.Extension == call.agentExtension).Calls++;
                        //    results.TotalFirstContact.Calls++;
                        //}

                        //results.AgentsMoreThanForty.FirstOrDefault(x => x.Extension == call.agentExtension).Calls++;
                        //results.TotalMoreThanForty.Calls++;
                    }
                }


                //porcentajes
                foreach (var agent in results.AgentsFirstContact)
                {
                    agent.Closing = decimal.Round((decimal)agent.Sales * 100 / (decimal)agent.Calls, 2);
                    agent.AverageSale = decimal.Round((decimal)agent.TotalSales / (decimal)agent.Sales, 2);
                }

                foreach (var agent in results.AgentsMoreThanForty)
                {
                    agent.Closing = decimal.Round((decimal)agent.Sales * 100 / (decimal)agent.Calls, 2);
                    agent.AverageSale = decimal.Round((decimal)agent.TotalSales / (decimal)agent.Sales, 2);
                }

                results.TotalFirstContact.Closing = decimal.Round((decimal)results.TotalFirstContact.Sales * 100 / (decimal)results.TotalFirstContact.Calls, 2);
                results.TotalFirstContact.AverageSale = decimal.Round((decimal)results.TotalFirstContact.TotalSales / (decimal)results.TotalFirstContact.Sales, 2);

                results.TotalMoreThanForty.Closing = decimal.Round((decimal)results.TotalMoreThanForty.Sales * 100 / (decimal)results.TotalMoreThanForty.Calls, 2);
                results.TotalMoreThanForty.AverageSale = decimal.Round((decimal)results.TotalMoreThanForty.TotalSales / (decimal)results.TotalMoreThanForty.Sales, 2);

                return results;
            }

            public static CallsByLocationViewModel.CallsByLocationResults GetCallsByLocation(CallsByLocationViewModel.SearchCalls model)
            {
                ePlatEntities db = new ePlatEntities();
                ecommerceEntities edb = new ecommerceEntities();

                CallsByLocationViewModel.CallsByLocationResults results = new CallsByLocationViewModel.CallsByLocationResults();

                results.TotalRecords = 0;
                results.Duration = new CallsByLocationViewModel.DurationMetrics();
                results.ExtensionCalls = new List<CallsByLocationViewModel.ExtensionCalls>();
                results.LabelCalls = new List<CallsByLocationViewModel.LabelCalls>();
                results.DestinationBookings = new List<CallsByLocationViewModel.DestinationBookings>();
                results.Sales = new List<CallsByLocationViewModel.CallSale>();
                results.CallsByCountry = new List<CallsByLocationViewModel.CallByCountry>();

                DateTime fromDate = DateTime.Parse(model.Search_I_FromDate);
                DateTime toDate = DateTime.Parse(model.Search_F_ToDate).AddDays(1);
                long? terminalID = model.Search_TerminalID;
                results.FromDate = model.Search_I_FromDate;
                results.ToDate = model.Search_F_ToDate;

                var PhonesCatalog = from p in db.tblTerminalPhones
                                    where p.terminalID == terminalID
                                    select p;

                int phonesLength = 0;
                string[] phones = new string[] { };
                if (model.Search_Phones != null)
                {
                    phones = model.Search_Phones.ToArray();
                    phonesLength = model.Search_Phones.Length;
                }
                else
                {
                    //All
                    phonesLength = PhonesCatalog.Count();
                    phones = PhonesCatalog.Select(x => x.phone).ToArray();
                }

                foreach (var phoneLabel in phones)
                {
                    if (results.PhoneLabels != "")
                    {
                        results.PhoneLabels += "<br />";
                    }
                    results.PhoneLabels += PhonesCatalog.FirstOrDefault(x => x.phone == phoneLabel).label + " : " + phoneLabel;
                }

                List<tblCountries> CountriesCatalog = (from c in db.tblCountries
                                                       select c).Take(3).ToList();

                List<tblStates> StatesCatalog = (from s in db.tblStates
                                                 select s).ToList();

                List<tblAreas> AreasCatalog = (from a in db.tblAreas
                                               select a).ToList();

                List<tblAreaCodes> CodesCatalog = (from c in db.tblAreaCodes
                                                   select c).ToList();

                //Sales
                int?[] excludedStatus = new int?[] { 2, 6 }; // Cancelled, Quinela
                int?[] destinationIDS = new int?[] { 10, 2, 1, 3, 4, 5, 23 };

                int? ecommerceTerminalID = 0;
                int? ecommerceMarketingSourceID = 0;

                var Terminal = (from t in db.tblTerminals
                                where t.terminalID == model.Search_TerminalID
                                select new
                                {
                                    t.ecommerceMarketingSourceID,
                                    t.ecommerceTerminalID
                                }).FirstOrDefault();

                if (Terminal != null)
                {
                    ecommerceTerminalID = Terminal.ecommerceTerminalID;
                    ecommerceMarketingSourceID = Terminal.ecommerceMarketingSourceID;
                }

                var Sales = from reservation in edb.tbaReservaciones
                            join destination in edb.tbaDestinos on reservation.idDestino equals destination.idDestino
                            into reservationDestination
                            from destination in reservationDestination.DefaultIfEmpty()
                            join lead in edb.tbaProspectos on reservation.idProspecto equals lead.idProspecto
                            into reservationLead
                            from lead in reservationLead.DefaultIfEmpty()
                            join resort in edb.tbaLugares on reservation.idLugar equals resort.idLugar
                            into reservationResort
                            from resort in reservationResort.DefaultIfEmpty()
                            join status in edb.tbaEstatusReservaciones on reservation.idEstatusReservacion equals status.idEstatusReservacion
                            into reservationStatus
                            from status in reservationStatus.DefaultIfEmpty()
                            where reservation.fechaDeVenta >= fromDate
                            && reservation.fechaDeVenta < toDate
                            && (reservation.idEstatusReservacion == null || !excludedStatus.Contains(reservation.idEstatusReservacion))
                            && destinationIDS.Contains(reservation.idDestino)
                            && !reservation.certificateNumber.Contains(".1")
                            && lead.idMarketingSource == ecommerceMarketingSourceID
                            && lead.idTerminal == ecommerceTerminalID
                            orderby reservation.fechaDeVenta
                            select new
                            {
                                reservation.idReservacion,
                                reservation.fechaDeVenta,
                                reservation.certificateNumber,
                                reservation.llegada,
                                reservation.salida,
                                reservation.numeroConfirmacionHotel,
                                lead.phone,
                                lead.phone2,
                                lead.firstName,
                                lead.lastName,
                                destination.destino,
                                resort.lugar,
                                status.estatusReservacion
                            };

                foreach (var sale in Sales)
                {
                    CallsByLocationViewModel.CallSale newSale = new CallsByLocationViewModel.CallSale();
                    newSale.ReservationID = sale.idReservacion;
                    newSale.SaleDate = sale.fechaDeVenta;
                    newSale.Certificate = sale.certificateNumber;
                    newSale.Phone1 = sale.phone;
                    newSale.Phone2 = sale.phone2;
                    newSale.FirstName = sale.firstName;
                    newSale.LastName = sale.lastName;
                    newSale.Arrival = (sale.llegada != null ? sale.llegada.Value.ToString("yyyy-MM-dd") : "");
                    newSale.Departure = (sale.salida != null ? sale.salida.Value.ToString("yyyy-MM-dd") : "");
                    newSale.Destination = sale.destino;
                    newSale.Resort = sale.lugar;
                    newSale.Confirmation = sale.numeroConfirmacionHotel;
                    newSale.Status = sale.estatusReservacion;
                    newSale.Matched = false;

                    //clasificar location
                    if (newSale.Phone1.Length >= 10)
                    {
                        string saleCode = newSale.Phone1.Substring(0, 3);
                        if (CodesCatalog.Count(x => x.code == saleCode) > 0)
                        {
                            newSale.AreaCodeID = CodesCatalog.FirstOrDefault(x => x.code == saleCode).areaCodeID;
                            newSale.Code = CodesCatalog.FirstOrDefault(x => x.code == saleCode).code;
                            newSale.AreaID = CodesCatalog.FirstOrDefault(x => x.code == saleCode).areaID;
                            newSale.Area = AreasCatalog.FirstOrDefault(x => x.areaID == newSale.AreaID).area;
                            newSale.StateID = AreasCatalog.FirstOrDefault(x => x.areaID == newSale.AreaID).stateID;
                            newSale.State = StatesCatalog.FirstOrDefault(x => x.stateID == newSale.StateID).state;
                            newSale.CountryID = StatesCatalog.FirstOrDefault(x => x.stateID == newSale.StateID).countryID;
                            newSale.Country = CountriesCatalog.FirstOrDefault(x => x.countryID == newSale.CountryID).country;
                        }
                    }

                    results.Sales.Add(newSale);

                    //Destination Bookings
                    if (results.DestinationBookings.Count(x => x.Destination == sale.destino) == 0)
                    {
                        CallsByLocationViewModel.DestinationBookings destination = new CallsByLocationViewModel.DestinationBookings();
                        destination.Destination = sale.destino;
                        destination.DestinationInitials = Utils.GeneralFunctions.GetInitials(sale.destino);
                        destination.Calls = 0;
                        destination.Bookings = 0;
                        results.DestinationBookings.Add(destination);
                    }
                    results.DestinationBookings.FirstOrDefault(x => x.Destination == sale.destino).Bookings++;
                }

                //Calls
                var CallsLog = from c in db.tblCallLogs
                               join code in db.tblAreaCodes on c.areaCodeID equals code.areaCodeID
                               into ccode
                               from code in ccode.DefaultIfEmpty()
                               join area in db.tblAreas on code.areaID equals area.areaID
                               into codearea
                               from area in codearea.DefaultIfEmpty()
                               join state in db.tblStates on area.stateID equals state.stateID
                               into areastate
                               from state in areastate.DefaultIfEmpty()
                               join destination in db.tblCallDestinationSettings on c.callDestinationID equals destination.callDestinationID
                               into calldestination
                               from destination in calldestination.DefaultIfEmpty()
                               join destinationname in db.tblDestinations on destination.destinationID equals destinationname.destinationID
                               into dindestinationname
                               from destinationname in dindestinationname.DefaultIfEmpty()
                               where c.callDateTime > fromDate
                               && c.callDateTime < toDate
                               && c.callIO == true
                               && (phonesLength == 0 || phones.Contains(c.phoneDialed))
                               && c.disposition == "ANSWERED"
                               select new CallsByLocationViewModel.CallsLogQuery()
                               {
                                   callLogID = c.callLogID,
                                   callDateTime = c.callDateTime,
                                   phoneSource = c.phoneSource,
                                   phoneDialed = c.phoneDialed,
                                   agentExtension = c.agentExtension,
                                   duration = c.duration,
                                   areaCodeID = c.areaCodeID,
                                   areaID = area.areaID,
                                   stateID = state.stateID,
                                   countryID = state.countryID,
                                   destinationID = destination.destinationID,
                                   destination = destinationname.destination
                               };

                foreach (var call in CallsLog)
                {
                    results.TotalRecords++;
                    if (call.duration > 40)
                    {
                        call.destination = (call.destination != null ? call.destination : "[Open]");

                        //Calls by Location
                        results.CallsByCountry = GenerateCodesStructure(results.CallsByCountry, CountriesCatalog, StatesCatalog, AreasCatalog, CodesCatalog, call.countryID, call.stateID, call.areaID, call.areaCodeID, call.destination);

                        //buscar si el número ya está considerado
                        Guid? leadID = new Guid();
                        bool IsFollowUp = NetCenterDataModel.CallsLog.IsFollowUp(call.callDateTime, call.phoneSource, ref leadID, model.Search_TerminalID);

                        if (!IsFollowUp || call.phoneSource.Length < 10)
                        {
                            //no es follow up
                            //Duration
                            results.Duration.MoreThanForty++;

                            //Extension calls
                            if (results.ExtensionCalls.Count(x => x.Extension == call.agentExtension) == 0)
                            {
                                CallsByLocationViewModel.ExtensionCalls extension = new CallsByLocationViewModel.ExtensionCalls();
                                extension.Extension = call.agentExtension;
                                extension.TotalCalls = 0;
                                results.ExtensionCalls.Add(extension);
                            }

                            results.ExtensionCalls.FirstOrDefault(x => x.Extension == call.agentExtension).TotalCalls++;

                            //Label calls
                            if (results.LabelCalls.Count(x => x.Phone == call.phoneDialed) == 0)
                            {
                                CallsByLocationViewModel.LabelCalls label = new CallsByLocationViewModel.LabelCalls();
                                label.Phone = call.phoneDialed;
                                label.Label = PhonesCatalog.FirstOrDefault(x => x.phone == call.phoneDialed).label;
                                label.TotalCalls = 0;
                                results.LabelCalls.Add(label);
                            }

                            results.LabelCalls.FirstOrDefault(x => x.Phone == call.phoneDialed).TotalCalls++;
                        }
                        else
                        {
                            //si es follow up
                            //Duration
                            results.Duration.Duplicated++;
                        }

                        CallsByLocationViewModel.CallByCode currentCode =
                        results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).States.FirstOrDefault(x => x.StateID == call.stateID).Areas.FirstOrDefault(x => x.AreaID == call.areaID).Codes.FirstOrDefault(x => x.AreaCodeID == call.areaCodeID);

                        CallsByLocationViewModel.Call currentCall = new CallsByLocationViewModel.Call();
                        currentCall.CallDate = call.callDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        currentCall.Source = call.phoneSource;
                        currentCall.Dialed = call.phoneDialed;
                        currentCall.Extension = call.agentExtension;
                        currentCall.DuracionSecs = call.duration;
                        currentCall.Duration = Utils.GeneralFunctions.DateFormat.SecondsToTime((int)call.duration);
                        currentCall.Label = PhonesCatalog.FirstOrDefault(x => x.phone == call.phoneDialed).label;
                        currentCall.Duplicated = (IsFollowUp && call.phoneSource.Length == 10 ? true : false);
                        currentCall.Destination = call.destination;
                        currentCode.Calls.Add(currentCall);

                        ////////////////////
                        if (!IsFollowUp || call.phoneSource.Length < 10)
                        {
                            //agregar calls por destino
                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).States.FirstOrDefault(x => x.StateID == call.stateID).Areas.FirstOrDefault(x => x.AreaID == call.areaID).Codes.FirstOrDefault(x => x.AreaCodeID == call.areaCodeID).DestinationBookings.FirstOrDefault(x => x.Destination == call.destination).Calls++;

                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).States.FirstOrDefault(x => x.StateID == call.stateID).Areas.FirstOrDefault(x => x.AreaID == call.areaID).DestinationBookings.FirstOrDefault(x => x.Destination == call.destination).Calls++;

                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).States.FirstOrDefault(x => x.StateID == call.stateID).DestinationBookings.FirstOrDefault(x => x.Destination == call.destination).Calls++;

                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).DestinationBookings.FirstOrDefault(x => x.Destination == call.destination).Calls++;

                            if (results.DestinationBookings.Count(x => x.Destination == call.destination) == 0)
                            {
                                CallsByLocationViewModel.DestinationBookings destination = new CallsByLocationViewModel.DestinationBookings();
                                destination.Destination = call.destination;
                                destination.DestinationInitials = Utils.GeneralFunctions.GetInitials(call.destination);
                                destination.Calls = 0;
                                destination.Bookings = 0;
                                results.DestinationBookings.Add(destination);
                            }
                            results.DestinationBookings.FirstOrDefault(x => x.Destination == call.destination).Calls++;

                            //agregar a total de calls
                            currentCode.TotalCodeCalls++;

                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).States.FirstOrDefault(x => x.StateID == call.stateID).Areas.FirstOrDefault(x => x.AreaID == call.areaID).TotalAreaCalls++;

                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).States.FirstOrDefault(x => x.StateID == call.stateID).TotalStateCalls++;

                            results.CallsByCountry.FirstOrDefault(x => x.CountryID == call.countryID).TotalCountryCalls++;

                            //buscar relacionar con venta
                            var RelatedSale = results.Sales.FirstOrDefault(x => (x.Phone1 == currentCall.Source || x.Phone2 == currentCall.Source) && x.Matched == false);

                            if (RelatedSale != null)
                            {
                                RelatedSale.Matched = true;
                                currentCall.Booking = true;
                            }
                        }
                    }
                    else
                    {
                        Guid? leadID = new Guid?();
                        bool IsFollowUp = NetCenterDataModel.CallsLog.IsFollowUp(call.callDateTime, call.phoneSource, ref leadID, model.Search_TerminalID, 3, false);
                        if (!IsFollowUp)
                        {
                            results.Duration.LessThanForty++;
                        }
                        else
                        {
                            results.Duration.LessThanFortyDuplicated++;
                        }
                    }
                }


                //relacionar ventas por código
                foreach (var sale in results.Sales)
                {
                    results.CallsByCountry = GenerateCodesStructure(results.CallsByCountry, CountriesCatalog, StatesCatalog, AreasCatalog, CodesCatalog, sale.CountryID, sale.StateID, sale.AreaID, sale.AreaCodeID, sale.Destination);

                    //agregar a total de ventas
                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).States.FirstOrDefault(x => x.StateID == sale.StateID).Areas.FirstOrDefault(x => x.AreaID == sale.AreaID).Codes.FirstOrDefault(x => x.AreaCodeID == sale.AreaCodeID).TotalCodeSales++;

                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).States.FirstOrDefault(x => x.StateID == sale.StateID).Areas.FirstOrDefault(x => x.AreaID == sale.AreaID).TotalAreaSales++;

                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).States.FirstOrDefault(x => x.StateID == sale.StateID).TotalStateSales++;

                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).TotalCountrySales++;

                    //agregar bookings por destino
                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).States.FirstOrDefault(x => x.StateID == sale.StateID).Areas.FirstOrDefault(x => x.AreaID == sale.AreaID).Codes.FirstOrDefault(x => x.AreaCodeID == sale.AreaCodeID).DestinationBookings.FirstOrDefault(x => x.Destination == sale.Destination).Bookings++;

                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).States.FirstOrDefault(x => x.StateID == sale.StateID).Areas.FirstOrDefault(x => x.AreaID == sale.AreaID).DestinationBookings.FirstOrDefault(x => x.Destination == sale.Destination).Bookings++;

                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).States.FirstOrDefault(x => x.StateID == sale.StateID).DestinationBookings.FirstOrDefault(x => x.Destination == sale.Destination).Bookings++;

                    results.CallsByCountry.FirstOrDefault(x => x.CountryID == sale.CountryID).DestinationBookings.FirstOrDefault(x => x.Destination == sale.Destination).Bookings++;
                }


                //orden, porcentajes y span
                if (results.Duration.MoreThanForty > 0)
                {
                    results.DestinationBookings = results.DestinationBookings.OrderByDescending(x => x.Bookings).ToList();

                    foreach (var dest in results.DestinationBookings)
                    {
                        if (dest.Calls > 0)
                        {
                            dest.DestinationPercentage = decimal.Round((decimal)dest.Bookings * 100 / (decimal)dest.Calls, 2, MidpointRounding.AwayFromZero);
                        }
                    }

                    results.BookingRate = decimal.Round((decimal)results.Sales.Count() * 100 / (decimal)results.Duration.MoreThanForty, 2, MidpointRounding.AwayFromZero);

                    results.CallsByCountry = results.CallsByCountry.OrderByDescending(x => x.TotalCountryCalls).ToList();

                    foreach (var country in results.CallsByCountry)
                    {
                        country.States = country.States.OrderByDescending(x => x.TotalStateCalls).ToList();
                        country.PercentageCountryCalls = decimal.Round((decimal)country.TotalCountryCalls * 100 / (decimal)results.Duration.MoreThanForty, 2, MidpointRounding.AwayFromZero);

                        if (country.TotalCountryCalls > 0)
                        {
                            country.BookingRate = decimal.Round((decimal)country.TotalCountrySales * 100 / (decimal)country.TotalCountryCalls, 2, MidpointRounding.AwayFromZero);
                            foreach (var dest in country.DestinationBookings)
                            {
                                if (dest.Calls > 0)
                                {
                                    dest.DestinationPercentage = decimal.Round((decimal)dest.Bookings * 100 / (decimal)dest.Calls);
                                }
                            }
                        }
                        foreach (var state in country.States)
                        {
                            state.Areas = state.Areas.OrderByDescending(x => x.TotalAreaCalls).ToList();
                            state.PercentageStateCalls = decimal.Round((decimal)state.TotalStateCalls * 100 / (decimal)results.Duration.MoreThanForty, 2, MidpointRounding.AwayFromZero);

                            if (country.TotalCountryCalls > 0)
                            {
                                state.PercentageCountryCalls = decimal.Round((decimal)state.TotalStateCalls * 100 / (decimal)country.TotalCountryCalls, 2, MidpointRounding.AwayFromZero);
                            }
                            if (state.TotalStateCalls > 0)
                            {
                                state.BookingRate = decimal.Round((decimal)state.TotalStateSales * 100 / (decimal)state.TotalStateCalls, 2, MidpointRounding.AwayFromZero);
                                foreach (var dest in state.DestinationBookings)
                                {
                                    if (dest.Calls > 0)
                                    {
                                        dest.DestinationPercentage = decimal.Round((decimal)dest.Bookings * 100 / (decimal)dest.Calls);
                                    }
                                }
                            }
                            foreach (var area in state.Areas)
                            {
                                area.Codes = area.Codes.OrderByDescending(x => x.TotalCodeCalls).ToList();
                                area.PercentageAreaCalls = decimal.Round((decimal)area.TotalAreaCalls * 100 / (decimal)results.Duration.MoreThanForty, 2, MidpointRounding.AwayFromZero);
                                if (state.TotalStateCalls > 0)
                                {
                                    area.PercentageStateCalls = decimal.Round((decimal)area.TotalAreaCalls * 100 / (decimal)state.TotalStateCalls, 2, MidpointRounding.AwayFromZero);
                                }
                                if (area.TotalAreaCalls > 0)
                                {
                                    area.BookingRate = decimal.Round((decimal)area.TotalAreaSales * 100 / (decimal)area.TotalAreaCalls, 2, MidpointRounding.AwayFromZero);
                                    foreach (var dest in area.DestinationBookings)
                                    {
                                        if (dest.Calls > 0)
                                        {
                                            dest.DestinationPercentage = decimal.Round((decimal)dest.Bookings * 100 / (decimal)dest.Calls);
                                        }
                                    }
                                }
                                foreach (var code in area.Codes)
                                {
                                    code.PercentageCodeCalls = decimal.Round((decimal)code.TotalCodeCalls * 100 / (decimal)results.Duration.MoreThanForty, 2, MidpointRounding.AwayFromZero);
                                    if (area.TotalAreaCalls > 0)
                                    {
                                        code.PercentageAreaCalls = decimal.Round((decimal)code.TotalCodeCalls * 100 / (decimal)area.TotalAreaCalls, 2, MidpointRounding.AwayFromZero);
                                    }
                                    if (code.TotalCodeCalls > 0)
                                    {
                                        code.BookingRate = decimal.Round((decimal)code.TotalCodeSales * 100 / (decimal)code.TotalCodeCalls, 2, MidpointRounding.AwayFromZero);
                                        foreach (var dest in code.DestinationBookings)
                                        {
                                            if (dest.Calls > 0)
                                            {
                                                dest.DestinationPercentage = decimal.Round((decimal)dest.Bookings * 100 / (decimal)dest.Calls);
                                            }
                                        }
                                    }

                                    country.Span++;
                                    state.Span++;
                                    area.Span++;
                                }
                            }
                        }
                    }
                }

                return results;
            }

            public static List<CallsByLocationViewModel.CallByCountry> GenerateCodesStructure(List<CallsByLocationViewModel.CallByCountry> CallsByCountry, List<tblCountries> CountriesCatalog, List<tblStates> StatesCatalog, List<tblAreas> AreasCatalog, List<tblAreaCodes> CodesCatalog, int? countryID, int? stateID, int? areaID, int? areaCodeID, string destination)
            {
                if (CallsByCountry.Count(x => x.CountryID == countryID) == 0)
                {
                    //no existe el pais en la lista
                    CallsByLocationViewModel.CallByCountry country = new CallsByLocationViewModel.CallByCountry();
                    country.CountryID = countryID;
                    if (CountriesCatalog.Count(x => x.countryID == countryID) > 0)
                    {
                        country.Country = CountriesCatalog.FirstOrDefault(x => x.countryID == countryID).country;
                    }
                    else
                    {
                        country.Country = "Undefined";
                    }
                    country.States = new List<CallsByLocationViewModel.CallByState>();
                    country.TotalCountryCalls = 0;
                    country.PercentageCountryCalls = 0;
                    country.DestinationBookings = new List<CallsByLocationViewModel.DestinationBookings>();
                    CallsByCountry.Add(country);
                }


                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.Count(x => x.StateID == stateID) == 0)
                {
                    //no existe el estado
                    CallsByLocationViewModel.CallByState state = new CallsByLocationViewModel.CallByState();
                    state.StateID = stateID;
                    if (StatesCatalog.Count(x => x.stateID == stateID) > 0)
                    {
                        state.State = StatesCatalog.FirstOrDefault(x => x.stateID == stateID).state;
                        state.StateCode = StatesCatalog.FirstOrDefault(x => x.stateID == stateID).stateCode;
                    }
                    else
                    {
                        state.State = "Undefined";
                        state.StateCode = "XX";
                    }
                    state.Areas = new List<CallsByLocationViewModel.CallByArea>();
                    state.TotalStateCalls = 0;
                    state.PercentageStateCalls = 0;
                    state.PercentageCountryCalls = 0;
                    state.DestinationBookings = new List<CallsByLocationViewModel.DestinationBookings>();
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.Add(state);
                }

                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.Count(x => x.AreaID == areaID) == 0)
                {
                    //no existe el area
                    CallsByLocationViewModel.CallByArea area = new CallsByLocationViewModel.CallByArea();
                    area.AreaID = areaID;
                    if (AreasCatalog.Count(x => x.areaID == areaID) > 0)
                    {
                        area.Area = AreasCatalog.FirstOrDefault(x => x.areaID == areaID).area;
                    }
                    else
                    {
                        area.Area = "Undefined";
                    }
                    area.Codes = new List<CallsByLocationViewModel.CallByCode>();
                    area.TotalAreaCalls = 0;
                    area.PercentageAreaCalls = 0;
                    area.PercentageStateCalls = 0;
                    area.DestinationBookings = new List<CallsByLocationViewModel.DestinationBookings>();
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.Add(area);
                }

                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.FirstOrDefault(x => x.AreaID == areaID).Codes.Count(x => x.AreaCodeID == areaCodeID) == 0)
                {
                    CallsByLocationViewModel.CallByCode code = new CallsByLocationViewModel.CallByCode();
                    code.AreaCodeID = areaCodeID;
                    if (CodesCatalog.Count(x => x.areaCodeID == areaCodeID) > 0)
                    {
                        code.Code = CodesCatalog.FirstOrDefault(x => x.areaCodeID == areaCodeID).code;
                        code.Places = CodesCatalog.FirstOrDefault(x => x.areaCodeID == areaCodeID).places;
                    }
                    else
                    {
                        code.Code = "000";
                        code.Places = "";
                    }
                    code.Calls = new List<CallsByLocationViewModel.Call>();
                    code.TotalCodeCalls = 0;
                    code.PercentageCodeCalls = 0;
                    code.PercentageAreaCalls = 0;
                    code.DestinationBookings = new List<CallsByLocationViewModel.DestinationBookings>();
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.FirstOrDefault(x => x.AreaID == areaID).Codes.Add(code);
                }

                //bookings
                //bookings by code by destination
                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.FirstOrDefault(x => x.AreaID == areaID).Codes.FirstOrDefault(x => x.AreaCodeID == areaCodeID).DestinationBookings.Count(x => x.Destination == destination) == 0)
                {
                    CallsByLocationViewModel.DestinationBookings newDestination = new CallsByLocationViewModel.DestinationBookings();
                    newDestination.Destination = destination;
                    newDestination.DestinationInitials = Utils.GeneralFunctions.GetInitials(destination);
                    newDestination.Calls = 0;
                    newDestination.Bookings = 0;
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.FirstOrDefault(x => x.AreaID == areaID).Codes.FirstOrDefault(x => x.AreaCodeID == areaCodeID).DestinationBookings.Add(newDestination);
                }

                //bookings by area by destination
                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.FirstOrDefault(x => x.AreaID == areaID).DestinationBookings.Count(x => x.Destination == destination) == 0)
                {
                    CallsByLocationViewModel.DestinationBookings newDestination = new CallsByLocationViewModel.DestinationBookings();
                    newDestination.Destination = destination;
                    newDestination.DestinationInitials = Utils.GeneralFunctions.GetInitials(destination);
                    newDestination.Calls = 0;
                    newDestination.Bookings = 0;
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).Areas.FirstOrDefault(x => x.AreaID == areaID).DestinationBookings.Add(newDestination);
                }

                //bookings by state by destination
                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).DestinationBookings.Count(x => x.Destination == destination) == 0)
                {
                    CallsByLocationViewModel.DestinationBookings newDestination = new CallsByLocationViewModel.DestinationBookings();
                    newDestination.Destination = destination;
                    newDestination.DestinationInitials = Utils.GeneralFunctions.GetInitials(destination);
                    newDestination.Calls = 0;
                    newDestination.Bookings = 0;
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).States.FirstOrDefault(x => x.StateID == stateID).DestinationBookings.Add(newDestination);
                }

                //bookings by country by destination
                if (CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).DestinationBookings.Count(x => x.Destination == destination) == 0)
                {
                    CallsByLocationViewModel.DestinationBookings newDestination = new CallsByLocationViewModel.DestinationBookings();
                    newDestination.Destination = destination;
                    newDestination.DestinationInitials = Utils.GeneralFunctions.GetInitials(destination);
                    newDestination.Calls = 0;
                    newDestination.Bookings = 0;
                    CallsByCountry.FirstOrDefault(x => x.CountryID == countryID).DestinationBookings.Add(newDestination);
                }

                return CallsByCountry;
            }
        }

        public static Guid? GetUserForAssignation(int? leadTypeID, long terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            Guid? userID = null;

            var AssignationSettings = (from a in db.tblAssignationSettings
                                       where a.leadTypeID == leadTypeID
                                       && a.terminalID == terminalID
                                       && a.distribute == true
                                       && a.active == true
                                       select a).FirstOrDefault();

            if (AssignationSettings != null)
            {

                //asignar automáticamente los leads a los agentes
                //obtener los roles que reciben leads
                List<Guid> roles = new List<Guid>();
                if(AssignationSettings.includeAgent && AssignationSettings.agentRoleID != null)
                {
                    roles.Add((Guid)AssignationSettings.agentRoleID);
                }
                if(AssignationSettings.includeCloser && AssignationSettings.closerRoleID != null)
                {
                    roles.Add((Guid)AssignationSettings.closerRoleID);
                }
                if (AssignationSettings.includeSupervisor && AssignationSettings.supervisorRoleID != null )
                {
                    roles.Add((Guid)AssignationSettings.supervisorRoleID);
                }
                if (AssignationSettings.includeAssistant && AssignationSettings.salesAssistantRoleID != null)
                {
                    roles.Add((Guid)AssignationSettings.salesAssistantRoleID);
                }
                if(AssignationSettings.includeSalesManager && AssignationSettings.salesManagerRoleID != null)
                {
                    roles.Add((Guid)AssignationSettings.salesManagerRoleID);
                }

                //obtener supervisores que no desean que su equipo reciba leads distribuidos automáticamente
                List<Guid> ExcludedSupervisors = new List<Guid>();
                if(AssignationSettings.excludeUsersSupervisedBy != null && AssignationSettings.excludeUsersSupervisedBy != "")
                {
                    ExcludedSupervisors = AssignationSettings.excludeUsersSupervisedBy.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                }

                //obtener personal de supervisores a excluir
                var ExcludedRepsQ = (from x in db.tblSupervisors_Agents
                                     join u in db.aspnet_Membership
                                     on x.agentUserID equals u.UserId
                                    where ExcludedSupervisors.Contains((Guid)x.supervisorUserID)
                                    && u.IsLockedOut == false
                                    select new
                                    {
                                        x.supervisorUserID,
                                        x.agentUserID
                                    }).ToList();

                var ExcludedRepsIDs = ExcludedRepsQ.Select(x => (Guid)x.agentUserID).ToList();

                //obtener la lista de usuarios a considerar para la distribución
                var usersInTerminal = (from t in db.tblUsers_Terminals
                                       where t.terminalID == terminalID
                                       select t.userID).ToList();

                var UserIDs = (from u in db.tblUsers_SysWorkGroups
                               join p in db.tblUserProfiles
                               on u.userID equals p.userID
                               join m in db.aspnet_Membership
                               on u.userID equals m.UserId
                               where roles.Contains(u.roleID)
                               && u.sysWorkGroupTeamID == AssignationSettings.sysWorkGroupTeamID
                               && m.IsLockedOut == false
                               && usersInTerminal.Contains(u.userID)
                               orderby p.phoneEXT
                               select u.userID).ToList();

                //contar asignaciones a partir de la fecha definida en la configuración ordenando por número de asignaciones y luego por extensión
                /*var Assignations = from l in db.tblLeads
                                   where l.terminalID == terminalID
                                   && l.inputDateTime > AssignationSettings.dateReset
                                   && l.leadTypeID == leadTypeID
                                   group l by new
                                   {
                                       l.assignedToUserID
                                   } into g
                                   select new
                                   {
                                       userID = g.Key.assignedToUserID,
                                       assignations = g.Count()
                                   };*/

                var ePlatUser = new Guid("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                var Assignations = from a in db.tblLeadAssignations
                                   join l in db.tblLeads
                                   on a.leadID equals l.leadID
                                   where a.assignedByUserID == ePlatUser
                                   && a.dateSaved > AssignationSettings.dateReset
                                   && l.initialTerminalID == terminalID
                                   && l.leadTypeID == leadTypeID
                                   group a by new
                                   {
                                       a.assignedToUserID
                                   } into g
                                   select new
                                   {
                                       userID = g.Key.assignedToUserID,
                                       assignations = g.Count()
                                   };

                int assignations = 0;
                foreach (Guid uid in UserIDs)
                {
                    if (!ExcludedRepsIDs.Contains(uid))
                    {
                        int cassignations = 0;
                        var currentUserAssignations = Assignations.FirstOrDefault(x => x.userID == uid);
                        if (currentUserAssignations != null)
                        {
                            cassignations = currentUserAssignations.assignations;
                            if (ExcludedSupervisors.Contains(uid))
                            {
                                cassignations /= ExcludedRepsQ.Count(x => x.supervisorUserID == uid) + 1;
                            }
                        }
                        if ((assignations == 0 && userID == null) || cassignations < assignations)
                        {
                            userID = uid;
                            assignations = cassignations;
                        }
                    }
                }
            } else
            {
                //informar que no se encontró la regla

            }
            return userID;
        }
    }
}
