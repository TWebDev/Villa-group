
using System;
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


namespace ePlatBack.Models.DataModels
{
    public class SeoItemDataModel
    {
        public class SeoItemCatalogs
        {
            public static List<SelectListItem> FillDrpTerminals()
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

            public static List<SelectListItem> FillDrpSeoItemsRelated(string itemType, int itemID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var query = from item in db.tblSeoItems
                            where item.tblSysItemTypes.sysItemType == itemType
                            && item.itemID == itemID
                            select new
                            {
                                item.seoItemID,
                                item.title
                            };
                foreach (var i in query.OrderBy(m => m.title))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.seoItemID.ToString(),
                        Text = i.title
                    });
                }
                return list;
            }
        }

        public List<SeoItemInfoModel> GetSeoItems(string itemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SeoItemInfoModel> list = new List<SeoItemInfoModel>();
            try
            {
                var _itemID = int.Parse(itemID);
                var query = from item in db.tblSeoItems
                            where item.tblSysItemTypes.sysItemType == itemType
                            && item.itemID == _itemID
                            select item;
                foreach (var i in query)
                {
                    list.Add(new SeoItemInfoModel()
                    {
                        SeoItemInfo_SeoItemID = int.Parse(i.seoItemID.ToString()),
                        SeoItemInfo_Title = i.title,
                        SeoItemInfo_Keywords = i.keywords,
                        SeoItemInfo_Description = i.description,
                        SeoItemInfo_FriendlyUrl = i.friendlyUrl,
                        SeoItemInfo_Url = i.url,
                        SeoItemInfo_Culture = i.culture,
                        SeoItemInfo_Index = i.index_,
                        SeoItemInfo_Follow = i.follow,
                        SeoItemInfo_TerminalName = i.tblTerminals.terminal
                    });
                }
            }
            catch
            {
            }
            return list;
        }

        public SeoItemInfoModel GetSeoItemPerID(int seoItemID)
        {
            ePlatEntities db = new ePlatEntities();
            SeoItemInfoModel model = new SeoItemInfoModel();
            try
            {
                var query = db.tblSeoItems.Single(m => m.seoItemID == seoItemID);
                model.SeoItemInfo_SeoItemID = seoItemID;
                model.SeoItemInfo_Title = query.title;
                model.SeoItemInfo_Keywords = query.keywords;
                model.SeoItemInfo_Description = query.description;
                model.SeoItemInfo_FriendlyUrl = query.friendlyUrl;
                model.SeoItemInfo_Url = query.url;
                model.SeoItemInfo_Culture = query.culture;
                model.SeoItemInfo_Index = query.index_;
                model.SeoItemInfo_Follow = query.follow;
                model.SeoItemInfo_Terminal = int.Parse(query.terminalID.ToString());
                model.SeoItemInfo_TerminalItem = int.Parse(query.terminalID.ToString());
            }
            catch { }
            return model;
        }

        public AttemptResponse SaveSeoItem(SeoItemInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.SeoItemInfo_SeoItemID != 0)
            {
                try
                {
                    tblSeoItems item = db.tblSeoItems.Single(m => m.seoItemID == model.SeoItemInfo_SeoItemID);
                    item.title = model.SeoItemInfo_Title;
                    item.keywords = model.SeoItemInfo_Keywords;
                    item.description = model.SeoItemInfo_Description;
                    item.friendlyUrl = model.SeoItemInfo_FriendlyUrl;
                    item.url = model.SeoItemInfo_Url;
                    item.culture = model.SeoItemInfo_Culture;
                    item.index_ = model.SeoItemInfo_Index;
                    item.follow = model.SeoItemInfo_Follow;
                    item.terminalID = (model.SeoItemInfo_Terminal != 0 ? model.SeoItemInfo_Terminal : model.SeoItemInfo_TerminalItem);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Seo Item Updated";
                    response.ObjectID = item.seoItemID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Seo Item NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblSeoItems item = new tblSeoItems();
                    item.title = model.SeoItemInfo_Title;
                    item.keywords = model.SeoItemInfo_Keywords;
                    item.description = model.SeoItemInfo_Description;
                    item.friendlyUrl = model.SeoItemInfo_FriendlyUrl;
                    item.url = model.SeoItemInfo_Url;
                    item.culture = model.SeoItemInfo_Culture;
                    item.index_ = model.SeoItemInfo_Index;
                    item.follow = model.SeoItemInfo_Follow;
                    item.terminalID = (model.SeoItemInfo_Terminal != 0 ? model.SeoItemInfo_Terminal : model.SeoItemInfo_TerminalItem);
                    item.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.SeoItemInfo_ItemType).sysItemTypeID;
                    item.itemID = model.SeoItemInfo_ItemID;
                    db.tblSeoItems.AddObject(item);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Seo Item Saved";
                    response.ObjectID = item.seoItemID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Seo Item NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeleteSeoItem(int seoItemID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblSeoItems.Single(m => m.seoItemID == seoItemID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Seo Item Deleted";
                response.ObjectID = seoItemID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Seo Item NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public List<SelectListItem> GetDDLData(string itemType, string itemID, string path = null)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                var _switch = path != null ? path.ToString() : itemType;
                switch (_switch)
                {
                    case "seo-item-related":
                        {
                            var _itemID = int.Parse(itemID);
                            list = SeoItemCatalogs.FillDrpSeoItemsRelated(itemType, _itemID);
                            list.Insert(0, ListItems.Default());
                            break;
                        }
                }
            }
            catch { }
            return list;
        }
    }
}
