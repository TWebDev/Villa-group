using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Controllers.Settings
{
    public class ProvidersController : Controller
    {
        //
        // GET: /Providers/

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult RenderProvidersInActivities()
        {
            var sim = new ProvidersModel.ProviderInfoModel
            {
                SearchProvidersModel = new ProvidersModel.SearchProvidersModel()
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10952);//fdsProviders
            return PartialView("_ProvidersManagementCatalogPartial", sim);
        }

        public PartialViewResult RenderProvidersInCatalogs()
        {
            var sim = new ProvidersModel.ProviderInfoModel
            {
                SearchProvidersModel = new ProvidersModel.SearchProvidersModel()
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10648);//fdsProviders
            return PartialView("_ProvidersManagementCatalogPartial", sim);
        }

        public ActionResult SearchProviders(ProvidersModel.SearchProvidersModel model)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            ProvidersModel.SearchProvidersModel scm = new ProvidersModel.SearchProvidersModel();
            scm.ListProviders = cdm.SearchProviders(model);
            return PartialView("_ProvidersSearchResultsCatalogPartial", scm);
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

        public JsonResult GetProvider(int ProviderInfo_ProviderID)
        {
            CatalogsDataModel.Providers cdm = new CatalogsDataModel.Providers();
            return Json(cdm.GetProvider(ProviderInfo_ProviderID));
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
