using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.Web.Security;

namespace ePlatBack.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public FormsSearchModel FormsSearchModel { get; set; }
        public List<FieldValueModel> ListFieldValues { get; set; }
        public List<FormsInfoModel> ListForms { get; set; }
        public List<NotificationHistoryModel> ListNotificationHistory { get; set; }
        public List<SPIViewModel.VLOCustomer> ListVLOCustomers { get; set; }
        public string FieldValue_FieldValues { get; set; }

        public VLOManifestModel VLOManifestModel { get; set; }
    }

    public class FormsSearchModel
    {
        public int FormSearch_FormID { get; set; }
        [Display(Name = "Form Name")]
        public string FormSearch_Name { get; set; }

        [Display(Name = "Description")]
        public string FormSearch_Description { get; set; }

        [Display(Name = "Date")]
        public string FormSearch_Date
        {
            get
            {
                return DateTime.Today.ToString("yyyy-MM-dd");
            }
        }
    }

    public class FormsInfoModel
    {
        public int FormInfo_FormID { get; set; }
        public string FormInfo_FormGUID { get; set; }
        public string FormInfo_Name { get; set; }
        public string FormInfo_Description { get; set; }
        public string FormInfo_EmailNotification { get; set; }
        public string FormInfo_SavedByUser { get; set; }
        public string FormInfo_DateSaved { get; set; }
    }

    public class FieldValueModel
    {
        public long FieldValue_FieldID { get; set; }
        public string FieldValue_Field { get; set; }
        public string FieldValue_Description { get; set; }
        public int FieldValue_FieldType { get; set; }
        public int FieldValue_FieldSubType { get; set; }
        public string FieldValue_FieldSubTypeText { get; set; }
        public List<SelectListItem> FieldValue_Options { get; set; }
        public string FieldValue_Value { get; set; }
        public string FieldValue_FieldValues { get; set; }
    }

    public class NotificationHistoryModel
    {
        public int NotificationHistory_FormID { get; set; }
        public string NotificationHistory_Transaction { get; set; }
        public string NotificationHistory_Receiver { get; set; }
        public string NotificationHistory_Email { get; set; }
        public string NotificationHistory_DateSent { get; set; }
        public string NotificationHistory_DateReceived { get; set; }
        public string NotificationHistory_DateOpened { get; set; }
        public string NotificationHitory_NumberClicks { get; set; }
        public List<KeyValuePair<string, string>> NotificationHistory_Clicks { get; set; }
        public string NotificationHistory_SentByUser { get; set; }
    }

    public class VLOManifestModel
    {
        [Display(Name = "From Date")]
        public string RequestDate { get; set; }
    }


    public class ePlatNotificationsModel
    {
        public static List<Notification> Notifications { get; set; }
        public static List<NotificationsReference> NotificationsSettings { get; set; }

        public class Notification
        {
            public long notificationID { get; set; }
            public long itemID { get; set; }
            public long sysItemTypeID { get; set; }
            public string sysItemTypeName { get; set; }
            public long terminalID { get; set; }
            public string terminal { get; set; }
            public Guid forUserID { get; set; }
            public string forUserFirstName { get; set; }
            public string forUserLastName { get; set; }
            public string description { get; set; }
            public bool read { get; set; }
            public DateTime eventDateTime { get; set; }
            public Guid? eventByUserID { get; set; }
            public string eventUserFirstName { get; set; }
            public string eventUserLastName { get; set; }
            public DateTime? deliveredDateTime { get; set; }
            public DateTime? readDateTime { get; set; }
            public string color { get; set; }
        }

        public class NotificationsReference
        {
            public long notificationReferenceID { get; set; }
            public int notificationTypeID { get; set; }
            public string notificationType { get; set; }
            public Guid userID { get; set; }
            public string user { get; set; }
            public string color { get; set; }
            public long? terminalID { get; set; }
            public string terminal { get; set; }
        }

        public class UpdateNotificationDelivered
        {
            public long notificationID { get; set; }
            public DateTime deliveredDate { get; set; }
            public DateTime readTime { get; set; }
        }
    }

}
