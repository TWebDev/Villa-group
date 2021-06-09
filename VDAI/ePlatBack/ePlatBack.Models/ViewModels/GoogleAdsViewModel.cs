using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ePlatBack.Models.ViewModels
{
    public class GoogleAdsViewModel
    {
        public class Reports
        {
            public class SearchAdsInvestment
            {
                [Display(Name = "Period")]
                public string Search_I_FromDate { get; set; }
                public string Search_F_ToDate { get; set; }

                [Display(Name = "Accounts")]
                public string[] Search_Accounts { get; set; }
                public List<SelectListItem> GoogleAdsAccounts { get; set; }
            }

            public class AdsInvestmentReport
            {
                public List<AccountInvestment> Accounts { get; set; }
            }
        }

        public class AccountInvestment
        {
            public string AccountName { get; set; }
            public List<CampaignInvestment> Campaigns { get; set; }
            public CampaignInvestment Totals { get; set; }
        }

        public class CampaignInvestment
        {
            public DateTime Date { get; set; }
            public string CampaignName { get; set; }
            public string Destination { get; set; }
            public int Clicks { get; set; }
            public int Impressions { get; set; }
            public decimal CostMXN { get; set; }
            public string Ctr { get; set; }
            public decimal CostUSD { get; set; }
            public decimal CPC { get; set; }
            public string Device { get; set; }
            public string Language { get; set; }
            public string Channel { get; set; }
        }
    }
}
