using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    public class MailingViewModel
    {
        public List<MailMessage> Mails { get; set; }
        public string Message { get; set; }

        public class MailAttachment
        {
            public string Name { get; set; }
            public string Location { get; set; }
        }

        public class MailAddress
        {
            public string DisplayName { get; set; }
            public string Address { get; set; }
        }

        public class MailMessage
        {
            public int Index { get; set; }
            public Guid MailMessageID { get; set; }
            public string UID { get; set; }
            public string Sender { get; set; }
            public List<MailAddress> To { get; set; }
            public List<MailAddress> CC { get; set; }
            public List<MailAddress> BCC { get; set; }
            public string EmailAddress { get; set; }
            public DateTime SendDate { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<MailAttachment> Attachments { get; set; }
            public string Importance { get; set; }
            public bool Replied { get; set; }
            public string Folder { get; set; }
            public Guid? LeadID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class EmailPreViewModel
        {
            public string TransactionID { get; set; }
            public string ItemType { get; set; }
            public string ItemID { get; set; }
            public string ReservationID { get; set; }
            public string Transaction { get; set; }
            public string FieldGroup { get; set; }
            public string FromAddress { get; set; }
            public string FromAlias { get; set; }
            public string Subject { get; set; }
            public string To { get; set; }
            public string ReplyTo { get; set; }
            public string CC { get; set; }
            public string BCC { get; set; }
            public string Body { get; set; }
            public string Status { get; set; }
            public string SysEvent { get; set; }
            public List<KeyValue> ListFieldsValues { get; set; }
        }

        public class GetMailing
        {
            public int emailNotificationLogID { get; set; }
            public long emailID { get; set; }
            public string description { get; set; }
            public int emailNotificationID { get; set; }
            public DateTime dateSent { get; set; }
            public Guid sentByUserID { get; set; }
            public string sentByUser { get; set; }
            public Guid? leadID { get; set; }
            public int? tourID { get; set; }
            public Guid? trackingID { get; set; }
            public DateTime? lastOpen { get; set; }
            public DateTime? lastClick { get; set; }
            public string preview { get; set; }
            public string subject { get; set; }
            public int? sysEventID { get; set; }
            public List<MailingTracking> trackingInfo { get; set; }
        }

        public class MailingTracking
        {
            public int emailNotificationEventID { get; set; }
            public int emailNotificationLogID { get; set; }
            public int sysEventID { get; set; }
            public string sysEvent { get; set; }
            public DateTime dateEvent { get; set; }
            public string url { get; set; }
        }
        public class MailMessageResponse
        {

            public System.Net.Mail.MailMessage MailMessage { get; set; }
            public bool? Sent { get; set; }
            public string Recipient { get; set; }
            public Guid? Transaction { get; set; }
            public Guid? ItemID { get; set; }
            public string Exception { get; set; }
            public string BodyWithoutTrackingID { get; set; }
        }
    }
}
