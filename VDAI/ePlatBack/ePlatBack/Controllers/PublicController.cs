using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models;
using System.IO;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
//using System.Web.Http;


namespace ePlatBack.Controllers
{
    [AllowAnonymous]
    public class PublicController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentConfirmation()
        {

            return View();
        }

        //public JsonResult GetPaymentConfirmationInfo(string transaction)
        //{
        //    PublicDataModel pdm = new PublicDataModel();
        //    transaction = transaction.Replace("#", "");
        //    return Json(pdm.GetPurchaseConfirmationInfo(Guid.Parse(transaction)), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetPaymentConfirmationInfo(Guid? transaction)
        {
            PublicDataModel pdm = new PublicDataModel();

            return Json(pdm.GetPurchaseConfirmationInfo(transaction), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSignature(Guid transaction)
        {
            var ip = HttpContext.Request.UserHostAddress;
            PublicDataModel pdm = new PublicDataModel();
            return Json(pdm.GetSignature(transaction, ip), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetSignature(Guid transaction, string signature)
        {
            PublicDataModel pdm = new PublicDataModel();
            return Json(pdm.SetSignature(transaction, signature), JsonRequestBehavior.AllowGet);
        }

        public ViewResult PreviewEmail(object model)
        {
            return View(model);
        }

        public AttemptResponse CompleteSalesRoomsSales(string terminals)
        {
            var request = HttpContext.Request;
            PublicDataModel pdm = new PublicDataModel();
            ePlatEntities db = new ePlatEntities();
            AttemptResponse attempt = new AttemptResponse();
            NotificationsModel.SearchNotificationsModel model = new NotificationsModel.SearchNotificationsModel();
            var _terminals = terminals.Split(',').ToArray();

            var masterEmail = new System.Net.Mail.MailMessage();
            masterEmail.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
            masterEmail.To.Add("efalcon@villagroup.com");
            masterEmail.Subject = "Issues on VLO Notifications Report";
            masterEmail.IsBodyHtml = true;
            var masterEmailBody = "<br />Parameters<br />Terminals: " + terminals;
            try
            {
                var today = DateTime.Today;
                var _iDate = today;
                var counter = 0;
                string content = "";

                do
                {
                    if (_iDate.DayOfWeek == DayOfWeek.Friday)
                    {
                        if ((today != _iDate || today.DayOfWeek != DayOfWeek.Friday) && (today - _iDate).TotalDays >= 7)
                            counter++;
                    }
                    _iDate = _iDate.AddDays(-1);
                }
                while (counter <= 1);
                model.SearchNotifications_I_Date = _iDate.AddDays(1).ToString("yyyy-MM-dd");
                model.SearchNotifications_F_Date = _iDate.AddDays(7).ToString("yyyy-MM-dd");

                var proceed = true;
                foreach (var terminal in _terminals)
                {
                    model.SearchNotifications_Terminals = terminal;
                    int count;
                    proceed = new ReportDataModel().CorrectCustomerIdFromSPI(model, request, out count, ref masterEmailBody, ref db);
                }

                attempt.Type = Attempt_ResponseTypes.Ok;
                attempt.Message = proceed ? "No incidences pending to fix." : "Pending incidences to fix. Check your mailbox.";
                attempt.ObjectID = proceed;
                return attempt;
            }
            catch (Exception ex)
            {
                masterEmailBody += "<br />Method: PublicController.CompleteSalesRoomsSales<br />Exception Message: " + Debugging.GetMessage(ex)
                    + "<br />Inner Exception: " + Debugging.GetInnerException(ex) + "<br />Source: " + (ex.Source ?? "") + "<br />StackTrace: " + (ex.StackTrace ?? "");
                masterEmail.Body = masterEmailBody;
                //EmailNotifications.SendSync(masterEmail);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = masterEmail } });
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Email NOT Sent";
                attempt.ObjectID = null;
                attempt.Exception = ex;
                return attempt;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminals">string ids separated by comma</param>
        /// <param name="to">string addresses separated by comma</param>
        /// <param name="cc">string addresses separated by comma</param>
        /// <returns></returns>
        public AttemptResponse SendSalesRoomsReport(string terminals, string to, string cc)
        {

            var request = HttpContext.Request;
            PublicDataModel pdm = new PublicDataModel();
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = 600;
            AttemptResponse attempt = new AttemptResponse();
            NotificationsModel.SearchNotificationsModel model = new NotificationsModel.SearchNotificationsModel();

            var _terminals = terminals.Split(',').ToArray();

            var masterEmail = new System.Net.Mail.MailMessage();
            masterEmail.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
            masterEmail.To.Add("efalcon@villagroup.com");
            masterEmail.Subject = "Issues on VLO Notifications Report";
            masterEmail.IsBodyHtml = true;

            var masterEmailBody = "<br />Parameters<br />Terminals: " + terminals + "<br />To: " + to + "<br />CC: " + cc;
            try
            {
                var today = DateTime.Today;
                var _iDate = today;
                var counter = 0;
                string content = "";

                do
                {
                    if (_iDate.DayOfWeek == DayOfWeek.Friday)
                    {
                        if ((today != _iDate || today.DayOfWeek != DayOfWeek.Friday) && (today - _iDate).TotalDays >= 7)
                            counter++;
                    }
                    _iDate = _iDate.AddDays(-1);
                }
                while (counter <= 1);
                model.SearchNotifications_I_Date = _iDate.AddDays(1).ToString("yyyy-MM-dd");
                model.SearchNotifications_F_Date = _iDate.AddDays(7).ToString("yyyy-MM-dd");

                var _email = db.tblEmails.Single(m => m.emailID == 192);
                var emailInstance = new System.Net.Mail.MailMessage();

                emailInstance.From = new System.Net.Mail.MailAddress(_email.sender, _email.alias);
                emailInstance.To.Add(to);//splitted by comma
                emailInstance.CC.Add((cc != null && cc != "" ? (cc + ",") : "") + "efalcon@villagroup.com");//splitted by comma
                emailInstance.Subject = _email.subject;
                emailInstance.Body = _email.content_;
                emailInstance.IsBodyHtml = true;
                emailInstance.Priority = System.Net.Mail.MailPriority.Normal;

                foreach (var terminal in _terminals)
                {
                    model.SearchNotifications_Terminals = terminal;
                    NotificationsModel response = pdm.SendSalesRoomsReport(model, false, request, ref masterEmailBody, ref db);
                    var _terminal = long.Parse(terminal);
                    content += "<br /><br /><br />"
                        + "<p style=\"font-family:verdana;font-size:10pt;text-align:center;font-weight:bold;\">" + db.tblTerminals.Single(m => m.terminalID == _terminal).terminal + "</p>"
                        + System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Reports/_SearchNotificationsResultsPartial.cshtml", response));
                }

                emailInstance.Body = emailInstance.Body.Replace("$InitialDate", model.SearchNotifications_I_Date)
                    .Replace("$FinalDate", model.SearchNotifications_F_Date)
                    .Replace("$Content", content)
                    .Replace("á", "&aacute;")
                    .Replace("é", "&eacute;")
                    .Replace("í", "&iacute;")
                    .Replace("ó", "&oacute;")
                    .Replace("ú", "&uacute;")
                    .Replace("ñ", "&ntilde;");

                //EmailNotifications.Send(emailInstance, false);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = emailInstance } });
                attempt.Type = Attempt_ResponseTypes.Ok;
                attempt.Message = "Email Sent";
                attempt.ObjectID = new { recipients = to };
                return attempt;
            }
            catch (Exception ex)
            {
                masterEmailBody += "<br />Method: PublicController.SendSalesRoomsReport<br />Exception Message: " + Debugging.GetMessage(ex)
                    + "<br />Inner Exception: " + Debugging.GetInnerException(ex) + "<br />Source: " + (ex.Source ?? "") + "<br />StackTrace: " + (ex.StackTrace ?? "");
                masterEmail.Body = masterEmailBody;
                //EmailNotifications.SendSync(masterEmail);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = masterEmail } });
                attempt.Type = Attempt_ResponseTypes.Error;
                attempt.Message = "Email NOT Sent";
                attempt.ObjectID = null;
                attempt.Exception = ex;
                return attempt;
            }
        }

        public AttemptResponse SendPreArrivalPurchaseReminder()
        {
            var context = HttpContext;
            AttemptResponse attempt = new AttemptResponse();
            PublicDataModel pdm = new PublicDataModel();

            pdm.SendPreArrivalPurchaseReminder(context);
            return attempt;
        }

        public ActionResult GetInvitationInfo(string i, string s, string u)
        {
            PublicDataModel pdm = new PublicDataModel();
            Guid trackingID = Guid.Parse(i);
            int sysEventID = int.Parse(s);
            u = u.Replace('¬', '#');
            pdm.InvitationSaveData(trackingID, sysEventID, u);
            return Redirect(u);
        }

        public async Task<ActionResult> GetImage(string i)
        {
            Task.Run(() => SaveEmailTracking(i));
            var dir = Server.MapPath("/content/themes/base/images");
            var path = Path.Combine(dir, "tracker.png");
            return base.File(path, "image/png");
        }
        public async Task<int> SaveEmailTracking(string i)
        {
            var trackingID = Guid.Parse(i);
            var sysEventID = 36;//open
            int result = await PublicDataModel.InvitationSaveData2(trackingID, sysEventID, "open");
            return result;
        }

        //public AttemptResponse FrontOfficeSync(int resortID, string range = null)
        //{

        //    AttemptResponse attempt = new AttemptResponse();
        //    PublicDataModel pdm = new PublicDataModel();
        //    range = range ?? "";
        //    attempt = pdm.FrontOfficeSync(resortID, range);

        //    return attempt;
        //}

        public JsonResult FrontOfficeSync(int resortID, string range = null)
        {
            AttemptResponse attempt = new AttemptResponse();
            range = range ?? "";
            attempt = PublicDataModel.FrontOfficeSync(resortID, range);

            return Json(attempt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncArrivals(int resortID, DateTime from, DateTime to)
        {
            PublicDataModel.GetReservationsToSync(resortID, from, to);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //public AttemptResponse ResetEmailsCounters()
        //{
        //    AttemptResponse response = new AttemptResponse();
        //    response = new PublicDataModel().ResetEmailsCounters();
        //    return response;
        //}

        public JsonResult ResetEmailsCounters()
        {
            AttemptResponse response = new AttemptResponse();
            response = new PublicDataModel().ResetEmailsCounters();
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendEmailsByRule(int id = 0, string date = null)
        {
            AttemptResponse attempt = new AttemptResponse();
            PublicDataModel pdm = new PublicDataModel();

            pdm.SendEmailsByRule(id, date);

            return Json(attempt, JsonRequestBehavior.AllowGet);
        }

        //public object OpenLink()
        //{

        //}

        //public object GetImage()
        //{

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">pop username</param>
        /// <param name="pass">password</param>
        /// <param name="server">pop server</param>
        /// <param name="days">number of past days to check</param>
        /// <returns></returns>
        public JsonResult CheckForBounces(string address = null, string pass = null, string server = null, int days = 0)
        {
            return Json(new PublicDataModel().CheckForBounces(address, pass, server, days), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult OpenUrl(string id, string url)
        //{
        //    PublicDataModel pdm = new PublicDataModel();
        //    pdm.OpenUrl(id);
        //    return Redirect(url);
        //}

        public AttemptResponse ImportMarketCodes(string resortID)
        {
            AttemptResponse attempt = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            var _resortID = int.Parse(resortID);
            var place = db.tblPlaces.Single(m => m.frontOfficeResortID == _resortID).placeID;
            //var place = 8;
            var client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
            var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/CtlgCodigos/" + resortID);
            //var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/CtlgTiposHab/" + resortID);
            str = str.Replace("{\"result\":[[", "[").Replace("]]}", "]");
            var _result = new JavaScriptSerializer().Deserialize<List<Importable>>(str);

            var now = DateTime.Now;
            foreach (var i in _result)
            {
                if(db.tblMarketCodes.Count(m => m.frontOfficeResortID == _resortID && m.marketCode == i.namecodigodemercado) == 0)
                {
                    var c = new tblMarketCodes();
                    c.frontOfficeMarketCodeID = i.idcodigodemercado != null ? int.Parse(i.idcodigodemercado) : (int?)null;
                    c.marketCode = i.namecodigodemercado;
                    c.programID = (int?)null;
                    c.arrivalsTravelSourceID = (int?)null;
                    c.frontOfficeResortID = _resortID;
                    c.leadSourceID = (long?)null;
                    c.majorCode = i.CodigoMayor;
                    c.import = false;
                    db.tblMarketCodes.AddObject(c);
                }

                //if (db.tblRoomTypes.Count(m => m.placeID == place && (m.roomType == i.codetipodehabitacion || m.roomTypeCode == i.codetipodehabitacion)) == 0)
                //{
                //    var q = new tblRoomTypes();
                //    q.roomType = i.nametipodehabitacion;
                //    q.roomTypeCode = i.codetipodehabitacion;
                //    q.placeID = place;
                //    q.dateSaved = now;
                //    q.savedByUserID = Guid.Parse("8cd24473-d223-430d-bd8f-a3db71168b6b");
                //    db.tblRoomTypes.AddObject(q);
                //}
            }
            db.SaveChanges();
            return attempt;
        }

        public JsonResult ImportCSV(string tag)
        {
            PublicDataModel pdm = new PublicDataModel();
            AttemptResponse response = pdm.Import(tag);
            
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        //public AttemptResponse ImportCSV(string tag)
        //{
        //    PublicDataModel pdm = new PublicDataModel();
        //    AttemptResponse response = pdm.Import(tag);
        //    return response;
        //}

        //public AttemptResponse CreateNotificationLogs(int notification, string from, string to)
        //{
        //    AttemptResponse attempt = new AttemptResponse();
        //    ePlatEntities db = new ePlatEntities();

        //    var fDate = DateTime.Parse(from);
        //    var tDate = DateTime.Parse(to).AddDays(1).AddSeconds(-1);

        //    var query = from fv in db.tblFieldsValues
        //                join f in db.tblFields on fv.fieldID equals f.fieldID
        //                join fg in db.tblFieldGroups on f.fieldGroupID equals fg.fieldGroupID
        //                where fg.emailNotificationID == notification && (fv.dateSaved >= fDate && fv.dateSaved <= tDate)
        //                && (f.field == "$ItemID" | f.field == "$Sent" | f.field == "$SentByUserID")
        //                //add fieldid or fieldname to only get transactions and reservations ids
        //                select new { f.field, fv.value, fv.transactionID };

        //    var list = query.GroupBy(m => m.transactionID);

        //    foreach (var i in list)
        //    {
        //        if (db.tblEmailNotificationLogs.Count(m => m.emailNotificationID == notification && m.trackingID == i.Key) == 0)
        //        {
        //            tblEmailNotificationLogs q = new tblEmailNotificationLogs
        //            {
        //                emailNotificationID = notification,
        //                dateSent = DateTime.Parse(i.FirstOrDefault(m => m.field == "$Sent").value),
        //                sentByUserID = Guid.Parse(i.FirstOrDefault(m => m.field == "$SentByUserID").value),
        //                reservationID = Guid.Parse(i.FirstOrDefault(m => m.field == "$ItemID").value),
        //                trackingID = i.Key
        //            };
        //            db.tblEmailNotificationLogs.AddObject(q);
        //        }
        //    }
        //    db.SaveChanges();
        //    return attempt;
        //}

        public JsonResult UpdateOptions(int from, int to)
        {
            return Json(PublicDataModel.UpdateOptions(from, to), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatePhones()
        {
            return Json(PublicDataModel.UpdatePhones(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult NormalizeReservations(int? resort, string date, string confirmation)
        {
            return Json(PublicDataModel.NormalizeReservations(resort, date, confirmation), JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult test(string fromDate, string toDate)
        {
            return Json(PublicDataModel.test(fromDate, toDate), JsonRequestBehavior.AllowGet);

        }

        public JsonResult TestAssignation(int leadTypeID, long terminalID)
        {
            return Json(NetCenterDataModel.GetUserForAssignation(leadTypeID, terminalID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableAccountCredits(string account)
        {
            return Json(PublicDataModel.GetAvailableAccountCredits(account), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult DecryptCard(string lead)
        {

            return Json(PublicDataModel.DecryptCard(lead), JsonRequestBehavior.AllowGet);
        }
    }

}
