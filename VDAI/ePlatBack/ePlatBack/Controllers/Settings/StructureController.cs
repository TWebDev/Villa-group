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
    public class StructureController : Controller
    {
        [Authorize]        
        public ActionResult Index()
        {
            return View();
        }
    }
}
