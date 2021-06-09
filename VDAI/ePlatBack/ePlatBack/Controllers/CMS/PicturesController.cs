using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using Newtonsoft.Json.Linq;

namespace ePlatBack.Controllers.CMS
{
    public class PicturesController : Controller
    {
        //
        // GET: /Pictures/

        public PartialViewResult RenderPicturesInActivities()
        {
            var pim = new PictureInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(10902);//fdsPictures
            return PartialView("_PicturesManagementPartial", pim);
        }

        public PartialViewResult RenderPicturesInPlaces()
        {
            var pim = new PictureInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11020);//fdsPictures
            return PartialView("_PicturesManagementPartial", pim);
        }

        public PartialViewResult RenderPicturesInRoomTypes()
        {
            var pim = new PictureInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11027);//fdsPictures
            return PartialView("_PicturesManagementPartial", pim);
        }

        public PartialViewResult RenderPicturesInPackages()
        {
            var pim = new PictureInfoModel
            {
            };
            ViewData["Privileges"] = AdminDataModel.GetViewPrivileges(11052);//fdsPictures
            return PartialView("_PicturesManagementPartial", pim);
        }

        public PartialViewResult RenderPictures()
        {
            var pim = new PictureInfoModel
            {

            };
            return PartialView("_PicturesManagementPartial", pim);
        }

        public JsonResult GetGalleryName(string itemType, string itemID)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetGalleryName(itemType, itemID));
        }

        public JsonResult GetTree(string sysItemType, string itemID, string currentProvider, string destination)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetTree(sysItemType, itemID, currentProvider, destination), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFiles(string directory)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetFiles(directory));
        }

        public JsonResult GetItemNames(string parameters, string itemType)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetItemNames(parameters, itemType));
        }

        public PartialViewResult RenderPictureDescription()
        {
            var model = new PictureDescriptionInfoModel { };
            return PartialView("_PictureDescriptionsPartial", model);
        }

        public JsonResult GetPictureDescriptions(int pictureID)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetPictureDescriptions(pictureID));
        }

        public JsonResult GetPictureDescription(int pictureDescriptionID)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetPictureDescription(pictureDescriptionID));
        }

        public JsonResult SavePictureDescription(PictureDescriptionInfoModel model)
        {
            PictureDataModel pdm = new PictureDataModel();
            AttemptResponse attempt = pdm.SavePictureDescription(model);
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

        public JsonResult DeletePictureDescription(int pictureDescriptionID)
        {
            PictureDataModel pdm = new PictureDataModel();
            AttemptResponse attempt = pdm.DeletePictureDescription(pictureDescriptionID);
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

        public JsonResult GetImagesPerItemType(string sysItemType, string itemID)
        {
            PictureDataModel pdm = new PictureDataModel();
            return Json(pdm.GetImagesPerItemType(sysItemType, itemID));
        }

        public JsonResult RemovePicture(int pictureID)
        {
            PictureDataModel pdm = new PictureDataModel();
            AttemptResponse attempt = pdm.RemovePicture(pictureID);
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

        public JsonResult SavePicturesOnGallery(string sysItemType, int itemID, string[] picturesArray)
        {
            PictureDataModel pdm = new PictureDataModel();
            AttemptResponse attempt = pdm.SavePicturesOnGallery(sysItemType, itemID, picturesArray);
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

        public PictureDataModel.FineUploaderResult UploadPicture(PictureDataModel.FineUpload upload, string path, string sysItemType)
        {
            PictureDataModel pdm = new PictureDataModel();
            return (pdm.UploadPicture(upload, path, sysItemType));
        }

        //public JsonResult UploadPicture()
        //{
        //    PictureDataModel pdm = new PictureDataModel();
        //    AttemptResponse attempt = pdm.UploadPicture();

        //    string errorLocation = "";
        //    if (attempt.Exception != null)
        //    {
        //        
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        GeneralInformation_LeadID = attempt.ObjectID,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
        //        InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
        //    });
        //}

        
    }
}
