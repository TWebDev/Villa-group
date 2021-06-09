using System;
using System.Web;
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
    public class SeoItemViewModel
    {
        public List<SeoItemInfoModel> SearchResultsModel { get; set; }
    }

    public class SeoItemInfoModel
    {
        public int SeoItemInfo_SeoItemID { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required")]
        public string SeoItemInfo_Title { get; set; }

        [Display(Name = "Keywords")]
        public string SeoItemInfo_Keywords { get; set; }

        [Display(Name = "Description")]
        public string SeoItemInfo_Description { get; set; }

        [Display(Name = "Friendly Url")]
        [Required(ErrorMessage = "Friendly Url is required")]
        public string SeoItemInfo_FriendlyUrl { get; set; }

        [Display(Name = "Url")]
        [Required(ErrorMessage = "Url is required")]
        public string SeoItemInfo_Url { get; set; }

        [Display(Name = "Language")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        public string SeoItemInfo_Culture { get; set; }
        public List<SelectListItem> SeoItemInfo_DrpCultures
        {
            get
            {
                return SeoItemDataModel.SeoItemCatalogs.FillDrpCultures();
            }
        }

        [Display(Name = "Index")]
        [Required(ErrorMessage = "Index is required")]
        public bool SeoItemInfo_Index { get; set; }

        [Display(Name = "Follow")]
        [Required(ErrorMessage = "Follow is required")]
        public bool SeoItemInfo_Follow { get; set; }

        public int SeoItemInfo_Terminal { get; set; }

        [Display(Name = "Terminal")]
        [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
        public int SeoItemInfo_TerminalItem { get; set; }
        public List<SelectListItem> SeoItemInfo_DrpTerminals
        {
            get
            {
                var list = TerminalDataModel.GetCurrentUserTerminals();
                list.Insert(0, Utils.ListItems.Default());
                return list;
            }
        }

        public string SeoItemInfo_TerminalName { get; set; }

        public string SeoItemInfo_ItemType { get; set; }

        public int SeoItemInfo_ItemID { get; set; }
    }
}
