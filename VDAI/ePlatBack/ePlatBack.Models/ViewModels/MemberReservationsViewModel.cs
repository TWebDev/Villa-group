using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    public class MemberReservationsViewModel
    {
        public class Reservation
        {
            public string CRS { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int ResortID { get; set; }
            public string Resort { get; set; }
            public DateTime? Arrival { get; set; }
            public DateTime? Departure { get; set; }
            public long PlaceID { get; set; }
            public string Status { get; set; }
        }
    }
}
