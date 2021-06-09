using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models;

namespace ePlatApi.Controllers.MarketPlace
{
    [Authorize]
    public class MarketPlaceSettingsController : ApiController
    {
        // GET: api/MarketPlaceSettings/0
        /// <summary>
        /// Permite obtener los detalles de configuración del marketplace a partir de su placeID.
        /// </summary>
        /// <returns></returns>
        public MarketPlaceViewModel.Settings Get(int placeID, int terminalTypeID)
        {
            return MarketPlaceDataModel.GetSettings(placeID, terminalTypeID);
        }
    }
}