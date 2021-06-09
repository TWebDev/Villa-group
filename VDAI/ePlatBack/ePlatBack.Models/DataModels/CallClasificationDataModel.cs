using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    class CallClasificationDataModel
    {
        public static List<SelectListItem> GetAllCallClasifications()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from cc in db.tblCallClasifications
                        select new { cc.callClasificationID, cc.callClasification };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.callClasificationID.ToString(), Text = i.callClasification });
            return list;
        }

        public static int? GetStateIDByPhone(string phone)
        {
            ePlatEntities db = new ePlatEntities();
            int? stateID = null;
            string areaCode = string.Empty;
            phone = phone.Replace("(", "");
            phone = phone.Replace(")", "");
            phone = phone.Replace("-", "");
            phone = phone.Replace(" ", "");
            if (phone.Length >= 10)
            {
                phone = phone.Substring(phone.Length - 10);
                areaCode = phone.Substring(0, 3);

                var State = (from a in db.tblAreaCodes
                           where a.code == areaCode
                           select a.tblAreas.stateID).FirstOrDefault();
                if (State != null){
                    stateID = State;
                }                
            }
            return stateID;
        }
    }
}
