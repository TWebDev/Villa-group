using System;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using System.Threading;

namespace ePlatBack.Controllers.CMS
{
    public class PackagesController : Controller
    {
        //
        // GET: /Packages/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                PackageViewModel pvm = new PackageViewModel();
                ViewData.Model = new PackageViewModel
                {
                    PackagesSearchModel = new PackagesSearchModel(),
                    PackageInfoModel = new PackageInfoModel(),
                    //PackageDescriptionModel = new PackageDescriptionModel(),
                    //PackageSettingsModel = new PackageSettingsModel(),
                    //PriceInfoModel = new PriceInfoModel(),
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public PartialViewResult RenderPackages()
        {
            var pim = new PackageInfoModel
            {
                PackageDescriptionInfoModel = new PackageDescriptionInfoModel(),
                PackageSettingsInfoModel = new PackageSettingsInfoModel(),
                PriceInfoModel = new PriceInfoModel(),
                SeoItemInfoModel = new SeoItemInfoModel(),
                PictureInfoModel = new PictureInfoModel(),
            };
            return PartialView("_PackagesManagementPartial", pim);
        }

        public ActionResult SearchPackages(PackagesSearchModel model)
        {
            PackageDataModel pdm = new PackageDataModel();
            PackageViewModel pvm = new PackageViewModel();

            pvm.SearchResults = pdm.SearchPackages(model);

            return PartialView("_SearchPackagesResults", pvm);
        }

        public JsonResult SavePackage(PackageInfoModel model)
        {
            
            PackageDataModel pdm = new PackageDataModel();
            AttemptResponse attempt = pdm.SavePackage(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new {
            ResponseType = attempt.Type,
            ItemID = attempt.ObjectID,
            ResponseMessage = attempt.Message,
            ExceptionMessage = Debugging.GetMessage(attempt.Exception),
            InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        //[HttpPost, ValidateInput(false)]
        public JsonResult SavePackageDescription(PackageDescriptionInfoModel model)
        {
            PackageDataModel pdm = new PackageDataModel();
            AttemptResponse attempt = pdm.SavePackageDescription(model);
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

        public JsonResult SavePackageSettings(PackageSettingsInfoModel model)
        {
            PackageDataModel pdm = new PackageDataModel();
            AttemptResponse attempt = pdm.SavePackageSettings(model);
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

        //public JsonResult GetCatalogsPerTerminal(int terminalID)
        //{
        //    PackageDataModel pdm = new PackageDataModel();
        //    return Json(pdm.GetCatalogsPerTerminal(terminalID), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetRoomTypesPerPlace(int placeID)
        //{
        //    return Json(PackageDataModel.PackagesCatalogs.GetRoomTypesPerPlace(placeID));
        //}

        public JsonResult GetPackageInfo(int packageID)
        {
            PackageDataModel pdm = new PackageDataModel();
            return Json(pdm.GetPackageInfo(packageID));
        }

        public JsonResult GetPackageDescriptions(int packageID)
        {
            PackageDataModel pdm = new PackageDataModel();
            return Json(pdm.GetPackageDescriptions(packageID));
        }

        public JsonResult GetPackageDescription(int packageDescriptionID)
        {
            PackageDataModel pdm = new PackageDataModel();
            return Json(pdm.GetPackageDescription(packageDescriptionID));
        }

        public JsonResult GetPackageSettings(int packageID)
        {
            PackageDataModel pdm = new PackageDataModel();
            return Json(pdm.GetPackageSettings(packageID));
        }

        public JsonResult DeletePackage(int packageID)
        {
            PackageDataModel pdm = new PackageDataModel();
            AttemptResponse attempt = pdm.DeletePackage(packageID);
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
            //return Json(new { ok = pdm.DeletePackage(packageID) });
        }

        public JsonResult GetPackageSetting(int packageSettingID)
        {
            PackageDataModel pdm = new PackageDataModel();
            return Json(pdm.GetPackageSetting(packageSettingID));
        }

        public JsonResult DeletePackageSetting(int packageSettingID, int packageID)
        {
            PackageDataModel pdm = new PackageDataModel();
            AttemptResponse attempt = pdm.DeletePackageSetting(packageSettingID, packageID);
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

        public JsonResult DeletePackageDescription(int packageDescriptionID)
        {
            PackageDataModel pdm = new PackageDataModel();
            AttemptResponse attempt = pdm.DeletePackageDescription(packageDescriptionID);
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

        public JsonResult GetDDLData(string itemID, string itemType)
        {
            PackageDataModel pdm = new PackageDataModel();
            return Json(pdm.GetDDLData(itemID, itemType), JsonRequestBehavior.AllowGet);
            //return Json(PackageDataModel.PackagesCatalogs.FillDrpTermsBlock(itemID), JsonRequestBehavior.AllowGet);
        }

        /*
         * Price-Related Methods
         */

        //public JsonResult SearchPricesItems(string item, string path)
        //{
        //    PriceDataModel pdm = new PriceDataModel();
        //    return Json(pdm.SearchPricesClasification(item, path));
        //}

        //public JsonResult GetPriceItem(int id, string path)
        //{
        //    PriceDataModel pdm = new PriceDataModel();
        //    return Json(pdm.GetPriceItem(id, path));
        //}

        //public JsonResult SavePrice(PriceInfoModel model)
        //{
        //    PriceDataModel pdm = new PriceDataModel();
        //    return Json(new { ok = pdm.SavePrice(model) });
        //}

        //public JsonResult GetPrices(string itemType, int itemID)
        //{
        //    PriceDataModel pdm = new PriceDataModel();
        //    return Json(pdm.GetPrices(itemType, itemID));
        //}

    }
}
