using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Models.ViewModels
{
    public class DashboardViewModel
    {
        public PendingCloseOuts PendingCloseOuts { get; set; }

        public DailyVolume DailyVolume { get; set; }

        public DailyCommissions DailyCommissions { get; set; }
        public List<SysComponentsPrivilegesModel> Privileges { get; set; }
    }

    public class DailyCommissions
    {
        public List<AgentCommission> AgentsCommissions { get; set; }
        public Money Production { get; set; }
        public Money ProductionNoIVA { get; set; }
        public Money Commission { get; set; }

        public class AgentCommission
        {
            public Guid UserID { get; set; }
            public string Agent { get; set; }
            public string JobPostion { get; set; }
            public Money Production { get; set; }
            public Money ProductionNoIVA { get; set; }
            public Money Commission { get; set; }
            public List<CommissionsStructure> Commissions { get; set; }
        }

        public class CommissionsStructure
        {
            public long PriceTypeID { get; set; }
            public string PriceType { get; set; }
            public decimal MinProfit { get; set; }
            public decimal? MaxProfit { get; set; }
            public decimal Percentage { get; set; }
            public Money Production { get; set; }
            public Money ProductionNoIVA { get; set; }
            public Money Commission { get; set; }
        }
    }

    public class AgentsVolume
    {
        public List<AgentVolume> Agents { get; set; }
        public decimal ExchangeRate { get; set; }
        public Money Total { get; set; }

        public class AgentVolume
        {
            public Guid AgentUserID { get; set; }
            public int Index { get; set; }
            public string Agent { get; set; }
            public Money Amount { get; set; }
            public decimal Percentage { get; set; }
        }
    }

    public class DailyVolume
    {
        public List<PointsOfSaleVolume> PoSVolume { get; set; }
        public List<PointsOfSaleVolume> LYPoSVolume { get; set; }
        public List<AgentVolume> AgentsVolume { get; set; }
        public List<ProgramVolume> ProgramsVolume { get; set; }

        public decimal ExchangeRate { get; set; }
        public decimal LYExchangeRate { get; set; }
        public Money Total { get; set; }
        public Money LYTotal { get; set; }
        public Money GoalTotal { get; set; }
        public decimal GoalPercentage { get; set; }

        public List<PriceTypeVolume> PriceTypeTotals { get; set; }
        public List<PriceTypeVolume> LYPriceTypeTotals { get; set; }

        public class DailyVolumeSet
        {
            public List<PointsOfSaleVolume> PoSVolume { get; set; }
            public List<AgentVolume> AgentsVolume { get; set; }
            public List<ProgramVolume> ProgramsVolume { get; set; }
            public decimal ExchangeRate { get; set; }
            public Money Total { get; set; }
            public List<PriceTypeVolume> PriceTypeTotals { get; set; }
        }

        public class AgentVolume
        {
            public Guid? AgentID { get; set; }
            public string Agent { get; set; }
            public int ConfirmedCoupons { get; set; }
            public List<PriceTypeVolume> PriceTypeVolume { get; set; }
            public Money Total { get; set; }
        }

        public class ProgramVolume
        {
            public string Program { get; set; }
            public int ConfirmedCoupons { get; set; }
            public List<PriceTypeVolume> PriceTypeVolume { get; set; }
            public Money Total { get; set; }
        }

        public class PointsOfSaleVolume
        {
            public int PointOfSaleID { get; set; }
            public string PointOfSale { get; set; }
            public int ConfirmedCoupons { get; set; }
            public string ShortName { get; set; }
            public List<PriceTypeVolume> PriceTypeVolume { get; set; }
            public Money Total { get; set; }
        }

        public class PriceTypeVolume
        {
            public int PriceTypeID { get; set; }
            public string PriceType { get; set; }
            public Money Total { get; set; }
            public decimal Percentage { get; set; }
            public int GlobalPercentage { get; set; }
        }
    }

    public class CloseOutsWithErrors
    {
        public List<CloseOutItem> CloseOuts { get; set; }

        public class CloseOutItem
        {
            public long CloseOutID { get; set; }
            public long PointOfSale { get; set; }
            public DateTime Date { get; set; }
            public decimal TotalCloseOut { get; set; }
            public decimal TotalCache { get; set; }
            public decimal Difference { get; set; }
        }
    }

    public class OnlineGoals
    {
        public List<OnlineTerminalGoal> Terminals { get; set; }

        public class OnlineTerminalGoal
        {
            public long TerminalID { get; set; }
            public string Terminal { get; set; }
            public int CurrencyID { get; set; }

            public int LastYear { get; set; }
            public int CurrentYear { get; set; }
            public string MonthName { get; set; }

            public decimal LYMonthlySales { get; set; }
            public decimal CYMonthlyGoal { get; set; }
            public decimal CYCurrentTotal { get; set; }
            public decimal GoalPercentage { get; set; }
            public decimal AverageSale { get; set; }
            public decimal Difference { get; set; }
            public decimal GoalProgress { get; set; }

            public decimal USDCurrentSales { get; set; }
            public decimal MXNCurrentSales { get; set; }
            public decimal MXNUSDCurrentSales { get; set; }

            public decimal LYUSDSales { get; set; }
            public decimal CYUSDSales { get; set; }
            public decimal LYMXNSales { get; set; }
            public decimal CYMXNSales { get; set; }

            public decimal LYMonthlySpend { get; set; }
            public decimal CYCurrentSpend { get; set; }
            public decimal LYCostPercentage { get; set; }
        }

        public class OnlineDailySalesAndMarketing
        {
            public DateTime Date { get; set; }
            public long PointOfSaleID { get; set; }
            public string PointOfSale { get; set; }
            public decimal USDSales { get; set; }
            public decimal MXNSales { get; set; }
            public decimal USDTotalSales { get; set; }
            public decimal USDMarketingSpend { get; set; }
            public decimal ExchangeRate { get; set; }
            public int ConfirmedCoupons { get; set; }
            public List<long> ConfirmedCouponsIDs { get; set; }
            public int Customers { get; set; }
            public List<Guid> CustomerIDs { get; set; }
        }
    }

    public class PendingCache
    {
        public List<PendingTerminalCache> PendingCacheTerminals { get; set; }
        public string Purchase_ServiceIDs { get; set; }

        public class PendingTerminalCache
        {
            public long TerminalID { get; set; }
            public string Terminal { get; set; }
            public List<PendingCloseOutCache> PendingCloseOuts { get; set; }
        }       

        public class PendingCloseOutCache
        {
            public long CloseOutID { get; set; }
            public DateTime CloseOutDate { get; set; }
            public string PointOfSale { get; set; }
            public int NotCachedCoupons { get; set; }
            public int OutOfDateCoupons { get; set; }
            //public List<PendingCouponCache> OutOfDateCoupons { get; set; }
        }

        public class PendingCouponCache
        {
            public long Purchase_ServiceID { get; set; }
            public string Folio { get; set; }
            public string CacheStatus { get; set; }
        }

        public class CouponsQuery
        {
            public long closeOutID { get; set; }
            public long purchase_ServiceID { get; set; }
            public string folio { get; set; }
            public DateTime? cancelationDateTime { get; set; }
            public DateTime? auditDate { get; set; }
            public DateTime? dateSaved { get; set; }
            public DateTime? dateLastUpdate { get; set; }
            public DateTime? dateGenerated { get; set; }
        }
    }

    public class PendingCloseOuts
    {
        public List<PendingAgentCloseOuts> PendingAgentCOs { get; set; }

        public class PendingAgentCloseOuts
        {
            public Guid AgentID { get; set; }
            public string Agent { get; set; }
            public List<CloseOutItem> PendingCloseOuts { get; set; }
        }

        public class CloseOutItem {
            public int PointOfSaleID { get; set; }
            public string PointOfSale { get; set; }
            public string CloseOutDate { get; set; }
            public List<CouponItem> PendingCoupons { get; set; }
            public List<RefundItem> PendingRefunds { get; set; }
            public List<PaymentItem> PendingPayments { get; set; }
        }

        public class CouponItem
        {
            public string Folio { get; set; }
            public string Customer { get; set; }
            public string Service { get; set; }
            public string Status { get; set; }
        }

        public class RefundItem
        {
            public Money Refund { get; set; }
            public string Customer { get; set; }
            public string Type { get; set; }
        }

        public class PaymentItem
        {
            public string Purchase { get; set; }
            public string Amount { get; set; }
        }
    }

    public class HostessStatus
    {
        public List<ProgramStatus> Programs { get; set; }

        public class ProgramStatus
        {
            public int? ProgramID { get; set; }
            public string Program { get; set; }
            public int Reservations { get; set; }
            public int CheckedIn { get; set; }
            public decimal CheckedInPercentage { get; set; }
            public List<BookingStatusItem> Status { get; set; }
        }

        public class BookingStatusItem
        {
            public int BookingStatusID { get; set; }
            public string BookingStatus { get; set; }
            public int Amount { get; set; }
            public decimal Percentage { get; set; }
        }

        public class ArrivalStatus
        {
            public Guid arrivalID { get; set; }
            public int? programID { get; set; }
            public string program { get; set; }
            public string reservationStatus { get; set; }
            public int? bookingStatusID { get; set; }
            public string bookingStatus { get; set; }
        }
    }

    public class ExchangeRatesStatus
    {
        public List<TerminalExchangeRates> TerminalsExchanges { get; set; }

        public class TerminalExchangeRates
        {
            public string Terminal { get; set; }
            public List<ExchangeRateItem> ExchangeRates { get; set; }
        }

        public class ExchangeRateItem
        {

            public long ExchangeRateID { get; set; }
            public decimal ExchangeRate { get; set; }
            public string Vigency { get; set; }
            public string FromCurrencyCode { get; set; }
            public string ToCurrencyCode { get; set; }
            public string ExchangeType { get; set; }
            public string PoS { get; set; }
            public double DaysToExpire { get; set; }
        }
    }

    public class AvailabilityStatus
    {
        public List<TerminalAvailability> TerminalsAvailability { get; set; }

        public class TerminalAvailability
        {
            public string Terminal { get; set; }
            public List<AvailabilityItem> Availability { get; set; }
            public List<PriceItem> Prices { get; set; }
        }
        
        public class AvailabilityItem
        {
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public string Provider { get; set; }
            public int Descriptions { get; set; }
            public long WeeklyAvailabilityID { get; set; }
            public string Hour { get; set; }
            public string Vigency { get; set; }
            public double DaysToExpire { get; set; }
        }

        public class PriceItem
        {
            public long ServiceID { get; set; }
            public string Service { get; set; }
            public string Provider { get; set; }
            public int Descriptions { get; set; }
            public long PriceID { get; set; }
            public string Price { get; set; }
            public string Vigency { get; set; }
            public double DaysToExpire { get; set; }
        }
    }
}
