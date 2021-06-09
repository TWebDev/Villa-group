using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class PreArrivalViewModel
    {
        public PreArrivalSearchModel PreArrivalSearchModel { get; set; }
        public List<PreArrivalSearchResultsModel> SearchResults { get; set; }
        public PreArrivalInfoModel PreArrivalInfoModel { get; set; }

        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
    }

    public class PreArrivalSearchResultsModel
    {
        public List<List<KeyValuePair<string, string>>> Results { get; set; }
    }

    public class PreArrivalReponse
    {
        public List<PreArrivalEmailsInfoModel> Emails { get; set; }
        public List<PreArrivalPhonesInfoModel> Phones { get; set; }
        public List<PreArrivalInteractionsInfoModel> Interactions { get; set; }
        public PreArrivalMemberInfoModel MemberInfo { get; set; }
    }

    public class PreArrivalInfoModel
    {
        [DoNotTrackChanges]
        [ScriptIgnore]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        [DoNotTrackChanges]
        public bool Info_DuplicateLead { get; set; }
        [LogReference]
        public Guid Info_LeadID { get; set; }
        [DoNotTrackChanges]
        public List<PreArrivalEmailsInfoModel> ListPreArrivalEmails { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string Info_FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string Info_LastName { get; set; }

        [Required(ErrorMessage = "Terminal is required")]
        [Display(Name = "Terminal")]
        public long Info_Terminal { get; set; }

        [Required(ErrorMessage = "Lead Status is required")]
        [Display(Name = "Lead Status")]
        public int Info_LeadStatus { get; set; }

        [Display(Name = "Lead Status Description")]
        public string Info_LeadStatusDescription { get; set; }

        [Display(Name = "Call Clasification")]
        public int? Info_CallClasification { get; set; }

        [Required(ErrorMessage = "Lead Source is required")]
        [Display(Name = "Lead Source")]
        public long Info_LeadSource { get; set; }

        //[Display(Name = "Pre Booking Status")]
        //public int? Info_SecondaryBookingStatus { get; set; }

        [Display(Name = "Time Zone")]
        public int Info_TimeZone { get; set; }

        [Display(Name = "Address")]
        public string Info_Address { get; set; }

        [Display(Name = "City")]
        public string Info_City { get; set; }

        [Display(Name = "State")]
        public string Info_State { get; set; }

        [Display(Name = "Zip Code")]
        public string Info_ZipCode { get; set; }

        [Display(Name = "Country")]
        public int Info_Country { get; set; }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public PreArrivalEmailsInfoModel PreArrivalEmailsInfoModel { get; set; }

        [Display(Name = "Lead Comments")]
        public string Info_LeadComments { get; set; }

        [Display(Name = "Confirmed")]
        public bool? Info_Confirmed { get; set; }

        [Display(Name = "Submission Form")]
        public bool? Info_SubmissionForm { get; set; }

        [Display(Name = "Activity Cert")]
        public bool? Info_ActivityCert { get; set; }

        [Display(Name = "Options Tour Discount")]
        public bool? Info_OptionsTourDiscount { get; set; }

        //public List<PreArrivalEmailsInfoModel> ListPreArrivalEmails { get; set; }
        [DoNotTrackChanges]
        public string PreArrivalEmails { get; set; }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public PreArrivalPhonesInfoModel PreArrivalPhonesInfoModel { get; set; }
        [DoNotTrackChanges]
        public List<PreArrivalPhonesInfoModel> ListPreArrivalPhones { get; set; }
        [DoNotTrackChanges]
        public string PreArrivalPhones { get; set; }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public PreArrivalMemberInfoModel PreArrivalMemberInfoModel { get; set; }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public PreArrivalBillingModel PreArrivalBillingModel { get; set; }
        [DoNotTrackChanges]
        public List<BillingResultsModel> ListPreArrivalBillings { get; set; }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public PreArrivalInteractionsInfoModel PreArrivalInteractionsInfoModel { get; set; }
        [DoNotTrackChanges]
        public List<PreArrivalInteractionsInfoModel> PreArrivalInteractions { get; set; }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public PreArrivalReservationsModel PreArrivalReservationsModel { get; set; }
        [DoNotTrackChanges]
        public List<ReservationResultsModel> ListPreArrivalReservations { get; set; }
        [DoNotTrackChanges]
        public MassUpdate MassUpdateModel { get; set; }
        [ScriptIgnore]
        [DoNotTrackChanges]
        public List<SelectListItem> CallClasifications
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpCallsClasification();
            }
        }
        [ScriptIgnore]
        [DoNotTrackChanges]
        public List<SelectListItem> TimeZones
        {
            get
            {
                var list = TimeZoneDataModel.GetAllTimeZones();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        [ScriptIgnore]
        [DoNotTrackChanges]
        public List<SelectListItem> Countries
        {
            get
            {
                var list = CountryDataModel.GetAllCountries();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        [ScriptIgnore]
        [DoNotTrackChanges]
        public List<SelectListItem> Terminals
        {
            get
            {
                var list = TerminalDataModel.GetActiveTerminalsList();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        [DoNotTrackChanges]
        [ScriptIgnore]
        public SearchToImportModel SearchToImportModel { get; set; }
    }

    public class PreArrivalSearchModel : ItemsLists
    {
        #region "layout fields"
        [Display(Name = "Search Filters")]
        public string Search_SearchFilters { get; set; }

        [Display(Name = "Columns to Display")]
        public string Search_ColumnHeaders { get; set; }

        [Display(Name = "Layout")]
        public string Search_LayoutName { get; set; }

        public int Search_ReportLayout { get; set; }

        public string Search_Fields { get; set; }

        public Guid Search_OwnerUserID { get; set; }

        [Display(Name = "Is Public")]
        [Required(ErrorMessage = "Is Public is required")]
        public bool Search_Public { get; set; }

        public string Search_Filters { get; set; }
        public string Search_Columns { get; set; }
        #endregion
        [Display(Name = "Is Test")]
        [FieldInfo(Name = "isTest")]
        [DataBaseInfo(Name = "tblLeads")]
        public bool IsTest { get; set; }

        [Display(Name = "Lead ID")]
        [FieldInfo(Name = "leadID")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_LeadID { get; set; }

        [Display(Name = "Reference ID")]
        [FieldInfo(Name = "reservationID")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_ReservationID { get; set; }

        [Display(Name = "Greeting Rep")]
        [FieldInfo(Name = "greetingRepID")]
        [FieldToRequest(Name = "greetingRep")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblGreetingReps", PrimaryKeyFieldName = "greetingRepID")]
        public int[] Search_GreetingRep { get; set; }

        [Display(Name = "OPC")]
        [FieldInfo(Name = "opcID")]
        [FieldToRequest(Name = "opc")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblOPCS", PrimaryKeyFieldName = "opcID")]
        public int[] Search_OPC { get; set; }

        [Display(Name = "Tags")]
        [FieldInfo(Name = "tags")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_Tags { get; set; }

        [Display(Name = "First Name")]
        [FieldInfo(Name = "firstName")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_FirstName { get; set; }

        [Display(Name = "Last Name")]
        [FieldInfo(Name = "lastName")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_LastName { get; set; }

        [Display(Name = "Assignation Date")]
        [FieldInfo(Name = "assignationDate")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_I_AssignationDate { get; set; }
        public string Search_F_AssignationDate { get; set; }

        [Display(Name = "Arrival Date")]
        [FieldInfo(Name = "arrivalDate")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_I_ArrivalDate { get; set; }
        public string Search_F_ArrivalDate { get; set; }

        [Display(Name = "Purchase Date")]
        [FieldInfo(Name = "dateSaved")]
        [DataBaseInfo(Name = "tblPaymentDetails", ForeignKeyFieldName = "reservationID", PrimaryKeyTableName = "tblReservations", PrimaryKeyFieldName = "reservationID")]
        public string Search_I_PurchaseDate { get; set; }
        public string Search_F_PurchaseDate { get; set; }

        [Display(Name = "Options Status")]
        [FieldInfo(Name = "bookingStatusID")]
        [FieldToRequest(Name = "bookingStatus")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblBookingStatus", PrimaryKeyFieldName = "bookingStatusID")]
        public int[] Search_BookingStatus { get; set; }

        [Display(Name = "Pre Booking Status")]
        [FieldInfo(Name = "secondaryBookingStatusID")]
        [FieldToRequest(Name = "bookingStatus")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblBookingStatus", PrimaryKeyFieldName = "bookingStatusID")]
        public int[] Search_SecondaryBookingStatus { get; set; }


        [Display(Name = "Resort")]
        [FieldInfo(Name = "placeID")]
        [FieldToRequest(Name = "place")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblPlaces", PrimaryKeyFieldName = "placeID")]
        public int[] Search_Place { get; set; }

        [Display(Name = "Lead Source")]
        [FieldInfo(Name = "leadSourceID")]
        [FieldToRequest(Name = "leadSource")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblLeadSources", PrimaryKeyFieldName = "leadSourceID")]
        public int[] Search_LeadSource { get; set; }

        [Display(Name = "Cancelation Date")]
        [FieldInfo(Name = "dateCancelation")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_I_CancelationDate { get; set; }
        public string Search_F_CancelationDate { get; set; }

        [Display(Name = "Confirmation Date")]
        [FieldInfo(Name = "dateConfirmation")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_I_ConfirmationDate { get; set; }
        public string Search_F_ConfirmationDate { get; set; }

        [FieldInfo(Name = "destinationID")]
        [FieldToRequest(Name = "destination")]
        [Display(Name = "Presentation Destination")]
        [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyTableName = "tblDestinations", PrimaryKeyFieldName = "destinationID")]
        public int[] Search_PresentationDestination { get; set; }

        [FieldInfo(Name = "destinationID")]
        [FieldToRequest(Name = "destination")]
        [Display(Name = "Destination")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblDestinations", PrimaryKeyFieldName = "destinationID")]
        public int[] Search_Destination { get; set; }

        [Display(Name = "Expiration Date")]
        [FieldInfo(Name = "dateExpiration")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_I_ExpirationDate { get; set; }
        public string Search_F_ExpirationDate { get; set; }

        [Display(Name = "Hotel Confirmation Number")]
        [FieldInfo(Name = "hotelConfirmationNumber")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_HotelConfirmationNumber { get; set; }

        [Display(Name = "Agency Name")]
        [FieldInfo(Name = "frontOfficeAgencyName")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_FrontOfficeAgencyName { get; set; }

        //[Display(Name = "Phone")]
        //[FieldInfo(Name = "phone")]
        //[DataBaseInfo(Name = "tblPhones", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        //public string Search_Phone { get; set; }

        [Display(Name = "Real Tour Date")]
        [FieldInfo(Name = "realTourDate")]
        [DataBaseInfo(Name = "tblPresentations", ForeignKeyFieldName = "reservationID", PrimaryKeyTableName = "tblReservations", PrimaryKeyFieldName = "reservationID")]
        public string Search_I_RealTourDate { get; set; }
        public string Search_F_RealTourDate { get; set; }

        [Display(Name = "Reservation Status")]
        [FieldInfo(Name = "reservationStatusID")]
        [FieldToRequest(Name = "reservationStatus")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblReservationStatus", PrimaryKeyFieldName = "reservationStatusID")]
        public int[] Search_ReservationStatus { get; set; }

        //[Display(Name = "Sales Date")]
        //[FieldInfo(Name = "dateSaved")]
        //[DataBaseInfo(Name = "tblOptionsSold", ForeignKeyFieldName = "reservationID", PrimaryKeyTableName = "tblReservations", PrimaryKeyFieldName = "reservationID")]
        ////[DataBaseInfo(Name = "tblContractsHistory", ForeignKeyFieldName = "presentationID", PrimaryKeyTableName = "tblPresentations", PrimaryKeyFieldName = "presentationID")]
        //public string Search_I_SalesDate { get; set; }
        //public string Search_F_SalesDate { get; set; }

        [Display(Name = "Spouse Last Name")]
        [FieldInfo(Name = "spouseLastName")]
        [DataBaseInfo(Name = "tblLeads")]
        public string[] Search_SpouseLastName { get; set; }

        [Display(Name = "Terminal")]
        [FieldInfo(Name = "terminalID")]
        [FieldToRequest(Name = "terminal")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblTerminals", PrimaryKeyFieldName = "terminalID")]
        public long[] Search_Terminal { get; set; }

        [Display(Name = "Input By User")]
        [FieldInfo(Name = "inputByUserID")]
        [FieldToRequest(Name = "UserName")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "aspnet_Users", PrimaryKeyFieldName = "UserId")]
        public Guid[] Search_InputByUser { get; set; }

        [Display(Name = "Assigned To User")]
        [FieldToRequest(Name = "UserName")]
        [FieldInfo(Name = "assignedToUserID")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "aspnet_Users", PrimaryKeyFieldName = "UserId")]
        public Guid[] Search_AssignedToUser { get; set; }

        [Display(Name = "Certificate Number")]
        [FieldInfo(Name = "certificateNumber")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_CertificateNumber { get; set; }

        [Display(Name = "Contract Number")]
        [FieldInfo(Name = "frontOfficeContractNumber")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_FrontOfficeContractNumber { get; set; }

        [Display(Name = "Departure Date")]
        [FieldInfo(Name = "departureDate")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_I_DepartureDate { get; set; }
        public string Search_F_DepartureDate { get; set; }

        [Display(Name = "Plan Type")]
        [FieldInfo(Name = "planTypeID")]
        [FieldToRequest(Name = "planType")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblPlanTypes", PrimaryKeyFieldName = "planTypeID")]
        public int[] Search_PlanType { get; set; }

        [Display(Name = "Qualification Status")]
        [FieldInfo(Name = "qualificationStatusID")]
        [FieldToRequest(Name = "qualificationStatus")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblQualificationStatus", PrimaryKeyFieldName = "qualificationStatusID")]
        public int[] Search_QualificationStatus { get; set; }

        //[Display(Name ="Market Code")]
        //[FieldInfo(Name = "marketCode")]
        //[FieldToRequest(Name = "marketCode")]
        //[DataBaseInfo(Name = "tblReservations", IsRelationShip = false)]

        [FieldToRequest(Name = "UserName")]
        [Display(Name = "Sales Agent User")]
        [FieldInfo(Name = "salesAgentUserID")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "aspnet_Users", PrimaryKeyFieldName = "UserId")]
        public Guid[] Search_SalesAgentUser { get; set; }

        [DataBaseInfo(Name = "tblLeads")]
        [FieldInfo(Name = "spouseFirstName")]
        [Display(Name = "Spouse First Name")]
        public string Search_SpouseFirstName { get; set; }

        [Display(Name = "Reservation Comments")]
        [FieldInfo(Name = "reservationComments")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_ReservationComments { get; set; }

        [Display(Name = "Interaction Comments")]
        [FieldInfo(Name = "interactionComments")]
        [DataBaseInfo(Name = "tblInteractions", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_InteractionComments { get; set; }

        [Display(Name = "Hostess Comments")]
        [FieldInfo(Name = "hostessComments")]
        [DataBaseInfo(Name = "tblPresentations", ForeignKeyFieldName = "reservationID", PrimaryKeyTableName = "tblReservations", PrimaryKeyFieldName = "reservationID")]
        public string Search_HostessComments { get; set; }

        [Display(Name = "Member Number")]
        [FieldInfo(Name = "memberNumber")]
        [DataBaseInfo(Name = "tblMemberInfo", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public string Search_MemberNumber { get; set; }

        [Display(Name = "Tour Status")]
        [FieldInfo(Name = "tourStatusID")]
        [FieldToRequest(Name = "tourStatus")]
        [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyTableName = "tblTourStatus", PrimaryKeyFieldName = "tourStatusID")]
        public int[] Search_TourStatus { get; set; }

        [Display(Name = "Final Tour Status")]
        [FieldInfo(Name = "finalTourStatusID")]
        [FieldToRequest(Name = "tourStatus")]
        [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyTableName = "tblTourStatus", PrimaryKeyFieldName = "tourStatusID")]
        public int[] Search_FinalTourStatus { get; set; }

        [Display(Name = "Sales Volume")]
        [FieldInfo(Name = "salesVolume")]
        [DataBaseInfo(Name = "tblContractsHistory", ForeignKeyFieldName = "presentationID", PrimaryKeyTableName = "tblPresentations", PrimaryKeyFieldName = "presentationID")]
        public string Search_SalesVolume { get; set; }

        [Display(Name = "Submission Form")]
        [FieldInfo(Name = "submissionForm")]
        [DataBaseInfo(Name = "tblLeads")]
        public bool? Search_SubmissionForm { get; set; }

        [Display(Name = "Is All Inclusive")]
        [FieldInfo(Name = "isAllInclusive")]
        [DataBaseInfo(Name = "tblMemberInfo", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public bool? Search_IsAllInclusive { get; set; }

        [Display(Name = "Has Options")]
        [FieldInfo(Name = "hasOptions")]
        [DataBaseInfo(Name = "tblMemberInfo", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        public bool? Search_HasOptions { get; set; }

        [Display(Name = "Address")]
        [FieldInfo(Name = "address")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_Address { get; set; }

        [Display(Name = "City")]
        [FieldInfo(Name = "city")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_City { get; set; }

        [Display(Name = "State")]
        [FieldInfo(Name = "state")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_State { get; set; }

        [Display(Name = "Postal Code")]
        [FieldInfo(Name = "zipcode")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_ZipCode { get; set; }

        [Display(Name = "Country")]
        [FieldInfo(Name = "countryID")]
        [FieldToRequest(Name = "country")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblCountries", PrimaryKeyFieldName = "countryID")]
        public string Search_Country { get; set; }

        [Display(Name = "Notes")]
        [FieldInfo(Name = "leadComments")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_LeadComments { get; set; }

        [Display(Name = "Input Date")]
        [FieldInfo(Name = "inputDateTime")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_I_InputDate { get; set; }
        public string Search_F_InputDate { get; set; }

        [Display(Name = "Number Adults")]
        [FieldInfo(Name = "numberAdults")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        //[DataBaseInfo(Name = "tblReservations")]
        public string Search_NumberAdults { get; set; }

        [Display(Name = "Number Children")]
        [FieldInfo(Name = "numberChildren")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        //[DataBaseInfo(Name = "tblReservations")]
        public string Search_NumberChildren { get; set; }

        [Display(Name = "Room Type")]
        [FieldInfo(Name = "roomTypeID")]
        [FieldToRequest(Name = "roomType")]
        [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyTableName = "tblRoomTypes", PrimaryKeyFieldName = "roomTypeID")]
        public int[] Search_RoomType { get; set; }

        [Display(Name = "Total Nights")]
        [FieldInfo(Name = "totalNights")]
        [DataBaseInfo(Name = "tblReservations", ForeignKeyFieldName = "leadID", PrimaryKeyTableName = "tblLeads", PrimaryKeyFieldName = "leadID")]
        //[DataBaseInfo(Name = "tblReservations")]
        public string Search_TotalNights { get; set; }

        [Display(Name = "Call Clasification")]
        [FieldInfo(Name = "callClasificationID")]
        [FieldToRequest(Name = "callClasification")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblCallClasifications", PrimaryKeyFieldName = "callClasificationID")]
        public int[] Search_CallClasification { get; set; }

        [Display(Name = "Lead Status")]
        [FieldInfo(Name = "leadStatusID")]
        [FieldToRequest(Name = "leadStatus")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "tblLeadStatus", PrimaryKeyFieldName = "leadStatusID")]
        public int[] Search_LeadStatus { get; set; }

        [Display(Name = "Tour Date")]
        [FieldInfo(Name = "datePresentation")]
        [DataBaseInfo(Name = "tblPresentations", ForeignKeyFieldName = "reservationID", PrimaryKeyTableName = "tblReservations", PrimaryKeyFieldName = "reservationID")]
        public string Search_I_TourDate { get; set; }
        public string Search_F_TourDate { get; set; }

        [Display(Name = "Final Booking Status")]
        [FieldInfo(Name = "finalBookingStatusID")]
        [FieldToRequest(Name = "bookingStatus")]
        [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyTableName = "tblBookingStatus", PrimaryKeyFieldName = "bookingStatusID")]
        public int[] Search_FinalBookingStatus { get; set; }

        [Display(Name = "Modification Date")]
        [FieldInfo(Name = "modificationDate")]
        [DataBaseInfo(Name = "tblLeads")]
        public string Search_I_ModificationDate { get; set; }
        public string Search_F_ModificationDate { get; set; }

        [Display(Name = "Modified By User")]
        [FieldToRequest(Name = "UserName")]
        [FieldInfo(Name = "modifiedByUserID")]
        [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyTableName = "aspnet_Users", PrimaryKeyFieldName = "UserId")]
        public Guid[] Search_ModifiedByUser { get; set; }

        [Display(Name = "Interaction Booking Status")]
        [FieldInfo(Name = "bookingStatusID")]
        [FieldToRequest(Name = "bookingStatus")]
        [DataBaseInfo(Name = "tblInteractions", IsRelationShip = true, PrimaryKeyTableName = "tblBookingStatus", PrimaryKeyFieldName = "bookingStatusID")]
        public int[] Search_InteractionBookingStatus { get; set; }

        [Display(Name = "Interacted With User")]
        [FieldInfo(Name = "interactedWithUserID")]
        [FieldToRequest(Name = "UserName")]
        [DataBaseInfo(Name = "tblInteractions", IsRelationShip = true, PrimaryKeyTableName = "aspnet_Users", PrimaryKeyFieldName = "UserId")]
        public Guid[] Search_InteractedWithUser { get; set; }

        [Display(Name = "Tour Time")]
        [FieldInfo(Name = "timePresentation")]
        [DataBaseInfo(Name = "tblPresentations", ForeignKeyFieldName = "reservationID", PrimaryKeyTableName = "tblReservations", PrimaryKeyFieldName = "reservationID")]
        public string Search_PresentationTime { get; set; }

        public PreArrivalSearchModel()
        {
            Search_SubmissionForm = null;
            Search_IsAllInclusive = null;
            Search_HasOptions = null;
        }
    }

    public class PreArrivalEmailsInfoModel
    {
        public long EmailsInfo_LeadEmailID { get; set; }

        public Guid EmailsInfo_LeadID { get; set; }

        [Display(Name = "Email Address")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email Address is invalid.")]
        public string EmailsInfo_Email { get; set; }

        [Display(Name = "Main")]
        public bool EmailsInfo_Main { get; set; }
    }

    public class PreArrivalPhonesInfoModel
    {
        [DoNotTrackChanges]
        [ScriptIgnore]
        public bool PhonesInfo_ShowContactInfo
        {
            get
            {

                var view = AdminDataModel.GetViewPrivileges(12021).FirstOrDefault();
                return view != null ? view.View : false;
            }
        }
        public long PhonesInfo_LeadPhoneID { get; set; }

        public Guid PhonesInfo_LeadID { get; set; }

        [Display(Name = "Phone Type")]
        public int PhonesInfo_PhoneType { get; set; }
        public string PhonesInfo_PhoneTypeText { get; set; }

        [Display(Name = "Phone Number")]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhonesInfo_PhoneNumber { get; set; }

        [Display(Name = "Extension Number")]
        public string PhonesInfo_ExtensionNumber { get; set; }

        [Display(Name = "Main")]
        public bool PhonesInfo_Main { get; set; }

        [Display(Name = "Do Not Call")]
        public bool PhonesInfo_DoNotCall { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> PhoneTypes
        {
            get
            {
                return PhoneDataModel.GetAllPhoneTypes();
            }
        }

    }

    public class PreArrivalMemberInfoModel
    {
        [Display(Name = "Club Type")]
        public string MemberInfo_ClubType { get; set; }

        [Display(Name = "Co Owner")]
        public string MemberInfo_CoOwner { get; set; }

        [Display(Name = "Member Number")]
        public string MemberInfo_MemberNumber { get; set; }

        [Display(Name = "Member Name")]
        public string MemberInfo_MemberName { get; set; }

        [Display(Name = "Contract Number")]
        public string MemberInfo_ContractNumber { get; set; }
    }

    public class PreArrivalInteractionsInfoModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public long InteractionsInfo_InteractionID { get; set; }

        public Guid? InteractionsInfo_LeadID { get; set; }
        public Guid? InteractionsInfo_TrialID { get; set; }

        [Display(Name = "Interaction Type")]
        public int InteractionsInfo_InteractionType { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> InteractionTypes
        {
            get
            {
                return InteractionDataModel.GetAllInteractionTypes();
            }
        }

        [Required(ErrorMessage = "Options Status is required")]
        [Display(Name = "Options Status")]
        public int? InteractionsInfo_BookingStatus { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> BookingStatus
        {
            get
            {
                var list = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                list.Insert(0, ListItems.Default("--Select One--", ""));
                return list;
            }
        }

        [Display(Name = "Interacted With User")]
        public Guid? InteractionsInfo_InteractedWithUser { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> Users
        {
            get
            {
                if (GeneralFunctions.IsUserInRole("Administrator", (Guid)Membership.GetUser().ProviderUserKey))
                {
                    return UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup();
                }
                else
                {
                    return UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
                }
            }
        }

        [Display(Name = "Interaction Comments")]
        public string InteractionsInfo_InteractionComments { get; set; }

        public long? InteractionsInfo_ParentInteraction { get; set; }

        [Display(Name = "Total Sold")]
        public string InteractionsInfo_TotalSold { get; set; }

        public Guid? InteractionsInfo_SavedByUser { get; set; }
        public string InteractionsInfo_DateSaved { get; set; }
    }

    public class PreArrivalReservationsModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public Guid ReservationInfo_LeadID { get; set; }
        public Guid ReservationInfo_ReservationID { get; set; }

        [Display(Name = "Arrival Date")]
        public string ReservationInfo_ArrivalDate { get; set; }

        [Display(Name = "Departure Date")]
        public string ReservationInfo_DepartureDate { get; set; }

        [Display(Name = "Destination")]
        public long ReservationInfo_Destination { get; set; }

        public string ReservationInfo_DestinationText { get; set; }

        [Display(Name = "Resort")]
        public long ReservationInfo_Place { get; set; }

        [Display(Name = "RoomType")]
        public long ReservationInfo_RoomType { get; set; }

        [Display(Name = "Room Number")]
        public string ReservationInfo_RoomNumber { get; set; }

        [Display(Name = "Adults")]
        public int ReservationInfo_Adults { get; set; }

        [Display(Name = "Children")]
        public int ReservationInfo_Children { get; set; }

        [Display(Name = "Additional Adults")]
        public int ReservationInfo_AdditionalAdults { get; set; }

        [Display(Name = "Additional Children")]
        public int ReservationInfo_AdditionalChildren { get; set; }

        [Display(Name = "Front Plan Type")]
        public string ReservationInfo_FrontPlanType { get; set; }

        [Display(Name = "Plan Type")]
        public int ReservationInfo_PlanType { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> PlanTypes
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetPlanTypes();
            }
        }

        [Display(Name = "Front Contract Number")]
        public string ReservationInfo_FrontContractNumber { get; set; }

        [Display(Name = "Front Certificate Number")]
        public string ReservationInfo_FrontCertificateNumber { get; set; }

        [Display(Name = "Certificate Number")]
        public string ReservationInfo_CertificateNumber { get; set; }

        [Display(Name = "Children Ages")]
        public string ReservationInfo_ChildrenAges { get; set; }

        [Display(Name = "Infants")]
        public int ReservationInfo_Infants { get; set; }

        [Display(Name = "Special Requests")]
        public string ReservationInfo_SpecialRequests { get; set; }

        [Display(Name = "Front Comments")]
        public string ReservationInfo_FrontComments { get; set; }

        [Display(Name = "Reservation Comments")]
        public string ReservationInfo_ReservationComments { get; set; }

        [Display(Name = "Resort Connect Reservation Comments")]
        public string ReservationInfo_ResortConnectReservationComments { get; set; }

        [Display(Name = "Sales Agent")]
        public Guid ReservationInfo_SalesAgent { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> Users
        {
            get
            {
                if (GeneralFunctions.IsUserInRole("Administrator", (Guid)Membership.GetUser().ProviderUserKey))
                {
                    return UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup();
                }
                else
                {
                    return UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
                }
            }
        }

        [Display(Name = "Sales Date")]
        public string ReservationInfo_SalesDate { get; set; }

        [Display(Name = "Hotel Confirmation Number")]
        public string ReservationInfo_HotelConfirmationNumber { get; set; }

        [Display(Name = "Package Nights")]
        public int ReservationInfo_PackageNights { get; set; }

        [Display(Name = "Package Price")]
        public decimal? ReservationInfo_PackagePrice { get; set; }

        [Display(Name = "Total Nights")]
        public int ReservationInfo_TotalNights { get; set; }

        [Display(Name = "Extra Night Price")]
        public decimal? ReservationInfo_ExtraNightPrice { get; set; }

        [Display(Name = "Reservation Status")]
        public int ReservationInfo_ReservationStatus { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> ReservationInfo_DrpReservationStatus
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetReservationStatus();
            }
        }

        [Display(Name = "Agency Name")]
        public string ReservationInfo_FrontOfficeAgencyName { get; set; }

        [Display(Name = "Reservation Status Date")]
        public string ReservationInfo_ReservationStatusDate { get; set; }

        [Display(Name = "Net Center Cost")]
        public decimal? ReservationInfo_NetCenterCost { get; set; }

        [Display(Name = "Adult Cost")]
        public decimal? ReservationInfo_AdultCost { get; set; }

        [Display(Name = "Child Cost")]
        public decimal? ReservationInfo_ChildCost { get; set; }

        [Display(Name = "Additional Adult Cost")]
        public decimal? ReservationInfo_AdditionalAdultCost { get; set; }

        [Display(Name = "Package Adults")]
        public int ReservationInfo_PackageAdults { get; set; }

        [Display(Name = "Package Children")]
        public int ReservationInfo_PackageChildren { get; set; }

        [Display(Name = "Addition Room Type")]
        public long? ReservationInfo_AdditionalRoomType { get; set; }

        [Display(Name = "Date Expiration")]
        public string ReservationInfo_DateExpiration { get; set; }

        [Display(Name = "Date Confirmation")]
        public string ReservationInfo_DateConfirmation { get; set; }

        [Display(Name = "Date Cancelation")]
        public string ReservationInfo_DateCancelation { get; set; }

        [Display(Name = "Certificate Comments")]
        public string ReservationInfo_CertificateComments { get; set; }

        [Display(Name = "Total Paid")]
        public decimal? ReservationInfo_TotalPaid { get; set; }

        [Display(Name = "Reservation Agent")]
        public Guid? ReservationInfo_ReservationAgent { get; set; }

        [Display(Name = "Front Status")]
        public int? ReservationInfo_FrontStatus { get; set; }

        [Display(Name = "Team Source")]
        public int? ReservationInfo_TeamSource { get; set; }

        [Display(Name = "Concierge Comments")]
        public string ReservationInfo_ConciergeComments { get; set; }

        [Display(Name = "Guests Names")]
        public string ReservationInfo_GuestsNames { get; set; }

        [Display(Name = "Confirmation Letter On Arrival")]
        public bool ReservationInfo_ConfirmationLetterOnArrival { get; set; }

        [Display(Name = "Transportation Included")]
        public bool ReservationInfo_TransportationIncluded { get; set; }

        [Display(Name = "Is Special Ocassion")]
        public bool ReservationInfo_IsSpecialOcassion { get; set; }

        [Display(Name = "Special Ocassion Comments")]
        public string ReservationInfo_SpecialOcassionComments { get; set; }

        [Display(Name = "All Inclusive Cost")]
        public decimal? ReservationInfo_AllInclusiveCost { get; set; }

        [Display(Name = "Transportation Cost")]
        public decimal? ReservationInfo_ReservationCost { get; set; }

        [Display(Name = "Upgrade Room Cost")]
        public decimal? ReservationInfo_UpgradeRoomCost { get; set; }

        [Display(Name = "Room Cost")]
        public decimal? ReservationInfo_RoomCost { get; set; }

        [Display(Name = "Greeting Rep")]
        public long? ReservationInfo_GreetingRep { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> GreetingReps
        {
            get
            {
                var list = GreetingRepDataModel.GetAllGreetingReps();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "OPC")]
        public long? ReservationInfo_OPC { get; set; }

        [Display(Name = "Confirmed Total Paid")]
        public decimal? ReservationInfo_ConfirmedTotalPaid { get; set; }

        [Display(Name = "Diamante Total Paid")]
        public decimal? ReservationInfo_DiamanteTotalPaid { get; set; }

        [Display(Name = "Room Upgraded")]
        public bool ReservationInfo_RoomUpgraded { get; set; }

        [Display(Name = "Number Adults")]
        public string ReservationInfo_NumberAdults { get; set; }

        [Display(Name = "Number Children")]
        public string ReservationInfo_NumberChildren { get; set; }

        [Display(Name = "Pre Check In")]
        public bool ReservationInfo_PreCheckIn { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> PreCheckIn
        {
            get
            {
                return ListItems.Booleans();
            }
        }

        public PreArrivalPresentationsModel PreArrivalPresentationsModel { get; set; }

        public PreArrivalOptionsSoldModel PreArrivalOptionsSoldModel { get; set; }

        public PreArrivalPaymentsModel PreArrivalPaymentsModel { get; set; }
        [DoNotTrackChanges]
        public List<PreArrivalPaymentsModel> ListPreArrivalPayments { get; set; }

        public PreArrivalFlightsModel PreArrivalFlightsModel { get; set; }
        [DoNotTrackChanges]
        public List<FlightResultsModel> ListPreArrivalFlights { get; set; }
    }

    public class PreArrivalPresentationsModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public Guid PresentationInfo_LeadID { get; set; }
        public Guid PresentationInfo_ReservationID { get; set; }
        public long PresentationInfo_PresentationID { get; set; }

        [Display(Name = "Lead Profile")]
        public long? PresentationInfo_LeadProfile { get; set; }

        [Display(Name = "Destination")]
        public long PresentationInfo_Destination { get; set; }

        [Display(Name = "Presentation Date")]
        public string PresentationInfo_DatePresentation { get; set; }

        [Display(Name = "Presentation Time")]
        public string PresentationInfo_TimePresentation { get; set; }

        [Display(Name = "Real Tour Date")]
        //[RequiredIf(OtherProperty = "PresentationInfo_FinalTourStatus", EqualsTo = new []{3, 6}, ErrorMessage ="Real Tour Date Required")]
        [RequiredIf(OtherProperty = "PresentationInfo_FinalTourStatus", EqualsTo = "3,6", ErrorMessage = "Real Tour Date Required")]
        public string PresentationInfo_RealTourDate { get; set; }

        [Display(Name = "Tour Status")]
        public int PresentationInfo_TourStatus { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> TourStatus
        {
            get
            {
                //var list = TourStatusDataModel.GetAlltourStatus();
                var list = TourStatusDataModel.GetTourStatusByCurrentWorkGroup();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Tour Status")]
        public string PresentationInfo_TourStatusText { get; set; }

        [Display(Name = "Final Tour Status")]
        public int PresentationInfo_FinalTourStatus { get; set; }

        [Display(Name = "Pre Booking Status")]
        public int PresentationInfo_SecondaryBookingStatus { get; set; }

        [Display(Name = "Final Booking Status")]
        public int? PresentationInfo_FinalBookingStatus { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> BookingStatus
        {
            get
            {
                //var list = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                var list = BookingStatusDataModel.GetSecondaryBookingStatus();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        public List<SelectListItem> FinalBookingStatus
        {
            get
            {
                var list = BookingStatusDataModel.GetFinalBookingStatus();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Concierge Comments")]
        public string PresentationInfo_HostessComments { get; set; }

        [Display(Name = "Total Attendants")]
        public int PresentationInfo_TotalAttendants { get; set; }

        [Display(Name = "SPI Client Name")]
        public string PresentationInfo_SPIClientName { get; set; }

        [Display(Name = "SPI Tour Date")]
        public string PresentationInfo_SpiTourDate { get; set; }

        [Display(Name = "SPI Client")]
        public int? PresentationInfo_SpiTour { get; set; }

        [Display(Name = "Source")]
        public string PresentationInfo_SPISource { get; set; }

        [Display(Name = "Volume Sold")]
        public string PresentationInfo_VolumeSold { get; set; }

        [Display(Name = "Lead Source")]
        public long? PresentationInfo_LeadSource { get; set; }
        public List<SelectListItem> PresentationInfo_LeadSources
        {
            get
            {
                var list = LeadSourceDataModel.GetLeadSourcesByTerminal();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        public List<SelectListItem> PresentationInfo_Tours
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetManifestByDate(0);
            }
        }
    }

    public class PreArrivalPaymentsModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public long PaymentInfo_PaymentDetailsID { get; set; }
        public Guid PaymentInfo_ReservationID { get; set; }

        public int PaymentInfo_TransactionType { get; set; }

        [Display(Name = "Points Qty")]
        public decimal PaymentInfo_Quantity { get; set; }

        public bool PaymentInfo_Points { get; set; }

        public string PaymentInfo_PointsRates { get; set; }

        [Display(Name = "Amount")]
        public decimal PaymentInfo_Amount { get; set; }

        [Display(Name = "Currency")]
        public string PaymentInfo_Currency { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> Currencies
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
            }
        }

        [Display(Name = "Net Center Cost")]
        public decimal? PaymentInfo_NetCenterCost { get; set; }

        [Display(Name = "Net Center Charge")]
        public decimal? PaymentInfo_NetCenterCharge { get; set; }

        [Display(Name = "Payment Comments")]
        public string PaymentInfo_PaymentComments { get; set; }

        [Display(Name = "Refund Account")]
        public string PaymentInfo_RefundAccount { get; set; }

        [Display(Name = "Invoice To Refund")]
        public int? PaymentInfo_InvoiceToRefund { get; set; }

        [Display(Name = "Charge Type")]
        public int PaymentInfo_ChargeType { get; set; }

        [Display(Name = "Charge Description")]
        public int? PaymentInfo_ChargeDescription { get; set; }

        public long PaymentInfo_MoneyTransaction { get; set; }

        [Display(Name = "Payment Type")]
        public int PaymentInfo_PaymentType { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> PaymentTypes
        {
            get
            {
                //return MasterChartDataModel.LeadsCatalogs.FillDrpPaymentTypes(0);
                return PreArrivalDataModel.PreArrivalCatalogs.GetPaymentTypesByWorkGroup();
            }
        }

        [Display(Name = "Credit Card")]
        public long PaymentInfo_BillingInfo { get; set; }

        public List<SelectListItem> PaymentInfo_DrpBillingInfo
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Insert(0, ListItems.Default());
                list.Add(new SelectListItem() { Value = "null", Text = "Add Info" });
                list.Add(ListItems.Default("+Add reference Number", "-1"));
                return list;
            }
        }

        [Display(Name = "Invoice Number")]
        //[RequiredIf(ErrorMessage = "Invoice Number required", OtherProperty = "PaymentInfo_MadeByEplat", EqualsTo = false)]
        public string PaymentInfo_Transaction { get; set; }

        [Display(Name = "Charge by ePlat")]
        public bool PaymentInfo_MadeByEplat { get; set; }

        [Display(Name = "Date Saved")]
        public string PaymentInfo_DateSaved { get; set; }

        //[Display(Name = "Certificate Numbers")]
        //public string PaymentInfo_CertificateNumbers { get; set; }

        //[Display(Name = "Concept")]
        //public long? PaymentInfo_ChargeBackConcept { get; set; }

        //[Display(Name = "OPC")]
        //public long? PaymentInfo_OPC { get; set; }

        //[Display(Name = "Other")]
        //public string PaymentInfo_Other { get; set; }

        //[Display(Name = "Promotion Team")]
        //public int? PaymentInfo_PromotionTeam { get; set; }

        //[Display(Name = "Charge To")]
        //public int? PaymentInfo_ChargedToCompany { get; set; }

        //[Display(Name = "Invitation")]
        //public string PaymentInfo_Invitation { get; set; }

        //[Display(Name = "Location")]
        //public int? PaymentInfo_Location { get; set; }

        //[Display(Name = "Apply Commission")]
        //public bool PaymentInfo_ApplyCommission { get; set; }

        [Display(Name = "CC Reference Number")]
        public int? PaymentInfo_CCReferenceNumber { get; set; }

        [Display(Name = "CC Type")]
        public int PaymentInfo_CCType { get; set; }

        public List<SelectListItem> PaymentInfo_DrpCardTypes
        {
            get
            {
                return CreditCardsDataModel.GetAllCreditCardTypes();
            }
        }

        //[Display(Name = "Budget")]
        //public int? PaymentInfo_Budget { get; set; }

        //[Display(Name = "Charge Back Folio")]
        //public string PaymentInfo_ChargeBackFolio { get; set; }

        public string PaymentInfo_Status { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> ChargeTypes
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetAllChargeTypes();
            }
        }
        [DoNotTrackChanges]
        public List<SelectListItem> ChargeDescriptions
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetAllChargeDescriptions();
            }
        }
        [DoNotTrackChanges]
        public List<SelectListItem> TransactionTypes
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpTransactionTypes();
            }
        }

        public PreArrivalPaymentsModel()
        {
            PaymentInfo_MadeByEplat = true;
            PaymentInfo_TransactionType = 1;
            PaymentInfo_Points = false;
        }
    }

    public class PreArrivalBillingModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public Guid BillingInfo_LeadID { get; set; }
        public long BillingInfo_BillingInfoID { get; set; }

        [Display(Name = "First Name")]
        public string BillingInfo_FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string BillingInfo_LastName { get; set; }

        [Display(Name = "Address")]
        public string BillingInfo_Address { get; set; }

        [Display(Name = "Country")]
        public int BillingInfo_Country { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> Countries
        {
            get
            {
                return CountryDataModel.GetAllCountries();
            }
        }

        [Display(Name = "Zip Code")]
        public string BillingInfo_ZipCode { get; set; }

        [Display(Name = "Card Holder Name")]
        public string BillingInfo_CardHolderName { get; set; }

        [Display(Name = "Card Number")]
        [StringLength(19)]
        public string BillingInfo_CardNumber { get; set; }

        [Display(Name = "Card Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Card Type is required")]
        public int BillingInfo_CardType { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> CardTypes
        {
            get
            {
                return CreditCardsDataModel.GetAllCreditCardTypes();
            }
        }

        [Display(Name = "Card Expiry")]
        [RegularExpression("^(0[1-9]|1[0-2])/+([0-9]{4})$", ErrorMessage = "Card Expiry is invalid.")]
        public string BillingInfo_CardExpiry { get; set; }

        [Display(Name = "Card CVV")]
        public string BillingInfo_CardCVV { get; set; }

        [Display(Name = "Comments")]
        public string BillingInfo_Comments { get; set; }
    }

    public class PreArrivalFlightsModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public Guid FlightInfo_ReservationID { get; set; }
        public long FlightInfo_FlightID { get; set; }

        [Display(Name = "Airline")]
        public string FlightInfo_AirlineName { get; set; }
        public int FlightInfo_Airline { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> Airlines
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpAirlines();
            }
        }
        public string FlightInfo_AirlineText { get; set; }

        [Display(Name = "Flight Number")]
        public string FlightInfo_FlightNumber { get; set; }

        [Display(Name = "Passengers Names")]
        public string FlightInfo_PassengerNames { get; set; }

        [Display(Name = "Passengers")]
        public int FlightInfo_Passengers { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> Passengers
        {
            get
            {
                var x = 1;
                var list = new List<SelectListItem>();
                while (x <= 20)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = x.ToString(),
                        Text = x.ToString()
                    });
                    x++;
                }
                return list;
            }
        }

        [Display(Name = "Destination")]
        public long FlightInfo_Destination { get; set; }

        [Display(Name = "Flight Comments")]
        public string FlightInfo_FlightComments { get; set; }

        [Display(Name = "Flight Date & Time")]
        public string FlightInfo_FlightDateTime { get; set; }

        [Display(Name = "Flight Type")]
        public int FlightInfo_FlightType { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> FlightTypes
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpFlightTypes();
            }
        }
        public string FlightInfo_FlightTypeText { get; set; }

        [Display(Name = "PickUp Time")]
        //[RequiredIf(OtherProperty = "FlightInfo_FlightType", EqualsTo = 2, ErrorMessage = "PickUp Time is required")]
        public string FlightInfo_PickUpTime { get; set; }

        //[Display(Name = "Transportation Letter Sent Date & Time")]
        //public string FlightInfo_TransportationLetterDateTime { get; set; }

        //[Display(Name = "Transportation Service Sent Date & Time")]
        //public string FlightInfo_TransportationServiceDateTime { get; set; }

        //[Display(Name = "Transportation Purchased")]
        //public bool FlightInfo_TransportationPurchased { get; set; }

        //[Display(Name = "Transportation Provider")]
        //public int FlightInfo_TransportationProvider { get; set; }
    }

    public class PreArrivalOptionsSoldModel
    {
        [DoNotTrackChanges]
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public Guid OptionInfo_ReservationID { get; set; }
        public long OptionInfo_OptionSoldID { get; set; }

        [Display(Name = "Option Type")]
        public int OptionInfo_OptionType { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> OptionTypes
        {
            get
            {
                var list = MasterChartDataModel.LeadsCatalogs.FillDrpOptionCategories();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Option Name")]
        public int OptionInfo_Option { get; set; }

        [Display(Name = "Option Description")]
        public string OptionInfo_OptionDescription { get; set; }

        [Display(Name = "Price Per Option")]
        public string OptionInfo_Price { get; set; }

        [Display(Name = "Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity is required")]
        public decimal OptionInfo_Quantity { get; set; }

        [Display(Name = "Date & Time")]
        public string OptionInfo_DateTime { get; set; }

        [Display(Name = "Max Volume Redemption")]
        public string OptionInfo_PointsRedemption { get; set; }

        public string OptionInfo_MaxRateRedemption { get; set; }

        [Display(Name = "Total Paid")]
        public decimal OptionInfo_TotalPaid { get; set; }

        [Display(Name = "Guest Name(s)")]
        [Required(ErrorMessage = "Guest Name is required")]
        public string OptionInfo_GuestNames { get; set; }

        [Display(Name = "Eligible for Credit")]
        public string OptionInfo_Eligible { get; set; }
        [DoNotTrackChanges]
        public List<SelectListItem> EligibleForCredit
        {
            get
            {
                return ListItems.Booleans();
            }
        }

        [Display(Name = "Credit Amount")]
        public string OptionInfo_CreditAmount { get; set; }

        [Display(Name = "Comments")]
        public string OptionInfo_Comments { get; set; }
    }

    public class MassUpdate
    {
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public string MassUpdate_Coincidences { get; set; }

        [Display(Name = "Assigned to User")]
        [DataBaseInfo(Name = "tblLeads")]
        [FieldInfo(Name = "assignedToUserID")]
        public string MassUpdate_AssignedToUser { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> MassUpdate_DrpUsers
        {
            get
            {
                var list = new List<SelectListItem>();
                return list;
            }
        }

        [Display(Name = "Terminal")]
        [DataBaseInfo(Name = "tblLeads")]
        [FieldInfo(Name = "terminalID")]
        [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
        public int MassUpdate_Terminal { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> MassUpdate_DrpTerminals
        {
            get
            {
                var list = TerminalDataModel.GetActiveTerminalsList();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Lead Status")]
        [DataBaseInfo(Name = "tblLeads")]
        [FieldInfo(Name = "leadStatusID")]
        public int MassUpdate_LeadStatus { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> MassUpdate_DrpLeadStatus
        {
            get
            {
                var list = LeadStatusDataModel.GetLeadStatusByTerminal();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Lead Source")]
        [DataBaseInfo(Name = "tblLeads")]
        [FieldInfo(Name = "leadSourceID")]
        public int MassUpdate_LeadSource { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> MassUpdate_DrpLeadSources
        {
            get
            {
                var list = LeadSourceDataModel.GetLeadSourcesByTerminal();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Booking Status")]
        [DataBaseInfo(Name = "tblLeads")]
        [FieldInfo(Name = "bookingStatusID")]
        public int MassUpdate_BookingStatus { get; set; }
        //tblInteractions

        [Display(Name = "Booking Status")]
        [DataBaseInfo(Name = "tblInteractions")]
        [FieldInfo(Name = "bookingStatusID")]
        [DoNotUpdate]
        [Range(1, int.MaxValue, ErrorMessage = "Booking Status is required.")]
        public int BookingStatusID { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> BookingStatus
        {
            get
            {
                List<SelectListItem> BookingStatus = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                BookingStatus.Insert(0, ListItems.Default());
                return BookingStatus;
            }
        }

        [Display(Name = "Interaction Type")]
        [DataBaseInfo(Name = "tblInteractions")]
        [FieldInfo(Name = "interactionTypeID")]
        [DoNotUpdate]
        [Range(1, int.MaxValue, ErrorMessage = "Interaction Type is required.")]
        public int InteractionTypeID { get; set; }
        //[ScriptIgnore]
        public List<SelectListItem> InteractionTypes
        {
            get
            {
                List<SelectListItem> InteractionTypes = InteractionDataModel.GetAllInteractionTypes();
                return InteractionTypes;
            }
        }

        [DataBaseInfo(Name = "tblInteractions")]
        [FieldInfo(Name = "interactionComments")]
        [DoNotUpdate]
        public string InteractionComments { get; set; }
        [ScriptIgnore]
        [Display(Name = "Saved By")]
        [DataBaseInfo(Name = "tblInteractions")]
        [FieldInfo(Name = "savedByUserID")]
        [DoNotUpdate]
        public Guid? SavedByUserID { get { return (Guid)Membership.GetUser().ProviderUserKey; } }
        //[ScriptIgnore]
        public List<SelectListItem> Users
        {
            get
            {
                var list = UserDataModel.GetUsersBySupervisor(null, true, false, true);
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        [Display(Name = "Interacted With User")]
        [DataBaseInfo(Name = "tblInteractions")]
        [FieldInfo(Name = "interactedWithUserID")]
        [DoNotUpdate]
        public Guid? InteractedWithUserID { get; set; }

        [Display(Name = "Date Saved")]
        [DataBaseInfo(Name = "tblInteractions")]
        [FieldInfo(Name = "dateSaved")]
        [DoNotUpdate]
        public DateTime? InteractionDateSaved { get { return DateTime.Now; } }

        public int MassUpdate_SendingEvent { get; set; }
    }

    public class SearchFields
    {
        public string FieldID { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string DisplayName { get; set; }
        public string PropertyName { get; set; }
        public string FilterID { get; set; }
    }

    public class ItemsLists
    {
        /// <summary>
        /// return all call clasifications
        /// </summary>
        public List<SelectListItem> CallClasifications
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpCallsClasification();
            }
        }

        /// <summary>
        /// return booking status of current workgroup
        /// </summary>
        public List<SelectListItem> BookingStatus
        {
            get
            {
                return BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
            }
        }

        public List<SelectListItem> SecondaryBookingStatus
        {
            get
            {
                return BookingStatusDataModel.GetSecondaryBookingStatus();
            }
        }

        /// <summary>
        /// return all lead status per session terminals
        /// </summary>
        public List<SelectListItem> LeadStatus
        {
            get
            {
                //return MasterChartDataModel.LeadsCatalogs.FillDrpLeadStatus();
                return new List<SelectListItem>();
            }
        }

        /// <summary>
        /// return all lead sources per session terminals and workgroup
        /// </summary>
        public List<SelectListItem> LeadSources
        {
            get
            {
                //return LeadSourceDataModel.GetLeadSourcesByTerminal();
                return new List<SelectListItem>();
            }
        }
        public List<SelectListItem> Destinations
        {
            get
            {
                //return PlaceDataModel.GetDestinationsByCurrentTerminals();
                return new List<SelectListItem>();
            }
        }
        public List<SelectListItem> Resorts
        {
            get
            {
                //return PlaceDataModel.GetResortsByProfile();
                return new List<SelectListItem>();
            }
        }

        public List<SelectListItem> RoomTypes
        {
            get
            {
                //return PlaceDataModel.GetRoomTypesByPlace(0);
                return new List<SelectListItem>();
            }
        }

        public List<SelectListItem> PlanTypes
        {
            get
            {
                return new List<SelectListItem>();
            }
        }
        public List<SelectListItem> Users
        {
            get
            {
                if (GeneralFunctions.IsUserInRole("Administrator", (Guid)Membership.GetUser().ProviderUserKey))
                {
                    return UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup();
                }
                else
                {
                    return UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
                }
            }
        }

        public List<SelectListItem> InteractionTypes
        {
            get
            {
                return InteractionDataModel.GetAllInteractionTypes();
            }
        }

        public List<SelectListItem> TourStatus
        {
            get
            {
                //return TourStatusDataModel.GetAlltourStatus();
                return TourStatusDataModel.GetTourStatusByCurrentWorkGroup();
            }
        }

        public List<SelectListItem> PreCheckIn
        {
            get
            {
                return ListItems.Booleans();
            }
        }

        /// <summary>
        /// return session terminals
        /// </summary>
        public List<SelectListItem> Terminals
        {
            get
            {
                var list = TerminalDataModel.GetActiveTerminalsList();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        public List<SelectListItem> TimeZones
        {
            get
            {
                return TimeZoneDataModel.GetAllTimeZones();
            }
        }

        public List<SelectListItem> Countries
        {
            get
            {
                return CountryDataModel.GetAllCountries();
            }
        }

        public List<SelectListItem> ReportLayouts
        {
            get
            {
                var list = ReportDataModel.ReportsCatalogs.FillDrpReportLayoutsByUser();
                list.Insert(0, ListItems.NotSet("New Layout"));
                list.Insert(0, ListItems.Default("None"));
                return list;
            }
        }

        public List<SelectListItem> GreetingReps
        {
            get
            {
                return GreetingRepDataModel.GetAllGreetingReps();
            }
        }

        /// <summary>
        /// Get payment types per point of sale or all if PoS is 0
        /// </summary>
        public List<SelectListItem> PaymentTypes
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpPaymentTypes(0);
            }
        }

        /// <summary>
        /// Get currencies except CAD
        /// </summary>
        public List<SelectListItem> Currencies
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD();
            }
        }

        public List<SelectListItem> ChargeTypes
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetAllChargeTypes();
            }
        }

        public List<SelectListItem> ChargeDescriptions
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetAllChargeDescriptions();
            }
        }

        public List<SelectListItem> TransactionTypes
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpTransactionTypes();
            }
        }

        public List<SelectListItem> CardTypes
        {
            get
            {
                return CreditCardsDataModel.GetAllCreditCardTypes();
            }
        }

        public List<SelectListItem> FlightTypes
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpFlightTypes();
            }
        }

        public List<SelectListItem> Airlines
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpAirlines();
            }
        }

        public List<SelectListItem> Passengers
        {
            get
            {
                var x = 1;
                var list = new List<SelectListItem>();
                while (x <= 12)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = x.ToString(),
                        Text = x.ToString()
                    });
                    x++;
                }
                return list;
            }
        }

        public List<SelectListItem> ReservationStatus
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetReservationStatus();
            }
        }
    }

    public class KeyValueModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class PreArrivalResultModel
    {
        public string Info_LeadID { get; set; }
        public string Info_FirstName { get; set; }
        public string Info_LastName { get; set; }
        public string Info_Terminal { get; set; }
        public string Info_LeadStatus { get; set; }
        public string Info_LeadStatusDescription { get; set; }
        public string Info_LeadSource { get; set; }
        public string Info_SecondaryBookingStatus { get; set; }
        public string Info_CallClasification { get; set; }
        public string Info_TimeZone { get; set; }
        public string Info_Address { get; set; }
        public string Info_City { get; set; }
        public string Info_State { get; set; }
        public string Info_ZipCode { get; set; }
        public string Info_Country { get; set; }
        public List<PreArrivalEmailsInfoModel> ListPreArrivalEmails { get; set; }
        public List<PreArrivalPhonesInfoModel> ListPreArrivalPhones { get; set; }
        public PreArrivalMemberInfoModel PreArrivalMemberInfoModel { get; set; }
        public List<BillingResultsModel> ListPreArrivalBillings { get; set; }
        public List<InteractionResultsModel> PreArrivalInteractions { get; set; }
        public List<ReservationResultsModel> ListPreArrivalReservations { get; set; }

        public string Info_Response { get; set; }
    }

    public class InteractionResultsModel
    {
        public string InteractionsInfo_InteractionID { get; set; }
        public string InteractionsInfo_LeadID { get; set; }
        public string InteractionsInfo_TrialID { get; set; }
        public string InteractionsInfo_BookingStatus { get; set; }
        public string InteractionsInfo_InteractionType { get; set; }
        public string InteractionsInfo_InteractionComments { get; set; }
        public string InteractionsInfo_SavedByUser { get; set; }
        public string InteractionsInfo_SavedByUserName { get; set; }
        public string InteractionsInfo_DateSaved { get; set; }
        public string InteractionsInfo_InteractedWithUser { get; set; }
        public string InteractionsInfo_ParentInteraction { get; set; }
        public string InteractionsInfo_TotalSold { get; set; }
    }

    public class BillingResultsModel
    {
        public string BillingInfo_BillingInfoID { get; set; }
        public string BillingInfo_CardHolderName { get; set; }
        public string BillingInfo_CardType { get; set; }
        public string BillingInfo_CardNumber { get; set; }
        public string BillingInfo_CardExpiry { get; set; }
        public string BillingInfo_CardCVV { get; set; }
        public bool BillingInfo_ShowInfo { get; set; }
        public bool BillingInfo_IsAdmin { get; set; }

        public BillingResultsModel()
        {
            BillingInfo_ShowInfo = true;
            BillingInfo_IsAdmin = false;
        }
    }

    public class ReservationResultsModel
    {
        public string ReservationInfo_ReservationID { get; set; }
        public string ReservationInfo_HotelConfirmationNumber { get; set; }
        public string ReservationInfo_FrontCertificateNumber { get; set; }
        public string ReservationInfo_ArrivalDate { get; set; }
        public string ReservationInfo_DestinationText { get; set; }
        public int ReservationInfo_ReservationStatus { get; set; }
        public string ReservationInfo_ReservationStatusText { get; set; }
        public bool? ReservationInfo_FoundInFront { get; set; }
        public string ReservationInfo_Distinctives { get; set; }
        public bool ReservationInfo_OptionsSold { get; set; }
        public List<AvailableLettersModel> ListAvailableLetters { get; set; }
    }

    public class FlightResultsModel
    {
        public string FlightInfo_FlightID { get; set; }
        public string FlightInfo_AirlineText { get; set; }
        public string FlightInfo_FlightNumber { get; set; }
        public string FlightInfo_PassengerNames { get; set; }
        public string FlightInfo_Passengers { get; set; }
        public string FlightInfo_FlightTypeText { get; set; }
        public string FlightInfo_FlightDateTime { get; set; }
        public string FlightInfo_FlightComments { get; set; }
    }

    public class ReservationResultModel
    {
        public string ReservationInfo_ReservationID { get; set; }
        public string ReservationInfo_Destination { get; set; }
        public string ReservationInfo_Place { get; set; }
        public string ReservationInfo_ReservationStatus { get; set; }
        public string ReservationInfo_RoomType { get; set; }
        public string ReservationInfo_RoomNumber { get; set; }
        public string ReservationInfo_HotelConfirmationNumber { get; set; }
        public string ReservationInfo_ArrivalDate { get; set; }
        public string ReservationInfo_DepartureDate { get; set; }
        public string ReservationInfo_NumberAdults { get; set; }
        public string ReservationInfo_NumberChildren { get; set; }
        public string ReservationInfo_FrontOfficeAgencyName { get; set; }
        public string ReservationInfo_TotalNights { get; set; }
        public string ReservationInfo_PlanType { get; set; }
        public string ReservationInfo_FrontPlanType { get; set; }
        public string ReservationInfo_TotalPaid { get; set; }
        public string ReservationInfo_ConfirmedTotalPaid { get; set; }
        public string ReservationInfo_DiamanteTotalPaid { get; set; }
        public string ReservationInfo_GreetingRep { get; set; }
        public bool ReservationInfo_IsSpecialOcassion { get; set; }
        public string ReservationInfo_SpecialOcassionComments { get; set; }
        public string ReservationInfo_ConciergeComments { get; set; }
        public string ReservationInfo_FrontComments { get; set; }
        public string ReservationInfo_ReservationComments { get; set; }
        public string ReservationInfo_ResortConnectReservationComments { get; set; }
        public string ReservationInfo_GuestsNames { get; set; }
        public bool ReservationInfo_RoomUpgraded { get; set; }
        public bool ReservationInfo_PreCheckIn { get; set; }
        public string ReservationInfo_FrontContractNumber { get; set; }
        public string ReservationInfo_FrontCertificateNumber { get; set; }
        public bool? ReservationInfo_FoundInFront { get; set; }
        //public int? ReservationInfo_SPITourID { get; set; }
        public PresentationResultModel PreArrivalPresentationsModel { get; set; }
        public List<FlightResultsModel> ListPreArrivalFlights { get; set; }
        public List<PaymentResultsModel> ListPreArrivalPayments { get; set; }
        public List<OptionsResultsModel> ListPreArrivalOptions { get; set; }
        public List<OptionsResultsModel> ListPreArrivalRCOptions { get; set; }
        public List<AvailableLettersModel> ListAvailableLetters { get; set; }
    }

    public class PresentationResultModel
    {
        public string PresentationInfo_PresentationID { get; set; }
        public string PresentationInfo_SecondaryBookingStatus { get; set; }
        public string PresentationInfo_FinalBookingStatus { get; set; }
        public string PresentationInfo_FinalTourStatus { get; set; }
        public string PresentationInfo_HostessComments { get; set; }
        public string PresentationInfo_RealTourDate { get; set; }
        public string PresentationInfo_DatePresentation { get; set; }
        public string PresentationInfo_TimePresentation { get; set; }
        public string PresentationInfo_TourStatus { get; set; }
        public string PresentationInfo_TourStatusText { get; set; }
        public int? PresentationInfo_SPITourID { get; set; }
    }
    public class PaymentResultsModel
    {
        public string PaymentInfo_PaymentDetailsID { get; set; }
        public string PaymentInfo_TransactionTypeText { get; set; }
        public string PaymentInfo_TransactionType { get; set; }
        public string PaymentInfo_PaymentType { get; set; }
        public string PaymentInfo_PaymentTypeText { get; set; }
        public string PaymentInfo_Amount { get; set; }
        public string PaymentInfo_Currency { get; set; }
        public string PaymentInfo_ChargeTypeText { get; set; }
        public string PaymentInfo_Invoice { get; set; }
        public string PaymentInfo_InvoiceToRefund { get; set; }
        public string PaymentInfo_ChargeDescriptionText { get; set; }
        public string PaymentInfo_PaymentComments { get; set; }
        public string PaymentInfo_DateSaved { get; set; }
        public string PaymentInfo_Status { get; set; }
        public bool PaymentInfo_PendingCharge { get; set; }
    }
    public class OptionsResultsModel
    {
        public string OptionInfo_OptionSoldID { get; set; }
        public string OptionInfo_Option { get; set; }
        public string OptionInfo_Quantity { get; set; }
        public string OptionInfo_Price { get; set; }
        public string OptionInfo_GuestNames { get; set; }
        public string OptionInfo_Eligible { get; set; }
        public string OptionInfo_CreditAmount { get; set; }
        public string OptionInfo_PointsRedemption { get; set; }
        public string OptionInfo_TotalPaid { get; set; }
        public string OptionInfo_Comments { get; set; }
    }

    public class SearchToImportModel
    {
        [Display(Name = "Arrival Date")]
        public string Search_I_ImportArrivalDate { get; set; }
        public string Search_F_ImportArrivalDate { get; set; }

        [Display(Name = "Resort")]
        public int[] SearchToImport_ImportResort { get; set; }

        public List<SelectListItem> Resorts
        {
            get
            {
                return PreArrivalDataModel.PreArrivalCatalogs.GetFrontResorts();
            }
        }

        public List<ImportModel> ListResults { get; set; }
    }

    public class ImportModel : FrontOfficeViewModel.LlegadasResult
    {
        public int Index { get; set; }
        public Guid? LeadID { get; set; }
        public string Resort { get; set; }
        public string LeadSource { get; set; }
        public Guid? AssignedToUserID { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string CheckIn { get; set; }
        //public List<KeyValuePair<long, Guid>> ListAssignation { get; set; }
        public List<object> ListAssignation { get; set; }
        public bool? Status { get; set; }
        public bool Import { get; set; }
        public string Message { get; set; }
        public string Tags { get; set; }
        public int Correo { get; set; }
        public int Phone { get; set; }
        public List<SelectListItem> LeadSources { get; set; }
        public List<SelectListItem> Users { get; set; }
        public ImportModel()
        {
            Status = null;
            Import = true;
            Message = "Not Processed";
        }
    }

    public class ImportResultsModel
    {
        //public int Resort { get; set; }
        public string Resort { get; set; }
        public List<KeyValuePair<string, SelectListItem>> UserCodeCountList { get; set; }
        public string a { get; set; }
        public List<FrontOfficeViewModel.LlegadasResult> lista { get; set; }
        public List<KeyValuePair<string, List<SelectListItem>>> Cosa { get; set; }
    }

    public class SearchToReassignModel
    {
        public bool Search { get; set; }
        public Guid? ReservationID { get; set; }
        public string ReservationToReassign { get; set; }
        [Display(Name = "Terminal")]
        public long? TerminalID { get; set; }
        [Display(Name ="Resort")]
        public long? PlaceID { get; set; }
        [Display(Name = "Arrival Date")]
        public DateTime? ArrivalDate { get; set; }
        [Display(Name = "Hotel Confirmation")]
        public string ConfirmationNumber { get; set; }
        [Display(Name = "CRS")]
        public string CRS { get; set; }
        public List<SelectListItem> Terminals
        {
            get
            {
                //var list = new List<SelectListItem>();
                //list.Add(new SelectListItem() { Value = "10", Text = "Pre-arrival RC" });
                //list.Add(new SelectListItem() { Value = "16", Text = "Pre-arrival TFR" });
                //list.Add(new SelectListItem() { Value = "80", Text = "Pre-arrival TVG" });
                var list = TerminalDataModel.GetCurrentUserTerminals();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        public List<SelectListItem> Resorts
        {
            get
            {
                return new List<SelectListItem>();
            }
        }
        public string Reservations { get; set; }

    }

    public class GroupLeadsModel
    {
        public string Leads { get; set; }

        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        public string GroupID { get; set; }
    }

    public class ReassignmentModel
    {
        public Guid? ReservationID { get; set; }
        public long? TerminalID { get; set; }
        public long? PlaceID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ArrivalDate { get; set; }
        public string ConfirmationNumber { get; set; }
        public string CRS { get; set; }
        public bool Options { get; set; }
        public bool Flights { get; set; }
        public bool Payments { get; set; }
        public bool Status { get; set; }

        public ReassignmentModel()
        {
            Options = false;
            Flights = false;
            Payments = false;
            Status = false;
        }
    }
}
