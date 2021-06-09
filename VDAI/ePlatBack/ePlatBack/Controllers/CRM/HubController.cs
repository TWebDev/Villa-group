using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using System.Web.Http.WebHost;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.Security.Principal;
using ePlatBack.Controllers.CRM;




namespace ePlatBack.Controllers.CRM
{
    public class HubController : Controller
    {
        
        public ActionResult Index()
        {
            ViewData.Model = new HubViewModel
            {
                GuestSearch = new GuestSearchModel(),
                GuestInfo= new GuestInfoModel(),
                
            };
            //HubDataModel.GetArrivals();
            return View();
        }
        public JsonResult GetGuest(string GuestReservationID)
        {
            
            HubDataModel hdm = new HubDataModel();
            GuestInfoModel gim = new GuestInfoModel();
            gim = hdm.GetGuest(GuestReservationID);
            return Json(gim);
        }
        

        public ActionResult GetReservationsView(string GuestHubID)
        {
            HubDataModel hdm = new HubDataModel();

            return PartialView("_GuestProfileResultsPartial",hdm.GetReservations(GuestHubID).Distinct());
        }

        public JsonResult GetMemberInfo(string GuestHubID)
        {
            HubDataModel hdm = new HubDataModel();
            return Json(hdm.GetMemberInfo(GuestHubID));
        }
        public ActionResult GetCountStays(string GuestHubID)
        {
            HubDataModel hdm = new HubDataModel();
            
            return Json(hdm.GetCountStays(GuestHubID));
        }

        public ActionResult GetHistoryReservationsView(string GuestHubID)
        {
            HubDataModel hdm = new HubDataModel();
            List<GuestReservationModel> lista = new List<GuestReservationModel>();
            lista = hdm.GetHistoryReservations(GuestHubID);
            return PartialView("_GuestHistoryReservationsResultsPartial", hdm.GetHistoryReservations(GuestHubID).Distinct());
        }

        public ActionResult Search(GuestSearchModel gsm)
        {
            HubDataModel hdm = new HubDataModel();
            HubViewModel hvm = new HubViewModel();
            List<GuestSearchModel> lista= new List<GuestSearchModel>();
            lista = hdm.GetArrivals(gsm);
            return PartialView("_SearchGuestsResultsPartial", hdm.GetArrivals(gsm).Distinct());
        }

        public JsonResult GetPreferences(String PreferenceType)
        {
            HubDataModel hdm = new HubDataModel();
            List<SelectListItem> Preferences = new List<SelectListItem>();
            Preferences = hdm.GetPreferencesList(PreferenceType);
            return Json(Preferences);
        }

        public JsonResult GetGuestPreferences(String GuestHubId)
        {
            HubDataModel hdm = new HubDataModel();
            List<GuestPreferences> Preferences = new List<GuestPreferences>();
            
            Preferences = hdm.GetPreferencesByGuest(GuestHubId);
            return Json(Preferences);
        }

