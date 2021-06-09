using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
   public class BrokerDataModel
    {
       public static List<SelectListItem> GetAllContracts()
       {
           ePlatEntities db = new ePlatEntities();
           List<SelectListItem> list = new List<SelectListItem>();
           var query = from c in db.tblBrokerContracts
                       select new { c.brokerContractID, c.brokerContractAbbr};
           foreach (var i in query)
               list.Add(new SelectListItem() { Value = i.brokerContractID.ToString(), Text = i.brokerContractAbbr });
           return list;
       }

       public static List<SelectListItem> GetBrokerContractsByTerminal(long? terminalID = null)
       {
           ePlatEntities db = new ePlatEntities();
           var _terminals = terminalID != null ? new long[] { (long)terminalID } : new UserSession().Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
           List<SelectListItem> list = new List<SelectListItem>();

           var query = from c in db.tblBrokerContracts_Terminals
                       where _terminals.Contains(c.terminalID)
                       select new { c.tblBrokerContracts.brokerContractID, c.tblBrokerContracts.brokerContractAbbr };
           foreach (var i in query)
               list.Add(new SelectListItem() { Value = i.brokerContractID.ToString(), Text = i.brokerContractAbbr });
           return list;
       }

    }
}
