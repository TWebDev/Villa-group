using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using System.Web.Script.Serialization;
using ePlatBack.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ePlatBack.Controllers.CMS
{
    public class ActivitiesController : Controller
    {
        //
        // GET: /Activities/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                ActivityViewModel avm = new ActivityViewModel();
                ViewData.Model = new ActivityViewModel
                {
                    ActivitiesSearchModel = new ActivitiesSearchModel(),
                    ActivityInfoModel = new ActivityInfoModel(),
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public PartialViewResult RenderActivities()
        {
            var aim = new ActivityInfoModel
            {
                ActivityCategoryInfoModel = new ActivityCategoryInfoModel(),
                ActivityDescriptionInfoModel = new ActivityDescriptionInfoModel(),
                ActivityScheduleInfoModel = new ActivityScheduleInfoModel(),
                ActivityMeetingPointInfoModel = new ActivityMeetingPointInfoModel(),
                ActivityAccountingAccountInfoModel = new ActivityAccountingAccountInfoModel(),
                //ActivityProvidersInfoModel = new ActivityProvidersInfoModel(),
                SearchProvidersModel = new ProvidersModel.SearchProvidersModel(),
                ProviderInfoModel = new ProvidersModel.ProviderInfoModel(),
                PriceInfoModel = new PriceInfoModel(),
                PictureInfoModel = new PictureInfoModel(),
                PrevActivitySearchModel = new PrevActivitySearchModel(),
                ActivityImportInfoModel = new ActivityImportInfoModel(),
                PriceTypeRulesInfoModel = new PriceTypeRulesInfoModel(),
                PricesEditorModel = new PricesEditorViewModel.ParamsPriceEditor(),
                StockInfoModel = new StockInfoModel()
            };
            ViewData["Privileges"] = new ActivityViewModel().Privileges;
            return PartialView("_ActivitiesManagementPartial", aim);
        }

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID =  null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
            return result;
        }

        public async Task<ActionResult> SearchActivities(ActivitiesSearchModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            ActivityViewModel avm = new ActivityViewModel();
            avm.SearchResults = adm.SearchActivities(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_SearchActivitiesResults", avm);
        }

        public ActionResult SearchProviders(ProvidersModel.SearchProvidersModel model)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            ProvidersModel.SearchProvidersModel scm = new ProvidersModel.SearchProvidersModel();
            scm.ListProviders = cdm.SearchProviders(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            /*new Thread(() =>
            {
                try
                {//logs
                    AdminDataModel.CreateTableUserLogActivity(model, "Search", (string)null, urlMethod, request);
                }
                catch (Exception ex)
                {
                    AdminDataModel.UserLogsActivitySendEx(ex, urlMethod, request);
                }
            }).Start();*/
            return PartialView("_ProvidersSearchResultsCatalogPartial", scm);
        }

        public async Task<JsonResult> GetActivityInfo(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = adm.GetActivityInfo(activityID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        public JsonResult GetActivityDescriptions(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = adm.GetActivityDescriptions(activityID);

            return Json(response);
        }

        public JsonResult GetActivitySchedules(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            return Json(adm.GetActivitySchedules(activityID));
        }

        public JsonResult GetActivityMeetingPoints(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            return Json(adm.GetActivityMeetingPoints(activityID));
        }

        public PartialViewResult GetActivityAccountingAccounts(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var model = new ActivityInfoModel();
            var response = adm.GetActivityAccountingAccounts(activityID);
            model.ListAccountingAccountsPerActivity = response;

            return PartialView("_AccountingAccountsPerActivityResultsPartial", model);
        }

        public ActionResult GetAccountingAccounts()
        {
            ActivityDataModel adm = new ActivityDataModel();
            ActivityViewModel avm = new ActivityViewModel();
            avm.AccountingAccountsResults = adm.GetAccountingAccounts();
            
            return PartialView("_AccountingAccountsResultsPartial", avm);
        }

        public ActionResult GetPriceTypeRules(long serviceID, long terminalID, DateTime? date)
        {
            PriceDataModel pdm = new PriceDataModel();
            ActivityViewModel avm = new ActivityViewModel();
            var response = PriceDataModel.GetRules(serviceID, terminalID, date);
            avm.PriceTypeRulesResults = response;
            var responseB = PriceDataModel.GetFutureRules(serviceID, terminalID);
            avm.NewPriceTypeRulesResults = responseB;

            return PartialView("_PriceTypeRulesResultsPartial", avm);
        }

        public JsonResult GetPriceTypeRule(long ruleID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getPrice = adm.GetPriceTypeRule(ruleID);
            
            return Json(getPrice);
        }

        public ActionResult GetPointsOfSale()
        {
            ActivityDataModel adm = new ActivityDataModel();
            ActivityViewModel avm = new ActivityViewModel();
            avm.PointsOfSaleResults = adm.GetPointsOfSale();
            
            return PartialView("_PointsOfSaleResultsPartial", avm);
        }
        
        public JsonResult GetActivityDescription(int activityDescriptionID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var response = adm.GetActivityDescription(activityDescriptionID);

            return Json(response);
        }

        public JsonResult GetActivitySchedule(int activityScheduleID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getActivity = adm.GetActivitySchedule(activityScheduleID);
            
            return Json(getActivity);
        }

        public JsonResult GetActivityMeetingPoint(int activityMeetingPointID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getActivity = adm.GetActivityMeetingPoint(activityMeetingPointID);
            
            return Json(getActivity);
        }

        public JsonResult GetActivityAccountingAccount(int accountingAccountID, long serviceID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getActivity = adm.GetActivityAccountingAccount(accountingAccountID, serviceID);
            
            return Json(getActivity);
        }

        public JsonResult GetAccountingAccount(int accountingAccountID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getAccount = adm.GetAccountingAccount(accountingAccountID);
            
            return Json(getAccount);
        }

        public JsonResult GetPointOfSale(int pointOfSaleID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getPointOfSale = adm.GetPointOfSale(pointOfSaleID);
            
            return Json(getPointOfSale);
        }

        public JsonResult GetProvider(int ProviderInfo_ProviderID)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            var getProvider = cdm.GetProvider(ProviderInfo_ProviderID);
            
            return Json(getProvider);
        }
        
        public async Task<JsonResult> RestoreActivity(long activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.RestoreActivity(activityID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(activityID, "RestoreActivity", attempt, urlMethod, request, terminalID));
            
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //Activitylogs      
        public async Task<JsonResult> SaveActivity(ActivityInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveActivity(model);
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

        public async Task<JsonResult> SaveCategoriesToActivity(ActivityCategoryInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveCategoriesToActivity(model);
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

        public async Task<JsonResult> SaveActivityDescription(ActivityDescriptionInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveActivityDescription(model);
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

        public async Task<JsonResult> SaveActivitySchedule(ActivityScheduleInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveActivitySchedule(model);
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

        public async Task<JsonResult> SaveMeetingPoint(ActivityMeetingPointInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveMeetingPoint(model);
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

        public async Task<JsonResult> SaveActivityAccountingAccount(ActivityAccountingAccountInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveActivityAccountingAccount(model);
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

        public async Task<JsonResult> SaveAccountingAccount(ActivityAccountingAccountsModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveAccountingAccount(model);
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

        public async Task<JsonResult> SavePriceTypeRule(PriceTypeRulesInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SavePriceTypeRule(model);
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

        public ActionResult SearchStockTransactions(StockInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            ActivityViewModel avm = new ActivityViewModel();
            avm.StockTransactionsResults = adm.SearchStockTransactions(model);
            
            return PartialView("_StockTransactionsResultsPartial", avm);
        }

        public async Task<JsonResult> SaveStockTransaction(StockInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SaveStockTransaction(model);
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

        public JsonResult GetStockBalance(long serviceID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getStock = adm.GetStockBalance(serviceID);
            
            return Json(getStock, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateStock(StockInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.UpdateStock(model);
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

            Task.Run(() => CreateTableUserLogActivityAsync(model, "UpdateStock", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public ActionResult RenderPriceTypeRuleClosing(int priceTypeRule)
        {
            PriceTypeRulesInfoModel model = new PriceTypeRulesInfoModel();
            model.PriceTypeRulesInfo_PriceTypeRuleID = priceTypeRule;
            
            return PartialView("_PriceTypeRuleClosingPartial", model);
        }

        public async Task<JsonResult> ClosePriceTypeRule(int priceTypeRuleID, DateTime? toDate)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.ClosePriceTypeRule(priceTypeRuleID, toDate);
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

            Task.Run(() => CreateTableUserLogActivityAsync(priceTypeRuleID, "", attempt, urlMethod, request, terminalID));            
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SavePointOfSale(ActivityPointsOfSaleModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.SavePointOfSale(model);
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

        public JsonResult SaveProvider(ProvidersModel.ProviderInfoModel model)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            AttemptResponse attempt = cdm.SaveProvider(model, this.ControllerContext);
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
        
        public async Task<JsonResult> DeleteActivity(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeleteActivity(activityID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(activityID, "Delete", attempt, urlMethod, request, terminalID));
            
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteActivityDescription(int activityDescriptionID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeleteActivityDescription(activityDescriptionID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(activityDescriptionID, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteActivitySchedule(int activityScheduleID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeleteActivitySchedule(activityScheduleID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(activityScheduleID, "Delete", attempt, urlMethod, request, terminalID));
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteActivityMeetingPoint(int activityMeetingPointID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeleteActivityMeetingPoint(activityMeetingPointID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(activityMeetingPointID, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteActivityAccountingAccount(int accountingAccountID, long serviceID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeleteActivityAccountingAccount(new int[] { accountingAccountID }, serviceID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(accountingAccountID, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteActivityAccountingAccounts(string accountingAccounts, long serviceID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            int[] _accounts = new JavaScriptSerializer().Deserialize(accountingAccounts, typeof(int[])) as int[];
            AttemptResponse attempt = adm.DeleteActivityAccountingAccount(_accounts, serviceID);
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

            Task.Run(() => CreateTableUserLogActivityAsync(accountingAccounts, "Delete", attempt, urlMethod, request, terminalID));

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public async Task<JsonResult> DeleteAccountingAccount(int targetID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeleteAccountingAccount(targetID);
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

        public JsonResult DeletePointOfSale(int targetID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeletePointOfSale(targetID);
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

        public JsonResult DeleteProvider(int targetID)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            AttemptResponse attempt = cdm.DeleteProvider(targetID);
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
        
        public async Task<JsonResult> DeletePriceTypeRule(int targetID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.DeletePriceTypeRule(targetID);
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

        public JsonResult GetDDLData(string itemType, string itemID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            return Json(adm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableDates(long service, int month, int year, long? terminalid)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var getAvailable = adm.GetAvailableDates(service, month, year, terminalid);
            
            return Json(new { DatesString = getAvailable }, JsonRequestBehavior.AllowGet);
        }
        
        //--
        public async Task<ActionResult> PrevSearchActivities(PrevActivitySearchModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            ActivityViewModel avm = new ActivityViewModel();
            avm.SearchImportResults = adm.PrevSearchActivities(model);
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(model, "Search", (string)null, urlMethod, request, terminalID));

            return PartialView("_PrevSearchActivitiesResults", avm);
        }

        public async Task<JsonResult> GetPrevActivitySettings(int activityID)
        {
            ActivityDataModel adm = new ActivityDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var response = adm.GetPrevActivitySettings(activityID);

            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            Task.Run(() => CreateTableUserLogActivityAsync(response, "Get", (string)null, urlMethod, request, terminalID));

            return Json(response);
        }

        ///evento temporal para la importacion de precios
        //public JsonResult ImportPrevActivityPrices()
        //{
        //    ActivityDataModel adm = new ActivityDataModel();
        //    AttemptResponse attempt = adm.ImportPrevActivityPrices();
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

        public async Task<JsonResult> ImportPrevActivitySettings(ActivityImportInfoModel model)
        {
            ActivityDataModel adm = new ActivityDataModel();
            AttemptResponse attempt = adm.ImportPrevActivitySettings(model);
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

        //
        // GET: /cms/Activities/GenerateFriendlyURLs
        public JsonResult GenerateFriendlyURLs(string id)
        {
            ActivityDataModel adm = new ePlatBack.Models.DataModels.ActivityDataModel();
            return Json(new
            {
                Success = true,
                UrlGenerated = adm.GenerateFriendlyURLs(id)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateFriendlyURL(long id)
        {
            return Json(new
            {
                Success = ActivityDataModel.GenerateFriendlyURL(id),
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
