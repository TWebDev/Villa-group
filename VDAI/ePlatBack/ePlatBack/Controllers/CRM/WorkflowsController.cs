using System;
//using System.IO;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ePlatBack.Models;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

namespace ePlatBack.Controllers.CRM
{
    public class WorkflowsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null)
        {
            int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
            return result;
        }

        public JsonResult GetEmails()
        {
            return Json(NotificationsDataModel.NotificationsCatalogs.GetEmails(), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SearchWorkflows(string name)
        public JsonResult SearchWorkflows(WorkflowViewModel.WorkflowInfoModel model)
        {
            WorkflowDataModel wdm = new WorkflowDataModel();
            return Json(wdm.SearchWorkflows(model), JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> SaveWorkflow(long id, string name, string workflow)
        public async Task<JsonResult> SaveWorkflow(WorkflowViewModel.WorkflowInfoModel model)
        {
            AttemptResponse attempt = new AttemptResponse();
            WorkflowDataModel wdm = new WorkflowDataModel();
            var request = HttpContext.Request;
            var urlMethod = new UserSession().Url;
            var terminals = new UserSession().Terminals;
            var terminalID = (string)null;
            if (terminals.Count() > 0)
            {
                terminalID = terminals.Count() > 2 ? terminals.Split(',').Select(m => m).ToArray().First() : terminals;
            }

            //convert string to json 

            //attempt = wdm.SaveWorkflow(id, name, workflow);
            attempt = wdm.SaveWorkflow(model);

            //Task.Run(() => CreateTableUserLogActivityAsync(model, "Save", AttemptResponse, urlMethod, request, terminalID));
            string errorLocation = string.Empty;
            if (attempt.Exception != null)
            {
                errorLocation = "<br />" + Debugging.GetErrorLocation(attempt.Exception);
            }
            return Json(new
            {
                ResponseType = attempt.Type,
                ResponseMessage = attempt.Message,
                ExceptionMessage = (attempt.Exception != null) ? attempt.Exception.Message : "",
                InnerException = (attempt.Exception != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        public async Task<JsonResult> GetWorkflow(int workflowID)
        {
            WorkflowDataModel wdm = new WorkflowDataModel();
            var session = new UserSession();
            var request = HttpContext.Request;
            var urlMethod = session.Url;
            var terminals = session.Terminals.Split(',');
            string terminalID;

            terminalID = terminals.FirstOrDefault();

            var wf = wdm.GetWorkflow(workflowID);

            Task.Run(() => CreateTableUserLogActivityAsync(wf, "Get", (string)null, urlMethod, request, terminalID));
            return Json(wf);
        }

        public PictureDataModel.FineUploaderResult UploadFile(PictureDataModel.FineUpload upload, WorkflowViewModel.WorkflowInfoModel model)
        {
            return (WorkflowDataModel.UploadFile(upload, model));
        }

        public JsonResult GetWorkflowsDependentFields()
        {
            return Json(WorkflowDataModel.GetWorkflowsDependentFields(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDDL(SelectListItem model)
        {
            WorkflowDataModel wdm = new WorkflowDataModel();
            return Json(wdm.GetDDL(model), JsonRequestBehavior.AllowGet);
        }
    }
}
