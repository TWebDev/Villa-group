using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.Web.Security;
using System.Text.RegularExpressions;
using ePlatBack.Models.Utils;

namespace ePlatBack.Models.ViewModels
{
    public class EmailsViewModel
    {
        public List<EmailInfoModel> SearchResults { get; set; }
        public List<EmailNotificationInfoModel> SearchNotificationsResults { get; set; }
        public List<FieldGroupsInfoModel> SearchFieldGroupsResults { get; set; }
        public EmailsSearchModel EmailsSearchModel { get; set; }
        public EmailInfoModel EmailInfoModel { get; set; }
        public EmailNotificationsSearchModel EmailNotificationsSearchModel { get; set; }
        public EmailNotificationInfoModel EmailNotificationInfoModel { get; set; }
        public FieldGroupsSearchModel FieldGroupsSearchModel { get; set; }
        public FieldGroupsInfoModel FieldGroupsInfoModel { get; set; }
        public List<FieldsInfoModel> SearchFieldsResults { get; set; }
        public List<SysComponentsPrivilegesModel> Privileges
        {
            get
            {
                return AdminDataModel.GetViewPrivileges(11293);
            }
        }
    }

    public class EmailsSearchModel
    {
        [Display(Name = "Terminal(s)")]
        public long[] EmailSearch_Terminals { get; set; }
        public List<SelectListItem> EmailSearch_DrpTerminals
        {
            get
            {
                return TerminalDataModel.GetActiveTerminalsList();
            }
        }

        [Display(Name = "Subject")]
        public string EmailSearch_Subject { get; set; }

        [Display(Name = "Sender")]
        public string EmailSearch_Sender { get; set; }

        [Display(Name = "Language")]
        public string[] EmailSearch_Culture { get; set; }
        public List<SelectListItem> EmailSearch_DrpCultures
        {
            get
            {
                return TerminalDataModel.TerminalsCatalogs.FillDrpCultures();
            }
        }
    }

    public class EmailInfoModel
    {
        public long? EmailInfo_EmailID { get; set; }

        [Display(Name = "Terminal(s)")]
        public long[] EmailInfo_Terminals { get; set; }
        public string EmailInfo_TerminalsText { get; set; }
        public List<SelectListItem> EmailInfo_DrpTerminals
        {
            get
            {
                return TerminalDataModel.GetActiveTerminalsList();
            }
        }

        [Display(Name = "Language")]
        public string EmailInfo_Culture { get; set; }
        public List<SelectListItem> EmailInfo_DrpCultures
        {
            get
            {
                var list = TerminalDataModel.TerminalsCatalogs.FillDrpCultures();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Subject")]
        public string EmailInfo_Subject { get; set; }

        [AllowHtml]
        [Display(Name = "Content")]
        public string EmailInfo_Content { get; set; }

        [Display(Name = "Template")]
        public int EmailInfo_EmailTemplate { get; set; }
        public string EmailInfo_EmailTemplateText { get; set; }
        public List<SelectListItem> EmailInfo_DrpEmailTemplates
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpEmailTemplatesByCulture("0");
                list.Insert(0, ListItems.Default("Select Language"));
                return list;
            }
        }

        [Display(Name = "Sender Email")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email Address is invalid.")]
        public string EmailInfo_Sender { get; set; }

        [Display(Name = "Sender Alias")]
        public string EmailInfo_Alias { get; set; }

        [Display(Name = "Description")]
        public string EmailInfo_Description { get; set; }
    }

    public class EmailNotificationsSearchModel
    {
        [Display(Name = "Terminal")]
        public long?[] EmailNotificationsSearch_Terminal { get; set; }
        public List<SelectListItem> EmailNotificationsSearch_DrpTerminals
        {
            get
            {
                return TerminalDataModel.GetActiveTerminalsList();
            }
        }

        [Display(Name = "Destination")]
        public long?[] EmailNotificationsSearch_Destination { get; set; }
        public List<SelectListItem> EmailNotificationsSearch_DrpDestinations
        {
            get
            {
                return PlaceDataModel.GetDestinationsByCurrentTerminals();
            }
        }

        [Display(Name = "Resort")]
        public long?[] EmailNotificationsSearch_Resort { get; set; }
        public List<SelectListItem> EmailNotificationsSearch_DrpResorts
        {
            get
            {
                return PlaceDataModel.GetResortsByDestinations();
                //return PlaceDataModel.GetPlacesByDestinationsPerTerminals();
            }
        }
    }

