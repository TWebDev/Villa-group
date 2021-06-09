using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System.Web.Security;
using System.Net;
using System.Threading;
using System.Threading.Tasks;


namespace ePlatBack.Controllers.Settings
{
    public class SurveysController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                ViewData.Model = new SurveyViewModel.SurveyIndexModel()
                {
                    SurveyModel = new SurveyViewModel.SurveyModel()
                    {
                        SurveyInfoField = new SurveyViewModel.SurveyInfoField()
                        {
                            I_FieldSubTypes = SurveyDataModel.SurveysCatalogs.FillDrpFieldSubTypes(2),
                            I_VisibilityOptions = SurveyDataModel.SurveysCatalogs.FillDrpVisibilityOptions(2)
                        },
                        SurveyFormField = new SurveyViewModel.SurveyFormField()
                        {
                            F_FieldSubTypes = SurveyDataModel.SurveysCatalogs.FillDrpFieldSubTypes(1),
                            F_VisibilityOptions = SurveyDataModel.SurveysCatalogs.FillDrpVisibilityOptions(1)
                        },

                    },
                    SearchSurveyModel = new SurveyViewModel.SearchSurveyModel(),
                    AbleToModify = GeneralFunctions.IsUserInRole("Administrator", null, true)
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }


        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request);
            return result;
        }

        public JsonResult PublishField(Guid transactionID, Guid fieldGuid)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = SurveyDataModel.PublishField(transactionID, fieldGuid);

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
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                Published = attempt.ObjectID
            });
        }

        public JsonResult SaveSurvey(string model){
            AttemptResponse attempt = new AttemptResponse();
            attempt = SurveyDataModel.SaveSurvey(model);

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
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                SurveyID = attempt.ObjectID
            });
        }

        public JsonResult GetReferrals(string fromDate, string toDate, int fieldGroupID, string[] terminals)
        {
            return Json(SurveyDataModel.GetReferrals(fromDate, toDate, fieldGroupID, terminals), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchSurvey(SurveyViewModel.SearchSurveyModel model)
        {
            SurveyDataModel sdm = new SurveyDataModel();
            List<SurveyViewModel.SurveyItem> pm = sdm.Search(model);
            return PartialView("_SearchSurveyResultsPartial", pm);
        }

        [HttpPost, Authorize]
        public JsonResult GetSurvey(int id) {
            return Json(SurveyDataModel.GetSurvey(id));
        }


        public JsonResult GetStatsParams(int id, string from, string to)
        {
            return Json(SurveyDataModel.GetStatsParams(id, from, to), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStats(int id, string fields)
        {
            //return Json(SurveyDataModel.GetStats(id, fields));
            return new JsonResult() {
                Data = SurveyDataModel.GetStats(id, fields),
                MaxJsonLength = Int32.MaxValue
            };
        }


        [ValidateInput(false)]
        public JsonResult SendEmail(string to, string cc, string subject, string content, string survey)
        {
            SurveyDataModel.SendEmail(to, cc, subject, content, survey);
            return Json(new { Success = true });
        }

        //TaskScheduler
        public JsonResult GetSurveysTafer()
        {
            AttemptResponse attempt = new AttemptResponse();
            var reference = "Daily Data Extract-Tafer Resorts-" + DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            attempt = SurveyDataModel.SurveyFindArchive(reference);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
           // var request  = HttpContext.Request;           
           // Task.Run(() => CreateTableUserLogActivityAsync("", "TaskScheduler", attempt, "", request));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                SurveyID = attempt.ObjectID
            });
        }
        public JsonResult GetSurveys()
        {
            AttemptResponse attempt = new AttemptResponse();          
            var reference = "Daily Data Extract-" + DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            attempt = SurveyDataModel.SurveyFindArchive(reference);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
          //  var request = HttpContext.Request;         
          //  Task.Run (() => CreateTableUserLogActivityAsync("", "TaskScheduler", attempt, "", request));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                SurveyID = attempt.ObjectID
            });           
        }
        public JsonResult Execute(string date)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = SurveyDataModel.SurveyFindArchive(date);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //  var request = HttpContext.Request;
            //  Task.Run(() => CreateTableUserLogActivityAsync("", "TaskScheduler", attempt, "", request));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                SurveyID = attempt.ObjectID
            });            
        }
        public JsonResult GetStatsTaskScheduler()
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = SurveyDataModel.GetStastsTaskScheduler();

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            //Task.Run(() => CreateTableUserLogActivityAsync("", "TaskScheduler", attempt, "", request));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                SurveyID = attempt.ObjectID
            });
        }
    }
}
