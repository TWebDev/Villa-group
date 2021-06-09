using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
  public  class ChargesDataModel
    {
        public static List<SelectListItem> GetAllChargeTypes()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from c in db.tblChargeTypes
                        select new { c.chargeTypeID, c.chargeType };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.chargeTypeID.ToString(), Text = i.chargeType });
            return list;
        }

        public static List<SelectListItem> GetAllChargeDescriptions()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from c in db.tblChargeDescriptions
                        select new { c.chargeDescriptionID, c.chargeDescription };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.chargeDescriptionID.ToString(), Text = i.chargeDescription });
            return list;
        }

       

      public class SaveCharge_Data{
         public int terminalID;
         public int moneyTransactionTypeID;
         public int merchanAccountID;
         public long billingInfoID;
         public long[] reservationPaymentIds;	
         public decimal authAmount;
         public RescomDataModel.ApplyPayment_Response paymentResponse;
         public string Reference_Code;
  }

        public static void SaveChargeResponse(SaveCharge_Data data)
        {
            ePlatEntities db = new ePlatEntities();
            foreach (var reservationPaymentId in data.reservationPaymentIds)
            {
                var dbPayment = (from x in db.tblPaymentDetails where x.paymentDetailsID == reservationPaymentId && (x.deleted == null || x.deleted == false) select x).Single();

                if (dbPayment.moneyTransactionID == null)
                {//insert
                    tblMoneyTransactions mt = new tblMoneyTransactions();
                    mt.authAmount = (decimal)data.paymentResponse.Authorization_Amount;
                    mt.authCode = data.paymentResponse.Auth_Code;
                    mt.authDate = data.paymentResponse.Authorization_Date;
                    mt.billingInfoID = data.billingInfoID;
                    mt.errorCode = data.paymentResponse.Error_Code.ToString();
                    mt.madeByEplat = true;
                    mt.merchantAccountID = data.merchanAccountID;
                    mt.reference = data.Reference_Code;
                    mt.transactionTypeID = data.moneyTransactionTypeID;
                    mt.transactionDate = DateTime.Now;
                    mt.terminalID = dbPayment.tblReservations.tblLeads.terminalID;
                    
                    //revisar que en lugar de generarse un pago a la hora guardar cambios
                    // solamente se haga la relacion
                    mt.tblPaymentDetails.Add(dbPayment);
                    db.tblMoneyTransactions.AddObject(mt);
                }
                else {  //  update                
                    dbPayment.tblMoneyTransactions.authAmount = (decimal)data.paymentResponse.Authorization_Amount;
                    dbPayment.tblMoneyTransactions.authCode = data.paymentResponse.Auth_Code;
                    dbPayment.tblMoneyTransactions.authDate = data.paymentResponse.Authorization_Date;
                    dbPayment.tblMoneyTransactions.billingInfoID = data.billingInfoID;
                    dbPayment.tblMoneyTransactions.errorCode = data.paymentResponse.Error_Code.ToString();
                    dbPayment.tblMoneyTransactions.madeByEplat = true;
                    dbPayment.tblMoneyTransactions.merchantAccountID = data.merchanAccountID;
                    dbPayment.tblMoneyTransactions.reference = data.Reference_Code;
                    dbPayment.tblMoneyTransactions.transactionTypeID = data.moneyTransactionTypeID;
                    dbPayment.tblMoneyTransactions.transactionDate = DateTime.Now;
                }

                db.SaveChanges();
            }
        }
    }
}