    public class EmailNotificationInfoModel
    {
        public int? EmailNotificationInfo_EmailNotificationID { get; set; }

        [Display(Name = "Email")]
        public long EmailNotificationInfo_Email { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpEmails
        {
            get
            {
                return EmailsDataModel.EmailsCatalogs.FillDrpEmailsByTerminals(0);
            }
        }
        public string EmailNotificationInfo_EmailSubject { get; set; }

        [Display(Name = "Terminal")]
        public long? EmailNotificationInfo_Terminal { get; set; }
        public string EmailNotificationInfo_TerminalText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpTerminals
        {
            get
            {
                var list = TerminalDataModel.GetActiveTerminalsList();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Event")]
        public int EmailNotificationInfo_Event { get; set; }
        public string EmailNotificationInfo_EventText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpEvents
        {
            get
            {
                return EmailsDataModel.EmailsCatalogs.FillDrpSysEventsPerTerminals(0);
            }
        }

        [Display(Name = "Reply To")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email Address is invalid.")]
        public string EmailNotificationInfo_ReplyTo { get; set; }

        [Display(Name = "BCC Email Accounts")]
        public string EmailNotificationInfo_EmailAccounts { get; set; }

        [Display(Name = "Alternate Recipients")]
        public string EmailNotificationInfo_AlternateAddresses { get; set; }

        [Display(Name = "Destination(s)")]
        public long[] EmailNotificationInfo_Destinations { get; set; }
        public string EmailNotificationInfo_DestinationText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpDestinations
        {
            get
            {
                return PlaceDataModel.GetDestinationsByCurrentTerminals();
            }
        }

        [Display(Name = "CC to Client")]
        public bool EmailNotificationInfo_CopyLead { get; set; }
        public string EmailNotificationInfo_CopyLeadText { get; set; }

        [Display(Name = "CC to Sender")]
        public bool EmailNotificationInfo_CopySender { get; set; }

        [Display(Name = "Required Fields")]
        public string[] EmailNotificationInfo_RequiredFields { get; set; }
        public string EmailNotificationInfo_RequiredFieldsText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpRequiredFields
        {
            get
            {
                return EmailsDataModel.EmailsCatalogs.FillDrpRequiredFields();
            }
        }

        [Display(Name = "Lead Source(s)")]
        public long[] EmailNotificationInfo_LeadSources { get; set; }
        public string EmailNotificationInfo_LeadSourcesText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpLeadSources
        {
            get
            {
                return LeadSourceDataModel.GetLeadSourcesByTerminal();
            }
        }

        [Display(Name = "Point(s) Of Sale")]
        public int[] EmailNotificationInfo_PointsOfSale { get; set; }
        public string EmailNotificationInfo_PointsOfSaleText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpPointsOfSale
        {
            get
            {
                return MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
            }
        }

        [Display(Name = "Lead Status(es)")]
        public int[] EmailNotificationInfo_LeadStatus { get; set; }
        public string EmailNotificationInfo_LeadStatusText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpLeadStatus
        {
            get
            {
                return LeadStatusDataModel.GetLeadStatusByTerminal();
            }
        }

        [Display(Name = "Booking Status(es)")]
        public int[] EmailNotificationInfo_BookingStatus { get; set; }
        public string EmailNotificationInfo_BookingStatusText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpBookingStatus
        {
            get
            {
                return BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
            }
        }

        [Display(Name = "Pre Booking Status(es)")]
        public int[] EmailNotificationInfo_PreBookingStatus { get; set; }
        public string EmailNotificationInfo_PreBookingStatusText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpPreBookingStatus
        {
            get
            {
                return BookingStatusDataModel.GetSecondaryBookingStatus();
            }
        }

        [Display(Name = "Broker Contract(s)")]
        public int[] EmailNotificationInfo_BrokerContracts { get; set; }
        public string EmailNotificationInfo_BrokerContractsText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpBrokerContracts
        {
            get
            {
                return BrokerDataModel.GetBrokerContractsByTerminal();
            }
        }

        [Display(Name = "Resort(s)")]
        public long[] EmailNotificationInfo_Resorts { get; set; }
        public string EmailNotificationInfo_ResortsText { get; set; }
        public List<SelectListItem> EmailNotificationInfo_DrpResorts
        {
            get
            {
                return PlaceDataModel.GetResortsByDestinations(null, true);
            }
        }

        [Display(Name = "Description")]
        public string EmailNotificationInfo_Description { get; set; }

        [Display(Name = "Is Active")]
        public bool EmailNotificationInfo_IsActive { get; set; }

        //public EmailNotificationInfoModel()
        //{
        //    EmailNotificationInfo_IsActive = true;
        //}
    }

    public class FieldGroupsSearchModel
    {
        [Display(Name = "Field Group")]
        public string FieldGroupsSearch_FieldGroup { get; set; }

        [Display(Name = "Saved By User")]
        public Guid?[] FieldGroupsSearch_SavedByUser { get; set; }
        public List<SelectListItem> FieldGroupsSearch_DrpUsers
        {
            get
            {

                return new List<SelectListItem>();
            }
        }
    }

    public class FieldGroupsInfoModel
    {
        public int? FieldGroupsInfo_FieldGroupID { get; set; }

        [Display(Name = "Group Name")]
        public string FieldGroupsInfo_FieldGroup { get; set; }

        public Guid? FieldGroupsInfo_FieldGroupGUID { get; set; }

        [Display(Name = "Description")]
        public string FieldGroupsInfo_Description { get; set; }

        [Display(Name = "Logo")]
        public string FieldGroupsInfo_Logo { get; set; }

        public string FieldGroupsInfo_SavedByUser { get; set; }
        public string FieldGroupsInfo_DateSaved { get; set; }

        [Display(Name = "Terminal")]
        public string FieldGroupInfo_Terminal { get; set; }

        [Display(Name = "Email Notification")]
        public int FieldGroupsInfo_EmailNotification { get; set; }
        public List<SelectListItem> FieldGroupsInfo_DrpEmailNotifications
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpEmailNotifications();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }
        public List<FieldsInfoModel> FieldsInGroup { get; set; }

        public FieldsInfoModel FieldsInfoModel { get; set; }
    }

    public class FieldsInfoModel
    {
        public long? FieldsInfo_FieldID { get; set; }

        [Display(Name = "Field Guid")]
        public Guid? FieldsInfo_FieldGuid { get; set; }
        public string FieldsInfo_FieldGuidText { get; set; }
        
        [Display(Name = "Field")]
        public string FieldsInfo_Field { get; set; }
        public string FieldsInfo_FieldValue { get; set; }
        [Display(Name = "Field Group")]
        public int FieldsInfo_FieldGroup { get; set; }
        public List<SelectListItem> FieldsInfo_DrpFieldGroups
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpFieldGroups();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Description")]
        public string FieldsInfo_Description { get; set; }

        [Display(Name = "Field Type")]
        public int? FieldsInfo_FieldType { get; set; }
        public string FieldsInfo_FieldTypeText { get; set; }
        public List<SelectListItem> FieldsInfo_DrpFieldTypes
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpFieldTypes();
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Sub Type")]
        public int? FieldsInfo_FieldSubType { get; set; }
        public string FieldsInfo_FieldSubTypeText { get; set; }
        public List<SelectListItem> FieldsInfo_DrpFieldSubTypes
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpFieldSubTypes(0);
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Parent Field")]
        public long FieldsInfo_ParentField { get; set; }
        public string FieldsInfo_ParentFieldText { get; set; }
        public List<SelectListItem> FieldsInfo_DrpFields
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpFieldsPerGroup(0);
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Visibility")]
        public int? FieldsInfo_Visibility { get; set; }

        [Display(Name = "Visible If")]
        public bool? FieldsInfo_VisibleIf { get; set; }

        [Display(Name = "Visible If Guid")]
        public string FieldsInfo_VisibleIfGuid { get; set; }
        public List<SelectListItem> FieldsInfo_DrpFieldGuids
        {
            get
            {
                var list = EmailsDataModel.EmailsCatalogs.FillDrpFieldGuidsPerGroup(0);
                list.Insert(0, ListItems.Default());
                return list;
            }
        }

        [Display(Name = "Options")]
        public string FieldsInfo_Options { get; set; }

        [Display(Name = "Order")]
        public int FieldsInfo_Order { get; set; }

        public FieldsInfoModel()
        {
            FieldsInfo_VisibleIf = false;
        }
    }

    public class AvailableLettersModel
    {
        public string Transaction { get; set; }
        public int ID { get; set; }
        public long EmailID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool Sent { get; set; }
        public string DateSent { get; set; }
        public bool Read { get; set; }
        public string DateRead { get; set; }
        public bool Signed { get; set; }
        public string DateSigned { get; set; }
        public int? EventID { get; set; }
    }

    public class EmailWorkflowModel
    {

    }
}
