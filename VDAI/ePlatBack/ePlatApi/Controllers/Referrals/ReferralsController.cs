using System;
using System.Collections.Generic;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;

namespace ePlatApi.Controllers.Referrals
{
    [Authorize]
    // POST: api/Referrals
    public class ReferralsController : ApiController
    {
        // GET: api/Referrals/
        /// <summary>
        /// Permite añadir referidos al cliente, solo es posible enviar un referido a la vez
        /// </summary>
        /// <returns></returns>
        public AttemptResponse Post(APIReferralsViewModel.NewReferral model)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.NewReferral(model);
            }
            else
            {
                return new AttemptResponse();
            }
        }
        // GET: api/Referrals/
        /// <summary>
        /// Permite Actualizar los datos de un referido del cliente, mientras no haya sido contactado
        /// </summary>
        /// <returns></returns>
        public AttemptResponse Patch(APIReferralsViewModel.UpdateReferral model)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.UpdateReferral(model);
            }
            else
            {
                return new AttemptResponse();
            }
        }
        // POST: api/Referrals/
        /// <summary>
        /// Permite enviar una invitacion al referidos
        /// </summary>
        /// <returns></returns>
        public AttemptResponse Post(APIReferralsViewModel.ReferralInvitation model, string memberAccount)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.SendInvitationsReferrals(model, memberAccount);
            }
            else
            {
                return new AttemptResponse();
            }
        }

        //public APIReferralsViewModel.ReferralInfo Get(string memberAcount, DateTime fromDate, DateTime toDate)
        //{

        //}
    }
}
