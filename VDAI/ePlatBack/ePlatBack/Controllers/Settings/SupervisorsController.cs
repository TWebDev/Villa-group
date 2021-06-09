using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System.Data.Entity;
using System.Web.Script.Serialization;
using ePlatBack.Models.Utils;
namespace ePlatBack.Controllers.Settings
{
    public class SupervisorsController : Controller
    {

        ePlatEntities db = new ePlatEntities();
        [Authorize]
        public ActionResult Index()
        {
            ViewData.Model = new UserViewModel
            {
                UserSearch = new UserSearchModel(),
                UserInfo = new UserInfoModel(),
                UserSupervisorSearch=new UserSupervisorSearchModel(),
            };
            return View();
        }
        public ActionResult Search(UserSupervisorSearchModel ussm)
        {
            UserDataModel udm = new UserDataModel();
            UserViewModel uvm = new UserViewModel();
            return PartialView("_SearchSupervisorResults",udm.SearchSupervisor(ussm));
        }

        public JsonResult GetSubordinates(Guid userID)
        {
            UserDataModel udm = new UserDataModel();
            return Json(udm.GetSubordinates(userID));
        }

        public JsonResult GetSupervisors(Guid userID)
        {
            UserDataModel udm = new UserDataModel();
            return Json(udm.GetSupervisors(userID));
        }
    }
}
