using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Controllers.Settings
{
    public class AccessProfiles : Controller
    {
        //
        // GET: /AccessProfiles/
        [Authorize]
        public ActionResult Index()
        {
             return View();            
        }

    }
}
