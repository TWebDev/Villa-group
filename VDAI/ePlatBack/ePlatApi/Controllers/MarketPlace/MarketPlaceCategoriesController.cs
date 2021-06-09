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
    public class MarketPlaceCategoriesController : ApiController
    {
        // GET: api/MarketPlaceCategories/0?language={culture}
        /// <summary>
        /// Permite obtener la lista de categorías a partir del ID del catálogo y el language.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MarketPlaceViewModel.Category> Get(int id, string language)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return MarketPlaceDataModel.GetCategories(id, language);
            }
            else
            {
                return new List<MarketPlaceViewModel.Category>();
            }
        }
    }
}
