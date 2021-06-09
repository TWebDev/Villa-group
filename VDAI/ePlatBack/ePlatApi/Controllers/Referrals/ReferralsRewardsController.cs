using System;
using System.Collections.Generic;
using System.Web.Http;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.Web;

namespace ePlatApi.Controllers.Referrals
{
    [Authorize]
    public class ReferralsRewardsController : ApiController
    {
        // GET: api/ReferralsRewards?memberAccount={memberAccount}
        /// <summary>
        /// Permite obtener un resumen de la informacion del cliente como lo son:
        ///     Nombre,
        ///     Terminos del servicio aceptados
        ///     Referidos,
        ///     Transaciones,
        /// </summary>
        /// <returns></returns>
        public APIReferralsViewModel.Rewards Get(string memberAccount)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.GetRewards(memberAccount);
            }
            else
            {
                return new APIReferralsViewModel.Rewards();
            }
        }
        // POST: api/ReferralsRewards?memberAccount={memberAccount}
        /// <summary>
        /// Permite que el cliente acepte los terminos y condiciones del programa Referrals
        /// </summary>
        /// <returns></returns>
        public AttemptResponse Post(string memberAccount)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.AcceptTermsAndConditions(memberAccount);
            }
            else
            {
                return new AttemptResponse();
            }
        }
    }
}
