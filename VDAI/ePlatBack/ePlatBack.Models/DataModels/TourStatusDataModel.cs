
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    public class TourStatusDataModel
    {
        public static UserSession session = new UserSession();
        public static List<SelectListItem> GetAlltourStatus()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from ts in db.tblTourStatus
                        select new { ts.tourStatusID, ts.tourStatus };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.tourStatusID.ToString(), Text = i.tourStatus });
            return list;
        }

        public static List<SelectListItem> GetTourStatusByCurrentWorkGroup()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var workgroup = session.WorkGroupID;
            var role = session.RoleID;

            var tourStatus = from ts in db.tblSysWorkGroups_TourStatus
                             where ts.sysWorkGroupID == workgroup
                             && ts.roleID == role
                             select new
                             {
                                 ts.tourStatusID,
                                 ts.tblTourStatus.tourStatus
                             };

            if (tourStatus.Count() == 0)
            {
                var _tourStatus = from t in db.tblSysWorkGroups_TourStatus
                                  where t.sysWorkGroupID == workgroup
                                  && t.roleID == null
                                  orderby t.orderIndex
                                  select new
                                  {
                                      t.tourStatusID,
                                      t.tblTourStatus.tourStatus
                                  };

                foreach (var i in _tourStatus)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.tourStatusID.ToString(),
                        Text = i.tourStatus
                    });
                }
            }
            else
            {
                foreach (var i in tourStatus)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.tourStatusID.ToString(),
                        Text = i.tourStatus
                    });
                }
            }

            return list;
        }
    }
}
