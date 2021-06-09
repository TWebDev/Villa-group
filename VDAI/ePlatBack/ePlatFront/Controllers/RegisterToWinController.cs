using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatFront.Controllers
{
    public class RegisterToWinController : Controller
    {
        //
        // GET: /RegisterToWin/

        public ActionResult Index()
        {
            LeadsGenerationViewModel vm = new LeadsGenerationViewModel();
            List<SelectListItem> countries = new List<SelectListItem>();
            countries.Add(new SelectListItem()
            {
                Value = "Undefined",
                Text = "Select your Country"
            });
            countries.Add(new SelectListItem(){
                Value = "USA",
                Text = "United States of America"
            });
            countries.Add(new SelectListItem(){
                Value = "Canada",
                Text = "Canada"
            });
            vm.Countries = countries;
            return View(vm);
        }

        [HttpPost]
        public JsonResult SaveInfo(LeadsGenerationViewModel model)
        {
            ControlsDataModel.LeadsGenerationDataModel dm = new ControlsDataModel.LeadsGenerationDataModel();

            AttemptResponse attempt = dm.Save(model);

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

    }
}
