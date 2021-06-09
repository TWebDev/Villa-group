using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using ePlatFront.Models;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;

namespace ePlatFront.Controllers
{
    [AllowCrossSiteJson]
    public class ActivitiesController : Controller
    {
        //
        // GET: /Activities/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Activities/Category/id
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Category(long id, string z)
        {
            ActivityDataModel activity = new ActivityDataModel();
            CategoryActivitiesViewModel viewModel = activity.GetCategoryActivities(id, z);
            viewModel.RightColumn = System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BlockContentPartial.cshtml", ControlsDataModel.BlockContentDataModel.getSidebarContent()));
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        //
        // GET: /Activities/CategoryV2/id
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult CategoryV2(long id, string z)
        {
            ActivityDataModel activity = new ActivityDataModel();
            CategoryActivitiesViewModel viewModel = activity.GetCategoryActivitiesV2(id, z);
            viewModel.RightColumn = System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BlockContentPartial.cshtml", ControlsDataModel.BlockContentDataModel.getSidebarContent()));
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        public PartialViewResult PurchaseProcess()
        {
            return PartialView();
        }

        //
        // GET: //Activities/Detail/id
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Detail(long id)
        {
            ActivityDataModel activity = new ActivityDataModel();
            ActivityDetailViewModel viewModel = activity.GetActivityDetail(id);
            if (viewModel != null)
            {
                if (viewModel.Description.IndexOf("ePlatTransportationQuotes()") >= 0)
                {
                    ////ePlatTransportationQuotes()
                    TransportationQuotes tq = new TransportationQuotes();
                    tq.TransportationServiceID = id;
                    viewModel.Description = viewModel.Description.Replace("ePlatTransportationQuotes()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_TransportationQuotesPartial.cshtml", tq)));
                }
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        //
        // GET: //Activities/Detail/id
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult DetailV2(long id)
        {
            ActivityDataModel activity = new ActivityDataModel();
            ActivityDetailViewModel viewModel = activity.GetActivityDetail(id);
            if (viewModel != null)
            {
                if (viewModel.Description.IndexOf("ePlatTransportationQuotes()") >= 0)
                {
                    ////ePlatTransportationQuotes()
                    TransportationQuotes tq = new TransportationQuotes();
                    tq.TransportationServiceID = id;
                    viewModel.Description = viewModel.Description.Replace("ePlatTransportationQuotes()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_TransportationQuotesPartialV2.cshtml", tq)));
                }
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        public ActionResult Reviews(Guid id)
        {
            ActivityDataModel activity = new ActivityDataModel();
            ActivityReviewsViewModel viewModel = activity.GetPurchaseReviews(id);
            return View(viewModel);
        }

        //
        // GET: //Activities/GetAvailableDates/id
        public JsonResult GetAvailableDates(long id)
        {
            ActivityDataModel activity = new ActivityDataModel();
            return Json(new
            {
                DatesString = activity.GetAvailableDates(id, int.Parse(Request.QueryString["month"]), int.Parse(Request.QueryString["year"]))
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableDatesV2(long id)
        {
            ActivityDataModel activity = new ActivityDataModel();
            return Json(new
            {
                DatesString = activity.GetAvailableDatesV2(id, int.Parse(Request.QueryString["month"]), int.Parse(Request.QueryString["year"]), long.Parse(Request.QueryString["terminalid"]))
            }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: //Activities/GetSchedulesAndPrices/id
        public JsonResult GetSchedulesAndPrices(long id)
        {
            ActivityDataModel activity = new ActivityDataModel();
            return Json(new
            {
                Schedules = activity.GetSchedulesForDate(id, Convert.ToDateTime(Request.QueryString["date"])),
                Prices = activity.GetPrices(id, Convert.ToDateTime(Request.QueryString["date"]), long.Parse(Request.QueryString["terminalid"]), Request.QueryString["culture"])
            }, JsonRequestBehavior.AllowGet);
        }

        //
        //GET: //
        public JsonResult GetTransportationPrices(long id, DateTime date, long placeId, long? transportationZoneId, bool? round, long terminalid, string culture = "en-US", bool orderAscending = false)
        {
            ActivityDataModel activity = new ActivityDataModel();
            if (transportationZoneId == 0)
            {
                transportationZoneId = PlaceDataModel.GetTransportationZoneForPlace(placeId);
            }

            return Json(new
            {
                Prices = activity.GetPrices(id, date, transportationZoneId, round, terminalid, culture, orderAscending)
            }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: //Activities/GetServiceType/id
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public JsonResult GetServiceType(long id, long terminalid)
        {
            ActivityDataModel activity = new ActivityDataModel();
            long serviceType = activity.GetItemType(id);
            if (serviceType == 1)
            {
                return Json(new
                {
                    ServiceType = serviceType
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                List<SelectListItem> places = new List<SelectListItem>();
                places.Add(new SelectListItem()
                {
                    Value = "",
                    Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Select_one,
                    Selected = true
                });
                places.Add(new SelectListItem()
                {
                    Value = "-1",
                    Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Other
                });
                foreach (SelectListItem item in PlaceDataModel.GetResortsByTerminalDomains(terminalid.ToString()))
                {
                    places.Add(item);
                }

                return Json(new
                {
                    ServiceType = serviceType,
                    Hotels = places,
                    Zones = MasterChartDataModel.LeadsCatalogs.FillDrpTransportationZonesByTerminal(terminalid),
                    OffersRoundTrip = activity.OffersRoundTransportation(id)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //
        // GET: //Activities/GetIndex
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public JsonResult GetIndex()
        {
            ActivityDataModel activities = new ActivityDataModel();
            long terminalid = ePlatBack.Models.Utils.GeneralFunctions.GetTerminalID();
            string culture = ePlatBack.Models.Utils.GeneralFunctions.GetCulture();
            return Json(new
            {
                Activities = activities.GetActivitiesIndex(terminalid, culture),
                Date = string.Format("{0:yyyy-MM-dd}", DateTime.Now)
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
