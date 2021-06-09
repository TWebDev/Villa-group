using ePlatBack.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.DataModels;

namespace ePlatApi.Controllers.MarketPlace
{
    [Authorize]
    public class MarketPlaceItemScheduleController : ApiController
    {
        /*
        // GET: api/MarketPlaceItemSchedule
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        */

        // GET: api/MarketPlaceItemSchedule/5
        public IEnumerable<MarketPlaceViewModel.ItemSchedule> Get(int id, DateTime date)
        {
            return ActivityDataModel.GetItemSchedules(id, date);
        }

        /*
        // POST: api/MarketPlaceItemSchedule
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MarketPlaceItemSchedule/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MarketPlaceItemSchedule/5
        public void Delete(int id)
        {
        }
        */
    }
}
