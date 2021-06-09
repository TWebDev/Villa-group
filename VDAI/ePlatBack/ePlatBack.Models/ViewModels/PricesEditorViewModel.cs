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
    public class PricesEditorViewModel
    {
        public class ParamsPriceEditor
        {
            public long ParamsPricesEditor_ServiceID { get; set; }
            [Display(Name = "Date")]
            [Required]
            public string ParamsPricesEditor_Date { get; set; }
            [Display(Name = "Terminal")]
            [Required]
            public long ParamsPricesEditor_TerminalID { get; set; }
            [Display(Name = "Point of Sale")]
            public int ParamsPricesEditor_PointOfSaleID { get; set; }
            [Display(Name = "Nationality")]
            public string ParamsPricesEditor_Nationality { get; set; }
            public List<SelectListItem> Nationalities
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = "Select one",
                        Value = ""
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Foreigners",
                        Value = "en-US"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Nationals",
                        Value = "es-MX"
                    });
                    return list;
                }
            }

            public List<SelectListItem> PointsOfSale
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                }
            }

            public List<SelectListItem> Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
        }

        public class RoundingRuleModel
        {
            public int PriceTypeID { get; set; }
            public bool RoundUp { get; set; }
            public bool RoundDown { get; set; }
            public bool RoundToFifty { get; set; }
        }

        public class PriceInfo
        {
            public string Provider { get; set; }
            public string Item { get; set; }
            public string ProviderContractCurrency { get; set; }
            public string ProviderContractDate { get; set; }
            public decimal ExchangeRate { get; set; }
            public string ExchangeRateType { get; set; }
            public List<PriceRuleModel> Rules { get; set; }
            public List<ComputedPriceModel> Prices { get; set; }
            public List<RoundingRuleModel> RoundingRules { get; set; }
        }
    }
}
