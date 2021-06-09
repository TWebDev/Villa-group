using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ePlatBack.Models.ViewModels
{
    public class FollowUpViewModel
    {
        public class LeadInfo
        {
            public Guid? LeadID { get; set; }
            public CustomerInfo CustomerInfo { get; set; }
            public ContactInfo ContactInfo { get; set; }
            public QualificationInfo QualificationInfo { get; set; }
            public bool Checked { get; set; }
        }

        public class ContactInfo
        {
            [Display(Name = "Phones")]
            public List<PhoneInfo> Phones { get; set; }
            [Display(Name = "Emails")]
            public List<EmailInfo> Emails { get; set; }
        }

        public class QualificationInfo
        {
            [Display(Name = "Qualification Status")]
            public int? QualificationStatusID { get; set; }
            public string Qualification { get; set; }
            [Display(Name = "NQ Reason")]
            public int? NQReasonID { get; set; }
        }

        public class CustomerInfo
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Display(Name = "Country")]
            public int? CountryID { get; set; }
            [Display(Name = "State")]
            public int? StateID { get; set; }
            public string State { get; set; }
            [Display(Name = "City")]
            public string City { get; set; }
            [Display(Name = "Marketing Source")]
            public long? LeadSourceID { get; set; }
            public string Source { get; set; }
            [Display(Name = "Channel")]
            public int? LeadSourceChannelID { get; set; }
            public string Channel { get; set; }
            [Display(Name = "First Contact Type")]
            public int? LeadTypeID { get; set; }
            public string FirstContactType { get; set; }
            [Display(Name = "Lead Status")]
            public int? LeadStatusID { get; set; }
            public string LeadStatus { get; set; }
            [Display(Name = "Interested in Destination")]
            public long? InterestedInDestinationID { get; set; }
            public string InterestedInDestination { get; set; }
            [Display(Name = "Destination")]
            public long? DestinationID { get; set; }
            public string Destination { get; set; }
            [Display(Name = "Input")]
            public int InputMethodID { get; set; }
            public string InputMethod { get; set; }
            [Display(Name = "Booking Status")]
            public int? BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            [Display(Name = "Interest Level")]
            public int? InterestLevelID { get; set; }
            public string InterestLevel { get; set; }
            [Display(Name = "Date Registered")]
            public DateTime DateSaved { get; set; }
            [Display(Name = "Assigned to")]
            public Guid? AssignedToUserID { get; set; }
            public string AssignedToAgent { get; set; }
            [Display(Name = "Mood")]
            public int? Mood { get; set; }
        }

        public class PhoneInfo
        {
            public long? PhoneID { get; set; }
            [Display(Name = "Phone Type")]
            public int PhoneTypeID { get; set; }
            [Display(Name = "Phone")]
            public string Phone { get; set; }
            [Display(Name = "Extension")]
            public string Ext { get; set; }
            [Display(Name = "Do Not call")]
            public bool DoNotCall { get; set; }
            [Display(Name = "Is Main")]
            public bool Main { get; set; }
            public bool Editing { get; set; }
        }

        public class EmailInfo {
            public long? EmailID { get; set; }
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Display(Name = "Is Main")]
            public bool Main { get; set; }
            public bool Editing { get; set; }
        }

        public class LogItem {
            public long? CallLogID { get; set; }
            public Guid? MailMessageID { get; set; }
            public long? InteractionID { get;set; }
            public string InteractionType { get; set; }
            public int InteractionTypeID { get; set; }
            public string CallDisposition { get; set; }
            public string CallDuration { get; set; }
            public string CallLabel { get; set; }
            public string CallExtension { get; set; }
            public string EmailSubject { get; set; }
            public string Agent { get; set; }
            public DateTime? Date { get; set; }
            public Guid? LeadID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public long? EmailID { get; set; }
            public string Email { get; set; }
            public string QualificationStatus { get; set; }
            public int? QualificationStatusID { get; set; }
            public List<SelectListItem> QualificationStatusList { get; set; }
            public string InterestedInDestination { get; set; }
            public long? InterestedInDestinationID { get; set; }
            public List<SelectListItem> DestinationsList { get; set; }
            public decimal? Total { get; set; }
            public string CurrencyCode { get; set; }
            public int? CurrencyID { get; set; }
            public List<SelectListItem> CurrenciesList { get; set; }
            public bool? FirstContact { get; set; }
            public string InteractionComments { get; set; }
            public string InterestLevel { get; set; }
            public int? InterestLevelID { get; set; }
            public List<SelectListItem> InsterestLevelsList { get; set; }
            public string BookingStatus { get; set; }
            public int? BookingStatusID { get; set; }
            public List<SelectListItem> BookingStatusList { get; set; }
            public string DiscardReason { get; set; }
            public int? DiscardReasonID { get; set; }
            public List<SelectListItem> DiscardReasonsList { get; set; }
            public long TerminalID { get; set; }
        }

        public class SearchFilters
        {
            [Display(Name = "From Date")]
            public DateTime? Search_FromDate { get; set; }
            [Display(Name = "To Date")]
            public DateTime? Search_ToDate { get; set; }
            [Display(Name = "Guest Name")]
            public string Search_GuestName { get; set; }
            [Display(Name = "Lead Status")]
            public int?[] Search_LeadStatusID { get; set; }
            [Display(Name = "Booking Status")]
            public int?[] Search_BookingStatusID { get; set; }
            [Display(Name = "First Contact Type")]
            public int?[] Search_LeadTypeID { get; set; }
            [Display(Name = "Marketing Channel")]
            public int?[] Search_LeadSourceChannelID { get; set; }
            [Display(Name = "Assigned to")]
            public Guid? Search_AssignedToUserID { get; set; }
        }

        public class SearchLeadsResponse
        {
            public List<LeadInfo> Leads { get; set; }
            public LeadsSummary Summary { get; set; }
        }

        public class LeadsSummary
        {

        }

        public class SummaryCounter
        {
            public string Field { get; set; }
            public int Value { get; set; }
            public string Counter { get; set; }
            public int Amount { get; set; }
        }

        public class PhoneAnalysis
        {
            public Guid? LeadID { get; set; }
            //agregar ubicación
            public int? CountryID { get; set; }
            public int? StateID { get; set; }
            public string State { get; set; }
            public string City { get; set; }
        }
    }
}
