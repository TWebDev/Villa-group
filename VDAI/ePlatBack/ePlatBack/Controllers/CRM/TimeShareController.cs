using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;



namespace ePlatBack.Controllers.CRM
{
    public class TimeShareController : Controller
    {
        //
        // GET: /TimeShare/
        [Authorize]
        public ActionResult Index()
        {
            ViewData.Model = new TimeShareViewModel
            {
                AccountancyModel_OutcomeSearchModel = new OutcomeSearchModel(),
                AccountancyModel_OutcomeInfoModel = new OutcomeInfoModel(),
                AccountancyModel_IncomeSearchModel = new IncomeSearchModel(),
                AccountancyModel_IncomeInfoModel = new IncomeInfoModel()
            };
            
            return View();
        }
        [Authorize]
        public PartialViewResult RenderTimeShareOperations()
        {
            var model = new TimeShareViewModel
            {
                AccountancyModel_OutcomeSearchModel = new OutcomeSearchModel(),
                AccountancyModel_OutcomeInfoModel = new OutcomeInfoModel(),
                AccountancyModel_IncomeSearchModel = new IncomeSearchModel(),
                AccountancyModel_IncomeInfoModel = new IncomeInfoModel()
            };
            ViewData["Privileges"] = new TimeShareViewModel().Privileges;
            return PartialView("_AccountancyManagementPartial", model);
        }
        [Authorize]
        public PartialViewResult RenderTimeShareOperationsFromMC()
        {
            var model = new TimeShareViewModel
            {
                AccountancyModel_OutcomeSearchModel = new OutcomeSearchModel(),
                AccountancyModel_OutcomeInfoModel = new OutcomeInfoModel(),
                AccountancyModel_IncomeSearchModel = new IncomeSearchModel(),
                AccountancyModel_IncomeInfoModel = new IncomeInfoModel()
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10743);
            return PartialView("_AccountancyManagementPartial", model);
        }

        public ActionResult SearchEgresses(OutcomeSearchModel model)
        {
            TimeShareDataModel mdm = new TimeShareDataModel();
            //AccountancyModel am = new AccountancyModel();
            TimeShareViewModel tvm = new TimeShareViewModel();
            tvm.AccountancyModel_SearchResults = mdm.SearchEgresses(model);
            return PartialView("_EgressesSearchResultsPartial", tvm);
        }

        public JsonResult GetEgressInfo(long OutcomeInfo_EgressID)
        {
            TimeShareDataModel mdm = new TimeShareDataModel();
            return Json(mdm.GetEgressInfo(OutcomeInfo_EgressID));
        }

        public JsonResult SaveEgress(OutcomeInfoModel model)
        {
            TimeShareDataModel mdm = new TimeShareDataModel();
            AttemptResponse attempt = mdm.SaveEgress(model);
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

        //public JsonResult DeleteEgress(long targetID)
        //{
        //    TimeShareDataModel mdm = new TimeShareDataModel();
        //    AttemptResponse attempt = mdm.DeleteEgress(targetID);
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
        //        InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
        //    });
        //}

        public ActionResult SearchIncomes(IncomeSearchModel model)
        {
            TimeShareDataModel mdm = new TimeShareDataModel();
            TimeShareViewModel am = new TimeShareViewModel();
            am.AccountancyModel_SearchIncomeResults = mdm.SearchIncomes(model);
            return PartialView("_IncomesSearchResultsPartial", am);
        }

        public JsonResult GetIncomeInfo(long IncomeInfo_IncomeID)
        {
            TimeShareDataModel mdm = new TimeShareDataModel();
            return Json(mdm.GetIncomeInfo(IncomeInfo_IncomeID));
        }

        public JsonResult SaveIncome(IncomeInfoModel model)
        {
            TimeShareDataModel mdm = new TimeShareDataModel();
            AttemptResponse attempt = mdm.SaveIncome(model);
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

        //public JsonResult DeleteIncome(long targetID)
        //{
        //    TimeShareDataModel mdm = new TimeShareDataModel();
        //    AttemptResponse attempt = mdm.DeleteIncome(targetID);
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
        //        InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
        //    });
        //}

        public JsonResult GetDependantEgressConcepts()
        {
            return Json(TimeShareDataModel.GetDependantEgressConcepts(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConceptAllowsBudget(long egressConceptID)
        {
            return Json(TimeShareDataModel.ConceptAllowsBudget(egressConceptID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFundsBalance()
        {
            return Json(TimeShareDataModel.FundsBalance(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetVarApp(int fundID, decimal amount)
        {
            TimeShareDataModel tdm = new TimeShareDataModel();
            AttemptResponse attempt = tdm.ResetVarApp(fundID, amount);

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

        public JsonResult GetDDLData(string itemType, string itemID)
        {
            TimeShareDataModel tdm = new TimeShareDataModel();
            return Json(tdm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }
    }
}
