using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    public class WebhooksViewModel
    {
        public class NewLeadWH
        {
            public Guid webhookID { get; set; }
            public string referenceID { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public string destination { get; set; }
            public string timeToReach { get; set; }
            public DateTime? arrivalDate { get; set; }
            public DateTime? departureDate { get; set; }
            public string notes { get; set; }
        }

        public class TerminalCounter {
            public long TerminalID { get; set; }
            public int Count { get; set; }
            public decimal CurrentPercentage { get; set; }
            public decimal? SharedPercentage { get; set; }
            public decimal Difference { get; set; }
       }

        public class PhoneTerminal
        {
            public long TerminalID { get; set; }
            public Guid LeadID { get; set; }
        }
    }
}
