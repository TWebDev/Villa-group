using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Controllers.Settings
{
    public class AdminController : Controller
    {
        //
        // GET: /Profiles/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                AdminViewModel avm = new AdminViewModel();
                ViewData.Model = new AdminViewModel
                {
                    WorkGroupInfoModel = new WorkGroupInfoModel(),
                    WorkGroupSearchModel = new WorkGroupSearchModel(),
                    BookingStatusInfoModel = new BookingStatusInfoModel(),
                    LeadTypeInfoModel = new LeadTypeInfoModel(),
                    LeadSourceInfoModel = new LeadSourceInfoModel(),
                    QualificationRequirementInfoModel = new QualificationRequirementInfoModel(),
                    ResortFeeTypeInfoModel = new ResortFeeTypeInfoModel(),
                    ReservationStatusInfoModel = new ReservationStatusInfoModel(),
                    VerificationAgreementInfoModel = new VerificationAgreementInfoModel(),
                    RoleInfoModel = new RoleInfoModel(),
                    RoleSearchModel = new RoleSearchModel(),
                    ProfileInfoModel = new ProfileInfoModel(),
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult GetComponentsTree()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetComponentsTree());
        }

        public JsonResult GetPrivileges(string roleID, int workgroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetPrivileges(roleID, workgroupID));
        }

        public JsonResult GetComponentsTypes()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetComponentsTypes());
        }
        
        [Authorize]
        public JsonResult SaveSysProfile(ProfileModel model) { 
        AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveSysProfile(model);
            string errorLocation = "";

            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        
        //public JsonResult SaveProfiles(ListProfileInfoModel list)
        //{
        //    AdminDataModel adm = new AdminDataModel();
        //    AttemptResponse attempt = adm.SaveProfiles(list);
        //    string errorLocation = "";
        //    if (attempt.Exception != null)
        //    {
        //        errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
        //    }

        //    return Json(new
        //    {
        //        ResponseType = attempt.Type,
        //        ItemID = attempt.ObjectID,
        //        ResponseMessage = attempt.Message,
        //        ExceptionMessage = Debugging.GetMessage(attempt.Exception),
        //        InnerException = Debugging.GetInnerException(attempt.Exception)
        //    });
        //}

        public JsonResult UpdateComponentName(int componentID, string componentName)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.UpdateComponentName(componentID, componentName);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult RemoveComponent(int componentID)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.RemoveComponent(componentID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        
        [Authorize]
        public JsonResult SaveComponent(ProfileInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveComponent(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetTables()
        {
            return Json(AdminDataModel.AdminCatalogs.FillDrpTables());
        }

        public JsonResult GetFields(string tableName)
        {
            return Json(AdminDataModel.AdminCatalogs.FillDrpFields(tableName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDDLData(string path, string id)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetDDLData(path, id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeComponentParent(int componentID, int parentComponentID)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.ChangeComponentParent(componentID, parentComponentID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetComponentInfo(int componentID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetComponentInfo(componentID));
        }

        //roles

        public JsonResult SearchRoles(string role)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.SearchRoles(role));
        }

        [Authorize]
        public JsonResult SaveRole(RoleInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveRole(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetRole(string roleID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetRole(roleID));
        }

        [Authorize]
        public JsonResult DeleteRole(string roleID)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.DeleteRole(roleID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        [Authorize]
        //workgroups
        public JsonResult AssignWorkGroupToUser(int workgroupID, string roleID)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.AssignWorkGroupToUser(workgroupID, roleID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }
        
        public JsonResult SearchWorkGroups(string workgroup)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.SearchWorkGroups(workgroup));
        }

        [Authorize]
        public JsonResult SaveWorkGroup(WorkGroupInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveWorkGroup(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetWorkGroup(int workgroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetWorkGroup(workgroupID));
        }

        [Authorize]
        public JsonResult DeleteWorkGroup(int workgroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.DeleteWorkGroup(workgroupID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetAllBookingStatus()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllBookingStatus());
        }

        public JsonResult GetActiveBookingStatusPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveBookingStatusPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveBookingStatusPerWorkGroup(int workgroupID, int[] bookingStatus)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveBookingStatusPerWorkGroup(workgroupID, bookingStatus);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveBookingStatus(BookingStatusInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveBookingStatus(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetAllLeadTypes()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllLeadTypes());
        }

        public JsonResult GetActiveLeadTypesPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveLeadTypesPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveLeadTypesPerWorkGroup(int workgroupID, int[] leadTypes)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveLeadTypesPerWorkGroup(workgroupID, leadTypes);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveLeadType(LeadTypeInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveLeadType(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetAllLeadSources()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllLeadSources());
        }

        public JsonResult GetActiveLeadSourcesPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveLeadSourcesPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveLeadSourcesPerWorkGroup(int workgroupID, int[] leadSources)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveLeadSourcesPerWorkGroup(workgroupID, leadSources);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveLeadSource(LeadSourceInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveLeadSource(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetAllQualificationRequirements()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllQualificationRequirements());
        }

        public JsonResult GetActiveQualificationRequirementsPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveQualificationRequirementsPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveQualificationRequirementsPerWorkGroup(int workgroupID, int[] qualificationRequirements)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveQualificationRequirementsPerWorkGroup(workgroupID, qualificationRequirements);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveQualificationRequirement(QualificationRequirementInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveQualificationRequirement(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetAllResortFeeTypes()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllResortFeeTypes());
        }

        public JsonResult GetActiveResortFeeTypesPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveResortFeeTypesPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveResortFeeTypesPerWorkGroup(int workgroupID, int[] resortFeeTypes)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveResortFeeTypesPerWorkGroup(workgroupID, resortFeeTypes);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveResortFeeType(ResortFeeTypeInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveResortFeeType(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult GetAllReservationStatus()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllReservationStatus());
        }

        public JsonResult GetActiveReservationStatusPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveReservationStatusPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveReservationStatusPerWorkGroup(int workgroupID, int[] reservationStatus)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveReservationStatusPerWorkGroup(workgroupID, reservationStatus);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveReservationStatus(ReservationStatusInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveReservationStatus(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        /**/
        public JsonResult GetAllVerificationAgreements()
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetAllVerificationAgreements());
        }

        public JsonResult GetActiveVerificationAgreementsPerWorkGroup(int sysWorkGroupID)
        {
            AdminDataModel adm = new AdminDataModel();
            return Json(adm.GetActiveVerificationAgreementsPerWorkGroup(sysWorkGroupID));
        }

        public JsonResult SaveVerificationAgreementsPerWorkGroup(int workgroupID, int[] verificationAgreements)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveVerificationAgreementsPerWorkGroup(workgroupID, verificationAgreements);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult SaveVerificationAgreement(VerificationAgreementInfoModel model)
        {
            AdminDataModel adm = new AdminDataModel();
            AttemptResponse attempt = adm.SaveVerificationAgreement(model);
            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ItemID = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

    }
}
