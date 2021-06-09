using ePlatBack.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Controllers.CRM
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserForAssignation(int? leadTypeID, long terminalID)
        {
            return Json(NetCenterDataModel.GetUserForAssignation(leadTypeID, terminalID));
        }
    }
}