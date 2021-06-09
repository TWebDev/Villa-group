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
    public class BlockInfoViewModel
    {
        public IEnumerable<BlockItem> BlocksList { get; set; }
        public BlockItem BlockItem { get; set; }
        public BlockDescription BlockDescription { get; set; }
    }

    public class BlockItem
    {
        public long BlockItem_BlockID { get; set; }
        [Display(Name = "Terminal")]
        [Required(ErrorMessage = "Terminal is required")]
        public long BlockItem_TerminalID { get; set; }
        public string BlockItem_TerminalName { get; set; }
        public List<SelectListItem> BlockItem_Terminals { get; set; }
        [Display(Name = "Block Name")]
        [Required(ErrorMessage = "Block Name is required")]
        public string BlockItem_Block { get; set; }
        [Display(Name = "General")]
        public bool BlockItem_General { get; set; }
        [Display(Name = "Frame")]
        public string BlockItem_Frame { get; set; }
        public List<BlockDescription> BlockItem_Descriptions { get; set; }
        public BlockDescription BlockItem_Description { get; set; }
    }

    public class BlockDescription
    {
        public long BlockDescription_ID { get; set; }
        public long BlockDescription_BlockID { get; set; }
        [AllowHtml]
        [Display(Name = "Content")]
        public string BlockDescription_Content { get; set; }
        [Display(Name = "Language")]
        [Required(ErrorMessage = "Language is required")]
        public string BlockDescription_Culture { get; set; }
        public string BlockDescription_Language { get; set; }
        public List<SelectListItem> Cultures { get; set; }
    }
}
