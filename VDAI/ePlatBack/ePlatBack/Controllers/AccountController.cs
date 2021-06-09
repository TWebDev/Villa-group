using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers
{
    public class AccountController : Controller
    {
        ePlatEntities db = new ePlatEntities();
        ePlatRepository rep = new ePlatRepository();
        //
        //********** GET: /Account/LogOn
        public ActionResult LogOn()
        {
            return View();
        }
        //**********

        //********** POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            ViewBag.Link = "";
            if (ModelState.IsValid)
            {
                if (model.WorkGroupID != "")
                {
                    if (Membership.ValidateUser(model.UserName.Trim(' '), model.Password.Trim(' ')))
                    {
                        //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                        Response.Cookies.Add(UserSession.GetTicket(model.WorkGroupID, model.RoleID, model.Terminals, model.UserName, model.RememberMe));

                        //comprobar la IP
                        if (AccountDataModel.ValidateIP(model.UserName))
                        {
                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        } else
                        {
                            ModelState.AddModelError("Username", "Your username needs approval to access from this location.");
                        }                        
                    }
                    else
                    {
                        //verificar la razón por la que no se pudo acceder
                        Guid currentUserID = ePlatBack.Models.DataModels.UserDataModel.GetUserID(model.UserName.Trim(' '));
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

                                        ModelState.AddModelError("Username", "Your username is locked.");
                                        ViewBag.Link = "<a class=\"login-link\" href=\"javascript:UI.approveUnlock('" + currentUserID + "')\">Click here to ask " + supervisor + " to unlock your username.</a>";
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("Username", "Your username is locked.");
                                    }
                                }
                                else
                                {
                                    //no está bloqueado
                                    int attempts = (5 - MembershipQ.FailedPasswordAttemptCount);
                                    if (attempts > 2)
                                    {
                                        ModelState.AddModelError("Password", "The provided password is invalid. You have " + attempts + " attempts more until it locks.");
                                        ViewBag.Link = "<a class=\"login-link\" href=\"javascript:UI.recoverPassword('" + currentUserID + "')\">Click here to reset your password</a>";
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("Password", "The provided password is invalid. You have " + attempts + " attempts more until it locks or wait 10 minutes and try again.");
                                        ViewBag.Link = "<a class=\"login-link\" href=\"javascript:UI.recoverPassword('" + currentUserID + "')\">Click here to reset your password</a>";
                                    }
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "The provided password is invalid.");
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        //**********

        public JsonResult SessionDetails()
        {
            return Json(rep.GetSessionDetails(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveTicket(string workGroupID, string roleID, string terminals)
        {
            Response.Cookies.Add(UserSession.GetTicket(workGroupID, roleID, terminals));
            if (!AccountDataModel.ValidateTicketTerminals() || !AccountDataModel.ValidateIP())
            {
                //LogOff();
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }
            return Json(new { ok = true });
        }
        /// <summary>
        /// Gets the available system workgroups for a specific user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult GetWorkGroupsByUser(string userName)
        {
            AttemptResponse attempt = rep.GetWorkGroupsPerUser(userName);

            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                Model = attempt.ObjectID,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AskForUnlock(Guid id)
        {
            AttemptResponse attempt = rep.AskForUnlock(id);

            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AskForRecoverPassword(Guid id)
        {
            AttemptResponse attempt = rep.AskForRecoverPassword(id);

            string errorLocation = "";
            if (attempt.Exception != null)
            {

                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }

            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the available system roles for a specific user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="workGroupID"></param>
        /// <returns></returns>
        public JsonResult GetRolesByUser(string userName, int workGroupID)
        {
            List<RolesModel> list = new List<RolesModel>();
            list = rep.GetRolesByUser(userName, workGroupID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the available terminals for a specific user
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTerminalsByUser()
        {
            List<TerminalsSetModel> list = new List<TerminalsSetModel>();
            Guid currentUserID;
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                currentUserID = (Guid)Membership.GetUser().ProviderUserKey;
                var terminals = from t in db.tblUsers_Terminals
                                where t.userID == currentUserID
                                select new
                                {
                                    tId = t.tblTerminals.terminalID,
                                    tName = t.tblTerminals.terminal
                                };

                foreach (var i in terminals.OrderBy(m => m.tName))
                {
                    list.Add(new TerminalsSetModel()
                    {
                        TerminalID = (int)i.tId,
                        Terminal = i.tName
                    });
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the available components for a specific user
        /// </summary>
        /// <param name="selectedWorkGroupID"></param>
        /// <param name="selectedRoleID"></param>
        /// <returns></returns>
        public JsonResult GetMenuComponents(int selectedWorkGroupID, Guid selectedRoleID)
        {
            if (rep.CurrentUser() != null)
            {
                try
                {
                    var list = rep.MenuComponents(selectedWorkGroupID, selectedRoleID);
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ResetPassword(string id)
        {
            ViewBag.Valid = rep.ValidResetPasswordUrl(id);
            return View();
        }

        public ActionResult ApproveUnlock(string id)
        {
            ViewBag.Message = rep.ApproveUnlock(id);
            return View();
        }

        //********** GET: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
        //**********

        ////********** GET: /Account/Register
        //public ActionResult Register()
        //{
        //    return View();
        //}
        ////**********

        ////********** POST: /Account/Register
        //[HttpPost]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Attempt to register the user
        //        MembershipCreateStatus createStatus;
        //        Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

        //        if (createStatus == MembershipCreateStatus.Success)
        //        {
        //            //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
        //            return RedirectToAction("UserRegistration", "UserRegistration");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", ErrorCodeToString(createStatus));
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}
        //**********

        //********** GET: /Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            if (ePlatBack.Models.DataModels.AdminDataModel.VerifyAccess())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        //**********

        //********** POST: /Account/ChangePassword


        [HttpPost]
        public ActionResult ResetPassword(string id, ChangePasswordModel model)
        {
            // ChangePassword will throw an exception rather
            // than return false in certain failure scenarios.
            if (rep.ResetPassword(id, model))
            {
                return RedirectToAction("ChangePasswordSuccess");
            }
            else
            {
                ModelState.AddModelError("", "I was not able to reset your password, URL invalid.");
            }
            // If we got this far, something failed, redisplay form
            ViewBag.Valid = false;
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather
                    // than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        MembershipUser currentUser = Membership.GetUser(); //, true /* userIsOnline */
                        changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
        }
        //**********

        //********** GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {         
                return View();           
        }
        //**********
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
