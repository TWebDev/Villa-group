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

    public class GuestPreferencesController : ApiController
    {
        // GET: api/GuestPreferences?resortId={resortId}&language={culture}
        /// <summary>
        /// Permite obtener la lista general de preferencias organizadas por tipo de preferencia a partir del resortid y language.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GuestViewModel.Preferences.PreferenceType> Get(int resortId, string culture)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return GuestDataModel.GetPreferencesResortList(resortId, culture);
            }
            else
            {

                return new List<GuestViewModel.Preferences.PreferenceType>();
            }            
        }

        // GET: api/GuestPreferences
        /// <summary>
        /// Permite obtener la lista general de preferencias organizadas por tipo de preferencia.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GuestViewModel.Preferences.PreferenceType> Get()
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return GuestDataModel.GetPreferencesList();
            }
            else
            {
                return new List<GuestViewModel.Preferences.PreferenceType>();
            }
        }

        // GET: api/GuestPreferences/00000000-0000-0000-0000-000000000000
        /// <summary>
        /// Permite obtener las preferencias del cliente, mediante su GuestHubID.
        /// </summary>
        /// <returns></returns>
        public GuestViewModel.PreferencesResponse Get(Guid id)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return GuestDataModel.GetPreferences(id, null, null);
            }
            else
            {
                return new GuestViewModel.PreferencesResponse();
            }
        }

        // GET: api/GuestPreferences/{id}?resortId={resortId}
        /// <summary>
        /// Permite obtener las preferencias del cliente, mediante el idCliente y el idResort de Front Office.
        /// </summary>
        /// <returns></returns>
        public GuestViewModel.PreferencesResponse Get(long id, int? resortId)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                return GuestDataModel.GetPreferences(null, id, resortId);
            }
            else
            {
                return new GuestViewModel.PreferencesResponse();
            }
        }

        //// POST: api/GuestPreferences
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT: api/GuestPreferences/5
        /// <summary>
        /// Permite actualizar las preferencias del cliente.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Put(Guid id, GuestViewModel.PreferencesModel model)
        {
            //try
            //{
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer != null ? Request.Headers.Referrer.Host : null), User.Identity.Name))
            {
                GuestDataModel.UpdatePreferences(id, model);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            //}
            //catch (Exception e)
            //{
            //    return Request.CreateResponse(HttpStatusCode.InternalServerError, e.InnerException);
            //}            
        }

        //// DELETE: api/GuestPreferences/5
        //public void Delete(int id)
        //{
        //}
    }
}
