using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;
using ePlatBack.Models.Utils;
using System.IO;

namespace ePlatBack.Models.DataModels
{
    public class PackageDataModel
    {
        public static UserSession session = new UserSession();
        public class PackagesCatalogs
        {
            public static List<SelectListItem> FillDrpDestinations()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> d = new List<SelectListItem>();
                d.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblDestinations)
                {
                    d.Add(new SelectListItem()
                    {
                        Value = i.destinationID.ToString(),
                        Text = i.destination.ToString(),
                    });
                }
                return d;
            }

            public static List<SelectListItem> FillDrpTerminalsPerUser()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> t = new List<SelectListItem>();
                t.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in TerminalDataModel.GetCurrentUserTerminals())
                {
                    t.Add(new SelectListItem()
                    {
                        Value = i.Value,
                        Text = i.Text
                    });
                }
                return t;
            }

            public static List<SelectListItem> FillDrpRelevance()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> r = new List<SelectListItem>();
                r.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                for (var x = 1; x <= 10; x++)
                {
                    r.Add(new SelectListItem()
                    {
                        Value = x.ToString(),
                        Text = x.ToString(),
                    });
                }
                return r;
            }

            public static List<SelectListItem> FillDrpTermsBlock()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var query = db.tblBlocks.Where(m => terminals.Contains(m.terminalID));

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.blockID.ToString(),
                        Text = i.block
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpCultures()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> c = new List<SelectListItem>();
                c.Add(new SelectListItem() { Value = "", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblLanguages)
                {
                    c.Add(new SelectListItem()
                    {
                        Value = i.culture.ToString(),
                        Text = i.language.ToString(),
                    });
                }
                return c;
            }

            public static List<SelectListItem> FillDrpPackages()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> p = new List<SelectListItem>();
                p.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPackages)
                {
                    p.Add(new SelectListItem()
                    {
                        Value = i.packageID.ToString(),
                        Text = i.package.ToString()
                    });
                }
                return p;
            }

            public static List<SelectListItem> GetRoomTypesPerPlace(int placeID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> rt = new List<SelectListItem>();
                rt.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                if (placeID != 0)
                {
                    foreach (var i in db.tblRoomTypes.Where(m => m.placeID == placeID).Select(m => m))
                    {
                        rt.Add(new SelectListItem()
                        {
                            Value = i.roomTypeID.ToString(),
                            Text = i.roomType.ToString()
                        });
                    }
                }
                return rt;
            }

            public static List<SelectListItem> FillDrpPlaces(int destinationID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> p = new List<SelectListItem>();
                if (destinationID != 0)
                {
                    p.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });

                    var Places = from place in db.tblPlaces
                                 where place.destinationID == destinationID
                                 orderby place.place
                                 select place;

                    foreach (var i in Places)
                    {
                        p.Add(new SelectListItem()
                        {
                            Value = i.placeID.ToString(),
                            Text = i.place.ToString()
                        });
                    }
                }
                else
                {
                    p.Add(new SelectListItem() { Value = "0", Text = "--Select Destination--", Selected = false });
                }
                return p;
            }

            public static List<SelectListItem> FillDrpPlanTypes()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPlanTypes)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.planTypeID.ToString(),
                        Text = i.planType
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpCategories(int catalogID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (catalogID != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    var query = from category in db.tblCategories
                                where category.catalogID == catalogID
                                && category.active == true
                                select category;
                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.categoryID.ToString(),
                            Text = i.category
                        });
                    }
                }
                else
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Catalog--", Selected = false });
                return list;
            }

            public static List<SelectListItem> FillDrpCatalogsPerTerminal()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var query = db.tblCatalogs_Terminals.Where(m => terminals.Contains(m.terminalID)).Select(m => new { m.catalogID, m.tblCatalogs.catalog });

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.catalogID.ToString(),
                        Text = i.catalog
                    });
                }
               
                return list;
            }
        }

        public List<SelectListItem> GetDDLData(string itemID, string itemType)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            switch (itemType)
            {
                case "categories":
                    {
                        list = PackagesCatalogs.FillDrpCategories(int.Parse(itemID));
                        break;
                    }
                case "terms":
                    {
                        list = PackagesCatalogs.FillDrpTermsBlock();
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "places":
                    {
                        list = PackagesCatalogs.FillDrpPlaces(int.Parse(itemID));
                        break;
                    }
                case "roomTypes":
                    {
                        list = PackagesCatalogs.GetRoomTypesPerPlace(int.Parse(itemID));
                        break;
                    }
                case "catalogs":
                    {
                        list = PackagesCatalogs.FillDrpCatalogsPerTerminal();
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "terminalsPerUser":
                    {
                        list = PackagesCatalogs.FillDrpTerminalsPerUser();
                        break;
                    }
            }

            return list;
        }

        public static List<PackageItemViewModel> GetAllPackages()
        {
            ePlatEntities db = new ePlatEntities();
            List<PackageItemViewModel> packages = new List<PackageItemViewModel>();
            string culture = Utils.GeneralFunctions.GetCulture();
            int currency = (culture == "es-MX" ? 2 : 1);
            long terminalID = Utils.GeneralFunctions.GetTerminalID();

            var catalogID = from catalog in db.tblCatalogs_Terminals
                            where catalog.terminalID == terminalID
                            select catalog.catalogID;

            var categoriesIDs = from c in db.tblCategories
                                where catalogID.Contains(c.catalogID)
                                select c.categoryID;

            var packageIDs = from p in db.tblCategories_Packages
                             where categoriesIDs.Contains(p.categoryID)
                             select p.packageID;

            packages = GetPackagesList(packageIDs.ToList(), (currency == 2 ? "MXN" : "USD"), culture);

            //p.tblPackages.relevance_ == 10 && 

            //var objPackages = from p in db.tblPackageSettings
            //                  where packageIDs.Contains(p.packageID)
            //                  && p.tblPrices.priceClasificationID == 1
            //                  && p.tblPrices.priceTypeID == 2
            //                  && (p.tblPrices.permanent_ || p.tblPrices.toDate > DateTime.Today)
            //                  && p.tblPrices.currencyID == currency
            //                  && p.tblPackages.active == true
            //                  && p.tblPackages.tblPackageDescriptions.Where(x => x.culture == culture).Count() > 0
            //                  orderby p.tblPackages.relevance_ descending
            //                  select new
            //                  {
            //                      p.packageID,
            //                      p.tblPackages.tblPackageDescriptions.FirstOrDefault(c => c.culture == culture).package,
            //                      p.tblPackages.tblPackageDescriptions.FirstOrDefault(c => c.culture == culture).includes,
            //                      p.tblPackages.nights,
            //                      p.tblPackages.adults,
            //                      p.tblPackages.children,
            //                      p.tblPrices.price,
            //                      p.tblPackages.tblCategories_Packages.FirstOrDefault().tblCategories.category,
            //                      p.tblPackages.tblPlanTypes.planType
            //                  };

            //foreach (var package in objPackages.OrderBy(x => x.category).ThenBy(x => x.price))
            //{
            //    tblSeoItems seoSettings = PageDataModel.GetSeo(2, package.packageID, culture);
            //    IEnumerable<PictureItemViewModel> pictureSettings = PageDataModel.GetPictures(2, package.packageID, culture, true);
            //    packages.Add(new PackageItemViewModel()
            //    {
            //        Package = package.package,
            //        Stay = (package.nights + 1) + " " + Resources.Models.Shared.SharedStrings.days + " & " + package.nights + " " + Resources.Models.Shared.SharedStrings.nights,
            //        Guests = package.adults + " " + Resources.Models.Shared.SharedStrings.adults + (package.children > 0 ? " & " + package.children + " " + Resources.Models.Shared.SharedStrings.children : ""),
            //        Price = (int)package.price,
            //        Url = (seoSettings != null ? seoSettings.friendlyUrl : ""),
            //        PictureUrl = (pictureSettings.Count() > 0 ? pictureSettings.First().Picture_Url : ""),
            //        Destination = package.category,
            //        Includes = package.includes,
            //        PlanType = package.planType
            //    });
            //}
            return packages;
        }

        public static List<PackageItemViewModel> GetBestPackages()
        {
            ePlatEntities db = new ePlatEntities();
            List<PackageItemViewModel> packages = new List<PackageItemViewModel>();
            string culture = Utils.GeneralFunctions.GetCulture();
            int currency = (culture == "es-MX" ? 2 : 1);
            long terminalID = Utils.GeneralFunctions.GetTerminalID();

            var catalogID = from catalog in db.tblCatalogs_Terminals
                            where catalog.terminalID == terminalID
                            select catalog.catalogID;

            var categoriesIDs = from c in db.tblCategories
                                where catalogID.Contains(c.catalogID)
                                select c.categoryID;

            var packageIDs = from p in db.tblCategories_Packages
                             where categoriesIDs.Contains(p.categoryID)
                             && p.tblPackages.relevance_ == 10
                             select p.packageID;

            packages = GetPackagesList(packageIDs.ToList(), (currency == 2 ? "MXN" : "USD"), culture).Where(x => x.Relevance == 10).ToList();

            //p.tblPackages.relevance_ == 10 && 

            //var objPackages = from p in db.tblPackageSettings
            //                  where packageIDs.Contains(p.packageID)
            //                  && p.tblPrices.priceClasificationID == 1
            //                  && p.tblPrices.priceTypeID == 2
            //                  && (p.tblPrices.permanent_ || p.tblPrices.toDate > DateTime.Today)
            //                  && p.tblPrices.currencyID == currency
            //                  && p.tblPackages.active == true
            //                  && p.tblPackages.tblPackageDescriptions.Where(x => x.culture == culture).Count() > 0
            //                  orderby p.tblPackages.relevance_ descending
            //                  select new
            //                  {
            //                      p.packageID,
            //                      p.tblPackages.tblPackageDescriptions.FirstOrDefault(c => c.culture == culture).package,
            //                      p.tblPackages.tblPackageDescriptions.FirstOrDefault(c => c.culture == culture).includes,
            //                      p.tblPackages.nights,
            //                      p.tblPackages.adults,
            //                      p.tblPackages.children,
            //                      p.tblPrices.price,
            //                      p.tblPackages.tblCategories_Packages.FirstOrDefault().tblCategories.category,
            //                      p.tblPackages.tblPlanTypes.planType
            //                  };

            //foreach (var package in objPackages)
            //{
            //    tblSeoItems seoSettings = PageDataModel.GetSeo(2, package.packageID, culture);
            //    IEnumerable<PictureItemViewModel> pictureSettings = PageDataModel.GetPictures(2, package.packageID, culture, true);
            //    packages.Add(new PackageItemViewModel()
            //    {
            //        Package = package.package,
            //        Stay = (package.nights + 1) + " " + Resources.Models.Shared.SharedStrings.days + " & " + package.nights + " " + Resources.Models.Shared.SharedStrings.nights,
            //        Guests = package.adults + " " + Resources.Models.Shared.SharedStrings.adults + (package.children > 0 ? " & " + package.children + " " + Resources.Models.Shared.SharedStrings.children : ""),
            //        Price = (int)package.price,
            //        Url = (seoSettings != null ? seoSettings.friendlyUrl : ""),
            //        PictureUrl = (pictureSettings.Count() > 0 ? pictureSettings.First().Picture_Url : ""),
            //        Destination = package.category,
            //        Includes = package.includes,
            //        PlanType = package.planType
            //    });
            //}
            return packages;
        }

        public DestinationPackagesViewModel GetPackagesListPageV3(string destination)
        {
            DestinationPackagesViewModel page = new DestinationPackagesViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            string currencyCode = "USD";
            if (culture == "es-MX")
            {
                currencyCode = "MXN";
            }
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            ePlatEntities db = new ePlatEntities();

            var catalogs = from c in db.tblCatalogs_Terminals
                           where c.terminalID == terminalID
                           select c.catalogID;

            var packageIDs = from x in db.tblCategories_Packages
                             where catalogs.Contains(x.tblCategories.catalogID)
                             && x.tblCategories.category == destination.Replace("-", " ")
                             select x.packageID;

            var objDestination = (from d in db.tblDestinationDescriptions
                                  where d.tblDestinations.destination == destination.Replace("-", " ")
                                  && d.terminalID == terminalID
                                  && d.culture == culture
                                  select new
                                  {
                                      d.destinationID,
                                      d.tblDestinations.destination,
                                      d.videoURL,
                                      d.videoTitle,
                                      d.description,
                                      d.description2
                                  }).FirstOrDefault();

            if (objDestination != null)
            {
                /*Destination Package Model*/
                page.Destination = objDestination.destination;
                page.PageTitle = page.Destination + " Packages";
                if (objDestination.videoURL.IndexOf("v=") >= 0)
                {
                    page.VideoUrl = objDestination.videoURL.Substring(objDestination.videoURL.IndexOf("v=") + 2, 11);
                }
                page.VideoTitle = objDestination.videoTitle;
                page.Content = objDestination.description;
                page.Content2 = objDestination.description2;

                //page.Pictures = PageDataModel.GetPictures(4, objDestination.destinationID, culture);
                page.Pictures = PageDataModel.GetPicturesFromPath("/content/themes/base/images/" + destination + "/destination/hd/");

                page.Packages = GetPackagesList(packageIDs.ToList(), currencyCode, culture);

                /*Page Model*/
                page.Culture = culture.Substring(0, 2).ToLower();

                DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
                if (objMaster != null)
                {
                    page.Scripts_Header = objMaster.Scripts_Header;
                    page.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                    page.Scripts_Footer = objMaster.Scripts_Footer;
                    page.Template_Header = objMaster.Template_Header;
                    page.Template_Footer = objMaster.Template_Footer;
                    page.Template_Logo = objMaster.Template_Logo;
                    page.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                    page.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
                }

                tblSeoItems objSeo = PageDataModel.GetSeo(4, objDestination.destinationID, culture);
                page.Seo_Title = objSeo.title;
                page.Seo_Keywords = objSeo.keywords;
                page.Seo_Description = objSeo.description;
                page.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                page.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");

                return page;
            }
            else
            {
                return null;
            }

        }


        public DestinationPackagesViewModel GetPackagesListPage(string destination)
        {
            DestinationPackagesViewModel page = new DestinationPackagesViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            string currencyCode = "USD";
            if (culture == "es-MX")
            {
                currencyCode = "MXN";
            }
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            ePlatEntities db = new ePlatEntities();

            var catalogs = from c in db.tblCatalogs_Terminals
                           where c.terminalID == terminalID
                           select c.catalogID;

            var packageIDs = from x in db.tblCategories_Packages
                             where catalogs.Contains(x.tblCategories.catalogID)
                             && x.tblCategories.category == destination.Replace("-", " ")
                             select x.packageID;

            var objDestination = (from d in db.tblDestinationDescriptions
                                  where d.tblDestinations.destination == destination.Replace("-", " ")
                                  && d.terminalID == terminalID
                                  && d.culture == culture
                                  select new
                                  {
                                      d.destinationID,
                                      destinationName = d.destination,
                                      d.tblDestinations.destination,
                                      d.videoURL,
                                      d.videoTitle,
                                      d.description
                                  }).FirstOrDefault();

            if (objDestination != null)
            {
                /*Destination Package Model*/
                page.Destination = objDestination.destinationName != null ? objDestination.destinationName : objDestination.destination;
                page.PageTitle = page.Destination + " Packages";
                if (objDestination.videoURL.IndexOf("v=") >= 0)
                {
                    page.VideoUrl = objDestination.videoURL.Substring(objDestination.videoURL.IndexOf("v=") + 2, 11);
                }
                page.VideoTitle = objDestination.videoTitle;
                page.Content = objDestination.description;

                page.Pictures = PageDataModel.GetPictures(4, objDestination.destinationID, culture);

                page.Packages = GetPackagesList(packageIDs.ToList(), currencyCode, culture);

                /*Page Model*/
                page.Culture = culture.Substring(0, 2).ToLower();

                DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
                if (objMaster != null)
                {
                    page.Scripts_Header = objMaster.Scripts_Header;
                    page.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                    page.Scripts_Footer = objMaster.Scripts_Footer;
                    page.Template_Header = objMaster.Template_Header;
                    page.Template_Footer = objMaster.Template_Footer;
                    page.Template_Logo = objMaster.Template_Logo;
                    page.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                    page.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
                }

                tblSeoItems objSeo = PageDataModel.GetSeo(4, objDestination.destinationID, culture);
                page.Seo_Title = objSeo.title;
                page.Seo_Keywords = objSeo.keywords;
                page.Seo_Description = objSeo.description;
                page.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                page.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");

                return page;
            }
            else
            {
                return null;
            }

        }

        public static List<PackageItemViewModel> GetPackagesList(List<long> packageIDs, string currencyCode, string culture)
        {
            ePlatEntities db = new ePlatEntities();
            List<PackageItemViewModel> packages = new List<PackageItemViewModel>();

            //var objPackages = from p in db.tblPackageSettings
            //                  where packageIDs.Contains(p.packageID)
            //                  && p.tblPrices.priceClasificationID == 1
            //                  && p.tblPrices.priceTypeID == 2
            //                  && (p.tblPrices.permanent_ || p.tblPrices.toDate > DateTime.Today)
            //                  && p.tblPrices.tblCurrencies.currencyCode == currencyCode
            //                  && p.tblPackages.active == true
            //                  && p.tblPackages.tblPackageDescriptions.Where(x=>x.culture == culture).Count() > 0
            //                  orderby p.tblPackages.relevance_ descending
            //                  select new
            //                  {
            //                      p.packageID,
            //                      p.tblPackages.tblPackageDescriptions.FirstOrDefault(c => c.culture == culture).package,
            //                      p.tblPackages.nights,
            //                      p.tblPackages.adults,
            //                      p.tblPackages.children,
            //                      p.tblPrices.price,
            //                      p.tblPlaces.tblDestinations.destination,
            //                      p.tblPackages.tblPlanTypes.planType
            //                  };


            int currencyID = (currencyCode == "MXN" ? 2 : 1);
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var objPackages = (from package in db.tblPackages
                              join price in db.tblPrices on package.packageID equals price.itemID
                              into package_price
                              from price in package_price.DefaultIfEmpty()
                              
                              join plan in db.tblPlanTypes on package.planTypeID equals plan.planTypeID
                              into package_plan
                              from plan in package_plan.DefaultIfEmpty()
                              join settings in db.tblPackageSettings on package.packageID equals settings.packageID
                              into package_settings
                              from settings in package_settings.DefaultIfEmpty()
                              join place in db.tblPlaces on settings.placeID equals place.placeID
                              into package_place
                              from place in package_place.DefaultIfEmpty()
                              join destination in db.tblDestinations on place.destinationID equals destination.destinationID
                              into package_destination
                              from destination in package_destination.DefaultIfEmpty()
                              where packageIDs.Contains(package.packageID)
                              && package.active == true

                              && price.priceClasificationID == 1
                              && price.priceTypeID == 2
                              && price.sysItemTypeID == 2
                              && (price.permanent_ || price.toDate > DateTime.Today)
                              && price.currencyID == currencyID
                              && price.terminalID == terminalID

                              
                               select new
                              {
                                  package.packageID,
                                  package.nights,
                                  package.adults,
                                  package.children,
                                  price.price,
                                  
                                  plan.planType,
                                  place.destinationID,
                                  destination.destination,
                                  package.relevance_
                              }).Distinct();

            var existingPackages = objPackages.Select(x => x.packageID).ToList();

            var objPackages2 = (from package in db.tblPackages
                               join price in db.tblPrices on package.packageID equals price.itemID
                               into package_price
                               from price in package_price.DefaultIfEmpty()
                                
                                join plan in db.tblPlanTypes on package.planTypeID equals plan.planTypeID
                               into package_plan
                               from plan in package_plan.DefaultIfEmpty()
                               join settings in db.tblPackageSettings on package.packageID equals settings.packageID
                               into package_settings
                               from settings in package_settings.DefaultIfEmpty()
                               join place in db.tblPlaces on settings.placeID equals place.placeID
                               into package_place
                               from place in package_place.DefaultIfEmpty()
                               join destination in db.tblDestinations on place.destinationID equals destination.destinationID
                               into package_destination
                               from destination in package_destination.DefaultIfEmpty()
                               where packageIDs.Contains(package.packageID)
                               && !existingPackages.Contains(package.packageID)
                               && package.active == true

                               && price.priceClasificationID == 1
                               && price.priceTypeID == 2
                               && price.sysItemTypeID == 2
                               && (price.permanent_ || price.toDate > DateTime.Today)
                               && price.currencyID == currencyID
                               && (price.terminalID == 1 || price.terminalID == 9)

                               
                                select new
                               {
                                   package.packageID,
                                   package.nights,
                                   package.adults,
                                   package.children,
                                   price.price,
                                   
                                   plan.planType,
                                   place.destinationID,
                                   destination.destination,
                                   package.relevance_
                               }).Distinct();

            objPackages = objPackages.Concat(objPackages2);

            var TerminalDescriptions = from p in db.tblPackageDescriptions
                                       where packageIDs.Contains(p.packageID)
                                       && p.terminalID == terminalID
                                       && p.culture == culture
                                       select new
                                       {
                                           p.packageID,
                                           p.package
                                       };

            var GeneralDescriptions = from p in db.tblPackageDescriptions
                                      where packageIDs.Contains(p.packageID)
                                      && !TerminalDescriptions.Select(x => x.packageID).Contains(p.packageID)
                                      && p.culture == culture
                                      select new
                                      {
                                          p.packageID,
                                          p.package
                                      };

            var DestinationDescriptions = (from d in db.tblDestinationDescriptions
                                          where d.terminalID == terminalID
                                          select d).ToList();

            var Retail = from p in db.tblPrices
                         where packageIDs.Contains(p.itemID)
                         && p.priceClasificationID == 1
                         && p.priceTypeID == 1
                         && p.sysItemTypeID == 2
                         && (p.permanent_ || p.toDate > DateTime.Today)
                         && p.currencyID == currencyID
                         select new
                         {
                             p.price,
                             p.priceID,
                             p.itemID
                         };

            foreach (var package in objPackages)
            {
                if (packages.Count(x => x.PackageID == package.packageID) == 0)
                {
                    tblSeoItems seoSettings = PageDataModel.GetSeo(2, package.packageID, culture);
                    IEnumerable<PictureItemViewModel> pictureSettings = PageDataModel.GetPictures(2, package.packageID, culture, true);
                    string packageName = "";
                    if (TerminalDescriptions.Count(x => x.packageID == package.packageID) > 0)
                    {
                        packageName = TerminalDescriptions.FirstOrDefault(x => x.packageID == package.packageID).package;
                    }
                    else
                    {
                        packageName = GeneralDescriptions.FirstOrDefault(x => x.packageID == package.packageID).package;
                    }
                    packages.Add(new PackageItemViewModel()
                    {
                        PackageID = package.packageID,
                        Package = packageName,
                        Stay = (package.nights + 1) + " " + Resources.Models.Shared.SharedStrings.days + " & " + package.nights + " " + Resources.Models.Shared.SharedStrings.nights,
                        Guests = package.adults + " " + Resources.Models.Shared.SharedStrings.adults + (package.children > 0 ? " & " + package.children + " " + Resources.Models.Shared.SharedStrings.children : ""),
                        Price = (int)package.price,
                        RetailPrice = (Retail.FirstOrDefault(x => x.itemID == package.packageID) != null ? (int)Retail.FirstOrDefault(x => x.itemID == package.packageID).price : 0),
                        Url = (seoSettings != null ? seoSettings.friendlyUrl : ""),
                        PictureUrl = (pictureSettings.Count() > 0 ? pictureSettings.First().Picture_Url : ""),
                        Destination = DestinationDescriptions.FirstOrDefault(x=> x.destinationID == package.destinationID) != null && DestinationDescriptions.FirstOrDefault(x => x.destinationID == package.destinationID).destination != null ? DestinationDescriptions.FirstOrDefault(x => x.destinationID == package.destinationID).destination :  package.destination,
                        PlanType = package.planType,
                        Relevance = package.relevance_
                    });
                }
            }
            return packages;
        }

        public PackageDetailViewModel GetPackageBrief(long packageid)
        {
            string culture = Utils.GeneralFunctions.GetCulture();
            string currencyCode = "USD";
            if (culture == "es-MX")
            {
                currencyCode = "MXN";
            }
            ePlatEntities db = new ePlatEntities();
            PackageDetailViewModel page = new PackageDetailViewModel();
            long terminaID = Utils.GeneralFunctions.GetTerminalID();
            var objPackage = (from p in db.tblPackageDescriptions
                              where p.packageID == packageid
                              && p.terminalID == terminaID
                              && p.culture == culture
                              && p.active == true
                              && p.tblPackages.active == true
                              select p).FirstOrDefault();

            if (objPackage == null)
            {
                objPackage = (from p in db.tblPackageDescriptions
                              where p.packageID == packageid
                              && p.culture == culture
                              && p.active == true
                              && p.tblPackages.active == true
                              select p).FirstOrDefault();
            }

            if (objPackage != null)
            {
                /*Package Model*/
                page.PackageID = objPackage.packageID;
                page.Package = objPackage.package;
                page.Rating = 5;
                page.Nights = objPackage.tblPackages.nights;
                page.Adults = objPackage.tblPackages.adults;
                page.Children = objPackage.tblPackages.children;
                page.Stay = (page.Nights + 1) + " " + Resources.Models.Shared.SharedStrings.days + " " + Resources.Models.Shared.SharedStrings.and + " " + page.Nights + " " + Resources.Models.Shared.SharedStrings.nights;
                page.Guests = page.Adults + " " + Resources.Models.Shared.SharedStrings.adults + (page.Children > 0 ? " " + Resources.Models.Shared.SharedStrings.and + " " + page.Children + " " + Resources.Models.Shared.SharedStrings.children : "");
                page.PlanType = objPackage.tblPackages.tblPlanTypes.tblPlanTypeDescriptions.First(c => c.culture == culture).planType;
                page.Description = objPackage.description;
                page.Options = objPackage.includes;
                if (objPackage.tblPackages.termsBlockID != null)
                {
                    tblBlockDescriptions termsDescription = objPackage.tblPackages.tblBlocks.tblBlockDescriptions.FirstOrDefault(x => x.culture == culture);
                    if (termsDescription != null)
                    {
                        page.Terms = termsDescription.content_;
                    }
                }


                tblPrices objPackagePrice = PriceDataModel.GetPrice(2, objPackage.packageID, 2, 1, DateTime.Today, currencyCode);
                if (objPackagePrice != null)
                {
                    page.Currency = currencyCode;
                    page.PackagePrice = (int)objPackagePrice.price;

                    DateTime expiration = new DateTime();
                    //if (objPackagePrice.toDate != null)
                    //{
                    //    expiration = (DateTime)objPackagePrice.toDate;
                    //}
                    //else
                    //{
                    //    expiration = DateTime.Today.AddDays(6);
                    //}

                    expiration = DateTime.Today;
                    int numberOfDaysForPromo = 3;
                    if (expiration.Day > numberOfDaysForPromo && expiration.Day % numberOfDaysForPromo != 0)
                    {
                        expiration = expiration.AddDays(numberOfDaysForPromo - (expiration.Day % numberOfDaysForPromo));
                    }
                    else if (expiration.Day <= numberOfDaysForPromo)
                    {
                        expiration = expiration.AddDays(numberOfDaysForPromo - expiration.Day);
                    }

                    DateTime lastDayOfMonth = DateTime.Parse(DateTime.Today.Year.ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                    if (expiration.Date > lastDayOfMonth.Date)
                    {
                        expiration = lastDayOfMonth;
                    }

                    ExpirationDate expirationDate = new ExpirationDate();
                    expirationDate.FullDate = expiration;
                    expirationDate.Day = expiration.Day;
                    expirationDate.Month = expiration.Month;
                    expirationDate.Year = expiration.Year;
                    if (expiration.DayOfWeek == DayOfWeek.Saturday || expiration.DayOfWeek == DayOfWeek.Sunday)
                    {
                        expirationDate.Hour = 15;
                    }
                    else
                    {
                        expirationDate.Hour = 22;
                    }
                    expirationDate.Minute = 00;
                    expirationDate.Second = 00;
                    page.ExpirationDate = expirationDate;
                    page.DestinationID = objPackagePrice.tblPackageSettings.First().tblPlaces.destinationID;
                    page.Destination = objPackagePrice.tblPackageSettings.First().tblPlaces.tblDestinations.destination;
                    page.Country = objPackagePrice.tblPackageSettings.First().tblPlaces.tblDestinations.tblCountries.country;
                }

                tblPrices objPackageRetailPrice = PriceDataModel.GetPrice(2, objPackage.packageID, 1, 1, DateTime.Today, currencyCode);
                if (objPackageRetailPrice != null)
                {
                    page.RetailPrice = (int)objPackageRetailPrice.price;
                }
                else
                {
                    page.RetailPrice = 0;
                }

                tblPrices objPackageAdditionalNight = PriceDataModel.GetPrice(2, objPackage.packageID, 2, 2, DateTime.Today, currencyCode);
                if (objPackageAdditionalNight != null)
                {
                    page.AdditionalNightPrice = (int)objPackageAdditionalNight.price;
                }
                else
                {
                    page.AdditionalNightPrice = 199;
                }

                tblPlaces objResort = GetPackageResort(objPackage.packageID);
                page.ResortID = objResort.placeID;
                page.Resort = objResort.place;
                page.Resort_Label = objResort.placeLabel;
                page.Resort_Location = objResort.address;
                page.Resort_Location_Lat = objResort.lat;
                page.Resort_Location_Lng = objResort.lng;
                if (objResort.placeClasificationID < 6)
                {
                    page.Resort_Stars = (decimal)objResort.placeClasificationID;
                }

                if (objResort.tblPlaceDescriptions.Count() > 0)
                {
                    tblPlaceDescriptions objResortDescriptions = objResort.tblPlaceDescriptions.First(r => r.culture == culture);
                    page.Resort_Description = objResortDescriptions.fullDescription;
                    page.Resort_AllInclusive = objResortDescriptions.allInclusive;
                    page.Resort_FAQ = objResortDescriptions.faq;
                    page.Resort_Amenities = objResortDescriptions.amenities;
                }

                page.Resort_Pictures = PageDataModel.GetPictures(6, objResort.placeID, culture);

                List<RoomItemViewModel> rooms = new List<RoomItemViewModel>();
                //Where(r => r.tblPrices.priceClasificationID == 1 && r.tblPrices.tblCurrencies.currencyCode == currencyCode && r.tblPrices.priceTypeID == 2 && (r.tblPrices.permanent_ || r.tblPrices.toDate > DateTime.Today))
                int currencyID = currencyCode == "MXN" ? 2 : 1;
                var Rooms = (from room in db.tblPackageSettings
                            join description in db.tblRoomTypeDescriptions on room.roomTypeID equals description.roomTypeID
                            into r_description
                            from description in r_description.DefaultIfEmpty()
                            join price in db.tblPrices on room.priceID equals price.priceID
                            into r_price
                            from price in r_price.DefaultIfEmpty()
                            where room.packageID == objPackage.packageID
                            && price.priceClasificationID == 1 
                            && price.currencyID == currencyID
                            && price.priceTypeID == 2
                            && (price.permanent_ || price.toDate > DateTime.Today)
                            && price.terminalID == terminaID
                            && description.culture == culture
                            select new { 
                                room.roomTypeID,
                                description.roomType,
                                description.description,
                                price.price
                            }).Distinct();

                List<long> consideredRooms = Rooms.Select(x => x.roomTypeID).ToList();

                var Rooms2 = (from room in db.tblPackageSettings
                             join description in db.tblRoomTypeDescriptions on room.roomTypeID equals description.roomTypeID
                             into r_description
                             from description in r_description.DefaultIfEmpty()
                             join price in db.tblPrices on room.priceID equals price.priceID
                             into r_price
                             from price in r_price.DefaultIfEmpty()
                             where room.packageID == objPackage.packageID
                             && price.priceClasificationID == 1
                             && price.currencyID == currencyID
                             && price.priceTypeID == 2
                             && (price.permanent_ || price.toDate > DateTime.Today)
                             && !consideredRooms.Contains(room.roomTypeID)
                             && price.terminalID == 1
                             && description.culture == culture
                              select new
                             {
                                 room.roomTypeID,
                                 description.roomType,
                                 description.description,
                                 price.price
                             }).Distinct();

                Rooms = Rooms.Concat(Rooms2);
                Rooms = Rooms.Distinct();

                if(Rooms.Count() == 0)
                {
                    var Rooms3 = (from room in db.tblPackageSettings
                                  join description in db.tblRoomTypeDescriptions on room.roomTypeID equals description.roomTypeID
                                  into r_description
                                  from description in r_description.DefaultIfEmpty()
                                  join price in db.tblPrices on room.priceID equals price.priceID
                                  into r_price
                                  from price in r_price.DefaultIfEmpty()
                                  where room.packageID == objPackage.packageID
                                  && price.priceClasificationID == 1
                                  && price.currencyID == currencyID
                                  && price.priceTypeID == 2
                                  && (price.permanent_ || price.toDate > DateTime.Today)
                                  && !consideredRooms.Contains(room.roomTypeID)
                                  select new
                                  {
                                      room.roomTypeID,
                                      description.roomType,
                                      description.description,
                                      price.price
                                  }).Distinct();

                    Rooms = Rooms.Concat(Rooms3);
                }

                foreach (var room in Rooms)
                {
                    //obtener foto
                    IEnumerable<PictureItemViewModel> roomPictures = PageDataModel.GetPictures(7, room.roomTypeID, culture, true);
                    string picUrl = "";
                    if (roomPictures.Count() > 0)
                    {
                        picUrl = roomPictures.FirstOrDefault().Picture_Url;
                    }
                    //obtener precio
                    rooms.Add(new RoomItemViewModel()
                    {

                        Room = room.roomType,
                        Description = room.description,
                        PictureUrl = picUrl,
                        Price = (int)room.price
                    });
                }
                page.Rooms = rooms;

                page.Pictures = PageDataModel.GetPictures(2, objPackage.packageID, culture);

                var VideoQ = (from v in db.tblVideos_SysItemTypes
                              where v.sysItemTypeID == 2
                              && v.itemID == packageid
                              select v.tblVideos.url).FirstOrDefault();

                if (VideoQ != null)
                {
                    page.VideoUrl = VideoQ;
                }
                else
                {
                    page.VideoUrl = "";
                }

                /*page.Reviews */
                IEnumerable<ReviewItemViewModel> reviews = new List<ReviewItemViewModel>();
                reviews = PageDataModel.GetReviewsFromSurvey(50, 52, 105, 60, 126, page.ResortID);
                reviews = reviews.Concat(PageDataModel.GetReviews(2, objPackage.packageID, culture));

                page.Reviews = reviews;

                /*Guest Pictures*/
                if (page.Destination != null)
                {
                    page.Guest_Pictures = new List<string>();
                    string serverPath = HttpContext.Current.Server.MapPath("~/content/themes/base/images/" + page.Destination.ToLower().Replace(" ", "-") + "/places/p" + page.ResortID + "/guests-pictures");
                    if (Directory.Exists(serverPath))
                    {
                        string[] fileEntries = Directory.GetFiles(serverPath);
                        foreach (string fileName in fileEntries)
                        {
                            string filePath = fileName.Replace("\\", "/");
                            filePath = filePath.Substring(filePath.IndexOf("/content"));
                            page.Guest_Pictures.Add(filePath);
                        }
                    }
                }

                /*Page Model*/
                tblSeoItems objSeo = PageDataModel.GetSeo(2, objPackage.packageID, culture);
                page.Seo_Title = objSeo.title;
                page.Seo_Keywords = objSeo.keywords;
                page.Seo_Description = objSeo.description;
                page.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                page.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");

                page.Culture = culture.Substring(0, 2).ToLower();

                DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
                if (objMaster != null)
                {
                    page.Scripts_Header = objMaster.Scripts_Header;
                    page.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                    page.Scripts_Footer = objMaster.Scripts_Footer;
                    page.Template_Header = objMaster.Template_Header;
                    page.Template_Footer = objMaster.Template_Footer;
                    page.Template_Logo = objMaster.Template_Logo;
                    page.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                    page.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
                    page.Terms = page.Terms.Replace("$phone", page.Template_Phone_Desktop);
                }

                //fill description
                int days = page.Nights + 1;
                page.Description = page.Description
                    .Replace("$days", days.ToString())
                    .Replace("$nights", page.Nights.ToString())
                    .Replace("$adults", page.Adults.ToString())
                    .Replace("$children", page.Children.ToString())
                    .Replace("$roomType", page.Rooms.FirstOrDefault().Room)
                    .Replace("$resortLabel", page.Resort_Label)
                    .Replace("$resort", page.Resort)
                    .Replace("$planType", page.PlanType)
                    .Replace("$destination", page.Destination)
                    .Replace("$additionalNightPrice", page.AdditionalNightPrice.ToString());

                //siblings
                long terminalID = Utils.GeneralFunctions.GetTerminalID();
                int catalogid = (from c in db.tblCatalogs_Terminals
                                 where c.terminalID == terminalID
                                 select c.catalogID).FirstOrDefault();

                if (catalogid != 0)
                {
                    long categoryid = objPackage.tblPackages.tblCategories_Packages.FirstOrDefault(c => c.tblCategories.catalogID == catalogid).categoryID;

                    List<long> packageIDs = (from p in db.tblCategories_Packages
                                             where p.categoryID == categoryid
                                             && p.packageID != objPackage.packageID
                                             select p.packageID).ToList();

                    page.Packages = GetPackagesList(packageIDs, currencyCode, culture);
                }

                return page;
            }
            else
            {
                return null;
            }
        }

        public PackageDetailViewModel GetPackageDetailPage(string friendlyUrl)
        {
            PackageDetailViewModel page = new PackageDetailViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            ePlatEntities db = new ePlatEntities();
            if (friendlyUrl.Substring(friendlyUrl.Length - 1) == "/")
            {
                friendlyUrl = friendlyUrl.Remove(friendlyUrl.Length - 1);
            }
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var PackageID = (from p in db.tblSeoItems
                             where p.friendlyUrl == friendlyUrl
                             && p.terminalID == terminalID
                             select p.itemID).FirstOrDefault();

            if (PackageID != null && PackageID > 0)
            {
                return GetPackageBrief((long)PackageID);
            }
            else
            {
                return null;
            }
        }

        public tblPlaces GetPackageResort(long packageID)
        {
            ePlatEntities db = new ePlatEntities();
            var resort = (from p in db.tblPackageSettings
                          where p.packageID == packageID
                          select p.tblPlaces).FirstOrDefault();
            return resort;
        }

        private string GetDestinationsPerPackage(Int64 packageID)
        {
            ePlatEntities db = new ePlatEntities();
            var destinationsPerPackages = (from z in db.tblPackageSettings
                                           where z.packageID == packageID
                                           select z.tblPlaces.tblDestinations.destination).Distinct();
            var listDestinations = "";
            var flag = 1;
            foreach (var a in destinationsPerPackages)
            {
                listDestinations += a;
                if (flag < destinationsPerPackages.Count())
                {
                    listDestinations += ", ";
                }
                flag++;
            }
            return listDestinations;
        }

        public List<PackagesSearchResultsModel> SearchPackages(PackagesSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            List<PackagesSearchResultsModel> listPackages = new List<PackagesSearchResultsModel>();
            List<KeyValuePair<string, long>> terminals = new List<KeyValuePair<string, long>>();
            if (model.Search_Terminals != "")
            {
                foreach (var terminal in model.Search_Terminals.Split(','))
                {
                    terminals.Add(new KeyValuePair<string, long>("", Int64.Parse(terminal)));
                }
            }
            var array = terminals.Select(m => m.Value);
            if (model.Search_Destination == 0)
            {
                var queryPackages = from package in db.tblPackages
                                    where (package.package.Contains(model.Search_Package) || model.Search_Package == null)
                                    && array.Contains(package.terminalID)
                                    && package.deleted == false
                                    select package;
                foreach (var i in queryPackages)
                {
                    var listDestinations = "";
                    listDestinations += GetDestinationsPerPackage(i.packageID);
                    listPackages.Add(new PackagesSearchResultsModel()
                    {
                        PackageID = int.Parse(i.packageID.ToString()),
                        Package = i.package,
                        Nights = i.nights,
                        Descriptions = db.tblPackageDescriptions.Where(m => m.packageID == i.packageID).Count(),
                        Settings = db.tblPackageSettings.Where(m => m.packageID == i.packageID).Count(),
                        Prices = db.tblPrices.Where(m => m.itemID == i.packageID && (m.toDate > DateTime.Now || m.toDate == null)).Count(),
                        Pictures = db.tblPictures_SysItemTypes.Where(m => m.itemID == i.packageID && m.sysItemTypeID == 2).Count(), //0,
                        Active = i.active,
                        Destinations = listDestinations,//"---",
                        Relevance = i.relevance_,
                    });
                }
            }
            else
            {
                var queryPackages = (from a in db.tblPackageSettings
                                     where (a.tblPlaces.tblDestinations.destinationID == model.Search_Destination || model.Search_Destination == 0)
                                     && (a.tblPackages.package.Contains(model.Search_Package) || model.Search_Package == null)
                                     && array.Contains(a.tblPackages.terminalID)
                                     && a.tblPackages.deleted == false
                                     select new
                                     {
                                         packageID = a.packageID,
                                         package = a.tblPackages.package,
                                         nights = a.tblPackages.nights,
                                         active = a.tblPackages.active,
                                         relevance = a.tblPackages.relevance_,
                                     }).Distinct();

                foreach (var i in queryPackages)
                {
                    var listDestinations = GetDestinationsPerPackage(i.packageID);
                    listPackages.Add(new PackagesSearchResultsModel()
                    {
                        PackageID = int.Parse(i.packageID.ToString()),
                        Package = i.package,
                        Nights = i.nights,
                        Descriptions = db.tblPackageDescriptions.Where(m => m.packageID == i.packageID).Count(),
                        Settings = db.tblPackageSettings.Where(m => m.packageID == i.packageID).Count(),
                        Prices = db.tblPrices.Where(m => m.itemID == i.packageID).Where(m => m.toDate > DateTime.Now).Count(),
                        Pictures = 0,
                        Active = i.active,
                        Relevance = i.relevance,
                        Destinations = listDestinations
                    });
                }
            }
            return listPackages;
        }

        public AttemptResponse SavePackage(PackageInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PackageInfo_PackageID != 0 && model.PackageInfo_PackageID != null)
            {
                try
                {
                    tblPackages package = db.tblPackages.Single(m => m.packageID == model.PackageInfo_PackageID);
                    var query = from category in db.tblCategories_Packages
                                where category.packageID == package.packageID
                                select category;
                    foreach (var i in query)
                        db.DeleteObject(i);
                    db.SaveChanges();
                    package.package = model.PackageInfo_Package;
                    package.planTypeID = model.PackageInfo_PlanType;
                    package.dateSaved = DateTime.Now;
                    package.savedByUserID = session.UserID;
                    package.terminalID = Int64.Parse(model.PackageInfo_Terminal.ToString());
                    package.nights = model.PackageInfo_Nights;
                    package.adults = model.PackageInfo_Adults;
                    package.children = model.PackageInfo_Children;
                    package.active = model.PackageInfo_IsActive;
                    package.relevance_ = model.PackageInfo_Relevance;
                    package.availability = model.PackageInfo_Availability;
                    package.purchased = model.PackageInfo_Purchased;
                    if (model.PackageInfo_TermsBlock != 0)
                        package.termsBlockID = Int64.Parse(model.PackageInfo_TermsBlock.ToString());
                    if (model.PackageInfo_Categories != null)
                    {
                        var categories = model.PackageInfo_Categories.Split(',');
                        foreach (var i in categories)
                        {
                            tblCategories_Packages categoryPackage = new tblCategories_Packages();
                            categoryPackage.categoryID = Int64.Parse(i);
                            categoryPackage.packageID = package.packageID;
                            db.tblCategories_Packages.AddObject(categoryPackage);
                        }
                    }
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = package.packageID;
                    response.Message = "Package Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Package NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPackages package = new tblPackages();
                    package.package = model.PackageInfo_Package;
                    package.planTypeID = model.PackageInfo_PlanType;
                    package.dateSaved = DateTime.Now;
                    package.savedByUserID = session.UserID;
                    package.terminalID = Int64.Parse(model.PackageInfo_Terminal.ToString());
                    package.nights = model.PackageInfo_Nights;
                    package.adults = model.PackageInfo_Adults;
                    package.children = model.PackageInfo_Children;
                    package.active = model.PackageInfo_IsActive;
                    package.relevance_ = model.PackageInfo_Relevance;
                    package.availability = model.PackageInfo_Availability;
                    package.purchased = model.PackageInfo_Purchased;
                    if (model.PackageInfo_TermsBlock > 0)
                    {
                        package.termsBlockID = model.PackageInfo_TermsBlock;
                    }
                    db.tblPackages.AddObject(package);
                    db.SaveChanges();
                    if (model.PackageInfo_Categories != null)
                    {
                        //foreach (var i in model.PackageInfo_Categories)
                        var categories = model.PackageInfo_Categories.Split(',');
                        foreach (var i in categories)
                        {
                            tblCategories_Packages categoryPackage = new tblCategories_Packages();
                            categoryPackage.categoryID = Int64.Parse(i);
                            categoryPackage.packageID = package.packageID;
                            db.tblCategories_Packages.AddObject(categoryPackage);
                        }
                    }
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = package.packageID;
                    response.Message = "Package Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Package NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SavePackageDescription(PackageDescriptionInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.PackageDescriptionInfo_PackageDescriptionID != 0)
            {
                try
                {
                    tblPackageDescriptions description = db.tblPackageDescriptions.Single(m => m.packageDescriptionID == model.PackageDescriptionInfo_PackageDescriptionID);
                    description.packageID = model.PackageDescriptionInfo_PackageID;
                    description.package = model.PackageDescriptionInfo_Package;
                    description.active = model.PackageDescriptionInfo_IsActive;
                    description.culture = model.PackageDescriptionInfo_Culture;
                    description.description = model.PackageDescriptionInfo_Description;
                    description.includes = model.PackageDescriptionInfo_Includes ?? "";
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = description.packageDescriptionID;
                    response.Message = "Package Description Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Package Description NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPackageDescriptions description = new tblPackageDescriptions();
                    description.packageID = model.PackageDescriptionInfo_PackageID;
                    description.package = model.PackageDescriptionInfo_Package;
                    description.active = model.PackageDescriptionInfo_IsActive;
                    description.culture = model.PackageDescriptionInfo_Culture;
                    description.description = model.PackageDescriptionInfo_Description;
                    description.includes = model.PackageDescriptionInfo_Includes ?? "";
                    db.AddTotblPackageDescriptions(description);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = description.packageDescriptionID;
                    response.Message = "Package Description Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Package Description NOt Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SavePackageSettings(PackageSettingsInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.PackageSettingsInfo_PackageSettingsID != 0)
            {
                try
                {
                    tblPackageSettings settings = db.tblPackageSettings.Single(m => m.packageSettingID == model.PackageSettingsInfo_PackageSettingsID);
                    settings.packageID = model.PackageSettingsInfo_PackageID;
                    settings.roomTypeID = model.PackageSettingsInfo_RoomType;
                    settings.placeID = model.PackageSettingsInfo_Place;
                    settings.priceID = model.PackageSettingsInfo_Price;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = settings.packageSettingID;
                    response.Message = "Package Settings Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Package Settings NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPackageSettings settings = new tblPackageSettings();
                    settings.packageID = model.PackageSettingsInfo_PackageID;
                    settings.roomTypeID = model.PackageSettingsInfo_RoomType;
                    settings.placeID = model.PackageSettingsInfo_Place;
                    settings.priceID = model.PackageSettingsInfo_Price;
                    db.AddTotblPackageSettings(settings);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = settings.packageSettingID;
                    response.Message = "Package Settings Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Package Settings NOT Saved";
                    return response;
                }
            }
        }

        //public List<SelectListItem> GetCatalogsPerTerminal(int terminalID)
        //{
        //    return PackageDataModel.PackagesCatalogs.FillDrpCatalogsPerTerminal(terminalID);
        //}
        //public List<CategoriesPerTerminalModel> GetCategoriesPerTerminal(int terminalID)
        //{
        //    ePlatEntities db = new ePlatEntities();
        //    List<CategoriesPerTerminalModel> listCategories = new List<CategoriesPerTerminalModel>();

        //    var query = from c in db.tblCatalogs_Terminals
        //                where c.terminalID == terminalID
        //                select new
        //                {
        //                    catalogID = c.catalogID,
        //                    catalogName = c.tblCatalogs.catalog
        //                };
        //    foreach (var i in query)
        //    {
        //        try
        //        {
        //            var query1 = from a in db.tblCategories
        //                         where a.catalogID == i.catalogID
        //                         && a.active == true
        //                         select new
        //                         {
        //                             categoryID = a.categoryID,
        //                             categoryName = a.category
        //                         };
        //            List<GenericListModel> list = new List<GenericListModel>();
        //            foreach (var s in query1)
        //            {
        //                list.Add(new GenericListModel()
        //                {
        //                    ItemID = int.Parse(s.categoryID.ToString()),
        //                    ItemName = s.categoryName
        //                });
        //            }
        //            listCategories.Add(new CategoriesPerTerminalModel()
        //            {
        //                CatalogID = i.catalogID,
        //                CatalogName = i.catalogName,
        //                Categories = list
        //            });
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    return listCategories;
        //}

        public PackageInfoModel GetPackageInfo(int packageID)
        {
            ePlatEntities db = new ePlatEntities();
            PackageInfoModel model = new PackageInfoModel();
            List<KeyValuePair<int, string>> listCategories = new List<KeyValuePair<int, string>>();
            var query = db.tblPackages.Single(m => m.packageID == packageID);

            var queryCategories = from category in db.tblCategories_Packages
                                  where category.packageID == packageID
                                  select new
                                  {
                                      categoryID = category.categoryID,
                                      category = category.tblCategories.category
                                  };
            foreach (var category in queryCategories)
                listCategories.Add(new KeyValuePair<int, string>(int.Parse(category.categoryID.ToString()), category.category));
            var termsBlock = query.termsBlockID;
            if (query.termsBlockID == null)
                termsBlock = 0;
            model.PackageInfo_PackageID = int.Parse(query.packageID.ToString());
            model.PackageInfo_Package = query.package;
            model.PackageInfo_PlanType = query.planTypeID;
            model.PackageInfo_Terminal = int.Parse(query.terminalID.ToString());
            model.PackageInfo_Nights = query.nights;
            model.PackageInfo_Adults = query.adults;
            model.PackageInfo_Children = query.children;
            model.PackageInfo_IsActive = query.active;
            model.PackageInfo_Relevance = query.relevance_;
            model.PackageInfo_Availability = query.availability;
            model.PackageInfo_Purchased = query.purchased;
            model.PackageInfo_TermsBlock = int.Parse(termsBlock.ToString());
            model.PackageInfo_ListCategories = listCategories;
            return model;
        }

        public AttemptResponse DeletePackage(int packageID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                tblPackages package = db.tblPackages.Single(m => m.packageID == packageID);
                package.deleted = true;
                package.deletedByUserID = session.UserID;
                package.dateDeleted = DateTime.Now;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = 0;
                response.Message = "Package Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                response.Message = "Package NOT Deleted";
                response.Exception = ex;
                return response;
            }
        }

        public PackageSettingsInfoModel GetPackageSetting(int packageSettingID)
        {
            ePlatEntities db = new ePlatEntities();
            var query = (from settings in db.tblPackageSettings
                         where settings.packageSettingID == packageSettingID
                         select settings).Single();

            PackageSettingsInfoModel model = new PackageSettingsInfoModel();
            model.PackageSettingsInfo_Destination = int.Parse(query.tblPlaces.destinationID.ToString());
            model.PackageSettingsInfo_Place = int.Parse(query.placeID.ToString());
            model.PackageSettingsInfo_RoomType = int.Parse(query.roomTypeID.ToString());
            model.PackageSettingsInfo_Price = int.Parse(query.priceID.ToString());
            return model;
        }

        public List<PackageDescriptionInfoModel> GetPackageDescriptions(int packageID)
        {
            ePlatEntities db = new ePlatEntities();
            List<PackageDescriptionInfoModel> list = new List<PackageDescriptionInfoModel>();
            try
            {
                var query = from description in db.tblPackageDescriptions
                            where description.packageID == packageID
                            select new
                            {
                                descriptionPackageID = description.packageDescriptionID,
                                packageID = description.packageID,
                                package = description.package,
                                active = description.active,
                                culture = description.culture,
                                description = description.description,
                                includes = description.includes
                            };
                foreach (var i in query)
                {
                    list.Add(new PackageDescriptionInfoModel()
                    {
                        PackageDescriptionInfo_PackageDescriptionID = int.Parse(i.descriptionPackageID.ToString()),
                        PackageDescriptionInfo_PackageID = int.Parse(i.packageID.ToString()),
                        PackageDescriptionInfo_Package = i.package,
                        PackageDescriptionInfo_IsActive = i.active,
                        PackageDescriptionInfo_Culture = i.culture,
                        PackageDescriptionInfo_Description = i.description,
                        PackageDescriptionInfo_Includes = i.includes
                    });
                }
            }
            catch
            {
            }
            return list;
        }

        public PackageDescriptionInfoModel GetPackageDescription(int packageDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            PackageDescriptionInfoModel model = new PackageDescriptionInfoModel();
            var query = db.tblPackageDescriptions.Single(m => m.packageDescriptionID == packageDescriptionID);
            model.PackageDescriptionInfo_Package = query.package;
            model.PackageDescriptionInfo_IsActive = query.active;
            model.PackageDescriptionInfo_Culture = query.culture;
            model.PackageDescriptionInfo_Description = query.description;
            model.PackageDescriptionInfo_Includes = query.includes;
            return model;
        }

        public List<PackageSettingsInfoModel> GetPackageSettings(int packageID)
        {
            ePlatEntities db = new ePlatEntities();
            List<PackageSettingsInfoModel> list = new List<PackageSettingsInfoModel>();
            try
            {
                var query = from setting in db.tblPackageSettings
                            where setting.packageID == packageID
                            select new
                            {
                                packageSettingID = setting.packageSettingID,
                                packageID = setting.packageID,
                                roomTypeID = setting.roomTypeID,
                                roomType = setting.tblRoomTypes.roomType,
                                place = setting.tblPlaces.place,
                                placeID = setting.placeID,
                                priceID = setting.priceID,
                                price = setting.tblPrices.price
                            };
                foreach (var i in query)
                {
                    list.Add(new PackageSettingsInfoModel()
                    {
                        PackageSettingsInfo_PackageSettingsID = int.Parse(i.packageSettingID.ToString()),
                        PackageSettingsInfo_PackageID = int.Parse(i.packageID.ToString()),
                        PackageSettingsInfo_RoomType = int.Parse(i.roomTypeID.ToString()),
                        PackageSettingsInfo_PlaceName = i.place,
                        PackageSettingsInfo_RoomTypeName = i.roomType,
                        PackageSettingsInfo_Place = int.Parse(i.placeID.ToString()),
                        PackageSettingsInfo_Price = int.Parse(i.priceID.ToString()),
                        PackageSettingsInfo_PriceName = i.price.ToString()
                    });
                }
            }
            catch
            {
            }
            return list;
        }

        public AttemptResponse DeletePackageSetting(int packageSettingID, int packageID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var queryValidation = (from a in db.tblPackageSettings
                                       where a.packageID == packageID
                                       select a).Count();
                if (queryValidation > 1)
                {
                    var query = (from setting in db.tblPackageSettings
                                 where setting.packageSettingID == packageSettingID
                                 select setting).Single();
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Package Setting Deleted";
                    response.ObjectID = packageSettingID;
                    return response;
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Package Setting NOT Deleted";
                    response.ObjectID = 0;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Package Setting NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeletePackageDescription(int packageDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = (from description in db.tblPackageDescriptions
                             where description.packageDescriptionID == packageDescriptionID
                             select description).Single();
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Package Description Deleted";
                response.ObjectID = packageDescriptionID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Package Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }
    }
}
