using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace ePlatBack.Models.DataModels
{
    public class OpcDataModel
    {
        public static UserSession session = new UserSession();
        public static List<SelectListItem> GetLocations()
        {
            return GetLocations(null);
        }
        
        public static List<SelectListItem> GetLocations(long? terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            List<long> terminals = new List<long>();
            if (terminalID == null)
            {
                terminals = session.Terminals != "" ?
                    session.Terminals.Split(',').Select(m => long.Parse(m)).ToList() :
                    session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
            }
            else
            {
                terminals.Add((long)terminalID);
            }           
            
            var query = from l in db.tblLocations
                        where terminals.Contains(l.terminalID)
                        orderby l.location
                        select new
                        {
                            l.locationID,
                            l.location
                        };

            foreach (var i in query)
            {
                list.Add(new SelectListItem() { Value = i.locationID.ToString(), Text = i.location });
            }

            return list;
        }

        public static List<SelectListItem> GetPromotionTeams()
        {
            return GetPromotionTeams(null, null);
        }

        public static List<SelectListItem> GetPromotionTeams(long? terminalID)
        {
            return GetPromotionTeams(terminalID, null);
        }

        public static List<SelectListItem> GetPromotionTeams(long? terminalID, int? companyID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            List<long> terminals = new List<long>();
            if (terminalID == null)
            {
                terminals = session.Terminals != "" ?
                    session.Terminals.Split(',').Select(m => long.Parse(m)).ToList() :
                    session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
            }
            else
            {
                terminals.Add((long)terminalID);
            }               

            int?[] companies = (companyID == null ? db.tblTerminals_Companies.Where(x => terminals.Contains(x.terminalID)).Select(x => (int?)x.companyID).ToArray() : new int?[1] { (int?)companyID });
            var query = from o in db.tblPromotionTeams
                        where companies.Contains(o.companyID) || o.companyID == null
                        orderby o.promotionTeam
                        select new { o.promotionTeamID, o.promotionTeam };
            foreach (var i in query)
            {
                list.Add(new SelectListItem() { Value = i.promotionTeamID.ToString(), Text = i.promotionTeam });
            }

            return list;
        }

        public static List<SelectListItem> GetCompanies()
        {
            return GetCompanies(null);
        }

        public static List<SelectListItem> GetCompanies(long? terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            long[] currentTerminals = (terminalID == null ? (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray() : new long[1] { (long)terminalID });
            var query = from c in db.tblTerminals_Companies
                        where currentTerminals.Contains(c.terminalID)
                        orderby c.tblCompanies.company
                        select new 
                        {
                            companyID = c.companyID,
                            company = c.tblCompanies.company
                        };

            foreach (var i in query)
            {
                list.Add(new SelectListItem() { Value = i.companyID.ToString(), Text = i.company });
            }

            return list;
        }

        public static List<SelectListItem> GetChargableCompanies(long? terminalID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            long[] currentTerminals = (terminalID == null ? (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray() : new long[1] { (long)terminalID });
            var query = from c in db.tblTerminals_Companies
                        where currentTerminals.Contains(c.terminalID)
                        && c.tblCompanies.companyTypeID != 1
                        orderby c.tblCompanies.company
                        select new
                        {
                            companyID = c.companyID,
                            company = c.tblCompanies.company
                        };

            list.Add(new SelectListItem() { Value = "", Text = "General Public" });
            foreach (var i in query)
            {
                list.Add(new SelectListItem() { Value = i.companyID.ToString(), Text = i.company });
            }

            return list;
        }

        public static List<SelectListItem> GetAllOPCs()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from o in db.tblOPCS
                        orderby o.opc
                        select new { o.opcID, o.opc};
            foreach (var i in query)
            {
                list.Add(new SelectListItem() { Value = i.opcID.ToString(), Text = i.opc });
            }
            return list;
        }

        public static List<SelectListItem> GetActiveOPCs()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var destinations = db.tblTerminals_Destinations.Where(d => terminals.Contains(d.terminalID)).Select(d => d.destinationID).ToArray();

            var query = (from o in db.tblOPC_PromotionTeams
                        where o.deleted != true
                        && destinations.Contains(o.tblPromotionTeams.destinationID)
                        orderby o.tblOPCS.opc
                        select new { o.opcID, o.tblOPCS.opc }).Distinct();
            foreach (var i in query.OrderBy(x => x.opc))
            {
                list.Add(new SelectListItem() { Value = i.opcID.ToString(), Text = i.opc });
            }

            return list;
        }
        /// <summary>
        /// get all opcs based on terminal(s) company(ies)
        /// </summary>
        /// <param name="terminalID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public static List<SelectListItem> FillDrpOPCsPerCompany(long? terminalID, int? companyID)
        //public static List<SelectListItem> FillDrpOPC(long? terminalID, int? companyID, int? promotionTeamID)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            long[] currentTerminals = (terminalID == null ? (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray() : new long[1] { (long)terminalID });
            int?[] companies = (companyID == null ? db.tblTerminals_Companies.Where(x => currentTerminals.Contains(x.terminalID)).Select(x => (int?)x.companyID).ToArray() : new int?[1] { (int?)companyID });

            var opcsQ = (from o in db.tblOPCS
                         where companies.Contains(o.companyID)
                         select new
                         {
                             o.opcID,
                             o.firstName,
                             o.lastName,
                             o.secondSurname,
                             o.middleName,
                             activeTeams = o.tblOPC_PromotionTeams.Count(x => x.deleted != true && x.terminateDate == null)
                         }).Distinct();

            foreach (var i in opcsQ)
            {
                var opc = (i.activeTeams == 0 ? " INACTIVE - " : "") + i.lastName + " " + i.secondSurname + ", " + i.firstName + " " + i.middleName;
                list.Add(new SelectListItem() { Value = i.opcID.ToString(), Text = opc });
            }

            list = list.OrderBy(m => m.Text).ToList();

            List<SelectListItem> orderedList = new List<SelectListItem>();
            List<SelectListItem> inactiveList = new List<SelectListItem>();
            foreach (var l in list)
            {
                if (l.Text.IndexOf("INACTIV") >= 0)
                {
                    inactiveList.Add(l);
                }
                else
                {
                    orderedList.Add(l);
                }

            }

            var newList = orderedList.Union(inactiveList);

            return newList.ToList();
        }
    }
}
