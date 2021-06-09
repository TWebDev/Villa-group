
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using System.Text;
using System.Text.RegularExpressions;
using ePlatBack.Models.eplatformDataModel;

namespace ePlatBack.Models.DataModels
{
    public class UserDataModel
    {
        public static UserSession session = new UserSession();
        public class UserCatalogs
        {
            public static List<SelectListItem> GetJobPositionsList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> jp = new List<SelectListItem>();
                foreach (var position in db.tblJobPositions.OrderBy(m => m.jobPosition))
                    jp.Add(new SelectListItem()
                    {
                        Value = position.jobPositionID.ToString(),
                        Text = position.jobPosition.ToString(),
                    });
                return jp;
            }

            public static List<SelectListItem> GetDestinationsList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> d = new List<SelectListItem>();
                foreach (var destination in db.tblDestinations)
                    d.Add(new SelectListItem()
                    {
                        Value = destination.destinationID.ToString(),
                        Text = destination.destination.ToString(),
                    });
                return d;
            }

            public static List<SelectListItem> GetSupervisorsList()
            {
                ePlatEntities db = new ePlatEntities();

                int workGroupID = (int)session.WorkGroupID;
                List<SelectListItem> list = new List<SelectListItem>();
                var query = (from us in db.tblUsers_SysWorkGroups
                             join mm in db.aspnet_Membership on us.userID equals mm.UserId
                             where us.sysWorkGroupID == workGroupID
                             && mm.IsApproved == true
                             //&& mm.IsLockedOut == false
                             select new
                             {
                                 id = us.userID,
                                 fn = mm.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName,
                                 ln = mm.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName,
                                 fl = mm.IsApproved,
                                 mm.IsLockedOut
                             }).Distinct();
                foreach (var u in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = u.id.ToString(),
                        Text = u.fn.ToString() + " " + u.ln.ToString() + (u.IsLockedOut ? " -Locked" : "")
                    });
                }
                list.Sort((x, y) => x.Text.CompareTo(y.Text));
                return list;
            }

            public static List<SelectListItem> GetSupervisorsListChange(string WorkGID)
            {
                ePlatEntities db = new ePlatEntities();

                int workGroupID = int.Parse(WorkGID);
                List<SelectListItem> list = new List<SelectListItem>();
                var query = (from us in db.tblUsers_SysWorkGroups
                             join mm in db.aspnet_Membership on us.userID equals mm.UserId
                             where us.sysWorkGroupID == workGroupID
                             && mm.IsApproved == true
                             //&& mm.IsLockedOut == false
                             select new
                             {
                                 id = us.userID,
                                 fn = mm.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName,
                                 ln = mm.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName,
                                 fl = mm.IsApproved,
                                 mm.IsLockedOut
                             }).Distinct();
                foreach (var u in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = u.id.ToString(),
                        Text = u.fn.ToString() + " " + u.ln.ToString() + (u.IsLockedOut ? " -Locked" : "")
                    });
                }
                list.Sort((x, y) => x.Text.CompareTo(y.Text));
                return list;
            }

            public static List<SelectListItem> GetUserWorkGroups()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> wg = new List<SelectListItem>();
                Guid userid = session.UserID;
                var userWG = from g in db.tblUsers_SysWorkGroups
                             where g.userID == userid
                             select new
                             {
                                 g.sysWorkGroupID,
                                 g.tblSysWorkGroups.sysWorkGroup
                             };
                foreach (var workgroup in userWG)
                    wg.Add(new SelectListItem()
                    {
                        Value = workgroup.sysWorkGroupID.ToString(),
                        Text = workgroup.sysWorkGroup,
                    });
                return wg;
            }

            public static List<SelectListItem> GetWorkGroupsList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> wg = new List<SelectListItem>();
                wg.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var workgroup in db.tblSysWorkGroups.OrderBy(x => x.sysWorkGroup))
                    wg.Add(new SelectListItem()
                    {
                        Value = workgroup.sysWorkGroupID.ToString(),
                        Text = workgroup.sysWorkGroup.ToString(),
                    });
                return wg;
            }

            public static List<SelectListItem> GetRolesList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> r = new List<SelectListItem>();
                r.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var role in db.aspnet_Roles.OrderBy(x => x.RoleName))
                    r.Add(new SelectListItem()
                    {
                        Value = role.RoleId.ToString(),
                        Text = role.RoleName.ToString(),
                    });
                //uvm.DrpRoles = r;
                return r;
            }

            public static List<SelectListItem> GetTerminalsList()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> t = new List<SelectListItem>();
                foreach (var terminal in db.tblTerminals)
                    t.Add(new SelectListItem()
                    {
                        Value = terminal.terminalID.ToString(),
                        Text = terminal.terminal.ToString(),
                    });
                return t;
            }

            public static List<SelectListItem> FillDrpProfilesInUse()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var i in db.tblUsers_SysWorkGroups.Select(m => new { m.roleID, m.sysWorkGroupID, m.aspnet_Roles.RoleName, m.tblSysWorkGroups.sysWorkGroup }).Distinct().OrderBy(m => m.sysWorkGroup).ThenBy(m => m.RoleName))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.sysWorkGroupID.ToString() + "|" + i.roleID.ToString(),
                        Text = i.sysWorkGroup + " - " + i.RoleName
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpAdministratorUsersInWorkGroup()
            {
                int? wgid = session.WorkGroupID;
                List<SelectListItem> users = new List<SelectListItem>();
                if (wgid != null)
                {

                    ePlatEntities db = new ePlatEntities();
                    var UsersInGroup = from u in db.tblUsers_SysWorkGroups
                                       where u.sysWorkGroupID == wgid
                                       && u.aspnet_Users1.aspnet_Membership.IsApproved == true
                                       && !u.aspnet_Users1.tblUserProfiles.FirstOrDefault().firstName.Contains("test")
                                       && u.aspnet_Roles.RoleName.Contains("dministrator")
                                       select new
                                       {
                                           u.userID,
                                           u.aspnet_Users1.tblUserProfiles.FirstOrDefault().firstName,
                                           u.aspnet_Users1.tblUserProfiles.FirstOrDefault().lastName
                                       };

                    foreach (var user in UsersInGroup.OrderBy(u => u.firstName))
                    {
                        users.Add(new SelectListItem()
                        {
                            Value = user.userID.ToString(),
                            Text = user.firstName + " " + user.lastName
                        });
                    }
                }
                return users;
            }

            public static List<SelectListItem> FillDrpUsersInWorkGroup(bool showLocked = false)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> users = new List<SelectListItem>();

                if (session.WorkGroupID != null)
                {
                    var query = from u in db.tblUsers_SysWorkGroups
                                join m in db.aspnet_Membership on u.userID equals m.UserId
                                join p in db.tblUserProfiles on u.userID equals p.userID
                                where u.sysWorkGroupID == session.WorkGroupID
                                && (showLocked ? m.IsLockedOut != null : m.IsLockedOut != true)
                                && !p.firstName.Contains("test")
                                select new
                                {
                                    u.userID,
                                    p.firstName,
                                    p.lastName,
                                    m.IsLockedOut
                                };

                    foreach (var i in query.OrderBy(m => m.firstName))
                    {
                        users.Add(new SelectListItem()
                        {
                            Value = i.userID.ToString(),
                            Text = i.firstName + " " + i.lastName + (showLocked && i.IsLockedOut ? " - Locked" : "")
                        });
                    }

                    return users;

                }

                return users;
            }

            public static List<SelectListItem> CompaniesList()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var companies = (from c in db.tblTerminals_Companies
                                join company in db.tblCompanies on c.companyID equals company.companyID
                                where terminals.Contains(c.terminalID)
                                orderby company.company
                                select new
                                {
                                    c.companyID,
                                    company.company
                                }).Distinct();


                //foreach (var i in companies.Distinct().OrderBy(x => x.company))
                foreach(var i in companies)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.companyID.ToString(),
                        Text = i.company
                    });
                }
                list = list.OrderByDescending(m => m.Text).ToList();
                list.Insert(0, ListItems.Default());
                return list;
            }

            public static List<SelectListItem> DepartamentList(string companyID)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var _companyID = int.Parse(companyID);

                var companies = from c in db.tblCompanyDepartments
                                where c.companyID == _companyID
                                orderby c.tblCompanies.company
                                select c;

                foreach (var i in companies.Distinct().OrderBy(x => x.department))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.departmentID.ToString(),
                        Text = i.department
                    });
                }
                list.Insert(0, ListItems.Default());
                return list;
            }//departament
            public static List<SelectListItem> GetActivieTerminalsAdmin()
            {
                var flag = false;
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                long[] currentTerminals = session.Terminals != "" ? (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray() : new long[] { };
                var Terminals = db.tblTerminals.Where(m => m.terminalID != null).OrderBy(m => m.terminal);

                flag = GeneralFunctions.IsUserInRole("Administrator", session.UserID);
                if (flag == false)
                {
                    Terminals = db.tblTerminals.Where(m => currentTerminals.Contains(m.terminalID)).OrderBy(m => m.terminal);
                }
                foreach (var i in Terminals)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.terminalID.ToString(),
                        Text = i.terminal
                    });
                }
                return list;
            }
        }

        ePlatEntities db = new ePlatEntities();
        Guid CurrentUserID = session.UserID;

        //Query Methods
        //public IEnumerable<UserSearchItemModel> SearchUserss(UserSearchModel usm)
        //{
        //    var searchResults = from us in db.aspnet_Users
        //                        join sa in db.tblSupervisors_Agents on us.UserId equals sa.agentUserID
        //                        where (sa.supervisorUserID == CurrentUserID
        //                        && (usm.Search_FirstName == null || us.tblUserProfiles.FirstOrDefault().firstName.Contains(usm.Search_FirstName))
        //                        && (usm.Search_LastName == null || us.tblUserProfiles.FirstOrDefault().lastName.Contains(usm.Search_LastName))
        //                        && (usm.Search_UserName == null || us.UserName.Contains(usm.Search_UserName))
        //                        && (usm.Search_JobPositions == null || us.tblUsers_JobPositions.FirstOrDefault().jobPositionID == usm.Search_JobPositions)
        //                        && (usm.Search_DestinationID == 0 || us.tblUsers_Destinations.FirstOrDefault().destinationID == usm.Search_DestinationID))
        //                        || (us.UserId == CurrentUserID && us.aspnet_Roles.FirstOrDefault().RoleName.Contains("Administrator"))
        //                        orderby us.aspnet_Membership.IsApproved
        //                        select new UserSearchItemModel()
        //                        {
        //                            UserID = us.UserId,
        //                            UserName = us.UserName,
        //                            FirstName = us.tblUserProfiles.FirstOrDefault().firstName,
        //                            LastName = us.tblUserProfiles.FirstOrDefault().lastName,
        //                            JobPosition = us.tblUsers_JobPositions.FirstOrDefault(x => x.userID == sa.agentUserID && x.toDate == null).tblJobPositions.jobPosition,
        //                            WorkGroup = us.tblUsers_SysWorkGroups1.FirstOrDefault(x => x.userID == sa.agentUserID).tblSysWorkGroups.sysWorkGroup,
        //                            Email = us.aspnet_Membership.LoweredEmail,
        //                            IsApproved = us.aspnet_Membership.IsApproved,
        //                            IsLocked = us.aspnet_Membership.IsLockedOut,
        //                            LastActivityDate = us.LastActivityDate.ToUniversalTime()
        //                        };
        //    if (usm.Search_FirstName == null && usm.Search_LastName == null && usm.Search_UserName == null && usm.Search_JobPositionID == 0 && usm.Search_DestinationID == 0)
        //        searchResults = searchResults.Where(m => m.IsApproved == true);
        //    return searchResults;
        //}

        //public List<UserSearchItemModel> listUsers = new List<UserSearchItemModel>();

        public IEnumerable<UserSearchItemModel> Search(UserSearchModel model)
        {
            IQueryable<aspnet_Users> query = db.aspnet_Users;
            List<UserSearchItemModel> list = new List<UserSearchItemModel>();

            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            //user admin
            if (!GeneralFunctions.IsUserInRole("Administrator", CurrentUserID))
            {   //user supervisor
                var users = db.sp_getSubordinatedUsers(CurrentUserID).Select(m => m.userID).ToList();
                users.Add(CurrentUserID);
                query = query.Where(m => users.Contains(m.UserId) && m.tblUsers_Terminals.FirstOrDefault(x => terminals.Contains(x.terminalID)).terminalID != null);
            }

            // search user by selected terminals
            //   query = query.Where(m => m.tblUsers_Terminals.FirstOrDefault(x => terminals.Contains(x.terminalID)).terminalID != null);
            //

            if (model.Search_FirstName != null)
            {
                query = query.Where(m => m.tblUserProfiles.FirstOrDefault().firstName.Contains(model.Search_FirstName));
            }

            if (model.Search_LastName != null)
            {
                query = query.Where(m => m.tblUserProfiles.FirstOrDefault().lastName.Contains(model.Search_LastName));
            }

            if (model.Search_UserName != null)
            {
                query = query.Where(m => m.UserName.Contains(model.Search_UserName));
            }

            if (model.Search_Terminal != null)
            {
                query = query.Where(m => m.tblUsers_Terminals.FirstOrDefault(x => model.Search_Terminal.Contains(x.terminalID)).user_TerminalID != null);
            }

            if (model.Search_JobPositions != null)
            {
                query = query.Where(m => m.tblUsers_JobPositions.FirstOrDefault(x => model.Search_JobPositions.Contains(x.jobPositionID)).user_JobPositionID != null);
            }
            if (model.Search_Destinations != null)
            {
                query = query.Where(m => m.tblUsers_Destinations.FirstOrDefault(x => model.Search_Destinations.Contains(x.destinationID)).user_DestinationID != null);
            }

            if (model.Search_Company != null)
            {
                foreach (var x in model.Search_Company)
                {
                    var company = model.SearchCompanyList.FirstOrDefault(y => y.Value == x).Text;
                    query = query.Where(m => m.tblUserProfiles.FirstOrDefault().tblCompanies.company.Contains(company));
                }
            }

            if (model.Search_Profile != null)
            {
                var _sysWorkGroups = new List<int>();
                var _roles = new List<Guid>();
                foreach (var i in model.Search_Profile)
                {
                    //var _sysWorkGroupID = int.Parse(i.Split('|')[0]);
                    _sysWorkGroups.Add(int.Parse(i.Split('|')[0]));
                    _roles.Add(Guid.Parse(i.Split('|')[1]));
                    //var _roleID = Guid.Parse(i.Split('|')[1]);
                    //query = query.Where(m => m.tblUsers_SysWorkGroups1.FirstOrDefault(x => x.sysWorkGroupID == _sysWorkGroupID && x.roleID == _roleID).sysWorkGroup_UserID != null);
                }
                query = query.Where(m => m.tblUsers_SysWorkGroups1.FirstOrDefault(x => _sysWorkGroups.Contains(x.sysWorkGroupID) && _roles.Contains(x.roleID)).sysWorkGroup_UserID != null);
            }
            //A user will be able to look for a blocked user, only when he search specifically for it.
            if (model.Search_FirstName == null && model.Search_LastName == null && model.Search_UserName == null && model.Search_JobPositions == null && model.Search_Destinations == null && model.Search_Terminal == null && model.Search_Profile == null && model.Search_Company == null)
            {
                query = query.Where(m => m.aspnet_Membership.IsApproved == true);
            }

            foreach (var i in query)
            {
                try
                {
                    var Company = i.tblUserProfiles.FirstOrDefault().companyID;
                    var Phone = i.tblUserProfiles.FirstOrDefault().personalPhone;
                    var Dep = i.tblUserProfiles.FirstOrDefault().departmentID;
                    var DePhone = i.tblUserProfiles.FirstOrDefault().departamentPhone;
                    var EXT = i.tblUserProfiles.FirstOrDefault().phoneEXT;
                    var Lan = i.tblUserProfiles.FirstOrDefault().culture;
                    if (Company == null && Phone == null)
                    {
                        list.Add(new UserSearchItemModel()
                        {
                            UserID = i.UserId,
                            UserName = i.UserName,
                            FirstName = i.tblUserProfiles.FirstOrDefault().firstName,
                            LastName = i.tblUserProfiles.FirstOrDefault().lastName,
                            JobPosition = i.tblUsers_JobPositions.FirstOrDefault(m => m.toDate == null).tblJobPositions.jobPosition,
                            WorkGroup = i.tblUsers_SysWorkGroups1.FirstOrDefault().tblSysWorkGroups.sysWorkGroup,
                            Role = i.tblUsers_SysWorkGroups1.FirstOrDefault().aspnet_Roles.RoleName,
                            Email = i.aspnet_Membership.LoweredEmail,
                            Company = Company == null ? " " : db.tblCompanies.FirstOrDefault(x => x.companyID == Company).company,
                            Departament = Dep == null ? " " : db.tblCompanyDepartments.FirstOrDefault(x => x.departmentID == Dep).department,
                            DepartamentPhone = DePhone == null ? "" : DePhone,
                            PhoneEXT = EXT == null ? " " : EXT,
                            PersonalPhoneNumber = Phone == null ? "" : Phone,
                            Language = Lan == null ? " " : Lan,
                            IsApproved = i.aspnet_Membership.IsApproved,
                            IsLocked = i.aspnet_Membership.IsLockedOut,
                            LastActivityDate = i.LastActivityDate,
                            OPC = db.tblOPCS.FirstOrDefault(x => x.userID == i.UserId) == null ? "" : i.tblOPCS.FirstOrDefault(x => x.userID == i.UserId).opc
                        });
                    }
                    else
                    {
                        list.Add(new UserSearchItemModel()
                        {
                            UserID = i.UserId,
                            UserName = i.UserName,
                            FirstName = i.tblUserProfiles.FirstOrDefault().firstName,
                            LastName = i.tblUserProfiles.FirstOrDefault().lastName,
                            JobPosition = i.tblUsers_JobPositions.FirstOrDefault(m => m.toDate == null).tblJobPositions.jobPosition,
                            // WorkGroup = i.tblUsers_SysWorkGroups.FirstOrDefault().tblSysWorkGroups.sysWorkGroup,
                            WorkGroup = i.tblUsers_SysWorkGroups1.FirstOrDefault().tblSysWorkGroups.sysWorkGroup,
                            Role = i.tblUsers_SysWorkGroups1.FirstOrDefault().aspnet_Roles.RoleName,
                            // Role = i.tblUsers_SysWorkGroups.FirstOrDefault().aspnet_Roles.RoleName,
                            Email = i.aspnet_Membership.LoweredEmail,
                            Company = Company == null ? " " : db.tblCompanies.FirstOrDefault(x => x.companyID == Company).company,
                            Departament = Dep == null ? " " : db.tblCompanyDepartments.FirstOrDefault(x => x.departmentID == Dep).department,
                            DepartamentPhone = DePhone == null ? "" : DePhone,
                            PhoneEXT = EXT == null ? " " : EXT,
                            PersonalPhoneNumber = Phone == null ? "" : Phone,
                            Language = Lan == null ? " " : Lan,
                            OPC = db.tblOPCS.FirstOrDefault(x => x.userID == i.UserId) == null ? "" : i.tblOPCS.FirstOrDefault(x => x.userID == i.UserId).opc,
                            IsApproved = i.aspnet_Membership.IsApproved,
                            IsLocked = i.aspnet_Membership.IsLockedOut,
                            LastActivityDate = i.LastActivityDate
                        });
                    }
                }
                catch (Exception ex) { }
            }
            return list;
        }

        //public IEnumerable<UserSearchItemModel> SearchUsers(UserSearchModel usm)
        //{
        //    List<UserSearchItemModel> list = new List<UserSearchItemModel>();
        //    var searchResults = (from us in db.aspnet_Users
        //                         join sa in db.tblSupervisors_Agents on us.UserId equals sa.agentUserID
        //                         where sa.supervisorUserID == CurrentUserID
        //                         && (usm.Search_FirstName == null || us.tblUserProfiles.FirstOrDefault().firstName.Contains(usm.Search_FirstName))
        //                         && (usm.Search_LastName == null || us.tblUserProfiles.FirstOrDefault().lastName.Contains(usm.Search_LastName))
        //                         && (usm.Search_UserName == null || us.UserName.Contains(usm.Search_UserName))
        //                         && (usm.Search_JobPositionID == 0 || us.tblUsers_JobPositions.FirstOrDefault().jobPositionID == usm.Search_JobPositionID)
        //                         && (usm.Search_DestinationID == 0 || us.tblUsers_Destinations.FirstOrDefault().destinationID == usm.Search_DestinationID)
        //                         orderby us.aspnet_Membership.IsApproved
        //                         select new UserSearchItemModel()
        //                         {
        //                             UserID = us.UserId,
        //                             UserName = us.UserName,
        //                             FirstName = us.tblUserProfiles.FirstOrDefault().firstName,
        //                             LastName = us.tblUserProfiles.FirstOrDefault().lastName,
        //                             JobPosition = us.tblUsers_JobPositions.FirstOrDefault(x => x.userID == sa.agentUserID && x.toDate == null).tblJobPositions.jobPosition,
        //                             WorkGroup = us.tblUsers_SysWorkGroups1.FirstOrDefault(x => x.userID == sa.agentUserID).tblSysWorkGroups.sysWorkGroup,
        //                             Email = us.aspnet_Membership.LoweredEmail,
        //                             IsApproved = us.aspnet_Membership.IsApproved,
        //                             IsLocked = us.aspnet_Membership.IsLockedOut,
        //                             LastActivityDate = us.LastActivityDate
        //                         }).Distinct();
        //    //a user only will be able to look for a blocked user, when he search it specifically.
        //    if (usm.Search_FirstName == null && usm.Search_LastName == null && usm.Search_UserName == null && usm.Search_JobPositionID == 0 && usm.Search_DestinationID == 0)
        //        searchResults = searchResults.Where(m => m.IsApproved == true);
        //    foreach (var i in searchResults)
        //    {
        //        listUsers.Add(new UserSearchItemModel()
        //        {
        //            UserID = i.UserID,
        //            UserName = i.UserName,
        //            FirstName = i.FirstName,
        //            LastName = i.LastName,
        //            JobPosition = i.JobPosition,
        //            WorkGroup = i.WorkGroup,
        //            Email = i.Email,
        //            IsApproved = i.IsApproved,
        //            IsLocked = i.IsLocked,
        //            LastActivityDate = i.LastActivityDate
        //        });
        //        SearchAllSubordinates(i, usm);
        //    }
        //    //add its own information in case is administrator
        //    //if (db.aspnet_Roles.Single(m => m.RoleId.Equals((Guid)session.RoleID)).RoleName == "Administrator")
        //    if (Utils.GeneralFunctions.IsUserInRole("Administrator", CurrentUserID))
        //    {
        //        var currentUser = db.aspnet_Users.Single(m => m.UserId == CurrentUserID);
        //        listUsers.Add(new UserSearchItemModel()
        //        {
        //            UserID = CurrentUserID,
        //            UserName = currentUser.UserName,
        //            FirstName = currentUser.tblUserProfiles.FirstOrDefault().firstName,
        //            LastName = currentUser.tblUserProfiles.FirstOrDefault().lastName,
        //            JobPosition = currentUser.tblUsers_JobPositions.FirstOrDefault(m => m.userID == CurrentUserID).tblJobPositions.jobPosition,
        //            WorkGroup = currentUser.tblUsers_SysWorkGroups1.FirstOrDefault(m => m.userID == CurrentUserID).tblSysWorkGroups.sysWorkGroup,
        //            Email = currentUser.aspnet_Membership.LoweredEmail,
        //            IsApproved = currentUser.aspnet_Membership.IsApproved,
        //            IsLocked = currentUser.aspnet_Membership.IsLockedOut,
        //            LastActivityDate = currentUser.LastActivityDate
        //        });
        //    }
        //    var tempList = listUsers.Select(m => new { m.UserID, m.UserName, m.FirstName, m.LastName, m.JobPosition, m.WorkGroup, m.Email, m.IsApproved, m.IsLocked, m.LastActivityDate }).Distinct();
        //    foreach (var i in tempList)
        //    {
        //        list.Add(new UserSearchItemModel()
        //        {
        //            UserID = i.UserID,
        //            UserName = i.UserName,
        //            FirstName = i.FirstName,
        //            LastName = i.LastName,
        //            JobPosition = i.JobPosition,
        //            WorkGroup = i.WorkGroup,
        //            Email = i.Email,
        //            IsApproved = i.IsApproved,
        //            IsLocked = i.IsLocked,
        //            LastActivityDate = i.LastActivityDate
        //        });
        //    }
        //    return list;
        //}

        //public List<UserSearchItemModel> SearchAllSubordinates(UserSearchItemModel user, UserSearchModel usm)
        //{
        //    var searchResults = (from us in db.aspnet_Users
        //                         join sa in db.tblSupervisors_Agents on us.UserId equals sa.agentUserID
        //                         where sa.supervisorUserID == user.UserID
        //                         && (usm.Search_FirstName == null || us.tblUserProfiles.FirstOrDefault().firstName.Contains(usm.Search_FirstName))
        //                         && (usm.Search_LastName == null || us.tblUserProfiles.FirstOrDefault().lastName.Contains(usm.Search_LastName))
        //                         && (usm.Search_UserName == null || us.UserName.Contains(usm.Search_UserName))
        //                         && (usm.Search_JobPositionID == 0 || us.tblUsers_JobPositions.FirstOrDefault().jobPositionID == usm.Search_JobPositionID)
        //                         && (usm.Search_DestinationID == 0 || us.tblUsers_Destinations.FirstOrDefault().destinationID == usm.Search_DestinationID)
        //                         orderby us.aspnet_Membership.IsApproved
        //                         select new UserSearchItemModel()
        //                         {
        //                             UserID = us.UserId,
        //                             UserName = us.UserName,
        //                             FirstName = us.tblUserProfiles.FirstOrDefault().firstName,
        //                             LastName = us.tblUserProfiles.FirstOrDefault().lastName,
        //                             JobPosition = us.tblUsers_JobPositions.FirstOrDefault(x => x.userID == sa.agentUserID && x.toDate == null).tblJobPositions.jobPosition,
        //                             WorkGroup = us.tblUsers_SysWorkGroups1.FirstOrDefault(x => x.userID == sa.agentUserID).tblSysWorkGroups.sysWorkGroup,
        //                             Email = us.aspnet_Membership.LoweredEmail,
        //                             IsApproved = us.aspnet_Membership.IsApproved,
        //                             IsLocked = us.aspnet_Membership.IsLockedOut,
        //                             LastActivityDate = us.LastActivityDate
        //                         }).Distinct();
        //    //a user only will be able to look for a blocked user, when he search it specifically.
        //    if (usm.Search_FirstName == null && usm.Search_LastName == null && usm.Search_UserName == null && usm.Search_JobPositionID == 0 && usm.Search_DestinationID == 0)
        //        searchResults = searchResults.Where(m => m.IsApproved == true);
        //    foreach (var i in searchResults)
        //    {
        //        listUsers.Add(new UserSearchItemModel()
        //        {
        //            UserID = i.UserID,
        //            UserName = i.UserName,
        //            FirstName = i.FirstName,
        //            LastName = i.LastName,
        //            JobPosition = i.JobPosition,
        //            WorkGroup = i.WorkGroup,
        //            Email = i.Email,
        //            IsApproved = i.IsApproved,
        //            IsLocked = i.IsLocked,
        //            LastActivityDate = i.LastActivityDate
        //        });
        //        SearchAllSubordinates(i, usm);
        //    }
        //    return listUsers;
        //}

        public UserInfoModel GetUserInfo(Guid userID)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();
            UserInfoModel model = new UserInfoModel();
            //var listJP = new List<KeyValuePair<int, string>>();
            //var listD = new List<KeyValuePair<long, string>>();
            //var listT = new List<KeyValuePair<long, string>>();
            //var listS = new List<KeyValuePair<Guid, string>>();
            var listWG = new List<KeyValuePair<string, string>>();
            var query = db.aspnet_Users.Single(m => m.UserId == userID);
            var UserProfile = query.tblUserProfiles.FirstOrDefault(m => m.userID == userID);
            var OPC = db.tblOPCS.FirstOrDefault(m => m.userID == userID);
            model.UserInfo_UserID = userID;
            model.UserInfo_UserName = query.LoweredUserName;
            model.UserInfo_FirstName = query.tblUserProfiles.FirstOrDefault(m => m.userID == userID).firstName;
            model.UserInfo_LastName = query.tblUserProfiles.FirstOrDefault(m => m.userID == userID).lastName;
            model.UserInfo_SPIUserName = query.tblUserProfiles.FirstOrDefault(m => m.userID == userID).SPIUserName;
            model.UserInfo_Email = query.aspnet_Membership.LoweredEmail;
            model.UserInfo_IsApproved = query.aspnet_Membership.IsApproved;
            model.UserInfo_IsLockedOut = query.aspnet_Membership.IsLockedOut;
            model.UserInfo_OPC = OPC != null ? OPC.opcID.ToString() : "-1";
            //new
            model.UserInfo_PhoneEXT = UserProfile.phoneEXT;
            model.UserInfo_PersonalPhoneNumber = UserProfile.personalPhone;
            model.UserInfo_DepartamentPhone = UserProfile.departamentPhone;
            if (UserProfile.companyID != null)
            {
                model.UserInfo_Company = db.tblCompanies.FirstOrDefault(x => x.companyID == UserProfile.companyID).companyID.ToString();
            }
            else
            {
                model.UserInfo_Company = "0";
            }
            if (UserProfile.departmentID != null)
            {
                model.UserInfo_Departament = db.tblCompanyDepartments.FirstOrDefault(x => x.departmentID == UserProfile.departmentID).departmentID.ToString();
            }
            else
            {
                model.UserInfo_Departament = "0";
            }
            if (UserProfile.culture != null)
            {
                model.UserInfo_Language = db.tblLanguages.FirstOrDefault(x => x.culture == UserProfile.culture).culture;
            }
            else
            {
                model.UserInfo_Language = "0";
            }
            var jobPositions = db.tblUsers_JobPositions.Where(m => m.userID == userID && m.toDate == null).Select(m => m.jobPositionID).ToArray();
            model.UserInfo_JobPositions = jobPositions.Select(m => m.ToString()).ToArray();

            var destinations = db.tblUsers_Destinations.Where(m => m.userID == userID).Select(m => m.destinationID).ToArray();
            model.UserInfo_Destinations = destinations.Select(m => m.ToString()).ToArray();

            var terminals = db.tblUsers_Terminals.Where(m => m.userID == userID).Select(m => m.terminalID).ToArray();
            model.UserInfo_Terminals = terminals.Select(m => m.ToString()).ToArray();

            var supervisors = db.tblSupervisors_Agents.Where(m => m.agentUserID == userID).Select(m => m.supervisorUserID).ToArray();
            model.UserInfo_Supervisors = supervisors.Select(m => m.ToString()).ToArray();

            foreach (var i in db.tblUsers_SysWorkGroups.Where(m => m.userID == userID && m.sysWorkGroupTeamID == null))
            {
                //listWG.Add(new KeyValuePair<string, string>(i.sysWorkGroupID + "|" + i.roleID, i.tblSysWorkGroups.sysWorkGroup + "|" + i.aspnet_Roles.RoleName));
                listWG.Add(new KeyValuePair<string, string>(i.sysWorkGroupID + "|" + i.roleID + "|" + i.manageReservations + "|" + i.manageServices, i.tblSysWorkGroups.sysWorkGroup + "|" + i.aspnet_Roles.RoleName + "|" + i.manageReservations + "|" + i.manageServices));
            }
            model.UserInfo_WorkGroupsRolesIDs = listWG;
            model.UserInfo_LastDateActivity = query.LastActivityDate.ToString();

            if (OPC != null)
            {
                model.UserInfo_OPC = OPC.opcID.ToString();
            }
            else
            {
                model.UserInfo_OPC = "-1";
            }

            return model;
        }

        public AttemptResponse DeleteUser(Guid userID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.aspnet_Membership.Single(m => m.UserId == userID);
                query.IsApproved = false;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "User Deleted";
                response.ObjectID = userID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "User NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveUser(UserInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.UserInfo_UserID != null)
            {
                try
                {
                    var user = db.aspnet_Users.Single(m => m.UserId == model.UserInfo_UserID);
                    var profile = db.tblUserProfiles.Single(m => m.userID == model.UserInfo_UserID);
                    var queryJP = db.tblUsers_JobPositions.Where(m => m.userID == model.UserInfo_UserID);
                    var jPositions = db.tblUsers_JobPositions.Where(m => m.userID == model.UserInfo_UserID).Select(m => m.jobPositionID);
                    var queryD = db.tblUsers_Destinations.Where(m => m.userID == model.UserInfo_UserID);
                    var queryWG = db.tblUsers_SysWorkGroups.Where(m => m.userID == model.UserInfo_UserID && m.sysWorkGroupTeamID == null);
                    var queryT = db.tblUsers_Terminals.Where(m => m.userID == model.UserInfo_UserID);
                    var queryS = db.tblSupervisors_Agents.Where(m => m.agentUserID == model.UserInfo_UserID);
                    MembershipUser membershipUser = Membership.GetUser(model.UserInfo_UserID);
                    membershipUser.Email = model.UserInfo_Email;
                    membershipUser.IsApproved = model.UserInfo_IsApproved;
                    if (model.UserInfo_ConfirmPassword != "123456")
                    {
                        membershipUser.ChangePassword(membershipUser.ResetPassword(), model.UserInfo_ConfirmPassword);
                    }
                    if (user.aspnet_Membership.IsLockedOut && !model.UserInfo_IsLockedOut)
                    {
                        membershipUser.UnlockUser();
                    }
                    else if (!user.aspnet_Membership.IsLockedOut && model.UserInfo_IsLockedOut)
                    {
                        user.aspnet_Membership.IsLockedOut = model.UserInfo_IsLockedOut;
                        user.aspnet_Membership.LastLockoutDate = DateTime.Now;
                    }
                    if (user.LoweredUserName != model.UserInfo_UserName.ToLower())
                    {
                        if (db.aspnet_Users.Where(m => m.LoweredUserName == model.UserInfo_UserName.ToLower()).Count() > 0)
                        {
                            throw new Exception("The username/email you are attempting to save is in use");
                        }
                    }
                    Membership.UpdateUser(membershipUser);
                    user.UserName = model.UserInfo_UserName;
                    user.LoweredUserName = model.UserInfo_UserName.ToLower();
                    profile.firstName = model.UserInfo_FirstName;
                    profile.lastName = model.UserInfo_LastName;
                    profile.SPIUserName = model.UserInfo_SPIUserName;
                    //new
                    //profile.phoneEXT = model.UserInfo_PhoneEXT == null ? null : model.UserInfo_PhoneEXT;
                    profile.phoneEXT = model.UserInfo_PhoneEXT != null && model.UserInfo_PhoneEXT != "" ? model.UserInfo_PhoneEXT : null;
                    profile.departamentPhone = model.UserInfo_DepartamentPhone == null ? null : model.UserInfo_DepartamentPhone;
                    profile.personalPhone = model.UserInfo_PersonalPhoneNumber == null ? null : model.UserInfo_PersonalPhoneNumber;
                    profile.culture = model.UserInfo_Language == "0" ? null : model.UserInfo_Language;
                    profile.companyID = model.UserInfo_Company == "0" ? (int?)null : int.Parse(model.UserInfo_Company);
                    profile.departmentID = model.UserInfo_Departament == "0" ? (int?)null : int.Parse(model.UserInfo_Departament);
                    //
                    if (model.UserInfo_JobPositions != null && model.UserInfo_WorkGroup != null && model.UserInfo_Terminals != null && model.UserInfo_Supervisors != null)
                    {
                        try
                        {
                            foreach (var i in queryJP)
                            {
                                if (!model.UserInfo_JobPositions.Contains(i.jobPositionID.ToString()))
                                {
                                    if (i.toDate == null)
                                    {
                                        i.toDate = DateTime.Now;
                                    }
                                }
                            }
                            foreach (var i in queryD)
                            {
                                db.DeleteObject(i);
                            }
                            foreach (var i in queryWG)
                            {
                                db.DeleteObject(i);
                            }
                            foreach (var i in queryT)
                            {
                                db.DeleteObject(i);
                            }
                            foreach (var i in queryS)
                            {
                                db.DeleteObject(i);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Error ocurred during saving proccess", ex);
                        }
                        foreach (var i in model.UserInfo_JobPositions)
                        {
                            //if (!jPositions.Contains(int.Parse(i)))
                            var jp = int.Parse(i);
                            if (queryJP.Where(m => m.jobPositionID == jp && m.toDate == null).Count() == 0)
                            {
                                var jPosition = new tblUsers_JobPositions();
                                jPosition.userID = Guid.Parse(model.UserInfo_UserID.ToString());
                                jPosition.jobPositionID = int.Parse(i);
                                jPosition.fromDate = DateTime.Today;
                                db.tblUsers_JobPositions.AddObject(jPosition);
                            }
                        }
                    }
                    foreach (var i in model.UserInfo_Destinations)
                    {
                        var destination = new tblUsers_Destinations();
                        destination.userID = Guid.Parse(model.UserInfo_UserID.ToString());
                        destination.destinationID = int.Parse(i);
                        destination.dateSaved = DateTime.Now;
                        destination.savedByUserID = session.UserID;
                        db.tblUsers_Destinations.AddObject(destination);
                    }


                    if (model.UserInfo_Terminals != null)
                    {
                        foreach (var i in model.UserInfo_Terminals)
                        {
                            var terminal = new tblUsers_Terminals();
                            terminal.userID = Guid.Parse(model.UserInfo_UserID.ToString());
                            terminal.terminalID = int.Parse(i);
                            terminal.dateSaved = DateTime.Now;
                            terminal.savedByUserID = session.UserID;
                            db.tblUsers_Terminals.AddObject(terminal);
                        }
                    }

                    if (model.UserInfo_WorkGroup != null)
                    {
                        foreach (var i in model.UserInfo_WorkGroup.Split(','))
                        {
                            var workgroup = new tblUsers_SysWorkGroups();
                            var items = i.Split('|');
                            workgroup.userID = Guid.Parse(model.UserInfo_UserID.ToString());
                            //workgroup.sysWorkGroupID = int.Parse(items.First());
                            workgroup.sysWorkGroupID = int.Parse(items[0]);
                            //workgroup.roleID = Guid.Parse(items.Last());
                            workgroup.roleID = Guid.Parse(items[1]);
                            workgroup.manageReservations = items.Length > 2 ? bool.Parse(items[2]) : false;
                            workgroup.manageServices = items.Length > 2 ? bool.Parse(items[3]) : false;
                            workgroup.dateSaved = DateTime.Now;
                            workgroup.savedByUserID = session.UserID;
                            db.tblUsers_SysWorkGroups.AddObject(workgroup);
                        }
                    }

                    if (model.UserInfo_Supervisors == null && model.UserInfo_UserName == "eplat@villagroup.com")
                    {
                        var supervisor = new tblSupervisors_Agents();
                        supervisor.agentUserID = Guid.Parse(model.UserInfo_UserID.ToString());
                        db.tblSupervisors_Agents.AddObject(supervisor);
                    }
                    else if (model.UserInfo_Supervisors != null)
                    {

                        foreach (var i in model.UserInfo_Supervisors)
                        {

                            var supervisor = new tblSupervisors_Agents();
                            supervisor.agentUserID = Guid.Parse(model.UserInfo_UserID.ToString());
                            supervisor.supervisorUserID = Guid.Parse(i);
                            db.tblSupervisors_Agents.AddObject(supervisor);
                        }
                    }

                    //add opc from user                                      
                    if (model.UserInfo_OPC != "-1")
                    {
                        var opcIDModel = long.Parse(model.UserInfo_OPC);
                        var Opc = db.tblOPCS.Single(x => x.opcID == opcIDModel);
                        Opc.userID = Guid.Parse(model.UserInfo_UserID.ToString());
                    }
                    else //take opc from user
                    {
                        var Opc = db.tblOPCS.FirstOrDefault(x => x.userID == model.UserInfo_UserID);
                        if (Opc != null)
                        {
                            Opc.userID = null;
                        }
                    }
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "User Updated";
                    response.ObjectID = model.UserInfo_UserID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "User NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    MembershipCreateStatus status;
                    Membership.CreateUser(model.UserInfo_UserName, model.UserInfo_Password, model.UserInfo_Email, null, null, model.UserInfo_IsApproved, null, out status);
                    if (status == MembershipCreateStatus.Success)
                    {
                        try
                        {
                            var userID = (Guid)Membership.GetUser(model.UserInfo_UserName).ProviderUserKey;
                            var user = db.aspnet_Users.Single(m => m.UserId == userID);
                            var profile = new tblUserProfiles();

                            profile.userID = userID;
                            profile.firstName = model.UserInfo_FirstName;
                            profile.lastName = model.UserInfo_LastName;
                            profile.SPIUserName = model.UserInfo_SPIUserName;
                            //new
                            profile.culture = model.UserInfo_Language == "0" ? null : model.UserInfo_Language;
                            //profile.phoneEXT = model.UserInfo_PhoneEXT == null ? null : model.UserInfo_PhoneEXT;
                            profile.phoneEXT = model.UserInfo_PhoneEXT != null && model.UserInfo_PhoneEXT != "" ? model.UserInfo_PhoneEXT : null;
                            profile.personalPhone = model.UserInfo_PersonalPhoneNumber == null ? null : model.UserInfo_PersonalPhoneNumber;
                            profile.departamentPhone = model.UserInfo_DepartamentPhone == null ? null : model.UserInfo_DepartamentPhone;
                            profile.companyID = int.Parse(model.UserInfo_Company) == 0 ? (int?)null : int.Parse(model.UserInfo_Company);
                            profile.departmentID = int.Parse(model.UserInfo_Departament) == 0 ? (int?)null : int.Parse(model.UserInfo_Departament);
                            db.tblUserProfiles.AddObject(profile);
                            user.aspnet_Membership.IsLockedOut = model.UserInfo_IsLockedOut;
                            foreach (var i in model.UserInfo_JobPositions)
                            {
                                var jPosition = new tblUsers_JobPositions();
                                jPosition.userID = userID;
                                jPosition.jobPositionID = int.Parse(i);
                                jPosition.fromDate = DateTime.Now;
                                db.tblUsers_JobPositions.AddObject(jPosition);
                            }

                            foreach (var i in model.UserInfo_Destinations)
                            {
                                var destination = new tblUsers_Destinations();
                                destination.userID = userID;
                                destination.destinationID = int.Parse(i);
                                destination.dateSaved = DateTime.Now;
                                destination.savedByUserID = session.UserID;
                                db.tblUsers_Destinations.AddObject(destination);
                            }
                            foreach (var i in model.UserInfo_Terminals)
                            {
                                var terminal = new tblUsers_Terminals();
                                terminal.userID = userID;
                                terminal.terminalID = int.Parse(i);
                                terminal.dateSaved = DateTime.Now;
                                terminal.savedByUserID = session.UserID;
                                db.tblUsers_Terminals.AddObject(terminal);
                            }

                            foreach (var i in model.UserInfo_WorkGroup.Split(','))
                            {
                                var workgroup = new tblUsers_SysWorkGroups();
                                var items = i.Split('|');
                                workgroup.userID = userID;
                                //workgroup.sysWorkGroupID = int.Parse(items.First());
                                //workgroup.roleID = Guid.Parse(items.Last());
                                workgroup.sysWorkGroupID = int.Parse(items[0]);
                                workgroup.roleID = Guid.Parse(items[1]);
                                workgroup.manageReservations = items.Length > 2 ? bool.Parse(items[2]) : false;
                                workgroup.manageServices = items.Length > 2 ? bool.Parse(items[3]) : false;
                                workgroup.dateSaved = DateTime.Now;
                                workgroup.savedByUserID = session.UserID;
                                db.tblUsers_SysWorkGroups.AddObject(workgroup);
                            }
                            if (model.UserInfo_Supervisors == null && model.UserInfo_UserName == "eplat@villagroup.com")
                            {
                                var supervisor = new tblSupervisors_Agents();
                                supervisor.agentUserID = Guid.Parse(model.UserInfo_UserID.ToString());
                                db.tblSupervisors_Agents.AddObject(supervisor);
                            }
                            else
                            {
                                foreach (var i in model.UserInfo_Supervisors)
                                {
                                    var supervisor = new tblSupervisors_Agents();
                                    supervisor.agentUserID = userID;
                                    supervisor.supervisorUserID = Guid.Parse(i);
                                    db.tblSupervisors_Agents.AddObject(supervisor);
                                }
                            }
                            if (model.UserInfo_OPC != "-1")//opc
                            {
                                var opcIDModel = long.Parse(model.UserInfo_OPC);
                                var Opc = db.tblOPCS.Single(x => x.opcID == opcIDModel);
                                Opc.userID = userID;
                            }
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "User Saved";
                            response.ObjectID = new { userID = userID, lastDate = user.LastActivityDate.ToString() };
                            return response;
                        }
                        catch (Exception ex)
                        {
                            Membership.DeleteUser(model.UserInfo_UserName, true);
                            throw new Exception("Incorrect data");
                        }
                    }
                    else
                    {
                        throw new Exception(status.ToString());
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "User NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }
        //---------------------------------------------------------------------------------------------
        public UserProfileModel Find(Guid userID)
        {
            var query = from us in db.aspnet_Users
                        where us.UserId == userID
                        select new
                        {
                            id = us.UserId,
                            userName = us.UserName,
                            firstName = us.tblUserProfiles.FirstOrDefault().firstName,
                            lastName = us.tblUserProfiles.FirstOrDefault().lastName,
                            spiUSerName = us.tblUserProfiles.FirstOrDefault().SPIUserName,
                            us.tblUserProfiles.FirstOrDefault().departamentPhone,
                            us.tblUserProfiles.FirstOrDefault().phoneEXT,
                            us.tblUserProfiles.FirstOrDefault().personalPhone,
                            us.tblUserProfiles.FirstOrDefault().companyID,
                            us.tblUserProfiles.FirstOrDefault().departmentID,
                            us.tblUserProfiles.FirstOrDefault().culture,
                            email = us.aspnet_Membership.LoweredEmail,
                            isApproved = us.aspnet_Membership.IsApproved,
                            isLockedOut = us.aspnet_Membership.IsLockedOut,
                            activityDate = us.LastActivityDate
                        };

            var queryJP = from jp in db.tblUsers_JobPositions
                          where jp.userID == userID
                          select jp.jobPositionID;
            var jobPositions = queryJP.ToArray();
            var queryD = from d in db.tblUsers_Destinations
                         where d.userID == userID
                         select d.destinationID;
            var destinations = queryD.ToArray();
            var queryT = from t in db.tblUsers_Terminals
                         where t.userID == userID
                         select t.terminalID;
            var terminals = queryT.ToArray();
            IEnumerable<tblUsers_SysWorkGroups>
                queryWGR = from wg in db.tblUsers_SysWorkGroups
                           where wg.userID == userID
                           select wg;

            List<string> wgrlist = new List<string>();
            foreach (tblUsers_SysWorkGroups wgr in queryWGR)
            {
                wgrlist.Add(wgr.sysWorkGroupID + "|" + wgr.roleID.ToString());
            }
            string[] workgroups = wgrlist.ToArray();


            var queryS = from s in db.tblSupervisors_Agents
                         join up in db.tblUserProfiles on s.agentUserID equals up.userID
                         where s.agentUserID == userID
                         select s.supervisorUserID;

            var supervisors = queryS.Select(m => Guid.Parse(m.ToString())).ToArray();

            UserProfileModel user = new UserProfileModel()
            {
                UserID = query.FirstOrDefault().id,
                UserName = query.FirstOrDefault().userName,
                FirstName = query.FirstOrDefault().firstName,
                LastName = query.FirstOrDefault().lastName,
                SPIUserName = query.FirstOrDefault().spiUSerName,
                Email = query.FirstOrDefault().email,
                IsApproved = query.FirstOrDefault().isApproved,
                IsLockedOut = query.FirstOrDefault().isLockedOut,
                SupervisorIDsArray = supervisors,
                JobPositionIDsArray = jobPositions,
                DestinationIDsArray = destinations,
                TerminalsArray = terminals,
                WorkGroupsRolesArray = workgroups
            };

            return user;
        }

        public static Guid GetUserID(string username)
        {
            ePlatEntities db = new ePlatEntities();
            Guid userID;
            username = username.ToLower();
            var user = (from u in db.aspnet_Users
                        where u.LoweredUserName == username
                        select u.UserId);

            if (user.Count() > 0)
            {
                userID = user.First();
            }
            else
            {
                userID = Guid.Parse("00000000-0000-0000-0000-000000000000");
            }
            return userID;
        }

        public static List<Guid?> RecursiveGetUsers(List<Guid?> userID, bool showActive, int workGroupID, List<long> terminals, bool isAdmin, ref int depth)
        {
            ePlatEntities db = new ePlatEntities();
            List<Guid?> list = new List<Guid?>();
            if (depth < 20)
            {
                depth++;
                var now = DateTime.Now;
                //var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
                list = (from agents in db.tblSupervisors_Agents
                        join membership in db.aspnet_Membership on agents.agentUserID equals membership.UserId
                        join workgroup in db.tblUsers_SysWorkGroups on agents.agentUserID equals workgroup.userID
                        join terminal in db.tblUsers_Terminals on agents.agentUserID equals terminal.userID
                        //where userID.Contains(agents.supervisorUserID)
                        where (userID.Contains(agents.supervisorUserID) || isAdmin)
                        && ((showActive && membership.IsLockedOut == false) || !showActive)
                        && workgroup.sysWorkGroupID == workGroupID
                        && terminals.Contains(terminal.terminalID)
                        && (agents.toDate == null || agents.toDate <= now)
                        select agents.agentUserID).ToList().Select(m => (Guid?)m).Distinct().ToList();

                if (list.Count() > 0 && !isAdmin)
                {
                    list = list.Concat(RecursiveGetUsers(list, showActive, workGroupID, terminals, isAdmin, ref depth)).ToList();
                }
            }
            return list;
        }

        public static List<SelectListItem> GetUsersBySupervisor(Guid? supervisorID = null, bool showColeagues = false, bool showAdmin = false, bool showOnlyActive = false, bool showName = true)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> _list = new List<SelectListItem>();
            //List<SelectListItem> list = new List<SelectListItem>();
            supervisorID = supervisorID ?? session.UserID;
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToList();
            var depth = 0;
            var ids = RecursiveGetUsers(new List<Guid?>() { supervisorID }, showOnlyActive, (int)session.WorkGroupID, terminals, isAdmin, ref depth).Select(m => (Guid)m).ToArray();

            if (depth >= 19)
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                message.IsBodyHtml = true;
                message.Priority = System.Net.Mail.MailPriority.Normal;

                message.To.Add("efalcon@villagroup.com");
                message.Subject = "Supervisor Loop";
                message.Body = "https://eplat.com/public/TestSupervisorDepth/?userID=" + supervisorID.ToString() + "&showActive=" + showOnlyActive + "&workGroupID=" + session.WorkGroupID.ToString() + "&terminals[0]=" + terminals[0];
                Utils.EmailNotifications.Send(message);
            }

            var users = (from profile in db.tblUserProfiles
                         join user in db.aspnet_Users on profile.userID equals user.UserId
                         join membership in db.aspnet_Membership on user.UserId equals membership.UserId
                         where ids.Contains(profile.userID)
                         select new
                         {
                             profile.userID,
                             profile.firstName,
                             profile.lastName,
                             userName = user.UserName,
                             locked = membership.IsLockedOut
                         }).ToList();

            foreach (var i in users.Distinct())
            {
                _list.Add(new SelectListItem()
                {
                    Value = i.userID.ToString(),
                    Text = (showName ? i.firstName + " " + i.lastName : i.userName),
                    Selected = i.locked
                });
                //list.Add(new SelectListItem()
                //{
                //    Value = i.userID.ToString(),
                //    Text = (showName ? i.firstName + " " + i.lastName : i.userName) + (i.locked ? " - Locked" : "")
                //});
            }

            if (showColeagues)
            {
                if (!GeneralFunctions.IsUserInRole("Agent"))
                {
                    var coleagues = from uw in db.tblUsers_SysWorkGroups
                                    join u in db.aspnet_Users on uw.userID equals u.UserId
                                    join m in db.aspnet_Membership on uw.userID equals m.UserId
                                    join up in db.tblUserProfiles on uw.userID equals up.userID
                                    where uw.roleID == session.RoleID
                                    && uw.userID != session.UserID
                                    && uw.sysWorkGroupID == session.WorkGroupID
                                    && ((showOnlyActive && !m.IsLockedOut) || (!showOnlyActive && m.IsLockedOut != null))
                                    select new
                                    {
                                        u.UserId,
                                        u.UserName,
                                        up.firstName,
                                        up.lastName,
                                        locked = m.IsLockedOut
                                    };

                    foreach (var i in coleagues)
                    {
                        if (_list.Count(m => m.Value == i.UserId.ToString()) == 0)
                        {
                            _list.Add(new SelectListItem()
                            {
                                Value = i.UserId.ToString(),
                                Text = (showName ? i.firstName + " " + i.lastName : i.UserName),
                                Selected = i.locked
                            });
                        }
                        //list.Add(new SelectListItem()
                        //{
                        //    Value = i.UserId.ToString(),
                        //    Text = (showName ? i.firstName + " " + i.lastName : i.UserName) + (i.locked ? " - Locked" : "")
                        //});
                    }
                }
            }

            _list.Insert(0, new SelectListItem()
            {
                Value = session.UserID.ToString(),
                Text = showName ? session.User : db.aspnet_Users.Single(m => m.UserId == session.UserID).UserName,
                Selected = false
            });
            //list.Insert(0, new SelectListItem()
            //{
            //    Value = session.UserID.ToString(),
            //    Text = showName ? session.User : db.aspnet_Users.Single(m => m.UserId == session.UserID).UserName
            //});

            if (_list.Count() > 1)
            {
                //list.Sort((x, y) => x.Text.CompareTo(y.Text));
                _list = _list.OrderBy(m => m.Selected).ThenBy(m => m.Text).ToList();
                foreach (var i in _list.Where(m => m.Selected == true))
                {
                    i.Text += " - Locked";
                    i.Selected = false;
                }
                
                
            }
            //list = list.Distinct().ToList();
            return _list.Distinct().ToList();
        }

        public List<SelectListItem> GetDDLData(string itemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            switch (itemType)
            {
                case "supervisors":
                    {
                        list = UserCatalogs.GetSupervisorsListChange(itemID);
                        break;
                    }
                case "selectedTerminals":
                    {
                        list = UserCatalogs.GetActivieTerminalsAdmin(); //TerminalDataModel.GetActiveTerminalsList();
                        break;
                    }
                case "company":
                    {
                        list = UserCatalogs.CompaniesList();
                        break;
                    }
                case "departament":
                    {
                        list = UserCatalogs.DepartamentList(itemID);
                        break;
                    }
                case "OPC":
                    {
                        list = ePlatBack.Models.DataModels.OpcDataModel.GetActiveOPCs();
                        list.Insert(0, new SelectListItem { Text = "--None--", Value = "-1" });
                        break;
                    }
                case "supervisorsUpdate":
                    {
                        list = UserCatalogs.GetSupervisorsList();
                        break;
                    }
            }
            return list;
        }

        //Insert/Delete Methods


        //Persistence
        public void Save()
        {
            db.SaveChanges();
        }


        //get users supervisors

        public IEnumerable<UserSupervisorSearchModel> SearchSupervisor(UserSupervisorSearchModel model)
        {


            List<UserSupervisorSearchModel> list = new List<UserSupervisorSearchModel>();
            ePlatEntities db = new ePlatEntities();
            var query = (from u in db.tblUserProfiles
                         join usw in db.tblUsers_SysWorkGroups on u.userID equals usw.userID
                         join em in db.aspnet_Membership on u.userID equals em.UserId
                         join ut in db.tblUsers_Terminals on u.userID equals ut.userID
                         join ro in db.aspnet_Roles on usw.roleID equals ro.RoleId
                         join t in db.tblTerminals on ut.terminalID equals t.terminalID
                         join ujb in db.tblUsers_JobPositions on u.userID equals ujb.userID
                         join jb in db.tblJobPositions on ujb.jobPositionID equals jb.jobPositionID
                         where ut.terminalID == model.Search_Terminal
                         && usw.sysWorkGroupID == model.Search_Workgroup
                         select new
                         {
                             Search_FirstName = u.firstName,
                             Search_LastName = u.lastName,
                             Search_JobPosition = jb.jobPosition,
                             email = em.Email,
                             userid = u.userID,
                         }).Distinct();
            if (model.Search_Roles.Equals("0"))
            {
                query = (from u in db.tblUserProfiles
                         join usw in db.tblUsers_SysWorkGroups on u.userID equals usw.userID
                         join em in db.aspnet_Membership on u.userID equals em.UserId
                         join ut in db.tblUsers_Terminals on u.userID equals ut.userID
                         join ro in db.aspnet_Roles on usw.roleID equals ro.RoleId
                         join t in db.tblTerminals on ut.terminalID equals t.terminalID
                         join ujb in db.tblUsers_JobPositions on u.userID equals ujb.userID
                         join jb in db.tblJobPositions on ujb.jobPositionID equals jb.jobPositionID
                         where ut.terminalID == model.Search_Terminal
                         && usw.sysWorkGroupID == model.Search_Workgroup
                         select new
                         {
                             Search_FirstName = u.firstName,
                             Search_LastName = u.lastName,
                             Search_JobPosition = jb.jobPosition,
                             email = em.Email,
                             userid = u.userID,
                         }).Distinct();
            }
            else
            {
                query = (from u in db.tblUserProfiles
                         join usw in db.tblUsers_SysWorkGroups on u.userID equals usw.userID
                         join em in db.aspnet_Membership on u.userID equals em.UserId
                         join ut in db.tblUsers_Terminals on u.userID equals ut.userID
                         join ro in db.aspnet_Roles on usw.roleID equals ro.RoleId
                         join t in db.tblTerminals on ut.terminalID equals t.terminalID
                         join ujb in db.tblUsers_JobPositions on u.userID equals ujb.userID
                         join jb in db.tblJobPositions on ujb.jobPositionID equals jb.jobPositionID
                         where ut.terminalID == model.Search_Terminal
                         && usw.sysWorkGroupID == model.Search_Workgroup
                         && (usw.roleID == new Guid(model.Search_Roles) && !(model.Search_Roles.Equals("00000000-0000-0000-0000-000000000000")))
                         select new
                         {
                             Search_FirstName = u.firstName,
                             Search_LastName = u.lastName,
                             Search_JobPosition = jb.jobPosition,
                             email = em.Email,
                             userid = u.userID,
                         }).Distinct();
            }

            foreach (var u in query)
            {
                list.Add(new UserSupervisorSearchModel()
                {

                    UserID = u.userid,
                    Search_FirstName = u.Search_FirstName,
                    Search_LastName = u.Search_LastName,
                    Search_JobPosition = u.Search_JobPosition,
                    Search_UserName = u.email,

                });
            }

            return list;
        }



        public IEnumerable<UserSupervisorOfModel> GetSubordinates(Guid userID)
        {

            List<UserSupervisorOfModel> list = new List<UserSupervisorOfModel>();
            ePlatEntities db = new ePlatEntities();
            var query = (from u in db.tblUserProfiles
                         join usw in db.tblUsers_SysWorkGroups on u.userID equals usw.userID
                         join em in db.aspnet_Membership on u.userID equals em.UserId
                         join ut in db.tblUsers_Terminals on u.userID equals ut.userID
                         join ro in db.aspnet_Roles on usw.roleID equals ro.RoleId
                         join t in db.tblTerminals on ut.terminalID equals t.terminalID
                         join ujb in db.tblUsers_JobPositions on u.userID equals ujb.userID
                         join jb in db.tblJobPositions on ujb.jobPositionID equals jb.jobPositionID
                         join asu in db.tblSupervisors_Agents on u.userID equals asu.agentUserID
                         where asu.supervisorUserID == userID
                         select new
                         {
                             Search_FirstName = u.firstName,
                             Search_LastName = u.lastName,
                             Search_JobPosition = jb.jobPosition,
                             email = em.Email,
                             userid = u.userID,
                         }).Distinct();


            foreach (var u in query)
            {
                list.Add(new UserSupervisorOfModel()
                {

                    UserID = u.userid,
                    Search_FirstName = u.Search_FirstName,
                    Search_LastName = u.Search_LastName,
                    Search_JobPosition = u.Search_JobPosition,
                    Search_UserName = u.email,

                });
            }
            return list;
        }



        public IEnumerable<UserSupervisorSearchModel> GetSupervisors(Guid userID)
        {

            List<UserSupervisorSearchModel> list = new List<UserSupervisorSearchModel>();
            ePlatEntities db = new ePlatEntities();
            var query = (from u in db.tblUserProfiles
                         join usw in db.tblUsers_SysWorkGroups on u.userID equals usw.userID
                         join em in db.aspnet_Membership on u.userID equals em.UserId
                         join ut in db.tblUsers_Terminals on u.userID equals ut.userID
                         join ro in db.aspnet_Roles on usw.roleID equals ro.RoleId
                         join t in db.tblTerminals on ut.terminalID equals t.terminalID
                         join ujb in db.tblUsers_JobPositions on u.userID equals ujb.userID
                         join jb in db.tblJobPositions on ujb.jobPositionID equals jb.jobPositionID
                         join asu in db.tblSupervisors_Agents on u.userID equals asu.supervisorUserID
                         where asu.agentUserID == userID
                         select new
                         {
                             Search_FirstName = u.firstName,
                             Search_LastName = u.lastName,
                             Search_JobPosition = jb.jobPosition,
                             email = em.Email,
                             userid = u.userID,
                         }).Distinct();


            foreach (var u in query)
            {
                list.Add(new UserSupervisorSearchModel()
                {

                    UserID = u.userid,
                    Search_FirstName = u.Search_FirstName,
                    Search_LastName = u.Search_LastName,
                    Search_JobPosition = u.Search_JobPosition,
                    Search_UserName = u.email,

                });
            }
            return list;

        }

        public static AttemptResponse SendActiveUsers()
        {
            AttemptResponse response = new AttemptResponse();

            try
            {
                System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(30, 42, "en-US");
                email.To.Add("robkis@yahoo.com");
                email.Bcc.Add("gguerrap@villagroup.com");
                //email.To.Add("gguerrap@villagroup.com");
                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Active Users Succesfully Sent";
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to send Active Users";
                response.ObjectID = 0;
                response.Exception = ex;
            }

            return response;
        }

        public static AttemptResponse SaveUserRequest(Guid userRequestID, string notifyTo, string jsonModel)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();

            tblUserRequests newRequest = new tblUserRequests();
            newRequest.userRequestID = userRequestID;
            newRequest.jsonModel = jsonModel;
            newRequest.dateSaved = DateTime.Now;
            newRequest.notifyToEmail = notifyTo;

            try
            {
                db.tblUserRequests.AddObject(newRequest);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = userRequestID;
                response.Message = "User request saved.";
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to save request.";
                response.ObjectID = 0;
                response.Exception = ex;
            }

            return response;
        }

        public static UserRequestsViewModel GetUserRequests()
        {
            UserRequestsViewModel model = new UserRequestsViewModel();
            model.Requests = new List<UserRequestsViewModel.UserRequestItem>();
            ePlatEntities db = new ePlatEntities();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            var Requests = (from r in db.tblUserRequests
                            orderby r.dateSaved descending
                            select new
                            {
                                r.userRequestID,
                                r.jsonModel,
                                r.dateSaved,
                                r.dateDocsChecked,
                                r.dateApproved,
                                r.dateDelivered,
                                r.notifyToEmail,
                                r.tblUserDocumentation,
                                r.requestDocument
                            }).Take(100).ToList();

            foreach (var ur in Requests)
            {
                UserRequestsViewModel.UserRequestItem request = new UserRequestsViewModel.UserRequestItem();
                request.UserRequestID = ur.userRequestID;
                request.Attachements = 0;
                request.Users = "";
                request.DateSavedDT = ur.dateSaved;
                request.DateSaved = ur.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt");
                request.DateDocsChecked = (ur.dateDocsChecked != null ? ur.dateDocsChecked.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "pending");
                request.DateApproved = (ur.dateApproved != null ? ur.dateApproved.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "pending");
                request.DateDelivered = (ur.dateDelivered != null ? ur.dateDelivered.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "pending");
                request.NotifyToEmail = ur.notifyToEmail;

                request.Attachements += ur.requestDocument != null && ur.requestDocument != "" ? 1 : 0;
                foreach (var us in ur.tblUserDocumentation)
                {
                    request.Attachements += us.confidencialityDocument != null && us.confidencialityDocument != "" ? 1 : 0;
                }

                try
                {
                    UserRequest usrequest = js.Deserialize<UserRequest>(ur.jsonModel);
                    foreach (var us in usrequest.Users)
                    {
                        if (request.Users != "")
                        {
                            request.Users += "<br />";
                        }
                        request.Users += us.FirstName + " " + us.LastName;
                    }
                }
                catch
                {

                }

                model.Requests.Add(request);
            }

            return model;
        }

        public static AttemptResponse LockUsers()
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities eplat = new ePlatEntities();
            ecommerceEntities eplatform = new ecommerceEntities();
            List<Guid> eplatUsersToLock = new List<Guid>();
            List<Guid> eplatformUsersToLock = new List<Guid>();
            List<ActiveUsers.ActiveUser> LockedUsers = new List<ActiveUsers.ActiveUser>();
            List<ActiveUsers.ActiveUser> NotifiedUsers = new List<ActiveUsers.ActiveUser>();

            try
            {
                List<ActiveUsers.ActiveUser> ActiveUsers = GetActiveUsers();

                foreach (var user in ActiveUsers)
                {
                    if (user.DaysToLock != 0)
                    {
                        double inactiveDays = (DateTime.Now - DateTime.Parse(user.LastActivityDate)).TotalDays;
                        if (inactiveDays > user.DaysToLock)
                        {
                            if (user.System == "ePlatform")
                            {
                                eplatformUsersToLock.Add(user.UserID);
                            }
                            else if (user.System == "ePlat")
                            {
                                eplatUsersToLock.Add(user.UserID);
                            }
                            LockedUsers.Add(user);
                        }
                        else if (inactiveDays < user.DaysToLock && inactiveDays > user.DaysToLock - 1)
                        {
                            //próximo a bloquearse
                            //NotifiedUsers.Add(user);
                            //enviar notificación
                        }
                    }
                }

                //obtener users to lock
                var ePlatUsers = from u in eplat.aspnet_Membership
                                 where eplatUsersToLock.Contains(u.UserId)
                                 select u;

                foreach (var u in ePlatUsers)
                {
                    u.IsLockedOut = true;
                    u.LastLockoutDate = DateTime.Now;
                }

                var ePlatformUsers = from u in eplatform.aspnet_Membership
                                     where eplatformUsersToLock.Contains(u.UserId)
                                     select u;

                foreach (var u in ePlatformUsers)
                {
                    u.IsLockedOut = true;
                    u.LastLockoutDate = DateTime.Now;
                }

                //guardar cambios
                /*eplat.SaveChanges();
                eplatform.SaveChanges();*/

                //enviar correo con summary de bloqueos
                string tblLockedUsers = "<table cellpadding=\"5\" cellspacing=\"0\" border=\"0\" style=\"font-famiy: verdana;\">";
                tblLockedUsers += "<thead><tr style=\"background-color:#252525; color:white;\"><th>System</th><th>User</th><th>Last Activity Date</th><th>Allowed Inactive Days</th></tr></thead><tbody>";
                foreach (var lu in LockedUsers)
                {
                    tblLockedUsers += "<tr><td>" + lu.System + "</td><td>" + lu.FirstName + " " + lu.LastName + "</td><td>" + lu.LastActivityDate + "</td><td style=\"text-align:center;\">" + lu.DaysToLock + "</td></tr>";
                }
                tblLockedUsers += "</tbody></table>";

                System.Net.Mail.MailMessage email = Utils.EmailNotifications.GetEmail(31, 42, "en-US");
                email.To.Add("gguerrap@villagroup.com");
                email.Body = email.Body.Replace("$Date", DateTime.Today.ToString("yyyy-MM-dd"));
                email.Body = email.Body.Replace("$table", tblLockedUsers);

                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = LockedUsers;
                response.Message = LockedUsers.Count() + " users were locked out.";
                //+ NotifiedUsers.Count() + " users were warned 24 hours before to be locked out.";
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to lock users";
                response.ObjectID = 0;
                response.Exception = ex;
            }

            return response;
        }

        public static List<ActiveUsers.ActiveUser> GetActiveUsers()
        {
            List<ActiveUsers.ActiveUser> Users = new List<ActiveUsers.ActiveUser>();
            ePlatEntities eplat = new ePlatEntities();
            ecommerceEntities eplatform = new ecommerceEntities();

            //eplat
            var ePlatUsers = from profile in eplat.tblUserProfiles
                             join user in eplat.aspnet_Users
                             on profile.userID equals user.UserId
                             into profile_user
                             from user in profile_user.DefaultIfEmpty()
                             join membership in eplat.aspnet_Membership
                             on profile.userID equals membership.UserId
                             into profile_membership
                             from membership in profile_membership.DefaultIfEmpty()
                             join userterminal in eplat.tblUsers_Terminals
                             on profile.userID equals userterminal.userID
                             into profile_userterminal
                             from userterminal in profile_userterminal.DefaultIfEmpty()
                             join terminal in eplat.tblTerminals
                             on userterminal.terminalID equals terminal.terminalID
                             into terminal_userterminal
                             from terminal in terminal_userterminal.DefaultIfEmpty()
                             where !profile.firstName.Contains("test")
                             && !profile.lastName.Contains("test")
                             && !profile.firstName.Contains("prueba")
                             && !profile.lastName.Contains("prueba")
                             && membership.IsLockedOut == false
                             orderby profile.firstName, terminal.terminal
                             select new
                             {
                                 profile.userID,
                                 profile.firstName,
                                 profile.lastName,
                                 user.LoweredUserName,
                                 membership.IsApproved,
                                 membership.IsLockedOut,
                                 membership.LoweredEmail,
                                 user.LastActivityDate,
                                 userterminal.terminalID,
                                 terminal.terminal
                             };

            var SysWorkGroups = (from w in eplat.tblUsers_SysWorkGroups
                                 where ePlatUsers.Select(x => x.userID).Distinct().Contains(w.userID)
                                 select new
                                 {
                                     w.userID,
                                     w.roleID,
                                     w.sysWorkGroupID
                                 }).Distinct().ToList();

            var Profiles = (from p in eplat.tblSysProfilesSettings
                            select new
                            {
                                p.roleID,
                                p.sysWorkGroupID,
                                p.allowedInactiveDays,
                                p.countAsUser
                            }).ToList();

            List<Guid> NotAsUser = new List<Guid>();

            foreach (var user in ePlatUsers)
            {
                if (NotAsUser.Count(x => x == user.userID) == 0)
                {
                    if (Users.Count(x => x.UserID == user.userID) == 0)
                    {
                        ActiveUsers.ActiveUser userInfo = new ActiveUsers.ActiveUser();
                        bool countAsUser = true;
                        userInfo.System = "ePlat";
                        userInfo.UserID = user.userID;
                        userInfo.FirstName = user.firstName;
                        userInfo.LastName = user.lastName;
                        userInfo.Email = user.LoweredEmail;
                        userInfo.Username = user.LoweredUserName;
                        userInfo.LastActivityDate = user.LastActivityDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                        userInfo.Terminals = new List<string>();
                        userInfo.DaysToLock = 0;
                        foreach (var role in SysWorkGroups.Where(x => x.userID == user.userID))
                        {
                            var profile = Profiles.FirstOrDefault(x => x.roleID == role.roleID && x.sysWorkGroupID == role.sysWorkGroupID);
                            if (profile != null)
                            {
                                if (profile.allowedInactiveDays > userInfo.DaysToLock)
                                {
                                    userInfo.DaysToLock = profile.allowedInactiveDays;
                                }
                                countAsUser = profile.countAsUser;
                            }
                        }
                        if (countAsUser)
                        {
                            Users.Add(userInfo);
                        }
                        else
                        {
                            NotAsUser.Add(user.userID);
                        }
                    }

                    if (Users.Count(x => x.UserID == user.userID) > 0)
                    {
                        Users.FirstOrDefault(x => x.UserID == user.userID).Terminals.Add(user.terminal);
                    }
                }
            }

            //eplatform
            var eplatformUsers = from user in eplatform.aspnet_Users
                                 join membership in eplatform.aspnet_Membership
                                 on user.UserId equals membership.UserId
                                 into user_membership
                                 from membership in user_membership.DefaultIfEmpty()
                                 join profile in eplatform.tbaDirectorio
                                 on user.UserId equals profile.idUsuario
                                 into user_profile
                                 from profile in user_profile.DefaultIfEmpty()
                                 join terminalusuario in eplatform.tbaTerminalesUsuarios
                                 on user.UserId equals terminalusuario.idUsuario
                                 into user_terminalusuario
                                 from userterminal in user_terminalusuario.DefaultIfEmpty()
                                 join terminal in eplatform.tbaTerminales
                                 on userterminal.idTerminal equals terminal.idTerminal
                                 where membership.IsLockedOut == false
                                 orderby user.LastActivityDate
                                 select new
                                 {
                                     user.UserId,
                                     profile.nombre,
                                     profile.apellido,
                                     user.LoweredUserName,
                                     membership.IsApproved,
                                     membership.IsLockedOut,
                                     membership.LoweredEmail,
                                     user.LastActivityDate,
                                     userterminal.idTerminal,
                                     terminal.terminal,
                                     profile.diasParaBloquear
                                 };

            foreach (var user in eplatformUsers)
            {
                if (Users.Count(x => x.UserID == user.UserId) == 0)
                {
                    ActiveUsers.ActiveUser userInfo = new ActiveUsers.ActiveUser();

                    userInfo.System = "ePlatform";
                    userInfo.UserID = user.UserId;
                    userInfo.FirstName = user.nombre;
                    userInfo.LastName = user.apellido;
                    userInfo.Email = user.LoweredEmail;
                    userInfo.Username = user.LoweredUserName;
                    userInfo.LastActivityDate = user.LastActivityDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                    userInfo.DaysToLock = (user.diasParaBloquear != null ? (int)user.diasParaBloquear : 0);
                    userInfo.Terminals = new List<string>();

                    Users.Add(userInfo);
                }
                Users.FirstOrDefault(x => x.UserID == user.UserId).Terminals.Add(user.terminal);
            }

            return Users;
        }

        public static DependantFields GetUserRequestDependentFields()
        {
            ePlatEntities eplat = new ePlatEntities();
            ecommerceEntities eplatform = new ecommerceEntities();
            DependantFields df = new DependantFields();
            df.Fields = new List<DependantFields.DependantField>();

            //System
            DependantFields.DependantField System = new DependantFields.DependantField();
            System.Field = "System";
            System.ParentField = "Terminal";
            System.Values = new List<DependantFields.FieldValue>();

            DependantFields.FieldValue val1 = new DependantFields.FieldValue();
            val1.ParentValue = null;
            val1.Value = "ePlat";
            val1.Text = "ePlat";
            System.Values.Add(val1);

            DependantFields.FieldValue val2 = new DependantFields.FieldValue();
            val2.ParentValue = null;
            val2.Value = "ePlatform";
            val2.Text = "ePlatform";
            System.Values.Add(val2);

            df.Fields.Add(System);

            //System > Destinations
            DependantFields.DependantField Destinations = new DependantFields.DependantField();
            Destinations.Field = "Destinations";
            Destinations.ParentField = "System";
            Destinations.Values = new List<DependantFields.FieldValue>();

            var DestinationsQ = from d in eplat.tblDestinations
                                orderby d.destination
                                select new
                                {
                                    d.destinationID,
                                    d.destination
                                };

            foreach (var destination in DestinationsQ)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlat";
                val.Value = destination.destinationID.ToString();
                val.Text = destination.destination;
                Destinations.Values.Add(val);
            }

            var DestinationsE = from d in eplatform.tbaDestinos
                                orderby d.destino
                                select new
                                {
                                    d.idDestino,
                                    d.destino
                                };

            foreach (var destination in DestinationsE)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlatform";
                val.Value = destination.idDestino.ToString();
                val.Text = destination.destino;
                Destinations.Values.Add(val);
            }

            df.Fields.Add(Destinations);

            //System > Terminals
            DependantFields.DependantField Terminals = new DependantFields.DependantField();
            Terminals.Field = "Terminals";
            Terminals.ParentField = "System";
            Terminals.Values = new List<DependantFields.FieldValue>();

            var Terminals1 = from t in eplat.tblTerminals
                             where !t.terminal.Contains("test")
                             && !t.terminal.Contains("prueba")
                             orderby t.terminal
                             select new
                             {
                                 t.terminalID,
                                 t.terminal
                             };

            foreach (var t in Terminals1)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlat";
                val.Value = t.terminalID.ToString();
                val.Text = t.terminal;
                Terminals.Values.Add(val);
            }

            var Terminals2 = from t in eplatform.tbaTerminales
                             where !t.terminal.Contains("test")
                             && !t.terminal.Contains("prueba")
                             orderby t.terminal
                             select new
                             {
                                 t.terminal,
                                 t.idTerminal
                             };

            foreach (var t in Terminals2)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlatform";
                val.Value = t.idTerminal.ToString();
                val.Text = t.terminal;
                Terminals.Values.Add(val);
            }

            df.Fields.Add(Terminals);

            //System > BasedOn
            DependantFields.DependantField BasedOn = new DependantFields.DependantField();
            BasedOn.Field = "BasedOn";
            BasedOn.ParentField = "System";
            BasedOn.Values = new List<DependantFields.FieldValue>();

            var BasedOn1 = from m in eplat.aspnet_Membership
                           join u in eplat.aspnet_Users
                           on m.UserId equals u.UserId
                           into m_u
                           from u in m_u.DefaultIfEmpty()
                           join p in eplat.tblUserProfiles
                           on m.UserId equals p.userID
                           into m_p
                           from p in m_p.DefaultIfEmpty()
                           where m.IsLockedOut != true
                           && !p.firstName.Contains("test")
                           && !p.firstName.Contains("prueba")
                           && !p.lastName.Contains("test")
                           && !p.lastName.Contains("prueba")
                           orderby p.firstName
                           select new
                           {
                               m.UserId,
                               p.firstName,
                               p.lastName,
                               u.UserName
                           };

            foreach (var u in BasedOn1)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlat";
                val.Value = u.UserId.ToString();
                val.Text = u.firstName + " " + u.lastName + (u.UserName.IndexOf("@") > 0 ? " [" + u.UserName.Remove(u.UserName.IndexOf("@")) + "]" : " [" + u.UserName + "]");
                BasedOn.Values.Add(val);
            }

            var BasedOn2 = from m in eplatform.aspnet_Membership
                           join u in eplatform.aspnet_Users
                           on m.UserId equals u.UserId
                           into m_u
                           from u in m_u.DefaultIfEmpty()
                           join p in eplatform.tbaDirectorio
                           on m.UserId equals p.idUsuario
                           into m_p
                           from p in m_p.DefaultIfEmpty()
                           where m.IsLockedOut != true
                           && !p.nombre.Contains("test")
                           && !p.nombre.Contains("prueba")
                           && !p.apellido.Contains("test")
                           && !p.apellido.Contains("prueba")
                           orderby p.nombre
                           select new
                           {
                               m.UserId,
                               p.nombre,
                               p.apellido,
                               u.UserName
                           };

            foreach (var u in BasedOn2)
            {
                DependantFields.FieldValue val = new DependantFields.FieldValue();
                val.ParentValue = "ePlatform";
                val.Value = u.UserId.ToString();
                val.Text = u.nombre + " " + u.apellido + (u.UserName.IndexOf("@") > 0 ? " [" + u.UserName.Remove(u.UserName.IndexOf("@")) + "]" : " [" + u.UserName + "]");
                BasedOn.Values.Add(val);
            }

            df.Fields.Add(BasedOn);

            return df;
        }

        public static PictureDataModel.FineUploaderResult UploadDocument(PictureDataModel.FineUpload upload, int type, Guid requestID, Guid? id = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (upload.Exception == null)
            {
                var firstPath = HttpContext.Current.Server.MapPath("~/");
                var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                var finalPath = secondPath + "ePlatBack\\Content\\files\\users\\";
                var fileName = type.ToString() + "-" + (type == 1 ? requestID : id) + "-" + upload.Filename;
                var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
                fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
                fileName = fileName.Replace("+", "-");

                for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
                {
                    var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                    var newFileName = fileName.Replace(encoded, "_");
                    fileName = newFileName;
                }

                var filePath = finalPath + "\\" + fileName;
                var _filePath = "/content/files/users/" + fileName;
                upload.SaveAs(filePath, false);

                //guardar la relación
                if (type == 1)
                {
                    //documento de solicitud
                    var UserRequests = (from r in db.tblUserRequests
                                        where r.userRequestID == requestID
                                        select r).FirstOrDefault();

                    if (UserRequests != null)
                    {
                        if (UserRequests.requestDocument != null)
                        {
                            //eliminar archivo
                            var deletePath = HttpContext.Current.Server.MapPath("~" + UserRequests.requestDocument);
                            var file = new System.IO.FileInfo(deletePath);
                            file.Delete();
                        }
                        UserRequests.requestDocument = _filePath;
                    }
                }
                else
                {
                    //documento de usuario
                    var UserDocument = (from d in db.tblUserDocumentation
                                        where d.userTemporalID == id
                                        select d).FirstOrDefault();

                    if (UserDocument != null)
                    {
                        //ya existe el registro del documento, actualizar path
                        UserDocument.confidencialityDocument = _filePath;
                    }
                    else
                    {
                        //no existe el registro del documento, crear registro
                        tblUserDocumentation newDocument = new tblUserDocumentation();
                        newDocument.userRequestID = requestID;
                        newDocument.userTemporalID = (Guid)id;
                        newDocument.confidencialityDocument = _filePath;
                        db.tblUserDocumentation.AddObject(newDocument);
                    }
                }
                db.SaveChanges();

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "File Uploaded";
                return new PictureDataModel.FineUploaderResult(true, new { response = response }, new { path = _filePath });
            }
            else
            {
                throw new Exception();
            }
        }

        public static UserRequest GetUserRequest(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            UserRequest request = new UserRequest();

            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            var Request = (from r in db.tblUserRequests
                           where r.userRequestID == id
                           select r).FirstOrDefault();
            var model = Request.jsonModel.Replace("\\t"," ");
            while(model.IndexOf("  ") > 0)
            {
                model = model.Replace("  ", " ");
            }
            //request = js.Deserialize<UserRequest>(Request.jsonModel);
            request = js.Deserialize<UserRequest>(model);

            //revisar si tiene permisos de edición
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UserSession session = new UserSession();
                request.EditPermissions = (session.RoleID == new Guid("87E4708C-14FB-426B-A69B-05F28FC5DCFC"));
            }
            else
            {
                request.EditPermissions = false;
            }

            //agregar fechas de eventos
            request.Saved = Request.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt");
            request.Checked = Request.dateDocsChecked != null ? Request.dateDocsChecked.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "";
            request.Approved = Request.dateApproved != null ? Request.dateApproved.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "";
            request.Delivered = Request.dateDelivered != null ? Request.dateDelivered.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "";
            request.DocumentPath = Request.requestDocument ?? "";
            request.DocumentName = Request.requestDocument != null ? Request.requestDocument.Substring(60).Replace("-"," ") : "";
            request.Notes = Request.notes;

            //agregar documentos
            var Documents = from d in db.tblUserDocumentation
                            where d.userRequestID == id
                            select d;

            foreach (var doc in Documents)
            {
                request.Users.FirstOrDefault(x => x.UserTemporalID == doc.userTemporalID).DocumentPath = doc.confidencialityDocument;
                request.Users.FirstOrDefault(x => x.UserTemporalID == doc.userTemporalID).DocumentName = doc.confidencialityDocument.Substring(60).Replace("-"," ");
            }

            return request;
        }

        public static AttemptResponse DeleteUserRequest(Guid userRequestID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var ToDelete = (from u in db.tblUserRequests
                            where u.userRequestID == userRequestID
                            select u).FirstOrDefault();
            try
            {
                db.tblUserRequests.DeleteObject(ToDelete);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = userRequestID;
                response.Message = "";
            }
            catch (Exception x)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to delete the user request.";
            }
            return response;
        }

        public static AttemptResponse SaveRequestNote(Guid userRequestID, string notes)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var UserRequest = (from u in db.tblUserRequests
                                   where u.userRequestID == userRequestID
                                   select u).FirstOrDefault();

                if (UserRequest != null)
                {
                    UserRequest.notes = notes;
                    db.SaveChanges();
                }

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = userRequestID;
                response.Message = "";
            }
            catch (Exception x)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to save the user request notes.";
            }

            return response;
        }

        public static AttemptResponse UserRequestChangeStatus(Guid userRequestID, int eventID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var UserRequest = (from r in db.tblUserRequests
                                   where r.userRequestID == userRequestID
                                   select r).FirstOrDefault();

                if (UserRequest != null)
                {
                    UserRequest request = js.Deserialize<UserRequest>(UserRequest.jsonModel);

                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                    message.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat");
                    message.IsBodyHtml = true;
                    message.Priority = System.Net.Mail.MailPriority.Normal;
                    string body = string.Empty;

                    switch (eventID)
                    {
                        case 1:
                            string email = UserRequest.notifyToEmail;

                            body = "<h2 style=\"font-weight: normal;\">New User Request</h2>";
                            body += "<p>Hi " + request.RequestedBy + ",<br /> I have registered your new user request. Our IT team will check the documentation and will follow up.</p><p><a href=\"https://eplat.villagroup.com/Users/UserRequestFollowUp/" + UserRequest.userRequestID + "\">Click here</a> to check the status of your request.</p><p>Regards,<br />ePlat</p>";

                            message.To.Add(email);
                            message.Bcc.Add("gguerrap@villagroup.com");
                            message.Subject = "New User Request";
                            message.Body = body;
                            //Utils.EmailNotifications.Send(message);
                            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = message } });
                            break;
                        case 2:
                            UserRequest.dateDocsChecked = DateTime.Now;
                            //notificar a Mr Kistner para la aprobación
                            body = "<h2 style=\"font-weight: normal;\">User Request</h2>";
                            body += "<p>Hi Mr. Kistner,<br />I have received a request for " + request.Users.Count() + " new user" + (request.Users.Count() > 1 ? "s" : "") + " to access " + request.TerminalsNames + ":</p>";
                            body += "<ul>";
                            foreach (var user in request.Users)
                            {
                                body += "<li>" + user.FirstName + " " + user.LastName + " as a " + user.JobPosition + "</li>";
                            }
                            body += "</ul>";
                            if (UserRequest.notes != null && UserRequest.notes.Trim() != "")
                            {
                                body += "<br />" + UserRequest.notes + "<br />";
                            }
                            body += "<p>The request was done by " + request.RequestedBy + ". ";
                            body += "The documentation was checked by Gerardo Guerra on " + UserRequest.dateDocsChecked.Value.ToString("yyyy-MM-dd hh:mm:ss tt") + "</p>";
                            body += "<p><a href=\"https://eplat.villagroup.com/Users/UserRequestFollowUp/" + UserRequest.userRequestID + "\">Click here to <b>revise</b> the documentation</a></p>";
                            body += "<p><a href=\"https://eplat.villagroup.com/Users/UserRequestFollowUp/" + UserRequest.userRequestID + "?eventid=3\">Click here to <b>approve</b> the request</a></p>";
                            body += "<p>Regards,<br />ePlat</p>";

                            message.To.Add("robkis@yahoo.com");
                            message.To.Add("lmelendez@villagroup.com");
                            message.Bcc.Add("gguerrap@villagroup.com");
                            message.Subject = "New User Request";
                            message.Body = body;
                            //Utils.EmailNotifications.Send(message);
                            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = message } });
                            break;
                        case 3:
                            UserRequest.dateApproved = DateTime.Now;
                            //notificar a Gerardo que ya fue aprobado, para que asigne la creación.
                            body = "<h2 style=\"font-weight: normal;\">User Request Approved</h2>";
                            body += "<p>Hi Gerardo,<br />A new request for " + request.Users.Count() + " user" + (request.Users.Count() > 1 ? "s" : "") + " from " + request.RequestedBy + " was approved. They need access to " + request.TerminalsNames + ".</p>";
                            body += "<p><a href=\"https://eplat.villagroup.com/Users/UserRequestFollowUp/" + UserRequest.userRequestID + "\">Click here to <b>open</b> the request information</a></p>";
                            body += "<p>Regards,<br />ePlat</p>";

                            message.To.Add("gguerrap@villagroup.com");
                            message.Subject = "User Request Approved";
                            message.Body = body;
                            //Utils.EmailNotifications.Send(message);
                            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = message } });
                            break;
                        case 4:
                            UserRequest.dateDelivered = DateTime.Now;
                            if (HttpContext.Current.User.Identity.IsAuthenticated)
                            {
                                UserSession session = new UserSession();
                                UserRequest.deliveredByUserID = session.UserID;
                            }
                            break;
                        case 5://hold
                            string emailhold = UserRequest.notifyToEmail;

                            body = "<h2 style=\"font-weight: normal;\">User Request in Hold</h2>";
                            body += "<p>Hi " + request.RequestedBy + ",<br /> I have registered your new user request and is now in hold. You can go back to the request and upload the documentation using the next URL:</p><p><a href=\"https://eplat.villagroup.com/Users/UserRequest/" + UserRequest.userRequestID + "\">https://eplat.villagroup.com/Users/UserRequest/" + UserRequest.userRequestID + "</a></p><p>Regards,<br />ePlat</p>";

                            message.To.Add(emailhold);
                            message.Bcc.Add("gguerrap@villagroup.com");
                            message.Subject = "User Request in Hold";
                            message.Body = body;
                            //Utils.EmailNotifications.Send(message);
                            EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = message } });
                            break;
                    }
                    db.SaveChanges();
                }

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = userRequestID;
                response.Message = "";
            }
            catch (Exception x)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Error trying to notify the user request.";
            }

            return response;
        }

        public static AttemptResponse UsersWarning()
        {
            AttemptResponse response = new AttemptResponse();
            List<ActiveUsers.ActiveUser> ActiveUsers = GetActiveUsers();
            List<UsersToWarn> users = new List<UsersToWarn>();
            var currentDate = DateTime.Now.Date;
            foreach (var user in ActiveUsers)
            {
                DateTime lastLog = DateTime.Parse(user.LastActivityDate);
                var daysInactive = (currentDate.Date - lastLog.Date).TotalDays;

                if (daysInactive < user.DaysToLock) //dias antes de bloquear
                {
                    var remDays = user.DaysToLock - daysInactive;
                    var count = (user.DaysToLock - remDays);
                    // if(user.DaysToLock > remDays && remDays < 4)//enviar mensaje 
                    if (count > 0 && count < 4 && user.Username != "root")
                    {
                        UsersToWarn userInactive = new UsersToWarn();
                        userInactive.user = user.Username;
                        userInactive.lastDateLog = user.LastActivityDate;
                        userInactive.email = user.Email;
                        userInactive.emailBody = "Tu Cuenta de " + user.System + " será suspendida por falta de actividad en " + (user.DaysToLock - remDays).ToString() + " días";
                        userInactive.daysBeforeLock = (user.DaysToLock - remDays).ToString();
                        userInactive.system = user.System;
                        users.Add(userInactive);
                    }
                    else//no enviar mensaje
                    {

                    }
                }
                else // usuario bloqueado por inactividad 
                {

                }
            }
            int x = 0;
            foreach (var user in users)
            {
                x++;
                response.Message += x + ".- " + user.user + " " + user.emailBody + Environment.NewLine;
            }

            return response;
        }


        public static AttemptResponse SaveSystemUser(SystemUsers.SystemUser model)
        {
            ePlatEntities eplat = new ePlatEntities();
            ecommerceEntities eplatform = new ecommerceEntities();
            AttemptResponse response = new AttemptResponse();

            //eplatUser
            try
            {
                if (model.system == "ePlat")
                {
                    #region DataEplat
                    var AsignationSupAgent = eplat.tblSupervisors_Agents.Count(x => x.agentUserID == model.userID && x.supervisorUserID == model.supervisorID && x.toDate == null);
                    if (AsignationSupAgent == 1)//verificar si es actualizacion o no
                    {
                        var agent = eplat.tblSupervisors_Agents.Single(x => x.supervisor_AgentID == model.supervisorAgentID);
                        var userInfo = eplat.aspnet_Membership.Single(x => x.UserId == model.userID);
                        var userProfile = eplat.tblUserProfiles.Single(x => x.userID == model.userID);

                        if (agent.supervisorUserID == model.supervisorID)//actualizar
                        {
                            agent.agentUserID = model.userID.Value;
                            if (userInfo.IsLockedOut != model.locked)
                            {
                                userInfo.IsLockedOut = model.locked;
                                userInfo.LastLockoutDate = model.locked ? DateTime.Now : DateTime.Parse("1754-01-01");
                                agent.toDate = model.locked ? DateTime.Now : (DateTime?)null;
                            }
                            userProfile.firstName = model.firstName;
                            userProfile.lastName = model.lastName;

                            response.Message = "The user was updated!";
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = model.userID;
                        }
                        else//agregar
                        {
                            //registro anterior
                            agent.fromDate = agent.fromDate == null ? DateTime.Now.Date : agent.fromDate;
                            agent.toDate = DateTime.Now.Date;

                            //nuevo registro 
                            tblSupervisors_Agents sup = new tblSupervisors_Agents();
                            sup.supervisorUserID = model.supervisorID;
                            sup.agentUserID = model.userID.Value;
                            sup.fromDate = DateTime.Now.Date;
                            sup.toDate = null;
                            sup.dateSaved = DateTime.Now.Date;
                            sup.savedByUserID = session.UserID;
                            eplat.tblSupervisors_Agents.AddObject(sup);

                            response.Message = "The Asignation was saved success!";
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = sup.supervisorUserID;
                        }
                    }
                    else if (AsignationSupAgent == 0)//nueva asignacion
                    {
                        var oldSup = eplat.tblSupervisors_Agents.Single(x => x.supervisor_AgentID == model.supervisorAgentID);
                        oldSup.fromDate = oldSup.fromDate == null ? DateTime.Now.Date : (DateTime?)null;
                        oldSup.toDate = DateTime.Now.Date;
                        oldSup.dateSaved = oldSup.dateSaved == null ? DateTime.Now.Date : oldSup.dateSaved;
                        oldSup.savedByUserID = oldSup.savedByUserID == null ? session.UserID : oldSup.savedByUserID;

                        tblSupervisors_Agents sup = new tblSupervisors_Agents();
                        sup.supervisorUserID = model.supervisorID;
                        sup.agentUserID = model.userID.Value;
                        sup.fromDate = DateTime.Now.Date;
                        sup.toDate = null;
                        sup.dateSaved = DateTime.Now.Date;
                        sup.savedByUserID = session.UserID;
                        eplat.tblSupervisors_Agents.AddObject(sup);

                        response.Message = "The Supervisor was update success!";
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = model.supervisorID;
                    }
                    else// el agente tiene mas de un supervisor, elegir el supervisor que aparece en el modelo
                    {

                    }
                    eplat.SaveChanges();
                    return response;
                    #endregion
                }
                else//ePlatform User
                {
                    var AsignationSupAgent = eplatform.tbaSupervisorAgentes.Count(x => x.idAgente == model.userID && x.idSupervisor == model.supervisorID && x.fechaFinal == null);
                    //verificar si es actualizacion
                    if (AsignationSupAgent == 1) //actualizacion
                    {
                        var agentSup = eplatform.tbaSupervisorAgentes.Single(x => x.idSupervisorAgente == model.supervisorAgentID);
                        var userInfo = eplatform.aspnet_Membership.Single(x => x.UserId == model.userID);
                        var userProfile = eplatform.tbaDirectorio.Single(x => x.idUsuario == model.userID);

                        if (agentSup.idSupervisor == model.supervisorID)//actualizar
                        {
                            agentSup.idAgente = model.userID.Value;
                            if (userInfo.IsLockedOut != model.locked)
                            {
                                userInfo.IsLockedOut = model.locked;
                                userInfo.LastLockoutDate = model.locked ? DateTime.Now : DateTime.Parse("1754-01-01");
                                agentSup.fechaFinal = model.locked ? DateTime.Now : (DateTime?)null;
                            }
                            userProfile.nombre = model.firstName;
                            userProfile.apellido = model.lastName;

                            response.Message = "The user was updated!";
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = model.userID;
                        }
                        else//nuevaasignacion
                        {
                            //registro anterior
                            agentSup.fechaInicial = agentSup.fechaInicial == null ? DateTime.Now.Date : agentSup.fechaInicial;
                            agentSup.fechaFinal = DateTime.Now.Date;

                            //nuevo registro 
                            tbaSupervisorAgentes sup = new tbaSupervisorAgentes();
                            sup.idSupervisor = model.supervisorID;
                            sup.idAgente = model.userID.Value;
                            sup.fechaInicial = DateTime.Now.Date;
                            sup.fechaFinal = null;
                            /*sup.dateSaved = DateTime.Now.Date;
                            sup.savedByUserID = session.UserID;*/
                            eplatform.tbaSupervisorAgentes.AddObject(sup);

                            response.Message = "The Asignation was saved success!";
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = sup.idSupervisorAgente;
                        }
                    }
                    else if (AsignationSupAgent == 0)// nueva asiganacion
                    {
                        var oldSup = eplatform.tbaSupervisorAgentes.Single(x => x.idSupervisorAgente == model.supervisorAgentID);
                        oldSup.fechaInicial = oldSup.fechaInicial == null ? DateTime.Now.Date : (DateTime?)null;
                        oldSup.fechaFinal = DateTime.Now.Date;
                        /*oldSup.dateSaved = oldSup.dateSaved == null ? DateTime.Now.Date : oldSup.dateSaved;
                        oldSup.savedByUserID = oldSup.savedByUserID == null ? session.UserID : oldSup.savedByUserID;*/

                        tbaSupervisorAgentes sup = new tbaSupervisorAgentes();
                        sup.idSupervisor = model.supervisorID;
                        sup.idAgente = model.userID.Value;
                        sup.fechaInicial = DateTime.Now.Date;
                        sup.fechaFinal = null;
                        /*sup.dateSaved = DateTime.Now.Date;
                        sup.savedByUserID = session.UserID;*/
                        eplatform.tbaSupervisorAgentes.AddObject(sup);

                        response.Message = "The Supervisor was update success!";
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = model.supervisorID;
                    }
                    else// mas de una asignacion
                    {

                    }
                    eplatform.SaveChanges();
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error !!";
                response.Type = Attempt_ResponseTypes.Error;
                response.Exception = ex;
                response.ObjectID = "0";
                return response;
            }
        }
        public SystemUsers.SystemUsersModel SearchOrganizationChart(SystemUsers.SearchSubordinates model)
        {
            ePlatEntities db = new ePlatEntities();
            ecommerceEntities eplatform = new ecommerceEntities();
            SystemUsers.SystemUsersModel Model = new SystemUsers.SystemUsersModel();
            List<SystemUsers.OrganizationChartsModel> OrgChartsModel = new List<SystemUsers.OrganizationChartsModel>();
            List<Guid> subordinates = new List<Guid>();
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
            Guid userID = Guid.Empty;
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToList();
            var showOnlyActive = true;

            if (model.userID == (Guid.Parse("00000000-0000-0000-0000-000000000000")))
                model.userID = null;

            if (model.userID == null)
            {
                if (isAdmin)
                {
                    Guid theOne = Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a");
                    subordinates.AddRange(GetUsersEplat(showOnlyActive, new List<Guid?>() { theOne }).Select(m => (Guid)m).ToList());
                    subordinates.AddRange(GetUsersEplatform(showOnlyActive, new List<Guid?>() { /*theOne*/ }).Select(m => (Guid)m).ToList());
                    subordinates.Add(theOne);
                    userID = theOne;
                }
                else
                {
                    //userID
                    Guid currentUserEplatID = (Guid)Membership.GetUser().ProviderUserKey;
                    Guid? currentUserEplatformID = db.tblUserProfiles.FirstOrDefault(x => x.userID == currentUserEplatID) == null ? (Guid?)userID : db.tblUserProfiles.FirstOrDefault(x => x.userID == currentUserEplatID).ecommerceUserID;
                    subordinates.AddRange(GetUsersEplat(showOnlyActive, new List<Guid?>() { currentUserEplatID }).Select(m => (Guid)m).ToList());
                    subordinates.AddRange(GetUsersEplatform(showOnlyActive, new List<Guid?>() { currentUserEplatformID }).Select(m => (Guid)m).ToList());
                    subordinates.Add(currentUserEplatformID.Value);
                    userID = currentUserEplatformID.Value;
                }
            }
            else
            {
                subordinates.AddRange(GetUsersEplat(showOnlyActive, new List<Guid?>() { model.userID }).Select(m => (Guid)m).ToList());
                subordinates.AddRange(GetUsersEplatform(showOnlyActive, new List<Guid?>() { model.userID }).Select(m => (Guid)m).ToList());
                subordinates.Add(model.userID.Value);
                userID = model.userID.Value;
            }
            //buscar infomacion de los subordinados 
            var baseDate = DateTime.Now.Date.AddDays(-16);
            List<SystemUsers.SystemUser> allSubordinates = new List<SystemUsers.SystemUser>();

            if (model.system == "ePlat" || model.system == "0")
            {
                List<SystemUsers.SystemUser> eplatUsers = (from sup in db.tblSupervisors_Agents
                                                           join x in db.aspnet_Users on sup.agentUserID equals x.UserId
                                                           join y in db.tblUserProfiles on sup.agentUserID equals y.userID
                                                           join user in db.tblUserProfiles on sup.supervisorUserID equals user.userID
                                                           join z in db.aspnet_Membership on sup.agentUserID equals z.UserId
                                                           join uj in db.tblUsers_JobPositions on sup.agentUserID equals uj.userID
                                                           join j in db.tblJobPositions on uj.jobPositionID equals j.jobPositionID
                                                           where ((subordinates.Contains(sup.agentUserID) && sup.toDate == null) || isAdmin == true && model.userID == null && sup.toDate == null)
                                                                 && (!z.Email.Contains("test") || !z.Email.Contains("Test") || !z.Email.Contains("example"))
                                                           select new SystemUsers.SystemUser
                                                           {
                                                               userID = x.UserId,
                                                               userName = x.UserName,
                                                               firstName = y.firstName,
                                                               lastName = y.lastName,
                                                               locked = z.IsLockedOut,
                                                               email = z.Email,
                                                               jobPosition = j.jobPosition,
                                                               supervisorID = sup.supervisorUserID,
                                                               system = "ePlat",
                                                               lastActivityDate = x.LastActivityDate,
                                                               supervisorAgentID = sup.supervisor_AgentID,
                                                               supervisorName = user.firstName + " " + user.lastName
                                                           }).ToList();
                allSubordinates.AddRange(eplatUsers);
            }

            if (model.system == "ePlatform" || model.system == "0")
            {
                List<SystemUsers.SystemUser> eplatformUSers =
                                           (from sup in eplatform.tbaSupervisorAgentes
                                            join x in eplatform.aspnet_Users on sup.idAgente equals x.UserId
                                            join z in eplatform.aspnet_Membership on sup.idAgente equals z.UserId
                                            join profile in eplatform.tbaDirectorio on sup.idAgente equals profile.idUsuario
                                            join profileSup in eplatform.tbaDirectorio on sup.idSupervisor equals profileSup.idUsuario
                                            where (subordinates.Contains(sup.idAgente.Value) && sup.fechaFinal == null) || (isAdmin == true && model.userID == null && sup.fechaFinal == null)
                                                  && (!profile.email.Contains("test") || !profile.email.Contains("Test") || !z.Email.Contains("example"))
                                                  && (profileSup.nombre != null && profileSup.apellido != null)
                                            select new SystemUsers.SystemUser
                                            {
                                                userID = x.UserId,
                                                userName = x.UserName,
                                                firstName = profile.nombre,
                                                lastName = profile.apellido,
                                                locked = z.IsLockedOut,
                                                email = profile.email,
                                                jobPosition = profile.puesto,
                                                supervisorID = sup.idSupervisor,
                                                system = "ePlatform",
                                                lastActivityDate = x.LastActivityDate,
                                                supervisorAgentID = sup.idSupervisorAgente,
                                                supervisorName = profileSup.nombre + " " + profileSup.apellido
                                            }).ToList();
                allSubordinates.AddRange(eplatformUSers);
            }
            Model.userList = new List<SystemUsers.SystemUser>();
            foreach (var user in allSubordinates.OrderBy(x => x.userName))
            {
                if (OrgChartsModel.Count(x => x.v == user.userID) == 0)
                {
                    SystemUsers.OrganizationChartsModel item = new SystemUsers.OrganizationChartsModel();
                    item.v = user.userID.Value;
                    item.f = user.firstName + " " + user.lastName + "<div class=" + '"' + "row" + '"' + ">" +
                             "<div class=" + '"' + "col-md-12 col-lg-12 text-center" + '"' + '>'
                             + "<h6>" + user.jobPosition + "</h6>" + "Last Activity Date" + "<div>" + user.lastActivityDate.ToString("yyyy-MM-dd") + "</div>" + "</div>" + "</div>";
                    item.nodoPadre = user.userID == userID ? null : user.supervisorID;
                    //item.nodoPadre = user.supervisorID;
                    if (user.firstName == "Alan" && user.lastName == "Chavez")
                    {

                    }
                    item.comentario = " Lock: " + user.locked;
                    OrgChartsModel.Add(item);
                    Model.userList.Add(user);
                }
            }
            Model.OrgCharts = OrgChartsModel;
            return Model;
        }

        public static DependantFields GetUsersSubordinatesByRole()
        {
            ePlatEntities db = new ePlatEntities();
            ecommerceEntities eplatform = new ecommerceEntities();

            DependantFields df = new DependantFields();
            df.Fields = new List<DependantFields.DependantField>();

            DependantFields.DependantField activeUsers = new DependantFields.DependantField();
            activeUsers.Field = "userID";
            activeUsers.ParentField = "system";
            activeUsers.Values = new List<DependantFields.FieldValue>();

            DependantFields.DependantField supervisorList = new DependantFields.DependantField();
            supervisorList.Field = "supervisorID";
            supervisorList.ParentField = "system";
            supervisorList.Values = new List<DependantFields.FieldValue>();

            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToList();

            activeUsers.Values.Add(new DependantFields.FieldValue()
            {
                ParentValue = "ePlat",
                Text = "-Select One-",
            });
            activeUsers.Values.Add(new DependantFields.FieldValue()
            {
                ParentValue = "ePlatform",
                Text = "-Select One-",
            });

            if (isAdmin)//show all users Actives eplatform and eplat
            {
                Guid theOne = Guid.Parse("c53613b6-c8b8-400d-95c6-274e6e60a14a");
                List<Guid> allUsers = GetUsersEplat(true, new List<Guid?>() { }).Select(x => (Guid)x).ToList();
                allUsers.AddRange(GetUsersEplatform(true, new List<Guid?>() { }).Select(m => (Guid)m).ToList());

                List<SystemUsers.SystemUser> queryEplat = (from user in db.tblUserProfiles
                                                           where allUsers.Contains(user.userID)
                                                                && !user.firstName.Contains("Test") && !user.lastName.Contains("Test") && !user.lastName.Contains("Prueba")
                                                                && !user.lastName.Contains("App")
                                                           select new SystemUsers.SystemUser
                                                           {
                                                               userID = user.userID,
                                                               userName = user.firstName + " " + user.lastName,
                                                               system = "ePlat"
                                                           }).ToList();

                List<SystemUsers.SystemUser> queryEplatform = (from user in eplatform.tbaDirectorio
                                                               where allUsers.Contains(user.idUsuario.Value)
                                                                     && user.nombre != null && user.apellido != null
                                                               select new SystemUsers.SystemUser
                                                               {
                                                                   userID = user.idUsuario,
                                                                   userName = user.nombre + " " + user.apellido,
                                                                   system = "ePlatform"
                                                               }).ToList();

                var users = queryEplat.Concat(queryEplatform);
                foreach (var user in users.OrderBy(x => x.userName))
                {
                    DependantFields.FieldValue item = new DependantFields.FieldValue();
                    item.ParentValue = user.system;
                    item.Text = user.userName;
                    item.Value = user.userID.ToString();
                    supervisorList.Values.Add(item);
                    activeUsers.Values.Add(item);
                }
                df.Fields.Add(activeUsers);
                df.Fields.Add(supervisorList);
            }
            else//show only subordinates
            {
                Guid currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
                List<Guid> allUsers = GetUsersEplat(true, new List<Guid?>() { currentUserID }).Select(x => (Guid)x).ToList();
                allUsers.AddRange(GetUsersEplatform(true, new List<Guid?>() { currentUserID }).Select(m => (Guid)m).ToList());

                var subordinates = GetActiveUsers().Where(x => allUsers.Contains(x.UserID)).ToList();
                foreach (var user in subordinates.OrderBy(x => x.FirstName))
                {
                    DependantFields.FieldValue item = new DependantFields.FieldValue();
                    item.ParentValue = user.System;
                    item.Text = user.FirstName + " " + user.LastName;
                    item.Value = user.UserID.ToString();
                    activeUsers.Values.Add(item);
                }
                df.Fields.Add(activeUsers);
            }
            return df;
        }

        public static List<Guid?> GetUsersEplatform(bool showActive, List<Guid?> userID = null, int? check = 0)
        {
            ecommerceEntities db = new ecommerceEntities();
            List<Guid?> list = new List<Guid?>();
            DateTime refDate = DateTime.Now.AddDays(-16);
            if (userID.Count() == 0)
            {
                list = (from mem in db.aspnet_Membership
                        where ((mem.LastLockoutDate > refDate && mem.IsLockedOut == true)
                              || mem.IsLockedOut == false)
                        select mem.UserId).ToList().Select(m => (Guid?)m).ToList();
            }
            else
            {
                list = (from agents in db.tbaSupervisorAgentes
                        join membershipAgnt in db.aspnet_Membership on agents.idAgente equals membershipAgnt.UserId
                        join membershipSup in db.aspnet_Membership on agents.idSupervisor equals membershipSup.UserId
                        join dirAgnt in db.tbaDirectorio on agents.idAgente equals dirAgnt.idUsuario
                        join dirSup in db.tbaDirectorio on agents.idSupervisor equals dirSup.idUsuario
                        where userID.Contains(agents.idSupervisor)
                              && (((membershipAgnt.LastLockoutDate > refDate && membershipAgnt.IsLockedOut == true) || membershipAgnt.IsLockedOut == false)
                              && ((membershipSup.LastLockoutDate > refDate && membershipSup.IsLockedOut == true) || membershipSup.IsLockedOut == false))
                              && ((dirAgnt.nombre != null && dirAgnt.apellido != null) && (dirSup.nombre != null && dirSup.apellido != null))
                        select agents.idAgente).ToList().Select(m => (Guid?)m).ToList();
            }
            if (list.Count() > 0)
            {
                if (list.Count() == userID.Count())
                    check++;
                if (check < 5)
                    list = list.Concat(GetUsersEplatform(true, list, check)).ToList();
            }
            return list;
        }

        public static List<Guid?> GetUsersEplat(bool showActive, List<Guid?> usersID)
        {
            ePlatEntities db = new ePlatEntities();
            List<Guid?> list = new List<Guid?>();
            DateTime refDate = DateTime.Now.AddDays(-16);
            if (usersID.Count() == 0)
            {
                list = (from membership in db.aspnet_Membership
                        where (membership.LastLockoutDate > refDate && membership.IsLockedOut == true)
                           || membership.IsLockedOut == false
                        select membership.UserId).ToList().Select(m => (Guid?)m).ToList();
            }
            else
            {
                /*list = (from agents in db.tblSupervisors_Agents
                        join membership in db.aspnet_Membership on agents.agentUserID equals membership.UserId
                        where (usersID.Contains(agents.supervisorUserID) && agents.toDate == null)
                             && ((membership.LastLockoutDate > refDate && membership.IsLockedOut == true)
                                || membership.IsLockedOut == false)
                        select agents.agentUserID).ToList().Select(m => (Guid?)m).ToList();*/
                list = (from agents in db.tblSupervisors_Agents
                        join memAgnt in db.aspnet_Membership on agents.agentUserID equals memAgnt.UserId
                        join memSup in db.aspnet_Membership on agents.supervisorUserID equals memSup.UserId
                        where (usersID.Contains(agents.supervisorUserID) && agents.toDate == null)
                             && ((memAgnt.LastLockoutDate > refDate && memAgnt.IsLockedOut == true)
                                || memAgnt.IsLockedOut == false)
                             && ((memSup.LastLockoutDate > refDate && memSup.IsLockedOut == true)
                                || memSup.IsLockedOut == false)
                        select agents.agentUserID).ToList().Select(m => (Guid?)m).ToList();
            }
            if (list.Count() > 0)
            {
                list = list.Concat(GetUsersEplat(true, list)).ToList();
            }
            return list;
        }

        public static List<SelectListItem> GetActiveUsersList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var user in GetActiveUsers())
            {
                list.Add(new SelectListItem()
                {
                    Text = user.FirstName + " " + user.LastName,
                    Value = user.UserID.ToString()
                });
            }
            return list;
        }
    }
}

