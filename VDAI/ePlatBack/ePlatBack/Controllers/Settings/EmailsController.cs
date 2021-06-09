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

namespace ePlatBack.Controllers.Settings
{
    public class EmailsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                ViewData.Model = new EmailsViewModel
                {
                EmailsSearchModel = new EmailsSearchModel(),
                EmailInfoModel = new EmailInfoModel(),
                EmailNotificationsSearchModel = new EmailNotificationsSearchModel(),
                EmailNotificationInfoModel = new EmailNotificationInfoModel(),
                FieldGroupsSearchModel = new FieldGroupsSearchModel(),
                FieldGroupsInfoModel = new FieldGroupsInfoModel { 
                    FieldsInfoModel = new FieldsInfoModel(),
                },
            };
            return View();
        }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SearchEmails(EmailsSearchModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            EmailsViewModel evm = new EmailsViewModel();
            evm.SearchResults = edm.SearchEmails(model);
            return PartialView("_SearchEmailsResults",evm);
        }

        public JsonResult GetEmailInfo(int EmailID)
        {
            EmailsDataModel edm = new EmailsDataModel();
            return Json(edm.GetEmailInfo(EmailID));
        }

        public JsonResult SaveEmail(EmailInfoModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            AttemptResponse attempt = edm.SaveEmail(model);

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

        public ActionResult SearchEmailNotifications(EmailNotificationsSearchModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            EmailsViewModel evm = new EmailsViewModel();
            evm.SearchNotificationsResults = edm.SearchEmailNotifications(model);
            return PartialView("_SearchEmailNotificationsResults", evm);
        }

        public JsonResult GetEmailNotificationInfo(int EmailNotificationID)
        {
            EmailsDataModel edm = new EmailsDataModel();
            return Json(edm.GetEmailNotificationInfo(EmailNotificationID));
        }

        public JsonResult SaveEmailNotification(EmailNotificationInfoModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            AttemptResponse attempt = edm.SaveEmailNotification(model);

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

        public ActionResult SearchFieldGroups(FieldGroupsSearchModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            EmailsViewModel evm = new EmailsViewModel();
            evm.SearchFieldGroupsResults = edm.SearchFieldGroups(model);
            return PartialView("_SearchFieldGroupsResultsPartial", evm);
        }

        public JsonResult SaveFieldGroup(FieldGroupsInfoModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            AttemptResponse attempt = edm.SaveFieldGroup(model);

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

        public JsonResult GetFieldGroupInfo(int FieldGroupID)
        {
            EmailsDataModel edm = new EmailsDataModel();
            return Json(edm.GetFieldGroupInfo(FieldGroupID));
        }

        public ActionResult GetFieldsPerGroup(List<FieldsInfoModel> list)
        {
            EmailsDataModel edm = new EmailsDataModel();
            EmailsViewModel evm = new EmailsViewModel();
            evm.SearchFieldsResults = list;
            return PartialView("_GetFieldsResultsPartial", evm);
        }

        public ActionResult GetFieldInfo(int FieldID)
        {
            EmailsDataModel edm = new EmailsDataModel();
            return Json(edm.GetFieldInfo(FieldID));
        }

        public JsonResult SaveField(FieldsInfoModel model)
        {
            EmailsDataModel edm = new EmailsDataModel();
            AttemptResponse attempt = edm.SaveField(model);

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

        public JsonResult GetDDLData(string itemType, string itemID)
        {
            EmailsDataModel edm = new EmailsDataModel();
            return Json(edm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }
    }
}
