using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ViewResult Index()
        {
            ViewBag.Message = "Dashboard";
            //ViewData.Model = new DashboardViewModel()
            //{
            //    PendingCloseOuts = DashboardDataModel.GetPendingCloseOuts()
            //};
            ViewData.Model = new DashboardViewModel() { 
                Privileges = AdminDataModel.GetViewPrivileges(1)
            };
            
            return View();
        }

        //public ViewResult Index2(string id)
        //{
        //    ViewBag.Message = ReportDataModel.Corrector(id);
        //    return View();
        //}

        public PartialViewResult GetDailyCommissions()
        {
            string returnPartial = "_DailyCommissionsPartial";
            DailyCommissions model = DashboardDataModel.GetDailyCommissions();
            return PartialView(returnPartial, model);
        }


        public JsonResult GetDailyVolumeTask(string id, long? terminalID = null)
        {            
            AttemptResponse response = new AttemptResponse();
            try
            {
                DashboardDataModel.GetDailyVolume(id, terminalID);
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "The Task was Execute Success";
                response.ObjectID = "1";
            }
            catch(Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "The Task was Execute Success";
                response.ObjectID = "0";
            }
            return Json(new
                {
                    ResponseType = response.Type,
                    ResponseMessage = response.Message,
                    ResponseObjectID = response.ObjectID
                });
        }

        public PartialViewResult GetDailyVolume(string id, long? terminalID = null)
        {
            string returnPartial = "_DailyVolumePartial";
            DailyVolume model = DashboardDataModel.GetDailyVolume(id, terminalID);
            return PartialView(returnPartial, model);
        }

        //public PartialViewResult GetAgentsVolume(string id)
        //{
        //    string returnPartial = "_AgentsVolumePartial";
        //    AgentsVolume model = DashboardDataModel.GetAgentsVolume(id);
        //    return PartialView(returnPartial, model);
        //}

        public PartialViewResult GetPendingCloseOuts()
        {
            string returnPartial = "_PendingCloseOutsPartial";
            PendingCloseOuts model = DashboardDataModel.GetPendingCloseOuts();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetHostessStatus()
        {
            string returnPartial = "_HostessStatusPartial";
            HostessStatus model = DashboardDataModel.GetHostessStatus();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetExchangeRates() {
            string returnPartial = "_ExchangeRatesStatusPartial";
            ExchangeRatesStatus model = DashboardDataModel.GetExchangeRatesStatus();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetPendingCache()
        {
            string returnPartial = "_PendingCacheEmptyPartial";
            //PendingCache model = DashboardDataModel.GetPendingCache();
            PendingCache model = new PendingCache();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetPendingCacheReport()
        {
            string returnPartial = "_PendingCachePartial";
            PendingCache model = DashboardDataModel.GetPendingCache();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetOnlineGoal()
        {
            string returnPartial = "_OnlineGoalsPartial";
            OnlineGoals model = DashboardDataModel.GetOnlineGoals();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetCloseOutsWithErrors()
        {
            string returnPartial = "_CloseOutErrorsPartial";
            CloseOutsWithErrors model = DashboardDataModel.GetCloseOutsWithErrors();
            return PartialView(returnPartial, model);
        }

        public PartialViewResult GetAvailabilityAlerts()
        {
            string returnPartial = "_AvailabilityStatusPartial";
            AvailabilityStatus model = DashboardDataModel.GetAvailabilityAlerts();
            return PartialView(returnPartial, model);
        }

        [HttpPost]
        public JsonResult WikiFieldsWithContent(string id)
        {
            return Json(new {
                Fields = WikiFieldDataModel.GetFieldsWithContent(id)
            });
        }

        public PartialViewResult WikiField(string id)
        {
            string returnPartial = "_WikiFieldPartial";
            dynamic model = WikiFieldDataModel.GetWikiField(id);
            return PartialView(returnPartial, model);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SaveWikiField(WikiFieldViewModel.Post model)
        {
            AttemptResponse attempt = WikiFieldDataModel.SaveWiki(model);

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

        public PartialViewResult Tickets()
        {
            string returnPartial = "_TicketsPartial";
            List<SelectListItem> selectedTerminals = MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
            selectedTerminals.Insert(0, ePlatBack.Models.Utils.ListItems.Default("Any", ""));
            List<SelectListItem> adminUsers = UserDataModel.UserCatalogs.FillDrpAdministratorUsersInWorkGroup();
            adminUsers.Insert(0, ePlatBack.Models.Utils.ListItems.Default("None", ""));
            dynamic model = new TicketViewModel()
            {
                SearchTicketViewModel = new TicketViewModel.Search_Ticket(),
                TicketInfo = new TicketViewModel.TicketItem()
                {
                    SelectedTerminals = selectedTerminals,
                    AdministratorUsers = adminUsers,
                    TicketStatusList = TicketDataModel.TicketCatalogs.FillDrpTicketStatus()
                }
            };
            return PartialView(returnPartial, model);
        }

        public JsonResult SearchTickets(TicketViewModel.Search_Ticket model)
        {
            TicketDataModel tdm = new TicketDataModel();
            return Json(tdm.GetTickets(model));
        }

        public JsonResult SaveTicket(TicketViewModel.TicketItem model)
        {
            TicketDataModel tdm = new TicketDataModel();
            return Json(tdm.SaveTicket(model));
        }
    }
}
