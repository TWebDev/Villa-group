using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    class CreditCardsDataModel
    {
        public static List<SelectListItem> GetAllCreditCardTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from c in db.tblCardTypes
                        select new { c.cardTypeID, c.cardType };
            foreach (var i in query)
            {
                if (!(i.cardTypeID == 2 && Utils.GeneralFunctions.GetCulture() == "es-MX")) {
                    list.Add(new SelectListItem() { Value = i.cardTypeID.ToString(), Text = i.cardType });
                }
            }
                
            return list;
        }

    }
}
