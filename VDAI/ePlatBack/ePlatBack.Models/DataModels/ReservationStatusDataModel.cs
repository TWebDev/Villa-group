using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    public class ReservationStatusDataModel
    {
        public static List<SelectListItem> GetallReservationStatus()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            foreach (var i in db.tblReservationStatus)
                list.Add(new SelectListItem() { Value = i.reservationStatusID.ToString(), Text = i.reservationStatus });
            return list;
        }
    }
}
