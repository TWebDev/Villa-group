using System;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers.CMS
{
    public class PricesController : Controller
    {
        //
        // GET: /Prices/
        //Rendered in Activities







        public PartialViewResult RenderPricesInActivities()
        {
            var pim = new PriceInfoModel
               {
                   PriceClasificationsSearchModel = new PriceClasificationsSearchModel(),
                   PriceClasificationInfoModel = new PriceClasificationInfoModel(),
                   CurrenciesSearchModel = new CurrenciesSearchModel(),
                   CurrencyInfoModel = new CurrencyInfoModel(),
               };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10900);//fdsPrices
            return PartialView("_PricesManagementPartial", pim);
        }

        public PartialViewResult RenderPricesInPackages()
        {
            var pim = new PriceInfoModel
            {
                PriceClasificationsSearchModel = new PriceClasificationsSearchModel(),
                PriceClasificationInfoModel = new PriceClasificationInfoModel(),
                CurrenciesSearchModel = new CurrenciesSearchModel(),
                CurrencyInfoModel = new CurrencyInfoModel(),
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11047);//fdsPrices
            return PartialView("_PricesManagementPartial", pim);
        }

        public ActionResult RenderPricesWizard(int providerID, long serviceID)
        {
            PriceWizardModel model = new PriceWizardModel();
            //model.PriceWizard_DrpCurrencies = PriceDataModel.PricesCatalogs.FillDrpCurrencies(int.Parse(Request["providerID"]));
            //model.PriceWizard_Units = PriceDataModel.PricesCatalogs.FillDrpUnits(long.Parse(Request["serviceID"]));
            model.PriceWizard_DrpCurrencies = PriceDataModel.PricesCatalogs.FillDrpCurrencies(providerID);
            model.PriceWizard_Units = PriceDataModel.PricesCatalogs.FillDrpUnits(serviceID);
            
            return PartialView("_PricesManagementWizardPartial", model);
        }

        public PartialViewResult RenderPrices()
        {
            var pim = new PriceInfoModel
            {
                PriceClasificationsSearchModel = new PriceClasificationsSearchModel(),
                PriceClasificationInfoModel = new PriceClasificationInfoModel(),
                CurrenciesSearchModel = new CurrenciesSearchModel(),
                CurrencyInfoModel = new CurrencyInfoModel(),
            };
            //ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10900);//fdsPrices
            return PartialView("_PricesManagementPartial", pim);
        }

        public JsonResult FillDrpPrices(string itemType, int itemID)
        {
            return Json(PriceDataModel.PricesCatalogs.FillDrpPrices(itemType, itemID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillDrpPriceClasifications()
        {
            return Json(PriceDataModel.PricesCatalogs.FillDrpPriceClasifications(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillDrpCurrencies()
        {
            return Json(PriceDataModel.PricesCatalogs.FillDrpCurrencies(null), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchPriceClasifications(string Search_PriceClasifications)
        {
            PriceDataModel pdm = new PriceDataModel();
            return Json(pdm.SearchPricesClasification(Search_PriceClasifications));
        }

        public JsonResult SearchCurrencies(string Search_Currencies)
        {
            PriceDataModel pdm = new PriceDataModel();
            return Json(pdm.SearchCurrencies(Search_Currencies));
        }

        public JsonResult GetPriceClasificationPerID(int priceClasificationID)
        {
            PriceDataModel pdm = new PriceDataModel();
            return Json(pdm.GetPriceClasificationPerID(priceClasificationID));
        }

        public JsonResult GetCurrencyPerID(int currencyID)
        {
            PriceDataModel pdm = new PriceDataModel();
            return Json(pdm.GetCurrencyPerID(currencyID));
        }

        public JsonResult SavePriceClasification(PriceClasificationInfoModel model)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.SavePriceClasification(model);

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

        public JsonResult SaveCurrency(CurrencyInfoModel model)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.SaveCurrency(model);
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

        public JsonResult DeletePriceClasification(int priceClasificationID)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.DeletePriceClasification(priceClasificationID);
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

        public JsonResult DeleteCurrency(int currencyID)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.DeleteCurrency(currencyID);
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

        public JsonResult SavePriceWizard(PriceWizardModel model)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.SavePriceWizard(model);
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

        public JsonResult SavePrice(PriceInfoModel model)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.SavePrice(model);
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

        public JsonResult ClonePrice(int priceID, string price)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.ClonePrice(priceID, price);
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

        //public ActionResult GetPrices(string itemType, long itemID, string culture)
        public ActionResult GetPrices(string itemType, long itemID, string culture, int? pos = null)
        {
            PriceDataModel pdm = new PriceDataModel();
            PriceViewModel pvm = new PriceViewModel();
            //pvm.SearchResultsModel = pdm.GetPrices(itemType, itemID);
            pvm.PricesTableModel = pdm.GetPrices(itemType, itemID, culture, pos);
            return PartialView("_SearchPricesPartial", pvm);
        }

        public JsonResult GetPricePerID(int priceID)
        {
            PriceDataModel pdm = new PriceDataModel();
            return Json(pdm.GetPricePerID(priceID));
        }

        public JsonResult DeletePrice(int priceID)
        {
            PriceDataModel pdm = new PriceDataModel();
            AttemptResponse attempt = pdm.DeletePrice(priceID);
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

        public ActionResult ValidateDates(string PriceInfo_ToDate, string PriceInfo_FromDate)
        {
            if (DateTime.Compare(DateTime.Parse(PriceInfo_FromDate), DateTime.Parse(PriceInfo_ToDate)) > 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SavePriceGroup(string sysItemType, string itemID, string priceGroupID, string prices)
        //{
        //    PriceDataModel pdm = new PriceDataModel();
        //    AttemptResponse attempt = pdm.SavePriceGroup(sysItemType, itemID, priceGroupID, prices);
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

        public string GetGroupPrices(string sysItemType, string itemID)
        {
            PriceDataModel pdm = new PriceDataModel();
            return pdm.GetGroupPrices(sysItemType, itemID);
        }

        public string GetPricesPerGroup(string groupID)
        {
            PriceDataModel pdm = new PriceDataModel();
            return pdm.GetPricesPerGroup(groupID);
        }

        public JsonResult GetRules(long serviceID, long? terminalID, DateTime? date)
        {
            return Json(PriceDataModel.GetRules(serviceID, terminalID, date), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDDLData(string itemType, string itemID)
        {
            PriceDataModel pdm = new PriceDataModel();
            return Json(pdm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }
    }
}
