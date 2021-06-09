using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ePlatBack.Models.ViewModels
{
    public class FrequentGuestsViewModel
    {
        public class SearchGuests
        {
            [Display(Name = "Resort")]
            public int Search_ResortID { get; set; }
            public List<SelectListItem> Search_Resorts { get; set; }
            [Display(Name = "Arrival Date")]
            public string Search_I_FromDate { get; set; }
            public string Search_F_ToDate { get; set; }
        }

        public class FrequentGuestsResult
        {
            public List<FrequentGuest> Guests { get; set; }
            public string Resort { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
        }

        public class FrequentGuest
        {
            public decimal? FrontOfficeGuestID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Stays { get; set; }
            public List<ReservationItem> Reservations { get; set; }
        }

        public class ReservationItem
        {
            public string Resort { get; set; }
            public string Agency { get; set; }
            public string Confirmation { get; set; }
            public string Arrival { get; set; }
            public int? Nights { get; set; }
            public string RoomType { get; set; }
            public string Currency { get; set; }
            public decimal? Spa { get; set; }
            public decimal? PoS { get; set; }
            public decimal? Stay { get; set; }
            public decimal? Others { get; set; }
            public decimal? Total { get; set; }
        }
    }
}
