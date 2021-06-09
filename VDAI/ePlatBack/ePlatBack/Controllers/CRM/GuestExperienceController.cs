
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
using ePlatBack.Models;

namespace ePlatBack.Controllers.CRM
{
    public class GuestExperienceController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData.Model = new GuestExperienceViewModel() {
                Search = new GuestExperienceViewModel.SearchFilters()
                {
                    Search_FromDate = DateTime.Today.ToString("yyyy-MM-dd"),
                    Search_ToDate = DateTime.Today.ToString("yyyy-MM-dd")
                }
            };
            return View();
        }

        [Authorize]
        public JsonResult GetArrivals(GuestExperienceViewModel.SearchFilters model)
        {
            return Json(GuestExperienceDataModel.GetArrivals(model), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetPreferencesForResort(int id)
        {
            return Json(GuestDataModel.GetPreferencesResortList(id, "en-US"), JsonRequestBehavior.AllowGet);
        }
    }
}
