
using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ePlatBack.Models.DataModels
{
    public class PageDataModel
    {
        public static UserSession session = new UserSession();

        ePlatEntities db = new ePlatEntities();
        private string builder = "[";
        //private string builder = "";
        public class PageCatalogs
        {
            public static List<SelectListItem> FillDrpCultures()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "", Text = "--Select One--", Selected = false });
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

            public static List<SelectListItem> FillDrpTerminals()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in TerminalDataModel.GetCurrentUserTerminals())
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.Value,
                        Text = i.Text
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpParentPages()
            {
                return FillDrpParentPages(null);
            }

            public static List<SelectListItem> FillDrpParentPages(long? terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                List<long> terminals = new List<long>();
                if (terminalID == null)
                {
                    terminals = session.Terminals != "" ?
                        session.Terminals.Split(',').Select(m => long.Parse(m)).ToList() :
                        session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
                }
                else
                {
                    terminals.Add((long)terminalID);
                }
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblContentPages.Where(x => terminals.Contains(x.terminalID)))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.pageID.ToString(),
                        Text = i.page
                    });
                }
                return list;
            }//pageID is to avoid a page to be parent of itself

            public static List<SelectListItem> FillDrpPageTypes()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPageTypes)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.pageTypeID.ToString(),
                        Text = i.pageType
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpPageStructures()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPageStructures)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.pageStructureID.ToString(),
                        Text = i.structureName
                    });
                }
                return list;
            }//this probably will need terminalID as parameter

            public static List<SelectListItem> FillDrpOrder()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                var query = db.tblContentPages.Count();
                for (var i = 0; i <= query; i++)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = (i + 1).ToString(),
                        Text = (i + 1).ToString()
                    });
                }
                return list;
            }

            public static string ReplaceTabs(string value, int spaces)
            {
                string newBuilder = new String(' ', spaces);
                return value.Replace("\t", newBuilder);
            }
        }

        //init back
        public object SearchPages(PageSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            return "";
        }

        public static PageContainer GetPageContainer(int id)
        {
            ePlatEntities db = new ePlatEntities();
            PageContainer container = new PageContainer();

            //revisar si está autorizado
            var Access = (from a in db.tblUsers_SysWorkGroups
                         join sP in db.tblSysProfiles 
                         on a.sysWorkGroupID equals sP.sysWorkGroupID
                         join c in db.tblSysComponents
                         on sP.sysComponentID equals c.sysComponentID
                         into c_sp
                         from c in c_sp.DefaultIfEmpty()
                         where a.userID == session.UserID
                         && sP.roleID == a.roleID
                         && sP.sysComponentID == id
                         select new {
                             sP.view_,
                             c.sysComponent,
                             c.description,
                             c.url
                         }).FirstOrDefault();

            if(Access != null)
            {
                if (Access.view_)
                {
                    container.Allowed = true;
                    container.Title = Access.sysComponent;
                    container.Subtitle = Access.description;
                    container.Url = Access.url;
                } else
                {
                    container.Allowed = false;
                }
            } else
            {
                container.Allowed = false;
            }

            return container;
        }

        public List<TreeItem> GetTree(string terminals)
        {
            ePlatEntities db = new ePlatEntities();
            List<TreeItem> pagesList = new List<TreeItem>();
            List<KeyValuePair<string, long>> listTerminals = new List<KeyValuePair<string, long>>();
            if (terminals != "")
            {
                foreach (var i in terminals.Split(','))
                    listTerminals.Add(new KeyValuePair<string, long>("", Int64.Parse(i)));
            }
            var arrayTerminals = listTerminals.Select(m => m.Value);
            var query = from page in db.tblContentPages
                        where arrayTerminals.Contains(page.terminalID)
                        orderby page.terminalID, page.order_ ascending
                        select page;

            string clickable = "";
            string showInMenu = "";
            foreach (var page in query)
            {
                if (page.parentPageID == null)
                {
                    clickable = page.clickable ? "checked='checked'" : "";
                    showInMenu = page.showInMenu ? "checked='checked'" : "";
                    pagesList.Add(new TreeItem()
                    {
                        attr = new TreeItemAttributes() { id = page.pageID.ToString() },
                        data = page.page,
                        metadata = new TreeItemMetaData()
                        {
                            clickable = "<input type='checkbox' disabled='true' " + clickable + "/>",
                            showInMenu = "<input type='checkbox' disabled='true' " + showInMenu + "/>",
                            delete = "<img src='/Content/themes/base/images/cross.png' class='right delete-item' />"
                        },
                        children = FindChildrenNodes(page.pageID)
                    });
                }
            }
            return pagesList;
        }

        public List<TreeItem> FindChildrenNodes(long pageID)
        {
            ePlatEntities db = new ePlatEntities();
            List<TreeItem> pagesList = new List<TreeItem>();
            var children = from page in db.tblContentPages
                           where page.parentPageID == pageID
                           orderby page.order_ ascending
                           select page;

            string clickable = "";
            string showInMenu = "";
            foreach (var page in children)
            {
                clickable = page.clickable ? "checked='checked'" : "";
                showInMenu = page.showInMenu ? "checked='checked'" : "";
                pagesList.Add(new TreeItem()
                {
                    attr = new TreeItemAttributes() { id = page.pageID.ToString() },
                    data = page.page,
                    metadata = new TreeItemMetaData()
                    {
                        clickable = "<input type='checkbox' disabled='true' " + clickable + "/>",
                        showInMenu = "<input type='checkbox' disabled='true' " + showInMenu + "/>",
                        delete = "<img src='/Content/themes/base/images/cross.png' class='right delete-item' />"
                    },
                    children = FindChildrenNodes(page.pageID)
                });
            }
            return pagesList;
        }

        public PageInfoModel GetPageInfo(int pageID)
        {
            ePlatEntities db = new ePlatEntities();
            PageInfoModel model = new PageInfoModel();
            try
            {
                var query = db.tblContentPages.Single(m => m.pageID == pageID);
                model.PageInfo_PageID = int.Parse(query.pageID.ToString());
                model.PageInfo_Page = query.page;
                model.PageInfo_Terminal = int.Parse(query.terminalID.ToString());
                model.PageInfo_ShowInMenu = query.showInMenu;
                model.PageInfo_Order = query.order_;
                if (query.parentPageID != null)
                {
                    model.PageInfo_ParentPage = (int)query.parentPageID;
                }
                else
                {
                    model.PageInfo_ParentPage = 0;
                }
                model.PageInfo_PageType = query.pageTypeID;
                model.PageInfo_Clickable = query.clickable;
            }
            catch
            {
            }
            return model;
        }

        public AttemptResponse SavePageInfo(PageInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PageInfo_PageID != 0)
            {
                try
                {
                    var query = db.tblContentPages.Single(m => m.pageID == model.PageInfo_PageID);
                    query.page = model.PageInfo_Page;
                    query.terminalID = model.PageInfo_Terminal;
                    query.showInMenu = model.PageInfo_ShowInMenu;
                    query.order_ = model.PageInfo_Order;
                    if (model.PageInfo_ParentPage != 0)
                    {
                        query.parentPageID = model.PageInfo_ParentPage;
                    }
                    else
                    {
                        query.parentPageID = null;
                    }
                    query.pageTypeID = model.PageInfo_PageType;
                    query.clickable = model.PageInfo_Clickable;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Page Updated";
                    response.ObjectID = query.pageID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Page NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var query = new tblContentPages();
                    query.page = model.PageInfo_Page;
                    query.terminalID = model.PageInfo_Terminal;
                    query.showInMenu = model.PageInfo_ShowInMenu;
                    query.order_ = model.PageInfo_Order;
                    if (model.PageInfo_ParentPage != 0)
                    {
                        query.parentPageID = model.PageInfo_ParentPage;
                    }
                    query.pageTypeID = model.PageInfo_PageType;
                    query.clickable = model.PageInfo_Clickable;
                    query.savedByUserID = session.UserID;
                    query.dateSaved = DateTime.Now;
                    db.tblContentPages.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Page Saved";
                    response.ObjectID = query.pageID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Page NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeletePage(int pageID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var page = db.tblContentPages.Single(m => m.pageID == pageID);
                db.DeleteObject(page);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Page Deleted";
                response.ObjectID = pageID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Page NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public List<PageDescriptionModel> GetPageDescriptions(int pageID)
        {
            ePlatEntities db = new ePlatEntities();
            List<PageDescriptionModel> list = new List<PageDescriptionModel>();
            try
            {
                var query = db.tblContentPageDescriptions.Where(m => m.pageID == pageID).Select(m => m);
                foreach (var i in query)
                {
                    list.Add(new PageDescriptionModel()
                    {
                        PageDescription_PageID = int.Parse(i.pageID.ToString()),
                        PageDescription_PageDescriptionID = int.Parse(i.pageDescriptionID.ToString()),
                        PageDescription_Culture = i.culture,
                        PageDescription_Header = i.header,
                        PageDescription_ContentHeader = i.content_header,
                        PageDescription_Content = i.content_,
                        PageDescription_AfterBody = i.afterBody,
                        PageDescription_Footer = i.footer,
                        PageDescription_PageStructure = i.pageStructureID,
                        PageDescription_IsActive = i.active,
                        PageDescription_Url = i.url
                    });
                }
            }
            catch
            {
            }
            return list;
        }

        public PageDescriptionModel GetPageDescription(int pageDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            PageDescriptionModel model = new PageDescriptionModel();
            try
            {
                var query = db.tblContentPageDescriptions.Single(m => m.pageDescriptionID == pageDescriptionID);
                model.PageDescription_PageID = int.Parse(query.pageID.ToString());
                model.PageDescription_PageDescriptionID = int.Parse(query.pageDescriptionID.ToString());
                model.PageDescription_Culture = query.culture;
                model.PageDescription_Header = query.header;
                model.PageDescription_ContentHeader = query.content_header;
                model.PageDescription_Content = query.content_;
                model.PageDescription_AfterBody = query.afterBody;
                model.PageDescription_Footer = query.footer;
                model.PageDescription_PageStructure = query.pageStructureID;
                model.PageDescription_IsActive = query.active;
                model.PageDescription_Url = query.url;
            }
            catch
            {
            }
            return model;
        }

        public AttemptResponse SavePageDescription(PageDescriptionModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PageDescription_PageDescriptionID != 0)
            {
                try
                {
                    var query = db.tblContentPageDescriptions.Single(m => m.pageDescriptionID == model.PageDescription_PageDescriptionID);
                    query.culture = model.PageDescription_Culture;
                    query.header = model.PageDescription_Header;
                    query.content_header = model.PageDescription_ContentHeader;
                    query.content_ = model.PageDescription_Content;
                    query.afterBody = model.PageDescription_AfterBody;
                    query.footer = model.PageDescription_Footer;
                    query.pageStructureID = model.PageDescription_PageStructure;
                    query.active = model.PageDescription_IsActive;
                    query.url = model.PageDescription_Url;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Page Description Updated";
                    response.ObjectID = query.pageDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Page Description NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var query = new tblContentPageDescriptions();
                    query.pageID = model.PageDescription_PageID;
                    query.culture = model.PageDescription_Culture;
                    query.header = model.PageDescription_Header;
                    query.content_header = model.PageDescription_ContentHeader;
                    query.content_ = model.PageDescription_Content;
                    query.afterBody = model.PageDescription_AfterBody;
                    query.footer = model.PageDescription_Footer;
                    query.pageStructureID = model.PageDescription_PageStructure;
                    query.active = model.PageDescription_IsActive;
                    query.url = model.PageDescription_Url;
                    db.tblContentPageDescriptions.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Page Description Saved";
                    response.ObjectID = query.pageDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Page Description NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeletePageDescription(int pageDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var description = db.tblContentPageDescriptions.Single(m => m.pageDescriptionID == pageDescriptionID);
                db.tblContentPageDescriptions.DeleteObject(description);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Page Description Deleted";
                response.ObjectID = pageDescriptionID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Page Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public List<SelectListItem> GetDDLData(string itemType, string itemID)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            switch (itemType)
            {
                case "pages":
                    {
                        if (itemID != null && itemID != "0" && itemID != "")
                        {
                            long id = long.Parse(itemID);
                            list = PageCatalogs.FillDrpParentPages(id);
                        }
                        else
                        {
                            list = PageCatalogs.FillDrpParentPages();
                        }
                        break;
                    }
                case "selectedTerminals":
                    {
                        return TerminalDataModel.GetActiveTerminalsList();
                    }
            }
            return list;
        }

        public string Upload(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var firstPath = HttpContext.Current.Server.MapPath("~/");
            var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
            var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\pages\\";
            var fileName = upload.FileName;
            string message;
            string fileUrl = String.Empty;
            try
            {
                var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
                fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
                fileName = fileName.Replace("+", "");
                for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
                {
                    var encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                    var newFileName = fileName.Replace(encoded, "_");
                    fileName = newFileName;
                }

                var filePath = finalPath + fileName;
                upload.SaveAs(filePath);
                fileUrl = "//eplatfront.villagroup.com/content/themes/base/images/pages/" + fileName;
                message = "Image Saved";
            }
            catch (Exception ex)
            {
                message = "Image NOT Saved:<br />" + ex.Message;
            }
            return @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + fileUrl + "\", \"" + message + "\");</script></body></html>";
        }

        public BrowseImageModel Browse(string CKEditorFuncNum, string CKEditor, string langCode)
        {
            var firstPath = HttpContext.Current.Server.MapPath("~/");
            var secondPath = firstPath.Substring(0, firstPath.LastIndexOf("ePlatBack"));
            var finalPath = secondPath + "ePlatFront\\Content\\themes\\base\\images\\pages\\";

            var browse = new BrowseImageModel { CKEditorFuncNum = CKEditorFuncNum };
            var directoryPath = "//eplatfront.villagroup.com/content/themes/base/images/pages/";
            List<string> paths = new List<string>();
            List<string> images = new List<string>();
            if (Directory.Exists(finalPath))
            {
                paths = Directory.GetFiles(finalPath).ToList();
            }
            for (var i = 0; i < paths.Count(); i++)
            {
                var a = paths[i].IndexOf("Content");
                var b = paths[i].Length - (paths[i].IndexOf("Content") + 1);
                var c = paths[i].Substring(a, b);
                //images.Add(c);
                images.Add("//eplatfront.villagroup.com/" + paths[i].Substring(paths[i].IndexOf("Content"), paths[i].Length - paths[i].IndexOf("Content")).Replace("\\", "/"));
            }
            browse.ImagePaths = images.AsEnumerable();
            return browse;
        }

        //end back

        //init front
        public PageViewModel GetDefaultPageInfo()
        {
            PageViewModel page = new PageViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            page.Culture = culture.Substring(0, 2).ToLower();
            DomainSettingsViewModel objMaster = GetMasterSettings();
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
            SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
            if (objSubmenus != null)
            {
                page.Submenu1 = objSubmenus.Submenu1;
                page.Submenu2 = objSubmenus.Submenu2;
                page.Submenu3 = objSubmenus.Submenu3;
            }
            return page;
        }

        public PageViewModel GetPageByID(long id)
        {
            PageViewModel page = new PageViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            page.Culture = culture.Substring(0, 2).ToLower();
            DomainSettingsViewModel objMaster = GetMasterSettings();
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
                page.Template_Controls = objMaster.Template_Controls;
                page.CanonicalDomain = objMaster.CanonicalDomain;
            }
            tblSeoItems objSeo = PageDataModel.GetSeo(5, id, culture);
            if (objSeo != null)
            {
                page.Seo_Title = objSeo.title;
                page.Seo_Keywords = objSeo.keywords;
                page.Seo_Description = objSeo.description;
                page.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                page.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");
                page.Seo_FriendlyUrl = objSeo.friendlyUrl;
            }
            tblSeoItems objSeo2 = PageDataModel.GetSeo(8, id, (culture == "es-MX" ? "en-US" : "es-MX"));
            if (objSeo2 != null)
            {
                page.Seo_OppositeLanguageUrl = objSeo2.friendlyUrl;
            }
            SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
            if (objSubmenus != null)
            {
                page.Submenu1 = objSubmenus.Submenu1;
                page.Submenu2 = objSubmenus.Submenu2;
                page.Submenu3 = objSubmenus.Submenu3;
            }
            //get contentPageDescription
            var ContentPage = from p in db.tblContentPageDescriptions
                              where p.pageID == id
                              && p.active == true
                              && p.culture == culture
                              select p;

            if (ContentPage.Count() > 0)
            {
                page.Content = ContentPage.FirstOrDefault().tblPageStructures.structureHtml;
                page.Content = page.Content.Replace("$Content", ContentPage.FirstOrDefault().content_);
                //switch (ContentPage.FirstOrDefault().pageStructureID)
                //{
                //    case 1:
                //        page.Content = "<div class=\"container\"><div class=\"row\"><div class=\"left-column col-lg-9 col-md-8 col-sm-8\">" + ContentPage.FirstOrDefault().content_ + "</div><div class=\"right-column col-lg-3 col-md-4 col-sm-4\">ePlatBlock(\"Right Column\")</div></div></div>";
                //        break;
                //}
                page.Content_Header = ContentPage.FirstOrDefault().content_header;
                page.Scripts_Header += ContentPage.FirstOrDefault().header;
                page.Scripts_AfterBody += ContentPage.FirstOrDefault().afterBody;
                page.Scripts_Footer += ContentPage.FirstOrDefault().footer;
            }

            return page;
        }

        public PageViewModel GetPage(string url)
        {
            PageViewModel page = new PageViewModel();

            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var itemQ = (from x in db.tblSeoItems
                         where x.friendlyUrl == url
                         && x.terminalID == terminalID
                         select x.itemID).FirstOrDefault();

            if (itemQ != null)
            {
                page = GetPageByID((long)itemQ);
            }
            else
            {
                string culture = Utils.GeneralFunctions.GetCulture();
                page.Culture = culture.Substring(0, 2).ToLower();
                DomainSettingsViewModel objMaster = GetMasterSettings();
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
                    page.Template_Controls = objMaster.Template_Controls;
                }
                tblSeoItems objSeo = PageDataModel.GetSeo(5, null, culture);
                if (objSeo != null)
                {
                    page.Seo_Title = objSeo.title;
                    page.Seo_Keywords = objSeo.keywords;
                    page.Seo_Description = objSeo.description;
                    page.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                    page.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");
                }
                SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
                if (objSubmenus != null)
                {
                    page.Submenu1 = objSubmenus.Submenu1;
                    page.Submenu2 = objSubmenus.Submenu2;
                    page.Submenu3 = objSubmenus.Submenu3;
                }
            }
            return page;
        }

        public static IEnumerable<ReviewItemViewModel> GetReviews(int sysItemTypeID, long itemID, string culture)
        {
            List<ReviewItemViewModel> reviews = new List<ReviewItemViewModel>();
            ePlatEntities db = new ePlatEntities();
            var objReviews = from r in db.tblReviews
                             where r.sysItemTypeID == sysItemTypeID
                             && r.itemID == itemID
                             && r.culture == culture
                             select r;
            foreach (tblReviews review in objReviews)
            {
                reviews.Add(new ReviewItemViewModel()
                {
                    Review = review.review,
                    Author = review.author,
                    From = review.from_,
                    Rating = review.rating,
                    Date = (review.dateSaved != null ? review.dateSaved.Value.ToString("yyyy-MM-dd hh:mm:ss tt") : "")
                });
            }

            return reviews;
        }

        public static IEnumerable<ReviewItemViewModel> GetReviewsFromSurvey(long guestNameFID, long cityFID, long commentsFID, long ratingFID, long resortFID, long? placeID = null, bool? constraintMarketingSource = false, long? constraingMSFID = null)
        {
            List<ReviewItemViewModel> reviews = new List<ReviewItemViewModel>();
            ePlatEntities db = new ePlatEntities();

            string resortName = "";
            if (placeID != null)
            {
                var placeQ = (from p in db.tblPlaces
                              where p.placeID == placeID
                              select p.oldPlaceName).FirstOrDefault();

                if (placeQ != null)
                {
                    resortName = placeQ;
                }
            }

            List<long> fieldIDs = new List<long>();
            fieldIDs.Add(guestNameFID);
            fieldIDs.Add(cityFID);
            fieldIDs.Add(commentsFID);
            fieldIDs.Add(ratingFID);
            fieldIDs.Add(resortFID);

            List<Guid> TransactionIDs = (from t in db.tblFieldsValues
                                         where t.fieldID == commentsFID
                                         && t.publish == true
                                         orderby t.dateSaved descending
                                         select t.transactionID).ToList();

            //get transaction IDs for comments from resort
            if (resortName != "")
            {
                TransactionIDs = (from t in db.tblFieldsValues
                                  where TransactionIDs.Contains(t.transactionID)
                                  && t.fieldID == resortFID
                                  && t.value.Contains(resortName)
                                  orderby t.dateSaved descending
                                  select t.transactionID).ToList();
            }
            else
            {
                TransactionIDs = new List<Guid>();
            }

            if (constraintMarketingSource == true)
            {
                long terminalid = Utils.GeneralFunctions.GetTerminalID();
                var marketingSourceQ = (from m in db.tblTerminals
                                        where m.terminalID == terminalid
                                        select m.oldMarketingSources).FirstOrDefault();

                List<string> marketingSources = new List<string>();
                if (marketingSourceQ != null)
                {
                    marketingSources = marketingSourceQ.Split(',').ToList();
                }

                TransactionIDs = (from t in db.tblFieldsValues
                                  where TransactionIDs.Contains(t.transactionID)
                                  && t.fieldID == constraingMSFID
                                  && marketingSources.Contains(t.value)
                                  orderby t.dateSaved descending
                                  select t.transactionID).ToList();
            }

            var fieldsQ = from f in db.tblFieldsValues
                          where TransactionIDs.Contains(f.transactionID)
                          && fieldIDs.Contains(f.fieldID)
                          select new
                          {
                              f.fieldID,
                              f.dateSaved,
                              f.value,
                              f.transactionID
                          };

            foreach (Guid tid in TransactionIDs)
            {
                ReviewItemViewModel review = new ReviewItemViewModel();
                review.Review = fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == commentsFID).value;
                review.Author = fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == guestNameFID).value;
                review.From = fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == cityFID).value;
                if (fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == ratingFID).value != null)
                {
                    review.Rating = decimal.Parse(fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == ratingFID).value);
                }
                review.Date = fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == commentsFID).dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt");
                review.Resort = fieldsQ.FirstOrDefault(x => x.transactionID == tid && x.fieldID == resortFID).value;

                reviews.Add(review);
            }

            return reviews;
        }

        public static IEnumerable<PictureItemViewModel> GetPicturesFromPath(string path)
        {
            List<PictureItemViewModel> pictures = new List<PictureItemViewModel>();

            string serverPath = HttpContext.Current.Server.MapPath(path);
            if (Directory.Exists(serverPath))
            {
                string[] fileEntries = Directory.GetFiles(serverPath);
                foreach (string fileName in fileEntries)
                {
                    PictureItemViewModel pictureItem = new PictureItemViewModel();
                    string filePath = fileName.Replace("\\", "/");
                    filePath = filePath.Substring(filePath.IndexOf("/content"));
                    pictureItem.Picture_Url = filePath;
                    pictureItem.Picture_Alt = "";
                    pictures.Add(pictureItem);
                }
            }
            return pictures;
        }

        public static IEnumerable<PictureItemViewModel> GetPictures(int sysItemTypeID, long itemID, string culture, bool isMain = false)
        {
            List<PictureItemViewModel> pictures = new List<PictureItemViewModel>();
            ePlatEntities db = new ePlatEntities();
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var objPictures = from p in db.tblPictures_SysItemTypes
                              where p.sysItemTypeID == sysItemTypeID
                              && p.terminalID == terminalID
                              && p.itemID == itemID
                              orderby p.order_
                              select new
                              {
                                  p.tblPictures.path,
                                  p.tblPictures.file_,
                                  p.tblPictures.tblPictureDescriptions.FirstOrDefault(desc => desc.culture == culture).description,
                                  p.main,
                                  p.tblPictures.tblPictureDescriptions.FirstOrDefault(desc => desc.culture == culture).alt,
                              };

            if (objPictures.Count() == 0)
            {
                objPictures = from p in db.tblPictures_SysItemTypes
                              where p.sysItemTypeID == sysItemTypeID
                              && p.itemID == itemID
                              orderby p.order_
                              select new
                              {
                                  p.tblPictures.path,
                                  p.tblPictures.file_,
                                  p.tblPictures.tblPictureDescriptions.FirstOrDefault(desc => desc.culture == culture).description,
                                  p.main,
                                  p.tblPictures.tblPictureDescriptions.FirstOrDefault(desc => desc.culture == culture).alt,
                              };
            }

            if (isMain)
            {
                objPictures = objPictures.Where(x => x.main == true);
            }

            foreach (var picture in objPictures)
            {
                pictures.Add(new PictureItemViewModel()
                {
                    Picture_Url = picture.path + picture.file_,
                    Picture_Description = picture.description,
                    Picture_Alt = picture.alt
                });
            }
            return pictures;
        }

        public static tblSeoItems GetSeo(int sysitemtype, long? itemID, string culture)
        {
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            ePlatEntities db = new ePlatEntities();
            tblSeoItems seo = new tblSeoItems();

            if (itemID != null)
            {
                seo = (from s in db.tblSeoItems
                       where s.sysItemTypeID == sysitemtype
                       && s.itemID == itemID
                       && s.culture == culture
                       && s.terminalID == terminalID
                       select s).FirstOrDefault();
            }
            else
            {
                seo = (from s in db.tblSeoItems
                       where s.friendlyUrl == "/"
                       && s.culture == culture
                       && s.terminalID == terminalID
                       select s).FirstOrDefault();
            }

            return seo;
        }

        public static string GetUrl(int itemTypeID, long id)
        {
            ePlatEntities db = new ePlatEntities();
            string culture = Utils.GeneralFunctions.GetCulture();
            string url = "";
            var UrlQ = (from u in db.tblSeoItems
                        where u.sysItemTypeID == itemTypeID
                        && u.itemID == id
                        && u.culture == culture
                        select u.friendlyUrl).FirstOrDefault();

            if (UrlQ != null)
            {
                url = UrlQ;
            }

            return url;
        }

        public static DomainSettingsViewModel GetMasterSettings()
        {
            DomainSettingsViewModel domainSettings = null;
            ePlatEntities db = new ePlatEntities();
            try
            {
                string domain = HttpContext.Current.Request.Url.Host;
                var settings = from s in db.tblTerminalDomains
                               where s.domain == domain
                               select s;

                if (settings.Count() > 0)
                {
                    domainSettings = new DomainSettingsViewModel();
                    domainSettings.Scripts_Header = settings.First().scriptsHeader;
                    domainSettings.Scripts_Footer = settings.First().scriptsFooter;
                    domainSettings.Scripts_AfterBody = settings.First().scriptsAfterBody;
                    domainSettings.Template_Footer = settings.First().masterPageFooter;
                    domainSettings.Template_Logo = settings.First().logo;
                    domainSettings.Template_Phone_Alt = settings.First().phoneAlt1;
                    domainSettings.Template_Controls = settings.First().controls.Split(',').ToList();

                    string alternateCampaign = string.Empty;
                    if (HttpContext.Current.Request.Params["ac"] != null)
                    {
                        //arriving from campaign, save in cookie;
                        alternateCampaign = HttpContext.Current.Request.Params["ac"];
                        var cookie = new HttpCookie("dvh_campaign", alternateCampaign);
                        cookie.Expires = DateTime.Today.AddDays(180);
                        HttpContext.Current.Response.AppendCookie(cookie);
                    }
                    else
                    {
                        //revisar si hay valor en coockie
                        var cookie = HttpContext.Current.Request.Cookies["dvh_campaign"];
                        if (cookie != null)
                        {
                            alternateCampaign = cookie.Value;
                        }
                    }

                    if (alternateCampaign != string.Empty)
                    {
                        var acPhone = (from a in db.tblTerminalPhones
                                 where a.terminalID == settings.FirstOrDefault().terminalID
                                 && a.label == alternateCampaign
                                 select new {
                                     a.formatedPhone,
                                     a.fromDate,
                                     a.toDate
                                 }).FirstOrDefault();

                        if (acPhone != null)
                        {
                            if(acPhone.toDate == null || (acPhone.toDate != null && acPhone.toDate > DateTime.Now))
                            {
                                domainSettings.Template_Phone_Desktop = acPhone.formatedPhone;
                                domainSettings.Template_Phone_Mobile = acPhone.formatedPhone;
                            } else
                            {
                                acPhone = (from a in db.tblTerminalPhones
                                               where a.terminalID == settings.FirstOrDefault().terminalID
                                               && a.main == true
                                               select new
                                               {
                                                   a.formatedPhone,
                                                   a.fromDate,
                                                   a.toDate
                                               }).FirstOrDefault();

                                if(acPhone != null)
                                {
                                    domainSettings.Template_Phone_Desktop = acPhone.formatedPhone;
                                    domainSettings.Template_Phone_Mobile = acPhone.formatedPhone;
                                }
                            }                            
                        }
                        //switch (alternateCampaign)
                        //{
                        //    case "RIV":
                        //        domainSettings.Template_Phone_Desktop = "1 800 935 2153";
                        //        domainSettings.Template_Phone_Mobile = "1 800 935 2153";
                        //        break;
                        //    case "GDN":
                        //        domainSettings.Template_Phone_Desktop = settings.First().phoneAlt2;
                        //        domainSettings.Template_Phone_Mobile = settings.First().phoneAlt2Mobile;
                        //        break;
                        //    case "GSH":
                        //        domainSettings.Template_Phone_Desktop = "1 800 349 8208";
                        //        domainSettings.Template_Phone_Mobile = "1 800 349 8208";
                        //        break;
                        //    case "MVL":
                        //        domainSettings.Template_Phone_Desktop = "1 888 207 1763";
                        //        domainSettings.Template_Phone_Mobile = "1 888 207 1763";
                        //        break;
                        //    case "FBK":
                        //        domainSettings.Template_Phone_Desktop = "1 800 935 2180";
                        //        domainSettings.Template_Phone_Mobile = "1 800 935 2180";
                        //        break;
                        //    case "SUS":
                        //        domainSettings.Template_Phone_Desktop = "1 888 963 7630";
                        //        domainSettings.Template_Phone_Mobile = "1 888 963 7630";
                        //        break;
                        //    case "DFA":
                        //        domainSettings.Template_Phone_Desktop = settings.First().phoneAlt2;
                        //        domainSettings.Template_Phone_Mobile = settings.First().phoneAlt2Mobile;
                        //        break;
                        //    case "FB":
                        //        domainSettings.Template_Phone_Desktop = "1 888 527 9726";
                        //        domainSettings.Template_Phone_Mobile = "1 888 527 9726";
                        //        break;
                        //}
                    }
                    else
                    {
                        if (Utils.GeneralFunctions.GetCulture() == "es-MX")
                        {
                            domainSettings.Template_Phone_Desktop = settings.First().phoneMX;
                            domainSettings.Template_Phone_Mobile = settings.First().phoneMXMobile;
                        }
                        else
                        {
                            domainSettings.Template_Phone_Desktop = settings.First().phoneUS;
                            domainSettings.Template_Phone_Mobile = settings.First().phoneUSMobile;
                        }
                    }

                    domainSettings.Template_Header = settings.First().masterPageHeader;
                    domainSettings.Template_Header = domainSettings.Template_Header.Replace("$PhoneAlt", settings.First().phoneAlt1);

                    if (domainSettings.Template_Phone_Mobile != null)
                    {
                        domainSettings.Template_Header = domainSettings.Template_Header.Replace("\"tel:$Phone\"", "\"tel:+" + domainSettings.Template_Phone_Mobile.Replace(" ", "") + "\"");
                    }
                    else if (domainSettings.Template_Phone_Desktop != null)
                    {
                        domainSettings.Template_Header = domainSettings.Template_Header.Replace("\"tel:$Phone\"", "\"tel:+" + domainSettings.Template_Phone_Desktop.Replace(" ", "") + "\"");
                    }

                    //fallback para myexperience websites
                    if (settings.First().phoneMX != null)
                    {
                        domainSettings.Template_Header = domainSettings.Template_Header.Replace("$PhoneMX", "<span class=\"phone-desktop hidden-xs hidden-sm\">" + settings.First().phoneMX + "</span><span class=\"phone-mobile hidden-md hidden-lg\">" + (settings.First().phoneMXMobile ?? settings.First().phoneMX) + "</span>");
                    }

                    domainSettings.Template_Header = domainSettings.Template_Header.Replace("$Phone", "<span class=\"phone-desktop hidden-xs hidden-sm\">" + domainSettings.Template_Phone_Desktop + "</span><a onclick=\"return gtag_report_conversion('tel:+" + (domainSettings.Template_Phone_Mobile ?? domainSettings.Template_Phone_Desktop).Replace(" ", "-") + "');\" href=\"tel:+" + (domainSettings.Template_Phone_Mobile ?? domainSettings.Template_Phone_Desktop).Replace(" ","-") + "\"><span class=\"phone-mobile hidden-md hidden-lg\">" + (domainSettings.Template_Phone_Mobile ?? domainSettings.Template_Phone_Desktop) + "</a></span>");
                }
                //dominio canónico
                long terminalID = Utils.GeneralFunctions.GetTerminalID();
                var canonical = (from c in db.tblTerminalDomains
                                where c.terminalID == terminalID
                                && c.main == true
                                select c.domain).FirstOrDefault();

                if (canonical != null)
                {
                    domainSettings.CanonicalDomain = canonical;
                }
            }
            catch { }
            return domainSettings;
        }

        public static SubmenusViewModel GetSubmenus()
        {
            ePlatEntities db = new ePlatEntities();
            SubmenusViewModel submenus = new SubmenusViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            long terminalID = Utils.GeneralFunctions.GetTerminalID();
            var ParentCategoriesQ = (from x in db.tblCategories
                                     where x.tblCatalogs.tblCatalogs_Terminals
                                     .FirstOrDefault().terminalID == terminalID
                                     && x.active == true
                                     && x.showOnWebsite == true
                                     && x.parentCategoryID == null
                                     select x.categoryID).Take(3);

            string[] arrSubmenus = new string[3];
            int index = 0;
            //foreach (long parentID in ParentCategoriesQ)
            //{
            //    var CategoriesQ = from c in db.tblCategories
            //                      where c.parentCategoryID == parentID
            //                      && c.active == true
            //                      && c.showOnWebsite == true
            //                      && c.tblCategories_Services.Count(
            //                            x => x.tblServices.tblServiceDescriptions.FirstOrDefault(o => o.culture == culture).active == true
            //                            && x.tblServices.tblProviders.isActive == true
            //                            ) > 0
            //                      orderby c.tblCategoryDescriptions.FirstOrDefault(x => x.culture == culture).category
            //                      select new
            //                      {
            //                          CategoryID = c.categoryID,
            //                          Category = c.category,
            //                          CategoryName = c.tblCategoryDescriptions
            //                          .FirstOrDefault(x => x.culture == culture).category,
            //                          CategoryIcon = c.tblCategoryDescriptions
            //                          .FirstOrDefault(x => x.culture == culture).imgIcon
            //                      };

            //    arrSubmenus[index] = "<ul " + (CategoriesQ.Count() > 15 ? "class=\"small-items\"" : "") + ">";
            //    foreach (var cat in CategoriesQ.OrderBy(x => x.CategoryName))
            //    {
            //        string url = PageDataModel.GetUrl(8, cat.CategoryID);
            //        if (url == "")
            //        {
            //            url = "/activities/category/" + cat.CategoryID;
            //        }
            //        arrSubmenus[index] += "<li><a href=\"" + url + "\">" + (cat.CategoryIcon != null ? "<img src=\"//eplatfront.villagroup.com" + cat.CategoryIcon + "?width=86&height=86&mode=crop\" width=\"43\" height=\"43\" alt=\"" + (cat.CategoryName != null ? cat.CategoryName : cat.Category) + "\" />" : "") + "<span>" + (cat.CategoryName != null ? cat.CategoryName : cat.Category) + "</span>" + "</a></li>";
            //    }
            //    arrSubmenus[index] += "</ul>";

            //    index++;
            //}

            foreach (long parentID in ParentCategoriesQ)
            {
                var CategoriesQ = from c in db.tblCategories
                                  where c.parentCategoryID == parentID
                                  && c.active == true
                                  && c.showOnWebsite == true
                                  && c.tblCategories_Services.Count(
                                        x => x.tblServices.tblServiceDescriptions.FirstOrDefault(o => o.culture == culture).active == true
                                        && x.tblServices.tblProviders.isActive == true
                                        ) > 0
                                  orderby c.tblCategoryDescriptions.FirstOrDefault(x => x.culture == culture).category
                                  select new
                                  {
                                      CategoryID = c.categoryID,
                                      Category = c.category,
                                      CategoryName = c.tblCategoryDescriptions
                                      .FirstOrDefault(x => x.culture == culture).category,
                                      CategoryIcon = c.tblCategoryDescriptions
                                      .FirstOrDefault(x => x.culture == culture).imgIcon
                                  };

                foreach (var cat in CategoriesQ.OrderBy(x => x.CategoryName))
                {
                    string url = PageDataModel.GetUrl(8, cat.CategoryID);
                    if (url == "")
                    {
                        url = "/activities/categoryv2/" + cat.CategoryID;
                    }
                    arrSubmenus[index] += "<div class=\"col s12 m6 l4\"><a class=\"waves-effect waves-light btn-large btn-flat white-text valign-wrapper\" href=\"" + url + "\">" + (cat.CategoryIcon != null ? "<img class=\"img-submenu-button\" width=\"43\" src=\"//eplatfront.villagroup.com" + cat.CategoryIcon + "?width=86&height=86&mode=crop\" alt=\"" + (cat.CategoryName != null ? cat.CategoryName : cat.Category) + "\" />" : "") + "<span style=\"vertical-align:top;\">" + (cat.CategoryName != null ? cat.CategoryName : cat.Category) + "</span>" + "</a></div>";
                }

                index++;
            }

            submenus.Submenu1 = arrSubmenus[0];
            submenus.Submenu2 = arrSubmenus[1];
            submenus.Submenu3 = arrSubmenus[2];

            return submenus;
        }
        //end front
    }

    /// <summary>
    /// Controller extension class that adds controller methods
    /// to render a partial view and return the result as string.
    /// 
    /// Based on http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
    /// </summary>
    public static class ControllerExtension
    {

        /// <summary>
        /// Renders a (partial) view to string.
        /// </summary>
        /// <param name="controller">Controller to extend</param>
        /// <param name="viewName">(Partial) view to render</param>
        /// <returns>Rendered (partial) view as string</returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName)
        {
            return controller.RenderPartialViewToString(viewName, null);
        }

        /// <summary>
        /// Renders a (partial) view to string.
        /// </summary>
        /// <param name="controller">Controller to extend</param>
        /// <param name="viewName">(Partial) view to render</param>
        /// <param name="model">Model</param>
        /// <returns>Rendered (partial) view as string</returns>
        public static string RenderPartialViewToString(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
