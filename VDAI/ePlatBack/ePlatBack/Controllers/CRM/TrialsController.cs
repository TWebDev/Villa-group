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
using Newtonsoft.Json;


namespace ePlatBack.Controllers.CRM
{
    public class TrialsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            List<SelectListItem> Agents = TrialsDataModel.GetAgents();
            List<SelectListItem> BookingStatus = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
            TrialsViewModel trialsModel = new TrialsViewModel()
            {
                Privileges = AdminDataModel.GetViewPrivileges(11698),
                AgentsList = JsonConvert.SerializeObject(Agents),
                BookingStatusList = JsonConvert.SerializeObject(BookingStatus),
                Search = new TrialsViewModel.SearchTrial()
                {
                    Search_Agents = Agents,
                    Search_BookingStatus = BookingStatus
                },
                Info = new TrialsViewModel.Trial()
                {
                    AgentsList = Agents,
                    BookingStatusList = BookingStatus
                }
            };

            return View(trialsModel);
        }

        [Authorize]
        public JsonResult GetAgents()
        {
            return Json(TrialsDataModel.GetAgents(), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveImport(string trials, bool unassigned)
        {
            AttemptResponse attempt = TrialsDataModel.SaveImport(trials, unassigned);
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

        [Authorize]
        public JsonResult SaveTrial(TrialsViewModel.Trial model)
        {
            AttemptResponse attempt = TrialsDataModel.SaveTrial(model);
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
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult SearchTrials(TrialsViewModel.SearchTrial model)
        {
            return Json(TrialsDataModel.SearchTrials(model), JsonRequestBehavior.AllowGet);
        }
    }
}
