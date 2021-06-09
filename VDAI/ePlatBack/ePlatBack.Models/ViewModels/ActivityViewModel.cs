using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class ActivityViewModel
    {
        public List<ActivitySearchResultsModel> SearchResults { get; set; }
        public ActivitiesSearchModel ActivitiesSearchModel { get; set; }
        public ActivityInfoModel ActivityInfoModel { get; set; }
        public List<ActivityImportSearchResultsModel> SearchImportResults { get; set; }
        public List<ActivityAccountingAccountsModel> AccountingAccountsResults { get; set; }
        public List<ActivityPointsOfSaleModel> PointsOfSaleResults { get; set; }
        public List<PriceRuleModel> PriceTypeRulesResults { get; set; }
        public List<PriceRuleModel> NewPriceTypeRulesResults { get; set; }
        public List<StockTransactionsResults> StockTransactionsResults { get; set; }
        [ScriptIgnore]
        public List<SysComponentsPrivilegesModel> Privileges
        {
            get
            {
                return AdminDataModel.GetViewPrivileges(10887);//fdsActivitiesManagement
            }
        }
    }

    public class ActivitySearchResultsModel
    {
        public int ActivityID { get; set; }
        public string Provider { get; set; }
        public string Activity { get; set; }
        public string Terminal { get; set; }
        public string Categories { get; set; }
        public string AccountingAccounts { get; set; }
        public bool HasAccountingAccounts { get; set; }
        public bool Published { get; set; }
        public int NDescriptions { get; set; }
        public int NCategories { get; set; }
        public int NPublicCategories { get; set; }
        public int NImages { get; set; }
        public int NSeoItems { get; set; }
        public int NSchedules { get; set; }
        public int NMeetingPoints { get; set; }
        public bool Rules { get; set; }
        public string RulesErrors { get; set; }
        public bool Deleted { get; set; }
        public string DeletedByUser { get; set; }
        public string DateDeleted { get; set; }
    }

    public class ActivitiesSearchModel
    {
        [Display(Name = "Category")]
        public int Search_Category { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> Search_DrpCategories
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpCategories();
            }
        }

        [Display(Name = "Activity ID")]
        public long? Search_ActivityID { get;set;}

        [Display(Name = "Activity")]
        public string Search_Activity { get; set; }

        [Display(Name = "Region")]
        public int Search_Region { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> Search_DrpRegions
        {
            get
            {
                //var list = ActivityDataModel.ActivitiesCatalogs.FillDrpDestinations(null);
                var list = ActivityDataModel.ActivitiesCatalogs.FillDrpRegions();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Provider")]
        public int Search_Provider { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> Search_DrpProviders
        {
            get
            {
                //var list = ActivityDataModel.ActivitiesCatalogs.FillDrpProviders(0);
                var list = ActivityDataModel.ActivitiesCatalogs.FillDrpProvidersByRegion(0);
                list.Insert(0, ListItems.Default("--Select Region / Provider--"));
                return list;
            }
        }

        [Display(Name = "Destination")]
        public long[] Search_Destination { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> Search_DrpDestinations
        {
            get
            {
                var list = ActivityDataModel.ActivitiesCatalogs.FillDrpDestinationsBySelectedTerminals();
                return list;
            }
        }

        public string Search_Terminals { get; set; }
    }

    public class ActivityInfoModel : StockTransactionsInfoModel
    {
        public int? ActivityInfo_ActivityID { get; set; }

        [Display(Name = "Item Type")]
        public int ActivityInfo_ItemType { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityInfo_DrpItemTypes
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpItemTypes();
            }
        }

        [Display(Name = "Terminal")]
        [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
        public int ActivityInfo_OriginalTerminal { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityInfo_DrpTerminals
        {
            get
            {
                //var list = TerminalDataModel.GetCurrentUserTerminals();
                var list = TerminalDataModel.GetActiveTerminalsList();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Required(ErrorMessage = "Activity is required")]
        [Display(Name = "Activity")]
        public string ActivityInfo_Activity { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        [Display(Name = "Destination")]
        public int ActivityInfo_Destination { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityInfo_DrpDestinations
        {
            get
            {
                //var list = new List<SelectListItem>();
                //list.Insert(0, ListItems.Default("--Select Terminal--"));
                return ActivityDataModel.ActivitiesCatalogs.FillDrpDestinationsByTerminal(0);
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "Provider is required")]
        [Display(Name = "Provider")]
        public int ActivityInfo_Provider { get; set; }
        public bool ActivityInfo_HasSpecialExchangeRate { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityInfo_DrpProviders
        {
            get
            {
                var list = ActivityDataModel.ActivitiesCatalogs.FillDrpProvidersByDestination(0,0);
                list.Insert(0, ListItems.Default("--Select Destination / Provider--"));
                return list;
            }
        }

        [Required(ErrorMessage = "Apply Whole Stay is required")]
        [Display(Name = "Apply Whole Stay")]
        public bool ActivityInfo_ApplyWholeStay { get; set; }

        [Required(ErrorMessage = "Length is required")]
        [Display(Name = "Length (Minutes)")]
        public int ActivityInfo_Length { get; set; }

        [Display(Name = "Zone")]
        public int ActivityInfo_Zone { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityInfo_DrpZones
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpZones(0);
            }
        }

        [Required(ErrorMessage = "Transportation Service is required")]
        [Display(Name = "Transportation Service")]
        public bool ActivityInfo_TransportationService { get; set; }

        [Required(ErrorMessage = "Offers Round Trip is required")]
        [Display(Name = "Offers Round Trip")]
        public bool ActivityInfo_OffersRoundTrip { get; set; }

        [Display(Name = "Minimum Age")]
        public string ActivityInfo_MinimumAge { get; set; }

        [Display(Name = "Minimum Height (Mts)")]
        public string ActivityInfo_MinimumHeight { get; set; }

        [Display(Name = "Maximum Weight(Kgs)")]
        public string ActivityInfo_MaximumWeight { get; set; }

        [Display(Name = "Babies Allowed")]
        [Required(ErrorMessage = "Babies Allowed is required")]
        public bool ActivityInfo_BabiesAllowed { get; set; }

        [Display(Name = "Children Allowed")]
        [Required(ErrorMessage = "Children Allowed is required")]
        public bool ActivityInfo_ChildrenAllowed { get; set; }

        [Display(Name = "Adults Allowed")]
        [Required(ErrorMessage = "Adults Allowed is required")]
        public bool ActivityInfo_AdultsAllowed { get; set; }

        [Display(Name = "Pregnants Allowed")]
        [Required(ErrorMessage = "Pregnants Allowed is required")]
        public bool ActivityInfo_PregnantsAllowed { get; set; }

        [Display(Name = "Senior Allowed")]
        [Required(ErrorMessage = "Senior Allowed is required")]
        public bool ActivityInfo_OldiesAllowed { get; set; }

        [Display(Name = "Video")]
        public string ActivityInfo_Video { get; set; }

        [Display(Name = "Video URL")]
        public string ActivityInfo_VideoURL { get; set; }

        [Display(Name = "Exclude For Commission")]
        public bool ActivityInfo_ExcludeForCommission { get; set; }

        [Display(Name = "Exclude For")]
        public int[] ActivityInfo_JobPositions { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityInfo_DrpJobPositions
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpJobPositionsPerTerminalCommissions(0);
            }
        }

        [Display(Name = "Avoid Rounding")]
        public bool ActivityInfo_AvoidRounding { get; set; }

        public List<KeyValuePair<int, string>> ActivityInfo_ListCategories { get; set; }

        public ActivityCategoryInfoModel ActivityCategoryInfoModel { get; set; }

        public ActivityDescriptionInfoModel ActivityDescriptionInfoModel { get; set; }

        public ActivityScheduleInfoModel ActivityScheduleInfoModel { get; set; }

        public ActivityMeetingPointInfoModel ActivityMeetingPointInfoModel { get; set; }

        public ActivityAccountingAccountInfoModel ActivityAccountingAccountInfoModel { get; set; }

        public ActivityAccountingAccountsModel ActivityAccountingAccountsModel { get; set; }

        public ActivityPointsOfSaleModel ActivityPointsOfSaleModel { get; set; }

        //public ActivityProvidersInfoModel ActivityProvidersInfoModel { get; set; }
        //public ActivityProviderInfoModel ActivityProviderInfoModel { get; set; }
        //public ActivityProviderSearchModel ActivityProviderSearchModel { get; set; }
        public ProvidersModel.SearchProvidersModel SearchProvidersModel { get; set; }

        public ProvidersModel.ProviderInfoModel ProviderInfoModel { get; set; }

        public PriceInfoModel PriceInfoModel { get; set; }

        public PictureInfoModel PictureInfoModel { get; set; }

        public PrevActivitySearchModel PrevActivitySearchModel { get; set; }

        public ActivityImportInfoModel ActivityImportInfoModel { get; set; }

        public PriceTypeRulesInfoModel PriceTypeRulesInfoModel { get; set; }

        public PricesEditorViewModel.ParamsPriceEditor PricesEditorModel { get; set; }

        public StockInfoModel StockInfoModel { get; set; }

        public List<ActivityAccountingAccountInfoModel> ListAccountingAccountsPerActivity { get; set; }

        public ActivityInfoModel()
        {
            ActivityInfo_ApplyWholeStay = false;
            ActivityInfo_TransportationService = false;
            ActivityInfo_OffersRoundTrip = false;
            ActivityInfo_BabiesAllowed = false;
            ActivityInfo_ChildrenAllowed = false;
            ActivityInfo_AdultsAllowed = false;
            ActivityInfo_PregnantsAllowed = false;
            ActivityInfo_OldiesAllowed = false;
            ActivityInfo_ExcludeForCommission = false;
        }
    }

    public class PrevActivitySearchModel
    {
        [Display(Name = "Category")]
        //[Range(1, int.MaxValue, ErrorMessage = "Category is required")]
        public int PrevActivitySearch_Category { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PrevActivitySearch_DrpCategories
        {
            get
            {
                var list = ActivityDataModel.ActivitiesCatalogs.PrevFillDrpCategories();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Provider")]
        public int PrevActivitySearch_Provider { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PrevActivitySearch_DrpProviders
        {
            get
            {
                var list = ActivityDataModel.ActivitiesCatalogs.PrevFillDrpProviders();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }


        [Display(Name = "Activity")]
        public string PrevActivitySearch_Activity { get; set; }

        public string PrevSearch_Terminals { get; set; }
    }

    public class ActivityImportInfoModel
    {
        public int ActivityImportInfo_ActivityID { get; set; }

        public int ActivityImportInfo_PrevActivityID { get; set; }

        public string ActivityImportInfo_Descriptions { get; set; }
        public List<SelectListItem> ActivityImportInfo_ListDescriptions { get; set; }

        public string ActivityImportInfo_Prices { get; set; }
        //[ScriptIgnore]
        public List<ActivityImportPricesModel> ActivityImportInfo_ListPrices { get; set; }

        public string ActivityImportInfo_Availability { get; set; }
        //[ScriptIgnore]
        public List<ActivityImportAvailabilityModel> ActivityImportInfo_ListAvailability { get; set; }
    }

    //prices
    public class ActivityImportPricesModel
    {
        public int ActivityImportPrices_PriceID { get; set; }

        public string ActivityImportPrices_Price { get; set; }

        public string ActivityImportPrices_Currency { get; set; }

        public string ActivityImportPrices_Unit { get; set; }
    }

    //availability/meeting points
    public class ActivityImportAvailabilityModel
    {
        public int ActivityImportAvailability_AvailabilityID { get; set; }

        public bool ActivityImportAvailability_Monday { get; set; }

        public bool ActivityImportAvailability_Tuesday { get; set; }

        public bool ActivityImportAvailability_Wednesday { get; set; }

        public bool ActivityImportAvailability_Thursday { get; set; }

        public bool ActivityImportAvailability_Friday { get; set; }

        public bool ActivityImportAvailability_Saturday { get; set; }

        public bool ActivityImportAvailability_Sunday { get; set; }

        public string ActivityImportAvailability_FromTime { get; set; }
        //[ScriptIgnore]
        public List<ActivityImportMeetingPointModel> ActivityImportAvailability_MeetingPoint { get; set; }
    }

    public class ActivityImportMeetingPointModel
    {
        public int ActivityImportMeetingPoint_MeetingPointID { get; set; }

        public string ActivityImportMeetingPoint_Place { get; set; }

        public string ActivityImportMeetingPoint_Time { get; set; }
    }

    public class ActivityImportSearchResultsModel
    {
        public int ActivityID { get; set; }
        public string Activity { get; set; }
    }

    public class ActivityCategoryInfoModel
    {
        public int ActivityCategoryInfo_ActivityID { get; set; }

        [Display(Name = "Terminal")]
        public int ActivityCategoryInfo_Terminal { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityCategoryInfo_DrpTerminals
        {
            get
            {
                var list = TerminalDataModel.GetActiveTerminalsList().OrderBy(m => m.Text).ToList();
                list.Insert(0, ListItems.Default());
                return list;
                //return PackageDataModel.PackagesCatalogs.FillDrpTerminals();
            }
        }

        [Display(Name = "Catalog")]
        public int ActivityCategoryInfo_Catalog { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityCategoryInfo_DrpCatalogs
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpCatalogsPerTerminal(0);
            }
        }

        [Display(Name = "Categories")]
        public int ActivityCategoryInfo_Category { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityCategoryInfo_DrpCategories
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpCategoriesPerCatalog(0);
            }
        }

        public string ActivityCategoryInfo_Categories { get; set; }
        //[ScriptIgnore]
        public List<KeyValuePair<int, string>> ActivityCategoryInfo_ListCategories { get; set; }
    }

    public class ActivityDescriptionInfoModel
    {
        public int ActivityDescriptionInfo_ActivityDescriptionID { get; set; }

        public int ActivityDescriptionInfo_ActivityID { get; set; }

        [Required(ErrorMessage = "Activity is required")]
        [Display(Name = "Activity Name Displayed")]
        public string ActivityDescriptionInfo_Activity { get; set; }

        [AllowHtml]
        [Display(Name = "Short Description")]
        public string ActivityDescriptionInfo_ShortDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Full Description")]
        public string ActivityDescriptionInfo_FullDescription { get; set; }

        [AllowHtml]
        [Display(Name = "Itinerary")]
        public string ActivityDescriptionInfo_Itinerary { get; set; }

        [AllowHtml]
        [Display(Name = "Includes")]
        public string ActivityDescriptionInfo_Includes { get; set; }

        [AllowHtml]
        [Display(Name = "Notes")]
        public string ActivityDescriptionInfo_Notes { get; set; }

        [AllowHtml]
        [Display(Name = "Recommendations")]
        public string ActivityDescriptionInfo_Recommendations { get; set; }

        [AllowHtml]
        [Display(Name = "Restrictions")]
        public string ActivityDescriptionInfo_Policies { get; set; }

        [AllowHtml]
        //[Display(Name = "Cancelation Policies")]
        [Display(Name = "Specific Activity Policies")]
        public string ActivityDescriptionInfo_CancelationPolicies { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        [Display(Name = "Language")]
        public string ActivityDescriptionInfo_Culture { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityDescriptionInfo_DrpCultures
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpCultures();
            }
        }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool ActivityDescriptionInfo_IsActive { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seo Item is required")]
        [Display(Name = "Seo Item")]
        public int ActivityDescriptionInfo_SeoItem { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityDescriptionInfo_DrpSeoItems
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpSeoItems("", 0);
            }
        }

        [Display(Name = "Tag")]
        public string ActivityDescriptionInfo_Tag { get; set; }

        public ActivityDescriptionInfoModel()
        {
            ActivityDescriptionInfo_IsActive = true;
        }
    }

    public class ActivityScheduleInfoModel
    {
        public int ActivityScheduleInfo_ActivityScheduleID { get; set; }

        public int ActivityScheduleInfo_ActivityID { get; set; }

        [Display(Name = "Mon")]
        public bool ActivityScheduleInfo_Monday { get; set; }

        [Display(Name = "Tue")]
        public bool ActivityScheduleInfo_Tuesday { get; set; }

        [Display(Name = "Wed")]
        public bool ActivityScheduleInfo_Wednesday { get; set; }

        [Display(Name = "Thu")]
        public bool ActivityScheduleInfo_Thursday { get; set; }

        [Display(Name = "Fri")]
        public bool ActivityScheduleInfo_Friday { get; set; }

        [Display(Name = "Sat")]
        public bool ActivityScheduleInfo_Saturday { get; set; }

        [Display(Name = "Sun")]
        public bool ActivityScheduleInfo_Sunday { get; set; }

        [Display(Name = "Is Permanent")]
        [Required(ErrorMessage = "Is Permanent is required")]
        public bool ActivityScheduleInfo_IsPermanent { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is required")]
        public string ActivityScheduleInfo_FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is required")]
        public string ActivityScheduleInfo_ToDate { get; set; }

        [Display(Name = "Is Specific Time")]
        [Required(ErrorMessage = "Is Specific Time is required")]
        public bool ActivityScheduleInfo_IsSpecificTime { get; set; }

        [Display(Name = "Time")]
        [Required(ErrorMessage = "From Time is required")]
        public string ActivityScheduleInfo_FromTime { get; set; }

        [Display(Name = "To Time")]
        [Required(ErrorMessage = "To Time is required")]
        public string ActivityScheduleInfo_ToTime { get; set; }

        [Display(Name = "Interval Time (Minutes)")]
        //[Range(1, 60)]
        public int? ActivityScheduleInfo_IntervalTime { get; set; }

        public ActivityScheduleInfoModel()
        {
            ActivityScheduleInfo_IsPermanent = false;
            ActivityScheduleInfo_IsSpecificTime = false;
        }
    }

    public class ActivityMeetingPointInfoModel
    {
        public long ActivityMeetingPointInfo_ActivityMeetingPointID { get; set; }

        public long ActivityMeetingPointInfo_ActivityID { get; set; }

        public string ActivityMeetingPointInfo_Destination { get; set; }

        public string ActivityMeetingPointInfo_DepartureTimes { get; set; }

        [Display(Name = "Departure Time")]
        [Required(ErrorMessage = "Departure Time is required")]
        public string ActivityMeetingPointInfo_DepartureTime { get; set; }

        [Display(Name = "Weekly Schedule")]
        [Range(1, int.MaxValue, ErrorMessage = "Weekly Schedule is required")]
        public long ActivityMeetingPointInfo_WeeklySchedule { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityMeetingPointInfo_DrpWeeklySchedule
        {
            get
            {
                return ActivityDataModel.ActivitiesCatalogs.FillDrpWeeklySchedule(0);
            }
        }

        [Display(Name = "Place")]
        //[Required(ErrorMessage = "Place is required")]
        [RequiredIf(OtherProperty = "ActivityMeetingPointInfo_AtYourHotel", EqualsTo = false, ErrorMessage = "Place is required")]
        public string ActivityMeetingPointInfo_PlaceString { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityMeetingPointInfo_DrpPlaces
        {
            get
            {
                var list = ActivityDataModel.ActivitiesCatalogs.FillDrpPlaces(0);
                return list;
            }
            set { }
        }

        public int ActivityMeetingPointInfo_Place { get; set; }

        [Display(Name = "Notes")]
        public string ActivityMeetingPointInfo_Notes { get; set; }

        [Display(Name = "At Your Hotel")]
        [Required(ErrorMessage = "At Your Hotel is required")]
        public bool ActivityMeetingPointInfo_AtYourHotel { get; set; }

        [Display(Name = "Is Active")]
        [Required(ErrorMessage = "Is Active is required")]
        public bool ActivityMeetingPointInfo_IsActive { get; set; }

        public ActivityMeetingPointInfoModel()
        {
            ActivityMeetingPointInfo_AtYourHotel = false;
            ActivityMeetingPointInfo_IsActive = true;
        }
    }

    public class ActivityAccountingAccountInfoModel
    {
        public long ActivityAccountingAccountInfo_ActivityAccountingAccountID { get; set; }

        public long ActivityAccountingAccountInfo_ActivityID { get; set; }

        [Display(Name = "Accounting Account")]
        public int[] ActivityAccountingAccountInfo_AccountingAccount { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityAccountingAccountInfo_DrpAccountingAccounts
        {
            get
            {
                var list = ActivityDataModel.ActivitiesCatalogs.FillDrpAccountingAccounts();
                return list;
            }
        }
        public string ActivityAccountingAccountInfo_AccountingAccountString { get; set; }

        [Display(Name = "Account Name")]
        [Required(ErrorMessage = "Account Name is required")]
        public string ActivityAccountingAccountInfo_AccountingAccountName { get; set; }

        public string ActivityAccountingAccountInfo_PriceType { get; set; }
        public string ActivityAccountingAccountInfo_AccountType { get; set; }

        [Display(Name = "Point Of Sale")]
        public string[] ActivityAccountingAccountInfo_PointOfSale { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> ActivityAccountingAccountInfo_DrpPointsOfSale
        {
            get
            {
                var list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                return list;
            }
        }
        public string ActivityAccountingAccountInfo_PointOfSaleString { get; set; }
    }

    //public class ActivityProvidersInfoModel
    //{
    //    public int ActivityProviderInfo_ProviderID { get; set; }

    //    [Display(Name = "Comercial Name")]
    //    [Required(ErrorMessage = "Comercial Name is required")]
    //    public string ActivityProviderInfo_ComercialName { get; set; }

    //    [Display(Name = "Legal Entity")]
    //    public string ActivityProviderInfo_TaxesName { get; set; }

    //    [Display(Name = "RFC")]
    //    public string ActivityProviderInfo_RFC { get; set; }

    //    [Display(Name = "Phone 1")]
    //    public string ActivityProviderInfo_Phone1 { get; set; }

    //    [Display(Name = "Ext 1")]
    //    public string ActivityProviderInfo_Ext1 { get; set; }

    //    [Display(Name = "Phone 2")]
    //    public string ActivityProviderInfo_Phone2 { get; set; }

    //    [Display(Name = "Ext 2")]
    //    public string ActivityProviderInfo_Ext2 { get; set; }

    //    [Display(Name = "Contact Name")]
    //    public string ActivityProviderInfo_ContactName { get; set; }

    //    [Display(Name = "Contact Email")]
    //    public string ActivityProviderInfo_ContactEmail { get; set; }

    //    [Display(Name = "Destination")]
    //    public string ActivityProviderInfo_Destination { get; set; }
    //    public List<SelectListItem> ActivityProviderInfo_DrpDestinations
    //    {
    //        get
    //        {
    //            var list = ActivityDataModel.ActivitiesCatalogs.FillDrpDestinations(0);
    //            list.Insert(0, ListItems.Default());
    //            return list;
    //        }
    //    }
    //    public string ActivityProviderInfo_DestinationName { get; set; }

    //    [Display(Name = "Provider Type")]
    //    public string ActivityProviderInfo_ProviderType { get; set; }
    //    public List<SelectListItem> ActivityProviderInfo_DrpProviderTypes
    //    {
    //        get
    //        {
    //            var list = ActivityDataModel.ActivitiesCatalogs.FillDrpProviderTypes();
    //            list.Insert(0, ListItems.Default());
    //            return list;
    //        }
    //    }
    //    public string ActivityProviderInfo_ProviderTypeName { get; set; }

    //    [Display(Name = "For Company")]
    //    public string ActivityProviderInfo_ForCompany { get; set; }
    //    public string ActivityProviderInfo_ForCompanyName { get; set; }
    //    public List<SelectListItem> ActivityProviderInfo_DrpForCompanies
    //    {
    //        get
    //        {
    //            var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
    //            list.Insert(0, ListItems.Default());
    //            return list;
    //        }
    //    }

    //    [Display(Name = "Is Active")]
    //    [Required(ErrorMessage = "Is Active is required")]
    //    public bool ActivityProviderInfo_IsActive { get; set; }

    //    public ProviderTypeInfoModel ProviderTypeInfoModel { get; set; }
    //}

    //public class ActivityProviderInfoModel : ProvidersModel.ProviderInfoModel
    //{
    //}

    //public class ActivityProviderSearchModel : ProvidersModel.SearchProvidersModel
    //{
    //}

    public class StockTransactionsResults
    {
        public string PurchaseServiceDetail { get; set; }
        public string Quantity { get; set; }
        public string DateSaved { get; set; }
        public string SavedByUser { get; set; }
        public string Ingress { get; set; }
        public string Description { get; set; }
    }

    public class ProviderTypeInfoModel
    {
        public int ProviderTypeInfo_ProviderTypeID { get; set; }

        public string ProviderTypeInfo_ProviderType { get; set; }
    }

    public class StockInfoModel : StockTransactionsInfoModel
    {
        public int StockInfo_StockID { get; set; }

        public long StockInfo_Service { get; set; }

        [Display(Name = "Quantity")]
        public decimal StockInfo_Quantity { get; set; }

        [Display(Name = "Minimal Stock")]
        public decimal StockInfo_MinimalStock { get; set; }

        public StockInfoModel()
        {
            StockTransactionInfo_Ingress = true;
        }
    }

    public class StockTransactionsInfoModel : SearchStockTransactionsModel
    {
        public int StockTransactionInfo_StockTransactionID { get; set; }
        public int StockTransactionInfo_Service { get; set; }
        public int StockTransactionInfo_Stock { get; set; }
        public long StockTransactionInfo_PurchaseServiceDetail { get; set; }
        [Display(Name = "Quantity")]
        public decimal StockTransactionInfo_Quantity { get; set; }
        [Display(Name = "Transaction Type")]
        public bool StockTransactionInfo_Ingress { get; set; }
        [Display(Name = "Description")]
        public string StockTransactionInfo_TransactionDescription { get; set; }
    }

    public class SearchStockTransactionsModel : PriceTypeRulesInfoModel
    {
        public int SearchStockTransactions_Stock { get; set; }
        [Display(Name = "Date")]
        public string SearchStockTransactions_I_Date { get; set; }
        public string SearchStockTransactions_F_Date { get; set; }

        [Display(Name = "Ingress")]
        public bool SearchStockTransactions_Ingress { get; set; }

        [Display(Name = "Egress")]
        public bool SearchStockTransactions_Egress { get; set; }
    }

    //priceTypes Rules
    public class PriceTypeRulesInfoModel
    {
        [Display(Name = "Date")]
        public string SearchRules_Date { get; set; }
        //[ScriptIgnore]
        public List<PriceRuleModel> ListPriceTypeRules { get; set; }

        public long PriceTypeRulesInfo_PriceTypeRuleID { get; set; }

        /// <summary>
        /// >>Rule(s) to be overwritten.
        /// </summary>
        public string PriceTypeRulesInfo_PriceTypeRules { get; set; }

        public long PriceTypeRulesInfo_SelectedService { get; set; }

        [Display(Name = "Terminal")]
        public long PriceTypeRulesInfo_Terminal { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PriceTypeRulesInfo_DrpTerminals
        {
            get
            {
                var list = TerminalDataModel.GetCurrentUserTerminals();
                //list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Provider")]
        public string PriceTypeRulesInfo_Provider { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PriceTypeRulesInfo_DrpProviders
        {
            get
            {
                return new List<SelectListItem>();
                //var list = CatalogsDataModel.Providers.ProvidersCatalogs.FillDrpProvidersPerDestinations(null);
                //list.Insert(0, ListItems.NotSet("All Providers"));
                //return list;
            }
        }

        /// <summary>
        /// >>Services ids selected in form.
        /// </summary>
        [Display(Name = "Service(s)")]
        public string[] PriceTypeRulesInfo_Service { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PriceTypeRulesInfo_DrpServices
        {
            get
            {
                var list = new List<SelectListItem>();
                //list.Insert(0, ListItems.NotSet("All Services"));
                return list;
            }
        }

        [Display(Name = "Generic Unit")]
        public int? PriceTypeRulesInfo_GenericUnit { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PriceTypeRulesInfo_DrpGenericUnits
        {
            get
            {
                var list = PriceDataModel.PricesCatalogs.FillDrpGenericUnits();
                list.Insert(0, ListItems.Default("--All--", ""));
                return list;
            }
        }

        [Display(Name = "Price Type")]
        public int PriceTypeRulesInfo_PriceType { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> PriceTypeRulesInfo_DrpPriceTypes
        {
            get
            {
                var list = PriceDataModel.PricesCatalogs.FillDrpPriceTypes(this.PriceTypeRulesInfo_Terminal);
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Is Base")]
        public bool PriceTypeRulesInfo_Base { get; set; }

        [Display(Name = "Uses Formula")]
        public bool PriceTypeRulesInfo_UsesFormula { get; set; }

        [Display(Name = "Formula")]
        public string PriceTypeRulesInfo_Formula { get; set; }

        [Display(Name = "Percentage")]
        [RequiredIf(OtherProperty = "PriceTypeRulesInfo_Base", EqualsTo = false, ErrorMessage = "Percentage is required")]
        public decimal? PriceTypeRulesInfo_Percentage { get; set; }

        [Display(Name = "> or <")]
        public bool? PriceTypeRulesInfo_MoreOrLess { get; set; }

        [Display(Name = "Than Price Type")]
        public int? PriceTypeRulesInfo_ThanPriceType { get; set; }

        [Display(Name = "Permanent")]
        public bool PriceTypeRulesInfo_Permanent { get; set; }

        [Display(Name = "From Date")]
        //[RequiredIf(OtherProperty = "PriceTypeRulesInfo_PriceType", EqualsTo = "3", ErrorMessage = "From Date is required")]
        public string PriceTypeRulesInfo_FromDate { get; set; }

        [Display(Name = "Termination Date")]
        public string PriceTypeRulesInfo_ToDate { get; set; }

        public PriceTypeRulesInfoModel()
        {
            PriceTypeRulesInfo_Base = true;
            PriceTypeRulesInfo_MoreOrLess = false;
            PriceTypeRulesInfo_UsesFormula = false;
        }
    }


    //related-items
    public class ActivityAccountingAccountsModel : AccountingAccountsModel.AccountingAccountInfoModel
    {
    }

    public class ActivityPointsOfSaleModel : PointsOfSaleModel.PointsOfSaleInfoModel
    {
    }

    //front
    public class CategoryActivitiesViewModel : PageViewModel
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public IEnumerable<ActivityListItem> Activities { get; set; }
        public long ZoneID { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> Zones { get; set; }
        public string RightColumn { get; set; }
    }

    public class ActivityListItem
    {
        public long ActivityID { get; set; }
        public string Activity { get; set; }
        public int? ZoneID { get; set; }
        public string Zone { get; set; }
        public string Url { get; set; }
        //[ScriptIgnore]
        public IEnumerable<PictureListItem> Pictures { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal PromoPrice { get; set; }
        public decimal Rating { get; set; }
        public int Savings { get; set; }
        public int PromoSavings { get; set; }
        public string Tag { get; set; }
        public string TagColor { get; set; }
        public string Category { get; set; }
        public string Currency { get; set; }
        public string Provider { get; set; }
    }

    public class ActivitiesByCategory
    {
        public long CategoryID { get; set; }
        public string Category { get; set; }
        //[ScriptIgnore]
        public List<ActivityListItem> Activities { get; set; }
    }

    public class ActivityReviewsViewModel : PageViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid PurchaseID { get; set; }
        //[ScriptIgnore]
        public List<ServiceReviewItem> ServicesReviews { get; set; }

        public class ServiceReviewItem : ReviewListItem
        {
            public long PurchaseServiceID { get; set; }
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public string Provider { get; set; }
            public DateTime ServiceDateTime { get; set; }
            public string ServicePicture { get; set; }
            public Decimal Rating { get; set; }
            public int NumberOfReviews { get; set; }
        }

        public class BookingExperience
        {
            public Guid PurchaseID { get; set; }
            public int Rating { get; set; }
            public string Review { get; set; }
            public Guid TransactionID { get; set; }
        }
    }

    public class ActivityDetailViewModel : PageViewModel
    {
        public int ItemTypeID { get; set; }
        public long ActivityID { get; set; }
        public string Activity { get; set; }

        public bool IsTransportation { get; set; }
        public decimal? Features_MinimumAge { get; set; }
        public decimal? Features_MinimumHeight { get; set; }
        public decimal? Features_MaximumWeight { get; set; }
        public bool Features_BabiesAllowed { get; set; }
        public bool Features_ChildrenAllowed { get; set; }
        public bool Features_AdultsAllowed { get; set; }
        public bool Features_PregnantsAllowed { get; set; }
        public bool Features_OldiesAllowed { get; set; }
        public string Length { get; set; }
        public string Description { get; set; }
        public string Itinerary { get; set; }
        public string Included { get; set; }
        public string Recommendations { get; set; }
        public string Notes { get; set; }
        public string Restrictions { get; set; }
        //promo, 
        public string Promo_MainTag { get; set; }
        public string Promo_TitleTag { get; set; }
        public string Promo_Description { get; set; }
        public string Promo_Instructions { get; set; }
        public string Promo_TagColor { get; set; }
        public string Promo_TextColor { get; set; }
        //video, 
        public string Video_Url { get; set; }
        //prices
        public IEnumerable<PriceListItem> Prices { get; set; }
        //schedules, 
        public IEnumerable<ScheduleListItem> Schedules { get; set; }
        //meeting points, 
        public IEnumerable<MeetingPointListItem> MeetingPoints { get; set; }
        //alternatives, 
        public IEnumerable<AlternativeListItem> AlternativeActivities { get; set; }
        //pictures, 
        public IEnumerable<PictureListItem> Pictures { get; set; }
        //reviews, rating
        public IEnumerable<ReviewListItem> Reviews { get; set; }
        public decimal Rating { get; set; }
        public ReviewListItem NewReview { get; set; }
    }

    public class PriceListItem
    {
        public long PriceID { get; set; }
        public string Unit { get; set; }
        public string UnitMin { get; set; }
        public string UnitMax { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal OfferPrice { get; set; }
        public string Currency { get; set; }
        public int Savings { get; set; }
        public int PriceTypeID { get; set; }
        public long? ExchangeRateID { get; set; }
        public long? DependingOnPriceID { get; set; }
        public decimal? DependingOnPriceQuantity { get; set; }
        public bool Highlight { get; set; }
    }

    public class ScheduleListItem
    {
        public string Time { get; set; }
        public string Day { get; set; }
    }



    public class MeetingPointListItem
    {
        public string Place { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }

    public class AlternativeListItem
    {
        public string Activity { get; set; }
        public string Url { get; set; }
        public string Picture { get; set; }
    }

    public class PictureListItem
    {
        public long PictureID { get; set; }
        public string Picture { get; set; }
        public string Description { get; set; }
    }

    public class ReviewListItem
    {
        public long ReviewItemID { get; set; }
        public int ReviewItemTypeID { get; set; }
        [Display(Name = "Review", ResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        [Required(ErrorMessageResourceName = "Review_Required",
ErrorMessageResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        public string Review { get; set; }
        [Display(Name = "Rating", ResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        [Required(ErrorMessageResourceName = "Rating_Required",
ErrorMessageResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        //[Range(1, 5, ErrorMessage = "Please rank the activity with the stars")]
        public decimal Rating { get; set; }
        [Display(Name = "Author", ResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        [Required(ErrorMessageResourceName = "Author_Required",
ErrorMessageResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        public string Author { get; set; }
        [Display(Name = "From", ResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        [Required(ErrorMessageResourceName = "From_Required",
ErrorMessageResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        public string From { get; set; }
        [Display(Name = "AuthCode", ResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        [Required(ErrorMessageResourceName = "AuthCode_Required",
ErrorMessageResourceType = typeof(Resources.Models.Controls.SubmitReview))]
        public string AuthCode { get; set; }
        public string Saved { get; set; }
        public string Picture { get; set; }
    }

    public class IndexListItem
    {
        public long ActivityID { get; set; }
        public string Url { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string MeetingPoints { get; set; }
        public string Provider { get; set; }
    }

    public class TransportationQuotes
    {
        public long TransportationServiceID { get; set; }
    }

}