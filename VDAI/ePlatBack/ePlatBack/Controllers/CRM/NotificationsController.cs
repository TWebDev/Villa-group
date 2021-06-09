using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using System.IO;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ePlatBack.Controllers.CRM
{
    public class NotificationsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData.Model = new NotificationsViewModel
            {
                FormsSearchModel = new FormsSearchModel(),
                VLOManifestModel = new VLOManifestModel(),
            };
            return View();
        }

        [Authorize]
        public ActionResult Index2()
        {
            ViewData.Model = new NotificationsViewModel
            {
                FormsSearchModel = new FormsSearchModel(),
                VLOManifestModel = new VLOManifestModel(),
            };
            return View();
        }

        public PartialViewResult RenderManifestPartial(string partial)
        {
            return PartialView("_ManifestPartial", new VLOManifestModel());
        }

        public ActionResult SearchForms(FormsSearchModel model)
        {
            NotificationsViewModel nvm = new NotificationsViewModel();
            NotificationsDataModel ndm = new NotificationsDataModel();
            nvm.ListForms = ndm.SearchForms(model);
            return PartialView("_NotificationsSearchResultsPartial", nvm);
        }

        public ActionResult RenderSingleSending(int fieldGroupID)
        {
            NotificationsViewModel model = new NotificationsViewModel();
            NotificationsDataModel edm = new NotificationsDataModel();
            model.ListFieldValues = edm.RenderSingleSending(fieldGroupID);
            ViewBag.FormName = edm.GetFormName(fieldGroupID);
            return PartialView("_FieldValuesManagementPartial", model);
        }

        public JsonResult SaveFieldValues(FieldValueModel model)
        {
            NotificationsDataModel edm = new NotificationsDataModel();
            var context = HttpContext;
            AttemptResponse attempt = edm.SaveFieldValues(model, null, false, context);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult DeleteNotificationValues(string transactionID)
        {
            NotificationsDataModel edm = new NotificationsDataModel();
            AttemptResponse attempt = edm.DeleteNotificationValues(transactionID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public ActionResult GetNotificationHistory(int FormID, string Date)
        {
            NotificationsViewModel nvm = new NotificationsViewModel();
            NotificationsDataModel ndm = new NotificationsDataModel();
            nvm.ListNotificationHistory = ndm.GetNotificationHistory(FormID, Date);
            return PartialView("_NotificationHistoryResultsPartial", nvm);
        }

        public async Task<int> TrackAsync(string id)
        {
            int result = await NotificationsDataModel.TrackEmailOpen(id);
            return result;
        }

        public async Task<ActionResult> GetImage(string id)//verificar el parámetro necesario
        {
            //method that will save the emailOpen event
            Task.Run(() => TrackAsync(id));

            var dir = Server.MapPath("/content/themes/base/images");
            var path = Path.Combine(dir, "tracker.png");
            return base.File(path, "image/png");
        }

        public ActionResult OpenLink(string id, string url)
        {
            if(url.IndexOf("mailto") != -1 || url.IndexOf("tel:") != -1)//si es correo o telefono, redirigir directamente
            {
                new NotificationsDataModel().TrackUrlClick(id, url);
                return Redirect(url);
            }
            //constatar que url posee formato correcto
            if (url.IndexOf("http") == -1)
            {
                url = "https://" + url;
            }
            //después del formateo, verificar si posee estructura correcta
            Uri r;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out r) && (r.Scheme == Uri.UriSchemeHttp || r.Scheme == Uri.UriSchemeHttps);
            if (!result)
            {
                var email = EmailNotifications.GetSystemEmail("Method OpenLink tried to redirect to url=" + url + "<br />transaction=" + id, "efalcon@villagroup.com");
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }
            else
            {
                new NotificationsDataModel().TrackUrlClick(id, url);
                //si posee la estructura correcta, verificar que no hay urls continuas en el mismo parametro y de ser así, redirigir a la última (corrección de caso especial)
                var _items = url.Replace("https", ",https");
                var items = _items.Split(',');
                if (items.Count() > 1)
                {
                    var email = EmailNotifications.GetSystemEmail("Method OpenLink tried to redirect to url=" + url + "<br />transaction=" + id, "efalcon@villagroup.com");
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                    return Redirect(items.Last());
                }
            }

            return Redirect(url);
        }

        public ActionResult _OpenLink(string id, string url)
        {
            //method to save open event
            new NotificationsDataModel().TrackUrlClick(id, url);

            if(url.IndexOf("mailto") != -1 || url.IndexOf("tel:") != -1)
            {
                return Redirect(url);
            }
            if (url.IndexOf("http") == -1 )
            {
                url = "https://" + url;
            }
            Uri r;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out r) && (r.Scheme == Uri.UriSchemeHttp || r.Scheme == Uri.UriSchemeHttps);

            if (!result)
            {
                var email = EmailNotifications.GetSystemEmail("Method OpenLink tried to redirect to url=" + url + "<br />transaction=" + id, "efalcon@villagroup.com");
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
            }

            var _items = url.Replace("https",",https");
            var items = _items.Split(',');
            if(items.Count() > 1)
            {
                var email = EmailNotifications.GetSystemEmail("Method OpenLink tried to redirect to url=" + url + "<br />transaction=" + id, "efalcon@villagroup.com");
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                return Redirect(items.Last());
            }

            return Redirect(url);
        }

        public JsonResult Unsuscribe()
        {
            NotificationsDataModel ndm = new NotificationsDataModel();
            return Json(ndm.Unsuscribe(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SentLetters(string id)
        {
            NotificationsDataModel ndm = new NotificationsDataModel();
            return Json(ndm.SentLetters(Guid.Parse(id)));
        }

        public ActionResult Preview(string id, string log)
        {
            NotificationsDataModel ndm = new NotificationsDataModel();
            ViewBag.Content = ndm.ViewLetter(Guid.Parse(id), int.Parse(log));

            return View("Preview");
        }

        public JsonResult SendLetter(string data)
        {
            var context = HttpContext;
            AttemptResponse attempt = new NotificationsDataModel().SendLetter(data, context);

            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetPersonalTitles()
        {
            return Json(MasterChartDataModel.LeadsCatalogs.FillDrpPersonalTitles(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetePlatNotifications()
        {
            return Json(NotificationsDataModel.GetNotifications(), JsonRequestBehavior.AllowGet);
        }//AL INICIAR SESSION MANDA LAS NOTIFICACIONES PENDIENTES POR LEER

        public JsonResult ClientNotification(string _itemID, string _sysItemTypeID, string _terminalID, string _forUserID, string _description, string _eventUserID)
        {
            ePlatHub hub = new ePlatHub();
            hub.addClientNotification(_itemID, _sysItemTypeID, _terminalID, _forUserID, _description, _eventUserID);
            return Json("ok");
        }

        public JsonResult ReadAndDeliveredNotification(string method, string itemID)
        {
            DateTime? date = NotificationsDataModel.ReadAndDeliveredDateTime(method, long.Parse(itemID));
            return Json(date);
        }

        public JsonResult UpdateNotificacions(string userID)
        {
            ePlatHub hub = new ePlatHub();
            hub.updateNotifications(userID);
            return Json("ok");
        }

        public void UpdateTarget(Guid userID)
        {
            ePlatHub hub = new ePlatHub();
            hub.updateTarget(userID);
        }

        public void SetNotificationReminder(int reminderID)
        {
            ePlatHub hub = new ePlatHub();
            Guid userID = RemindersDataModel.SaveReminderNotification(reminderID);
            hub.updateTarget(userID);
        }
    }
}
