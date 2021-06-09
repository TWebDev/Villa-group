using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.ViewModels
{
    //init back
    public class PageInfoViewModel
    {
        public PageSearchModel PageSearchModel { get; set; }
        public PageInfoModel PageInfoModel { get; set; }

        public BlockInfoViewModel BlockInfoViewModel { get; set; }
    }

    public class PageContainer
    {
        public bool Allowed { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Url { get; set; }
    }

    public class NewTreeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public TreeItemState state { get; set; }
        public List<NewTreeItem> children { get; set; }
    }

    public class TreeItemState
    {
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
    }

    public class TreeItem
    {
        public TreeItemAttributes attr { get; set; }
        public string data { get; set; }
        public TreeItemMetaData metadata { get; set; }
        public List<TreeItem> children { get; set; }
    }

    public class TreeItemAttributes
    {
        public string id { get; set; }
    }

    public class TreeItemMetaData
    {
        public string clickable { get; set; }
        public string showInMenu { get; set; }
        public string delete { get; set; }
    }

    public class PageSearchModel
    {
        [Display(Name = "Page")]
        public string Search_Page { get; set; }

        public string Search_Terminals { get; set; }
    }

    public class PageInfoModel
    {
        public int PageInfo_PageID { get; set; }

        [Display(Name = "Terminal")]
        [Required(ErrorMessage = "Terminal is required")]
        public int PageInfo_Terminal { get; set; }
        public List<SelectListItem> PageInfo_DrpTerminals
        {
            get
            {
                var list = TerminalDataModel.GetCurrentUserTerminals();
                list.Insert(0, Utils.ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Page")]
        [Required(ErrorMessage = "Page is required")]
        public string PageInfo_Page { get; set; }

        [Display(Name = "Show In Menu")]
        [Required(ErrorMessage = "Show In Menu is required")]
        public bool PageInfo_ShowInMenu { get; set; }

        [Display(Name = "Order")]
        [Required(ErrorMessage = "Order is required")]
        public int PageInfo_Order { get; set; }
        public List<SelectListItem> PageInfo_DrpOrders
        {
            get
            {
                return PageDataModel.PageCatalogs.FillDrpOrder();
            }
        }

        [Display(Name = "Parent Page")]
        public int PageInfo_ParentPage { get; set; }
        public List<SelectListItem> PageInfo_DrpPages
        {
            get
            {
                return PageDataModel.PageCatalogs.FillDrpParentPages();
            }
        }

        [Display(Name = "Page Type")]
        [Required(ErrorMessage = "Page Type is required")]
        public int PageInfo_PageType { get; set; }
        public List<SelectListItem> PageInfo_DrpPageTypes
        {
            get
            {
                return PageDataModel.PageCatalogs.FillDrpPageTypes();
            }
        }

        [Display(Name = "Is Clickable")]
        [Required(ErrorMessage = "Is Clickable is required")]
        public bool PageInfo_Clickable { get; set; }

        public PageDescriptionModel PageDescriptionModel { get; set; }

        public SeoItemInfoModel SeoItemInfoModel { get; set; }

        public PageInfoModel()
        {
            PageInfo_ShowInMenu = true;
            PageInfo_Clickable = true;
        }
    }

    public class PageDescriptionModel
    {
        public int PageDescription_PageDescriptionID { get; set; }

        public int PageDescription_PageID { get; set; }

        [Display(Name = "Language")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        public string PageDescription_Culture { get; set; }
        public List<SelectListItem> PageDescription_DrpCultures
        {
            get
            {
                return PageDataModel.PageCatalogs.FillDrpCultures();
            }
        }

        [AllowHtml]
        [Display(Name = "Header")]
        public string PageDescription_Header { get; set; }

        [AllowHtml]
        [Display(Name = "Header Content")]
        public string PageDescription_ContentHeader { get; set; }

        [AllowHtml]
        [Display(Name = "Content")]
        [Required(ErrorMessage = "Content is required")]
        public string PageDescription_Content { get; set; }

        [AllowHtml]
        [Display(Name = "After Body")]
        public string PageDescription_AfterBody { get; set; }

        [AllowHtml]
        [Display(Name = "Footer")]
        public string PageDescription_Footer { get; set; }

        [Display(Name = "Page Structure")]
        [Range(1, int.MaxValue, ErrorMessage = "Page Structure is required")]
        public int PageDescription_PageStructure { get; set; }
        public List<SelectListItem> PageDescription_DrpPageStructures
        {
            get
            {
                return PageDataModel.PageCatalogs.FillDrpPageStructures();
            }
        }

        [Display(Name = "Is Active")]
        [Required(ErrorMessage = "Is Active is required")]
        public bool PageDescription_IsActive { get; set; }

        [Display(Name = "Url")]
        public string PageDescription_Url { get; set; }

        public PageDescriptionModel()
        {
            PageDescription_IsActive = true;
        }
    }

    public class BrowseImageModel
    {
        public IEnumerable<string> ImagePaths { get; set; }

        public string CKEditorFuncNum { get; set; }
    }

    //end back

    //init front
    public class PageViewModel
    {
        public string Culture { get; set; }
        public string Scripts_Header { get; set; }
        public string Scripts_AfterBody { get; set; }
        public string Scripts_Footer { get; set; }
        public string Seo_Title { get; set; }
        public string Seo_Keywords { get; set; }
        public string Seo_Description { get; set; }
        public string Seo_Index { get; set; }
        public string Seo_Follow { get; set; }
        public string Seo_FriendlyUrl { get; set; }
        public string Seo_OppositeLanguageUrl { get; set; }
        public string CanonicalDomain { get; set; }
        public string Template_Header { get; set; }
        public string Template_Footer { get; set; }
        public string Template_Logo { get; set; }
        public string Template_Phone_Desktop { get; set; }
        public string Template_Phone_Mobile { get; set; }
        public List<string> Template_Controls { get; set; }
        public string Content_Header { get; set; }
        public string Content { get; set; }
        public string Content2 { get; set; }
        /*Html*/
        public string Submenu1 { get; set; }
        public string Submenu2 { get; set; }
        public string Submenu3 { get; set; }
        /*Controls*/
        public PurchaseProcessViewModel PurchaseProcess
        {
            get
            {
                return new PurchaseProcessViewModel();
            }
        }

        public FreeVacationViewModel FreeVacationControl
        {
            get
            {
                return new FreeVacationViewModel();
            }
        }

        public FreeGetawayViewModel FreeGetawayControl
        {
            get
            {
                FreeGetawayViewModel vm = new FreeGetawayViewModel();
                vm.FreeGetaway_ValidationString = Guid.NewGuid().ToString().Substring(0, 4);
                return vm;
            }
        }

        public RedeemMyPackageViewModel RedeemMyPackageControl
        {
            get
            {
                return new RedeemMyPackageViewModel();
            }
        }

        public QuoteRequestViewModel QuoteRequestControl
        {
            get
            {
                return new QuoteRequestViewModel();
            }
        }
        public SpecialPromoForCityViewModel SpecialPromoControl
        {
            get
            {
                return new SpecialPromoForCityViewModel();
            }
        }
        public CategoriesViewModel CategoriesViewModel
        {
            get
            {
                return new CategoriesViewModel();
            }
        }
    }

    public class PictureItemViewModel
    {
        public string Picture_Url { get; set; }
        public string Picture_Description { get; set; }
        public string Picture_Alt { get; set; }
    }

    public class DomainSettingsViewModel
    {
        public string Scripts_Header { get; set; }
        public string Scripts_AfterBody { get; set; }
        public string Scripts_Footer { get; set; }
        public string Template_Header { get; set; }
        public string Template_Footer { get; set; }
        public string Template_Logo { get; set; }
        public string Template_Phone_Desktop { get; set; }
        public string Template_Phone_Mobile { get; set; }
        public string Template_Phone_Alt { get; set; }
        public List<string> Template_Controls { get; set; }
        public string CanonicalDomain { get; set; }
    }

    public class SubmenusViewModel
    {
        public string Submenu1 { get; set; }
        public string Submenu2 { get; set; }
        public string Submenu3 { get; set; }
    }

    //end front
}
