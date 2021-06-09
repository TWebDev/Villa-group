using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using ePlatFront.Models;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;

namespace ePlatFront.Controllers
{
    [AllowCrossSiteJson]
    public class PurchaseController : Controller
    {
        // GET: /Purchase/ShoppingCart
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult ShoppingCart()
        {
            PageDataModel dm = new PageDataModel();
            PageViewModel pvm = dm.GetDefaultPageInfo();
            pvm.Seo_Title = "Shopping Cart - My Experience Tours";
            pvm.Seo_Follow = "nofollow";
            pvm.Seo_Index = "noindex";
            pvm.CanonicalDomain = ePlatBack.Models.Utils.GeneralFunctions.GetCanonicalDomain();
            pvm.Seo_FriendlyUrl = "/Purchase/ShoppingCart";
            return View(pvm);            
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult CheckOut()
        {
            PageDataModel dm = new PageDataModel();
            PurchaseProcessViewModel pvm = new PurchaseProcessViewModel();
            pvm.Page = dm.GetDefaultPageInfo();
            pvm.Page.Seo_Title = "Check Out - My Experience Tours";
            pvm.Page.Seo_Follow = "nofollow";
            pvm.Page.Seo_Index = "noindex";
            pvm.Page.CanonicalDomain = ePlatBack.Models.Utils.GeneralFunctions.GetCanonicalDomain();
            pvm.Page.Seo_FriendlyUrl = "/Purchase/ShoppingCart";
            return View(pvm);
        }

        //
        // POST: /Purchase/Save
        [HttpPost]
        public async Task<JsonResult> Save(PurchaseProcessViewModel purchase)
        {
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            string browser = Request.Browser.Browser + " - " + Request.Browser.Version + " [" + Request.UserAgent + "]";
            string os = Request.Browser.Platform;
            purchase.IP = ip;
            purchase.Browser = browser;
            purchase.OS = os;
            
            AttemptResponse attempt = new AttemptResponse();
            attempt = await PurchaseDataModel.Save(purchase);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            SavePurchaseResponse r = (SavePurchaseResponse)attempt.ObjectID;

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                r.PurchaseID,
                r.SessionID,
                Items = PurchaseDataModel.GetPurchaseItems(r.PurchaseID),
                AuthCode = PurchaseDataModel.GetAuthCode(r.PurchaseID)
            });
        }
        
        //
        // POST: /Purchase/PartialSave
        [HttpPost]
        public JsonResult PartialSave(PurchaseProcessViewModel purchase)
        {
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            string browser = Request.Browser.Browser + " - " + Request.Browser.Version + " [" + Request.UserAgent + "]";
            string os = Request.Browser.Platform;
            purchase.IP = ip;
            purchase.Browser = browser;
            purchase.OS = os;

            AttemptResponse attempt = new AttemptResponse();
            attempt = PurchaseDataModel.PartialSave(purchase);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                PurchaseID = (attempt.ObjectID != null ? attempt.ObjectID  : ""),
                Items = PurchaseDataModel.GetPurchaseItems((Guid)attempt.ObjectID)
            });
        }

        //
        // GET: /Purchase/GetPromo
        public JsonResult GetPromo(string services, string traveldates, long terminalid, string promocode)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = PromoDataModel.GetPromo(services, traveldates, terminalid, promocode);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : "",
                Promos = attempt.ObjectID        
            }, JsonRequestBehavior.AllowGet);
        }

        //
        //GET: /Purchase/GetPaymentsProvider
        public JsonResult GetPaymentsProvider()
        {
            return Json(PurchaseDataModel.GetPaymentsProvider(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartSettings()
        {
            return Json(PurchaseDataModel.GetCartSettings(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PaymentGatewayResponse(Guid id, string resultIndicator)
        {
            if (PurchaseDataModel.PaymentGatewayResponse(id, resultIndicator))
            {
                return RedirectToAction("ConfirmationPage", new { id });
            }
            else
            {
                return RedirectToAction("CheckOut");
            }
        }

        //
        //GET: /Purchase/ConfirmationPage
        public ActionResult ConfirmationPage(Guid id)
        {          
            return View(PurchaseDataModel.GetPurchaseConfirmation(id));
        }

        //
        //GET: /Purchase/Confirmation
        public ActionResult Confirmation(ProsaResponseModel response)
        {
            //procesar respuesta
            bool validResponse = PurchaseDataModel.ProcessResponse(response);
            //mostrar confirmación
            PageDataModel pageModel = new PageDataModel();
            PageViewModel pageToView = pageModel.GetPage("/purchase-confirmation");
            if (validResponse == true) {
                pageToView.Content = pageToView.Content.Replace("$AuthCode",response.EM_Auth);
                pageToView.Content = pageToView.Content.Replace("$Total", (decimal.Parse(response.EM_Total) / 100).ToString());
                pageToView.Scripts_Footer += "<script>BookingEngine.deleteCart();$('#payment-valid').slideDown('fast');</script>";
            }
            else
            {
                pageToView.Content = pageToView.Content.Replace("$Error", response.EM_Response + "<br />" + response.EM_RefNum);
                pageToView.Scripts_Footer += "<script>$(function(){window.location.hash = '#shopping-cart';$('#btnGoCheckout').trigger('click');$('#payment-invalid').slideDown('fast');});</script>";
            }

            if (pageToView.Content.IndexOf("ePlatBlock(\"Right Column\")") >= 0)
            {
                pageToView.Content = pageToView.Content.Replace("ePlatBlock(\"Right Column\")", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BlockContentPartial.cshtml", ControlsDataModel.BlockContentDataModel.getSidebarContent())));
            }

            return View("~/Views/Pages/Index.cshtml", pageToView);
        }
    }
}
