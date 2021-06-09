using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using ePlatBack.Models;
using System.Threading.Tasks;

namespace ePlatBack.Controllers.Settings
{
    public class TaskSchedulerController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                TaskSchedulerViewModel tvm = new TaskSchedulerViewModel();
                ViewData.Model = new TaskSchedulerViewModel
                {
                    SearchTask = new  TaskSchedulerViewModel.SearchTasks(),
                    SaveTask = new TaskSchedulerViewModel.SaveTaskModel(),
                    GetTaskModel = new TaskSchedulerViewModel.GetTaskScheduler(),
                    TaskResult = new TaskSchedulerViewModel.TasksResults(),
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RenderTaskSchedulerManagement()
        {
            return PartialView("TaskSchedulerManagementPartial", new TaskSchedulerViewModel.SaveTaskModel());
        }

        public ActionResult SearchTasks(TaskSchedulerViewModel.SearchTasks model)
        {
            TaskSchedulerDataModel cdm = new TaskSchedulerDataModel();
            TaskSchedulerViewModel.TasksResults ssp = new TaskSchedulerViewModel.TasksResults();
            ssp = cdm.SearchTaskScheduler(model);
            return PartialView("TaskSchedulerPartialResult", ssp);
        }

        public ActionResult SaveTask(TaskSchedulerViewModel.SaveTaskModel model)
        {
            TaskSchedulerDataModel cdm = new TaskSchedulerDataModel();
            AttemptResponse attempt = cdm.SaveTask(model);
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

        public ActionResult DeleteTask(int targetID)
        {
            TaskSchedulerDataModel cdm = new TaskSchedulerDataModel();
            AttemptResponse attempt = cdm.DeleteTaskScheduler(targetID);
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

        public ActionResult GetTask(int targetID)
        {
            TaskSchedulerDataModel cdm = new TaskSchedulerDataModel();
            return Json(cdm.GetTaskScheduler(targetID));
        }

        //REMINDERS
        public ActionResult SaveReminder(RemindersViewModel.Reminder model)
        {
            AttemptResponse response = new AttemptResponse();
            response = RemindersDataModel.SaveReminder(model);
            string errorLocation = "";
            if (response.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(response.Exception);
            }
            return Json(new {
                ResponseType = response.Type,
                ResponseItemID = response.ObjectID,
                ResponseMessage = response.Message,
                ExceptionMessage = Debugging.GetMessage(response.Exception),
                InnerException = Debugging.GetInnerException(response.Exception)
            });
        }

        public JsonResult GetReminders ()
        {
            return Json(RemindersDataModel.GetRemindersUser(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReminder (int reminderID)
        {
            AttemptResponse response = RemindersDataModel.DeleteReminder(reminderID);
            string errorLocation = "";
            if (response.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(response.Exception);
            }
            return Json(new
            {
                ResponseType = response.Type,
                ResponseItemID = response.ObjectID,
                ResponseMessage = response.Message,
                ExceptionMessage = Debugging.GetMessage(response.Exception),
                InnerException = Debugging.GetInnerException(response.Exception)
            });
        }

        public ActionResult GetCurrentTask()
        {
            return Json(TaskSchedulerDataModel.TaskReport(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TaskTest()
        {
            return Json(new
            {
                ResponseType = "1",
                ResponseItemID = "20",
                ResponseMessage = "This is a answer test " + DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            },JsonRequestBehavior.AllowGet);
        }
    }
}