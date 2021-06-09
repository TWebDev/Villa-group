using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatApi.Controllers.MarketPlace
{
    [Authorize]
    public class MarketPlaceItemController : ApiController
    {
        // GET: api/MarketPlaceItem/0&language={culture}&currencyID={1}&pointOfSaleID={2}
        /// <summary>
        /// Permite obtener los detalles de un elemento a partir de su ID.
        /// </summary>
        /// <returns></returns>
        public MarketPlaceViewModel.ItemDetail Get(long id, string language, int currencyID, int pointOfSaleID)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return MarketPlaceDataModel.GetItem(id, language, currencyID, pointOfSaleID);
            }
            else
            {
                return new MarketPlaceViewModel.ItemDetail();
            }
        }
    }
}
