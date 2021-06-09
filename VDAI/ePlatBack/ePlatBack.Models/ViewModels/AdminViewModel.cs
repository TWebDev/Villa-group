using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Web.Routing;
using System.Diagnostics;

namespace ePlatBack.Models.ViewModels
{
    public class AdminViewModel
    {
        public WorkGroupSearchModel WorkGroupSearchModel { get; set; }
        public WorkGroupInfoModel WorkGroupInfoModel { get; set; }
        public BookingStatusInfoModel BookingStatusInfoModel { get; set; }
        public LeadTypeInfoModel LeadTypeInfoModel { get; set; }
        public LeadSourceInfoModel LeadSourceInfoModel { get; set; }
        public QualificationRequirementInfoModel QualificationRequirementInfoModel { get; set; }
        public ResortFeeTypeInfoModel ResortFeeTypeInfoModel { get; set; }
        public ReservationStatusInfoModel ReservationStatusInfoModel { get; set; }
        public VerificationAgreementInfoModel VerificationAgreementInfoModel { get; set; }
        public RoleSearchModel RoleSearchModel { get; set; }
        public RoleInfoModel RoleInfoModel { get; set; }
        public ProfileInfoModel ProfileInfoModel { get; set; }
        public UserLogsActivityYYYYMM SearchUserLogActivity { get; set; }
    }

    //public class ProfileInfoModel
    //{
    //    public int ProfileInfo_ProfileID { get; set; }

    //    [Display(Name = "WorkGroup")]
    //    public int ProfileInfo_WorkGroup { get; set; }
    //    public List<SelectListItem> ProfileInfo_DrpWorkGroups
    //    {
    //        get
    //        {
    //            return AdminDataModel.AdminCatalogs.FillDrpWorkGroups();
    //        }
    //    }

    //    [Display(Name = "Role")]
    //    public string ProfileInfo_Role { get; set; }
    //    public List<SelectListItem> ProfileInfo_DrpRoles
    //    {
    //        get
    //        {
    //            return AdminDataModel.AdminCatalogs.FillDrpRoles();
    //        }
    //    }

    //    public int ProfileInfo_ComponentID { get; set; }

    //    public bool ProfileInfo_Create { get; set; }

    //    public bool ProfileInfo_Edit { get; set; }

    //    public bool ProfileInfo_View { get; set; }

    //    public string ProfileInfo_Alias { get; set; }

    //    public string ProfileInfo_Url { get; set; }

    //    public int ProfileInfo_ComponentOrder { get; set; }

    //    public string ProfileInfo_TableName { get; set; }

    //    public string ProfileInfo_TableField { get; set; }

    //    //properties for new component
    //    public string ProfileInfo_Component { get; set; }

    //    public int ProfileInfo_ComponentTypeID { get; set; }

    //    public int ProfileInfo_ParentComponentID { get; set; }

    //    public string ProfileInfo_Description { get; set; }

    //    //--properties for resort assignation
    //    [Display(Name = "Destination")]
    //    public int ProfileInfo_Destination { get; set; }
    //    public List<SelectListItem> ProfileInfo_DrpDestinations
    //    {
    //        get
    //        {
    //            return PlaceDataModel.PlacesCatalogs.FillDrpDestinations();
    //        }
    //    }

    //    [Display(Name = "Resort")]
    //    public string[] ProfileInfo_Resorts { get; set; }
    //    public List<SelectListItem> ProfileInfo_DrpResorts
    //    {
    //        get
    //        {
    //            var list = new List<SelectListItem>();
    //            list.Insert(0, Utils.ListItems.Default("--Select Destination--","0",false));
    //            return list;
    //        }
    //    }
    //}

    public class ProfileInfoModel
    {
        //sysprofile properties
        public int ProfileInfo_ProfileID { get; set; }

