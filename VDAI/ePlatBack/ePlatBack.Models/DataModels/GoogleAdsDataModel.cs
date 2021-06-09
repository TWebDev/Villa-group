using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePlatBack.Models.ViewModels;
using System.Web.Mvc;
using System.Data;
using System.IO;

using Google.Api.Ads.AdWords.Lib;
using Google.Api.Ads.AdWords.Util.Reports;
using Google.Api.Ads.AdWords.v201809;
using Google.Api.Ads.Common.Lib;
using Google.Api.Ads.Common.Util.Reports;

namespace ePlatBack.Models.DataModels
{
    public class GoogleAdsDataModel
    {
        public class Reports
        {
            public static GoogleAdsViewModel.Reports.AdsInvestmentReport GetAdsInvestments(GoogleAdsViewModel.Reports.SearchAdsInvestment model)
            {
                GoogleAdsViewModel.Reports.AdsInvestmentReport report = new GoogleAdsViewModel.Reports.AdsInvestmentReport();
                report.Accounts = new List<GoogleAdsViewModel.AccountInvestment>();

                string[] customerIDs = new string[] { };
                if (model.Search_Accounts != null)
                {
                    customerIDs = model.Search_Accounts;
                }

                //obtención de terminales
                ePlatEntities db = new ePlatEntities();
                var Terminals = from ls in db.tblLeadSourcesSettings
                                join t in db.tblTerminals on ls.terminalID equals t.terminalID
                                into ls_t
                                from t in ls_t.DefaultIfEmpty()
                                select new
                                {
                                    ls.terminalID,
                                    ls.clientCustomerID,
                                    t.terminal
                                };

                foreach (var cid in customerIDs)
                {
                    GoogleAdsViewModel.AccountInvestment newAccount = new GoogleAdsViewModel.AccountInvestment();

                    var terminal = Terminals.FirstOrDefault(x => x.clientCustomerID == cid);
                    newAccount.AccountName = cid + " - " + terminal.terminal;
                    newAccount.Campaigns = new List<GoogleAdsViewModel.CampaignInvestment>();
                    newAccount.Totals = new GoogleAdsViewModel.CampaignInvestment();

                    AdWordsUser user = new AdWordsUser();
                    (user.Config as AdWordsAppConfig).ClientCustomerId = cid;

                    DateTime fromDate = DateTime.Parse(model.Search_I_FromDate);
                    DateTime toDate = fromDate.AddDays(1);
                    if (model.Search_F_ToDate != null)
                    {
                        toDate = DateTime.Parse(model.Search_F_ToDate).AddDays(1);
                    }
                    DateTime currentDate = fromDate;

                    //obtención de los tipos de cambio aplicables en ese rango
                    var ER = (from r in db.tblExchangeRates
                             where r.terminalID == terminal.terminalID
                             && r.toDate > fromDate
                             && r.fromDate < toDate
                             select r).ToList();


                    while (currentDate < toDate)
                    {
                        ReportDefinition definition = new ReportDefinition()
                        {
                            reportName = "Campaign Performance Report",
                            reportType = ReportDefinitionReportType.CAMPAIGN_PERFORMANCE_REPORT,
                            downloadFormat = DownloadFormat.CSV,
                            dateRangeType = ReportDefinitionDateRangeType.CUSTOM_DATE,

                            selector = new Selector()
                            {
                                fields = new string[] {
                                "CampaignId",//0
                                "CampaignName",//1
                                "CampaignStatus",//2
                                "Clicks",//3
                                "Impressions",//4
                                "Cost",//5
                                "Ctr",//6
                                "Device",//7
                                "AdNetworkType1",//8
                                "AdNetworkType2"
                            },
                                predicates = new Predicate[] {
                                        Predicate.In("CampaignStatus", new string[] {
                                        "ENABLED",
                                        "PAUSED"
                                    })
                                },
                                dateRange = new DateRange()
                                {
                                    min = currentDate.ToString("yyyy-MM-dd"),
                                    max = currentDate.ToString("yyyy-MM-dd")
                                }
                            }
                        };

                        // Optional: Include zero impression rows.
                        AdWordsAppConfig config = (AdWordsAppConfig)user.Config;
                        config.IncludeZeroImpressions = true;

                        ReportUtilities utilities = new ReportUtilities(user, "v201809", definition);
                        using (ReportResponse response = utilities.GetResponse())
                        {
                            using (var reader = new StreamReader(response.Stream, Encoding.UTF8))
                            {
                                int lineIndex = 0;
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();
                                    string[] values = line.Split(',');

                                    if (lineIndex >= 2)
                                    {
                                        GoogleAdsViewModel.CampaignInvestment c = new GoogleAdsViewModel.CampaignInvestment();
                                        c.Date = currentDate;
                                        c.CampaignName = values[1];
                                        if(c.CampaignName.IndexOf(" ") >= 0)
                                        {
                                            c.Destination = c.CampaignName.Substring(0, c.CampaignName.IndexOf(" "));
                                        } else
                                        {
                                            c.Destination = "";
                                        }
                                        if(c.Destination == "Puerto")
                                        {
                                            c.Destination = "Vallarta";
                                        }
                                        c.Clicks = int.Parse(values[3]);
                                        c.Impressions = int.Parse(values[4]);
                                        string costMXN = values[5];
                                        if (costMXN.Length > 4)
                                        {
                                            costMXN = costMXN.Substring(0,costMXN.Length - 4);
                                        }
                                        c.CostMXN = decimal.Round(decimal.Parse(costMXN) / 100, 2);
                                        c.Ctr = values[6];
                                        //revisar si hay un tipo de cambio
                                        var er = (from r in ER
                                                 where r.fromDate <= currentDate
                                                 && r.toDate > currentDate
                                                 select r).FirstOrDefault();
                                        if (er != null)
                                        {
                                            c.CostUSD = decimal.Round(c.CostMXN / er.exchangeRate, 2);
                                            if(c.Clicks > 0)
                                            {
                                                c.CPC = decimal.Round(c.CostUSD / c.Clicks, 2);
                                            }                                            
                                        }
                                        else
                                        {
                                            c.CostUSD = 0;
                                            c.CPC = 0;
                                        }
                                        c.Device = values[7];
                                        if (c.Device.IndexOf("Computers") >= 0)
                                        {
                                            c.Device = "Desktop";
                                        } else
                                        {
                                            c.Device = c.Device != "" ? c.Device.Substring(0, c.Device.IndexOf(" ")) : "";
                                        }
                                        c.Language = "English";
                                        c.Channel = values[9];
                                        if(c.Channel == "Display Network")
                                        {
                                            c.Channel = "Google Display";
                                        } else if(c.Channel == "Search partners")
                                        {
                                            c.Channel = "Google Search Partners";
                                        }
                                        
                                        if (c.CostMXN > 0)
                                        {
                                            if(c.CampaignName == " --")
                                            {
                                                newAccount.Totals.Clicks += c.Clicks;
                                                newAccount.Totals.Impressions += c.Impressions;
                                                newAccount.Totals.CostMXN += c.CostMXN;
                                                newAccount.Totals.CostUSD += c.CostUSD;
                                            } else
                                            {
                                                newAccount.Campaigns.Add(c);
                                            }                                            
                                        }
                                    }
                                    lineIndex++;
                                }
                            }
                        }

                        currentDate = currentDate.AddDays(1);
                    }

                    newAccount.Campaigns = newAccount.Campaigns.OrderBy(x => x.Date).ThenBy(x => x.CampaignName).ThenByDescending(x => x.CostMXN).ToList();
                    if (newAccount.Totals.Impressions > 0)
                    {
                        newAccount.Totals.Ctr = decimal.Round((decimal)newAccount.Totals.Clicks * 100 / newAccount.Totals.Impressions, 2).ToString() + "%";
                    }
                    if (newAccount.Totals.Clicks > 0)
                    {
                        newAccount.Totals.CPC = decimal.Round(newAccount.Totals.CostUSD / newAccount.Totals.Clicks , 2);
                    }

                    report.Accounts.Add(newAccount);
                }

                return report;
            }

            public static List<SelectListItem> GetAdsAccounts()
            {
                List<SelectListItem> list = new List<SelectListItem>();
                ePlatEntities db = new ePlatEntities();
                UserSession session = new UserSession();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var MarketingSources = from s in db.tblLeadSourcesSettings
                                       join ls in db.tblLeadSources on s.leadSourceID equals ls.leadSourceID
                                       into s_ls
                                       from ls in s_ls.DefaultIfEmpty()
                                       join t in db.tblTerminals on s.terminalID equals t.terminalID
                                       into s_t
                                       from t in s_t.DefaultIfEmpty()
                                       where terminals.Contains(s.terminalID)
                                       select new
                                       {
                                           ls.leadSource,
                                           s.clientCustomerID,
                                           t.terminal
                                       };

                foreach (var source in MarketingSources)
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Value = source.clientCustomerID,
                        Text = source.terminal + " - " + source.leadSource
                    };
                    list.Add(item);
                }

                return list;
            }
        }
    }
}
