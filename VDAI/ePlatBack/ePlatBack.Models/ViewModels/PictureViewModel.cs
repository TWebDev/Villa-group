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
using Newtonsoft.Json;

namespace ePlatBack.Models.ViewModels
{
    public class PictureViewModel
    {
        public PictureInfoModel PictureInfoModel { get; set; }
    }

    public class PictureInfoModel
    {
        public string PictureInfo_ItemType { get; set; }
        public int PictureInfo_ItemID { get; set; }
    }

    public class PictureDescriptionInfoModel
    {
        public int PictureDescriptionInfo_PictureID { get; set; }

        public int PictureDescriptionInfo_PictureDescriptionID { get; set; }

        [Required(ErrorMessage = "Alt is required")]
        [Display(Name = "Alt")]
        public string PictureDescriptionInfo_Alt { get; set; }

        [Display(Name = "Description")]
        public string PictureDescriptionInfo_Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        [Display(Name = "Language")]
        public string PictureDescriptionInfo_Culture { get; set; }
        public List<SelectListItem> PictureDescriptionInfo_DrpCultures
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpCultures();
            }
        }
    }

    public class PictureModel
    {
        public int PictureID { get; set; }
        public string File { get; set; }
        public string Format { get; set; }
        public string Path { get; set; }
        public int Descriptions { get; set; }
        ////
        public bool Main { get; set; }
        public int Order { get; set; }
    }

    public class PicasaAvatar
    {
        [JsonProperty(PropertyName = "entry")]
        public EntryInfo Entry { get; set; }

        public class EntryInfo
        {
            [JsonProperty(PropertyName = "photo$nickname")]
            public TValue NickName { get; set; }
            [JsonProperty(PropertyName = "gphoto$thumbnail")]
            public TValue Picture { get; set; }
        }

        public class TValue
        {
            [JsonProperty(PropertyName = "$t")]
            public string Value { get; set; }
        }
    }
}
