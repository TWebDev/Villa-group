
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
   public  class LeadStatusDataModel
    {
       ePlatEntities db = new ePlatEntities();
       public static UserSession session = new UserSession();
       public static List<SelectListItem> GetAllLeadStatus()
       {
           ePlatEntities db = new ePlatEntities();
           List<SelectListItem> list = new List<SelectListItem>();

           var leadStatus = from t in db.tblLeadStatus
                           select new { t.leadStatusID, t.leadStatus};

           foreach (var i in leadStatus)
           {
               list.Add(new SelectListItem() { Text = i.leadStatus, Value = i.leadStatusID.ToString() });
           }

           return list;
       }

       public static List<SelectListItem> GetLeadStatusByTerminal()
       {
           ePlatEntities db = new ePlatEntities();
           List<SelectListItem> list = new List<SelectListItem>();

           var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
           var leadStatus = db.tblTerminals_LeadStatus.Where(m => terminals.Contains(m.terminalID)).Select(m => new { m.leadStatusID, m.tblLeadStatus.leadStatus });
           
           foreach (var i in leadStatus)
           {
               list.Add(new SelectListItem()
               {
                   Value = i.leadStatusID.ToString(),
                   Text = i.leadStatus
               });
           }
           return list;
       }
    }
}
