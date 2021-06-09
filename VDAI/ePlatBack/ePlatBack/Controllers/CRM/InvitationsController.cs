using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers.CRM
{
    public class InvitationsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData.Model = new InvitationViewModel()
            {
                InvitationForm = new InvitationViewModel.InvitationInfoView(),
                Terminal = InvitationDataModel.GetTerminal()
            };
            return View();
        }

        [Authorize]
        public JsonResult GetInvitations(DateTime? date)
        {
            return Json(InvitationDataModel.GetInvitations(date), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDependantFields()
        {
            return Json(InvitationDataModel.InvitationCatalogs.GetDependantFields(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveInvitationInfo(InvitationViewModel.InvitationInfoModel model)
        {
            AttemptResponse attempt = InvitationDataModel.SaveInvitationInfo(model);

            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null ? errorLocation + attempt.Exception.InnerException.ToString() : "")
            });
        }
    }
}
