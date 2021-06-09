
using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;

namespace ePlatBack.Models
{
    public class AccountDataModel
    {
        public static bool ValidateTicketTerminals()
        {
            UserSession session = new UserSession();

            var clientTerminals = session.Terminals != "" ? session.Terminals.Split(',').Select(m => m).ToArray() : DataModels.TerminalDataModel.GetCurrentUserTerminals().Select(m => m.Value).ToArray();
            var userTerminals = session.UserTerminals != "" ? session.UserTerminals.Split(',').Select(m => m).ToArray() : DataModels.TerminalDataModel.GetCurrentUserTerminals().Select(m => m.Value).ToArray();
            bool validTerminals = true;

            foreach (var i in clientTerminals)
            {
                if (!userTerminals.Contains(i))
                {
                    validTerminals = false;
                    break;
                }
            }
            return validTerminals;
        }

        public static bool ValidateIP(string username = "")
        {
            ePlatEntities db = new ePlatEntities();
            Guid userID;
            bool valid = true;
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                userID = (Guid)Membership.GetUser().ProviderUserKey;
            }
            else
            {
                var user = db.aspnet_Users.FirstOrDefault(x => x.LoweredUserName == username.ToLower());
                if (user != null)
                {
                    userID = user.UserId;
                }
                else
                {
                    userID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                    valid = false;
                }
            }

