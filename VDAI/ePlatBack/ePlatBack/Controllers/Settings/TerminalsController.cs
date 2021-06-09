using System;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ePlatBack.Models.Utils;

namespace ePlatBack.Controllers
{
    public class TerminalsController : Controller
    {
        //
        // GET: /Terminals/
        [Authorize]
        public ActionResult Index()
        {
            if (AdminDataModel.VerifyAccess())
            {
                TerminalViewModel tvm = new TerminalViewModel();
                ViewData.Model = new TerminalViewModel
                {
                    TerminalsSearchModel = new TerminalsSearchModel(),
                    TerminalInfoModel = new TerminalInfoModel(),
                    CatalogInfoModel = new CatalogInfoModel(),
                    DomainInfoModel = new DomainInfoModel(),
                };
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// render terminals partial view 
        /// </summary>
        /// <returns>partial view name and its viewModel</returns>
        public PartialViewResult RenderTerminals()
        {
            var tim = new TerminalInfoModel
            {
                CatalogInfoModel = new CatalogInfoModel(),
                DestinationsPerTerminalInfoModel = new DestinationsPerTerminalInfoModel(),
                DomainInfoModel = new DomainInfoModel(),
            };
            return PartialView("_TerminalsManagementPartial", tim);
        }

        /// <summary>
        /// render catalogs partial view
        /// </summary>
        /// <returns>partial view name and its viewModel</returns>
        public PartialViewResult RenderCatalogs()
        {
            var tim = new CatalogInfoModel
            {
                CategoryInfoModel = new CategoryInfoModel(),
                //CategoryDescriptionInfoModel = new CategoryDescriptionInfoModel(),
            };
            return PartialView("_CatalogsManagementPartial", tim);
        }

        /// <summary>
        /// render categories partial view
        /// </summary>
        /// <returns>partial view name and its viewModel</returns>
        public PartialViewResult RenderCategories()
        {
            var cim = new CategoryInfoModel
            {
                CategoryDescriptionInfoModel = new CategoryDescriptionInfoModel(),
            };
            return PartialView("_CategoriesManagementPartial", cim);
        }

        public PartialViewResult RenderDomains()
        {
            var dim = new DomainInfoModel
            {
                BannerGroupInfoModel = new BannerGroupInfoModel(),
            };
            return PartialView("_DomainsManagementPartial", dim);
        }

        public PartialViewResult RenderBannerGroups()
        {
            var bim = new BannerGroupInfoModel
            {
                BannerInfoModel = new BannerInfoModel(),
            };
            return PartialView("_BannersGroupsManagementPartial", bim);
        }

        /// <summary>
        /// search terminals based on the search criteria
        /// </summary>
        /// <param name="TerminalSearchContent">terminalsSearchModel with search criteria</param>
        /// <returns>list of terminalsSearchResultsModel with matching terminals information</returns>
        public ActionResult SearchTerminals(TerminalsSearchModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            TerminalViewModel tvm = new TerminalViewModel();
            tvm.SearchResults = tdm.SearchTerminals(model);

            return PartialView("_SearchTerminalsResults", tvm);
        }

        /// <summary>
        /// specific terminal information
        /// </summary>
        /// <param name="terminalID">int terminal id</param>
        /// <returns>terminalInfoModel with terminal information</returns>
        public JsonResult GetTerminal(int terminalID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetTerminal(terminalID));
        }

        /// <summary>
        /// Gets all Catalogs registered
        /// </summary>
        /// <returns>list of (key,value) pairs with catalos information</returns>
        public JsonResult GetCatalogs(int terminalID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetCatalogs(terminalID));
        }

        public JsonResult GetDestinationsPerTerminal(int terminalID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetDestinationsPerTerminal(terminalID));
        }

