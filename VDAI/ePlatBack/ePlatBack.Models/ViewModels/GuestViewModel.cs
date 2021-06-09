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
    public class GuestViewModel
    {
        public class Guest
        {
            public class GuestInfo {
                public Guid? GuestHubID { get; set; }
                public long? GuestID { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string Email { get; set; }
                public string RK { get; set; }
                public string ArrivalDate { get; set; }
            }
        }

        public class GuestsByPreference {
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public int ResortID { get; set; }
            public string PreferenceIDs { get; set; }
            public List<Preferences.PreferenceName> Preferences { get; set; }
            public List<Guest.GuestInfo> Guests { get; set; }
        }

        public class Preferences
        {
            public class PreferenceTypeGuest {
                public int PreferenceTypeID { get; set; }
                public string PreferenceTypeName { get; set; }
                public List<PreferenceItemValue> Preferences { get; set; }
            }

            public class PreferenceItemValue
            {
                public int PreferenceID { get; set; }
                public string Preference { get; set; }
                public string Value { get; set; }
            }

            public class PreferenceType
            {
                public int PreferenceTypeID { get; set; }
                public string PreferenceTypeName { get; set; }
                public string PreferenceTypeShortName { get; set; }
                public string Description { get; set; }
                public string Observations { get; set; }
                public List<PreferenceItem> Preferences { get; set; }
            }

            public class PreferenceItem
            {
                public int PreferenceID { get; set; }
                public string Preference { get; set; }
                public string PreferenceShortName { get; set; }
                public string Description { get; set; }
                public bool ShowCheckbox { get; set; }
                public bool ShowTextbox { get; set; }
            }

            public class PreferenceName
            {
                public int PreferenceID { get; set; }
                public string Preference { get; set; }
            }

            public class ImportantDate
            {
                public string Name { get; set; }
                public DateTime Date { get; set; }
            }
        }

        public class PreferencesResponse
        {
            public Guid GuestHubID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int[] PreferencesIDs { get; set; }
            public List<Preferences.PreferenceTypeGuest> Preferences { get; set; }
            public LastReservation Reservation { get; set; }
        }

        public class LastReservation
        {
            public long ReservationID { get; set; }
            public string Resort { get; set; }
            public string CRS { get; set; }
            public DateTime ArrivalDate { get; set; }
            public int Nights { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
        }

        public class PreferencesModel
        {
            
            public string PreferencesIDs { get; set; }
            public string OpenPreferences { get; set; }
            public string Source { get; set; }
        }

        public class OpenPreference
        {
            public int PreferenceID { get; set; }
            public string Value { get; set; }
        }

        public class GuestHubIDJSONResponse
        {
            public Guid GuestHubID { get; set; }
        }
    }
}
