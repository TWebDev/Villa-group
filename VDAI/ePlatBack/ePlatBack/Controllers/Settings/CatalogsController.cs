using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models;
using System.Threading.Tasks;

namespace ePlatBack.Controllers.Settings
{
    public class CatalogsController : Controller
    {
        //
        // GET: /Catalogs/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                CatalogsViewModel cvm = new CatalogsViewModel();
                //if (RouteData.Values["catalog"] != null)
                //{
                string catalog = RouteData.Values["catalog"].ToString();
                var catalogTag = CatalogsDataModel.Capitals(catalog);
                long? sysComponentForCatalog = AdminDataModel.GetSysComponentIDByURL("/settings/catalogs/get/" + catalog);
                ViewData.Model = new CatalogsViewModel
                {
                    // ReportsModel = new ReportsModel(),
                    catalogName = catalog,
                    catalogTag = catalogTag
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
            //}
            //else
            //{

            /*CatalogsViewModel cvm = new CatalogsViewModel();
           CatalogsViewModel view = new CatalogsViewModel();*/
            //cvm = new CatalogsViewModel
            // {
            //     SearchAccountingAccountsModel = new AccountingAccountsModel.SearchAccountingAccountsModel(),
            //     AccountingAccountInfoModel = new AccountingAccountsModel.AccountingAccountInfoModel(),
            //     SearchBudgetsModel = new BudgetsModel.SearchBudgetsModel(),
            //     BudgetInfoModel = new BudgetsModel.BudgetInfoModel(),
            //     SearchCommissionsModel = new CommissionsModel.SearchCommissionsModel(),
            //     CommissionsInfoModel = new CommissionsModel.CommissionsInfoModel(),
            //     SearchCompaniesModel = new CompaniesModel.SearchCompaniesModel(),
            //     CompaniesInfoModel = new CompaniesModel.CompanyInfoModel(),
            //     SearchCouponFoliosModel = new CouponFoliosModel.SearchCouponFoliosModel(),
            //     CouponFoliosInfoModel = new CouponFoliosModel.CouponFoliosInfoModel(),
            //     SearchDestinationsModel = new DestinationsModel.SearchDestinationsModel(),
            //     DestinationsInfoModel = new DestinationsModel.DestinationsInfoModel(),
            //     SearchExchangeRatesModel = new ExchangeRatesModel.SearchExchangeRatesModel(),
            //     ExchangeRatesInfoModel = new ExchangeRatesModel.ExchangeRatesInfoModel(),
            //     SearchLocationsModel = new LocationsModel.SearchLocationsModel(),
            //     LocationInfoModel = new LocationsModel.LocationInfoModel(),
            //     SearchOPCSModel = new OPCSModel.SearchOPCSModel(),
            //     SearchPlaceClasificationsModel = new PlaceClasificationsModel.SearchPlaceClasificationsModel(),
            //     PlaceClasificationsInfoModel = new PlaceClasificationsModel.PlaceClasificationsInfoModel(),
            //     SearchPlaceTypesModel = new PlaceTypesModel.SearchPlaceTypesModel(),
            //     PlaceTypesInfoModel = new PlaceTypesModel.PlaceTypesInfoModel(),
            //     SearchPointsOfSaleModel = new PointsOfSaleModel.SearchPointsOfSaleModel(),
            //     PointsOfSaleInfoModel = new PointsOfSaleModel.PointsOfSaleInfoModel(),
            //     SearchPromotionTeamsModel = new PromotionTeamsModel.SearchPromotionTeamsModel(),
            //     PromotionTeamsInfoModel = new PromotionTeamsModel.PromotionTeamsInfoModel(),
            //     PromoSearchModel = new PromosModel.PromoSearchModel(),
            //     PromoInfoModel = new PromosModel.PromoInfoModel(),
            //     SearchProvidersModel = new ProvidersModel.SearchProvidersModel(),
            //     ProviderInfoModel = new ProvidersModel.ProviderInfoModel(),
            //     SearchTransportationZonesModel = new TransportationZonesModel.SearchTransportationZonesModel(),
            //     TransportationZonesInfoModel = new TransportationZonesModel.TransportationZonesInfoModel(),
            //     SearchZonesModel = new ZonesModel.SearchZonesModel(),
            //     ZonesInfoModel = new ZonesModel.ZonesInfoModel(),
            //     //
            //     SeachParties = new PartiesSales.SeachParties(),
            //     PartiesInfo = new PartiesSales.PartiesInfo()

            // };

            //OPCSModel.OPCSInfoModel opcInfo = new OPCSModel.OPCSInfoModel();

            //var listPayingCompanies = CatalogsDataModel.PayingCompanies.PayingCompaniesCatalogs.FillDrpPayingCompanies();
            //listPayingCompanies.Insert(0, ListItems.Default());
            //opcInfo.OPCSInfo_DrpPayingCompanies = listPayingCompanies;

            //var listUsers = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey);
            //listUsers.Insert(0, ListItems.Default());
            //opcInfo.OPCSInfo_DrpUsers = listUsers;

            //var listCompanies = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
            //listCompanies.Insert(0, ListItems.Default());
            //opcInfo.OPCSInfo_DrpCompanies = listCompanies;

            //OPCSModel.OPCTeamInfoModel teamInfo = new OPCSModel.OPCTeamInfoModel();

            //var listPromotionTeams = CatalogsDataModel.PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams();
            //listPromotionTeams.Insert(0, ListItems.Default());
            //teamInfo.OPCTeamInfo_DrpPromotionTeams = listPromotionTeams;

            //var listJobPositions = CatalogsDataModel.General.FillDrpJobPositions();
            //listJobPositions.Insert(0, ListItems.Default());
            //teamInfo.OPCTeamInfo_DrpJobPositions = listJobPositions;


            //var listParentOPCs = ePlatBack.Models.DataModels.OpcDataModel.GetActiveOPCs();
            //listParentOPCs.Insert(0, ListItems.Default());
            //listParentOPCs.Insert(1, new SelectListItem { Text = "None", Value = "-1" });
            //teamInfo.OPCTeamInfo_DrpParentOpcs = listParentOPCs;

            //opcInfo.OPCSInfo_Team = teamInfo;
            //view.OPCSInfoModel = opcInfo;

            //    ViewData.Model = cvm;
            //    return View();
            //}
        }

