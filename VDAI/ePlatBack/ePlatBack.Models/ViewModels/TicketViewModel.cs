using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using System.ComponentModel.DataAnnotations;
using ePlatBack.Models.Utils.Custom.Attributes;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.ViewModels
{
    public class TicketViewModel
    {
        public List<TicketItem> Tickets { get; set; }
        public TicketItem TicketInfo { get; set; }

        public Search_Ticket SearchTicketViewModel { get; set; }

        public class TicketItem
        {
            public Guid supportTicketID { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
            public long? terminalID { get; set; }
            public string reference { get; set; }
            public Guid? assignedToUserID { get; set; }
            public string changeset { get; set; }
            public int order { get; set; }

            public string Terminal { get; set; }
            public string RequestedByUser { get; set; }
            public string AssignedToUser { get; set; }
            public int SupportTicketStatusID { get; set; }
            public string SupportTicketStatus { get; set; }
            public string StatusComment { get; set; }
            public List<TicketStatus> StatusHistory { get; set; }
            public List<SelectListItem> SelectedTerminals { get; set; }
            public List<SelectListItem> AdministratorUsers { get; set; }
            public List<SelectListItem> TicketStatusList { get; set; }
        }

        public class TicketStatus
        {
            public string SupportTicketStatus { get; set; }
            public string Date { get; set; }
            public string SavedByUser { get; set; }
            public string Comments { get; set; }
        }

        public class Search_Ticket
        {
            [Display(Name = "Requested by")]
            public Guid? Search_CreatedByUserID { get; set; }
            [Display(Name = "Assigned to")]
            public Guid? Search_AssignedToUserID { get; set; }
            [Display(Name = "Status")]
            public int[] Search_SupportTicketStatusID { get; set; }
            public List<SelectListItem> Search_TicketStatus
            {
                get
                {
                    return TicketDataModel.TicketCatalogs.FillDrpTicketStatus();
                }
            }
            public List<SelectListItem> Search_Users
            {
                get
                {
                    List<SelectListItem> users = UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup();
                    users.Insert(0, Utils.ListItems.Default("Any", ""));
                    return users;
                }
            }

            public List<SelectListItem> Search_AdministratorUsers
            {
                get
                {
                    List<SelectListItem> users = UserDataModel.UserCatalogs.FillDrpAdministratorUsersInWorkGroup();
                    users.Insert(0, Utils.ListItems.Default("Any", ""));
                    return users;
                }
            }
        }

        public class TicketUpdateItem
        {
            public Guid SupportTicketID { get; set; }
            public int Order { get; set; }
            public int[] Status { get; set; }
        }
    }
}
