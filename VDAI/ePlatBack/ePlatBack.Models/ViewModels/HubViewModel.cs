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
    public class HubViewModel
    {
        public GuestSearchModel GuestSearch { get; set; }
        public GuestInfoModel GuestInfo { get; set; }       
    }

    public class GuestSearchModel
    {

        [Display(Name = "Guest ID")]
        public Guid GuestHubID { get; set; }

        [Display(Name = "CRS")]
        public string crs { get; set; }

        [Display(Name = "Reservation ID")]
        public int ReservationID { get; set; }

        [Display(Name = "Guest Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }


        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }


        [Display(Name = "Pick A Date")]
        public string SearchDateI { get; set; }
        public string SearchDatef { get; set; }

        [Display(Name = "Resorts")]
        public string Search_Resort_Value { get; set; }

        [Display(Name = "Resorts")]
        public int?[] Search_Resort { get; set; }
        public List<SelectListItem> ResortList
        {
            get
            {
                var list = HubDataModel.GetResortList();
                return list;
            }
        }




        [Display(Name = "Preference Type")]
        public string PreferenceType { get; set; }
        public List<SelectListItem> PreferenceTypesList
        {
            get
            {
                var list = HubDataModel.GetTypePreferencesList();
                return list;
            }
        }
        public List<SelectListItem> SearchPreferencesList = new List<SelectListItem>();


        [Display(Name = "Preference")]
        public string Preference { get; set; }
        public List<SelectListItem> PreferencesList
        {
            get
            {
                var list = HubDataModel.GetPreferencesListDefault();
                return list;
            }
        }

        [Display(Name = "Pick A Special Date")]
        public string SearchSpecialDate { get; set; }

    }

    public class GuestInfoModel
    {

        public PreferenceRegistrationModel PreferenceRegistration = new PreferenceRegistrationModel();

        [Display(Name = "Guest Picture")]
        public int picture { get; set; }

        public Guid GuestHubID { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }

        //[Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Display(Name = "Country")]
        public string Country { get; set; }

        public Int32 CountryID { get; set; }

        //[Display(Name = "State")]
        public string State { get; set; }

        //[Display(Name = "City")]
        public string City { get; set; }

        //[Display(Name = "Email")]
        public string Email { get; set; }

        public string Email2 { get; set; }

        public string Email3 { get; set; }

        //[Display(Name = "Guest Status")]
        public string BookingStatus { get; set; }




    }

    public class PreferenceRegistrationModel
    {
        public string GuestHubID { get; set; }



        [Display(Name = "Preference Type")]
        public string PreferenceType_Value { get; set; }
        public List<SelectListItem> PreferenceTypesList
        {
            get
            {
                var list = HubDataModel.GetTypePreferencesList();
                return list;
            }
        }


        [Display(Name = "Preference")]
        public string PreferenceValue { get; set; }
        public List<SelectListItem> PreferencesList
        {
            get
            {
                var list = HubDataModel.GetPreferencesListDefault();
                return list;
            }
        }


        [Display(Name = "Event")]
        public string SpecialDateName { get; set; }



        [Display(Name = "Month")]
        public string MonthValue { get; set; }
        public List<SelectListItem> MonthList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                for (int x = 01; x < 13; x++)
                {
                    if (x < 10)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = "0" + Convert.ToString(x),
                            Text = "0" + Convert.ToString(x)
                        });
                    }
                    else
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = Convert.ToString(x),
                            Text = Convert.ToString(x)
                        });
                    }
                }
                return list;
            }
        }



        [Display(Name = "Day")]
        public string DayValue { get; set; }
        public List<SelectListItem> DayList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                for (int x = 1; x < 32; x++)
                {
                    if (x < 10)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = "0" + Convert.ToString(x),
                            Text = "0" + Convert.ToString(x)
                        });
                    }
                    else
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = Convert.ToString(x),
                            Text = Convert.ToString(x)
                        });
                    }
                }
                return list;
            }
        }



        [Display(Name = "Year")]
        public string YearValue { get; set; }
        public List<SelectListItem> YearList
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                for (int x = 2017; x > 1950; x--)
                {

                    list.Add(new SelectListItem()
                    {
                        Value = Convert.ToString(x),
                        Text = Convert.ToString(x)
                    });
                }
                return list;
            }
        }

    }


    public class GuestPreferences
    {
        public string PreferenceTypeID { get; set; }

        [Display(Name = "Preference Type")]
        public int PreferenceType { get; set; }
        public int PreferenceID { get; set; }

        [Display(Name = "Preference")]
        public string PreferenceName { get; set; }

        public string GuestHubID { get; set; }

        //important dates

        public long ImportantDatesID { get; set; }
        public string DateName { get; set; }
        public string Date { get; set; }
        public Guid GuestPreferencesID { get; set; }
    }


    public class GuestReservationModel
    {
        public string ArrivalID { get; set; }

        public string Destination { get; set; }

        public int Nights { get; set; }

        public DateTime ArrivalDate { get; set; }

        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public int Adults { get; set; }
        public int Childrens { get; set; }
        public string PlanType { get; set; }
        public string Total { get; set; }

        public string AgencyName { get; set; }
        public string Source { get; set; }



    }


    public class HotelExpertFile
    {
        public int callNumber { get; set; }

        public DateTime callIndate { get; set; }
        public int statCode { get; set; }
        public string room { get; set; }
        public string guest { get; set; }
        public string problemDefinition { get; set; }
        public string receiver { get; set; }
        public string shift { get; set; }
        public int resolution { get; set; }
        public string repairman { get; set; }

        public DateTime callOutdate { get; set; }
        public string closedBy { get; set; }
        public string closedByDepartment { get; set; }
        public DateTime timerStart { get; set; }
        public string callerCategory { get; set; }

        public int problemNumber { get; set; }
        public int subProblemNumber { get; set; }
        public float compute01 { get; set; }

        public string problemDescription { get; set; }
        public string subProblemSubDesc { get; set; }
        public double compute02 { get; set; }

        public double compute024 { get; set; }


    }

    public class GuestTasksModel
    {
        public int taskID { get; set; }
        public string task { get; set; }
        public string subTask { get; set; }
        public DateTime date { get; set; }
        public string room { get; set; }

        public Guid GuestHubID { get; set; }

    }

    public class SurveysFile
    {
        public string key { get; set; }
        public DateTime dateIn { get; set; }
        public DateTime dateOut { get; set; }

        public string email { get; set; }
        public string organziationID { get; set; }
        public bool hadComment { get; set; }
        public bool hadProblem { get; set; }
        public int propertyCode { get; set; }
        public string marketCodeID { get; set; }
        public float rate { get; set; }
        public string room { get; set; }
        public DateTime sentDate { get; set; }
        public DateTime submittedDate { get; set; }
        public string surveyLenguage { get; set; }
        public string surveyID { get; set; }
        public Guid guestHubID { get; set; }

    }

    public class GuestMemberInfo
    {
        public int ContractNumber { get; set; }
        public string ResortContractNumber { get; set; }
        public int AccountNumber { get; set; }
        public string ContractStatus { get; set; }
        public string SourceOfSale { get; set; }
        public string ClubType { get; set; }


    }

    public class GuestClarabridgeInfo
    {
        int surveyAppledID { get; set; }

        DateTime dateIn { get; set; }

        DateTime dateOut { get; set; }

        string email { get; set; }
        string organizationID { get; set; }

        bool hadComment { get; set; }
        bool hadProblem { get; set; }
        int propertyCode { get; set; }
        int rate { get; set; }

        string surveyLenguage { get; set; }
        int surveyID { get; set; }


    }

    public class GuestPreferencesApi
    {
        public string GuestName { get; set; }
        public Guid GuestHubID { get; set; }
        public int ResortID { get; set; }
        public int GuestID { get; set; }
        public string Email { get; set; }
        public List<PreferencesAPI> Preferences { get; set; }
    }
    public class PreferencesAPI
    {
        public int PreferenceID { get; set; }
        public string Preference { get; set; }
        public int PreferenceTypeID { get; set; }
    }

    public class SurveysAnswers
    {
        public string Topic { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