        [Authorize]
        public ActionResult Index2()
        {
            if (AdminDataModel.VerifyAccess())
            {
                CatalogsViewModel cvm = new CatalogsViewModel();
                //if (RouteData.Values["catalog"] != null)
                //{
                string catalog = RouteData.Values["catalog"].ToString();
                var catalogTag = CatalogsDataModel.Capitals(catalog);
                long? sysComponentForCatalog = AdminDataModel.GetSysComponentIDByURL("/settings/catalogs2/get/" + catalog);
                ViewData.Model = new CatalogsViewModel
                {
                    // ReportsModel = new ReportsModel(),
                    catalogName = catalog,
                    catalogTag = catalogTag
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
            return result;
        }

        public PartialViewResult RenderCatalogs(string catalogName, bool? cache = true)
        {
            var returnPartial = "";
            dynamic model;
            switch (catalogName)
            {
                case "commissions":
                    {
                        returnPartial = "_CommissionsSearchCatalogPartial";
                        model = new CommissionsModel.SearchCommissionsModel();
                        return PartialView(returnPartial, model);
                    }
                case "accountingAccounts":
                    {
                        returnPartial = "_AccountingAccountsSearchCatalogPartial";
                        model = new AccountingAccountsModel.SearchAccountingAccountsModel();
                        //ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10638);
                        return PartialView(returnPartial, model);
                        //model = new AccountingAccountsModel.AccountingAccountInfoModel
                        //{
                        //    SearchAccountingAccountsModel = new AccountingAccountsModel.SearchAccountingAccountsModel()
                        //};
                        //returnPartial = "_AccountingAccountsManagementCatalogPartial";
                        //ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10638);
                        //return PartialView(returnPartial, model);
                    }
                case "budgets":
                    {
                        returnPartial = "_BudgetsSearchCatalogPartial";
                        model = new BudgetsModel.SearchBudgetsModel();
                        return PartialView(returnPartial, model);
                    }
                case "companies":
                    {
                        returnPartial = "_CompaniesSearchCatalogPartial";
                        model = new CompaniesModel.SearchCompaniesModel();
                        return PartialView(returnPartial, model);
                    }
                case "destinations":
                    {
                        returnPartial = "_DestinationsSearchCatalogPartial";
                        model = new DestinationsModel.SearchDestinationsModel();
                        return PartialView(returnPartial, model);
                    }
                case "exchangeRates":
                    {
                        returnPartial = "_ExchangeRatesSearchCatalogPartial";
                        model = new ExchangeRatesModel.SearchExchangeRatesModel();
                        return PartialView(returnPartial, model);
                    }
                case "locations":
                    {
                        returnPartial = "_LocationsSearchCatalogPartial";
                        model = new LocationsModel.SearchLocationsModel();
                        return PartialView(returnPartial, model);
                    }
                case "opcs":
                    {
                        returnPartial = "_OPCSSearchCatalogPartial";
                        model = new OPCSModel.SearchOPCSModel();
                        return PartialView(returnPartial, model);
                    }
                case "placeClasifications":
                    {
                        returnPartial = "_PlaceClasificationsSearchCatalogPartial";
                        model = new PlaceClasificationsModel.SearchPlaceClasificationsModel();
                        return PartialView(returnPartial, model);
                    }
                case "placeTypes":
                    {
                        returnPartial = "_PlaceTypesSearchCatalogPartial";
                        model = new PlaceTypesModel.SearchPlaceTypesModel();
                        return PartialView(returnPartial, model);
                    }
                case "providers":
                    {
                        returnPartial = "_ProvidersSearchCatalogPartial";
                        model = new ProvidersModel.SearchProvidersModel();
                        ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11305);
                        return PartialView(returnPartial, model);
                        //model = new ProvidersModel.ProviderInfoModel
                        //{
                        //    SearchProvidersModel = new ProvidersModel.SearchProvidersModel()
                        //};
                        //returnPartial = "_ProvidersManagementCatalogPartial";
                        //ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11305);
                        //return PartialView(returnPartial, model);
                    }
                case "promotionTeams":
                    {
                        returnPartial = "_PromotionTeamsSearchCatalogPartial";
                        model = new PromotionTeamsModel.SearchPromotionTeamsModel();
                        return PartialView(returnPartial, model);
                    }
                case "transportationZones":
                    {
                        returnPartial = "_TransportationZonesSearchCatalogPartial";
                        model = new TransportationZonesModel.SearchTransportationZonesModel();
                        return PartialView(returnPartial, model);
                    }
                case "zones":
                    {
                        returnPartial = "_ZonesSearchCatalogPartial";
                        model = new ZonesModel.SearchZonesModel();
                        return PartialView(returnPartial, model);
                    }
                case "couponFolios":
                    {
                        returnPartial = "_CouponFoliosSearchCatalogPartial";
                        model = new CouponFoliosModel.SearchCouponFoliosModel();
                        return PartialView(returnPartial, model);
                    }
                case "pointsOfSale":
                    {
                        returnPartial = "_PointsOfSaleSearchCatalogPartial";
                        model = new PointsOfSaleModel.SearchPointsOfSaleModel();
                        ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11310);
                        return PartialView(returnPartial, model);
                        //model = new PointsOfSaleModel.PointsOfSaleInfoModel
                        //{
                        //    SearchPointsOfSaleModel = new PointsOfSaleModel.SearchPointsOfSaleModel()
                        //};
                        //returnPartial = "_PointsOfSaleManagementCatalogPartial";
                        //ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11310);
                        //return PartialView(returnPartial, model);
                    }

                case "parties":
                    {
                        returnPartial = "_SalesRoomPartiesSearchPartial";
                        model = new PartiesSales.SeachParties();
                        return PartialView(returnPartial, model);
                    }
                case "Promos":
                    {
                        returnPartial = "_PromosSearchCatalogPartial";
                        model = new PromosModel.PromoSearchModel();
                        return PartialView(returnPartial, model);
                    }
                case "options":
                    {
                        returnPartial = "_OptionsSearchCatalogPartial";
                        model = new OptionsModel.OptionsSearchModel();
                        return PartialView(returnPartial, model);
                    }
                case "Banks":
                    {
                        returnPartial = "_BankSearchPartial";
                        model = new Banks.SearchBanks();
                        return PartialView(returnPartial,model);
                    }
                case "Boats":
                    {
                        returnPartial = "_BoatSearchCatalog";
                        model = new Boats.SearchBoats();
                        return PartialView(returnPartial, model);
                    }
                case "SalesChannels":
                    {
                        returnPartial = "_SalesChannelsSearchCatalog";
                        model = new SalesChanels.SearchSaleChannel();
                        return PartialView(returnPartial, model);
                    }
                case "Bracelets":
                    {
                        returnPartial = "_BraceletSearchCatalog";
                        model = new Bracelets.SearchBracelet();
                        return PartialView(returnPartial,model);
                    }
                case "Reminders":
                    {
                        returnPartial = "_ReminderSearchCatalog";
                        model = new Reminders.SearchReminder();
                        return PartialView(returnPartial,model);
                    }
                case "HotelPickUps":
                    {
                        returnPartial = "_HotelPickUpsPartial";
                        model = new HotelPickUps.SearchHotels();
                        return PartialView(returnPartial, model);
                    }
                case "marketCodes":
                    {
                        returnPartial = "_MarketCodesSearchPartial";
                        model = new MarketCodes.SearchMarketCodes();
                        return PartialView(returnPartial, model);
                    }
                case "userLeadSources":
                    {
                        returnPartial = "_UsersLeadSourcesSearchPartial";
                        model = new UsersLeadSources.SearchUserLeadSources();
                        return PartialView(returnPartial, model);
                    }
                default:
                    {
                        return PartialView(returnPartial);
                    }

            }

        }
        public ActionResult RenderCommissionsManagement()
        {
            return PartialView("_CommissionsManagementCatalogPartial", new CommissionsModel.CommissionsInfoModel());
        }
        public ActionResult RenderAccountingAccountsManagement()
        {
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11296);
            return PartialView("_AccountingAccountsManagementCatalogPartial", new AccountingAccountsModel.AccountingAccountInfoModel());
        }
        public ActionResult RenderBudgetsManagement()
        {
            return PartialView("_BudgetsManagementCatalogPartial", new BudgetsModel.BudgetInfoModel());
        }
        public ActionResult RenderCompaniesManagement()
        {
            return PartialView("_CompaniesManagementCatalogPartial", new CompaniesModel.CompanyInfoModel());
        }
        public ActionResult RenderDestinationsManagement()
        {
            return PartialView("_DestinationsManagementCatalogPartial", new DestinationsModel.DestinationsInfoModel());
        }
        public ActionResult RenderExchangeRatesManagement()
        {
            return PartialView("_ExchangeRatesManagementCatalogPartial", new ExchangeRatesModel.ExchangeRatesInfoModel());
        }
        public ActionResult RenderOpcsManagement()
        {
            return PartialView("_OPCSManagementCatalogPartial", new OPCSModel.OPCSInfoModel());
        }
        public ActionResult RenderLocationsManagement()
        {
            return PartialView("_LocationsManagementCatalogPartial", new LocationsModel.LocationInfoModel());
        }
        public ActionResult RenderPlaceClasificationsManagement()
        {
            return PartialView("_PlaceClasificationsManagementCatalogPartial", new PlaceClasificationsModel.PlaceClasificationsInfoModel());
        }

