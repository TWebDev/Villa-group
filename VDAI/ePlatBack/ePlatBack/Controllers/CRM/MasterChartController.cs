using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System.Data.Entity;
using System.Web.Script.Serialization;
using ePlatBack.Models.Utils;
using System.Threading;
using System.IO;
using System.Web.Mvc.Ajax;
using System.Dynamic;
using System.Reflection;
using System.ComponentModel;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.Text.RegularExpressions;
using ePlatBack.Models.Utils.Custom;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ePlatBack.Controllers.CRM
{

    public class MasterChartController : Controller
    {
        //
        // GET: /MasterChart/
        ePlatEntities db = new ePlatEntities();

        [Authorize]
        public ActionResult AddReferral(Guid referredByID)
        {
            ViewData.Model = new LeadModel.Views.LeadGeneralInformation()
                { // predefined values // eventually this predefined values should be pulled base on the page.
                    GeneralInformation_InputMethodID = 1, //Manual
                    GeneralInformation_LeadStatusID = 1, //New
                    GeneralInformation_ReferredByID = new Guid(referredByID.ToString())
                };
            return View();
        }

        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                var privileges = AdminDataModel.GetViewPrivileges(10301);
                var usersColeagues = UserDataModel.GetUsersBySupervisor(null, true);
                var users = UserDataModel.GetUsersBySupervisor();
                var usersActive = UserDataModel.GetUsersBySupervisor(null, false, false, true);


                if (Request.Params["referralID"] != null || Request.Params["leadID"] != null)
                {
                    ViewData.Model = new MasterChartViewModel
                    {
                        leadSearch = new LeadModel.Views.Search() { Search_DrpInteractedWithUser = usersColeagues, Search_DrpAssignedTo = usersColeagues, Search_DrpInputBy = usersColeagues },
                        massUpdate = new LeadModel.Views.MassUpdate() { Users = users },
                        hotelReservations = new HotelReservationModel.Views.ReservationGeneralInformation() { ArrivalDate = null },
                        billingInformation = new BillingInfoModel.Views.BillingInfo(),
                        leadGeneralInformation = new LeadModel.Views.LeadGeneralInformation()
                        {
                            GeneralInformation_LeadID = (Request.Params["referralID"] != null) ? new Guid(Request.Params["referralID"].ToString()) : new Guid(Request.Params["leadID"].ToString()),
                            GeneralInformation_DrpAssignedToUser = usersColeagues,
                            GeneralInformation_Interactions = new LeadModel.Views.Interactions() { Users = usersActive }
                        },
                        reservationCharges = new LeadModel.Views.ReservationCharges(),
                        Privileges = AdminDataModel.GetViewPrivileges(10301)
                    };
                    return View();
                }
                else
                {
                    LeadModel.Views.MassUpdate massUpdateInfo = null;
                    PurchasesModel.PurchaseInfoModel purchaseInfo = null;
                    HotelReservationModel.Views.ReservationGeneralInformation reservationInfo = null;
                    TimeShareViewModel timeshareInfo = null;
                    if (privileges.FirstOrDefault(m => m.Component == "fdsMassUpdate") == null || privileges.FirstOrDefault(m => m.Component == "fdsMassUpdate").View)
                    {
                        massUpdateInfo = new LeadModel.Views.MassUpdate() { Users = users };
                    }
                    if (privileges.FirstOrDefault(m => m.Component == "fdsPurchasesManagement") == null || privileges.FirstOrDefault(m => m.Component == "fdsPurchasesManagement").View)
                    {
                        purchaseInfo = new PurchasesModel.PurchaseInfoModel()
                    {
                        PurchaseServiceModel = new PurchasesModel.PurchaseServiceModel()
                        {
                            PurchaseServiceDetailModel = new PurchasesModel.PurchaseServiceDetailModel()
                        },
                        PurchasePaymentModel = new PurchasesModel.PurchasePaymentModel()
                    };
                    }
                    if (privileges.FirstOrDefault(m => m.Component == "frmReservationGeneralInformation") == null || privileges.FirstOrDefault(m => m.Component == "frmReservationGeneralInformation").View)
                    {
                        reservationInfo = new HotelReservationModel.Views.ReservationGeneralInformation()
                        {
                            ArrivalDate = null
                        };
                    }
                    if (privileges.FirstOrDefault(m => m.Component == "fdsAccountancyManagement") != null && privileges.FirstOrDefault(m => m.Component == "fdsAccountancyManagement").View)
                    {
                        timeshareInfo = new TimeShareViewModel()
                        {
                            AccountancyModel_OutcomeSearchModel = new OutcomeSearchModel(),
                            AccountancyModel_OutcomeInfoModel = new OutcomeInfoModel(),
                            AccountancyModel_IncomeSearchModel = new IncomeSearchModel(),
                            AccountancyModel_IncomeInfoModel = new IncomeInfoModel()
                        };
                    }
                    ViewData.Model = new MasterChartViewModel
                {
                    leadSearch = new LeadModel.Views.Search() { Search_DrpInteractedWithUser = usersColeagues, Search_DrpAssignedTo = usersColeagues, Search_DrpInputBy = usersColeagues },
                    massUpdate = massUpdateInfo,
                    //hotelReservations = new HotelReservationModel.Views.ReservationGeneralInformation() { ArrivalDate = null },
                    hotelReservations = reservationInfo,
                    billingInformation = new BillingInfoModel.Views.BillingInfo(),
                    leadGeneralInformation = new LeadModel.Views.LeadGeneralInformation()
                    { // predefined values // eventually this predefined values should be pulled base on the page.
                        GeneralInformation_InputMethodID = 1, //Manual
                        GeneralInformation_LeadStatusID = 1, //New
                        FastSaleInfoModel = new FastSaleModel.FastSaleInfoModel(),
                        GeneralInformation_DrpAssignedToUser = usersColeagues,
                        GeneralInformation_Interactions = new LeadModel.Views.Interactions() { Users = usersActive }
                    },
                    reservationCharges = new LeadModel.Views.ReservationCharges(),
                    PurchaseInfoModel = purchaseInfo,
                    TimeShareModel = timeshareInfo,
                    Privileges = privileges
                };
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        //public ActionResult Index()
        //{
        //    if (AdminDataModel.VerifyAccess())
        //    {
        //        var privileges = AdminDataModel.GetViewPrivileges(10301);
        //        var usersColeagues = UserDataModel.GetUsersBySupervisor(null, true);
        //        var users = UserDataModel.GetUsersBySupervisor();
        //        var usersActive = UserDataModel.GetUsersBySupervisor(null, false, false, true);
        //        if (Request.Params["referralID"] != null || Request.Params["leadID"] != null)
        //        {
        //            ViewData.Model = new MasterChartViewModel
        //            {
        //                leadSearch = new LeadModel.Views.Search() { Search_DrpInteractedWithUser = usersColeagues, Search_DrpAssignedTo = usersColeagues, Search_DrpInputBy = usersColeagues },
        //                massUpdate = new LeadModel.Views.MassUpdate() { Users = users },
        //                hotelReservations = new HotelReservationModel.Views.ReservationGeneralInformation() { ArrivalDate = null },
        //                billingInformation = new BillingInfoModel.Views.BillingInfo(),
        //                leadGeneralInformation = new LeadModel.Views.LeadGeneralInformation()
        //                {
        //                    GeneralInformation_LeadID = (Request.Params["referralID"] != null) ? new Guid(Request.Params["referralID"].ToString()) : new Guid(Request.Params["leadID"].ToString()),
        //                    GeneralInformation_DrpAssignedToUser = usersColeagues,
        //                    GeneralInformation_Interactions = new LeadModel.Views.Interactions() { Users = usersActive }
        //                },
        //                reservationCharges = new LeadModel.Views.ReservationCharges(),
        //                Privileges = AdminDataModel.GetViewPrivileges(10301)
        //            };
        //            return View();
        //        }
        //        else
        //        {
        //            PurchasesModel.PurchaseInfoModel purchaseInfo = null;
        //            TimeShareViewModel timeshareInfo = null;
        //            if (privileges.FirstOrDefault(m => m.Component == "fdsPurchasesManagement") == null || privileges.FirstOrDefault(m => m.Component == "fdsPurchasesManagement").View)
        //            {
        //                purchaseInfo = new PurchasesModel.PurchaseInfoModel()
        //            {
        //                PurchaseServiceModel = new PurchasesModel.PurchaseServiceModel()
        //                {
        //                    PurchaseServiceDetailModel = new PurchasesModel.PurchaseServiceDetailModel()
        //                },
        //                PurchasePaymentModel = new PurchasesModel.PurchasePaymentModel()
        //            };
        //            }
        //            if()
        //            ViewData.Model = new MasterChartViewModel
        //        {
        //            leadSearch = new LeadModel.Views.Search() { Search_DrpInteractedWithUser = usersColeagues, Search_DrpAssignedTo = usersColeagues, Search_DrpInputBy = usersColeagues },
        //            massUpdate = new LeadModel.Views.MassUpdate() { Users = users },
        //            hotelReservations = new HotelReservationModel.Views.ReservationGeneralInformation() { ArrivalDate = null },
        //            billingInformation = new BillingInfoModel.Views.BillingInfo(),
        //            leadGeneralInformation = new LeadModel.Views.LeadGeneralInformation()
        //            { // predefined values // eventually this predefined values should be pulled base on the page.
        //                GeneralInformation_InputMethodID = 1, //Manual
        //                GeneralInformation_LeadStatusID = 1, //New
        //                FastSaleInfoModel = new FastSaleModel.FastSaleInfoModel(),
        //                GeneralInformation_DrpAssignedToUser = usersColeagues,
        //                GeneralInformation_Interactions = new LeadModel.Views.Interactions() { Users = usersActive }
        //            },
        //            reservationCharges = new LeadModel.Views.ReservationCharges(),
        //            PurchaseInfoModel = purchaseInfo,
        //            //PurchaseInfoModel = new PurchasesModel.PurchaseInfoModel()
        //            //{
        //            //    PurchaseServiceModel = new PurchasesModel.PurchaseServiceModel()
        //            //    {
        //            //        PurchaseServiceDetailModel = new PurchasesModel.PurchaseServiceDetailModel()
        //            //    },
        //            //    PurchasePaymentModel = new PurchasesModel.PurchasePaymentModel()
        //            //},
        //            TimeShareModel = timeshareInfo,
        //            //TimeShareModel = new TimeShareViewModel()
        //            //{
        //            //    AccountancyModel_OutcomeSearchModel = new OutcomeSearchModel(),
        //            //    AccountancyModel_OutcomeInfoModel = new OutcomeInfoModel(),
        //            //    AccountancyModel_IncomeSearchModel = new IncomeSearchModel(),
        //            //    AccountancyModel_IncomeInfoModel = new IncomeInfoModel()
        //            //},
        //            Privileges = AdminDataModel.GetViewPrivileges(10301)
        //            //AccountancyModel = new AccountancyModel(){
        //            //    AccountancyModel_OutcomeSearchModel = new AccountancyModel.OutcomeSearchModel(),
        //            //    AccountancyModel_OutcomeInfoModel = new AccountancyModel.OutcomeInfoModel(),
        //            //    AccountancyModel_IncomeSearchModel = new AccountancyModel.IncomeSearchModel(),
        //            //    AccountancyModel_IncomeInfoModel = new AccountancyModel.IncomeInfoModel()
        //            //}
        //        };
        //            return View();
        //        }
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        public ActionResult RenderFastSale()
        {
            ViewData.Model = new FastSaleModel.FastSaleInfoModel()
            {
                Privileges = AdminDataModel.GetViewPrivileges(11182)
            };
            ///
            //FastSaleModel.FastSaleInfoModel model = new FastSaleModel.FastSaleInfoModel();
            return PartialView("_FastSalePartial", ViewData.Model);
        }

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
            return result;
        }

        public JsonResult GetManifest(string date)
        {
            MasterChartDataModel.Purchases purchase = new MasterChartDataModel.Purchases();
            return Json(purchase.GetManifest(date));
        }

        public async Task<JsonResult> SearchCoupons(LeadModel.Views.Search model)
        {
            MasterChartDataModel.Purchases purchase = new MasterChartDataModel.Purchases();
            PurchasesModel pm = new PurchasesModel();
            var coupons = purchase.SearchCoupons(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            pm.JsonRecentCoupons = coupons;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return Json(pm.JsonRecentCoupons);
        }

        public async Task<JsonResult> SearchCouponsByDate(LeadModel.Views.Search model)
        {
            MasterChartDataModel.Purchases purchase = new MasterChartDataModel.Purchases();
            PurchasesModel pm = new PurchasesModel();
            var coupons = purchase.SearchCouponsByDate(model);
            var urlMethod = new UserSession().Url;
            var request = HttpContext.Request;
            pm.JsonRecentCoupons = coupons;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return Json(pm.JsonRecentCoupons);
        }

        /// <summary>
        /// Find the users that match with the search criteria.
        /// </summary>
        /// <param name="usm">UserViewModel properties</param>
        /// <returns></returns>
        public async Task<ActionResult> Search(LeadModel.Views.Search lsm)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(lsm, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_leadSearchResultPartial", lead.Search(lsm));
        }

        [HttpPost]
        public JsonResult SearchReferrals(Guid leadID)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            List<LeadModel.Items.SearchResults> currentReferrals = lead.SearchReferrals(leadID);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;


            return Json(new { currentReferrals });
        }

        [HttpPost]
        public JsonResult SearchReferringLead(Guid leadID)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            List<LeadModel.Items.SearchResults> referringLead = lead.SearchReferringLead(leadID);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(referringLead, "Search", (string)null, urlMethod, request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(new { referringLead });
        }

        public ActionResult RenderPurchaseTicket(string PurchaseID, int PurchaseTicketID)
        {
            var _purchase = Guid.Parse(PurchaseID);
            var purchaseCulture = db.tblPurchases.Single(m => m.purchaseID == _purchase).culture;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(purchaseCulture);
            PurchasesModel.TicketInfoModel model = new MasterChartDataModel.Purchases().GetTicketInfo(PurchaseID, PurchaseTicketID);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(model, "", (string)null, urlMethod, request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return PartialView("_PurchaseTicketPartial", model);
        }

        [HttpPost]
        public async Task<JsonResult> FindLead(Guid leadID)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            LeadModel.Fields.LeadGeneralInformation response = lead.Find(leadID);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(new { leadDetails = response });
        }

        [HttpPost]
        public JsonResult GetItemLogs(string referenceID, string referenceText)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;
            ///*new Thread(() =>
            //{
            //    try
            //    {//logs
            //        AdminDataModel.CreateTableUserLogActivity(lead.GetItemLogs(referenceID, referenceText), "Get", (string)null, urlMethod, request);
            //    }
            //    catch (Exception ex)
            //    {
            //        AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
            //    }
            //}).Start();*/
            return Json(lead.GetItemLogs(referenceID, referenceText));
        }

        [HttpPost]
        public async Task<JsonResult> DeleteLead(Guid leadID)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            AttemptResponse attempt = lead.TryToDelete(leadID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(leadID, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                GeneralInformation_LeadID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult MassUpdate(LeadModel.Views.MassUpdate lvm)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            AttemptResponse attempt = lead.MassUpdate(lvm);
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

        public JsonResult MassInsert(LeadModel.Views.MassUpdate lvm)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            AttemptResponse attempt = lead.MassInsert(lvm);
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

        public JsonResult MassSending(LeadModel.Views.MassUpdate lvm)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            AttemptResponse attempt = lead.MassSending(lvm);
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

        //public JsonResult DuplicateLeads(LeadModel.Views.MassUpdate lvm)
        //{
        //    MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
        //    //AttemptResponse attempt = new AttemptResponse();
        //    //if (lvm.MassUpdate_Coincidences != null)
        //    //{
        //    //    var _coincidences = lvm.MassUpdate_Coincidences.Replace("'", "");
        //    //    var _now = DateTime.Now;
        //    //    foreach (var i in _coincidences.Split(','))
        //    //    {
        //    //        var _leadID = Guid.Parse(i);
        //    //        var _lead = db.tblLeads.Single(m => m.leadID == _leadID);
        //    //        var newLead = new tblLeads();

        //    //        Mapper.CreateMap<tblLeads, tblLeads>();
        //    //        newLead = Mapper.Map<tblLeads, tblLeads>(_lead);

        //    //        //newLead = Reflection.CloneObject(_lead) as tblLeads;

        //    //        newLead.leadStatusID = lvm.DuplicateLeadsModel.DuplicateLeads_LeadStatus;
        //    //        newLead.assignedToUserID = lvm.DuplicateLeadsModel.DuplicateLeads_AssignToUser;
        //    //        newLead.inputDateTime = _now;
        //    //    }
        //    //    //db.SaveChanges();
        //    //}

        //    AttemptResponse attempt = lead.DuplicateLeads(lvm);
        //    string errorLocation = "";
        //    if (attempt.Exception != null)
        //    {
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ItemID = attempt.ObjectID,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
        //        InnerException = Debugging.GetInnerException(attempt.Exception)
        //    });
        //}

        //public JsonResult FillDrpUsersPerTerminal(string terminals)
        //{
        //    return Json(MasterChartDataModel.LeadsCatalogs.FillDrpUsersPerTerminal(terminals), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetResortsByDestination(int destinationID)
        {
            return Json(PlaceDataModel.GetResortsByDestination(destinationID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetResortByID(long resortID)
        {
            return Json(PlaceDataModel.GetResortByID(resortID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBrokerContractsByTerminal(int terminalID)
        {
            return Json(BrokerDataModel.GetBrokerContractsByTerminal(terminalID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRoomTypesByPlace(int placeID)
        {
            return Json(PlaceDataModel.GetRoomTypesByPlace(placeID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOptionTypesByPlace(int optionType, int resortID)
        {
            return Json(MasterChartDataModel.LeadsCatalogs.FillDrpOptionTypes(optionType, resortID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOptionPrice(int optionID)
        {
            return Json(MasterChartDataModel.LeadsCatalogs.FillDrpOptionPrice(optionID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOptionCreditAmount(int optionID)
        {
            return Json(MasterChartDataModel.LeadsCatalogs.GetOptionCreditAmount(optionID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOptionCategories(int placeID)
        {
            return Json(MasterChartDataModel.LeadsCatalogs.GetOptionCategories(), JsonRequestBehavior.AllowGet);
        }

        public static string DateReverter(Match m)
        {
            var newValue = "";
            if (m.ToString().Contains("/Date("))
            {
                //var z = new JavaScriptSerializer().Serialize(m.ToString());// "/Date(1497544951597)/";
                string strTicks = m.ToString().Split(new char[] { '(', ')' })[1];
                DateTime veriOldDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                //var b = new DateTime(TimeZoneInfo.ConvertTimeToUtc(long.Parse(strTicks) * 10000 + veriOldDate.Ticks));
                var _ = new DateTime(long.Parse(strTicks) * 10000);
                var __ = new DateTime(_.Ticks + veriOldDate.Ticks);
                var ___ = TimeZoneInfo.ConvertTimeFromUtc(__, TimeZoneInfo.Local);

                //var a = TimeZoneInfo.ConvertTimeToUtc(new DateTime(long.Parse(strTicks) * 10000 + veriOldDate.Ticks));
                //var _a = TimeZoneInfo.ConvertTimeToUtc(new DateTime(veriOldDate.Ticks));
                //var __a = new DateTime(veriOldDate.Ticks);
                //var b = TimeZoneInfo.ConvertTimeToUtc(new DateTime(long.Parse(strTicks) * 10000 + 621356076000000000));
                //var c = new DateTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(long.Parse(strTicks))).Ticks * 10000 + veriOldDate.Ticks);
                //var _c = new DateTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(long.Parse(strTicks))).Ticks * 10000 + (veriOldDate.Ticks / 10000));
                //var d = new DateTime(TimeZoneInfo.ConvertTimeToUtc(new DateTime(long.Parse(strTicks))).Ticks * 10000 + 621356076000000000);
                //var e = new DateTime(TimeZoneInfo.ConvertTimeFromUtc(new DateTime(long.Parse(strTicks) * 10000), TimeZoneInfo.Utc).Ticks + veriOldDate.Ticks);
                //var _e = new DateTime(TimeZoneInfo.ConvertTimeFromUtc(new DateTime(long.Parse(strTicks) * 10000), TimeZoneInfo.Local).Ticks + veriOldDate.Ticks);
                //var f = new DateTime(long.Parse(strTicks) * 10000 + 621356076000000000);

                long ticks = long.Parse(strTicks) * 10000 + veriOldDate.Ticks;
                //DateTime theDate = new DateTime(ticks);
                DateTime theDate = ___;
                //DateTime theDate = new DateTime(ticks).ToLocalTime();
                newValue = theDate.ToString();
                ///pi.SetValue(jsonPropertyValue, newValue);
            }
            return newValue;
        }

        //public class emailItem
        //{
        //    public string Email { get; set; }         
        //}

        /// <summary>
        /// Converts JSON content to its ListType for each property with the Custon Data Annotation [ListType] defined.
        /// </summary>
        /// <param name="lgim"></param>
        static void parseListTypes<T>(ref T lgim)
        {
            //parsear el Json stringified de los campos relacionados con tablas.
            JavaScriptSerializer jss = new JavaScriptSerializer();

            //get properties with the dataAttribute "ListType" 
            Type attribute = typeof(ListTypeAttribute);
            var props = lgim.GetType().GetProperties();
            var listTypeProps = props.Where(prop => Attribute.IsDefined(prop, attribute));
            // iterate properties to parse them properly to it's corresponding type.
            foreach (var p in listTypeProps)
            {

                object[] attributes = p.GetCustomAttributes(attribute, true);
                ListTypeAttribute da = attributes[0] as ListTypeAttribute;
                //ListTypeAttribute da = attributes[0] as ListTypeAttribute;
                // instead of (ListTypeAttribute)attributes[0]
                PropertyInfo uiProperty = lgim.GetType().GetProperty(p.Name);
                string[] uiList = (string[])uiProperty.GetValue(lgim);

                //Make sure things like /Date(1370626189827)/  (including the slashes)  are replaced by its serverSide valid date
                uiList[0] = Regex.Replace(uiList[0], @"\/Date\(\d*\)/", new MatchEvaluator(DateReverter));
                var value = jss.Deserialize(uiList[0], da.Type);
                lgim.GetType().GetProperty(p.Name).SetValue(lgim, value);
            }
        }

        //static void parseListTypes(ref LeadModel.Fields.GeneralInformation lgim)
        //{
        //    //parsear el Json stringified de los campos relacionados con tablas.
        //    JavaScriptSerializer jss = new JavaScriptSerializer();

        //    //get properties with the dataAttribute "ListType" 
        //    Type attribute = typeof(ListTypeAttribute);
        //    var props = lgim.GetType().GetProperties();
        //    var listTypeProps = props.Where(prop => Attribute.IsDefined(prop, attribute));
        //    // iterate properties to parse the properly to it's corresponding type.
        //    foreach (var p in listTypeProps)
        //    {
        //        object[] attributes = p.GetCustomAttributes(attribute, true);
        //        ListTypeAttribute da = attributes[0] as ListTypeAttribute;
        //        // instead of (ListTypeAttribute)attributes[0]
        //        PropertyInfo uiProperty = lgim.GetType().GetProperty(p.Name);
        //        string[] uiList = (string[])uiProperty.GetValue(lgim);
        //        var value = jss.Deserialize(uiList[0], da.Type);
        //        lgim.GetType().GetProperty(p.Name).SetValue(lgim, value);

        //    }
        //}

        //Controller
        // GET: /Vestidos/Aleatorios
        public ActionResult GetReservationPanel(Guid leadID)
        {
            //reservationgere new HotelReservationModel { };
            HotelReservationModel.Views.ReservationGeneralInformation reservation = new HotelReservationModel.Views.ReservationGeneralInformation() { LeadID = leadID };
            return PartialView("_HotelReservationsPartial", reservation);
        }

        public JsonResult FindReservation(Guid reservationID)
        {
            MasterChartDataModel.HotelReservation reservation = new MasterChartDataModel.HotelReservation();
            HotelReservationModel.Fields.ReservationGeneralInformation reservationDetails = reservation.Find(reservationID);
            var result = new JsonNetResult
            {
                Data = new { reservationDetails = reservationDetails },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Settings = { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult SendConfirmationLetter(Guid reservationID, int evtID)
        {
            MasterChartDataModel.HotelReservation rsv = new MasterChartDataModel.HotelReservation();
            AttemptResponse attempt = rsv.SendLetter(reservationID, evtID);
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
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult FindBillingInfo(long billingInfoID)
        {
            MasterChartDataModel.BillingInfo billingInfo = new MasterChartDataModel.BillingInfo();
            BillingInfoModel.Fields.BillingInfo billingInfoDetails = billingInfo.Find(billingInfoID);
            return Json(new { billingInfoDetails });
        }

        public JsonResult SearchReservations(Guid leadID)
        {
            MasterChartDataModel.HotelReservation reservation = new MasterChartDataModel.HotelReservation();
            List<HotelReservationModel.Items.ReservationSearchResults> leadReservations = reservation.SearchReservations(leadID);

            return Json(new { leadReservations });
        }

        public JsonResult SearchBillingInfo(Guid leadID)
        {
            MasterChartDataModel.BillingInfo billingInfo = new MasterChartDataModel.BillingInfo();
            List<BillingInfoModel.Items.BillingInfoSearchResults> leadBillingInfo = billingInfo.SearchBillingInfo(leadID);

            return Json(new { leadBillingInfo });
        }

        public JsonResult SearchPendingCharges(Guid leadID)
        {
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            List<LeadModel.Items.ReservationPendingCharges> pendingCharges = lead.SearchPendingCharges(leadID);

            return Json(new { pendingCharges });
        }
        public JsonResult SaveLead(LeadModel.Fields.LeadGeneralInformation lgim)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            MasterChartDataModel.Lead lead = new MasterChartDataModel.Lead();
            parseListTypes<LeadModel.Fields.LeadGeneralInformation>(ref lgim);

            //AttemptResponse attempt = (lgim.GeneralInformation_LeadID != null) ?
            //                        lead.TryToUpdate(lgim) :
            //                        lead.TryToCreate(lgim);

            AttemptResponse attempt = new AttemptResponse();

            if (lgim.GeneralInformation_DuplicateLead == true && lgim.GeneralInformation_LeadID != null)
            {
                //duplicate
                attempt = lead.TryToDuplicate(lgim);
            }
            else if (lgim.GeneralInformation_DuplicateLead != true && lgim.GeneralInformation_LeadID != null)
            {
                //update
                attempt = lead.TryToUpdate(lgim);
            }
            else if (lgim.GeneralInformation_LeadID == null)
            {
                //create
                attempt = lead.TryToCreate(lgim);
            }


            // AttemptResponse attempt = new AttemptResponse();

            string gettingErrorLocationException = "";

            string errorLocation = "";
            if (attempt.Exception != null)
            {
                try
                {
                    if (attempt.Exception != null)
                    {
                        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
                    }
                }
                catch (Exception gettingErrorLocationEx)
                {

                    gettingErrorLocationException = "GettingErrorLocationMessage<[{" + gettingErrorLocationEx.Message + "}]><br />";

                    if (gettingErrorLocationEx.InnerException != null)
                    {
                        gettingErrorLocationException += "GettingErrorLocationInnerException<[{" + gettingErrorLocationEx.InnerException + "}]><br />";
                    }
                    else
                    {
                        gettingErrorLocationException += "GettingErrorLocationInnerException<[{null}]><br />";
                    }


                }

            }

            var ResponseType = attempt.Type;
            var GeneralInformation_LeadID = attempt.ObjectID;
            var ResponseMessage = attempt.Message;
            var ExceptionMessage = Debugging.GetMessage(attempt.Exception); // (attempt.Exception != null) ? attempt.Exception.Message : "";
            var InnerException = gettingErrorLocationException + errorLocation + Debugging.GetInnerException(attempt.Exception);
            //if (attempt.Exception != null && attempt.Exception.InnerException != null)
            //{
            //    InnerException += attempt.Exception.InnerException.ToString();
            //}

            return Json(new
            {
                ResponseType = ResponseType,
                GeneralInformation_LeadID = GeneralInformation_LeadID,
                ResponseMessage = ResponseMessage,
                ExceptionMessage = ExceptionMessage,
                InnerException = InnerException
            });
        }

        public JsonResult SaveReservation(HotelReservationModel.Fields.ReservationGeneralInformation rgim)
        {
            var cosa = Request.Form;


            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");




            MasterChartDataModel.HotelReservation reservation = new MasterChartDataModel.HotelReservation();
            rgim.ParsePropertiesBackToObjects();


            //parseListTypes<HotelReservationModel.Fields.ReservationGeneralInformation>(ref rgim);
            AttemptResponse attempt = (rgim.ReservationID != null) ?
                                    reservation.TryToUpdate(rgim) :
                                    reservation.TryToCreate(rgim);



            // AttemptResponse attempt = new AttemptResponse();

            string errorLocation = "";
            if (attempt.Exception != null)
            {
                //Debugging debug = new Debugging();
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            var ResponseType = attempt.Type;
            var ObjectIds = attempt.ObjectID;
            var ResponseMessage = attempt.Message;
            var ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "";
            var InnerException = "";

            try
            {
                InnerException = (attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "";
            }
            catch (Exception ex)
            {

                ;
            }


            return Json(new
            {
                ResponseType = ResponseType,
                ObjectIds = ObjectIds,
                ResponseMessage = ResponseMessage,
                ExceptionMessage = ExceptionMessage,
                InnerException = InnerException
            });
        }

        public JsonResult SaveBillingInfo(BillingInfoModel.Fields.BillingInfo bim)
        {
            var cosa = Request.Form;
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            MasterChartDataModel.BillingInfo billingInfo = new MasterChartDataModel.BillingInfo();
            bim.ParsePropertiesBackToObjects();


            //parseListTypes<HotelReservationModel.Fields.ReservationGeneralInformation>(ref rgim);
            AttemptResponse attempt = (bim.billingInfoID != null) ?
                                    billingInfo.TryToUpdate(bim) :
                                    billingInfo.TryToCreate(bim);



            // AttemptResponse attempt = new AttemptResponse();

            string errorLocation = "";
            if (attempt.Exception != null)
            {
                //Debugging debug = new Debugging();
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            var ResponseType = attempt.Type;
            var ObjectIds = attempt.ObjectID;
            var ResponseMessage = attempt.Message;
            var ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "";
            var InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "";

            return Json(new
            {
                ResponseType = ResponseType,
                ObjectIds = ObjectIds,
                ResponseMessage = ResponseMessage,
                ExceptionMessage = ExceptionMessage,
                InnerException = InnerException
            });
        }

        [HttpPost]
        //public MasterChartDataModel.Import.FineUploaderResult UploadFile(MasterChartDataModel.Import.FineUpload upload, bool hasHeader)
        public MasterChartDataModel.Import.FineUploaderResult UploadFile(PictureDataModel.FineUpload upload, bool hasHeader)
        {
            MasterChartDataModel.Import import = new MasterChartDataModel.Import();
            return (import.UploadFile(upload, hasHeader));
        }

        public JsonResult ImportData(bool[] comparableFlags, string[] fileData, string[] fields, string[] values, string[] secValues, string[] columns, string[] sections)
        {
            MasterChartDataModel.Import import = new MasterChartDataModel.Import();
            AttemptResponse attempt = import.ImportData(comparableFlags, fileData, fields, values, secValues, columns, sections);

            string errorLocation = "";
            if (attempt.Exception != null)
            {
                if (attempt.Exception.StackTrace != null)
                    errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                GeneralInformation_LeadID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetDDLData(string itemType, string itemID)
        {
            //if (itemID == null)
            //{
            //    MasterChartDataModel.Import import = new MasterChartDataModel.Import();
            //    return Json(import.GetDDLData(itemType), JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    MasterChartDataModel.Purchases purchases = new MasterChartDataModel.Purchases();
            //    return Json(purchases.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
            //}
            MasterChartDataModel.Common mdm = new MasterChartDataModel.Common();
            return Json(mdm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTablesFields(int[] sysWorkGroup, string[] tableName)
        {
            return Json(MasterChartDataModel.LeadsCatalogs.FillDrpTableFields(sysWorkGroup, tableName), JsonRequestBehavior.AllowGet);
        }

        public void DownloadImportErrors()
        {
            var dirErrors = Server.MapPath(@"~/Content/files/errors/");
            var errorFilePath = Path.Combine(dirErrors, "Errors_Importation.txt");
            Response.ClearContent();
            Response.Clear();
            Response.ClearHeaders();
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", "attachment; filename=Errors_Importation.txt;");
            Response.TransmitFile(errorFilePath);
        }

        public JsonResult ValidateData(string fileName)
        {
            MasterChartDataModel.Import import = new MasterChartDataModel.Import();
            AttemptResponse attempt = import.ValidateData(fileName);

            string errorLocation = "";
            if (attempt.Exception != null)
            {
                if (attempt.Exception.StackTrace != null)
                    errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;
            ///*new Thread(() =>
            //{
            //    try
            //    {//logs
            //        AdminDataModel.CreateTableUserLogActivity(json.Serialize(attempt), "", (string)null);
            //    }
            //    catch (Exception ex)
            //    {
            //        AdminDataModel.UserLogsActivitySendEx(ex);
            //    }
            //}).Start();*/
            return Json(new
            {
                ResponseType = attempt.Type,
                GeneralInformation_LeadID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        public class JsonNetResult : JsonResult
        {
            public JsonNetResult()
            {
                Settings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error
                };
            }

            public Newtonsoft.Json.JsonSerializerSettings Settings { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("JSON GET is not allowed");

                HttpResponseBase response = context.HttpContext.Response;
                response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

                if (this.ContentEncoding != null)
                    response.ContentEncoding = this.ContentEncoding;
                if (this.Data == null)
                    return;

                var scriptSerializer = Newtonsoft.Json.JsonSerializer.Create(this.Settings);

                using (var sw = new StringWriter())
                {
                    scriptSerializer.Serialize(sw, this.Data);
                    response.Write(sw.ToString());
                }
            }
        }
        public abstract class BaseController : Controller
        {
            protected override JsonResult Json(object data, string contentType,
               System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
            {
                return new JsonNetResult
                {
                    Data = data,
                    ContentType = contentType,
                    ContentEncoding = contentEncoding,
                    JsonRequestBehavior = behavior
                };
            }
        }
        public JsonResult getServerTimestamp()
        {
            DateTime dt = DateTime.Now;
            var result = new JsonNetResult
            {
                Data = new
                {
                    year = dt.Year,
                    month = dt.Month,
                    day = dt.Day,
                    hour = dt.Hour,
                    minute = dt.Minute,
                    second = dt.Second,
                    datetime = dt
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Settings = { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult MakeCharge(int BillingInfoID, string ReservationPaymentIds, Guid LeadID)
        {
            //make sure the billingInfo and the selected payments belongs to the current LeadID
            bool Ok = true;
            string Error = "";
            var BillingInfo = from x in db.tblBillingInfo
                              where x.leadID == LeadID && x.billingInfoID == BillingInfoID
                              select x;

            if (BillingInfo.Count() < 1) { Error = "Billing Info Not Found."; Ok = false; }

            var ArrReservationPaymentIds = ReservationPaymentIds.Split(',').Select(p => long.Parse(p)).ToArray();
            // get only pending payments (with no authCode AKA invoice number), to prevent duplicated 
            // charges in case 2 agents are working with the same reservation.
            var ReservationPayments = from x in db.tblPaymentDetails
                                      where ArrReservationPaymentIds.Contains(x.paymentDetailsID)
                                      && x.tblReservations.tblLeads.leadID == LeadID
                                      && (x.tblMoneyTransactions.authCode == null || x.tblMoneyTransactions.authCode == "")
                                      && (x.deleted == null || x.deleted == false)
                                      select x;

            var totalPaymentsFoundOnDB = ReservationPayments.Count();
            if (totalPaymentsFoundOnDB == 0) { Error = "No Pending Payments Found."; Ok = false; }
            if ((ArrReservationPaymentIds.Count() != totalPaymentsFoundOnDB)) { Error = "Mismatching between payments"; Ok = false; }

            if (!Ok)
            {
                return Json(new
                {
                    ResponseType = Attempt_ResponseTypes.Error,
                    ObjectIds = LeadID,
                    ResponseMessage = Error + " Payment cancelled.",
                    ExceptionMessage = "",
                    InnerException = ""
                });
            } //else continue


            IDictionary<string, double> subtotales = new Dictionary<string, double>();
            double totalToCharge = 0;

            foreach (var p in ReservationPayments)
            {
                double amount = double.Parse(p.amount.ToString());
                totalToCharge += amount;
                string abbreviations = p.tblChargeTypes.chargeTypeAbbreviation + "-" + p.tblChargeDescriptions.chargeDescriptionAbbreviation;
                if (!subtotales.ContainsKey(abbreviations)) { subtotales.Add(abbreviations, 0.0); }
                subtotales[abbreviations] += amount;
            }

            // the payment reference needs the Broker contract abbr, and the chargetype-chargedescriptions with ther corresponding subtotals.

            string brokerAbbr = "";

            try
            {
                brokerAbbr = BillingInfo.First().tblLeads.tblBrokerContracts.brokerContractAbbr;
            }
            catch (Exception)
            {

                throw;
            }


            string paymentReference = brokerAbbr;
            foreach (var x in subtotales)
            {
                paymentReference += "|" + x.Key + "=" + x.Value;
            }

            //make sure the reference lenght is <= 50
            if (paymentReference.Length > 50)
            {
                paymentReference = paymentReference.Substring(0, 50);
            }

            // assuming that in the client side we only allow to group payments that are going to be charged 
            // under the same merchant account, then we use the first one to get
            // the merchantAccount information
            MerchantAccountDataModel.MerchantAccountInfo merchant = MerchantAccountDataModel.GetMerchantAccount(ReservationPayments.First().paymentDetailsID);

            if (merchant == null)
            {
                Error = "No Merchant Account Found.";

                return Json(new
                {
                    ResponseType = Attempt_ResponseTypes.Error,
                    ObjectIds = LeadID,
                    ResponseMessage = Error + " Payment cancelled.",
                    ExceptionMessage = "",
                    InnerException = ""
                });
            } //else continue

            RescomDataModel.ApplyPayment_Data Data = new RescomDataModel.ApplyPayment_Data();
            Data.UserName = merchant.Username;
            Data.Password = merchant.Password;

            Data.Card_Holder = BillingInfo.First().cardHolderName;
            Data.Card_Holder_Address = BillingInfo.First().address;
            Data.Card_Holder_Zip = BillingInfo.First().zipcode;
            Data.Card_Number = mexHash.mexHash.DecryptString(BillingInfo.First().cardNumber);

            var expiration = BillingInfo.First().cardExpiry.Split('/');

            Data.Expiration_Date = expiration[0] + "/01/" + expiration[1];
            Data.Reference_Code = paymentReference;
            Data.Transaction_Amount = totalToCharge;

            RescomDataModel.ApplyPayment_Response result;

            try
            {
                result = RescomDataModel.ApplyPayment(Data);
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    ResponseType = Attempt_ResponseTypes.Error,
                    ObjectIds = LeadID,
                    ResponseMessage = "Error while processing the charge.",
                    ExceptionMessage = ex.Message,
                    InnerException = ex.InnerException.ToString() ?? ""
                });
            }

            //continue with the process

            string responseMessage = "<strong>Charge Summary</strong><br />Amount :<strong>$ " + totalToCharge.ToString("N2") + "</strong> :";
            responseMessage += "<br />" + RescomDataModel.GetPaymentErrorDescription(result.Error_Code);

            // update the just charged payments with the corresponding Authorization Code (invoice number).
            var dbReservationPaymentsIds = (from x in ReservationPayments select x.paymentDetailsID).ToArray();
            ChargesDataModel.SaveCharge_Data scData = new ChargesDataModel.SaveCharge_Data();

            scData.paymentResponse = result;
            scData.merchanAccountID = merchant.MerchantAccountID;
            scData.moneyTransactionTypeID = 1; //Payment [1] or Refund [2] // for this case, Within this method (makeCharge), this is always 1
            scData.terminalID = merchant.TerminalID;
            scData.reservationPaymentIds = ArrReservationPaymentIds;
            scData.Reference_Code = Data.Reference_Code;
            scData.billingInfoID = BillingInfo.First().billingInfoID;

            ChargesDataModel.SaveChargeResponse(scData);


            var ResponseType = (result.Error_Code == 0) ? 1 : -1;
            var ObjectIds = LeadID;
            var ResponseMessage = responseMessage;
            var ExceptionMessage = "";
            var InnerException = "";

            return Json(new
            {
                ResponseType = ResponseType,
                ObjectIds = ObjectIds,
                ResponseMessage = ResponseMessage,
                ExceptionMessage = ExceptionMessage,
                InnerException = InnerException
            });

        }

        //Purchases
        public JsonResult AllowAccessToServices()
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.AllowAccessToServices();
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
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public ActionResult GetLeadPurchases(string PurchaseInfo_Lead)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            PurchasesModel pm = new PurchasesModel();
            pm.ListPurchases = mdm.GetLeadPurchases(PurchaseInfo_Lead);

            return PartialView("_PurchasesSearchResultsPartial", pm);
        }

        public async Task<JsonResult> SaveFastSale(FastSaleModel.FastSaleInfoModel model)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.SaveFastSale(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public async Task<JsonResult> SavePurchase(PurchasesModel.PurchaseInfoModel model)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.SavePurchase(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public async Task<JsonResult> GetPurchase(string PurchaseInfo_PurchaseID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = mdm.GetPurchase(PurchaseInfo_PurchaseID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        public async Task<JsonResult> DeletePurchase(string targetID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.DeletePurchase(targetID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(targetID, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public async Task<ActionResult> GetPurchaseServices(string PurchaseService_PurchaseID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            PurchasesModel model = new PurchasesModel();
            var response = mdm.GetPurchaseServices(PurchaseService_PurchaseID);
            model.ListPurchasesServices = response;
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(response, "GetPurchaseServices", (string)null, urlMethod, request, terminalID));
            return PartialView("_PurchasesServicesResultsPartial", model);
        }

        public async Task<JsonResult> SavePurchaseService(PurchasesModel.PurchaseServiceModel model)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.SavePurchaseService(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public async Task<JsonResult> GetPurchaseService(int PurchaseService_PurchaseServiceID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = mdm.GetPurchaseService(PurchaseService_PurchaseServiceID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeletePurchaseService(int targetID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.DeletePurchaseService(targetID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(targetID, "Delete", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult GetCouponRef(Guid purchaseID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var couponRef = mdm.GetCouponRef(purchaseID);
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(couponRef, "Get", (string)null,urlMethod,Request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(couponRef);
        }

        public JsonResult PrintAllCoupons(Guid purchaseID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var coupRefObj = mdm.PrintAllCoupons(purchaseID);
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(coupRefObj, "Get", (string)null, urlMethod, Request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(coupRefObj);
        }

        public JsonResult GetCouponRefObj(long purchaseServiceID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var coupRefObj = mdm.GetCouponRefObj(purchaseServiceID);
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(coupRefObj, "Get", (string)null, urlMethod, Request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(coupRefObj);
        }

        public JsonResult GetCouponString(string url)
        {
            using (WebClient client = new WebClient())
            {
                var _string = client.DownloadString(url);
                _string = _string.Replace("<link href=\"/Content/themes/mex/css/main.css\" rel=\"stylesheet\" />", "");
                _string = _string.Replace("\"/", "\"//eplatfront.villagroup.com/");
                //mike
                //_string = _string.Replace("<label for=\"username\"></label>", "<label for=\"username\">"+ new UserSession().User +"</label>");
                var _script = _string.Substring(_string.IndexOf("<script>") + 8, (_string.IndexOf("</script>") - _string.IndexOf("<script>") - 9));
                _string = _string.Replace(_script, "");
                _script = url.IndexOf('#') != -1 ? _script.Replace("window.location.hash", ("\"#" + url.Split('#')[1] + "\"")) : _script;
                return Json(new { _coupon = _string, _script = _script });
            }
        }

        public JsonResult IsTransportationService(long serviceID)
        {
            var model = new MasterChartDataModel.Purchases();
            return Json(model.IsTransportationService(serviceID), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetNextCouponFolio(int pointOfSaleID, long terminalID, string timeSpan)
        public JsonResult GetNextCouponFolio(Guid purchaseID, string timeSpan)
        {
            var model = new MasterChartDataModel.Purchases();
            return Json(model.GetNextCouponFolio(purchaseID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExchangeRates(string date, int pointOfSaleID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            //if (date != null)
            //{
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var rates = mdm.GetExchangeRates(DateTime.Parse(date), pointOfSaleID);
            /*new Thread(() =>
            {
            try
            {//logs
                AdminDataModel.CreateTableUserLogActivity(rates, "Get", (string)null, urlMethod,Request);
            }
            catch (Exception ex)
            {
                AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
            }
            }).Start();*/
            return Json(rates, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetPurchasePayments(string PurchasePayment_Purchase)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            PurchasesModel model = new PurchasesModel();
            var response = mdm.GetPurchasePayments(PurchasePayment_Purchase);
            model.ListPurchasesPayments = response;
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return PartialView("_PurchasesPaymentsResultsPartial", model);
        }

        public ActionResult GetPurchaseTickets(string PurchasePayment_Purchase)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            PurchasesModel model = new PurchasesModel();
            model.ListPurchaseTickets = mdm.GetPurchaseTickets(PurchasePayment_Purchase);

            return PartialView("_PurchaseTicketsResultsPartial", model);
        }

        public async Task<ActionResult> GetPurchasePayment(string PurchasePayment_PaymentDetailsID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = mdm.GetPurchasePayment(PurchasePayment_PaymentDetailsID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> SavePayment(PurchasesModel.PurchasePaymentModel model)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.SavePayment(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public async Task<JsonResult> DeletePayment(int targetID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            AttemptResponse attempt = mdm.DeletePayment(targetID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(targetID, "Delete", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        //public JsonResult _GetTicketInfo(string PurchaseID)
        //{
        //    MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
        //    return Json(mdm.GetTicketInfo(PurchaseID));
        //}

        public JsonResult CanApplyPromo(Guid PurchaseService_Purchase)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var applyPromo = mdm.CanApplyPromo(PurchaseService_Purchase);
            /*new Thread(() =>
            {
                try
                {//logs
                    var json = new System.Web.Script.Serialization.JavaScriptSerializer();
                    AdminDataModel.CreateTableUserLogActivity(applyPromo, "", (string)null, urlMethod, Request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(applyPromo, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetPromoInfo(long PurchaseService_Promo)
        public JsonResult GetPromoInfo(long promoID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var PromoInfo = mdm.GetPromoInfo(promoID);

            return Json(PromoInfo, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ApplyPromoToCoupons(long promoID, string purchase_services)
        //{
        //    MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
        //    AttemptResponse attempt = mdm.ApplyPromoToCoupons(promoID, purchase_services);

        //    string errorLocation = "";
        //    if (attempt.Exception != null)
        //    {
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }

        //    //var request = HttpContext.Request;
        //    //var urlMethod = new UserSession().Url;
        //    //Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request));

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ItemID = attempt.ObjectID,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
        //        InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
        //    });
        //}

        public JsonResult GetExchangeRateOfPurchase(Guid purchaseID)
        {
            return Json(MasterChartDataModel.LeadsCatalogs.GetExchangeRateOfPurchase(purchaseID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSpecificRate(DateTime? date, string currency, long terminalid, int pos)
        {
            return Json(MasterChartDataModel.Purchases.GetSpecificRate(date, currency, terminalid, pos));
        }

        public JsonResult SetPurchaseServiceAsIssued(int PurchaseService_PurchaseServiceID)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var purchaseServices = mdm.SetPurchaseServiceAsIssued(PurchaseService_PurchaseServiceID);
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(purchaseServices, "", (string)null, urlMethod,Request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(purchaseServices);
        }

        public JsonResult SetPurchaseServicesAsIssued(Guid PurchaseInfo_Purchase)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var purchaseServices = mdm.SetPurchaseServicesAsIssued(PurchaseInfo_Purchase);
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(purchaseServices, "", (string)null, urlMethod, Request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return Json(purchaseServices);
        }

        public async Task<JsonResult> SendCouponsByEmail(Guid PurchaseService_Purchase, string PurchaseService_Service, bool SendToProvider, bool SendToOther, string OtherMail)
        {
            MasterChartDataModel.Purchases mdm = new MasterChartDataModel.Purchases();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            AttemptResponse attempt = mdm.SendCouponsByEmail(PurchaseService_Purchase, PurchaseService_Service, SendToProvider, SendToOther, OtherMail);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(attempt, "", (string)null, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public JsonResult GetBuyer(Guid purchaseID)
        {
            return Json(new MasterChartDataModel.Purchases().GetBuyer(purchaseID), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetBudgetUsedInWeek(int opcID, int promotionTeamID, string paymentDate, long terminalID)
        //{
        //    return Json(new MasterChartDataModel.Purchases().GetBudgetUsedInWeek(opcID, promotionTeamID, paymentDate, terminalID));
        //}

        public async Task<JsonResult> GetChargeBackTicketInfo(Guid purchaseID, string printFromDate, string printToDate)
        {
            var response = new MasterChartDataModel.Purchases().GetChargeBackTicketInfo(purchaseID, printFromDate, printToDate);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        public JsonResult GetChargeVoucher(Guid purchaseID)
        {
            return Json(new MasterChartDataModel.Purchases().GetChargeVoucher(purchaseID));
        }

        public JsonResult GetBankCommission(long terminalID, DateTime date, int? cardTypeID)
        {
            if (cardTypeID == null)
            {
                return Json(new { commission = MasterChartDataModel.Purchases.GetBankCommission(terminalID, date) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { commission = MasterChartDataModel.Purchases.GetBankCommission(terminalID, date, cardTypeID) }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IsPoSOnline(int pointOfSaleID)
        {
            return Json(new { isOnline = MasterChartDataModel.LeadsCatalogs.IsPoSOnline(pointOfSaleID) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LimitRefundsPerType(long terminalID)
        {
            return Json(new { limitRefunds = MasterChartDataModel.LeadsCatalogs.LimitRefundsPerType(terminalID) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QuickSaleDependantLists()
        {
            return Json(MasterChartDataModel.LeadsCatalogs.QuickSaleDependantLists(), JsonRequestBehavior.AllowGet);
        }
    }
}
