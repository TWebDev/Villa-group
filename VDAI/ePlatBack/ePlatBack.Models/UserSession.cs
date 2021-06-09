using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace ePlatBack.Models
{
    public class UserSession
    {
        ePlatEntities db = new ePlatEntities();

        public static HttpCookie GetTicket(string workGroupID, string roleID, string terminals, string username = "", bool rememberMe = false)
        {
            //obtener primer terminal si viene null
            if (terminals == null)
            {
                ePlatEntities db = new ePlatEntities();
                Guid userid = (username != "" ? db.aspnet_Users.Single(x => x.UserName == username).UserId : (Guid)Membership.GetUser().ProviderUserKey);
                var UserTerminal = (from u in db.tblUsers_Terminals
                                    where u.userID == userid
                                    select u.terminalID).FirstOrDefault();
                if (UserTerminal != null)
                {
                    terminals = UserTerminal.ToString();
                }
            }

            // Query the user store to get this user's User Data
            var userTerminals = string.Join(",", DataModels.TerminalDataModel.GetCurrentUserTerminals().Select(m => m.Value).ToArray());
            string userDataString = string.Concat(workGroupID, "|", roleID, "|", terminals, "|", userTerminals);

            // Create the cookie that contains the forms authentication ticket
            HttpCookie authCookie;
            if (username == "")
            {
                //get username and persistence
                var userName = Membership.GetUser().UserName;
                var isPersistent = ((System.Web.Security.FormsIdentity)System.Web.HttpContext.Current.User.Identity).Ticket.IsPersistent;
                authCookie = FormsAuthentication.GetAuthCookie(userName, isPersistent);
            }
            else
            {
                authCookie = FormsAuthentication.GetAuthCookie(username, rememberMe);
            }

            // Get the FormsAuthenticationTicket out of the encrypted cookie

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

            // Create a new FormsAuthenticationTicket that includes our custom User Data

            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userDataString);

            // Update the authCookie's Value to use the encrypted version of newTicket
            string hashCookies = FormsAuthentication.Encrypt(newTicket);
            //authCookie.Value = ;
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashCookies);
            cookie.HttpOnly = true;
            // Manually add the authCookie to the Cookies collection

            return cookie;
        }

        private string GetCookieValue(int arrayIndex)
        {
            var userName = Membership.GetUser().UserName;
            var isPersistent = ((System.Web.Security.FormsIdentity)System.Web.HttpContext.Current.User.Identity).Ticket.IsPersistent;
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(userName, isPersistent);

            //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            string encryptedCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH"].Value;
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedCookie);
            string userDataString = ticket.UserData;
            if (arrayIndex == 3 && userDataString.Split('|').Count() != 4)
            {
                return string.Join(",", DataModels.TerminalDataModel.GetCurrentUserTerminals().Select(m => m.Value).ToArray());
            }
            string[] userDataPieces = userDataString.Split("|".ToCharArray());
            return userDataPieces[arrayIndex];
        }

        public int? WorkGroupID
        {
            get
            {
                string result = GetCookieValue(0);
                if (result == "")
                {
                    return null;
                }
                else
                {
                    return int.Parse(result);
                }
            }
        }

        public Guid? RoleID
        {
            get
            {
                string result = GetCookieValue(1);
                if (result == "")
                {
                    return null;
                }
                else
                {
                    return Guid.Parse(result);
                }
            }
        }

        public string Terminals
        {
            get
            {
                string result = GetCookieValue(2);
                return result;
            }
        }

        public string UserTerminals
        {
            get
            {
                string result = GetCookieValue(3);
                return result;
            }
        }

        public Guid UserID
        {
            get
            {
                return (Guid)Membership.GetUser().ProviderUserKey;
            }
        }

        public string User
        {
            get
            {
                //Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
                Guid userid = UserID;
                var userQ = from u in db.tblUserProfiles
                            where u.userID == userid
                            select u;

                if (userQ.Count() > 0)
                {
                    return userQ.FirstOrDefault().firstName + " " + userQ.FirstOrDefault().lastName;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Extension
        {
            get
            {
                Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
                var userQ = from u in db.tblUserProfiles
                            where u.userID == userid
                            select u;

                if (userQ.Count() > 0)
                {
                    var extension = userQ.FirstOrDefault().phoneEXT ?? "";
                    return extension;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Phone
        {
            get
            {
                Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
                var userQ = from u in db.tblUserProfiles
                            where u.userID == userid
                            select u;

                if (userQ.Count() > 0)
                {
                    var phone = userQ.FirstOrDefault().departamentPhone ?? "";
                    return phone;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Email
        {
            get
            {
                Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
                return db.aspnet_Membership.Single(m => m.UserId == userid).Email;
            }
        }

        public string Services
        {
            get
            {
                Guid userid = (Guid)Membership.GetUser().ProviderUserKey;
                var userQ = from u in db.tblUsers_SysWorkGroups
                            where u.userID == userid
                            select u;

                //if (userQ.Count() > 0)
                //{
                if (userQ.FirstOrDefault(m => m.sysWorkGroupID == WorkGroupID) != null && userQ.FirstOrDefault(m => m.sysWorkGroupID == WorkGroupID).manageServices == true)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
                //return (bool)userQ.FirstOrDefault(m => m.sysWorkGroupID == WorkGroupID).manageServices ? "true" : "false";
                //}
                //else
                //{
                //    return false;
                //}
            }
        }

        public string Url
        {
            get
            {
                return HttpContext.Current.Request.UrlReferrer.ToString();
            }
        }
    }
}
