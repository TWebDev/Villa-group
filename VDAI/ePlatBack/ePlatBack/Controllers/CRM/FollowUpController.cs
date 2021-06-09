using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models;


namespace ePlatBack.Controllers.CRM
{
    [Authorize]
    public class FollowUpController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        #region Log

        public JsonResult GetLog(DateTime date, Guid[] userID)
        {
            return Json(FollowUpDataModel.GetLogs(date, userID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependentFields()
        {
            return Json(FollowUpDataModel.GetDependentFields(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLog(FollowUpViewModel.LogItem model)
        {
            AttemptResponse attempt = FollowUpDataModel.SaveLog(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                Log = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        #endregion Log

        #region Email

        public JsonResult GetMails(string folder, string search)
        {
            //return Json(MailingDataModel.GetMails(folder), JsonRequestBehavior.AllowGet);
            return new JsonResult()
            {
                Data = MailingDataModel.GetMails(folder, search),
                ContentType = "application/json",
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult DeleteMail(Guid id)
        {
            AttemptResponse attempt = MailingDataModel.DeleteMail(id);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        #endregion Email

        #region Lead

        public JsonResult SaveLead(string lead)
        {
            AttemptResponse attempt = FollowUpDataModel.SaveLead(lead);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                Lead = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult PhoneAnalysis(string phone)
        {
            AttemptResponse attempt = FollowUpDataModel.PhoneAnalysis(phone);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                Analysis = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SearchLeads(FollowUpViewModel.SearchFilters model)
        {
            return Json(FollowUpDataModel.SearchLeads(model), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
