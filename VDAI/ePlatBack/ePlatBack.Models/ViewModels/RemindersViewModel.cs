using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    public class RemindersViewModel
    {
        public class SearchRemindersModel
        {
            public List<Reminder> Result { get; set; }
        }
        public class Reminder
        {
            public int? reminderID { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public DateTime dateAlarm { get; set; }
            public Guid? forUserID { get; set; }
            public DateTime dateSaved { get; set; }
            public Guid savedByUserID { get; set; }
            public DateTime? lastModificationDate { get; set; }
            public Guid? modifiedByUserID { get; set; }
            public string typeID { get; set; }
            public string type { get; set; }
            public daysAndMonths repeat { get; set; }
            public int[] numberDays { get; set; }
            public string numberDayString { get; set; }
        }
        public class daysAndMonths {
            public Weekly weekly { get; set; }
            public Montly montly { get; set; }
        }
        public class Weekly
        {
            public bool? Monday { get; set; }
            public bool? Tuesday { get; set; }
            public bool? Wednesday { get; set; }
            public bool? Thursday { get; set; }
            public bool? Friday { get; set; }
            public bool? Saturday { get; set; }
            public bool? Sunday { get; set; }
        }
        public class Montly
        {
            public bool? January { get; set; }
            public bool? February { get; set; }
            public bool? March { get; set; }
            public bool? April { get; set; }
            public bool? May { get; set; }
            public bool? June { get; set; }
            public bool? July { get; set; }
            public bool? August { get; set; }
            public bool? September { get; set; }
            public bool? October { get; set; }
            public bool? November { get; set; }
            public bool? December { get; set; }
        }
    }
}
