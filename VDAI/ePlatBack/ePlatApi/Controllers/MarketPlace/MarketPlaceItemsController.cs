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
    public class MarketPlaceItemsController : ApiController
    {
        // GET: api/MarketPlaceItems/?categoryID={0}&language={culture}&currencyID={1}&pointOfSaleID={2}
        /// <summary>
        /// Permite obtener la lista de elementos a partir del ID de la categoría, el language, el ID de la moneda y el ID del punto de venta.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MarketPlaceViewModel.Item> Get(long categoryID, string language, int currencyID, int pointOfSaleID)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return MarketPlaceDataModel.GetItems(categoryID, language, currencyID, pointOfSaleID);
            }
            else
            {
                return new List<MarketPlaceViewModel.Item>();
            }
        }
    }
}
