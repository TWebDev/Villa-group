using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatApi.Controllers.CRM
{
    public class GuestHubIDController : ApiController
    {
        //// GET: api/GuestHubID
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/GuestHubID/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // GET: api/GuestHubID/5
        public GuestViewModel.GuestHubIDJSONResponse Get(int resortid, DateTime date, string rk)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return GuestDataModel.GetGuestHubID(resortid, rk, date, "ePlatAPI - GuestHubID");
            }
            else
            {
                return new GuestViewModel.GuestHubIDJSONResponse();
            }
        }

        //// POST: api/GuestHubID
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/GuestHubID/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/GuestHubID/5
        //public void Delete(int id)
        //{
        //}
    }
}
