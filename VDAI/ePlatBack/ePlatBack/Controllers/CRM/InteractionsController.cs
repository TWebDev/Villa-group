using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using ePlatBack.Models;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.Threading.Tasks;

namespace ePlatBack.Controllers.CRM
{
    public class InteractionsController : Controller
    {
        //Interactions
        [Authorize]
        public JsonResult GetInteractionsForTrial(Guid id)
        {
            return Json(InteractionDataModel.GetInteractionsList(null, id), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetInteractionsForLead(Guid id)
        {
            return Json(InteractionDataModel.GetInteractionsList(id, null), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInteractionTypes()
        {
            return Json(InteractionDataModel.GetAllInteractionTypes(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInterestLevels()
        {
            return Json(InteractionDataModel.GetInterestLevels(), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public JsonResult DeleteInteraction(long id)
        {
            AttemptResponse attempt = InteractionDataModel.DeleteInteraction(id);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                InteractionID = id,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        [Authorize]
        [HttpPost]
        public JsonResult SaveInteractionInfo(InteractionsViewModel.Interaction model)
        {
            AttemptResponse attempt = InteractionDataModel.SaveInteractionInfo(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                Interaction = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
    }
}
