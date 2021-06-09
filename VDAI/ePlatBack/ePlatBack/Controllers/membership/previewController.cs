using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.DataModels;
using System.Web.Mvc;
using ePlatBack.Models;


namespace ePlatBack.Controllers.membership
{
    public class previewController : Controller
    {
        ePlatEntities db = new ePlatEntities();
        Memberships memberships = new Memberships();
        public ActionResult Index()
        {
          

            return View("~/Views/cardsManagement/Preview.cshtml");
        }

        public JsonResult addPreview(int location, string name, string lastname, string email, 
            int freeTrialDays, string phone , string comment, string bookingStatus, Boolean sendEmail)
        {
          
            var response = memberships.addPreview(location, name, lastname,email, freeTrialDays, phone, comment, 
                bookingStatus, sendEmail);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPreviewsPerDay (DateTime fromDate, DateTime toDate)
        {

            var previews = memberships.getPreviewsPerDay(fromDate, toDate);
            
            return Json(previews);
        }
        public JsonResult updatePreview(Guid harvestingid, string name, string lastname, string email,
            int location, string phone, string comment, DateTime dueDate, string bookingStatus)
        {

            var previews = memberships.updatePreview(harvestingid,name,lastname,email,location, phone, comment, dueDate, bookingStatus);

            return Json(previews);
        }

    }
}