        [Display(Name = "WorkGroup")]
        public int ProfileInfo_WorkGroup { get; set; }
        public List<SelectListItem> ProfileInfo_DrpWorkGroups
        {
            get
            {
                var list = AdminDataModel.AdminCatalogs.GetAllWorkGroups();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Role")]
        public Guid ProfileInfo_Role { get; set; }
        public List<SelectListItem> ProfileInfo_DrpRoles
        {
            get
            {
                var list = AdminDataModel.AdminCatalogs.GetAllRoles();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Target WorkGroup")]
        public int ProfileInfo_TargetWorkGroup { get; set; }

        [Display(Name = "Target Role")]
        public Guid ProfileInfo_TargetRole { get; set; }

        public int ProfileInfo_ComponentID { get; set; }
        public bool ProfileInfo_Create { get; set; }
        public bool ProfileInfo_Edit { get; set; }
        public bool ProfileInfo_View { get; set; }
        public int ProfileInfo_ComponentOrder { get; set; }

        //component properties
        public string ProfileInfo_Component { get; set; }
        public int ProfileInfo_ParentComponent { get; set; }
        public int ProfileInfo_ComponentType { get; set; }
        public string ProfileInfo_Url { get; set; }
        public string ProfileInfo_Description { get; set; }
        public string ProfileInfo_TableName { get; set; }
        public string ProfileInfo_FieldName { get; set; }

        //component alias
        public string ProfileInfo_Alias { get; set; }

        //resort assignation properties
        [Display(Name = "Destination")]
        public int ProfileInfo_Destination { get; set; }
        public List<SelectListItem> ProfileInfo_DrpDestinations
        {
            get
            {
                var list = PlaceDataModel.GetAllDestinations();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Resort")]
        public string[] ProfileInfo_Resorts { get; set; }
        public List<SelectListItem> ProfileInfo_DrpResorts
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Insert(0, ListItems.Default("--Select Destination--", "0", false));
                return list;
            }
        }
    }

    public class WorkGroupSearchModel
    {
        [Display(Name = "WorkGroup")]
        public string WorkGroupSearch_WorkGroup { get; set; }
    }

    public class WorkGroupInfoModel
    {
        public int WorkGroupInfo_WorkGroupID { get; set; }

        [Required(ErrorMessage = "WorkGroup is required")]
        [Display(Name = "WorkGroup")]
        public string WorkGroupInfo_WorkGroup { get; set; }
    }

    public class BookingStatusInfoModel
    {
        public int BookingStatusInfo_BookingStatusID { get; set; }

        [Required(ErrorMessage = "Booking Status is required")]
        [Display(Name = "Booking Status")]
        public string BookingStatusInfo_BookingStatus { get; set; }
    }

    public class LeadTypeInfoModel
    {
        public int LeadTypeInfo_LeadTypeID { get; set; }

        [Required(ErrorMessage = "Lead Type is required")]
        [Display(Name = "Lead Type")]
        public string LeadTypeInfo_LeadType { get; set; }
    }

    public class LeadSourceInfoModel
    {
        public int LeadSourceInfo_LeadSourceID { get; set; }

        [Required(ErrorMessage = "Lead Source is required")]
        [Display(Name = "Lead Source")]
        public string LeadSourceInfo_LeadSource { get; set; }

        [Required(ErrorMessage = "Lead Source Initials is required")]
        [Display(Name = "Lead Source Initials")]
        public string LeadSourceInfo_LeadSourceInitials { get; set; }

        public bool LeadSourceInfo_IsActive { get; set; }

        public LeadSourceInfoModel()
        {
            LeadSourceInfo_IsActive = true;
        }
    }

    public class QualificationRequirementInfoModel
    {
        public int QualificationRequirementInfo_QualificationRequirementID { get; set; }

        [Required(ErrorMessage = "Qualification Requirement is required")]
        [Display(Name = "Qualification Requirement")]
        public string QualificationRequirementInfo_QualificationRequirement { get; set; }
    }

    public class ResortFeeTypeInfoModel
    {
        public int ResortFeeTypeInfo_ResortFeeTypeID { get; set; }

        [Required(ErrorMessage = "Resort Fee Type is required")]
        [Display(Name = "Resort Fee Type")]
        public string ResortFeeTypeInfo_ResortFeeType { get; set; }
    }

    public class ReservationStatusInfoModel
    {
        public int ReservationStatusInfo_ReservationStatusID { get; set; }

        [Required(ErrorMessage = "Reservation Status is required")]
        [Display(Name = "Reservation Status")]
        public string ReservationStatusInfo_ReservationStatus { get; set; }
    }

    public class VerificationAgreementInfoModel
    {
        public int VerificationAgreementInfo_VerificationAgreementID { get; set; }

        [Required(ErrorMessage = "Verification Agreement is required")]
        [Display(Name = "Verification Agreement")]
        public string VerificationAgreementInfo_VerificationAgreement { get; set; }
    }

    public class RoleSearchModel
    {
        [Display(Name = "Role")]
        public string RoleSearch_Role { get; set; }
    }

    public class RoleInfoModel
    {
        public string RoleInfo_RoleID { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string RoleInfo_Role { get; set; }
    }

    public class SysComponentsPrivilegesModel
    {
        public int ComponentID { get; set; }
        public string Component { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public bool Required { get; set; }
        public string Alias { get; set; }
        //public string Url { get; set; }
        //public List<WorkGroupsRelatedModel> Alias { get; set; }
        public int ComponentOrder { get; set; }
    }

    public class ProfileModel
    {
        public List<ProfileInfoModel> Profile_ListComponents { get; set; }

        public List<string> Profile_Resorts { get; set; }

        public string Profile_SysWorkGroup { get; set; }

        public string Profile_Role { get; set; }
    }

    //public class ListProfileInfoModel
    //{
    //    public string ProfileInfo_SysworkGroup { get; set; }

    //    public string ProfileInfo_Role { get; set; }

    //    public List<ProfileInfoModel> listProfileInfoModel { get; set; }

    //    public List<string> listProfileResorts { get; set; }
    //}

    public class SysComponentModel
    {
        public int ComponentID { get; set; }
        public string ComponentName { get; set; }
        public int ParentComponentID { get; set; }
        public int ComponentTypeID { get; set; }
        public string ComponentTypeName { get; set; }
        public string Url { get; set; }
        //public bool Create { get; set; }
        //public bool Edit { get; set; }
        //public bool View { get; set; }
        public string Alias { get; set; }
        public int ComponentOrder { get; set; }
        public string TableName { get; set; }
        public string TableField { get; set; }
    }

    public class SysProfilePrivileges
    {
        public List<SelectListItem> ListProfileResorts { get; set; }
        public List<SysComponentsPrivilegesModel> ListSysComponentsPrivileges { get; set; }
    }

    public class UserActivityLogs
    {
        public List<LogsAsignation> asignation { get; set; }

        public class LogsAsignation
        {
            public string logsName { get; set; }
            public string logsValue { get; set; }
            public List<logAsignationlist> loglist { get; set; }
        }
        public class logAsignationlist
        {
            public string listLogsName { get; set; }
            public List<Items> ListLogValues { get; set; }
        }
        public class Items
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }

}
