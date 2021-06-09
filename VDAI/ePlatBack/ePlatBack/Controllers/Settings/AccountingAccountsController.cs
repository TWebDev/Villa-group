using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers.Settings
{
    public class AccountingAccountsController : Controller
    {
        //
        // GET: /AccountingAccounts/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public PartialViewResult RenderAccountingAccountsInActivities()
        {
            var aam = new AccountingAccountsModel.AccountingAccountInfoModel
            {
                SearchAccountingAccountsModel = new AccountingAccountsModel.SearchAccountingAccountsModel()
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10950);
            return PartialView("_AccountingAccountsManagementCatalogPartial", aam);
        }
        //public PartialViewResult RenderAccountingAccountsInCatalogs()
        //{
        //    var aam = new AccountingAccountsModel.AccountingAccountInfoModel
        //    {
        //        SearchAccountingAccountsModel = new AccountingAccountsModel.SearchAccountingAccountsModel()
        //    };
        //    ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10638);
        //    return PartialView("_AccountingAccountsManagementCatalogPartial", aam);
        //}
        public ActionResult SearchAccountingAccounts(AccountingAccountsModel.SearchAccountingAccountsModel model)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            AccountingAccountsModel.SearchAccountingAccountsModel szm = new AccountingAccountsModel.SearchAccountingAccountsModel();
            szm.ListAccountingAccounts = cdm.SearchAccountingAccounts(model);
            szm.ListAccountingAccountsSummary = cdm.GetAccountinAccountsSummary(model);
            return PartialView("_AccountingAccountsSearchResultsCatalogPartial", szm);
        }
        public JsonResult SaveAccountingAccount(AccountingAccountsModel.AccountingAccountInfoModel model)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            AttemptResponse attempt = cdm.SaveAccountingAccount(model);
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
        public JsonResult GetAccountingAccount(int AccountingAccountInfo_AccountingAccountID)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            return Json(cdm.GetAccountingAccount(AccountingAccountInfo_AccountingAccountID));
        }
        public JsonResult DeleteAccountingAccount(int targetID)
        {
            CatalogsDataModel.AccountingAccounts cdm = new CatalogsDataModel.AccountingAccounts();
            AttemptResponse attempt = cdm.DeleteAccountingAccount(targetID);
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
    }
}
