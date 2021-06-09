
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using System.IO;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.Web.Security;
using System.Text.RegularExpressions;
using ePlatBack.Models.Utils;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class MasterChartViewModel
    {
        public LeadModel.Views.Search leadSearch { get; set; }
        public LeadModel.Views.LeadGeneralInformation leadGeneralInformation { get; set; }
        public LeadModel.Views.MassUpdate massUpdate { get; set; }
        public BillingInfoModel.Views.BillingInfo billingInformation { get; set; }
        public HotelReservationModel.Views.ReservationGeneralInformation hotelReservations { get; set; }
        public LeadModel.Views.ReservationCharges reservationCharges { get; set; }
        public PurchasesModel.PurchaseInfoModel PurchaseInfoModel { get; set; }

        //public AccountancyModel AccountancyModel { get; set; }
        public TimeShareViewModel TimeShareModel { get; set; }
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }

        public class EgressTypeModel
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public long[] Terminals { get; set; }
        }
    }

    public class LeadModel
    {
        public class Fields
        {
            [LogReference(Name = "Lead")]
            [DataBaseInfo(Name = "tblLeads")]
            public class LeadGeneralInformation
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "leadID")]
                public Guid? GeneralInformation_LeadID { get; set; }

                [Display(Name = "Duplicated Lead Assigned To")]
                [DataBaseInfo(Name = "assignedToUserID")]
                public Guid GeneralInformation_AssignedToUserID { get; set; }

                public DateTime? GeneralInformation_AssignationDate { get; set; }

                [Display(Name = "Title")]
                [DataBaseInfo(Name = "personalTitleID")]
                public int? GeneralInformation_Title { get; set; }

                #region "GeneralInformation_FirstName"
                [Required(ErrorMessage = "First Name is required.")]
                [Display(Name = "First Name")]
                [DataBaseInfo(Name = "firstName")]
                [LogReference]
                #endregion
                public string GeneralInformation_FirstName { get; set; }
                #region "GeneralInformation_LastName"
                [Required(ErrorMessage = "Last Name is required.")]
                [Display(Name = "Last Name")]
                [DataBaseInfo(Name = "lastName")]
                [LogReference]
                #endregion
                public string GeneralInformation_LastName { get; set; }
                #region "GeneralInformation_Email"
                //[ScriptIgnore]
                [RegularExpression("[\\S].{1,}", ErrorMessage = "You have to speficy at least 1 email.")]
                [Required(ErrorMessage = "You have to specify at least 1 email.")]
                [ListType(typeof(Items.Emails))]
                [ParseBackToObject]
                [DataBaseInfo(RelationShipType = typeof(tblLeadEmails),
                    PrimaryKeyModelName = "emailID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "emailID"
                    )
                ]
                #endregion
                public object GeneralInformation_EmailsList { get; set; }
                #region "GeneralInformation_Phone"
                //[ScriptIgnore]
                //[RegularExpression("^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "You have to speficy at least 1 phone number.")]
                [Required(ErrorMessage = "You have to specify at least 1 phone.")]
                [ParseBackToObject]
                [ListType(typeof(LeadModel.Items.Phones))]
                [DataBaseInfo(RelationShipType = typeof(tblPhones),
                    PrimaryKeyModelName = "phoneID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "phoneID"
                    )
                ]
                #endregion
                public object GeneralInformation_PhonesList { get; set; }
                #region "GeneralInformation_TerminalID"
                [Display(Name = "Terminal")]
                [Range(1, int.MaxValue, ErrorMessage = "Terminal is required.")]
                [DataBaseInfo(Name = "terminalID")]
                #endregion
                public long GeneralInformation_TerminalID { get; set; }
                [DoNotUpdate, DoNotTrackChanges]
                [DataBaseInfo(Name = "inputMethodID")]
                public int GeneralInformation_InputMethodID { get; set; }
                #region "GeneralInformation_LeadStatusID"
                [Range(1, int.MaxValue, ErrorMessage = "Lead Status is required.")]
                [Display(Name = "Lead Status")]
                [DataBaseInfo(Name = "leadStatusID")]
                #endregion
                public int GeneralInformation_LeadStatusID { get; set; }
                [DataBaseInfo(Name = "leadStatusDescription")]
                [Display(Name = "Lead Status Description")]
                public string GeneralInformation_LeadStatusDescription { get; set; }
                #region "GeneralInformation_LeadSourceID"
                [Display(Name = "Lead Source")]
                [DataBaseInfo(Name = "leadSourceID")]
                #endregion
                public long? GeneralInformation_LeadSourceID { get; set; }
                #region "GeneralInformation_BookingStatusID"
                [Display(Name = "Booking Status")]
                [DataBaseInfo(Name = "bookingStatusID")]
                #endregion
                public int? GeneralInformation_BookingStatusID { get; set; }
                [Display(Name = "Time Zone")]
                [DataBaseInfo(Name = "timeZoneID")]
                public int GeneralInformation_TimeZoneID { get; set; }


                public int? GeneralInformation_LastInteractionTypeID { get; set; }

                #region "GeneralInformation_CallClasificationID"
                [Display(Name = "Call Clasification")]
                [DataBaseInfo(Name = "callClasificationID")]
                #endregion
                public int? GeneralInformation_CallClasificationID { get; set; }

                [Display(Name = "Confirmed")]
                [DataBaseInfo(Name = "confirmed")]
                public bool GeneralInformation_Confirmed { get; set; }
                [Display(Name = "Submission Form")]
                [DataBaseInfo(Name = "submissionForm")]
                public bool GeneralInformation_SubmissionForm { get; set; }
                [Display(Name = "Activity Cert")]
                [DataBaseInfo(Name = "activityCert")]
                public bool GeneralInformation_ActivityCert { get; set; }

                [Display(Name = "Options Tour Discount")]
                [DataBaseInfo(Name = "optionsTourDiscount")]
                public bool GeneralInformation_OptionsTourDiscount { get; set; }
                //member info
                //[ScriptIgnore]
                private Fields.MemberInfo _GeneralInformation_MemberInformation = new Fields.MemberInfo();
                //[ScriptIgnore]
                [DataBaseInfo(RelationShipType = typeof(tblMemberInfo), PrimaryKeyModelName = "memberInfoID", IsRelationShip = true, Cardinality = Utils.DataBaseRelationShipCardinality.OneToOne)]
                public Fields.MemberInfo GeneralInformation_MemberInformation
                {
                    get { return _GeneralInformation_MemberInformation; }
                    set { _GeneralInformation_MemberInformation = value; }
                }


                //Referral Info
                //[ScriptIgnore]
                private Fields.ReferralInfo _GeneralInformation_ReferralInformation = new Fields.ReferralInfo();
                //[ScriptIgnore]
                [DataBaseInfo(RelationShipType = typeof(tblReferralInformation), PrimaryKeyModelName = "referralInfoID", IsRelationShip = true, Cardinality = Utils.DataBaseRelationShipCardinality.OneToOne)]
                public Fields.ReferralInfo GeneralInformation_ReferralInformation
                {
                    get { return _GeneralInformation_ReferralInformation; }
                    set { _GeneralInformation_ReferralInformation = value; }
                }

                //Last Contract Info
                //[ScriptIgnore]
                private Fields.LastContractInfo _GeneralInformation_LastContractInformation = new Fields.LastContractInfo();
                //[ScriptIgnore]
                [DataBaseInfo(RelationShipType = typeof(tblLastContractInfo), PrimaryKeyModelName = "lastContractInfoID", IsRelationShip = true, Cardinality = Utils.DataBaseRelationShipCardinality.OneToOne)]
                public Fields.LastContractInfo GeneralInformation_LastContractInformation
                {
                    get { return _GeneralInformation_LastContractInformation; }
                    set { _GeneralInformation_LastContractInformation = value; }
                }

                // siempre no lo hice como parte de leads, lo hice de la misma manera que una reservacion, es decir, como  un objeto separado.
                //#region "GeneralInformation_BillingInfo"
                ////[RegularExpression("[\\S].{1,}", ErrorMessage = "You have to speficy at least 1 email.")]
                ////[Required(ErrorMessage = "You have to specify at least 1 email.")]
                //[ListType(typeof(Items.BillingInfo))]
                //[ParseBackToObject]
                //[DataBaseInfo(RelationShipType = typeof(tblBillingInfo),
                //    PrimaryKeyModelName = "BillingInfoID",
                //    IsRelationShip = true,
                //    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                //    PrimaryKeyDatabaseName = "billingInfoID"
                //    )
                //]
                //#endregion
                //public object GeneralInformation_BillingInfoList { get; set; }

                //
                [DataBaseInfo(Name = "leadDescription")]
                [Display(Name = "Lead Description")]
                public string GeneralInformation_LeadDescription { get; set; }

                #region "GeneralInformation_Interactions"
                //[ScriptIgnore]
                [ListType(typeof(Items.Interactions))]
                [ParseBackToObject]
                [DataBaseInfo(RelationShipType = typeof(tblInteractions),
                    PrimaryKeyModelName = "InteractionID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "interactionID"
                    )
                ]
                #endregion
                public object GeneralInformation_InteractionsList { get; set; }


                [DoNotUpdate, DoNotTrackChanges]
                [DataBaseInfo(Name = "referredByID")]
                public Guid? GeneralInformation_ReferredByID { get; set; }


                #region "GeneralInformation_CountryID"
                [Display(Name = "Country")]
                [DataBaseInfo(Name = "countryID")]
                #endregion
                public int? GeneralInformation_countryID { get; set; }

                #region "GeneralInformation_Address"
                [Display(Name = "Address")]
                [DataBaseInfo(Name = "address")]
                #endregion
                public string GeneralInformation_Address { get; set; }

                #region "GeneralInformation_City"
                [Display(Name = "City")]
                [DataBaseInfo(Name = "city")]
                #endregion
                public string GeneralInformation_City { get; set; }

                #region "GeneralInformation_State"
                [Display(Name = "State")]
                [DataBaseInfo(Name = "state")]
                #endregion
                public string GeneralInformation_State { get; set; }

                #region "GeneralInformation_ZipCode"
                [Display(Name = "Zip Code")]
                [DataBaseInfo(Name = "zipcode")]
                #endregion
                public string GeneralInformation_ZipCode { get; set; }

                [DataBaseInfo(Name = "leadComments")]
                [Display(Name = "Lead Comments")]
                public string GeneralInformation_LeadComments { get; set; }

                //Manifest Fields
                [DataBaseInfo(Name = "spiCustomerID")]
                public long? SpiCustomerID { get; set; }

                [DataBaseInfo(Name = "frontOfficeGuestID")]
                public int? SpiFrontOfficeGuestID { get; set; }

                [DataBaseInfo(Name = "frontOfficeResortID")]
                public int? SpiFrontOfficeResortID { get; set; }

                //#region "GeneralInformation_BillingInfo"
                //[ListType(typeof(Items.BillingInfo))]
                //[ParseBackToObject]
                //[DataBaseInfo(RelationShipType = typeof(tblBillingInfo),
                //    PrimaryKeyModelName = "BillingInfoID",
                //    IsRelationShip = true,
                //    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                //    PrimaryKeyDatabaseName = "billingInfoID"
                //    )
                //]
                //#endregion

                #region "GeneralInformation_BrokerContract"
                [Display(Name = "Broker Contract")]
                [DataBaseInfo(Name = "brokerContractID")]
                #endregion
                public int? GeneralInformation_brokerContractID { get; set; }

                [DataBaseInfo(Name = "reservationMadeDate")]
                [Display(Name = "Reservation Made Date")]
                public DateTime? GeneralInformation_ReservationMadeDate { get; set; }

                public bool GeneralInformation_DuplicateLead { get; set; }
                public LeadGeneralInformation()
                {
                    GeneralInformation_DuplicateLead = false;
                }
            }

            [LogReference(Name = "Lead Member Information")]
            public class MemberInfo
            {
                [IsPrimaryKey]
                [DataBaseInfo(Name = "memberInfoID")]
                public long MemberInformationID { get; set; }
                //[DataBaseInfo(Name = "clubTypeID")]
                //[Display(Name = "Club Type")]
                //public int ClubTypeID { get; set; }
                [DataBaseInfo(Name = "clubType")]
                [Display(Name = "Club Type")]
                public string ClubType { get; set; }
                [DataBaseInfo(Name = "memberNumber")]
                [Display(Name = "Member Number")]
                public string MemberNumber { get; set; }
                [DataBaseInfo(Name = "contractNumber")]
                [Display(Name = "Contract Number")]
                public string ContractNumber { get; set; }
                [Display(Name = "Is National")]
                [DataBaseInfo(Name = "isNational")]
                public bool IsNational { get; set; }
                [DataBaseInfo(Name = "presentationConfirmed")]
                [Display(Name = "Presentation Confirmed")]
                public bool PresentationConfirmed { get; set; }
                [DataBaseInfo(Name = "isVIP")]
                [Display(Name = "Is VIP")]
                public bool IsVIP { get; set; }
                [DataBaseInfo(Name = "pushedToOnSiteConcierge")]
                [Display(Name = "Pushed To On-Site Concierge")]
                public bool PushedToOnSiteConcierge { get; set; }
                [DataBaseInfo(Name = "isAllInclusive")]
                [Display(Name = "Is All Inclusive")]
                public bool IsAllInclusive { get; set; }
                [DataBaseInfo(Name = "needTransportation")]
                [Display(Name = "Need Transportation")]
                public bool NeedsTransportation { get; set; }
                [DataBaseInfo(Name = "hasOptions")]
                [Display(Name = "Has Options")]
                public bool HasOptions { get; set; }
                [DataBaseInfo(Name = "coOwner")]
                [Display(Name = "Co Owner")]
                public string CoOwner { get; set; }
                [DataBaseInfo(Name = "memberName")]
                [Display(Name = "Member Name")]
                public string MemberName { get; set; }
            }

            [LogReference(Name = "Lead Referral Information")]
            public class ReferralInfo
            {
                [IsPrimaryKey]
                [DataBaseInfo(Name = "referralInformationID")]
                public long ReferralInformationID { get; set; }

                [DataBaseInfo(Name = "referralRelationShip")]
                [Display(Name = "Relationship")]
                public string ReferralRelationShip { get; set; }

                [DataBaseInfo(Name = "referredByComment")]
                [Display(Name = "Referred-By Comment")]
                public string ReferredByComment { get; set; }
            }


            [LogReference(Name = "Lead Last Contract Information")]
            public class LastContractInfo
            {
                [IsPrimaryKey]
                [DataBaseInfo(Name = "lastContractInfoID")]
                public long LastContractInfoID { get; set; }


                [DataBaseInfo(Name = "travelDate")]
                [Display(Name = "Travel Date")]
                public DateTime? TravelDate { get; set; }
                [DataBaseInfo(Name = "presentationDate")]
                [Display(Name = "Presentation Date")]
                public DateTime? PresentationDate { get; set; }
                [DataBaseInfo(Name = "upgradeDate")]
                [Display(Name = "Upgrade Date")]
                public DateTime? UpgradeDate { get; set; }
                [DataBaseInfo(Name = "upgradeVolume")]
                [Display(Name = "Upgrade Volume")]
                public Decimal? UpgradeVolume { get; set; }
                [DataBaseInfo(Name = "contractNumber")]
                [Display(Name = "Contract Number")]
                public string ContractNumber { get; set; }
                [DataBaseInfo(Name = "salesCenter")]
                [Display(Name = "Sales Center")]
                public string SalesCenter { get; set; }
                [DataBaseInfo(Name = "legacyKey")]
                [Display(Name = "Legacy Key")]
                public string LegacyKey { get; set; }
                [DataBaseInfo(Name = "usageType")]
                [Display(Name = "Usage Type")]
                public string UsageType { get; set; }
                [DataBaseInfo(Name = "category")]
                [Display(Name = "Category")]
                public string Category { get; set; }
                [DataBaseInfo(Name = "season")]
                [Display(Name = "Season")]
                public string Season { get; set; }



            }



            //public class PaymetDetails { 

            //}
            //[DataBaseInfo(Name = "tblBillingInfo")]
            //[LogReference(Name = "Billing Information")]

        }
        public class Items
        {
            [LogReference(Name = "Phone")]
            [DataBaseInfo(Name = "tblPhones")]
            public class Phones
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long? phoneID { get; set; }
                [LogReference]
                public string phone { get; set; }
                public string ext { get; set; }
                public int phoneTypeID { get; set; }
                public bool main { get; set; }
                public bool doNotCall { get; set; }
            }
            [LogReference(Name = "Email")]
            [DataBaseInfo(Name = "tblLeadEmails")]
            public class Emails
            {
                [
                DoNotUpdate,
                DoNotTrackChanges,
                IsPrimaryKey
                ]
                public long? emailID { get; set; }
                [LogReference]
                public string email { get; set; }
                public bool main { get; set; }

            }
            [LogReference(Name = "SharedLead")]
            [DataBaseInfo(Name = "tblSharedLeads")]
            public class SharedLeads
            {
                [DoNotUpdate,
                DoNotTrackChanges,
                IsPrimaryKey
                ]
                public Guid? sharedLeadID { get; set; }
                public Guid assignedToUserID { get; set; }
                public Guid assignedByUserID { get; set; }
                public DateTime dateAssigned { get; set; }
            }
            [LogReference(Name = "Interaction")]
            [DataBaseInfo(Name = "tblInteractions")]
            public class Interactions
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "interactionID")]
                public long? InteractionID { get; set; }
                //public Guid LeadID { get; set;  }
                [OnValueChanged(Action = Utils.OnValueChangedActions.Update, ValueType = Utils.AutomaticValueTypes.CurrentValue, Target = "bookingStatusID")]
                [DataBaseInfo(Name = "bookingStatusID")]
                public int BookingStatusID { get; set; }
                [OnValueChanged(Action = Utils.OnValueChangedActions.Update, ValueType = Utils.AutomaticValueTypes.CurrentValue, Target = "lastInteractionTypeID")]
                [DataBaseInfo(Name = "interactionTypeID")]
                public int InteractionTypeID { get; set; }
                [DataBaseInfo(Name = "interactionComments")]
                public string InteractionComments { get; set; }
                [DataBaseInfo(Name = "savedByUserID")]
                [AutomaticValue(
                    AutomaticValueType = Utils.AutomaticValueTypes.CurrentUser,
                    ValuePath = "aspnet_Users1.UserName")
                    //ValuePath = "aspnet_Users.UserId")
                ]
                [DoNotTrackChanges, DoNotUpdate]
                public object SavedByUserID { get; set; }
                //public Guid? SavedByUserID { get; set; }
                [DataBaseInfo(Name = "interactedWithUserID")]
                public Guid InteractedWithUserID { get; set; }
                // // //
                private DateTime? interactionDateSaved;
                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? InteractionDateSaved
                {
                    get { return interactionDateSaved; }
                    set
                    {
                        if (value.ToString().Contains("/Date("))
                        {
                            string fecha = value.ToString().Replace("/Date(", "").Replace(")/", "");
                            long fechaLong = long.Parse(fecha);
                            DateTime dt = new DateTime(fechaLong * 10000);
                            interactionDateSaved = dt;
                        }
                        else { interactionDateSaved = value; }

                        //Match match = Regex.Match(value.ToString()

                        //interactionDateSaved = DateTime.SpecifyKind(value, DateTimeKind.Utc); 
                    }
                }

            }

            public class LeadReferralInformationModel
            {
                public string relatedTable
                {
                    get
                    {
                        return "tblLeads";
                    }
                }
                public int LeadReferralInformation_LeadStatusID { get; set; }

                public string LeadReferralInformation_TimeZoneID { get; set; }
            }

            public class SearchResults : PurchasesModel.PurchaseInfoModel
            {
                public Guid LeadID { get; set; }
                [Display(Name = "First Name")]
                public string FirstName { get; set; }
                [Display(Name = "Last Name")]
                public string LastName { get; set; }
                [Display(Name = "Email")]
                public string Email { get; set; }

                [Display(Name = "Phone")]
                public string Phone { get; set; }

                [Display(Name = "Booking Status")]
                public string BookingStatus { get; set; }

                [Display(Name = "Lead Status")]
                public string LeadStatus { get; set; }

                [Display(Name = "Lead Source")]
                public string LeadSource { get; set; }

                [Display(Name = "Arrival Date")]
                [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
                public DateTime? ArrivalDate { get; set; }

                [Display(Name = "Assigned To")]
                public string AssignedTo { get; set; }

                [Display(Name = "Lead Comments")]
                public string LeadComments { get; set; }

                [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
                [Display(Name = "Last Modification Date")]
                public DateTime? LastModificationDate { get; set; }

                [Display(Name = "Resort")]
                public string Resort { get; set; }

                [Display(Name = "Total Paid")]
                public decimal? TotalPaid { get; set; }

                [Display(Name = "Member Account")]
                public string MemberAccount { get; set; }

                [Display(Name = "Presentation Datetime")]
                public string PresentationDateTime { get; set; }
                public DateTime? PresentationDateTime_ { get; set; }
                //public Guid? PurchaseID { get; set; }

                //[Display(Name = "Purchase Status")]
                //public string PurchaseStatus { get; set; }

                //[Display(Name = "Purchase Date")]
                //public DateTime? PurchaseDate { get; set; }
                //public string PurchaseDateTime { get; set; }

                //[Display(Name = "Total")]
                //public decimal? Total { get; set; }

                //[Display(Name = "Currency")]
                //public string Currency { get; set; }

                //[Display(Name = "Agent")]
                //public string Agent { get; set; }

                //[Display(Name = "Point Of Sale")]
                //public string PointOfSale { get; set; }

                //[Display(Name = "Terminal")]
                //public string Terminal { get; set; }
                [Display(Name = "Input Date & Time")]
                public DateTime InputDateTime { get; set; }
                [Display(Name = "Input By User")]
                public string InputByUser { get; set; }

                [Display(Name = "Assignation Date")]
                public DateTime? AssignationDate { get; set; }

                public int TotalCoincidences { get; set; }
            }

            public class Search
            {
                public List<SearchResults> SearchResults { get; set; }
                //public IQueryable<SearchResults> SearchResults { get; set; }

                public string Coincidences { get; set; }
            }

            public class ReservationCharges
            {

                [DataBaseInfo(Name = "paymentDetailsID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long ReservationPaymentDetailsID { get; set; }

                [Display(Name = "Amount")]
                [DataBaseInfo(Name = "amount")]
                public decimal? Amount { get; set; }
                [Display(Name = "Net Center Cost")]
                [DataBaseInfo(Name = "netCenterCost")]
                public decimal? NetCenterCost { get; set; }
                [Display(Name = "Net Center Charge")]
                [DataBaseInfo(Name = "netCenterCharge")]
                public decimal? NetCenterCharge { get; set; }
                [Display(Name = "Payment Comments")]
                [DataBaseInfo(Name = "paymentComments")]
                public string PaymentComments { get; set; }
                [DataBaseInfo(Name = "refundAccount")]
                [Display(Name = "Refund Account")]
                public string RefundAccount { get; set; }
                //public DateTime? TransactionDate { get; set; }

                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime DateSaved { get; set; }
                //-----------------------
                [Display(Name = "Certificate Number")]
                [DataBaseInfo(Name = "certificateNumber")]
                public string CertificateNumber { get; set; }

                [Display(Name = "Hotel Confirmation Number")]
                [DataBaseInfo(Name = "hotelConfirmationNumber")]
                public string HotelConfirmationNumber { get; set; }

                [Display(Name = "Charge Description")]
                [DataBaseInfo(Name = "chargeDescription")]
                public string ChargeDescription { get; set; }

                [Display(Name = "Charge Type")]
                [DataBaseInfo(Name = "chargeType")]
                public string ChargeType { get; set; }



                [Display(Name = "Error Code")]
                [DataBaseInfo(Name = "errorCode")]
                public string ErrorCode { get; set; }

                [DataBaseInfo(Name = "transactionDate")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? AttemptDate { get; set; }

                [DataBaseInfo(Name = "authDate")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? AuthDate { get; set; }

                [Display(Name = "Invoice Number")]
                [DataBaseInfo(Name = "authCode")]
                public string AuthCode { get; set; }

            }

            public class ReservationPendingCharges : ReservationCharges
            {

                public string ToBeBilledBy { get; set; }
                public int ToBeBilledByID { get; set; }
            }


        }
        public class Import
        {
            public class LeadsModel
            {
                public Guid LeadID { get; set; }

                //[Required(ErrorMessage = "Initial Terminal is required")]
                public Int64 InitialTerminalID { get; set; }
                //[Required(ErrorMessage = "Terminal is required")]
                public Int64 TerminalID { get; set; }
                //[Required(ErrorMessage = "First Name is required")]
                public string FirstName { get; set; }
                //[Required(ErrorMessage = "Last Name is required")]
                public string LastName { get; set; }
                public string City { get; set; }
                public string State { get; set; }
                public int CountryID { get; set; }
                public string Email { get; set; }
                public string SpouseFirstName { get; set; }
                public string SpouseLastName { get; set; }
                public string Address { get; set; }
                public string ZipCode { get; set; }
                public int LeadTypeID { get; set; }
                public int BookingStatusID { get; set; }
                public int QualificationStatusID { get; set; }
                public int LeadSourceID { get; set; }
                //[Required]
                public Guid InputByUserID { get; set; }
                public Guid AssignedToUserID { get; set; }
                public Guid ModifiedByUserID { get; set; }
                //[Required]
                public DateTime InputDateTime { get; set; }
                public DateTime AssignationDate { get; set; }
                public DateTime ModificationDate { get; set; }
                public Guid ReferredByID { get; set; }
                public int OfferID { get; set; }
                public string leadGenerator { get; set; }
                public string ToSatus { get; set; }
                public string ToCloser { get; set; }
                public string LeadComments { get; set; }
                //[Required]
                public int InputMethodID { get; set; }
                //[Required]
                public bool Deleted { get; set; }
                public Guid DeletedByUserID { get; set; }
                //[Required]
                public bool IsTest { get; set; }
                public DateTime DateDeleted { get; set; }
                public string LeadStatusDescription { get; set; }
                public Int64 MemberID { get; set; }
                //[Required(ErrorMessage = "Lead Status is required")]
                public int LeadStatusID { get; set; }
                public int TimeZoneID { get; set; }
                public LeadsModel()
                {

                }
            }

            public class ImportRowsModel
            {
                public string Header { get; set; }
                public string Value { get; set; }
            }
            public class FieldsPerTableModel
            {
                public string Key { get; set; }
                public string Value { get; set; }

            }
        }
        public class Views
        {
            public bool ManageReservations
            {
                get
                {
                    return new MasterChartDataModel.Lead().View();
                }
            }

            public class SearchCoupons
            {
                [Display(Name = "Coupon Folio")]
                //[RequiredIf(OtherProperty = "SearchCoupon_PurchaseDate", EqualsTo = null, ErrorMessage = "Coupon Folio is required")]
                public string SearchCoupon_Coupon { get; set; }

                [Display(Name = "Sales From Date")]
                [RequiredIf(OtherProperty = "SearchCoupon_Coupon", EqualsTo = null, ErrorMessage = "Sales Date is required")]
                public string SearchCoupon_PurchaseDate { get; set; }
                [Display(Name = "Point Of Sale")]
                public int SearchCoupon_PointOfSale { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> SearchCoupon_DrpPointsOfSale
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                    }
                }

                //fields to search by service date
                [Display(Name = "Provider")]
                //public string SearchCouponByDate_Provider { get; set; }
                public int[] SearchCouponByDate_Provider { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> SearchCouponByDate_DrpProviders
                {
                    get
                    {
                        var list = new PurchasesModel.PurchaseInfoModel().Providers;
                        return list;
                    }
                }

                [Display(Name = "Point Of Sale")]
                public int[] SearchCouponByDate_PointOfSale { get; set; }

                [Display(Name = "Service")]
                //public string SearchCouponByDate_Service { get; set; }
                public long[] SearchCouponByDate_Service { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> SearchCouponByDate_DrpServices
                {
                    get
                    {
                        var list = new PurchasesModel.PurchaseInfoModel().Services;
                        return list;
                    }
                }

                [Display(Name = "Service Date")]
                [Required(ErrorMessage = "Service Date is required")]
                public string SearchCouponByDate_I_ServiceDate { get; set; }
                public string SearchCouponByDate_F_ServiceDate { get; set; }

                //[Display(Name = "Status")]
                //public int[] SearchCoupon_ServiceStatus { get; set; }
                //public List<SelectListItem> SearchCoupon_DrpServiceStatus
                //{
                //    get
                //    {
                //        return MasterChartDataModel.LeadsCatalogs.FillDrpServiceStatus();
                //    }
                //}
            }

            public class Search : SearchCoupons
            {
                [ScriptIgnore]
                public bool ShowReservations
                {
                    get
                    {
                        return new MasterChartDataModel.Lead().View();
                    }
                }
                
                [ScriptIgnore]
                public bool ShowServices
                {
                    get
                    {
                        return new MasterChartDataModel.Purchases().View();
                    }
                }

                public Guid? Search_LeadID { get; set; }

                [Display(Name = "First Name")]
                [DataBaseInfo(Name = "tblLeads.firstName")]
                public string Search_FirstName { get; set; }

                [Display(Name = "Last Name")]
                public string Search_LastName { get; set; }

                [Display(Name = "Email")]
                public string Search_Email { get; set; }

                [Display(Name = "Phone")]
                public string Search_Phone { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> BookingStatus
                {
                    get
                    {
                        //List<SelectListItem> BookingStatuses = MasterChartDataModel.LeadsCatalogs.FillDrpBookingStatus(false);
                        List<SelectListItem> BookingStatuses = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                        //the method FillDrpBookingStatus has already the "--Select One--" option
                        BookingStatuses.Insert(1, Utils.ListItems.NotSet());
                        return BookingStatuses;
                    }
                }

                [Display(Name = "Booking Status")]
                public int[] Search_BookingStatus { get; set; }

                [ScriptIgnore]
                public List<SelectListItem> Search_DrpBookingStatus
                {
                    get
                    {
                        return BookingStatus;
                    }
                }

                [Display(Name = "Interacted With User")]
                public Guid[] Search_InteractedWithUser { get; set; }

                [ScriptIgnore]
                public List<SelectListItem> Search_DrpInteractedWithUser
                {
                    get;set;
                    //get
                    //{
                    //    return Search_Subordinates;
                    //}
                }

                [Display(Name = "Interaction Booking Status")]
                public int[] Search_InteractionBookingStatus { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpInteractionBookingStatus
                {
                    get
                    {
                        return BookingStatus;
                    }
                }

                [Display(Name = "Lead Status")]
                public int[] Search_LeadStatus { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpLeadStatus
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpLeadStatus(false);
                    }
                }

                [Display(Name = "Lead Status Description")]
                public string Search_LeadStatusDescription { get; set; }

                [Display(Name = "Lead Source")]
                public long[] Search_LeadSource { get; set; }

                [ScriptIgnore]
                public List<SelectListItem> Search_DrpLeadSources
                {
                    get
                    {
                        //return LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                        return LeadSourceDataModel.GetLeadSourcesByTerminal();
                    }
                }

                [Display(Name = "Arrival Date Range")]
                //public string Search_ArrivalDateStart { get; set; }
                public string Search_I_ArrivalDate { get; set; }
                public string Search_F_ArrivalDate { get; set; }


                // prearrival needs

                [Display(Name = "Confirmation Number")]
                public string Search_ConfirmationNumber { get; set; }

                [Display(Name = "Resort")]
                public long[] Search_Resort { get; set; }

                [ScriptIgnore]
                public List<SelectListItem> Search_DrpResorts
                {
                    get
                    {
                        List<SelectListItem> Resorts = PlaceDataModel.GetResortsByProfile();
                        //Resorts.Insert(0, Utils.ListItems.Default());
                        return Resorts;
                    }
                }

                //the list 'Search_Subordinates' is used to feed more than one DropDownList.
                //****************
                //[ScriptIgnore]
                //public List<SelectListItem> Search_Subordinates
                //{
                //    get
                //    {
                //        var Users = UserDataModel.GetUsersBySupervisor(null, true);
                //        Users.Insert(0, ListItems.Default());
                //        return Users;
                //        //MembershipUser CurrentUser = Membership.GetUser();
                //        //List<SelectListItem> Users = new List<SelectListItem>();
                //        //if (GeneralFunctions.IsUserInRole("Administrator", (Guid)CurrentUser.ProviderUserKey))
                //        //{
                //        //    Users = UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup();
                //        //}
                //        //else
                //        //{
                //        //    Users = UserDataModel.GetUsersBySupervisor((Guid)CurrentUser.ProviderUserKey, true);
                //        //}
                //        ////Users.Insert(0, Utils.ListItems.Default());
                //        //return Users;
                //    }
                //}
                //****************


                [Display(Name = "Assigned To")]
                public Guid[] Search_AssignedTo { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpAssignedTo
                {
                    get;set;
                    //get
                    //{
                    //    return Search_Subordinates;
                    //}
                }
                [Display(Name = "Club Type")]
                public string Search_ClubType { get; set; }

                [Display(Name = "Member Number")]
                public string Search_MemberNumber { get; set; }

                [Display(Name = "Tour Date Range")]
                //public DateTime? Search_TourDateStart { get; set; }
                public DateTime? Search_I_TourDate { get; set; }
                public DateTime? Search_F_TourDate { get; set; }

                [Display(Name = "Is VIP")]
                public bool[] Search_IsVip { get; set; }

                public List<SelectListItem> Search_DrpIsVip
                {
                    get
                    {
                        List<SelectListItem> IsVIP = Utils.ListItems.Booleans();
                        //IsVIP.Insert(0, Utils.ListItems.Default());
                        return IsVIP;
                    }
                }

                [Display(Name = "Pre Check In")]
                public bool[] Search_PreCheckIn { get; set; }
                public List<SelectListItem> Search_DrpPreCheckIn
                {
                    get
                    {
                        return ListItems.Booleans();
                    }
                }

                [Display(Name = "Interaction Type")]
                public int[] Search_LastInteractionType { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpInteractionTypes
                {
                    get
                    {
                        List<SelectListItem> InteractionTypes = InteractionDataModel.GetAllInteractionTypes();
                        InteractionTypes.Insert(0, Utils.ListItems.Default("[None]"));
                        return InteractionTypes;
                    }
                }

                //[Display(Name = "Call Clasification")]
                //public int[] Search_CallClasification { get; set; }
                //public List<SelectListItem> Search_DrpCallClasification
                //{
                //    get
                //    {
                //        List<SelectListItem> CallClasifications = MasterChartDataModel.LeadsCatalogs.FillDrpCallsClasification();
                //        CallClasifications.Insert(0, ListItems.Default("[None]"));
                //        return CallClasifications;
                //    }
                //}

                [Display(Name = "Submission Form")]
                public object[] Search_SubmissionForm { get; set; }
                public List<SelectListItem> Search_DrpSubmissionForm
                {
                    get
                    {
                        List<SelectListItem> SubmissionForm = Utils.ListItems.Booleans();
                        //SubmissionForm.Insert(0, Utils.ListItems.Default());
                        SubmissionForm.Insert(1, Utils.ListItems.NotSet());
                        return SubmissionForm;
                    }
                }

                [Display(Name = "Activity Cert")]
                public object[] Search_ActivityCert { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpActivityCert
                {
                    get
                    {
                        List<SelectListItem> ActivityCert = Utils.ListItems.Booleans();
                        ActivityCert.Insert(1, Utils.ListItems.NotSet());
                        return ActivityCert;
                    }
                }

                [Display(Name = "Time Zone")]
                public int[] Search_TimeZone { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpTimeZones
                {
                    get
                    {
                        List<SelectListItem> TimeZones = TimeZoneDataModel.GetAllTimeZones();
                        //TimeZones.Insert(0,Utils.ListItems.Default());
                        return TimeZones;
                    }
                }

                [Display(Name = "Real Tour Date Range")]
                //public DateTime? Search_RealTourDateStart { get; set; }
                public DateTime? Search_I_RealTourDate { get; set; }
                public DateTime? Search_F_RealTourDate { get; set; }

                //***********************
                // the list 'TourStatus' is used to feed more than one DropDownList
                [ScriptIgnore]
                public List<SelectListItem> TourStatus
                {
                    get
                    {
                        List<SelectListItem> TourStatuses = TourStatusDataModel.GetAlltourStatus();
                        //TourStatuses.Insert(0, Utils.ListItems.Default());
                        return TourStatuses;
                    }
                }
                //***************

                [Display(Name = "Call Clasification")]
                public int[] Search_CallClasification { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpCallClasifications
                {
                    get
                    {
                        //var list = MasterChartDataModel.LeadsCatalogs.FillDrpCallClasifications();
                        var list = MasterChartDataModel.LeadsCatalogs.FillDrpCallsClasification();
                        //list.RemoveAt(0);
                        return list;
                    }
                }

                [Display(Name = "Tour Status")]
                public int[] Search_TourStatus { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpTourStatus
                {
                    get
                    {
                        return TourStatus;
                    }
                }

                [Display(Name = "Sale Date Range")]
                //public DateTime? Search_SaleDateStart { get; set; }
                public DateTime? Search_I_SaleDate { get; set; }
                public DateTime? Search_F_SaleDate { get; set; }

                [Display(Name = "Final Tour Status")]
                public int[] Search_FinalTourStatus { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpFinalTourStatus
                {
                    get
                    {
                        return TourStatus;
                    }
                }

                [Display(Name = "Final Booking Status")]
                public int[] Search_FinalBookingStatus { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpFinalbookingStatus
                {
                    get
                    {
                        return BookingStatus;
                    }
                }

                [Display(Name = "Input By")]
                public Guid[] Search_InputBy { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpInputBy
                {
                    get;
                    set;
                    //get
                    //{
                    //    return Search_Subordinates;
                    //}
                }

                [Display(Name = "Input Date Range")]
                //public DateTime? Search_InputDateStart { get; set; }
                public DateTime? Search_I_InputDate { get; set; }
                public DateTime? Search_F_InputDate { get; set; }

                [Display(Name = "Modification Date")]
                //public DateTime? Search_ModificationDateStart { get; set; }
                public DateTime? Search_I_ModificationDate { get; set; }
                public DateTime? Search_F_ModificationDate { get; set; }

                [Display(Name = "Greeting Rep")]
                public long[] Search_GreetingRep { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpGreetingRep
                {
                    get
                    {
                        List<SelectListItem> GreetingReps = GreetingRepDataModel.GetAllGreetingReps();
                        //GreetingReps.Insert(0, Utils.ListItems.Default());
                        return GreetingReps;
                    }
                }

                [Display(Name = "OPC")]
                public long[] Search_OPC { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpOpc
                {
                    get
                    {
                        List<SelectListItem> OPCs = OpcDataModel.GetAllOPCs();
                        // OPCs.Insert(0, Utils.ListItems.Default());
                        return OPCs;
                    }
                }

                [Display(Name = "Is Confirmed")]
                public object[] Search_IsConfirmed { get; set; }
                public List<SelectListItem> Search_DrpIsConfirmed
                {
                    get
                    {
                        List<SelectListItem> IsConfirmed = Utils.ListItems.Booleans();
                        IsConfirmed.Insert(0, Utils.ListItems.NotSet());
                        return IsConfirmed;
                    }
                }

                [Display(Name = "Coupon")]
                public string Search_CouponNumber { get; set; }

                [Display(Name = "Transaction")]
                public string Search_AuthCode { get; set; }

                [Display(Name = "Purchase Status")]
                public int[] Search_PurchaseStatus { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpPurchaseStatus
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpPurchaseStatus();
                    }
                }



                [Display(Name = "Point Of Sale")]
                public string[] Search_PointOfSale { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpPointsOfSale
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                    }
                }

                [Display(Name = "Reservations Agent")]
                public Guid?[] Search_ReservationsAgent { get; set; }

                [Display(Name = "Sales Agent")]
                public Guid?[] Search_SalesAgent { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpAgents
                {
                    get
                    {
                        var list = new PurchasesModel.Views().Agents;
                        //var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrentWorkGroupAgents(true, true);
                        if (list.Where(m => m.Selected).Count() > 0)
                        {
                            list.Where(m => m.Selected).FirstOrDefault().Selected = false;
                        }
                        return list;
                    }
                }

                [Display(Name = "Purchase Date")]
                public string Search_I_PurchaseDate { get; set; }
                public string Search_F_PurchaseDate { get; set; }

                [Display(Name = "Top")]
                public string Search_Top { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Search_DrpTop
                {
                    get
                    {
                        var list = new List<SelectListItem>();
                        var selected = new MasterChartDataModel.Purchases().View();
                        list.Add(new SelectListItem { Value = "10", Text = "10", Selected = selected });
                        list.Add(new SelectListItem { Value = "20", Text = "20" });
                        list.Add(new SelectListItem { Value = "30", Text = "30" });
                        list.Add(new SelectListItem { Value = "40", Text = "40" });
                        list.Add(new SelectListItem { Value = "50", Text = "50" });
                        list.Insert(0, ListItems.NotSet("--All--", "null", !selected));
                        return list;
                    }
                }
            }

            public class MassUpdate
            {
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
                        list.Insert(0, ListItems.Default("--Select Terminal--"));
                        //var list = MasterChartDataModel.LeadsCatalogs.FillDrpUsersPerTerminal(null);
                        //if (list.Count() > 0)
                        //{
                        //    list.Insert(0, ListItems.Default());
                        //}
                        //else
                        //{
                        //    list.Insert(0, ListItems.Default("--Select Terminal--"));
                        //}
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
                        return MasterChartDataModel.LeadsCatalogs.FillDrpTerminals();
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
                        var list = LeadStatusDataModel.GetAllLeadStatus();
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }

                //[Display(Name = "Tour Status")]
                //[DataBaseInfo(Name = "tblPresentations")]
                //[FieldInfo(Name = "tourStatusID")]
                //[FieldToRequest(Name = "reservationID")]
                //public int MassUpdate_TourStatus { get; set; }
                //public List<SelectListItem> MassUpdate_DrpTourStatus
                //{
                //    get
                //    {
                //        var list = TourStatusDataModel.GetAlltourStatus();
                //        list.Insert(0, ListItems.Default());
                //        return list;
                //    }
                //}

                [Display(Name = "Lead Source")]
                [DataBaseInfo(Name = "tblLeads")]
                [FieldInfo(Name = "leadSourceID")]
                public int MassUpdate_LeadSource { get; set; }
                //[ScriptIgnore]
                public List<SelectListItem> MassUpdate_DrpLeadSources
                {
                    get
                    {
                        //var session = new UserSession();
                        //var list = LeadSourceDataModel.GetLeadSourcesByWorkGroup(int.Parse(session.WorkGroupID.ToString()));
                        var list = LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }

                [Display(Name = "Booking Status")]
                [DataBaseInfo(Name = "tblLeads")]
                [FieldInfo(Name = "bookingStatusID")]
                public int MassUpdate_BookingStatus { get; set; }
                //[ScriptIgnore]
                public List<SelectListItem> MassUpdate_DrpBookingStatus
                {
                    get
                    {
                        var list = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                        list.Insert(0, ListItems.Default());
                        return list;

                    }
                }

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
                        //BookingStatus.Insert(0, Utils.ListItems.Default());
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
                        //InteractionTypes.Insert(0, Utils.ListItems.Default());
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
                    get;
                    set;
                    //get
                    //{
                    //    return UserDataModel.GetUsersBySupervisor();

                    //    //MembershipUser CurrentUser = Membership.GetUser();
                    //    //List<SelectListItem> Users = UserDataModel.GetUsersBySupervisor((Guid)CurrentUser.ProviderUserKey);
                    //    ////if (Users.Count() > 1)
                    //    ////{
                    //    ////    Users.Insert(0, Utils.ListItems.Default());
                    //    ////}
                    //    //return Users;
                    //}
                }
                [Display(Name = "Interacted With User")]
                [DataBaseInfo(Name = "tblInteractions")]
                [FieldInfo(Name = "interactedWithUserID")]
                [DoNotUpdate]
                public Guid InteractedWithUserID { get; set; }

                [Display(Name = "Date Saved")]
                [DataBaseInfo(Name = "tblInteractions")]
                [FieldInfo(Name = "dateSaved")]
                [DoNotUpdate]
                public DateTime? InteractionDateSaved { get { return DateTime.Now; } }

                public int MassUpdate_SendingEvent { get; set; }
                //public DuplicateLeadsModel DuplicateLeadsModel { get; set; }
            }

            //public class DuplicateLeadsModel
            //{
            //    [Display(Name = "Lead Status")]
            //    public int? DuplicateLeads_LeadStatus { get; set; }

            //    public List<SelectListItem> DuplicateLeads_DrpLeadStatus
            //    {
            //        get
            //        {
            //            var list = LeadStatusDataModel.GetAllLeadStatus();
            //            list.Insert(0, ListItems.Default());
            //            return list;
            //        }
            //    }

            //    [Display(Name = "Assign to User")]
            //    public Guid DuplicateLeads_AssignToUser { get; set; }

            //    public List<SelectListItem> DuplicateLeads_DrpAssignToUser
            //    {
            //        get
            //        {
            //            MembershipUser CurrentUser = Membership.GetUser();
            //            return UserDataModel.GetUsersBySupervisor((Guid)CurrentUser.ProviderUserKey, true);
            //        }
            //    }
            //}
            public class separator { }
            //public class MassUpdates
            //{
            //    public string MassUpdate_Coincidences { get; set; }

            //    [Display(Name = "Assigned to User")]
            //    [DataBaseInfo(Name = "assignedToUserID")]
            //    public string MassUpdate_AssignedToUser { get; set; }
            //    [ScriptIgnore]
            //    public List<SelectListItem> MassUpdate_DrpUsers
            //    {
            //        get
            //        {
            //            var list = new List<SelectListItem>();
            //            list.Insert(0, ListItems.Default("--Select Terminal--"));
            //            return list;
            //        }
            //    }

            //    [Display(Name = "Terminal")]
            //    [DataBaseInfo(Name = "terminalID")]
            //    [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            //    public int MassUpdate_Terminal { get; set; }
            //    [ScriptIgnore]
            //    public List<SelectListItem> MassUpdate_DrpTerminals
            //    {
            //        get
            //        {
            //            return MasterChartDataModel.LeadsCatalogs.FillDrpTerminals();
            //        }
            //    }

            //    //[Display(Name = "Lead Status")]
            //    //[DataBaseInfo(Name = "leadStatusID")]
            //    //public int MassUpdate_LeadStatus { get; set; }
            //    //public List<SelectListItem> MassUpdate_DrpLeadStatus
            //    //{
            //    //    get
            //    //    {
            //    //        var list = LeadStatusDataModel.GetAllLeadStatus();
            //    //        list.Insert(0, ListItems.Default());
            //    //        return list;
            //    //    }
            //    //}

            //    //[Display(Name = "Tour Status")]
            //    //[DataBaseInfo(Name = "tourStatusID")]
            //    //public int MassUpdate_TourStatus { get; set; }
            //    //public List<SelectListItem> MassUpdate_DrpTourStatus
            //    //{
            //    //    get
            //    //    {
            //    //        var list = TourStatusDataModel.GetAlltourStatus();
            //    //        list.Insert(0, ListItems.Default());
            //    //        return list;
            //    //    }
            //    //}

            //    //[Display(Name = "Lead Source")]
            //    //[DataBaseInfo(Name = "leadSourceID")]
            //    //public int MassUpdate_LeadSource { get; set; }
            //    //public List<SelectListItem> MassUpdate_DrpLeadSources
            //    //{
            //    //    get
            //    //    {
            //    //        var session = new UserSession();
            //    //        var list = LeadSourceDataModel.GetLeadSourcesByWorkGroup(int.Parse(session.WorkGroupID.ToString()));
            //    //        list.Insert(0, ListItems.Default());
            //    //        return list;
            //    //    }
            //    //}

            //    //[Display(Name = "Booking Status")]
            //    //[DataBaseInfo(Name = "bookingStatusID")]
            //    //public int MassUpdate_BookingStatus { get; set; }
            //    //public List<SelectListItem> MassUpdate_DrpBookingStatus
            //    //{
            //    //    get
            //    //    {
            //    //        var list = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
            //    //        list.Insert(0, ListItems.Default());
            //    //        return list;

            //    //    }
            //    //}
            //}

            public class PhoneNumbers
            {
                //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
                [RegularExpression(@"^(\d{10,15})$", ErrorMessage = "Entered phone format is not valid.")]
                [Display(Name = "Phone Number")]
                [LogReference]
                public string PhoneNumber { get; set; }
                [Display(Name = "Phone Type")]
                //[Range(1, int.MaxValue, ErrorMessage = "Phone Type is required.")]
                public int PhoneTypeID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> PhoneTypes
                {
                    get
                    {
                        List<SelectListItem> phoneTypes = PhoneDataModel.GetAllPhoneTypes();
                        phoneTypes.Insert(0, Utils.ListItems.Default());
                        return phoneTypes;
                    }
                }
                [Display(Name = "Extension Number")]
                public string ExtensionNumber { get; set; }
                [Display(Name = "Main")]
                public bool Main { get; set; }
                [Display(Name = "Do Not Call")]
                public bool DoNotCall { get; set; }
            }

            public class Interactions
            {
                [Display(Name = "Booking Status")]
                [DataBaseInfo(Name = "tblInteractions")]
                [Range(1, int.MaxValue, ErrorMessage = "Booking Status is required.")]
                public int BookingStatusID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> BookingStatus
                {
                    get
                    {
                        List<SelectListItem> BookingStatus = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                        //BookingStatus.Insert(0, Utils.ListItems.Default());
                        return BookingStatus;
                    }
                }


                [Display(Name = "Interaction Type")]
                [Range(1, int.MaxValue, ErrorMessage = "Interaction Type is required.")]
                public int InteractionTypeID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> InteractionTypes
                {
                    get
                    {
                        List<SelectListItem> InteractionTypes = InteractionDataModel.GetAllInteractionTypes();
                        //InteractionTypes.Insert(0, Utils.ListItems.Default());
                        return InteractionTypes;
                    }
                }

                public string InteractionComments { get; set; }
                [Display(Name = "Saved By")]
                public Guid? SavedByUserID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Users
                {
                    get;set;
                    //get
                    //{
                    //    //MembershipUser CurrentUser = Membership.GetUser();
                    //    //List<SelectListItem> Users = UserDataModel.GetUsersBySupervisor((Guid)CurrentUser.ProviderUserKey);

                    //    List<SelectListItem> Users = UserDataModel.GetUsersBySupervisor(null,false,false,true);

                    //    if (Users.Count() > 1)
                    //    {
                    //        Users.Insert(0, Utils.ListItems.Default("--Select One--",null));
                    //    }
                    //    return Users;
                    //}
                }
                [Display(Name = "Interacted With User")]
                [DataBaseInfo(Name = "interactedWithUserID")]
                public Guid InteractedWithUserID { get; set; }
                [DataBaseInfo(Name = "dateSaved")]
                [Display(Name = "Date Saved")]
                public DateTime? InteractionDateSaved { get; set; }
            }

            public class Emails
            {
                #region "GeneralInformation_Email"
                [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email Address is invalid.")]
                [Display(Name = "Email")]
                #endregion
                public string Email { get; set; }
                [Display(Name = "Main")]
                public bool Main { get; set; }

            }

            public class MemberInfo
            {

                [Display(Name = "Club Type")]
                public int ClubTypeID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> ClubTypes
                {
                    get
                    {
                        List<SelectListItem> clubTypes = new List<SelectListItem>();
                        clubTypes.Insert(0, Utils.ListItems.Default());
                        return clubTypes;
                    }
                }

            }

            public class BillingInfo
            {
                [
                    DoNotUpdate,
                    DoNotTrackChanges,
                    IsPrimaryKey
                    ]
                public long? billingInfoID { get; set; }


                [DataBaseInfo(Name = "leadID")]
                public Guid? LeadID { get; set; }

                [Display(Name = "First Name")]
                public string FirstName { get; set; }

                [Display(Name = "Last Name")]
                public string LastName { get; set; }

                [Display(Name = "Address")]
                public string Address { get; set; }

                [Display(Name = "City")]
                public string City { get; set; }


                [Display(Name = "State")]
                public string State { get; set; }

                #region "CountryID"
                [Range(1, int.MaxValue, ErrorMessage = "Country is required.")]
                [Display(Name = "Country")]
                [DataBaseInfo(Name = "countryID")]
                #endregion
                public int CountryID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> Countries
                {
                    get
                    {
                        List<SelectListItem> Countries = CountryDataModel.GetAllCountries();
                        return Countries;
                    }
                }


                [Display(Name = "Zip Code")]
                public string Zipcode { get; set; }

                [Display(Name = "Card Holder Name")]
                public string CardHolderName { get; set; }

                [Display(Name = "Card Number")]
                public string CardNumber { get; set; }

                #region "CardTypeID"
                [Range(1, int.MaxValue, ErrorMessage = "Card Type is required.")]
                [Display(Name = "Card Type")]
                [DataBaseInfo(Name = "cardTypeID")]
                #endregion
                public int CardTypeID { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> CreditCardTypes
                {
                    get
                    {
                        List<SelectListItem> CreditCardTypes = CreditCardsDataModel.GetAllCreditCardTypes();
                        return CreditCardTypes;
                    }
                }


                [RegularExpression("^(0[1-9]|1[0-2])/+([0-9]{4})$", ErrorMessage = "Card Expiry is invalid.")]
                [Display(Name = "Card Expiry")]
                public string CardExpiry { get; set; }

                [Display(Name = "Card CVV")]
                public int CardCVV { get; set; }

                [Display(Name = "Billing Comments")]
                public string BillingComments { get; set; }
            }

            public class LeadGeneralInformation : Fields.LeadGeneralInformation
            {
                //public bool ManageReservations
                //{
                //    get
                //    {
                //        return new MasterChartDataModel.Lead().View();
                //    }
                //}
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_Titles
                {
                    get
                    {
                        //return MasterChartDataModel.LeadsCatalogs.FillDrpPersonalTitles("en-US");
                        return MasterChartDataModel.LeadsCatalogs.FillDrpPersonalTitles();
                    }
                }
                #region "GeneralInformation_Phone"
                //[RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email Address is invalid.")]
                [Display(Name = "Phone Number")]
                #endregion
                public string GeneralInformation_Phone { get; set; }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_Terminals
                {
                    get
                    {
                        //List<SelectListItem> Terminals = TerminalDataModel.GetCurrentUserTerminals();
                        List<SelectListItem> Terminals = TerminalDataModel.GetActiveTerminalsList();
                        Terminals.Insert(0, Utils.ListItems.Default());
                        return Terminals;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_LeadStatuses
                {
                    get
                    {


                        //List<SelectListItem> leadStatuses = LeadStatusDataModel.GetAllLeadStatus();
                        var leadStatuses = LeadStatusDataModel.GetLeadStatusByTerminal();
                        leadStatuses.Insert(0, Utils.ListItems.Default());
                        return leadStatuses;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_LeadSources
                {
                    get
                    {
                        ////List<SelectListItem> LeadSources = LeadSourceDataModel.GetLeadSourcesByWorkGroup(1);
                        //List<SelectListItem> LeadSources = LeadSourceDataModel.GetAllLeadSources();
                        //LeadSources.Add(Utils.ListItems.Default());
                        //return LeadSources;

                        //var list = LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                        var list = LeadSourceDataModel.GetLeadSourcesByTerminal();
                        list.Insert(0, ListItems.Default());
                        return list;

                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_TimeZones
                {
                    get
                    {
                        return TimeZoneDataModel.GetAllTimeZones();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_Countries
                {
                    get
                    {
                        return CountryDataModel.GetAllCountries();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_BrokerContracts
                {
                    get
                    {
                        return BrokerDataModel.GetAllContracts();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_DrpAssignedToUser
                {
                    get;
                    set;
                    //get
                    //{
                    //    var users = new Search().Search_Subordinates;
                    //    users.Insert(0, ListItems.Default());
                    //    return users;
                    //}
                }
                [ScriptIgnore]
                public List<SelectListItem> GeneralInformation_CallClasificationStatus
                {
                    get
                    {
                        //List<SelectListItem> callClasifications = CallClasificationDataModel.GetAllCallClasifications();
                        List<SelectListItem> callClasifications = MasterChartDataModel.LeadsCatalogs.FillDrpCallsClasification();
                        callClasifications.Insert(0, Utils.ListItems.Default());
                        return callClasifications;
                    }
                }

                private Emails _GeneralInformation_Emails = new Emails();
                public Emails GeneralInformation_Emails
                {
                    get { return _GeneralInformation_Emails; }
                    set { _GeneralInformation_Emails = value; }
                }

                private PhoneNumbers _GeneralInformation_PhoneNumbers = new PhoneNumbers();
                public PhoneNumbers GeneralInformation_PhoneNumbers
                {
                    get { return _GeneralInformation_PhoneNumbers; }
                    set { _GeneralInformation_PhoneNumbers = value; }
                }

                private Interactions _GeneralInformation_Interactions = new Interactions();
                public Interactions GeneralInformation_Interactions
                {
                    get { return _GeneralInformation_Interactions; }
                    set { _GeneralInformation_Interactions = value; }
                }

                private BillingInfo _GeneralInformation_BillingInfo = new BillingInfo();
                public BillingInfo GeneralInformation_BillingInfo
                {
                    get { return _GeneralInformation_BillingInfo; }
                    set { _GeneralInformation_BillingInfo = value; }
                }


                private Views.MemberInfo _GeneralInformation_MemberInformation_View = new Views.MemberInfo();
                public Views.MemberInfo GeneralInformation_MemberInformation_View
                {
                    get { return _GeneralInformation_MemberInformation_View; }
                    set { _GeneralInformation_MemberInformation_View = value; }
                }

                public FastSaleModel.FastSaleInfoModel FastSaleInfoModel { get; set; }

            }

            public class ItemLogsModel
            {
                public string ItemLogs_Field { get; set; }
                public string ItemLogs_PreviousValue { get; set; }
                public string ItemLogs_CurrentValue { get; set; }
                public string ItemLogs_UserName { get; set; }
                public string ItemLogs_LogDateTime { get; set; }
            }

            public class ReservationCharges
            {

                public long BillingInfoID { get; set; }
                public long[] ReservationPaymentDetailsIDs { get; set; }

            }

        }

    }

    public class BillingInfoModel
    {

        public class Fields
        {

            [DataBaseInfo(Name = "tblBillingInfo")]
            [LogReference(Name = "Billing Info")]
            public class BillingInfo
            {
                [
                        DoNotUpdate,
                        DoNotTrackChanges,
                        IsPrimaryKey
                        ]
                [DataBaseInfo(Name = "billingInfoID")]
                public long? billingInfoID { get; set; }


                [DataBaseInfo(Name = "leadID")]
                public Guid? BillingInfo_LeadID { get; set; }

                [Display(Name = "First Name")]
                [DataBaseInfo(Name = "firstName")]
                public string FirstName { get; set; }

                [Display(Name = "Last Name")]
                [DataBaseInfo(Name = "lastName")]
                public string LastName { get; set; }

                [Display(Name = "Address")]
                [DataBaseInfo(Name = "address")]
                public string Address { get; set; }

                [Display(Name = "City")]
                [DataBaseInfo(Name = "city")]
                public string City { get; set; }


                [Display(Name = "State")]
                [DataBaseInfo(Name = "state")]
                public string State { get; set; }

                #region "CountryID"
                //[Range(1, int.MaxValue, ErrorMessage = "Country is required.")]
                [Display(Name = "Country")]
                [DataBaseInfo(Name = "countryID")]
                #endregion
                public int CountryID { get; set; }

                [Display(Name = "Zip Code")]
                [DataBaseInfo(Name = "zipcode")]
                public string Zipcode { get; set; }

                [Display(Name = "Card Holder Name")]
                [DataBaseInfo(Name = "cardHolderName")]
                public string CardHolderName { get; set; }

                [Display(Name = "Card Number")]
                [Required(ErrorMessage = "Card Number is required")]
                [DataBaseInfo(Name = "cardNumber")]
                public string CardNumber { get; set; }

                #region "CardTypeID"
                [Range(1, int.MaxValue, ErrorMessage = "Card Type is required.")]
                [Display(Name = "Card Type")]
                [DataBaseInfo(Name = "cardTypeID")]
                #endregion
                public int CardTypeID { get; set; }

                //[RegularExpression("^(0[1-9]|1[0-2])/+([0-9]{4})$", ErrorMessage = "Card Expiry is invalid.")]
                [RegularExpression("^(0[1-9]|1[0-2])[/]([0-9]{4})$", ErrorMessage = "Card Expiry is invalid.")]
                [Display(Name = "Card Expiry")]
                [DataBaseInfo(Name = "cardExpiry")]
                public string CardExpiry { get; set; }

                [Display(Name = "Card CVV")]
                [DataBaseInfo(Name = "cardCVV")]
                public string CardCVV { get; set; }

                [Display(Name = "Billing Comments")]
                [DataBaseInfo(Name = "billingComments")]
                public string BillingComments { get; set; }

                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                [ParseBackToObject]
                public DateTime? DateSaved { get; set; }
            }

        }
        public class Items
        {
            public class BillingInfoSearchResults
            {
                [DataBaseInfo(Name = "leadID")]
                public Guid? LeadID { get; set; }
                [DataBaseInfo(Name = "billingInfoID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long? BillingInfoID { get; set; }
                [Display(Name = "Card Holder Name")]
                [DataBaseInfo(Name = "cardHolderName")]
                public string CardHolderName { get; set; }

                [Display(Name = "Arrival Date")]
                [DataBaseInfo(Name = "cardNumber")]
                public string CardNumber { get; set; }

                [Display(Name = "Card Expiry")]
                [DataBaseInfo(Name = "cardExpiry")]
                public string CardExpiry { get; set; }

                [Display(Name = "Card CVV")]
                [DataBaseInfo(Name = "cardCVV")]
                public string CardCVV { get; set; }

            }

        }
        public class Views
        {
            public class BillingInfo : Fields.BillingInfo
            {
                [ScriptIgnore]
                public List<SelectListItem> Countries
                {
                    get
                    {
                        List<SelectListItem> Countries = CountryDataModel.GetAllCountries();
                        return Countries;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> CreditCardTypes
                {
                    get
                    {
                        List<SelectListItem> CreditCardTypes = CreditCardsDataModel.GetAllCreditCardTypes();
                        CreditCardTypes.Insert(0, ListItems.Default());
                        return CreditCardTypes;
                    }
                }


            }
        }
    }

    public class PaymentModel
    {

        public class Fields
        {
            [LogReference(Name = "Reservation Payment Details")]
            [DataBaseInfo(Name = "tblPaymentDetails")]
            public class ReservationPaymentDetails
            {

                [DataBaseInfo(Name = "paymentDetailsID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long ReservationPaymentDetailsID { get; set; }

                [DataBaseInfo(Name = "reservationID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public Guid? ReservationID { get; set; }

                [Display(Name = "Amount")]
                public decimal Amount { get; set; }

                public decimal? NetCenterCost { get; set; }
                public decimal? NetCenterCharge { get; set; }

                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                public DateTime DateSaved { get; set; }

                [Display(Name = "Payment Comments")]
                public string PaymentComments { get; set; }

                [Display(Name = "Refund Account")]
                public string RefundAccount { get; set; }

                public DateTime? TransactionDate { get; set; }

                #region "ChargeTypeID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Charge Type")]
                [DataBaseInfo(Name = "ChargeTypeID")]
                #endregion
                public int ChargeTypeID { get; set; }

                #region "ChargeDescriptionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Description is required.")]
                [Display(Name = "Charge Description")]
                [DataBaseInfo(Name = "ChargeDescriptionID")]
                #endregion
                public int ChargeDescriptionID { get; set; }

                #region "BillingInfoID"
                [Display(Name = "Billing Info")]
                [DataBaseInfo(Name = "billingInfoID")]
                #endregion
                public long? BillingInfoID { get; set; }

                #region "MoneyTransactionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Money Transaction")]
                [DataBaseInfo(Name = "MoneyTransactionID")]
                #endregion
                public long? MoneyTransactionID { get; set; }


                // // //

                [Display(Name = "Error Code")]
                [DataBaseInfo(Name = "errorCode")]
                [GetValueFrom(ValuePath = "tblMoneyTransactions.errorCode")]
                public string ErrorCode { get; set; }

                [DataBaseInfo(Name = "transactionDate")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp, ValuePath = "tblMoneyTransactions.transactionDate")]
                [DoNotTrackChanges]
                [GetValueFrom(ValuePath = "tblMoneyTransactions.transactionDate")]
                public DateTime? AttemptDate { get; set; }

                [DataBaseInfo(Name = "authDate")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp, ValuePath = "tblMoneyTransactions.authDate")]
                [DoNotTrackChanges]
                public DateTime? AuthDate { get; set; }

                [GetValueFrom(ValuePath = "tblMoneyTransactions.authCode")]
                [Display(Name = "Invoice Number")]
                [DataBaseInfo(Name = "authCode")]
                public string AuthCode { get; set; }

                // // //

            }
        }

        public class Items
        {
            [LogReference(Name = "Payment")]
            [DataBaseInfo(Name = "tblPaymentDetails")]
            public class ReservationPaymentDetails
            {
                [DataBaseInfo(Name = "paymentDetailsID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long ReservationPaymentDetailsID { get; set; }

                [Display(Name = "Amount")]
                public decimal Amount { get; set; }
                public decimal? NetCenterCost { get; set; }
                public decimal? NetCenterCharge { get; set; }
                public DateTime DateSaved { get; set; }
                [Display(Name = "Payment Comments")]
                public string PaymentComments { get; set; }
                [Display(Name = "Refund Account")]
                public string RefundAccount { get; set; }
                //public DateTime? TransactionDate { get; set; }

                #region "ChargeTypeID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Charge Type")]
                [DataBaseInfo(Name = "ChargeTypeID")]
                #endregion
                public int ChargeTypeID { get; set; }

                #region "ChargeDescriptionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Description is required.")]
                [Display(Name = "Charge Description")]
                [DataBaseInfo(Name = "ChargeDescriptionID")]
                #endregion
                public int ChargeDescriptionID { get; set; }

                #region "BillingInfoID"
                [Display(Name = "Billing Info")]
                [DataBaseInfo(Name = "billingInfoID")]
                #endregion
                public long? BillingInfoID { get; set; }

                #region "MoneyTransactionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Money Transaction")]
                [DataBaseInfo(Name = "MoneyTransactionID")]
                #endregion
                public long? MoneyTransactionID { get; set; }

            }
        }

        public class Views
        {
            public class ReservationPaymentDetails : Fields.ReservationPaymentDetails
            {

                [Display(Name = "Amount")]
                public decimal Amount { get; set; }
                [Display(Name = "Net Center Cost")]
                public decimal? NetCenterCost { get; set; }
                [Display(Name = "Net Center Charge")]
                public decimal? NetCenterCharge { get; set; }
                [Display(Name = "Payment Comments")]
                public string PaymentComments { get; set; }
                [Display(Name = "Refund Account")]
                public string RefundAccount { get; set; }
                //public DateTime? TransactionDate { get; set; }
                #region "ChargeTypeID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Charge Type")]
                [DataBaseInfo(Name = "ChargeTypeID")]
                #endregion
                public int ChargeTypeID { get; set; }
                #region "ChargeDescriptionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Description is required.")]
                [Display(Name = "Charge Description")]
                [DataBaseInfo(Name = "ChargeDescriptionID")]
                #endregion
                public int ChargeDescriptionID { get; set; }
            }
        }

    }

    public class ServicePurchaseModel
    {
        public class Fields
        {

        }

        public class Items
        {

        }

        public class Views
        {

        }
    }

    public class HotelReservationModel
    {

        public class Fields
        {
            [DataBaseInfo(Name = "tblReservations")]
            [LogReference(Name = "Reservation")]
            public class ReservationGeneralInformation
            {
                [DataBaseInfo(Name = "leadID")]
                public Guid? LeadID { get; set; }
                [DataBaseInfo(Name = "reservationID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public Guid? ReservationID { get; set; }
                [Display(Name = "Hotel Confirmation Number")]
                [DataBaseInfo(Name = "hotelConfirmationNumber")]
                public string HotelConfirmationNumber { get; set; }
                [Required(AllowEmptyStrings = false, ErrorMessage = "Arrival Date is required")]
                [Display(Name = "Arrival Date")]
                [DataBaseInfo(Name = "arrivalDate")]
                [ParseBackToObject]
                public DateTime? ArrivalDate { get; set; }
                [Display(Name = "Destination")]
                [DataBaseInfo(Name = "destinationID")]
                public long? DestinationID { get; set; }
                [Range(1, int.MaxValue, ErrorMessage = "Resort is required")]
                [Display(Name = "Resort")]
                [DataBaseInfo(Name = "placeID")]
                public long? PlaceID { get; set; }
                [Display(Name = "Room Number")]
                [DataBaseInfo(Name = "roomNumber")]
                public string RoomNumber { get; set; }
                [Display(Name = "Room Type")]
                [DataBaseInfo(Name = "roomTypeID")]
                public long? RoomTypeID { get; set; }
                [Display(Name = "Total Nights")]
                [DataBaseInfo(Name = "totalNights")]
                public int? TotalNights { get; set; }
                [Display(Name = "Total Paid")]
                [DataBaseInfo(Name = "totalPaid")]
                public decimal? TotalPaid { get; set; }
                [Display(Name = "Confirmed Total Paid")]
                [DataBaseInfo(Name = "confirmedTotalPaid")]
                public decimal? ConfirmedTotalPaid { get; set; }
                [Display(Name = "Diamante Total Paid")]
                [DataBaseInfo(Name = "diamanteTotalPaid")]
                public decimal? DiamanteTotalPaid { get; set; }
                [Display(Name = "Room Upgraded")]
                [DataBaseInfo(Name = "roomUpgraded")]
                public bool RoomUpgraded { get; set; }
                [Display(Name = "Is Special Ocassion")]
                [DataBaseInfo(Name = "isSpecialOcassion")]
                public bool IsSpecialOcassion { get; set; }
                [Display(Name = "Special Ocassion Comments")]
                [DataBaseInfo(Name = "specialOcassionComments")]
                public string SpecialOcassionComments { get; set; }
                [Display(Name = "Confirmation Letter On Arrival")]
                [DataBaseInfo(Name = "confirmationLetterOnArrival")]
                public bool ConfirmationLetterOnArrival { get; set; }

                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                [ParseBackToObject]
                public DateTime? DateSaved { get; set; }


                private Fields.PresentationGeneralInformation _GeneralInformation_PresentationInformation = new Fields.PresentationGeneralInformation();

                [DataBaseInfo(RelationShipType = typeof(tblPresentations), PrimaryKeyModelName = "presentationID", IsRelationShip = true, Cardinality = Utils.DataBaseRelationShipCardinality.OneToOne)]
                public Fields.PresentationGeneralInformation GeneralInformation_PresentationInformation
                {
                    get { return _GeneralInformation_PresentationInformation; }
                    set { _GeneralInformation_PresentationInformation = value; }
                }

                [Display(Name = "Greeting Rep.")]
                [DataBaseInfo(Name = "greetingRepID")]
                public long? GreetingRepID { get; set; }

                [Display(Name = "OPC")]
                [DataBaseInfo(Name = "opcID")]
                public long? OpcID { get; set; }

                [Display(Name = "Concierge Comments")]
                [DataBaseInfo(Name = "conciergeComments")]
                public string ConciergeComments { get; set; }

                [Display(Name = "Guests Names")]
                [DataBaseInfo(Name = "guestsNames")]
                public string GuestsNames { get; set; }

                [Display(Name = "Pre Check In")]
                [DataBaseInfo(Name = "preCheckIn")]
                public bool PreCheckIn { get; set; }

                #region "GeneralInformation_Email"
                //[RegularExpression("[\\S].{1,}", ErrorMessage = "You have to speficy at least 1 email.")]
                // [Required(ErrorMessage = "You have to specify at least 1 email.")]
                //[ScriptIgnore]
                [ListType(typeof(Items.ReservationPaymentDetails))]
                [ParseBackToObject]
                [DataBaseInfo(RelationShipType = typeof(tblPaymentDetails),
                    PrimaryKeyModelName = "ReservationPaymentDetailsID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "paymentDetailsID"
                    )
                ]
                #endregion
                public object GeneralInformation_ReservationPaymentsList { get; set; }
                //[ScriptIgnore]
                [ListType(typeof(Items.FlightInformation))]
                [ParseBackToObject]
                [DataBaseInfo(RelationShipType = typeof(tblFlights),
                    PrimaryKeyModelName = "FlightID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "flightID"
                    )]
                public object GeneralInformation_FlightInformationList { get; set; }
                //[ScriptIgnore]
                [ListType(typeof(Items.EmailNotificationLogs))]
                [ParseBackToObject]
                [DataBaseInfo(RelationShipType = typeof(tblEmailNotificationLogs),
                    PrimaryKeyModelName = "EmailNotificationLogID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "emailNotificationLogID"
                    )]
                public object GeneralInformation_EmailNotificationLogsList { get; set; }
                //[ScriptIgnore]
                [ListType(typeof(Items.OptionsSoldInformation))]
                [ParseBackToObject]
                [DataBaseInfo(RelationShipType = typeof(tblOptionsSold),
                    PrimaryKeyModelName = "OptionSoldID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "optionSoldID", Name = "tblOptionsSold"
                )]
                public object GeneralInformation_OptionsSoldList { get; set; }
                [DataBaseInfo(Name = "numberAdults")]
                [Display(Name = "Number of Adults")]
                public string NumberAdults { get; set; }
                [DataBaseInfo(Name = "numberChildren")]
                [Display(Name = "Number Of Children")]
                public string NumberChildren { get; set; }
            }
            [LogReference(Name = "Presentation")]
            [DataBaseInfo(Name = "tblPresentations")]
            public class PresentationGeneralInformation
            {
                [IsPrimaryKey]
                [DataBaseInfo(Name = "presentationID")]
                public long Presentations_PresentationID { get; set; }

                [DataBaseInfo(Name = "leadID")]
                [DoNotTrackChanges]
                public Guid? LeadID { get; set; }

                [Display(Name = "Presentation Date")]
                [DataBaseInfo(Name = "datePresentation")]
                [ParseBackToObject]
                public DateTime? Presentations_TourDate { get; set; }



                [Display(Name = "Presentation Time")]
                [DataBaseInfo(Name = "timePresentation")]
                [ParseBackToObject]
                public TimeSpan? Presentations_TourTime { get; set; }


                [Display(Name = "Real Tour Date")]
                [DataBaseInfo(Name = "realTourDate")]
                [ParseBackToObject]
                public DateTime? Presentations_RealTourDate { get; set; }
                [Display(Name = "Tour Status")]
                [DataBaseInfo(Name = "tourStatusID")]
                public int? Presentations_TourStatusID { get; set; }
                [Display(Name = "Hostess Comments")]
                [DataBaseInfo(Name = "hostessComments")]
                public string Presentations_HostessComments { get; set; }
                [Display(Name = "Final Tour Status")]
                [DataBaseInfo(Name = "finalTourStatusID")]
                public int? Presentations_FinalTourStatusID { get; set; }
                [Display(Name = "Final Booking Status")]
                [DataBaseInfo(Name = "finalBookingStatusID")]
                public int? Presentations_FinalBookingStatusID { get; set; }


                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                [ParseBackToObject]
                public DateTime? DateSaved { get; set; }

                #region "ContractsHistory"
                //[ScriptIgnore]
                [ParseBackToObject]
                [ListType(typeof(Items.ContractsHistory))]
                [DataBaseInfo(RelationShipType = typeof(tblContractsHistory),
                    PrimaryKeyModelName = "ContractHistoryID",
                    IsRelationShip = true,
                    Cardinality = Utils.DataBaseRelationShipCardinality.OneToMany,
                    PrimaryKeyDatabaseName = "contractHistoryID", Name = "tblContractsHistory"
                    )
                ]
                #endregion
                public object Presentations_ContractsHistoryList { get; set; }

            }

            public class ReservationPaymentDetails
            {

                [DataBaseInfo(Name = "paymentDetailsID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long ReservationPaymentDetailsID { get; set; }

                [DataBaseInfo(Name = "reservationID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public Guid? ReservationID { get; set; }

                [Display(Name = "Amount")]
                public decimal? Amount { get; set; }

                public decimal? NetCenterCost { get; set; }
                public decimal? NetCenterCharge { get; set; }

                public DateTime DateSaved { get; set; }

                [Display(Name = "Payment Comments")]
                public string PaymentComments { get; set; }

                [Display(Name = "Refund Account")]
                public string RefundAccount { get; set; }

                public DateTime? TransactionDate { get; set; }

                #region "ChargeTypeID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Charge Type")]
                [DataBaseInfo(Name = "ChargeTypeID")]
                #endregion
                public int ChargeTypeID { get; set; }

                #region "ChargeDescriptionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Description is required.")]
                [Display(Name = "Charge Description")]
                [DataBaseInfo(Name = "ChargeDescriptionID")]
                #endregion
                public int ChargeDescriptionID { get; set; }

                #region "BillingInfoID"
                [Display(Name = "Billing Info")]
                [DataBaseInfo(Name = "billingInfoID")]
                #endregion
                public long? BillingInfoID { get; set; }

                #region "MoneyTransactionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Money Transaction")]
                [DataBaseInfo(Name = "MoneyTransactionID")]
                #endregion
                public long? MoneyTransactionID { get; set; }

            }

            [LogReference(Name = "Flight")]
            [DataBaseInfo(Name = "tblFlights")]
            public class FlightInformation
            {
                [DoNotUpdate, IsPrimaryKey, DoNotTrackChanges]
                [DataBaseInfo(Name = "flightID")]
                public long? FlightID { get; set; }

                [DoNotUpdate, IsPrimaryKey, DoNotTrackChanges]
                [DataBaseInfo(Name = "reservationID")]
                public Guid? ReservationID { get; set; }

                [Display(Name = "Airline")]
                [DataBaseInfo(Name = "airLineID")]
                public int AirlineID { get; set; }


                [Display(Name = "Flight Number")]
                [DataBaseInfo(Name = "flightNumber")]
                public string FlightNumber { get; set; }

                [Display(Name = "Passengers Names")]
                [DataBaseInfo(Name = "passengersNames")]
                public string PassengersNames { get; set; }

                [DataBaseInfo(Name = "passengers")]
                [Display(Name = "Number Of Passengers")]
                public int? Passengers { get; set; }

                [DataBaseInfo(Name = "destinationID")]
                [Display(Name = "Destination")]
                public long? DestinationID { get; set; }

                [DataBaseInfo(Name = "flightComments")]
                [Display(Name = "Flight Comments")]
                public string FlightComments { get; set; }

                [DataBaseInfo(Name = "flightDateTime")]
                [Display(Name = "Flight Date Time")]
                [ParseBackToObject]
                public DateTime? FlightDateTime { get; set; }

                [DataBaseInfo(Name = "transportationLetterSentDateTime")]
                [Display(Name = "Transportation Letter Sent Date")]
                [ParseBackToObject]
                public DateTime? TransportationLetterSentDateTime { get; set; }

                [DataBaseInfo(Name = "transportationLetterSentByUserID")]
                [Display(Name = "Transportation Letter Sent By")]
                public Guid? TransportationLetterSentByUser { get; set; }

                [DataBaseInfo(Name = "transportationServiceSentDateTime")]
                [Display(Name = "Transportation Service Sent Date Time")]
                [ParseBackToObject]
                public DateTime? TransportationServiceSentDateTime { get; set; }

                [DataBaseInfo(Name = "transportationServiceSentByUserID")]
                [Display(Name = "Transportation Service Sent By")]
                public Guid? TransportationServiceSentByUser { get; set; }

                [DataBaseInfo(Name = "flightTypeID")]
                [Display(Name = "Flight Type")]
                public int FlightTypeID { get; set; }

                [DataBaseInfo(Name = "transportationPurchased")]
                [Display(Name = "Transportation Purchased")]
                public bool TransportationPurchased { get; set; }

                [DataBaseInfo(Name = "transportationProviderID")]
                [Display(Name = "Transportation Provider")]
                public int TransportationProviderID { get; set; }

                private DateTime? dateSaved;
                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? DateSaved
                {
                    get { return dateSaved; }
                    set
                    {
                        if (value.ToString().Contains("/Date("))
                        {
                            string fecha = value.ToString().Replace("/Date(", "").Replace(")/", "");
                            long fechaLong = long.Parse(fecha);
                            DateTime dt = new DateTime(fechaLong * 10000);
                            dateSaved = dt;
                        }
                        else { dateSaved = value; }

                        //Match match = Regex.Match(value.ToString()

                        //interactionDateSaved = DateTime.SpecifyKind(value, DateTimeKind.Utc); 
                    }
                }

                public FlightInformation()
                {
                    TransportationPurchased = false;
                }
            }

            [DataBaseInfo(Name = "tblEmailNotificationLogs")]
            public class EmailNotificationLogs
            {
                [DoNotUpdate, IsPrimaryKey, DoNotTrackChanges]
                [DataBaseInfo(Name = "emailNotificationLogID")]
                public int EmailNotificationLogID { get; set; }

                [DoNotUpdate, IsPrimaryKey, DoNotTrackChanges]
                [DataBaseInfo(Name = "reservationID")]
                public Guid? ReservationID { get; set; }

                [Display(Name = "Email Notification")]
                [DataBaseInfo(Name = "emailNotificationID")]
                public string EmailNotificationID { get; set; }

                [Display(Name = "Date Sent")]
                [DataBaseInfo(Name = "dateSent")]
                [ParseBackToObject]
                public DateTime? DateSent { get; set; }

                [Display(Name = "Sent By User")]
                [DataBaseInfo(Name = "sentByUserID")]
                public Guid? SentByUserID { get; set; }
            }

            [DataBaseInfo(Name = "tblOptionsSold")]
            [LogReference(Name = "Options")]
            public class OptionsSoldInformation
            {
                [DoNotUpdate, IsPrimaryKey, DoNotTrackChanges]
                [DataBaseInfo(Name = "optionSoldID")]
                public long? OptionSoldID { get; set; }

                [DoNotUpdate, IsPrimaryKey, DoNotTrackChanges]
                [DataBaseInfo(Name = "reservationID")]
                public Guid? ReservationID { get; set; }

                [Display(Name = "Option Type")]
                public int OptionType { get; set; }

                [Display(Name = "Option Name")]
                [DataBaseInfo(Name = "optionID")]
                //[Range(1, int.MaxValue, ErrorMessage = "Option Type is required")]
                public int? OptionID { get; set; }

                [Display(Name = "Quantity")]
                [DataBaseInfo(Name = "quantity")]
                public int? Quantity { get; set; }

                [Display(Name = "Price Per Option")]
                [DataBaseInfo(Name = "optionPrice")]
                //[Required( ErrorMessage = "Option Price is required")]
                public float? OptionPrice { get; set; }

                [Display(Name = "Points Redemption")]
                [DataBaseInfo(Name = "pointsRedemption")]
                public string PointsRedemption { get; set; }

                [Display(Name = "Total Paid")]
                [DataBaseInfo(Name = "totalPaid")]
                //[Required(ErrorMessage = "Total Paid is required")]
                public string TotalPaid { get; set; }

                [Display(Name = "Guest Name(s)")]
                [DataBaseInfo(Name = "guestName")]
                //[Required(ErrorMessage = "Guest Name(s) is required")]
                public string GuestName { get; set; }

                [Display(Name = "Eligible for Credit")]
                [DataBaseInfo(Name = "eligibleForCredit")]
                public bool EligibleForCredit { get; set; }

                [Display(Name = "Credit Amount")]
                //[RequiredIf(OtherProperty = "GeneralInformation_OptionsSoldInformation.EligibleForCredit", EqualsTo = "true", ErrorMessage = "Credit Amount is required")]
                [DataBaseInfo(Name = "creditAmount")]
                public string CreditAmount { get; set; }

                [Display(Name = "Comments")]
                [DataBaseInfo(Name = "comments")]
                public string Comments { get; set; }

                [Display(Name = "Deleted")]
                [DataBaseInfo(Name = "deleted")]
                public bool Deleted { get; set; }
            }
        }
        public class Items
        {
            public class ReservationSearchResults
            {
                [DataBaseInfo(Name = "leadID")]
                public Guid? LeadID { get; set; }
                [DataBaseInfo(Name = "reservationID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public Guid? ReservationID { get; set; }
                [Display(Name = "Hotel Confirmation Number")]
                [DataBaseInfo(Name = "hotelConfirmationNumber")]
                public string HotelConfirmationNumber { get; set; }
                [Display(Name = "Arrival Date")]
                [DataBaseInfo(Name = "arrivalDate")]
                public DateTime? ArrivalDate { get; set; }
                [Display(Name = "Destination")]
                [DataBaseInfo(Name = "tblDestinations.destination")]
                public string Destination { get; set; }
            }
            [LogReference(Name = "Contract")]
            [DataBaseInfo(Name = "tblContractsHistory")]
            public class ContractsHistory
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "contractHistoryID")]
                public long? ContractHistoryID { get; set; }

                [DataBaseInfo(Name = "salesDate")]
                public DateTime SalesDate { get; set; }
                [DataBaseInfo(Name = "salesVolume")]
                public decimal SalesVolume { get; set; }

                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? DateSaved { get; set; }
                //public DateTime? DateSaved { get; set; }
            }

            public class ReservationPaymentDetails
            {
                [DataBaseInfo(Name = "paymentDetailsID")]
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                public long? ReservationPaymentDetailsID { get; set; }

                [Display(Name = "Amount")]
                [DataBaseInfo(Name = "amount")]
                public decimal? Amount { get; set; }
                [Display(Name = "Net Center Cost")]
                [DataBaseInfo(Name = "netCenterCost")]
                public decimal? NetCenterCost { get; set; }
                [Display(Name = "Net Center Charge")]
                [DataBaseInfo(Name = "netCenterCharge")]
                public decimal? NetCenterCharge { get; set; }
                [Display(Name = "Payment Comments")]
                [DataBaseInfo(Name = "paymentComments")]
                public string PaymentComments { get; set; }
                [DataBaseInfo(Name = "refundAccount")]
                [Display(Name = "Refund Account")]
                public string RefundAccount { get; set; }
                //public DateTime? TransactionDate { get; set; }

                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? DateSaved { get; set; }

                #region "ChargeTypeID"
                [Display(Name = "Charge Type")]
                [DataBaseInfo(Name = "chargeTypeID")]
                [LogReference]
                #endregion
                public int ChargeTypeID { get; set; }


                //antes de crear el CustomAttribute [GetValueFrom], el chargeType lo obtenia de un DropDown en el lado del cliente basado en el id
                // pero eso no es muy confiable, por que ya sea por permisos o por lo que sea, el DropDown puede no contener un elemento con el valor buscado.
                //Entonces [GetValueFrom] soluciona ese problema, falta implementarlo en todas las consultas que llenan o muestran info del lado del cliente.
                //Una vez implementado el [GetValueFrom], seguir incluyendo el chargeTypeID (por ejemplo), para cuando el cliente le de editar a la info entonces poder seleccionar el 
                //elemento en el dropDown basado en su valor.


                //#region "ChargeType"
                //[Display(Name = "Charge Type")]
                //[DataBaseInfo(Name = "chargeType")]
                //[GetValueFrom(ValuePath="tblChargeTypes.chargeType")]
                //#endregion
                //public string ChargeType { get; set; }

                #region "ChargeDescriptionID"
                [Display(Name = "Charge Description")]
                [DataBaseInfo(Name = "chargeDescriptionID")]
                [LogReference]
                #endregion
                public int ChargeDescriptionID { get; set; }

                #region "MoneyTransactionID"
                [Range(1, int.MaxValue, ErrorMessage = "Charge Type is required.")]
                [Display(Name = "Money Transaction")]
                [DataBaseInfo(Name = "moneyTransactionID")]
                #endregion
                public long? MoneyTransactionID { get; set; }

                [GetValueFrom(ValuePath = "tblMoneyTransactions.errorCode")]
                public string ErrorCode { get; set; }

                [GetValueFrom(ValuePath = "tblMoneyTransactions.transactionDate")]
                public DateTime? AttemptDate { get; set; }


                [GetValueFrom(ValuePath = "tblMoneyTransactions.authCode")]
                public string AuthCode { get; set; }

                //AuthDate= x.tblMoneyTransactions.authDate,



            }

            [LogReference(Name = "Flight")]
            [DataBaseInfo(Name = "tblFlights")]
            public class FlightInformation
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "flightID")]
                public long? FlightID { get; set; }
                [DataBaseInfo(Name = "airLineID")]
                public int AirlineID { get; set; }
                [DataBaseInfo(Name = "flightNumber")]
                public string FlightNumber { get; set; }
                [DataBaseInfo(Name = "passengersNames")]
                public string PassengersNames { get; set; }
                [DataBaseInfo(Name = "passengers")]
                public int? Passengers { get; set; }
                [DataBaseInfo(Name = "destinationID")]
                public long DestinationID { get; set; }
                [DataBaseInfo(Name = "flightComments")]
                public string FlightComments { get; set; }
                [DataBaseInfo(Name = "flightDateTime")]
                public DateTime FlightDateTime { get; set; }
                [DataBaseInfo(Name = "transportationLetterSentDateTime")]
                public DateTime? TransportationLetterSentDateTime { get; set; }
                [DataBaseInfo(Name = "transportationLetterSentByUserID")]
                public Guid? TransportationLetterSendByUserID { get; set; }
                [DataBaseInfo(Name = "transportationServiceSentDateTime")]
                public DateTime? TransportationServiceSentDateTime { get; set; }
                [DataBaseInfo(Name = "transportationServiceSentByUserID")]
                public Guid? TransportationServiceSentByUserID { get; set; }
                [DataBaseInfo(Name = "flightTypeID")]
                public int FlightTypeID { get; set; }
                [DataBaseInfo(Name = "transportationPurchased")]
                public bool TransportationPurchased { get; set; }
                [DataBaseInfo(Name = "transportationProviderID")]
                public int TransportationProviderID { get; set; }

                private DateTime? dateSaved;
                [DataBaseInfo(Name = "dateSaved")]
                [AutomaticValue(AutomaticValueType = Utils.AutomaticValueTypes.DateTimeStamp)]
                [DoNotTrackChanges]
                public DateTime? DateSaved
                {
                    get { return dateSaved; }
                    set
                    {
                        if (value.ToString().Contains("/Date("))
                        {
                            string fecha = value.ToString().Replace("/Date(", "").Replace(")/", "");
                            long fechaLong = long.Parse(fecha);
                            DateTime dt = new DateTime(fechaLong * 10000);
                            dateSaved = dt;
                        }
                        else { dateSaved = value; }

                        //Match match = Regex.Match(value.ToString()

                        //interactionDateSaved = DateTime.SpecifyKind(value, DateTimeKind.Utc); 
                    }
                }
            }

            [LogReference(Name = "EmailNotification")]
            [DataBaseInfo(Name = "tblEmailNotificationLogs")]
            public class EmailNotificationLogs
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "emailNotificationLogID")]
                public int EmailNotificationLogID { get; set; }

                [DataBaseInfo(Name = "emailNotificationID")]
                public int EmailNotificationID { get; set; }

                [DataBaseInfo(Name = "dateSent")]
                public DateTime DateSent { get; set; }

                [DataBaseInfo(Name = "sentByUserID")]
                public Guid SentByUserID { get; set; }

                //[DataBaseInfo(Name = "leadID")]
                //public Guid? LeadID { get; set; }

                //[DataBaseInfo(Name = "reservationID")]
                //public Guid? ReservationID { get; set; }

                //[DataBaseInfo(Name = "purchaseID")]
                //public Guid? PurchaseID { get; set; }
            }

            [LogReference(Name = "OptionSold")]
            [DataBaseInfo(Name = "tblOptionsSold")]
            public class OptionsSoldInformation
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "optionSoldID")]
                public long? OptionSoldID { get; set; }
                [DataBaseInfo(Name = "optionTypeID")]
                public int OptionType { get; set; }
                [DataBaseInfo(Name = "optionID")]
                public int OptionID { get; set; }
                [DataBaseInfo(Name = "quantity")]
                public int Quantity { get; set; }
                [DataBaseInfo(Name = "optionPrice")]
                public string OptionPrice { get; set; }
                [DataBaseInfo(Name = "pointsRedemption")]
                public string PointsRedemption { get; set; }
                [DataBaseInfo(Name = "totalPaid")]
                public string TotalPaid { get; set; }
                [DataBaseInfo(Name = "guestName")]
                public string GuestName { get; set; }
                [DataBaseInfo(Name = "eligibleForCredit")]
                public bool EligibleForCredit { get; set; }
                [DataBaseInfo(Name = "creditAmount")]
                public string CreditAmount { get; set; }
                [DataBaseInfo(Name = "comments")]
                public string Comments { get; set; }
                [DataBaseInfo(Name = "deleted")]
                public bool Deleted { get; set; }
            }
        }
        public class Views
        {
            public class ReservationGeneralInformation : HotelReservationModel.Fields.ReservationGeneralInformation
            {
                [ScriptIgnore]
                public List<SelectListItem> RoomTypes
                {
                    get
                    {
                        List<SelectListItem> RoomTypes = RoomDataModel.GetAllRoomTypes();
                        RoomTypes.Insert(0, Utils.ListItems.Default());
                        return RoomTypes;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> Resorts
                {
                    get
                    {
                        List<SelectListItem> Resorts = PlaceDataModel.GetPlacesByType(1);
                        Resorts.Insert(0, Utils.ListItems.Default());
                        return Resorts;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> Destinations
                {
                    get
                    {
                        return PlaceDataModel.PlacesCatalogs.FillDrpDestinations();
                    }
                }

                private Views.PresentationGeneralInformation _GeneralInformation_PresentationInformation = new Views.PresentationGeneralInformation();
                public Views.PresentationGeneralInformation GeneralInformation_PresentationInformation
                {
                    get { return _GeneralInformation_PresentationInformation; }
                    set { _GeneralInformation_PresentationInformation = value; }
                }


                private Views.ReservationPaymentDetails _GeneralInformation_ReservationPaymentDetails = new ReservationPaymentDetails();
                public Views.ReservationPaymentDetails GeneralInformation_ReservationPaymentDetails
                {
                    get { return _GeneralInformation_ReservationPaymentDetails; }
                    set { _GeneralInformation_ReservationPaymentDetails = value; }
                }

                private Views.FlightInformation _GeneralInformation_FlightInformation = new FlightInformation();
                public Views.FlightInformation GeneralInformation_FlightInformation
                {
                    get { return _GeneralInformation_FlightInformation; }
                    set { _GeneralInformation_FlightInformation = value; }
                }
                private Views.EmailNotificationLogs _GeneralInformation_EmailNotificationLogs = new EmailNotificationLogs();
                public Views.EmailNotificationLogs GeneralInformation_EmailNotificationLogs
                {
                    get { return _GeneralInformation_EmailNotificationLogs; }
                    set { _GeneralInformation_EmailNotificationLogs = value; }
                }
                private Views.OptionsSoldInformation _GeneralInformation_OptionsSoldInformation = new OptionsSoldInformation();
                public Views.OptionsSoldInformation GeneralInformation_OptionsSoldInformation
                {
                    get { return _GeneralInformation_OptionsSoldInformation; }
                    set { _GeneralInformation_OptionsSoldInformation = value; }
                }
                [ScriptIgnore]
                public List<SelectListItem> GreetingReps
                {
                    get
                    {
                        var list = GreetingRepDataModel.GetAllGreetingReps();
                        list.Insert(0, ListItems.Default());
                        return list;
                    }

                }
                [ScriptIgnore]
                public List<SelectListItem> OPCs
                {
                    get
                    {
                        var list = OpcDataModel.GetAllOPCs();
                        list.Insert(0, ListItems.Default());
                        return list;
                    }

                }

            }

            public class PresentationGeneralInformation : Fields.PresentationGeneralInformation
            {
                [ScriptIgnore]
                public List<SelectListItem> Presentations_TourStatus
                {
                    get
                    {
                        List<SelectListItem> TourStatus = TourStatusDataModel.GetAlltourStatus();
                        TourStatus.Insert(0, Utils.ListItems.Default());
                        return TourStatus;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> Presentations_BookingStatus
                {
                    get
                    {
                        List<SelectListItem> BookingStatus = BookingStatusDataModel.GetAllBookingStatus();
                        BookingStatus.Insert(0, Utils.ListItems.Default());
                        return BookingStatus;
                    }
                }


                private Views.ContractsHistory _Presentations_ContractsHistory = new Views.ContractsHistory();
                public Views.ContractsHistory Presentations_ContractsHistory
                {
                    get { return _Presentations_ContractsHistory; }
                    set { _Presentations_ContractsHistory = value; }
                }

            }

            public class ContractsHistory
            {
                [DoNotUpdate, DoNotTrackChanges, IsPrimaryKey]
                [DataBaseInfo(Name = "contractHistoryID")]
                public long ContractHistoryID { get; set; }

                [DataBaseInfo(Name = "salesDate")]
                [Display(Name = "Sales Date")]
                public DateTime? SalesDate { get; set; }
                [DataBaseInfo(Name = "salesVolume")]
                [Display(Name = "Sales Volume")]
                public decimal? SalesVolume { get; set; }
            }

            public class ReservationPaymentDetails : Fields.ReservationPaymentDetails
            {
                [ScriptIgnore]
                public List<SelectListItem> ChargeDescriptions
                {
                    get
                    {
                        List<SelectListItem> Descriptions = ChargesDataModel.GetAllChargeDescriptions();
                        return Descriptions;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> ChargeTypes
                {
                    get
                    {
                        List<SelectListItem> Descriptions = ChargesDataModel.GetAllChargeTypes();
                        return Descriptions;
                    }
                }

            }

            public class FlightInformation : Fields.FlightInformation
            {
                [ScriptIgnore]
                public List<SelectListItem> Airlines
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpAirlines();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> Destinations
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpDestinations();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> FlightTypes
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpFlightTypes();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> TransportationProviders
                {
                    get
                    {
                        var list = MasterChartDataModel.LeadsCatalogs.FillDrpProvidersPerDestinationInTerminals(null, 3);
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> NumberPassengers
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
            }
            public class EmailNotificationLogs : Fields.EmailNotificationLogs
            {
                [ScriptIgnore]
                public List<SelectListItem> EmailNotifications
                {
                    get
                    {
                        var list = MasterChartDataModel.LeadsCatalogs.FillDrpEmailNotifications();
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> Users
                {
                    get
                    {
                        //var list = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey);
                        var list = UserDataModel.GetUsersBySupervisor();
                        
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }
            }
            public class OptionsSoldInformation : Fields.OptionsSoldInformation
            {
                [ScriptIgnore]
                public List<SelectListItem> OptionCategories
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpOptionCategories();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> OptionTypes
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpOptionTypes();
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> OptionsQuantity
                {
                    get
                    {
                        var list = new List<SelectListItem>();
                        for (var i = 1; i <= 20; i++)
                        {
                            list.Add(new SelectListItem()
                            {
                                Value = i.ToString(),
                                Text = i.ToString()
                            });
                        }
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> EligibleForCredits
                {
                    get
                    {
                        var list = ListItems.Booleans();
                        list.Insert(0, ListItems.Default());
                        return list;
                    }
                }
                [ScriptIgnore]
                public List<SelectListItem> OptionPrices
                {
                    get
                    {
                        return MasterChartDataModel.LeadsCatalogs.FillDrpOptionPrice();
                    }
                }
            }
        }
    }

    public class PurchasesModel
    {
        [ScriptIgnore]
        public List<SysComponentsPrivilegesModel> CouponsPrivileges
        {
            get
            {
                return AdminDataModel.GetViewPrivileges(10678);//fdsPurchasesServices
            }
        }
        public List<PurchaseInfoModel> ListPurchases { get; set; }
        public List<RecentCouponsModel> ListRecentCoupons { get; set; }
        public List<PurchaseServiceModel> ListPurchasesServices { get; set; }
        public string JsonRecentCoupons { get; set; }
        public List<PurchasePaymentModel> ListPurchasesPayments { get; set; }

        public List<PurchaseTicketsModel> ListPurchaseTickets { get; set; }

        public class PurchaseInfoModel : PurchasesModel.Views
        {
            [ScriptIgnore]
            public bool ManageServices
            {
                get
                {
                    return new MasterChartDataModel.Purchases().View();
                }
            }

            public Guid? PurchaseInfo_PurchaseID { get; set; }

            public Guid PurchaseInfo_Lead { get; set; }

            [Display(Name = "Language")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
            public string PurchaseInfo_Culture { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpCultures
            {
                get
                {
                    var list = Cultures;
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }
            public string PurchaseInfo_CultureString { get; set; }

            [Display(Name = "Date")]
            [Required(ErrorMessage = "Date is required")]
            public string PurchaseInfo_PurchaseDateTime { get; set; }
            public DateTime? Purchase_PurchaseDateTime { get; set; }

            [Display(Name = "Landing Page")]
            public string PurchaseInfo_OriginalLandingPage { get; set; }

            [Display(Name = "IP Address")]
            public string PurchaseInfo_IPAddress { get; set; }

            [Display(Name = "Apply Promo Code")]
            public int PurchaseInfo_Promo { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpPromos
            {
                get
                {
                    var list = PromosPurchases;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
            public int PurchaseInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpTerminals
            {
                get
                {
                    var list = Terminals;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            public string PurchaseInfo_TerminalString { get; set; }

            [Display(Name = "Customer Requests")]
            public string PurchaseInfo_CustomerRequests { get; set; }

            [Display(Name = "Total")]
            public string PurchaseInfo_Total { get; set; }
            public decimal? Purchase_Total { get; set; }

            [Display(Name = "Paid")]
            public string PurchaseInfo_Paid { get; set; }

            [Display(Name = "Currency")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Currency is required")]
            public string PurchaseInfo_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                    list = list.Where(m => m.Value != "CAD").ToList();
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }

            //[Display(Name = "Purchase Status")]
            [Display(Name = "Status")]
            [Range(1, int.MaxValue, ErrorMessage = "Status is required")]
            public int PurchaseInfo_PurchaseStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpPurchaseStatus
            {
                get
                {
                    var list = PurchaseStatus;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            public string PurchaseInfo_PurchaseStatusString { get; set; }

            [Display(Name = "Reservations Agent")]
            [Required(ErrorMessage = "Reservations Agent is required", AllowEmptyStrings = false)]
            public string PurchaseInfo_Agent { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpAgents
            {
                get
                {
                    var list = Agents;
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }

            //[Display(Name = "Purchase Comments")]
            [Display(Name = "Sales Agent Comments")]
            public string PurchaseInfo_PurchaseComments { get; set; }

            [Display(Name = "Notes")]
            public string PurchaseInfo_Notes { get; set; }

            [Display(Name = "Feedback")]
            public string PurchaseInfo_Feedback { get; set; }

            [Display(Name = "Sales Agent")]
            public string PurchaseInfo_User { get; set; }

            [Display(Name = "Point Of Sale")]
            [Range(1, int.MaxValue, ErrorMessage = "Point Of Sale is required")]
            public int PurchaseInfo_PointOfSale { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpPointsOfSale
            {
                get
                {
                    var list = PointsOfSale;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            
            [Display(Name = "Location")]
            public int? PurchaseInfo_Location { get; set; }
            public List<SelectListItem> PurchaseInfo_DrpLocations
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            public string PurchaseInfo_PointOfSaleString { get; set; }
            public bool PurchaseInfo_PointOfSaleAcceptCharges { get; set; }

            [Display(Name = "Staying At Place")]
            public string PurchaseInfo_StayingAtPlace { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseInfo_DrpStayingAtPlaces
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Insert(0, ListItems.Default("-- Select Terminal--"));
                    //var list = PlaceDataModel.GetResortsByTerminals("", true);
                    //list.Insert(0, ListItems.Default());
                    //list.Add(ListItems.Default("--Other--", "null"));
                    return list;
                }
            }

            [Display(Name = "Staying At")]
            public string PurchaseInfo_StayingAt { get; set; }

            [Display(Name = "Room Number")]
            public string PurchaseInfo_StayingAtRoom { get; set; }

            public string PurchaseInfo_PurchaseServicesQuantity { get; set; }

            [Display(Name = "Days Left")]
            public int Purchase_CountDown { get; set; }
            public string Purchase_CountDownString { get; set; }

            public DateTime? Purchase_ServiceDateTime { get; set; }

            [Display(Name = "Customer Requests")]
            public string Purchase_CustomerRequests { get; set; }

            public decimal PurchaseInfo_BankCommission { get; set; }

            public string PurchaseInfo_FlagsByTerminalPurchase { get; set; }

            public bool PurchaseInfo_AllowUnitsEdition { get; set; }

            public PurchaseServiceModel PurchaseServiceModel { get; set; }

            public PurchasePaymentModel PurchasePaymentModel { get; set; }

            public string PurchaseInfo_MarketingProgram { get; set; }
            public string PurchaseInfo_Subdivision { get; set; }
            public string PurchaseInfo_Source { get; set; }
            public int? PurchaseInfo_SpiOPCID { get; set; }
            public int? PurchaseInfo_OPCID { get; set; }
            public int? PurchaseInfo_TourID { get; set; }
            public string PurchaseInfo_TourDate { get; set; }
        }

        public class PurchaseServiceModel : PurchasesModel.Views
        {
            public string PurchaseService_FirstName { get; set; }
            public string PurchaseService_LastName { get; set; }
            public string PurchaseService_Coupon { get; set; }
            public string PurchaseService_LeadID { get; set; }
            public string PurchaseService_PointOfSaleID { get; set; }

            public long PurchaseService_PurchaseServiceID { get; set; }

            public Guid PurchaseService_Purchase { get; set; }

            public string PurchaseService_MainPicture { get; set; }

            [Display(Name = "Provider")]
            public string PurchaseService_Provider { get; set; }
            public string PurchaseService_ProviderText { get; set; }
            public string PurchaseService_ProviderEmail { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpProviders
            {
                get
                {
                    var list = Providers;
                    list.Insert(0, ListItems.Default("--All--", "", true));
                    //list.Insert(0, ListItems.Default("--Select One--", "0", true));
                    return list;
                }
            }

            [Display(Name = "Activity")]
            [Range(1, int.MaxValue, ErrorMessage = "Activity is required")]
            public long PurchaseService_Service { get; set; }
            public string PurchaseService_ServiceText { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpServices
            {
                get
                {
                    var list = Services;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            public string PurchaseService_ServiceString { get; set; }

            [Display(Name = "Whole Stay")]
            [Required(ErrorMessage = "Whole Stay is required")]
            public bool PurchaseService_WholeStay { get; set; }

            [Display(Name = "Activity Date")]
            [Required(ErrorMessage = "Service Date is required")]
            public string PurchaseService_ServiceDateTime { get; set; }

            [Display(Name = "Availability")]
            //[Range(1, int.MaxValue, ErrorMessage = "Weekly Schedule is required")]
            public long PurchaseService_WeeklyAvailability { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpWeeklyAvailabilities
            {
                get
                {
                    var list = WeeklyAvailabilities;
                    if (list.Count() == 0)
                    {
                        list.Insert(0, ListItems.Default("--Select Service--", "0"));
                    }
                    else
                    {
                        list.Insert(0, ListItems.Default());
                    }
                    return list;
                }
            }
            public string PurchaseService_WeeklyAvailabilityString { get; set; }

            [Display(Name = "Meeting Point")]
            public string PurchaseService_MeetingPoint { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpMeetingPoints
            {
                get
                {
                    var list = MeetingPoints;
                    if (list.Count() == 0)
                    {
                        list.Insert(0, ListItems.Default("--Select Schedule--", "0"));
                    }
                    else
                    {
                        list.Insert(0, ListItems.Default());
                    }
                    return list;
                }
            }
            public string PurchaseService_MeetingPointString { get; set; }

            [Display(Name = "Activity Status")]
            [Range(1, int.MaxValue, ErrorMessage = "Activity Status is required")]
            public int PurchaseService_ServiceStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpServiceStatus
            {
                get
                {
                    var list = ServiceStatus;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }
            public string PurchaseService_ServiceStatusString { get; set; }

            [Display(Name = "Confirmation Date")]
            public string PurchaseService_ConfirmationDateTime { get; set; }

            [Display(Name = "Confirmed By")]
            public string PurchaseService_ConfirmedByUser { get; set; }
            public string PurchaseService_ConfirmedByUserName { get; set; }

            [Display(Name = "Date Saved")]
            public string PurchaseService_DateSaved { get; set; }

            [Display(Name = "Confirmation Number")]
            public string PurchaseService_ConfirmationNumber { get; set; }

            [Display(Name = "Providers Promo")]
            public long PurchaseService_Promo { get; set; }
            public string PurchaseService_PromoString { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpPromos
            {
                get
                {
                    var list = Promos;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Total")]
            //[Required(ErrorMessage = "Total is required")]
            public string PurchaseService_Total { get; set; }

            [Display(Name = "Savings")]
            public string PurchaseService_Savings { get; set; }

            [Display(Name = "Currency")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Currency is required")]
            public string PurchaseService_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }

            [Display(Name = "Reserved For")]
            [Required(ErrorMessage = "Reserved For is required")]
            public string PurchaseService_ReservedFor { get; set; }

            [Display(Name = "Children Ages")]
            public string PurchaseService_ChildrenAges { get; set; }

            [Display(Name = "Custom Meeting Point")]
            public string PurchaseService_CustomMeetingPoint { get; set; }

            [Display(Name = "Custom Meeting Time")]
            public string PurchaseService_CustomMeetingTime { get; set; }

            [Display(Name = "Coupon Notes")]
            public string PurchaseService_Note { get; set; }

            [Display(Name = "Airline")]
            public string PurchaseService_Airline { get; set; }

            [Display(Name = "Flight Number")]
            public string PurchaseService_FlightNumber { get; set; }

            [Display(Name = "Destination")]
            public long PurchaseService_Destination { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpDestinations
            {
                get
                {
                    var list = Destinations;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Transportation Zone")]
            public int PurchaseService_TransportationZone { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpTransportationZones
            {
                get
                {
                    var list = TransportationZones;
                    if (list.Count() == 0)
                    {
                        list.Insert(0, ListItems.Default("--Select Destination--", "0"));
                    }
                    else
                    {
                        list.Insert(0, ListItems.Default());
                    }
                    return list;
                }
            }

            [Display(Name = "Special Request")]
            public string PurchaseService_SpecialRequest { get; set; }

            [Display(Name = "Cancelation Charge")]
            public string PurchaseService_CancelationCharge { get; set; }

            [Display(Name = "Cancelation Date")]
            public string PurchaseService_CancelationDateTime { get; set; }

            [Display(Name = "Canceled/Refunded By")]
            public string PurchaseService_CanceledByUser { get; set; }
            public string PurchaseService_CanceledByUserName { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpUsers
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpSalesAgents(null, true);
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
            }

            [Display(Name = "Cancelation Number")]
            public string PurchaseService_CancelationNumber { get; set; }

            [Display(Name = "Audit")]
            public bool PurchaseService_Audit { get; set; }

            [Display(Name = "CloseOut")]
            public bool PurchaseService_CloseOut { get; set; }

            [Display(Name = "Issued")]
            public bool PurchaseService_Issued { get; set; }
            public bool PurchaseService_IssuedValue { get; set; }

            [Display(Name = "Round Trip")]
            public bool PurchaseService_Round { get; set; }

            [Display(Name = "Round Airline")]
            public string PurchaseService_RoundAirline { get; set; }

            [Display(Name = "Round Flight Number")]
            public string PurchaseService_RoundFlightNumber { get; set; }

            [Display(Name = "Round Flight Time")]
            public string PurchaseService_RoundFlightTime { get; set; }

            [Display(Name = "Round Date")]
            public string PurchaseService_RoundDate { get; set; }

            [Display(Name = "Round Meeting Time")]
            public string PurchaseService_RoundMeetingTime { get; set; }

            public string PurchaseService_CurrentUnits { get; set; }
            //[ScriptIgnore]
            public List<ListPurchaseServiceDetailModel> ListPurchaseServiceDetails { get; set; }

            public string PurchaseService_CouponNumber { get; set; }

            [Display(Name = "Replacement Of")]
            public string PurchaseService_ReplacementOf { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpSiblingsCoupons
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpSiblingsCoupons("null");
                    list.Insert(0, ListItems.NotSet("--None--"));
                    return list;
                }
            }

            [Display(Name = "Open Coupon")]
            public int PurchaseService_OpenCoupon { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseService_DrpOpenCoupons
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Insert(0, new SelectListItem() { Value = "0", Text = "Specific Date" });
                    list.Insert(1, new SelectListItem() { Value = "1", Text = "One Month" });
                    list.Insert(2, new SelectListItem() { Value = "3", Text = "Three Months" });
                    list.Insert(3, new SelectListItem() { Value = "6", Text = "Six Months" });
                    list.Insert(4, new SelectListItem() { Value = "12", Text = "One Year" });
                    return list;
                }
            }

            [Display(Name = "Coupon Reference")]
            [RegularExpression(@"(([A-Z,Ñ]+[0-9]+)|([0-9]+[A-Z,Ñ]+))$", ErrorMessage = "Format has to fit system's folios")]
            public string PurchaseService_CouponReference { get; set; }

            public bool PurchaseService_IsClosed { get; set; }

            [Display(Name = "Sold By OPC")]
            public bool PurchaseService_SoldByOPC { get; set; }
            public bool PurchaseService_AvoidEdition { get; set; }
            public PurchaseServiceModel()
            {
                PurchaseService_WholeStay = false;
                PurchaseService_Audit = false;
                PurchaseService_CloseOut = false;
                PurchaseService_Issued = false;
                PurchaseService_Round = false;
            }

            public PurchaseServiceDetailModel PurchaseServiceDetailModel { get; set; }
        }

        public class PurchasePaymentModel : PurchasesModel.Views
        {
            public long PurchasePayment_PaymentDetailsID { get; set; }

            public Guid PurchasePayment_Purchase { get; set; }

            [Display(Name = "Transaction Type")]
            public string PurchasePayment_TransactionType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpTransactionTypes
            {
                get
                {
                    return TransactionTypes;
                }
            }

            [Display(Name = "Payment Type")]
            [Required(ErrorMessage = "Payment Type is required", AllowEmptyStrings = false)]
            public string PurchasePayment_PaymentType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpPaymentTypes
            {
                get
                {
                    return PaymentTypes;
                }
            }

            [Display(Name = "Payment Date")]
            [Required(ErrorMessage = "Payment Date is required")]
            public string PurchasePayment_DateSaved { get; set; }

            [Display(Name = "Certificate Number(s)")]
            public string PurchasePayment_CertificateNumbers { get; set; }

            [Display(Name = "Concept")]
            public string PurchasePayment_ChargeBackConcept { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpChargeBackConcepts
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpEgressConcepts(null, 3).OrderByDescending(m => m.Text);
                    return list.ToList();
                }
            }

            [Display(Name = "Apply Charge")]
            public bool PurchasePayment_ApplyCharge { get; set; }

            [Display(Name = "Transaction ID")]
            public string PurchasePayment_TransactionCode { get; set; }

            [Display(Name = "Amount")]
            [Required(ErrorMessage = "Amount is required")]
            public decimal PurchasePayment_Amount { get; set; }

            public decimal PurchasePayment_ExchangeRate { get; set; }

            [Display(Name = "Currency")]
            [Required(ErrorMessage = "Currency is required", AllowEmptyStrings = false)]
            public string PurchasePayment_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpCurrencies
            {
                get
                {
                    return Currencies;
                }
            }

            [Display(Name = "Coupon(s)")]
            [ArrayMinLength(ErrorMessage = "Coupon(s) is required", minLength = 1)]
            public string[] PurchasePayment_Coupons { get; set; }
            public string PurchasePayment_ChangeCouponStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpCoupons
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCouponsByPurchase("");
                    return list;
                }
            }
            public string PurchasePayment_CouponsServices { get; set; }

            [Display(Name = "Billing Info")]
            [RequiredIf(OtherProperty = "PurchasePayment_PaymentType", EqualsTo = 2, ErrorMessage = "Billing Info is required")]
            public string PurchasePayment_BillingInfo { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpBillingInfo
            {
                get
                {
                    var list = BillingInfo;
                    list.Insert(0, ListItems.Default());
                    list.Insert(0, ListItems.Default("+Add Billing Info", "-1"));
                    list.Insert(0, ListItems.Default("+Add reference Number", "-2"));
                    return list;
                }
            }

            [Display(Name = "Card Number")]
            [RequiredIf(OtherProperty = "PurchasePayment_BillingInfo", EqualsTo = "-2", ErrorMessage = "Card Number is required")]
            public string PurchasePayment_ReferenceNumber { get; set; }

            [Display(Name = "Card Type")]
            public int? PurchasePayment_CardType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpCardTypes
            {
                get
                {
                    var list = CreditCardsDataModel.GetAllCreditCardTypes();
                    return list;
                }
            }

            [Display(Name = "OPC")]
            //[RequiredIf(OtherProperty = "PurchasePayment_PaymentType", EqualsTo = "3")]
            public string PurchasePayment_OPC { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpOPCS
            {
                get
                {
                    var list = OPCS;
                    list.Insert(0, ListItems.Default("--Select One--", ""));
                    return list;
                }
                set { }
            }

            [Display(Name = "OPC Telephone Number")]
            public string PurchasePayment_OPCNumber { get; set; }

            [Display(Name = "OPC Name")]
            [RequiredIf(OtherProperty = "PurchasePayment_OPC", EqualsTo = "null", ErrorMessage = "OPC Name is required")]
            public string PurchasePayment_Other { get; set; }

            [Display(Name = "Select Company")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Select Company is required")]
            public string PurchasePayment_Company { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpCompanies
            {
                get
                {
                    //return Companies;
                    //return MasterChartDataModel.LeadsCatalogs.FillDrpMarketingCompaniesPerTerminals(null);
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCompaniesPerPromotionTeams("null", 0);
                }
            }

            [Display(Name = "Location")]
            public string PurchasePayment_Location { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpLocations
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpLocationsPerCurrentTerminals(0);
                }
            }

            [Display(Name = "Invitation")]
            public string PurchasePayment_Invitation { get; set; }

            [Display(Name = "Promotion Team")]
            public string PurchasePayment_PromotionTeam { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpPromotionTeams
            {
                get
                {
                    return PromotionTeams;
                }
            }

            [Display(Name = "Comments")]
            public string PurchasePayment_PaymentComments { get; set; }

            [Display(Name = "Refund Bank Account")]
            public string PurchasePayment_RefundAccount { get; set; }

            [Display(Name = "Charge Commission Included")]
            public bool PurchasePayment_ApplyCommission { get; set; }
            public decimal? PurchasePayment_CardCommission { get; set; }

            [Display(Name = "Budget")]
            public string PurchasePayment_Budget { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchasePayment_DrpBudgets
            {
                get
                {
                    var list = CatalogsDataModel.Budgets.BudgetsCatalogs.FillDrpBudgetsPerTeam();
                    list.Insert(0, ListItems.Default("--None--"));
                    return list;

                }
            }

            [Display(Name = "Use Extension")]
            public bool PurchasePayment_IsVoucher { get; set; }

            public string PurchasePayment_PurchaseReference { get; set; }

            public string PurchasePayment_AuthCode { get; set; }

            public string PurchasePayment_ErrorCode { get; set; }

            public string PurchasePaymentInfo_DateSaved { get; set; }

            public bool PurchasePaymentInfo_IsRelatedToCloseOut { get; set; }

            public bool PurchasePaymentInfo_Deleted { get; set; }

            public PurchasePaymentModel()
            {
                PurchasePayment_ApplyCharge = true;
                PurchasePayment_ApplyCommission = true;
                PurchasePayment_IsVoucher = false;
            }
        }

        public class PurchaseServiceDetailModel : PurchasesModel.Views
        {
            public long PurchaseServiceDetail_PurchaseServiceDetailID { get; set; }

            public long PurchaseServiceDetail_PurchaseService { get; set; }

            [Display(Name = "Price Type")]
            public int PurchaseServiceDetail_PriceType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseServiceDetail_DrpPriceTypes
            {
                get
                {
                    var list = PriceTypes;
                    list.Insert(0, ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Price")]
            public long PurchaseServiceDetail_Price { get; set; }
            public string PurchaseServiceDetail_PriceString { get; set; }

            [Display(Name = "Unit")]
            public string PurchaseServiceDetail_PriceUnit { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseServiceDetail_DrpPriceUnits
            {
                get
                {
                    var list = PricesUnits;
                    if (list.Count() == 0)
                    {
                        list.Insert(0, ListItems.Default("--Select Price--"));
                    }
                    else
                    {
                        list.Insert(0, ListItems.Default());
                    }
                    return list;
                }
            }
            public string PurchaseServiceDetail_PriceUnitString { get; set; }

            [Display(Name = "Quantity")]
            public string PurchaseServiceDetail_Quantity { get; set; }

            [Display(Name = "Coupon")]
            public string PurchaseServiceDetail_Coupon { get; set; }

            public string PurchaseServiceDetail_CustomPrice { get; set; }

            public string PurchaseServiceDetail_ExchangeRate { get; set; }

            public long PurchaseServiceDetail_NetPrice { get; set; }
            public string PurchaseServiceDetail_NetPriceString { get; set; }

            [Display(Name = "Apply Promo")]
            public bool PurchaseServiceDetail_Promo { get; set; }
            public bool PurchaseServiceDetail_PromoValue { get; set; }
        }

        public class ListPurchaseServiceDetailModel
        {
            public long ServiceDetailID { get; set; }
            public long PurchaseServiceID { get; set; }
            public string Unit { get; set; }
            public long PriceID { get; set; }
            public decimal Quantity { get; set; }
            public string Coupon { get; set; }
            public decimal CustomPrice { get; set; }
            public bool Promo { get; set; }
            public string PriceType { get; set; }
            public int? PriceTypeID { get; set; }
            public decimal? DealPrice { get; set; }
            public long? ExchangeRateID { get; set; }
        }

        public class Views
        {
            [ScriptIgnore]
            public List<SelectListItem> Cultures
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCultures();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Promos
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPromosPerService(null);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PromosPurchases
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPromosPerPurchase();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Terminals
            {
                get
                {
                    //return TerminalDataModel.GetCurrentUserTerminals();
                    return TerminalDataModel.GetActiveTerminalsList();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrencies();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PurchaseStatus
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPurchaseStatus();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Agents
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSalesAgents(null);
                    //return MasterChartDataModel.LeadsCatalogs.ListWorkGroupUsers();
                    //return MasterChartDataModel.LeadsCatalogs.FillDrpCurrentWorkGroupAgents();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PointsOfSale
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Providers
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpProvidersPerDestinationInTerminals();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Services
            {
                get
                {

                    return MasterChartDataModel.LeadsCatalogs.FillDrpServices(0);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Destinations
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpDestinations();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> TransportationZones
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpTransportationZones(0);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> WeeklyAvailabilities
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpWeeklyAvailability(0);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> MeetingPoints
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpMeetingPoints(0);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> ServiceStatus
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpServiceStatus();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PriceTypes
            {
                get
                {
                    //return MasterChartDataModel.LeadsCatalogs.FillDrpPriceTypes();
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPriceTypeRules(0, 0, 0, 0);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PricesUnits
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPricesUnits(0, 0, null, null, "", 0, null);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PaymentTypes
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPaymentTypes(0);
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> OPCS
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpOPC();
                    
                    //list.Insert(list.Count(), ListItems.NotSet("--Not Registered--"));
                    return list;
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Companies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCompaniesPerOPC("null");
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PromotionTeams
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPromotionTeamsPerOPC("null");
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> BillingInfo
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpBillingInfo("");
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> TransactionTypes
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpTransactionTypes();
                }
            }
        }

        public class TicketInfoModel
        {
            [Display(Name = "TicketInfo_DateSaved", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_DateSaved { get; set; }

            [Display(Name = "TicketInfo_DateIssued", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_DateIssued { get; set; }

            [Display(Name = "TicketInfo_Culture", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Culture { get; set; }

            [Display(Name = "TicketInfo_PointOfSale", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_PointOfSale { get; set; }

            [Display(Name = "TicketInfo_ExchangeRate", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_ExchangeRate { get; set; }

            [Display(Name = "TicketInfo_Phone", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Phone { get; set; }

            [Display(Name = "TicketInfo_Customer", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Customer { get; set; }

            [Display(Name = "TicketInfo_ComercialName", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_ComercialName { get; set; }

            [Display(Name = "TicketInfo_BankLegend", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_BankLegend { get; set; }

            [Display(Name = "TicketInfo_Terminal", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Terminal { get; set; }
            public long TicketInfo_TerminalID { get; set; }

            [Display(Name = "TicketInfo_Company", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Company { get; set; }

            [Display(Name = "TicketInfo_TicketNumber", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_TicketNumber { get; set; }

            [Display(Name = "TicketInfo_UserName", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_UserName { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_Services", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<SelectListItem> TicketInfo_Services { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_CashPayments", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_CashPayments { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_CCPayments", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_CCPayments { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_TCPayments", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_TCPayments { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_WTPayments", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_WTPayments { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_CashRefunds", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_CashRefunds { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_CCRefunds", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_CCRefunds { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_TCRefunds", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_TCRefunds { get; set; }

            //[ScriptIgnore]
            [Display(Name = "TicketInfo_WTRefunds", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public List<PurchasesModel.TicketPaymentModel> TicketInfo_WTRefunds { get; set; }

            [Display(Name = "TicketInfo_TotalPaidUSD", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalPaidUSD { get; set; }

            [Display(Name = "TicketInfo_TotalPaidMXN", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalPaidMXN { get; set; }

            [Display(Name = "TicketInfo_TotalPaidCAD", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalPaidCAD { get; set; }

            [Display(Name = "TicketInfo_TotalRefundUSD", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalRefundUSD { get; set; }

            [Display(Name = "TicketInfo_TotalRefundMXN", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalRefundMXN { get; set; }

            [Display(Name = "TicketInfo_TotalRefundCAD", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalRefundCAD { get; set; }

            [Display(Name = "TicketInfo_Signature", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Signature { get; set; }

            [Display(Name = "TicketInfo_SignatureAgent", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_SignatureAgent { get; set; }

            [Display(Name = "TicketInfo_Gratitude", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Gratitude { get; set; }

            [Display(Name = "TicketInfo_Policies", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Policies { get; set; }

            [Display(Name = "TicketInfo_Comments", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Comments { get; set; }

            [Display(Name = "TicketInfo_URL")]
            public string TicketInfo_URL { get; set; }
            [Display(Name = "TicketInfo_Transactions", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_Transactions { get; set; }
            [Display(Name = "TicketInfo_PurchaseDetails", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_PurchaseDetails { get; set; }
            [Display(Name = "TicketInfo_TotalPaid", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_TotalPaid { get; set; }
            [Display(Name = "TicketInfo_TotalRefund", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public string TicketInfo_TotalRefund { get; set; }

            [Display(Name = "TicketInfo_PurchaseTotal", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_PurchaseTotal { get; set; }
            [Display(Name = "TicketInfo_PurchaseTotalMXN", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_PurchaseTotalMXN { get; set; }
            [Display(Name = "TicketInfo_TotalPaidCashUSD", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_TotalPaidCashUSD { get; set; }
            //[Display(Name = "TicketInfo_TotalPaidCashMXN", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            //public decimal TicketInfo_TotalPaidCashMXN { get; set; }
            public string TicketInfo_TotalPaidLetter { get; set; }
            [Display(Name = "TicketInfo_ChangeDue", ResourceType = typeof(Resources.Models.Shared.PurchaseTicket))]
            public decimal TicketInfo_ChangeDue { get; set; }
            public decimal TicketInfo_ChangeDueMXN { get; set; }
        }

        public class TicketPaymentModel
        {
            public int PaymentType { get; set; }
            public string CardNumber { get; set; }
            public string CardType { get; set; }
            public string Currency { get; set; }
            public decimal Amount { get; set; }
        }

        public class ChargebackTicketModel
        {
            public int PaymentID { get; set; }
            public string Folio { get; set; }
            public string Invitation { get; set; }
            public string PersonToCharge { get; set; }
            public string Signature { get; set; }
            public string PayingCompany { get; set; }
            public string ClientName { get; set; }
            public string ProgramOPC { get; set; }
            public string BudgetLetter { get; set; }
            public string Notes { get; set; }
            public string TotalDue { get; set; }
            public string Legend { get; set; }
            public string AmountPaidByCustomer { get; set; }
            public string AmountPaidByCustomerMXN { get; set; }
            public string TotalDue2 { get; set; }
            public string BudgetAmount { get; set; }
            public string SalesAgent { get; set; }
            public string ExchangeRateUsed { get; set; }
            public string TotalUSD { get; set; }
            public string TotalMXN { get; set; }
            public string PointOfSale { get; set; }
            public string Date { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Services { get; set; }
            public string Container { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Egresses { get; set; }
            public string DepositAppliedToTours { get; set; }
            public string DepositAppliedToToursMXN { get; set; }
            public string CashGiftUSD { get; set; }
            public string CommissionCashGiftUSD { get; set; }
            public string CashActivityUSD { get; set; }
            public string CashGiftMXN { get; set; }
            public string CommissionCashGiftMXN { get; set; }
            public string CashActivityMXN { get; set; }
            public string TotalToChargeBackInUSD { get; set; }
            public string TotalToChargeBackInMXN { get; set; }
            public string TotalToChargeUSD { get; set; }
            public string TotalToChargeMXN { get; set; }
            public string Enterprise { get; set; }
            public string RFC { get; set; }
            public string Address { get; set; }
        }

        public class RecentCouponsResultsModel
        {
            public string PointOfSale { get; set; }
            public string DateSaved { get; set; }
            //[ScriptIgnore]
            public List<RecentCouponsModel> Coupons { get; set; }
            public string DateLastUpdate { get; set; }
        }

        public class RecentCouponsModel
        {
            public string DT_RowId { get; set; }
            public Guid RecentCoupon_PurchaseID { get; set; }
            public Guid RecentCoupon_LeadID { get; set; }
            public string RecentCoupon_FirstName { get; set; }
            public string RecentCoupon_Coupon { get; set; }
            public string RecentCoupon_ServiceStatus { get; set; }
            public string RecentCoupon_Total { get; set; }
            public string RecentCoupon_DateSaved { get; set; }
            public string RecentCoupon_ServiceDate { get; set; }
            public string RecentCoupon_Location { get; set; }
        }

        public class PurchaseTicketsModel
        {
            public int PurchaseTicketID { get; set; }
            public string PurchaseID { get; set; }
            public string PurchaseTotal { get; set; }
            public string JsonTicket { get; set; }
            public string SavedByUser { get; set; }
            public string DateSaved { get; set; }
            public string Reprints { get; set; }
        }
    }

    public class FastSaleModel
    {
        public class FastSaleInfoModel : SPIViewModel.AgencyCustomer
        {
            [Display(Name = "Title")]
            public int FastSaleInfo_Title { get; set; }
            public List<SelectListItem> FastSaleInfo_DrpTitles
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPersonalTitles(null);
                }
            }

            [Display(Name = "First Name")]
            [Required]
            public string FastSaleInfo_FirstName { get; set; }

            [Display(Name = "Last Name")]
            [Required]
            public string FastSaleInfo_LastName { get; set; }

            [Display(Name = "Email")]
            public string FastSaleInfo_Email { get; set; }

            [Display(Name = "Phone")]
            //[RegularExpression(@"^(\d{10,15})$", ErrorMessage = "Entered phone format is not valid.")]
            public string FastSaleInfo_Phone { get; set; }

            [Display(Name = "Terminal")]
            [Required]
            public long FastSaleInfo_Terminal { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> FastSaleInfo_DrpTerminals
            {
                get
                {
                    var list = TerminalDataModel.GetActiveTerminalsList();
                    if (list.Count() > 1)
                    {
                        list.Insert(0, ListItems.Default());
                    }
                    return list;
                }
            }

            [Display(Name = "Point Of Sale")]
            public string FastSaleInfo_PointOfSale { get; set; }

            /// <summary>
            /// Return points of sale of selected terminals
            /// </summary>
            [ScriptIgnore]
            public List<SelectListItem> FastSaleInfo_DrpPointsOfSale
            {
                get
                {
                    return CatalogsDataModel.PointsOfSale.PoinsOfSaleCatalogs.FillDrpPointsOfSale();
                }
            }

            [Display(Name = "Prices Currency")]
            public int FastSaleInfo_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> FastSaleInfo_DrpCurrencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD();
                }
            }

            [Display(Name = "Coupon Language")]
            public string FastSaleInfo_Language { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> FastSaleInfo_DrpLanguages
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCultures();
                }
            }

            [Display(Name = "Staying At Place")]
            public string FastSaleInfo_StayingAtPlace { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> FastSaleInfo_DrpPlaces
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "Other Place")]
            public string FastSaleInfo_StayingAtOhterPlace { get; set; }

            [Display(Name = "Room Number")]
            public string FastSaleInfo_RoomNumber { get; set; }

            [Display(Name = "Source")]
            public int FastSaleInfo_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> FastSaleInfo_DrpLeadSourcesPerSysWorkGroup
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                }
            }

            [Display(Name = "Location")]
            public int FastSaleInfo_Location { get; set; }
            public List<SelectListItem> FastSaleInfo_DrpLocations
            {
                get
                {
                    //return new List<SelectListItem>();
                    return MasterChartDataModel.LeadsCatalogs.FillDrpLocationsPerCurrentTerminals(0);
                }
            }
            public List<SysComponentsPrivilegesModel> Privileges { get; set; }
            //public string FastSaleInfo_MarketingProgram { get; set; }
            //public string FastSaleInfo_Subdivision { get; set; }
            //public string FastSaleInfo_Source { get; set; }
            //public int? FastSaleInfo_SpiOPCID { get; set; }
            //public int? FastSaleInfo_OPCID { get; set; }
            //public int? FastSaleInfo_TourID { get; set; }
            //public string FastSaleInfo_FrontOfficeGuestID { get; set; }
            //public string FastSaleInfo_FrontOfficeResortID { get; set; }
            //public int? FastSaleInfo_CustomerID { get; set; }
            //public string FastSaleInfo_OPC { get; set; }
            //public string FastSaleInfo_Country { get; set; }

        }

        public List<SPIViewModel.AgencyCustomer> ListManifest { get; set; }
    }
}