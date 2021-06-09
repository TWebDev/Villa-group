using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ePlatBack.Models.ViewModels
{
    public class WorkflowViewModel
    {
        public class Workflow
        {
            public string Step { get; set; }
            public string StepDetails { get; set; }
            //public List<Workflow> Children { get; set; }
            public Workflow[] Children { get; set; }
        }

        public class WorkflowInfoModel
        {
            public long? WorkflowID { get; set; }
            public string System { get; set; }
            public long TerminalID { get; set;}
            //public string DestinationID { get; set; }
            //public string PlaceID { get; set; }
            public long?[] DestinationID { get; set; }
            public long?[] PlaceID { get; set; }
            public string SenderAddress { get; set; }
            public string SenderPassword { get; set; }
            public string ReplyTo { get; set; }
            public string DifussionWay { get; set; }
            public string DifussionWayDetails { get; set; }
            public string Terminal { get; set; }
            public string Name { get; set; }
            //public string WorkflowJson { get; set; }
            public WorkflowModel[] WorkflowJson { get; set; }
        }

        public class WorkflowModel
        {
            public string Action { get; set; }
            public string DelayDays { get; set; }
            public string DelayHours { get; set; }
            public string Email { get; set; }
            public string ParentStep { get; set; }
            public string Step { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}
