using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Models.DataModels
{
    public class MarketPlaceDataModel
    {
        public static List<MarketPlaceViewModel.Category> GetCategories(int catalogID, string culture)
        {
            List<MarketPlaceViewModel.Category> categories = new List<MarketPlaceViewModel.Category>();
            ePlatEntities db = new ePlatEntities();

            var categoryIDs = (from c in db.tblCategories
                               where c.catalogID == catalogID
                               && c.active == true
                               && c.showOnWebsite == true
                               select c.categoryID).ToList();

            categories = (from x in db.tblCategoryDescriptions
                          where categoryIDs.Contains(x.categoryID)
                          select new MarketPlaceViewModel.Category()
                          {
                              CategoryID = x.categoryID,
                              CategoryName = x.category
                          }).ToList();

            return categories;
        }

        public static MarketPlaceViewModel.ItemDetail GetItem(long id, string culture, int currencyID, int pointOfSaleID)
        {
            MarketPlaceViewModel.ItemDetail item = new MarketPlaceViewModel.ItemDetail();
            ePlatEntities db = new ePlatEntities();

            var itemQ = (from a in db.tblServices
                         join description in db.tblServiceDescriptions
                         on a.serviceID equals description.serviceID
                         into serviceDescription
                         from description in serviceDescription.DefaultIfEmpty()
                         join provider in db.tblProviders
                         on a.providerID equals provider.providerID
                         where a.serviceID == id
                         && description.culture == culture
                         && a.deleted != true
                         && description.active == true
                         select new
                         {
                             a.serviceID,
                             a.originalTerminalID,
                             a.transportationService,
                             description.service,
                             a.itemTypeID,
                             a.length,
                             description.fullDescription,
                             description.itinerary,
                             description.includes,
                             description.recommendations,
                             description.notes,
                             description.policies,
                             provider.comercialName,
                             a.applyWholeStay
                         }).FirstOrDefault();

            if (itemQ != null)
            {
                item.ItemID = itemQ.serviceID;
                item.ItemTypeID = itemQ.itemTypeID;
                item.ItemName = itemQ.service;
                item.Length = (itemQ.length / 60) + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.hours + (itemQ.length % 60 > 0 ? " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.and + " " + itemQ.length % 60 + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.minutes : "");
                item.Description = FilterHtml(itemQ.fullDescription);
                item.Itinerary = FilterHtml(itemQ.itinerary);
                item.Included = FilterHtml(itemQ.includes);
                item.Recommendations = FilterHtml(itemQ.recommendations);
                item.Notes = FilterHtml(itemQ.notes);
                item.Restrictions = FilterHtml(itemQ.policies);
                item.Provider = itemQ.comercialName;
                item.ApplyForMultipleDates = itemQ.applyWholeStay;

                ActivityDataModel adm = new ActivityDataModel();
                long terminalid = 0;
                item.Prices = new List<PriceListItem>();

                if (terminalid == null)
                {
                    terminalid = itemQ.originalTerminalID;
                }
                DateTime nextActivePriceDate = adm.GetNextActivePriceDate(id, terminalid);
                item.Prices = GetPrices(id, nextActivePriceDate, pointOfSaleID, culture, currencyID);

                //schedules, 
                item.Schedules = adm.GetSchedules(id);
                //pictures, 
                item.Pictures = PictureDataModel.GetPictures(1, id);
                //reviews
                IEnumerable<ReviewListItem> reviews = adm.GetReviews(1, id);

                if (reviews.Count() > 0)
                {
                    decimal points = 0;
                    foreach (ReviewListItem r in reviews)
                    {
                        points += r.Rating;
                    }
                    item.Rating = points / reviews.Count();
                }
                else
                {
                    item.Rating = null;
                }
            }

            return item;
        }

        public static List<PriceListItem> GetPrices(long itemID, DateTime itemDate, int pointOfSaleID, string culture, int currencyID)
        {
            List<PriceListItem> prices = new List<PriceListItem>();
            ePlatEntities db = new ePlatEntities();

            int? retailPriceTypeID = null;
            int? offerPriceTypeID = null;
            List<int> priceTypes = new List<int>();

            var PoS = (from p in db.tblPointsOfSale
                       where p.pointOfSaleID == pointOfSaleID
                       select new
                       {
                           p.retailPriceTypeID,
                           p.offerPriceTypeID,
                           p.mxnOfferPriceTypeID
                       }).FirstOrDefault();

            if (PoS != null)
            {
                retailPriceTypeID = PoS.retailPriceTypeID;
                if (retailPriceTypeID != null)
                {
                    priceTypes.Add((int)retailPriceTypeID);
                }
                if (currencyID == 2)
                {
                    offerPriceTypeID = PoS.mxnOfferPriceTypeID;
                    if (offerPriceTypeID == null)
                    {
                        offerPriceTypeID = PoS.offerPriceTypeID;
                    }
                }
                else
                {
                    offerPriceTypeID = PoS.offerPriceTypeID;
                }

                if (offerPriceTypeID != null)
                {
                    priceTypes.Add((int)offerPriceTypeID);
                }
            }

            var itemTerminal = (from s in db.tblServices
                        where s.serviceID == itemID
                        select s.originalTerminalID).FirstOrDefault();

            var ComputedPrices = PriceDataModel.GetComputedPrices(itemID, itemDate, 99999, itemTerminal, DateTime.Now, culture);
            var PricesQ = from p in ComputedPrices
                          where priceTypes.Contains(p.PriceTypeID)
                          && p.CurrencyID == currencyID
                          orderby p.PriceID ascending, p.PriceTypeID ascending
                          select new
                          {
                              p.PriceID,
                              p.Price,
                              p.PriceTypeID,
                              p.Unit,
                              p.AdditionalInfo,
                              p.Min,
                              p.Max,
                              p.CurrencyCode,
                              p.ToTransportationZoneID,
                              p.FromTransportationZoneID,
                              p.ToTransportationZone,
                              p.SysItemTypeID,
                              p.ExchangeRateID,
                              p.DependingOnPriceID,
                              p.DependingOnPriceQuantity,
                              p.Highlight
                          };

            decimal retail = 0;
            foreach (var price in PricesQ)
            {

                if (price.PriceTypeID == retailPriceTypeID)
                {
                    //retail
                    retail = decimal.Round(price.Price, 2, MidpointRounding.AwayFromZero);
                }
                else if (price.PriceTypeID == offerPriceTypeID)
                {
                    //offer
                    var unit = PriceDataModel.GetUnit(price.PriceID, culture);
                    PriceListItem newPrice = new PriceListItem()
                    {
                        PriceID = price.PriceID,
                        Unit = price.Unit + " " + price.AdditionalInfo,
                        UnitMin = price.Min,
                        UnitMax = price.Max,
                        RetailPrice = retail,
                        OfferPrice = decimal.Round(price.Price, 2, MidpointRounding.AwayFromZero),
                        Currency = price.CurrencyCode,
                        Savings = Convert.ToInt32(retail > 0 ? (100 - (price.Price * 100 / retail)) : 0),
                        PriceTypeID = price.PriceTypeID,
                        ExchangeRateID = price.ExchangeRateID,
                        DependingOnPriceID = price.DependingOnPriceID,
                        DependingOnPriceQuantity = price.DependingOnPriceQuantity,
                        Highlight = price.Highlight
                    };

                    prices.Add(newPrice);
                    
                    retail = 0;
                }
            }

            prices = prices.OrderByDescending(x => x.OfferPrice).ToList();

            return prices;
        }

        public static string FilterHtml(string str)
        {
            if (str != null)
            {
                str = str.Replace("\r", "");
                str = str.Replace("\n", "");
                str = str.Replace("\t", "");
            }
            return str;
        }

        public static List<MarketPlaceViewModel.Item> GetItems(long categoryID, string culture, int currencyID, int pointOfSaleID)
        {
            List<MarketPlaceViewModel.Item> items = new List<MarketPlaceViewModel.Item>();
            ePlatEntities db = new ePlatEntities();

            var ItemsQ = (from a in db.tblCategories_Services
                          join service in db.tblServices on a.serviceID equals service.serviceID
                          join provider in db.tblProviders on service.providerID equals provider.providerID
                          join description in db.tblServiceDescriptions on service.serviceID equals description.serviceID
                          into serviceDescription
                          from description in serviceDescription.DefaultIfEmpty()
                          where a.categoryID == categoryID
                          && provider.isActive == true
                          && service.deleted != true
                          && description.active && description.culture == culture
                          select new
                          {
                              ItemID = a.serviceID,
                              ItemName = description.service,
                              ItemType = service.itemTypeID,
                              Provider = provider.comercialName,
                              Length = service.length,
                              WholeStay = service.applyWholeStay
                          }).Distinct();

            foreach (var currentItem in ItemsQ)
            {
                if (currentItem.ItemName != null)
                {
                    MarketPlaceViewModel.Item item = new MarketPlaceViewModel.Item();
                    item.ItemID = currentItem.ItemID;
                    item.ItemName = currentItem.ItemName;
                    item.ItemTypeID = currentItem.ItemType;
                    item.Image = PictureDataModel.GetMainPictureItem(1, currentItem.ItemID);
                    item.Rating = GetRating(1, currentItem.ItemID);
                    decimal? retailPrice = 0;
                    decimal? price = 0;
                    int savings = 0;
                    GetPricesFromCache(categoryID, currentItem.ItemID, currencyID, culture, pointOfSaleID, ref retailPrice, ref price, ref savings, ref db);
                    item.RetailPrice = retailPrice;
                    item.Price = price;
                    item.Savings = savings;
                    item.CurrencyCode = (currencyID == 1 ? "USD" : "MXN");
                    item.Provider = currentItem.Provider;
                    item.Length = (currentItem.Length / 60) + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.hours + (currentItem.Length % 60 > 0 ? " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.and + " " + currentItem.Length % 60 + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.minutes : "");
                    items.Add(item);
                    item.ApplyForMultipleDates = currentItem.WholeStay;
                }
            }

            items = items.OrderBy(a => a.Price).ToList();

            return items;
        }

        public static decimal? GetRating(int itemTypeID, long id)
        {
            ePlatEntities db = new ePlatEntities();
            decimal? rating = null;
            string culture = Utils.GeneralFunctions.GetCulture();
            var RevsQ = from r in db.tblReviews
                        where r.sysItemTypeID == itemTypeID
                        && r.itemID == id
                        && r.active == true
                        && r.culture == culture
                        select r.rating;

            if (RevsQ.Count() > 0)
            {
                decimal points = 0;
                foreach (decimal r in RevsQ)
                {
                    points += r;
                }
                rating = points / RevsQ.Count();
            }

            return rating;
        }

        public static void GetPricesFromCache(long categoryID, long itemID, int currencyID, string culture, int pointOfSaleID, ref decimal? retailPrice, ref decimal? price, ref int savings, ref ePlatEntities db)
        {
            decimal? offerCheapest = 0;
            decimal? retailCheapest = 0;

            //obtener tipos de precio disponibles para el dominio
            int? retailPriceTypeID = null;
            int? offerPriceTypeID = null;
            List<int> priceTypes = new List<int>();

            var PoS = (from p in db.tblPointsOfSale
                       where p.pointOfSaleID == pointOfSaleID
                       select new
                       {
                           p.retailPriceTypeID,
                           p.offerPriceTypeID,
                           p.mxnOfferPriceTypeID
                       }).FirstOrDefault();

            if (PoS != null)
            {
                retailPriceTypeID = PoS.retailPriceTypeID;
                if (retailPriceTypeID != null)
                {
                    priceTypes.Add((int)retailPriceTypeID);
                }
                if (culture == "es-MX")
                {
                    offerPriceTypeID = PoS.mxnOfferPriceTypeID;
                    if (offerPriceTypeID == null)
                    {
                        offerPriceTypeID = PoS.offerPriceTypeID;
                    }
                }
                else
                {
                    offerPriceTypeID = PoS.offerPriceTypeID;
                }
                if (offerPriceTypeID != null)
                {
                    priceTypes.Add((int)offerPriceTypeID);
                }
            }

            var PricesQ = from p in db.tblServices_PricesCache
                          where p.serviceID == itemID
                          && p.price > 0
                          && priceTypes.Contains(p.priceTypeID)
                          && p.currencyID == currencyID
                          && !p.unit.ToLower().Contains("hild")
                          && !p.unit.ToLower().Contains("kid")
                          && !p.unit.ToLower().Contains("iño")
                          && !p.unit.ToLower().Contains("menor")
                          && p.dependingOnPriceID == null
                          orderby p.unit, p.priceTypeID descending
                          select new
                          {
                              p.price,
                              p.priceTypeID,
                              p.unit
                          };

            if (PricesQ.Count() > 0)
            {
                foreach (var currentPrice in PricesQ)
                {
                    if (currentPrice.priceTypeID == offerPriceTypeID)
                    {
                        if (offerCheapest == 0 || currentPrice.price < offerCheapest)
                        {
                            price = currentPrice.price;
                            offerCheapest = price;
                        }
                    }
                    else if (currentPrice.priceTypeID == retailPriceTypeID)
                    {
                        if (retailCheapest == 0 || currentPrice.price < retailCheapest)
                        {
                            retailPrice = currentPrice.price;
                            retailCheapest = retailPrice;
                        }
                    }
                }
            }
            else
            {
                var PricesChildQ = (from p in db.tblServices_PricesCache
                                    where p.serviceID == itemID
                                    && p.price > 0
                                    && priceTypes.Contains(p.priceTypeID)
                                    && p.currencyID == currencyID
                                    orderby p.price ascending, p.priceTypeID descending
                                    select new
                                    {
                                        p.price,
                                        p.priceTypeID
                                    }).Take(2);

                foreach (var currentPrice in PricesChildQ)
                {
                    if (currentPrice.priceTypeID == offerPriceTypeID)
                    {
                        if (offerCheapest == 0 || currentPrice.price < offerCheapest)
                        {
                            price = currentPrice.price;
                            offerCheapest = price;
                        }
                    }
                    else if (currentPrice.priceTypeID == retailPriceTypeID)
                    {
                        if (retailCheapest == 0 || currentPrice.price < retailCheapest)
                        {
                            retailPrice = currentPrice.price;
                            retailCheapest = retailPrice;
                        }
                    }
                }
            }

            savings = Convert.ToInt32(retailPrice > 0 ? (100 - (price * 100 / retailPrice)) : 0);
        }

        public static MarketPlaceViewModel.Settings GetSettings(int placeID, int terminalTypeID)
        {
            MarketPlaceViewModel.Settings settings = new MarketPlaceViewModel.Settings();
            ePlatEntities db = new ePlatEntities();

            var terminals = (from t in db.tblTerminals
                            where t.terminalTypeID == terminalTypeID
                            select t.terminalID).ToList();

            var catalogQ = (from c in db.tblCatalogs_Places
                            join t in db.tblCatalogs_Terminals
                            on c.catalogID equals t.catalogID
                            into c_t
                            from t in c_t.DefaultIfEmpty()
                            where c.placeID == placeID
                            && terminals.Contains(t.terminalID)
                            select new { c.catalogID }).FirstOrDefault();

            if (catalogQ != null)
            {
                settings.CatalogID = catalogQ.catalogID;
            }

            var pointOfSaleQ = (from p in db.tblPointsOfSale
                                where p.placeID == placeID
                                && terminals.Contains(p.terminalID)
                                select new { p.pointOfSaleID, p.terminalID }).FirstOrDefault();

            if (pointOfSaleQ != null)
            {
                settings.PointOfSaleID = pointOfSaleQ.pointOfSaleID;
                settings.TerminalID = pointOfSaleQ.terminalID;
            }

            return settings;
        }
    }
}
