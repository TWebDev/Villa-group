
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    public class TimeZoneDataModel
    {
        public static List<SelectListItem> GetAllTimeZones()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from tz in db.tblTimeZones
                        select new { tz.timeZoneID, tz.timeZone };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.timeZoneID.ToString(), Text = i.timeZone });
            return list;
        }
    }
}