        /// <summary>
        /// Get all categories based on a specific catalogID
        /// </summary>
        /// <param name="catalogID">int catalog id</param>
        /// <returns>list of categoryInfoModel with categories information</returns>
        public JsonResult GetCategoriesPerCatalog(int catalogID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetCategoriesPerCatalog(catalogID));
        }

        /// <summary>
        /// Fill dropDownListCategories
        /// </summary>
        /// <param name="catalogID">int catalog id</param>
        /// <returns>list of (value,text) pairs of categories and id's </returns>
        public JsonResult GetDDLCategories(int catalogID)
        {
            return Json(TerminalDataModel.TerminalsCatalogs.FillDrpCategoriesByCatalog(catalogID), JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// get selected catagory information
        /// </summary>
        /// <param name="categoryID">int category id</param>
        /// <returns>categoryInfoModel with category information</returns>
        public JsonResult GetCategory(int categoryID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetCategory(categoryID));
        }

        /// <summary>
        /// Gets all descriptions of a Category
        /// </summary>
        /// <param name="categoryID">int category id</param>
        /// <returns>list of categoryDescriptionInfoModel with category descriptions information</returns>
        public JsonResult GetDescriptionsPerCategory(int categoryID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetDescriptionsPerCategory(categoryID));
        }

        /// <summary>
        /// get category description info
        /// </summary>
        /// <param name="categoryDescriptionID">int category description id</param>
        /// <returns>categoryDescriptionInfoModel with category description information</returns>
        public JsonResult GetCategoryDescription(int categoryDescriptionID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetCategoryDescription(categoryDescriptionID));
        }

        /// <summary>
        /// Gets all Domains based on a specific TerminalID
        /// </summary>
        /// <param name="terminalID">int terminal id</param>
        /// <returns>list of domainInfoModel with terminal domains information</returns>
        public JsonResult GetDomainsPerTerminal(int terminalID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetDomainsPerTerminal(terminalID));
        }

        /// <summary>
        /// Save or update a terminal
        /// </summary>
        /// <param name="model">terminalInfoModel that contains the terminal information</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveTerminal(TerminalInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveTerminal(model);
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

        /// <summary>
        /// Saves or Update a Catalog
        /// </summary>
        /// <param name="model">catalogInfoModel that contains catalog information</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveCatalog(CatalogInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveCatalog(model);
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

        /// <summary>
        /// Delete a specific catalog
        /// </summary>
        /// <param name="catalogID">int catalog id</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult DeleteCatalog(int catalogID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteCatalog(catalogID);
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

        /// <summary>
        /// delete a specific terminal
        /// </summary>
        /// <param name="terminalID">int terminal id</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult DeleteTerminal(int terminalID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteTerminal(terminalID);
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

        /// <summary>
        /// save or update a category
        /// </summary>
        /// <param name="model">categoryInfoModel with category information</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveCategory(CategoryInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveCategory(model);
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

        /// <summary>
        /// Delete a specific category
        /// </summary>
        /// <param name="categoryID">int category id</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult DeleteCategory(int categoryID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteCategory(categoryID);
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

        /// <summary>
        /// assign categories to a specific catalog
        /// </summary>
        /// <param name="catalogID">string[] catalog id</param>
        /// <param name="categories">string[] categories id´s</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveCategoriesPerCatalog(string[] catalogID, string[] categories)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveCategoriesPerCatalog(catalogID, categories);
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

        /// <summary>
        /// assign catalogs to a specific terminal
        /// </summary>
        /// <param name="catalogID">string[] terminal id</param>
        /// <param name="categories">string[] catalogs id´s</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveCatalogsPerTerminal(string[] terminal, string[] catalogs)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveCatalogsPerTerminal(terminal, catalogs);
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

        public JsonResult SaveDestinationsPerTerminal(string[] terminal, string[] destinations)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveDestinationsPerTerminal(terminal, destinations);
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

        /// <summary>
        /// save or update a category description
        /// </summary>
        /// <param name="model">categoryDescriptionInfoModel that contains category description information</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveCategoryDescription(CategoryDescriptionInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveCategoryDescription(model);
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

        /// <summary>
        /// Delete a specific category description
        /// </summary>
        /// <param name="descriptionID">int category description id</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult DeleteCategoryDescription(int categoryDescriptionID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteCategoryDescription(categoryDescriptionID);
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

        /// <summary>
        /// save or update a terminal domain
        /// </summary>
        /// <param name="model">domainInfoModel that contains terminal domain information</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult SaveDomain(DomainInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveDomain(model);
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

        public JsonResult GetDomain(int terminalDomainID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetDomain(terminalDomainID));
        }

        /// <summary>
        /// delete a specific terminal domain
        /// </summary>
        /// <param name="domainID">int terminal domain id</param>
        /// <returns>AttemptResponse Object with operation response</returns>
        public JsonResult DeleteDomain(int domainID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteDomain(domainID);
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

        public JsonResult GetTree(int itemID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetTree(itemID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFiles(string directory, int terminalDomainID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetFiles(directory, terminalDomainID));
        }

        public PictureDataModel.FineUploaderResult UploadBanner(PictureDataModel.FineUpload upload, string bannerName, string path, string bannerGroup, string url, string permanent, string from, string to, string culture, long terminalID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return (tdm.UploadBanner(upload, bannerName, path, bannerGroup, url, permanent, from, to, culture, terminalID));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        public JsonResult GetBannerGroupsPerDomain(int terminalDomainID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetBannerGroupsPerDomain(terminalDomainID));
        }

        public JsonResult GetBannerGroup(long bannerGroupID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetBannerGroup(bannerGroupID));
        }

        public JsonResult GetBannersPerGroup(long bannerGroupID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetBannersPerGroup(bannerGroupID));
        }

        public JsonResult SaveBannerGroup(BannerGroupInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.SaveBannerGroup(model);
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

        public JsonResult DeleteBannerGroup(long bannerGroupID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteBannerGroup(bannerGroupID);
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

        public JsonResult UpdateBanner(BannerInfoModel model)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.UpdateBanner(model);
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

        public JsonResult DeleteBanner(long bannerID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.DeleteBanner(bannerID);
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

        public JsonResult GetBannerInfo(long bannerID)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            return Json(tdm.GetBannerInfo(bannerID));
        }

        public JsonResult UpdateBannersOrder(string[] items)
        {
            TerminalDataModel tdm = new TerminalDataModel();
            AttemptResponse attempt = tdm.UpdateBannersOrder(items);
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
    }
}
