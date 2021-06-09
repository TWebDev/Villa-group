using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.ViewModels
{
    public class APIReferralsViewModel
    {
        public class ReferralReservation
        {
            public int ReservationID { get; set; }
            public string LastModificationDate { get; set; }
            public string Terminal { get; set; }
            public string SalesDate { get; set; }
            public string CertNumber { get; set; }
            public string SalesAgent { get; set; }
            public string ToCloser { get; set; }
            public decimal? PackagePrice { get; set; }
            public int? PackageNights { get; set; }
            public int? PackageAdults { get; set; }
            public string Destination { get; set; }
            public string ArrivalDate { get; set; }
            public string DepartureDate { get; set; }
            public string GuestFirstName { get; set; }
            public string GuestLastName { get; set; }
            public string MarketingSource { get; set; }
            public string LeadType { get; set; }
            public string CertificateComments { get; set; }
            public string ReservationStatus { get; set; }
            public string ConfirmationDate { get; set; }
            public string LeadGenerator { get; set; }
            public string TimeshareOffer { get; set; }
            public string ReservationAgent { get; set; }
            public string ReservationCallStatus { get; set; }
            public string CancelationDate { get; set; }
            public string CancelationReason { get; set; }
            public string UnitType { get; set; }
            public string HotelConfirmation { get; set; }
            public string Resort { get; set; }
            public List<ReferralFlightInfo> Flights { get; set; }
            public string SpecialRequestComments { get; set; }
            public string PlanType { get; set; }
            public string VerificationAgent { get; set; }
            public int? NumberOfChildren { get; set; }
            public int? TotalNights { get; set; }

        }
        
        public class ReferralFlightInfo
        {
            public string FlightDateTime { get; set; }
        }

        public class Rewards
        {
            public int CurrentBalance { get; set; }
            public int AssignedCredits { get; set; }
            public int RedeemCredits { get; set; }
            public CustomerDetails CustomerDetails { get; set; }
            public List<ReferralInfo> Referrals { get; set; }
            public List<History> History { get; set; }
        }
        public class CustomerDetails
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool TermsAndConditions { get; set; }
            public long? ResortID { get; set; }
            public Guid ID { get; set; }
            public string ProfilePicturePath { get; set; }
            public bool IsTest { get; set; }
            public string Membership { get; set; }
        }
        public class ListTable
        {
            public int TotalActivePoints { get; set; }
            public List<History> HistoryPoints { get; set; }
        }

        public class History
        {
            public int ActivePoints { get; set; }
            public Guid? RedemptionID { get; set; }
            public int StatusCode { get; set; }
            public int? Balance { get; set; }
            public int ExpiratedPoints { get; set; }
            public long ResortID { get; set; }
            public int Points { get; set; }
            public string Concept { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public int TransactionTypeID { get; set; }
            public DateTime GeneratedDate { get; set; }
            public string Resort { get; set; }
        }

        public class ExpiredCredits
        {
            public long? PlaceID { get; set; }
            public Guid? ReferralID { get; set; }
            public int Points { get; set; }
            public string Concept { get; set; }
            public int TransactionTypeID { get; set; }
            public DateTime? GeneratedDate { get; set; }
        }

        public class ReferralInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ID { get; set; }
            public DateTime InputDate { get; set; }
            public string Status { get; set; }
            public string SRCImg { get; set; }
            public IEnumerable<CreditsInfo> CreditsGenerated { get; set; }
        }

        public class CreditsInfo
        {
            public int Credits { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public int TransactionTypeID { get; set; }
        }

        public class RedeemCredits
        {
            public string MemberAccount { get; set; }
            public int TransactionTypeID { get; set; }
            public string Concept { get; set; }
            public int Amount { get; set; }
            public string Reference { get; set; }
            public long ResortID { get; set; }
        }

        public class NewReferral
        {
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Phone { get; set; }
            [Required]
            public string Relationship { get; set; }
            [Required]
            public string ReferredBy { get; set; }
            public long TerminalID { get; set; }
            public int SourceChannelID { get; set; }
            public int SourceID { get; set; }
            public Guid PreLeadID { get; set; }
        }

        public class UpdateReferral
        {
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            [Required]
            public string Relationship { get; set; }
            [Required]
            public string ReferredByMemberAccount { get; set; }
            [Required]
            public Guid ID { get; set; }
        }

        public class RedemptionInfo
        {
            [Required]
            public int Credits { get; set; }
            [Required]
            public string Concept { get; set; }
            public DateTime? AppliedDate { get; set; }
            [Required]
            public string MemberAccount { get; set; }
            [Required]
            public long? ResortID { get; set; }
        }
        public class ReferralInvitation
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int NotificationID { get; set; }
        }

        public class EmailTemplate
        {
            public string Alias { get; set; }
            public string Content { get; set; }
            public string Sender { get; set; }
            public string Subject { get; set; }
            public int EmailNotificationID { get; set; }
            public long TerminalID { get; set; }
            public int EventID { get; set; }
            public string ReplyTo { get; set; }
        }
        public class OwnerDetails
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public long EmailID { get; set; }
            public bool IsTest { get; set; }
            public Guid LeadID { get; set; }
            public string MemberAccount { get; set; }
            public long TerminalID { get; set; }
        }
    }
}
