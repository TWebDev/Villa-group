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
using System.Threading;
using System.Threading.Tasks;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Controllers.CRM
{
    public class ReportsController : Controller
    {
        // GET: /crm/reports/get/report
        [Authorize]
        public ActionResult Index(string report)
        {
            if (AdminDataModel.VerifyAccess())
            {
                ReportViewModel rvm = new ReportViewModel();
                var reportTag = ReportDataModel.ReportsCatalogs.Capitals(report);
                long? sysComponentForReport = AdminDataModel.GetSysComponentIDByURL("/crm/reports/get/" + report);
                ViewData.Model = new ReportViewModel
                {
                    //ReportsModel = new ReportsModel(),
                    ReportName = report,
                    ReportTag = reportTag
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Index2(string report)
        {
            if (AdminDataModel.VerifyAccess())
            {
                ReportViewModel rvm = new ReportViewModel();
                var reportTag = ReportDataModel.ReportsCatalogs.Capitals(report);
                long? sysComponentForReport = AdminDataModel.GetSysComponentIDByURL("/crm/reports2/get/" + report);
                ViewData.Model = new ReportViewModel
                {
                    ReportName = report,
                    ReportTag = reportTag
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
            return result;
        }

        [Authorize]
        public PartialViewResult RenderReports(string reportName, bool? cache = true)
        {
            var returnPartial = "";
            dynamic model;
            switch (reportName)
            {
                case "indicatorsPerResort":
                    {
                        returnPartial = "_SearchIndicatorsPerResortPartial";
                        model = new IndicatorsModel.SearchIndicatorsModel();
                        return PartialView(returnPartial, model);
                    }
                case "indicators":
                    {
                        returnPartial = "_SearchIndicatorsPartial";
                        //returnPartial = "_SearchIndicatorsPerResortPartial";
                        model = new IndicatorsModel.SearchIndicatorsModel();
                        return PartialView(returnPartial, model);
                    }
                case "pipeline":
                    {
                        returnPartial = "_SearchPipelinePartial";
                        model = new PipelineModel.SearchPipelineModel();
                        return PartialView(returnPartial, model);
                    }
                case "exchangeTour":
                    {
                        returnPartial = "_SearchExchangeToursPartial";
                        model = new ExchangeTourModel.SearchExchangeTourModel();
                        return PartialView(returnPartial, model);
                    }
                case "preBookedArrivals":
                    {
                        returnPartial = "_SearchPreBookedArrivalsPartial";
                        model = new PreBookedArrivalsModel.SearchPreBookedArrivalsModel();
                        return PartialView(returnPartial, model);
                    }
                case "newReferralLeads":
                    {
                        //returnPartial = "_SearchNewReferralLeadsPartial";
                        returnPartial = "_SearchNewReferralsPartial";
                        //model = new NewReferralLeadsModel.SearchNewReferralLeadsModel();
                        model = new NewReferralsModel.SearchNewReferralsModel();
                        return PartialView(returnPartial, model);
                    }
                case "weeklyReport":
                    {
                        returnPartial = "_SearchWeeklyPartial";
                        model = new WeeklyModel.SearchWeeklyModel();
                        return PartialView(returnPartial, model);
                    }
                case "reservationsMade":
                    {
                        returnPartial = "_SearchReservationsMadePartial";
                        model = new ReservationsMadeModel.SearchReservationsMadeModel();
                        return PartialView(returnPartial, model);
                    }
                case "preBookedLeads":
                    {
                        returnPartial = "_SearchPreBookedContactedLeadsPartial";
                        model = new PreBookedContactedLeadsModel.SearchPreBookedContactedLeadsModel();
                        return PartialView(returnPartial, model);
                    }
                case "optionsSold":
                    {
                        returnPartial = "_SearchOptionsSoldPartial";
                        model = new OptionsSoldModel.SearchOptionsSoldModel();
                        return PartialView(returnPartial, model);
                    }
                case "confirmedLeads":
                    {
                        returnPartial = "_SearchConfirmedLeadsPartial";
                        model = new ConfirmedLeadsModel.SearchConfirmedLeadsModel();
                        return PartialView(returnPartial, model);
                    }
                case "dynamic":
                    {
                        returnPartial = "_SearchDynamicPartial";
                        model = new DynamicModel.SearchDynamicModel();
                        return PartialView(returnPartial, model);
                    }
                case "closeOutsHistory":
                    {
                        returnPartial = "_SearchCloseOutsHistoryPartial";
                        model = new CloseOutHistoryModel.SearchCloseOutsModel();
                        return PartialView(returnPartial, model);
                    }
                case "masterCloseOut":
                    {
                        returnPartial = "_SearchMasterCloseOutPartial";
                        model = new MasterCloseOutModel.SearchMasterCloseOutModel();
                        return PartialView(returnPartial, model);
                    }
                case "closeOut":
                    {
                        //nombre de la parcial de parámetros del reporte
                        returnPartial = "_SearchCloseOutPartial";
                        //modelo de la parcial de parámetros del reporte
                        CloseOutModel.SearchCloseOutModel closeoutModel = new CloseOutModel.SearchCloseOutModel();
                        closeoutModel.SearchCloseOut_Date = DateTime.Today.ToString("yyyy-MM-dd");

                        return PartialView(returnPartial, closeoutModel);
                    }
                case "prices":
                    {
                        returnPartial = "_SearchPricesPartial";
                        PricesModel.SearchPricesModel activityPricesModel = new PricesModel.SearchPricesModel();
                        activityPricesModel.SearchPrices_Date = DateTime.Today.ToString("yyyy-MM-dd");
                        activityPricesModel.SearchPrices_BookingDate = DateTime.Today.ToString("yyyy-MM-dd");
                        activityPricesModel.Search_Currency = 2;
                        activityPricesModel.Search_UpdateCache = !(bool)cache;

                        return PartialView(returnPartial, activityPricesModel);
                    }
                case "pricesCustomReport":
                    {
                        returnPartial = "_SearchPricesCustomPartial";
                        PricesModel.SearchPricesCustomModel searchPrices = new PricesModel.SearchPricesCustomModel();

                        return PartialView(returnPartial, searchPrices);
                    }
                case "timeshareOperation":
                    {
                        returnPartial = "_SearchChargeBacksPartial";
                        ChargeBacksModel.SearchChargeBacks search = new ChargeBacksModel.SearchChargeBacks();
                        return PartialView(returnPartial, search);
                    }
                case "auditCoupons":
                    {
                        returnPartial = "_SearchAuditCouponsPartial";
                        AuditCouponsModel.SearchInvoice search = new AuditCouponsModel.SearchInvoice()
                        {
                            InvoiceInfo = new AuditCouponsModel.InvoiceItem()
                        };
                        return PartialView(returnPartial, search);
                    }
                //case "commissions":
                case "demonstrationsPayroll":
                    {
                        returnPartial = "_SearchCommissionsPartial";
                        CommissionsReportModel.SearchCommissionsReportModel search = new CommissionsReportModel.SearchCommissionsReportModel();
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }
                case "incomePolicy":
                    {
                        returnPartial = "_SearchPolicyPartial";
                        ProductionModel.SearchProduction search = new ProductionModel.SearchProduction();
                        return PartialView(returnPartial, search);
                    }
                case "invoices":
                    {
                        returnPartial = "_SearchInvoicePartial";
                        InvoiceModel.SearchInvoice search = new InvoiceModel.SearchInvoice();
                        var listCompanies = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                        listCompanies.Insert(0, new SelectListItem()
                        {
                            Text = "Público en General",
                            Value = ""
                        });
                        search.Search_Companies = listCompanies;
                        return PartialView(returnPartial, search);
                    }
                case "productionPerActivity":
                    {
                        returnPartial = "_SearchProductionPartial";
                        ProductionModel.SearchProduction search = new ProductionModel.SearchProduction();
                        return PartialView(returnPartial, search);
                    }
                case "productionPerCategory":
                    {
                        returnPartial = "_SearchProductionCategoriesPartial";
                        ProductionModel.SearchProduction search = new ProductionModel.SearchProduction();
                        return PartialView(returnPartial, search);
                    }
                case "couponSales":
                    {
                        returnPartial = "_SearchCouponSalesPartial";
                        ProductionModel.SearchProduction search = new ProductionModel.SearchProduction();
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }
                case "pettyCashStatement":
                    {
                        returnPartial = "_SearchCashStatementPartial";
                        CashStatementModel.SearchStatement search = new CashStatementModel.SearchStatement();
                        return PartialView(returnPartial, search);
                    }
                case "productionPerProvider":
                    {
                        returnPartial = "_SearchProductionProvidersPartial";
                        ProvidersProductionModel.SearchProviderProduction search = new ProvidersProductionModel.SearchProviderProduction();
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }
                case "couponsHistory":
                    {
                        returnPartial = "_SearchCouponsHistoryPartial";
                        CouponsHistoryModel.SearchCouponsHistory search = new CouponsHistoryModel.SearchCouponsHistory();
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }
                case "userPermissions":
                    {
                        returnPartial = "_SearchUserPermissionsPartial";
                        UserPermissionsModel.SearchUserPermissions search = new UserPermissionsModel.SearchUserPermissions();
                        return PartialView(returnPartial, search);
                    }
                case "legacyUserPermissions":
                    {
                        returnPartial = "_SearchLegacyUserPermissionsPartial";
                        LegacyUserPermissionsModel.SearchUserPermissions search = new LegacyUserPermissionsModel.SearchUserPermissions();
                        return PartialView(returnPartial, search);
                    }
                case "paymentsAssignation":
                    {
                        returnPartial = "_SearchPaymentsAssignationPartial";
                        PaymentAssignationViewModel.SearchPaymentAssignation search = new PaymentAssignationViewModel.SearchPaymentAssignation();
                        return PartialView(returnPartial, search);
                    }
                case "billing":
                    {
                        returnPartial = "_SearchBillingPartial";
                        BillingModel.SearchBilling search = new BillingModel.SearchBilling();
                        search.Search_I_FromDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                        search.Search_F_ToDate = search.Search_I_FromDate;
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }

                case "budgets":
                    {
                        returnPartial = "_SearchBudgetsPartial";
                        BudgetsViewModel.SearchBudgetsViewModel search = new BudgetsViewModel.SearchBudgetsViewModel();
                        search.Search_I_FromDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                        search.Search_F_ToDate = search.Search_I_FromDate;
                        return PartialView(returnPartial, search);
                    }
                case "ioBalance":
                    {
                        returnPartial = "_SearchIOBalancePartial";
                        IncomeOutcomeModel.SearchIncomeOutcomeModel search = new IncomeOutcomeModel.SearchIncomeOutcomeModel();
                        search.Search_I_FromDate = DateTime.Now.ToString("yyyy-MM-01");
                        search.Search_F_ToDate = DateTime.Now.ToString("yyyy-MM-" + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString());
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }
                case "quoteRequests":
                    {
                        returnPartial = "_SearchQuoteRequestsPartial";
                        QuoteRequestsReportModel.SearchQuoteRequests search = new QuoteRequestsReportModel.SearchQuoteRequests();
                        search.Search_I_FromDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                        search.Search_F_ToDate = search.Search_I_FromDate;
                        return PartialView(returnPartial, search);
                    }
                case "sweepstakes":
                    {
                        returnPartial = "_SearchSweepstakesPartial";
                        SweepstakesReportModel.SearchSweepstakes search = new SweepstakesReportModel.SearchSweepstakes();
                        search.Search_I_FromDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                        search.Search_F_ToDate = search.Search_I_FromDate;
                        return PartialView(returnPartial, search);
                    }
                case "roomUpgrades":
                    {
                        returnPartial = "_SearchRoomUpgradesPartial";
                        model = new RoomUpgradesModel.SearchRoomUpgradesModel();
                        return PartialView(returnPartial, model);
                    }
                case "concierges":
                    {
                        returnPartial = "_SearchConciergesPartial";
                        model = new ConciergesModel.SearchConciergesModel();
                        return PartialView(returnPartial, model);
                    }
                case "22DayArrival":
                    {
                        returnPartial = "_SearchPreCheckInPartial";
                        model = new PreCheckInModel.SearchPreCheckInModel();
                        return PartialView(returnPartial, model);
                    }
                case "arrivals":
                    {
                        returnPartial = "_SearchArrivalsPartial";
                        model = new ArrivalsReport.SearchArrivalsModel();
                        return PartialView(returnPartial, model);
                    }
                case "incorrectPreBooked":
                    {
                        returnPartial = "_SearchIncorrectPreBookedToursPartial";
                        model = new IncorrectlyPreBookedToursModel.SearchIncorrectPreBookedTours();
                        return PartialView(returnPartial, model);
                    }
                case "duplicateLeads":
                    {
                        returnPartial = "_SearchDuplicateLeadsPartial";
                        model = new DuplicateLeadsReport.SearchDuplicateLeadsModel();
                        return PartialView(returnPartial, model);
                    }
                case "externalSales":
                    {
                        returnPartial = "_SearchExternalSalesPartial";
                        ProductionModel.SearchProduction search = new ProductionModel.SearchProduction();
                        search.Search_Cache = (bool)cache;
                        return PartialView(returnPartial, search);
                    }
                case "accountingAccounts":
                    {
                        returnPartial = "_SearchAccoutingAccountsPartial";
                        model = new AccountingAccountsViewModel.searchAccountingAccounts();
                        return PartialView(returnPartial, model);
                    }
                case "diamante":
                    {
                        returnPartial = "_SearchDiamantePartial";
                        model = new DiamanteModel.SearchDiamante();
                        return PartialView(returnPartial, model);
                    }
                case "prearrivalsFeedback":
                    {
                        returnPartial = "_SearchFeedbackPartial";
                        model = new PreArrivalFeedbackModel.SearchFeedback();
                        return PartialView(returnPartial, model);
                    }
                case "ActivityLogs":
                    {
                        returnPartial = "_SearchUserLogsActivity";
                        model = new UserLogsActivityYYYYMM.SearchUserLogsActivity();
                        return PartialView(returnPartial, model);
                    }

                case "callsByLocation":
                    {
                        returnPartial = "_SearchCallsByLocationPartial";
                        model = new CallsByLocationViewModel.SearchCalls();
                        return PartialView(returnPartial, model);
                    }
                case "agentsPerformance":
                    {
                        returnPartial = "_SearchAgentsPerformancePartial";
                        model = new CallsByLocationViewModel.SearchCalls();
                        return PartialView(returnPartial, model);
                    }
                case "notifications":
                    {
                        returnPartial = "_SearchNotificationsPartial";
                        model = new NotificationsModel.SearchNotificationsModel();
                        return PartialView(returnPartial, model);
                    }
                case "SearchSalesByTeam":
                    {
                        returnPartial = "_SearchSalesByTeam";
                        model = new SalesByTeam.SearchSalesByTeam();
                        return PartialView(returnPartial, model);
                    }
                case "prearrivalManifest":
                    {
                        returnPartial = "_SearchPreArrivalManifestPartial";
                        //model = new PreArrivalReport.SearchPreArrivalReport();
                        model = new SearchPreArrivalWeeklyReportModel();
                        return PartialView(returnPartial, model);
                    }
                case "weeklyBudget":
                    {
                        returnPartial = "_SearchPreArrivalWeeklyBudgetPartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                //case "weeklySales":
                case "salesByAgent":
                    {
                        returnPartial = "_SearchPreArrivalSalesByAgentPartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                case "salesCommissions":
                    {
                        returnPartial = "_SearchPreArrivalWeeklyCommissionsPartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                //case "prearrivalSales":
                case "prearrivalCommissions":
                    {
                        returnPartial = "_SearchPreArrivalCommissionsReportPartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                //case "prearrivalWeekly":
                case "prearrivalReporting":
                    {
                        returnPartial = "_SearchPreArrivalReportingPartial";
                        model = new SearchPreArrivalWeeklyReportModel();
                        return PartialView(returnPartial, model);
                    }
                case "prearrivalWeekly":
                    {
                        returnPartial = "_SearchPreArrivalWeeklyReport";
                        model = new SearchPreArrivalWeeklyReportModel();
                        return PartialView(returnPartial, model);
                    }
                case "optionsPercentage":
                    {
                        returnPartial = "_PreArrivalOptionsPercentagePartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                case "transportations":
                    {
                        returnPartial = "_SearchPreArrivalTransportationPartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                case "prearrivalImportHistory":
                    {
                        returnPartial = "_SearchPreArrivalImportHistoryPartial";
                        model = new PreArrivalImportHistory.Search();
                        return PartialView(returnPartial, model);
                    }
                case "invitationsBalance":
                    {
                        returnPartial = "_SearchInvitationBalance";
                        model = new InvitationBalance.SearchInvitationBalance();
                        return PartialView(returnPartial, model);
                    }
                case "googleAdsInvestment":
                    {
                        returnPartial = "_SearchAdsInvestmentPartial";
                        model = new GoogleAdsViewModel.Reports.SearchAdsInvestment()
                        {
                            GoogleAdsAccounts = GoogleAdsDataModel.Reports.GetAdsAccounts()
                        };
                        return PartialView(returnPartial, model);
                    }
                ///////
                case "frequentGuests":
                    {
                        returnPartial = "_SearchFrequentGuestsPartial";
                        model = new FrequentGuestsViewModel.SearchGuests()
                        {
                            Search_Resorts = PreArrivalDataModel.PreArrivalCatalogs.GetFrontResorts()
                        };
                        return PartialView(returnPartial, model);
                    }
                case "invitationsDeposits":
                    {
                        returnPartial = "_SearchInvitationsDepositsReport";
                        model = new SPIInvitationReport.searchInvitationModel();
                        return PartialView(returnPartial, model);
                    }
                case "preArrivalInvoices":
                    {
                        returnPartial = "_SearchPreArrivalInvoicesPartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                //case "preArrivalSalesPerOptions":
                case "preArrivalSalesByResort-OptionType":
                    {
                        returnPartial = "_SearchPreArrivalSalesByResortOptionTypePartial";
                        model = new PreArrivalReport.SearchPreArrivalReport();
                        return PartialView(returnPartial, model);
                    }
                default:
                    {
                        return PartialView(returnPartial);
                    }

            }
        }

        [Authorize]
        public ActionResult RenderMassUpdate()
        {
            return PartialView("_LeadMassUpdatePartial", new LeadModel.Views.MassUpdate());
        }

        [Authorize]
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

        [Authorize]
        public async Task<ActionResult> SearchDuplicateLeads(DuplicateLeadsReport.SearchDuplicateLeadsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            DuplicateLeadsReport ar = new DuplicateLeadsReport();
            ar.ListDuplicateLeadsReportModel = rdm.SearchDuplicateLeads(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_DuplicateLeadsResultsPartial", ar);
        }

        [Authorize]
        public async Task<ActionResult> SearchArrivals(ArrivalsReport.SearchArrivalsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ArrivalsReport ar = new ArrivalsReport();
            ar.Privileges = AdminDataModel.GetViewPrivileges(11269);
            ar.ListArrivalsReportModel = rdm.SearchArrivals(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ArrivalsReportResultsPartial", ar);
        }
        [Authorize]

        public async Task<ActionResult> SearchIncorrectPreBookedTours(IncorrectlyPreBookedToursModel.SearchIncorrectPreBookedTours model)
        {
            ReportDataModel rdm = new ReportDataModel();
            IncorrectlyPreBookedToursModel m = new IncorrectlyPreBookedToursModel();
            m.ListPreBookedTours = rdm.SearchIncorrectlyPreBookedTours(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request));

            return PartialView("_IncorrectPreBookedToursResultsPartial", m);
        }

        [Authorize]
        public async Task<ActionResult> SearchBudgetsViewModel(BudgetsViewModel.SearchBudgetsViewModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_BudgetsResultsPartial", rdm.GetBudgets(model));
        }

        [Authorize]
        public async Task<ActionResult> SearchBilling(BillingModel.SearchBilling model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;


            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            if (model.Search_Cache)
            {
                return PartialView("_BillingResultsPartial", rdm.GetBillingFromCache(model));
            }
            else
            {
                return PartialView("_BillingResultsPartial", rdm.GetBilling(model));
            }
        }

        [Authorize]
        public async Task<ActionResult> SearchIOBalance(IncomeOutcomeModel.SearchIncomeOutcomeModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            if (model.Search_Cache)
            {
                return PartialView("_IOBalanceResultsPartial", rdm.GetIOBalanceFromCache(model));
            }
            else
            {
                return PartialView("_IOBalanceResultsPartial", rdm.GetIOBalance(model));
            }
        }
        [Authorize]
        public async Task<ActionResult> SearchIndicatorsPerResort(IndicatorsModel.SearchIndicatorsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            IndicatorsModel im = new IndicatorsModel();
            im.ListIndicatorsReportModel = rdm.SearchIndicatorsPerResort(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_IndicatorsPerResortReportResultsPartial", im);
        }
        [Authorize]
        public async Task<ActionResult> SearchIndicators(IndicatorsModel.SearchIndicatorsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            IndicatorsModel im = new IndicatorsModel();
            im.Privileges = AdminDataModel.GetViewPrivileges(10563);
            im.ListIndicatorsReportModel = rdm.SearchIndicators(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_IndicatorsReportResultsPartial", im);
        }
        [Authorize]
        public async Task<ActionResult> SearchPipeline(PipelineModel.SearchPipelineModel model)
        {
            var privileges = AdminDataModel.GetViewPrivileges(11148);
            ReportDataModel rdm = new ReportDataModel();
            PipelineModel pm = new PipelineModel();
            pm.Privileges = privileges;
            pm.ListPipelineReportModel = rdm.SearchPipeline(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PipelineReportResultsPartial", pm);
        }
        [HttpPost]
        public async Task<ActionResult> SearchExchangeTours(ExchangeTourModel.SearchExchangeTourModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ExchangeTourModel etm = new ExchangeTourModel();
            etm.ListExchangeToursReportModel = rdm.SearchExchangeTours(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_ExchangeToursReportResultsPartial", etm);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreBookedArrivals(PreBookedArrivalsModel.SearchPreBookedArrivalsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            PreBookedArrivalsModel pam = new PreBookedArrivalsModel();

            pam.Privileges = AdminDataModel.GetViewPrivileges(10570);
            pam.ListPreBookedArrivalsReportModel = rdm.SearchPreBookedArrivals(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PreBookedArrivalsReportResultsPartial", pam);
        }

        [Authorize]
        public async Task<ActionResult> SearchNewReferrals(NewReferralsModel.SearchNewReferralsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            NewReferralsModel nrm = new NewReferralsModel();
            nrm.ListNewReferralsReport = rdm.SearchNewReferrals(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_NewReferralsReportResultsPartial", nrm);
        }

        [Authorize]
        public async Task<ActionResult> SearchWeekly(WeeklyModel.SearchWeeklyModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            WeeklyModel nrm = new WeeklyModel();
            nrm.ListWeeklyReport = rdm.SearchWeekly(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_WeeklyReportResultsPartial", nrm);
        }

        [Authorize]
        public async Task<ActionResult> SearchReservationsMade(ReservationsMadeModel.SearchReservationsMadeModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ReservationsMadeModel rmm = new ReservationsMadeModel();
            rmm.ListReservationsMade = rdm.SearchReservationsMade(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_ReservationsMadeResultsPartial", rmm);
        }

        //public ActionResult SearchNewReferralLeads(NewReferralLeadsModel.SearchNewReferralLeadsModel model)
        //{
        //    ReportDataModel rdm = new ReportDataModel();
        //    NewReferralLeadsModel nrm = new NewReferralLeadsModel();
        //    nrm.ListNewReferralLeadsReportModel = rdm.SearchNewReferralLeads(model);
        //    return PartialView("_NewReferralLeadsReportResultsPartial", nrm);
        //}

        [Authorize]
        public async Task<ActionResult> SearchPreBookedContactedLeads(PreBookedContactedLeadsModel.SearchPreBookedContactedLeadsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            PreBookedContactedLeadsModel clm = new PreBookedContactedLeadsModel();
            clm.ListPreBookedContactedLeadsReport = rdm.SearchPreBookedContactedLeads(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PreBookedContactedLeadsReportResultsPartial", clm);
        }

        [Authorize]
        public async Task<ActionResult> SearchOptionsSold(OptionsSoldModel.SearchOptionsSoldModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            OptionsSoldModel osm = new OptionsSoldModel();
            osm.ListOptionsSoldReportModel = rdm.SearchOptionsSold(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_OptionsSoldReportResultsPartial", osm);
        }

        [Authorize]
        public async Task<ActionResult> SearchConfirmedLeads(ConfirmedLeadsModel.SearchConfirmedLeadsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ConfirmedLeadsModel clm = new ConfirmedLeadsModel();
            clm.ListConfirmedLeadsReportModel = rdm.SearchConfirmedLeads(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;


            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ConfirmedLeadsReportResultsPartial", clm);
        }

        [Authorize]
        public ActionResult SearchDiamante(DiamanteModel.SearchDiamante model)
        {
            ReportDataModel rdm = new ReportDataModel();
            DiamanteModel dm = new DiamanteModel();
            dm.ListDiamanteReportModel = rdm.SearchDiamanteReport(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request));
            return PartialView("_DiamanteReportResultsPartial", dm);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreCheckIn(PreCheckInModel.SearchPreCheckInModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            PreCheckInModel pcm = new PreCheckInModel();
            pcm.ListPreCheckInModel = rdm.SearchPreCheckIn(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PreCheckInReportResultsPartial", pcm);
        }

        [Authorize]
        public async Task<ActionResult> SearchRoomUpgrades(RoomUpgradesModel.SearchRoomUpgradesModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            RoomUpgradesModel rum = new RoomUpgradesModel();
            rum.ListRoomUpgradesReportModel = rdm.SearchRoomUpgrades(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_RoomUpgradesReportResultsPartial", rum);
        }

        [Authorize]
        public async Task<ActionResult> SearchConcierges(ConciergesModel.SearchConciergesModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ConciergesModel cm = new ConciergesModel();
            cm.ListConciergesReportModel = rdm.SearchConcierges(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ConciergesReportResultsPartial", cm);
        }

        [Authorize]
        public async Task<ActionResult> SearchDynamic(DynamicModel.SearchDynamicModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            DynamicModel.DynamicResults dm = new DynamicModel.DynamicResults();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_DynamicReportResultsPartial", rdm.SearchDynamic(model));
            //return Json(rdm.SearchDynamic(model));
        }

        // [Models.DataModels.AdminDataModel.ActivityLogs]
        [Authorize]
        public async Task<ActionResult> SearchChargeBacks(ChargeBacksModel.SearchChargeBacks model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ChargeBacksModel.ChargeBacksResults results = rdm.SearchChargeBacks(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ChargeBacksResultsPartial", results);
        }

        [Authorize]
        public async Task<ActionResult> SearchCashStatement(CashStatementModel.SearchStatement model)
        {
            ReportDataModel rdm = new ReportDataModel();
            CashStatementModel.CashStatementResults results = rdm.SearchCashStatement(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_CashStatementResultsPartial", results);
        }

        [Authorize]
        public async Task<ActionResult> SearchMasterCloseOut(MasterCloseOutModel.SearchMasterCloseOutModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            MasterCloseOutModel.MasterCloseOutResults com = rdm.SearchMasterCloseOut(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_MasterCloseOutResultsPartial", com);
        }
        //verfy
        [Authorize]
        public JsonResult ProcessCouponInfo(long id)
        {
            ReportDataModel rdm = new ReportDataModel();
            return Json(new
            {
                Processed = rdm.ProcessCouponInfo(id, "Master Close Out")
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        //[Models.DataModels.AdminDataModel.ActivityLogs]
        public async Task<ActionResult> SearchCloseOutsHistory(CloseOutHistoryModel.SearchCloseOutsModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            CloseOutHistoryModel.CloseOutsHistoryResults com = rdm.SearchCloseOutsHistory(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;

            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Get", (string)null, urlMethod, request, terminalID));
            return PartialView("_CloseOutsHistoryResultsPartial", com);
        }

        [Authorize]
        public async Task<ActionResult> SearchCloseOut(CloseOutModel.SearchCloseOutModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            CloseOutModel com = rdm.GetCloseOut(model);
            var request = HttpContext.Request;
            var terminals = new UserSession().Terminals;
            var urlMethod = new UserSession().Url;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            else
            {
                terminalID = model.SearchCloseOut_TerminalID.ToString();
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Get", (string)null, urlMethod, request, terminalID));
            return PartialView("_CloseOutResultsPartial", com);
        }

        [Authorize]
        public async Task<ActionResult> SearchCouponsHistory(CouponsHistoryModel.SearchCouponsHistory model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            if (model.Search_Cache)
            {
                CouponsHistoryModel coupons = new CouponsHistoryModel();
                coupons = rdm.GetCouponsHistoryFromCache(model);
                if (coupons.Coupons.Count() == 0)
                {
                    coupons = rdm.GetCouponsHistory(model);
                }
                return PartialView("_CouponsHistoryResultsPartial", coupons);
            }
            else
            {
                return PartialView("_CouponsHistoryResultsPartial", rdm.GetCouponsHistory(model));
            }
        }

        [Authorize]
        public async Task<ActionResult> SearchUserPermissions(UserPermissionsModel.SearchUserPermissions model)
        {
            ReportDataModel rdm = new ReportDataModel();
            UserPermissionsModel upm = rdm.GetUserPermissions(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_UserPermissionsResultsPartial", upm);
        }

        //[Authorize]
        //public async Task<ActionResult> SearchLegacyUserPermissions(LegacyUserPermissionsModel.SearchUserPermissions model)
        //{
        //    ReportDataModel rdm = new ReportDataModel();
        //    LegacyUserPermissionsModel upm = rdm.GetLegacyUserPermissions(model);
        //    var request = HttpContext.Request;
        //    var urlMethod = new UserSession().Url;

        //    var terminals = new UserSession().Terminals;
        //    var terminalID = (string)null;
        //    if (terminals.Count() > 0)
        //    {
        //        terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
        //    }
        //    Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
        //    return PartialView("_LegacyUserPermissionsResultsPartial", upm);
        //}

        [Authorize]
        public async Task<ActionResult> SearchCommissions(CommissionsReportModel.SearchCommissionsReportModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            CommissionsReportModel com = new CommissionsReportModel();
            if (model.Search_Cache)
            {
                com = rdm.GetCommissionsFromCache(model);
            }
            else
            {
                com = rdm.GetCommissions(model);
            }
            return PartialView("_CommissionsResultsPartial", com);
        }

        [Authorize]
        public async Task<ActionResult> SearchPolicy(ProductionModel.SearchProduction model)
        {
            ReportDataModel rdm = new ReportDataModel();
            IncomePolicyModel ipm = rdm.GetPolicy(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PolicyResultsPartial", ipm);
        }

        [Authorize]
        public async Task<ActionResult> SearchProduction(ProductionModel.SearchProduction model)
        {
            ReportDataModel rdm = new ReportDataModel();
            IncomePolicyModel ipm = rdm.GetPolicy(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PolicyResultsPartial", ipm);
        }

        [Authorize]
        public async Task<ActionResult> SearchCategoriesProduction(ProductionModel.SearchProduction model)
        {
            ReportDataModel rdm = new ReportDataModel();
            CategoriesProductionModel cpm = rdm.GetCategoriesProduction(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_CategoriesResultsPartial", cpm);
        }

        [Authorize]
        public async Task<ActionResult> SearchProvidersProduction(ProvidersProductionModel.SearchProviderProduction model)
        {
            ReportDataModel rdm = new ReportDataModel();
            ProvidersProductionModel ppm = new ProvidersProductionModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            if (model.Search_Cache)
            {
                ppm = rdm.GetProvidersProductionFromCache(model);
            }
            else
            {
                ppm = rdm.GetProvidersProduction(model);
            }
            return PartialView("_ProvidersResultsPartial", ppm);
        }

        [Authorize]
        public async Task<ActionResult> SearchPricesCustomReport(PricesModel.SearchPricesCustomModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            PricesModel.PricesCustomModel pm = rdm.SearchPricesCustomReport(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PricesCustomResultsPartial", pm);
        }

        [Authorize]
        public async Task<JsonResult> GetReportLayout(int id)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var getReport = rdm.GetReportLayout(id);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(getReport, "Get", (string)null, urlMethod, request, terminalID));
            return Json(getReport, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> DeleteReportLayout(int id)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.DeleteReportLayout(id);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(id, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message
            });
        }
        //verify
        [Authorize]
        public async Task<JsonResult> SetCustomLayout(PricesModel.PricesReportLayout model)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SaveReportLayout(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminalID));
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
                LayoutID = attempt.ObjectID
            });
        }

        [Authorize]
        public async Task<ActionResult> SearchPaymentsAssignation(PaymentAssignationViewModel.SearchPaymentAssignation model)
        {
            PaymentAssignationDataModel pdm = new PaymentAssignationDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PaymentsAssignationResultsPartial", pdm.GetPaymentsAssignation(new Guid(model.PurchaseID), model.Date));
        }

        //public JsonResult SavePaymentsAssignation(string id)
        //{
        //    AttemptResponse attempt = new AttemptResponse();
        //    PaymentAssignationDataModel pdm = new PaymentAssignationDataModel();
        //    attempt = pdm.SavePaymentAssignation(id);

        //    string errorLocation = string.Empty;
        //    if (attempt.Exception != null)
        //    {
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
        //        InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
        //    });
        //}

        [Authorize]
        public async Task<ActionResult> SearchAuditInvoices(AuditCouponsModel.SearchInvoice model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_AuditCouponsResultsPartial", rdm.SearchAuditInvoices(model));
        }

        [Authorize]
        public async Task<JsonResult> GetProviderInvoice(long id)
        {
            CouponDataModel cdm = new CouponDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var getProvider = cdm.GetProviderInvoice(id);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(getProvider, "Get", (string)null, urlMethod, request, terminalID));
            return Json(getProvider);
        }

        [Authorize]
        public async Task<JsonResult> SearchCoupon(AuditCouponsModel.SearchCoupon model)
        {
            CouponDataModel cdm = new CouponDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return Json(cdm.GetCouponsList(model));
        }

        [Authorize]
        public async Task<JsonResult> SaveProviderInvoice(AuditCouponsModel.InvoiceItem model)
        {
            AttemptResponse attempt = new AttemptResponse();
            CouponDataModel cdm = new CouponDataModel();
            attempt = cdm.SaveProviderInvoice(model);

            string errorLocation = string.Empty;
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
                ProviderInvoiceID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public async Task<JsonResult> SaveAsNoShow(string id)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SaveAsNoShow(id);

            string errorLocation = string.Empty;
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
            Task.Run(() => CreateTableUserLogActivityAsync(id, "Save", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public async Task<JsonResult> SaveAsConfirmed(string id)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SaveAsConfirmed(id);

            string errorLocation = string.Empty;
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
            Task.Run(() => CreateTableUserLogActivityAsync(id, "Save", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public async Task<JsonResult> DeletePartial(long id)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.DeletePartial(id);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ObjectID = attempt.ObjectID,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public async Task<JsonResult> SavePartial(ChargeBacksModel.Partial model)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SavePartial(model);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ObjectID = attempt.ObjectID,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public async Task<JsonResult> SaveCharged(long id)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SaveCharged(id);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var urlMethod = new UserSession().Url;

            var request = HttpContext.Request;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(id, "Save", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public async Task<ActionResult> GetAuditDetails(long id)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var getAudit = rdm.GetAuditDetails(id);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(getAudit, "Get", (string)null, urlMethod, request, terminalID));
            return PartialView("_AuditDetailResultsPartial", getAudit);
        }

        [Authorize]
        public async Task<PartialViewResult> GetCouponCost(long id, long PurchaseServiceDetailID)
        {
            string returnPartial = "_CouponCostPartial";
            dynamic model = ReportDataModel.GetCouponCost(id, PurchaseServiceDetailID);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Get", (string)null, urlMethod, request, terminalID));
            return PartialView(returnPartial, model);
        }

        [Authorize]
        public async Task<JsonResult> SaveCustomCost(AuditCouponsModel.CustomCostModel model)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SaveCustomCost(model);

            string errorLocation = string.Empty;
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
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                CouponCost = attempt.ObjectID
            });
        }

        [Authorize]
        public async Task<ActionResult> SearchExternalSales(ProductionModel.SearchProduction model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ExternalSalesResultsPartial", rdm.GetExternalSales(model));
        }

        [Authorize]
        public async Task<ActionResult> SearchCouponSales(ProductionModel.SearchProduction model)
        {
            ReportDataModel rdm = new ReportDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            if (model.Search_Cache)
            {
                return PartialView("_CouponSalesResultsPartial", rdm.GetCouponsListFromCache(model));
            }
            else
            {
                return PartialView("_CouponSalesResultsPartial", rdm.GetCouponsList(model));
            }
        }

        //public ActionResult SearchInvoice(InvoiceModel.SearchInvoice model)
        //{
        //    ReportDataModel rdm = new ReportDataModel();
        //    InvoiceModel inm = rdm.GetInvoice(model);
        //    return PartialView("_InvoiceResultsPartial",inm);
        //}

        [Authorize]
        public async Task<ActionResult> GetSavedCloseOut(long id)
        {
            ReportDataModel rdm = new ReportDataModel();
            CloseOutModel com = rdm.GetSavedCloseOut(id);
            com.Deletable = false;
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(com, "Get", (string)null, urlMethod, request, terminalID));
            return PartialView("_CloseOutResultsPartial", com);
        }

        [Authorize]
        public async Task<ActionResult> SearchPricesReport(PricesModel.SearchPricesModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            PricesModel.PricesCustomModel pm = rdm.SearchPricesReport(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PricesCustomResultsPartial", pm);
        }

        [Authorize]
        public async Task<ActionResult> SearchActivityPrices(PricesModel.SearchPricesModel model)
        {
            ReportDataModel rdm = new ReportDataModel();
            PricesModel pm = new PricesModel();
            MasterChartDataModel.Purchases pu = new MasterChartDataModel.Purchases();
            pm.ExchangeRate = pu.GetExchangeRates(DateTime.Now, model.Search_TerminalID, model.Search_PointOfSaleID);
            pm.ListPrices = rdm.GetActivityPrices(model);
            //pm.SelectedCurrency = model.Search_Currency;
            ePlatBack.Models.UserSession session = new ePlatBack.Models.UserSession();
            pm.FullView = (session.RoleID == new Guid("2a846d2b-9e6a-458f-81f8-00c5cf09bba1") ? false : true);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;


            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ActivityPricesResultsPartial", pm);
        }

        [Authorize]
        public async Task<ActionResult> SearchQuoteRequests(QuoteRequestsReportModel.SearchQuoteRequests model)
        {
            ReportDataModel rdm = new ReportDataModel();
            QuoteRequestsReportModel results = rdm.SearchQuoteRequests(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_QuoteRequestsResultsPartial", results);
        }

        [Authorize]
        public async Task<ActionResult> SearchSweepstakes(SweepstakesReportModel.SearchSweepstakes model)
        {
            ReportDataModel rdm = new ReportDataModel();
            SweepstakesReportModel results = rdm.SearchSweepstakes(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_SweepstakesResultsPartial", results);
        }

        //
        [Authorize]
        public async Task<ActionResult> SearchAccountingAccounts(AccountingAccountsViewModel.searchAccountingAccounts model)
        {
            ReportDataModel rdm = new ReportDataModel();
            AccountingAccountsViewModel results = rdm.SearchAccounts(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;


            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_SearchAcountingAccountsResultPartial", results);
        }

        [Authorize]
        public async Task<ActionResult> SearchPrearrivalsFeedback(PreArrivalFeedbackModel.SearchFeedback model)
        {
            ArrivalsDataModel adm = new ArrivalsDataModel();
            List<PreArrivalFeedbackModel.FeedbackResults> results = adm.SearchPrearrivalsFeedback(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PrearrivalsFeedbackResultsPartial", results);
        }

        [Authorize]
        public async Task<ActionResult> SearchAgentsPerformance(CallsByLocationViewModel.SearchCalls model)
        {
            AgentsPerformanceViewModel.AgentsPerformanceResults results = NetCenterDataModel.Reports.GetAgentsPerformance(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_AgentsPerformanceResultsPartial", results);
        }

        [Authorize]
        public async Task<ActionResult> SearchCallsByLocation(CallsByLocationViewModel.SearchCalls model)
        {
            CallsByLocationViewModel.CallsByLocationResults results = NetCenterDataModel.Reports.GetCallsByLocation(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;


            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request));
            return PartialView("_CallsByLocationResultsPartial", results);
        }

        [Authorize]
        //public async Task<ActionResult> SearchPreArrivalManifest(PreArrivalReport.SearchPreArrivalReport model)
        public async Task<ActionResult> SearchPreArrivalManifest(SearchPreArrivalWeeklyReportModel model)
        {
            UserSession session = new UserSession();
            var report = new PreArrivalReport();
            report.ListManifest = new ReportDataModel().SearchPreArrivalManifestReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalManifestReportResultsPartial", report);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreArrivalWeeklyBudget(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            var report = new PreArrivalReport();
            report.ListManifest = new ReportDataModel().SearchPreArrivalWeeklyBudgetReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalWeeklyBudgetReportResultsPartial", report);
        }

        [Authorize]
        //public async Task<ActionResult> SearchPreArrivalWeeklyBudgetPerPurchase(PreArrivalReport.SearchPreArrivalReport model)
        public async Task<ActionResult> SearchPreArrivalSalesByAgent(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            var report = new PreArrivalReport();
            report.ListManifest = new ReportDataModel().SearchPreArrivalSalesByAgentReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalSalesByAgentReportResultsPartial", report);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreArrivalWeeklyCommissionsReport(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            var report = new PreArrivalReport();
            report.ListManifest = new ReportDataModel().SearchPreArrivalWeeklyCommissionsReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalWeeklyCommissionsResultsPartial", report);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreArrivalCommissionsReport(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            var report = new PreArrivalReport();
            report.ListManifest = new ReportDataModel().SearchPreArrivalCommissionsReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalCommissionsReportResultsPartial", report);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreArrivalWeeklyReport(SearchPreArrivalWeeklyReportModel model)
        {
            UserSession session = new UserSession();
            ReportDataModel rdm = new ReportDataModel();
            var report = new PreArrivalWeeklyReportModel();
            
            report.Response = rdm.SearchPreArrivalWeeklyReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalWeeklyReportResults", report);
        }

        [Authorize]
        //public async Task<ActionResult> SearchPreArrivalWeeklyReport(SearchPreArrivalWeeklyReportModel model)
        public async Task<ActionResult> SearchPreArrivalReportingReport(SearchPreArrivalWeeklyReportModel model)
        {
            UserSession session = new UserSession();
            ReportDataModel rdm = new ReportDataModel();
            var report = new SearchPreArrivalWeeklyReportModel();

            report.Results = rdm.SearchPreArrivalReportingReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalReportingReportResultsPartial", report);
        }

        [Authorize]
        //public async Task<ActionResult> SearchPreArrivalOptionsPercentageReport(PreArrivalReport.SearchPreArrivalReport model)
        public async Task<ActionResult> SearchPreArrivalOptionsPercentageReport(SearchPreArrivalWeeklyReportModel model)
        {
            UserSession session = new UserSession();
            var report = new PreArrivalReport();
            report.ListManifest = new ReportDataModel().SearchPreArrivalOptionsPercentageReport(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalOptionsPercentageResultsPartial", report);
        }

        [Authorize]
        public async Task<ActionResult> SearchPreArrivalInvoices(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            ReportDataModel rdm = new ReportDataModel();
            var report = new PreArrivalReport();

            report.ListManifest = rdm.SearchPreArrivalInvoices(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalInvoicesResultsPartial", report);
        }

        [Authorize]
        //public async Task<ActionResult> SearchPreArrivalSalesPerOption(PreArrivalReport.SearchPreArrivalReport model)
        public async Task<ActionResult> SearchPreArrivalSalesByResortOptionType(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            ReportDataModel rdm = new ReportDataModel();
            var report = new PreArrivalReport();

            report.ListManifest = rdm.SearchPreArrivalSalesByResortOptionType(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalSalesByResortOptionTypeResultsPartial", report);
        }

        [Authorize]
        public async Task<ActionResult> SearchTransportations(PreArrivalReport.SearchPreArrivalReport model)
        {
            UserSession session = new UserSession();
            ReportDataModel rdm = new ReportDataModel();
            var report = new PreArrivalReport();

            //var model = new PreArrivalReport.SearchPreArrivalReport();
            //model.Search_DateType = 1;
            //model.Search_Resorts = new long?[] { 556 };
            //model.Search_I_ArrivalDate = "2019-08-02";
            //model.Search_F_ArrivalDate = "2019-08-02";

            report.ListManifest = rdm.SearchTransportations(model);
            
            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            //return Json(new { result = true}, JsonRequestBehavior.AllowGet);
            return PartialView("_PreArrivalTransportationResults", report);
        }

        public async Task<ActionResult> SearchPreArrivalImportHistory(PreArrivalImportHistory.Search model)
        {
            UserSession session = new UserSession();
            ReportDataModel rdm = new ReportDataModel();
            var report = new PreArrivalImportHistory.Search();

            report.Results = rdm.SearchPreArrivalImportHistory(model);

            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PreArrivalImportHistoryResultsPartial", report);
        }

        [Authorize]
        public JsonResult GetDDLData(string itemType, string itemID)
        {
            ReportDataModel rdm = new ReportDataModel();
            return Json(rdm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetDDLData(string path, long? id)
        //{
        //    ReportDataModel rdm = new ReportDataModel();
        //    return Json(rdm.GetDDLData(path, id), JsonRequestBehavior.AllowGet);
        //}

        [ValidateInput(false)]
        public FileResult GetExcelFile(string filename, string content, string time)
        {
            Response.HeaderEncoding = Encoding.UTF8;
            return File(Encoding.UTF8.GetBytes(ControlsDataModel.Excel.GenerateBasicFile(content)), MediaTypeNames.Application.Octet, filename + " " + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
        }

        [ValidateInput(false)]
        public FileResult GetCSVFile(string filename, string content, string time)
        {
            Response.HeaderEncoding = Encoding.UTF8;
            return File(Encoding.UTF8.GetBytes(content), MediaTypeNames.Application.Octet, filename + " " + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv");
        }
        //Activitylogs
        [Authorize]
        public async Task<JsonResult> SaveCloseOut(CloseOutModel.CloseOutSaveModel closeOut)
        {
            AttemptResponse attempt = new AttemptResponse();
            var urlMethod = new UserSession().Url;
            attempt = ReportDataModel.SaveCloseOut(closeOut);
            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            var request = HttpContext.Request;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            else
            {
                terminalID = closeOut.TerminalID.ToString();
            }
            Task.Run(() => CreateTableUserLogActivityAsync(closeOut, "SaveCloseOut", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                CloseOutID = attempt.ObjectID
            });
        }
        //ActivityLogs
        [Authorize]
        public async Task<JsonResult> DeleteCloseOut(int closeOutID)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.DeleteCloseOut(closeOutID);
            var terminals = new UserSession().Terminals;
            var urlMethod = new UserSession().Url;
            string errorLocation = string.Empty;
            var request = HttpContext.Request;
            var terminalID = (string)null;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(closeOutID, "Delete", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                CloseOutID = attempt.ObjectID
            });
        }

        //public JsonResult DeleteCloseOut(int closeOutID)
        //{
        //    AttemptResponse attempt = new AttemptResponse();
        //    attempt = ReportDataModel.DeleteCloseOut(closeOutID);

        //    string errorLocation = string.Empty;
        //    if (attempt.Exception != null)
        //    {
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }            

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
        //        InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
        //        CloseOutID = attempt.ObjectID
        //    });
        //}

        [Authorize]
        public JsonResult MarkAsAudited()
        {
            return Json(new { status = ReportDataModel.MarkAsAudited() }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public async Task<JsonResult> SaveAudit(long id, string invoice)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = ReportDataModel.SaveAudit(id, invoice);

            string errorLocation = string.Empty;
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

            Task.Run(() => CreateTableUserLogActivityAsync(id, "Save", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        [Authorize]
        public ActionResult SearchActivityLogs(UserLogsActivityYYYYMM.SearchUserLogsActivity model)
        {
            ReportDataModel rdm = new ReportDataModel();
            UserLogsActivityYYYYMM result = rdm.SearchUserLogsActivityItems(model);
            return PartialView("_UserLogsActivityResultPartial", result);
        }

        //public ActionResult ShowUserFromActivityLogs(string UserName)
        //{
        //    ePlatBack.Models.DataModels.UserDataModel pdm = new UserDataModel();
        //    return PartialView("_SearchUsersResults", pdm.Search());
        //}

        [Authorize]
        public JsonResult GetActivityLogsByTime()
        {
            return Json(ReportDataModel.GetActivityLogs());
        }

        [Authorize]
        public async Task<ActionResult> GetNotificationsReport(NotificationsModel.SearchNotificationsModel model)
        {
            ePlatEntities db = new ePlatEntities();
            ReportDataModel rdm = new ReportDataModel();
            NotificationsModel nvm = new NotificationsModel();

            var masterEmail = new System.Net.Mail.MailMessage();
            masterEmail.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
            masterEmail.To.Add("efalcon@villagroup.com");
            masterEmail.Subject = "Issues on VLO Notifications Report";
            masterEmail.IsBodyHtml = true;
            var masterEmailBody = "<br />Parameters<br />Terminals: ";

            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            var response = new List<NotificationsModel.NotificationsReportModel>();
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            model.SearchNotifications_Terminals = terminalID;

            try
            {
                masterEmailBody += terminalID;
                response = rdm.GetNotificationsReport(model, true, request, ref masterEmailBody, ref db);
                nvm.ListNotifications = response;
            }
            catch (Exception ex)
            {
                masterEmailBody += "<br />Method: PublicController.CompleteSalesRoomsSales<br />Exception Message: " + Debugging.GetMessage(ex)
                    + "<br />Inner Exception: " + Debugging.GetInnerException(ex) + "<br />Source: " + (ex.Source ?? "") + "<br />StackTrace: " + (ex.StackTrace ?? "");
                //masterEmail.Body = masterEmailBody;
                //EmailNotifications.SendSync(masterEmail);
            }
            masterEmail.Body = masterEmailBody;
            //EmailNotifications.SendSync(masterEmail);
            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = masterEmail } });
            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return PartialView("_SearchNotificationsResultsPartial", nvm);
        }

        [Authorize]
        public ActionResult SearchSalesByTeam(SalesByTeam.SearchSalesByTeam model)
        {
            ReportDataModel rdm = new ReportDataModel();
            SalesByTeam.SalesByTeamResult result = rdm.salesByTeamDataModel(model);
            return PartialView("_salesByTeamResultPartial", result);
        }

        [Authorize]
        public ActionResult SearchInvitationBalance(InvitationBalance.SearchInvitationBalance model)
        {
            ReportDataModel rdm = new ReportDataModel();
            InvitationBalance result = rdm.SearchInvitationsBalance(model);
            return PartialView("_InvitationsBalanceResult", result);
        }

        [Authorize]
        public ActionResult SearchGoogleAdsInvesments(GoogleAdsViewModel.Reports.SearchAdsInvestment model)
        {
            return PartialView("_GoogleAdsInvestmentsResultsPartial", GoogleAdsDataModel.Reports.GetAdsInvestments(model));
        }

        [Authorize]
        public ActionResult SearchFrequentGuests(FrequentGuestsViewModel.SearchGuests model)
         {
             return PartialView("_FrequentGuestsResultsPartial", FrequestGuestsDataModel.SearchFrequentGuests(model));
         }
        
        public ActionResult SearchInvitationsDeposits(SPIInvitationReport.searchInvitationModel model)
        {
            SPIInvitationReport.invitationReportResult result = spiInvitationDataModel.SearchInvitationsDeposits(model);
            return PartialView("_InvitationsDepositsPartial", result);
        }

        /*senses*/
        public ActionResult ByAmbassador()
        {
            return View("~/Views/Reports/ByAmbassador.cshtml");
        }
        public ActionResult Payments()
        {
            return View("~/Views/Reports/Payments.cshtml");
        }
        public ActionResult PollsStatistics()
        {
            return View("~/Views/Reports/PollsStatistics.cshtml");
        }
        public JsonResult GetDataByAmbassador(DateTime fromDate, DateTime toDate)
        {
            ReportDataModel rdm = new ReportDataModel();
            var response = rdm.GetDataByAmbassador(fromDate, toDate);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPollsStatistics(DateTime fromDate, DateTime toDate, string locationID, string pollType)
        {
            ReportDataModel rdm = new ReportDataModel();
            var response = rdm.GetPollsStatistics(fromDate, toDate, locationID, pollType);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDataPayments(DateTime fromDate, DateTime toDate)
        {
            ReportDataModel rdm = new ReportDataModel();
            var response = rdm.GetDataPayments(fromDate, toDate);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Memberships()
        {
            return View("~/Views/Reports/CardsPolls.cshtml");
        }
        public JsonResult getMembershipsInfo(DateTime fromDate, DateTime toDate, string code, string type)
        {
            ReportDataModel rdm = new ReportDataModel();
            var response = rdm.getMembershipsInfo(fromDate, toDate, code, type);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getContactInfo(Int64 membershipSalesID)
        {
            ReportDataModel rdm = new ReportDataModel();
            var response = rdm.getContactInfo(membershipSalesID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}