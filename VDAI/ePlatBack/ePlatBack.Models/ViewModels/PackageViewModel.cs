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
    public class PackageViewModel
    {
        //public Guid CurrentUser { get; set; }
        //public string SelectedWorkGroupID { get; set; }
        //public string SelectedRoleID { get; set; }

        public List<PackagesSearchResultsModel> SearchResults { get; set; }

        public PackagesSearchModel PackagesSearchModel { get; set; }

        public PackageInfoModel PackageInfoModel { get; set; }

        //public PackageDescriptionModel PackageDescriptionModel { get; set; }

        //public PackageSettingsModel PackageSettingsModel { get; set; }

        //public PriceInfoModel PriceInfoModel { get; set; }
    }

    public class PackagesSearchModel
    {
        [Display(Name = "Package")]
        public string Search_Package { get; set; }
        [Display(Name = "Destination")]
        public int Search_Destination { get; set; }
        public List<SelectListItem> Search_DrpDestinations
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpDestinations();
            }
        }

        public string Search_Terminals { get; set; }
    }
    
    public class PackageInfoModel
    {
        public int? PackageInfo_PackageID { get; set; }

        [Required(ErrorMessage = "Package is required")]
        [Display(Name = "Package")]
        public string PackageInfo_Package { get; set; }

        [Display(Name = "Terminal")]        
        [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
        public int PackageInfo_Terminal { get; set; }
        public List<SelectListItem> PackageInfo_DrpTerminals
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpTerminalsPerUser();
            }
        }

        [Required(ErrorMessage = "Nights is required")]
        [Display(Name = "Nights")]
        public int PackageInfo_Nights { get; set; }

        [Required(ErrorMessage = "Adults is required")]
        [Display(Name = "Adults")]
        public int PackageInfo_Adults { get; set; }

        [Required(ErrorMessage = "Children is required")]
        [Display(Name = "Children")]
        public int PackageInfo_Children { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Active")]
        public bool PackageInfo_IsActive { get; set; }

        [Display(Name = "Relevance")]
        [Range(1, int.MaxValue, ErrorMessage = "Relevance is required")]
        public int PackageInfo_Relevance { get; set; }
        public List<SelectListItem> PackageInfo_DrpRelevances
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpRelevance();
            }
        }

        [Required(ErrorMessage = "Availability is required")]
        [Display(Name = "Availability")]
        public int PackageInfo_Availability { get; set; }

        [Required(ErrorMessage = "Purchased is required")]
        [Display(Name = "Purchased")]
        public int PackageInfo_Purchased { get; set; }

        [Display(Name = "Terms Block")]
        public int PackageInfo_TermsBlock { get; set; }
        public List<SelectListItem> PackageInfo_DrpTermsBlocks
        {
            get
            {
                var list = PackageDataModel.PackagesCatalogs.FillDrpTermsBlock();
                list.Insert(0, Utils.ListItems.Default());
                return list;
            }
        }

        public List<GenericListModel> PackageInfo_Descriptions { get; set; }

        [Display(Name = "Catalog")]
        public string PackageInfo_Catalog { get; set; }
        public List<SelectListItem> PackageInfo_DrpCatalogs
        {
            get
            {
                var list = PackageDataModel.PackagesCatalogs.FillDrpCatalogsPerTerminal();
                list.Insert(0, Utils.ListItems.Default());
                return list;
            }
        }
        //[Required(ErrorMessage = "Categories are required")]
        public string PackageInfo_Categories { get; set; }

        [Display(Name = "Categories")]
        public int PackageInfo_Category { get; set; }
        public List<SelectListItem> PackageInfo_DrpCategories
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpCategories(0);
            }
        }

        [Required(ErrorMessage = "Plan Type is required")]
        [Display(Name = "Plan Type")]
        public int PackageInfo_PlanType { get; set; }
        public List<SelectListItem> PackageInfo_DrpPlanTypes
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpPlanTypes();
            }
        }
        
        public PackageInfoModel()
        {
            PackageInfo_IsActive = true;
        }

        public List<KeyValuePair<int, string>> PackageInfo_ListCategories { get; set; }

        public List<PackageSettingsProvisionalModel> PackageInfo_Settings { get; set; }
        //***//
        public PackageDescriptionInfoModel PackageDescriptionInfoModel { get; set; }

        public PackageSettingsInfoModel PackageSettingsInfoModel { get; set; }

        public PriceInfoModel PriceInfoModel { get; set; }

        public SeoItemInfoModel SeoItemInfoModel { get; set; }

        public PictureInfoModel PictureInfoModel { get; set; }
    }

    public class PackageDescriptionInfoModel
    {
        public int PackageDescriptionInfo_PackageDescriptionID { get; set; }

        public int PackageDescriptionInfo_PackageID { get; set; }

        [Required(ErrorMessage = "Package is required")]
        [Display(Name = "Package")]
        public string PackageDescriptionInfo_Package { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Active")]
        public bool PackageDescriptionInfo_IsActive { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        [Display(Name = "Language")]
        public string PackageDescriptionInfo_Culture { get; set; }
        public List<SelectListItem> PackageDescriptionInfo_DrpCultures
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpCultures();
            }
        }

        [AllowHtml]
        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string PackageDescriptionInfo_Description { get; set; }

        [AllowHtml]
        [Display(Name = "Includes")]
        public string PackageDescriptionInfo_Includes { get; set; }

        public PackageDescriptionInfoModel()
        {
            PackageDescriptionInfo_IsActive = true;
        }
    }

    public class PackageSettingsInfoModel
    {
        public int PackageSettingsInfo_PackageSettingsID { get; set; }
        
        public int PackageSettingsInfo_PackageID { get; set; }

        [Display(Name = "Destination")]
        public int PackageSettingsInfo_Destination { get; set; }
        public List<SelectListItem> PackageSettingsInfo_DrpDestinations
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpDestinations();
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "Place is required")]
        [Display(Name = "Place")]
        public int PackageSettingsInfo_Place { get; set; }//DrpPlaces
        public List<SelectListItem> PackageSettingsInfo_DrpPlaces
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpPlaces(0);
            }
        }
        
        public string PackageSettingsInfo_PlaceName { get; set; }
        public string PackageSettingsInfo_RoomTypeName { get; set; }
        public string PackageSettingsInfo_PriceName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Room Type is required")]
        [Display(Name = "Room Type")]
        public int PackageSettingsInfo_RoomType { get; set; }//DrpRoomTypes
        public List<SelectListItem> PackageSettingsInfo_DrpRoomTypes
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.GetRoomTypesPerPlace(0);
            }
        }
        
        [Range(1, int.MaxValue, ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        public int PackageSettingsInfo_Price { get; set; }//DrpPrices
        public List<SelectListItem> PackageSettingsInfo_DrpPrices
        {
            get
            {
                return PriceDataModel.PricesCatalogs.FillDrpPrices("",0);
            }
        }
    }

    public class PackagesSearchResultsModel
    {
        public int PackageID { get; set; }
        public string Package { get; set; }
        public int Nights { get; set; }
        public int Descriptions { get; set; }
        public int Settings { get; set; }
        public int Prices { get; set; }
        public int Pictures { get; set; }
        public bool Active { get; set; }
        public string Destinations { get; set; }
        public int Relevance { get; set; }
    }

    public class GenericListModel
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Item2 { get; set; }
    }

    public class CategoriesPerTerminalModel
    {
        public int CatalogID { get; set; }
        public string CatalogName { get; set; }
        public List<GenericListModel> Categories { get; set; }
    }

    public class PackageSettingsProvisionalModel
    {
        public int PackageSettingID { get; set; }
        public int PlaceID { get; set; }
        public string Place { get; set; }
        public int RoomTypeID { get; set; }
        public int PriceID { get; set; }
    }

    public class DestinationPackagesViewModel : PageViewModel
    {
        public string Destination { get; set; }
        public string PageTitle { get; set; }
        public string VideoUrl { get; set; }
        public string VideoTitle { get; set; }
        public IEnumerable<PictureItemViewModel> Pictures { get; set; }
        public IEnumerable<PackageItemViewModel> Packages { get; set; }
    }

    public class PackageItemViewModel
    {
        public long PackageID { get; set; }
        public string Package { get; set; }
        public int RetailPrice { get; set; }
        public int Price { get; set; }
        public decimal Rate { get; set; }
        public string Stay { get; set; }
        public string Guests { get; set; }
        public string PictureUrl { get; set; }
        public string Url { get; set; }
        public string Destination { get; set; }
        public string PlanType { get; set; }
        public string Includes { get; set; }
        public int Relevance { get; set; }
    }

    public class PackageDetailViewModel : PageViewModel
    {
        public long PackageID { get; set; }
        public string Package { get; set; }
        public decimal Rating { get; set; }
        public int Nights { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public string Stay { get; set; }
        public string Guests { get; set; }
        public string PlanType { get; set; }
        public string Currency { get; set; }
        public int PackagePrice { get; set; }
        public int RetailPrice { get; set; }
        public int AdditionalNightPrice { get; set; }
        public ExpirationDate ExpirationDate { get; set; }
        public string VideoUrl { get; set; }
        public string Description { get; set; }
        public string Options { get; set; }
        public string Terms { get; set; }
        public long DestinationID { get; set; }
        public long ResortID { get; set; }
        public string Destination { get; set; }
        public string Country { get; set; }
        public string Resort { get; set; }
        public string Resort_Label { get; set; }
        public string Resort_Description { get; set; }
        public string Resort_AllInclusive { get; set; }
        public string Resort_FAQ { get; set; }
        public string Resort_Amenities { get; set; }
        public string Resort_Location { get; set; }
        public string Resort_Location_Lat { get; set; }
        public string Resort_Location_Lng { get; set; }
        public decimal Resort_Stars { get; set; }
        public IEnumerable<PictureItemViewModel> Resort_Pictures { get; set; }
        public IEnumerable<RoomItemViewModel> Rooms { get; set; }
        public IEnumerable<PictureItemViewModel> Pictures { get; set; }
        public IEnumerable<ReviewItemViewModel> Reviews { get; set; }
        public IEnumerable<PackageItemViewModel> Packages { get; set; }
        public List<string> Guest_Pictures { get; set; }

    }

    public class ExpirationDate
    {
        public DateTime FullDate { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }

    public class RoomItemViewModel
    {
        public string Room { set; get; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public int Price { get; set; }
    }

    public class ReviewItemViewModel
    {
        public string Review { get; set; }
        public string Author { get; set; }
        public string From { get; set; }
        public decimal Rating { get; set; }
        public string Date { get; set; }
        public string Resort { get; set; }
    }
}
