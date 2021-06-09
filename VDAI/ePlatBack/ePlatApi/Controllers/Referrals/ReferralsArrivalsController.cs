using System.Collections.Generic;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatApi.Controllers.Referrals
{
    [Authorize]
    public class ReferralsArrivalsController : ApiController
    {
        // GET: api/ReferralsArrivals
        public IEnumerable<APIReferralsViewModel.ReferralReservation> Get(string startDate, string endDate)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return ApiReferralsDataModel.GetReservations(startDate, endDate, 1);
            }
            else
            {
                return new List<APIReferralsViewModel.ReferralReservation>();
            }                
        }

        /*
        // GET: api/Arrivals/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Arrivals
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Arrivals/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Arrivals/5
        public void Delete(int id)
        {
        }
        */
    }
}
