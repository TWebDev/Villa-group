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
    public class PaymentAssignationViewModel
    {
        public Guid PurchaseID { get; set; }
        public string Customer { get; set; }
        public long TerminalID { get; set; }
        public List<CloseOutItem> CloseOuts { get; set; }
        
        public class CloseOutItem
        {
            public long CloseOutID { get; set; }
            public string PointOfSale { get; set; }
            public int PointOfSaleID { get; set; }
            public DateTime Date { get; set; }
            public Guid? UserID { get; set; }
            public List<PaymentItem> Payments { get; set; }
            public List<AssignationItem> Assignations { get; set; }
            public List<ExtraPaymentItem> ExtraPayments { get; set; }
            public string DiagnosisMessage { get; set; }
            public bool Status { get; set; }
        }

        public class PaymentItem
        {
            public long PaymentDetailsID { get; set; }
            public DateTime Date { get; set; }
            public string PaymentType { get; set; }
            public string SavedBy { get; set; }
            public decimal TotalPayment { get; set; }
            public decimal TotalRemaining { get; set; }
            public int CurrencyID { get; set; }
            public string CurrencyCode { get; set; }
            public int MoneyTransactionTypeID { get; set; }
        }

        public class AssignationItem
        {
            public long PaymentAssignationID { get; set; }
            public string Service { get; set; }
            public long PurchaseServiceID { get; set; }
            public long PurchaseServiceDetailID { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public string PriceType { get; set; }
            public string Folio { get; set; }
            public Money Total { get; set; }
            public int CurrencyID { get; set; }

            public decimal ExchangeRate { get; set; }
            public long? ExchangeRateID { get; set; }
            public long? PaymentDetailsID { get; set; }
            public string PaymentType { get; set; }
            public Money PaymentSubtotal { get; set; }
            public decimal PaymentTotal { get; set; }
            public int PaymentCurrencyID { get; set; }
            public int MoneyTransactionTypeID { get; set; }
        }

        public class ExtraPaymentItem
        {
            public Money ExtraAmount { get; set; }
            public long ExtraFromPaymentDetailsID { get; set; }
            public string PaymentDetailDescription { get; set; }
            public long RefundPaymentDetailsID { get; set; }
            public string RefundDetailDescription { get; set; }
        }

        public class SearchPaymentAssignation
        {
            [Required]
            [Display(Name = "Purchase ID")]
            public string PurchaseID { get; set; }

            [Display(Name = "Date")]
            public string Date { get; set; }
        }
    }
}
