using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;

namespace ePlatFront.Controllers
{
    public class PagesController : Controller
    {
        //
        // GET: /Pages/
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult IndexV2(long id)
        {
            return Index(id);
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Index(long id)
        {
            PageDataModel dm = new PageDataModel();
            PageViewModel pvm = dm.GetPageByID(id);
            
            if (pvm.Content.IndexOf("ePlatBlock(\"Right Column\")") >= 0)
            {
                pvm.Content = pvm.Content.Replace("ePlatBlock(\"Right Column\")", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BlockContentPartial.cshtml", ControlsDataModel.BlockContentDataModel.getSidebarContent())));
            }
            if (pvm.Content.IndexOf("ePlatFreeVacation()") >= 0)
            {
                pvm.Content = pvm.Content.Replace("ePlatFreeVacation()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_FreeVacationPartial.cshtml", new FreeVacationViewModel())));
            }
            if (pvm.Content.IndexOf("ePlatBlog()") >= 0)
            {
                pvm.Content = pvm.Content.Replace("ePlatBlog()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BlogContentPartial.cshtml", ControlsDataModel.BlogContentDataModel.getBlogsList(3))));
            }
            if (pvm.Content.IndexOf("ePlatContactForm()") >= 0)
            {
                pvm.Content = pvm.Content.Replace("ePlatContactForm()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ContactFormPartial.cshtml", new ContactFormViewModel())));
            }
            if (pvm.Content.IndexOf("ePlatGroupsForm()") >= 0)
            {
                pvm.Content = pvm.Content.Replace("ePlatGroupsForm()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_GroupsFormPartial.cshtml", new GroupsFormViewModel())));
            }
            if (pvm.Content.IndexOf("ePlatVallartaAdventures()") >= 0)
            {
                ActivityDataModel adm = new ActivityDataModel();
                IEnumerable<ActivityListItem> Activities = adm.GetActivitiesList(null, 1);
                pvm.Content = pvm.Content.Replace("ePlatVallartaAdventures()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ActivitiesListPartial.cshtml", Activities)));
            }
            if (pvm.Content.IndexOf("ePlatPVDiscovery()") >= 0)
            {
                ActivityDataModel adm = new ActivityDataModel();
                IEnumerable<ActivityListItem> Activities = adm.GetActivitiesList(null, 197);
                pvm.Content = pvm.Content.Replace("ePlatPVDiscovery()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ActivitiesListPartial.cshtml", Activities)));
            }
            if (pvm.Content.IndexOf("ePlatServicesByProvider(") >= 0)
            {
                string prov = pvm.Content.Substring(pvm.Content.IndexOf("ePlatServicesByProvider(") + 24);
                prov = prov.Remove(prov.IndexOf(")"));
                int providerid = int.Parse(prov);

                ActivityDataModel adm = new ActivityDataModel();
                IEnumerable<ActivityListItem> Activities = adm.GetActivitiesList(null, providerid);

                string pattern = @"ePlatServicesByProvider\([0-9]{2,}\)";
                string replacement =  System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ActivitiesListPartial.cshtml", Activities));
                Regex rgx = new Regex(pattern);
                pvm.Content = rgx.Replace(pvm.Content, replacement);
            }
            if (pvm.Content.IndexOf("ePlatServicesByPromoID(") >= 0)
            {
                string promo = pvm.Content.Substring(pvm.Content.IndexOf("ePlatServicesByPromoID(") + 23);
                promo = promo.Remove(promo.IndexOf(")"));
                int promoid = int.Parse(promo);

                ActivityDataModel adm = new ActivityDataModel();
                IEnumerable<ActivitiesByCategory> Categories = adm.GetActivitiesForPromo(promoid);

                string pattern = @"ePlatServicesByPromoID\([0-9]{2,}\)";
                string replacement = System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ActivitiesByCategoryPartial.cshtml", Categories));
                Regex rgx = new Regex(pattern);
                pvm.Content = rgx.Replace(pvm.Content, replacement);
            }
            if (pvm.Content.IndexOf("ePlatServicesByPromoIDMaterialize(") >= 0)
            {
                string promo = pvm.Content.Substring(pvm.Content.IndexOf("ePlatServicesByPromoIDMaterialize(") + 34);
                promo = promo.Remove(promo.IndexOf(")"));
                int promoid = int.Parse(promo);

                ActivityDataModel adm = new ActivityDataModel();
                IEnumerable<ActivitiesByCategory> Categories = adm.GetActivitiesForPromo(promoid);

                string pattern = @"ePlatServicesByPromoIDMaterialize\([0-9]{2,}\)";
                string replacement = System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ActivitiesByCategoryPartialV2.cshtml", Categories));
                Regex rgx = new Regex(pattern);
                pvm.Content = rgx.Replace(pvm.Content, replacement);
            }
            if (pvm.Content.IndexOf("ePlatServicesByPromoIDAll(") >= 0)
            {
                string promo = pvm.Content.Substring(pvm.Content.IndexOf("ePlatServicesByPromoIDAll(") + 23);
                promo = promo.Remove(promo.IndexOf(")"));
                int promoid = int.Parse(promo);

                ActivityDataModel adm = new ActivityDataModel();
                IEnumerable<ActivitiesByCategory> Categories = adm.GetActivitiesForPromo(promoid);

                string pattern = @"ePlatServicesByPromoIDAll\([0-9]{2,}\)";
                string replacement = System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ActivitiesByPromoPartial.cshtml", Categories));
                Regex rgx = new Regex(pattern);
                pvm.Content = rgx.Replace(pvm.Content, replacement);
            }
            if (pvm.Content.IndexOf("ePlatBestDeals()") >= 0) 
            {
                IEnumerable<PackageItemViewModel> packages = PackageDataModel.GetBestPackages();
                pvm.Content = pvm.Content.Replace("ePlatBestDeals()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BestDealsListPartial.cshtml", packages)));
            }
            if (pvm.Content.IndexOf("ePlatAllPackages()") >= 0)
            {
                IEnumerable<PackageItemViewModel> packages = PackageDataModel.GetAllPackages();
                pvm.Content = pvm.Content.Replace("ePlatAllPackages()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_AllPackagesListPartial.cshtml", packages)));
            }
            if (pvm.Content_Header != null && pvm.Content_Header.IndexOf("ePlatBestDealsCarousel()") >= 0)
            {
                IEnumerable<PackageItemViewModel> packages = PackageDataModel.GetBestPackages();
                pvm.Content_Header = pvm.Content_Header.Replace("ePlatBestDealsCarousel()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BestDealsCarouselPartial.cshtml", packages)));
            }
            if (pvm.Content_Header != null && pvm.Content_Header.IndexOf("ePlatBestDealsCarouselDestinations()") >= 0)
            {
                IEnumerable<PackageItemViewModel> packages = PackageDataModel.GetBestPackages().OrderBy(x => x.Price).ToList();
                pvm.Content_Header = pvm.Content_Header.Replace("ePlatBestDealsCarouselDestinations()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_BestDealsCarouselDestinationsPartial.cshtml", packages)));
            }
            if (pvm.Content_Header != null)
            {
                pvm.Content_Header = pvm.Content_Header.Replace("1 877 606 0014", pvm.Template_Phone_Desktop);
                pvm.Content_Header = pvm.Content_Header.Replace("1 888 588 9705", pvm.Template_Phone_Mobile);
            }
            if (pvm.Content.IndexOf("ePlatBookingReviews()") >= 0)
            {
                IEnumerable<ReviewItemViewModel> reviews = PageDataModel.GetReviewsFromSurvey(50, 52, 59, 54, 126, null, true, 146);
                pvm.Content = pvm.Content.Replace("ePlatBookingReviews()", System.Web.HttpUtility.HtmlDecode(this.RenderPartialViewToString("~/Views/Shared/_ReviewsListPartial.cshtml", reviews)));

            }
            return View(pvm);
        }

    }
}
