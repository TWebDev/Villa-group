
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    public class RoomDataModel
    {
        public static List<SelectListItem> GetAllRoomTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from rt in db.tblRoomTypes
                        select new {rt.roomTypeID, rt.roomType};
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.roomTypeID.ToString(), Text = i.roomType });
            return list;
        }
    }
}
