using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class CatalogsViewModel
    {
        public string catalogName { get; set; }

        public string catalogTag { get; set; }

        public AccountingAccountsModel.SearchAccountingAccountsModel SearchAccountingAccountsModel { get; set; }

        public AccountingAccountsModel.AccountingAccountInfoModel AccountingAccountInfoModel { get; set; }

        public BudgetsModel.SearchBudgetsModel SearchBudgetsModel { get; set; }

        public BudgetsModel.BudgetInfoModel BudgetInfoModel { get; set; }

        public CommissionsModel.SearchCommissionsModel SearchCommissionsModel { get; set; }

        public CommissionsModel.CommissionsInfoModel CommissionsInfoModel { get; set; }

        public CompaniesModel.SearchCompaniesModel SearchCompaniesModel { get; set; }

        public CompaniesModel.CompanyInfoModel CompaniesInfoModel { get; set; }

        public CouponFoliosModel.SearchCouponFoliosModel SearchCouponFoliosModel { get; set; }

        public CouponFoliosModel.CouponFoliosInfoModel CouponFoliosInfoModel { get; set; }

        public DestinationsModel.SearchDestinationsModel SearchDestinationsModel { get; set; }

        public DestinationsModel.DestinationsInfoModel DestinationsInfoModel { get; set; }

        public ExchangeRatesModel.SearchExchangeRatesModel SearchExchangeRatesModel { get; set; }

        public ExchangeRatesModel.ExchangeRatesInfoModel ExchangeRatesInfoModel { get; set; }

        public LocationsModel.SearchLocationsModel SearchLocationsModel { get; set; }

        public LocationsModel.LocationInfoModel LocationInfoModel { get; set; }

        public OPCSModel.SearchOPCSModel SearchOPCSModel { get; set; }

        public OPCSModel.OPCSInfoModel OPCSInfoModel { get; set; }

        public PlaceClasificationsModel.SearchPlaceClasificationsModel SearchPlaceClasificationsModel { get; set; }

        public PlaceClasificationsModel.PlaceClasificationsInfoModel PlaceClasificationsInfoModel { get; set; }

        public PlaceTypesModel.SearchPlaceTypesModel SearchPlaceTypesModel { get; set; }

        public PlaceTypesModel.PlaceTypesInfoModel PlaceTypesInfoModel { get; set; }

        public PointsOfSaleModel.SearchPointsOfSaleModel SearchPointsOfSaleModel { get; set; }

        public PointsOfSaleModel.PointsOfSaleInfoModel PointsOfSaleInfoModel { get; set; }

        public PromosModel.PromoSearchModel PromoSearchModel { get; set; }

        public PromosModel.PromoInfoModel PromoInfoModel { get; set; }

        public PromotionTeamsModel.SearchPromotionTeamsModel SearchPromotionTeamsModel { get; set; }

        public PromotionTeamsModel.PromotionTeamsInfoModel PromotionTeamsInfoModel { get; set; }

        public ProvidersModel.SearchProvidersModel SearchProvidersModel { get; set; }

        public ProvidersModel.ProviderInfoModel ProviderInfoModel { get; set; }

        public TransportationZonesModel.SearchTransportationZonesModel SearchTransportationZonesModel { get; set; }

        public TransportationZonesModel.TransportationZonesInfoModel TransportationZonesInfoModel { get; set; }

        public ZonesModel.SearchZonesModel SearchZonesModel { get; set; }

        public ZonesModel.ZonesInfoModel ZonesInfoModel { get; set; }
        //SalesParties
        public PartiesSales.SeachParties SeachParties { get; set; }

        public PartiesSales.PartiesInfo PartiesInfo { get; set; }
        //
        [ScriptIgnore]
        public List<SysComponentsPrivilegesModel> Privileges
        {
            get
            {
                return AdminDataModel.GetViewPrivileges(10590);
            }
        }

    }



    public class AccountingAccountsModel
    {
        public class SearchAccountingAccountsModel
        {
            //busqueda
            [Display(Name = "Accounting Account")]
            public string SearchAccountingAccount_AccountingAccount { get; set; }

            [Display(Name = "Account Name")]
            public string SearchAccountingAccount_AccountingAccountName { get; set; }

            [Display(Name = "Company")]
            public string[] SearchAccountingAccount_Companies { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchAccountingAccount_DrpCompanies
            {
                get
                {
                    return CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                }
            }

            [Display(Name = "Account Type")]
            public bool[] SearchAccountingAccount_AccountType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchAccountingAccount_DrpAccountTypes
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "true",
                        Text = "Income"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "false",
                        Text = "Outcome"
                    });
                    return list;
                }
            }

            //resultados
            public List<AccountingAccountInfoModel> ListAccountingAccounts { get; set; }

            public List<AccountingAccountSummary> ListAccountingAccountsSummary { get; set; }
        }

        public class AccountingAccountSummary
        {
            public string AccountingAccountName { get; set; }
            public int IncomeRegisteredAccounts { get; set; }
            public int OutcomeRegisteredAccounts { get; set; }
            public int NumberOfPriceTypes { get; set; }
            public int RelatedServices { get; set; }
        }

        public class AccountingAccountInfoModel
        {
            public int AccountingAccountInfo_AccountingAccountID { get; set; }

            [Display(Name = "Accounting Account")]
            [Required(ErrorMessage = "Accounting Account is required")]
            public string AccountingAccountInfo_Account { get; set; }

            [Display(Name = "Account Name")]
            [Required(ErrorMessage = "Account Name is required")]
            public string AccountingAccountInfo_AccountName { get; set; }

            [Display(Name = "Company")]
            [Range(1, int.MaxValue, ErrorMessage = "Company is required")]
            public int AccountingAccountInfo_Company { get; set; }
            public string AccountingAccountInfo_CompanyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> AccountingAccountInfo_DrpCompanies
            {
                get
                {
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Price Type")]
            public int?[] AccountingAccountInfo_PriceType { get; set; }
            public string AccountingAccountInfo_PriceTypeText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> AccountingAccountInfo_DrpPriceTypes
            {
                get
                {
                    var list = PriceDataModel.PricesCatalogs.FillDrpPriceTypes();
                    //list.Insert(0, ListItems.Default("All",""));
                    return list;
                }
            }

            [Display(Name = "Article USD")]
            public string AccountingAccountInfo_ArticleUSD { get; set; }

            [Display(Name = "Article MXM")]
            public string AccountingAccountInfo_ArticleMXN { get; set; }

            [Display(Name = "Account Type")]
            public bool AccountingAccountInfo_AccountType { get; set; }
            public string AccountingAccountInfo_AccountTypeText { get; set; }

            //public List<SysComponentsPrivilegesModel> Privileges
            //{
            //    get
            //    {
            //        return AdminDataModel.GetViewPrivileges(11296);
            //    }
            //}

            public SearchAccountingAccountsModel SearchAccountingAccountsModel { get; set; }
        }
    }

    public class BudgetsModel
    {
        public class SearchBudgetsModel
        {
            [Display(Name = "Code Letter")]
            public string SearchBudget_LeadCode { get; set; }

            [Display(Name = "Budget Description")]
            public string SearchBudget_LeadQualification { get; set; }

            [Display(Name = "Date")]
            public string SearchBudget_I_Date { get; set; }

            [Display(Name = "To Date")]
            public string SearchBudget_F_Date { get; set; }

            [Display(Name = "Promotion Team")]
            public int[] SearchBudget_PromotionTeam { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchBudget_DrpPromotionTeams
            {
                get
                {
                    return CatalogsDataModel.PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams();
                }
            }

            public List<BudgetInfoModel> ListBudgets { get; set; }
        }

        public class BudgetInfoModel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11297);
                }
            }
            public int BudgetInfo_BudgetID { get; set; }

            [Display(Name = "Code Letter")]
            public string BudgetInfo_LeadCode { get; set; }

            [Display(Name = "Budget Description")]
            public string BudgetInfo_LeadQualification { get; set; }

            [Display(Name = "Budget Amount")]
            public decimal BudgetInfo_Budget { get; set; }

            [Display(Name = "Is Budget Extension")]
            public bool BudgetInfo_BudgetExt { get; set; }

            [Display(Name = "Currency")]
            public int BudgetInfo_Currency { get; set; }
            public string BudgetInfo_CurrencyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> BudgetInfo_DrpCurrencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesIntID();
                }
            }

            [Display(Name = "Start Date")]
            public string BudgetInfo_FromDate { get; set; }

            [Display(Name = "Ending Date")]
            [RequiredIf(OtherProperty = "BudgetInfo_Permanent", EqualsTo = false, ErrorMessage = "Ending Date is required")]
            //[RequiredIf(OtherProperty = null, Property = "BudgetInfo_ToDate", EqualsTo = false, ErrorMessage = "Ending Date is required")]
            public string BudgetInfo_ToDate { get; set; }

            [Display(Name = "Is Permanent")]
            public bool BudgetInfo_Permanent { get; set; }

            [Display(Name = "Applies Per Client")]
            public bool BudgetInfo_PerClient { get; set; }

            [Display(Name = "Applies Per Week")]
            public bool BudgetInfo_PerWeek { get; set; }

            [Display(Name = "Weekday Of Reset")]
            public string BudgetInfo_ResetDayOfWeek { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> BudgetInfo_DrpWeekDays
            {
                get
                {
                    var list = GeneralFunctions.WeekDays.Select(m => new SelectListItem() { Value = m.Key, Text = m.Value }).ToList();
                    return list;
                }
            }

            [Display(Name = "Promotion Team(s)")]
            public int[] BudgetInfo_PromotionTeam { get; set; }
            public string BudgetInfo_PromotionTeams { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> BudgetInfo_DrpPromotionTeams
            {
                get
                {
                    return CatalogsDataModel.PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams();
                }
            }

            public bool BudgetInfo_InUse { get; set; }

            public BudgetInfoModel()
            {
                BudgetInfo_Permanent = false;
                BudgetInfo_PerClient = true;
                BudgetInfo_PerWeek = false;
            }
        }
    }

    public class CommissionsModel
    {
        public class SearchCommissionsModel
        {
            [Display(Name = "Price Type")]
            public int[] CommissionsSearch_PriceTypes { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsSearch_DrpPriceTypes
            {
                get
                {
                    return PriceDataModel.PricesCatalogs.FillDrpPriceTypes();
                }
            }

            [Display(Name = "Commission Percentage")]
            public string CommissionsSearch_CommissionPercentage { get; set; }

            [Display(Name = "Job Position")]
            public int[] CommissionsSearch_JobPositions { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsSearch_DrpJobPositions
            {
                get
                {
                    return CatalogsDataModel.General.FillDrpJobPositions();
                }
            }
            //[ScriptIgnore]
            public List<CommissionsInfoModel> ListCommissions { get; set; }
        }

        public class CommissionsInfoModel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11295);
                }
            }

            public long CommissionsInfo_CommissionID { get; set; }

            [Display(Name = "Commission or Bonus")]
            public bool CommissionsInfo_IsBonus { get; set; }
            public string CommissionsInfo_CommissionType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_CommissionTypes
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = "Regular Commission",
                        Value = "false"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Monthly Bonus Commission",
                        Value = "true"
                    });
                    return list;
                }
            }

            [Display(Name = "Min Volume")]
            public string CommissionsInfo_MinVolume { get; set; }

            [Display(Name = "Max Volume")]
            public string CommissionsInfo_MaxVolume { get; set; }

            [Display(Name = "Currency")]
            public string CommissionsInfo_VolumeCurrencyCode { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpCurrencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                }
            }

            [Display(Name = "Min Profit %")]
            public string CommissionsInfo_MinProfit { get; set; }

            [Display(Name = "Max Profit %")]
            public string CommissionsInfo_MaxProfit { get; set; }

            [Display(Name = "Price Type")]
            //[Range(1, int.MaxValue, ErrorMessage = "Price Type is required")]
            public int[] CommissionsInfo_PriceType { get; set; }
            public string CommissionsInfo_PriceTypeText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpPriceTypes
            {
                get
                {
                    var list = PriceDataModel.PricesCatalogs.FillDrpPriceTypes();
                    //list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Job Position")]
            [Range(1, int.MaxValue, ErrorMessage = "Job Position is required")]
            public int CommissionsInfo_JobPosition { get; set; }
            public string CommissionsInfo_JobPositionText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpJobPositions
            {
                get
                {
                    var list = CatalogsDataModel.General.FillDrpJobPositions();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "WorkGroup")]
            public int CommissionsInfo_SysWorkGroup { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpSysWorkGroups
            {
                get
                {
                    var list = UserDataModel.UserCatalogs.GetUserWorkGroups();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            public long CommissionsInfo_Terminal { get; set; }
            public string CommissionsInfo_TerminalText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpTerminals
            {
                get
                {
                    //var list = TerminalDataModel.GetCurrentUserTerminals();
                    var list = TerminalDataModel.GetActiveTerminalsList();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Commission Percentage")]
            [RequiredIf(OtherProperty = "CommissionsInfo_CommissionAmount", EqualsTo = "0", ErrorMessage = "Specify Percentage or Amount")]
            public decimal CommissionsInfo_CommissionPercentage { get; set; }

            [Display(Name = "Commission Amount")]
            [RequiredIf(OtherProperty = "CommissionsInfo_CommissionPercentage", EqualsTo = "0", ErrorMessage = "Specify Percentage or Amount")]
            public decimal CommissionsInfo_CommissionAmount { get; set; }

            [Display(Name = "Amount Currency")]
            [RequiredIfDistinct(OtherProperty = "CommissionsInfo_CommissionAmount", DistinctOf = "0", ErrorMessage = "Must specify amount's currency")]
            public string CommissionsInfo_CommissionCurrency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpAmountCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }

            [Display(Name = "Point(s) Of Sale")]
            public string[] CommissionsInfo_PointsOfSale { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CommissionsInfo_DrpPointsOfSale
            {
                get
                {
                    return CatalogsDataModel.PointsOfSale.PoinsOfSaleCatalogs.FillDrpPointsOfSale();
                }
            }

            [Display(Name = "Is Override")]
            public bool CommissionsInfo_Override { get; set; }
            public string CommissionsInfo_IsOverride { get; set; }
            public CommissionsInfoModel()
            {
                CommissionsInfo_Override = false;
            }

            [Required(ErrorMessage = "Is Permanent is required")]
            [Display(Name = "Is Permanent")]
            public bool CommissionsInfo_IsPermanent { get; set; }

            [Display(Name = "From Date")]
            public string CommissionsInfo_FromDate { get; set; }

            [Display(Name = "To Date")]
            [RequiredIf(OtherProperty = "CommissionsInfo_IsPermanent", EqualsTo = false, ErrorMessage = "To Date is required")]
            public string CommissionsInfo_ToDate { get; set; }

            [Display(Name = "Apply On Sales")]
            public bool CommissionsInfo_ApplyOnSales { get; set; }

            [Display(Name = "Apply On Deal Price")]
            public bool CommissionsInfo_ApplyOnDealPrice { get; set; }

            [Display(Name = "Apply On Adult Sales")]
            public bool CommissionsInfo_ApplyOnAdultSales { get; set; }

            [Display(Name = "Only if Charged")]
            public bool CommissionsInfo_OnlyIfCharged { get; set; }
        }
    }

    public class CompaniesModel
    {
        public class SearchCompaniesModel
        {
            [Display(Name = "Company")]
            public string SearchCompanies_Company { get; set; }

            [Display(Name = "Country")]
            public string SearchCompanies_Countries { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchCompanies_DrpCompanies
            {
                get
                {
                    return CountryDataModel.GetAllCountries();
                }
            }

            public List<CompanyInfoModel> ListCompanies { get; set; }
        }

        public class CompanyInfoModel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11298);
                }
            }
            public int CompanyInfo_CompanyID { get; set; }

            [Display(Name = "Company")]
            [Required(ErrorMessage = "Company is required")]
            public string CompanyInfo_Company { get; set; }

            [Display(Name = "Short Name")]
            public string CompanyInfo_ShortName { get; set; }

            [Display(Name = "Address")]
            public string CompanyInfo_Address { get; set; }

            [Display(Name = "City")]
            public string CompanyInfo_City { get; set; }

            [Display(Name = "State")]
            public string CompanyInfo_State { get; set; }

            [Display(Name = "Country")]
            [Range(1, int.MaxValue, ErrorMessage = "Country is required")]
            public int CompanyInfo_Country { get; set; }
            public string CompanyInfo_CountryText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CompanyInfo_DrpCountries
            {
                get
                {
                    var list = CountryDataModel.GetAllCountries();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            [Display(Name = "ZipCode")]
            public string CompanyInfo_ZipCode { get; set; }
            [Display(Name = "RFC")]
            public string CompanyInfo_RFC { get; set; }
            [Display(Name = "Company Type")]
            [Range(1, int.MaxValue, ErrorMessage = "CompanyType is required")]
            public int CompanyTypeID { get; set; }
            public string CompanyType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CompanyInfo_DrpCompanyTypes
            {
                get
                {
                    var listType = CompanyTypeDataModel.GetAllCompanyType();
                    listType.Insert(0, ListItems.Default());
                    return listType;
                }
            }
            [Display(Name = "Terminals")]
            [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            public long[] CompanyInfo_TerminalID { get; set; }
            public string[] CompanyInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CompanyInfo_DrpTerminals
            {
                get
                {
                    return TerminalDataModel.GetActiveTerminalsList();
                }
            }
        }
    }

    public class DestinationsModel
    {
        public class SearchDestinationsModel : DestinationsSearchModel
        {
            public List<DestinationsInfoModel> ListDestinations { get; set; }
        }

        public class DestinationsInfoModel : DestinationInfoModel
        {


            public string DestinationInfo_DestinationName { get; set; }

            public DestinationDescriptionsModel DestinationDescriptionsModel { get; set; }
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11299);
                }
            }
        }

        public class DestinationDescriptionsModel
        {
            public int DestinationDescription_DestinationDescriptionID { get; set; }

            public int DestinationDescription_DestinationID { get; set; }

            [AllowHtml]
            [Display(Name = "Description")]
            [Required(ErrorMessage = "Description is required")]
            public string DestinationDescription_Description { get; set; }

            [Display(Name = "Language")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
            public string DestinationDescription_Culture { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> DestinationDescription_DrpCultures
            {
                get
                {
                    var list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpCultures();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            public int DestinationDescription_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> DestinationDescription_DrpTerminals
            {
                get
                {
                    var list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Video URL")]
            public string DestinationDescription_VideoURL { get; set; }

            [Display(Name = "Video Title")]
            public string DestinationDescription_VideoTitle { get; set; }
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11222);
                }
            }
        }
    }

    public class ExchangeRatesModel
    {
        public class ExchangeRateForDate
        {
            public long ExchangeRateID { get; set; }
            public DateTime Date { get; set; }
            public DateTime DateSaved { get; set; }
            public string CurrencyCode { get; set; }
            public decimal ExchangeRate { get; set; }
            public int CurrencyID { get; set; }
        }

        public class SearchExchangeRatesModel
        {
            [Display(Name = "Date")]
            public string SearchExchangeRates_FromDate { get; set; }

            //[Display(Name = "To Date")]
            //public string SearchExchangeRates_F_Date { get; set; }

            [Display(Name = "From Currency")]
            public string[] SearchExchangeRates_FromCurrency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchExchangeRates_DrpCurrencies
            {
                get
                {
                    return PriceDataModel.PricesCatalogs.FillDrpCurrencies(null);
                }
            }

            [Display(Name = "To Currency")]
            public string[] SearchExchangeRates_ToCurrency { get; set; }

            [Display(Name = "Terminal")]
            public string[] SearchExchangeRates_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchExchangeRates_DrpTerminals
            {
                get
                {
                    return TerminalDataModel.GetActiveTerminalsList();
                }
            }

            [Display(Name = "Provider")]
            public string[] SearchExchangeRates_Providers { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchExchangeRates_DrpProviders
            {
                get
                {
                    return CatalogsDataModel.Providers.ProvidersCatalogs.FillDrpProvidersPerDestinations(null);
                }
            }
            public List<ExchangeRatesInfoModel> ListExchangeRates { get; set; }

            public string ExchangeRatesInfo_ProviderTerminal { get; set; }


            //add
            [Display(Name = "Exchange Rate Type")]
            public int[] SearchExchangeRates_ExchangeRateType { get; set; }
            public string[] SearchExchangeRates_ExchangeRateTypeText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRates_DrpExchangeRateTypes
            {
                get
                {
                    return CatalogsDataModel.ExchangeRates.ExchangeRatesCatalogs.FillDrpExchangeRateTypes();
                }
            }

            [Display(Name = "Select Provider")]
            public bool SearchExchangeRates_OptionProvider { get; set; }

        }

        public class ExchangeRatesInfoModel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11300);
                }
            }
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> _privileges
            {
                get
                {
                    //return new CatalogsViewModel().Privileges;
                    return AdminDataModel.GetViewPrivileges(11151);
                }
            }
            public bool ExchangeRatesInfo_IsClonation { get; set; }

            public long ExchangeRatesInfo_RateToBeCloned { get; set; }

            public long ExchangeRatesInfo_ExchangeRateID { get; set; }

            public string ExchangeRatesInfo_RateToReplace { get; set; }

            [Display(Name = "Exchange Rate")]
            [Required(ErrorMessage = "Exchange Rate is required")]
            public string ExchangeRatesInfo_ExchangeRate { get; set; }

            [Display(Name = "Is Permanent")]
            public bool ExchangeRatesInfo_Permanent { get; set; }

            [Display(Name = "From Date")]
            [Required(ErrorMessage = "From Date is required")]
            public string ExchangeRatesInfo_I_Date { get; set; }
            //public string ExchangeRatesInfo_FromDate { get; set; }

            [Display(Name = "To Date")]
            [Required(ErrorMessage = "To Date is required")]
            public string ExchangeRatesInfo_F_Date { get; set; }
            //public string ExchangeRatesInfo_ToDate { get; set; }

            [Display(Name = "From Currency")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "From Currency is required")]
            public string ExchangeRatesInfo_FromCurrency { get; set; }
            public string ExchangeRatesInfo_FromCurrencyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRatesInfo_DrpFromCurrencies
            {
                get
                {
                    //var list = PriceDataModel.PricesCatalogs.FillDrpCurrencies();
                    //list.Insert(0, ListItems.Default("--Select One--", "", false));
                    //return list;
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem() { Value = "1", Text = "1 - US Dollar" });
                    list.Add(new SelectListItem() { Value = "3", Text = "1 - Canadian Dollar" });
                    return list;
                }
            }

            [Display(Name = "To Currency")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "To Currency is required")]
            public string ExchangeRatesInfo_ToCurrency { get; set; }
            public string ExchangeRatesInfo_ToCurrencyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRatesInfo_DrpToCurrencies
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem() { Value = "2", Text = "Mexican Pesos" });
                    return list;
                }
            }

            [Display(Name = "Provider")]
            [RequiredIf(Property = "ExchangeRatesInfo_Provider", ErrorMessage = "Provider is required")]
            public int? ExchangeRatesInfo_Provider { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRatesInfo_DrpProviders
            {
                get
                {
                    var list = new List<SelectListItem>();
                    //var list = CatalogsDataModel.Providers.ProvidersCatalogs.FillDrpProvidersPerDestinations(null);
                    //list.Insert(0, ListItems.Default("--Select Terminal / Provider--", "null"));
                    list.Insert(0, ListItems.Default("--Select Terminal--", "null"));
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            public long ExchangeRatesInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRatesInfo_DrpTerminals
            {
                get
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            public string ExchangeRatesInfo_ProviderTerminal { get; set; }

            [Display(Name = "Exchange Rate Type")]
            public int ExchangeRatesInfo_ExchangeRateType { get; set; }
            public string ExchangeRatesInfo_ExchangeRateTypeText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRatesInfo_DrpExchangeRateTypes
            {
                get
                {
                    return CatalogsDataModel.ExchangeRates.ExchangeRatesCatalogs.FillDrpExchangeRateTypes();
                }
            }

            [Display(Name = "Point(s) Of Sale")]
            public int[] ExchangeRatesInfo_PointsOfSale { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ExchangeRatesInfo_DrpPointsOfSale
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                }
            }

            public ExchangeRatesInfoModel()
            {
                ExchangeRatesInfo_Permanent = true;
                ExchangeRatesInfo_IsClonation = false;
            }

            public string ExchangeRatesInfo_ExchangeRateDateSaved { get; set; }
            public string ExchangeRatesInfo_ExchangeRateUser { get; set; }
            public string ExchangeRatesInfo_ExchangeRateLastModifyDate { get; set; }
            public string ExchangeRatesInfo_ExchangeRateLastModifyUser { get; set; }
        }
    }

    public class LocationsModel
    {
        public class SearchLocationsModel
        {
            [Display(Name = "Location")]
            public string SearchLocation_Location { get; set; }

            public List<LocationInfoModel> ListLocations { get; set; }
        }

        public class LocationInfoModel
        {
            public int LocationInfo_LocationID { get; set; }

            [Display(Name = "Location")]
            [Required(ErrorMessage = "Location is required", AllowEmptyStrings = false)]
            public string LocationInfo_Location { get; set; }

            [Display(Name = "Destination")]
            [Range(1, int.MaxValue, ErrorMessage = "Destination is required")]
            public long LocationInfo_Destination { get; set; }
            public string LocationInfo_DestinationText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> LocationInfo_DrpDestinations
            {
                get
                {
                    var list = PlaceDataModel.GetDestinationsByCurrentTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Location Code")]
            public string LocationInfo_LocationCode { get; set; }

            [Display(Name = "Terminal")]
            [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            public long LocationInfo_Terminal { get; set; }
            public string LocationInfo_TerminalText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> LocationInfo_DrpTerminals
            {
                get
                {
                    var list = TerminalDataModel.GetActiveTerminalsList();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class OPCSModel
    {
        public class SearchOPCSModel
        {
            [Display(Name = "First or Last Name")]
            public string SearchOPCS_OPC { get; set; }

            [Display(Name = "Marketing Company")]
            public string SearchOPCS_Companies { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchOPCS_DrpCompanies
            {
                get
                {
                    return CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                }
            }

            [Display(Name = "Team")]
            public int[] SearchOPCS_PromotionTeam { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchOPCS_DrpPromotionTeams
            {
                get
                {
                    var list = CatalogsDataModel.PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Job Position")]
            public int[] Search_OPCS_JobPosition { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchOPCS_DrpJobPositions
            {
                get
                {
                    var list = CatalogsDataModel.General.FillDrpJobPositions();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Paying Company")]
            public int?[] SearchOPCS_PayingCompany { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchOPCS_DrpPayingCompanies
            {
                get
                {
                    var list = CatalogsDataModel.PayingCompanies.PayingCompaniesCatalogs.FillDrpPayingCompanies();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            //[Display(Name = "Legacy Key")]
            [Display(Name = "SPI Employee ID")]
            public string SearchOPCS_LegacyKey { get; set; }

            [Display(Name = "Avance ID")]
            public string SearchOPCS_AvanceID { get; set; }

            public long[] SearchOPCS_Terminals { get; set; }

            public List<OPCSInfoModel> ListOPCS { get; set; }
        }

        public class OPCTeamInfoModel
        {
            public long OPCPromotionTeamID { get; set; }
            [Display(Name = "Team / Sales Room / Department")]
            [Range(1, int.MaxValue, ErrorMessage = "Team is Required")]
            public int OPCTeamInfo_PromotionTeam { get; set; }
            public string OPCTeamInfo_PromotionTeamText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OPCTeamInfo_DrpPromotionTeams
            {
                get
                {
                    var listPromotionTeams = CatalogsDataModel.PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeams();
                    listPromotionTeams.Insert(0, ListItems.Default());
                    return listPromotionTeams;
                }
            }

            [Display(Name = "Job Position")]
            [Range(1, int.MaxValue, ErrorMessage = "Job Position is Required")]
            public int OPCTeamInfo_JobPosition { get; set; }
            public string OPCTeamInfo_JobPositionText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OPCTeamInfo_DrpJobPositions
            {
                get
                {
                    var listJobPositions = CatalogsDataModel.General.FillDrpJobPositions();
                    listJobPositions.Insert(0, ListItems.Default());
                    return listJobPositions;
                }
            }

            [Display(Name = "Reporting to")]
            public long? OPCTeamInfo_ParentOpc { get; set; }
            public string OPCTeamInfo_ParentOpcText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OPCTeamInfo_DrpParentOpcs
            {
                get
                {
                    var listParentOPCs = ePlatBack.Models.DataModels.OpcDataModel.GetActiveOPCs();
                    listParentOPCs.Insert(0, ListItems.Default());
                    listParentOPCs.Insert(1, new SelectListItem { Text = "None", Value = "-1" });
                    return listParentOPCs;
                }
            }

            [Display(Name = "Start Date")]
            public string OPCTeamInfo_EnlistDate { get; set; }

            [Display(Name = "Termination")]
            public bool OPCTeamInfo_Deleted { get; set; }

            [Display(Name = "Termination Date")]
            public string OPCTeamInfo_TerminateDate { get; set; }

            [Display(Name = "Termination Reason")]
            public string OPCTeamInfo_TerminateReason { get; set; }

            public bool OPCTeamInfo_HasSubs { get; set; }
            public int OPCTeamInfo_Subs { get; set; }

            [Display(Name = "Assign subordinates to:")]
            public long? OPCTeamInfo_AssignToParentOpc { get; set; }
            public string OPCTeamInfo_AssignToParentOpcText { get; set; }
            public string OPCTeamInfo_DateSaved { get; set; }
            public string OPCTeamInfo_TemporalID { get; set; }
        }

        public class OPCHistoryModel
        {
            public string Credential { get; set; }
            public string OPC { get; set; }
            public string Movement { get; set; }
            public DateTime Date { get; set; }
            public string Team { get; set; }
            public string PayingCompany { get; set; }
            public string Company { get; set; }
            public string Reason { get; set; }
        }

        public class OPCSInfoModel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11302);
                }
            }


            public long OPCSInfo_OpcID { get; set; }

            [Display(Name = "OPC")]
            [Required(ErrorMessage = "OPC is required")]
            public string OPCSInfo_Opc { get; set; }

            [Display(Name = "First Name")]
            [Required(ErrorMessage = "First Name is required")]
            public string OPCSInfo_FirstName { get; set; }

            [Display(Name = "Middle Name")]
            public string OPCSInfo_MiddleName { get; set; }

            [Display(Name = "Last Name")]
            [Required(ErrorMessage = "Last Name is required")]
            public string OPCSInfo_LastName { get; set; }

            [Display(Name = "Second Surname")]
            public string OPCSInfo_SecondSurname { get; set; }

            [Display(Name = "Credential Number")]
            [Required(ErrorMessage = "Credential Number is required")]
            public string OPCSInfo_Credential { get; set; }

            [Display(Name = "Main Phone Number")]
            public string OPCSInfo_Phone1 { get; set; }

            [Display(Name = "Alternate Phone Number")]
            public string OPCSInfo_Phone2 { get; set; }

            [Display(Name = "Paying Company")]
            [Required(ErrorMessage = "Paying Company is required")]
            [Range(1, int.MaxValue, ErrorMessage = "Paying Company is Required")]
            public int OPCSInfo_PayingCompany { get; set; }
            public string OPCSInfo_PayingCompanyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OPCSInfo_DrpPayingCompanies
            {
                get
                {
                    var listPayingCompanies = CatalogsDataModel.PayingCompanies.PayingCompaniesCatalogs.FillDrpPayingCompanies();
                    listPayingCompanies.Insert(0, ListItems.Default());
                    return listPayingCompanies;
                }
            }

            [Display(Name = "SPI Employee ID")]
            [Required(ErrorMessage = "SPI Employee ID is required")]
            public string OPCSInfo_LegacyKey { get; set; }

            [Display(Name = "Avance ID")]
            public string OPCSInfo_AvanceID { get; set; }

            [Display(Name = "User")]
            public string OPCSInfo_User { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OPCSInfo_DrpUsers
            {
                get
                {
                    var listUsers = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey);
                    listUsers.Insert(0, ListItems.Default());
                    return listUsers;
                }
            }

            [Display(Name = "Marketing Company")]
            [Range(1, int.MaxValue, ErrorMessage = "Company is Required")]
            public int OPCSInfo_Company { get; set; }
            public string OPCSInfo_CompanyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OPCSInfo_DrpCompanies
            {
                get
                {
                    var listCompanies = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                    listCompanies.Insert(0, ListItems.Default());
                    return listCompanies;
                }
            }

            [Display(Name = "Hired Date")]
            public DateTime? OPCSInfo_EnlistDate { get; set; }

            public List<OPCTeamInfoModel> OPCSInfo_Teams { get; set; }
            [Required(ErrorMessage = "Must have at least 1 Team assigned")]
            public string OPCSInfo_TeamsStr { get; set; }

            [Required(ErrorMessage = "You are editing Team Info. Please click ADD INFO or CANCEL before clicking SAVE.")]
            public string OPCSInfo_TeamInfoEditing { get; set; }

            //public OPCTeamInfoModel OPCSInfo_Team { get; set; }
            public OPCTeamInfoModel OPCSInfo_Team { get { return new OPCTeamInfoModel(); } }

            public string OPCSInfo_PromotionTeamText { get; set; }
            public string OPCSInfo_JobPositionText { get; set; }
            public string OPCSInfo_Destination { get; set; }
            public string OPCSInfo_LastTerminationDate { get; set; }
            public string OPCSInfo_Active { get; set; }
        }
    }

    public class PlaceClasificationsModel
    {
        public class SearchPlaceClasificationsModel : PlaceClasificationsSearchModel
        {
            public List<PlaceClasificationsInfoModel> ListPlaceClasifications { get; set; }
        }

        public class PlaceClasificationsInfoModel : PlaceClasificationInfoModel
        {
            public string PlaceClasificationInfo_PlaceTypeName { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PlaceClasificationInfo_DrpPlaceTypes
            {
                get
                {
                    var list = CatalogsDataModel.PlaceTypes.PlaceTypesCatalogs.FillDrpActivePlaceTypes();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class PlaceTypesModel
    {
        public class SearchPlaceTypesModel : PlaceTypesSearchModel
        {
            public List<PlaceTypesInfoModel> ListPlaceTypes { get; set; }
        }

        public class PlaceTypesInfoModel : PlaceTypeInfoModel
        {
        }
    }

    public class ProvidersModel
    {
        public class SearchProvidersModel
        {
            [Display(Name = "Comercial Name")]
            public string SearchProviders_ComercialName { get; set; }

            [Display(Name = "Destination")]
            public string SearchProviders_Destinations { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchProviders_DrpDestinations
            {
                get
                {
                    return CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();
                }
            }

            public List<ProviderInfoModel> ListProviders { get; set; }
        }

        public class ProviderInfoModel
        {
            public int ProviderInfo_ProviderID { get; set; }

            [Display(Name = "Destination")]
            [Range(1, int.MaxValue, ErrorMessage = "Destination is required")]
            public long ProviderInfo_Destination { get; set; }
            public string ProviderInfo_DestinationText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ProviderInfo_DrpDestinations
            {
                get
                {
                    var list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Comercial Name")]
            [Required(ErrorMessage = "Comercial Name is required")]
            public string ProviderInfo_ComercialName { get; set; }

            [Display(Name = "Short Name")]
            public string ProviderInfo_ShortName { get; set; }

            [Display(Name = "RFC")]
            public string ProviderInfo_RFC { get; set; }

            [Display(Name = "Legal Entity")]
            public string ProviderInfo_TaxesName { get; set; }

            [Display(Name = "Phone 1")]
            public string ProviderInfo_Phone1 { get; set; }

            [Display(Name = "Ext 1")]
            public string ProviderInfo_Ext1 { get; set; }

            [Display(Name = "Phone 2")]
            public string ProviderInfo_Phone2 { get; set; }

            [Display(Name = "Ext 2")]
            public string ProviderInfo_Ext2 { get; set; }

            [Display(Name = "Contact Name")]
            public string ProviderInfo_ContactName { get; set; }

            [Display(Name = "Contact Email")]
            public string ProviderInfo_ContactEmail { get; set; }

            [Display(Name = "Provider Type")]
            [Range(1, int.MaxValue, ErrorMessage = "Provider Type is required")]
            public int ProviderInfo_ProviderType { get; set; }
            public string ProviderInfo_ProviderTypeText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ProviderInfo_DrpProviderTypes
            {
                get
                {
                    var list = CatalogsDataModel.Providers.ProvidersCatalogs.FillDrpProviderTypes();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            public string ProviderInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ProviderInfo_DrpTerminals
            {
                get
                {
                    return TerminalDataModel.GetActiveTerminalsList();
                }
            }

            [Display(Name = "Is Active")]
            [Required(ErrorMessage = "Is Active is required")]
            public bool ProviderInfo_IsActive { get; set; }

            [Display(Name = "For Company")]
            public int ProviderInfo_ForCompany { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ProviderInfo_DrpForCompanies
            {
                get
                {
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Contract Currency")]
            public string ProviderInfo_ContractCurrency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ProviderInfo_DrpCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesIntID();
                    list = list.Where(m => m.Value != "3").ToList();
                    list.Insert(0, ListItems.NotSet("US Dollar & Mexican Peso"));
                    return list;
                }
            }

            [Display(Name = "File To Upload")]
            public string ProviderInfo_FileToUpload { get; set; }

            [Display(Name = "USD Avance Provider ID")]
            public string ProviderInfo_AvanceProvider { get; set; }

            [Display(Name = "MXN Avance Provider ID")]
            public string ProviderInfo_MXNAvanceProvider { get; set; }

            [Display(Name = "Invoice Currency")]
            public string ProviderInfo_InvoiceCurrency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ProviderInfo_DrpInvoiceCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesIntID();
                    list = list.Where(m => m.Value != "3").ToList();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            public SearchProvidersModel SearchProvidersModel { get; set; }

            public ProviderInfoModel()
            {
                ProviderInfo_IsActive = true;
            }
        }
    }

    public class PromotionTeamsModel
    {
        public class SearchPromotionTeamsModel
        {
            [Display(Name = "Promotion Team")]
            public string SearchPromotionTeams_PromotionTeam { get; set; }

            [Display(Name = "Destination")]
            public string[] SearchPromotionTeams_Destinations { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPromotionTeams_DrpDestinations
            {
                get
                {
                    return PlaceDataModel.GetDestinationsByCurrentTerminals();
                }
            }

            [Display(Name = "Company")]
            public string[] SearchPromotionTeams_Companies { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPromotionTeams_DrpCompanies
            {
                get
                {
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                    list = list.Where(m => m.Value != "0").ToList();
                    return list;

                    //return CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                }
            }

            public List<PromotionTeamsInfoModel> ListPromotionTeams { get; set; }
        }

        public class PromotionTeamsInfoModel
        {
            public int PromotionTeamsInfo_PromotionTeamID { get; set; }

            [Display(Name = "Promotion Team")]
            [Required(ErrorMessage = "Promotion Team is required")]
            public string PromotionTeamsInfo_PromotionTeam { get; set; }

            [Display(Name = "Destination")]
            [Range(1, int.MaxValue, ErrorMessage = "Destination is required")]
            public int PromotionTeamsInfo_Destination { get; set; }
            public string PromotionTeamsInfo_DestinationText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromotionTeamsInfo_DrpDestinations
            {
                get
                {
                    var list = PlaceDataModel.GetDestinationsByCurrentTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Company")]
            [Range(1, int.MaxValue, ErrorMessage = "Company is required")]
            public int PromotionTeamsInfo_Company { get; set; }
            public string PromotionTeamsInfo_CompanyText { get; set; }

            [Display(Name = "Gifting Budget")]
            public int? PromotionTeamsInfo_GiftingBudget { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromotionTeamsInfo_DrpCompanies
            {
                get
                {
                    //var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpMarketingCompanies();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class TransportationZonesModel
    {
        public class SearchTransportationZonesModel : TransportationZonesSearchModel
        {
            public List<TransportationZonesInfoModel> ListTransportationZones { get; set; }
        }

        public class TransportationZonesInfoModel : TransportationZoneInfoModel
        {
            public string TransportationZoneInfo_DestinationName { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> TransportationZoneInfo_DrpDestinations
            {
                get
                {
                    var list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class ZonesModel
    {
        public class SearchZonesModel : ZonesSearchModel
        {
            public List<ZonesInfoModel> ListZones { get; set; }
        }

        public class ZonesInfoModel : ZoneInfoModel
        {
            public string ZoneInfo_DestinationName { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ZoneInfo_DrpDestinations
            {
                get
                {
                    var list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class CouponFoliosModel
    {
        public class SearchCouponFoliosModel
        {
            [Display(Name = "From Folio")]
            public string SearchCouponFolios_FromFolio { get; set; }

            [Display(Name = "To Folio")]
            public string SearchCouponFolios_ToFolio { get; set; }

            [Display(Name = "Serial")]
            public string SearchCouponFolios_Serial { get; set; }

            [Display(Name = "Company")]
            public string SearchCouponFolios_Companies { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchCouponFolios_DrpCompanies
            {
                get
                {
                    return CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                }
            }

            public List<CouponFoliosInfoModel> ListCouponFolios { get; set; }
        }

        public class CouponFoliosInfoModel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11309);
                }
            }

            public long CouponFoliosInfo_CouponFolioID { get; set; }

            [Display(Name = "From Folio")]
            [Required(ErrorMessage = "From Folio is required")]
            public string CouponFoliosInfo_FromFolio { get; set; }
            public string CouponFoliosInfo_FromFolioText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CouponFoliosInfo_DrpFolios
            {
                get
                {
                    return CatalogsDataModel.CouponFolios.CouponFoliosCatalogs.FillDrpCouponFolios();
                }
            }

            [Display(Name = "To Folio")]
            [Required(ErrorMessage = "To Folio is required")]
            public string CouponFoliosInfo_ToFolio { get; set; }
            public string CouponFoliosInfo_ToFolioText { get; set; }

            [Display(Name = "Serial")]
            public string CouponFoliosInfo_Serial { get; set; }

            [Display(Name = "Delivered")]
            public string CouponFoliosInfo_Delivered { get; set; }
            //public int CouponFoliosInfo_Delivered { get; set; }

            [Display(Name = "Generated")]
            public string CouponFoliosInfo_Generated { get; set; }
            //public int CouponFoliosInfo_Generated { get; set; }

            [Display(Name = "Available")]
            public string CouponFoliosInfo_Available { get; set; }
            //public int CouponFoliosInfo_Available { get; set; }

            [Display(Name = "Company")]
            public string CouponFoliosInfo_Company { get; set; }
            //public int CouponFoliosInfo_Company { get; set; }
            public string CouponFoliosInfo_CompanyText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CouponFoliosInfo_DrpCompanies
            {
                get
                {
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Point Of Sale")]
            public string CouponFoliosInfo_PointOfSale { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CouponFoliosInfo_DrpPointsOfSale
            {
                get
                {
                    var list = CatalogsDataModel.PointsOfSale.PoinsOfSaleCatalogs.FillDrpPointsOfSale();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class PointsOfSaleModel
    {
        public class SearchPointsOfSaleModel
        {
            [Display(Name = "Point Of Sale")]
            public string SearchPointsOfSale_PointOfSale { get; set; }

            //[Display(Name = "Terminal")]
            //public long[] SearchPointsOfSale_Terminals { get; set; }
            //public List<SelectListItem> SearchPointsOfSale_DrpTerminals
            //{
            //    get
            //    {
            //        return TerminalDataModel.GetActiveTerminalsList();
            //    }
            //}

            [Display(Name = "Place")]
            public string[] SearchPointsOfSale_Places { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPointsOfSale_DrpPlaces
            {
                get
                {
                    //return PlaceDataModel.GetResortsByProfile();
                    return PlaceDataModel.GetPlacesByDestinationsPerTerminals();
                }
            }

            public List<PointsOfSaleInfoModel> ListPointsOfSale { get; set; }
        }

        public class PointsOfSaleInfoModel
        {
            public int PointsOfSaleInfo_PointOfSaleID { get; set; }

            [Display(Name = "Point Of Sale")]
            [Required(ErrorMessage = "Point Of Sale is required")]
            public string PointsOfSaleInfo_PointOfSale { get; set; }

            [Display(Name = "Short Name")]
            [Required(ErrorMessage = "Short Name is required")]
            public string PointsOfSaleInfo_ShortName { get; set; }

            [Display(Name = "Place")]
            [Range(1, int.MaxValue, ErrorMessage = "Place is required")]
            public long PointsOfSaleInfo_Place { get; set; }
            public string PointsOfSaleInfo_PlaceText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PointsOfSaleInfo_DrpPlaces
            {
                get
                {
                    //var list = PlaceDataModel.GetResortsByProfile();
                    var list = PlaceDataModel.GetPlacesByDestinationsPerTerminals();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Policies Block")]
            public long PointsOfSaleInfo_PoliciesBlock { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PointsOfSaleInfo_DrpPoliciesBlocks
            {
                get
                {
                    var list = CatalogsDataModel.PointsOfSale.PoinsOfSaleCatalogs.FillDrpPoliciesBlock();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Accept Charges")]
            [Required(ErrorMessage = "Accept Charges is required")]
            public bool PointsOfSaleInfo_AcceptCharges { get; set; }

            [Display(Name = "Terminal")]
            public long PointsOfSaleInfo_Terminal { get; set; }
            public string PointsOfSaleInfo_TerminalName { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PointsOfSaleInfo_DrpTerminals
            {
                get
                {
                    return TerminalDataModel.GetCurrentUserTerminals();
                }
            }

            public SearchPointsOfSaleModel SearchPointsOfSaleModel { get; set; }
        }
    }

    public class PromosModel
    {
        public class PromoSearchModel
        {
            [Display(Name = "Promo Type")]
            public string[] PromoSearch_PromoTypes { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromoSearch_DrpPromoTypes
            {
                get
                {
                    return CatalogsDataModel.Promos.PromosCatalogs.FillDrpPromoTypes();
                }
            }

            [Display(Name = "Promo")]
            public string PromoSearch_Promo { get; set; }

            [Display(Name = "Booking Window Dates")]
            public string PromoSearch_FromDateBW { get; set; }

            public string PromoSearch_ToDateBW { get; set; }

            [Display(Name = "Travel Window Dates")]
            public string PromoSearch_FromDateTW { get; set; }

            public string PromoSearch_ToDateTW { get; set; }

            [Display(Name = "Promo Code")]
            public string PromoSearch_PromoCode { get; set; }

            public List<PromoInfoModel> ListPromos { get; set; }
        }

        public class PromoInfoModel
        {
            public long? PromoInfo_PromoID { get; set; }
            public DateTime PromoInfo_DateSaved { get; set; }

            [Display(Name = "Promo")]
            public string PromoInfo_Promo { get; set; }

            [Display(Name = "From Date BW")]
            public string PromoInfo_FromDateBW { get; set; }

            [Display(Name = "To Date BW")]
            public string PromoInfo_ToDateBW { get; set; }

            [Display(Name = "From Date TW")]
            public string PromoInfo_FromDateTW { get; set; }

            [Display(Name = "To Date TW")]
            public string PromoInfo_ToDateTW { get; set; }

            [Display(Name = "Amount")]
            public decimal? PromoInfo_Amount { get; set; }

            [Display(Name = "Currency")]
            public int? PromoInfo_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromoInfo_DrpCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesIntID();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Percentage")]
            public int? PromoInfo_Percentage { get; set; }

            [Display(Name = "Terminal")]
            public long PromoInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromoInfo_DrpTerminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpTerminals();
                }
            }

            [Display(Name = "Any Price")]
            public bool PromoInfo_AnyPrice { get; set; }

            [Display(Name = "Whole Stay")]
            public bool PromoInfo_WholeStay { get; set; }

            [Display(Name = "Per Night")]
            public bool PromoInfo_PerNight { get; set; }

            [Display(Name = "Combinable")]
            public bool PromoInfo_Combinable { get; set; }

            [Display(Name = "Monday")]
            public bool PromoInfo_Monday { get; set; }

            [Display(Name = "Tuesday")]
            public bool PromoInfo_Tuesday { get; set; }

            [Display(Name = "Wednesday")]
            public bool PromoInfo_Wednesday { get; set; }

            [Display(Name = "Thursday")]
            public bool PromoInfo_Thursday { get; set; }

            [Display(Name = "Friday")]
            public bool PromoInfo_Friday { get; set; }

            [Display(Name = "Saturday")]
            public bool PromoInfo_Saturday { get; set; }

            [Display(Name = "Sunday")]
            public bool PromoInfo_Sunday { get; set; }

            [Display(Name = "Publish")]
            public bool PromoInfo_Publish { get; set; }

            [Display(Name = "Promo Code")]
            public string PromoInfo_PromoCode { get; set; }

            [Display(Name = "Apply On Person")]
            public bool PromoInfo_applyOnPerson { get; set; }

            [Display(Name = "Is Package")]
            public bool PromoInfo_isPackage { get; set; }

            [Display(Name = "Type")]
            public int[] PromoInfo_Type { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromoTypes
            {
                get
                {
                    return CatalogsDataModel.Promos.FillDrpPromoTypes();
                }
            }
            // public List<string> PublishList { get; set; }
            [Display(Name = "Description")]
            public string PromoInfo_DescriptionID { get; set; }
            public string PromoInfo_DescriptionTag { get; set; }
            public string PromoInfo_DescriptionTitle { get; set; }
            public string PromoInfo_DescriptionDescription { get; set; }
            public string PromoInfo_DescriptionInstructions { get; set; }
            public int PromoInfo_DescriptionCulture { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PromoCultureList
            {
                get
                {
                    return CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpCultures();
                }
            }

        }
        public class PromoDescriptionModel
        {
            public long? PromoDescription_PromoDescriptionID { get; set; }

            public long PromoDescription_PromoID { get; set; }

            [Display(Name = "PromoTag")]
            public string PromoDescription_PromoTag { get; set; }

            [Display(Name = "Title")]
            public string PromoDescription_Title { get; set; }

            [Display(Name = "Description")]
            public string PromoDescription_Description { get; set; }

            [Display(Name = "Instructions")]
            public string PromoDescription_Instructions { get; set; }

            [Display(Name = "Language")]
            public string PromoDescription_Culture { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> CultureList
            {
                get
                {
                    var list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpCultures();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }
        public class PromoTypesModel
        {
            public int PromoTypesModelID { get; set; }
            public long PromoID { get; set; }
            public int PromoTypeID { get; set; }
        }

        public class GetPromoModel
        {
            public long? GetPromoID { get; set; }
            public long GetPromoTerminal { get; set; }
            public string GetPromoName { get; set; }
            public string GetPromoFromDateBW { get; set; }
            public string GetPromoToDateBW { get; set; }
            public string GetPromoFromDateTW { get; set; }
            public string GetPromoToDateTW { get; set; }
            public int GetPromoCurrency { get; set; }
            public decimal? GetPromoAmount { get; set; }
            public decimal? GetPromoPercentage { get; set; }
            public bool GetPromoAnyPrice { get; set; }
            public bool GetPromoWholeStay { get; set; }
            public bool GetPromoPerNight { get; set; }
            public bool GetPromoCombinable { get; set; }
            public bool GetPromoMonday { get; set; }
            public bool GetPromoTuesday { get; set; }
            public bool GetPromoWednesday { get; set; }
            public bool GetPromoThursday { get; set; }
            public bool GetPromoFriday { get; set; }
            public bool GetPromoSaturday { get; set; }
            public bool GetPromoSunday { get; set; }
            public bool GetPromoPublish { get; set; }
            public bool GetPromoApplyPerson { get; set; }
            public bool GetPromoPackage { get; set; }
            public string GetPromoCode { get; set; }
            public List<PromoTypesModel> GetPromoType { get; set; }
            public List<string> GetPromoDescription { get; set; }

        }
    }
    //Sales-Parties
    public class PartiesSales
    {
        public class SalesRoomPartiesViewModel
        {
            public List<PartiesSearchItem> SearchSalesRoomResults { get; set; }
            public SeachParties SearchSalesRooms { get; set; }
            public PartiesInfo SaveSalesRooms { get; set; }
        }

        public class SeachParties//search parties
        {
            [Display(Name = "Date")]
            public string SearchDate_FromDate { get; set; }

            [Display(Name = "Program")]
            public string SearchProgramID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchProgramsDrpList
            {
                get
                {
                    var list = CatalogsDataModel.PartiesSalesDataModel.SearchProgramList();
                    list.Insert(0, ListItems.Default());
                    return list;
                    //return CatalogsDataModel.PartiesSalesDataModel.ProgramList();
                }
            }
            [Display(Name = "Terminals")]
            public string SearchTerminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchTerminalList
            {
                get
                {
                    return TerminalDataModel.GetActiveTerminalsList();
                }
            }
            //place
        }
        public class PartiesInfo//add,update
        {
            public string PartiesInfo_SalesPartyID { get; set; }
            [Required(ErrorMessage = "Date is Required")]
            [Display(Name = "Date")]
            public string PartiesInfo_FromDate { get; set; }
            [Required(ErrorMessage = "Allotment is Required")]
            [Display(Name = "Allotment")]
            public string PartiesInfo_Allotment { get; set; }
            [Display(Name = "Sales Rooms")]
            public string PartiesInfo_Room { get; set; }//lista
            [ScriptIgnore]
            public List<SelectListItem> PartiesInfo_RoomDrpList
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = "--Any--",
                        Value = "0"
                    });
                    return list;
                }
            }
            [Required(ErrorMessage = "Place is Required")]
            [Display(Name = "Place")]
            public string PartiesInfo_PlaceID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PartiesInfo_PlacesDrpList
            {
                get
                {
                    var list = CatalogsDataModel.PartiesSalesDataModel.PlacesList(); //places by terminal
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            [Required(ErrorMessage = "Program is Required")]
            [Display(Name = "Program")]
            public string PartiesInfo_ProgramName { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PartiesInfo_ProgramsDrpList
            {
                get
                {
                    var list = new List<SelectListItem>();//CatalogsDataModel.PartiesSalesDataModel.ProgramList();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            [Required(ErrorMessage = "Terminal is Required")]
            [Display(Name = "Terminals")]
            public string PartiesInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PartiesInfo_Terminalist
            {
                get
                {
                    var list = TerminalDataModel.GetActiveTerminalsList();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            public string PartiesInfo_DateSaved { get; set; }
            public string PartiesInfo_SavedByUser { get; set; }
        }
        public class PartiesSearchItem//tabla
        {
            public long SalesRoomPartiesID { get; set; }
            [Display(Name = "Sales Room Parties")]
            public string SalesRoomParties { get; set; }
            [Display(Name = "Program")]
            public string Program { get; set; }
            [Display(Name = "Place")]
            public string Place { get; set; }
            [Display(Name = "Date")]
            public string PartiesDate { get; set; }
            [Display(Name = "Allotment")]
            public string Allotment { get; set; }
            [Display(Name = "Terminal")]
            public string Terminal { get; set; }
            [Display(Name = "Date Saved")]
            public string DateSaved { get; set; }
            [Display(Name = "Saved By User")]
            public string SavedByUser { get; set; }
        }
        public class GetSalesRoomsParties
        {
            public long SalesRoomsPartiesID { get; set; }
            public string SalesRoom { get; set; }
            public string Program { get; set; }
            public string Place { get; set; }
            public string Date { get; set; }
            public string Allotment { get; set; }
            public string Terminal { get; set; }
            public string DateSaved { get; set; }
            public string SavedByUser { get; set; }
        }//get roomsparties
    }

    public class OptionsModel
    {
        public class OptionsSearchModel
        {
            [Display(Name = "Option Name")]
            public string OptionsSearch_OptionName { get; set; }

            [Display(Name = "Option Type")]
            public int?[] OptionsSearch_OptionType { get; set; }

            [Display(Name = "Resort")]
            public long[] OptionsSearch_Place { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OptionsSearch_DrpPlaces
            {
                get
                {
                    return PlaceDataModel.GetResortsByProfile();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> OptionsSearch_DrpOptionTypes
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpOptionCategories();
                }
            }

            public List<OptionsInfomodel> ListOptions { get; set; }
        }
        public class OptionsInfomodel
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    //return AdminDataModel.GetViewPrivileges();
                    return AdminDataModel.GetViewPrivileges(1);
                }
            }

            public int OptionsInfo_OptionID { get; set; }

            [Display(Name = "Option Name")]
            [Required(ErrorMessage = "Option Name is required")]
            public string OptionsInfo_OptionName { get; set; }

            [Display(Name = "Option Description")]
            public string OptionsInfo_OptionDescription { get; set; }

            [Display(Name = "Option Type")]
            [Range(1, int.MaxValue, ErrorMessage = "Option Type is required")]
            public int? OptionsInfo_OptionType { get; set; }
            public string OptionsInfo_OptionTypeText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OptionsInfo_DrpOptionTypes
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpOptionCategories();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Resort(s)")]
            public long[] OptionsInfo_Place { get; set; }
            public string OptionsInfo_PlaceText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> OptionsInfo_DrpPlaces
            {
                get
                {
                    return PlaceDataModel.GetResortsByProfile();
                }
            }

            [Display(Name = "Gold Card Price")]
            public decimal OptionsInfo_GoldCardPrice { get; set; }

            [Display(Name = "Resort Credit")]
            public decimal OptionsInfo_ResortCredit { get; set; }

            public bool OptionsInfo_Active { get; set; }
        }
    }

    public class MarketCodes
    {
        public class SearchMarketCodes
        {
            [Display(Name = "Market Code")]
            public string MarketCodeSearch_MarketCode { get; set; }
            [Display(Name = "Resort")]
            public int?[] MarketCodeSearch_Places { get; set; }
            public List<SelectListItem> MarketCodeSearch_DrpPlaces
            {
                get
                {
                    return CatalogsDataModel.MarketCodesModel.FillFrontPlaces();
                }
            }

            [Display(Name = "Assigned To")]
            //public Guid?[] MarketCodeSearch_Users { get; set; }
            public Guid?[] MarketCodeSearch_Users { get; set; }
            public List<SelectListItem> MarketCodeSearch_DrpUsers
            {
                get
                {
                    var users = UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup(true);
                    return users;
                }
            }

            [Display(Name = "Lead Source(s)")]
            public string[] MarketCodeSearch_LeadSources { get; set; }
            public List<SelectListItem> MarketCodeSearch_DrpLeadSources
            {
                get
                {
                    var list = LeadSourceDataModel.GetLeadSourcesByTerminal();
                    list.Insert(0, new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Not Assigned--"
                    });
                    return list;
                }
            }
            public List<MarketCodesResults> ListResults { get; set; }
        }

        public class MarketCodeInfo
        {
            public bool MarketCodeInfo_Assign { get; set; }
            public int MarketCodeInfo_MarketCodeID { get; set; }
            
            [Display(Name = "Market Code")]
            public string MarketCodeInfo_MarketCode { get; set; }
            
            public string MarketCodeInfo_UsersText { get; set; }
            
            public string MarketCodeInfo_MarketCodes { get; set; }
            [Display(Name = "Assign Selected Codes To")]
            public long? MarketCodeInfo_AssignTo { get; set; }
            
            [Display(Name = "Resort")]
            public int? MarketCodeInfo_Place { get; set; }
            public List<SelectListItem> MarketCodeInfo_DrpPlaces
            {
                get
                {
                    var list = CatalogsDataModel.MarketCodesModel.FillFrontPlaces();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            public long? MarketCodeInfo_Terminal { get; set; }
            public List<SelectListItem> MarketCodeInfo_DrpTerminals
            {
                get
                {
                    var list = TerminalDataModel.GetActiveTerminalsList();
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "User(s)")]
            public Guid?[] MarketCodeInfo_Users { get; set; }
            public List<SelectListItem> MarketCodeInfo_DrpUsers
            {
                get
                {
                    return UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup(true);
                }
            }

            [Display(Name = "Lead Source")]
            public long MarketCodeInfo_LeadSource { get; set; }
            public List<SelectListItem> MarketCodeInfo_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }
        }

        public class MarketCodesResults
        {
            public bool MarketCodeInfo_Checked { get; set; }
            public int MarketCodeInfo_MarketCodeID { get; set; }
            public string MarketCodeInfo_MarketCode { get; set; }
            public string MarketCodeInfo_UsersText { get; set; }
            public string MarketCodeInfo_MarketCodes { get; set; }
            public string MarketCodeInfo_Resort { get; set; }
            public string MarketCodeInfo_LeadSource { get; set; }
        }
    }

    public class Banks 
    {
        public class BanksViewModel
        {
            public SearchBanks SearchBank { get; set; }
            public BankItem SaveBank { get; set; }
        }
        public class SearchBanks {

            [Display (Name ="Bank Name")]
            public string SearchBank { get; set; }
            [Display (Name="Registered between")]
            public DateTime? SearchFromDate {get;set;}
            [Display (Name="and")]
            public DateTime? SearchToDate { get; set; }
            [Display (Name="Terminal")]
            public string[] SearchTerminal { get; set; }
            public List<SelectListItem> TerminalList { 
                get{
                   var list = TerminalDataModel.GetCurrentUserTerminals();
                   list.Add( new  SelectListItem() 
                   { 
                    Value="0",
                    Text="Select One"
                   });
                   return list;
                }
            }

        }
        public class BankItem {
            public int? BankID { get; set; }
            [Display (Name="Bank")]
            [Required]
            public string BankName { get; set; }
            [Display (Name="Key Sat")]
            public string CveSat { get; set; }
            [Display(Name = "From Date")]
            [Required]
            public DateTime? FromDate { get; set; }
            [Display(Name = "To Date")]
            public DateTime? ToDate { get; set; }
            [Display (Name="Terminal")]
            public string Terminal { get; set; }
            [Required]
            public long TerminalID { get; set; }
        }
        //public class NewBank { 
        //    public int NewBankID { get; set; }
        //    [Display (Name="Bank")]
        //    [Required]
        //    public string  NewBankName { get; set; }
        //    [Display (Name="CveSat")]
        //    public string NewCveSat { get; set; }
        //    [Display (Name="From Date")]
        //    [Required]
        //    public DateTime NewFromDate { get; set; }
        //    [Display (Name="To Date")]
        //    public DateTime? NewToDate { get; set; }
        //    [Display (Name="Terminals")]
        //    public long NewTerminalID { get; set; }
        //    public List<SelectListItem> TerminalList {
        //        get {
        //            var list = TerminalDataModel.GetCurrentUserTerminals();
        //            list.Add(new SelectListItem() 
        //            { 
        //                Value="0",
        //                Text="Select One"
        //            });
        //        return list;
        //        }
        //    }
        //}
        //public class GetBanks {
        //    public int BankID { get; set; }
        //    public string Bank { get; set; }
        //    public string CveSat { get; set; }
        //    public string FromDate { get; set; }
        //    public string toDate { get; set; }
        //    public long TerminalID { get; set; }
        //}
    }

    public class Boats 
    {
        public class BoatsViewModel
        {
            public List<BoatSearchItems> SearchBoatResults { get; set; }
            public SearchBoats SearchBoat { get; set; }
            public NewBoat SaveBoat { get; set; }
        }
        public class NewBoat {
            public int BoatID { get; set; }
            [Display (Name="Boat")]
            [Required]
            public string Boat { get; set; }
            [Display (Name="Qouta")]

            public int Qouta { get; set; }
            [Display (Name="Alias")]
            [Required]
            public string Shortname { get; set; }
            public Guid SavedByUser { get; set; }
            public DateTime DateSaved { get; set; }
            [Display (Name="Provider")]
            public int ProvidersID { get; set; }
            public List<SelectListItem> ProvidersList
            {
                get 
                {
                    var list= CatalogsDataModel.Providers.ProvidersCatalogs.FillDrpProvidersPerTerminals(null);
                    list.Add(new SelectListItem(){
                        Value="0",
                        Text="--Select One--"
                    });
                    return list;
                }
            }
            [Display(Name = "Terminal")]     
            public long TerminalID { get; set; }
            public List<SelectListItem> TerminalList 
            {
                get {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="Select One"
                    });
                return list;
                }
            }
        }
        public class SearchBoats
        {
            [Display (Name="Boat Name")]
            public string SearchBoat { get; set; }
            [Display (Name="Alias")]
            public string Alias { get; set; }
            [Display (Name="Terminal")]
            public string Terminal { get; set; }
            public List<SelectListItem> TerminalList
            {
                get 
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="Select One"
                    });
                return list;
                }
            }
        }
        public class BoatSearchItems 
        {
            public int BoatSearchItemID { get; set; }
            [Display (Name="Boat")]
            public string BoatSearchItemName { get; set; }
            [Display (Name="Qouta")]
            public int BoatSearchItemQuota { get; set; }
            [Display (Name="Alias")]
            public string BoatSearchItemAlias { get; set; }
            [Display (Name="Provider")]
            public string BoatSearchItemProviders { get; set; }
            [Display (Name="Terminal")]
            public string BoatSearchItemTerminal { get; set; }
            
        }
        public class GetBoats 
        {
            public int BoatID { get; set; }
            public string Boat { get; set; }
            public int Qouta { get; set; }
            public string Alias { get; set; }
            public int? Providers { get; set; }
            public long? Terminals { get; set; }

        }

        
    }

    public class SalesChanels 
    {
        public class SalesChannelViewModel 
        {
            public List<SaleChannelSearchItems> SearchSalesChannelsResults { get; set; }
            public SearchSaleChannel SearchSalesChannels { get; set; }
            public NewSalesChannel SaveBoat { get; set; }
        }
        public class NewSalesChannel 
        {
            public int SalesChannnelID { get; set; }
            [Display (Name="Sale Channel")]
            [Required]
            public string SalesChannel { get; set; }
            public Guid SavedByUser { get; set; }
            public DateTime DateSaved { get; set; }
            [Display(Name = "Terminal")]
            [Required]
            public long TerminalID { get; set; }
            public List<SelectListItem> TerminalList
            {
                get {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="",
                        Text="Select One"
                    });
                return list;
                }
            }
        }
        public class SearchSaleChannel
        {
            [Display (Name="Sale Channel")]
            public string Search_SaleChannel { get; set; }
            [Display (Name="Terminal")]
            public string SearchTerminal { get; set; }
            public List<SelectListItem> TerminalList {
                get {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="Select One"
                    });
                return list;
                }
            }
        }
        public class SaleChannelSearchItems 
        {
            public int SaleChannelSearchItemID { get; set; }
            public string SaleChannelSearchItemName { get; set; }
            public string SaleChannelSearchItemUser { get; set; }
            public DateTime SaleChannelSearchItemDate { get; set; }
            public string SaleChannelSearchItemTerminal { get; set; }
        }
        public class GetSalesChannels 
        {
            public int SalesChannelID { get; set; }
            public string SaleChannel { get; set; }
            public Guid UserId { get; set; }
            public string DateSaved { get; set; }
            public long? Terminal { get; set; }
        }
    }

    public class Bracelets
    {
        public class BraceletsViewModel
        {
            public List<BraceletSearchItems> SearchBraceletResult { get; set; }
            public SearchBracelet SearchBracelet { get; set; }
            public NewBracelet SaveBracelet { get; set; }
        }

        public class NewBracelet 
        {
            public int BraceletID { get; set; }
            [Display (Name="Bracelet")]
            [Required]
            public string Bracelet { get; set; }
            [Display (Name="Terminal")]
            [Required]
            public long TerminalID { get; set; }
            public List<SelectListItem> TerminalList
            {
                get 
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="",
                        Text="--Select One--"
                    });
                return list;
                }
            }
        }

        public class SearchBracelet 
        {
            [Display (Name="Bracelet")]
            public string Bracelet { get; set; }
            [Display (Name="Terminal")]
            public string Terminal { get; set; }
             public List<SelectListItem> TerminalList
            {
                get 
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="Select One"
                    });
                return list;
                }
            }
        
        }

        public class BraceletSearchItems
        {
            public int BraceletID { get; set; }
            public string Bracelet { get; set; }
            public string Terminal { get; set; }
        }

        public class GetBracelets 
        {
            public int GetBraceletID { get; set; }
            public string GetBracelet { get; set; }
            public long? GetTerminalID { get; set; }
        }
    }

    public class Reminders
    {
        public class RemindersViewModel
        {
            public List<ReminderSearchItems> SearchRemindersResult { get; set; }
            public SearchReminder SearchReminder { get; set; }
            public NewReminder SaveReminder { get; set; }
        }
        public class NewReminder
        {
            public long ReminderID { get; set; }
            [Display (Name="Tour")]
            [Required]
            public long ServiceID { get; set; }
            public List<SelectListItem> ServicesList
            {
                get 
                {
                    var list = ActivityDataModel.ActivitiesCatalogs.GetServicesPerTerminalsActives();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="--Select One--"
                    });
                    return list;
                }
                
            }
            [Display (Name="From Date")]
            [Required]
            public DateTime FromDate { get; set; }
            [Display (Name="To Date")]
            [Required]
            public DateTime ToDate { get; set; }
            [Display (Name="Message")]
            [Required]
            public string Message { get; set; }
            [Display (Name="Date Saved")]
            public DateTime DateSaved { get; set; }
            [Display (Name="Saved by User")]
            public Guid SavedByUserID { get; set; }
            [Display (Name="Terminal")]
            [Required]
            public long TerminalID { get; set; }
            public List<SelectListItem> TerminalList 
            {
                get 
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="",
                        Text="--Select One--"
                    });
                    return list;
                }
            }
        }
        public class SearchReminder 
        {
            [Display (Name="From Date")]
            public DateTime? FromDate { get; set; }
            [Display (Name="To Date")]
            public DateTime? ToDate { get; set; }
            [Display (Name="Terminal")]
            public string Terminal { get; set; }
            public List<SelectListItem> TerminalList
            {
                get 
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem()
                    {
                        Value="0",
                        Text="Select One"
                    });
                return list;
                }
            }
        }
        public class ReminderSearchItems
        {
            public long ReminderID { get; set; }
            [Display (Name="Tour")]
            public string Service { get; set; }
            [Display (Name="From Date")]
            public DateTime? FromDate { get; set; }
            [Display (Name="To Date")]
            public DateTime? ToDate { get; set; }
            [Display (Name="Message")]
            public string Message { get; set; }
            [Display (Name="Terminal")]
            public string Terminal { get; set; }

        }
        public class GetReminder
        {
            public long GetReminderID { get; set; }
            public string GetFromDate { get; set; }
            public string GetToDate { get; set; }
            public string GetMessage { get; set; }
            public long? GetServiceID { get; set; }
            public long GetTerminalID { get; set; }


        }
    }

    public class Operators 
    {
        public class OperatorViewModel 
        {
            public List<OperatorSearchItems> SearchOperatorResult { get; set; }
            public SearchOperator SearchOperator { get; set; }
            public NewOperator SaveOperador { get; set; }
        }
        public class NewOperator
        {
            public Guid NewOperatorID { get; set; }
            public string NewOperatorName { get; set; }
            public string NewContactEmail { get; set; }
            public bool NewDiscount { get; set; }
            public bool NewAuthorization { get; set; }
            public int SalesChannelID { get; set; }
            public List<SelectListItem> SalesList
            {
                get
                {
                    var list = CatalogsDataModel.SalesChannelsModel.FillDrpSalesChannelsPerTerminals(null);
                    list.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Select One--"
                    });
                    return list;
                }
            }
            public int TerminalID { get; set; }
            public List<SelectListItem> TerminalList 
            {
                get
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="--Select One--"
                    });
                    return list;
                }
            }
            public int CancelationLimit { get; set; }
            public string AditionalInfo { get; set; }
            public decimal MaxDiscount { get; set; }
            public Guid SavedByExecutiveUserID { get; set; }
            public DateTime DateSaved { get; set; }
            public int CompanyID { get; set; }
            public List<SelectListItem> CompanyList
            {
                get 
                {
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="--Select One--"
                    });
                    return list;
                }
            }
        }
        public class SearchOperator 
        {
            [Display (Name="Operator")]
            public string Operator { get; set; }
            [Display (Name="Company")]
            public string Company {get;set;}
            public List<SelectListItem> CompaniesList
            {
                get
                {
                    var list = CatalogsDataModel.Companies.CompaniesCatalogs.FillDrpCompanies();
                    list.Add(new SelectListItem() 
                    { 
                        Value="0",
                        Text="Select One"
                    });
                    return list;
                }
            }
        }
        public class OperatorSearchItems
        {
            public Guid OperatorID { get; set; }
            [Display (Name="Operator")]
            public string Operator { get; set; }
            [Display (Name="SaleChannel")]
            public string SalesChannel { get; set; }
            [Display (Name="Saved By User")]
            public string SavedByUser { get; set; }
            [Display (Name="Date Saved")]
            public string DateSaved { get; set; }
            [Display (Name="Company")]
            public string Company { get; set; }
        }
        public class GetOperator
        {
            public Guid OperatorID { get; set; }
            public string Operator { get; set; }
            public string Contact { get; set; }
            public bool Discount { get; set; }
            public bool Authorization { get; set; }
            public int SalesChannelID { get; set; }
            public int CancelationLimit { get; set; }
            public string AditionInfo { get; set; }
            public decimal MaxDiscount { get; set; }
            public int CompanyID { get; set; }
        }
    }

    public class Tours
    {
        public class QoutaViewModel 
        {
            public List<Tour_QoutaSearchItems> SearchQoutaResult { get; set; }
            public Tour_SearchQouta SearchQouta { get; set; }
            public Tour_NewQouta SaveQouta { get; set; }
        }
        public class Tour_NewQouta 
        {
            public int NewQoutaID { get; set; }
            [Display (Name="Service")]
            public long NewServiceID { get; set; }
            public List<SelectListItem> NewServicesList
            {
                get
                {
                    var list = ActivityDataModel.ActivitiesCatalogs.FillDrpServicesPerTerminal(null);
                    list.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Select One--"
                    });
                    return list;
                }

            }
            [Display (Name="Qouta")]
            public int NewQouta { get; set; }
            [Display (Name="From Date")]
            public DateTime NewFromDate { get; set; }
            [Display (Name="To Date")]
            public DateTime NewToDate { get; set; }
            [Display (Name="Note")]
            public string NewNote { get; set; }
            [Display (Name="Permanent")]
            public bool permanent { get; set; }
            public DateTime NewDateSaved { get; set; }
            public Guid NewUserID { get; set; }
        }
        public class Tour_SearchQouta
        {
            [Display (Name="From Date")]
            public DateTime? SearchDate_FromDate { get; set; }
            [Display (Name="To Date")]
            public DateTime? SearchDate_ToDate { get; set; }
            [Display (Name="Tour")]
            public long SearchServiceID { get; set; }
            public List<SelectListItem> ServicesList
            {
                get
                {
                    var list = ActivityDataModel.ActivitiesCatalogs.FillDrpServicesPerTerminal(null);
                    list.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Select One--"
                    });
                    return list;
                }

            }
        }
        public class Tour_QoutaSearchItems
        {
            public long QoutaID { get; set; }
            [Display (Name="Tour")]
            public string Tour { get; set; }
            [Display (Name="Qouta")]
            public int Qouta { get; set; }
            [Display (Name="From Date")]
            public string FromDate { get; set; }
            [Display (Name="To Date")]
            public string ToDate { get; set; }
            [Display (Name="Note")]
            public string Note { get; set; }
            [Display (Name="Date Saved")]
            public string DateSaved { get; set; }
            [Display (Name="Saved By User")]
            public string SavedByUser { get; set; }
            
        }
        public class Tour_GetQouta
        {
            public int GetQoutaID { get; set; }
            public int GetQouta { get; set; }
            public string GetFromDate { get; set; }
            public string GetToDate { get; set; }
            public string GetNotes { get; set; }
            public bool Getpermanent { get; set; }

        }
    }

    public class HotelPickUps
    {
        public class SearchHotels {
            [Display(Name = "Hotel Name")]
            public string Search_HotelName { get; set; }
            [Display(Name = "Destination")]
            public long? Search_DestinationID { get; set; }
            [Display(Name = "Zone")]
            public int? Search_ZoneID { get; set; }
            [Display(Name = "Terminal")]
            public long? Search_TerminalID { get; set; }
        }

        public class HotelPickUp
        {
            public Guid? HotelPickUpID { get; set; }
            public string Hotel { get; set; }
            public int SpiHotelID { get; set; }
            public long? DestinationID { get; set; }
            public List<DependantFields.FieldValue> Destinations { get; set; }
            public int? ZoneID { get; set; }
            public string Picture { get; set; }
            public string DescriptionENUS { get; set; }
            public string DescriptionESMX { get; set; }
            public string Lat { get; set; }
            public string Lng { get; set; }
            public long? TerminalID { get; set; }
            public bool PendingChanges { get; set; }
        }
    }

    public class UsersLeadSources
    {
        public class SearchUserLeadSources : Lists
        {
            [Display(Name = "User(s)")]
            public Guid?[] Search_Users { get; set; }

            [Display(Name = "Lead Source(s)")]
            public long?[] Search_LeadSources { get; set; }

            [Display(Name = "Resort(s)")]
            public long?[] Search_Resorts { get; set; }

            public List<UserLeadSources> ListResults { get; set; }
        }

        public class UserLeadSources : Lists
        {
            public long? User_LeadSourceID { get; set; }

            [Display(Name = "User(s)")]
            public Guid[] Users { get; set; }
            public string User { get; set; }

            [Display(Name = "Lead Source(s)")]
            public long?[] LeadSources { get; set; }
            public string LeadSource { get; set; }

            [Display(Name = "Resort(s)")]
            public int?[] Resorts { get; set; }
            public string Resort { get; set; }

            [Display(Name = "From Date")]
            public string FromDate { get; set; }

            [Display(Name = "To Date")]
            public string ToDate { get; set; }

            public string SavedByUser { get; set; }
        }

        public class Lists
        {
            public List<SelectListItem> ListUsers
            {
                get
                {
                    return UserDataModel.GetUsersBySupervisor();
                }
            }

            public List<SelectListItem> ListLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }

            public List<SelectListItem> ListResorts
            {
                get
                {
                    return PreArrivalDataModel.PreArrivalCatalogs.GetFrontResorts();
                }
            }
        }
    }
}
