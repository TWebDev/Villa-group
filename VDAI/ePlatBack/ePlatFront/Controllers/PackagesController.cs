using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.ViewModels;
using ePlatFront.Models;

namespace ePlatFront.Controllers
{
    public class PackagesController : Controller
    {
        //
        // GET: /Packages/
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Index(string destination)
        {
            PackageDataModel packagesModel = new PackageDataModel();
            DestinationPackagesViewModel viewModel = packagesModel.GetPackagesListPage(destination);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        // PackagesV2
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult IndexV2(string destination)
        {
            ePlatBack.Models.Utils.GeneralFunctions.SaveMSISDN();
            PackageDataModel packagesModel = new PackageDataModel();
            DestinationPackagesViewModel viewModel = packagesModel.GetPackagesListPage(destination);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        // PackagesV2
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult IndexV3(string destination)
        {
            ePlatBack.Models.Utils.GeneralFunctions.SaveMSISDN();
            PackageDataModel packagesModel = new PackageDataModel();
            DestinationPackagesViewModel viewModel = packagesModel.GetPackagesListPageV3(destination);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Detail(string destination, string package)
        {
            string redirectionURL = UrlModels.RedirectionUrlRequired(UrlModels.GetFriendlyUrl());
            if (redirectionURL != string.Empty)
            {
                return Redirect(redirectionURL);
            }
            else
            {
                PackageDataModel packagesModel = new PackageDataModel();
                PackageDetailViewModel viewModel = packagesModel.GetPackageDetailPage(UrlModels.GetFriendlyUrl());
                if (viewModel != null)
                {
                    return View(viewModel);
                }
                else
                {
                    return HttpNotFound("404");
                }
            }

        }

        //Packages V2
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult DetailV2(long id)
        {
            PackageDataModel packagesModel = new PackageDataModel();
            PackageDetailViewModel viewModel = packagesModel.GetPackageBrief(id);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                //return HttpNotFound("404");
                return Redirect("~/");
            }
        }

        //Packages V3
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult DetailV3(long id)
        {
            PackageDataModel packagesModel = new PackageDataModel();
            PackageDetailViewModel viewModel = packagesModel.GetPackageBrief(id);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                //return HttpNotFound("404");
                return Redirect("~/");
            }
        }

        //Packages V4 - DVH
        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult DetailV4(long id)
        {
            PackageDataModel packagesModel = new PackageDataModel();
            PackageDetailViewModel viewModel = packagesModel.GetPackageBrief(id);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                //return HttpNotFound("404");
                return Redirect("~/");
            }
        }

        [OutputCache(Location = OutputCacheLocation.Client, Duration = 600)]
        public ActionResult Brief(long id)
        {
            PackageDataModel packagesModel = new PackageDataModel();
            PackageDetailViewModel viewModel = packagesModel.GetPackageBrief(id);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return HttpNotFound("404");
            }
        }
    }
}
