using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models
{
    //********** page model
    public class AdminModel
    {
        public Guid CurrentUser { get; set; }
        public string SelectedRoleID { get; set; }
        public string SelectedWorkGroupID { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "WorkGroup Name")]
        public string WorkGroupName { get; set; }

        [Display(Name = "Role Name")]
        public string NewRoleName { get; set; }

        [Display(Name = "WorkGroup Name")]
        public string NewWorkGroupName { get; set; }

        public List<RolesModel> RolesList { get; set; }
        //public List<SysComponentModel> sysComponentsList { get; set; }
        public List<ComponentTypesModel> ComponentTypesList { get; set; }
        //public RoleWorkGroupModel roleWorkGroup { get; set; }
    }
    //**********

    //********** model used to get data of components
    //public class SysComponentModel
    //{
    //    public int ComponentID { get; set; }
    //    public string ComponentName { get; set; }
    //    public int ParentComponentID { get; set; }
    //    public int ComponentTypeID { get; set; }
    //    public string ComponentTypeName { get; set; }
    //    public string Url { get; set; }
    //    //public bool Create { get; set; }
    //    //public bool Edit { get; set; }
    //    //public bool View { get; set; }
    //    //public string Alias { get; set; }
    //    public int ComponentOrder { get; set; }
    //}
    //**********

    public class SysComponentsPrivilegesModela
    {
        public int ComponentID { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public string Alias { get; set; }
        //public string Url { get; set; }
        //public List<WorkGroupsRelatedModel> Alias { get; set; }
        public int ComponentOrder { get; set; }
    }

    //********** model used to get all roles from database
    public class RolesModel
    {
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }
    }
    //**********

    public class WorkGroupsViewModel
    {
        public Guid UserID { get; set; }
        public int UserState { get; set; } //1=Unlocked,2=Locked,3=NotExists
        public string Picture { get; set; }
        public List<WorkGroupsRelatedModel> WorkGroups { get; set; }
    }

    //********** model used to get workgroups from database
    public class WorkGroupsRelatedModel
    {
        public int WorkGroupID { get; set; }
        public string WorkGroupName{ get; set; }
    }
    //**********

    public class WorkGroupBookingStatusModel
    {
        public int SysWorkGroupBookingStatusID { get; set; }
        public int BookingStatusID { get; set; }
        public int OrderIndex { get; set; }
    }

    public class WorkGroupItemsModel
    {
        public int MainItemID { get; set; }
        public int SecondItemID { get; set; }
    }

    public class WorkGroupMarketingSourcesModel
    {
        public int SysWorkGroupMarketingSourceID { get; set; }
        public int MarketingSourceID { get; set; }
    }

    public class WorkGroupResortFeeTypesModel
    {
        public int SysWorkGroupResortFeeTypeID { get; set; }
        public int ResortFeeTypeID { get; set; }
    }

    public class WorkGroupReservationStatusModel
    {
        public int ReservationStatusSysWorkGroupID { get; set; }
        public int ReservationStatusID { get; set; }
    }

    public class WorkGroupsComponentsModel
    {
        public int SysWorkGroupID { get; set; }
        public string SysWorkGroup { get; set; }
        public List<WorkGroupItemsModel> BookingStatus { get; set; }
        public int[] LeadTypes { get; set; }
        public int[] MarketingSources { get; set; }
        public int[] QualificationRequirements { get; set; }
        public int[] ResortFeeTypes { get; set; }
        public int[] ReservationStatus { get; set; }
        public int[] VerificationAgreements { get; set; }
    }

    //********** model used to get component types from database
    public class ComponentTypesModel
    {
        public int ComponentTypeID { get; set; }
        public string ComponentType { get; set; }
    }
    //**********

    //********** model used to save new component to database
    public class NewComponentModel
    {
        public int SysComponentID { get; set; }
        public Guid RoleID { get; set; }
        public int SysWorkGroupID { get; set; }
        public string SysComponent { get; set; }
        public int SysParentComponentID { get; set; }
        public int SysComponentTypeID { get; set; }
        public string Description { get; set; }
        public int SysComponentOrder { get; set; }
    }
    //**********

    //********** model used to save/update component profiles
    public class SysProfileModel
    {
        public int SysProfileID { get; set; }
        public Guid RoleID { get; set; }
        public int SysWorkGroupID { get; set; }
        public int SysComponentID { get; set; }
        public bool Create_ { get; set; }
        public bool Edit_ { get; set; }
        public bool View_ { get; set; }
        public string Alias { get; set; }
        public string Url { get; set; }
        public int SysComponentOrder { get; set; }
    }
    //**********

    public class SysProfileContainerModel
    {
        public List<SysProfileModel> Container { get; set; }
    }
}