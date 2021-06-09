using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ccservice;
using System.Net;
using ePlatBack.Models.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ePlatBack.Models.DataModels
{
   public class RescomDataModel
    {
       public class ApplyPayment_Data {
           public string UserName { get; set; }
           public string Password { get; set; }

           public string Card_Holder { get; set; }
           public string Card_Holder_Address { get; set; }
           public string Card_Holder_Zip { get; set; }
           public string Card_Number { get; set; }
           public string Card_Security_Number { get; set; }
           public string Expiration_Date { get; set; }
           public string Reference_Code { get; set; }
           public double Transaction_Amount { get; set; }
           public int CurrencyID { get; set; }
       }

       public class ApplyPayment_Response {
           public string Auth_Code { get; set; }
           public double Authorization_Amount { get; set; }
           public DateTime? Authorization_Date { get; set; }
           public int Error_Code { get; set; }
       }

      public static IDictionary<int, string> ApplyPayment_ErrorCodes = new Dictionary<int, string>() 
       { 
            {0, "CC Approved"},
            {-1, "CC Declined"},
            {-2, "No authentication provided <br /> "
                + "No card holder provided <br /> " 
                + "No card holder address provided <br /> "
                + "No card_holder_zip provided <br /> "
                + "No expiration date provided <br /> "
                + "No card number provided <br /> "
                + "No reference code provided <br /> "
                + "0 amount provided <br /> "},
            {-3, "CreditCard not valid <br > Date not valid"},
            {-4, "Access denied (wrong login or password )"},
            {-5, "Loan record not found"},
            {-6, "Funding record not found"},
            {-7, "Legal Merchant record not found"},
            {-8, "Merchant record not found"},
            {-9, "Error instantiating data access class"},
            {-10, "Error instantiating credit card class"},
            {-11, "Error HTTPS required on remote client"}
       };

       public static string GetPaymentErrorDescription(int PaymentErrorCode){
           string errorDescription = "No error description found for " + PaymentErrorCode;
           if (ApplyPayment_ErrorCodes.ContainsKey(PaymentErrorCode)) { 
               errorDescription = ApplyPayment_ErrorCodes[PaymentErrorCode];
           }
           return errorDescription;
       }

        public static ApplyPayment_Response _ApplyPayment(ApplyPayment_Data Data)
        {
            BankService RescomBank = new BankService();
            AuthHeader Authorization = new AuthHeader();

            Authorization.Username = Data.UserName;
            Authorization.Password = Data.Password;

            RescomBank.AuthHeaderValue = Authorization;
            CardAuthorizationPubResponse result = new ccservice.CardAuthorizationPubResponse();
            //CardAuthorizationPubResponse result = RescomBank.AuthorizeCC(input);

            int cctype = 0;
            switch (Data.Card_Number.Substring(0, 1))
            {
                case "4":
                    cctype = 4;
                    break;
                case "5":
                    cctype = 5;
                    break;
            }

            ePlatEntities db = new ePlatEntities();
            int userid = 0;
            int merchantid = 0;
            var Merchant = (from m in db.tblMerchantAccounts
                            where m.username == Data.UserName
                            && m.currencyID == Data.CurrencyID
                            select m).FirstOrDefault();

            if (Merchant != null)
            {
                if (Merchant.userID != null)
                {
                    userid = (int)Merchant.userID;
                }
                if (Merchant.merchantAccount != null)
                {
                    merchantid = (int)Merchant.merchantID;
                }
            }

            StringBuilder input = new StringBuilder();
            input.Append("<CardAuth>");
            input.AppendFormat("<Primary_Bank>{0}</Primary_Bank>", "2");
            input.AppendFormat("<User_Id>{0}</User_Id>", userid);
            input.AppendFormat("<Merchant_Id>{0}</Merchant_Id>", merchantid);
            input.AppendFormat("<Card_Number>{0}</Card_Number>", Data.Card_Number);
            input.AppendFormat("<Expiration_Date>{0}</Expiration_Date>", Data.Expiration_Date.Replace("/01/", "/")); //month & "/" & year
            input.AppendFormat("<Transaction_Amount>{0}</Transaction_Amount>", Data.Transaction_Amount);
            input.AppendFormat("<Authorization_Reference>{0}</Authorization_Reference>", Data.Reference_Code);
            input.AppendFormat("<Resort>{0}</Resort>", "0");
            input.AppendFormat("<Account>{0}</Account>", "0");
            input.AppendFormat("<Version>{0}</Version>", "0");
            input.AppendFormat("<Card_Holder_Name>{0}</Card_Holder_Name>", Data.Card_Holder);
            input.AppendFormat("<Card_Holder_Address>{0}</Card_Holder_Address>", Data.Card_Holder_Address);
            input.AppendFormat("<Card_Holder_Zip>{0}</Card_Holder_Zip>", Data.Card_Holder_Zip);
            input.AppendFormat("<Card_Type>{0}</Card_Type>", cctype);
            input.AppendFormat("<Card_CV2_Code>{0}</Card_CV2_Code>", "1");
            input.AppendFormat("<Card_CV2_Value>{0}</Card_CV2_Value>", Data.Card_Security_Number);
            input.AppendFormat("<Card_Class_Code>{0}</Card_Class_Code>", "3");
            input.AppendFormat("<Fraud_Code>{0}</Fraud_Code>", "1");
            input.AppendFormat("<Months_Deferred>{0}</Months_Deferred>", "0");
            input.AppendFormat("<Number_Payments>{0}</Number_Payments>", "0");
            input.AppendFormat("<Plan_Type>{0}</Plan_Type>", "0");
            input.Append("</CardAuth>");

            result = RescomBank.CreditCardProcessAuthorReq(input.ToString());

            // the string date comes in format (month/day/year) // parse it to a dateObject
            // 11/15/2008 
            // 3/4/2009

            DateTime? Authorization_Date = null;
            if (result.Authorization_Date != "")
            {
                var authDateArr = result.Authorization_Date.Split('/').Select(x => int.Parse(x)).ToArray();
                Authorization_Date = new DateTime(authDateArr[2], authDateArr[0], authDateArr[1]);
            }
            ApplyPayment_Response response = new ApplyPayment_Response();
            response.Auth_Code = result.Auth_Code;
            response.Authorization_Amount = result.Authorization_Amount;
            response.Authorization_Date = Authorization_Date;
            response.Error_Code = result.Error_Code;
            return response;
        }

        public static ApplyPayment_Response ApplyPayment(ApplyPayment_Data Data) {
           BankService RescomBank = new BankService();
           AuthHeader Authorization = new AuthHeader();
           
           Authorization.Username = Data.UserName;
           Authorization.Password = Data.Password;
           
           RescomBank.AuthHeaderValue = Authorization;
           CardAuthorizationPubResponse result = new ccservice.CardAuthorizationPubResponse();
           //CardAuthorizationPubResponse result = RescomBank.AuthorizeCC(input);

           if (Data.CurrencyID == 2)
           {
               int cctype = 0;
               switch (Data.Card_Number.Substring(0,1)) {
                   case "4":
                       cctype = 4;
                       break;
                   case "5":
                       cctype = 5;
                       break;
               }

               ePlatEntities db =  new ePlatEntities();
               int userid = 0;
               int merchantid = 0;
               var Merchant = (from m in db.tblMerchantAccounts
                              where m.username == Data.UserName
                              && m.currencyID == Data.CurrencyID
                              select m).FirstOrDefault();

               if (Merchant != null)
               {
                   if (Merchant.userID != null)
                   {
                       userid = (int)Merchant.userID;
                   }
                   if (Merchant.merchantAccount != null)
                   {
                       merchantid = (int)Merchant.merchantID;
                   }
               }

               StringBuilder input = new StringBuilder();
               input.Append("<CardAuth>");
               input.AppendFormat("<Primary_Bank>{0}</Primary_Bank>", "2");
               input.AppendFormat("<User_Id>{0}</User_Id>", userid);
               input.AppendFormat("<Merchant_Id>{0}</Merchant_Id>", merchantid);
               input.AppendFormat("<Card_Number>{0}</Card_Number>", Data.Card_Number);
               input.AppendFormat("<Expiration_Date>{0}</Expiration_Date>", Data.Expiration_Date.Replace("/01/","/")); //month & "/" & year
               input.AppendFormat("<Transaction_Amount>{0}</Transaction_Amount>", Data.Transaction_Amount);
               input.AppendFormat("<Authorization_Reference>{0}</Authorization_Reference>", Data.Reference_Code);
               input.AppendFormat("<Resort>{0}</Resort>", "0");
               input.AppendFormat("<Account>{0}</Account>", "0");
               input.AppendFormat("<Version>{0}</Version>", "0");
               input.AppendFormat("<Card_Holder_Name>{0}</Card_Holder_Name>", Data.Card_Holder);
               input.AppendFormat("<Card_Holder_Address>{0}</Card_Holder_Address>", Data.Card_Holder_Address);
               input.AppendFormat("<Card_Holder_Zip>{0}</Card_Holder_Zip>", Data.Card_Holder_Zip);
               input.AppendFormat("<Card_Type>{0}</Card_Type>", cctype);
               input.AppendFormat("<Card_CV2_Code>{0}</Card_CV2_Code>", "1");
               input.AppendFormat("<Card_CV2_Value>{0}</Card_CV2_Value>", Data.Card_Security_Number);
               input.AppendFormat("<Card_Class_Code>{0}</Card_Class_Code>", "3");
               input.AppendFormat("<Fraud_Code>{0}</Fraud_Code>", "1");
               input.AppendFormat("<Months_Deferred>{0}</Months_Deferred>", "0");
               input.AppendFormat("<Number_Payments>{0}</Number_Payments>", "0");
               input.AppendFormat("<Plan_Type>{0}</Plan_Type>", "0");
               input.Append("</CardAuth>");

               result = RescomBank.CreditCardProcessAuthorReq(input.ToString());
           }
           else if (Data.CurrencyID == 1)
           {
               CardAuthorizationPub input = new CardAuthorizationPub();
               input.Card_Holder = Data.Card_Holder;
               input.Card_Holder_Address = Data.Card_Holder_Address;
               input.Card_Holder_Zip = Data.Card_Holder_Zip;
               input.Card_Number = Data.Card_Number;
               input.Expiration_Date = Data.Expiration_Date;
               input.Reference_Code = Data.Reference_Code;
               input.Transaction_Amount = Data.Transaction_Amount;

               System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
               result = RescomBank.AuthorizeCC(input);
           }
           

           // the string date comes in format (month/day/year) // parse it to a dateObject
           // 11/15/2008 
           // 3/4/2009

           DateTime? Authorization_Date = null;
           if (result.Authorization_Date != "") {
                var authDateArr = result.Authorization_Date.Split('/').Select(x => int.Parse(x)).ToArray();
                Authorization_Date = new DateTime(authDateArr[2], authDateArr[0], authDateArr[1]);
           }
           ApplyPayment_Response response = new ApplyPayment_Response();
           response.Auth_Code=result.Auth_Code;
           response.Authorization_Amount= result.Authorization_Amount;
           response.Authorization_Date = Authorization_Date; 
           response.Error_Code= result.Error_Code;
           return response;
        }
    }


    public class ResortConnectDataModel
    {
        public static MemberReservationsViewModel.Reservation GetReservation(string confirmation)
        {
            MemberReservationsViewModel.Reservation reservation = new MemberReservationsViewModel.Reservation();

            resortConnectEntities db = new resortConnectEntities();
            reservation = (from r in db.Reservation
                           join c in db.Reservation_Contact
                           on r.ReservationId equals c.ReservationId
                           into r_c
                           from c in r_c.DefaultIfEmpty()
                           join p in db.Contact
                           on c.ContactId equals p.ContactId
                           into c_p
                           from p in c_p.DefaultIfEmpty()
                          where r.ConfirmationNumber == confirmation
                           select new MemberReservationsViewModel.Reservation()
                          {
                              CRS = r.ConfirmationNumber,
                              FirstName = p.FirstName,
                              LastName = p.LastName,
                              ResortID = r.ResortNumber,
                              Arrival = r.CheckInDate,
                              Departure = r.CheckOutDate,
                              Status = r.ReservationStatus
                          }).FirstOrDefault();

            if(reservation != null)
            {
                ePlatEntities edb = new ePlatEntities();
                var resort = (from r in edb.tblPlaces
                             where r.resortConnectResortID == reservation.ResortID
                             select new { r.oldPlaceName, r.placeID }).FirstOrDefault();

                if(resort != null)
                {
                    reservation.Resort = resort.oldPlaceName;
                    reservation.PlaceID = resort.placeID;
                }
            }

            return reservation;
        }
    }
}