        public ActionResult RenderPlaceTypesManagement()
        {
            return PartialView("_PlaceTypesManagementCatalogPartial", new PlaceTypesModel.PlaceTypesInfoModel());
        }
        //77
        public ActionResult RenderProvidersManagement()
        {
            return PartialView("~/Views/Providers/_ProvidersManagementCatalogPartial.cshtml", new ProvidersModel.ProviderInfoModel());
        }

        public ActionResult RenderPromotionTeamsManagement()
        {
            return PartialView("_PromotionTeamsManagementCatalogPartial", new PromotionTeamsModel.PromotionTeamsInfoModel());
        }
        public ActionResult RenderTransportationZonesManagement()
        {
            return PartialView("_TransportationZonesManagementCatalogPartial", new TransportationZonesModel.TransportationZonesInfoModel());
        }
        public ActionResult RenderZonesManagement()
        {
            return PartialView("_ZonesManagementCatalogPartial", new ZonesModel.ZonesInfoModel());
        }
        public ActionResult RenderCouponFoliosManagement()
        {
            return PartialView("_CouponFoliosManagementCatalogPartial", new CouponFoliosModel.CouponFoliosInfoModel());
        }
        public ActionResult RenderPointsOfSaleManagement()
        {
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11310);
            return PartialView("_PointsOfSaleManagementCatalogPartial", new PointsOfSaleModel.PointsOfSaleInfoModel());
        }

        public ActionResult RenderPartiesManagement()
        {
            return PartialView("_SalesRoomsPartiesManagementPartial", new PartiesSales.PartiesInfo());
        }
        public ActionResult RenderPromosManagement()
        {
            return PartialView("_PromosManagementCatalogPartial", new PromosModel.PromoInfoModel());
        }
        public ActionResult RenderOptionsManagement()
        {
            return PartialView("_OptionsManagementPartial", new OptionsModel.OptionsInfomodel());
        }
        public ActionResult RenderDestinationsDescriptionCatalogPartial()
        {
            return PartialView("_DestinationsDescriptionsCatalogPartial", new DestinationsModel.DestinationDescriptionsModel());
        }

        //Banks
        public ActionResult RenderBankInfo()
        {
            return PartialView("_BankInfoPartial", new Banks.BankItem());
        }
        //Boats
        public ActionResult RenderPartiesBoats()
        {
            return PartialView("_BoatManagementCatalog", new Boats.NewBoat());
        }

        //SalesChannels
        public ActionResult RenderPartiesSalesChannels() 
        {
            return PartialView("_SalesChannelManagementCatalog", new SalesChanels.NewSalesChannel());
        }
        
        //Bracelets
        public ActionResult RenderPartiesBracelets()
        {
            return PartialView("_BraceletsManagementCatalog", new Bracelets.NewBracelet());
        }
        //Reminders
        public ActionResult RenderPartiesReminders()
        {
            return PartialView("_ReminderManagementCatalog", new Reminders.NewReminder());
        }
        //accounting accounts
        public async Task<ActionResult> SearchAccountingAccounts(AccountingAccountsModel.SearchAccountingAccountsModel model)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            AccountingAccountsModel.SearchAccountingAccountsModel szm = new AccountingAccountsModel.SearchAccountingAccountsModel();
            szm.ListAccountingAccounts = cdm.SearchAccountingAccounts(model);
            szm.ListAccountingAccountsSummary = cdm.GetAccountinAccountsSummary(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_AccountingAccountsSearchResultsCatalogPartial", szm);
        }
        public async Task<JsonResult> SaveAccountingAccount(AccountingAccountsModel.AccountingAccountInfoModel model)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            AttemptResponse attempt = cdm.SaveAccountingAccount(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        public async Task<JsonResult> GetAccountingAccount(int AccountingAccountInfo_AccountingAccountID)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetAccountingAccount(AccountingAccountInfo_AccountingAccountID);
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }


            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }
        public async Task<JsonResult> DeleteAccountingAccount(int targetID)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            AttemptResponse attempt = cdm.DeleteAccountingAccount(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //commissions
        public async Task<ActionResult> SearchCommissions(CommissionsModel.SearchCommissionsModel model)
        {
            CatalogsDataModel.Commissions cdm = new CatalogsDataModel.Commissions();
            CommissionsModel.SearchCommissionsModel scm = new CommissionsModel.SearchCommissionsModel();
            scm.ListCommissions = cdm.SearchCommissions(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_CommissionsSearchResultsCatalogPartial", scm);
        }

        //
        public async Task<ActionResult> SearchMarketCodes(MarketCodes.SearchMarketCodes model)
        {
            CatalogsDataModel.MarketCodesModel mkm = new CatalogsDataModel.MarketCodesModel();
            MarketCodes.SearchMarketCodes smk = new MarketCodes.SearchMarketCodes();
            smk.ListResults = mkm.SearchMarketCodes(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_MarketCodesSearchResultsPartial", smk);
        }

        public async Task<JsonResult> GetMarketCode(int marketCodeID)
        {
            CatalogsDataModel.MarketCodesModel cdm = new CatalogsDataModel.MarketCodesModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetMarketCodeInfo(marketCodeID);
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }


            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        public async Task<JsonResult> AssignMarketCodes(MarketCodes.MarketCodeInfo model)
        {
            CatalogsDataModel.MarketCodesModel cdm = new CatalogsDataModel.MarketCodesModel();
            AttemptResponse attempt = model.MarketCodeInfo_Assign ? cdm.AssignMarketCodes(model) : cdm.SaveMarketCode(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //public async Task<JsonResult> SaveMarketCode(MarketCodes.MarketCodeInfo model)
        //{
        //    CatalogsDataModel.MarketCodesModel cdm = new CatalogsDataModel.MarketCodesModel();
        //    AttemptResponse attempt = cdm.SaveMarketCode(model);
        //    string errorLocation = "";
        //    if (attempt.Exception != null)
        //    {
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }
        //    var request = HttpContext.Request;
        //    var urlMethod = new UserSession().Url;
        //    var terminals = new UserSession().Terminals;
        //    var terminalID = (string)null;
        //    if (terminals.Count() > 0)
        //    {
        //        terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
        //    }

        //    Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request, terminalID));

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ItemID = attempt.ObjectID,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
        //        InnerException = Debugging.GetInnerException(attempt.Exception)
        //    });
        //}

        public ActionResult RenderMarketCodesManagement()
        {
            return PartialView("_MarketCodesManagementPartial", new MarketCodes.MarketCodeInfo());
        }

        public JsonResult GetProgramsAndSources()
        {

            return Json(CatalogsDataModel.MarketCodesModel.GetProgramsAndSources(), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SaveCommission(CommissionsModel.CommissionsInfoModel model)
        {
            CatalogsDataModel.Commissions cdm = new CatalogsDataModel.Commissions();
            AttemptResponse attempt = cdm.SaveCommission(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetCommission(int CommissionsInfo_CommissionID)
        {
            CatalogsDataModel.Commissions cdm = new CatalogsDataModel.Commissions();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetCommission(CommissionsInfo_CommissionID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteCommission(int targetID)
        {
            CatalogsDataModel.Commissions cdm = new CatalogsDataModel.Commissions();
            AttemptResponse attempt = cdm.DeleteCommission(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //budgets
        public async Task<ActionResult> SearchBudgets(BudgetsModel.SearchBudgetsModel model)
        {
            CatalogsDataModel.Budgets cdm = new CatalogsDataModel.Budgets();
            BudgetsModel.SearchBudgetsModel sbm = new BudgetsModel.SearchBudgetsModel();
            sbm.ListBudgets = cdm.SearchBudgets(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_BudgetsSearchResultsCatalogPartial", sbm);
        }

        public async Task<JsonResult> SaveBudget(BudgetsModel.BudgetInfoModel model)
        {
            CatalogsDataModel.Budgets cdm = new CatalogsDataModel.Budgets();
            AttemptResponse attempt = cdm.SaveBudget(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetBudget(int BudgetInfo_BudgetID)
        {
            CatalogsDataModel.Budgets cdm = new CatalogsDataModel.Budgets();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetBudget(BudgetInfo_BudgetID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteBudget(int targetID)
        {
            CatalogsDataModel.Budgets cdm = new CatalogsDataModel.Budgets();
            AttemptResponse attempt = cdm.DeleteBudget(targetID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br >" + Debugging.GetErrorLocation(attempt.Exception);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //companies
        public async Task<ActionResult> SearchCompanies(CompaniesModel.SearchCompaniesModel model)
        {
            CatalogsDataModel.Companies cdm = new CatalogsDataModel.Companies();
            CompaniesModel.SearchCompaniesModel scm = new CompaniesModel.SearchCompaniesModel();
            scm.ListCompanies = cdm.SearchCompanies(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_CompaniesSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> SaveCompany(CompaniesModel.CompanyInfoModel model)
        {
            CatalogsDataModel.Companies cdm = new CatalogsDataModel.Companies();
            AttemptResponse attempt = cdm.SaveCompany(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetCompany(int CompanyInfo_CompanyID)
        {
            CatalogsDataModel.Companies cdm = new CatalogsDataModel.Companies();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetCompany(CompanyInfo_CompanyID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteCompany(int targetID)
        {
            CatalogsDataModel.Companies cdm = new CatalogsDataModel.Companies();
            AttemptResponse attempt = cdm.DeleteCompany(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //couponFolios
        public async Task<ActionResult> SearchCouponFolios(CouponFoliosModel.SearchCouponFoliosModel model)
        {
            CatalogsDataModel.CouponFolios cdm = new CatalogsDataModel.CouponFolios();
            CouponFoliosModel.SearchCouponFoliosModel scm = new CouponFoliosModel.SearchCouponFoliosModel();
            scm.ListCouponFolios = cdm.SearchCouponFolios(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_CouponFoliosSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> SaveCouponFolio(CouponFoliosModel.CouponFoliosInfoModel model)
        {
            CatalogsDataModel.CouponFolios cdm = new CatalogsDataModel.CouponFolios();
            AttemptResponse attempt = cdm.SaveCouponFolio(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetCouponFolio(int CouponFoliosInfo_CouponFolioID)
        {
            CatalogsDataModel.CouponFolios cdm = new CatalogsDataModel.CouponFolios();
            var response = cdm.GetCouponFolio(CouponFoliosInfo_CouponFolioID);
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

        public async Task<JsonResult> DeleteCouponFolio(int targetID)
        {
            CatalogsDataModel.CouponFolios cdm = new CatalogsDataModel.CouponFolios();
            AttemptResponse attempt = cdm.DeleteCouponFolio(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //destinations
        public PartialViewResult RenderDestinations()
        {
            var dim = new DestinationsModel.DestinationsInfoModel
            {
                DestinationDescriptionsModel = new DestinationsModel.DestinationDescriptionsModel(),
            };
            return PartialView("_DestinationsManagementCatalogPartial", dim);
        }

        public async Task<ActionResult> SearchDestinations(DestinationsModel.SearchDestinationsModel model)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            DestinationsModel.SearchDestinationsModel sdm = new DestinationsModel.SearchDestinationsModel();
            sdm.ListDestinations = cdm.SearchDestinations(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_DestinationsSearchResultsCatalogPartial", sdm);
        }

        public async Task<JsonResult> SaveDestination(DestinationsModel.DestinationsInfoModel model)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            AttemptResponse attempt = cdm.SaveDestination(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetDestination(int DestinationInfo_DestinationID)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetDestination(DestinationInfo_DestinationID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteDestination(int targetID)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            AttemptResponse attempt = cdm.DeleteDestination(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> SaveDestinationDescription(DestinationsModel.DestinationDescriptionsModel model)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            AttemptResponse attempt = cdm.SaveDestinationDescription(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetDestinationDescriptions(int DestinationInfo_DestinationID)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetDestinationDescription(DestinationInfo_DestinationID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> GetDestinationDescription(int DestinationDescription_DestinationDescriptionID)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetDestinationDescription(DestinationDescription_DestinationDescriptionID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteDestinationDescription(int targetID)
        {
            CatalogsDataModel.Destinations cdm = new CatalogsDataModel.Destinations();
            AttemptResponse attempt = cdm.DeleteDestinationDescription(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //exchangeRates
        public async Task<ActionResult> SearchExchangeRates(ExchangeRatesModel.SearchExchangeRatesModel model)
        {
            CatalogsDataModel.ExchangeRates cdm = new CatalogsDataModel.ExchangeRates();
            ExchangeRatesModel.SearchExchangeRatesModel scm = new ExchangeRatesModel.SearchExchangeRatesModel();
            scm.ListExchangeRates = cdm.SearchExchangeRates(model);
            UserSession session = new UserSession();
            var url = new UserSession().Url;
            var request = HttpContext.Request;
            string terminalID = (string)null;
            if(session.Terminals.Count()>0)
            {
                terminalID = session.Terminals.Count() > 2 ? session.Terminals.Split(',').Select(m => m).ToArray().First() : session.Terminals;
            }

            var terminals = new UserSession().Terminals;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, url, request, terminalID));
            return PartialView("_ExchangeRatesSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> SaveExchangeRate(ExchangeRatesModel.ExchangeRatesInfoModel model)
        {
            CatalogsDataModel.ExchangeRates cdm = new CatalogsDataModel.ExchangeRates();
            AttemptResponse attempt = cdm.SaveExchangeRate(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetExchangeRate(int ExchangeRatesInfo_ExchangeRateID)
        {
            CatalogsDataModel.ExchangeRates cdm = new CatalogsDataModel.ExchangeRates();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetExchangeRate(ExchangeRatesInfo_ExchangeRateID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteExchangeRate(int targetID)
        {
            CatalogsDataModel.ExchangeRates cdm = new CatalogsDataModel.ExchangeRates();
            AttemptResponse attempt = cdm.DeleteExchangeRate(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //public JsonResult CloneExchangeRate(long exchangeRateID, decimal exchangeRate, bool isUpdate)
        //{
        //    CatalogsDataModel.ExchangeRates cdm = new CatalogsDataModel.ExchangeRates();
        //    AttemptResponse attempt = cdm.CloneExchangeRate(exchangeRateID, exchangeRate, isUpdate);
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

        //locations
        public async Task<ActionResult> SearchLocations(LocationsModel.SearchLocationsModel model)
        {
            CatalogsDataModel.Locations cdm = new CatalogsDataModel.Locations();
            LocationsModel.SearchLocationsModel scm = new LocationsModel.SearchLocationsModel();
            scm.ListLocations = cdm.SearchLocations(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_LocationsSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> SaveLocation(LocationsModel.LocationInfoModel model)
        {
            CatalogsDataModel.Locations cdm = new CatalogsDataModel.Locations();
            AttemptResponse attempt = cdm.SaveLocation(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetLocation(int LocationInfo_LocationID)
        {
            CatalogsDataModel.Locations cdm = new CatalogsDataModel.Locations();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetLocation(LocationInfo_LocationID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteLocation(int targetID)
        {
            CatalogsDataModel.Locations cdm = new CatalogsDataModel.Locations();
            AttemptResponse attempt = cdm.DeleteLocation(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //opcs
        public async Task<ActionResult> SearchOPCS(OPCSModel.SearchOPCSModel model)
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            OPCSModel.SearchOPCSModel scm = new OPCSModel.SearchOPCSModel();
            scm.ListOPCS = cdm.SearchOPCS(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_OPCSSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> SaveOPC(OPCSModel.OPCSInfoModel model)
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            AttemptResponse attempt = cdm.SaveOPC(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetOPC(int OPCSInfo_OPCID)
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetOPC(OPCSInfo_OPCID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public JsonResult SendChangesReport()
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            AttemptResponse attempt = cdm.SendChangesReport();
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }

        public FileResult GetOPCsList(string id)
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            string table = cdm.GetOPCsListTable(id);
            Response.HeaderEncoding = Encoding.UTF8;
            return File(Encoding.UTF8.GetBytes(table), MediaTypeNames.Application.Octet, "Lista Actualizada de Promotores " + DateTime.Today.ToString("yyyyMMdd") + ".xls");
        }

        public FileResult GetOPCsHistory(string id)
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            string table = cdm.GetOPCsHistoryTable(id);
            Response.HeaderEncoding = Encoding.UTF8;
            return File(Encoding.UTF8.GetBytes(table), MediaTypeNames.Application.Octet, "Historial de Altas y Bajas " + DateTime.Today.ToString("yyyyMMdd") + ".xls");
        }

        public async Task<JsonResult> DeleteOPC(int targetID)
        {
            CatalogsDataModel.OPCS cdm = new CatalogsDataModel.OPCS();
            AttemptResponse attempt = cdm.DeleteOPC(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //placeClasifications
        public async Task<ActionResult> SearchPlaceClasifications(PlaceClasificationsModel.SearchPlaceClasificationsModel model)
        {
            CatalogsDataModel.PlaceClasifications cdm = new CatalogsDataModel.PlaceClasifications();
            PlaceClasificationsModel.SearchPlaceClasificationsModel pcm = new PlaceClasificationsModel.SearchPlaceClasificationsModel();
            pcm.ListPlaceClasifications = cdm.SearchPlaceClasifications(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PlaceClasificationsSearchResultsCatalogPartial", pcm);
        }

        public async Task<JsonResult> SavePlaceClasification(PlaceClasificationsModel.PlaceClasificationsInfoModel model)
        {
            CatalogsDataModel.PlaceClasifications cdm = new CatalogsDataModel.PlaceClasifications();
            AttemptResponse attempt = cdm.SavePlaceClasification(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetPlaceClasification(int PlaceClasificationInfo_PlaceClasificationID)
        {
            CatalogsDataModel.PlaceClasifications cdm = new CatalogsDataModel.PlaceClasifications();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetPlaceClasification(PlaceClasificationInfo_PlaceClasificationID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        public async Task<JsonResult> DeletePlaceClasification(int targetID)
        {
            CatalogsDataModel.PlaceClasifications cdm = new CatalogsDataModel.PlaceClasifications();
            AttemptResponse attempt = cdm.DeletePlaceClasification(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //placeTypes
        public async Task<ActionResult> SearchPlaceTypes(PlaceTypesModel.SearchPlaceTypesModel model)
        {
            CatalogsDataModel.PlaceTypes cdm = new CatalogsDataModel.PlaceTypes();
            PlaceTypesModel.SearchPlaceTypesModel spm = new PlaceTypesModel.SearchPlaceTypesModel();
            spm.ListPlaceTypes = cdm.SearchPlaceTypes(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }
            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PlaceTypesSearchResultsCatalogPartial", spm);
        }

        public async Task<JsonResult> SavePlaceType(PlaceTypesModel.PlaceTypesInfoModel model)
        {
            CatalogsDataModel.PlaceTypes cdm = new CatalogsDataModel.PlaceTypes();
            AttemptResponse attempt = cdm.SavePlaceType(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetPlaceType(int PlaceTypeInfo_PlaceTypeID)
        {
            CatalogsDataModel.PlaceTypes cdm = new CatalogsDataModel.PlaceTypes();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetPlaceType(PlaceTypeInfo_PlaceTypeID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeletePlaceType(int targetID)
        {
            CatalogsDataModel.PlaceTypes cdm = new CatalogsDataModel.PlaceTypes();
            AttemptResponse attempt = cdm.DeletePlaceType(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //points of sale
        public async Task<ActionResult> SearchPointsOfSale(PointsOfSaleModel.SearchPointsOfSaleModel model)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            PointsOfSaleModel.SearchPointsOfSaleModel spsm = new PointsOfSaleModel.SearchPointsOfSaleModel();
            spsm.ListPointsOfSale = cdm.SearchPointsOfSale(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PointsOfSaleSearchResultsCatalogPartial", spsm);
        }
        public async Task<JsonResult> SavePointOfSale(PointsOfSaleModel.PointsOfSaleInfoModel model)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            AttemptResponse attempt = cdm.SavePointOfSale(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        public async Task<JsonResult> GetPointOfSale(int PointsOfSaleInfo_PointOfSaleID)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetPointOfSale(PointsOfSaleInfo_PointOfSaleID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }
        public async Task<JsonResult> DeletePointOfSale(int targetID)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            AttemptResponse attempt = cdm.DeletePointOfSale(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //promotion teams
        public async Task<ActionResult> SearchPromotionTeams(PromotionTeamsModel.SearchPromotionTeamsModel model)
        {
            CatalogsDataModel.PromotionTeams cdm = new CatalogsDataModel.PromotionTeams();
            PromotionTeamsModel.SearchPromotionTeamsModel sptm = new PromotionTeamsModel.SearchPromotionTeamsModel();
            sptm.ListPromotionTeams = cdm.SearchPromotionTeams(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_PromotionTeamsSearchResultsCatalogPartial", sptm);
        }

        public async Task<JsonResult> SavePromotionTeam(PromotionTeamsModel.PromotionTeamsInfoModel model)
        {
            CatalogsDataModel.PromotionTeams cdm = new CatalogsDataModel.PromotionTeams();
            AttemptResponse attempt = cdm.SavePromotionTeam(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetPromotionTeam(int PromotionTeamsInfo_PromotionTeamID)
        {
            CatalogsDataModel.PromotionTeams cdm = new CatalogsDataModel.PromotionTeams();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetPromotionTeam(PromotionTeamsInfo_PromotionTeamID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeletePromotionTeam(int targetID)
        {
            CatalogsDataModel.PromotionTeams cdm = new CatalogsDataModel.PromotionTeams();
            AttemptResponse attempt = cdm.DeletePromotionTeam(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //providers
        public async Task<ActionResult> SearchProviders(ProvidersModel.SearchProvidersModel model)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            ProvidersModel.SearchProvidersModel scm = new ProvidersModel.SearchProvidersModel();
            scm.ListProviders = cdm.SearchProviders(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ProvidersSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> SaveProvider(ProvidersModel.ProviderInfoModel model)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            AttemptResponse attempt = cdm.SaveProvider(model, this.ControllerContext);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetProvider(int ProviderInfo_ProviderID)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetProvider(ProviderInfo_ProviderID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteProvider(int targetID)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            AttemptResponse attempt = cdm.DeleteProvider(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetFilesOfProvider(int providerID)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            return Json(cdm.GetFilesOfProvider(providerID), JsonRequestBehavior.AllowGet);
        }

        public PictureDataModel.FineUploaderResult UploadFile(PictureDataModel.FineUpload upload, string path, bool newProvider)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            return (cdm.UploadFile(upload, path, newProvider));
        }

        public JsonResult DeleteFileOfProvider(string file)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            AttemptResponse attempt = cdm.DeleteFileOfProvider(file);
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

            Task.Run(() => CreateTableUserLogActivityAsync(file, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }


        //trasportation
        public async Task<ActionResult> SearchTransportationZones(TransportationZonesModel.SearchTransportationZonesModel model)
        {
            CatalogsDataModel.TransportationZones cdm = new CatalogsDataModel.TransportationZones();
            TransportationZonesModel.SearchTransportationZonesModel stm = new TransportationZonesModel.SearchTransportationZonesModel();
            stm.ListTransportationZones = cdm.SearchTransportationZones(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_TransportationZonesSearchResultsCatalogPartial", stm);
        }

        public async Task<JsonResult> SaveTransportationZone(TransportationZonesModel.TransportationZonesInfoModel model)
        {
            CatalogsDataModel.TransportationZones cdm = new CatalogsDataModel.TransportationZones();
            AttemptResponse attempt = cdm.SaveTransportationZone(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetTransportationZone(int TransportationZoneInfo_TransportationZoneID)
        {
            CatalogsDataModel.TransportationZones cdm = new CatalogsDataModel.TransportationZones();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetTransportationZone(TransportationZoneInfo_TransportationZoneID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteTransportationZone(int targetID)
        {
            CatalogsDataModel.TransportationZones cdm = new CatalogsDataModel.TransportationZones();
            AttemptResponse attempt = cdm.DeleteTransportationZone(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //users - lead sources
        public ActionResult RenderUsersLeadSources()
        {
            return PartialView("_UsersLeadSourcesManagementPartial", new UsersLeadSources.UserLeadSources());
        }

        public async Task<ActionResult> SearchUsersLeadSources(UsersLeadSources.SearchUserLeadSources model)
        {
            CatalogsDataModel.UserLeadSources cdm = new CatalogsDataModel.UserLeadSources();
            UsersLeadSources.SearchUserLeadSources sdm = new UsersLeadSources.SearchUserLeadSources();
            sdm.ListResults = cdm.SearchUsersLeadSources(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_UsersLeadSourcesResultsPartial", sdm);
        }

        public async Task<JsonResult> SaveUserLeadSource(UsersLeadSources.UserLeadSources model)
        {
            CatalogsDataModel.UserLeadSources cdm = new CatalogsDataModel.UserLeadSources();
            AttemptResponse attempt = cdm.SaveUserLeadSource(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetUserLeadSource(int id)
        {
            CatalogsDataModel.UserLeadSources cdm = new CatalogsDataModel.UserLeadSources();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetUserLeadSource(id);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        //zones
        public async Task<ActionResult> SearchZones(ZonesModel.SearchZonesModel model)
        {
            CatalogsDataModel.Zones cdm = new CatalogsDataModel.Zones();
            ZonesModel.SearchZonesModel szm = new ZonesModel.SearchZonesModel();
            szm.ListZones = cdm.SearchZones(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_ZonesSearchResultsCatalogPartial", szm);
        }
        //
        public async Task<JsonResult> SaveZone(ZonesModel.ZonesInfoModel model)
        {
            CatalogsDataModel.Zones cdm = new CatalogsDataModel.Zones();
            AttemptResponse attempt = cdm.SaveZone(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> GetZone(int ZoneInfo_ZoneID)
        {
            CatalogsDataModel.Zones cdm = new CatalogsDataModel.Zones();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = cdm.GetZone(ZoneInfo_ZoneID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));
            return Json(response);
        }

        public async Task<JsonResult> DeleteZone(int targetID)
        {
            CatalogsDataModel.Zones cdm = new CatalogsDataModel.Zones();
            AttemptResponse attempt = cdm.DeleteZone(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        #region SalesRoomParties
        //SalesRoomParties
        public async Task<ActionResult> SearchSalesRooms(PartiesSales.SeachParties model)
        {
            CatalogsDataModel.PartiesSalesDataModel cdm = new CatalogsDataModel.PartiesSalesDataModel();
            PartiesSales.SalesRoomPartiesViewModel ssp = new PartiesSales.SalesRoomPartiesViewModel();
            ssp.SearchSalesRoomResults = cdm.SearchParties(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));
            return PartialView("_SalesRoomsPartiesResult", ssp);
        }
        public async Task<JsonResult> SaveSalesRoomParties(PartiesSales.PartiesInfo model)
        {
            CatalogsDataModel.PartiesSalesDataModel cdm = new CatalogsDataModel.PartiesSalesDataModel();
            AttemptResponse attempt = cdm.SaveParties(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        public async Task<JsonResult> DeleteSalesRoomParties(int targetID)
        {
            CatalogsDataModel.PartiesSalesDataModel cdm = new CatalogsDataModel.PartiesSalesDataModel();
            AttemptResponse attempt = cdm.DeleteParties(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        public JsonResult DuplicateSalesRoomParties(string TakeFromDate, string CopyToDate, string TerminalID)
        {
            CatalogsDataModel.PartiesSalesDataModel cdm = new CatalogsDataModel.PartiesSalesDataModel();
            AttemptResponse attempt = cdm.SalesRoomsDuplicate(TakeFromDate, CopyToDate, TerminalID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(TakeFromDate, "", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        public async Task<JsonResult> GetSalesRoomParties(int PartiesInfo_SalesPartyID)
        {
            CatalogsDataModel.PartiesSalesDataModel cdm = new CatalogsDataModel.PartiesSalesDataModel();
            var json = new System.Web.Script.Serialization.JavaScriptSerializer();
            var response = cdm.FindSalesRooms(PartiesInfo_SalesPartyID);
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
        //endparties
        #endregion

        #region "Options"
        public async Task<ActionResult> SearchOptions(OptionsModel.OptionsSearchModel model)
        {
            CatalogsDataModel.Options cdm = new CatalogsDataModel.Options();
            OptionsModel.OptionsSearchModel scm = new OptionsModel.OptionsSearchModel();
            scm.ListOptions = cdm.SearchOptions(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_OptionsSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> GetOption(int OptionsInfo_OptionID)
        {
            CatalogsDataModel.Options cdm = new CatalogsDataModel.Options();
            var response = cdm.GetOption(OptionsInfo_OptionID);
            var urlMethod = new UserSession().Url;
            var request = HttpContext.Request;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        public async Task<JsonResult> SaveOption(OptionsModel.OptionsInfomodel model)
        {
            CatalogsDataModel.Options cdm = new CatalogsDataModel.Options();
            AttemptResponse attempt = cdm.SaveOption(model);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteOption(int targetID)
        {
            CatalogsDataModel.Options cdm = new CatalogsDataModel.Options();
            AttemptResponse attempt = cdm.DeleteOption(targetID);
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
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        #endregion

        #region Promos
        //public ActionResult SearchPromos(PromosModel.PromoSearchModel model)
        //{
        //    CatalogsDataModel.Promos cdm =  new CatalogsDataModel.Promos();
        //    PromosModel.PromoSearchModel ssp = new PromosModel.PromoSearchModel();
        //    ssp.ListPromos = cdm.SearchPromos(model);
        //    return PartialView("_PromosSearchResultsCatalogPartial",ssp);
        //}

        //public JsonResult GetPromo(int PromoInfo_PromoID)
        //{
        //    CatalogsDataModel.Promos cdm = new CatalogsDataModel.Promos();
        //    return Json(cdm.GetPromo(PromoInfo_PromoID));
        //} 

        //public JsonResult SavePromo(PromosModel.PromoInfoModel model)
        //{
        //    CatalogsDataModel.Promos cdm = new CatalogsDataModel.Promos();
        //    AttemptResponse attempt = cdm.SavePromo(model);
        //    string errorLocation = "";
        //    if(attempt.Exception!=null)
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

        //public JsonResult DeletePromo(int targetID)
        //{
        //    CatalogsDataModel.Promos cdm = new CatalogsDataModel.Promos();
        //    AttemptResponse attempt = cdm.DeletePromo(targetID);
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
        #endregion



        //general
        public JsonResult GetDDLData(string itemType, string itemID)
        {
            CatalogsDataModel.General cdm = new CatalogsDataModel.General();
            return Json(cdm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }

        #region Banks
        //Banks
        [HttpPost]
        public ActionResult SaveBank(Banks.BankItem model)
        {
            CatalogsDataModel.BanksModel cdm = new CatalogsDataModel.BanksModel();
            AttemptResponse attempt = cdm.SaveBank(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        [HttpDelete]
        public JsonResult DeleteBank(int id)
        {
            CatalogsDataModel.BanksModel cmd = new CatalogsDataModel.BanksModel();
            AttemptResponse attempt = cmd.DeleteBank(id);
            string erroLocation = "";
            if (attempt.Exception != null)
            {
                erroLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(BankId, "Delete", attempt, urlMethod, request));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SearchBanks(Banks.SearchBanks model)
        {
            CatalogsDataModel.BanksModel cdm = new CatalogsDataModel.BanksModel();
            Banks.BanksViewModel ssp = new Banks.BanksViewModel();
            return Json(cdm.SearchBanks(model), JsonRequestBehavior.AllowGet);

            //return PartialView("_BankSearchResultCatalog", ssp);
        }

        //public JsonResult GetBankFromModel (int BankID)
        //{
        //    CatalogsDataModel.BanksModel cdm = new CatalogsDataModel.BanksModel();
        //    //var request = HttpContext.Request;
        //   // var urlMethod = new UserSession().Url;
        //    var response = cdm.GetBanks(BankID);
        //  //  Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request));
        //    return Json(response);
        //}
        #endregion

        #region Boats
        public ActionResult SaveBoat(Boats.NewBoat model) 
        {
            CatalogsDataModel.BoatsModel cdm = new CatalogsDataModel.BoatsModel();
            AttemptResponse attempt = cdm.SaveBoat(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request));
            return Json(new
            {
                ResponseType=attempt.Type,
                ItemID= attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage= Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public JsonResult DeleteBoat(int BoatID) 
        {
            CatalogsDataModel.BoatsModel cmd = new CatalogsDataModel.BoatsModel();
            AttemptResponse attempt = cmd.DeleteBoat(BoatID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(BoatID, "Delete", attempt, urlMethod, request));

            return Json(new 
            { 
                ResponseType= attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException= Debugging.GetInnerException(attempt.Exception)                
            });
        }
        public ActionResult SearchBoats(Boats.SearchBoats model)
        {
            CatalogsDataModel.BoatsModel cdm = new CatalogsDataModel.BoatsModel();
            Boats.BoatsViewModel ssp = new Boats.BoatsViewModel();
            ssp.SearchBoatResults = cdm.SearchBoats(model);
            return PartialView("_BoatSearchResultCatalog", ssp);

        }
        public JsonResult GetBoatFromModel(int BoatID) 
        {
            CatalogsDataModel.BoatsModel cdm = new CatalogsDataModel.BoatsModel();
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;
            var response = cdm.GetBoats(BoatID);
            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request));
            return Json(response);
        }
        #endregion

        #region Sales Channel
        public ActionResult SaveSalesChannels(SalesChanels.NewSalesChannel model)
        {
            CatalogsDataModel.SalesChannelsModel cdm = new CatalogsDataModel.SalesChannelsModel();
            AttemptResponse attempt = cdm.SaveSaleChannel(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }

            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request));
            return Json(new
            {
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });

        }
        public JsonResult DeleteSalesChannels(int SalesChannelsID) 
        {
            CatalogsDataModel.SalesChannelsModel cdm = new CatalogsDataModel.SalesChannelsModel();
            AttemptResponse attempt = cdm.DeleteSaleChannel(SalesChannelsID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(SalesChannelsID, "Delete", attempt, urlMethod, request));

            return Json(new 
            { 
                ResponseType= attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public ActionResult SearchSalesChannels(SalesChanels.SearchSaleChannel model)
        {
            CatalogsDataModel.SalesChannelsModel cdm = new CatalogsDataModel.SalesChannelsModel();
            SalesChanels.SalesChannelViewModel ssp = new SalesChanels.SalesChannelViewModel();
            ssp.SearchSalesChannelsResults = cdm.SearchSalesChannels(model);
            return PartialView("_SalesChannelsSearchResultCatalog", ssp);
        }
        public JsonResult GetSalesChannels(int SalesChannelID)
        {
            CatalogsDataModel.SalesChannelsModel cdm = new CatalogsDataModel.SalesChannelsModel();
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;
            var response = cdm.GetSalesChannels(SalesChannelID);
            //Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request));
            return Json(response);
        }
        #endregion

        #region Bracelets
        public ActionResult SaveBracelets(Bracelets.NewBracelet model)
        {
            CatalogsDataModel.BraceletsModel cdm = new CatalogsDataModel.BraceletsModel();
            AttemptResponse attempt = cdm.SaveBracelet(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation="<br/>"+Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public ActionResult SearchBracelets(Bracelets.SearchBracelet model)
        {
            CatalogsDataModel.BraceletsModel cdm = new CatalogsDataModel.BraceletsModel();
            Bracelets.BraceletsViewModel ssp = new Bracelets.BraceletsViewModel();
            ssp.SearchBraceletResult = cdm.SearchBracelets(model);
            return PartialView("_BraceletsSearchResultCatalog", ssp);
        }
        public JsonResult DeleteBracelets(int BraceletID)
        {
            CatalogsDataModel.BraceletsModel cdm = new CatalogsDataModel.BraceletsModel();
            AttemptResponse attempt = cdm.DeleteBracelet(BraceletID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public JsonResult GetBracelets(int BraceletID)
        {
            CatalogsDataModel.BraceletsModel cdm = new CatalogsDataModel.BraceletsModel();
            var response = cdm.GetBracelets(BraceletID);
            return Json(response);
        }
        #endregion

        #region Reminders
        public ActionResult SaveReminders(Reminders.NewReminder model)
        {
            CatalogsDataModel.ReminderModel cdm = new CatalogsDataModel.ReminderModel();
            AttemptResponse attempt = cdm.SaveReminder(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public ActionResult SearchReminders(Reminders.SearchReminder model)
        {
            CatalogsDataModel.ReminderModel cdm = new CatalogsDataModel.ReminderModel();
            Reminders.RemindersViewModel ssp = new Reminders.RemindersViewModel();
            ssp.SearchRemindersResult = cdm.SearchReminder(model);
            return PartialView("_ReminderSearchResultCatalog",ssp);
        }
        public JsonResult DeleteReminders(long ReminderID)
        {
            CatalogsDataModel.ReminderModel cdm = new CatalogsDataModel.ReminderModel();
            AttemptResponse attempt = cdm.DeleteReminder(ReminderID);
            string errorLocation = "";
            if(attempt.Exception!=null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public JsonResult GetReminders(int ReminderID)
        {
            CatalogsDataModel.ReminderModel cdm = new CatalogsDataModel.ReminderModel();
            var response = cdm.GetReminders(ReminderID);
            return Json(response);
        }
        #endregion

        #region Operators
        public ActionResult SaveOperator(Operators.NewOperator model)
        {
            CatalogsDataModel.OperatorModel cdm = new CatalogsDataModel.OperatorModel();
            AttemptResponse attempt = cdm.SaveOperator(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ReponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });

        }
        public ActionResult SearchOperator(Operators.SearchOperator model)
        {
            CatalogsDataModel.OperatorModel cdm = new CatalogsDataModel.OperatorModel();
            Operators.OperatorViewModel ssp = new Operators.OperatorViewModel();
            ssp.SearchOperatorResult = cdm.SearchOperator(model);
            return PartialView("",ssp);

        }
        public JsonResult DeleteOperator(Guid OperatorID)
        {
            CatalogsDataModel.OperatorModel cdm = new CatalogsDataModel.OperatorModel();
            AttemptResponse attempt = cdm.DeleteOperator(OperatorID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=Debugging.GetMessage(attempt.Exception),
                ExceptionMessage=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public JsonResult GetOperator(Guid OperatorID)
        {
            CatalogsDataModel.OperatorModel cdm = new CatalogsDataModel.OperatorModel();
            var response = cdm.GetOperator(OperatorID);
            return Json(response);
        }
        #endregion

        #region Tours_Qoutas
        public ActionResult SaveQouta(Tours.Tour_NewQouta model)
        {
            CatalogsDataModel.TourModel cdm = new CatalogsDataModel.TourModel();
            AttemptResponse attempt = cdm.SaveQouta(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public ActionResult SearchQouta(Tours.Tour_SearchQouta model)
        {
            CatalogsDataModel.TourModel cdm = new CatalogsDataModel.TourModel();
            Tours.QoutaViewModel ssp = new Tours.QoutaViewModel();
            ssp.SearchQoutaResult = cdm.SearchQouta(model);
            return PartialView("_",ssp);
        }
        public JsonResult DeleteQouta(int QoutaID)
        {
            CatalogsDataModel.TourModel cdm = new CatalogsDataModel.TourModel();
            AttemptResponse attempt = cdm.DeleteQouta(QoutaID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new 
            { 
                ResponseType=attempt.Type,
                ItemID=attempt.ObjectID,
                ResponseMessage=attempt.Message,
                ExceptionMessage=Debugging.GetMessage(attempt.Exception),
                InnerException=Debugging.GetInnerException(attempt.Exception)
            });
        }
        public JsonResult GetQouta(int QoutaID)
        {
            CatalogsDataModel.TourModel cdm = new CatalogsDataModel.TourModel();
            var response = cdm.GetQouta(QoutaID);
            return Json(response);
        }
        #endregion

        #region Hotel Pick Ups
        public JsonResult SearchHotelPickUps(HotelPickUps.SearchHotels model)
        {
            CatalogsDataModel.HotelPickUpsModel cdm = new CatalogsDataModel.HotelPickUpsModel();
            return Json(cdm.SearchHotelPickUps(model));
        }

        public JsonResult GetDependentFields()
        {
            return Json(CatalogsDataModel.HotelPickUpsModel.GetDependentFields(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePickUp(HotelPickUps.HotelPickUp model)
        {
            CatalogsDataModel.HotelPickUpsModel cdm = new CatalogsDataModel.HotelPickUpsModel();
            AttemptResponse attempt = cdm.SavePickUp(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br/>" + Debugging.GetErrorLocation(attempt.Exception);
            }
            //var request = HttpContext.Request;
            //var urlMethod = new UserSession().Url;

            //Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", attempt, urlMethod, request));

            return Json(new
            {
                ResponseType = attempt.Type,
                HotelPickUpID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        [HttpPost]

        public PictureDataModel.FineUploaderResult UploadPickUpPicture(PictureDataModel.FineUpload upload, int SpiHotelID)
        {
            return (CatalogsDataModel.HotelPickUpsModel.UploadPickUpPicture(upload, SpiHotelID));
        }

        #endregion
    }
}
