using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace ePlatBack.Models
{
    public class UserStateModel
    {
        public Guid UserID { get; set; }
        public int State { get; set; }
        //1=Unlocked,2=Locked,3=NotExists,4=InvalidWorkGroup,5=InvalidIP,6=InvalidPwd,7=Signed
        public string Message { get; set; }
        public string Picture { get; set; }
    }

    //**********
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    //**********

    //**********
    public class LogOnModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string WorkGroupID { get; set; }

        public string RoleID { get; set; }

        public string Terminals { get; set; }

        public List<WorkGroupsRolesModel> AvailableWorkGroups { get; set; }
    }
    //**********

    //**********
    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    //**********

    //********** model used to get workgroups/roles of specific user
    //public class WorkGroupsRolesModel
    //{
    //    public int SysWorkGroupID { get; set; }
    //    public string SysWorkGroup { get; set; }
    //    public Guid RoleID { get; set; }
    //    public String RoleName { get; set; }
    //}
    //**********
    public class WorkGroupsRolesModel
    {
        public List<WorkGroupsRelatedModel> WorkGroups { get; set; }
        public List<RolesModel> Roles { get; set; }
    }

    public class MenuComponentsModel
    {
        public int SysComponentID { get; set; }
        public int SysParentComponentID { get; set; }
        public string SysComponentDescription { get; set; }
        public string SysComponentName { get; set; }
        public string SysComponentUrl { get; set; }
        public string SysComponentAlias { get; set; }
        public int SysComponentTypeID { get; set; }
    }

    public class MessageModel
    {
        public string MessageID { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
        public string FromUserID { get; set; }
        public string ToUserID { get; set; }
        public string ReceivedDateTime { get; set; }
        public string ReadDateTime { get; set; }
    }

    public class NotificationModel
    {
        public string NotificationID { get; set; }
        public string Notification { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public bool Important { get; set; }
        public string WorkGroupID { get; set; }
    }

    public class Notification
    {
        public string notificationID { get; set; }
        public string title { get; set; }
        public string itemID { get; set; }
        public string sysItemTypeID { get; set; }
        public string sysItemTypeName { get; set; }
        public string terminalID { get; set; }
        public string terminal { get; set; }
        public string forUserID { get; set; }
        public string forUserFirstName { get; set; }
        public string forUserLastName { get; set; }
        public string description { get; set; }
        public bool read { get; set; }
        public string eventDateTime { get; set; }
        public string eventByUserID { get; set; }
        public string eventUserFirstName { get; set; }
        public string eventUserLastName { get; set; }
        public string deliveredDateTime { get; set; }
        public string readDateTime { get; set; }
        public string url { get; set; }
        public bool action { get; set; }
        public string functionString { get; set; }
        public string button { get; set; }
    }

    public class ActiveUser
    {
        public Guid UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public List<Connection> Connections { get; set; }
    }

    public class ConnectionParams
    {
        public Guid UserID { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string SelectedTerminalIDs { get; set; }
        public int WorkGroupID { get; set; }
        public Guid RoleID { get; set; }
    }

    public class Connection
    {
        public string ConnectionID { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string DateTime { get; set; }
        public string SelectedTerminalIDs { get; set; }
        public string SelectedTerminals { get; set; }
        public int WorkGroupID { get; set; }
    }

    public class SessionDetails
    {
        public string TerminalIDs { get; set; }
        public int? WorkGroupID { get; set; }
        public string WorkGroup { get; set; }
        public Guid? RoleID { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public List<UserTerminal> UserTerminals { get; set; }
        public string Culture { get; set; }
        public Guid UserID { get; set; }
    }

    public class UserTerminal
    {
        public long TerminalID { get; set; }
        public string Terminal { get; set; }
        public int? WorkGroupID { get; set; }
        public string WorkGroup { get; set; }
        public Guid RoleID { get; set; }
        public string Role { get; set; }
        public bool Visible { get; set; }
    }
}
