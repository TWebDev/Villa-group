using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.DataModels;
using System.Web.Mvc;
using ePlatBack.Models;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;

namespace ePlatBack.Controllers.management
{
    public class managementController : Controller
    {
        ContentManagement content = new ContentManagement();
        public ActionResult hotels()
        {           
            return View("~/Views/Senses/ContentManagement/Hotels.cshtml");
        }
        public ActionResult articles()
        {


            return View("~/Views/Senses/ContentManagement/Articles.cshtml");
        }
        public ActionResult Restaurants()
        {

            return View("~/Views/Senses/ContentManagement/Restaurants.cshtml");
        }
        public ActionResult Events()
        {
            return View("~/Views/Senses/ContentManagement/Events.cshtml");
        }
        public ActionResult Spas()
        {
            return View("~/Views/Senses/ContentManagement/Spas.cshtml");
        }
        public ActionResult ArtGalleries()
        {

            return View("~/Views/Senses/ContentManagement/ArtGalleries.cshtml");
        }
        public ActionResult Reviews()
        {

            return View("~/Views/Senses/ContentManagement/Reviews.cshtml");
        }
        public ActionResult Surveys()
        {
            return View("~/Views/Senses/Surveys/Surveys.cshtml");
        }
        public ActionResult Providers()
        {
            return View("~/Views/Senses/ContentManagement/Providers.cshtml");
        }
        [System.Web.Http.HttpPost]
        public JsonResult getPlaces(int type)
        {
            var hotels = content.getPlaces(type);
            return Json(hotels, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getDataPlaces()
        {
            var data = content.getDataPlaces();


            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult addPlace(string data)
        {
            var dato = content.AddPlace(data);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult getDataPlaceSelected(Int64 id, int sys)
        {
            var dato = content.getDataPlaceSelected(id, sys);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult updatePlace(string data)
        {
            var dato = content.updatePlace(data);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult verifyPlace(string name, int type)
        {
            var dato = content.verifyPlace(name, type);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult joinPlace(Int64 id, int sys)
        {
            var dato = content.joinPlace(id, sys);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult unlinkPlace(Int64 id, int sys)
        {
            var dato = content.unlinkPlace(id, sys);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPlacesOrientationsUpdated()
        {
            var dato = content.getPlacesOrientationsUpdated();


            return Json(dato, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult addFeature(int id, string feature)
        {
            var dato = content.addFeature(id, feature);


            return Json(dato, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]

        public JsonResult uploadImage()
        {

            if (Request.Files.Count > 0)
            {

                ePlatEntities db = new ePlatEntities();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    tblPictures picture = new tblPictures();
                    var file = Request.Files[i];

                    
                    Random rand = new Random();
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    var fileName = Convert.ToString(rand.Next() + "senses" + i + ext);
                    var path = Path.Combine(Server.MapPath("~/Images/senses/"), fileName);
                    picture.path = path;
                    picture.format = ext;
                    picture.file_ = fileName;
                    db.tblPictures.AddObject(picture);
                    db.SaveChanges();
                    var pictureid = picture.pictureID;
                    tblPictures_SysItemTypes sys = new tblPictures_SysItemTypes();
                    sys.pictureID = pictureid;
                    sys.sysItemTypeID = Convert.ToInt64(Request.Params.Get("sys")); ;
                    sys.terminalID = 62;
                    sys.itemID = Convert.ToInt64(Request.Params.Get("id"));
                    db.tblPictures_SysItemTypes.AddObject(sys);
                    file.SaveAs(path);
                    db.SaveChanges();

                }

                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            return Json("not " + Request.Params.Get("id"), JsonRequestBehavior.AllowGet);

        }
        public JsonResult updatePLaceID(Int64 id, int sys)
        {
            ePlatEntities db = new ePlatEntities();
            var updates = (from item in db.tblPictures_SysItemTypes
                           where item.terminalID == 62 && item.itemID == 999999999
                           select item
                           ).ToList();

            foreach (var item in updates)
            {
                item.itemID = id;
                item.main = false;
                item.sysItemTypeID = sys;
                db.SaveChanges();
            }


            return Json("ok", JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult getImagesPlace(Int64 id, int sys)
        {
            ePlatEntities db = new ePlatEntities();
            var images = (from img in db.tblPictures_SysItemTypes
                          join i in db.tblPictures on img.pictureID equals i.pictureID
                          where img.itemID == id && img.sysItemTypeID == sys
                          select new
                          {
                              id = i.pictureID,
                              main = img.main,
                              idsys = img.picture_SysItemTypeID,
                              path = i.path,
                              name = i.file_,
                              logo = img.logo
                          }
                          ).ToList();

            return Json(images, JsonRequestBehavior.AllowGet);

        }
        public JsonResult deleteImage(Int64 id, Int64 idsys)
        {
            var response = content.deleteImage(id, idsys);


            return Json("ok", JsonRequestBehavior.AllowGet);

        }

        public JsonResult updateMain(Int64 id, Int64 idsys, int sys)
        {
            var response = content.updateMain(id, idsys, sys);


            return Json("ok", JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult addArticle(string data)
        {
            var response = content.addArticle(data);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getArticles()
        {
            var response = content.getArticles();


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult getItemsOrientationsUpdated(int itemid)
        {
            var response = content.getItemsOrientationsUpdated(itemid);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult getItemSelected(Int64 id, int sys)
        {
            var response = content.getItemSelected(id, sys);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult updateArticle(string data)
        {
            var response = content.updateArticle(data);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult deleteArticle(Int64 pageid, DateTime date, int sys)
        {
            var response = content.deleteArticle(pageid, date, sys);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult searchArticles(string data)
        {
            var response = content.searchArticles(data);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult AddRoom(Int64 placeid, int sys, string room, DateTime date, int price, string description)
        {
            var response = content.AddRoom(placeid, sys, room, date, price, description);


            return Json(response, JsonRequestBehavior.AllowGet);

        }
        [System.Web.Http.HttpPost]
        public JsonResult EditRoom(Int64 placeid, int sys, string room, DateTime date, int price, string description)
        {
            var response = content.EditRoom(placeid, sys, room, date, price, description);


            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetRooms(Int64 id)
        {
            var response = content.GetRooms(id);

            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public JsonResult DeleteRoom(Int64 id, int sysid, Int64 picid)
        {
            var response = content.DeleteRoom(id, sysid, picid);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveSignature(Guid leadID, string signature, int type)
        {
            var response = content.SaveSignature(leadID, signature, type);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetOrientations()
        {
            var response = content.GetOrientations();

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllPlaces()
        {
            var response = content.GetAllPlaces();

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveEvent(string data)
        {
            var response = content.SaveEvent(data);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UploadImageEvent()
        {

            if (Request.Files.Count > 0)
            {

                ePlatEntities db = new ePlatEntities();

                var id = Convert.ToInt64(Request.Params.Get("id"));
                var evento = (from eve in db.tblCalendarEvents
                              where eve.EventId == id
                              select eve
                              ).FirstOrDefault();


                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    Random rand = new Random();
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    var fileName = Convert.ToString(rand.Next() + ext);
                    var path = Path.Combine(Server.MapPath("~/Images/senses/"), fileName);
                    evento.Picture = fileName;
                    db.SaveChanges();
                    file.SaveAs(path);
                    return Json(fileName, JsonRequestBehavior.AllowGet);
                }


            }
            return Json("not", JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEvents()
        {
            var response = content.GetEvents();

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEventSelected(int id)
        {
            var response = content.GetEventSelected(id);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteImageEvent(int id)
        {
            var response = content.DeleteImageEvent(id);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateEvent(string data)
        {
            var response = content.UpdateEvent(data);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SearchEvents(string data)
        {
            var response = content.SearchEvents(data);

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetReviews()
        {
            var response = content.GetReviews();

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public JsonResult StatusReview(Int64 id)
        {
            var response = content.StatusReview(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEvent(Int64 eventid)
        {
            var response = content.DeleteEvent(eventid);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateFeature(Int64 id, string text)
        {
            var response = content.updateFeature(id, text);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deleteFeature(Int64 id)
        {
            var response = content.deleteFeature(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getSurveysGroup()
        {
            var response = content.getSurveysGroup();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getTemplateSurvey(Int64 fieldGroupID)
        {
            var response = content.getTemplateSurvey(fieldGroupID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveSurvey(int fieldGroupID, string data,  Guid ambassadorID, Boolean contactInfo, 
            string culture, Int64 answerID, DateTime date, string membership, int location, 
            int membershipToLink, int locationID, DateTime departureDate)
        {
            var response = content.SaveSurvey(fieldGroupID, data, ambassadorID, contactInfo, culture, answerID, 
                date, membership, location, membershipToLink,  locationID, departureDate);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAmbassadors()
        {
            var response = content.GetAmbassadors();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSurveysSaved(DateTime fromDate, DateTime toDate, string folio, string locationID)
        {
            var response = content.GetSurveysSaved(fromDate, toDate, folio, locationID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSurveyToEdit(Int64 fieldGroupID, Int64 answerID, int membershipID)
        {
            var response = content.GetSurveyToEdit(fieldGroupID, answerID, membershipID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSurvey(int fieldGroupID, string data, Guid ambassadorID, 
            Boolean contactInfo, string culture, Int64 answerID,DateTime date,
            string membership, int location, int membershipToLink, int locationID , DateTime departureDate)
        {
            var response = content.UpdateSurvey(fieldGroupID, data, ambassadorID, contactInfo, 
                culture, answerID, date, membership, location, membershipToLink, locationID, departureDate);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deletePoll(Int64 id)
        {
            var response = content.deletePoll(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProviders(Int64 terminalid)
        {
            var response = content.GetProviders(terminalid);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInfoForProviders()
        {
            var response = content.GetInfoForProviders();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getEmails()
        {
            var response = content.getEmails();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetActivesCodes()
        {
            var response = content.GetActivesCodes();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProvider(Int64 providerID)
        {
            var response = content.getProvider(providerID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveProvider(string data)
        {
            var response = content.saveProvider(data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateProvider(Int64 providerID, string data)
        {
            var response = content.updateProvider(providerID, data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UploadImageProvider()
        {
            if (Request.Files.Count > 0)
            {

                ePlatEntities db = new ePlatEntities();

                var id = Convert.ToInt64(Request.Params.Get("id"));
                var provider = (from eve in db.tblProviders
                              where eve.providerID == id
                              select eve
                              ).FirstOrDefault();           
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    Random rand = new Random();
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    var fileName = Convert.ToString(rand.Next() + ext);
                    var path = Path.Combine(Server.MapPath("~/Images/senses/"), fileName);
                    provider.image = fileName;
                    db.SaveChanges();
                    file.SaveAs(path);
                    return Json(fileName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("not", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UploadLogoProvider()
        {
            if (Request.Files.Count > 0)
            {

                ePlatEntities db = new ePlatEntities();

                var id = Convert.ToInt64(Request.Params.Get("id"));
                var provider = (from eve in db.tblProviders
                                where eve.providerID == id
                                select eve
                              ).FirstOrDefault();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    Random rand = new Random();
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    var test = file.FileName;
                    var fileName = Convert.ToString(rand.Next() + ext);
                    var path = Path.Combine(Server.MapPath("~/Images/senses/"), fileName);
                    provider.logo = fileName;
                    db.SaveChanges();
                    file.SaveAs(path);
                    return Json(fileName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("not", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UploadSurvey()
        {
            if (Request.Files.Count > 0)
            {

                ePlatEntities db = new ePlatEntities();
               
                var id = Convert.ToInt64(Request.Params.Get("id"));
                var survey = (from eve in db.tblFieldGroupsAnswers
                                where eve.FieldGroupsAnswersID == id
                                select eve
                              ).FirstOrDefault();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    Random rand = new Random();
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    var test = file.FileName;
                    var fileName = Convert.ToString(rand.Next() + ext);
                    
                   
                    var firstPath = HttpContext.Server.MapPath("~/");
                    var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                    var finalPath = secondPath + "ePlat\\Content\\files\\surveys\\";
                    var path = Path.Combine(finalPath, fileName);

                    survey.Picture = fileName;
                    db.SaveChanges();
                    file.SaveAs(path);
                    return Json(fileName, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("not", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteImageProvider(int id)
        {
            var response = content.DeleteImageProvider(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLogoProvider(int id)
        {
            var response = content.DeleteLogoProvider(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteProvider(int id)
        {
            var response = content.DeleteProvider(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocations()
        {
            var response = content.GetLocations();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveMembershipComment(int membershipSaleID, string comment,
            string bookingStatus, DateTime dueDate)
        {
            var response = content.saveMembershipComment(membershipSaleID, comment,
                bookingStatus, dueDate);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getBookingStatusOptions()
        {
            var response = content.getBookingStatusOptions();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }

}
