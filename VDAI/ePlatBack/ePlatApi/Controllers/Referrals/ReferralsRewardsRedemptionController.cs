using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System;
using System.Web.Http;

namespace ePlatApi.Controllers.Referrals
{
    [Authorize]
    public class ReferralsRewardsRedemptionController : ApiController
    {
        // GET: api/ReferralsRewardsRedemption/?resortId={resortId}&language={culture}
        /// <summary>
        /// Permite obtener el estado en el que se encuentra un RedemptionID
        /// </summary>
        /// <returns></returns>
        public APIReferralsViewModel.RedemptionInfo Get(Guid id)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.GetRedemptionInfo(id);
            }
            else
            {
                return new APIReferralsViewModel.RedemptionInfo();
            }
        }

        // POST: api/ReferralsRewardsRedemption/
        /// <summary>
        /// Permite generar un cargo al cliente
        /// </summary>
        /// <returns></returns>
        public ApiReferralsDataModel.RedemptionResponse Post(APIReferralsViewModel.RedeemCredits model)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.AuthorizeCharge(model);
            }
            else
            {
                return new ApiReferralsDataModel.RedemptionResponse();
            }
        }

        // DELETE: api/ReferralsRewards
        /// <summary>
        /// Permite borrar un RedemptionID y retorna los creditos al cliente
        /// </summary>
        /// <returns></returns>
        public ApiReferralsDataModel.RedemptionResponse Delete(Guid id)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return ApiReferralsDataModel.RefundCharge(id);
            }
            else
            {
                return new ApiReferralsDataModel.RedemptionResponse();
            }
        }
        // POST: api/ReferralsRewards
        /// <summary>
        /// Retorna la cantidad disponibles de creditos que tiene el cliente
        /// </summary>
        /// <returns>
        /// </returns>


        public ApiReferralsDataModel.RedemptionResponse Get(string memberAccount)
        {
            if (ApiValidationDataModel.IsValid(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), (Request.Headers.Referrer?.Host), User.Identity.Name))
            {
                return new ApiReferralsDataModel.RedemptionResponse
                {
                    Amount = ApiReferralsDataModel.GetLeadCredits(memberAccount).TotalActivePoints,
                    Status = RedemptionStatus.Ok,
                    Date = DateTime.Now
                };
            }
            else
            {
                return  new ApiReferralsDataModel.RedemptionResponse
                {
                    Status = RedemptionStatus.UserNotAuthorized,
                };
            }
        }

    }
}