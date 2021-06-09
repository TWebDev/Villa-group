using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ePlatBack.Models;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using System.Net;
using System.Net.Mail;
using ePlatBack.Models.Utils;
using System.Globalization;
using System.Threading.Tasks;
//using System.Net.Http;
//using System.Web.Http;

namespace ePlatBack.Controllers.Settings
{
    public class MailSenderController : Controller
    {
        //public async Task<int> CreateTableUserLogActivityAsync(dynamic _item, string type, dynamic _attempt, string urlMethod = "", HttpRequestBase request = null, string terminalID = null)
        //{
        //    int result = await GeneralFunctions.TriggerServicesLog(_item, type, _attempt, urlMethod, request, terminalID);
        //    return result;
        //}


        public JsonResult MailSender(string date = null)
        {
            AttemptResponse attempt = new EmailsDataModel().EmailSenderByRule(date);
            //UserSession session = new UserSession();

            //var request = HttpContext.Request;
            //var urlMethod = session.Url;
            //var terminalID = session.Terminals.IndexOf(",") != -1 ? session.Terminals.Split(',').Select(m => m).ToArray().First() : session.Terminals;

            //Task.Run(() => CreateTableUserLogActivityAsync("", "TaskScheduler", attempt));

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
                InnerException = (attempt.Exception != null && attempt.Exception.InnerException != null) ? errorLocation + attempt.Exception.InnerException.ToString() : ""
            });
        }

        
    }
}
