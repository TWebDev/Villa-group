
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    public class BookingStatusDataModel
    {
        public static UserSession session = new UserSession();
        public static List<SelectListItem> GetAllBookingStatus()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from bs in db.tblBookingStatus
                        select new { bs.bookingStatusID, bs.bookingStatus };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.bookingStatusID.ToString(), Text = i.bookingStatus });
            return list;
        }

        public static List<SelectListItem> GetBookingStatusByCurrentWorkGroup()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var workgroup = session.WorkGroupID;
            var role = session.RoleID;

            var bookingStatus = from bs in db.tblSysWorkGroups_BookingStatus
                                join bt in db.tblTerminals_BookingStatus on bs.bookingStatusID equals bt.bookingStatusID into bs_bt
                                from bt in bs_bt.DefaultIfEmpty()
                                where terminals.Contains(bt.terminalID)
                                && bs.sysWorkGroupID == workgroup
                                && bt.isPrimary == true
                                select new
                                {
                                    bs.bookingStatusID,
                                    bs.tblBookingStatus.bookingStatus,
                                    bs.roleID,
                                    bs.orderIndex
                                };

            if(bookingStatus.Select(m => m.roleID).Contains(role))
            {
                bookingStatus = bookingStatus.Where(m => m.roleID == role);
            }
            else
            {
                bookingStatus = bookingStatus.Where(m => m.roleID == (Guid?)null);
            }

            foreach (var i in bookingStatus.Distinct().OrderBy(x => x.orderIndex))
            {
                list.Add(new SelectListItem()
                {
                    Value = i.bookingStatusID.ToString(),
                    Text = i.bookingStatus
                });
            }

            return list;
        }

        public static List<SelectListItem> GetBookingStatusByCurrentWorkGroup(int? workgroupID)//1,0
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var role = session.RoleID;

            var bookingStatus = from bs in db.tblSysWorkGroups_BookingStatus
                                join bt in db.tblTerminals_BookingStatus on bs.bookingStatusID equals bt.bookingStatusID into bs_bt
                                from bt in bs_bt.DefaultIfEmpty()
                                where terminals.Contains(bt.terminalID)
                                && bs.sysWorkGroupID == workgroupID
                                && bt.isPrimary == true
                                && bt.isFinal == false
                                select new
                                {
                                    bs.bookingStatusID,
                                    bs.tblBookingStatus.bookingStatus,
                                    bs.roleID
                                };

            if (bookingStatus.Select(m => m.roleID).Contains(role))
            {
                bookingStatus = bookingStatus.Where(m => m.roleID == role);
            }
            else
            {
                bookingStatus = bookingStatus.Where(m => m.roleID == (Guid?)null);
            }

            foreach (var i in bookingStatus)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.bookingStatusID.ToString(),
                    Text = i.bookingStatus
                });
            }
            return list;
        }

        public static List<SelectListItem> GetSecondaryBookingStatus()//0,0
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var workgroup = session.WorkGroupID;
            var role = session.RoleID;

            var query = from bs in db.tblSysWorkGroups_BookingStatus
                        join bt in db.tblTerminals_BookingStatus on bs.bookingStatusID equals bt.bookingStatusID into bs_bt
                        from bt in bs_bt.DefaultIfEmpty()
                        where terminals.Contains(bt.terminalID)
                        && bs.sysWorkGroupID == workgroup
                        && bt.isPrimary == false
                        && bt.isFinal == false
                        orderby bs.orderIndex
                        select new
                        {
                            bs.bookingStatusID,
                            bs.tblBookingStatus.bookingStatus,
                            bs.roleID,
                            bs.orderIndex
                        };

            if(query.Select(m => m.roleID).Contains(role))
            {
                query = query.Where(m => m.roleID == role);
            }
            else
            {
                query = query.Where(m => m.roleID == (Guid?)null);
            }
            query = query.Distinct();
            foreach (var i in query.OrderBy(m => m.orderIndex))
            {
                list.Add(new SelectListItem()
                {
                    Value = i.bookingStatusID.ToString(),
                    Text = i.bookingStatus
                });
            }

            return list;
        }

        public static List<SelectListItem> GetFinalBookingStatus()//0,1
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var workgroup = session.WorkGroupID;
            var role = session.RoleID;

            var query = from bs in db.tblSysWorkGroups_BookingStatus
                        join bt in db.tblTerminals_BookingStatus on bs.bookingStatusID equals bt.bookingStatusID into bs_bt
                        from bt in bs_bt.DefaultIfEmpty()
                        where terminals.Contains(bt.terminalID)
                        && bs.sysWorkGroupID == workgroup
                        && bt.isPrimary == false
                        && bt.isFinal == true
                        orderby bs.orderIndex
                        select new
                        {
                            bs.bookingStatusID,
                            bs.tblBookingStatus.bookingStatus,
                            bs.roleID,
                            bs.orderIndex
                        };

            if (query.Select(m => m.roleID).Contains(role))
            {
                query = query.Where(m => m.roleID == role);
            }
            else
            {
                query = query.Where(m => m.roleID == (Guid?)null);
            }
            query = query.Distinct();
            foreach (var i in query.OrderBy(m => m.orderIndex))
            {
                list.Add(new SelectListItem()
                {
                    Value = i.bookingStatusID.ToString(),
                    Text = i.bookingStatus
                });
            }

            return list;
        }
    }
}
