using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatFront.Controllers
{
    public class CouponsController : Controller
    {
        //
        // GET: /Coupons/

        public ActionResult Index(string coupon, string username)
        {
            CouponDataModel model = new CouponDataModel();
            CouponViewModel viewModel = model.GetCoupon(coupon);
            viewModel.BarCode = CouponDataModel.GenerateBarCode(viewModel.CouponNumber);
            viewModel.PrintedBy = username != null && username != "" ? username.Replace("_", " ") : "";
            return View(viewModel);
        }

        public JsonResult GetCouponBarCode(string id)
        {
            return Json(new
            {
                BarCodeImage = CouponDataModel.GenerateBarCode(id)
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
