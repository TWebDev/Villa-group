using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ePlatBack.Models.ViewModels
{
    public class GuestExperienceViewModel
    {
        public SearchFilters Search { get; set; }

        public class ArrivalsResponse
        {
            public List<ArrivalItem> Arrivals { get; set; }
            public Overview Overview { get; set; }
        }

        public class SearchFilters
        {
            [Display(Name = "Resort")]
            public int Search_FrontOfficeResortID{ get; set; }

            [Display(Name = "From Date")]
            public string Search_FromDate { get; set; }

            [Display(Name = "To Date")]
            public string Search_ToDate { get; set; }

            [Display(Name = "Preferences")]
            public int[] Search_Preferences { get; set; }

            [Display(Name = "Guest Name")]
            public string Search_GuestName { get; set; }

            [Display(Name = "Reservation Key")]
            public string Search_ConfirmationNumber { get; set; }

            [Display(Name = "CRS")]
            public string Search_Crs { get; set; }
        }

        public class Overview
        {
            public int RecurringClients { get; set; }
            public int Members { get; set; }
            public int WithSurveys { get; set; }
            public int HadProblems { get; set; }
            public int PrearrivalTafer { get; set; }
            public int PrearrivalResortCom { get; set; }
            public int WithPreferences { get; set; }
            public List<PreferenceCounter> Preferences { get; set; }
            public int WithComplaints { get; set; }
            public int WithCurrentTickets { get; set; }
        }

        public class PreferenceCounter
        {
            public int PreferenceID { get; set; }
            public int PreferenceTypeID { get; set; }
            public string Preference { get; set; }
            public int Amount { get; set; }
        }

        public class Profile
        {
            public CustomerInfo CustomerInfo { get; set; }
            public MemberInfo MemberInfo { get; set; }
            public List<PastReservation> Reservations { get; set; }
            public PrearrivalInfo PrearrivalInfo { get; set; }
            public List<GuestViewModel.Preferences.PreferenceTypeGuest> Preferences { get; set; }
            public List<InteractionsViewModel.Interaction> Interactions { get; set; }
            public List<ClarabridgeSurvey> ClarabridgeSurveys { get; set; }
            public List<Ticket> CurrentTickets { get; set; }
        }

        public class PrearrivalInfo
        {
            public long? PrearrivalTerminalID { get; set; }
            public Guid? LeadID { get; set; }
            public string PrearrivalDepartment { get; set; }
            public string BookingStatus { get; set; }
            public string AssignedTo { get; set; }
            public decimal? OptionsTotal { get; set; }
            public List<AdditionalItem> Additionals { get; set; }
        }

        public class CustomerInfo
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Picture { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }

            public int Stays { get; set; }
            public double Nights { get; set; }
            public decimal TotalSpend { get; set; }
            public decimal Rent { get; set; }
            public decimal Package { get; set; }
            public decimal Spa { get; set; }
            public decimal Pos { get; set; }
            public decimal Others { get; set; }

            public string TaferMembership { get; set; }
            public string FacebookID { get; set; }

        }

        public class MemberInfo
        {
            public string Contract { get; set; }
        }

        public class Reservation
        {
            public int FrontOfficeResortID { get; set; }
            public decimal FrontOfficeReservationID { get; set; }
            public string ConfirmationNumber { get; set; }
            public string CRS { get; set; }
            public DateTime? CheckIn { get; set; }
            public DateTime? CheckOut { get; set; }
            public double Nights { get; set; }
            public int? Adults { get; set; }
            public int? Children { get; set; }
            public string RoomType { get; set; }
            public string Agency { get; set; }
            public string Resort { get; set; }
        }

        public class ArrivalItem : Reservation
        {
            public Guid ArrivalID { get; set; }
            public Guid? GuestHubID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Guest
            {
                get
                {
                    return FirstName + " " + LastName;
                }
            }
            public string RoomNumber { get; set; }
            public string Market { get; set; }
            public decimal? Rate { get; set; }
            public string Notes { get; set; }

            public bool Recurring { get; set; }
            public bool Member { get; set; }
            public bool Survey { get; set; }
            public bool SurveyProblem { get; set; }
            public decimal SurveyRate { get; set; }
            public bool Prearrival { get; set; }
            public bool Preferences { get; set; }
            public bool CurrentTickets { get; set; }
            public bool Complaints { get; set; }

            public Profile GuestProfile { get; set; }
        }

        public class PastReservation : Reservation
        {
            public string Destination { get; set; }
            public string PlanType { get; set; }
            public decimal Total { get; set; }
            public decimal Rate { get; set; }
            public decimal Rent { get; set; }
            public decimal Package { get; set; }
            public decimal Spa { get; set; }
            public decimal Pos { get; set; }
            public decimal Others { get; set; }
            public int? ClarabridgeSurveyID { get; set; }
            public List<Ticket> Tickets { get; set; }
        }

        public class Ticket
        {
            public string Task { get; set; }
            public string Clasification { get; set; }
            public string Department { get; set; }
            public string Type { get; set; }
            public DateTime Opened { get; set; }
            public DateTime Closed { get; set; }
        }

        public class ClarabridgeSurvey
        {
            public int ClarabridgeSurveyID { get; set; }
            public DateTime? SubmittedDate { get; set; }
            public string Resort { get; set; }
            public bool? HadComments { get; set; }
            public bool? HadProblems { get; set; }
            public decimal Rate { get; set; }
        }

        public class AdditionalItem
        {
            public string Additional { get; set; }
            public decimal Amount { get; set; }
            public Money Total { get; set; }
        }
    }
}
