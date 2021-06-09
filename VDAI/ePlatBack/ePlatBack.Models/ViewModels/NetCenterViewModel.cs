using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ePlatBack.Models.ViewModels
{
    public class NetCenterViewModel
    {
        public class CallLogEvent
        {
            public class CallStarted
            {
                public string agi_channel { get; set; }
                public string agi_uniqueid { get; set; }
                public string agi_callerid { get; set; }
                public string agi_dnid { get; set; }
                public string agi_extension { get; set; }
                public string agi_accountcode { get; set; }
            }

            public class CallFinished
            {
                public string uniqueid { get; set; }
                public string answer { get; set; }
                public string source { get; set; }
                public string destination { get; set; }
                public int billsec { get; set; }
                public string dstchannel { get; set; }
                public string disposition { get; set; }
                public string accountcode { get; set; }
                public string channel { get; set; }
            }
        }
    }
}
