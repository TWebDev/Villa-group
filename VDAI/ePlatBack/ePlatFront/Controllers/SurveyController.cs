using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;

namespace ePlatFront.Controllers
{
    public class SurveyController : Controller
    {
        //
        // GET: /Survey/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSurvey(string id)
        {
            return Json(SurveyDataModel.GetSurvey(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSurvey(string model)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = SurveyDataModel.SaveSurveyValues(model);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult GetSurveyInfo(string id, string transactionid)
        {
            return Json(SurveyDataModel.GetSurveyInfo(id, transactionid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTransactionID(string id, string purchaseid)
        {
            return Json(SurveyDataModel.GetTransactionID(id, purchaseid), JsonRequestBehavior.AllowGet);
        }

    }
}
