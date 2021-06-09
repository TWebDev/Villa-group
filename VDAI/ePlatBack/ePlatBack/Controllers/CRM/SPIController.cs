using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Script.Serialization;

namespace ePlatBack.Controllers.CRM
{
    public class SPIController : Controller
    {
        [Authorize]
        public JsonResult GetCustomerHistory(int spiCustomerID)
        {
            return Json(SPIDataModel.GetCustomerHistory(spiCustomerID), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult SearchCustomer(string firstname, string lastname)
        {
            return Json(SPIDataModel.GetDDLSearchCustomer(firstname, lastname), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult SearchCustomerHistory(string firstname, string lastname)
        {
            return Json(SPIDataModel.SearchCustomerHistory(firstname, lastname), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult RenderAgencyManifest(){
            return PartialView("_AgencyManifestPartial", new SPIViewModel.AgencyManifest());
        }

        [Authorize]
        public JsonResult GetManifestForAgency(string date)
        {
            var _date = DateTime.Parse(date, CultureInfo.InvariantCulture);
            var terminal = long.Parse(new UserSession().Terminals.Split(',').FirstOrDefault());
            var list = new List<SPIViewModel.AgencyCustomer>();
            list = SPIDataModel.GetManifestForAgency(_date, terminal);
            var response = new JavaScriptSerializer().Serialize(list);
            return Json(response);
        }

        [Authorize]
        public JsonResult GetManifestForVLO(string date)
        {
            var _date = DateTime.Parse(date, CultureInfo.InvariantCulture);
            var terminal = long.Parse(new UserSession().Terminals.Split(',').FirstOrDefault());
            var list = new List<SPIViewModel.VLOCustomer>();
            list = SPIDataModel.GetManifestForVLO(_date, terminal, false);
            
            //var response = new JavaScriptSerializer().Serialize(list);
            //return Json(response);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CorrectSPIData(long terminalID)
        {
            var response = SPIDataModel.CorrectSPIData(terminalID);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
