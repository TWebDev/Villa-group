using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatApi.Controllers.MarketPlace
{
    [Authorize]
    public class MarketPlaceCartsController : ApiController
    {
        /*
        // GET: api/MarketPlacePurchases
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        */

        // GET: api/MarketPlacePurchases/5
        public MarketPlaceViewModel.Cart Get(Guid? id)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return PurchaseDataModel.GetMarketPlaceCart(id, null);
            }
            else
            {
                return new MarketPlaceViewModel.Cart();
            }
        }

        public MarketPlaceViewModel.Cart Get(string confirmation)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return PurchaseDataModel.GetMarketPlaceCart(null, confirmation);
            }
            else
            {
                return new MarketPlaceViewModel.Cart();
            }
        }


        // POST: api/MarketPlacePurchases
        public AttemptResponse Post(MarketPlaceViewModel.Cart cart)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                cart.IPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                var browser = System.Web.HttpContext.Current.Request.Browser;
                if (browser != null)
                {
                    cart.Browser = browser.Browser + " - " + browser.Version + " [" + System.Web.HttpContext.Current.Request.UserAgent != null ? System.Web.HttpContext.Current.Request.UserAgent : "" + "]";
                    cart.OS = browser.Platform;
                }

                return PurchaseDataModel.SaveMarketPlaceCart(cart);
            }
            else
            {
                AttemptResponse response = new AttemptResponse();
                response.Type = Attempt_ResponseTypes.Error;
                return response;
            }
        }

        /*
        // PUT: api/MarketPlacePurchases/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MarketPlacePurchases/5
        public void Delete(int id)
        {
        }
        */
    }
}
