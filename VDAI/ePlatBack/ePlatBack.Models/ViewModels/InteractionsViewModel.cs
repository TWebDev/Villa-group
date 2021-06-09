using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    public class InteractionsViewModel
    {
        public class Interaction
        {
            public long? InteractionID { get; set; }
            public Guid? InteractionLeadID { get; set; }
            public Guid? InteractionTrialID { get; set; }
            public int? InteractionBookingStatusID { get; set; }
            public string InteractionBookingStatus { get; set; }
            public int InteractionTypeID { get; set; }
            public string InteractionType { get; set; }
            public string InteractionComments { get; set; }
            public int? InteractionInterestLevelID { get; set; }
            public string InteractionInterestLevel { get; set; }
            public Guid? InteractedWithUserID { get; set; }
            public string InteractedWithUser { get; set; }
            public string InteractionSavedByUser { get; set; }
            public long? ParentInteractionID { get; set; }
            public DateTime InteractionDate { get; set; }
            public List<Interaction> InteractionNotes { get; set; }
            public bool Editing { get; set; }
            public bool ShowNotes { get; set; }
            public decimal? TotalSold { get; set; }
        }
    }
}
