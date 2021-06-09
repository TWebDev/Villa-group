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
    public class TerminalViewModel
    {
        public TerminalsSearchModel TerminalsSearchModel { get; set; }

        public List<TerminalsSearchResultsModel> SearchResults { get; set; }

        public TerminalInfoModel TerminalInfoModel { get; set; }

        public CatalogInfoModel CatalogInfoModel { get; set; }

        public DestinationsPerTerminalInfoModel DestinationsPerTerminalInfoModel { get; set; }

        public DomainInfoModel DomainInfoModel { get; set; }
    }

    public class TerminalsSearchModel
    {
        public string Search_Terminals { get; set; }

        [Display(Name = "Terminal")]
        public string Search_Terminal { get; set; }
    }

    public class TerminalsSearchResultsModel
    {
        public int TerminalID { get; set; }
        public string Prefix { get; set; }
        public string Terminal { get; set; }
    }

    public class TerminalInfoModel
    {
        public int TerminalInfo_TerminalID { get; set; }

        [Display(Name = "Prefix")]
        [Required(ErrorMessage = "Prefix is required")]
        public string TerminalInfo_Prefix { get; set; }

        [Display(Name = "Terminal")]
        [Required(ErrorMessage = "Terminal is required")]
        public string TerminalInfo_Terminal { get; set; }

        [Display(Name = "Terminal Source")]
        public bool TerminalInfo_IsNew { get; set; }

        [Display(Name = "Catalogs")]
        public bool TerminalInfo_Catalogs { get; set; }

        [Display(Name = "Domains")]
        public bool TerminalInfo_Domains { get; set; }

        public TerminalInfoModel()
        {
            TerminalInfo_IsNew = false;
        }

        public List<DomainsModel> TerminalInfo_DomainsPerTerminal { get; set; }

        public CatalogInfoModel CatalogInfoModel { get; set; }

        public DestinationsPerTerminalInfoModel DestinationsPerTerminalInfoModel { get; set; }

        public DomainInfoModel DomainInfoModel { get; set; }
    }

    public class CatalogsSearchModel
    {
        public int CatalogsSearch_TerminalID { get; set; }

        public List<KeyValuePair<int, int>> CatalogsSearch_Catalogs { get; set; }
    }

    public class CatalogInfoModel
    {
        public int CatalogInfo_TerminalID { get; set; }

        public int CatalogInfo_CatalogID { get; set; }

        [Required(ErrorMessage = "Catalog is required")]
        [Display(Name = "Catalog")]
        public string CatalogInfo_Catalog { get; set; }

        public CategoryInfoModel CategoryInfoModel { get; set; }

        //public CategoryDescriptionInfoModel CategoryDescriptionInfoModel { get; set; }
    }

    public class DestinationsPerTerminalInfoModel
    {
        public int DestinationsPerTerminal_TerminalID { get; set; }

        [Display(Name = "Destination")]
        public string[] DestinationsPerTerminal_Destination { get; set; }
        public List<SelectListItem> DestinationsPerTerminal_DrpDestinations
        {
            get
            {
                return PlaceDataModel.GetAllDestinations();
            }
        }
    }

    public class CategoryInfoModel
    {
        public int CategoryInfo_CatalogID { get; set; }

        public int CategoryInfo_CategoryID { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public string CategoryInfo_Category { get; set; }

        [Display(Name = "Parent Category")]
        public int CategoryInfo_ParentCategory { get; set; }
        public List<SelectListItem> CategoryInfo_DrpParentCategories
        {
            get
            {
                return TerminalDataModel.TerminalsCatalogs.FillDrpCategoriesByCatalog(0);
            }
        }

        public bool CategoryInfo_IsActive { get; set; }

        [Display(Name = "Show On Website")]
        public bool CategoryInfo_ShowOnWebsite { get; set; }

        public CategoryDescriptionInfoModel CategoryDescriptionInfoModel { get; set; }

        public CategoryInfoModel()
        {
            CategoryInfo_IsActive = true;
            CategoryInfo_ShowOnWebsite = true;
        }
    }

    public class CategoryDescriptionInfoModel
    {
        public int CategoryDescriptionInfo_CategoryID { get; set; }

        public int CategoryDescriptionInfo_CategoryDescriptionID { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Language is required")]
        public string CategoryDescriptionInfo_Culture { get; set; }
        public List<SelectListItem> CategoryDescriptionInfo_DrpCultures
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpCultures();
            }
        }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required")]
        public string CategoryDescriptionInfo_Category { get; set; }

        [Display(Name = "Image Icon")]
        public string CategoryDescriptionInfo_ImgIcon { get; set; }

        [Display(Name = "Image Header")]
        public string CategoryDescriptionInfo_ImgHeader { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string CategoryDescriptionInfo_Description { get; set; }

        //[AllowHtml]
        //[Display(Name = "Policies")]
        //public string CategoryDescriptionInfo_Policies { get; set; }

        [Display(Name = "Show on Website")]
        public bool CategoryDescriptionInfo_ShowOnWebsite { get; set; }

        public CategoryDescriptionInfoModel()
        {
            CategoryDescriptionInfo_ShowOnWebsite = true;
        }
    }

    public class DomainInfoModel
    {
        public int DomainInfo_TerminalID { get; set; }

        public int DomainInfo_DomainID { get; set; }

        [AllowHtml]
        [Display(Name = "Domain")]
        [Required(ErrorMessage = "Domain is required")]
        public string DomainInfo_Domain { get; set; }

        [AllowHtml]
        [Display(Name = "Default Page")]
        [Required(ErrorMessage = "Default Page is required")]
        public string DomainInfo_DefaultPage { get; set; }

        [AllowHtml]
        [Display(Name = "Default Master Page")]
        [Required(ErrorMessage = "Default Master Page is required")]
        public string DomainInfo_DefaultMasterPage { get; set; }

        [AllowHtml]
        [Display(Name = "Master Page Header")]
        public string DomainInfo_MasterPageHeader { get; set; }

        [AllowHtml]
        [Display(Name = "Master Page Footer")]
        public string DomainInfo_MasterPageFooter { get; set; }

        [AllowHtml]
        [Display(Name = "Scripts Header")]
        public string DomainInfo_ScriptsHeader { get; set; }

        [AllowHtml]
        [Display(Name = "Scripts After Body")]
        public string DomainInfo_ScriptsAfterBody { get; set; }

        [AllowHtml]
        [Display(Name = "Scripts Footer")]
        public string DomainInfo_ScriptsFooter { get; set; }

        [AllowHtml]
        [Display(Name = "Logo")]
        [Required(ErrorMessage = "Logo is required")]
        public string DomainInfo_Logo { get; set; }

        [Display(Name = "US Phone")]
        public string DomainInfo_PhoneUS { get; set; }

        [Display(Name = "MX Phone")]
        public string DomainInfo_PhoneMX { get; set; }

        [Display(Name = "US Mobile Phone")]
        public string DomainInfo_PhoneUSMobile { get; set; }

        [Display(Name = "MX Mobile Phone")]
        public string DomainInfo_PhoneMXMobile { get; set; }

        [Display(Name = "Alt1 Phone")]
        public string DomainInfo_PhoneAlt1 { get; set; }

        [Display(Name = "Alt1 Mobile Phone")]
        public string DomainInfo_PhoneAlt1Mobile { get; set; }

        [Display(Name = "Alt2 Phone")]
        public string DomainInfo_PhoneAlt2 { get; set; }

        [Display(Name = "Alt2 Mobile Phone")]
        public string DomainInfo_PhoneAlt2Mobile { get; set; }

        [Display(Name = "Language")]
        public string DomainInfo_Culture { get; set; }
        public List<SelectListItem> DomainInfo_DrpCultures
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpCultures();
            }
        }

        public BannerGroupInfoModel BannerGroupInfoModel { get; set; }
    }

    public class TerminalsSetModel
    {
        public bool IsBlank { get; set; }
        public bool IsExisting { get; set; }
        public int TerminalID { get; set; }
        public string Prefix { get; set; }
        public string Terminal { get; set; }
        public bool Catalogs { get; set; }
        public bool Domains { get; set; }
    }

    public class DomainsModel
    {
        public int TerminalDomainID { get; set; }
        public int TerminalID { get; set; }
        public string Domain { get; set; }
        public string DefaultPage { get; set; }
        public string DefaultMasterPage { get; set; }
        public string HeaderMasterPage { get; set; }
        public string AfterBodyMasterPage { get; set; }
        public string FooterMasterPage { get; set; }
        public string Logo { get; set; }
        public string PhoneUS { get; set; }
        public string PhoneMX { get; set; }
    }

    public class BannerGroupInfoModel
    {
        public int BannerGroupInfo_TerminalDomainID{get;set;}
        
        public long BannerGroupInfo_BannerGroupID { get; set; }

        [Display(Name = "Banner Group")]
        [Required(ErrorMessage = "Banner Group is required")]
        public string BannerGroupInfo_BannerGroup { get; set; }

        [Display(Name = "Width")]
        [Required(ErrorMessage = "Width is required")]
        public int BannerGroupInfo_Width { get; set; }

        [Display(Name = "Height")]
        [Required(ErrorMessage = "Height is required")]
        public int BannerGroupInfo_Height { get; set; }

        public BannerInfoModel BannerInfoModel { get; set; }
    }

    public class BannerItem
    {
        public string Path { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public int Order { get; set; }
    }

    public class BannerGroup
    {
        public long BannerGroupID { get; set; }
        public List<BannerItem> Banners { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class BannerInfoModel
    {
        public long BannerInfo_BannerGroupID { get; set; }

        public long BannerInfo_BannerID { get; set; }

        [Display(Name = "Banner")]
        [Required(ErrorMessage = "Banner is required")]
        public string BannerInfo_BannerName { get; set; }

        [Display(Name = "Path")]
        [Required(ErrorMessage = "Path is required")]
        public string BannerInfo_Path { get; set; }

        [Display(Name = "Url")]
        public string BannerInfo_Url { get; set; }

        public int BannerInfo_Order { get; set; }

        [Display(Name = "Is Permanent")]
        [Required(ErrorMessage = "Is Permanent is required")]
        public bool BannerInfo_Permanent { get; set; }

        [Display(Name = "From")]
        public string BannerInfo_FromDate { get; set; }

        [Display(Name = "To")]
        public string BannerInfo_ToDate { get; set; }

        [Display(Name = "Culture")]
        public string BannerInfo_Culture { get; set; }
        public List<SelectListItem> BannerInfo_DrpCultures
        {
            get
            {
                return TerminalDataModel.TerminalsCatalogs.FillDrpCultures();
            }
        }

        [Display(Name = "Terminal")]
        public long? BannerInfo_TerminalID { get; set; }
        public List<SelectListItem> BannerInfo_DrpTerminals
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpTerminals();
            }
        }

        public BannerInfoModel()
        {
            BannerInfo_Permanent = false;
        }
    }

    public class DependantFields
    {
        public List<DependantField> Fields { get; set; }

        public class DependantField
        {
            public string Field { get; set; }
            public string ParentField { get; set; }
            public string GrandParentField { get; set; }
            public List<FieldValue> Values { get; set; }
        }

        public class FieldValue : SelectListItem{
            public object ParentValue { get; set; }
            public object GrandParentValue { get; set; }
        }
    }
}
