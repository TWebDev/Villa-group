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
using System.Threading;
using System.Threading.Tasks;

namespace ePlatBack.Controllers.CRM
{
    public class HostessController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData.Model = new ArrivalsViewModel
            {
                Privileges = AdminDataModel.GetViewPrivileges(11287),
                ArrivalForm = new ArrivalsViewModel.ArrivalInfoView()
                {
                    TourInfoForm = new ArrivalsViewModel.TourInfo(),
                    Privileges = AdminDataModel.GetViewPrivileges(11573)
                },
                SearchForecast = new ArrivalsViewModel.SearchArrivalsForecast(),
                SearchArrivals = new ArrivalsViewModel.SearchInArrivals(),
                SearchBinnacle = new ArrivalsViewModel.SearchInBinnacle(),
                SearchPenetration = new ArrivalsViewModel.SearchPenetrationReport(),
                SearchGlobalPenetration = new ArrivalsViewModel.SearchGlobalPenetrationReport()
            };
            return View();
        }

        [Authorize]
        public ActionResult Index2()
        {
            ViewData.Model = new ArrivalsViewModel
            {
                ArrivalForm = new ArrivalsViewModel.ArrivalInfoView(),
                SearchForecast = new ArrivalsViewModel.SearchArrivalsForecast(),
                SearchArrivals = new ArrivalsViewModel.SearchInArrivals(),
                SearchBinnacle = new ArrivalsViewModel.SearchInBinnacle(),
                SearchPenetration = new ArrivalsViewModel.SearchPenetrationReport(),
                SearchGlobalPenetration = new ArrivalsViewModel.SearchGlobalPenetrationReport()
            };
            return View();
        }

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request);
            return result;
        }

        [Authorize]
        public async Task<JsonResult> GetArrivals(DateTime? date, string name, bool avoidRequestToFront)
        {
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var getArrivals = ArrivalsDataModel.GetArrivals(date, name, avoidRequestToFront);

            Task.Run(() => CreateTableUserLogActivityAsync(getArrivals, "Get", (string)null, urlMethod, request));

            return Json(getArrivals, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependantFields()
        {
            var dependantFields =ArrivalsDataModel.GetDependantFields();   
            return Json(dependantFields, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CleanDuplicates(DateTime date)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ArrivalsDataModel.CleanDuplicates(date);

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
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SaveArrivalInfo(ArrivalsViewModel.ArrivalInfoModel model)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ArrivalsDataModel.SaveArrivalInfo(model);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request));

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                ArrivalInfo = attempt.ObjectID
            });
        }

        public async Task<ActionResult> SearchArrivalsForecast(ArrivalsViewModel.SearchArrivalsForecast model)
        {
            ArrivalsDataModel adm = new ArrivalsDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request));

            return PartialView("_ResultsArrivalsForecastPartial", adm.SearchArrivalsForecast(model));
        }

        public async Task<ActionResult> SearchPenetrationReport(ArrivalsViewModel.SearchPenetrationReport model)
        {
            ArrivalsDataModel adm = new ArrivalsDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request));
            
            return PartialView("_ResultsPenetrationPartial", adm.SearchPenetrationReport(model));
        }

        public async Task<ActionResult> SearchGlobalPenetrationReport(ArrivalsViewModel.SearchGlobalPenetrationReport model)
        {
            ArrivalsDataModel adm = new ArrivalsDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request));

            return PartialView("_ResultsGlobalPenetrationPartial", adm.SearchGlobalPenetrationReport(model));
        }

        public JsonResult GetParties(DateTime date, int programID)
        {
            var getParties = ArrivalsDataModel.GetParties(date, programID);   
            return Json(getParties, JsonRequestBehavior.AllowGet);
        }
    }
}
