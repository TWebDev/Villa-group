using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.ViewModels
{
    public class MarketPlaceViewModel
    {
        public class Category
        {
            public long CategoryID { get; set; }
            public string CategoryName { get; set; }
        }

        public class Settings
        {
            public int CatalogID { get; set; }
            public int PointOfSaleID { get; set; }
            public long TerminalID { get; set; }
        }

        public class ItemDetail
        {
            public long ItemID { get; set; }
            public int? ItemTypeID { get; set; }
            public string ItemName { get; set; }

            public string Length { get; set; }
            public string Description { get; set; }
            public string Itinerary { get; set; }
            public string Included { get; set; }
            public string Recommendations { get; set; }
            public string Notes { get; set; }
            public string Restrictions { get; set; }
            public IEnumerable<PriceListItem> Prices { get; set; }
            public IEnumerable<ScheduleListItem> Schedules { get; set; }
            public IEnumerable<PictureListItem> Pictures { get; set; }
            public decimal? Rating { get; set; }
            public string Provider { get; set; }
            public bool ApplyForMultipleDates { get; set; }
        }
        public class Item
        {
            public long ItemID { get; set; }
            public string ItemName { get; set; }
            public int? ItemTypeID { get; set; }
            public string Provider { get; set; }
            public decimal? RetailPrice { get; set; }
            public decimal? Price { get; set; }
            public int Savings { get; set; }
            public string CurrencyCode { get; set; }
            public List<PictureListItem> Image { get; set; }
            public decimal? Rating { get; set; }
            public string Length { get; set; }
            public bool ApplyForMultipleDates { get; set; }
        }

        public class ItemSchedule
        {
            public long WeeklyAvailabilityID { get; set; }
            public string Schedule { get; set; }
        }

        public class Cart
        {
            public string ConfirmationNumber { get; set; }
            public long TerminalID { get; set; }
            public long PlaceID { get; set; }
            public int PointOfSaleID { get; set; }
            public decimal Total { get; set; }
            public int CurrencyID { get; set; }
            public string Language { get; set; }
            public List<CartItem> Items { get; set; }
            public string IPAddress { get; set; }
            public string Browser { get; set; }
            public string OS { get; set; }
        }

        public class CartItem
        {
            public long? CartItemID { get; set; }
            public long ItemID { get; set; }
            public string Item { get; set; }
            public int? ItemTypeID { get; set; }
            public List<PictureListItem> Picture { get; set; }
            public string Length { get; set; }
            public string ReservedFor { get; set; }
            public string Provider { get; set; }
            public string ItemDate { get; set; }
            public long? WeeklyAvailabilityID { get; set; }
            public string Schedule { get; set; }
            public decimal Total { get; set; }
            public bool Deleted { get; set; }
            public DateTime? DateSaved { get; set; }
            public List<CartItemDetail> Details { get; set; }
            public bool ApplyForMultipleDates { get; set; }
        }

        public class CartItemDetail
        {
            public long PriceID { get; set; }
            public int PriceTypeID { get; set; }
            public long? ExchangeRateID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public string Unit { get; set; }
            public bool Deleted { get; set; }
        }
    }
}
