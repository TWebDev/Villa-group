using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.ViewModels
{
    public class CouponViewModel
    {
        [Display(Name = "Security_String", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public Guid PurchaseID { get; set; }
        public long ItemID { get; set; }
        public int Type { get; set; }
        public string Logo { get; set; }
        public string BarCode { get; set; }
        public string Website { get; set; }
        public string WebsiteUrl { get; set; }
        public string Operator { get; set; }
        [Display(Name = "Destination", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string Destination { get; set; }
        [Display(Name = "Coupon_Number", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string CouponNumber { get; set; }
        public string CouponReference { get; set; }
        [Display(Name = "Guest_Name", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string GuestName { get; set; }
        public string ReservedFor { get; set; }
        [Display(Name = "Transaction_ID", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string FirstAuthCode { get; set; }

        [Display(Name = "Activity_Service", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string ActivityName { get; set; }
        [Display(Name = "Activity_Service_Date", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string ActivityDateAndSchedule { get; set; }
        [Display(Name = "Valid_for", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string Units { get; set; }
        [Display(Name = "Meeting_Time", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string MeetingTime { get; set; }
        [Display(Name = "Meeting_Point", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string MeetingPoint { get; set; }
        public string CouponNotes { get; set; }

        public List<CouponServiceInfo> PackageServices { get; set; }

        [Display(Name = "Provider", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string Provider { get; set; }
        public string InvoiceCurrency { get; set; }
        [Display(Name = "Provider_Phone", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string ProviderPhone { get; set; }
        [Display(Name = "Confirmation_Number", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string ConfirmationNumber { get; set; }
        [Display(Name = "Toll_Free", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string PhoneNumbers { get; set; }
        public string PhoneNumbersShortCoupon { get; set; }
        [Display(Name = "Exchange_Rate", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public decimal ExchangeRate { get; set; }
        [Display(Name = "Purchase_Date", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string PurchaseDate { get; set; }
        [Display(Name = "Travel_Info", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string TravelInfo { get; set; }
        public bool? RoundTrip { get; set; }
        public string RoundDate { get; set; }
        public string RoundTravelInfo { get; set; }
        public string RoundHotel { get; set; }
        public string RoundFlightTime { get; set; }
        public string RoundMeetingTime { get; set; }
        [Display(Name = "Recommendations", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string Recommendations { get; set; }
        public string Disclaimer { get; set; }
        [Display(Name = "Agent", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string SalesAgent { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public string Payment { get; set; }
        public bool Audited { get; set; }
        public bool AbleToUnaudit { get; set; }
        public string Audit { get; set; } // audited - date- user - factura
        public bool PaidToProvider { get; set; }
        public string PaidToProviderInfo { get; set; }
        public string CloseOut { get; set; } // 2014-01-12 (gguerrap)
        /*public decimal Cost { get; set; }
        public string CostCurrencyCode { get; set; }*/
        public List<Money> Cost { get; set; }
        public bool CustomCost { get; set; }
        public long? ProviderInvoiceID { get; set; }
        public string TerminalID { get; set; }
        public int PrintCounter { get; set; }
        public decimal? CancelationCharge { get; set; }
        public string PrintedBy { get; set; }
        public string Invitation { get; set; }
        public string CouponTotal { get; set; }
        public string CouponCurrencyCode { get; set; }
    }

    public class CouponsList
    {
        public List<Money> TotalCost { get; set; }
        public List<CouponViewModel> Coupons { get; set; }
    }

    public class CouponServiceInfo
    {
        [Display(Name = "Activity_Service", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string ActivityName { get; set; }
        [Display(Name = "Activity_Service_Date", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string ActivityDateAndSchedule { get; set; }
        [Display(Name = "Valid_for", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string Units { get; set; }
        [Display(Name = "Meeting_Time", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string MeetingTime { get; set; }
        [Display(Name = "Meeting_Point", ResourceType = typeof(Resources.Models.Coupon.CouponStrings))]
        public string MeetingPoint { get; set; }
        public string CouponNotes { get; set; }
    }

    public class CouponReportModel
    {
        [Display(Name="ID")]
        public long CouponID { get; set; }
        public Guid PurchaseID { get; set; }
        [Display(Name = "Folio")]
        public string Folio { get; set; }
        [Display(Name = "Coupon Reference")]
        public string CouponReference { get; set; }
        [Display(Name = "Saved on")]
        public string DateSaved { get; set; }
        [Display(Name = "Confirmation Date")]
        public string ConfirmationDate { get; set; }
        [Display(Name = "Cancelation Date")]
        public string CancelationDate { get; set; }
        [Display(Name = "Sales Agent")]
        public string SalesAgent { get; set; }
        [Display(Name = "Reservations Agent")]
        public string ReservationsAgent { get; set; }
        [Display(Name = "Pax")]
        public decimal UnitsQty { get; set; }
        [Display(Name = "PoS")]
        public string PointOfSale { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Activity")]
        public string Service { get; set; }
        [Display(Name = "Activity Date")]
        public string ServiceDate { get; set; }
        [Display(Name = "Units")]
        public string Units { get; set; }
        [Display(Name = "Provider")]
        public string Provider { get; set; }
        [Display(Name = "Avance Provider ID")]
        public string ProviderAvanceID { get; set; }
        [Display(Name = "Customer")]
        public string Customer { get; set; }
        [Display(Name = "Language")]
        public string Culture { get; set; }
        [Display(Name = "Price Type")]
        public string PriceType { get; set; }
        [Display(Name = "Total")]
        public Money Total { get; set; }
        [Display(Name = "Total b/ IVA")]
        public Money TotalNoIVA { get; set; }
        public bool CustomCost { get; set; }
        [Display(Name = "Cost")]
        public Money Cost { get; set; }
        [Display(Name = "Cost b/ IVA")]
        public Money CostNoIVA { get; set; }
        [Display(Name = "Utility")]
        public Money Utility { get; set; }
        [Display(Name = "Utility b/ IVA")]
        public Money UtilityNoIVA { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Promo")]
        public string Promo { get; set; }
        [Display(Name = "Transaction")]
        public string AuthCode { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
    }
}