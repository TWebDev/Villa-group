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
using ePlatBack.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace ePlatBack.Controllers.CRM
{
    public class Invitations2Controller : Controller
    {
        [Authorize]
        //public ActionResult Index2() use Layout 2
        public ActionResult Index()
        {
            UserSession session = new UserSession();
            ePlatEntities db = new ePlatEntities();
            var spiUser = db.tblUserProfiles.Single(x => x.userID == session.UserID);
            if (spiUser.SPIUserName != null)
            {
                //PreManifest Button
                var privileges = AdminDataModel.GetViewPrivileges(11696);//Invitations
                ViewData.Model = new spiInvitation()
                {
                    Invitation = new spiInvitation.searchInvitation()
                    {
                        Privileges = privileges
                    },
                };
            }
            else
            {
                return View("WarningSPIUser");
            }
            return View();
        }

        //Use Layout3
        public ActionResult Index2(string report)
        {
            var partial = "";
            switch (report)
            {
                case "invitationsDeposits":
                    {
                        SPIInvitationReport.searchInvitationModel model = new SPIInvitationReport.searchInvitationModel();
                        partial = "_SearchInvitationsDepositsReport";
                        return PartialView(partial, model);
                    }
                default:
                    {
                        return PartialView(partial);
                    }
            }
        }

        public ActionResult RenderInvitationsInfo()
        {
            UserSession session = new UserSession();
            var terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            var invitationInfo = new spiInvitation.invitationModelTable()
            {
                Privileges = AdminDataModel.GetViewPrivileges(11771),//EDITAR
                fillDrpCurrencies = MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies(),//
                fillDrpSPICountries = spiInvitationDataModel.spiInvitationCatalog.fillDrpCountriesSPI(),
                fillDrpSPILanguage = spiInvitationDataModel.spiInvitationCatalog.fillDrpLanguageSPI(),
                fillDrpSPIGroups = spiInvitationDataModel.spiInvitationCatalog.fillDrpGroupsSPI(),
                fillDrpSPILocation = spiInvitationDataModel.spiInvitationCatalog.fillDrpLocationSPI(),
                fillDrpSPIHotel = spiInvitationDataModel.spiInvitationCatalog.fillDrpHotelSPI(),
                fillDrpSPISalesRooms = spiInvitationDataModel.spiInvitationCatalog.fillDrpSalesRoomSPI(),
                //fillDrpJalador = spiInvitationDataModel.spiInvitationCatalog.fillDrpJalador(),
                //fillDrpOPC = spiInvitationDataModel.spiInvitationCatalog.fillDrpOPCLegacyKey(),
                fillDrpPrograms = MasterChartDataModel.LeadsCatalogs.FillDrpPrograms(),
                fillDrpPresentationPlaces = spiInvitationDataModel.spiInvitationCatalog.fillDrpPresentationPlaces()
                // fillDrpOPC = MasterChartDataModel.LeadsCatalogs.FillDrpOPC(terminals[0], null)
            };
            invitationInfo.fillDrpCurrencies.Insert(0, ListItems.Default());
           // invitationInfo.fillDrpOPC.Insert(0, ListItems.Default());
            return PartialView("_InvitationInfoPartialSPI", invitationInfo);
        }
        public ActionResult RenderPreManifestInvitation()//modal
        {
            return PartialView("_InvitationResultPartial", new spiInvitation.invitationModel());
        }

        public ActionResult RenderEmailPreview(bool sendEmail, string model)//previewEmail
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            spiInvitation.emailPreview emailData = new spiInvitation.emailPreview();
            var emailModel = Json.Deserialize<spiInvitation.invitationModelTable>(model);
            emailData.email = spiInvitationDataModel.sendEmail(sendEmail, emailModel);   
            return PartialView("_InvitationEmailPreviewResul",emailData);
        }
        public JsonResult SearchInvitations(spiInvitation.searchInvitation model)
        {
            spiInvitationDataModel idm = new spiInvitationDataModel();
            return Json(idm.searchInvitations(model), JsonRequestBehavior.AllowGet);
        }
        public JsonResult searchGuestToMatch(spiInvitation.invitationModel model)
        {
            spiInvitationDataModel idm = new spiInvitationDataModel();
            return Json(idm.searchGuests(model), JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveSPIInvitation(spiInvitation.invitationModelTable model)
        {
            AttemptResponse attempt = spiInvitationDataModel.saveInvitation(model);
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
                ObjectID = attempt.ObjectID
              //  InnerException = (attempt.Exception != null ? errorLocation + attempt.Exception.InnerException.ToString() : "")
            });
        }
        public JsonResult manifestInvitation(string spiInvitationID,int salesRoomID, int? customerID = null)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = spiInvitationDataModel.preManifestedInvitation(spiInvitationID,salesRoomID, customerID);
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
               // InnerException = (attempt.Exception != null ? errorLocation + attempt.Exception.InnerException.ToString() : "")
            });
        }
        public JsonResult GetDependentFieldsFromInvitations()
        {
            return Json(spiInvitationDataModel.GetDependentFieldsFromspiInvitation(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveInvitationDeposits(string model)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<spiInvitation.invitationDeposits> deposits = js.Deserialize<List<spiInvitation.invitationDeposits>>(model);
            AttemptResponse attempt = spiInvitationDataModel.SavePayments(deposits);
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
              //InnerException = (attempt.Exception != null ? errorLocation + attempt.Exception.InnerException.ToString() : "")
            });
        }
        public JsonResult SendEmail(Guid invitationID)
        {
            AttemptResponse attempt = new AttemptResponse();
            attempt = spiInvitationDataModel.SendEmailData(invitationID);
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ResponseObjectID = attempt.ObjectID,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                //InnerException = (attempt.Exception != null ? errorLocation + attempt.Exception.InnerException.ToString() : "")
            });
        }
        public JsonResult GetDepositsTotalAmount (Guid invitationID)
        {
            return Json(spiInvitationDataModel.GetInvitationDepositsTotal(invitationID), JsonRequestBehavior.AllowGet);
        } 

    }
}
