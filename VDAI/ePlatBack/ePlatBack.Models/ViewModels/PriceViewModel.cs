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
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.ViewModels
{
    public class PriceViewModel
    {
        public PriceInfoModel PriceInfoModel { get; set; }

        public PriceClasificationsSearchModel PriceClasificationsSearchModel { get; set; }

        public PriceClasificationInfoModel PriceClasificationInfoModel { get; set; }

        public CurrenciesSearchModel CurrenciesSearchModel { get; set; }

        public CurrencyInfoModel CurrencyInfoModel { get; set; }

        //public List<PriceInfoModel> SearchResultsModel { get; set; }

        public PricesTableModel PricesTableModel { get; set; }
    }

    public class PricesTableModel
    {
        public List<PriceType> PriceTypes { get; set; }
        public List<PriceRowModel> Prices { get; set; }
        public string Terminal { get; set; }
        public List<PriceInfoModel> SearchResultsModel { get; set; }
    }

    public class PriceRowModel
    {
        public long PriceID { get; set; }
        public string Vigency { get; set; }
        public string TWVigency { get; set; }
        public string Unit { get; set; }
        //public bool Base { get; set; }
        //public bool Active { get; set; }
        public List<PricesModel.PriceDetail> PricesPerType { get; set; }
    }

    //[CustomValidation(typeof(PriceDataModel.PricesCatalogs), "DateGreaterThan")]
    public class PriceInfoModel : PriceUnitInfoModel
    {
        public int PriceInfo_PriceID { get; set; }

        public string PriceInfo_PriceToReplace { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price Type is required")]
        [Display(Name = "Price Type")]
        public int PriceInfo_PriceType { get; set; }
        public List<SelectListItem> PriceInfo_DrpPriceTypes
        {
            get
            {
                var list = PriceDataModel.PricesCatalogs.FillDrpPriceTypes();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        public string PriceInfo_ItemType { get; set; }

        public int PriceInfo_ItemID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price Clasification is required")]
        [Display(Name = "Price Clasification")]
        public int PriceInfo_PriceClasification { get; set; }
        public List<SelectListItem> PriceInfo_DrpPriceClasifications
        {
            get
            {
                return PriceDataModel.PricesCatalogs.FillDrpPriceClasifications();
            }
        }

        [Required(ErrorMessage = "Price")]
        [Display(Name = "Price")]
        public decimal PriceInfo_Price { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Currency is required")]
        [Display(Name = "Currency")]
        public string PriceInfo_Currency { get; set; }
        public List<SelectListItem> PriceInfo_DrpCurrencies
        {
            get
            {
                var list = PriceDataModel.PricesCatalogs.FillDrpCurrencies(null);
                list.Insert(0, ListItems.Default("--Select One--", ""));
                return list;
            }
        }

        //property for display search results
        public string PriceInfo_CurrencyCode { get; set; }

        [Required(ErrorMessage = "Is Permanent is required")]
        [Display(Name = "Is Permanent")]
        public bool PriceInfo_IsPermanent { get; set; }

        [Display(Name = "From Date")]
        public string PriceInfo_FromDate { get; set; }

        [Display(Name = "To Date")]
        [RequiredIf(OtherProperty = "PriceInfo_IsPermanent", EqualsTo = false, ErrorMessage = "To Date is required")]
        public string PriceInfo_ToDate { get; set; }

        [Display(Name = "TW From Date")]
        [Required(ErrorMessage = "TW From Date is required")]
        public string PriceInfo_TWFromDate { get; set; }

        [Display(Name = "TW To Date")]
        [RequiredIf(OtherProperty = "PriceInfo_TWPermanent", EqualsTo = false, ErrorMessage = "TW To Date is required")]
        public string PriceInfo_TWToDate { get; set; }

        [Display(Name = "TW Permanent")]
        public bool PriceInfo_TWPermanent { get; set; }

        [Display(Name = "Generic Unit")]
        public string PriceInfo_GenericUnit { get; set; }
        public List<SelectListItem> PriceInfo_DrpGenericUnits
        {
            get
            {
                var list = PriceDataModel.PricesCatalogs.FillDrpGenericUnits();
                list.Insert(0, ListItems.NotSet("--None--"));
                return list;
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
        [Display(Name = "Terminal")]
        public int PriceInfo_Terminal { get; set; }
        public List<SelectListItem> PriceInfo_DrpTerminals
        {
            get
            {
                return PackageDataModel.PackagesCatalogs.FillDrpTerminalsPerUser();
            }
        }

        //property for display search results
        public string PriceInfo_TerminalName { get; set; }
        public string PriceInfo_PriceTypeName { get; set; }
        public string PriceInfo_PriceClasificationName { get; set; }

        [Required(ErrorMessage = "Taxes Included is required")]
        [Display(Name = "Taxes Included")]
        public bool PriceInfo_TaxesIncluded { get; set; }

        [Display(Name = "From Transportation Zone")]
        public int PriceInfo_FromTransportationZone { get; set; }
        public List<SelectListItem> PriceInfo_DrpFromTransportationZones
        {
            get
            {
                return PriceDataModel.PricesCatalogs.FillDrpTransportationZones();
            }
        }

        [Display(Name = "To Transportation Zone")]
        public int PriceInfo_ToTransportationZone { get; set; }
        public List<SelectListItem> PriceInfo_DrpToTransportationZones
        {
            get
            {
                return PriceDataModel.PricesCatalogs.FillDrpTransportationZones();
            }
        }

        [Display(Name = "Url Redeem")]
        public string PriceInfo_UrlRedeem { get; set; }

        [Display(Name = "Url Compare")]
        public string PriceInfo_UrlCompare { get; set; }

        public bool? PriceInfo_IsBase { get; set; }

        public bool? PriceInfo_IsActive { get; set; }

        public List<PriceUnitInfoModel> PriceInfo_PriceUnits { get; set; }

        public int[] PriceInfo_PriceTypes { get; set; }

        [Display(Name = "Show Prices For")]
        public string PriceInfo_Culture { get; set; }
        public List<SelectListItem> PriceInfo_DrpCultures
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "es-MX", Text = "Nationals" });
                list.Add(new SelectListItem() { Value = "en-US", Text = "Foreigners" });
                return list;
                //return PriceDataModel.PricesCatalogs.FillDrpCultures();
            }
        }

        [Display(Name = "Use Online")]
        public bool PriceInfo_UseOnline { get; set; }

        [Display(Name= "Use Onsite")]
        public bool PriceInfo_UseOnsite { get; set; }

        [Display(Name = "Point Of Sale")]
        public int? PriceInfo_PointOfSale { get; set; }
        public List<SelectListItem> PriceInfo_DrpPointsOfSale
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
            }
        }

        public PriceInfoModel()
        {
            PriceInfo_IsPermanent = false;
            PriceInfo_TaxesIncluded = true;
            PriceInfo_TWPermanent = false;
            PriceInfo_UseOnline = true;
            PriceInfo_UseOnsite = true;
        }

        public PriceClasificationsSearchModel PriceClasificationsSearchModel { get; set; }

        public PriceClasificationInfoModel PriceClasificationInfoModel { get; set; }

        public CurrenciesSearchModel CurrenciesSearchModel { get; set; }

        public CurrencyInfoModel CurrencyInfoModel { get; set; }
    }

    public class PriceUnitInfoModel
    {
        public int PriceUnitInfo_PriceUnitID { get; set; }

        public int PriceUnitInfo_PriceID { get; set; }

        [Display(Name = "Unit")]
        public string PriceUnitInfo_Unit { get; set; }

        [Display(Name = "Additional Info")]
        public string PriceUnitInfo_AdditionalInfo { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Language is required")]
        public string PriceUnitInfo_Culture { get; set; }
        public List<SelectListItem> PriceUnitInfo_DrpCultures
        {
            get
            {
                var list = new List<SelectListItem>();
                list = PriceDataModel.PricesCatalogs.FillDrpCultures();
                return list;
            }
        }
        public string PriceUnitInfo_Culture_Hdn { get; set; }

        [Display(Name = "Min")]
        public string PriceUnitInfo_Min { get; set; }

        [Display(Name = "Max")]
        public string PriceUnitInfo_Max { get; set; }
    }

    public class PriceClasificationsSearchModel
    {
        [Display(Name = "Price Clasification")]
        public string Search_PriceClasifications { get; set; }
    }

    public class PriceClasificationInfoModel
    {
        public int PriceClasification_PriceClasificationID { get; set; }

        [Required(ErrorMessage = "Price Clasification is required")]
        [Display(Name = "Price Clasification")]
        public string PriceClasification_PriceClasification { get; set; }
    }

    public class CurrenciesSearchModel
    {
        [Display(Name = "Currency")]
        public string Search_Currencies { get; set; }
    }

    public class CurrencyInfoModel
    {
        public int CurrencyInfo_CurrencyID { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [Display(Name = "Currency")]
        public string CurrencyInfo_Currency { get; set; }

        [Required(ErrorMessage = "Currency Code is required")]
        [Display(Name = "Currency Code")]
        public string CurrencyInfo_CurrencyCode { get; set; }
    }

    public class ComputedPriceModel
    {
        public long PriceID { get; set; }
        public long ServiceID { get; set; }
        public int? GenericUnitID { get; set; }
        public long SysItemTypeID { get; set; }
        public int PriceTypeID { get; set; }
        public string PriceType { get; set; }
        public long? PromoID { get; set; }
        public bool IsCost { get; set; }
        public bool IsMinimal { get; set; }
        public decimal Price { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencyCode { get; set; }
        public bool Permanent { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool TwPermanent { get; set; }
        public DateTime? TwFromDate { get; set; }
        public DateTime? TwToDate { get; set; }
        public int TerminalID { get; set; }
        public string Terminal { get; set; }
        public bool TaxesIncluded { get; set; }
        public int? FromTransportationZoneID { get; set; }
        public int? ToTransportationZoneID { get; set; }
        public string ToTransportationZone { get; set; }
        public string Culture { get; set; }
        public string Unit { get; set; }
        public string AdditionalInfo { get; set; }
        public string FullUnit { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string MinMax { get; set; }
        public long? ExchangeRateID { get; set; }
        public bool Base { get; set; }
        public string Rule { get; set; }
        public long? DependingOnPriceID { get; set; }
        public decimal? DependingOnPriceQuantity { get; set; }
        public bool Highlight { get; set; }
        public long? Service_PriceTypeID { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string GenericUnit { get; set; }
        public bool OnSite { get; set; }
        public bool OnLine { get; set; }
        public string Rounding { get; set; }
    }

    public class PriceRuleModel
    {
        public long Service_PriceTypeID { get; set; }
        public long TerminalID { get; set; }
        public int? ProviderID { get; set; }
        public long? ServiceID { get; set; }
        public int? GenericUnitID { get; set; }
        public string RuleFrom { get; set; }
        public int PriceTypeID { get; set; }
        public string PriceType { get; set; }
        public long? PromoID { get; set; }
        public bool IsCost { get; set; }
        public bool IsMinimal { get; set; }
        public string RuleFor { get; set; }
        public bool IsBasePrice { get; set; }
        public string Formula { get; set; }
        public string FormulaText { get; set; }
        public decimal? Percentage { get; set; }
        public bool? MoreOrLess { get; set; }
        public int? ThanPriceTypeID { get; set; }
        public string ThanPriceType { get; set; }
        public string Level { get; set; }
        public DateTime SavedOn { get; set; }
        public string SavedBy { get; set; }
        public int PriceTypeOrder { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsPermanent { get; set; }
        public DateTime FromDate { get; set; }
    }

    public class PriceExchangeRate
    {
        public long RateID { get; set; }
        public decimal Rate { get; set; }
        public string Level { get; set; }
        public int? TypeID { get; set; }
        public string Type { get; set; }
    }

    public class PriceWizardModel
    {
        [Display(Name = "Creation or Edition")]
        public bool PriceWizard_IsNewUnit { get; set; }

        [Display(Name = "Unit to Edit")]
        public long PriceWizard_Unit { get; set; }
        public List<SelectListItem> PriceWizard_Units { get; set; }

        [Display(Name = "Start Booking Date")]
        public string PriceWizard_StartBookingDate { get; set; }

        [Display(Name = "Is Permanent")]
        public bool PriceWizard_IsBookingPermanent { get; set; }

        [Display(Name = "End Booking Date")]
        public string PriceWizard_EndBookingDate { get; set; }

        [Display(Name = "Start Service Date")]
        public string PriceWizard_StartTravelDate { get; set; }

        [Display(Name = "Is Permanent")]
        public bool PriceWizard_IsTravelPermanent { get; set; }

        [Display(Name = "End Service Date")]
        public string PriceWizard_EndTravelDate { get; set; }

        [Display(Name = "Price")]
        public decimal PriceWizard_Price { get; set; }

        [Display(Name = "Currency")]
        public string PriceWizard_Currency { get; set; }

        public List<SelectListItem> PriceWizard_DrpCurrencies { get; set; }

        [Display(Name = "Language")]
        public string PriceWizard_Language { get; set; }
        public List<SelectListItem> PriceWizard_DrpLanguages
        {
            get
            {
                return PriceDataModel.PricesCatalogs.FillDrpCultures();
            }
        }

        [Display(Name = "Min")]
        public string PriceWizard_Min { get;set;}
        [Display(Name = "Max")]
        public string PriceWizard_Max { get; set; }

        [Display(Name = "Additional Info")]
        public string PriceWizard_AdditionalInfo { get; set; }

        [Display(Name = "Unit")]
        public string PriceWizard_PriceUnit { get; set; }

        [Display(Name = "Generic Unit")]
        public string PriceWizard_GenericUnit { get; set; }

        public List<SelectListItem> PriceWizard_DrpGenericUnits
        {
            get
            {
                var list = PriceDataModel.PricesCatalogs.FillDrpGenericUnits();
                list.Insert(0, ListItems.NotSet("--None--"));
                return list;
            }
        }

        public string PriceWizard_ItemType { get; set; }
        public long PriceWizard_Terminal { get; set; }
        public int PriceWizard_PriceType { get; set; }
        public long PriceWizard_ItemID { get; set; }
        public bool PriceWizard_TaxesIncluded { get; set; }
        public int PriceWizard_PriceClasification { get; set; }
        
        [Display(Name = "From Transportation Zone")]
        public int PriceWizard_FromTransportationZoneID { get; set; }
        
        [Display(Name ="To Transportation Zone")]
        public int PriceWizard_ToTransportationZoneID { get; set; }
        public List<SelectListItem> PriceWizard_DrpTransportationZones
        {
            get
            {
                return PriceDataModel.PricesCatalogs.FillDrpTransportationZones();
            }
        }
        public string PriceWizard_URLRedeem { get; set; }
        public string PriceWizard_URLCompare { get; set; }
        public string PriceWizard_PriceUnits { get; set; }

        public PriceWizardModel()
        {
            PriceWizard_IsNewUnit = true;
            PriceWizard_IsBookingPermanent = true;
            PriceWizard_IsTravelPermanent = true;
            PriceWizard_TaxesIncluded = true;
            PriceWizard_URLRedeem = "";
            PriceWizard_URLCompare = "";
            PriceWizard_PriceClasification = 1;
        }
    }

    public class PriceWizardUnit_PriceUnits
    {
        public string PriceWizardUnit_Language { get; set; }
        public string PriceWizardUnit_Unit { get; set; }
        public string PriceWizardUnit_Min { get; set; }
        public string PriceWizardUnit_Max { get; set; }
        public string PriceWizardUnit_AdditionalInfo { get; set; }
    }
}
