using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatFront.Models;
using MvcSiteMapProvider.Web;

namespace ePlatFront.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Index()
        {
            PageDataModel pageModel = new PageDataModel();
            return View(pageModel.GetPage(""));
        }

        public ActionResult Sitemap()
        {
            return new XmlSiteMapResult();
        }

        public ActionResult ConversionCode(int id, int? pointOfSaleID)
        {
            ViewBag.HeadScript = ControlsDataModel.GetHeadScript();
            ViewBag.ConversionCode = ControlsDataModel.GetConversionCode(id, pointOfSaleID);
            return View();
        }

        [HandleError]
        public ActionResult Router(string path)
        {
            string friendlyUrl = UrlModels.GetFriendlyUrl();
            string clientUrl = UrlModels.GetDomain() + Request.RawUrl;

            if (friendlyUrl == "/")
            { //si la ruta es igual a "" entonces enviar página Default.
                string defaultPage = UrlModels.GetDefaultPage();
                return new TransferResult(Url.Content("~" + defaultPage));
            }
            else if (friendlyUrl == "/sitemap.xml")
            {
                return new XmlSiteMapResult();
            }
            else if (friendlyUrl == "/coupons.aspx")
            {
                return Redirect("http://" + clientUrl.Replace("www","legacy").Replace("beta","legacy"));
                //return new TransferResult("/coupons/" + HttpContext.Request.QueryString["id"]);
            }
            else
            {
                return new TransferResult(Url.Content("~" + UrlModels.GetRealUrl(friendlyUrl, UrlModels.GetVirtualParams())));
            }
        }
    }
}