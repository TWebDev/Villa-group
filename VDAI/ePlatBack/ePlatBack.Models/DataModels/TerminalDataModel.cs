
using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Text.RegularExpressions;

namespace ePlatBack.Models.DataModels
{
    public class TerminalDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();

        public class TerminalsCatalogs
        {
            public static List<SelectListItem> FillDrpCategoriesByCatalog(int catalogID)
            {
        ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                if (catalogID != 0)
                {
                    foreach (var i in db.tblCategories.Where(m => m.catalogID == catalogID).OrderBy(x => x.category))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.categoryID.ToString(),
                            Text = i.category
                        });
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpCultures()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var i in db.tblLanguages)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.culture,
                        Text = i.language
                    });
                }
                return list;
            }
        }

        /// <summary>
        /// Gets all terminals.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetAllTerminals()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                
                var terminals = from t in db.tblTerminals 
                                select new {t.terminalID,t.terminal};

                foreach (var i in terminals)
                {
                    list.Add(new SelectListItem() { Text = i.terminal, Value = i.terminalID.ToString() });
                }
            }
            return list;
        }
        /// <summary>
        /// Gets all terminals By Work Group.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetAllTerminalsByWorkGroup(int workGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {

                var terminals = from t in db.tblTerminals_SysWorkGroups
                                where t.sysWorkGroupID == workGroupID
                                select new
                                {
                                    t.tblTerminals.terminalID,
                                    t.tblTerminals.terminal
                                };

                foreach (var i in terminals)
                {
                    list.Add(new SelectListItem() { Text = i.terminal, Value = i.terminalID.ToString() });
                }
            }
            return list;
        }
        /// <summary>
        /// Gets the available terminals for the current logged in user.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetCurrentUserTerminals()
        {
            List<SelectListItem> list;
            
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Guid currentUserID = session.UserID;
                list = GetTerminalsByUserID(currentUserID);
            }
            else { list= new List<SelectListItem>();}

            return list;
        }
        /// <summary>
        /// Gets the available terminals for a specific user
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetTerminalsByUserID(Guid userID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                
                var terminals = from t in db.tblUsers_Terminals
                                where t.userID == userID
                                select new
                                {
                                    t.tblTerminals.terminalID,
                                    t.tblTerminals.terminal
                                };

                foreach (var i in terminals)
                {
                    list.Add(new SelectListItem() { Text = i.terminal, Value = i.terminalID.ToString() });
                }
            }
            return list;
        }
        /// <summary>
        /// Get active terminals
        /// </summary>
        /// <returns>list of currently active terminals in client side</returns>
        public static List<SelectListItem> GetActiveTerminalsList()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            long[] currentTerminals = session.Terminals != "" ? (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray() : new long[] { };

            foreach (var i in db.tblTerminals.Where(m => currentTerminals.Contains(m.terminalID)).OrderBy(m => m.terminal))
            {
                list.Add(new SelectListItem()
                {
                    Value = i.terminalID.ToString(),
                    Text = i.terminal
                });
            }
            return list;
        }

        public static string GetPrefix(long terminalid)
        {
            ePlatEntities db = new ePlatEntities();
            string prefix = "";
            var terminalQ = from t in db.tblTerminals
                            where t.terminalID == terminalid
                            select t.prefix;

            if (terminalQ != null)
            {
                prefix = terminalQ.First();
            }

            return prefix;
        }

        //public static List<SelectListItem> GetCurrentUserTerminals()
        //{
        //    ePlatEntities db = new ePlatEntities();
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    Guid currentUserID;
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        var terminals = from t in db.tblUsers_Terminals
        //                        where t.userID == currentUserID
        //                        select new
        //                        {
        //                            t.tblTerminals.terminalID,
        //                            t.tblTerminals.terminal
        //                        };

        //        foreach (var i in terminals)
        //        {
        //            list.Add(new SelectListItem() { Text = i.terminal, Value = i.terminalID.ToString() });
        //        }
        //    }
        //    return list;
        //}
        public List<TerminalsSearchResultsModel> SearchTerminals(TerminalsSearchModel model)
        {
            List<TerminalsSearchResultsModel> list = new List<TerminalsSearchResultsModel>();

            var query = from t in db.tblTerminals
                        where (model.Search_Terminal == null || t.terminal.Contains(model.Search_Terminal))
                        select new
                        {
                            terminalID = t.terminalID,
                            prefix = t.prefix,
                            terminal = t.terminal
                        };

            foreach (var i in query)
            {
                list.Add(new TerminalsSearchResultsModel()
                {
                    Prefix = i.prefix,
                    Terminal = i.terminal,
                    TerminalID = int.Parse(i.terminalID.ToString())
                });
            }
            return list;
        }

        public TerminalInfoModel GetTerminal(int terminalID)
        {
            TerminalInfoModel model = new TerminalInfoModel();
            var query = db.tblTerminals.Single(m => m.terminalID == terminalID);
            model.TerminalInfo_Prefix = query.prefix;
            model.TerminalInfo_Terminal = query.terminal;
            return model;
        }

        public List<KeyValuePair<int, string>> GetCatalogs(int terminalID)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            if (terminalID != 0)
            {
                foreach (var i in db.tblCatalogs_Terminals.Where(m => m.terminalID == terminalID))
                {
                    list.Add(new KeyValuePair<int, string>(i.catalogID, i.tblCatalogs.catalog));
                }
            }
            else
            {
                foreach (var i in db.tblCatalogs)
                {
                    var query = db.tblCatalogs_Terminals.Where(m => m.catalogID == i.catalogID);
                    var terminals = "";
                    foreach (var a in query)
                    {
                        terminals += a.tblTerminals.prefix + ", ";
                    }
                    terminals = terminals != "" ? terminals.Substring(0, terminals.Length - 2) : terminals;
                    list.Add(new KeyValuePair<int, string>(i.catalogID, i.catalog + " - " + terminals));
                }
            }
            return list;
        }

        public List<KeyValuePair<int, string>> GetDestinationsPerTerminal(int terminalID)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            if (terminalID != 0)
            {
                foreach (var i in db.tblTerminals_Destinations.Where(m => m.terminalID == terminalID))
                {
                    list.Add(new KeyValuePair<int, string>(int.Parse(i.destinationID.ToString()), i.tblDestinations.destination));
                }
            }
            else
            {
                foreach (var i in db.tblDestinations)
                {
                    list.Add(new KeyValuePair<int, string>(int.Parse(i.destinationID.ToString()), i.destination));
                }
            }
            return list;
        }

        public List<CategoryInfoModel> GetCategoriesPerCatalog(int catalogID)
        {
            List<CategoryInfoModel> list = new List<CategoryInfoModel>();

            foreach (var i in db.tblCategories.Where(m => m.catalogID == catalogID).Select(m => m).OrderBy(x => x.category))
            {
                var parent = i.parentCategoryID ?? 0;

                list.Add(new CategoryInfoModel()
                {
                    CategoryInfo_CategoryID = int.Parse(i.categoryID.ToString()),
                    CategoryInfo_Category = i.category,
                    CategoryInfo_ParentCategory = int.Parse(parent.ToString()),
                    CategoryInfo_IsActive = i.active
                });
            }
            return list;
        }

        public List<CategoryDescriptionInfoModel> GetDescriptionsPerCategory(int categoryID)
        {
            List<CategoryDescriptionInfoModel> list = new List<CategoryDescriptionInfoModel>();
            foreach (var i in db.tblCategoryDescriptions.Where(m => m.categoryID == categoryID).Select(m => m))
            {
                list.Add(new CategoryDescriptionInfoModel()
                {
                    CategoryDescriptionInfo_CategoryDescriptionID = int.Parse(i.categoryDescriptionID.ToString()),
                    CategoryDescriptionInfo_Culture = i.culture,
                    CategoryDescriptionInfo_ShowOnWebsite = i.showOnWebsite != null ? (bool)i.showOnWebsite : false,
                    CategoryDescriptionInfo_Category = i.category,
                    CategoryDescriptionInfo_ImgIcon = i.imgIcon,
                    CategoryDescriptionInfo_ImgHeader = i.imgHeader,
                    //CategoryDescriptionInfo_Description = i.description,
                    //CategoryDescriptionInfo_Policies = i.policies
                    CategoryDescriptionInfo_Description = i.description
                });
            }

            return list;
        }

        public CategoryDescriptionInfoModel GetCategoryDescription(int categoryDescriptionID)
        {
            CategoryDescriptionInfoModel model = new CategoryDescriptionInfoModel();
            var query = db.tblCategoryDescriptions.Single(m => m.categoryDescriptionID == categoryDescriptionID);
            model.CategoryDescriptionInfo_Culture = query.culture;
            model.CategoryDescriptionInfo_Category = query.category;
            model.CategoryDescriptionInfo_ShowOnWebsite = query.showOnWebsite != null ? (bool)query.showOnWebsite : false;
            model.CategoryDescriptionInfo_ImgHeader = query.imgHeader;
            model.CategoryDescriptionInfo_ImgIcon = query.imgIcon;
            model.CategoryDescriptionInfo_Description = query.description;
            return model;
        }

        public CategoryInfoModel GetCategory(int categoryID)
        {
            CategoryInfoModel model = new CategoryInfoModel();
            var query = db.tblCategories.Single(m => m.categoryID == categoryID);
            model.CategoryInfo_Category = query.category;
            model.CategoryInfo_ParentCategory = query.parentCategoryID != null ? int.Parse(query.parentCategoryID.ToString()) : 0;
            model.CategoryInfo_ShowOnWebsite = query.showOnWebsite;
            return model;
        }

        public List<DomainInfoModel> GetDomainsPerTerminal(int terminalID)
        {
            List<DomainInfoModel> list = new List<DomainInfoModel>();
            foreach (var i in db.tblTerminalDomains.Where(m => m.terminalID == terminalID).Select(m => m))
            {
                list.Add(new DomainInfoModel()
                {
                    DomainInfo_TerminalID = terminalID,
                    DomainInfo_DomainID = i.terminalDomainID,
                    DomainInfo_Domain = i.domain,
                    DomainInfo_DefaultPage = i.defaultPage,
                    DomainInfo_DefaultMasterPage = i.defaultMasterPage,
                    DomainInfo_MasterPageHeader = i.masterPageHeader,
                    DomainInfo_MasterPageFooter = i.masterPageFooter,
                    DomainInfo_ScriptsHeader = i.scriptsHeader,
                    DomainInfo_ScriptsAfterBody = i.scriptsAfterBody,
                    DomainInfo_ScriptsFooter = i.scriptsFooter,
                    DomainInfo_Logo = i.logo,
                    DomainInfo_PhoneUS = i.phoneUS,
                    DomainInfo_PhoneMX = i.phoneMX,
                    DomainInfo_PhoneUSMobile = i.phoneUSMobile,
                    DomainInfo_PhoneMXMobile = i.phoneMXMobile,
                    DomainInfo_PhoneAlt1 = i.phoneAlt1,
                    DomainInfo_PhoneAlt1Mobile = i.phoneAlt1Mobile,
                    DomainInfo_PhoneAlt2 = i.phoneAlt2,
                    DomainInfo_PhoneAlt2Mobile = i.phoneAlt2Mobile,
                    DomainInfo_Culture = i.culture
                });
            }
            return list;
        }

        public AttemptResponse SaveTerminal(TerminalInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            //terminal existente
            if (model.TerminalInfo_TerminalID != 0)
            {
                try
                {
                    //actualizacion
                    if (model.TerminalInfo_IsNew == false)
                    {
                        try
                        {
                            tblTerminals terminal = db.tblTerminals.Single(m => m.terminalID == model.TerminalInfo_TerminalID);
                            terminal.prefix = model.TerminalInfo_Prefix;
                            terminal.terminal = model.TerminalInfo_Terminal;
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = model.TerminalInfo_TerminalID;
                            response.Message = "Terminal Updated";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.ObjectID = model.TerminalInfo_TerminalID;
                            response.Message = "Terminal NOT Updated";
                            response.Exception = ex;
                            return response;
                        }
                    }
                    else
                    {
                        int terminalID;
                        try
                        {
                            //copia de existente
                            tblTerminals terminal = new tblTerminals();
                            terminal.prefix = db.tblTerminals.Single(m => m.terminalID == model.TerminalInfo_TerminalID).prefix;
                            terminal.terminal = db.tblTerminals.Single(m => m.terminalID == model.TerminalInfo_TerminalID).terminal + "_copy";
                            db.tblTerminals.AddObject(terminal);
                            db.SaveChanges();
                            terminalID = int.Parse(terminal.terminalID.ToString());
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = "Terminal NOT Saved";
                            response.ObjectID = 0;
                            response.Exception = ex;
                            return response;
                        }
                        try
                        {
                            if (model.TerminalInfo_Domains == true)
                            {
                                var domains = db.tblTerminalDomains.Where(m => m.terminalID == model.TerminalInfo_TerminalID).Select(m => m);
                                foreach (var i in domains)
                                {
                                    tblTerminalDomains terminalDomain = new tblTerminalDomains();
                                    terminalDomain.terminalID = terminalID;
                                    terminalDomain.domain = i.domain;
                                    terminalDomain.defaultPage = i.defaultPage;
                                    terminalDomain.defaultMasterPage = i.defaultMasterPage;
                                    terminalDomain.masterPageHeader = i.masterPageHeader;
                                    terminalDomain.masterPageFooter = i.masterPageFooter;
                                    terminalDomain.scriptsHeader = i.scriptsHeader;
                                    terminalDomain.scriptsAfterBody = i.scriptsAfterBody;
                                    terminalDomain.scriptsFooter = i.scriptsFooter;
                                    terminalDomain.logo = i.logo;
                                    terminalDomain.phoneUS = i.phoneUS;
                                    terminalDomain.phoneMX = i.phoneMX;
                                    terminalDomain.phoneUSMobile = i.phoneUSMobile;
                                    terminalDomain.phoneMXMobile = i.phoneMXMobile;
                                    terminalDomain.phoneAlt1 = i.phoneAlt1;
                                    terminalDomain.phoneAlt1Mobile = i.phoneAlt1Mobile;
                                    terminalDomain.phoneAlt2 = i.phoneAlt2;
                                    terminalDomain.phoneAlt2Mobile = i.phoneAlt2Mobile;
                                    terminalDomain.culture = i.culture;
                                    db.tblTerminalDomains.AddObject(terminalDomain);
                                }
                            }
                            if (model.TerminalInfo_Catalogs == true)
                            {
                                var catalogs = db.tblCatalogs_Terminals.Where(m => m.terminalID == model.TerminalInfo_TerminalID).Select(m => m);
                                foreach (var i in catalogs)
                                {
                                    tblCatalogs_Terminals catalog_terminal = new tblCatalogs_Terminals();
                                    catalog_terminal.terminalID = terminalID;
                                    catalog_terminal.catalogID = i.catalogID;
                                    db.tblCatalogs_Terminals.AddObject(catalog_terminal);
                                }
                            }
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = terminalID;
                            response.Message = "Terminal Saved";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = "Collections NOT Saved";
                            response.ObjectID = 0;
                            response.Exception = ex;
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Terminal NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else //nueva terminal
            {
                try
                {
                    if (model.TerminalInfo_IsNew == false)
                    {
                        tblTerminals terminal = new tblTerminals();
                        terminal.terminal = model.TerminalInfo_Terminal;
                        terminal.prefix = model.TerminalInfo_Prefix;
                        db.tblTerminals.AddObject(terminal);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = terminal.terminalID;
                        response.Message = "Terminal Saved";
                        return response;
                    }
                    else
                    {
                        throw new Exception("No Terminal Selected");
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Terminal NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveCatalog(CatalogInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            if (model.CatalogInfo_CatalogID != 0)
            {
                try
                {
                    tblCatalogs catalog = db.tblCatalogs.Single(m => m.catalogID == model.CatalogInfo_CatalogID);
                    catalog.catalog = model.CatalogInfo_Catalog;
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Catalog Updated";
                    response.ObjectID = model.CatalogInfo_CatalogID;
                    db.SaveChanges();
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Catalog NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblCatalogs catalog = new tblCatalogs();
                    catalog.catalog = model.CatalogInfo_Catalog;
                    db.tblCatalogs.AddObject(catalog);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Catalog Saved";
                    response.ObjectID = catalog.catalogID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Catalog NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeleteCatalog(int catalogID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblCatalogs.Single(m => m.catalogID == catalogID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Catalog Deleted";
                response.ObjectID = catalogID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Catalog NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteTerminal(int terminalID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblTerminals.Single(m => m.terminalID == terminalID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Terminal Deleted";
                response.ObjectID = terminalID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Terminal NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveCategory(CategoryInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            if (model.CategoryInfo_CategoryID != 0)
            {
                try
                {
                    tblCategories category = db.tblCategories.Single(m => m.categoryID == model.CategoryInfo_CategoryID);
                    category.category = model.CategoryInfo_Category;
                    if (model.CategoryInfo_ParentCategory != 0)
                    {
                        category.parentCategoryID = model.CategoryInfo_ParentCategory;
                    }
                    else
                    {
                        category.parentCategoryID = null;
                    }
                    category.showOnWebsite = model.CategoryInfo_ShowOnWebsite;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Category Updated";
                    response.ObjectID = category.categoryID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Category NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblCategories category = new tblCategories();
                    category.category = model.CategoryInfo_Category;
                    category.catalogID = model.CategoryInfo_CatalogID;
                    category.active = true;
                    if (model.CategoryInfo_ParentCategory != 0)
                    {
                        category.parentCategoryID = model.CategoryInfo_ParentCategory;
                    }
                    else
                    {
                        category.parentCategoryID = null;
                    }
                    category.showOnWebsite = model.CategoryInfo_ShowOnWebsite;
                    db.tblCategories.AddObject(category);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Category Saved";
                    response.ObjectID = category.categoryID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Category NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeleteCategory(int categoryID)
        {
            AttemptResponse response = new AttemptResponse();

            try
            {
                var query = db.tblCategories.Single(m => m.categoryID == categoryID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Category Deleted";
                response.ObjectID = categoryID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Category NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveCategoriesPerCatalog(string[] catalogID, string[] categories)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var catalog = int.Parse(catalogID[0]);
                var query = db.tblCategories.Where(m => m.catalogID == catalog).Select(m => m);
                foreach (var i in query)
                {
                    i.active = false;
                    if (categories != null)
                    {
                        if (categories.Contains(i.categoryID.ToString()))
                            i.active = true;
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Categories Saved";
                response.ObjectID = catalogID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Categories NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveCatalogsPerTerminal(string[] terminal, string[] catalogs)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var terminalID = int.Parse(terminal[0]);
                var query = db.tblCatalogs_Terminals.Where(m => m.terminalID == terminalID).Select(m => m);
                foreach (var i in query)
                    db.tblCatalogs_Terminals.DeleteObject(i);
                if (catalogs != null)
                {
                    foreach (var i in catalogs)
                    {
                        tblCatalogs_Terminals catalog = new tblCatalogs_Terminals();
                        catalog.terminalID = terminalID;
                        catalog.catalogID = int.Parse(i);
                        db.tblCatalogs_Terminals.AddObject(catalog);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Catalogs Saved";
                response.ObjectID = terminalID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Catalogs NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveDestinationsPerTerminal(string[] terminal, string[] destinations)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var terminalID = int.Parse(terminal[0]);
                var query = db.tblTerminals_Destinations.Where(m => m.terminalID == terminalID);
                foreach (var i in query)
                {
                    db.tblTerminals_Destinations.DeleteObject(i);
                }
                if (destinations != null)
                {
                    foreach (var i in destinations)
                    {
                        tblTerminals_Destinations destination = new tblTerminals_Destinations();
                        destination.terminalID = terminalID;
                        destination.destinationID = long.Parse(i);
                        db.tblTerminals_Destinations.AddObject(destination);
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Destinations Saved";
                response.ObjectID = terminalID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Destinations NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveCategoryDescription(CategoryDescriptionInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            if (model.CategoryDescriptionInfo_CategoryDescriptionID != 0)
            {
                try
                {
                    tblCategoryDescriptions description = db.tblCategoryDescriptions.Single(m => m.categoryDescriptionID == model.CategoryDescriptionInfo_CategoryDescriptionID);

                    description.culture = model.CategoryDescriptionInfo_Culture;
                    description.category = model.CategoryDescriptionInfo_Category;
                    description.showOnWebsite = model.CategoryDescriptionInfo_ShowOnWebsite;
                    description.imgHeader = model.CategoryDescriptionInfo_ImgHeader;
                    description.imgIcon = model.CategoryDescriptionInfo_ImgIcon;
                    description.description = model.CategoryDescriptionInfo_Description;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Category Description Updated";
                    response.ObjectID = description.categoryDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Category Description NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblCategoryDescriptions description = new tblCategoryDescriptions();
                    description.categoryID = model.CategoryDescriptionInfo_CategoryID;
                    description.culture = model.CategoryDescriptionInfo_Culture;
                    description.category = model.CategoryDescriptionInfo_Category;
                    description.showOnWebsite = model.CategoryDescriptionInfo_ShowOnWebsite;
                    description.imgHeader = model.CategoryDescriptionInfo_ImgHeader;
                    description.imgIcon = model.CategoryDescriptionInfo_ImgIcon;
                    description.description = model.CategoryDescriptionInfo_Description;
                    db.tblCategoryDescriptions.AddObject(description);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Category Description Saved";
                    response.ObjectID = description.categoryDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Category Description NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeleteCategoryDescription(int categoryDescriptionID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblCategoryDescriptions.Single(m => m.categoryDescriptionID == categoryDescriptionID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Category Description Deleted";
                response.ObjectID = categoryDescriptionID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Category Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveDomain(DomainInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();

            if (model.DomainInfo_DomainID != 0)
            {
                try
                {
                    tblTerminalDomains terminalDomain = db.tblTerminalDomains.Single(m => m.terminalDomainID == model.DomainInfo_DomainID);
                    terminalDomain.domain = model.DomainInfo_Domain;
                    terminalDomain.defaultPage = model.DomainInfo_DefaultPage;
                    terminalDomain.defaultMasterPage = model.DomainInfo_DefaultMasterPage;
                    terminalDomain.masterPageHeader = model.DomainInfo_MasterPageHeader;
                    terminalDomain.masterPageFooter = model.DomainInfo_MasterPageFooter;
                    terminalDomain.scriptsHeader = model.DomainInfo_ScriptsHeader;
                    terminalDomain.scriptsAfterBody = model.DomainInfo_ScriptsAfterBody;
                    terminalDomain.scriptsFooter = model.DomainInfo_ScriptsFooter;
                    terminalDomain.logo = model.DomainInfo_Logo;
                    terminalDomain.phoneUS = model.DomainInfo_PhoneUS;
                    terminalDomain.phoneMX = model.DomainInfo_PhoneMX;
                    terminalDomain.phoneUSMobile = model.DomainInfo_PhoneUSMobile;
                    terminalDomain.phoneMXMobile = model.DomainInfo_PhoneMXMobile;
                    terminalDomain.phoneAlt1 = model.DomainInfo_PhoneAlt1;
                    terminalDomain.phoneAlt1Mobile = model.DomainInfo_PhoneAlt1Mobile;
                    terminalDomain.phoneAlt2 = model.DomainInfo_PhoneAlt2;
                    terminalDomain.phoneAlt2Mobile = model.DomainInfo_PhoneAlt2Mobile;
                    terminalDomain.culture = model.DomainInfo_Culture;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Terminal Domain Updated";
                    response.ObjectID = terminalDomain.terminalDomainID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Domain NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblTerminalDomains terminalDomain = new tblTerminalDomains();
                    terminalDomain.terminalID = model.DomainInfo_TerminalID;
                    terminalDomain.domain = model.DomainInfo_Domain;
                    terminalDomain.defaultPage = model.DomainInfo_DefaultPage;
                    terminalDomain.defaultMasterPage = model.DomainInfo_DefaultMasterPage;
                    terminalDomain.masterPageHeader = model.DomainInfo_MasterPageHeader;
                    terminalDomain.masterPageFooter = model.DomainInfo_MasterPageFooter;
                    terminalDomain.scriptsHeader = model.DomainInfo_ScriptsHeader;
                    terminalDomain.scriptsAfterBody = model.DomainInfo_ScriptsAfterBody;
                    terminalDomain.scriptsFooter = model.DomainInfo_ScriptsFooter;
                    terminalDomain.logo = model.DomainInfo_Logo;
                    terminalDomain.phoneUS = model.DomainInfo_PhoneUS;
                    terminalDomain.phoneMX = model.DomainInfo_PhoneMX;
                    terminalDomain.phoneUSMobile = model.DomainInfo_PhoneUSMobile;
                    terminalDomain.phoneMXMobile = model.DomainInfo_PhoneMXMobile;
                    terminalDomain.phoneAlt1 = model.DomainInfo_PhoneAlt1;
                    terminalDomain.phoneAlt1Mobile = model.DomainInfo_PhoneAlt1Mobile;
                    terminalDomain.phoneAlt2 = model.DomainInfo_PhoneAlt2;
                    terminalDomain.phoneAlt2Mobile = model.DomainInfo_PhoneAlt2Mobile;
                    terminalDomain.culture = model.DomainInfo_Culture;
                    db.tblTerminalDomains.AddObject(terminalDomain);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Terminal Domain Saved";
                    response.ObjectID = terminalDomain.terminalDomainID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Domain NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public DomainInfoModel GetDomain(int terminalDomainID)
        {
            DomainInfoModel model = new DomainInfoModel();

            var query = db.tblTerminalDomains.Single(m => m.terminalDomainID == terminalDomainID);
            model.DomainInfo_TerminalID = int.Parse(query.terminalID.ToString());
            model.DomainInfo_Domain = query.domain;
            model.DomainInfo_DefaultPage = query.defaultPage;
            model.DomainInfo_DefaultMasterPage = query.defaultMasterPage;
            model.DomainInfo_MasterPageHeader = query.masterPageHeader;
            model.DomainInfo_MasterPageFooter = query.masterPageFooter;
            model.DomainInfo_ScriptsHeader = query.scriptsHeader;
            model.DomainInfo_ScriptsAfterBody = query.scriptsAfterBody;
            model.DomainInfo_ScriptsFooter = query.scriptsFooter;
            model.DomainInfo_Logo = query.logo;
            model.DomainInfo_PhoneUS = query.phoneUS;
            model.DomainInfo_PhoneMX = query.phoneMX;
            model.DomainInfo_PhoneUSMobile = query.phoneUSMobile;
            model.DomainInfo_PhoneMXMobile = query.phoneMXMobile;
            model.DomainInfo_PhoneAlt1 = query.phoneAlt1;
            model.DomainInfo_PhoneAlt1Mobile = query.phoneAlt1Mobile;
            model.DomainInfo_PhoneAlt2 = query.phoneAlt2;
            model.DomainInfo_PhoneAlt2Mobile = query.phoneAlt2Mobile;
            model.DomainInfo_Culture = query.culture;
            return model;
        }

        public AttemptResponse DeleteDomain(int domainID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblTerminalDomains.Single(m => m.terminalDomainID == domainID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Domain Deleted";
                response.ObjectID = domainID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Domain NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        private string builder = "[";
        private static string firstPath = HttpContext.Current.Server.MapPath("~/");
        private static string secondPath = (firstPath.LastIndexOf("ePlatBack") >=0 ? firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack")) : "");
        private static string finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\banners\\";

        public string GetTree(int itemID)
        {
            var tempPath = finalPath + "\\" + "b" + itemID;
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            var directories = Directory.GetDirectories(finalPath, ".", SearchOption.TopDirectoryOnly);
            var childrenNodes = directories.Count();
            foreach (var directory in directories)
            {
                var _name = directory.Substring(directory.LastIndexOf("\\") + 1, directory.Length - (directory.LastIndexOf("\\") + 1));
                var bgID = long.Parse(_name.Substring(1, _name.Length - 1));
                var bannerQ = db.tblBannerGroups.Where(m => m.bannerGroupID == bgID).FirstOrDefault();
                var name = (bannerQ != null ? bannerQ.bannerGroup :  "Banner Group " + bgID);
                builder += "{\"attr\": { \"id\": \"" + _name + "_" + _name + "\"}, \"data\": \"" + name + "\", \"children\": [";
                builder += "]}";
                if (Directory.GetParent(directory).GetDirectories(".", SearchOption.TopDirectoryOnly).Last().FullName != directory)
                {
                    builder += ",";
                }
            }
            builder += "]";
            return builder;
        }

        public object GetFiles(string directory, int terminalDomainID)
        {
            var firstPath = HttpContext.Current.Server.MapPath("~/");
            var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
            var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\banners";
            string[] files = Directory.GetFiles(@finalPath + directory);
            List<BannerInfoModel> list = new List<BannerInfoModel>();
            
            foreach (var file in files)
            {
                var imagePath = file.Substring(file.IndexOf("\\Content\\"), (file.Length - file.LastIndexOf("\\Content\\"))).Replace("\\", "/").ToLower();
                var query = db.tblBanners.Where(m => (m.path).Equals(imagePath) && m.terminalID == db.tblTerminalDomains.FirstOrDefault(x => x.terminalDomainID == terminalDomainID).terminalID);
                foreach (var i in query.OrderBy(m => m.order_))
                {
                    list.Add(new BannerInfoModel()
                    {
                        BannerInfo_BannerID = i.bannerID,
                        BannerInfo_BannerName = i.bannerName,
                        BannerInfo_Path = i.path,
                        BannerInfo_BannerGroupID = i.bannerGroupID,
                        BannerInfo_FromDate = i.fromDate != null ? ((DateTime)i.fromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                        BannerInfo_Order = i.order_,
                        BannerInfo_Permanent = i.permanent_,
                        BannerInfo_ToDate = i.toDate != null ? ((DateTime)i.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                        BannerInfo_Url = i.url
                    });
                }
                //var query = db.tblBanners.Single(m => (m.path).Equals(imagePath));
                //list.Add(new BannerInfoModel()
                //{
                //    BannerInfo_BannerID = query.bannerID,
                //    BannerInfo_BannerName = query.bannerName,
                //    BannerInfo_Path = query.path,
                //    BannerInfo_BannerGroupID = query.bannerGroupID,
                //    BannerInfo_FromDate = query.fromDate != null ? ((DateTime)query.fromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                //    BannerInfo_Order = query.order_,
                //    BannerInfo_Permanent = query.permanent_,
                //    BannerInfo_ToDate = query.toDate != null ? ((DateTime)query.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                //    BannerInfo_Url = query.url
                //});
            }
            return list;
        }

        public PictureDataModel.FineUploaderResult UploadBanner(PictureDataModel.FineUpload upload, string bannerName, string path, string bannerGroup, string url, string permanent, string from, string to, string culture, long? terminalID)
        {
            AttemptResponse response = new AttemptResponse();
            var firstPath = HttpContext.Current.Server.MapPath("~/");
            var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
            var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\banners";
            var fileName = upload.Filename;

            var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
            fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
            fileName = fileName.Replace("+", "");
            for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
            {
                var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                var newFileName = fileName.Replace(encoded, "_");
                fileName = newFileName;
            }

            var filePath = finalPath + path + fileName;
            //var picturePath = "/content/themes/base/images/banners" + path.Replace("\\", "/");
            var picturePath = "/content/themes/base/images/banners" + path.Replace("\\", "/") + fileName;
            try
            {
                //var existingBanner = db.tblBanners.Where(m => m.path == picturePath && m.bannerName == fileName).Count();
                var existingBanner = db.tblBanners.Where(m => m.path == picturePath).Count();
                if (existingBanner > 0)
                {
                    throw new Exception("Another banner has same name, change name before upload");
                }
                upload.SaveAs(filePath);
                tblBanners banner = new tblBanners();
                //banner.bannerName = fileName;
                banner.bannerName = bannerName;
                banner.path = picturePath;
                banner.bannerGroupID = long.Parse(bannerGroup);
                banner.url = url;
                banner.permanent_ = bool.Parse(permanent);
                banner.fromDate = from != "" ? DateTime.Parse(from, CultureInfo.InvariantCulture) : DateTime.Today;
                banner.toDate = to != "" ? DateTime.Parse(to, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                banner.culture = culture;
                banner.terminalID = terminalID;
                db.tblBanners.AddObject(banner);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Banner Uploaded";
                response.ObjectID = banner.bannerID;
                return new PictureDataModel.FineUploaderResult(true, new { response = response }, new { path = picturePath });
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Banner NOT Uploaded";
                response.ObjectID = 0;
                response.Exception = ex;
                return new PictureDataModel.FineUploaderResult(false, new { response = response });
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        public List<BannerGroupInfoModel> GetBannerGroupsPerDomain(int terminalDomainID)
        {
            List<BannerGroupInfoModel> list = new List<BannerGroupInfoModel>();
            foreach (var i in db.tblBannerGroups) //.Where(m => m.terminalDomainID == terminalDomainID)
            {
                list.Add(new BannerGroupInfoModel()
                {
                    BannerGroupInfo_BannerGroupID = i.bannerGroupID,
                    BannerGroupInfo_TerminalDomainID = (int)i.terminalDomainID,
                    BannerGroupInfo_BannerGroup = i.bannerGroup,
                    BannerGroupInfo_Width = (int)i.width,
                    BannerGroupInfo_Height = (int)i.height
                });
            }
            return list;
        }

        public BannerGroupInfoModel GetBannerGroup(long bannerGroupID)
        {
            BannerGroupInfoModel model = new BannerGroupInfoModel();

            var query = db.tblBannerGroups.Single(m => m.bannerGroupID == bannerGroupID);
            model.BannerGroupInfo_BannerGroupID = query.bannerGroupID;
            model.BannerGroupInfo_TerminalDomainID = (int)query.terminalDomainID;
            model.BannerGroupInfo_BannerGroup = query.bannerGroup;
            model.BannerGroupInfo_Width = (int)query.width;
            model.BannerGroupInfo_Height = (int)query.height;
            return model;
        }

        public List<BannerInfoModel> GetBannersPerGroup(long bannerGroupID)
        {
            List<BannerInfoModel> list = new List<BannerInfoModel>();
            foreach (var i in db.tblBanners.Where(m => m.bannerGroupID == bannerGroupID))
            {
                list.Add(new BannerInfoModel()
                {
                    BannerInfo_BannerGroupID = i.bannerGroupID,
                    BannerInfo_BannerID = i.bannerID,
                    BannerInfo_BannerName = i.bannerName,
                    BannerInfo_FromDate = i.fromDate != null ? ((DateTime)i.fromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                    BannerInfo_Order = i.order_,
                    BannerInfo_Path = i.path,
                    BannerInfo_Permanent = i.permanent_,
                    BannerInfo_ToDate = i.toDate != null ? ((DateTime)i.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                    BannerInfo_Url = i.url,
                    BannerInfo_TerminalID = i.terminalID
                });
            }
            return list;
        }

        public static List<BannerGroup> GettingBannerGroups(string ids)
        {
            ePlatEntities db = new ePlatEntities();
            string culture = Utils.GeneralFunctions.GetCulture();
            List<BannerGroup> list = new List<BannerGroup> { };
            List<long> arrayids = new List<long>();
            string[] arrayStr = ids.Split(',');
            foreach (string s in arrayStr)
            {
                arrayids.Add(Convert.ToInt64(s));
            }
            //IEnumerable<long> arrayids = arrayStr.Select(x => long.Parse(x));
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var groupsQ = from g in db.tblBannerGroups
                          where arrayids.Contains(g.bannerGroupID)
                          select g;

            foreach (var g in groupsQ)
            {
                tblBannerGroups bannerGroup = g;
                List<BannerItem> banners = new List<BannerItem>();
                //foreach (var b in bannerGroup.tblBanners.Where(x => x.culture == culture))
                foreach (var b in bannerGroup.tblBanners.Where(x=>x.culture == culture && x.terminalID == terminalID && (x.permanent_ || x.toDate >= DateTime.Today.AddDays(1).AddSeconds(-1))).OrderBy(o => o.order_))
                {
                    BannerItem newBanner = new BannerItem()
                    {
                        Path = b.path,
                        Url = b.url,
                        Html = b.html,
                        Order = b.order_
                    };
                    banners.Add(newBanner);
                }

                BannerGroup newGroup = new BannerGroup()
                {
                    BannerGroupID = g.bannerGroupID,
                    Width = g.width,
                    Height = g.height,
                    Banners = banners
                };
                list.Add(newGroup);
            }            

            return list;
        }

        public AttemptResponse SaveBannerGroup(BannerGroupInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.BannerGroupInfo_BannerGroupID != 0)
            {
                try
                {
                    var bannerGroup = db.tblBannerGroups.Single(m => m.bannerGroupID == model.BannerGroupInfo_BannerGroupID);
                    bannerGroup.bannerGroup = model.BannerGroupInfo_BannerGroup;
                    bannerGroup.width = model.BannerGroupInfo_Width;
                    bannerGroup.height = model.BannerGroupInfo_Height;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Banner Group Updated";
                    response.ObjectID = bannerGroup.bannerGroupID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Banner Group NOT Updated";
                    response.Exception = ex;
                    response.ObjectID = 0;
                    return response;
                }
            }
            else
            {
                try
                {
                    var bannerGroup = new tblBannerGroups();
                    bannerGroup.bannerGroup = model.BannerGroupInfo_BannerGroup;
                    bannerGroup.terminalDomainID = model.BannerGroupInfo_TerminalDomainID;
                    bannerGroup.width = model.BannerGroupInfo_Width;
                    bannerGroup.height = model.BannerGroupInfo_Height;
                    db.tblBannerGroups.AddObject(bannerGroup);
                    db.SaveChanges();
                    try
                    {
                        Directory.CreateDirectory(finalPath + "b" + bannerGroup.bannerGroupID);
                    }
                    catch (Exception ex)
                    {
                        db.tblBannerGroups.DeleteObject(db.tblBannerGroups.Single(m => m.bannerGroupID == bannerGroup.bannerGroupID));
                        db.SaveChanges();
                        throw new Exception("There was an error trying to create directory");
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Banner Group Saved";
                    response.ObjectID = bannerGroup.bannerGroupID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Banner Group NOT Saved";
                    response.Exception = ex;
                    response.ObjectID = 0;
                    return response;
                }
            }
        }

        public AttemptResponse DeleteBannerGroup(long bannerGroupID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var firstPath = HttpContext.Current.Server.MapPath("~/");
                var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                var finalPath = secondPath + "ePlatFront";

                var query = db.tblBannerGroups.Single(m => m.bannerGroupID == bannerGroupID);
                var path = finalPath + "\\Content\\themes\\base\\images\\banners\\b" + query.bannerGroupID;

                if (Directory.GetFiles(path, ".", SearchOption.AllDirectories).Count() == 0)
                {
                    Directory.Delete(path);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Banner Group Deleted";
                    response.ObjectID = bannerGroupID;
                    return response;
                }
                else
                {
                    throw new Exception("It Contains Other Files");
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Banner Group NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse UpdateBanner(BannerInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            //if (model.BannerInfo_BannerID != 0)
            {
                try
                {
                    var banner = db.tblBanners.Single(m => m.bannerID == model.BannerInfo_BannerID);
                    banner.bannerName = model.BannerInfo_BannerName;
                    banner.path = model.BannerInfo_Path;
                    banner.url = model.BannerInfo_Url;
                    banner.permanent_ = model.BannerInfo_Permanent;
                    banner.fromDate = DateTime.Parse(model.BannerInfo_FromDate, CultureInfo.InvariantCulture);
                    //banner.fromDate = (model.BannerInfo_Permanent) ? DateTime.Today : DateTime.Parse(model.BannerInfo_FromDate, CultureInfo.InvariantCulture); //check if this will depend from IsPermanent value
                    if (model.BannerInfo_Permanent)
                    {
                        banner.toDate = (DateTime?)null;
                    }
                    else
                    {
                        banner.toDate = DateTime.Parse(model.BannerInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    }
                    banner.terminalID = model.BannerInfo_TerminalID;
                    banner.order_ = model.BannerInfo_Order;
                    banner.culture = model.BannerInfo_Culture;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Banner Updated";
                    response.ObjectID = banner.bannerID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Banner NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            //else
            //{
            //    try
            //    {
            //        var banner = new tblBanners();
            //        banner.bannerGroupID = model.BannerInfo_BannerGroupID;
            //        banner.bannerName = model.BannerInfo_BannerName;
            //        banner.path = model.BannerInfo_Path;
            //        banner.url = model.BannerInfo_Url;
            //        banner.order_ = model.BannerInfo_Order;
            //        banner.fromDate = (model.BannerInfo_Permanent) ? DateTime.Today : DateTime.Parse(model.BannerInfo_FromDate, CultureInfo.InvariantCulture); //check if this will depend from IsPermanent value
            //        banner.permanent_ = model.BannerInfo_Permanent;
            //        if (!model.BannerInfo_Permanent)
            //        {
            //            banner.toDate = DateTime.Parse(model.BannerInfo_ToDate, CultureInfo.InvariantCulture);
            //        }
            //        db.AddObject("tblBanners", banner);
            //        db.SaveChanges();
            //        response.Type = Attempt_ResponseTypes.Ok;
            //        response.Message = "Banner Saved";
            //        response.ObjectID = banner.bannerID;
            //        return response;
            //    }
            //    catch (Exception ex)
            //    {
            //        response.Type = Attempt_ResponseTypes.Error;
            //        response.Message = "Banner NOT Saved";
            //        response.ObjectID = 0;
            //        response.Exception = ex;
            //        return response;
            //    }
            //}
        }

        public AttemptResponse DeleteBanner(long bannerID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var firstPath = HttpContext.Current.Server.MapPath("~/");
                var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
                var finalPath = secondPath + "ePlatFront";

                var query = db.tblBanners.Single(m => m.bannerID == bannerID);
                var path = finalPath + query.path;// +query.bannerName;
                var file = new FileInfo(path.Replace("/", "\\"));
                file.Delete();
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Banner Deleted";
                response.ObjectID = bannerID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Banner NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public BannerInfoModel GetBannerInfo(long bannerID)
        {
            ePlatEntities db = new ePlatEntities();
            BannerInfoModel model = new BannerInfoModel();

            var query = db.tblBanners.Single(m => m.bannerID == bannerID);
            model.BannerInfo_BannerGroupID = query.bannerGroupID;
            model.BannerInfo_BannerID = query.bannerID;
            model.BannerInfo_BannerName = query.bannerName;
            model.BannerInfo_FromDate = query.fromDate != null ? ((DateTime)query.fromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            model.BannerInfo_ToDate = query.toDate != null ? ((DateTime)query.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            model.BannerInfo_Permanent = query.permanent_;
            model.BannerInfo_Order = query.order_;
            model.BannerInfo_Path = query.path;
            model.BannerInfo_Url = query.url;
            model.BannerInfo_Culture = query.culture;
            model.BannerInfo_TerminalID = query.terminalID;
            return model;
        }

        public AttemptResponse UpdateBannersOrder(string[] items)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var banners = items.Select((i, v) => new { value = long.Parse(i), index = v });
                foreach (var i in banners)
                {
                    db.tblBanners.Single(m => m.bannerID == i.value).order_ = i.index;
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Banners Order Updated";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Banners Order NOT Updated";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }
    }
}
