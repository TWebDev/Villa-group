using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    class GreetingRepDataModel
    {
        public static List<SelectListItem> GetAllGreetingReps()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from gr in db.tblGreetingReps
                        select new { gr.greetingRepID, gr.greetingRep };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.greetingRepID.ToString(), Text = i.greetingRep });
            return list;
        }

    }
}
