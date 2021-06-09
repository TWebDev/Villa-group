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
    [Authorize]
    public class PreferencesSegmentationController : ApiController
    {
        // GET: api/PreferencesSegmentation
        public GuestViewModel.GuestsByPreference Get(int resortID, string fromDate, string toDate, string preferences)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return GuestDataModel.GetGuestsByPreference(resortID, fromDate, toDate, preferences);
            }
            else
            {

                return new GuestViewModel.GuestsByPreference();
            }  
        }

        /*
        // GET: api/PreferencesSegmentation/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PreferencesSegmentation
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/PreferencesSegmentation/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PreferencesSegmentation/5
        public void Delete(int id)
        {
        }*/
    }
}