        public JsonResult SavePreference(PreferenceRegistrationModel prm)
        {
            HubDataModel hdm = new HubDataModel();
            List<SelectListItem> Preferences = new List<SelectListItem>();
            
            AttemptResponse attempt=hdm.SaveGuestPreference(prm);
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });

        }
        //*****test*****
        public AttemptResponse SaveGuest()
        {
            Guid GuestHubID;
            GuestHubID = Guid.NewGuid();
            AttemptResponse attempt = new AttemptResponse();
            HubDataModel hdm = new HubDataModel();
            
            GuestInfoModel gim = new GuestInfoModel();
            gim.FirstName = "TestName";
            gim.LastName = "TestLastName";
            gim.Email = "Email1";
            gim.Email2 = "Email2";
            gim.Email3 = "Email3";
            gim.City = "TestCity";
            gim.State = "TestState";
            gim.CountryID = 1;
            gim.GuestHubID = GuestHubID;
            
            attempt=hdm.SaveGuestHub(gim);
            return attempt;
        }
        //*****test*****

        /*
        public JsonResult MigrateGuests()
        {
            HubDataModel hdm = new HubDataModel();
            AttemptResponse attempt = new AttemptResponse();
            attempt= hdm.MigrateGuests();
            return Json(new {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            
            });
        }
        */
        public JsonResult DeleteGuestPreference(string preferenceType,string guestPreferenceID)
        {
            
            
            AttemptResponse attempt = new AttemptResponse();
            HubDataModel hdm = new HubDataModel();
            attempt=hdm.DeletePreference(guestPreferenceID,preferenceType);
            
            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }


       
        public ActionResult GetGuestInfo(string id)
        {
            HubDataModel hdm = new HubDataModel();

            return PartialView("_GuestProfileReservationPartial",hdm.GetGuestInfoByReservationID(id));
        }

        public JsonResult UploadHotelExpert()
        {
            AttemptResponse attempt = new AttemptResponse();
            HubDataModel hdm = new HubDataModel();
            //hdm.BindGuestToTask();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if(file!= null && file.ContentLength>0)
                {
                    
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        file.SaveAs(path);
                        attempt=hdm.LoadFileHE(path);     
                }
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

        public PictureDataModel.FineUploaderResult UploadGuestPicture(PictureDataModel.FineUpload upload)
        {
            var q = Request.QueryString["Filename1"];
            upload.Filename = (q.ToString() + ".jpg");

            //this one works for bot - form and query string
            
            return (HubDataModel.SaveGuestPicture(upload));
        }

        /*[HttpPost]
        public ActionResult UploadGuestPicture(GuestInfoModel gim)
        {
            AttemptResponse attempt = new AttemptResponse();
            HubDataModel hdm = new HubDataModel();
            //hdm.BindGuestToTask();
            

           
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/GuestsPictures"), (gim.GuestHubID.ToString()+".jpg"));
                    file.SaveAs(path);
                    
                }
            }
            
            HubViewModel hvm = new HubViewModel();

            return View("_GuestProfilePartial", gim);

        }
        */

        public ActionResult GetTaskView(string GuestHubID)
        {
            HubDataModel hdm = new HubDataModel();
            List<GuestTasksModel> list = new List<GuestTasksModel>();
            list = hdm.GetTasks(GuestHubID);
            return PartialView("_GuestTasksResultsPartial", list);
        }



        public JsonResult UploadSurveys()
        {
            AttemptResponse attempt = new AttemptResponse();
            HubDataModel hdm = new HubDataModel();
            //hdm.BindGuestToTask();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    file.SaveAs(path);
                    attempt = hdm.LoadFileSurveys(path);
                }
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

        public ActionResult GetClarabridgeData(string GuestHubID)
        {
            HubDataModel hdm = new HubDataModel();
            return PartialView("_GuestClaraBridgeResultsPartial",hdm.GetClaraBridgeInfo(GuestHubID));
        }

        public JsonResult GetAnswersSurveys(string survey)
        {
            HubDataModel hdm = new HubDataModel();
            return Json(hdm.GetSurveyAnswers(survey));
        }
        
        //[BasicAuthenticationAttribute("TaferProfile","T4f3RCrMUs3R",BasicRealm="1234")]
        //public JsonResult GetPreferencesAPI(GuestPreferencesApi gpa)
        //{
        //    HubDataModel hdm = new HubDataModel();
        //    return Json(hdm.GetPreferencesByGuestAPI(gpa));
        //}

        //[BasicAuthenticationAttribute("TaferProfile", "T4f3RCrMUs3R", BasicRealm = "1234")]
        //public JsonResult GetPreferencesGuestAPI(GuestPreferencesApi gpa)
        //{
            
        //    HubDataModel hdm = new HubDataModel();
        //    return Json(hdm.GetPreferencesGuestByGuestAPI(gpa));
        //}

        //[BasicAuthenticationAttribute("TaferProfile", "T4f3RCrMUs3R", BasicRealm = "1234")]
        //public JsonResult SavePreferencesGuestAPI(GuestPreferencesApi gpa)
        //{

        //    HubDataModel hdm = new HubDataModel();
        //    AttemptResponse attempt = hdm.SaveGuestPreferenceAPI(gpa);
        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ItemID = attempt.ObjectID,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
        //        InnerException = Debugging.GetInnerException(attempt.Exception)
        //    });
        //}
    }

}
