using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System.Data.Entity;
using System.Web.Script.Serialization;
using ePlatBack.Models.Utils;
using Newtonsoft.Json;


namespace ePlatBack.Controllers.Settings
{
    public class UsersController : Controller
    {
        //
        // GET: /cms/users/
        ePlatEntities db = new ePlatEntities();

        /// <summary>
        /// Load the main view to edit user's information.
        /// </summary>
        /// <returns>Returns /Views/Users/Index.cshtml</returns>
        [Authorize]
        public ActionResult Index()
        {
            var privileges = AdminDataModel.GetViewPrivileges(36);//11276
            if (AdminDataModel.VerifyAccess())
            {
                ViewData.Model = new UserViewModel
                {
                    UserSearch = new UserSearchModel()
                    {
                        Privileges = privileges,
                    },
                    UserInfo = new UserInfoModel()
                    {
                        Privileges = privileges,
                    },
                    Privileges = privileges,
                };
                ViewData["Privileges"] = privileges;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Find the users that match with the search criteria.
        /// </summary>
        /// <param name="usm">UserViewModel properties</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Search(UserSearchModel usm)
        {
            UserDataModel udm = new UserDataModel();
            UserViewModel uvm = new UserViewModel();
            //return PartialView("_SearchUsersResults", udm.SearchUsers(usm));
            return PartialView("_SearchUsersResults", udm.Search(usm));
        }

        [Authorize]
        public JsonResult GetUserInfo(Guid userID)
        {
            UserDataModel udm = new UserDataModel();
            return Json(udm.GetUserInfo(userID));
        }

        [Authorize]
        public JsonResult DeleteUser(Guid userID)
        {
            UserDataModel udm = new UserDataModel();
            AttemptResponse attempt = udm.DeleteUser(userID);
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
        public JsonResult SaveUser(UserInfoModel model)
        {
            UserDataModel udm = new UserDataModel();
            AttemptResponse attempt = udm.SaveUser(model);
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
        //-----------------------------------------------------------
        [Authorize]
        public JsonResult GetProfile(Guid userID)
        {
            UserDataModel user = new UserDataModel();
            return Json(user.Find(userID), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult GetDDLData(string itemType, string itemID)
        {
            UserDataModel udm = new UserDataModel();
            return Json(udm.GetDDLData(itemType, itemID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendActiveUsers()
        {
            AttemptResponse attempt = UserDataModel.SendActiveUsers();
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

        public ActionResult ActiveUsers()
        {
            ViewData.Model = new ActiveUsers()
            {
                Users = JsonConvert.SerializeObject(UserDataModel.GetActiveUsers())
            };
            return View();
        }

        [Authorize]
        public ActionResult UserRequests()
        {
            UserRequestsViewModel model = UserDataModel.GetUserRequests();
            model.RequestsJson = JsonConvert.SerializeObject(model.Requests);
            ViewData.Model = model;
            return View();
        }

        [Authorize]
        [HttpDelete]
        public JsonResult DeleteUserRequest(Guid id)
        {
            AttemptResponse attempt = UserDataModel.DeleteUserRequest(id);
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
            });
        }

        public ActionResult UserRequest(Guid? id)
        {
            if(id != null)
            {
                ViewData["UserRequest"] = JsonConvert.SerializeObject(UserDataModel.GetUserRequest((Guid)id));
            }            
            ViewData.Model = new UserRequest();            
            return View();
        }

        public ActionResult UserRequestFollowUp(Guid id, int? eventID = null)
        {
            if (eventID != null)
            {
                //hay un evento, procesar el cambio de status
                AttemptResponse changeStatusAttempt = UserDataModel.UserRequestChangeStatus(id, (int)eventID);
            }

            ViewData.Model = UserDataModel.GetUserRequest(id);
            return View();
        }

        public JsonResult GetRequestDependentFields()
        {
            return Json(UserDataModel.GetUserRequestDependentFields(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveRequestNote(Guid userRequestID, string notes)
        {
            AttemptResponse attempt = UserDataModel.SaveRequestNote(userRequestID, notes);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                UserRequestID = attempt.ObjectID,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        [HttpPost]
        public JsonResult UserRequestChangeStatus(Guid userRequestID, int eventID)
        {
            AttemptResponse attempt = UserDataModel.UserRequestChangeStatus(userRequestID, eventID);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                UserRequestID = attempt.ObjectID,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            });
        }

        public JsonResult LockUsers()
        {
            AttemptResponse attempt = UserDataModel.LockUsers();
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

        public JsonResult SaveUserRequest(Guid userRequestID, string notifyTo, string jsonModel)
        {
            AttemptResponse attempt = UserDataModel.SaveUserRequest(userRequestID, notifyTo, jsonModel);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                UserRequestID = attempt.ObjectID,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public PictureDataModel.FineUploaderResult UploadDocument(PictureDataModel.FineUpload upload, int type, Guid requestID, Guid? id)
        {
            return (UserDataModel.UploadDocument(upload, type, requestID, id));
        }

        public JsonResult NotifyNewUserRequest(Guid id)
        {
            AttemptResponse attempt = UserDataModel.UserRequestChangeStatus(id, 1);
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                UserRequestID = attempt.ObjectID,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UsersWarn()
        {
            AttemptResponse attempt = UserDataModel.UsersWarning();
            string errorLocation = "";
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                UserRequestID = attempt.ObjectID,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            },JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveSystemUserInfo(SystemUsers.SystemUser model)
        {
            AttemptResponse attempt = UserDataModel.SaveSystemUser(model);
            string errorLocation = "";
            if(attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                UserRequestID = attempt.ObjectID,
                ExceptionMessage = Debugging.GetMessage(attempt.Exception),
                InnerException = Debugging.GetInnerException(attempt.Exception)
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UsersOrganizationCharts()
        {
            SystemUsers.SearchSubordinates ss = new SystemUsers.SearchSubordinates();
            return PartialView("_UsersOrganizationCharts",ss);
        }
        public JsonResult SearchUsersSubordinates(SystemUsers.SearchSubordinates model)
        {
            UserDataModel udm = new UserDataModel();
            SystemUsers.SystemUsersModel OrgChart = udm.SearchOrganizationChart(model);
            return Json(OrgChart, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RenderSystemUsersInfo()//modal
        {
            return PartialView("_UsersSubordinatesResult", new SystemUsers.SystemUser());
        }
        public JsonResult GetDependantFieldsFromSystemUsers()
        {
            return Json(UserDataModel.GetUsersSubordinatesByRole(),JsonRequestBehavior.AllowGet);
        }
    }
}
