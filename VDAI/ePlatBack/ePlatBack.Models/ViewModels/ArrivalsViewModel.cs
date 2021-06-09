using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class ArrivalsViewModel
    {
        [ScriptIgnore]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public ArrivalInfoView ArrivalForm { get; set; }
        public SearchArrivalsForecast SearchForecast { get; set; }
        public SearchPenetrationReport SearchPenetration { get; set; }
        public SearchGlobalPenetrationReport SearchGlobalPenetration { get; set; }
        public SearchInArrivals SearchArrivals { get; set; }
        public SearchInBinnacle SearchBinnacle { get; set; }

        public class ArrivalsResponse {
            public List<ArrivalInfoModel> Arrivals { get; set; }
            public List<ArrivalsSummary> Summary { get; set; }
            public List<PrearrivalInfo> Prearrivals { get; set; }
            public List<TeamPowerLine> PowerLine { get; set; }
            public List<SurveyInfo> Surveys { get; set; }
            public bool IsPast { get; set; }
        }

        public class SurveyInfo
        {
            public string Key { get; set; }
            public DateTime? SentDate { get; set; }
            public DateTime? SubmittedDate { get; set; }
            public double? Rate { get; set; }
            public DateTime? ArrivalDate { get; set; }
            public string SurveyRoomNumber { get; set; }
            public string Email { get; set; }

            public IEnumerable<RawAnswer> RawAnswers { get; set; }
            public List<SurveyAnswer> Answers { get; set; }
        }

        public class RawAnswer
        {
            public int QuestionID { get; set; }
            public string Answer { get; set; }
        }

        public class SurveyAnswer
        {
            public string Description { get; set; }
            public int Order { get; set; }
            public int? FieldSubTypeID { get; set; }
            public string Answer { get; set; }
        }

        public class TeamPowerLine
        {
            public int PromotionTeamID { get; set; }
            public string Program { get; set; }
            public int Total { get; set; }
            public int Rows { get; set; }
            public int Bookings { get; set; }
            public decimal BookingPercentage { get; set; }
            public List<BookingStatusTotals> BSTotals { get; set; }
            public List<PromotorPowerLine> Promotors { get; set; }
            public List<QualificationStatusPowerLine> QualificationStatuses { get; set; }
        }

        public class PromotorPowerLine
        {
            public long OPCID { get; set; }
            public string OPCName { get; set; }
            public int Total { get; set; }
            public decimal Percentage { get; set; }
            public int Rows { get; set; }
            public int Bookings { get; set; }
            public decimal BookingPercentage { get; set; }
            public List<BookingStatusTotals> BSTotals { get; set; }
            public List<QualificationStatusPowerLine> QualificationStatuses { get; set; }
        }

        public class QualificationStatusPowerLine
        {
            public string Qualification { get; set; }
            public int Total { get; set; }
            public decimal Percentage { get; set; }
            public int Rows { get; set; }
            public int Bookings { get; set; }
            public decimal BookingPercentage { get; set; }
            public List<BookingStatusTotals> BSTotals { get; set; }
            public List<GuestBookingStatus> Guests { get; set; }
        }

        public class GuestBookingStatus
        {
            public Guid ArrivalID { get; set; }
            public string Guest { get; set; }
            public List<BookingStatusBit> BookingStatus { get; set; }
        }

        public class BookingStatusBit
        {
            public int BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public string BookingStatusCode { get; set; }
            public bool Selected { get; set; }
        }

        public class BookingStatusTotals
        {
            public int BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public string BookingStatusCode { get; set; }
            public int Amount { get; set; }
            public decimal Percentage { get; set; }
        }

        public class SearchPenetrationReport
        {
            [Required(ErrorMessage = "Date range is required")]
            [Display(Name = "Dates Range")]
            public string SearchPenetration_I_ArrivalDate { get; set; }
            [Required]
            public string SearchPenetration_F_ArrivalDate { get; set; }
            [Display(Name = "Program")]
            public int?[] SearchPenetration_ProgramID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Programs
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPrograms(null, true);
                }
            }
        }

        public class SearchGlobalPenetrationReport
        {
            [Required(ErrorMessage = "Date range is required")]
            [Display(Name = "Dates Range")]
            public string SearchGlobalPenetration_I_ArrivalDate { get; set; }
            [Required]
            public string SearchGlobalPenetration_F_ArrivalDate { get; set; }
            [Display(Name = "Program")]
            public int?[] SearchGlobalPenetration_ProgramID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Programs
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPrograms(null, true);
                }
            }
        }

        public class SearchArrivalsForecast
        {
            [Required(ErrorMessage = "Date range is required")]
            [Display(Name = "Dates Range")]
            public string SearchForecast_I_ArrivalDate { get; set; }
            [Required]
            public string SearchForecast_F_ArrivalDate { get; set; }
        }

        public class SearchInArrivals
        {
            [Display(Name = "Program")]
            public int? SearchInArrivals_ProgramID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Programs
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPrograms("Any");
                }
            }
            [Display(Name = "Reservation Status")]
            public string SearchInArrivals_ReservationStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> ReservationStatuses
            {
                get
                {
                    return ArrivalsDataModel.GetReservationStatus();
                }
            }
            [Display(Name = "Bookings Status")]
            public int? SearchInArrivals_BookingStatusID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> BookingStatuses
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpBookingStatusByWorkGroup("Any");
                }
            }

            [Display(Name = "Search by Column")]
            public string SearchInArrivals_Column { get; set; }

            public string SearchInArrivals_ColumnValue { get; set; }
        }

        public class SearchInBinnacle
        {
            [Display(Name = "Show Program")]
            public int? SearchInBinnacle_ProgramID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Programs
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPrograms("Any");
                }
            }
        }

        public class ArrivalInfoModel
        {
            public Guid ArrivalID { get; set; }

            public Guid? ExtensionArrivalID { get; set; }
            public Guid? LeadID { get; set; }

            public string Picture { get; set; }

            //Front Office Information
            public long FrontOfficeReservationID { get; set; }
            public int FrontOfficeResortID { get; set; }
            public long? FrontOfficeGuestID { get; set; }
            public DateTime ArrivalDateOnly { get; set; }
            public string ArrivalDate { get; set; }
            public string RoomNumber { get; set; }
            public int Nights { get; set; }
            public string Guest { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public int? CountryID { get; set; }
            public string RealCountry { get; set; }
            public string AgencyName { get; set; }
            public string AgencyCode { get; set; }
            public string Source { get; set; }
            public string MarketCode { get; set; }
            public string ReservationStatus { get; set; }
            public string RoomType { get; set; }
            public string ConfirmationNumber { get; set; }
            public string Crs { get; set; }
            public string DateLastUpdate { get; set; }

            //Prearrival Information
            public Guid PreArrivalReservationID { get; set; }
            public string PreArrivalStatus { get; set; }
            public string PreArrivalOptionsTotal { get; set; }
            public string PreArrivalFeedBack { get; set; }

            public List<Interaction> Interactions { get; set; }

            public List<FlightInfo> FlightsInfo {get;set;}

            public List<KeyValuePair<string, string>> ConfirmationLetters { get; set; }

            //Hostess Information
            public int? GuestTypeID { get; set; }
            public int? TravelSourceID { get; set; }
            public int? HostessQualificationStatusID { get; set; }
            public string HostessQualificationStatus { get; set; }
            public string Nationality { get; set; }
            public int? NQReasonID { get; set; }
            public string NQReason { get; set; }
            public int? ProgramID { get; set; }
            public string Program { get; set; }
            public int? PromotionTeamID { get; set; }
            public long? OPCID { get; set; }
            public string OPCName { get; set; }
            public int? BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public string HostessName { get; set; }
            public string HostessInputDateTime { get; set; }
            public string VipCardType { get; set; }
            public string VipCardStatus { get; set; }
            public string VipCardStatusAgent { get; set; }

            public string Comments { get; set; }
            public string PresentationDate { get; set; }
            public long? SalesRoomPartyID { get; set; }
            public string SalesRoom { get; set; }
            public string Party { get; set; }
            public string Gifting { get; set; }
            public decimal? Deposit { get; set; }
            public int? DepositCurrencyID { get; set; }
            public string DepositCurrency { get; set; }
            public string InvitationNumber { get; set; }
            public Guid? InvitationID { get; set; }
            public int? PresentationPax { get; set; }
            public bool? ShowStatus { get; set; }
            public string SalesStatus { get; set; }
            public decimal? SalesVolume { get; set; }
            public long? SPICustomerID { get; set; }
            public long? SPIRelatedCustomerID { get; set; }
            public int? CheckInQualificationStatusID { get; set; }
            public string CheckInQualificationStatus { get; set; }
            public int? CheckInTourStatusID { get; set; }
            public bool ManifestedAsPA { get; set; }
            public string PickUpTimeHour { get; set; }
            public string PickUpTimeMinute { get; set; }
            public string PickUpTimeMeridian { get; set; }
            public string GuestEmail { get; set; }
            public bool? Confirmed { get; set; }
            public string InvitationGenerated { get; set; }
            public string PreCheckIn { get; set; }
            public string ExtensionReservation { get; set; }
            public string CheckinDateTime { get; set; }
            public string Contract { get; set; }
            public string PlanType { get; set; }
            public bool? PrintedLetterOnHand { get; set; }

            //Guest Info
            public Guid? GuestHubID { get; set; }
            public string Email1 { get; set; }
            public string Email2 { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public int? StateID { get; set; }
            public string ZipCode { get; set; }


            //PRECHECKIN
            //Prearrival
            public string PCIPrearrivalStatus { get; set; }
            public string PCILastInteractionAgent { get; set; }
            public string PCILastInteractionDate { get; set; }
            //VipCard
            public string PCIVipCardType { get; set; }
            public string PCIVipCardStatus { get; set; }
            public string PCIVipCardStatusAgent { get; set; }
            //PreCheckin
            public List<GuestCompanion> Companions { get; set; }
            public string CompanionsJson { get; set; }
            public string PCIConciergeComments { get; set; }
            public bool PCICompleted { get; set; }

            //Guest Info Preferences
            public string Preferences { get; set; }

            //Member Info
            public string ClubType { get; set; }
            public string AccountNumber { get; set; }
            public string ContractNumber { get; set; }
            public string CoOwner { get; set; }

            List<TourInfo> ToursHistory { get; set; }

            //Clarabridge
            public string SentDate { get; set; }
            public string SubmittedDate { get; set; }
            public string Rate { get; set; }
            public DateTime? SurveyArrivalDate { get; set; }
            public string SurveyRoomNumber { get; set; }
            public List<SurveyAnswer> Answers { get; set; }
        }

        public class TourInfo
        {
            public DateTime TourDate { get; set; }
            public string SalesCenter { get; set; }
            public string TourSource { get; set; }
            public string SourceGroup { get; set; }
            public string SourceItem { get; set; }
            public string Qualification { get; set; }
            public string TourContractNumber { get; set; }
            public string Volume { get; set; }
            public List<GuestCompanion> LegalNames { get; set; }
        }

        public class Interaction
        {
            public string Type { get; set; }
            public string Status { get; set; }
            public string Comments { get; set; }
            public string User { get; set; }
            public string Date { get; set; }
        }

        public class FlightInfo
        {
            public string FlightType { get; set; }
            public string DateTime { get; set; }
            public string Airline { get;set; }
            public string FlightNumber { get; set; }
            public int NumberOfPassengers { get; set; }
        }

        public class ArrivalInfoView
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges { get; set; }
            public Guid ArrivalID { get; set; }
            [Display(Name = "Manifested as Prearrival")]
            public Guid? LeadID { get; set; }
            [Display(Name = "Reservation ID")]
            public long FrontOfficeReservationID { get; set; }
            public int FrontOfficeResortID { get; set; }
            [Display(Name = "Arrival Date")]
            public string ArrivalDate { get; set; }
            [Display(Name = "Room Number")]
            public string RoomNumber { get; set; }
            [Display(Name = "Nights")]
            public int Nights { get; set; }
            public string Guest { get; set; }
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Display(Name = "Adults")]
            public int Adults { get; set; }
            [Display(Name = "Children")]
            public int Children { get; set; }
            [Display(Name = "Infants")]
            public int Infants { get; set; }
            [Display(Name = "Country")]
            public string Country { get; set; }
            public string CountryCode { get; set; }
            [Display(Name = "Country")]
            public int? CountryID { get; set; }
            public string RealCountry { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Countries
            {
                get { 
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCountries();
                }
            }
            [Display(Name = "Agency")]
            public string AgencyName { get; set; }
            public string AgencyCode { get; set; }
            [Display(Name = "Market Code")]
            public string Source { get; set; }
            public string MarketCode { get; set; }
            [Display(Name = "Reservation Status")]
            public string ReservationStatus { get; set; }
            [Display(Name = "Room Type")]
            public string RoomType { get; set; }
            [Display(Name = "Confirmation Number")]
            public string ConfirmationNumber { get; set; }
            [Display(Name = "CRS")]
            public string Crs { get; set; }
            [Display(Name = "Check In Time")]
            public string CheckinDateTime { get; set; }
            [Display(Name = "Member Account")]
            public string Contract { get; set; }
            [Display(Name = "Plan Type")]
            public string PlanType { get; set; }
            [Display(Name = "Front Office Last Update")]
            public string DateLastUpdate { get; set; }
            [Display(Name = "Guest Type")]
            public int? GuestTypeID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> GuestTypes {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpGuestTypes();
                }
            }
            [Display(Name = "Check In Type")]
            public int? TravelSourceID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> TravelSources
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Hostess Qualification")]
            public int? HostessQualificationStatusID { get; set; }
            public string HostessQualificationStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> QualificationStatuses {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpQualificationStatus();
                }
            }
            [Display(Name = "Nationality")]
            public string Nationality { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Nationalities {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "NAC",
                        Text = "National"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "INT",
                        Text = "International"
                    });
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            [Display(Name = "PreCheckIn")]
            public string PreCheckIn { get; set; }
            [Display(Name = "Extension")]
            public string ExtensionReservation { get; set; }

            //guest info
            [Display(Name = "Email 1")]
            public string Email1 { get; set; }
            [Display(Name = "Email 2")]
            public string Email2 { get; set; }
            [Display(Name = "Phone 1")]
            public string Phone1 { get; set; }
            [Display(Name = "Phone 2")]
            public string Phone2 { get; set; }
            [Display(Name = "City")]
            public string City { get; set; }
            [Display(Name = "State")]
            public int? StateID { get; set; }
            public string State { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> States
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

            //PRECHECKIN
            //Prearrival
            [Display(Name = "Prearrival Status")]
            public string PCIPrearrivalStatus { get; set; }
            [Display(Name = "Last Interaction Agent")]
            public string PCILastInteractionAgent { get; set; }
            [Display(Name = "Last Interaction Date")]
            public string PCILastInteractionDate { get; set; }
            //VipCard
            [Display(Name = "Card Type")]
            public string PCIVipCardType { get; set; }
            [Display(Name = "Card Status")]
            public string PCIVipCardStatus { get; set; }
            [Display(Name = "Card Status Agent")]
            public string PCIVipCardStatusAgent { get; set; }
            //PreCheckin
            [Display(Name = "Guest Companion")]
            public List<GuestCompanion> Companions { get; set; }
            [Display(Name = "Pre-Check In Comments")]
            public string PCIConciergeComments { get; set; }
            [Display(Name = "Pre-Check In Completed")]
            public bool PCICompleted { get; set; }

            //guest preferences
            public string Preferences { get; set; }
            public List<GuestViewModel.Preferences.PreferenceType> PreferenceTypes
            {
                get
                {
                    return GuestDataModel.GetPreferencesList();
                }
            }

            //member info
            [Display(Name = "Club Type")]
            public string ClubType { get; set; }
            [Display(Name = "Account Number")]
            public string AccountNumber { get; set; }
            [Display(Name = "Contract Number")]
            public string ContractNumber { get; set; }
            [Display(Name = "Co Owner")]
            public string CoOwner { get; set; }
            public TourInfo TourInfoForm { get; set; }


            //prearrival info
            public Guid PreArrivalReservationID { get; set; }
            [Display(Name = "Status")]
            public string PreArrivalStatus { get; set; }
            [Display(Name = "Options Total")]
            public string PreArrivalOptionsTotal { get; set; }
            [Display(Name = "Feedback")]
            public string PreArrivalFeedBack { get; set; }
            [Display(Name = "Printed Letter On Hand")]
            public bool? PrintedLetterOnHand { get; set; }

            public List<Interaction> Interactions { get; set; }

            public List<FlightInfo> FlightsInfo { get; set; }

            //hostess info
            [Display(Name = "NQ Reason")]
            public int? NQReasonID { get; set; }
            public string NQReason { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> NQReasons {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Marketing Program")]
            public int? ProgramID { get; set; }
            public string Program { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Programs {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPrograms();
                }
            }
            [Display(Name = "Team")]
            public int? PromotionTeamID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> PromotionTeams {
                get
                {
                    //return CatalogsDataModel.PromotionTeams.PromotionTeamsCatalogs.FillDrpPromotionTeamsByTerminal();
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Promoter")]
            public long? OPCID { get; set; }
            public string OPCName { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> OPCs {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Booking Status")]
            public int? BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> BookingStatuses
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpBookingStatusByWorkGroup();
                }
            }
            [Display(Name = "Concierge")]
            public string HostessName { get; set; }
            [Display(Name = "Welcome Time")]
            public string HostessInputDateTime { get; set; }
            [Display(Name = "Concierge Comments")]
            public string Comments { get; set; }
            [Display(Name = "Presentation Date")]
            public string PresentationDate { get; set; }
            [Display(Name = "Party")]
            public long? SalesRoomPartyID { get; set; }
            public string SalesRoom { get; set; } 
            public string Party { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> SalesRoomsParties
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [Display(Name = "Gifting")]
            public string Gifting { get; set; }
            [Display(Name = "Deposit")]
            public decimal? Deposit { get; set; }
            public int? DepositCurrencyID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesIntID();
                }
            }
            [Display(Name = "Invitation")]
            public string InvitationNumber { get; set; }
            public Guid? InvitationID { get; set; }
            [Display(Name = "Room Number")]
            public string ManifestRoomNumber { get; set; }
             [Display(Name = "Presentation PAX")]
            public int? PresentationPax { get; set; }

            public bool? ShowStatus { get; set; }
            public string SalesStatus { get; set; }
            public decimal? SalesVolume { get; set; }
            [Display(Name = "SPI Customer ID")]
            public long? SPICustomerID { get; set; }
            [Display(Name = "Customer")]
            public long? SPIRelatedCustomerID { get; set; }
            public int? CheckInQualificationStatusID { get; set; }
            public string CheckInQualificationStatus { get; set; }
            public int? CheckInTourStatusID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> TourStatuses {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "Card Type")]
            public string VipCardType { get; set; }
            public List<SelectListItem> VipCardTypes
            {
                get
                {
                    return ArrivalsDataModel.GetCardTypes();
                }
            }
            [Display(Name = "Card Status")]
            public string VipCardStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> VipCardStatuses
            {
                get
                {
                    List<SelectListItem> cardStatuses = new List<SelectListItem>();
                    cardStatuses.Add(new SelectListItem()
                    {
                        Value = "Not Activated",
                        Text = "Not Activated"
                    });
                    cardStatuses.Add(new SelectListItem()
                    {
                        Value = "Activated",
                        Text = "Activated"
                    });
                    cardStatuses.Add(new SelectListItem()
                    {
                        Value = "Canceled",
                        Text = "Canceled"
                    });
                    return cardStatuses;
                }
            }
            [Display(Name = "Card Agent")]
            public string VipCardStatusAgent { get; set; }
            public bool ManifestedAsPA { get; set; }
            [Display(Name = "Pick Up Time")]
            public string PickUpTimeHour { get; set; }
            public string PickUpTimeMinute { get; set; }
            public string PickUpTimeMeridian { get; set; }
            [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
            [Display(Name = "Guest Email")]
            public string GuestEmail { get; set; }
            [Display(Name = "Confirmation")]
            public bool? Confirmed { get; set; }
            [Display(Name = "Invitation Generated")]
            public string InvitationGenerated { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Hours
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpHours();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Minutes
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpMinutes();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Meridians
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpMeridians();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> ConfirmationStatuses
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpConfirmationStatuses();
                }
            }
            [Display(Name = "Survey Sent")]
            public DateTime? SentDate { get; set; }
            [Display(Name = "Survey Completed")]
            public DateTime? SubmittedDate { get; set; }
            [Display(Name = "Global Rate")]
            public double? Rate { get; set; }
            [Display(Name = "Room Number")]
            public string SurveyRoomNumber { get; set; }
        }

        public class ArrivalsSummary
        {
            public int? ProgramID { get; set; }
            public string Program { get; set; }
            public int Active { get; set; }
            public int CheckedIn { get; set; }
            public int CheckedOut { get; set; }
            //[ScriptIgnore]
            public List<ArrivalBookingStatus> BookingStatuses { get; set; }
        }

        public class ArrivalBookingStatus {
            public int? BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public int Amount { get; set; }
            public decimal Percentage { get; set; }
        }

        public class PrearrivalInfo
        {
            public Guid ArrivalID { get; set; }
            public DateTime? ArrivalDate { get; set; }
            public string ArrivalDateString { get; set; }
            public Guid LeadID { get; set; }
            public Guid ReservationID { get; set; }
            public string HotelConfirmationNumber { get; set; }
            public string Guest { get; set; }
            public string ClubType { get; set; }
            public string AccountNumber { get; set; }
            public string ContractNumber { get; set; }
            public string CoOwner { get; set; }
            public decimal? PreArrivalOptionsTotal { get; set; }
            public bool Located { get; set; }
            public bool PreCheckIn { get; set; }
            public int? BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public decimal? TotalPaid { get; set; }
            public string ConciergeComments { get; set; }
            public string HostessQualificationStatus { get; set; }

            public int Interactions { get; set; }
            public int Flights { get; set; }

            public string Email1 { get; set; }
            public string Email2 { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public int? CountryID { get; set; }
            public string ZipCode { get; set; }
            //Prearrival
            public string PCIPrearrivalStatus { get; set; }
            public long TerminalID { get; set; }
            public string AssignedToFirstName { get; set; }
            public string AssignedToLastName { get; set; }
            public long? FrontOfficeReservationID { get; set; }
        }

        public class GuestCompanion
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public DateTime BirthDate { get; set; }
        }

        public class ArrivalsForecast
        {
            public List<ProgramArrivalsInfo> Programs { get; set; }
            public List<ArrivalFrontInfo> Arrivals { get; set; }

            public List<string> ErrorMessages { get; set; }
        }

        public class GlobalPenetrationReport 
        {
            public string DatesRange { get; set; }
            public List<PenetrationPerDay> PenetrationPerDay { get; set; }
            public PenetrationPerDay Totals { get; set; }
            public PenetrationPerDay Booked { get; set; }
            public PenetrationPercentages PenetrationPercentages { get; set; }

            public int TotalCheckInMembers { get; set; }
            public int TotalCheckInInHouse { get; set; }
            public int TotalGlobalCheckIn { get; set; }

            public int Members { get; set; }
            public int GuestOfMembers { get; set; }
            public int Exchange { get; set; }

            public int TotalQMembers { get; set; }
            public int TotalQInHouse { get; set; }
            public int QGlobalSummary { get; set; }

            public decimal MembersQPenetration { get; set; }
            public decimal InHouseQPenetration { get; set; }
            public decimal GlobalQPenetration { get; set; }

            public List<NQReasonTotals> InHouseNQReasons { get; set; }
            public int InHouseNQTotal { get; set; }
            public int InHouseNQBooked { get; set; }

            public List<NQReasonTotals> MembersNQReasons { get; set; }
            public int MembersNQTotal { get; set; }
            public int MembersNQBooked { get; set; }
        }

        public class NQReasonTotals
        {
            public int NQReasonID { get; set; }
            public string NQReason { get; set; }
            public int NQReasonTotal { get; set; }
            public int NQReasonBooked { get; set; }
        }

        public class PenetrationPercentages
        {
            public decimal MembersQ { get; set; }
            public decimal MembersNQ { get; set; }
            public decimal MembersDoubles { get; set; }
            public decimal MembersArrivalsTotal { get; set; }
            public decimal InHouseQ { get; set; }
            public decimal InHouseNQ { get; set; }
            public decimal AirportQ { get; set; }
            public decimal InHouseDoubles { get; set; }
            public decimal NetCenterQ { get; set; }
            public decimal NetCenterNQ { get; set; }
            public decimal ReferralQ { get; set; }
            public decimal ReferralNQ { get; set; }
            public decimal InHouseAirportArrivalsTotal { get; set; }
            public decimal GlobalSummary { get; set; }
        }

        public class PenetrationPerDay
        {
            public string Date { get; set; }
            public string DayOfWeek { get; set; }
            public int MembersQ { get; set; }
            public int MembersNQ { get; set; }
            public int MembersDoubles { get; set; }
            public int MembersNotContacted { get; set; }
            public int MembersArrivalsTotal { get; set; }
            public int InHouseQ { get; set; }
            public int InHouseNQ { get; set; }
            public int AirportQ { get; set; }
            public int InHouseNotContacted { get; set; }
            public int InHouseDoubles { get; set; }
            public int NetCenterQ { get; set; }
            public int NetCenterNQ { get; set; }
            public int ReferralQ { get; set; }
            public int ReferralNQ { get; set; }
            public int InHouseAirportArrivalsTotal { get; set; }
            public int GlobalSummary { get; set; }
        }

        public class PenetrationReport
        {
            public string MonthName { get; set; }
            public string Resort { get; set; }
            public string DatesRange { get; set; }
            public List<PromoterPenetration> PromotersPenetration { get; set; }
            public List<PenetrationDimension> Totals { get; set; }
        }

        public class PromoterPenetration
        {
            public long? OpcID { get; set; }
            public string Promoter { get; set; }
            public List<PenetrationDimension> DimensionGroups { get; set; }
        }

        public class PenetrationDimension
        {
            public string Group { get; set; }
            public int Booked { get; set; }
            public int TalkedTo { get; set; }
            public int Total { get; set; }
            public decimal Percentage { get; set; }
        }

        public class ProgramArrivalsInfo
        {
            public int ProgramID { get; set; }
            public string Program { get; set; }
            public int Amount { get; set; }
            public List<CheckInTypeInfo> CheckInTypes { get; set; }
        }

        public class CheckInTypeInfo
        {
            public int? TravelSourceID { get; set; }
            public string TravelSource { get; set; }
            public int Amount { get; set; }
            public decimal Percentage { get; set; }
        }

        public class ArrivalFrontInfo
        {
            public long FrontOfficeReservationID { get; set; }
            public int FrontOfficeResortID { get; set; }
            public string ArrivalDate { get; set; }
            public string RoomNumber { get; set; }
            public int Nights { get; set; }
            public string Guest { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string Country { get; set; }
            public string AgencyName { get; set; }
            public string Source { get; set; }
            public string ReservationStatus { get; set; }
            public string RoomType { get; set; }
            public string ConfirmationNumber { get; set; }
            public string Crs { get; set; }
            public int? ProgramID { get; set; }
            public string Program { get; set; }
            public int? TravelSourceID { get; set; }
            public string TravelSource { get; set; }
            public int? HostessQualificationStatusID { get; set; }
            public int? NQReasonID { get; set; }
            public string NQReason { get; set; }
            public string Nationality { get; set; }
        }
    }
}
