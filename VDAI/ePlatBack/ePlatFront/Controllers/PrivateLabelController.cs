using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using ePlatFront.Models;

namespace ePlatFront.Controllers
{
    [AllowCrossSiteJson]
    public class PrivateLabelController : Controller
    {
        //
        // GET: /PrivateLabel/       
        public JsonResult GetHTMLStructure()
        {
            var content = System.IO.File.ReadAllText(Server.MapPath(@"~/Content/plugins/privatelabel/privatelabel.html"));
            return Json(new { HTML = content }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCatalog(int id, string culture)
        {
            return Json(PrivateLabelDataModel.GetCatalog(id, culture), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivitiesForCategory(long id, string culture, long terminalid)
        {
            return Json(PrivateLabelDataModel.GetActivitiesForCategory(id, culture, terminalid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivityDetail(long id,string culture, long terminalid)
        {
            ActivityDataModel adm =  new ActivityDataModel();
            return Json(adm.GetActivityDetail(id, culture, terminalid), JsonRequestBehavior.AllowGet);
        }

        //System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BlockContentPartial.cshtml", ControlsDataModel.BlockContentDataModel.getBlockContent("Right Column")));
    }
}
