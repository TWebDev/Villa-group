using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatApi.Controllers.RCReservations
{
    [Authorize]
    public class MemberReservationsController : ApiController
    {
        //// GET: api/MemberReservations
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/MemberReservations/5
        public MemberReservationsViewModel.Reservation Get(string id)
        {
            return ResortConnectDataModel.GetReservation(id);
        }
        /*
        // POST: api/MemberReservations
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MClass1.csemberReservations/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MemberReservations/5
        public void Delete(int id)
        {
        }
        */
    }
}
