
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;



namespace ePlatBack.Models.DataModels
{
    public class LeadSourceDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();
        public static List<SelectListItem> GetAllLeadSources()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();

            var leadSources = from x in db.tblLeadSources
                              select new { x.leadSourceID, x.leadSource };

            foreach (var i in leadSources)
            {
                list.Add(new SelectListItem() { Text = i.leadSource, Value = i.leadSourceID.ToString() });
            }
            return list;
        }

        //public static List<SelectListItem> GetLeadSourcesByWorkGroup(int workgroupID)
        public static List<SelectListItem> GetLeadSourcesByWorkGroup(ePlatEntities dataContext = null)
        {
            ePlatEntities db = dataContext ?? new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();

            var workgroup = session.WorkGroupID;
            var role = session.RoleID;

            var ls = from ts in db.tblLeadSources_SysWorkGroups
                             where ts.sysWorkGroupID == workgroup
                             && ts.roleID == role
                             select new
                             {
                                 ts.leadSourceID,
                                 ts.tblLeadSources.leadSource
                             };

            if (ls.Count() == 0)
            {
                var _ls = from t in db.tblLeadSources_SysWorkGroups
                                  where t.sysWorkGroupID == workgroup
                                  && t.roleID == null
                                  orderby t.order_
                                  select new
                                  {
                                      t.leadSourceID,
                                      t.tblLeadSources.leadSource
                                  };

                foreach (var i in _ls)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.leadSourceID.ToString(),
                        Text = i.leadSource
                    });
                }
            }
            else
            {
                foreach (var i in ls)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.leadSourceID.ToString(),
                        Text = i.leadSource
                    });
                }
            }

            //var leadSources = db.tblLeadSources_SysWorkGroups.Where(m => m.sysWorkGroupID == session.WorkGroupID && m.tblLeadSources.active).Select(m => new { m.tblLeadSources.leadSourceID, m.tblLeadSources.leadSource, m.order_ });
            //var _leadSources = leadSources.Where(m => m.order_ != 0).OrderBy(m => m.order_).ToList();
            
            //_leadSources.AddRange(leadSources.Where(m => (m.order_ == 0 || m.order_ == null)).OrderBy(m => m.leadSource).ToList());
            
            //foreach (var i in _leadSources)
            //{
            //    list.Add(new SelectListItem() { Text = i.leadSource, Value = i.leadSourceID.ToString() });
            //}
            return list;
        }
        /// <summary>
        /// Get leadsources filtered by terminal and workgroup
        /// </summary>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetLeadSourcesByTerminal(ePlatEntities dataContext = null)
        {
            ePlatEntities db = dataContext ?? new ePlatEntities();

            var _terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var _leadSources = GetLeadSourcesByWorkGroup();
            var _lsByTerminal = db.tblTerminals_LeadSources.Where(m => _terminals.Contains(m.terminalID) && m.tblLeadSources.active).Select(m => m.leadSourceID).ToList();

            _leadSources = _leadSources.Where(m => _lsByTerminal.Contains(long.Parse(m.Value))).ToList();

            return _leadSources;
        }
    }
}
