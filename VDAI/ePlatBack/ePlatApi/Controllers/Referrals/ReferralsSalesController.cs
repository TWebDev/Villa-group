using System.Collections.Generic;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatApi.Controllers.Referrals
{
    [Authorize]
    public class ReferralsSalesController : ApiController
    {
        // GET: api/ReferralsSales
        public IEnumerable<APIReferralsViewModel.ReferralReservation> Get(string startDate, string endDate)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return ApiReferralsDataModel.GetReservations(startDate, endDate, 2);
            }
            else
            {
                return new List<APIReferralsViewModel.ReferralReservation>();
            }
        }
    }
}
