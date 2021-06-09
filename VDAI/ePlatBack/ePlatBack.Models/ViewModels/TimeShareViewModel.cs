using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using ePlatBack.Models.Utils;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Models.ViewModels
{
    public class TimeShareViewModel
    {
//        public AccountancyModel AccountancyModel { get; set; }
        public OutcomeSearchModel AccountancyModel_OutcomeSearchModel { get; set; }

        public OutcomeInfoModel AccountancyModel_OutcomeInfoModel { get; set; }

        public List<OutcomeInfoModel> AccountancyModel_SearchResults { get; set; }

        public IncomeSearchModel AccountancyModel_IncomeSearchModel { get; set; }

        public IncomeInfoModel AccountancyModel_IncomeInfoModel { get; set; }

        public List<IncomeInfoModel> AccountancyModel_SearchIncomeResults { get; set; }

        public List<SysComponentsPrivilegesModel> Privileges
        {
            get
            {
                return AdminDataModel.GetViewPrivileges(10968);//timeshare operations
            }
        }
    }

        public class OutcomeSearchModel
        {
            [Display(Name = "Folio")]
            public int? OutcomeSearch_Folio { get; set; }

            [Display(Name = "Customer")]
            public string OutcomeSearch_Customer { get; set; }

            [Display(Name = "Date")]
            public string OutcomeSearch_I_Date { get; set; }
            public string OutcomeSearch_F_Date { get; set; }

            [Display(Name = "OPC")]
            public string[] OutcomeSearch_Opc { get; set; }
            public List<SelectListItem> OutcomeSearch_DrpOpcs
            {
                get
                {

                    //var list = new PurchasesModel.Views().OPCS;
                    //list.RemoveAt(list.Count() - 1); //element with "null"-"other" pair in 0 index
                    //list.Insert(0, ListItems.NotSet("--Select One--"));
                    return MasterChartDataModel.LeadsCatalogs.FillDrpOPC();
                }
            }

            [Display(Name = "Agent")]
            public string[] OutcomeSearch_Agent { get; set; }
            public List<SelectListItem> OutcomeSearch_DrpAgents
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.ListWorkGroupUsers();
                    //return MasterChartDataModel.LeadsCatalogs.FillDrpCurrentWorkGroupAgents(true,true);
                }
            }

            [Display(Name = "Point(s) Of Sale")]
            public string[] OutcomeSearch_PointOfSale { get; set; }
            public List<SelectListItem> OutcomeSearch_DrpPointsOfSale
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(true);
                }
            }

            [Display(Name = "Invitation Number")]
            public string OutcomeSearch_InvitationNumber { get; set; }
        }

        public class OutcomeInfoModel : Views
        {
            public long OutcomeInfo_EgressID { get; set; }

            [Display(Name = "Gifting")]
            [Range(1, int.MaxValue, ErrorMessage = "Gifting is required")]
            public int OutcomeInfo_EgressType { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpEgressTypes
            {
                get
                {
                    return EgressTypes;
                }
            }
            
            [Display(Name = "Concept")]
            public long OutcomeInfo_EgressConcept { get; set; }
            public string OutcomeInfo_EgressConceptText { get; set; }
            //public List<SelectListItem> OutcomeInfo_DrpEgressConcepts
            public List<SelectListItem> OutcomeInfo_DrpEgressConcepts
            {
                get
                {
                    //return EgressConcepts;
                    return new List<SelectListItem>();
                }
            }
            public int OutcomeInfo_Fund { get; set; }
            
            [Display(Name = "Point Of Sale")]
            [Required(ErrorMessage = "Point Of Sale is required")]
            public int OutcomeInfo_PointOfSale { get; set; }
            public string OutcomeInfo_PointOfSaleText { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpPointsOfSale
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(true);
                }
            }
            
            [Display(Name = "Admin Fee Percentage")]
            public decimal? OutcomeInfo_AdminFee { get; set; }
            
            [Display(Name = "OPC")]
            [Required(ErrorMessage = "OPC is required", AllowEmptyStrings = false)]
            public string OutcomeInfo_Opc { get; set; }
            public string OutcomeInfo_OpcText { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpOpcs
            {
                get
                {
                    //var list = new PurchasesModel.Views().OPCS;
                    //list.RemoveAt(list.Count() - 1); //element with "null"-"other" pair in 0 index
                    return new List<SelectListItem>();
                }
            }
            
            [Display(Name = "OPC Full Name")]
            public string OutcomeInfo_OtherOpc { get; set; }
            
            [Display(Name = "Promotion Team")]
            public string OutcomeInfo_PromotionTeam { get; set; }
            public string OutcomeInfo_PromotionTeamText { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpPromotionTeams
            {
                get
                {
                    return new PurchasesModel.Views().PromotionTeams;
                }
            }

            [Display(Name = "Budget")]
            public string OutcomeInfo_Budget { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpBudgets
            {
                get
                {
                    var list = CatalogsDataModel.Budgets.BudgetsCatalogs.FillDrpBudgetsPerTeam();
                    list.Insert(0, ListItems.Default("--None--"));
                    return list;

                }
            }
            
            [Display(Name = "Charged To:")]
            public string OutcomeInfo_ChargedToCompany { get; set; }
            public string OutcomeInfo_ChargedToCompanyText { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpCompanies
            {
                get
                {
                    //return new PurchasesModel.Views().Companies;
                    return MasterChartDataModel.LeadsCatalogs.FillDrpMarketingCompaniesPerTerminals(null);
                }
            }
            
            [Display(Name = "Customer's Full Name")]
            [Required(ErrorMessage = "Customer's Full Name is required")]
            public string OutcomeInfo_Customer { get; set; }
            
            [Display(Name = "Invitation Number")]
            public string OutcomeInfo_Invitation { get; set; }
            
            [Display(Name = "Amount")]
            [Required(ErrorMessage = "Amount is required")]
            public decimal OutcomeInfo_Amount { get; set; }
            
            [Display(Name = "Amount Of Sale")]
            public decimal? OutcomeInfo_AmountOfSale { get; set; }
            
            [Display(Name = "Currency Of Sale")]
            [RequiredIfDistinct(OtherProperty = "OutcomeInfo_AmountOfSale", DistinctOf = "", ErrorMessage = "Currency Of Sale is required")]
            public string OutcomeInfo_CurrencyOfSale { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpCurrenciesOfSale
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                    list.Insert(0, ListItems.NotSet("--Select One--",""));
                    return list;
                }
            }
            
            [Display(Name = "Currency")]
            [Required(ErrorMessage = "Currency is required", AllowEmptyStrings = false)]
            public string OutcomeInfo_Currency { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpCurrencies
            {
                get
                {
                    return Currencies;
                }
            }
            
            [Display(Name = "Location")]
            public string OutcomeInfo_Location { get; set; }
            public string OutcomeInfo_LocationText { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpLocations
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpLocationsPerCurrentTerminals(0);
                    return list;
                }
            }
            
            [Display(Name = "Terminal")]
            public int OutcomeInfo_Terminal { get; set; }
            public List<SelectListItem> OutcomeInfo_DrpTerminals
            {
                get
                {
                    return new PurchasesModel.Views().Terminals;
                }
            }
            [Display(Name = "Date Saved")]
            public string OutcomeInfo_DateSaved { get; set; }
            public string OutcomeInfo_SavedByUser { get; set; }
            
            [Display(Name = "Agent Comments")]
            public string OutcomeInfo_AgentComments { get; set; }
            public string OutcomeInfo_Agent { get; set; }
            public string OutcomeInfo_EgressTypeConcept { get; set; }
            public bool OutcomeInfo_SaveAndPrint { get; set; }
            public bool OutcomeInfo_UpdateEgress { get; set; }

            //manifest fields

        }

        public class IncomeSearchModel : Views
        {
            [Display(Name = "Company")]
            public string[] IncomeSearch_Company { get; set; }
            public List<SelectListItem> IncomeSearch_DrpCompanies
            {
                get
                {
                    var list = Companies;
                    //list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Date")]
            public string IncomeSearch_I_Date { get; set; }
            public string IncomeSearch_F_Date { get; set; }

            [Display(Name = "Guard")]
            public string[] IncomeSearch_Receiver { get; set; }
            public List<SelectListItem> IncomeSearch_DrpReceivers
            {
                get
                {
                    var list = Users;
                    //var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrentWorkGroupAgents(true, true);
                    //list.Insert(0, ListItems.Default());
                    return list;
                }
            }
        }

        public class IncomeInfoModel : Views
        {
            public long IncomeInfo_IncomeID { get; set; }

            [Display(Name = "Income Concept")]
            public int IncomeInfo_IncomeConcept { get; set; }
            public string IncomeInfo_IncomeConceptText { get; set; }
            public List<SelectListItem> IncomeInfo_DrpIncomeConcepts
            {
                get
                {
                    var list = IncomeConcepts;
                    //list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Company")]
            public string IncomeInfo_Company { get; set; }
            public List<SelectListItem> IncomeInfo_DrpCompanies
            {
                get
                {
                    var list = Companies;
                    //list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            //public int IncomeInfo_Fund { get; set; }

            [Display(Name = "Amount")]
            [Required(ErrorMessage = "Amount is required")]
            public decimal IncomeInfo_Amount { get; set; }

            [Display(Name = "Currency")]
            [Required(ErrorMessage = "Currency is required", AllowEmptyStrings = false)]
            public string IncomeInfo_Currency { get; set; }
            public List<SelectListItem> IncomeInfo_DrpCurrencies
            {
                get
                {
                    var list = Currencies;
                    //list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }

            [Display(Name = "Guard")]
            public string IncomeInfo_ReceiverUser { get; set; }
            //public List<SelectListItem> IncomeInfo_DrpReceivers
            //{
            //    get
            //    {
            //        var list = Users;
            //        //list.Insert(0, ListItems.NotSet("--Select One--", ""));
            //        return list;
            //    }
            //}

            [Display(Name = "Fund")]
            public int IncomeInfo_Fund { get; set; }
            public List<SelectListItem> IncomeInfo_DrpFunds
            {
                get
                {
                    return Funds;
                }
            }

            [Display(Name = "Reset Fund")]
            public int IncomeInfo_ResetFund { get; set; }

            [Display(Name = "Correct Amount of Fund")]
            public decimal IncomeInfo_ResetAmount { get; set; }

            public string IncomeInfo_DateSaved { get; set; }
        }

        public class Views : SPIViewModel.AgencyCustomer
        {
            public List<SelectListItem> EgressTypes
            {
                get
                {
                    //return MasterChartDataModel.LeadsCatalogs.FillDrpEgressTypes();
                    return new List<SelectListItem>();
                }
            }

            public List<EgressConceptModel> EgressConcepts
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpEgressConcepts();
                }
            }
            //public List<SelectListItem> EgressConcepts
            //{
            //    get
            //    {
            //        return MasterChartDataModel.LeadsCatalogs.FillDrpEgressConcepts();
            //    }
            //}

            public List<SelectListItem> Users
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.ListWorkGroupUsers();
                    //return MasterChartDataModel.LeadsCatalogs.FillDrpCurrentWorkGroupAgents();
                }
            }

            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                }
            }

            public List<SelectListItem> Companies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCompaniesPerSelectedTerminals();
                }
            }

            public List<SelectListItem> IncomeConcepts
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpIncomeConcepts();
                }
            }

            public List<SelectListItem> Funds
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpFundsPerSelectedTerminals(null, true);
                }
            }

        }

        public class EgressConceptModel
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public string Gifting { get; set; }
        }
}
