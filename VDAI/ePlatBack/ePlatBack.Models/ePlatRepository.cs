
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models;
using System.Data;
using System.Data.Entity;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models
{
    public class ePlatRepository
    {
        private ePlatEntities db = new ePlatEntities();


        /// <summary>
        /// Gets the current logged userID
        /// </summary>
        /// <returns></returns>
        public string CurrentUser()
        {
            try
            {
                MembershipUser user = Membership.GetUser();
                var userID = user.ProviderUserKey.ToString();
                return userID;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the system workgroups assigned to a specific user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public AttemptResponse GetWorkGroupsPerUser(string userName)
        {
            AttemptResponse response = new AttemptResponse();

            if (userName != "")
            {
                WorkGroupsViewModel model = new WorkGroupsViewModel();
                model.WorkGroups = new List<WorkGroupsRelatedModel>();

                List<WorkGroupsRelatedModel> list = new List<WorkGroupsRelatedModel>();
                Guid currentUserID = Guid.Parse("00000000-0000-0000-0000-000000000000");

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    currentUserID = (Guid)Membership.GetUser(HttpContext.Current.User.Identity.Name).ProviderUserKey;
                }
                else
                {
                    if (userName != "" && userName != null)
                    {
                        currentUserID = DataModels.UserDataModel.GetUserID(userName);

                        if (currentUserID == new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            //no existe el usuario
                            response.Message = "The username doesn't exist, please verify if it's correctly typed.";
                            model.UserState = 3;
                            response.Type = Attempt_ResponseTypes.Error;
                        }
                    }
                }

                if (currentUserID != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    var MembershipQ = (from m in db.aspnet_Membership
                                       where m.UserId == currentUserID
                                       select m).FirstOrDefault();

                    if (MembershipQ != null)
                    {
                        if (MembershipQ.IsLockedOut)
                        {
                            model.UserState = 2;
                            //está bloqueado
                            if (MembershipQ.FailedPasswordAttemptWindowStart <= DateTime.UtcNow.AddMinutes(-15))
                            {
                                response.Message = "Your username is locked.";
                                response.Type = Attempt_ResponseTypes.Error;
                            }
                            else
                            {
                                response.Message = "Your username is locked.";
                                model.UserID = currentUserID;
                                response.Type = Attempt_ResponseTypes.Ok;
                            }
                        }
                        else
                        {
                            model.UserState = 1;
                            model.UserID = currentUserID;
                            var query = (from wg in db.tblUsers_SysWorkGroups
                                         where wg.userID == currentUserID
                                         && wg.sysWorkGroupTeamID == null
                                         select wg.sysWorkGroupID).Distinct();

                            if (query.Count() > 0)
                            {
                                response.Type = Attempt_ResponseTypes.Ok;
                                //PENDING - obtener foto


                                //obtener work groups
                                var workgroups = from a in db.tblSysWorkGroups
                                                 where query.Contains(a.sysWorkGroupID)
                                                 select new
                                                 {
                                                     id = a.sysWorkGroupID,
                                                     name = a.sysWorkGroup
                                                 };
                                foreach (var w in workgroups)
                                {

                                    list.Add(new WorkGroupsRelatedModel()
                                    {
                                        WorkGroupID = w.id,
                                        WorkGroupName = w.name
                                    });
                                }
                                model.WorkGroups = list;
                            }
                            else
                            {
                                response.Message = "The username doesn't belong to a Work Group. Please contact your ePlat Administrator.";
                                response.Type = Attempt_ResponseTypes.Error;
                            }
                        }
                    }
                }
                response.ObjectID = model;
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Ok;
            }
            
            return response;
        }

        public AttemptResponse AskForUnlock(Guid userID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                //obtener datos de usuario
                string userSender = "";
                string userFullName = "";
                string userLastLoginDate = "";
                var userQ = (from u in db.tblUserProfiles
                             where u.userID == userID
                             select new
                             {
                                 u.firstName,
                                 u.lastName
                             }).FirstOrDefault();

                if (userQ != null)
                {
                    userFullName = userQ.firstName + " " + userQ.lastName;
                }

                var userM = (from u in db.aspnet_Membership
                             where u.UserId == userID
                             select new
                             {
                                 u.LoweredEmail,
                                 u.LastLoginDate
                             }).FirstOrDefault();

                if (userM != null)
                {
                    userSender = userM.LoweredEmail;
                    userLastLoginDate = userM.LastLoginDate.ToString("yyyyMMddhhmmss");
                }

                //obtener datos del supervisor
                string supervisorEmail = "";
                string supervisorFirstName = "";
                string supervisorFullName = "";

                var SupervisorIDQ = (from s in db.tblSupervisors_Agents
                                     where s.agentUserID == userID
                                     orderby s.supervisor_AgentID
                                     select s.supervisorUserID).FirstOrDefault();
                if (SupervisorIDQ != null)
                {
                    tblUserProfiles supervisorProfile = db.tblUserProfiles.FirstOrDefault(x => x.userID == SupervisorIDQ);
                    if (supervisorProfile != null)
                    {
                        supervisorFullName = supervisorProfile.firstName + " " + supervisorProfile.lastName;
                        supervisorEmail = supervisorProfile.aspnet_Users.aspnet_Membership.LoweredEmail;
                        supervisorFirstName = supervisorProfile.firstName;
                    }
                }

                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email.To.Add(supervisorEmail);
                //email.To.Add("gguerrap@villagroup.com");
                email.From = new System.Net.Mail.MailAddress(userSender, userFullName);
                email.Bcc.Add("gguerrap@villagroup.com");
                email.IsBodyHtml = true;
                email.Priority = System.Net.Mail.MailPriority.High;

                email.Subject = "Please unlock my Credentials";
                email.Body = "<span style=\"font-family:Verdana;font-size:12px;\">Hi " + supervisorFirstName + ",<br /><br />We have received a request from " + userFullName + " to unlock his/her credentials. If you approve to restore the access, <a href=\"https://eplat.villagroup.com/Account/ApproveUnlock/" + userID + "-" + userLastLoginDate + "\">click here</a>. If you are not agree, just discard this email.<br /><br />Regards, <br />ePlat</span>";

                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<ViewModels.MailMessageResponse>() { new ViewModels.MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = "";
                response.Message = "We sent an email to " + supervisorFullName + " asking to unlock your credentials. You will receive and email when your access has been restored.";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                //response.ErrorMessage = ex.Message;
                response.Exception = ex;
                response.Message = "Something weird happened trying to send your Unlock message, please try again.";
                return response;
            }
        }

        public string ApproveUnlock(string userStr)
        {
            string message = "";
            if (userStr.Length == 51)
            {
                string userFirstName = "";
                Guid userID = new Guid(userStr.Substring(0, 36));
                string lastLoginStr = userStr.Substring(37);

                //desbloquear
                var MembershipQ = (from m in db.aspnet_Membership
                                   where m.UserId == userID
                                   select m).FirstOrDefault();

                if (MembershipQ != null)
                {
                    //obtener perfil del usuario
                    var userProfile = (from u in db.tblUserProfiles
                                      where u.userID == userID
                                      select new
                                      {
                                          u.firstName
                                      }).FirstOrDefault();
                    if (userProfile != null)
                    {
                        userFirstName = userProfile.firstName;
                    }

                    if (MembershipQ.IsLockedOut)
                    {
                       string userLastLoginDate = MembershipQ.LastLoginDate.ToString("yyyyMMddhhmmss");

                        if (lastLoginStr == userLastLoginDate)
                        {
                            //URL válida
                            
                            MembershipUser membershipUser = Membership.GetUser(userID);
                            membershipUser.UnlockUser();
                            Membership.UpdateUser(membershipUser);

                            message = "Credentials of " + userFirstName + " has been unlocked. I have sent an email to notify him/her";

                            //notificar
                            string userEmail = MembershipQ.LoweredEmail;
                            System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                            email.To.Add(userEmail);
                            email.From = new System.Net.Mail.MailAddress("ecommerce@villagroup.com", "ePlat");
                            email.Bcc.Add("gguerrap@villagroup.com");
                            email.IsBodyHtml = true;
                            email.Priority = System.Net.Mail.MailPriority.High;

                            email.Subject = "Credentials Unlocked";
                            email.Body = "<span style=\"font-family:Verdana;font-size:12px;\">Hi " + userFirstName + ",<br /><br />Your access to ePlat has been restored.<br /><br />Regards, <br />ePlat</span>";

                            //Utils.EmailNotifications.Send(email);
                            EmailNotifications.Send(new List<ViewModels.MailMessageResponse>() { new ViewModels.MailMessageResponse() { MailMessage = email } });
                        }
                        else
                        {
                            message = "I was not able to unlock this credentials because the URL is invalid.";
                        }
                    }
                    else
                    {
                        message = "Credentials of " + userFirstName + " are not locked, this user should be able to access ePlat.";
                    }
                }
                else
                {
                    message = "I was not able to find the credentials for this User. Should be a problem with the URL.";
                }
            }
            else
            {
                message = "Invalid Url. I was not able to unlock the credentials.";
            }

            return message;
        }

        public AttemptResponse AskForRecoverPassword(Guid userID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                //obtener datos de usuario
                string userFirstName = "";
                string userLastLoginDate = "";
                string userEmail = "";
                var userQ = (from u in db.tblUserProfiles
                             where u.userID == userID
                             select new
                             {
                                 u.firstName,
                                 u.lastName
                             }).FirstOrDefault();

                if (userQ != null)
                {
                    userFirstName = userQ.firstName;
                }

                var userM = (from u in db.aspnet_Membership
                             where u.UserId == userID
                             select new
                             {
                                 u.LoweredEmail,
                                 u.LastLoginDate
                             }).FirstOrDefault();

                if (userM != null)
                {
                    userEmail = userM.LoweredEmail;
                    userLastLoginDate = userM.LastLoginDate.ToString("yyyyMMddhhmmss");
                }

                System.Net.Mail.MailMessage email = new System.Net.Mail.MailMessage();
                email.To.Add(userEmail);
                email.From = new System.Net.Mail.MailAddress("ecommerce@villagroup.com", "ePlat");
                email.Bcc.Add("gguerrap@villagroup.com");
                email.IsBodyHtml = true;
                email.Priority = System.Net.Mail.MailPriority.High;

                email.Subject = "Password Recovery";
                email.Body = "<span style=\"font-family:Verdana;font-size:12px;\">Hi " + userFirstName + ",<br /><br />We have received a request to recover your password. <a href=\"https://eplat.villagroup.com/Account/ResetPassword/" + userID + "-" + userLastLoginDate + "\">Click here</a> and go to the Reset Password page where you can set a new password for your account.<br /><br />Regards, <br />ePlat</span>";

                //Utils.EmailNotifications.Send(email);
                EmailNotifications.Send(new List<ViewModels.MailMessageResponse>() { new ViewModels.MailMessageResponse() { MailMessage = email } });

                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = "";
                response.Message = "We sent an email to " + userEmail + " with a link to reset your password. Please check your email.";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                //response.ErrorMessage = ex.Message;
                response.Exception = ex;
                response.Message = "Something weird happened trying to send your Recovery Password message, please try again.";
                return response;
            }
        }

        public bool ResetPassword(string userStr, ChangePasswordModel model)
        {
            bool changePasswordSucceeded = false;
            try
            {
                if (userStr.Length == 51)
                {
                    Guid userID = new Guid(userStr.Substring(0, 36));
                    string lastLoginStr = userStr.Substring(37);

                    //desbloquear
                    var MembershipQ = (from m in db.aspnet_Membership
                                       where m.UserId == userID
                                       select m).FirstOrDefault();

                    if (MembershipQ != null)
                    {
                        string userLastLoginDate = MembershipQ.LastLoginDate.ToString("yyyyMMddhhmmss");

                        if (lastLoginStr == userLastLoginDate)
                        {
                            MembershipUser membershipUser = Membership.GetUser(userID);
                            changePasswordSucceeded = membershipUser.ChangePassword(membershipUser.ResetPassword(), model.NewPassword);
                        }
                    }
                }
            }
            catch (Exception)
            {
                changePasswordSucceeded = false;
            }
            return changePasswordSucceeded;
        }

        public bool ValidResetPasswordUrl(string userStr)
        {
            bool valid = false;
            if (userStr.Length == 51)
            {
                Guid userID = new Guid(userStr.Substring(0, 36));
                string lastLoginStr = userStr.Substring(37);

                //desbloquear
                var MembershipQ = (from m in db.aspnet_Membership
                                   where m.UserId == userID
                                   select m).FirstOrDefault();

                if (MembershipQ != null)
                {
                    string userLastLoginDate = MembershipQ.LastLoginDate.ToString("yyyyMMddhhmmss");

                    if (lastLoginStr == userLastLoginDate)
                    {
                        valid = true;
                    }
                }
            }
            return valid;
        }

        public SessionDetails GetSessionDetails()
        {
            SessionDetails details = new SessionDetails();
            UserSession session = new UserSession();

            details.Username = session.User;
            var Profile = (from p in db.tblUserProfiles
                          where p.userID == session.UserID
                          select new
                          {
                              p.firstName,
                              p.lastName,
                              p.culture,
                              p.profilePicturePath
                          }).Single();
            details.FirstName = Profile.firstName;
            details.LastName = Profile.lastName;
            details.Photo = Profile.profilePicturePath;
            details.Culture = Profile.culture;
            //obtener roles
            var RolesWG = (from r in db.tblUsers_SysWorkGroups
                        join role in db.aspnet_Roles on r.roleID equals role.RoleId
                        into r_role from role in r_role.DefaultIfEmpty()
                        join wg in db.tblSysWorkGroups on r.sysWorkGroupID equals wg.sysWorkGroupID
                        into r_wg from wg in r_wg.DefaultIfEmpty()
                        where r.userID == session.UserID
                        select new
                        {
                            r.roleID,
                            role.RoleName,
                            r.sysWorkGroupID,
                            wg.sysWorkGroup
                        }).ToList();

            var UserTerminals = from u in db.tblUsers_Terminals
                                join terminal in db.tblTerminals on u.terminalID equals terminal.terminalID
                                into u_terminal from terminal in u_terminal.DefaultIfEmpty()
                                where u.userID == session.UserID
                                select new
                                {
                                    u.terminalID,
                                    terminal.terminal,
                                    terminal.sysWorkGroupID
                                };
            details.UserID = session.UserID;
            details.UserTerminals = new List<UserTerminal>();
            foreach (var rwg in RolesWG)
            {
                foreach (var t in UserTerminals.Where(x => x.sysWorkGroupID == rwg.sysWorkGroupID))
                {
                    UserTerminal trwg = new UserTerminal();
                    trwg.TerminalID = t.terminalID;
                    trwg.Terminal = t.terminal;
                    trwg.WorkGroupID = rwg.sysWorkGroupID;
                    trwg.WorkGroup = rwg.sysWorkGroup;
                    trwg.RoleID = rwg.roleID;
                    trwg.Role = rwg.RoleName;
                    trwg.Visible = true;
                    details.UserTerminals.Add(trwg);
                }
            }
            details.UserTerminals = details.UserTerminals.OrderBy(x => x.Terminal).ToList();

            details.TerminalIDs = session.Terminals;
            details.WorkGroupID = session.WorkGroupID;
            if (session.WorkGroupID != null)
            {
                details.WorkGroup = RolesWG.FirstOrDefault(x => x.sysWorkGroupID == session.WorkGroupID).sysWorkGroup;
            }
            details.RoleID = session.RoleID;
            if (session.RoleID != null)
            {
                details.Role = RolesWG.FirstOrDefault(x => x.roleID == session.RoleID).RoleName;
            }
            
            return details;
        }

        /// <summary>
        /// Gets the system roles assigned to a specific user based on his/her system workgroup
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="workGroupID"></param>
        /// <returns></returns>
        public List<RolesModel> GetRolesByUser(string userName, int workGroupID)
        {
            List<RolesModel> list = new List<RolesModel>();
            Guid currentUser = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (userName != null && userName != "")
            {
                currentUser = (Guid)Membership.GetUser(userName).ProviderUserKey;
            }
            else
            {
                try
                {
                    currentUser = (Guid)Membership.GetUser().ProviderUserKey;
                }
                catch
                {
                }
            }
            var query = from roles in db.tblUsers_SysWorkGroups
                        where roles.userID == currentUser
                        && roles.sysWorkGroupID == workGroupID
                        select new
                        {
                            roleID = roles.roleID,
                            roleName = roles.aspnet_Roles.RoleName
                        };

            foreach (var i in query)
            {
                list.Add(new RolesModel()
                {
                    RoleID = i.roleID,
                    RoleName = i.roleName
                });
            }
            return (list);
        }

        /// <summary>
        /// Gets all the Job Positions registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpJobPositions()
        {
            List<SelectListItem> jp = new List<SelectListItem>();
            jp.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            foreach (var position in db.tblJobPositions)
                jp.Add(new SelectListItem()
                {
                    Value = position.jobPositionID.ToString(),
                    Text = position.jobPosition.ToString(),
                });
            //uvm.DrpJobPositions = jp;
            return jp;
        }

        /// <summary>
        /// Gets all Categories registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpParentCategories()
        {
            List<SelectListItem> c = new List<SelectListItem>();
            c.Add(new SelectListItem()
            {
                Value = "0",
                Text = "--Select One--",
                Selected = false
            });
            var query = from pc in db.tblCategories
                        //where pc.catalogID == catalogID
                        select pc;

            foreach (var i in query)
            {
                c.Add(new SelectListItem()
                {
                    Value = i.categoryID.ToString(),
                    Text = i.category.ToString(),
                });
            }
            return c;
        }

        /// <summary>
        /// Gets all Destinations registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpDestinations()
        {
            List<SelectListItem> d = new List<SelectListItem>();
            d.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            foreach (var destination in db.tblDestinations)
                d.Add(new SelectListItem()
                {
                    Value = destination.destinationID.ToString(),
                    Text = destination.destination.ToString(),
                });
            //uvm.DrpDestinations = d;
            return d;
        }

        /// <summary>
        /// Gets all system workgroups registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpWorkGroups()
        {
            List<SelectListItem> wg = new List<SelectListItem>();
            wg.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            foreach (var workgroup in db.tblSysWorkGroups)
                wg.Add(new SelectListItem()
                {
                    Value = workgroup.sysWorkGroupID.ToString(),
                    Text = workgroup.sysWorkGroup.ToString(),
                });
            return wg;
        }

        /// <summary>
        /// Gets all system roles registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpRoles()
        {
            List<SelectListItem> r = new List<SelectListItem>();
            r.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            foreach (var role in db.aspnet_Roles)
                r.Add(new SelectListItem()
                {
                    Value = role.RoleId.ToString(),
                    Text = role.RoleName.ToString(),
                });
            //uvm.DrpRoles = r;
            return r;
        }

        /// <summary>
        /// Gets all Terminals registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpTerminals()
        {
            List<SelectListItem> t = new List<SelectListItem>();
            t.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            foreach (var terminal in db.tblTerminals)
                t.Add(new SelectListItem()
                {
                    Value = terminal.terminalID.ToString(),
                    Text = terminal.terminal.ToString(),
                });
            //uvm.DrpTerminals = t;
            return t;
        }

        /// <summary>
        /// Gets all users in the specified system workgroup
        /// </summary>
        /// <param name="workGroupID"></param>
        /// <returns></returns>
        public List<SelectListItem> FillDrpSupervisors(int workGroupID)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });

            var query = from us in db.tblUsers_SysWorkGroups
                        join up in db.tblUserProfiles on us.userID equals up.userID
                        where us.sysWorkGroupID == workGroupID
                        select new
                        {
                            id = us.userID,
                            fn = up.firstName,
                            ln = up.lastName
                        };

            foreach (var u in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = u.id.ToString(),
                    Text = u.fn.ToString() + " " + u.ln.ToString()
                });
            }

            return list;
        }

        /// <summary>
        /// Gets all types of system components registered
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> FillDrpComponentTypes()
        {
            List<SelectListItem> c = new List<SelectListItem>();
            c.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            foreach (var componentType in db.tblSysComponentTypes)
                c.Add(new SelectListItem()
                {
                    Value = componentType.sysComponentTypeID.ToString(),
                    Text = componentType.sysComponentType.ToString(),
                });
            return c;
        }

        /// <summary>
        /// Creates a new user in system
        /// </summary>
        /// <param name="urm"></param>
        //public void AddUser(UserRegistryModel_ urm)
        //{
        //    //var validUser = from user in db.tblUsers_SysWorkGroups
        //    //                where user.userID == urm.NSession
        //    //                && user.tblSysWorkGroups.sysWorkGroup == urm.selectedWorkGroup
        //    //                select user.aspnet_Users.aspnet_Roles.FirstOrDefault(m => m.RoleId == user.roleID).RoleName;

        //    tblUserProfiles uProfile = new tblUserProfiles();

        //    uProfile.userID = urm.NUserID;
        //    uProfile.firstName = urm.NFirstName;
        //    uProfile.lastName = urm.NLastName;
        //    uProfile.SPIUserName = urm.NSPIUserName;
        //    db.AddTotblUserProfiles(uProfile);

        //    if (urm.DrpNSupervisors != null)
        //    {
        //        for (var i = 0; i < urm.DrpNSupervisors.Count(); i++)
        //        {
        //            tblSupervisors_Agents supervisor = new tblSupervisors_Agents();
        //            supervisor.agentUserID = urm.NUserID;
        //            supervisor.supervisorUserID = urm.DrpNSupervisors[i];
        //            db.AddTotblSupervisors_Agents(supervisor);
        //        }
        //    }

        //    if (urm.DrpNJobPositions != null)
        //    {
        //        for (var i = 0; i < urm.DrpNJobPositions.Count(); i++)
        //        {
        //            tblUsers_JobPositions uJPosition = new tblUsers_JobPositions();
        //            uJPosition.userID = urm.NUserID;
        //            uJPosition.jobPositionID = urm.DrpNJobPositions[i];
        //            uJPosition.fromDate = DateTime.Now;
        //            db.AddTotblUsers_JobPositions(uJPosition);
        //        }
        //    }
        //    if (urm.DrpNDestinations != null)
        //    {
        //        for (var i = 0; i < urm.DrpNDestinations.Count(); i++)
        //        {
        //            tblUsers_Destinations uDestination = new tblUsers_Destinations();
        //            uDestination.userID = urm.NUserID;
        //            uDestination.destinationID = urm.DrpNDestinations[i];
        //            uDestination.dateSaved = DateTime.Now;
        //            uDestination.savedByUserID = urm.NSession;
        //            db.AddTotblUsers_Destinations(uDestination);
        //        }
        //    }
        //    if (urm.DrpNWorkGroups != null)
        //    {
        //        for (var i = 0; i < urm.DrpNWorkGroups.Count(); i++)
        //        {
        //            tblUsers_SysWorkGroups uWorkgroup = new tblUsers_SysWorkGroups();
        //            uWorkgroup.userID = urm.NUserID;
        //            uWorkgroup.sysWorkGroupID = urm.DrpNWorkGroups[i];
        //            uWorkgroup.roleID = urm.DrpNRoles[i];
        //            uWorkgroup.dateSaved = DateTime.Now;
        //            uWorkgroup.savedByUserID = urm.NSession;
        //            db.AddTotblUsers_SysWorkGroups(uWorkgroup);
        //            var roleID = urm.DrpNRoles[i];
        //            Roles.AddUserToRole(urm.NUserName, (from role in db.aspnet_Roles 
        //                                                where role.RoleId == roleID
        //                                                select role.RoleName).Single());
        //        }


        //    }
        //    if (urm.DrpNTerminals != null)
        //    {
        //        for (var i = 0; i < urm.DrpNTerminals.Count(); i++)
        //        {
        //            tblUsers_Terminals uTerminal = new tblUsers_Terminals();
        //            uTerminal.userID = urm.NUserID;
        //            uTerminal.terminalID = urm.DrpNTerminals[i];
        //            uTerminal.dateSaved = DateTime.Now;
        //            uTerminal.savedByUserID = urm.NSession;
        //            db.AddTotblUsers_Terminals(uTerminal);
        //        }
        //    }
        //    db.SaveChanges();
        //}

        /// <summary>
        /// Creates a new system component
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int createNewComponent(NewComponentModel model)
        {
            tblSysComponents component = new tblSysComponents();
            component.sysComponent = model.SysComponent;
            component.sysComponentTypeID = model.SysComponentTypeID;
            component.sysParentComponentID = model.SysParentComponentID;
            component.description = model.Description;
            db.tblSysComponents.AddObject(component);
            db.SaveChanges();
            return int.Parse(component.sysComponentID.ToString());
        }

        /// <summary>
        /// Gets data of the specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        //public List<UserModel> getUsers(Guid userID)
        //{
        //    List<UserModel> list = new List<UserModel>();

        //    var query = from us in db.aspnet_Users
        //                join up in db.tblUserProfiles on us.UserId equals up.userID
        //                where us.UserId.Equals(userID)
        //                select new
        //                {
        //                    id = us.UserId,
        //                    userName = us.UserName,
        //                    firstName = up.firstName,
        //                    lastName = up.lastName,
        //                    spiUSerName = up.SPIUserName,
        //                    email = us.aspnet_Membership.LoweredEmail,
        //                    isApproved = us.aspnet_Membership.IsApproved,
        //                    isLockedOut = us.aspnet_Membership.IsLockedOut,
        //                    activityDate = us.LastActivityDate,
        //                };

        //    var queryJP = from jp in db.tblUsers_JobPositions
        //                  where jp.userID == userID
        //                  select jp.tblJobPositions.jobPosition;
        //    var jobPositions = queryJP.ToArray();
        //    var queryD = from d in db.tblUsers_Destinations
        //                 where d.userID == userID
        //                 select d.tblDestinations.destination;
        //    var destinations = queryD.ToArray();
        //    var queryT = from t in db.tblUsers_Terminals
        //                 where t.userID == userID
        //                 select t.tblTerminals.terminal;
        //    var terminals = queryT.ToArray();
        //    var queryWG = from wg in db.tblUsers_SysWorkGroups
        //                  where wg.userID == userID
        //                  select wg.tblSysWorkGroups.sysWorkGroup;
        //    var workgroups = queryWG.ToArray();
        //    var queryR = from r in db.aspnet_Roles
        //                 join vwr in db.tblUsers_SysWorkGroups on r.RoleId equals vwr.roleID
        //                 where vwr.userID == userID
        //                 select r.RoleName;
        //    var roles = queryR.ToArray();

        //    var queryS = from s in db.tblSupervisors_Agents
        //                 join up in db.tblUserProfiles on s.agentUserID equals up.userID
        //                 where s.agentUserID == userID
        //                 select s.supervisorUserID;

        //    var supervisors = "";
        //    foreach (var i in queryS)
        //    {
        //        var query2 = from a in db.tblUserProfiles
        //                     where a.userID == i
        //                     select new {
        //                         fn = a.firstName,
        //                         ln = a.lastName
        //                     };
        //        foreach (var u in query2)
        //        {
        //            supervisors += u.fn.ToString() + " " + u.ln.ToString()+",";
        //        }
        //    }

        //    foreach (var i in query)
        //    {
        //        list.Add(new UserModel()
        //        {
        //            NUserID = i.id,
        //            NUserName = i.userName,
        //            NFirstName = i.firstName,
        //            NLastName = i.lastName,
        //            NSPIUserName = i.spiUSerName,
        //            NEmail = i.email,
        //            NIsApproved = i.isApproved,
        //            NIsLockedOut = i.isLockedOut,
        //            DrpNSupervisors = supervisors,
        //            DrpNJobPositions = jobPositions,
        //            DrpNDestinations = destinations,
        //            DrpNTerminals = terminals,
        //            DrpNWorkGroups = workgroups,
        //            DrpNRoles = roles
        //        });
        //    }

        //    return list;
        //}

        /// <summary>
        /// Updates data of a specific user
        /// </summary>
        /// <param name="urm">UserRegistryModel properties</param>
        //public void UpdateUser(UserRegistryModel_ urm)
        //{
        //    var oldData = (from us in db.tblUserProfiles
        //                  where us.userID == urm.NUserID
        //                  select us).First();

        //        oldData.firstName = urm.NFirstName;
        //        oldData.lastName = urm.NLastName;
        //        oldData.SPIUserName = urm.NSPIUserName;

        //        if (urm.DrpNSupervisors != null)
        //    {
        //        var oldRows = from rows in db.tblSupervisors_Agents
        //                      where rows.agentUserID == urm.NUserID
        //                      select rows;
        //        foreach (var oldRow in oldRows)
        //        {
        //            db.tblSupervisors_Agents.DeleteObject(oldRow);
        //        }
        //        for (var i = 0; i < urm.DrpNSupervisors.Count(); i++)
        //        {
        //            tblSupervisors_Agents supervisor = new tblSupervisors_Agents();
        //            supervisor.agentUserID = urm.NUserID;
        //            supervisor.supervisorUserID = urm.DrpNSupervisors[i];
        //            db.AddTotblSupervisors_Agents(supervisor);
        //        }
        //    }

        //    if (urm.DrpNJobPositions != null)
        //    {
        //        var oldRows = from rows in db.tblUsers_JobPositions
        //                      where rows.userID == urm.NUserID
        //                      select rows;
        //        foreach (var oldRow in oldRows)
        //        {
        //            db.tblUsers_JobPositions.DeleteObject(oldRow);
        //        }
        //        for (var i = 0; i < urm.DrpNJobPositions.Count(); i++)
        //        {
        //            tblUsers_JobPositions uJPosition = new tblUsers_JobPositions();
        //            uJPosition.userID = urm.NUserID;
        //            uJPosition.jobPositionID = urm.DrpNJobPositions[i];
        //            db.AddTotblUsers_JobPositions(uJPosition);
        //        }
        //    }
        //    if (urm.DrpNDestinations != null)
        //    {
        //        var oldRows = from rows in db.tblUsers_Destinations
        //                      where rows.userID == urm.NUserID
        //                      select rows;
        //        foreach (var oldRow in oldRows)
        //        {
        //            db.tblUsers_Destinations.DeleteObject(oldRow);
        //        }
        //        for (var i = 0; i < urm.DrpNDestinations.Count(); i++)
        //        {
        //            tblUsers_Destinations uDestination = new tblUsers_Destinations();
        //            uDestination.userID = urm.NUserID;
        //            uDestination.destinationID = urm.DrpNDestinations[i];
        //            uDestination.dateSaved = DateTime.Now;
        //            uDestination.savedByUserID = urm.NSession;
        //            db.AddTotblUsers_Destinations(uDestination);
        //        }
        //    }

        //    if (urm.DrpNWorkGroups != null)
        //    {
        //        var oldRows = from rows in db.tblUsers_SysWorkGroups
        //                      where rows.userID == urm.NUserID
        //                      select rows;
        //        foreach (var oldRow in oldRows)
        //        {
        //            db.tblUsers_SysWorkGroups.DeleteObject(oldRow);
        //        }
        //        for (var i = 0; i < urm.DrpNWorkGroups.Count(); i++)
        //        {
        //            tblUsers_SysWorkGroups uWorkgroup = new tblUsers_SysWorkGroups();
        //            uWorkgroup.userID = urm.NUserID;
        //            uWorkgroup.sysWorkGroupID = urm.DrpNWorkGroups[i];
        //            uWorkgroup.roleID = urm.DrpNRoles[i];
        //            uWorkgroup.dateSaved = DateTime.Now;
        //            uWorkgroup.savedByUserID = urm.NSession;
        //            uWorkgroup.dateSaved = DateTime.Now;
        //            db.AddTotblUsers_SysWorkGroups(uWorkgroup);
        //        }
        //    }
        //    if (urm.DrpNTerminals != null)
        //    {
        //        var oldRows = from rows in db.tblUsers_Terminals
        //                      where rows.userID == urm.NUserID
        //                      select rows;
        //        foreach (var oldRow in oldRows)
        //        {
        //            db.tblUsers_Terminals.DeleteObject(oldRow);
        //        }
        //        for (var i = 0; i < urm.DrpNTerminals.Count(); i++)
        //        {
        //            tblUsers_Terminals uTerminal = new tblUsers_Terminals();
        //            uTerminal.userID = urm.NUserID;
        //            uTerminal.terminalID = urm.DrpNTerminals[i];
        //            uTerminal.dateSaved = DateTime.Now;
        //            uTerminal.savedByUserID = urm.NSession;
        //            db.AddTotblUsers_Terminals(uTerminal);
        //        }
        //    }
        //    db.SaveChanges();
        //}
        //**********
        //********** gives a list of available fields depending on workgroup/role
        /*public List<VisibleFields> availableFields(string currentWorkGroup, string currentRole)
        {
            List<VisibleFields> fieldsList = new List<VisibleFields>();
            var b = from i in db.aspnet_Roles
                    where i.RoleName == currentRole
                    select i.RoleId;
            var r = (Guid)b.Single();
            var wg = int.Parse(currentWorkGroup);
            var a = from i in db.tblSysProfiles
                    join j in db.tblSysComponentAliases on i.sysComponentID equals j.sysComponentID
                    where i.sysWorkGroupID == wg
                    && j.sysWorkGroupID == wg
                    && i.roleID == r
                    select new
                    {
                        field = i.sysComponentID,
                        alias = j.alias,
                        create = i.create_,
                        edit = i.edit_,
                        view = i.view_
                    };
            
            foreach (var x in a)
            {
                fieldsList.Add(new VisibleFields()
                {
                    Component = Int32.Parse(x.field.ToString()),
                    Alias = x.alias,
                    Create = x.create,
                    Edit = x.edit,
                    View = x.view
                });
            }

            return fieldsList;
        }
        //**********
         * */

        //public void updatePrivileges(SysProfileModel model)
        //{
        //    tblSysProfiles updatedProfile = db.tblSysProfiles.Single(m => m.sysProfileID == model.SysProfileID);
        //    //check if alias exists
        //    if (model.Alias != null)
        //    {
        //        var query = from a in db.tblSysComponentAliases
        //                    where a.sysWorkGroupID == model.SysWorkGroupID
        //                    && a.sysComponentID == model.SysComponentID
        //                    select a.sysComponentAliasID;
        //        var existingAlias = query.SingleOrDefault();

        //        if (existingAlias != 0)
        //        {
        //            tblSysComponentAliases updatedAlias = db.tblSysComponentAliases.Single(m => m.sysComponentAliasID == existingAlias);
        //            updatedAlias.alias = model.Alias;
        //        }
        //        else
        //        {
        //            tblSysComponentAliases alias = new tblSysComponentAliases();
        //            alias.sysWorkGroupID = model.SysWorkGroupID;
        //            alias.alias = model.Alias;
        //            alias.sysComponentID = model.SysComponentID;
        //            db.AddTotblSysComponentAliases(alias);
        //        }
        //    }
        //    //tblSysComponentsOrder updatedComponentOrder = db.tblSysComponentsOrder.Single(m => m.sysComponentOrderID == existingComponentOrder);
        //    //updatedComponentOrder.sysComponentOrder = model.SysComponentOrder;
        //    updatedProfile.create_ = model.Create_;
        //    updatedProfile.edit_ = model.Edit_;
        //    updatedProfile.view_ = model.View_;
        //    updatedProfile.sysComponentOrder = model.SysComponentOrder;
        //    db.SaveChanges();
        //}

        //**********
        //********** inserts new workgroup/role profile to database
        //public void createPrivileges(SysProfileModel model)
        //{
        //    tblSysProfiles sysProfile = new tblSysProfiles();
        //    tblSysComponentAliases alias = new tblSysComponentAliases();
        //    tblSysComponentsOrder order = new tblSysComponentsOrder();
        //    sysProfile.roleID = model.RoleID;
        //    sysProfile.sysWorkGroupID = model.SysWorkGroupID;
        //    sysProfile.sysComponentID = model.SysComponentID;
        //    sysProfile.create_ = model.Create_;
        //    sysProfile.edit_ = model.Edit_;
        //    sysProfile.view_ = model.View_;
        //    db.AddTotblSysProfiles(sysProfile);
        //    alias.sysWorkGroupID = model.SysWorkGroupID;
        //    alias.alias = model.Alias;
        //    alias.sysComponentID = model.SysComponentID;
        //    db.AddTotblSysComponentAliases(alias);
        //    order.sysWorkGroupID = model.SysWorkGroupID;
        //    order.sysComponentOrder = model.SysComponentOrder;
        //    order.sysComponentID = model.SysComponentID;
        //    db.AddTotblSysComponentsOrder(order);
        //    db.SaveChanges();
        //}

        /// <summary>
        /// Creates a system profile of a just created system component
        /// </summary>
        /// <param name="model"></param>
        public void createPrivileges(NewComponentModel model)
        {
            tblSysProfiles sysProfile = new tblSysProfiles();
            sysProfile.roleID = model.RoleID;
            sysProfile.sysWorkGroupID = model.SysWorkGroupID;
            sysProfile.sysComponentID = model.SysComponentID;
            sysProfile.sysComponentOrder = model.SysComponentOrder;
            db.tblSysProfiles.AddObject(sysProfile);
            db.SaveChanges();
        }

        /// <summary>
        /// Finds the components of the navigation menu filtered by the system workgroup and role of the user
        /// </summary>
        /// <param name="selectedWorkGroupID"></param>
        /// <param name="selectedRoleID"></param>
        /// <returns></returns>
        public List<MenuComponentsModel> MenuComponents(int selectedWorkGroupID, Guid selectedRoleID)
        {
            List<MenuComponentsModel> list = new List<MenuComponentsModel>();

            var query = from profile in db.tblSysProfiles
                        join component in db.tblSysComponents on profile.sysComponentID equals component.sysComponentID
                        where profile.sysWorkGroupID == selectedWorkGroupID
                        && profile.roleID == selectedRoleID
                        && (component.sysComponentTypeID == 1 || component.sysComponentTypeID == 2 || component.sysComponentTypeID == 18)
                        && profile.view_ == true
                        orderby profile.tblSysComponents.sysParentComponentID, profile.sysComponentOrder, component.sysComponent
                        //orderby profile.tblSysComponents.sysParentComponentID, profile.sysComponentOrder
                        select new
                        {
                            sysComponentID = profile.sysComponentID,
                            sysParentComponentID = component.sysParentComponentID,
                            componentUrl = component.url,
                            componentAlias = component.tblSysComponentAliases.FirstOrDefault(m => m.sysWorkGroupID == selectedWorkGroupID).alias,
                            componentName = component.sysComponent,
                            componentType = component.sysComponentTypeID
                        };
            foreach (var component in query)
            {
                var parentComponent = 0;
                if (component.sysParentComponentID != null)
                {
                    parentComponent = int.Parse(component.sysParentComponentID.ToString());
                }
                if (component.componentType == 18)
                {
                    list.Add(new MenuComponentsModel()
                    {
                        SysComponentID = int.Parse(component.sysComponentID.ToString()),
                        SysParentComponentID = parentComponent,
                        SysComponentTypeID = component.componentType,
                        SysComponentName = component.componentName,
                        SysComponentUrl = "/pages/container/" + component.sysComponentID.ToString(),
                        SysComponentAlias = component.componentAlias
                    });
                }
                else
                {
                    list.Add(new MenuComponentsModel()
                    {
                        SysComponentID = int.Parse(component.sysComponentID.ToString()),
                        SysParentComponentID = parentComponent,
                        SysComponentTypeID = component.componentType,
                        SysComponentName = component.componentName,
                        SysComponentUrl = component.componentUrl,
                        SysComponentAlias = component.componentAlias
                    });
                }
            }
            list = list.Where(x => x.SysParentComponentID == 0).OrderBy(x => x.SysParentComponentID).Concat(list.Where(x => x.SysParentComponentID != 0).OrderBy(x => x.SysParentComponentID).ThenBy(x => x.SysComponentName)).ToList();
            return list;
        }

        /// <summary>
        /// Saves changes made to booking Status, lead Types, Marketing Sources, Qualification Requirements,
        /// Resort Fee Types, Reservation Status, and Verification Agreements related to a specified workgroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Boolean saveWorkGroupRelatedElements(WorkGroupsComponentsModel model)
        {
            var oldBookingStatus = from old in db.tblSysWorkGroups_BookingStatus
                                   where old.sysWorkGroupID == model.SysWorkGroupID
                                   select old;
            foreach (var row in oldBookingStatus)
            {
                db.tblSysWorkGroups_BookingStatus.DeleteObject(row);
            }
            if (model.BookingStatus != null)
            {
                for (var i = 0; i < model.BookingStatus.Count(); i++)
                {
                    tblSysWorkGroups_BookingStatus table = new tblSysWorkGroups_BookingStatus();
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    table.bookingStatusID = model.BookingStatus[i].MainItemID;
                    table.orderIndex = model.BookingStatus[i].SecondItemID;
                    db.AddTotblSysWorkGroups_BookingStatus(table);
                }
            }

            var oldLeadTypes = from old in db.tblLeadTypes_SysWorkGroups
                               where old.sysWorkGroupID == model.SysWorkGroupID
                               select old;
            foreach (var row in oldLeadTypes)
            {
                db.tblLeadTypes_SysWorkGroups.DeleteObject(row);
            }
            if (model.LeadTypes != null)
            {
                for (var i = 0; i < model.LeadTypes.Count(); i++)
                {
                    tblLeadTypes_SysWorkGroups table = new tblLeadTypes_SysWorkGroups();
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    table.leadTypeID = model.LeadTypes[i];
                    db.AddTotblLeadTypes_SysWorkGroups(table);
                }
            }

            var oldMarketingSources = from old in db.tblLeadSources_SysWorkGroups
                                      where old.sysWorkGroupID == model.SysWorkGroupID
                                      select old;
            foreach (var row in oldMarketingSources)
            {
                db.tblLeadSources_SysWorkGroups.DeleteObject(row);
            }
            if (model.MarketingSources != null)
            {
                for (var i = 0; i < model.MarketingSources.Count(); i++)
                {
                    tblLeadSources_SysWorkGroups table = new tblLeadSources_SysWorkGroups();
                    table.leadSourceID = Int64.Parse(model.MarketingSources[i].ToString());
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    db.AddTotblLeadSources_SysWorkGroups(table);
                }
            }

            var oldQualificationRequirements = from old in db.tblSysWorkGroups_QualificationRequirements
                                               where old.sysWorkGroupID == model.SysWorkGroupID
                                               select old;
            foreach (var row in oldQualificationRequirements)
            {
                db.tblSysWorkGroups_QualificationRequirements.DeleteObject(row);
            }
            if (model.QualificationRequirements != null)
            {
                for (var i = 0; i < model.QualificationRequirements.Count(); i++)
                {
                    tblSysWorkGroups_QualificationRequirements table = new tblSysWorkGroups_QualificationRequirements();
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    table.qualificationRequirementID = int.Parse(model.QualificationRequirements[i].ToString());
                    db.AddTotblSysWorkGroups_QualificationRequirements(table);
                }
            }

            var oldResortFeeTypes = from old in db.tblSysWorkGroups_ResortFeeTypes
                                    where old.sysWorkGroupID == model.SysWorkGroupID
                                    select old;
            foreach (var row in oldResortFeeTypes)
            {
                db.tblSysWorkGroups_ResortFeeTypes.DeleteObject(row);
            }
            if (model.ResortFeeTypes != null)
            {
                for (var i = 0; i < model.ResortFeeTypes.Count(); i++)
                {
                    tblSysWorkGroups_ResortFeeTypes table = new tblSysWorkGroups_ResortFeeTypes();
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    table.resortFeeTypeID = int.Parse(model.ResortFeeTypes[i].ToString());
                    db.AddTotblSysWorkGroups_ResortFeeTypes(table);
                }
            }

            var oldReservationStatus = from old in db.tblReservationStatus_SysWorkGroups
                                       where old.sysWorkGroupID == model.SysWorkGroupID
                                       select old;
            foreach (var row in oldReservationStatus)
            {
                db.tblReservationStatus_SysWorkGroups.DeleteObject(row);
            }
            if (model.ReservationStatus != null)
            {
                for (var i = 0; i < model.ReservationStatus.Count(); i++)
                {
                    tblReservationStatus_SysWorkGroups table = new tblReservationStatus_SysWorkGroups();
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    table.reservationStatusID = int.Parse(model.ReservationStatus[i].ToString());
                    db.AddTotblReservationStatus_SysWorkGroups(table);
                }
            }

            var oldVerificationAgreements = from old in db.tblSysWorkGroups_VerificationAgreements
                                            where old.sysWorkGroupID == model.SysWorkGroupID
                                            select old;
            foreach (var row in oldVerificationAgreements)
            {
                db.tblSysWorkGroups_VerificationAgreements.DeleteObject(row);
            }
            if (model.VerificationAgreements != null)
            {
                for (var i = 0; i < model.VerificationAgreements.Count(); i++)
                {
                    tblSysWorkGroups_VerificationAgreements table = new tblSysWorkGroups_VerificationAgreements();
                    table.sysWorkGroupID = model.SysWorkGroupID;
                    table.verificationAgreementID = int.Parse(model.VerificationAgreements[i].ToString());
                    db.AddTotblSysWorkGroups_VerificationAgreements(table);
                }
            }

            try
            {
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}