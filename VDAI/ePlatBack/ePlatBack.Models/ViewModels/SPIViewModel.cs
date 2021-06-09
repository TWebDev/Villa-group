using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;


namespace ePlatBack.Models.ViewModels
{
    public class SPIViewModel
    {
        public class CustomerHistory
        {
            public int CustomerID { get; set; }
            public List<CustomerTour> History { get; set; }
        }

        public class CustomerTour
        {
            public string TourDate { get; set; }
            public string SalesCenter { get; set; }
            public string TourSource { get; set; }
            public string SourceGroup { get; set; }
            public string SourceItem { get; set; }
            public string Qualification { get; set; }
            public string TourContractNumber { get; set; }
            public decimal? Volume { get; set; }
            public string ContractStatus { get; set; }
            public List<LegalName> LegalNames { get; set; }
        }

        public class CustomerHistoryResults
        {
            public List<CustomerHistoryItem> History { get; set; }
            public List<LegalName> LegalNames { get; set; }
        }

        public class CustomerSearchResult
        {
            public int CustomerID { get; set; }
            public string Customer { get; set; }
            public string AccountNumber { get; set; }
        }

        public class CustomerHistoryItem
        {
            public DateTime TourDateTime { get; set; }
            public string TourDate { get; set; }
            public string SalesCenter { get; set; }
            public string TourSource { get; set; }
            public string SourceGroup { get; set; }
            public string SourceItem { get; set; }
            public string Qualification { get; set; }
            public string TourContractNumber { get; set; }
            public decimal? Volume { get; set; }

            public int CustomerID { get; set; }
            public string AccountNumber { get; set; }
            public string CustomerName { get; set; }
            public bool LastTour { get; set; }
            public bool LastContract { get; set; }
            public string LegalNames { get; set; }
        }

        public class LegalName {
            public string Name { get; set; }
            public string DateOfBirth { get; set; }
            public int Age { get; set; }
        }

        public class AgencyManifest
        {
            public string ManifestSearch_Date { get; set; }
            public List<AgencyCustomer> list { get; set; }
        }

        public class AgencyCustomer
        {
            public string DT_RowId { get; set; }
            public int? CustomerID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Country { get; set; }
            public string MarketingProgram { get; set; }
            public string Subdivision { get; set; }
            public string Source { get; set; }
            public int? OPCID { get; set; }
            public string OPC { get; set; }
            public int? FrontOfficeGuestID { get; set; }
            public int? FrontOfficeResortID { get; set; }
            public int? TourID { get; set; }
            public string TourDate { get; set; }
        }

        public class VLOCustomer
        {
            public string VPANumber { get; set; }
            public string Title { get; set; }
            public string[] Titles
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpPersonalTitles();
                    return list.Select(m => m.Text).ToArray();
                }
            }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<string> Email { get; set; }
            public List<string> EmailString { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public string ContractStatus { get; set; }
            public string Country { get; set; }
            public int? VLOID { get; set; }
            public string VLO { get; set; }
            public List<SelectListItem> VLOS {get;set;}
            public Guid? VLOUserID { get; set; }
            public string SalesCenter { get; set; }
            public string Culture { get; set; }
            public string ActivationDate { get; set; }
            public string CollectDate { get; set; }
            public string PD { get; set; }
            public int? CustomerID { get; set; }
            public int? TourID { get; set; }
            public long Letter { get; set; }
            public List<SelectListItem> Letters {get;set;}
            public bool? SendStatus { get; set; }
            public string SendStatusString { get; set; }
        }


    }
}
