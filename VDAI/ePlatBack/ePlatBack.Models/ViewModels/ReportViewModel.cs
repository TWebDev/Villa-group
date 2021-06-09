using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using ePlatBack.Models;
using System.ComponentModel.DataAnnotations;
using ePlatBack.Models.Utils.Custom.Attributes;
using ePlatBack.Models.Utils;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class ReportViewModel
    {
        public string ReportName { get; set; }

        public string ReportTag { get; set; }
    }

    public class ArrivalsReport
    {
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public class SearchArrivalsModel
        {
            [Display(Name = "Arrival Date")]
            public string SearchArrival_I_ArrivalDate { get; set; }
            public string SearchArrival_F_ArrivalDate { get; set; }

            [Display(Name = "Resort")]
            public string[] SearchArrival_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchArrival_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Booking Status")]
            public string[] SearchArrival_BookingStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchArrival_DrpBookinStatus
            {
                get
                {
                    //return BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                    return BookingStatusDataModel.GetSecondaryBookingStatus();
                }
            }

            [Display(Name = "Lead Source")]
            public string[] SearchArrival_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchArrival_DrpLeadSources
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpLeadSourcesByWorkGroup();
                }
            }

            [Display(Name = "Assigned To User")]
            public string[] SearchArrival_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchArrival_DrpAssignedToUsers
            {
                get
                {
                    //var user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor();
                }
            }
        }

        public List<ArrivalsReportModel> ListArrivalsReportModel { get; set; }
    }

    public class DuplicateLeadsReport
    {
        public class SearchDuplicateLeadsModel
        {
            [Display(Name = "Arrival Date")]
            public string SearchDuplicateLeads_I_ArrivalDate { get; set; }
            public string SearchDuplicateLeads_F_ArrivalDate { get; set; }

            [Display(Name = "Resort")]
            public string[] SearchDuplicateLeads_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDuplicateLeads_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Lead Source")]
            public string[] SearchDuplicateLeads_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDuplicateLeads_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                }
            }

            [Display(Name = "Lead Status")]
            public string[] SearchDuplicateLeads_LeadStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDuplicateLeads_DrpLeadStatus
            {
                get
                {
                    return LeadStatusDataModel.GetLeadStatusByTerminal();
                }
            }

            [Display(Name = "Assigned To User")]
            public string[] SearchDuplicateLeads_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDuplicateLeads_DrpAssignedToUsers
            {
                get
                {
                    //var user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor();
                }
            }
        }

        public List<DuplicateLeadsReportModel> ListDuplicateLeadsReportModel { get; set; }

        public LeadModel.Views.MassUpdate MassUpdateModel { get; set; }
    }

    public class IndicatorsModel
    {
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public class SearchIndicatorsModel
        {
            [Required(ErrorMessage = "At least one date range is required")]
            public string SearchIndicator_DatesFlag { get; set; }

            [Display(Name = "Arrival Date")]
            [RequiredIf(OtherProperty = "SearchIndicator_I_TourDate", EqualsTo = "", ErrorMessage = "At least One Date is Required")]
            public string SearchIndicator_I_ArrivalDate { get; set; }

            //[Display(Name = "Arrival Date")]
            //[RequiredIf("SearchIndicator_I_ArrivalDate", ErrorMessage = "At least one date range is required")]
            public string SearchIndicator_F_ArrivalDate { get; set; }

            [Display(Name = "Tour Date")]
            [RequiredIf(OtherProperty = "SearchIndicator_I_ArrivalDate", EqualsTo = "", ErrorMessage = "At least One Date is Required")]
            public string SearchIndicator_I_TourDate { get; set; }

            //[Display(Name = "Final Tour Date")]
            //[RequiredIf("SearchIndicator_I_TourDate", ErrorMessage = "At least one date range is required")]
            public string SearchIndicator_F_TourDate { get; set; }

            [Display(Name = "Lead Source")]
            [Required(ErrorMessage = "At least one lead source is required")]
            //public string[] SearchIndicator_LeadSource { get; set; }
            public long?[] SearchIndicator_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchIndicator_DrpLeadSources
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpLeadSourcesByWorkGroup();
                }
            }

            [Display(Name = "Resort")]
            [Required(ErrorMessage = "At least one resort is required")]
            //public string[] SearchIndicator_Resort { get; set; }
            public long?[] SearchIndicator_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchIndicator_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Assigned To User")]
            [Required(ErrorMessage = "User is required")]
            public string[] SearchIndicator_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchIndicator_DrpAssignedToUsers
            {
                get
                {
                    //var user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor();
                }
            }

            public SearchIndicatorsModel()
            {
                SearchIndicator_DatesFlag = null;
            }
        }

        public List<IndicatorsReportModel> ListIndicatorsReportModel { get; set; }
    }

    public class PipelineModel
    {
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public class SearchPipelineModel
        {
            [Required(ErrorMessage = "Date range is required")]
            [Display(Name = "Arrival Date")]
            public string SearchPipeline_I_ArrivalDate { get; set; }

            [Required(ErrorMessage = "Date range is required")]
            public string SearchPipeline_F_ArrivalDate { get; set; }

            [Display(Name = "Lead Source")]
            public string[] SearchPipeline_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPipeline_DrpLeadSources
            {
                get
                {
                    //return ReportDataModel.ReportsCatalogs.FillDrpLeadSourcesByWorkGroup();
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }

            [Display(Name = "Resort")]
            public string[] SearchPipeline_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPipeline_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Assigned To User")]
            public string[] SearchPipeline_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPipeline_DrpAssignedToUsers
            {
                get
                {
                    //var _user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor();
                }
            }

        }

        public List<PipelineReportModel> ListPipelineReportModel { get; set; }
    }

    public class ExchangeTourModel
    {

        public class SearchExchangeTourModel : GenericSearchFieldsReportModel
        {
            [Display(Name = "Final Tour Date")]
            public string SearchExchangeTour_I_FinalTourDate { get; set; }
            public string SearchExchangeTour_F_FinalTourDate { get; set; }
        }

        public List<ExchangeTourReportModel> ListExchangeToursReportModel { get; set; }
    }

    public class PreBookedArrivalsModel
    {
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        public List<PreBookedArrivalsReportModel> ListPreBookedArrivalsReportModel { get; set; }

        public class SearchPreBookedArrivalsModel : GenericSearchFieldsReportModel
        {
            [Display(Name = "Purchase Date")]
            public string SearchPreBooked_I_PurchaseDate { get; set; }

            public string SearchPreBooked_F_PurchaseDate { get; set; }

            [Display(Name = "Include W/Optionals")]
            public bool SearchPreBooked_HasOptions { get; set; }

            public SearchPreBookedArrivalsModel()
            {
                SearchPreBooked_HasOptions = false;
            }
        }
    }

    public class NewReferralsModel
    {
        public List<NewReferralsReportModel> ListNewReferralsReport { get; set; }

        public class SearchNewReferralsModel
        {
            [Display(Name = "Date Range")]
            public string SearchNewReferrals_I_DateRange { get; set; }

            public string SearchNewReferrals_F_DateRange { get; set; }

            [Display(Name = "Input By User")]
            public Guid[] SearchNewReferrals_InputByUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchNewReferrals_DrpInputByUser
            {
                get
                {
                    //var user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor();
                }
            }
        }
    }

    public class WeeklyModel
    {
        public List<WeeklyReportModel> ListWeeklyReport { get; set; }

        public class SearchWeeklyModel
        {
            [Display(Name = "Final Tour Date")]
            public string SearchWeekly_I_PresentationDate { get; set; }

            public string SearchWeekly_F_PresentationDate { get; set; }

            [Display(Name = "Booking Status")]
            public string[] SearchWeekly_BookingStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchWeekly_DrpBookinStatus
            {
                get
                {
                    //return BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                    return BookingStatusDataModel.GetSecondaryBookingStatus();
                }
            }

            [Display(Name = "Resort")]
            public string[] SearchWeekly_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchWeekly_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Lead Source")]
            public string[] SearchWeekly_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchWeekly_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                }
            }

            [Display(Name = "Assigned To User")]
            public string[] SearchWeekly_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchWeekly_DrpAssignedToUsers
            {
                get
                {
                    //var _user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor();
                }
            }
        }
    }

    public class ReservationsMadeModel
    {
        public List<ReservationsMadeReportModel> ListReservationsMade { get; set; }

        public class SearchReservationsMadeModel
        {
            [Display(Name = "Input Date")]
            public string SearchReservationsMade_I_InputDate { get; set; }
            public string SearchReservationsMade_F_InputDate { get; set; }

            [Display(Name = "Reservation Made Date")]
            public string SearchReservationsMade_I_ReservationMadeDate { get; set; }
            public string SearchReservationsMade_F_ReservationMadeDate { get; set; }

            //[Display(Name = "User(s)")]
            //public string[] SearchReservationsMade_User { get; set; }
            //public List<SelectListItem> SearchReservationsMade_DrpUsers
            //{
            //    get
            //    {
            //        var list = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
            //        return list;
            //    }
            //}
        }
    }

    //public class NewReferralLeadsModel
    //{
    //    public List<NewReferralLeadsReportModel> ListNewReferralLeadsReportModel { get; set; }

    //    public List<GenericStringModel> ListProperties { get; set; }

    //    public class SearchNewReferralLeadsModel
    //    {
    //        public Guid SearchNewReferralLeads_UserID { get; set; }

    //        [Display(Name = "Date Range")]
    //        public string SearchNewReferralLeads_I_DateRange { get; set; }

    //        public string SearchNewReferralLeads_F_DateRange { get; set; }

    //        [Display(Name = "Input By User")]
    //        public string[] SearchNewReferralLeads_InputByUser { get; set; }
    //        public List<SelectListItem> SearchNewReferralLeads_DrpInputByUsers
    //        {
    //            get
    //            {
    //                MembershipUser CurrentUser = Membership.GetUser();
    //                List<SelectListItem> Users = UserDataModel.GetUsersBySupervisor((Guid)CurrentUser.ProviderUserKey);
    //                return Users;
    //            }
    //        }
    //    }
    //}

    public class PreBookedContactedLeadsModel
    {
        public List<PreBookedContactedLeadsReportModel> ListPreBookedContactedLeadsReport { get; set; }

        public class SearchPreBookedContactedLeadsModel : GenericSearchFieldsReportModel
        {
            [Display(Name = "Resort")]
            [Required(ErrorMessage = "At least one resort is required")]
            public string[] GenericSearchFieldsReport_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> GenericSearchFieldsReport_DrpResorts
            {
                get
                {
                    //List<SelectListItem> TheList = ReportDataModel.ReportsCatalogs.FillDrpResorts();
                    //TheList.Insert(0, Utils.ListItems.Default());
                    //return TheList;
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Exclude Duplicated")]
            public bool SearchPreBookedContactedLeads_ExcludeDuplicated { get; set; }

            public SearchPreBookedContactedLeadsModel()
            {
                SearchPreBookedContactedLeads_ExcludeDuplicated = true;
            }
        }
    }

    public class OptionsSoldModel
    {
        public List<OptionsSoldReportModel> ListOptionsSoldReportModel { get; set; }

        public class SearchOptionsSoldModel : GenericSearchFieldsReportModel
        {
            [Display(Name = "Resort")]
            [Required(ErrorMessage = "At least one resort is required")]
            public string[] GenericSearchFieldsReport_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> GenericSearchFieldsReport_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "User")]
            public string[] SearchOptionsSold_User { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchOptionsSold_DrpUsers
            {
                get
                {
                    //var list = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
                    return UserDataModel.GetUsersBySupervisor(null, true);
                }
            }
        }
    }

    public class ConfirmedLeadsModel
    {
        public List<ConfirmedLeadsReportModel> ListConfirmedLeadsReportModel { get; set; }

        public class SearchConfirmedLeadsModel : GenericSearchFieldsReportModel
        {
            [Display(Name = "Assigned To User")]
            public string[] SearchConfirmedLeads_User { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchConfirmedLeads_DrpUsers
            {
                get
                {
                    return UserDataModel.GetUsersBySupervisor();
                }
            }

            [Display(Name = "Tour Status")]
            public string[] SearchConfirmedLeads_TourStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchConfirmedLeads_DrpTourStatus
            {
                get
                {
                    List<SelectListItem> TheList = ReportDataModel.ReportsCatalogs.FillDrpTourStatus();
                    return TheList;
                }
            }
        }
    }

    public class DiamanteModel
    {
        public List<DiamanteReportModel> ListDiamanteReportModel { get; set; }
        public class SearchDiamante
        {
            [Display(Name = "Arrival Date")]
            public string SearchDiamante_I_ArrivalDate { get; set; }
            public string SearchDiamante_F_ArrivalDate { get; set; }

            [Display(Name = "Lead Source")]
            public long?[] SearchDiamante_LeadSources { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDiamante_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }

            [Display(Name = "Resort(s)")]
            public long?[] SearchDiamante_Resorts { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDiamante_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Assigned to User")]
            public Guid?[] SearchDiamante_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDiamante_DrpAssignedToUsers
            {
                get
                {
                    //return UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
                    return UserDataModel.GetUsersBySupervisor(null, true);
                }
            }

        }

        public class DiamanteReportModel
        {
            public string DiamanteReport_AssignedToUser { get; set; }
            public string DiamanteReport_TotalLeads { get; set; }
            public string DiamanteReport_NumberOpportunities { get; set; }
            public string DiamanteReport_NumberToursBooked { get; set; }
            public string DiamanteReport_BookingPercentage { get; set; }
            public string DiamanteReport_NumberToursShowed { get; set; }
            public string DiamanteReport_ToursPercentage { get; set; }
            public string DiamanteReport_OptionsVolume { get; set; }
            public string DiamanteReport_TimeshareVolume { get; set; }
            //public string DiamanteReport_MissedTours { get; set; }
            //public string DiamanteReport_MissedSales { get; set; }
            //public string DiamanteReport_MissedSalesVolume { get; set; }
            public string DiamanteReport_Qs { get; set; }
            public string DiamanteReport_QualifiedTours { get; set; }
            public string DiamanteReport_CapturePercentage { get; set; }
            public string DiamanteReport_QFactor { get; set; }
        }
    }

    public class aDynamicModel
    {
        public class aSearchDynamicModel : GenericSearchFieldsReportModel
        {
            public string SearchDynamic_DisplayColumns { get; set; }

            [Display(Name = "First Name")]
            public string SearchDynamic_FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string SearchDynamic_LastName { get; set; }

            [Display(Name = "Booking Status")]
            public string[] SearchDynamic_BookingStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpBookingStatus
            {
                get
                {
                    var list = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Lead Status")]
            public string[] SearchDynamic_LeadStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpLeadStatus
            {
                get
                {
                    var list = LeadStatusDataModel.GetAllLeadStatus();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Assigned To User")]
            public string[] SearchDynamic_AssignedToUser { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpAssignedToUsers
            {
                get
                {
                    //var list = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey);
                    //list.Insert(0, Utils.ListItems.Default());
                    return UserDataModel.GetUsersBySupervisor();
                }
            }

            [Display(Name = "Member Number")]
            public string SearchDynamic_MemberNumber { get; set; }

            [Display(Name = "Input Date")]
            public string SearchDynamic_I_InputDateTime { get; set; }

            public string SearchDynamic_F_InputDateTime { get; set; }

            //this property is string in Sugar
            [Display(Name = "Input By User")]
            public string[] SearchDynamic_InputByUser { get; set; }
            [ScriptIgnore]
            //pending check which users will show this list
            public List<SelectListItem> SearchDynamic_DrpInputByUsers
            {
                get
                {
                    //var list = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey);
                    //var onlyCurrentUser = false;
                    //if (list.Count() == 0)
                    //    onlyCurrentUser = true;
                    //if (!onlyCurrentUser)
                    //    list.Sort((x, y) => x.Text.CompareTo(y.Text));
                    //list.Insert(0, Utils.ListItems.Default());
                    return UserDataModel.GetUsersBySupervisor();
                }
            }

            //pending check how to include call1, call2, call3

            //this is tourDate in Sugar
            [Display(Name = "Presentation Date")]
            public string SearchDynamic_I_PresentationDate { get; set; }

            public string SearchDynamic_F_PresentationDate { get; set; }

            [Display(Name = "Tour Status")]
            public string[] SearchDynamic_TourStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpTourStatus
            {
                get
                {
                    var list = TourStatusDataModel.GetAlltourStatus();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Sales Volume")]
            public string SearchDynamic_SalesVolume { get; set; }

            //this is not a range and is tourTime in Sugar
            [Display(Name = "Presentation Time")]
            public string SearchDynamic_I_PresentationTime { get; set; }

            public string SearchDynamic_F_PresentationTime { get; set; }

            //this is confirmationLetter in Sugar
            [Display(Name = "Confirmation Letter On Arrival")]
            public string SearchDynamic_ConfirmationLetterOnArrival { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpConfirmationLetterOnArrival
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Is VIP")]
            public string SearchDynamic_IsVIP { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpIsVIP
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Modification Date")]
            public string SearchDynamic_I_ModificationDate { get; set; }

            public string SerchDynamic_F_ModifcationDate { get; set; }

            //pending check why AssignedTo is duplicated in Sugar but with different input options

            [Display(Name = "Real Tour Date")]
            public string SearchDynamic_I_RealTourDate { get; set; }

            public string SearchDynamic_F_RealTourDate { get; set; }

            [Display(Name = "Greeting Rep")]
            public string[] SearchDynamic_GreetingRep { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearhcDynamic_DrpGreetingReps
            {
                get
                {
                    var list = GreetingRepDataModel.GetAllGreetingReps();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "OPC")]
            public string[] SearchDynamic_OPC { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpOPCS
            {
                get
                {
                    var list = OpcDataModel.GetAllOPCs();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            //pending check if statusDescription will be added

            [Display(Name = "Reservation Status")]
            public string[] SearchDynamic_ReservationStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpReservationStatus
            {
                get
                {
                    var list = ReservationStatusDataModel.GetallReservationStatus();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Is All Inclusive")]
            public string SearchDynamic_IsAllInclusive { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpIsAllInclusive
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Need Transportation")]
            public string SearchDynamic_NeedTransportation { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpNeedTransportation
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Has Options")]
            public string SearchDynamic_HasOptions { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpHasOptions
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Is Referral")]
            public string SearchDynamic_IsReferral { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpIsReferral
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Time Zone")]
            public string[] SearchDynamic_TimeZone { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpTimeZones
            {
                get
                {
                    var list = TimeZoneDataModel.GetAllTimeZones();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }

            [Display(Name = "Submission Form")]
            public string SearchDynamic_SubmissionForm { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchDynamic_DrpSubmissionForm
            {
                get
                {
                    var list = Utils.ListItems.Booleans();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }
        }
    }

    public class DynamicModel
    {
        public class DynamicResults
        {
            public List<List<KeyValuePair<string, string>>> ListDynamicResults { get; set; }

            public string SearchTotalRows { get; set; }

            public object testValue1 { get; set; }

            public object testValue2 { get; set; }
        }

        public class SearchDynamicModel : GenericSearchFieldsReportModel
        {
            public string SearchDynamic_Columns { get; set; }

            [Display(Name = "Search Filters")]
            public string SearchDynamic_SearchFilters { get; set; }

            //--LayoutSaving
            public int SearchDynamic_ReportLayoutID { get; set; }

            [Display(Name = "Layout")]
            public string SearchDynamic_ReportLayout { get; set; }

            public string SearchDynamic_Fields { get; set; }

            public Guid SearchDynamic_OwnerUserID { get; set; }

            [Display(Name = "Is Public")]
            [Required(ErrorMessage = "Is Public is required")]
            public bool SearchDynamic_Public { get; set; }

            public string SearchDynamic_SysComponentID { get; set; }

            public string SearchDynamic_LayoutType { get; set; }//propose
            //--

            [Display(Name = "Columns to Display")]
            public string SearchDynamic_ColumnHeaders { get; set; }

            [Display(Name = "Greeting Rep")]
            [FieldInfo(Name = "greetingRepID")]
            [FieldToRequest(Name = "greetingRep")]
            [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblGreetingReps", PrimaryKeyModelName = "greetingRepID")]
            public int[] SearchDynamic_GreetingRep { get; set; }

            [Display(Name = "OPC")]
            [FieldInfo(Name = "opcID")]
            [FieldToRequest(Name = "opc")]
            [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblOPCS", PrimaryKeyModelName = "opcID")]
            public int[] SearchDynamic_OPC { get; set; }

            [Display(Name = "First Name")]
            [FieldInfo(Name = "firstName")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_FirstName { get; set; }

            [Display(Name = "Last Name")]
            [FieldInfo(Name = "lastName")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_LastName { get; set; }

            [Display(Name = "Arrival Date")]
            [FieldInfo(Name = "arrivalDate")]
            [DataBaseInfo(Name = "tblReservations")]
            public string SearchDynamic_I_ArrivalDate { get; set; }
            public string SearchDynamic_F_ArrivalDate { get; set; }

            [Display(Name = "Booking Status")]
            [FieldInfo(Name = "bookingStatusID")]
            [FieldToRequest(Name = "bookingStatus")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "tblBookingStatus", PrimaryKeyModelName = "bookingStatusID")]
            public int[] SearchDynamic_BookingStatus { get; set; }

            [Display(Name = "Resort")]
            [FieldInfo(Name = "placeID")]
            [FieldToRequest(Name = "place")]
            [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblPlaces", PrimaryKeyModelName = "placeID")]
            public int[] SearchDynamic_Place { get; set; }

            [Display(Name = "Lead Source")]
            [FieldInfo(Name = "leadSourceID")]
            [FieldToRequest(Name = "leadSource")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "tblLeadSources", PrimaryKeyModelName = "leadSourceID")]
            public int[] SearchDynamic_LeadSource { get; set; }

            [Display(Name = "Cancelation Date")]
            [FieldInfo(Name = "dateCancelation")]
            [DataBaseInfo(Name = "tblReservations")]
            public string SearchDynamic_I_CancelationDate { get; set; }
            public string SearchDynamic_F_CancelationDate { get; set; }

            [Display(Name = "Confirmation Date")]
            [FieldInfo(Name = "dateConfirmation")]
            [DataBaseInfo(Name = "tblReservations")]
            public string SearchDynamic_I_ConfirmationDate { get; set; }
            public string SearchDynamic_F_ConfirmationDate { get; set; }

            [FieldInfo(Name = "destinationID")]
            [FieldToRequest(Name = "destination")]
            [Display(Name = "Presentation Destination")]
            [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblDestinations", PrimaryKeyModelName = "destinationID")]
            public int[] SearchDynamic_Destination { get; set; }

            [Display(Name = "Expiration Date")]
            [FieldInfo(Name = "dateExpiration")]
            [DataBaseInfo(Name = "tblReservations")]
            public string SearchDynamic_I_ExpirationDate { get; set; }
            public string SearchDynamic_F_ExpirationDate { get; set; }

            [DataBaseInfo(Name = "tblReservations")]
            [Display(Name = "Hotel Confirmation Number")]
            [FieldInfo(Name = "hotelConfirmationNumber")]
            public string SearchDynamic_HotelConfirmationNumber { get; set; }

            [Display(Name = "Phone")]
            [FieldInfo(Name = "phone")]
            [DataBaseInfo(Name = "tblPhones")]
            public string SearchDynamic_Phone { get; set; }

            [Display(Name = "Presentation Date")]
            [FieldInfo(Name = "datePresentation")]
            [DataBaseInfo(Name = "tblPresentations")]
            public string SearchDynamic_I_PresentationDate { get; set; }
            public string SearchDynamic_F_PresentationDate { get; set; }

            [Display(Name = "Reservation Status")]
            [FieldInfo(Name = "reservationStatusID")]
            [FieldToRequest(Name = "reservationStatus")]
            [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblReservationStatus", PrimaryKeyModelName = "reservationStatusID")]
            public int[] SearchDynamic_ReservationStatus { get; set; }

            [Display(Name = "Sales Date")]
            [FieldInfo(Name = "salesDate")]
            [DataBaseInfo(Name = "tblContractsHistory")]
            public string SearchDynamic_I_SalesDate { get; set; }
            public string SearchDynamic_F_SalesDate { get; set; }

            [DataBaseInfo(Name = "tblLeads")]
            [FieldInfo(Name = "spouseLastName")]
            [Display(Name = "Spouse Last Name")]
            public string[] SearchDynamic_SpouseLastName { get; set; }

            [Display(Name = "Terminal")]
            [FieldInfo(Name = "terminalID")]
            [FieldToRequest(Name = "terminal")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "tblTerminals", PrimaryKeyModelName = "terminalID")]
            public long[] SearchDynamic_Terminal { get; set; }

            [Display(Name = "Input By User")]
            [FieldInfo(Name = "inputByUserID")]
            [FieldToRequest(Name = "UserName")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "aspnet_Users", PrimaryKeyModelName = "UserId")]
            public Guid[] SearchDynamic_InputByUser { get; set; }

            [Display(Name = "Assigned To User")]
            [FieldToRequest(Name = "UserName")]
            [FieldInfo(Name = "assignedToUserID")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "aspnet_Users", PrimaryKeyModelName = "UserId")]
            public Guid[] SearchDynamic_AssignedToUser { get; set; }

            [Display(Name = "Certificate Number")]
            [FieldInfo(Name = "certificateNumber")]
            [DataBaseInfo(Name = "tblReservations")]
            public string SearchDynamic_CertificateNumber { get; set; }

            [Display(Name = "Departure Date")]
            [FieldInfo(Name = "departureDate")]
            [DataBaseInfo(Name = "tblReservations")]
            public string SearchDynamic_I_DepartureDate { get; set; }
            public string SearchDynamic_F_DepartureDate { get; set; }

            [Display(Name = "Email")]
            [FieldInfo(Name = "email")]
            [DataBaseInfo(Name = "tblLeadEmails")]
            public string SearchDynamic_Email { get; set; }

            [Display(Name = "Plan Type")]
            [FieldInfo(Name = "planTypeID")]
            [FieldToRequest(Name = "planType")]
            [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblPlanTypes", PrimaryKeyModelName = "planTypeID")]
            public int[] SearchDynamic_PlanType { get; set; }

            [Display(Name = "Qualification Status")]
            [FieldInfo(Name = "qualificationStatusID")]
            [FieldToRequest(Name = "qualificationStatus")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "tblQualificationStatus", PrimaryKeyModelName = "qualificationStatusID")]
            public int[] SearchDynamic_QualificationStatus { get; set; }

            [FieldToRequest(Name = "UserName")]
            [Display(Name = "Sales Agent User")]
            [FieldInfo(Name = "salesAgentUserID")]
            [DataBaseInfo(Name = "tblReservations", IsRelationShip = true, PrimaryKeyDatabaseName = "aspnet_Users", PrimaryKeyModelName = "UserId")]
            public Guid[] SearchDynamic_SalesAgentUser { get; set; }

            [DataBaseInfo(Name = "tblLeads")]
            [FieldInfo(Name = "spouseFirstName")]
            [Display(Name = "Spouse First Name")]
            public string SearchDynamic_SpouseFirstName { get; set; }

            [Display(Name = "Hostess Comments")]
            [FieldInfo(Name = "hostessComments")]
            [DataBaseInfo(Name = "tblPresentations")]
            public string SearchDynamic_HostessComments { get; set; }

            [Display(Name = "Member Number")]
            [FieldInfo(Name = "memberNumber")]
            [DataBaseInfo(Name = "tblMemberInfo")]
            public string SearchDynamic_MemberNumber { get; set; }

            [Display(Name = "Tour Status")]
            [FieldInfo(Name = "tourStatusID")]
            [FieldToRequest(Name = "tourStatus")]
            [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblTourStatus", PrimaryKeyModelName = "tourStatusID")]
            public string SearchDynamic_TourStatus { get; set; }

            [Display(Name = "Final Tour Status")]
            [FieldInfo(Name = "finalTourStatusID")]
            [FieldToRequest(Name = "tourStatus")]
            [DataBaseInfo(Name = "tblPresentations", IsRelationShip = true, PrimaryKeyDatabaseName = "tblTourStatus", PrimaryKeyModelName = "tourStatusID")]
            public string SearchDynamic_FinalTourStatus { get; set; }

            [Display(Name = "Real Tour Date")]
            [FieldInfo(Name = "realTourDate")]
            [DataBaseInfo(Name = "tblPresentations")]
            public string SearchDynamic_RealTourDate { get; set; }

            [Display(Name = "Sales Volume")]
            [FieldInfo(Name = "salesVolume")]
            [DataBaseInfo(Name = "tblContractsHistory")]
            public string SearchDynamic_SalesVolume { get; set; }

            [Display(Name = "Is Referral")]
            [FieldInfo(Name = "referredByID")]
            [DataBaseInfo(Name = "tblLeads")]
            public bool? SearchDynamic_IsReferral { get; set; }

            [Display(Name = "Submission Form")]
            [FieldInfo(Name = "submissionForm")]
            [DataBaseInfo(Name = "tblLeads")]
            public bool? SearchDynamic_SubmissionForm { get; set; }

            [Display(Name = "Is All Inclusive")]
            [FieldInfo(Name = "isAllInclusive")]
            [DataBaseInfo(Name = "tblMemberInfo")]
            public bool? SearchDynamic_IsAllInclusive { get; set; }

            [Display(Name = "Has Options")]
            [FieldInfo(Name = "hasOptions")]
            [DataBaseInfo(Name = "tblMemberInfo")]
            public bool? SearchDynamic_HasOptions { get; set; }

            [Display(Name = "Address")]
            [FieldInfo(Name = "address")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_Address { get; set; }

            [Display(Name = "City")]
            [FieldInfo(Name = "city")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_City { get; set; }

            [Display(Name = "State")]
            [FieldInfo(Name = "state")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_State { get; set; }

            [Display(Name = "Postal Code")]
            [FieldInfo(Name = "zipcode")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_ZipCode { get; set; }

            [Display(Name = "Country")]
            [FieldInfo(Name = "countryID")]
            [FieldToRequest(Name = "country")]
            [DataBaseInfo(Name = "tblLeads", IsRelationShip = true, PrimaryKeyDatabaseName = "tblCountries", PrimaryKeyModelName = "countryID")]
            public string SearchDynamic_Country { get; set; }

            [Display(Name = "Notes")]
            [FieldInfo(Name = "leadComments")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_LeadComments { get; set; }

            [Display(Name = "Creation Date")]
            [FieldInfo(Name = "inputDateTime")]
            [DataBaseInfo(Name = "tblLeads")]
            public string SearchDynamic_I_CreationDate { get; set; }
            public string SearchDynamic_F_CreationDate { get; set; }

            public SearchDynamicModel()
            {
                SearchDynamic_IsReferral = null;
                SearchDynamic_SubmissionForm = null;
                SearchDynamic_IsAllInclusive = null;
                SearchDynamic_HasOptions = null;
            }
        }
    }

    public class RentersModel
    {
        public class SearchRenters
        {
            [Display(Name = "Arrival Date")]
            public string SearchRenter_I_ArrivalDate { get; set; }
            public string SearchRenter_F_ArrivalDate { get; set; }

            [Display(Name = "Resort(s)")]
            public long?[] SearchRenter_Resorts { get; set; }

            [Display(Name = "Lead Source(s)")]
            public long?[] SearchRenter_LeadSources { get; set; }

            [Display(Name = "Booking Status")]
            public int?[] SearchRenter_BookingStatus { get; set; }

            public List<SelectListItem> SearchRenter_DrpResorts
            {
                get
                {
                    return PlaceDataModel.GetResortsByProfile();
                }
            }

            public List<SelectListItem> SearchRenter_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }

            public List<SelectListItem> SearchRenter_DrpBookingStatus
            {
                get
                {
                    return BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                }
            }
        }

        public class RentersResults
        {
            public string AssignedToUser { get; set; }
            public string LeadsContacted { get; set; }
            public string LeadsBooked { get; set; }
            public string BookingPercentage { get; set; }
            public string TotalPaid { get; set; }
        }

    }

    public class GenericSearchFieldsReportModel
    {
        //[Display(Name = "Arrival Date")]
        //public string GenericSearchFieldsReport_ArrivalDate { get; set; }

        [Display(Name = "Arrival Date")]
        public string GenericSearchFieldsReport_I_ArrivalDate { get; set; }

        public string GenericSearchFieldsReport_F_ArrivalDate { get; set; }

        [Display(Name = "Booking Status")]
        public string[] GenericSearchFieldsReport_BookingStatus { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> GenericSearchFieldsReport_DrpBookingStatus
        {
            get
            {
                //List<SelectListItem> TheList = ReportDataModel.ReportsCatalogs.FillDrpBookingStatusByWorkGroup();
                List<SelectListItem> TheList = BookingStatusDataModel.GetSecondaryBookingStatus();
                return TheList;
            }
        }

        [Display(Name = "Lead Source")]
        public string[] GenericSearchFieldsReport_LeadSource { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> GenericSearchFieldsReport_DrpLeadSources
        {
            get
            {
                //return LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                return LeadSourceDataModel.GetLeadSourcesByTerminal();
            }
        }

        [Display(Name = "Lead Status")]
        public string[] GenericSearchFieldsReport_LeadStatus { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> GenericSearchFieldsReport_DrpLeadStatus
        {
            get
            {
                return LeadStatusDataModel.GetAllLeadStatus();
            }
        }

        [Display(Name = "Resort")]
        public string[] GenericSearchFieldsReport_Resort { get; set; }
        [ScriptIgnore]
        public List<SelectListItem> GenericSearchFieldsReport_DrpResorts
        {
            get
            {
                List<SelectListItem> TheList = ReportDataModel.ReportsCatalogs.FillDrpResorts();
                //TheList.Insert(0, Utils.ListItems.Default());
                return TheList;
                //return ReportDataModel.ReportsCatalogs.FillDrpResorts();
            }
        }
    }

    public class RoomUpgradesModel
    {
        public List<RoomUpgradeReportModel> ListRoomUpgradesReportModel { get; set; }

        public class SearchRoomUpgradesModel
        {
            [Display(Name = "Resort")]
            public long[] SearchRoomUpgrades_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchRoomUpgrades_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Arrival Date")]
            public string SearchRoomUpgrades_I_ArrivalDate { get; set; }
            public string SearchRoomUpgrades_F_ArrivalDate { get; set; }
        }
    }

    public class RoomUpgradeReportModel
    {
        public string RoomUpgradeReport_Name { get; set; }
        public string RoomUpgradeReport_Resort { get; set; }
        public string RoomUpgradeReport_ArrivalDate { get; set; }
        public string RoomUpgradeReport_AssignedToUser { get; set; }
        public string RoomUpgradeReport_BookingStatus { get; set; }
        public string RoomUpgradeReport_RealTourDate { get; set; }
        public string RoomUpgradeReport_FinalTourStatus { get; set; }
        public string RoomUpgradeReport_SalesVolume { get; set; }
    }

    public class PreCheckInModel
    {
        public List<PreCheckInReportModel> ListPreCheckInModel { get; set; }

        public class SearchPreCheckInModel
        {
            [Display(Name = "Arrival Date")]
            public string SearchPreCheckIn_I_ArrivalDate { get; set; }
            public string SearchPreCheckIn_F_ArrivalDate { get; set; }

            [Display(Name = "Resort")]
            public long?[] SearchPreCheckIn_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPreCheckIn_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Lead Source")]
            public string[] SearchPreCheckIn_LeadSource { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPreCheckIn_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByWorkGroup();
                }
            }

            [Display(Name = "User")]
            public string[] SearchPreCheckIn_User { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchPreCheckIn_DrpUsers
            {
                get
                {
                    //var list = UserDataModel.GetUsersBySupervisor((Guid)Membership.GetUser().ProviderUserKey, true);
                    return UserDataModel.GetUsersBySupervisor(null, true);
                }
            }
        }
    }

    public class PreCheckInReportModel
    {
        public string PreCheckInReport_FirstName { get; set; }
        public string PreCheckInReport_LastName { get; set; }
        public string PreCheckInReport_Email { get; set; }
        public string PreCheckInReport_Phone { get; set; }
        public string PreCheckInReport_BookingStatus { get; set; }
        public string PreCheckInReport_LeadSource { get; set; }
        public string PreCheckInReport_Resort { get; set; }
        public string PreCheckInReport_ArrivalDate { get; set; }
    }

    public class ConciergesModel
    {
        public List<ConciergeReportModel> ListConciergesReportModel { get; set; }

        public class SearchConciergesModel
        {
            [Display(Name = "Arrival Date")]
            public string SearchConcierges_I_ArrivalDate { get; set; }
            public string SearchConcierges_F_ArrivalDate { get; set; }

            [Display(Name = "Resort")]
            public long?[] SearchConcierges_Resort { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchConcierges_DrpResorts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpResorts();
                }
            }

            [Display(Name = "Booking Status")]
            public int?[] SearchConcierges_BookingStatus { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchConcierges_DrpBookingStatus
            {
                get
                {
                    //return BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                    return BookingStatusDataModel.GetSecondaryBookingStatus();
                }
            }

            [Display(Name = "Lead Source")]
            public long?[] SearchConcierges_LeadSources { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchConcierges_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }
        }
    }

    public class ConciergeReportModel
    {
        public string ConciergeReport_ContractNumber { get; set; }
        public string ConciergeReport_LeadSource { get; set; }
        public string ConciergeReport_ClubType { get; set; }
        public string ConciergeReport_CoOwner { get; set; }
        public string ConciergeReport_NumberAdults { get; set; }
        public string ConciergeReport_NumberChildren { get; set; }
        public string ConciergeReport_FlightInfo { get; set; }
        public string ConciergeReport_Name { get; set; }
        public string ConciergeReport_ArrivalDate { get; set; }
        public string ConciergeReport_AssignedToUser { get; set; }
        public string ConciergeReport_BookingStatus { get; set; }
        public string ConciergeReport_Resort { get; set; }
        public string ConciergeReport_TotalPaid { get; set; }
        public string ConciergeReport_ConciergeComments { get; set; }
    }

    public class ArrivalsReportModel
    {
        public string ArrivalsReport_FirstName { get; set; }
        public string ArrivalsReport_LastName { get; set; }
        public string ArrivalsReport_ClubType { get; set; }
        public string ArrivalsReport_ArrivalDate { get; set; }
        public string ArrivalsReport_Resort { get; set; }
        public string ArrivalsReport_BookingStatus { get; set; }
        public string ArrivalsReport_NumberAdults { get; set; }
        public string ArrivalsReport_NumberChildren { get; set; }
        public string ArrivalsReport_FlightInformation { get; set; }
        public string ArrivalsReport_LeadSource { get; set; }
        public string ArrivalsReport_AccountNumber { get; set; }
        public string ArrivalsReport_ContractNumber { get; set; }
        public string ArrivalsReport_CoOwner { get; set; }
        public string ArrivalsReport_AssignedToUser { get; set; }
    }

    public class DuplicateLeadsReportModel
    {
        public Guid DuplicateLeadsReport_LeadID { get; set; }
        public string DuplicateLeadsReport_Lead { get; set; }
        public string DuplicateLeadsReport_Name { get; set; }
        public string DuplicateLeadsReport_BookingStatus { get; set; }
        public string DuplicateLeadsReport_LeadStatus { get; set; }
        public string DuplicateLeadsReport_LeadSource { get; set; }
        public DateTime DuplicateLeadsReport_ArrivalDate { get; set; }
        public string DuplicateLeadsReport_AssignedToUser { get; set; }
        public string DuplicateLeadsReport_Resort { get; set; }
        public string DuplicateLeadsReport_ConfirmationNumber { get; set; }
        public string DuplicateLeadsReport_PhoneNumber { get; set; }
        public string DuplicateLeadsReport_Email { get; set; }
        public string DuplicateLeadsReport_MemberAccount { get; set; }
    }

    public class IndicatorsReportModel
    {
        public string IndicatorsReport_FirstName { get; set; }
        public string IndicatorsReport_LastName { get; set; }
        public string IndicatorsReport_Arrivals { get; set; }
        public string IndicatorsReport_Bookings { get; set; }
        public string IndicatorsReport_BookingsPercentage { get; set; }
        public string IndicatorsReport_Tours { get; set; }
        public string IndicatorsReport_ToursPercentage { get; set; }
        public string IndicatorsReport_CapturePercentage { get; set; }
        public string IndicatorsReport_Qualified { get; set; }
        public string IndicatorsReport_QFactor { get; set; }
        public string IndicatorsReport_Volume { get; set; }
        public string IndicatorsReport_VPG { get; set; }
        public string IndicatorsReport_MissedSales { get; set; }
        public string IndicatorsReport_MissedSalesVolume { get; set; }
        public string IndicatorsReport_MVPG { get; set; }
        public string IndicatorsReport_TotalPaidPercentage { get; set; }
        public string IndicatorsReport_TotalPaidVolume { get; set; }
        public string IndicatorsReport_TotalPaidPerArrival { get; set; }
        public string IndicatorsReport_Resort { get; set; }
    }

    public class PipelineReportModel
    {
        public string PipelineReport_AgentName { get; set; }
        public string PipelineReport_CombinedLeads { get; set; }
        public string PipelineReport_NewLeads { get; set; }
        public string PipelineReport_NewLeadsPercentage { get; set; }
        public string PipelineReport_InProcessLeads { get; set; }
        public string PipelineReport_AssignedLeads { get; set; }
        public string PipelineReport_PushedLeads { get; set; }
        public string PipelineReport_CancelledLeads { get; set; }
        public string PipelineReport_DuplicateLeads { get; set; }
        public string PipelineReport_BankedLeads { get; set; }
        public string PipelineReport_InternationalLeads { get; set; }
        public string PipelineReport_DeadLeads { get; set; }
        public string PipelineReport_CallClasification { get; set; }
        public string PipelineReport_LastMinuteLeads { get; set; }
    }

    public class ExchangeTourReportModel
    {
        public string ExchangeTourReport_FirstName { get; set; }
        public string ExchangeTourReport_LastName { get; set; }
        public string ExchangeTourReport_UserName { get; set; }
        public string ExchangeTourReport_LeadSource { get; set; }
        public string ExchangeTourReport_ArrivalDate { get; set; }
        public string ExchangeTourReport_BookingStatus { get; set; }
        public string ExchangeTourReport_TotalPaid { get; set; }
        public string ExchangeTourReport_Resort { get; set; }
        public string ExchangeTourReport_FinalTourDate { get; set; }
        public string ExchangeTourReport_FinalTourStatus { get; set; }
        public string ExchangeTourReport_FinalBookingStatus { get; set; }
        public string ExchangeTourReport_HostessComments { get; set; }
        public string ExchangeTourReport_SalesVolume { get; set; }
    }

    public class PreBookedArrivalsReportModel
    {
        public string PreBookedArrivalsReport_FirstName { get; set; }
        public string PreBookedArrivalsReport_LastName { get; set; }
        public string PreBookedArrivalsReport_ArrivalDate { get; set; }
        public string PreBookedArrivalsReport_HotelConfirmationNumber { get; set; }
        public string PreBookedArrivalsReport_FrontOfficeCertificateNumber { get; set; }
        public string PreBookedArrivalsReport_PresentationDateTime { get; set; }
        public string PreBookedArrivalsReport_Resort { get; set; }
        public string PreBookedArrivalsReport_BookingStatus { get; set; }
        public string PreBookedArrivalsReport_PreBookedByUser { get; set; }
        public string PreBookedArrivalsReport_BookingStatusDate { get; set; }
        public string PreBookedArrivalsReport_LeadSource { get; set; }
        public string PreBookedArrivalsReport_UserName { get; set; }
        public string PreBookedArrivalsReport_Status { get; set; }
        public string PreBookedArrivalsReport_TotalPaid { get; set; }
        public string PreBookedArrivalsReport_SalesVolume { get; set; }
    }

    public class NewReferralsReportModel
    {
        public string NewReferralsReport_User { get; set; }
        public string NewReferralsReport_LeadSource { get; set; }
        public string NewReferralsReport_ReferredFirstName { get; set; }
        public string NewReferralsReport_ReferredLastName { get; set; }
        public string NewReferralsReport_ReferredEmail { get; set; }
        public string NewReferralsReport_ReferredAddress { get; set; }
        public string NewReferralsReport_ReferredCity { get; set; }
        public string NewReferralsReport_ReferredState { get; set; }
        public string NewReferralsReport_ReferredZipCode { get; set; }
        public string NewReferralsReport_ReferredCountry { get; set; }
        public string NewReferralsReport_ReferredDateCreated { get; set; }
        public string NewReferralsReport_ReferredHomePhone { get; set; }
        public string NewReferralsReport_ReferredByLead { get; set; }
        public string NewReferralsReport_ReferredComments { get; set; }
        public string NewReferralsReport_ReferredWorkPhone { get; set; }
        public string NewReferralsReport_ReferredMobilePhone { get; set; }
    }

    //public class NewReferralLeadsReportModel
    //{
    //    public string NewReferralLeadsReport_AgentID { get; set; }
    //    public string NewReferralLeadsReport_InputDate { get; set; }
    //    public string NewReferralLeadsReport_Agent { get; set; }
    //    public string NewReferralLeadsReport_ReferralsNumber { get; set; }
    //    public string NewReferralLeadsReport_Referrals { get; set; }
    //    //public List<GenericStringModel> NewReferralLeadsReport_Referrals { get; set; }
    //}

    public class IncorrectlyPreBookedToursModel
    {
        public List<IncorrectlyPreBookedToursReportModel> ListPreBookedTours { get; set; }

        public class SearchIncorrectPreBookedTours
        {
            [Display(Name = "Arrival Date")]
            public string Search_I_ArrivalDate { get; set; }
            public string Search_F_ArrivalDate { get; set; }

            [Display(Name = "Resort")]
            public long?[] Search_Resorts { get; set; }

            [ScriptIgnore]
            public List<SelectListItem> Search_DrpResorts
            {
                get
                {
                    return PlaceDataModel.GetResortsByProfile();
                }
            }

            [Display(Name = "Lead Source")]
            public int[] Search_LeadSources { get; set; }

            [ScriptIgnore]
            public List<SelectListItem> Search_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }


            [Display(Name = "Agent")]
            public Guid?[] Search_Agents { get; set; }

            [ScriptIgnore]
            public List<SelectListItem> Search_DrpUsers
            {
                get
                {
                    //var user = (Guid)Membership.GetUser().ProviderUserKey;
                    return UserDataModel.GetUsersBySupervisor(null, true);
                }
            }
        }
    }

    public class PreArrivalReport
    {
        public List<PreArrivalReportModel> ListManifest { get; set; }
        public class SearchPreArrivalReport
        {
            [Display(Name = "Purchase Date")]
            public string Search_I_PurchaseDate { get; set; }
            public string Search_F_PurchaseDate { get; set; }

            [Display(Name = "Arrival Date")]
            public string Search_I_ArrivalDate { get; set; }
            public string Search_F_ArrivalDate { get; set; }

            [Display(Name = "Date Type")]
            public int Search_DateType { get; set; }
            public List<SelectListItem> Search_DrpDateTypes
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem() { Value = "1", Text = "Arrival Date" });
                    list.Add(new SelectListItem() { Value = "2", Text = "Departure Date" });
                    list.Add(new SelectListItem() { Value = "3", Text = "Purchase Date" });
                    return list;
                }
            }

            [Display(Name = "Real Tour Date")]
            public string Search_I_RealTourDate { get; set; }
            public string Search_F_RealTourDate { get; set; }

            [Display(Name = "Resort")]
            public long?[] Search_Resorts { get; set; }

            [Display(Name = "Assigned To User")]
            public Guid?[] Search_AssignedToUsers { get; set; }

            [Display(Name = "Lead Source")]
            public long?[] Search_LeadSources { get; set; }

            [Display(Name = "Destination")]
            public long?[] Search_Destination { get; set; }

            [Display(Name = "Merchant Account")]
            public int?[] Search_MerchantAccount { get; set; }

            [Display(Name = "Hotel Confirmation Number / CRS")]
            public string Search_HotelConfirmationNumber { get; set; }

            [Display(Name = "Sort By Resort")]
            public bool Search_SortByResort { get; set; }

            public List<SelectListItem> Search_DrpDestinations
            {
                get
                {
                    return PlaceDataModel.GetDestinationsByCurrentTerminals();
                }
            }

            public List<SelectListItem> Search_DrpLeadSources
            {
                get
                {
                    return LeadSourceDataModel.GetLeadSourcesByTerminal();
                }
            }

            public List<SelectListItem> Search_DrpUsers
            {
                get
                {
                    return UserDataModel.GetUsersBySupervisor(null, true);
                }
            }

            public List<SelectListItem> Search_DrpResorts
            {
                get
                {
                    return PlaceDataModel.GetResortsByProfile();
                }
            }

            public List<SelectListItem> Search_DrpResortsByUser
            {
                get
                {
                    return PlaceDataModel.GetResortsByUser();
                }
            }

            public List<SelectListItem> Search_MerchantAccounts
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpMerchantAccounts();
                }
            }
        }
    }

    public class ProductionModel
    {
        public class ServiceIncomeItem
        {
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public int NumberOfCoupons { get; set; }
            public List<TotalDetail> SalesPerPrice { get; set; }
            public List<Money> SalesTotal { get; set; }
        }

        public class TotalDetail
        {
            public int PriceTypeID { get; set; }
            public List<Money> Sales { get; set; }
            public List<CouponDetail> Coupons { get; set; }
        }

        public class ExternalSales
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string PointsOfSale { get; set; }
            public string Terminal { get; set; }
            public List<ExternalCouponModel> Coupons { get; set; }
            public int NumberOfPurchases { get; set; }
            public int NumberOfCoupons { get; set; }
        }

        public class MinimalPrice
        {
            public long? PromoID { get; set; }
            public int PriceTypeID { get; set; }
        }

        public class ExternalCouponModel
        {
            public string PointOfSale { get; set; }
            public string SalesAgent { get; set; }
            public string Folio { get; set; }
            public string Customer { get; set; }
            public string ConfirmationDate { get; set; }
            public string CancelationDate { get; set; }
            public string Service { get; set; }
            public decimal UnitsQty { get; set; }
            public string Units { get; set; }
            public string Status { get; set; }
            public List<Money> DealPrice { get; set; }
            public List<Money> MinimalPrice { get; set; }
            public List<Money> UnitDealPrice { get; set; }
            public List<Money> UnitMinimalPrice { get; set; }
            public List<Money> UnitDifference { get; set; }
        }

        public class CouponSales
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string PointsOfSale { get; set; }
            public string Terminal { get; set; }
            public int NumberOfPurchases { get; set; }
            public int NumberOfCoupons { get; set; }
            public List<CouponReportModel> Coupons { get; set; }
            public List<Money> GrandTotal { get; set; }
            public List<Money> GrandTotalNoIVA { get; set; }
            public List<Money> TotalCost { get; set; }
            public List<Money> TotalCostNoIVA { get; set; }
            public List<Money> TotalUtility { get; set; }
            public List<Money> TotalUtilityNoIVA { get; set; }

            public string[] Columns { get; set; }
            public List<Summary> Summaries { get; set; }
            public List<Summary2> Summaries2 { get; set; }
        }

        public class Summary
        {
            public int SummaryID { get; set; }
            public string Concept { get; set; }
            public decimal Quantity { get; set; }
            public string QuantityConcept { get; set; }
            public List<string> Folios { get; set; }
            public List<Money> GrandTotal { get; set; }
            public List<Money> GrandTotalNoIVA { get; set; }
            public List<Money> TotalCost { get; set; }
            public List<Money> TotalCostNoIVA { get; set; }
            public List<Money> TotalUtility { get; set; }
            public List<Money> TotalUtilityNoIVA { get; set; }
        }

        public class Summary2
        {
            public int SummaryID { get; set; }
            public List<LogGroup> GroupsList { get; set; }
            public List<PriceType> PriceTypes { get; set; }
            public List<TotalsByDay> SalesbyDay { get; set; }
            public List<Group> GrandTotalByGroups { get; set; }
            public List<Money> GrandTotal { get; set; }
            public List<TotalByPriceType> TotalType { get; set; }

            public class TotalsByDay// Primera Fila 
            {
                public DateTime Date { get; set; }
                public List<Group> Groups { get; set; }
                public List<Money> TotalByDay { get; set; }
                public List<TotalByPriceType> TotalDayPriceType { get; set; }

            }
            public class Group //Agente o Punto de venta relacionado con el tipo de precio y el total
            {
                public object GroupID { get; set; }
                public List<TotalByPriceType> PriceTypeSales { get; set; } //total por tipo de precio
                public List<Money> TotalGroup { get; set; }
            }

            public class TotalByPriceType// Tipo de Precio y el total por tipo de precio
            {
                public List<Money> TotalSales { get; set; }
                public int PriceTypeID { get; set; }
            }

            public class LogGroup//Nombre del Agente o Lugar
            {
                public object GroupID { get; set; }
                public string GroupName { get; set; }

            }
        }


        public class SearchProduction
        {
            [Display(Name = "Use Cache")]
            public bool Search_Cache { get; set; }

            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Providers")]
            public int[] Search_ProviderID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Providers
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "Service or Product")]
            public long[] Search_ServiceID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Services
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "Summary By")]
            public int[] Search_Summary { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_SummaryOptions
            {
                get
                {
                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem()
                    {
                        Text = "Point of Sale",
                        Value = "1"
                    });
                    options.Add(new SelectListItem()
                    {
                        Text = "Agent",
                        Value = "2"
                    });
                    options.Add(new SelectListItem()
                    {
                        Text = "Unit",
                        Value = "3"
                    });
                    options.Add(new SelectListItem()
                    {
                        Text = "Culture",
                        Value = "4"
                    });

                    options.Add(new SelectListItem()
                    {
                        Text = "Price Type by Point of Sale",
                        Value = "5"
                    });
                    options.Add(new SelectListItem()
                    {
                        Text = "Price Type by Agent",
                        Value = "6"
                    });
                    options.Add(new SelectListItem()
                    {
                        Text = "Location",
                        Value = "7"
                    });
                    return options;
                }
            }

            [Display(Name = "Columns")]
            public string[] Search_Columns { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_ColumnsOptions
            {
                get
                {
                    List<SelectListItem> options = new List<SelectListItem>();
                    options.Add(new SelectListItem() { Text = "Folio", Value = "Folio" });
                    options.Add(new SelectListItem() { Text = "Coupon Reference", Value = "CouponReference" });
                    options.Add(new SelectListItem() { Text = "Saved On", Value = "DateSaved" });
                    options.Add(new SelectListItem() { Text = "Confirmation Date", Value = "ConfirmationDate" });
                    options.Add(new SelectListItem() { Text = "Cancelation Date", Value = "CancelationDate" });
                    options.Add(new SelectListItem() { Text = "Sales Agent", Value = "SalesAgent" });
                    options.Add(new SelectListItem() { Text = "Reservations Agent", Value = "ReservationsAgent" });
                    options.Add(new SelectListItem() { Text = "Pax", Value = "UnitsQty" });
                    options.Add(new SelectListItem() { Text = "Point of Sale", Value = "PointOfSale" });
                    options.Add(new SelectListItem() { Text = "Category", Value = "Category" });
                    options.Add(new SelectListItem() { Text = "Provider", Value = "Provider" });
                    options.Add(new SelectListItem() { Text = "Service or Product", Value = "Service" });
                    options.Add(new SelectListItem() { Text = "Activity Date", Value = "ServiceDate" });
                    options.Add(new SelectListItem() { Text = "Units", Value = "Units" });
                    options.Add(new SelectListItem() { Text = "Avance Provider ID", Value = "ProviderAvanceID" });
                    options.Add(new SelectListItem() { Text = "Customer", Value = "Customer" });
                    options.Add(new SelectListItem() { Text = "Language", Value = "Culture" });
                    options.Add(new SelectListItem() { Text = "Price Type", Value = "PriceType" });
                    options.Add(new SelectListItem() { Text = "Total", Value = "Total" });
                    options.Add(new SelectListItem() { Text = "Total b/ IVA", Value = "TotalNoIVA" });
                    options.Add(new SelectListItem() { Text = "Cost", Value = "Cost" });
                    options.Add(new SelectListItem() { Text = "Cost b/ IVA", Value = "CostNoIVA" });
                    options.Add(new SelectListItem() { Text = "Utility", Value = "Utility" });
                    options.Add(new SelectListItem() { Text = "Utility b/ IVA", Value = "UtilityNoIVA" });
                    options.Add(new SelectListItem() { Text = "Status", Value = "Status" });
                    options.Add(new SelectListItem() { Text = "Promo", Value = "Promo" });
                    options.Add(new SelectListItem() { Text = "Transaction", Value = "AuthCode" });
                    options.Add(new SelectListItem() { Text = "Location", Value = "Location" });
                    return options;
                }
            }


            [Display(Name = "Categories")]
            public long[] Search_CategoryID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Categories
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCategories();
                }
            }

            [Display(Name = "Point of Sale")]
            public int[] Search_PointOfSaleID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(long.Parse(Search_Terminals.FirstOrDefault().Value));
                    return list;
                }
            }

            [Display(Name = "Accounting Account")]
            public int[] Search_AccountingAccountID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_AccountingAccounts
            {
                get
                {
                    return ActivityDataModel.ActivitiesCatalogs.FillDrpAccountingAccounts();
                }
            }

            [Display(Name = "Company")]
            public int? Search_CompanyID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Companies
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
        }
    }

    public class SweepstakesReportModel
    {
        [Display(Name = "Terminal")]
        public string Terminal { get; set; }

        [Display(Name = "Dates")]
        public string Dates { get; set; }
        public string Json { get; set; }
        public List<SweepstakeItem> Sweepstakes { get; set; }
        public class SweepstakeItem
        {
            public Guid TransactionID { get; set; }
            public string Date { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Terminal { get; set; }
        }
        public class SearchSweepstakes
        {
            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
        }
    }

    public class QuoteRequestsReportModel
    {
        [Display(Name = "Terminal")]
        public string Terminal { get; set; }

        [Display(Name = "Dates")]
        public string Dates { get; set; }

        public string Json { get; set; }

        public List<QuoteRequestItem> QuoteRequests { get; set; }

        public class QuoteRequestItem
        {
            public Guid TransactionID { get; set; }
            public string DateSaved { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Destination { get; set; }
            public string Resort { get; set; }
            public string Arrival { get; set; }
            public string Departure { get; set; }
            public string Adults { get; set; }
            public string Children { get; set; }
            public string Comments { get; set; }
            public string TimeToBeReached { get; set; }
            public string Terminal { get; set; }
        }

        public class SearchQuoteRequests
        {
            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
        }
    }

    public class IncomeOutcomeModel
    {
        [Display(Name = "Terminal")]
        public string Terminal { get; set; }
        [Display(Name = "Points of Sale")]
        public string PointsOfSale { get; set; }
        [Display(Name = "Currency")]
        public string Currency { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        public string Message { get; set; }

        public List<IncomeOutcomeCouponDetail> Details { get; set; }

        public List<IncomeOutcomeAccountTotal> Totals { get; set; }

        public List<BankCommissionProfit> Profits { get; set; }

        public Money IncomeSubtotal { get; set; }
        public Money IncomeIVA { get; set; }
        public Money IncomeTotal { get; set; }
        public Money OutcomeSubtotal { get; set; }
        public Money OutcomeIVA { get; set; }
        public Money OutcomeTotal { get; set; }
        public Money UtilityTotal { get; set; }

        public class IncomeOutcomeCouponDetail
        {
            public long PurchaseServiceID { get; set; }
            public Guid PurchaseID { get; set; }
            public string PointOfSale { get; set; }
            public string MarketingProgram { get; set; }
            public string Customer { get; set; }
            public string Folio { get; set; }
            public string CouponReference { get; set; }
            public string Date { get; set; }
            public string CancelationDate { get; set; }
            public string Service { get; set; }
            public string Status { get; set; }
            public string CloseOut { get; set; }
            public string CancelationCloseOut { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public string PriceType { get; set; }
            public Money UnitTotal { get; set; }

            public int? IncomeAccAccID { get; set; }
            public string IncomeAccAccNumber { get; set; }
            public string IncomeAccAccName { get; set; }
            public decimal IncomeExchangeRate { get; set; }
            public Money Total { get; set; }

            public int? OutcomeAccAccID { get; set; }
            public string OutcomeAccAccNumber { get; set; }
            public string OutcomeAccAccName { get; set; }
            public decimal OutcomeExchangeRate { get; set; }
            public Money Cost { get; set; }
            public bool CustomCost { get; set; }
            public string Audited { get; set; }
            public string InvoiceNumber { get; set; }
        }

        public class IncomeOutcomeAccountTotal
        {
            public string AccAccName { get; set; }
            public string PriceType { get; set; }

            public int? AccAccIncomeID { get; set; }
            public string AccAccIncomeNumber { get; set; }

            public Money IncomeSubtotal { get; set; }
            public Money IncomeIVA { get; set; }
            public Money IncomeTotal { get; set; }

            public int? AccAccOutcomeID { get; set; }
            public string AccAccOutcomeNumber { get; set; }

            public Money OutcomeSubtotal { get; set; }
            public Money OutcomeIVA { get; set; }
            public Money OutcomeTotal { get; set; }

            public Money Utility { get; set; }
        }

        public class SearchIncomeOutcomeModel
        {
            [Display(Name = "Use Cache")]
            public bool Search_Cache { get; set; }
            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [Display(Name = "Points of Sale")]
            public int[] Search_SelectedPointsOfSale { get; set; }
            [Display(Name = "Currency")]
            public int Search_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
            //[ScriptIgnore]
            public List<SelectListItem> PointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD().OrderBy(x => x.Text).ToList();
                }
            }
        }

        public class ExchangeRateItem
        {
            public long? ExchangeRateID { get; set; }
            public decimal ExchangeRate { get; set; }
            public string Vigency { get; set; }
            public string Type { get; set; }
        }

        public class CouponQuery
        {
            public long terminalID { get; set; }
            public string culture { get; set; }
            public int pointOfSaleID { get; set; }
            public bool online { get; set; }
            public bool paid { get; set; }
            public bool canceled { get; set; }
            public Guid purchaseID { get; set; }
            public string shortName { get; set; }
            public string pointOfSale { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public DateTime? confirmationDateTime { get; set; }
            public string service { get; set; }
            public string purchaseServiceStatus { get; set; }
            public IEnumerable<CloseoutDetailQuery> closeouts { get; set; }
            public bool? audit { get; set; }
            public DateTime? auditDate { get; set; }
            public Guid? auditUserID { get; set; }
            public string auditFirstName { get; set; }
            public string auditLastName { get; set; }
            public string auditInvoice { get; set; }
            public long serviceID { get; set; }
            public DateTime serviceDateTime { get; set; }
            public DateTime dateSaved { get; set; }
            public int currencyID { get; set; }
            public string currencyCode { get; set; }
            public long? promoID { get; set; }
            public IEnumerable<CouponDetailQuery> details { get; set; }
            public bool? applyToCost { get; set; }
        }
        public class CloseoutDetailQuery
        {
            public DateTime closeOutDate { get; set; }
            public string shortName { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public bool paid { get; set; }
        }
        public class CouponDetailQuery
        {
            public string coupon { get; set; }
            public decimal quantity { get; set; }
            public long? priceID { get; set; }
            public long? netPriceID { get; set; }
            public decimal? dealPrice { get; set; }
            public bool promo { get; set; }
            public string priceType { get; set; }
            public int? priceTypeID { get; set; }
            public long purchaseServiceDetailID { get; set; }
            public decimal? customPrice { get; set; }
            public decimal? customCost { get; set; }
            public decimal? customCostAlt { get; set; }
            public decimal? customExchangeRate { get; set; }
            public int priceCurrencyID { get; set; }
        }

        public class BankCommissionProfit
        {
            public Guid? PurchaseID { get; set; }
            public int? TransactionType { get; set; }
            public decimal? AmountUSD { get; set; }
            public decimal? CommissionVolumeUSD { get; set; }
            public decimal? ProfitVolumeUSD { get; set; }
            public decimal? AmountMXN { get; set; }
            public decimal? CommissionVolumeMXN { get; set; }
            public decimal? ProfitVolumeMXN { get; set; }
            public string CardType { get; set; }
            public GenericListItem Commission { get; set; }
        }
    }

    public class BudgetsViewModel
    {
        [Display(Name = "Terminal")]
        public string Terminal { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "Points of Sale")]
        public string PointsOfSale { get; set; }
        [Display(Name = "Budgets Type")]
        public string BudgetsType { get; set; }
        public List<BudgetsResult> Summary { get; set; }

        public class SearchBudgetsViewModel
        {
            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [Display(Name = "Points of Sale")]
            public int[] Search_SelectedPointsOfSale { get; set; }
            [Display(Name = "Summary Budget By")]
            public string[] Search_SelectedBudgetType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
            //[ScriptIgnore]
            public List<SelectListItem> PointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> BudgetsTypeListItem
            {
                get
                {
                    List<SelectListItem> BudgetsTypeList = new List<SelectListItem>();
                    BudgetsTypeList.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Payments"
                    });
                    BudgetsTypeList.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Egresses"
                    });
                    return BudgetsTypeList;
                }
            }

        }

        public class BudgetsResult
        {
            public int BudgetID { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Cant { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Team { get; set; }
            public Money Budget { get; set; }
            public Money Used { get; set; }
            public Money Available { get; set; }
            public List<DetailsBudget> Details { get; set; }
        }
        public class DetailsBudget
        {
            public long DetailBudgetID { get; set; }
            public decimal ExchangeRate { get; set; }
            public DateTime Date { get; set; }
            public Money Amount { get; set; }
            public Money Budget { get; set; }
            public string Type { get; set; }
            public string Opc { get; set; }
            public string Location { get; set; }
            public string Agent { get; set; }
            public string ModifiedByUser { get; set; }
            public string LastModification { get; set; }
            public string Client { get; set; }
        }

        public class BudgetQuery
        {
            public long? ID { get; set; }
            public int budgetID { get; set; }
            public DateTime dateSaved { get; set; }
            public string Type { get; set; }
            public string Team { get; set; }
            public string Description { get; set; }
            public Guid? LastModifyUser { get; set; }
            public DateTime? LastDateModify { get; set; }
            public Decimal BudgetAmount { get; set; }
            public string CurrencyCode { get; set; }
            public string TransactionType { get; set; }
            public string OpcName { get; set; }
            public string Location { get; set; }
            public string Client { get; set; }
            public Guid UserID { get; set; }
            public string List { get; set; }
            public int pointOfSaleID { get; set; }
        }

    }

    public class BillingModel
    {
        [Display(Name = "Terminal")]
        public string Terminal { get; set; }
        [Display(Name = "Companies")]
        public string Companies { get; set; }
        [Display(Name = "Points of Sale")]
        public string PointsOfSale { get; set; }
        [Display(Name = "Currency")]
        public string Currency { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "Report Type")]
        public string ReportType { get; set; }
        public string ErrorMessage { get; set; }

        public List<BillingDetail> Details { get; set; }
        public List<CompanyBilling> CompaniesTotals { get; set; }

        public class AccAcc
        {
            public int? AccAccID { get; set; }
            public string AccAccNumber { get; set; }
            public string AccAccName { get; set; }
            public string AccAccArticle { get; set; }
            public string AccAccPriceType { get; set; }
        }

        public class BillingDetail
        {
            public Guid PurchaseID { get; set; }
            public string PointOfSale { get; set; }
            public string Customer { get; set; }
            public string Folio { get; set; }
            public string CouponReference { get; set; }
            public string Date { get; set; }
            public string Category { get; set; }
            public string Service { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public Money UnitTotal { get; set; }
            public string CloseOut { get; set; }
            public string PriceType { get; set; }
            public List<BillingAssignation> Assignations { get; set; }
            public string Audited { get; set; }
        }

        public class BillingAssignation
        {
            public string PaymentType { get; set; }
            public string CxCCompany { get; set; }
            public string CxCOPC { get; set; }
            public string CxCOPCLegacyKey { get; set; }
            public string CxCOPCTeam { get; set; }
            public string CxCInvitation { get; set; }
            public string CxCBudget { get; set; }
            public int? AccAccID { get; set; }
            public string AccAccNumber { get; set; }
            public string AccAccName { get; set; }
            public string AccAccArticle { get; set; }
            public Money Payment { get; set; }
            public decimal ExchangeRate { get; set; }
            public Money Total { get; set; }
        }

        public class CompanyBilling
        {
            public int? CxCCompanyID { get; set; }
            public string CxCCompany { get; set; }
            public List<AccAccDetail> AccountingAccounts { get; set; }
            public Money Subtotal { get; set; }
            public Money IVA { get; set; }
            public Money Total { get; set; }
        }

        public class AccAccDetail
        {
            public int? AccAccID { get; set; }
            public string AccAccNumber { get; set; }
            public string AccAccName { get; set; }
            public string AccAccArticle { get; set; }
            public string AccAccPriceType { get; set; }
            public int Units { get; set; }
            public Money AverageUnit { get; set; }
            public Money Subtotal { get; set; }
            public Money IVA { get; set; }
            public Money Total { get; set; }
        }

        public class SearchBilling
        {
            [Display(Name = "Use Cache")]
            public bool Search_Cache { get; set; }
            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [Display(Name = "Companies")]
            public int?[] Search_SelectedCompanies { get; set; }
            [Display(Name = "Points of Sale")]
            public int[] Search_SelectedPointsOfSale { get; set; }
            [Display(Name = "Currency")]
            public string Search_Currency { get; set; }
            [Display(Name = "Transaction Type")]
            public int Search_MoneyTransactionType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
            //[ScriptIgnore]
            public List<SelectListItem> Companies
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            //[ScriptIgnore]
            public List<SelectListItem> PointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD().OrderBy(x => x.Text).ToList();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> MoneyTransactions
            {
                get
                {
                    List<SelectListItem> newList = new List<SelectListItem>();
                    newList.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Invoice"
                    });
                    newList.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Credit Note"
                    });
                    return newList;
                }
            }
        }
    }

    public class InvoiceModel
    {
        public string Date { get; set; }
        public string LegalEntity { get; set; }
        public string RFC { get; set; }
        public string Address { get; set; }
        public string CP { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string TotalInText { get; set; }

        public class InvoiceItem
        {
            public int NumberOfPurchases { get; set; }
            public Guid PurchaseID { get; set; }
            public string PurchaseDateID { get; set; }
            public List<CouponDetail> Coupons { get; set; }
            public List<PurchasesModel.PurchasePaymentModel> Payments { get; set; }
            public decimal TotalNoIVA { get; set; }
        }

        public class SearchInvoice
        {
            [Display(Name = "Date")]
            public string Search_Date { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
            [Display(Name = "Company")]
            public int? Search_CompanyID { get; set; }
            public List<SelectListItem> Search_Companies { get; set; }
        }
    }

    public class IncomePolicyModel
    {
        public string PointOfSale { get; set; }
        public string Date { get; set; }
        public string AccountingAccounts { get; set; }
        public List<PriceType> PriceTypes { get; set; }
        public List<PolicyItem> Incomes { get; set; }
        public List<ProductionModel.TotalDetail> IncomesTotalPerPrice { get; set; }
        public List<Money> IncomesTotal { get; set; }
        public List<Money> IncomesIVATotal { get; set; }
        public List<PolicyItem> Refunds { get; set; }
        public List<ProductionModel.TotalDetail> RefundsTotalPerPrice { get; set; }
        public List<Money> RefundsTotal { get; set; }
        public List<Money> RefundsIVATotal { get; set; }
        public List<Money> GrandTotal { get; set; }
        public List<Money> GrandIVATotal { get; set; }

        public class PolicyItem
        {
            public int? AccountingAccountID { get; set; }
            public string Account { get; set; }
            public string AccountName { get; set; }
            public List<ProductionModel.ServiceIncomeItem> Services { get; set; }
            public List<Money> Total { get; set; }
        }
    }

    public class UserPermissionsModel
    {
        public List<UserItem> Users { get; set; }
        public List<SysComponentItem> SysComponents { get; set; }
        public class UserItem
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string JobPosition { get; set; }
            public string SupervisedBy { get; set; }
            public string Status { get; set; }
            public string Locked { get; set; }
            public DateTime LastActivityDate { get; set; }
            public string Terminals { get; set; }
            public List<AccessProfileItem> AccessProfiles { get; set; }
        }

        public class AccessProfileItem
        {
            public string WorkGroup { get; set; }
            public string Role { get; set; }
            public List<PermissionItem> Permissions { get; set; }
        }

        public class PermissionItem
        {
            public long SysComponentID { get; set; }
            public bool Read { get; set; }
        }

        public class SysComponentItem
        {
            public long SysComponentID { get; set; }
            public string SysComponent { get; set; }
        }

        public class SearchUserPermissions
        {
            [Display(Name = "Status")]
            public bool? Search_Status { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_StatusList
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = "Any",
                        Value = ""
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Active",
                        Value = "true"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Inactive",
                        Value = "false"
                    });
                    return list;
                }
            }
            [Display(Name = "WorkGroups")]
            public int[] Search_WorkGroups { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_WorkGroupsList
            {
                get
                {
                    return AdminDataModel.AdminCatalogs.GetAllWorkGroups();
                }
            }
            [Display(Name = "Roles")]
            public Guid[] Search_Roles { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_RolesList
            {
                get
                {
                    return AdminDataModel.AdminCatalogs.GetAllRoles();
                }
            }
            [Display(Name = "Terminals")]
            public long[] Search_Terminals { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_TerminalsList
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpTerminals();
                }
            }
        }
    }

    public class LegacyUserPermissionsModel
    {
        public List<UserItem> Users { get; set; }

        public List<SysComponentItem> SysComponents { get; set; }

        public class UserItem
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string Locked { get; set; }
            public DateTime LastActivityDate { get; set; }
            public string Terminals { get; set; }
            public List<AccessProfileItem> AccessProfiles { get; set; }
        }

        public class AccessProfileItem
        {
            public string Role { get; set; }
            public List<PermissionItem> Permissions { get; set; }
        }

        public class PermissionItem
        {
            public int SysComponentID { get; set; }
            public bool Read { get; set; }
        }

        public class SysComponentItem
        {
            public int SysComponentID { get; set; }
            public string SysComponent { get; set; }
        }

        public class SearchUserPermissions
        {
            [Display(Name = "Status")]
            public bool? Search_Status { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_StatusList
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = "Any",
                        Value = ""
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Active",
                        Value = "true"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Inactive",
                        Value = "false"
                    });
                    return list;
                }
            }
        }
    }

    public class CouponsHistoryModel
    {
        public string PointOfSale { get; set; }
        public string Provider { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string AuditedStatus { get; set; }
        public string CouponStatus { get; set; }
        public List<CouponItem> Coupons { get; set; }

        public class CouponQueryItem
        {
            public Guid purchaseID { get; set; }
            public long purchase_ServiceID { get; set; }
            public long serviceID { get; set; }
            public string comercialName { get; set; }
            public string service { get; set; }
            public DateTime serviceDateTime { get; set; }
            public long? promoID { get; set; }
            public string promo { get; set; }
            public int serviceStatusID { get; set; }
            public string purchaseServiceStatus { get; set; }
            public long? replacementOf { get; set; }
            public string coupon { get; set; }
            public Guid? userID { get; set; }
            public bool online { get; set; }
            public int pointOfSaleID { get; set; }
            public string culture { get; set; }
            public decimal total { get; set; }
            public int currencyID { get; set; }
            public string currencyCode { get; set; }
            public DateTime dateSaved { get; set; }
            public DateTime? confirmationDateTime { get; set; }
            public Guid? confirmedByUserID { get; set; }
            public DateTime? cancelationDateTime { get; set; }
            public Guid? canceledByUserID { get; set; }
            public bool? audit { get; set; }
            public Guid? auditedByUserID { get; set; }
            public DateTime? auditDate { get; set; }
            public string auditInvoice { get; set; }
            public long? providerInvoiceID { get; set; }
            public string invoiceNumber { get; set; }
            public string folio { get; set; }
            public IEnumerable<CloseOutQueryItem> closeouts { get; set; }
            public IEnumerable<DetailQueryItem> details { get; set; }
            public bool? applyToCost { get; set; }
        }

        public class CloseOutQueryItem
        {
            public Guid? salesAgentUserID { get; set; }
            public string shortName { get; set; }
            public DateTime closeOutDate { get; set; }
            public long purchase_ServiceID { get; set; }
            public bool paid { get; set; }
        }

        public class DetailQueryItem
        {
            public int? priceTypeID { get; set; }
            public string priceType { get; set; }
            public long? netPriceID { get; set; }
            public long? priceID { get; set; }
            public bool promo { get; set; }
            public decimal quantity { get; set; }
            public long purchaseServiceDetailID { get; set; }
            public decimal? customCost { get; set; }
            public decimal? customCostAlt { get; set; }
            public decimal? customCostAltNoIVA { get; set; }
            public decimal? customCostNoIVA { get; set; }
            public string currencyCode { get; set; }
        }

        public class CouponItem
        {
            public Guid PurchaseID { get; set; }
            public long Purchase_ServiceID { get; set; }
            public string Folio { get; set; }
            public string Provider { get; set; }
            public string Service { get; set; }
            public string ServiceDate { get; set; }
            public string Promo { get; set; }
            public string Status { get; set; }
            public int ServiceStatusID { get; set; }
            public string Replacements { get; set; }
            //public string PriceType { get; set; }
            public decimal Total { get; set; }
            public string Currency { get; set; }
            public Money Cost { get; set; }
            public string DateSaved { get; set; }
            public string AgentPurchase { get; set; }
            public string DateConfirmed { get; set; }
            public string AgentConfirmed { get; set; }
            public string DateCanceled { get; set; }
            public string AgentCanceled { get; set; }
            public string CloseOut { get; set; }
            public string AuditedByUser { get; set; }
            public string AuditedOnDate { get; set; }
            public string AuditedOnInvoice { get; set; }
            public List<CouponItemDetail> Details { get; set; }
        }

        public class CouponItemDetail : BillingModel.AccAcc
        {
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public Money UnitCostTotal { get; set; }
        }

        public class SearchCouponsHistory
        {
            [Display(Name = "Use Cache")]
            public bool Search_Cache { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Point of Sale")]
            public int[] Search_PointOfSaleID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "From Coupon Number")]
            public string Search_FromCoupon { get; set; }

            [Display(Name = "To Coupon Number")]
            public string Search_ToCoupon { get; set; }

            [Display(Name = "Saved on Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }

            [Display(Name = "Activity Dates")]
            public string Search_I_ActivityFromDate { get; set; }
            public string Search_F_ActivityToDate { get; set; }

            [Display(Name = "Audit Dates")]
            public string Search_I_AuditFromDate { get; set; }
            public string Search_F_AuditToDate { get; set; }

            [Display(Name = "Audit Status")]
            public bool[] Search_Audited { get; set; }

            [Display(Name = "Coupon Status")]
            public int[] Search_CouponStatusID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_AuditStatus
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "true",
                        Text = "Audited"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "false",
                        Text = "Not Audited"
                    });
                    return list;
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Search_CouponStatus
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpServiceStatus();
                }
            }

            [Display(Name = "Providers")]
            public int[] Search_ProviderID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Providers
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
        }
    }

    public class ProvidersProductionModel
    {
        public string Dates { get; set; }
        public string PointOfSale { get; set; }
        public string SelectedProviders { get; set; }
        public List<PriceType> PriceTypes { get; set; }
        public List<ServiceIncomeItem> ActivitiesIncomes { get; set; }
        public List<TotalByProvider> TotalsByProvider { get; set; }
        public List<Money> Totals { get; set; }

        public class ServiceIncomeItem
        {
            public long ServiceID { get; set; }
            public string Provider { get; set; }
            public string Service { get; set; }
            public int NumberOfCoupons { get; set; }
            public List<ProductionModel.TotalDetail> SalesPerPrice { get; set; }
            public List<Money> ServiceTotal { get; set; }
        }

        public class TotalByProvider
        {
            public int ProviderID { get; set; }
            public string Provider { get; set; }
            public List<ProductionModel.TotalDetail> SalesPerPrice { get; set; }
            public List<Money> ProviderTotal { get; set; }
        }

        public class SearchProviderProduction
        {
            [Display(Name = "Use Cache")]
            public bool Search_Cache { get; set; }

            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Point of Sale")]
            public int[] Search_PointOfSaleID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "Providers")]
            public int[] Search_ProviderID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Providers
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
        }
    }

    public class CategoriesProductionModel
    {
        public string PointOfSale { get; set; }
        public string Dates { get; set; }
        public string SelectedCategories { get; set; }
        public List<PriceType> PriceTypes { get; set; }
        public List<CategoryItem> Categories { get; set; }
        public List<ProductionModel.TotalDetail> CategoriesTotalPrice { get; set; }
        public List<Money> CategoriesTotal { get; set; }

        public class CategoryItem
        {
            public long? CategoryID { get; set; }
            public string Category { get; set; }
            public List<ProductionModel.ServiceIncomeItem> Services { get; set; }
            public List<Money> Total { get; set; }
        }
    }

    public class PriceType
    {
        public int PriceTypeID { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
    }

    public class CouponDetailStored
    {
        public long PurchaseServiceID { get; set; }
        public CouponDetail Coupon { get; set; }
        public int PriceTypeID { get; set; }
    }

    public class CouponDetail
    {
        public Guid PurchaseID { get; set; }
        public string Customer { get; set; }
        public long PurchaseServiceID { get; set; }
        public int PointOfSaleID { get; set; }
        public string Folio { get; set; }
        public string CouponReference { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime SavedOnDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string Service { get; set; }
        public string Provider { get; set; }
        public int ProviderID { get; set; }
        public string ProviderAvanceID { get; set; }
        public decimal UnitsQty { get; set; }
        public string Units { get; set; }
        public string PointOfSale { get; set; }
        public string SalesAgent { get; set; }
        public string ReservationsAgent { get; set; }
        public string PriceType { get; set; }
        public decimal Percentage { get; set; }
        public decimal? Amount { get; set; }
        public int? CommissionCurrencyID { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public decimal CommissionPercentage { get; set; }
        public Money Total { get; set; }
        public Money TotalNoIVA { get; set; }
        public bool CustomCost { get; set; }
        public Money Cost { get; set; }
        public Money CostNoIVA { get; set; }
        public Money Utility { get; set; }
        public Money UtilityNoIVA { get; set; }
        public Money DealDiff { get; set; }
        public Money DealDiffNoIVA { get; set; }
        public decimal AdultUnits { get; set; }
        public decimal ChildUnits { get; set; }
        public Money Commission { get; set; }
        public decimal ExchangeRate { get; set; }
        public long ExchangeRateID { get; set; }
        public string ExchangeRateVigency { get; set; }
        public Money SalesMXN { get; set; }
        public Money CommissionMXN { get; set; }
        public decimal UtilityPercentage { get; set; }
        public string Promo { get; set; }
        public string AuthCode { get; set; }
        public string PendingCharges { get; set; }
        public int? LocationID { get; set; }
        public string Location { get; set; }
    }

    public class CommissionsReportModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PointOfSale { get; set; }
        public List<PriceType> PriceTypes { get; set; }
        public List<CommissionsWorker> Workers { get; set; }
        public List<CommissionTotal> Totals { get; set; }

        public class CouponQuery
        {
            public Guid? userID { get; set; }
            public bool online { get; set; }
            public Guid? confirmedByUserID { get; set; }
            public Guid? agentID { get; set; }
            public long serviceID { get; set; }
            public string service { get; set; }
            public IEnumerable<int?> priceTypeIDs { get; set; }
            public int pointOfSaleID { get; set; }
            public string currencyCode { get; set; }
            public long purchase_ServiceID { get; set; }
            public DateTime? confirmationDateTime { get; set; }
            public DateTime? cancelationDateTime { get; set; }
        }

        public class CommissionsWorker
        {
            public Guid UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int JobPositionID { get; set; }
            public string JobPosition { get; set; }
            public List<Guid?> SubordinatedUsers { get; set; }
            public List<CommissionsByService> ListServices { get; set; }
            public List<CommissionDetail> TotalsPerPrice { get; set; }
            public CommissionTotal Total { get; set; }
        }

        public class CommissionsByService
        {
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public List<CommissionDetail> CommissionsPerPrice { get; set; }
            public CommissionTotal Subtotal { get; set; }
        }

        public class CommissionDetail
        {
            public int PriceTypeID { get; set; }
            public List<CouponDetail> Coupons { get; set; }
            public List<Money> Sales { get; set; }
            public List<Money> Commissions { get; set; }
            public Money SalesMXN { get; set; }
            public Money CommissionMXN { get; set; }
        }

        public class CommissionTotal
        {
            public List<Money> Sales { get; set; }
            public List<Money> Commissions { get; set; }
            public Money SalesMXN { get; set; }
            public Money CommissionMXN { get; set; }
        }

        public class SearchCommissionsReportModel
        {
            [Display(Name = "Use Cache")]
            public bool Search_Cache { get; set; }

            [Required]
            [Display(Name = "Dates")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Point of Sale")]
            public int[] Search_PointOfSaleID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(long.Parse(Search_Terminals.FirstOrDefault().Value));
                    return list;
                }
            }

            [Display(Name = "Sales Agent")]
            public Guid[] Search_SalesAgentID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_SalesAgents
            {
                get
                {
                    return ReportDataModel.ReportsCatalogs.FillDrpUsersWithCommission(long.Parse(Search_Terminals.FirstOrDefault().Value));
                }
            }

            [Display(Name = "Report Type")]
            public int Search_ReportType { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_ReportTypes
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        //Text = "Commissions",
                        Text = "Demonstrations",
                        Value = "1"
                    });
                    list.Add(new SelectListItem()
                    {
                        Text = "Bonus",
                        Value = "2"
                    });
                    return list;
                }
            }
        }
    }

    public class MasterCloseOutModel
    {
        public class SearchMasterCloseOutModel
        {
            [Required]
            [Display(Name = "Dates Range")]
            public string Search_I_FromDate { get; set; }

            [Required]
            [Display(Name = "To Date")]
            public string Search_F_ToDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Points of Sale")]
            public int[] Search_PointOfSaleID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(long.Parse(Search_Terminals.FirstOrDefault().Value), null, null, true);
                    return list;
                }
            }

            [Display(Name = "Only Coupons to be Processed")]
            public bool Search_Pending { get; set; }
        }

        public class MasterCloseOutResults
        {
            public List<CloseOutsPerDay> CloseOutsByDate { get; set; }
            public List<long> CouponIDs { get; set; }
            public int TotalCoupons { get; set; }
            public int Cached { get; set; }
            public int OutOfDate { get; set; }
            public int NotCached { get; set; }
        }

        public class CloseOutsPerDay
        {
            public DateTime Date { get; set; }
            public List<CloseOutsPerPos> PointsOfSale { get; set; }
            public int NumberOfCoupons { get; set; }
        }

        public class CloseOutsPerPos
        {
            public int PointOfSaleID { get; set; }
            public string PointOfSale { get; set; }
            public List<CloseOutsPerAgent> CloseOuts { get; set; }
            public int NumberOfCoupons { get; set; }
        }

        public class CloseOutsPerAgent
        {
            public long CloseOutID { get; set; }
            public string Agent { get; set; }
            public string SavedOn { get; set; }
            public List<CouponBrief> Coupons { get; set; }
            public int NumberOfCoupons { get; set; }
        }

        public class CouponBrief
        {
            public Guid PurchaseID { get; set; }
            public long Purchase_ServiceID { get; set; }
            public string Folio { get; set; }
            public string CouponReference { get; set; }
            public string Item { get; set; }
            public string Customer { get; set; }
            public string Status { get; set; }
            public bool Closed { get; set; }
            public DateTime? CachedOn { get; set; }
            public string StatusMessage { get; set; }
        }
    }

    public class CloseOutModel
    {
        public long? CloseOutID { get; set; }
        public string Company { get; set; }
        public string PointOfSale { get; set; }
        public string Agent { get; set; }
        public string Date { get; set; }
        public bool Deletable { get; set; }
        public string ExchangeRate { get; set; }
        public List<ChargeBackConcept> ChargeBackConcepts { get; set; }
        public List<CloseOutListItem> ListSales { get; set; }
        public CloseOutListSubTotals ListSalesSubtotal { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalExtraPaid { get; set; }
        public List<CloseOutListItem> ListCancelations { get; set; }
        public CloseOutListSubTotals ListRefundsSubtotal { get; set; }
        public decimal TotalToRefund { get; set; }
        public decimal GrandTotalToRefund { get; set; }
        public decimal TotalRefunded { get; set; }
        public CloseOutTotals Totals { get; set; }
        public List<long> RelatedPayments { get; set; }
        public string Notes { get; set; }

        public class SearchCloseOutModel
        {
            [Required]
            [Display(Name = "Date")]
            public string SearchCloseOut_Date { get; set; }

            [Required]
            [Range(1, Int32.MaxValue)]
            [Display(Name = "Point of Sale")]
            public int SearchCloseOut_PointOfSaleID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchCloseOut_PointsOfSale
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                    list.Insert(0, ePlatBack.Models.Utils.ListItems.Default("Select One"));
                    return list;
                }
            }

            [Display(Name = "Sales Agent")]
            public Guid? SearchCloseOut_SalesAgentID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchCloseOut_SalesAgents
            {
                get
                {
                    UserSession us = new UserSession();
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Text = us.User,
                        Value = us.UserID.ToString()
                    });
                    return list;
                }
            }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long SearchCloseOut_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchCloseOut_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
        }

        public class ChargeBackConcept
        {
            public long ConceptID { get; set; }
            public string Concept { get; set; }
        }

        public class CloseOutTotals
        {
            public int Coupons { get; set; }
            public List<Money> Sales { get; set; }
            public List<Money> SalesMXN { get; set; }
            public List<Money> Refunds { get; set; }
            public List<Money> RefundsMXN { get; set; }
            public List<Money> GrandTotal { get; set; }
            public List<Money> GrandTotalMXN { get; set; }
            public List<Money> Cash { get; set; }
            public List<Money> CreditCard { get; set; }
            public List<Money> ChargeBack { get; set; }
            public List<Money> TravelerCheck { get; set; }
            public List<Money> WireTransfer { get; set; }
            public List<Money> Certificate { get; set; }
            public List<Money> BankCommissions { get; set; }
        }

        public class CloseOutCouponDetails
        {
            public long Purchase_ServiceID { get; set; }
            public string DateTime { get; set; }
            public string DateTimeConfirmed { get; set; }
            public string DateTimeCanceled { get; set; }
            public string Coupon { get; set; }
            public string CouponReference { get; set; }
            public string Activity { get; set; }
            public string SalesAgent { get; set; }
            public string CancelationAgent { get; set; }
            public string Units { get; set; }
            public decimal Total { get; set; }
            public string Currency { get; set; }
            public string PriceType { get; set; }
            public string Status { get; set; }
        }

        public class CloseOutListSubTotals
        {
            public List<Money> CouponsTotal { get; set; }
            public decimal CouponsTotalAmount { get; set; }
            public List<Money> Cash { get; set; }
            public List<Money> CreditCard { get; set; }
            public List<Money> ChargeBack { get; set; }
            public List<Money> TravelerCheck { get; set; }
            public List<Money> WireTransfer { get; set; }
            public List<Money> Certificate { get; set; }
            public decimal TransactionsTotalAmount { get; set; }
        }

        public class CloseOutListItem
        {
            public Guid PurchaseID { get; set; }
            public string Diagnosis { get; set; }
            public DateTime PurchaseDateTime { get; set; }
            public bool AssignationStatus { get; set; }
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public List<CloseOutCouponDetails> Coupons { get; set; }

            public decimal CouponsTotal { get; set; }
            public string Currency { get; set; }
            public decimal CouponsTotalAmount { get; set; }
            public List<Money> Cash { get; set; }
            public List<Money> CreditCard { get; set; }
            public List<Money> ChargeBack { get; set; }
            public List<ChargeBackConceptSubtotal> CBSubtotals { get; set; }
            public List<Money> TravelerCheck { get; set; }
            public List<Money> WireTransfers { get; set; }
            public List<Money> Certificate { get; set; }
            public decimal TransactionsTotalAmount { get; set; }
        }

        public class ChargeBackConceptSubtotal
        {
            public long ConceptID { get; set; }
            public List<SubtotalWithBudget> SubtotalWithBudget { get; set; }
        }

        public class SubtotalWithBudget
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public string Budget { get; set; }
        }

        public class CloseOutSaveModel
        {
            public string CloseOutDate { get; set; }
            public int PointOfSaleID { get; set; }
            public string SalesAgentUserID { get; set; }
            public long TerminalID { get; set; }
            public string JsonModel { get; set; }
            public string Notes { get; set; }
        }
    }

    public class Money
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class CloseOutHistoryModel
    {
        public class CloseOutsHistoryResults
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11192);//divTblCloseOutResults
                }
            }
            public string Company { get; set; }
            public string DatesRange { get; set; }
            public string PointOfSale { get; set; }
            public string SalesAgent { get; set; }
            public string Terminal { get; set; }
            public int NumberOfCoupons { get; set; }
            public List<CloseOutsHistoryListItem> CloseOuts { get; set; }
            public List<Money> Sales { get; set; }
            public List<Money> Refunds { get; set; }
            public List<Money> Totals { get; set; }
            public List<Money> CCCommissions { get; set; }

            public CloseOutModel.CloseOutTotals TotalDetails { get; set; }
        }

        public class SearchCloseOutsModel
        {
            [Required]
            [Display(Name = "Dates Range")]
            public string Search_I_FromDate { get; set; }

            [Required]
            [Display(Name = "To Date")]
            public string Search_F_ToDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Points of Sale")]
            public int[] Search_PointOfSaleID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(long.Parse(Search_Terminals.FirstOrDefault().Value));
                    return list;
                }
            }

            [Display(Name = "Sales Agent")]
            public Guid? Search_SalesAgentID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_SalesAgents
            {
                get
                {
                    UserSession us = new UserSession();
                    List<SelectListItem> list = new List<SelectListItem>();
                    ReportDataModel rdm = new ReportDataModel();
                    list = (List<SelectListItem>)rdm.GetDDLData("salesAgentsByTerminal", Search_Terminals.FirstOrDefault().Value);

                    return list;
                }
            }
        }

        public class CloseOutsHistoryListItem
        {
            public long CloseOutID { get; set; }
            public string Date { get; set; }
            public string PointOfSale { get; set; }
            public string Agent { get; set; }
            public List<Money> Sales { get; set; }
            public List<Money> Refunds { get; set; }
            public List<Money> Totals { get; set; }
            public List<Money> CCCommissions { get; set; }
            public string Notes { get; set; }
            public string SavedBy { get; set; }
            public string SavedOnDate { get; set; }
            public Money TotalInCache { get; set; }
            public List<CouponCache> CouponsInCache { get; set; }
        }

        public class CouponCache
        {
            public long Purchase_ServiceID { get; set; }
            public string Folio { get; set; }
            public decimal COTotal { get; set; }
            public decimal Total { get; set; }
            public decimal? CancelationCharge { get; set; }
            public int StatusID { get; set; }
            public string Status { get; set; }
        }
    }

    public class AuditCouponsModel
    {
        public List<InvoiceItem> Invoices { get; set; }

        public class InvoiceItem
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11259);//divProviderInvoice
                }
            }
            public long ProviderInvoiceID { get; set; }
            [Required]
            public string Invoice { get; set; }
            public DateTime Date { get; set; }
            public string DateString { get; set; }
            public string SavedBy { get; set; }
            public string Provider { get; set; }
            [Display(Name = "Provider")]
            public int? ProviderID { get; set; }
            [Display(Name = "Invoice Currency")]
            public string InvoiceCurrency { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> ProvidersList
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            public int NumberOfCoupons { get; set; }
            [Required]
            public string PurchaseServicesIDs { get; set; }
            public string PurchaseSelectedServicesIDs { get; set; }
            public string PaidPurchaseServicesIDs { get; set; }
            public List<CouponViewModel> Coupons { get; set; }
        }

        public class SearchInvoice
        {
            [Display(Name = "Coupon Folio")]
            public string SearchInvoice_Folio { get; set; }
            [Display(Name = "Terminal")]
            public long? SearchInvoice_TerminalID { get; set; }
            [Display(Name = "Provider")]
            public int? SearchInvoice_ProviderID { get; set; }
            [Display(Name = "Invoice Date")]
            public string SearchInvoice_InvoiceDate { get; set; }
            [Display(Name = "Invoice")]
            public string SearchInvoice_Invoice { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> SearchInvoice_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
            //[ScriptIgnore]
            public List<SelectListItem> SearchInvoice_Providers
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
            public InvoiceItem InvoiceInfo { get; set; }
        }

        public class SearchCoupon
        {
            [Required]
            [Display(Name = "Coupon Folio")]
            public string Search_Folio { get; set; }
        }
        public class InvoiceDetail
        {
            [ScriptIgnore]
            public List<SysComponentsPrivilegesModel> Privileges
            {
                get
                {
                    return AdminDataModel.GetViewPrivileges(11264);//divAuditDetails
                }
            }

            public long ProviderInvoiceID { get; set; }
            public string Date { get; set; }
            public string Provider { get; set; }
            public string Invoice { get; set; }
            public string ExchangeRate { get; set; }


            public List<PointOfSaleDetail> PointsOfSale { get; set; }
            public List<ServiceDetail> TotalServices { get; set; }
            public List<AccAccDetail> TotalAccAcc { get; set; }

            public List<Money> Total { get; set; }
            public List<Money> TotalNoIVA { get; set; }
            public List<Money> TotalIVA { get; set; }
            public List<Money> Cost { get; set; }
            public List<Money> CostNoIVA { get; set; }
            public List<Money> CostIVA { get; set; }
            public decimal NumberOfUnits { get; set; }
        }

        public class PointOfSaleDetail
        {
            public int PointOfSaleID { get; set; }
            public string PointOfSale { get; set; }
            public int NumberOfCoupons { get; set; }
            public decimal NumberOfUnits { get; set; }
            public List<Money> Total { get; set; }
            public List<Money> Cost { get; set; }
            public List<ServiceDetail> Services { get; set; }
        }

        public class AccAccDetail : BillingModel.AccAcc
        {
            public List<Money> Cost { get; set; }
            public List<Money> CostNoIVA { get; set; }
            public List<Money> CostIVA { get; set; }
        }

        public class ServiceDetail
        {
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public int NumberOfCoupons { get; set; }
            public decimal NumberOfUnits { get; set; }
            public List<Money> Total { get; set; }
            public List<Money> TotalNoIVA { get; set; }
            public List<Money> TotalIVA { get; set; }
            public List<Money> Cost { get; set; }
            public List<Money> CostNoIVA { get; set; }
            public List<Money> CostIVA { get; set; }
            public List<CouponAuditDetails> Coupons { get; set; }
        }

        public class CouponAuditDetails
        {
            public long PurchaseServiceID { get; set; }
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public string Folio { get; set; }
            public string CouponReference { get; set; }
            public string Date { get; set; }
            public string CurrencyOfSale { get; set; }
            public string CultureOfSale { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal NumberOfUnits { get; set; }
            public List<CouponAuditUnit> Units { get; set; }
            public List<Money> Total { get; set; }
            public List<Money> Cost { get; set; }
            public List<Money> CostNoIVA { get; set; }
            public List<Money> CostIVA { get; set; }
        }

        public class CouponAuditUnit : BillingModel.AccAcc
        {
            public long PurchaseServiceDetailID { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public string PriceType { get; set; }
            public List<Money> UnitCost { get; set; }
            public List<CurrencyRule> UnitRule { get; set; }
            public bool CustomCost { get; set; }
            public List<Money> Cost { get; set; }
            public List<Money> CostNoIVA { get; set; }
            public List<Money> CostIVA { get; set; }
        }

        public class CustomCostModel
        {
            public long PurchaseServiceDetailID { get; set; }
            public decimal CustomCost { get; set; }
            public decimal CustomCostNoIVA { get; set; }
            public decimal CustomCostAlt { get; set; }
            public decimal CustomCostAltNoIVA { get; set; }
            public decimal CustomCostPercentage { get; set; }
            public decimal CustomCostExchangeRate { get; set; }
            public string CustomCostNote { get; set; }
        }

        public class CouponCostViewModel
        {
            public long PurchaseServiceID { get; set; }
            public string Folio { get; set; }
            public string Service { get; set; }
            public string Provider { get; set; }

            [Display(Name = "Confirmation Date")]
            public string ConfirmationDateTime { get; set; }
            [Display(Name = "Currency")]
            public string CurrencyCode { get; set; }
            [Display(Name = "Language")]
            public string Culture { get; set; }

            public long ExchangeRateID { get; set; }
            [Display(Name = "Exchange Rate")]
            public decimal ExchangeRate { get; set; }
            [Display(Name = "Rate Type")]
            public string ExchangeRateType { get; set; }
            [Display(Name = "Contract")]
            public string ProviderContract { get; set; }

            public long PurchaseServiceDetailID { get; set; }

            public int BasePriceTypeID { get; set; }
            public string BasePriceType { get; set; }
            public string BasePriceCurrency { get; set; }
            public List<Money> BasePriceNoIVA { get; set; }
            public List<Money> BasePriceIVA { get; set; }
            public List<Money> BasePriceTotal { get; set; }

            public decimal CostPercentage { get; set; }

            public List<Money> CostPriceNoIVA { get; set; }
            public List<Money> CostPriceIVA { get; set; }
            public List<Money> CostPriceTotal { get; set; }

            public string BaseRuleMXN { get; set; }
            public string BaseRuleUSD { get; set; }

            public string CostRuleMXN { get; set; }
            public string CostRuleUSD { get; set; }

            public decimal NewCostTotal { get; set; }
            [Display(Name = "Notes")]
            public string Notes { get; set; }
        }
    }

    public class CurrencyRule
    {
        public string Rule { get; set; }
        public string Currency { get; set; }
    }

    public class PreArrivalFeedbackModel
    {
        public class SearchFeedback
        {
            [Required]
            [Display(Name = "Dates Range")]
            public string Search_I_FromDate { get; set; }

            [Required]
            public string Search_F_ToDate { get; set; }
        }

        public class FeedbackResults
        {
            public string ArrivalDate { get; set; }
            public string Resort { get; set; }
            public string LeadSource { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PrearrivalBookingStatus { get; set; }
            public string PresentationDate { get; set; }
            public string AssignedTo { get; set; }
            public string ReservationStatus { get; set; }
            public string OnsiteQualification { get; set; }
            public string NQReason { get; set; }
            public string Team { get; set; }
            public string Promotor { get; set; }
            public string OnsiteBookingStatus { get; set; }
            public string OnsitePresentationDate { get; set; }
            public string PrearrivalFeedback { get; set; }
        }
    }

    public class ChargeBacksModel
    {
        public class SearchChargeBacks
        {
            [Required]
            [Display(Name = "Dates Range")]
            public string Search_I_FromDate { get; set; }

            [Required]
            [Display(Name = "To Date")]
            public string Search_F_ToDate { get; set; }

            [Display(Name = "Layout")]
            public int? Search_LayoutID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Layouts
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "",
                        Text = "None"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Coupons CXC"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Charge Backs to OPCs"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "3",
                        Text = "Cash Repositions"
                    });
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                    return list;
                }
            }

            [Display(Name = "Funds")]
            public int[] Search_FundIDs { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Funds
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    return list;
                }
            }

            [Display(Name = "Charge Back Types")]
            public int[] Search_ChargeBackTypeIDs { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_ChargeBackTypes
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Customer's Deposit"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Charge Backs"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "3",
                        Text = "Coupons CXC"
                    });
                    return list;
                }
            }

            [Display(Name = "Concepts")]
            public long?[] Search_ChargeBackConceptIDs { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_ChargeBackConcepts
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    return list;
                }
            }

            [Display(Name = "Points Of Sale")]
            public long?[] Search_PointsOfSaleIDs { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_PointsOfSale
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    return list;
                }
            }

            [Display(Name = "Charged to")]
            public int?[] Search_CompanyIDs { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Companies
            {
                get
                {
                    List<SelectListItem> list = OpcDataModel.GetCompanies();
                    return list;
                }
            }

            [Display(Name = "Teams")]
            public int?[] Search_PromotionTeamIDs { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_PromotionTeams
            {
                get
                {
                    List<SelectListItem> list = OpcDataModel.GetPromotionTeams();
                    return list;
                }
            }

            [Display(Name = "OPC")]
            public long? Search_OPCID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_OPCs
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpAllOPCs(null, null);
                    list.Insert(0, new SelectListItem()
                    {
                        Value = "",
                        Text = "All"
                    });
                    return list;
                }
            }

            [Display(Name = "Locations")]
            public int?[] Search_LocationIDs { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Locations
            {
                get
                {
                    List<SelectListItem> list = OpcDataModel.GetLocations();
                    return list;
                }
            }

            [Display(Name = "Agents")]
            public Guid?[] Search_AgentIDs { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Agents
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    return list;
                }
            }

        }
        public class ChargeBacksResults
        {
            public string Dates { get; set; }
            public string Layout { get; set; }
            public string Terminal { get; set; }
            public string Funds { get; set; }
            public long[] ChargeBackConcepts { get; set; }
            public string PointOfSale { get; set; }
            public string ChargedTo { get; set; }
            public string Teams { get; set; }
            public string OPC { get; set; }
            public string Locations { get; set; }
            public string Agents { get; set; }

            public List<ChargeItem> Deposits { get; set; }
            public List<ChargeByConcept> DepositConceptsTotals { get; set; }
            public ChargeTotals DepositsTotals { get; set; }

            public List<ChargeItem> ChargeBacks { get; set; }
            public List<ChargeByConcept> ChargeBackConceptsTotals { get; set; }
            public ChargeTotals ChargeBacksTotals { get; set; }

            public List<ChargeItem> CouponsCxC { get; set; }
            public List<TotalByCompany> TotalCompanies { get; set; }
            public List<ChargeByConcept> CouponConceptsTotals { get; set; }
            public ChargeTotals CouponsTotals { get; set; }
        }

        public class TotalByCompany
        {
            public int? CxCCompanyID { get; set; }
            public string CxCCompany { get; set; }
            public List<Money> Totals { get; set; }
            public Money TotalMXN { get; set; }
        }

        public class ChargeTotals
        {
            public List<Money> Amount { get; set; }
            public Money AmountMXN { get; set; }
            public List<Money> Balance { get; set; }
            public Money BalanceMXN { get; set; }
            public List<Money> Charges { get; set; }
            public Money ChargesMXN { get; set; }
        }

        public class ChargeByConcept
        {
            public long ConceptID { get; set; }
            public string Concept { get; set; }
            public List<Money> Amount { get; set; }
            public Money AmountMXN { get; set; }
            public List<Money> Charges { get; set; }
            public Money ChargesMXN { get; set; }
        }

        public class ChargeItem
        {
            public long ChargeID { get; set; }
            public DateTime Date { get; set; }
            public string Fund { get; set; }
            public string Invitation { get; set; }
            public string Customer { get; set; }
            public string Agent { get; set; }
            public string PointOfSale { get; set; }
            public long OPCID { get; set; }
            public string OPC { get; set; }
            public string OPCAvanceID { get; set; }
            public string OPCLegacyKey { get; set; }
            public string Program { get; set; }
            public string PromoTeam { get; set; }
            public string Location { get; set; }
            public string ChargedTo { get; set; }
            public string PayingCompany { get; set; }
            public string Budget { get; set; }
            public long ConceptID { get; set; }
            public string Concept { get; set; }
            public string Folio { get; set; }
            public decimal Amount { get; set; }
            public decimal Charge { get; set; }
            public int? CurrencyID { get; set; }
            public string CurrencyCode { get; set; }
            public string Comments { get; set; }
            public bool Charged { get; set; }
            public string ChargedBy { get; set; }
            public string ChargedDate { get; set; }
            public decimal Partials { get; set; }
            public decimal Balance { get; set; }
            public decimal ER { get; set; }
            public List<Partial> PartialPayments { get; set; }
        }

        public class Partial
        {
            public long PartialID { get; set; }
            public long ChargeID { get; set; }
            public string Description { get; set; }
            public string PartialDate { get; set; }
            public string User { get; set; }
            public decimal Amount { get; set; }
            public int CurrencyID { get; set; }
            public string CurrencyCode { get; set; }
            public decimal AmountInPaymentCurrency { get; set; }
            public decimal Balance { get; set; }
            public bool Deleted { get; set; }
            public string DeletedDetails { get; set; }
        }
    }

    public class CashStatementModel
    {
        public class CashStatementResults
        {
            public string Month { get; set; }
            public string Terminal { get; set; }
            public string Fund { get; set; }
            public string FundCurrencyCode { get; set; }
            public decimal InitialAmount { get; set; }
            public decimal FinalAmount { get; set; }
            public List<StatementItem> Items { get; set; }
            public decimal TotalEgress { get; set; }
            public decimal TotalIncome { get; set; }
        }

        public class StatementItem
        {
            public DateTime OriginalDate { get; set; }
            public string Date { get; set; }
            public string DateModified { get; set; }
            public string Concept { get; set; }
            public string Invitation { get; set; }
            public string Agent { get; set; }
            public decimal Amount { get; set; }
            public string CurrencyCode { get; set; }
            public decimal AmountEgress { get; set; }
            public decimal AmountIncome { get; set; }
            public decimal AmountBalance { get; set; }
        }

        public class SearchStatement
        {
            [Required]
            [Display(Name = "Month")]
            public string Search_Month { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Months
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    DateTime date = DateTime.Parse("2015-04-01");
                    for (int i = 0; date.AddMonths(i) <= DateTime.Today; i++)
                    {
                        DateTime currentDate = date.AddMonths(i);
                        list.Add(ListItems.Default(currentDate.ToString("MMMM, yyyy", CultureInfo.InvariantCulture), currentDate.ToString("yyyy-MM-dd")));
                    };
                    return list;
                }
            }

            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    List<SelectListItem> list = MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                    return list;
                }
            }

            [Required]
            [Display(Name = "Funds")]
            public int Search_FundID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_Funds
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    return list;
                }
            }
        }
    }

    public class WeeklyReportModel
    {
        public string WeeklyReport_Name { get; set; }
        public string WeeklyReport_LeadSource { get; set; }
        public string WeeklyReport_User { get; set; }
        public string WeeklyReport_MemberNumber { get; set; }
        public string WeeklyReport_TourStatus { get; set; }
        public string WeeklyReport_FinalTourStatus { get; set; }
        public string WeeklyReport_TourDate { get; set; }
        public string WeeklyReport_RealTourDate { get; set; }
        public string WeeklyReport_SalesVolume { get; set; }
        public string WeeklyReport_ArrivalDate { get; set; }
        public string WeeklyReport_BookingStatus { get; set; }
        public string WeeklyReport_Resort { get; set; }
        public string WeeklyReport_TotalPaid { get; set; }
    }

    public class ReservationsMadeReportModel
    {
        public string ReservationsMade_LeadID { get; set; }
        public string ReservationsMade_FirstName { get; set; }
        public string ReservationsMade_LastName { get; set; }
        public string ReservationsMade_InputDate { get; set; }
        public string ReservationsMade_ReservationDate { get; set; }
    }

    public class PreBookedContactedLeadsReportModel
    {
        public string PreBookedContactedLeadsReport_Resort { get; set; }
        public string PreBookedContactedLeadsReport_PreBookedLeads { get; set; }
        public string PreBookedContactedLeadsReport_ContactedLeads { get; set; }
    }

    public class IncorrectlyPreBookedToursReportModel
    {
        public string LeadID { get; set; }
        public string Name { get; set; }
        public string ClubType { get; set; }
        public string AccountNumber { get; set; }
        public string ContractNumber { get; set; }
        public string CoOwner { get; set; }
        public string ArrivalDate { get; set; }
        public string Resort { get; set; }
        public string BookingStatus { get; set; }
        public string PresentationDate { get; set; }
        //public string HasMemberInfo { get; set; }
        //public string Adults { get; set; }
        //public string Children { get; set; }
        public string FlightInfo { get; set; }
        public string LeadStatus { get; set; }
        public string LeadSource { get; set; }
        public string AssignedToUser { get; set; }
    }

    public class OptionsSoldReportModel
    {
        public string OptionsSoldReport_Resort { get; set; }
        public string OptionsSoldReport_Arrivals { get; set; }
        public string OptionsSoldReport_ArrivalsWithOptions { get; set; }
        public string OptionsSoldReport_LeadsWithOptions { get; set; }
        public string OptionsSoldReport_OptionsCapture { get; set; }
        public string OptionsSoldReport_TotalPaidVolume { get; set; }
        public string OptionsSoldReport_TotalNights { get; set; }
    }

    public class ConfirmedLeadsReportModel
    {
        public string ConfirmedLeadsReport_UserName { get; set; }
        public string ConfirmedLeadsReport_FirstName { get; set; }
        public string ConfirmedLeadsReport_LastName { get; set; }
        public string ConfirmedLeadsReport_ArrivalDate { get; set; }
        public string ConfirmedLeadsReport_BookingStatus { get; set; }
        public string ConfirmedLeadsReport_TourStatus { get; set; }
        public string ConfirmedLeadsReport_FinalTourStatus { get; set; }
    }

    public class TourInfo
    {
        public int? ResortID { get; set; }
        public long? ReservationID { get; set; }
        public string TourStatus { get; set; }
        public DateTime? TourDate { get; set; }
        public decimal? Volume { get; set; }
        public string ContractStatus { get; set; }
        public string Source { get; set; }
        public int CustomerID { get; set; }
        public Guid? ArrivalID { get; set; }
    }

    public class PreArrivalReportModel : PreArrivalWeeklyReportResults
    {
        public long?[] Terminals { get; set; }
        public bool Extemporaneous { get; set; }
        public bool IsExternal { get; set; }
        public bool IsLinked { get; set; }
        public bool TourFound { get; set; }
        public bool IsBooked { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        public string NumberNights { get; set; }
        public string ConfirmationNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string BookingStatus { get; set; }
        public string SecondaryBookingStatus { get; set; }
        public string ReservationStatus { get; set; }
        public string BookingSource { get; set; }
        public string Resort { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RoomType { get; set; }
        public string FrontAgencyName { get; set; }
        public string NumberOfPax { get; set; }
        public string ContactType { get; set; }
        public string ContactDate { get; set; }
        public string OptionsIncludedInPackage { get; set; }
        public string ServiceSoldByPreArrival { get; set; }
        public string ServiceSoldComments { get; set; }
        public string PricePaid { get; set; }
        public string PurchaseDate { get; set; }
        public string PaymentType { get; set; }
        public string InvoiceNumber { get; set; }
        public string MemberNumber { get; set; }
        public string TransactionReference { get; set; }
        public string Hooked { get; set; }//bookingStatus
        public string QualificationStatus { get; set; }
        public string SalesVolume { get; set; }
        public string OptionSoldID { get; set; }
        public string OptionsQuantity { get; set; }
        public string OptionTypes { get; set; }
        public string Options { get; set; }
        public string OptionsTotal { get; set; }
        public string FlightInfo { get; set; }
        public string TourStatus { get; set; }
        public string TourDate { get; set; }
        public string Concierge { get; set; }
        public string ConciergeComments { get; set; }
        public string AssignedToUser { get; set; }
        public string AssignedToUserID { get; set; }
        public string ChargedByUser { get; set; }
        public string ChargedByUserID { get; set; }
        public string LeadSourceID { get; set; }
        public string LeadSource { get; set; }
        public string LeadStatus { get; set; }
        public bool Paid { get; set; }
        public string MarketCode { get; set; }
        public string PresentationModifiedByUser { get; set; }
        public string PresentationModificationDate { get; set; }
        public string CommissionPercentage { get; set; }
        public string CommissionVolume { get; set; }
        //public List<KeyValuePair<string, string>> Chart { get; set; }
        public string Chart { get; set; }
        public string MerchantAccount { get; set; }
        //Email
        public string ReservationID { get; set; }
        public string EmailNotificationID { get; set; }
        public string TransactionID { get; set; }
        public string EmailBody { get; set; }
        //Transportation
        public string Airline { get; set; }
        public string FlightType { get; set; }
        public string FlightNumber { get; set; }
        public string FlightDateTime { get; set; }
        public string FlightDate { get; set; }
        public string FlightTime { get; set; }
        public string FlightComments { get; set; }
        public string PickUpTime { get; set; }
        public string NumberOfPassengers { get; set; }
        public string Passengers { get; set; }
        public string Source { get; set; }
        public Dictionary<string, int> ListOptionTypes { get; set; }
        public List<KeyValuePair<string, int>> aListOptionTypes { get; set; }
        //public List<KeyValuePair<string,KeyValuePair<string, decimal>>> ListPayments { get; set; }
        public List<GenericStringModel> ListPayments { get; set; }
        public List<TourInfo> ToursInfo { get; set; }
    }


    public class ManifestReportModel
    {
        public Guid? leadGroupID { get; set; }
        public int frontOfficeResortID { get; set; }
        public long frontOfficeReservationID { get; set; }
        public Guid reservationID { get; set; }
        public Guid assignedToUserID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public long? leadSourceID { get; set; }
        public int leadStatusID { get; set; }
        public string place { get; set; }
        public string hotelConfirmationNumber { get; set; }
        public DateTime? arrivalDate { get; set; }
        public string frontOfficeAgencyName { get; set; }
        public int? totalNights { get; set; }
        public int bookingStatusID { get; set; }
        public int? secondaryBookingStatusID { get; set; }
        public int? arrivalBookingStatusID { get; set; }
        public string reservationStatusID { get; set; }
        public long? placeID { get; set; }
        public List<tblOptionsSold> options { get; set; }
    }
    public class ModSelectListItem : SelectListItem
    {
        public string PropertyName { get; set; }
        public string FieldType { get; set; }
    }

    public class DynamicSearchFilters
    {
        public string FieldID { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public string FieldType { get; set; }
        public string PropertyName { get; set; }
    }

    public class GenericStringModel
    {
        public string Generic_Property1 { get; set; }
        public string Generic_Property2 { get; set; }
        public string Generic_Property3 { get; set; }
        public string Generic_Property4 { get; set; }
        public string Generic_Property5 { get; set; }
        public string Generic_Property6 { get; set; }
    }

    public class PricesModel
    {
        public List<ActivityPricesReportModel> ListPrices { get; set; }
        public string ExchangeRate { get; set; }
        public int SelectedCurrency { get; set; }
        public bool FullView { get; set; }
        public class SearchPricesCustomModel
        {
            [Required]
            [Display(Name = "Date")]
            public string Search_Date { get; set; }

            [Display(Name = "Layout")]
            [Range(1, int.MaxValue,
            ErrorMessage = "You need to select a Layout.")]
            public int Search_Layout { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_DrpLayouts
            {
                get
                {
                    var list = ReportDataModel.ReportsCatalogs.FillDrpLayouts();
                    list.Insert(0, new SelectListItem()
                    {
                        Value = "",
                        Text = "Select One",
                        Selected = true
                    });
                    list.Insert(1, new SelectListItem()
                    {
                        Value = "-1",
                        Text = "+ Add Layout"
                    });
                    return list;
                }
            }
        }

        public class PricesReportLayout
        {
            public int? LayoutID { get; set; }
            [Display(Name = "Layout")]
            public string Layout { get; set; }
            [Display(Name = "Terminal")]
            public long TerminalID { get; set; }
            [Display(Name = "User")]
            public Guid UserID { get; set; }
            public string User { get; set; }
            [Display(Name = "Saved on")]
            public string DateSaved { get; set; }
            [Display(Name = "Culture")]
            public string Culture { get; set; }
            [Display(Name = "Currencies")]
            public int[] SelectedCurrencies { get; set; }
            [Display(Name = "Price Types")]
            public int[] SelectedPriceTypes { get; set; }
            [Display(Name = "Available for Roles")]
            public Guid[] SelectedRoles { get; set; }
            [Display(Name = "Category")]
            public long? CategoryID { get; set; }
            [Display(Name = "Activities")]
            public long[] SelectedServices { get; set; }
            [Display(Name = "Use Settings from PoS")]
            public int? PointOfSaleID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Cultures
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCultures();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD();
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> PriceTypes
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPriceTypesForTerminal(long.Parse(Terminals.FirstOrDefault().Value));
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Roles
            {
                get
                {
                    return AdminDataModel.AdminCatalogs.FillDrpActiveRoles(long.Parse(Terminals.FirstOrDefault().Value));
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Categories
            {
                get
                {
                    List<SelectListItem> list = ActivityDataModel.ActivitiesCatalogs.FillDrpCategories();
                    list.Insert(0, ListItems.Default("All", ""));
                    return list;
                }
            }
            [ScriptIgnore]
            public List<SelectListItem> Services
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpServicesWithProvider(long.Parse(Terminals.FirstOrDefault().Value));
                }
            }
            //[ScriptIgnore]
            public List<SelectListItem> PointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
        }

        public class SearchPricesModel
        {
            [Required]
            [Display(Name = "Activity Date")]
            public string SearchPrices_Date { get; set; }

            [Required]
            [Display(Name = "Booking Date")]
            public string SearchPrices_BookingDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Provider")]
            public int? Search_Provider { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_DrpProviders
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpProvidersPerDestinationInTerminals(long.Parse(Search_Terminals.First().Value));
                    list.Insert(0, new SelectListItem() { Text = "All", Value = "" });
                    return list;
                }
            }

            [Display(Name = "Activity")]
            public long Search_Activity { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_DrpActivities
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpServices(0, long.Parse(Search_Terminals.First().Value));
                    list.Insert(0, ePlatBack.Models.Utils.ListItems.Default("All"));
                    return list;
                }
            }


            [Display(Name = "Currency")]
            public int? Search_Currency { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_DrpCurrencies
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD();
                    list.Insert(0, new SelectListItem()
                    {
                        Value = "",
                        Text = "All"
                    });
                    return list;
                }
            }

            [Display(Name = "Language")]
            public string Search_Culture { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_DrpCultures
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpCultures();
                    return list;
                }
            }

            [Display(Name = "Use Settings from PoS")]
            public int Search_PointOfSaleID { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> Search_DrpPointsOfSale
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            public bool Search_UpdateCache { get; set; }
        }

        public class PricesCustomModel
        {
            public string ReportLayoutName { get; set; }
            public string Date { get; set; }
            public string ExchangeRate { get; set; }
            public string ProviderExchangeRate { get; set; }
            public List<CurrencyType> Currencies { get; set; }
            public List<PriceType> PriceTypes { get; set; }
            public List<PriceItem> Prices { get; set; }
        }

        public class CurrencyType
        {
            public int CurrencyID { get; set; }
            public string CurrencyCode { get; set; }
        }

        public class PriceItem
        {
            public long ServiceID { get; set; }
            public string Category { get; set; }
            public string Provider { get; set; }
            public string Activity { get; set; }
            public string Unit { get; set; }
            public List<PriceDetail> PricesPerType { get; set; }
        }

        public class PriceDetail
        {
            public string PriceType { get; set; }
            public int PriceTypeID { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public string Rule { get; set; }
            public bool Base { get; set; }
            public bool Active { get; set; }
            public bool IsCost { get; set; }
            public long? PromoID { get; set; }
            public long? DependingOnPriceID { get; set; }
        }
    }

    public class ActivityPricesReportModel
    {
        public string Prices_Provider { get; set; }
        public string Prices_Activity { get; set; }
        public string Prices_Unit { get; set; }
        public string Prices_Currency_1_PriceType_1 { get; set; }
        public string Prices_Currency_1_PriceType_2 { get; set; }
        public string Prices_Currency_1_PriceType_3 { get; set; }
        public string Prices_Currency_1_PriceType_4 { get; set; }
        public string Prices_Currency_1_PriceType_5 { get; set; }
        public string Prices_Currency_2_PriceType_1 { get; set; }
        public string Prices_Currency_2_PriceType_2 { get; set; }
        public string Prices_Currency_2_PriceType_3 { get; set; }
        public string Prices_Currency_2_PriceType_4 { get; set; }
        public string Prices_Currency_2_PriceType_5 { get; set; }
        public string Prices_Currency_1_PriceType_1_Rule { get; set; }
        public string Prices_Currency_1_PriceType_2_Rule { get; set; }
        public string Prices_Currency_1_PriceType_3_Rule { get; set; }
        public string Prices_Currency_1_PriceType_4_Rule { get; set; }
        public string Prices_Currency_1_PriceType_5_Rule { get; set; }
        public string Prices_Currency_2_PriceType_1_Rule { get; set; }
        public string Prices_Currency_2_PriceType_2_Rule { get; set; }
        public string Prices_Currency_2_PriceType_3_Rule { get; set; }
        public string Prices_Currency_2_PriceType_4_Rule { get; set; }
        public string Prices_Currency_2_PriceType_5_Rule { get; set; }
    }

    public class CommissionsReportClass
    {
        public int jobPositionID { get; set; }
        public long terminalID { get; set; }
        public int priceTypeID { get; set; }
        public string PriceType { get; set; }
        public decimal commissionPercentage { get; set; }
        public decimal? commissionAmount { get; set; }
        public int? commissionCurrencyID { get; set; }
        public bool @override { get; set; }
        public IEnumerable<int> pointOfSaleIDs { get; set; }
        public decimal minProfit { get; set; }
        public decimal minVolume { get; set; }
        public decimal? maxProfit { get; set; }
        public decimal? maxVolume { get; set; }
        public string volumeCurrencyCode { get; set; }
        public bool permanent_ { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public bool applyOnSales { get; set; }
        public bool applyOnDealPrice { get; set; }
        public bool applyOnAdultSales { get; set; }
        public bool applyOnChildSales { get; set; }
        public bool onlyIfCharged { get; set; }
    }

    public class AccountingAccountsViewModel
    {
        [Display(Name = "Accounting Account(s)")]
        public string Category { get; set; }
        [Display(Name = "Terminal")]
        public string Terminals { get; set; }
        [Display(Name = "Provider")]
        public string Provider { get; set; }
        [Display(Name = "Company")]
        public string Company { get; set; }
        public List<AccountingAccountsResult> Summary { get; set; }

        public class searchAccountingAccounts
        {
            [Display(Name = "Accounting Account")]
            public int[] searchCategory { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> CategoriesList
            {
                get
                {
                    return new List<SelectListItem>();

                }
            }
            [Display(Name = "Terminals")]
            public string searchTerminals { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> TerminalList
            {
                get
                {
                    var list = MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                    list.Insert(0, Utils.ListItems.Default());
                    return list;
                }
            }
            [Display(Name = "Providers")]
            public int[] searchProvider { get; set; }
            //[ScriptIgnore]
            public List<SelectListItem> ProvidersList
            {
                get
                {
                    return new List<SelectListItem>();

                }
            }

        }
        public class AccountingAccountsResult
        {
            public int? AccountAccountsID { get; set; }
            public string AccountAccountsName { get; set; }
            public string AccountAccountsServiceID { get; set; }
            public string AccountAccountsProvider { get; set; }
            public string AccountAccountsServiceName { get; set; }
            public string AccountAccountsTerminals { get; set; }
            public string AccountAccountsCategories { get; set; }
        }

        public class ServiceAccountQuery
        {
            public int? AccountID { get; set; }
            public string AccountName { get; set; }
            public long ServiceID { get; set; }
            public string ProviderName { get; set; }
            public string TerminalName { get; set; }
            public string ServiceName { get; set; }
            public bool Assigned { get; set; }
            public string CategoryName { get; set; }
        }

        public class category
        {
            public int categoryID { get; set; }
            public string categoryName { get; set; }
        }
    }

    public class AgentsPerformanceViewModel
    {
        public class AgentsPerformanceResults
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string PhoneLabels { get; set; }

            public List<AgentPerformance> AgentsMoreThanForty { get; set; }
            public TotalPerformance TotalMoreThanForty { get; set; }
            public List<AgentPerformance> AgentsFirstContact { get; set; }
            public TotalPerformance TotalFirstContact { get; set; }
        }

        public class AgentPerformance
        {
            public Guid UserID { get; set; }
            public Guid? EcommerceUserID { get; set; }
            public string Agent { get; set; }
            public string Extension { get; set; }
            public int Calls { get; set; }
            public int Sales { get; set; }
            public decimal Closing { get; set; }
            public decimal TotalSales { get; set; }
            public decimal AverageSale { get; set; }
        }

        public class TotalPerformance
        {
            public int Calls { get; set; }
            public int Sales { get; set; }
            public decimal Closing { get; set; }
            public decimal TotalSales { get; set; }
            public decimal AverageSale { get; set; }
        }
    }

    public class CallsByLocationViewModel
    {

        public class SearchCalls
        {
            [Required]
            [Display(Name = "Dates Range")]
            public string Search_I_FromDate { get; set; }

            [Required]
            [Display(Name = "To Date")]
            public string Search_F_ToDate { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            [Display(Name = "Terminal")]
            public long? Search_TerminalID { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_Terminals
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpSelectedTerminals();
                }
            }

            [Display(Name = "Phone Labels")]
            public string[] Search_Phones { get; set; }
            [ScriptIgnore]
            public List<SelectListItem> Search_PhonesList
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }
        }
        public class CallsByLocationResults
        {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string PhoneLabels { get; set; }

            public int TotalRecords { get; set; }
            public DurationMetrics Duration { get; set; }
            public List<ExtensionCalls> ExtensionCalls { get; set; }
            public List<LabelCalls> LabelCalls { get; set; }
            public List<DestinationBookings> DestinationBookings { get; set; }

            public List<CallSale> Sales { get; set; }
            public decimal BookingRate { get; set; }

            public List<CallByCountry> CallsByCountry { get; set; }
        }

        public class DestinationBookings
        {
            public string Destination { get; set; }
            public string DestinationInitials { get; set; }
            public int Calls { get; set; }
            public int Bookings { get; set; }
            public decimal DestinationPercentage { get; set; }
        }

        public class CallsLogQuery
        {
            public long callLogID { get; set; }
            public DateTime? callDateTime { get; set; }
            public string phoneSource { get; set; }
            public string phoneDialed { get; set; }
            public string agentExtension { get; set; }
            public int? duration { get; set; }
            public int? areaCodeID { get; set; }
            public int? areaID { get; set; }
            public int? stateID { get; set; }
            public int? countryID { get; set; }
            public long? destinationID { get; set; }
            public string destination { get; set; }
        }

        public class DurationMetrics
        {
            public int MoreThanForty { get; set; }
            public int LessThanForty { get; set; }
            public int Duplicated { get; set; }
            public int LessThanFortyDuplicated { get; set; }
        }

        public class ExtensionCalls
        {
            public string Extension { get; set; }
            public int TotalCalls { get; set; }
        }

        public class LabelCalls
        {
            public string Label { get; set; }
            public string Phone { get; set; }
            public int TotalCalls { get; set; }
        }

        public class CallSale
        {
            public int ReservationID { get; set; }
            public DateTime? SaleDate { get; set; }
            public string Certificate { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Arrival { get; set; }
            public string Departure { get; set; }
            public string Destination { get; set; }
            public string Resort { get; set; }
            public string Confirmation { get; set; }
            public string Status { get; set; }
            public bool Matched { get; set; }

            public int? AreaCodeID { get; set; }
            public string Code { get; set; }
            public int? AreaID { get; set; }
            public string Area { get; set; }
            public int? StateID { get; set; }
            public string State { get; set; }
            public int? CountryID { get; set; }
            public string Country { get; set; }
        }

        public class Call
        {
            public string CallDate { get; set; }
            public string Source { get; set; }
            public string Dialed { get; set; }
            public string Extension { get; set; }
            public int? DuracionSecs { get; set; }
            public string Duration { get; set; }
            public string Label { get; set; }
            public bool Duplicated { get; set; }
            public bool Booking { get; set; }
            public string Destination { get; set; }
        }

        public class CallByCountry
        {
            public int? CountryID { get; set; }
            public string Country { get; set; }
            public List<CallByState> States { get; set; }
            public int TotalCountryCalls { get; set; }
            public decimal PercentageCountryCalls { get; set; }
            public int TotalCountrySales { get; set; }
            public decimal BookingRate { get; set; }
            public int Span { get; set; }
            public List<DestinationBookings> DestinationBookings { get; set; }
        }

        public class CallByState
        {
            public int? StateID { get; set; }
            public string State { get; set; }
            public string StateCode { get; set; }
            public List<CallByArea> Areas { get; set; }
            public int TotalStateCalls { get; set; }
            public decimal PercentageStateCalls { get; set; }
            public decimal PercentageCountryCalls { get; set; }
            public int TotalStateSales { get; set; }
            public decimal BookingRate { get; set; }
            public int Span { get; set; }
            public List<DestinationBookings> DestinationBookings { get; set; }
        }

        public class CallByArea
        {
            public int? AreaID { get; set; }
            public string Area { get; set; }
            public List<CallByCode> Codes { get; set; }
            public int TotalAreaCalls { get; set; }
            public decimal PercentageAreaCalls { get; set; }
            public decimal PercentageStateCalls { get; set; }
            public int TotalAreaSales { get; set; }
            public decimal BookingRate { get; set; }
            public int Span { get; set; }
            public List<DestinationBookings> DestinationBookings { get; set; }
        }

        public class CallByCode
        {
            public int? AreaCodeID { get; set; }
            public string Code { get; set; }
            public string Places { get; set; }
            public List<Call> Calls { get; set; }
            public int TotalCodeCalls { get; set; }
            public decimal PercentageCodeCalls { get; set; }
            public decimal PercentageAreaCalls { get; set; }
            public int TotalCodeSales { get; set; }
            public decimal BookingRate { get; set; }
            public List<DestinationBookings> DestinationBookings { get; set; }
        }
    }

    public class NotificationsModel
    {
        public bool ShowVLOClassification { get; set; }
        public List<NotificationsReportModel> ListNotifications { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public class SearchNotificationsModel
        {
            public string SearchNotifications_Terminals { get; set; }
            [Display(Name = "Form")]
            public int[] SearchNotifications_Forms { get; set; }

            [Display(Name = "Date")]
            public string SearchNotifications_I_Date { get; set; }
            public string SearchNotifications_F_Date { get; set; }
            public List<SelectListItem> SearchNotifications_DrpForms
            {
                get
                {
                    return NotificationsDataModel.NotificationsCatalogs.FillDrpFieldGroups();
                }
            }
        }
        public class NotificationsReportModel
        {
            public string NotificationsReport_VLO { get; set; }
            public int NotificationsReport_FormID { get; set; }
            public string NotificationsReport_Form { get; set; }
            public string NotificationsReport_Terminal { get; set; }
            public string NotificationsReport_Sent { get; set; }
            public string NotificationsReport_Received { get; set; }
            public string NotificationsReport_Opened { get; set; }
            public string NotificationsReport_OpenRate { get; set; }
            public string NotificationsReport_Clicked { get; set; }
            public List<NotificationsModel.NotificationsClickModel> NotificationsReport_ClicksList { get; set; }
            public string NotificationsReport_ClickRate { get; set; }
            public string NotificationsReport_Clicks { get; set; }
            public string NotificationsReport_TotalSent { get; set; }
            public string NotificationsReport_TotalReceived { get; set; }
            public string NotificationsReport_TotalOpened { get; set; }
            public string NotificationsReport_TotalOpenRate { get; set; }
            public string NotificationsReport_TotalClicks { get; set; }
            public string NotificationsReport_TotalClickRate { get; set; }
        }
        public class NotificationsClickModel
        {
            public string NotificationsReport_Transaction { get; set; }
            public string NotificationsReport_FormName { get; set; }
            public string NotificationsReport_GuestName { get; set; }
            public string NotificationsReport_VPANumber { get; set; }
            public string NotificationsReport_VLO { get; set; }
            public List<KeyValuePair<string, string>> NotificationsReport_Urls { get; set; }
            public string NotificationsReport_Status { get; set; }
        }

        public NotificationsModel()
        {
            ShowVLOClassification = true;
        }
    }

    public class UserLogsActivityYYYYMM
    {
        [Display(Name = "From Date ")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Contact")]

        public List<ActivityLogsResult> Summary { get; set; }

        public class SearchUserLogsActivity
        {
            [Required(ErrorMessage = "this field is required")]
            [Display(Name = "From Date")]
            public string SearchFromDate { get; set; }

            [Required(ErrorMessage = "this field is required")]
            [Display(Name = "To Date")]
            public string SearchToDate { get; set; }

            [Display(Name = "Contact")]
            public bool SearchConctactInfo { get; set; }

            [Display(Name = "Users")]
            public Guid[] SearchUserID { get; set; }
            public List<SelectListItem> DropSubordinateUsersList
            {
                get
                {
                    return ReportDataModel.GetSubordinateUsers();
                }
            }
            [Display(Name = "Terminal")]
            public int? searchTerminalID { get; set; }
            public List<SelectListItem> TerminalList
            {
                get
                {
                    var list = TerminalDataModel.GetCurrentUserTerminals();
                    list.Add(new SelectListItem()
                    {
                        Value = "",
                        Text = "--All--"
                    });
                    return list;
                }
            }

            [Display(Name = "Actions")]
            public int moduleID { get; set; }
            public List<SelectListItem> ModuleList
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "-1",
                        Text = "--All--"
                    });
                    return list;
                }

            }
            [Required]
            [Display(Name = "Module")]
            public int pageID { get; set; }
            public List<SelectListItem> pagesList
            {
                get
                {
                    return ReportDataModel.getModules();
                }
            }
        }
        public class ActivityLogsResult
        {
            public long? UserLogActivityIDInfo { get; set; }
            public DateTime? DateSavedInfo { get; set; }
            public Guid? UserIDInfo { get; set; }
            public string UserName { get; set; }
            public string ControllerInfo { get; set; }
            public string MethodInfo { get; set; }
            public string JsonModelInfo { get; set; }
            public string DescriptionInfo { get; set; }
            public string UrlMethodInfo { get; set; }
            public string UrlInfo { get; set; }
            public bool? ContactInfo { get; set; }
            public string terminalInfo { get; set; }
        }
        public class ActivitylogsTable
        {
            public long? UserLogActivity { get; set; }
            public string DateSaved { get; set; }
            public string UserName { get; set; }
            public string Controller { get; set; }
            public string Method { get; set; }
            public string Description { get; set; }
            public string UrlMethod { get; set; }
            public string Url { get; set; }
            public bool? ContactInfo { get; set; }
            public string terminal { get; set; }
        }
        public class ContactInfo
        {
            public int ContactInfoID { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string UserName { get; set; }
            public Guid UserID { get; set; }
        }
    }

    public class SalesByTeam
    {
        public class SearchSalesByTeam
        {
            [Required]
            [Display(Name = "From Date")]
            public DateTime Search_I_FromDate { get; set; }
            [Required]
            [Display(Name = "To Date")]
            public DateTime Search_F_ToDate { get; set; }
            [Required]
            [Display(Name = "Terminal")]
            public int SearchTerminalID { get; set; }
            public List<SelectListItem> fillDropTerminals
            {
                get
                {
                    var list = new List<SelectListItem>();
                    return list;
                }
            }
            [Required]
            [Display(Name = "Currencies")]
            public int SearchCurrency { get; set; }
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesNoCAD().OrderBy(x => x.Text).ToList();
                }
            }
        }
        public class SalesByTeamResult
        {
            public List<SalesByTeamInfo> Summary { get; set; }
            public List<totalProgram> totalByProgram { get; set; }
            public List<Money> total { get; set; }
            public percentageTable tableWithPercentage { get; set; }
        }

        public class SalesByTeamInfo
        {
            public Guid purchaseID { get; set; }
            public DateTime datePurchase { get; set; }
            public string marketingProgram { get; set; }
            public string subDivisionTeam { get; set; }
            public string folio { get; set; }
            public string couponReference { get; set; }
            public long couponInfoID { get; set; }
            public string unit { get; set; }
            public string priceType { get; set; }
            public int priceTypeID { get; set; }
            public int priceTypeGroupID { get; set; }
            public string priceTypeGroup { get; set; }
            public string currency { get; set; }
            public int serviceStatusID { get; set; }
            public string status { get; set; }
            public Money unitTotal { get; set; }
        }
        public class totalProgram
        {
            public string program { get; set; }
            public float percentage { get; set; }
            public List<totalByTeam> listTeam { get; set; }
            public Money totalByProgram { get; set; }
        }
        public class totalPriceType
        {
            public int priceTypeID { get; set; }
            public string priceType { get; set; }
            public int priceTypeGroupID { get; set; }
            public string priceTypeGroup { get; set; }
            public float percentage { get; set; }
            public Money totalPriceTypes { get; set; }
        }
        public class totalByTeam
        {
            public string Team { get; set; }
            public float percentage { get; set; }
            public Money totalTeam { get; set; }
            public List<totalPriceType> totalpriceTypesByTeam { get; set; }
        }

        public class programList
        {
            public string program { get; set; }
            public float programPercentage { get; set; }
            public Money programTotal { get; set; }
            public float totalPercentage { get; set; }
            // public Money totalGeneral { get; set; }            
            public List<totalByTeam> teamList { get; set; }
            //no depende directamente de la lista de programas
            public List<totalPriceType> priceTypes { get; set; }
        }

        public class percentageTable
        {
            public List<programList> programs { get; set; }
            public List<totalPriceType> prices { get; set; }
            public Totals tfoot { get; set; }
        }

        public class Totals
        {
            public Money totalByProgramsAndTeam { get; set; }
            public List<totalPriceType> totalByPrices { get; set; }
            public Money totalGrand { get; set; }
        }

        /*   public class totalByPercentage
           {
               public List<programsList>totalProgram { get; set; }          
               public List<totalPriceType> totalPercentagePT { get; set; }           
               public List<teamsList> totalTeams { get; set; }
               public List<priceTypes> prices { get; set; }            
               public int percentageGeneralTotal { get; set; }
               public Money generalTotal { get; set; }//suma de todos los tipos de precio 
               public class programsList
               {
                   public string programs { get; set; }
                   public int percentage { get; set; }
                   public Money totalPrograms { get; set; }// suma de todos los equipos que tiene el programa
               }
               public class teamsList //detalles
               {                
                   public string teams { get; set; }
                   public int percentagePriceType { get; set; }
                   public Money totalTeams { get; set; }
               }
               public class priceTypes
               {
                   public string priceT { get; set; }
                   public int percentage { get; set; }
                   public Money totalPriceType { get; set; }               
               }
           }*/
    }

    public class SearchPreArrivalWeeklyReportModel
    {
        [Display(Name = "Arrival Date")]
        public string Search_I_ArrivalDate { get; set; }
        public string Search_F_ArrivalDate { get; set; }

        [Display(Name = "Real Tour Date")]
        public string Search_I_RealTourDate { get; set; }
        public string Search_F_RealTourDate { get; set; }

        [Display(Name = "Purchase Date")]
        public string Search_I_PurchaseDate { get; set; }
        public string Search_F_PurchaseDate { get; set; }

        [Display(Name = "Assigned To User(s)")]
        public Guid?[] Search_AssignedToUsers { get; set; }
        public List<SelectListItem> Search_DrpUsers
        {
            get
            {
                return UserDataModel.GetUsersBySupervisor();
            }
        }

        [Display(Name = "Terminal(s)")]
        public long?[] Search_Terminals { get; set; }
        public List<SelectListItem> Search_DrpTerminals
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "10", Text = "Pre-arrival RC" });
                list.Add(new SelectListItem() { Value = "16", Text = "Pre-arrival TFR" });
                list.Add(new SelectListItem() { Value = "80", Text = "Pre-arrival TVG" });
                return list;
            }
        }

        [Display(Name = "Resort(s)")]
        public long?[] Search_Resorts { get; set; }
        public List<SelectListItem> Search_DrpResorts
        {
            get
            {
                return PlaceDataModel.GetResortsByProfile();
            }
        }

        [Display(Name = "Lead Source(s)")]
        public long?[] Search_LeadSources { get; set; }
        public List<SelectListItem> Search_DrpLeadSources
        {
            get
            {
                return LeadSourceDataModel.GetLeadSourcesByTerminal();
            }
        }

        [Display(Name = "Show Expanded")]
        public bool Search_Expanded { get; set; }

        [Display(Name = "Version")]
        public int Search_Version { get; set; }
        public List<SelectListItem> Search_DrpVersions
        {
            get
            {
                var list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "1", Text = "Version 1" });
                list.Add(new SelectListItem() { Value = "2", Text = "Version 2" });
                return list;
            }
        }
        public string Terminal { get; set; }
        public List<PreArrivalWeeklyReportResults> Results { get; set; }
        public List<PreArrivalWeeklyReportResults> ResultsLY { get; set; }
        public List<PreArrivalReportModel> Results1 { get; set; }
        public List<PreArrivalReportModel> Results2 { get; set; }
        public List<PreArrivalReportModel> Results2LY { get; set; }

        public SearchPreArrivalWeeklyReportModel()
        {
            Search_Expanded = false;
        }
    }

    public class PreArrivalWeeklyReportModel
    {
        public string Resort { get; set; }
        public Guid? ReservationID { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<string> CertificateNumber { get; set; }
        public string LeadSourceID { get; set; }
        public string LeadSource { get; set; }
        public string TotalArrivals { get; set; }
        public string NotAllowed { get; set; }
        public string RealArrivals { get; set; }
        public string NonContactable { get; set; }
        public string Contactable { get; set; }
        public string Contacted { get; set; }
        public string ContactPenetration { get; set; }
        public string ArrivalsSold { get; set; }
        public string Penetration { get; set; }
        public string ArrivalsTotalSold { get; set; }
        public string OptionsTotalSold { get; set; }
        public string AvgPerContactableArrival { get; set; }
        public string AvgPerContactedArrival { get; set; }
        public string AvgPerPurchase { get; set; }
        //public bool IsMember { get; set; }
        public List<SearchPreArrivalWeeklyReportModel> Response { get; set; }
    }

    public class PreArrivalWeeklyReportResults
    {
        public string Range { get; set; }
        public string RangeFlag { get; set; }
        public List<ResultsPerResort> ItemsPerResort { get; set; }
        //public List<PreArrivalWeeklyReportModel> Results { get; set; }//new
    }

    public class ResultsPerResort
    {
        public string Resort { get; set; }
        public PreArrivalWeeklyReportModel Items { get; set; }
        public List<PreArrivalWeeklyReportModel> Results { get; set; }//new
    }

    public class InvitationBalance
    {
        public SearchInvitationBalance search { get; set; }
        public List<InvitationBalanceModel> balance { get; set; }

        public class SearchInvitationBalance
        {
            [Display(Name = "From Date")]
            public DateTime? searchFromDate { get; set; }
            [Display(Name = "To Date")]
            public DateTime? searchToDate { get; set; }
            [Display(Name = "Invitation Number")]
            public string searchInvitationNumber { get; set; }
        }
        public class InvitationBalanceModel
        {
            public Guid invitationID { get; set; }
            public DateTime date { get; set; }
            public string invitationNumber { get; set; }
            public long? opcID { get; set; }
            public string opcName { get; set; }
            public string guest { get; set; }
            public string depositCurrency { get; set; }
            public Money depositAmount { get; set; }
            public Money balance { get; set; }
            public List<CouponsReference> coupons { get; set; }
            public List<Egresses> egresses { get; set; }
        }
        public class CouponsReference
        {
            public long couponID { get; set; }
            public string concept { get; set; }
            public string coupon { get; set; }
            public string user { get; set; }
            public Money total { get; set; }
            public string exchangeRateCoupon { get; set; }
            public string exchangeRateIDCoupon { get; set; }
            public DateTime? couponDate { get; set; }
            public string invitationNumber { get; set; }
        }
        public class Egresses
        {
            public long egressID { get; set; }
            public int opcID { get; set; }
            public string opcName { get; set; }
            public int currencyID { get; set; }
            public string currency { get; set; }
            public long? conceptID { get; set; }
            public string egressConcept { get; set; }
            public Money amount { get; set; }
            public string exchangeRateEgress { get; set; }
            public string exchangeRateIDEgress { get; set; }
            public DateTime egressReference { get; set; }
            public DateTime? egressDate { get; set; }
        }
    }

    public class PreArrivalImportHistory
    {
        public class Search
        {
            [Display(Name = "Arrival Date")]
            public string Search_I_ArrivalDate { get; set; }
            public string Search_F_ArrivalDate { get; set; }

            [Display(Name = "Input Date")]
            public string Search_I_InputDate { get; set; }
            public string Search_F_InputDate { get; set; }

            [Display(Name = "Resort")]
            public long?[] Search_Resorts { get; set; }
            public List<SelectListItem> Resorts
            {
                get
                {
                    return PlaceDataModel.GetResortsByProfile();
                }
            }
            public List<Response> Results { get; set; }
        }

        public class Response
        {
            public string HotelConfirmationNumber { get; set; }
            public string FrontOfficeCertificateNumber { get; set; }
            public string ArrivalDate { get; set; }
            public string TeamAssigned { get; set; }
            public string AssignedToUser { get; set; }
            public string InputByUser { get; set; }
            public string Resort { get; set; }
        }
    }

    public class PreArrivalBonusPayroll
    {
        public class Search
        {
            [Display(Name = "Arrival Date")]
            public string Search_I_ArrivalDate { get; set; }
            public string Search_F_ArrivalDate { get; set; }

            [Display(Name = "Tour Date")]
            public string Search_I_TourDate { get; set; }
            public string Search_F_TourDate { get; set; }

            public Guid?[] Search_Users { get; set; }
            public List<SelectListItem> Users
            {
                get
                {
                    return UserDataModel.GetUsersBySupervisor(null, true);
                }
            }
        }
    }
}


