using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.ViewModels
{
    public class UserViewModel
    {
        public List<UserSearchItemModel> SearchResults { get; set; }
        public UserSearchModel UserSearch { get; set; }
        public UserInfoModel UserInfo { get; set; }
        public UserSupervisorSearchModel UserSupervisorSearch { get; set; }
        public UserSupervisorOfModel UserSupervisorOf { get; set; }
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public SystemUsers UsersCharts { get; set; }
        //public List<SysComponentsPrivilegesModel> Privileges
        //{
        //    get
        //    {
        //        return AdminDataModel.GetViewPrivileges(36);//11276
        //    }
        //}
    }

    public class UserRequestsViewModel
    {
        public List<UserRequestItem> Requests { get; set; }
        public string RequestsJson { get; set; }

        public class UserRequestItem
        {
            public Guid UserRequestID { get; set; }
            public int Attachements { get; set; }
            public string Users { get; set; }
            public DateTime DateSavedDT { get; set; }
            public string DateSaved { get; set; }
            public string DateDocsChecked { get; set; }
            public string DateApproved { get; set; }
            public string DateDelivered { get; set; }
            public string NotifyToEmail { get; set; }
        }
    }

    public class UserRequest
    {
        public Guid UserRequestID { get; set; }
        public List<UserRequestInfo> Users { get; set; }
        public string System { get; set; }
        public long[] Destinations { get; set; }
        public string DestinationsNames { get; set; }
        public long[] Terminals { get; set; }
        public string TerminalsNames { get; set; }
        public string RequestedBy { get; set; }
        public string NotifyTo { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentName { get; set; }
        public string Saved { get; set; }
        public string Checked { get; set; }
        public string Approved { get; set; }
        public string Delivered { get; set; }
        public bool EditPermissions { get; set; }
        public string Notes { get; set; }

        public class UserRequestInfo
        {
            public Guid? UserTemporalID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string JobPosition { get; set; }
            public Guid? BasedOn { get; set; }
            public string BasedOnUser { get; set; }
            public string DirectSupervisor { get; set; }
            public string DocumentPath { get; set; }
            public string DocumentName { get; set; }
        }
    }

    public class ActiveUsers
    {
        public string Users { get; set; }

        public class ActiveUser
        {
            public string System { get; set; }
            public Guid UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string LastActivityDate { get; set; }
            public int? DaysToLock { get; set; }
            public List<string> Terminals { get; set; }
        }
    }

    public class UserSearchModel
    {
        [Display(Name = "First Name")]
        public string Search_FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string Search_LastName { get; set; }

        [Display(Name = "Username")]
        public string Search_UserName { get; set; }

        [Display(Name = "Job Position")]
        public int?[] Search_JobPositions { get; set; }

        public List<SelectListItem> JobPositions
        {
            get
            {
                var list = UserDataModel.UserCatalogs.GetJobPositionsList();
                return list;
            }
        }

        [Display(Name = "Destinations")]
        public long?[] Search_Destinations { get; set; }

        [Display(Name = "Terminal")]
        public long[] Search_Terminal { get; set; }
        public List<SelectListItem> Search_DrpSelectedTerminals
        {
            get
            {
                return TerminalDataModel.GetActiveTerminalsList();
            }
        }

        public List<SelectListItem> Destinations
        {
            get
            {
                var list = UserDataModel.UserCatalogs.GetDestinationsList();
                return list;
            }
        }

        [Display(Name = "Profile")]
        public string[] Search_Profile { get; set; }
        public List<SelectListItem> Search_DrpProfiles
        {
            get
            {
                var list = UserDataModel.UserCatalogs.FillDrpProfilesInUse();
                return list;
            }
        }
        [Display(Name = "Company")]
        public string[] Search_Company { get; set; }
        public List<SelectListItem> SearchCompanyList
        {
            get
            {
                var list = UserDataModel.UserCatalogs.CompaniesList();
                return list;
            }
        }

        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        /// </summary>
        //public List<SysComponentsPrivilegesModel> Privileges
        //{
        //    get
        //    {
        //        return AdminDataModel.GetViewPrivileges(11276);// fdsuserManagement
        //    }
        //}

    }//busqueda

    public class UserSearchItemModel
    {
        public Guid UserID { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Job Position")]
        public string JobPosition { get; set; }

        [Display(Name = "Work Group")]
        public string WorkGroup { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Is Locked")]
        public bool IsLocked { get; set; }

        [Display(Name = "Last Activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }  
      
        [Display(Name = "Departament")]
        public string Departament { get; set; }  
    
        [Display(Name = "Departament Phone")]
        public string DepartamentPhone { get; set; }

        [Display(Name = "EXT")]
        public string PhoneEXT { get; set; }

        [Display(Name = "Personal Phone Number")]
        public string PersonalPhoneNumber { get; set; }

        [Display(Name = "Language")]
        public string Language { get; set; }

        [Display(Name = "OPC")]
        public string OPC { get; set; }

        //
        //public List<SysComponentsPrivilegesModel> Privileges
        //{
        //    get
        //    {
        //        return AdminDataModel.GetViewPrivileges(11278);// table Search User Result
        //    }
        //}
    }//tabla

    public class UserInfoModel
    {
        public Guid? UserInfo_UserID { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "User Name")]
        public string UserInfo_UserName { get; set; }

        [Display(Name = "First Name")]
        public string UserInfo_FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string UserInfo_LastName { get; set; }

        [Display(Name = "SPI Username")]
        public string UserInfo_SPIUserName { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "Min 6 chars", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string UserInfo_Password { get; set; }

        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("UserInfo_Password", ErrorMessage = "Not Match")]
        [Display(Name = "Confirm Password")]
        public string UserInfo_ConfirmPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Address")]
        [Display(Name = "E-Mail")]
        public string UserInfo_Email { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Is Active")]
        public bool UserInfo_IsApproved { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Is Locked Out")]
        public bool UserInfo_IsLockedOut { get; set; }

        [Display(Name = "Manage Reservations")]
        public bool UserInfo_ManageReservations { get; set; }

        [Display(Name = "Manage Services")]
        public bool UserInfo_ManageServices { get; set; }

        [Display(Name = "Supervisor")]
        [Required(ErrorMessage = "Supervisor is Required")]
        public string[] UserInfo_Supervisors { get; set; }
        public List<SelectListItem> SupervisorsList
        {
            get
            {
                //var list = new List<SelectListItem>();
                //list.Insert(0, ListItems.Default());
                //return list; 
                return UserDataModel.UserCatalogs.GetSupervisorsList();
            }
        }
        public List<KeyValuePair<Guid, string>> UserInfo_SupervisorsIDs { get; set; }

        //[Required(ErrorMessage = "Supervisor is required")]
        //public string UserInfo_Supervisor { get; set; }

        [Display(Name = "Job Positions")]
        //[Required(ErrorMessage = "Job Position is Required")]
        public string[] UserInfo_JobPositions { get; set; }
        public List<SelectListItem> JobPositionsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetJobPositionsList();
            }
        }
        public List<KeyValuePair<int, string>> UserInfo_JobPositionsIDs { get; set; }

        //[Required(ErrorMessage = "Job Position is required")]
        //public string UserInfo_JobPosition { get; set; }

        [Display(Name = "Destinations")]
        public string[] UserInfo_Destinations { get; set; }
        public List<SelectListItem> DestinationsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetDestinationsList();
            }
        }
        public List<KeyValuePair<long, string>> UserInfo_DestinationsIDs { get; set; }

        //[Required(ErrorMessage = "Destination is required")]
        //public string UserInfo_Destination { get; set; }

        [Display(Name = "WorkGroup")]
        public string UserInfo_WorkGroups { get; set; }
        public List<SelectListItem> WorkGroupsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetWorkGroupsList();
            }
        }
        public List<KeyValuePair<string, string>> UserInfo_WorkGroupsRolesIDs { get; set; }

       // [Required(ErrorMessage = "WorkGroup and Role are required")]
        public string UserInfo_WorkGroup { get; set; }

        [Display(Name = "Role")]
        public string UserInfo_Roles { get; set; }
        public List<SelectListItem> RolesList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetRolesList();
            }
        }
        public List<KeyValuePair<Guid, string>> UserInfo_RolesIDs { get; set; }

        [Display(Name = "Terminal(s)")]
       // [Required(ErrorMessage = "Terminal is Required")] 
        public string[] UserInfo_Terminals { get; set; }
        public List<SelectListItem> TerminalsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetTerminalsList();
            }
        }
        public List<KeyValuePair<long, string>> UserInfo_TerminalsIDs { get; set; }

        public string UserInfo_LastDateActivity { get; set; }

        public UserInfoModel()
        {
            UserInfo_IsApproved = true;
            UserInfo_IsLockedOut = false;
        }
        [Display(Name = "Company")]
        public string UserInfo_Company { get; set; }
       // [Required(ErrorMessage = "Departament Phone is Required")]   
        [Display(Name = "Departament Phone")]
        public string UserInfo_DepartamentPhone { get; set; }
        [Display(Name = "EXT")]
        public string UserInfo_PhoneEXT { get; set; }
       // [Required(ErrorMessage = "Personal Phone Number is required")]
        [Display(Name = "Personal Phone Number")]
        public string UserInfo_PersonalPhoneNumber { get; set; }
        [Display(Name = "Language")]
        public string UserInfo_Language { get; set; }
        public List<SelectListItem> LanguageList
        {
            get
            {
               var list = MasterChartDataModel.LeadsCatalogs.FillDrpCultures();
               list.Insert(0, ListItems.Default());
               return list;
            }   
        }
        public List<SelectListItem> CompanyList
        {
            get
            {
                var list = UserDataModel.UserCatalogs.CompaniesList();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        [Display(Name = "Departament")]
        public string UserInfo_Departament { get; set; }
        public List<SelectListItem> DepartamentList
        {
            get
            {
                var list = new List<SelectListItem>(); 
                list.Insert(0, ListItems.Default());
                return list; 
            }
        }
        [Display(Name = "OPC")]
        public string UserInfo_OPC {get;set;}
        public List<SelectListItem> ListOPC
        {
            get 
            {
                var list = new List<SelectListItem>();
                list.Insert(0,ListItems.Default());
                return list;
            }
        }
      
        /// </summary>
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }

        //public List<SysComponentsPrivilegesModel> Privileges
        //{
        //    get
        //    {
        //        return AdminDataModel.GetViewPrivileges(11279);//fds UserInfo Model
        //    }
        //}       
    }//save,update,delete

    public class UserProfileModel
    {
        public Guid UserID { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "SPI Username")]
        public string SPIUserName { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "Min 6 chars", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Not Match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Address")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Display(Name = "Is Active")]
        public bool IsApproved { get; set; }

        [Display(Name = "Is Locked Out")]
        public bool IsLockedOut { get; set; }

        public Guid[] SupervisorIDsArray { get; set; }
        public List<SelectListItem> SupervisorsList
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Insert(0, ListItems.Default());
                return list;
               // return UserDataModel.UserCatalogs.GetSupervisorsList(string);
            }
        }

        public int[] JobPositionIDsArray { get; set; }
        public List<SelectListItem> JobPositionsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetJobPositionsList();
            }
        }

        public long[] DestinationIDsArray { get; set; }
        public List<SelectListItem> DestinationsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetDestinationsList();
            }
        }

        public string[] WorkGroupsRolesArray { get; set; }
        public List<SelectListItem> WorkGroupsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetWorkGroupsList();
            }
        }

        public List<SelectListItem> RolesList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetRolesList();
            }
        }

        public long[] TerminalsArray { get; set; }
        public List<SelectListItem> TerminalsList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetTerminalsList();
            }
        }
 
        //public List<SysComponentsPrivilegesModel> Privileges
        //{
        //    get
        //    {
        //        return AdminDataModel.GetViewPrivileges(36);
        //    }
        //}

    }


    public class UserSupervisorSearchModel
    {
        
        public Guid UserID { get; set; }
        [Display(Name = "First Name")]
        public string Search_FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string Search_LastName { get; set; }

        [Display(Name = "Username")]
        public string Search_UserName { get; set; }

        [Display(Name = "Job Position")]
        public string Search_JobPosition { get; set; }
        
        [Display(Name = "Job Position")]
        public int?[] Search_JobPositions { get; set; }

        public List<SelectListItem> JobPositions
        {
            get
            {
                var list = UserDataModel.UserCatalogs.GetJobPositionsList();
                return list;
            }
        }


        [Display(Name = "Terminal")]
        public long Search_Terminal { get; set; }
        public List<SelectListItem> Search_DrpSelectedTerminals
        {
            get
            {
                return TerminalDataModel.GetActiveTerminalsList();
            }
        }



        [Display(Name = "WorkGroup")]
        public long Search_Workgroup { get; set; }

        [Display(Name = "WorkGroup")]
        public String Search_WorkgroupValue { get; set; }
        public List<SelectListItem> Search_DrpSelectedWorkGroups
        {
            get
            {
                return UserDataModel.UserCatalogs.GetWorkGroupsList();
            }
        }


        [Display(Name = "Roles")]
        public string Search_Roles { get; set; }

        public List<SelectListItem> RolesList
        {
            get
            {
                return UserDataModel.UserCatalogs.GetRolesList();
            }
        }

        public List<SelectListItem> Destinations
        {
            get
            {
                var list = UserDataModel.UserCatalogs.GetDestinationsList();
                return list;
            }
        }

        public List<SelectListItem> Search_DrpProfiles
        {
            get
            {
                var list = UserDataModel.UserCatalogs.FillDrpProfilesInUse();
                return list;
            }
        }
    }


    public class UserSupervisorOfModel
    {
        public Guid UserID { get; set; }

        [Display(Name = "First Name")]
        public string Search_FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string Search_LastName { get; set; }

        [Display(Name = "Username")]
        public string Search_UserName { get; set; }

        [Display(Name = "Job Position")]
        public string Search_JobPosition { get; set; }
    }

    public class UsersToWarn
    {
        public string user { get; set; }
        public string email { get; set; }
        public string emailBody { get; set; }
        public string lastDateLog { get; set; }
        public string daysBeforeLock { get; set; }
        public string system { get; set; }

    }

    public class SystemUsers
    {
        public  List<OrganizationChartsModel> SystemUserList { get; set; }

        public class SearchSubordinates
        {
            [Display(Name = "Users")]
            public Guid? userID { get; set; }
            public List<SelectListItem> userList
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Required]
            [Display(Name = "System")]
            public string system { get; set; }
            public List<SelectListItem> systemList
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = "-Select One-",
                        Value = "0"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "ePlat",
                        Value = "ePlat"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "ePlatform",
                        Value = "ePlatform"
                    });
                    return list;
                }
            }
        }

        public class SystemUser
        {
           
            public Guid? userID { get; set; }
            [Display(Name = "User")]
            public string userName { get; set; }
            [Display(Name = "First Name")]
            public string firstName { get; set; }
            [Display(Name = "Last Name")]
            public string lastName { get; set; }
            [Display(Name = "Locked")]
            public bool locked { get; set; }
            [Display(Name = "Email")]
            public string email { get; set; }
            [Display(Name = "System")]
            public string system { get; set; }
            [Display(Name = "Job Position")]
            public string jobPosition { get; set; }
            [Display(Name = "Supervisor")]
            public Guid? supervisorID { get; set; }
            public string supervisorName { get; set; }
            public DateTime lastActivityDate { get; set; }
            public long supervisorAgentID { get; set; }
            public List<SelectListItem> supervisorList
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
        }

        public class OrganizationChartsModel
        {
            public Guid v { get; set; }//supervisor
            public string f { get; set; }//
            public Guid? nodoPadre { get; set; }
            public string comentario { get; set; }
        }

        public class SystemUsersModel
        {
            public List<OrganizationChartsModel> OrgCharts { get; set; }
            public List<SystemUser> userList { get; set; }
        }
    }
}
