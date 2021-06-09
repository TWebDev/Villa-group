using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.ViewModels
{
    public class TrialsViewModel
    {
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public SearchTrial Search { get; set; }
        public Trial Info { get; set; }
        public string AgentsList { get; set; }
        public string BookingStatusList { get; set; }

        public class SearchTrial
        {
            [Display(Name = "Contract Number")]
            public string Search_ContractNumber { get; set; }
            [Display(Name = "Booking Status")]
            public int?[] Search_BookingStatusID { get; set; }
            public List<SelectListItem> Search_BookingStatus { get; set; }
            [Display(Name = "Assigned to")]
            public Guid?[] Search_AssignedToUserID { get; set; }
            public List<SelectListItem> Search_Agents { get; set; }
        }

        public class Trial
        {
            public Guid? TrialID { get; set; }
            [Display(Name = "Contract Number")]
            public string ContractNumber { get; set; }
            [Display(Name = "Xref No.")]
            public string Reference { get; set; }
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Display(Name = "Sales Type")]
            public string SalesType { get; set; }
            [Display(Name = "Contract Status")]
            public string ContractStatus { get; set; }
            [Display(Name = "Source of Sale")]
            public string SourceOfSale { get; set; }
            [Display(Name = "Date Entered")]
            public DateTime DateInput { get; set; }
            [Display(Name = "Sales Date")]
            public DateTime DateSale { get; set; }
            [Display(Name = "Volume")]
            public decimal? Volume { get; set; }
            [Display(Name = "Expiration Date")]
            public DateTime? DateExpiration { get; set; }
            [Display(Name = "Assigned To")]
            public Guid? AssignedToUserID { get; set; }
            public string AssignedTo { get; set; }
            public List<SelectListItem> AgentsList { get; set; }
            [Display(Name = "Main Phone")]

            public string Phone1 { get; set; }
            [Display(Name = "Alternate Phone")]
            public string Phone2 { get; set; }
            [Display(Name = "Main Email")]
            public string Email1 { get; set; }
            [Display(Name = "Secondary Email")]
            public string Email2 { get; set; }
            public long? TerminalID { get; set; }
            [Display(Name = "Booking Status")]
            public int? BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public List<SelectListItem> BookingStatusList { get; set; }
            public Object _cellVariants { get; set; }
        }
    }
}
