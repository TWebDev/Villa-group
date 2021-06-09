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
    public class PointsOfSaleController : Controller
    {
        //
        // GET: /PointsOfSale/

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult RenderPointsOfSaleInActivities()
        {
            var psm = new PointsOfSaleModel.PointsOfSaleInfoModel
            {
                SearchPointsOfSaleModel = new PointsOfSaleModel.SearchPointsOfSaleModel()
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10951);
            return PartialView("_PointsOfSaleManagementCatalogPartial", psm);
        }
        //public PartialViewResult RenderPointsOfSaleInCatalogs()
        //{
        //    var psm = new PointsOfSaleModel.PointsOfSaleInfoModel
        //    {
        //        SearchPointsOfSaleModel = new PointsOfSaleModel.SearchPointsOfSaleModel()
        //    };
        //    ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10646);
        //    return PartialView("_PointsOfSaleManagementCatalogPartial", psm);
        //}
        public ActionResult SearchPointsOfSale(PointsOfSaleModel.SearchPointsOfSaleModel model)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            PointsOfSaleModel.SearchPointsOfSaleModel spsm = new PointsOfSaleModel.SearchPointsOfSaleModel();
            spsm.ListPointsOfSale = cdm.SearchPointsOfSale(model);
            return PartialView("_PointsOfSaleSearchResultsCatalogPartial", spsm);
        }
        public JsonResult SavePointOfSale(PointsOfSaleModel.PointsOfSaleInfoModel model)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            AttemptResponse attempt = cdm.SavePointOfSale(model);
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
        public JsonResult GetPointOfSale(int PointsOfSaleInfo_PointOfSaleID)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            return Json(cdm.GetPointOfSale(PointsOfSaleInfo_PointOfSaleID));
        }
        public JsonResult DeletePointOfSale(int targetID)
        {
            CatalogsDataModel.PointsOfSale cdm = new CatalogsDataModel.PointsOfSale();
            AttemptResponse attempt = cdm.DeletePointOfSale(targetID);
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
