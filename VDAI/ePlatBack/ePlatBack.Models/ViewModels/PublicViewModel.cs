using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    class PublicViewModel
    {

    }

    public class PaymentConfirmationModel : EmailPreViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserFirstName { get; set; }
        public string OptionsSold { get; set; }
        public string Resort { get; set; }
        public string ArrivalDate { get; set; }
        public string ConfirmationNumber { get; set; }
        public string DepartureDate { get; set; }
        public string Amount { get; set; }
        public string Invoice { get; set; }
        public string PaymentDate { get; set; }
        public string CardHolder { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string MerchantAccount { get; set; }
        public string SignatureStatus { get; set; }
        public string Signature { get; set; }
        public string PaymentInformation { get; set; }
    }

    public class EmailPreViewModel
    {
        public string LeadID { get; set; }
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
        public string AltTo { get; set; }
        public string Body { get; set; }
        public List<KeyValue> ListFieldsValues { get; set; }
        public string Status { get; set; }
        public string SysEvent { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class OptionalsSoldModel
    {
        public string option { get; set; }
        public string date { get; set; }
        public string quantity { get; set; }
        public string cost { get; set; }
    }

    public class ResortConnectOptionals
    {
        public string ConfirmationNumber { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public string BaseCurrencyAmount { get; set; }
        public string Note { get; set; }
        public DateTime? Date { get; set; }
        public string CarrierName { get; set; }
        public string FlightNumber { get; set; }
        public int? ResortID { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class Importable
    {
        public string idcodigodemercado { get; set; }
        public string namecodigodemercado { get; set; }
        public string codigo { get; set; }
        public string CodigoMayor { get; set; }
        //public string idtipodehabitacion { get; set; }
        //public string nametipodehabitacion { get; set; }
        //public string codetipodehabitacion { get; set; }
        //public string descripcion { get; set; }
    }

    public class GenericListItem
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public string Property3 { get; set; }
        public string Property4 { get; set; }
        public string Property5 { get; set; }
    }

    public class MailMessageResponse
    {
        public System.Net.Mail.MailMessage MailMessage { get; set; }
        public bool? Sent { get; set; }
        public string Recipient { get; set; }
        public Guid? Transaction { get; set; }
        public Guid? ItemID { get; set; }
        public string Exception { get; set; }
    }

    public class GlobalVars
    {
        public static TokenResult Token { get; set; } = new TokenResult();
        public static int GetTokenfailureCount { get; set; } = 0;
    }

    public class TokenResult
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Access_token { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string Token_type { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int Expires_in { get; set; }
        [JsonProperty(PropertyName = "userName")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = ".issued")]
        public DateTime Issued { get; set; }
        [JsonProperty(PropertyName = ".expires")]
        public DateTime Expires { get; set; }
        public string Error { get; set; }
        public string ServerURL { get; set; } = "https://developers.eplat.com";
    }

    public class RedeemCredits
    {
        public long ResortID { get; set; }
        public Guid ID { get; set; }
        public string Account { get; set; }
        public List<ShoppingCart> ShoppingCart { get; set; }
    }
    public class ShoppingCart
    {
        public int TransactionTypeID { get; set; }
        public int CreditsApplied { get; set; }
        public string Title { get; set; }
    }

    public class RedemptionResponse
    {
        public RedemptionStatus Status = RedemptionStatus.NotFound;
        public int Amount = 0;
        public Guid? AuthorizationCode = Guid.Empty;
        public DateTime? Date = null;
    }

    public enum RedemptionStatus
    {
        UserNotAuthorized = -4,
        NotEnoughData = -3,
        NotEnoughCredits = -2,
        NotFound = -1,
        AcceptTermsConditions = 0,
        Ok = 1,
        Applied = 2
    }
    //public class RedeemCredits//modelo para mandar a la api
    //{
    //    public string MemberAccount { get; set; }
    //    public int TransactionTypeID { get; set; }
    //    public string Concept { get; set; }
    //    public int Amount { get; set; }
    //    public string Reference { get; set; }
    //    public long ResortID { get; set; }
    //}
}
