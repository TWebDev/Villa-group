using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Controllers.CMS
{
    public class SeoItemsController : Controller
    {
        //
        // GET: /SeoItems/

        public PartialViewResult RenderSeoItemsInActivities()
        {
            var sim = new SeoItemInfoModel
            {

            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10901);//activities/fdsSeoItems
            return PartialView("_SeoItemsManagementPartial", sim);
        }

        public PartialViewResult RenderSeoItemsInDestinations()
        {
            var sim = new SeoItemInfoModel
            {

            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10642);//catalogs/fdsDestinations
            return PartialView("_SeoItemsManagementPartial", sim);
        }

        public PartialViewResult RenderSeoItemsInCategories()
        {
            var sim = new SeoItemInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11041);
            return PartialView("_SeoItemsManagementPartial", sim);
        }

        public PartialViewResult RenderSeoItemsInPackages()
        {
            var sim = new SeoItemInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11048);
            return PartialView("_SeoItemsManagementPartial", sim);
        }

        public PartialViewResult RenderSeoItemsInPages()
        {
            var sim = new SeoItemInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11062);
            return PartialView("_SeoItemsManagementPartial", sim);
        }

        public PartialViewResult RenderSeoItems()
        {
            var sim = new SeoItemInfoModel
            {

            };
            return PartialView("_SeoItemsManagementPartial", sim);
        }

        public ActionResult GetSeoItems(string itemType, string itemID)
        {
            SeoItemDataModel sdm = new SeoItemDataModel();
            SeoItemViewModel svm = new SeoItemViewModel();
            svm.SearchResultsModel = sdm.GetSeoItems(itemType, itemID);
            return PartialView("_SearchSeoItemsPartial", svm);
        }

        public JsonResult GetSeoItemPerID(int seoItemID)
        {
            SeoItemDataModel sdm = new SeoItemDataModel();
            return Json(sdm.GetSeoItemPerID(seoItemID));
        }

        public JsonResult SaveSeoItem(SeoItemInfoModel model)
        {
            SeoItemDataModel sdm = new SeoItemDataModel();
            AttemptResponse attempt = sdm.SaveSeoItem(model);
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

        public JsonResult DeleteSeoItem(int seoItemID)
        {
            SeoItemDataModel sdm = new SeoItemDataModel();
            AttemptResponse attempt = sdm.DeleteSeoItem(seoItemID);
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

        //public JsonResult GetDDLData(string path, string itemType, int itemID)
        public JsonResult GetDDLData(string itemType, string itemID, string path)
        {
            SeoItemDataModel sdm = new SeoItemDataModel();
            return Json(sdm.GetDDLData(itemType, itemID, path), JsonRequestBehavior.AllowGet);
        }
    }
}
