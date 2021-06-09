using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.Utils.Custom;
namespace ePlatBack.Models.DataModels
{
   public class MerchantAccountDataModel
    {
       ePlatEntities db = new ePlatEntities();
       
        //public class MerchantAccountCredentials
        //{
        //    public string Username = "";
        //    public string Password = "";
        //}

        public class MerchantAccountInfo {
            public int MerchantAccountID { get; set; }
            public string MerchantAccountNumber { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string MerchantProvider { get; set; }
            public string MerchantAccountBillingName { get; set; }
            public int TerminalID { get; set; }
        }
       /// <summary>
        /// Gets Merchant Account based on many conditions like destination, resort, terminal etc.
       /// </summary>
       /// <param name="PaymentDetailID"></param>
       /// <returns></returns>
        public static MerchantAccountInfo GetMerchantAccount(long PaymentDetailID)
        {
            ePlatEntities db = new ePlatEntities();
            var payment= (from x in db.tblPaymentDetails where x.paymentDetailsID == PaymentDetailID select x).First();
            
             UserSession session = new UserSession();
            int? sysWorkGroupID=session.WorkGroupID; // make sure everytime the workgroup is change on the client side, so we can have accurate info against the sysWorkGroup.
            long terminalID= payment.tblReservations.tblLeads.terminalID;
            long? destinationID= payment.tblReservations.destinationID;
            long? placeID = payment.tblReservations.placeID;
            long? leadSourceID = payment.tblReservations.tblLeads.leadSourceID;
            int? leadTypeID = payment.tblReservations.tblLeads.leadTypeID;
            int chargeTypeID= payment.chargeTypeID;
            int? chargeDescriptionID= payment.chargeDescriptionID;


            var wgMerchantCombinations= from x in db.tblMerchantAccountSettings where x.sysWorkGroupID ==  sysWorkGroupID  select x;
            
            double higherMatch=0;
            MerchantAccountInfo MA= null;
            foreach (var mac in wgMerchantCombinations)
            {
                double currentMatches=1; //sysWorkGroupID matches by default since it was used in the previous query
                
                 if( mac.terminalID== terminalID ){ currentMatches+=1;}
                else if(mac.terminalID==null ){ currentMatches +=.5;}                

                if( mac.destinationID== destinationID ){ currentMatches+=1;}
                else if(mac.destinationID==null ){ currentMatches +=.5;}

                 if( mac.placeID== placeID ){ currentMatches+=1;}
                else if(mac.placeID==null ){ currentMatches +=.5;}

                 if( mac.leadSourceID== leadSourceID ){ currentMatches+=1;}
                else if(mac.leadSourceID==null ){ currentMatches +=.5;}

                if( mac.leadTypeID== leadTypeID ){ currentMatches+=1;}
                else if(mac.leadTypeID==null ){ currentMatches +=.5;}

                 if( mac.chargeTypeID== chargeTypeID ){ currentMatches+=1;}
                else if(mac.chargeTypeID==null ){ currentMatches +=.5;}

                if( mac.chargeDescriptionID== chargeDescriptionID ){ currentMatches+=1;}
                else if(mac.chargeDescriptionID==null ){ currentMatches +=.5;}

                if(currentMatches > higherMatch ){
                
                    MA=new MerchantAccountInfo(){
                       MerchantAccountNumber=mac.tblMerchantAccounts.merchantAccount,
                       MerchantAccountBillingName = mac.tblMerchantAccounts.merchantAccountBillingName,
                       MerchantAccountID = mac.tblMerchantAccounts.merchantAccountID,
                       MerchantProvider = mac.tblMerchantAccounts.merchantProvider,
                       Password = mac.tblMerchantAccounts.password,
                       Username = mac.tblMerchantAccounts.username
                    };

                    higherMatch=currentMatches;                    
                }
            }
    return MA;           
        }
    }
}
