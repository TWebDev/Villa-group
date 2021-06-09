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
    public class PlacesController : Controller
    {
        //
        // GET: /newPlaces/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess() == true)
            {
                PlaceViewModel pvm = new PlaceViewModel();
                ViewData.Model = new PlaceViewModel
                {
                    PlacesSearchModel = new PlacesSearchModel(),
                    PlaceInfoModel = new PlaceInfoModel(),
                    ZoneInfoModel = new ZoneInfoModel(),
                    ZonesSearchModel = new ZonesSearchModel(),
                    PlaceTypeInfoModel = new PlaceTypeInfoModel(),
                    PlaceTypesSearchModel = new PlaceTypesSearchModel(),
                    DestinationInfoModel = new DestinationInfoModel(),
                    DestinationsSearchModel = new DestinationsSearchModel(),
                    PlaceClasificationInfoModel = new PlaceClasificationInfoModel(),
                    PlaceClasificationsSearchModel = new PlaceClasificationsSearchModel(),
                    TransportationZoneInfoModel = new TransportationZoneInfoModel(),
                    TransportationZonesSearchModel = new TransportationZonesSearchModel(),
                };
                return View();
            }           
              return  RedirectToAction("Index", "Home");
        }

        //public ActionResult NewView()
        //{
        //    var Access = AdminDataModel.VerifyAccess();
        //    if (Access == false)
        //        return RedirectToAction("Index", "Home");
        //    else
        //        return View();
        //}

        public PartialViewResult RenderPlaces()
        {
            var pim = new PlaceInfoModel
            {
                PlaceDescriptionModel = new PlaceDescriptionModel(),
                PlacePlanTypeModel = new PlacePlanTypeModel(),
                RoomTypeInfoModel = new RoomTypeInfoModel(),
                //PictureInfoModel = new PictureInfoModel(),
            };
            return PartialView("_PlacesManagementPartial", pim);
        }

        public ActionResult RenderPictures()
        {
            var pim = new PictureInfoModel{};
            return View("_PicturesManagementPartial", pim);
        }

        //public PartialViewResult RenderPictures()
        //{
        //    var pim = new PictureInfoModel
        //    {

        //    };
        //    return PartialView("_PicturesManagementPartial", pim);
        //}

        public PartialViewResult RenderRoomTypes()
        {
            var rim = new RoomTypeInfoModel
            {
                RoomTypeDescriptionModel = new RoomTypeDescriptionModel(),
            };
            return PartialView("_RoomTypesPartial", rim);
        }

        public ActionResult SearchPlaces(PlacesSearchModel model)
        {
            PlaceViewModel pvm = new PlaceViewModel();
            PlaceDataModel pdm = new PlaceDataModel();
            pvm.SearchResults = pdm.SearchPlacesResults(model);

            return PartialView("_SearchPlacesResultsPartial", pvm);
        }

        public JsonResult SavePlace(PlaceInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SavePlace(model);
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

        public JsonResult SavePlaceDescription(PlaceDescriptionModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SavePlaceDescription(model);
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

        public JsonResult SavePlacePlanType(PlacePlanTypeModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SavePlacePlanType(model);
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

        public JsonResult SaveRoomType(RoomTypeInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SaveRoomType(model);
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

        public JsonResult SaveRoomTypeDescription(RoomTypeDescriptionModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SaveRoomTypeDescription(model);
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

        public JsonResult GetPlace(int id)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetPlace(id));
        }

        public JsonResult GetPlaceDescriptions(int id)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetPlaceDescriptions(id));
        }

        public JsonResult GetPlacePlanTypes(int id)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetPlacePlanTypes(id));
        }

        public JsonResult GetRoomTypes(int id)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetRoomTypes(id));
        }

        public JsonResult GetRoomTypeDescriptions(int roomTypeID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetRoomTypeDescriptions(roomTypeID));
        }

        public JsonResult GetPlaceDescription(int placeDescriptionID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetPlaceDescription(placeDescriptionID));
        }

        public JsonResult GetPlacePlanType(int placePlanTypeID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetPlacePlanType(placePlanTypeID));
        }

        public JsonResult GetRoomType(int roomTypeID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetRoomType(roomTypeID));
        }

        public JsonResult GetRoomTypeDescription(int roomTypeDescriptionID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetRoomTypeDescription(roomTypeDescriptionID));
        }

        public JsonResult GetDDLData(string path, int id)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetDDLData(path, id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchPlacesRelatedItems(string item, string path)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.SearchPlacesRelatedItems(item, path));
        }

        public JsonResult SaveZone(ZoneInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SaveZone(model);
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

        public JsonResult SavePlaceType(PlaceTypeInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SavePlaceType(model);
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

        public JsonResult SaveDestination(DestinationInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SaveDestination(model);
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

        public JsonResult SavePlaceClasification(PlaceClasificationInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SavePlaceClasification(model);
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

        public JsonResult SaveTransportationZone(TransportationZoneInfoModel model)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.SaveTransportationZone(model);
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

        public JsonResult DeletePlaceItem(int id, string path)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.DeletePlaceItem(id, path);
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

        public JsonResult GetPlaceItem(int id, string path)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            return Json(pdm.GetPlaceItem(id, path));
        }

        public JsonResult DeletePlace(int id)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.DeletePlace(id);
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

        public JsonResult DeletePlaceDescription(int placeDescriptionID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.DeletePlaceDescription(placeDescriptionID);
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

        public JsonResult DeletePlacePlanType(int placePlanTypeID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.DeletePlacePlanType(placePlanTypeID);
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

        public JsonResult DeleteRoomType(int roomTypeID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.DeleteRoomType(roomTypeID);
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

        public JsonResult DeleteRoomTypeDescription(int roomTypeDescriptionID)
        {
            PlaceDataModel pdm = new PlaceDataModel();
            AttemptResponse attempt = pdm.DeleteRoomTypeDescription(roomTypeDescriptionID);
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
