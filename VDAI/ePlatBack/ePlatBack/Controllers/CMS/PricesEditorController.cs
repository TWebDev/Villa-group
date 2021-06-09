using System;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers.CMS
{
    public class PricesEditorController : Controller
    {
        public PartialViewResult RenderPricesInActivities()
        {
            PricesEditorViewModel.ParamsPriceEditor pe = new PricesEditorViewModel.ParamsPriceEditor();
            return PartialView("_PricesEditorPartial", pe);
        }

        [Authorize]
        public JsonResult GetPricesInfo(PricesEditorViewModel.ParamsPriceEditor model)
        {
            return Json(PricesEditorDataModel.GetPricesInfo(model));
        }
    }
}