            if (valid)
            {
                var onlyApprovedIPs = db.tblUserProfiles.FirstOrDefault(x => x.userID == userID).onlyApprovedIPs;

                if (onlyApprovedIPs)
                {
                    string ip = HttpContext.Current.Request.UserHostAddress;

                    if (ip != "::1")
                    {
                        var UserIPs = (from u in db.tblUserIPs
                                       where u.userID == userID
                                       select u).ToList();

                        if (UserIPs.Count() > 0)
                        {
                            //revisar si la actual está registrada
                            var currentIP = UserIPs.FirstOrDefault(x => x.IP == ip);
                            if (currentIP == null)
                            {
                                //si no esta registrada, registrarla y revisar si es del group para validar el acceso
                                var IPfromGroup = (from g in db.tblIPsIndex
                                                   where g.ip == ip
                                                   select g).FirstOrDefault();

                                tblUserIPs userIP = new tblUserIPs();
                                userIP.userID = userID;
                                userIP.IP = ip;
                                userIP.dateSaved = DateTime.Now;
                                userIP.lastAccessFromIP = DateTime.Now;
                                if (IPfromGroup != null)
                                {
                                    userIP.approved = true;
                                    userIP.dateApproved = DateTime.Now;
                                    userIP.approvedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                                    valid = true;
                                }
                                else
                                {
                                    userIP.approved = false;
                                    valid = false;
                                }

                                db.tblUserIPs.AddObject(userIP);
                            }
                            else
                            {
                                //si está registrada, revisar si está aprobada
                                currentIP.lastAccessFromIP = DateTime.Now;

                                if (!currentIP.approved)
                                {
                                    //revisar si ya fue incluida en el indice
                                    var IPfromGroup = (from g in db.tblIPsIndex
                                                       where g.ip == ip
                                                       select g).FirstOrDefault();

                                    //si ya está en el índice, aprobar por sistema
                                    if (IPfromGroup != null)
                                    {
                                        currentIP.approved = true;
                                        currentIP.dateApproved = DateTime.Now;
                                        currentIP.approvedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                                        valid = true;
                                    }
                                    else
                                    {
                                        currentIP.approved = false;
                                        valid = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //registrar la ip como aprobada por sistema.
                            db.tblUserIPs.AddObject(new tblUserIPs()
                            {
                                userID = userID,
                                IP = ip,
                                dateSaved = DateTime.Now,
                                lastAccessFromIP = DateTime.Now,
                                approved = true,
                                dateApproved = DateTime.Now,
                                approvedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A")
                            });
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            //catch error al guardar.
                        }
                    }
                }
            }

            return valid;
        }

        public static UserStateModel ValidateUser(string username)
        {
            UserStateModel response = new UserStateModel();
            ePlatEntities db = new ePlatEntities();

            if (username != "")
            {
                Guid currentUserID = Guid.Parse("00000000-0000-0000-0000-000000000000");
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    currentUserID = (Guid)Membership.GetUser(HttpContext.Current.User.Identity.Name).ProviderUserKey;
                }
                else
                {
                    if (username != "" && username != null)
                    {
                        currentUserID = DataModels.UserDataModel.GetUserID(username);

                        if (currentUserID == new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            //no existe el usuario
                            response.Message = "The username doesn't exist, please verify if it's correctly typed.";
                            response.State = 3;
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
                            response.State = 2;
                            //está bloqueado
                            if (MembershipQ.FailedPasswordAttemptWindowStart <= DateTime.UtcNow.AddMinutes(-15))
                            {
                                response.Message = "Your username is locked.";
                            }
                            else
                            {
                                response.Message = "Your username is locked.";
                                response.UserID = currentUserID;
                            }
                        }
                        else
                        {
                            response.UserID = currentUserID;
                            var query = (from wg in db.tblUsers_SysWorkGroups
                                         join nwg in db.tblSysWorkGroups on wg.sysWorkGroupID equals nwg.sysWorkGroupID
                                         into wg_nwg
                                         from nwg in wg_nwg.DefaultIfEmpty()
                                         where wg.userID == currentUserID
                                         && (nwg.companiesGroupID != null || wg.sysWorkGroupID == 31)
                                         select wg.sysWorkGroupID).Distinct();

                            if (query.Count() > 0)
                            {
                                var Picture = (from p in db.tblUserProfiles
                                               where p.userID == currentUserID
                                               select p.profilePicturePath).FirstOrDefault();

                                if (Picture != null && Picture != "")
                                {
                                    response.Picture = Picture;
                                }
                                response.Message = "";
                                response.State = 1;
                            }
                            else
                            {
                                response.State = 4;
                                response.Message = "The username doesn't belong to an ePlat.com Work Group. Please contact your ePlat Administrator.";
                            }
                        }
                    }
                }

            }
            return response;
        }

        public static UserStateModel SignIn(LogOnModel model)
        {
            UserStateModel response = new UserStateModel();
            ePlatEntities db = new ePlatEntities();

            Guid currentUserID = ePlatBack.Models.DataModels.UserDataModel.GetUserID(model.UserName.Trim(' '));

            if (Membership.ValidateUser(model.UserName.Trim(' '), model.Password.Trim(' ')))
            {
                //comprobar la IP
                if (AccountDataModel.ValidateIP(model.UserName))
                {
                    if (model.WorkGroupID == null || model.WorkGroupID == "")
                    {
                        //buscar su workgroup
                        var UserWorkGroupRole = (from w in db.tblUsers_SysWorkGroups
                                                 join nwg in db.tblSysWorkGroups on w.sysWorkGroupID equals nwg.sysWorkGroupID
                                         into wg_nwg
                                                 from nwg in wg_nwg.DefaultIfEmpty()
                                                 where w.userID == currentUserID
                                                 && (nwg.companiesGroupID != null || w.sysWorkGroupID == 31)
                                                 select new
                                                 {
                                                     w.sysWorkGroupID,
                                                     w.roleID
                                                 }).FirstOrDefault();
                        if (UserWorkGroupRole != null)
                        {
                            model.WorkGroupID = UserWorkGroupRole.sysWorkGroupID.ToString();
                            model.RoleID = UserWorkGroupRole.roleID.ToString();
                            var TerminalID = (from t in db.tblUsers_Terminals
                                              where t.userID == currentUserID
                                              select t.terminalID).FirstOrDefault();
                            if (TerminalID != 0)
                            {
                                model.Terminals = TerminalID.ToString();
                            }
                        }
                    }

                    HttpContext.Current.Response.Cookies.Add(UserSession.GetTicket(model.WorkGroupID, model.RoleID, model.Terminals, model.UserName, model.RememberMe));

                    response.UserID = currentUserID;
                    response.State = 7;
                }
                else
                {
                    response.UserID = currentUserID;
                    response.State = 5;
                    response.Message = "Your username needs approval to access from this location.";
                }
            }
            else
            {
                //verificar la razón por la que no se pudo acceder
                if (currentUserID != null)
                {
                    var MembershipQ = (from m in db.aspnet_Membership
                                       where m.UserId == currentUserID
                                       select m).FirstOrDefault();

                    if (MembershipQ != null)
                    {
                        if (MembershipQ.IsLockedOut)
                        {
                            //está bloqueado
                            if (MembershipQ.FailedPasswordAttemptWindowStart > DateTime.UtcNow.AddMinutes(-15))
                            {
                                string supervisor = "a Supervisor";
                                var SupervisorIDQ = (from s in db.tblSupervisors_Agents
                                                     where s.agentUserID == currentUserID
                                                     orderby s.supervisor_AgentID
                                                     select s.supervisorUserID).FirstOrDefault();
                                if (SupervisorIDQ != null)
                                {
                                    tblUserProfiles supervisorProfile = db.tblUserProfiles.FirstOrDefault(x => x.userID == SupervisorIDQ);
                                    if (supervisorProfile != null)
                                    {
                                        supervisor = supervisorProfile.firstName + " " + supervisorProfile.lastName;
                                    }
                                }

                                response.State = 2;
                                response.Message = "Your username is locked.<br /><a class=\"login-link\" href=\"javascript:UI.approveUnlock('" + currentUserID + "')\">Click here to ask " + supervisor + " to unlock your username.</a>";
                            }
                            else
                            {
                                response.State = 2;
                                response.Message = "Your username is locked.";
                            }
                        }
                        else
                        {
                            //no está bloqueado
                            int attempts = (5 - MembershipQ.FailedPasswordAttemptCount);
                            if (attempts > 2)
                            {
                                response.State = 6;
                                response.Message = "The provided password is invalid. You have " + attempts + " attempts more until it locks.<br /><a class=\"login-link\" href=\"javascript:UI.recoverPassword('" + currentUserID + "')\">Click here to reset your password</a>";
                            }
                            else
                            {
                                response.State = 6;
                                response.Message = "The provided password is invalid. You have " + attempts + " attempts more until it locks or wait 10 minutes and try again.<br /><a class=\"login-link\" href=\"javascript:UI.recoverPassword('" + currentUserID + "')\">Click here to reset your password</a>";
                            }
                        }
                    }
                }
                else
                {
                    response.State = 6;
                    response.Message = "The provided password is invalid.";
                }
            }

            return response;
        }

        public static SessionDetails GetSessionDetails()
        {
            ePlatEntities db = new ePlatEntities();
            SessionDetails details = new SessionDetails();
            UserSession session = new UserSession();

            var Username = (from u in db.aspnet_Users
                           where u.UserId == session.UserID
                           select u.LoweredUserName).FirstOrDefault();

            if(Username != null)
            {
                details.Username = Username;
            }

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
                           into r_role
                           from role in r_role.DefaultIfEmpty()
                           join wg in db.tblSysWorkGroups on r.sysWorkGroupID equals wg.sysWorkGroupID
                           into r_wg
                           from wg in r_wg.DefaultIfEmpty()
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
                                into u_terminal
                                from terminal in u_terminal.DefaultIfEmpty()
                                join wg in db.tblSysWorkGroups on terminal.sysWorkGroupID equals wg.sysWorkGroupID
                                into t_wg
                                from wg in t_wg.DefaultIfEmpty()
                                where u.userID == session.UserID
                                select new
                                {
                                    u.terminalID,
                                    terminal.terminal,
                                    terminal.sysWorkGroupID,
                                    wg.sysWorkGroup
                                };
            details.UserID = session.UserID;
            details.UserTerminals = new List<UserTerminal>();

            foreach (var t in UserTerminals)
            {
                UserTerminal trwg = new UserTerminal();
                trwg.TerminalID = t.terminalID;
                trwg.Terminal = t.terminal;
                trwg.Visible = true;
                trwg.WorkGroupID = t.sysWorkGroupID;
                trwg.WorkGroup = t.sysWorkGroup;
                details.UserTerminals.Add(trwg);
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
    }
}
