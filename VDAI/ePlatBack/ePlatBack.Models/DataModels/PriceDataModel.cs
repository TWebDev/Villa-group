
using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.DataModels
{
    public class PriceDataModel
    {
        public static UserSession session = new UserSession();
        public class PricesCatalogs
        {
            public static List<SelectListItem> FillDrpPrices(string itemType, int itemID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                if (itemID != 0)
                {
                    //foreach (var i in db.tblPrices.Where(m => m.itemID == itemID).Where(m => m.tblSysItemTypes.sysItemType == itemType).Select(m => m))
                    foreach (var i in db.tblPrices.Where(m => m.itemID == itemID && m.tblSysItemTypes.sysItemType == itemType).Select(m => m).OrderBy(m => m.price))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.priceID.ToString(),
                            Text = i.price.ToString()
                        });
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpPriceTypes(long? terminalID = null)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> pt = new List<SelectListItem>();
                List<long> terminals = new List<long>();
                if (terminalID == null)
                {
                    terminals = session.Terminals != "" ?
                        session.Terminals.Split(',').Select(m => long.Parse(m)).ToList() :
                        session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToList();
                }
                else
                {
                    terminals.Add((long)terminalID);
                }

                foreach (var i in db.tblServices_PriceTypes.Where(x => terminals.Contains(x.terminalID) && x.providerID == null).Select(x => new { x.priceTypeID, x.tblPriceTypes.priceType, x.tblPriceTypes.order_ }).Distinct().OrderBy(m => m.order_))
                {
                    pt.Add(new SelectListItem()
                    {
                        Value = i.priceTypeID.ToString(),
                        Text = i.priceType.ToString() + " (P" + i.priceTypeID + ")"
                    });
                }
                if (pt.Count() == 0)
                {
                    foreach (var i in db.tblPriceTypes.Where(m => m.priceTypeID <= 2).OrderBy(m => m.order_))
                    {
                        pt.Add(new SelectListItem()
                        {
                            Value = i.priceTypeID.ToString(),
                            Text = i.priceType + " (P" + i.priceTypeID + ")"
                        });
                    }
                }
                return pt;
            }

            //public static List<SelectListItem> FillDrpItemTypes()
            //{
            //    ePlatEntities db = new ePlatEntities();
            //    List<SelectListItem> it = new List<SelectListItem>();
            //    it.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
            //    foreach (var i in db.tblSysItemTypes)
            //    {
            //        it.Add(new SelectListItem()
            //        {
            //            Value = i.sysItemTypeID.ToString(),
            //            Text = i.sysItemType.ToString()
            //        });
            //    }
            //    return it;
            //}

            public static List<SelectListItem> FillDrpPriceClasifications()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> pc = new List<SelectListItem>();
                pc.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPriceClasifications.OrderBy(m => m.priceClasification))
                {
                    pc.Add(new SelectListItem()
                    {
                        Value = i.priceClasificationID.ToString(),
                        Text = i.priceClasification.ToString()
                    });
                }
                return pc;
            }

            public static List<SelectListItem> FillDrpCurrencies(int? providerID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                //c.Add(new SelectListItem() { Value = "", Text = "--Select One--", Selected = false });
                //if (providerID == null)
                //{
                //    foreach (var i in db.tblCurrencies)
                //    {
                //        list.Add(new SelectListItem()
                //        {
                //            Value = i.currencyCode.ToString(),
                //            Text = i.currency.ToString()
                //        });
                //    }
                //}
                var contractCurrency = providerID != null ? db.tblProviders.Single(m => m.providerID == providerID).contractCurrencyID : null;
                var currencies = contractCurrency != null ? db.tblCurrencies.Where(m => m.currencyID == contractCurrency).Select(m => new { m.currencyCode, m.currency }) : db.tblCurrencies.Select(m => new { m.currencyCode, m.currency });

                foreach (var i in currencies)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.currencyCode.ToString(),
                        Text = i.currency.ToString()
                    });
                }

                return list;
            }

            public static List<SelectListItem> FillDrpTransportationZones()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> z = new List<SelectListItem>();
                z.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblTransportationZones.OrderBy(m => m.tblDestinations.destination))
                {
                    z.Add(new SelectListItem()
                    {
                        Value = i.transportationZoneID.ToString(),
                        Text = i.tblDestinations.destination + " - " + i.transportationZone.ToString()
                    });
                }
                return z;
            }

            public static List<SelectListItem> FillDrpCultures()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var i in db.tblLanguages)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.culture,
                        Text = i.language
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpGenericUnits()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var i in db.tblGenericUnits.OrderBy(m => m.unit))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.genericUnitID.ToString(),
                        Text = i.unit
                    });
                }

                return list;
            }

            public static bool IsPriceOpen(long? priceID)
            {
                ePlatEntities db = new ePlatEntities();
                if (priceID != null)
                {
                    var toDate = db.tblPrices.Single(m => m.priceID == (long)priceID).toDate;
                    if (toDate == (DateTime?)null || toDate > DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            public static string IsPriceInUse(PriceInfoModel model, int? priceID = null)
            {
                ePlatEntities db = new ePlatEntities();
                var _priceID = priceID ?? model.PriceInfo_PriceID;

                if (db.tblPurchaseServiceDetails.Where(m => (m.priceID == _priceID || m.netPriceID == _priceID)).Count() > 0)
                {
                    var _price = db.tblPrices.Single(m => m.priceID == _priceID);
                    var _priceUnits = db.tblPriceUnits.Where(m => m.priceID == _priceID);
                    var _genericUnit = model.PriceInfo_GenericUnit != "null" && model.PriceInfo_GenericUnit != null ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;

                    if (model.PriceInfo_PriceType != _price.priceTypeID || model.PriceInfo_Price != _price.price || model.PriceInfo_Currency != _price.tblCurrencies.currencyCode
                        || DateTime.Parse(model.PriceInfo_FromDate).Date != _price.fromDate.Value.Date || _genericUnit != _price.genericUnitID)
                    {
                        return "in use";
                    }
                    else
                    {
                        var result = "not modified";
                        foreach (var i in model.PriceInfo_PriceUnits)
                        {
                            if (i.PriceUnitInfo_PriceUnitID == 0)
                            {
                                result = "units";
                            }
                        }
                        //foreach (var _unit in _priceUnits)
                        //{
                        //    var _modelUnit = model.PriceInfo_PriceUnits.Where(m => m.PriceUnitInfo_PriceUnitID == _unit.priceUnitID);
                        //    if (_modelUnit.Count() > 0)
                        //    {
                        //        if (_modelUnit.FirstOrDefault().PriceUnitInfo_Unit != _unit.unit || _modelUnit.FirstOrDefault().PriceUnitInfo_Min != _unit.min || _modelUnit.FirstOrDefault().PriceUnitInfo_Max != _unit.max)
                        //        {
                        //            result = "units";
                        //        }
                        //    }
                        //}
                        if (model.PriceInfo_IsPermanent != _price.permanent_ || (model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate).Date != _price.toDate.Value.Date : _price.toDate != null))
                        {
                            return "try to close";
                        }
                        return result;
                    }
                }
                else
                {
                    return "not in use";
                }
            }

            public static string IsPriceInUse(PriceWizardModel model, int? priceID = null)
            {
                ePlatEntities db = new ePlatEntities();
                var _priceID = priceID ?? model.PriceWizard_Unit;

                if (db.tblPurchaseServiceDetails.Where(m => (m.priceID == _priceID || m.netPriceID == _priceID)).Count() > 0)
                {
                    return "in use";
                    //var _price = db.tblPrices.Single(m => m.priceID == _priceID);
                    //var _priceUnits = db.tblPriceUnits.Where(m => m.priceID == _priceID);
                    //var _genericUnit = model.PriceWizard_GenericUnit != "null" && model.PriceWizard_GenericUnit != null ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;

                    //if (model.PriceWizard_PriceType != _price.priceTypeID || model.PriceWizard_Price != _price.price || model.PriceWizard_Currency != _price.tblCurrencies.currencyCode
                    //    || DateTime.Parse(model.PriceWizard_StartBookingDate).Date != _price.fromDate.Value.Date || _genericUnit != _price.genericUnitID)
                    //{
                    //    return "in use";
                    //}
                    //else
                    //{
                    //    var result = "not modified";
                    //    foreach (var i in new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits))
                    //    {
                    //        if (i.PriceUnitInfo_PriceUnitID == 0)
                    //        {
                    //            result = "units";
                    //        }
                    //    }

                    //    if (model.PriceInfo_IsPermanent != _price.permanent_ || (model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate).Date != _price.toDate.Value.Date : _price.toDate != null))
                    //    {
                    //        return "try to close";
                    //    }
                    //    return result;
                    //}
                }
                else
                {
                    return "not in use";
                }
            }

            public static string _IsPriceInUse(PriceInfoModel model, int? priceID = null)
            {
                ePlatEntities db = new ePlatEntities();
                var _priceID = priceID ?? model.PriceInfo_PriceID;
                if (db.tblPurchaseServiceDetails.Where(m => (m.priceID == _priceID || m.netPriceID == _priceID)).Count() > 0)
                {
                    var price = db.tblPrices.Single(m => m.priceID == _priceID);

                    if (model.PriceInfo_PriceType != price.priceTypeID
                        || model.PriceInfo_Currency != price.tblCurrencies.currencyCode
                        || DateTime.Parse(model.PriceInfo_FromDate).Date != price.fromDate.Value.Date
                        || model.PriceInfo_Price != price.price)
                    {
                        return "in use";
                    }
                    else
                    {
                        //if (model.PriceInfo_IsPermanent != price.permanent_
                        //    || (model.PriceInfo_ToDate == null || DateTime.Parse(model.PriceInfo_ToDate).Date != price.toDate.Value.Date))
                        if (model.PriceInfo_IsPermanent != price.permanent_
                            || ((DateTime?)DateTime.Parse(model.PriceInfo_ToDate) != price.toDate))
                        {
                            return "try to close";
                        }
                        return "in use";
                    }
                }
                else
                {
                    return "not in use";
                }
            }

            public static object PriceAlreadyExists(PriceInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                var _prices = db.tblPrices.Where(m => m.itemID == model.PriceInfo_ItemID && m.tblSysItemTypes.sysItemType == model.PriceInfo_ItemType
                    && m.terminalID == model.PriceInfo_Terminal && m.priceTypeID == model.PriceInfo_PriceType
                    && m.tblCurrencies.currencyCode == model.PriceInfo_Currency).Select(m => m);

                if (_prices.Count() > 0)
                {
                    var twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate);
                    var twToDate = model.PriceInfo_TWToDate != null && !model.PriceInfo_TWPermanent ? DateTime.Parse(model.PriceInfo_TWToDate).AddDays(1).AddSeconds(-1) : DateTime.MaxValue;
                    var fromDate = DateTime.Parse(model.PriceInfo_FromDate);
                    var toDate = model.PriceInfo_ToDate != null && !model.PriceInfo_IsPermanent ? DateTime.Parse(model.PriceInfo_ToDate).AddDays(1).AddSeconds(-1) : DateTime.MaxValue;

                    //                    _prices = _prices.Where(m => (m.twFromDate <= twFromDate && (m.twPermanent_ || m.twToDate >= twToDate))
                    //|| (m.twFromDate > twFromDate && (model.PriceInfo_TWPermanent || m.twToDate < twToDate))
                    //|| (twFromDate < m.twFromDate && (model.PriceInfo_TWPermanent || twToDate > m.twFromDate))
                    //|| (twFromDate > m.twFromDate && (model.PriceInfo_TWPermanent || twToDate > m.twToDate)));

                    _prices = _prices.Where(m => (m.twFromDate < twFromDate && twFromDate < twToDate)
                        || (m.twFromDate < twToDate && twToDate <= (m.toDate ?? DateTime.MaxValue))
                        || (twFromDate < m.twFromDate && m.twFromDate < twToDate)
                        || (twFromDate < (m.twToDate ?? DateTime.MaxValue) && (m.twToDate ?? DateTime.MaxValue) <= twToDate));

                    _prices = _prices.Where(m => (m.fromDate < fromDate && toDate < (m.toDate ?? DateTime.MaxValue))
                        || (m.fromDate < toDate && toDate <= (m.toDate ?? DateTime.MaxValue))
                        || (fromDate < m.fromDate && m.fromDate < toDate)
                        || (fromDate < (m.toDate ?? DateTime.MaxValue) && (m.toDate ?? DateTime.MaxValue) <= toDate));

                    //_prices = _prices.Where(m => m.twPermanent_ || m.twToDate >= m.toDate);
                    _prices = _prices.Where(m => m.twToDate >= m.toDate || m.twToDate == null || m.toDate == null);

                    var list = _prices.ToList();
                    foreach (var _price in _prices)
                    {
                        var units = _price.tblPriceUnits.Where(x => x.culture == (model.PriceInfo_Currency == "MXN" ? "es-MX" : "en-US"));
                        if (units.Count() == 0 || model.PriceInfo_PriceUnits.Where(m => m.PriceUnitInfo_Unit.Trim().ToLower() == units.FirstOrDefault().unit.Trim().ToLower()).Count() == 0)
                        {
                            //prices = _prices.Where(m => m != _price);
                            list.Remove(_price);
                        }
                    }


                    //if (_prices.Count() == 0)
                    if (list.Count() == 0)
                    {
                        return new { exists = false, priceID = 0 };
                    }
                    else
                    {
                        return new { exists = true, prices = list.Select(m => new { m.priceID, m.price, m.tblCurrencies.currencyCode, priceUnit = m.tblPriceUnits.Where(x => x.culture == (model.PriceInfo_Currency == "MXN" ? "es-MX" : "en-US")).Select(x => new { x.unit, x.min, x.max }).FirstOrDefault() }) };
                        //return new { exists = true, prices = _prices.Select(m => new { m.priceID, m.price, m.tblCurrencies.currencyCode, priceUnit = m.tblPriceUnits.Where(x => x.culture == (model.PriceInfo_Currency == "MXN" ? "es-MX" : "en-US")).Select(x => new { x.unit, x.min, x.max }).FirstOrDefault() }) };
                    }
                }
                else
                {
                    return new { exists = false, priceID = 0 };
                }
            }

            public static object _PriceAlreadyExists(PriceInfoModel model)
            {
                ePlatEntities db = new ePlatEntities();
                var _fromDate = model.PriceInfo_FromDate != null ? DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture) : (DateTime?)null;

                var _price = db.tblPrices.Where(m => m.tblCurrencies.currencyCode == model.PriceInfo_Currency && m.priceTypeID == model.PriceInfo_PriceType
                    && m.terminalID == model.PriceInfo_Terminal && ((m.permanent_ && m.fromDate <= DateTime.Today) || (!m.permanent_ && m.fromDate <= DateTime.Today && m.toDate >= DateTime.Today))
                    && m.itemID == model.PriceInfo_ItemID && m.tblSysItemTypes.sysItemType == model.PriceInfo_ItemType);

                if (_price.Count() > 0)
                {
                    return new { exists = true, prices = _price.Select(m => new { m.priceID, m.price, m.tblCurrencies.currencyCode, m.tblPriceUnits.FirstOrDefault(x => x.culture == (model.PriceInfo_Currency == "MXN" ? "es-MX" : "en-US")).unit, m.tblPriceUnits.FirstOrDefault(x => x.culture == (model.PriceInfo_Currency == "MXN" ? "es-MX" : "en-US")).min, m.tblPriceUnits.FirstOrDefault(x => x.culture == (model.PriceInfo_Currency == "MXN" ? "es-MX" : "en-US")).max }).ToList() };
                }
                else
                {
                    return new { exists = false, priceID = 0 };
                }
            }

            public static bool IsPriceActive(long priceID)
            {
                ePlatEntities db = new ePlatEntities();
                var _now = DateTime.Now;
                var _price = db.tblPrices.Single(m => m.priceID == priceID);
                //if ((_price.permanent_ && _price.fromDate <= DateTime.Today) || (!_price.permanent_ && _price.fromDate <= DateTime.Today && _price.toDate >= DateTime.Today))
                if ((_price.permanent_ && _price.fromDate <= _now) || (!_price.permanent_ && _price.fromDate <= _now && _price.toDate >= _now))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static List<SelectListItem> FillDrpUnits(long serviceID)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                //var computedPrices = GetComputedPrices(serviceID, null, db.tblServices.Single(m => m.serviceID == serviceID).originalTerminalID, true).Where(m => m.Base);
                var computedPrices = GetComputedPrices(serviceID, null, 99999, db.tblServices.Single(m => m.serviceID == serviceID).originalTerminalID).Where(m => m.Base);
                var pricesUnits = computedPrices.Select(m => new { m.PriceID, m.Unit, m.Price, m.CurrencyCode });

                foreach (var i in pricesUnits.OrderBy(m => m.Unit).ThenBy(m => m.CurrencyCode))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.PriceID.ToString(),
                        Text = i.Unit + " $" + i.Price + " " + i.CurrencyCode
                    });
                }
                return list;
            }
        }

        public static tblPrices GetPrice(long sysItemTypeID, long itemID, int priceTypeID, int priceClasificationID, DateTime date, string currencyCode)
        {
            ePlatEntities db = new ePlatEntities();
            long terminalid = Utils.GeneralFunctions.GetTerminalID();
            var packagePrice = (from p in db.tblPrices
                                where p.sysItemTypeID == sysItemTypeID
                                && p.itemID == itemID
                                && p.priceTypeID == priceTypeID
                                && p.priceClasificationID == priceClasificationID
                                && (p.permanent_ || p.fromDate <= date && p.toDate >= date)
                                && p.tblCurrencies.currencyCode == currencyCode
                                && p.terminalID == terminalid
                                select p).FirstOrDefault();

            if (packagePrice == null)
            {
                packagePrice = (from p in db.tblPrices
                                where p.sysItemTypeID == sysItemTypeID
                                && p.itemID == itemID
                                && p.priceTypeID == priceTypeID
                                && p.priceClasificationID == priceClasificationID
                                && (p.permanent_ || p.fromDate <= date && p.toDate >= date)
                                && p.tblCurrencies.currencyCode == currencyCode
                                && (p.terminalID == 1 || p.terminalID == 9)
                                select p).FirstOrDefault();
            }
            return packagePrice;
        }

        public List<GenericListModel> SearchPricesClasification(string Search_PriceClasifications)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from pc in db.tblPriceClasifications
                        where (Search_PriceClasifications == null || pc.priceClasification.Contains(Search_PriceClasifications))
                        select pc;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.priceClasificationID,
                    ItemName = i.priceClasification
                });
            }
            return list;
        }

        public List<GenericListModel> SearchCurrencies(string Search_Currencies)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = from c in db.tblCurrencies
                        where (Search_Currencies == null || c.currency.Contains(Search_Currencies))
                        select c;
            foreach (var i in query)
            {
                list.Add(new GenericListModel()
                {
                    ItemID = i.currencyID,
                    ItemName = i.currency,
                    Item2 = i.currencyCode
                });
            }
            return list;
        }

        public List<GenericListModel> GetPriceClasificationPerID(int priceClasificationID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();

            var query = (from pc in db.tblPriceClasifications where pc.priceClasificationID == priceClasificationID select pc).Single();

            list.Add(new GenericListModel()
            {
                ItemID = query.priceClasificationID,
                ItemName = query.priceClasification
            });
            return list;
        }

        public List<GenericListModel> GetCurrencyPerID(int currencyID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            var query = (from c in db.tblCurrencies where c.currencyID == currencyID select c).Single();
            list.Add(new GenericListModel()
            {
                ItemID = query.currencyID,
                ItemName = query.currency,
                Item2 = query.currencyCode
            });
            return list;
        }

        public AttemptResponse SavePriceClasification(PriceClasificationInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PriceClasification_PriceClasificationID != 0)
            {
                try
                {
                    tblPriceClasifications clasification = db.tblPriceClasifications.Single(m => m.priceClasificationID == model.PriceClasification_PriceClasificationID);
                    clasification.priceClasification = model.PriceClasification_PriceClasification;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Price Clasification Updated";
                    response.ObjectID = clasification.priceClasificationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Price Clasification NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPriceClasifications clasification = new tblPriceClasifications();
                    clasification.priceClasification = model.PriceClasification_PriceClasification;
                    db.tblPriceClasifications.AddObject(clasification);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Price Clasification Saved";
                    response.ObjectID = clasification.priceClasificationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Price Clasification NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveCurrency(CurrencyInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.CurrencyInfo_CurrencyID != 0)
            {
                try
                {
                    tblCurrencies currency = db.tblCurrencies.Single(m => m.currencyID == model.CurrencyInfo_CurrencyID);
                    currency.currency = model.CurrencyInfo_Currency;
                    currency.currencyCode = model.CurrencyInfo_CurrencyCode;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Currency Updated";
                    response.ObjectID = currency.currencyID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Currency NOT Updated";
                    response.Exception = ex;
                    response.ObjectID = 0;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblCurrencies currency = new tblCurrencies();
                    currency.currency = model.CurrencyInfo_Currency;
                    currency.currencyCode = model.CurrencyInfo_CurrencyCode;
                    db.tblCurrencies.AddObject(currency);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Currency Saved";
                    response.ObjectID = currency.currencyID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Currency NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeletePriceClasification(int priceClasificationID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                ePlatEntities db = new ePlatEntities();
                var query = (from pc in db.tblPriceClasifications where pc.priceClasificationID == priceClasificationID select pc).Single();
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Price Clasification Deleted";
                response.ObjectID = priceClasificationID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Price Clasification NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteCurrency(int currencyID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                ePlatEntities db = new ePlatEntities();
                var query = (from c in db.tblCurrencies where c.currencyID == currencyID select c).Single();
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Currency Deleted";
                response.ObjectID = currencyID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Currency NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse ClonePrice(int priceID, string price)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            tblPrices _price = new tblPrices();
            try
            {
                var query = db.tblPrices.Single(m => m.priceID == priceID);
                var units = db.tblPriceUnits.Where(m => m.priceID == priceID);
                _price.priceTypeID = query.priceTypeID;
                _price.sysItemTypeID = query.sysItemTypeID;
                _price.itemID = query.itemID;
                _price.priceClasificationID = query.priceClasificationID;
                _price.price = decimal.Parse(price);
                _price.currencyID = query.currencyID;
                _price.permanent_ = query.permanent_;
                _price.fromDate = DateTime.Today;
                _price.toDate = query.toDate;
                _price.genericUnitID = query.genericUnitID;
                _price.terminalID = query.terminalID;
                _price.taxesIncluded = query.taxesIncluded;
                _price.fromTransportationZoneID = query.fromTransportationZoneID;
                _price.toTransportationZoneID = query.toTransportationZoneID;
                _price.urlRedeem = query.urlRedeem;
                _price.urlCompare = query.urlCompare;
                _price.dateSaved = DateTime.Now;
                _price.savedByUserID = session.UserID;
                //clone units
                foreach (var i in units)
                {
                    var unit = new tblPriceUnits();
                    unit.priceID = _price.priceID;
                    unit.unit = i.unit;
                    unit.additionalInfo = i.additionalInfo;
                    unit.culture = i.culture;
                    unit.min = i.min;
                    unit.max = i.max;
                    _price.tblPriceUnits.Add(unit);
                    //db.AddObject("tblPriceUnits", unit);
                }
                db.tblPrices.AddObject(_price);
                //code used when cloning a permanent price. it was already defined, not changes necessary
                query.toDate = DateTime.Today.AddSeconds(-1);
                query.permanent_ = false;
                query.dateLastModification = DateTime.Now;
                query.modifiedByUserID = session.UserID;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Price Saved";
                response.ObjectID = new { priceID = _price.priceID, date = _price.fromDate };
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Price NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SavePriceWizard(PriceWizardModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var _now = DateTime.Now;
            try
            {
                if (model.PriceWizard_IsNewUnit)
                {
                    #region "Create price with new unit"
                    tblPrices price = new tblPrices();
                    price.priceTypeID = model.PriceWizard_PriceType;
                    price.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                    price.itemID = model.PriceWizard_ItemID;
                    price.priceClasificationID = model.PriceWizard_PriceClasification;
                    price.price = model.PriceWizard_Price;
                    price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                    price.permanent_ = model.PriceWizard_IsBookingPermanent;
                    price.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                    price.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                    price.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                    price.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                    price.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                    price.terminalID = model.PriceWizard_Terminal;
                    price.taxesIncluded = model.PriceWizard_TaxesIncluded;
                    price.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                    price.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                    price.urlRedeem = model.PriceWizard_URLRedeem;
                    price.urlCompare = model.PriceWizard_URLCompare;
                    price.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                    price.dateSaved = DateTime.Now;
                    price.savedByUserID = session.UserID;
                    //create units
                    var list = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                    foreach (var i in list)
                    {
                        var unit = new tblPriceUnits();
                        unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                        unit.unit = i.PriceWizardUnit_Unit;
                        unit.min = i.PriceWizardUnit_Min;
                        unit.max = i.PriceWizardUnit_Max;
                        unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                        price.tblPriceUnits.Add(unit);
                    }
                    db.tblPrices.AddObject(price);
                    //db.saveChanges();
                    #endregion
                }
                else
                {
                    var currentPrice = db.tblPrices.Single(m => m.priceID == model.PriceWizard_Unit);
                    var currentPriceEndBookingDate = currentPrice.toDate;
                    var statusOfCurrentPrice = PriceDataModel.PricesCatalogs.IsPriceInUse(model);
                    var startBookingDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                    var endBookingDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                    var startTravelDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                    var endTravelDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);

                    if (statusOfCurrentPrice == "not in use")
                    {
                        #region "Save Price without verification since it is not in use"
                        //it is not neccessary to create a new price since the current one could be modified without a problem
                        currentPrice.priceTypeID = model.PriceWizard_PriceType;
                        currentPrice.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                        currentPrice.itemID = model.PriceWizard_ItemID;
                        currentPrice.priceClasificationID = model.PriceWizard_PriceClasification;
                        currentPrice.price = model.PriceWizard_Price;
                        currentPrice.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                        currentPrice.permanent_ = model.PriceWizard_IsBookingPermanent;
                        currentPrice.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                        currentPrice.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                        currentPrice.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                        currentPrice.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                        currentPrice.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                        currentPrice.terminalID = model.PriceWizard_Terminal;
                        currentPrice.taxesIncluded = model.PriceWizard_TaxesIncluded;
                        currentPrice.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                        currentPrice.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                        currentPrice.urlRedeem = model.PriceWizard_URLRedeem;
                        currentPrice.urlCompare = model.PriceWizard_URLCompare;
                        currentPrice.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                        currentPrice.modifiedByUserID = session.UserID;
                        currentPrice.dateLastModification = _now;
                        //delete current and create new units
                        var currentUnits = db.tblPriceUnits.Where(m => m.priceID == currentPrice.priceID);
                        foreach (var i in currentUnits)
                        {
                            db.DeleteObject(i);
                        }
                        var priceUnits = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                        foreach (var i in priceUnits)
                        {
                            var unit = new tblPriceUnits();
                            unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                            unit.unit = i.PriceWizardUnit_Unit;
                            unit.min = i.PriceWizardUnit_Min;
                            unit.max = i.PriceWizardUnit_Max;
                            unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                            currentPrice.tblPriceUnits.Add(unit);
                        }
                        //db.saveChanges();
                        #endregion
                    }
                    else
                    {
                        //since current price is being used, a new price must be created
                        //validar si se cierra el existente o no
                        var priceUnits = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                        var BWOverlap = currentPrice.fromDate <= startBookingDate && (currentPrice.permanent_ || startBookingDate <= currentPrice.toDate);
                        var TWOverlap = currentPrice.twFromDate <= startTravelDate && (currentPrice.twPermanent_ || startTravelDate <= currentPrice.twToDate);
                        var isUnitEqual = priceUnits.Where(m => m.PriceWizardUnit_Unit.Trim().ToLower() == PricesCatalogs.FillDrpUnits(model.PriceWizard_ItemID).FirstOrDefault(x => x.Value == model.PriceWizard_Unit.ToString()).Text.Split('$')[0].Trim().ToLower()).Count() > 0;

                        if (BWOverlap && TWOverlap && isUnitEqual)
                        {
                            tblPrices priceA = new tblPrices();
                            priceA.priceTypeID = model.PriceWizard_PriceType;
                            priceA.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                            priceA.itemID = model.PriceWizard_ItemID;
                            priceA.priceClasificationID = model.PriceWizard_PriceClasification;
                            priceA.price = model.PriceWizard_Price;
                            priceA.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                            priceA.permanent_ = model.PriceWizard_IsBookingPermanent;
                            priceA.fromDate = startBookingDate == DateTime.Today ? _now : startBookingDate;
                            priceA.toDate = endBookingDate;
                            priceA.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                            priceA.twFromDate = startTravelDate;
                            priceA.twToDate = endTravelDate;
                            priceA.terminalID = model.PriceWizard_Terminal;
                            priceA.taxesIncluded = model.PriceWizard_TaxesIncluded;
                            priceA.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                            priceA.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                            priceA.urlRedeem = model.PriceWizard_URLRedeem;
                            priceA.urlCompare = model.PriceWizard_URLCompare;
                            priceA.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                            priceA.dateSaved = _now;
                            priceA.savedByUserID = session.UserID;
                            //create units
                            foreach (var i in priceUnits)
                            {
                                var unit = new tblPriceUnits();
                                unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                                unit.unit = i.PriceWizardUnit_Unit;
                                unit.min = i.PriceWizardUnit_Min;
                                unit.max = i.PriceWizardUnit_Max;
                                unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                                priceA.tblPriceUnits.Add(unit);
                            }
                            db.tblPrices.AddObject(priceA);

                            if (currentPrice.permanent_)
                            {
                                //current bookingDate is permanent
                                if (!model.PriceWizard_IsBookingPermanent)
                                {
                                    #region "create price to cover permanency of current price after the new price closes"
                                    var priceB = new tblPrices();
                                    priceB.priceTypeID = currentPrice.priceTypeID;
                                    priceB.sysItemTypeID = currentPrice.sysItemTypeID;
                                    priceB.itemID = currentPrice.itemID;
                                    priceB.priceClasificationID = currentPrice.priceClasificationID;
                                    priceB.price = currentPrice.price;
                                    priceB.currencyID = currentPrice.currencyID;
                                    priceB.permanent_ = true;
                                    priceB.fromDate = endBookingDate.Value.AddSeconds(1);
                                    priceB.toDate = (DateTime?)null;
                                    priceB.twPermanent_ = currentPrice.twPermanent_;
                                    //priceB.twFromDate = currentPrice.twFromDate;
                                    priceB.twFromDate = endTravelDate != null ? endTravelDate.Value.AddSeconds(1) : currentPrice.twFromDate;
                                    priceB.twToDate = currentPrice.twToDate;
                                    priceB.terminalID = currentPrice.terminalID;
                                    priceB.taxesIncluded = currentPrice.taxesIncluded;
                                    priceB.fromTransportationZoneID = currentPrice.fromTransportationZoneID;
                                    priceB.toTransportationZoneID = currentPrice.toTransportationZoneID;
                                    priceB.urlRedeem = currentPrice.urlRedeem;
                                    priceB.urlCompare = currentPrice.urlCompare;
                                    priceB.genericUnitID = currentPrice.genericUnitID;
                                    priceB.dateSaved = _now;
                                    priceB.savedByUserID = session.UserID;
                                    //create units
                                    foreach (var i in currentPrice.tblPriceUnits)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.culture;
                                        unit.unit = i.unit;
                                        unit.min = i.min;
                                        unit.max = i.max;
                                        unit.additionalInfo = i.additionalInfo;
                                        priceB.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(priceB);
                                    #endregion
                                }
                            }
                            else
                            {
                                //current bookingDate is not permanent
                                if (currentPrice.toDate > endBookingDate)
                                {
                                    #region "create price to cover date range of current price after the new price closes"
                                    var priceB = new tblPrices();
                                    priceB.priceTypeID = currentPrice.priceTypeID;
                                    priceB.sysItemTypeID = currentPrice.sysItemTypeID;
                                    priceB.itemID = currentPrice.itemID;
                                    priceB.priceClasificationID = currentPrice.priceClasificationID;
                                    priceB.price = currentPrice.price;
                                    priceB.currencyID = currentPrice.currencyID;
                                    priceB.permanent_ = false;
                                    priceB.fromDate = endBookingDate.Value.AddSeconds(1);
                                    priceB.toDate = currentPriceEndBookingDate;
                                    priceB.twPermanent_ = currentPrice.twPermanent_;
                                    //priceB.twFromDate = currentPrice.twFromDate;
                                    priceB.twFromDate = endTravelDate != null ? endTravelDate.Value.AddSeconds(1) : currentPrice.twFromDate;
                                    priceB.twToDate = currentPrice.twToDate;
                                    priceB.terminalID = currentPrice.terminalID;
                                    priceB.taxesIncluded = currentPrice.taxesIncluded;
                                    priceB.fromTransportationZoneID = currentPrice.fromTransportationZoneID;
                                    priceB.toTransportationZoneID = currentPrice.toTransportationZoneID;
                                    priceB.urlRedeem = currentPrice.urlRedeem;
                                    priceB.urlCompare = currentPrice.urlCompare;
                                    priceB.genericUnitID = currentPrice.genericUnitID;
                                    priceB.dateSaved = _now;
                                    priceB.savedByUserID = session.UserID;
                                    //create units
                                    foreach (var i in currentPrice.tblPriceUnits)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.culture;
                                        unit.unit = i.unit;
                                        unit.min = i.min;
                                        unit.max = i.max;
                                        unit.additionalInfo = i.additionalInfo;
                                        priceB.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(priceB);
                                    #endregion
                                }
                            }
                            //currentPrice.toDate = startBookingDate == DateTime.Today ? _now.AddSeconds(-1) : startBookingDate.AddSeconds(-1);
                            currentPrice.toDate = startTravelDate.AddSeconds(-1);
                            currentPrice.permanent_ = false;
                            //closing of tw dates
                            currentPrice.twToDate = startTravelDate.AddSeconds(-1);
                            //
                            currentPrice.modifiedByUserID = session.UserID;
                            currentPrice.dateLastModification = _now;
                        }
                        else
                        {
                            if (BWOverlap)
                            {
                                if (TWOverlap)
                                {
                                    //bw and tw
                                    #region "create price without close existing since it has different unit"
                                    tblPrices price = new tblPrices();
                                    price.priceTypeID = model.PriceWizard_PriceType;
                                    price.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                                    price.itemID = model.PriceWizard_ItemID;
                                    price.priceClasificationID = model.PriceWizard_PriceClasification;
                                    price.price = model.PriceWizard_Price;
                                    price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                                    price.permanent_ = model.PriceWizard_IsBookingPermanent;
                                    price.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                                    price.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                                    price.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                                    price.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                                    price.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                                    price.terminalID = model.PriceWizard_Terminal;
                                    price.taxesIncluded = model.PriceWizard_TaxesIncluded;
                                    price.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                                    price.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                                    price.urlRedeem = model.PriceWizard_URLRedeem;
                                    price.urlCompare = model.PriceWizard_URLCompare;
                                    price.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                                    price.dateSaved = _now;
                                    price.savedByUserID = session.UserID;
                                    //create units
                                    var modelUnits = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                                    foreach (var i in modelUnits)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                                        unit.unit = i.PriceWizardUnit_Unit;
                                        unit.min = i.PriceWizardUnit_Min;
                                        unit.max = i.PriceWizardUnit_Max;
                                        unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                                        price.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(price);
                                    #endregion
                                }
                                else if (isUnitEqual)
                                {
                                    //bw and unit

                                }
                                else
                                {
                                    //only bw
                                    #region "create price without close existing since it has different unit and tw date range"
                                    tblPrices price = new tblPrices();
                                    price.priceTypeID = model.PriceWizard_PriceType;
                                    price.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                                    price.itemID = model.PriceWizard_ItemID;
                                    price.priceClasificationID = model.PriceWizard_PriceClasification;
                                    price.price = model.PriceWizard_Price;
                                    price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                                    price.permanent_ = model.PriceWizard_IsBookingPermanent;
                                    price.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                                    price.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                                    price.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                                    price.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                                    price.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                                    price.terminalID = model.PriceWizard_Terminal;
                                    price.taxesIncluded = model.PriceWizard_TaxesIncluded;
                                    price.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                                    price.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                                    price.urlRedeem = model.PriceWizard_URLRedeem;
                                    price.urlCompare = model.PriceWizard_URLCompare;
                                    price.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                                    price.dateSaved = _now;
                                    price.savedByUserID = session.UserID;
                                    //create units
                                    var list = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                                    foreach (var i in list)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                                        unit.unit = i.PriceWizardUnit_Unit;
                                        unit.min = i.PriceWizardUnit_Min;
                                        unit.max = i.PriceWizardUnit_Max;
                                        unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                                        price.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(price);
                                    #endregion
                                }
                            }
                            else
                            {
                                if (TWOverlap)
                                {
                                    if (isUnitEqual)
                                    {
                                        //tw and unit

                                    }
                                    else
                                    {
                                        //only tw
                                        #region "create price without close existing since it has different unit and bw date range"
                                        tblPrices price = new tblPrices();
                                        price.priceTypeID = model.PriceWizard_PriceType;
                                        price.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                                        price.itemID = model.PriceWizard_ItemID;
                                        price.priceClasificationID = model.PriceWizard_PriceClasification;
                                        price.price = model.PriceWizard_Price;
                                        price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                                        price.permanent_ = model.PriceWizard_IsBookingPermanent;
                                        price.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                                        price.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                                        price.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                                        price.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                                        price.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                                        price.terminalID = model.PriceWizard_Terminal;
                                        price.taxesIncluded = model.PriceWizard_TaxesIncluded;
                                        price.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                                        price.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                                        price.urlRedeem = model.PriceWizard_URLRedeem;
                                        price.urlCompare = model.PriceWizard_URLCompare;
                                        price.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                                        price.dateSaved = _now;
                                        price.savedByUserID = session.UserID;
                                        //create units
                                        var list = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                                        foreach (var i in list)
                                        {
                                            var unit = new tblPriceUnits();
                                            unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                                            unit.unit = i.PriceWizardUnit_Unit;
                                            unit.min = i.PriceWizardUnit_Min;
                                            unit.max = i.PriceWizardUnit_Max;
                                            unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                                            price.tblPriceUnits.Add(unit);
                                        }
                                        db.tblPrices.AddObject(price);
                                        #endregion
                                    }
                                }
                                else
                                {
                                    //only unit

                                }
                            }
                            //--------------------------------------------------------------------------------------------------------------------




                            //check if exist coupons in new travel date range
                            if (db.tblPurchaseServiceDetails.Where(m => (m.priceID == currentPrice.priceID || m.netPriceID == currentPrice.priceID) && m.tblPurchases_Services.serviceDateTime >= startTravelDate && m.tblPurchases_Services.serviceDateTime <= endTravelDate).Count() > 0)
                            {
                                if (TWOverlap && isUnitEqual)
                                {
                                    #region "create price but close travel window"
                                    tblPrices priceA = new tblPrices();
                                    priceA.priceTypeID = model.PriceWizard_PriceType;
                                    priceA.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                                    priceA.itemID = model.PriceWizard_ItemID;
                                    priceA.priceClasificationID = model.PriceWizard_PriceClasification;
                                    priceA.price = model.PriceWizard_Price;
                                    priceA.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                                    priceA.permanent_ = model.PriceWizard_IsBookingPermanent;
                                    priceA.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                                    priceA.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                                    priceA.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                                    priceA.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                                    priceA.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                                    priceA.terminalID = model.PriceWizard_Terminal;
                                    priceA.taxesIncluded = model.PriceWizard_TaxesIncluded;
                                    priceA.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                                    priceA.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                                    priceA.urlRedeem = model.PriceWizard_URLRedeem;
                                    priceA.urlCompare = model.PriceWizard_URLCompare;
                                    priceA.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                                    priceA.dateSaved = DateTime.Now;
                                    priceA.savedByUserID = session.UserID;
                                    //create units
                                    var list = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                                    foreach (var i in list)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                                        unit.unit = i.PriceWizardUnit_Unit;
                                        unit.min = i.PriceWizardUnit_Min;
                                        unit.max = i.PriceWizardUnit_Max;
                                        unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                                        priceA.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(priceA);

                                    tblPrices priceB = new tblPrices();
                                    priceB.priceTypeID = currentPrice.priceTypeID;
                                    priceB.sysItemTypeID = currentPrice.sysItemTypeID;
                                    priceB.itemID = currentPrice.itemID;
                                    priceB.priceClasificationID = currentPrice.priceClasificationID;
                                    priceB.price = currentPrice.price;
                                    priceB.currencyID = currentPrice.currencyID;
                                    priceB.permanent_ = currentPrice.permanent_;
                                    priceB.fromDate = endBookingDate.Value.AddSeconds(1);
                                    priceB.toDate = currentPriceEndBookingDate;
                                    priceB.twPermanent_ = currentPrice.twPermanent_;
                                    priceB.twFromDate = endTravelDate != null ? endTravelDate.Value.AddSeconds(1) : currentPrice.twFromDate;
                                    priceB.twToDate = currentPrice.twToDate;
                                    priceB.terminalID = currentPrice.terminalID;
                                    priceB.taxesIncluded = currentPrice.taxesIncluded;
                                    priceB.fromTransportationZoneID = currentPrice.fromTransportationZoneID;
                                    priceB.toTransportationZoneID = currentPrice.toTransportationZoneID;
                                    priceB.urlRedeem = currentPrice.urlRedeem;
                                    priceB.urlCompare = currentPrice.urlCompare;
                                    priceB.genericUnitID = currentPrice.genericUnitID;
                                    priceB.dateSaved = _now;
                                    priceB.savedByUserID = session.UserID;
                                    //create units
                                    foreach (var i in currentPrice.tblPriceUnits)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.culture;
                                        unit.unit = i.unit;
                                        unit.min = i.min;
                                        unit.max = i.max;
                                        unit.additionalInfo = i.additionalInfo;
                                        priceB.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(priceB);

                                    currentPrice.toDate = startTravelDate.AddSeconds(-1);
                                    currentPrice.toDate = startTravelDate.AddSeconds(-1);
                                    currentPrice.permanent_ = false;
                                    #endregion
                                }
                                //email notification
                                var coupons = string.Join(",", db.tblPurchaseServiceDetails.Where(m => (m.netPriceID == currentPrice.priceID || m.priceID == currentPrice.priceID) && startTravelDate <= m.tblPurchases_Services.serviceDateTime && m.tblPurchases_Services.serviceDateTime <= endTravelDate).Select(m => m.purchase_ServiceID).ToArray());
                                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                                mail.From = new System.Net.Mail.MailAddress("eplat@villagroup.com", "Eplat Administration");
                                mail.IsBodyHtml = true;
                                mail.Priority = System.Net.Mail.MailPriority.High;
                                mail.Subject = "Travel Window Overlap";
                                //mail.To.Add("gguerrap@villagroup.com,efalcon@villagroup.com");
                                mail.To.Add("efalcon@villagroup.com");
                                mail.Body = "These coupon(s) ids: " + coupons + "have their service datetime overlaping the travel window of a recently created price with id: ";
                                //EmailNotifications.Send(mail);
                                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = mail } });
                            }
                            else
                            {
                                #region "create price without close existing since it has different date ranges and/or unit"
                                tblPrices price = new tblPrices();
                                price.priceTypeID = model.PriceWizard_PriceType;
                                price.sysItemTypeID = model.PriceWizard_ItemType == "service" ? 1 : 2;
                                price.itemID = model.PriceWizard_ItemID;
                                price.priceClasificationID = model.PriceWizard_PriceClasification;
                                price.price = model.PriceWizard_Price;
                                price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceWizard_Currency).currencyID;
                                price.permanent_ = model.PriceWizard_IsBookingPermanent;
                                price.fromDate = DateTime.Parse(model.PriceWizard_StartBookingDate);
                                price.toDate = model.PriceWizard_IsBookingPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndBookingDate).AddDays(1).AddSeconds(-1);
                                price.twPermanent_ = model.PriceWizard_IsTravelPermanent;
                                price.twFromDate = DateTime.Parse(model.PriceWizard_StartTravelDate);
                                price.twToDate = model.PriceWizard_IsTravelPermanent ? (DateTime?)null : DateTime.Parse(model.PriceWizard_EndTravelDate).AddDays(1).AddSeconds(-1);
                                price.terminalID = model.PriceWizard_Terminal;
                                price.taxesIncluded = model.PriceWizard_TaxesIncluded;
                                price.fromTransportationZoneID = model.PriceWizard_FromTransportationZoneID != 0 ? model.PriceWizard_FromTransportationZoneID : (int?)null;
                                price.toTransportationZoneID = model.PriceWizard_ToTransportationZoneID != 0 ? model.PriceWizard_ToTransportationZoneID : (int?)null;
                                price.urlRedeem = model.PriceWizard_URLRedeem;
                                price.urlCompare = model.PriceWizard_URLCompare;
                                price.genericUnitID = model.PriceWizard_GenericUnit != "null" ? int.Parse(model.PriceWizard_GenericUnit) : (int?)null;
                                price.dateSaved = DateTime.Now;
                                price.savedByUserID = session.UserID;
                                //create units
                                var list = new JavaScriptSerializer().Deserialize<List<PriceWizardUnit_PriceUnits>>(model.PriceWizard_PriceUnits);
                                foreach (var i in list)
                                {
                                    var unit = new tblPriceUnits();
                                    unit.culture = i.PriceWizardUnit_Language.ToLower() == "español" ? "es-MX" : "en-US";
                                    unit.unit = i.PriceWizardUnit_Unit;
                                    unit.min = i.PriceWizardUnit_Min;
                                    unit.max = i.PriceWizardUnit_Max;
                                    unit.additionalInfo = i.PriceWizardUnit_AdditionalInfo;
                                    price.tblPriceUnits.Add(unit);
                                }
                                db.tblPrices.AddObject(price);
                                #endregion
                            }
                        }
                    }
                }
                //db.SaveChanges();

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Price/Unit Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Unit NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SavePrice(PriceInfoModel model, DateTime? date = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.PriceInfo_PriceID != 0)
            {
                #region "Update"
                try
                {
                    var currentPrice = db.tblPrices.Single(m => m.priceID == model.PriceInfo_PriceID);
                    switch (PricesCatalogs.IsPriceInUse(model))
                    {
                        case "try to close":
                            {
                                //its a try to modify vigency of booking dates, not necessarily closing them
                                #region "Try To Close"
                                try
                                {
                                    currentPrice.permanent_ = model.PriceInfo_IsPermanent;
                                    currentPrice.toDate = DateTime.Parse(model.PriceInfo_ToDate).Date == DateTime.Today ? DateTime.Now : model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                    currentPrice.dateLastModification = DateTime.Now;
                                    currentPrice.modifiedByUserID = session.UserID;
                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Price Updated";
                                    response.ObjectID = 0;
                                    return response;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                                #endregion
                            }
                        case "in use":
                            {
                                #region "In Use"
                                try
                                {
                                    var _date = DateTime.Now;
                                    var newPrice = new tblPrices();
                                    newPrice.priceTypeID = model.PriceInfo_PriceType;
                                    newPrice.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                    newPrice.itemID = model.PriceInfo_ItemID;
                                    newPrice.priceClasificationID = model.PriceInfo_PriceClasification;
                                    newPrice.price = model.PriceInfo_Price;
                                    newPrice.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                    newPrice.fromDate = DateTime.Parse(model.PriceInfo_FromDate) <= DateTime.Today ? _date : DateTime.Parse(model.PriceInfo_FromDate);
                                    newPrice.toDate = model.PriceInfo_ToDate != null && !model.PriceInfo_IsPermanent ? DateTime.Parse(model.PriceInfo_ToDate).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                    newPrice.permanent_ = model.PriceInfo_IsPermanent;
                                    newPrice.twFromDate = currentPrice.twFromDate;
                                    newPrice.twToDate = currentPrice.twToDate;
                                    newPrice.twPermanent_ = currentPrice.twPermanent_;
                                    newPrice.terminalID = model.PriceInfo_Terminal;
                                    newPrice.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                    newPrice.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                    newPrice.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                    newPrice.urlRedeem = model.PriceInfo_UrlRedeem != null ? model.PriceInfo_UrlRedeem : "";
                                    newPrice.urlCompare = model.PriceInfo_UrlCompare != null ? model.PriceInfo_UrlCompare : "";
                                    newPrice.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                    newPrice.useOnLine = model.PriceInfo_UseOnline;
                                    newPrice.useOnSite = model.PriceInfo_UseOnsite;
                                    newPrice.dateSaved = _date;
                                    newPrice.savedByUserID = session.UserID;
                                    foreach (var i in model.PriceInfo_PriceUnits)
                                    {
                                        var unit = new tblPriceUnits();
                                        unit.culture = i.PriceUnitInfo_Culture;
                                        unit.unit = i.PriceUnitInfo_Unit;
                                        unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                        unit.min = i.PriceUnitInfo_Min;
                                        unit.max = i.PriceUnitInfo_Max;
                                        unit.adult = false;
                                        unit.child = false;
                                        newPrice.tblPriceUnits.Add(unit);
                                    }
                                    db.tblPrices.AddObject(newPrice);
                                    currentPrice.permanent_ = false;
                                    currentPrice.toDate = DateTime.Parse(model.PriceInfo_FromDate) <= DateTime.Today ? _date.AddSeconds(-1) : DateTime.Parse(model.PriceInfo_FromDate).AddSeconds(-1);
                                    currentPrice.dateLastModification = _date;
                                    currentPrice.modifiedByUserID = session.UserID;
                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Price Updated";
                                    response.ObjectID = 0;
                                    return response;
                                }
                                catch (Exception ex)
                                {
                                    response.Type = Attempt_ResponseTypes.Error;
                                    response.Message = "Price NOT Updated";
                                    response.ObjectID = 0;
                                    response.Exception = ex;
                                    return response;
                                }
                                #endregion
                            }
                        case "not in use":
                            {
                                #region "Not In Use"
                                try
                                {
                                    tblPrices price = db.tblPrices.Single(m => m.priceID == model.PriceInfo_PriceID);
                                    price.priceTypeID = model.PriceInfo_PriceType;
                                    price.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                    price.itemID = model.PriceInfo_ItemID;
                                    price.priceClasificationID = model.PriceInfo_PriceClasification;
                                    price.price = model.PriceInfo_Price;
                                    price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                    price.permanent_ = model.PriceInfo_IsPermanent;
                                    price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                    price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture);
                                    price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                    price.terminalID = model.PriceInfo_Terminal;
                                    price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                    price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                    price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                    price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                    price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                    price.dateLastModification = DateTime.Now;
                                    price.modifiedByUserID = session.UserID;

                                    #region "Price Units"
                                    var priceUnitID = 0;
                                    var dbUnits = price.tblPriceUnits.Select(m => m.priceUnitID).ToArray();
                                    if (model.PriceInfo_PriceUnits != null)
                                    {
                                        foreach (var i in model.PriceInfo_PriceUnits)
                                        {
                                            if (i.PriceUnitInfo_PriceUnitID != 0)
                                            {
                                                //pop element from array of units to delete
                                                dbUnits = dbUnits.Where(m => m != i.PriceUnitInfo_PriceUnitID).ToArray();
                                                var unit = db.tblPriceUnits.Single(m => m.priceUnitID == i.PriceUnitInfo_PriceUnitID);
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                            }
                                            else
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                price.tblPriceUnits.Add(unit);
                                            }
                                        }
                                    }

                                    if (dbUnits.Count() > 0)
                                    {
                                        foreach (var i in dbUnits)
                                        {
                                            db.DeleteObject(db.tblPriceUnits.Single(m => m.priceUnitID == i));
                                        }
                                    }
                                    #endregion

                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Price Updated";
                                    response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                                    return response;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                                #endregion
                            }
                        case "units":
                            {
                                #region "Units Modified"
                                try
                                {
                                    var _dbUnits = db.tblPriceUnits.Where(m => m.priceID == currentPrice.priceID);
                                    foreach (var i in model.PriceInfo_PriceUnits)
                                    {
                                        if (_dbUnits.Where(m => m.priceUnitID == i.PriceUnitInfo_PriceUnitID).Count() == 0)
                                        {
                                            var _unit = new tblPriceUnits();
                                            _unit.priceID = i.PriceUnitInfo_PriceID;
                                            _unit.min = i.PriceUnitInfo_Min;
                                            _unit.max = i.PriceUnitInfo_Max;
                                            _unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                            _unit.culture = i.PriceUnitInfo_Culture;
                                            _unit.unit = i.PriceUnitInfo_Unit;
                                            db.tblPriceUnits.AddObject(_unit);
                                        }
                                        else
                                        {
                                            _dbUnits = _dbUnits.Where(m => m.priceUnitID != i.PriceUnitInfo_PriceUnitID);
                                        }
                                    }
                                    if (_dbUnits.Count() > 0)
                                    {
                                        foreach (var i in _dbUnits)
                                        {
                                            db.DeleteObject(i);
                                        }
                                    }
                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Price Updated";
                                    response.ObjectID = 0;
                                    return response;
                                }
                                catch (Exception ex)
                                {
                                    response.Type = Attempt_ResponseTypes.Error;
                                    response.Message = "Price NOT Updated";
                                    response.ObjectID = 0;
                                    response.Exception = ex;
                                    return response;
                                }
                                #endregion
                            }
                        default:
                            {
                                response.Type = Attempt_ResponseTypes.Ok;
                                response.Message = "No changes made";
                                response.ObjectID = 0;
                                return response;
                            }
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Price NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                {
                    #region "Price of Activity or Transportation"
                    var priceExists = PricesCatalogs.PriceAlreadyExists(model);

                    if (model.PriceInfo_PriceToReplace != null)
                    {
                        if (model.PriceInfo_PriceToReplace != "null")
                        {
                            var _priceID = long.Parse(model.PriceInfo_PriceToReplace);
                            var _priceToReplace = db.tblPrices.Single(m => m.priceID == _priceID);
                            var fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                            var toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                            var _now = DateTime.Now;
                            var originalEndingDate = _priceToReplace.toDate;

                            _priceToReplace.modifiedByUserID = session.UserID;
                            _priceToReplace.dateLastModification = _now;

                            if (!_priceToReplace.permanent_ && !model.PriceInfo_IsPermanent)
                            {
                                #region "Both Prices Not Permanent"
                                //|----PTR----|
                                //   |-B-||-C-|
                                if (_priceToReplace.toDate >= toDate && _priceToReplace.fromDate < fromDate)
                                {
                                    try
                                    {
                                        _priceToReplace.toDate = fromDate == DateTime.Today ? _now.AddSeconds(-1) : fromDate.AddSeconds(-1);
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : fromDate;
                                        priceB.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        #region "Create C"
                                        if (priceB.toDate.Value.AddSeconds(1) < originalEndingDate)
                                        {
                                            tblPrices priceC = new tblPrices();
                                            //var priceUnitIDC = 0;
                                            priceC.priceTypeID = _priceToReplace.priceTypeID;
                                            priceC.sysItemTypeID = _priceToReplace.sysItemTypeID;
                                            priceC.itemID = _priceToReplace.itemID;
                                            priceC.priceClasificationID = _priceToReplace.priceClasificationID;
                                            priceC.price = _priceToReplace.price;
                                            priceC.currencyID = _priceToReplace.currencyID;
                                            priceC.permanent_ = _priceToReplace.permanent_;
                                            //priceC.fromDate = DateTime.Parse(model.PriceInfo_ToDate).AddDays(1);
                                            priceC.fromDate = priceB.toDate.Value.AddSeconds(1);
                                            priceC.toDate = originalEndingDate;
                                            priceC.twPermanent_ = _priceToReplace.twPermanent_;
                                            priceC.twFromDate = _priceToReplace.twFromDate;
                                            priceC.twToDate = _priceToReplace.twToDate;
                                            priceC.terminalID = _priceToReplace.terminalID;
                                            priceC.taxesIncluded = _priceToReplace.taxesIncluded;
                                            priceC.fromTransportationZoneID = _priceToReplace.fromTransportationZoneID;
                                            priceC.toTransportationZoneID = _priceToReplace.toTransportationZoneID;
                                            priceC.urlRedeem = _priceToReplace.urlRedeem;
                                            priceC.urlCompare = _priceToReplace.urlCompare;
                                            priceC.unit = _priceToReplace.unit;
                                            priceC.genericUnitID = _priceToReplace.genericUnitID;
                                            priceC.useOnLine = model.PriceInfo_UseOnline;
                                            priceC.useOnSite = model.PriceInfo_UseOnsite;
                                            priceC.dateSaved = _now;
                                            priceC.savedByUserID = session.UserID;

                                            foreach (var i in _priceToReplace.tblPriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.unit;
                                                unit.additionalInfo = i.additionalInfo;
                                                unit.min = i.min;
                                                unit.max = i.max;
                                                unit.culture = i.culture;
                                                priceC.tblPriceUnits.Add(unit);
                                            }

                                            db.tblPrices.AddObject(priceC);
                                        }
                                        #endregion
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        //response.ObjectID = 
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOt Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                //|------|
                                //   |-------|
                                if (_priceToReplace.toDate >= fromDate && _priceToReplace.fromDate < fromDate)
                                {
                                    try
                                    {
                                        _priceToReplace.toDate = fromDate == DateTime.Today ? _now.AddSeconds(-1) : fromDate.AddSeconds(-1);
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = fromDate == DateTime.Today ? _now : fromDate;
                                        priceB.toDate = DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        //response.ObjectID = 
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                //    |-------|
                                //|-------|
                                if (_priceToReplace.fromDate <= toDate && _priceToReplace.toDate > toDate)
                                {
                                    try
                                    {
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = fromDate == DateTime.Today ? _now : fromDate;
                                        priceB.toDate = DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        _priceToReplace.fromDate = priceB.toDate.Value.AddSeconds(1);
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        //response.ObjectID = 
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                //   |---|
                                //|---------|
                                if (_priceToReplace.fromDate >= fromDate && _priceToReplace.toDate <= toDate)
                                {
                                    try
                                    {
                                        //db.DeleteObject(_priceToReplace);
                                        var deletion = DeletePrice((int)_priceToReplace.priceID);
                                        if (deletion.Type != Attempt_ResponseTypes.Ok)
                                        {
                                            throw new Exception(deletion.Exception.Message);
                                        }
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = fromDate == DateTime.Today ? _now : fromDate;
                                        priceB.toDate = DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        //response.ObjectID = 
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                #endregion
                            }
                            if (!_priceToReplace.permanent_ && model.PriceInfo_IsPermanent)
                            {
                                #region "Only New Price Permanent"
                                //|-------|
                                //    |-------
                                if (_priceToReplace.fromDate < fromDate && _priceToReplace.toDate >= fromDate)
                                {
                                    try
                                    {
                                        _priceToReplace.toDate = fromDate == DateTime.Today ? _now.AddSeconds(-1) : fromDate.AddSeconds(-1);
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                        priceB.toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        //response.ObjectID = 
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                //   |---|
                                //|---------
                                if (_priceToReplace.fromDate >= fromDate)
                                {
                                    try
                                    {
                                        //db.DeleteObject(_priceToReplace);
                                        var deletion = DeletePrice((int)_priceToReplace.priceID);
                                        if (deletion.Type != Attempt_ResponseTypes.Ok)
                                        {
                                            throw new Exception(deletion.Exception.Message);
                                        }
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                        priceB.toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        response.ObjectID = 0;
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                #endregion
                            }
                            if (_priceToReplace.permanent_ && !model.PriceInfo_IsPermanent)
                            {
                                #region "Only Old Price Permanent"
                                //-----------
                                //  |----|
                                if (_priceToReplace.fromDate < fromDate)
                                {
                                    try
                                    {
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                        priceB.toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        _priceToReplace.toDate = priceB.fromDate.Value.AddSeconds(-1);
                                        #region "Create C"
                                        tblPrices priceC = new tblPrices();

                                        priceC.priceTypeID = _priceToReplace.priceTypeID;
                                        priceC.sysItemTypeID = _priceToReplace.sysItemTypeID;
                                        priceC.itemID = _priceToReplace.itemID;
                                        priceC.priceClasificationID = _priceToReplace.priceClasificationID;
                                        priceC.price = _priceToReplace.price;
                                        priceC.currencyID = _priceToReplace.currencyID;
                                        priceC.permanent_ = _priceToReplace.permanent_;
                                        priceC.fromDate = priceB.toDate.Value.AddSeconds(1);
                                        priceC.toDate = originalEndingDate;
                                        priceC.twPermanent_ = _priceToReplace.twPermanent_;
                                        priceC.twFromDate = _priceToReplace.twFromDate;
                                        priceC.twToDate = _priceToReplace.twToDate;
                                        priceC.terminalID = _priceToReplace.terminalID;
                                        priceC.taxesIncluded = _priceToReplace.taxesIncluded;
                                        priceC.fromTransportationZoneID = _priceToReplace.fromTransportationZoneID;
                                        priceC.toTransportationZoneID = _priceToReplace.toTransportationZoneID;
                                        priceC.urlRedeem = _priceToReplace.urlRedeem;
                                        priceC.urlCompare = _priceToReplace.urlCompare;
                                        priceC.unit = _priceToReplace.unit;
                                        priceC.genericUnitID = _priceToReplace.genericUnitID;
                                        priceC.useOnLine = model.PriceInfo_UseOnline;
                                        priceC.useOnSite = model.PriceInfo_UseOnsite;
                                        priceC.dateSaved = _now;
                                        priceC.savedByUserID = session.UserID;

                                        foreach (var i in _priceToReplace.tblPriceUnits)
                                        {
                                            var unit = new tblPriceUnits();
                                            unit.unit = i.unit;
                                            unit.additionalInfo = i.additionalInfo;
                                            unit.min = i.min;
                                            unit.max = i.max;
                                            unit.culture = i.culture;
                                            priceC.tblPriceUnits.Add(unit);
                                        }

                                        db.tblPrices.AddObject(priceC);

                                        #endregion
                                        _priceToReplace.permanent_ = false;
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        response.ObjectID = 0;
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                //   |--------
                                //|------|
                                if (_priceToReplace.fromDate >= fromDate)
                                {
                                    try
                                    {
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                        priceB.toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        _priceToReplace.fromDate = priceB.toDate.Value.AddSeconds(1);
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        response.ObjectID = 0;
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                #endregion
                            }
                            if (_priceToReplace.permanent_ && model.PriceInfo_IsPermanent)
                            {
                                #region "Both Prices Permanent"
                                //-----------
                                //   |-------
                                if (_priceToReplace.fromDate < fromDate)
                                {
                                    try
                                    {
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                        priceB.toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        _priceToReplace.toDate = priceB.fromDate.Value.AddSeconds(-1);
                                        _priceToReplace.permanent_ = false;
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        response.ObjectID = 0;
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }

                                }
                                //    |------
                                //|----------
                                if (_priceToReplace.fromDate >= fromDate)
                                {
                                    try
                                    {
                                        var deletion = DeletePrice((int)_priceToReplace.priceID);
                                        if (deletion.Type != Attempt_ResponseTypes.Ok)
                                        {
                                            throw new Exception(deletion.Exception.Message);
                                        }
                                        #region "Create B"
                                        tblPrices priceB = new tblPrices();
                                        priceB.priceTypeID = model.PriceInfo_PriceType;
                                        priceB.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                        priceB.itemID = model.PriceInfo_ItemID;
                                        priceB.priceClasificationID = model.PriceInfo_PriceClasification;
                                        priceB.price = model.PriceInfo_Price;
                                        priceB.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                        priceB.permanent_ = model.PriceInfo_IsPermanent;
                                        priceB.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                        priceB.toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.twPermanent_ = model.PriceInfo_TWPermanent;
                                        priceB.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                                        priceB.twToDate = model.PriceInfo_TWToDate != null ? DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1) : (DateTime?)null;
                                        priceB.terminalID = model.PriceInfo_Terminal;
                                        priceB.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        priceB.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        priceB.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        priceB.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        priceB.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        priceB.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        priceB.useOnLine = model.PriceInfo_UseOnline;
                                        priceB.useOnSite = model.PriceInfo_UseOnsite;
                                        priceB.dateSaved = _now;
                                        priceB.savedByUserID = session.UserID;

                                        if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                                        {
                                            foreach (var i in model.PriceInfo_PriceUnits)
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                priceB.tblPriceUnits.Add(unit);
                                            }
                                        }
                                        db.tblPrices.AddObject(priceB);
                                        #endregion
                                        db.SaveChanges();
                                        response.Type = Attempt_ResponseTypes.Ok;
                                        response.Message = "Price Saved";
                                        response.ObjectID = 0;
                                        return response;
                                    }
                                    catch (Exception ex)
                                    {
                                        response.Type = Attempt_ResponseTypes.Error;
                                        response.Message = "Price NOT Saved";
                                        response.ObjectID = 0;
                                        response.Exception = ex;
                                        return response;
                                    }
                                }
                                #endregion
                            }
                            return response;
                        }
                        else
                        {
                            #region "Create"
                            tblPrices price = new tblPrices();
                            var priceUnitID = 0;
                            price.priceTypeID = model.PriceInfo_PriceType;
                            price.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                            price.itemID = model.PriceInfo_ItemID;
                            price.priceClasificationID = model.PriceInfo_PriceClasification;
                            price.price = model.PriceInfo_Price;
                            price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                            price.permanent_ = model.PriceInfo_IsPermanent;
                            price.fromDate = date != null ? date : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? DateTime.Now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                            price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            price.twPermanent_ = model.PriceInfo_TWPermanent;
                            price.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                            price.twToDate = model.PriceInfo_TWPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            price.terminalID = model.PriceInfo_Terminal;
                            price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                            price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                            price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                            price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                            price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                            price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                            price.useOnLine = model.PriceInfo_UseOnline;
                            price.useOnSite = model.PriceInfo_UseOnsite;
                            price.dateSaved = DateTime.Now;
                            price.savedByUserID = session.UserID;

                            if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                            {
                                foreach (var i in model.PriceInfo_PriceUnits)
                                {
                                    var unit = new tblPriceUnits();
                                    unit.unit = i.PriceUnitInfo_Unit;
                                    unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                    unit.min = i.PriceUnitInfo_Min;
                                    unit.max = i.PriceUnitInfo_Max;
                                    unit.culture = i.PriceUnitInfo_Culture;
                                    price.tblPriceUnits.Add(unit);
                                }
                            }
                            db.tblPrices.AddObject(price);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Price Saved";
                            response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                            return response;
                            #endregion
                        }
                    }
                    else
                    {
                        if ((bool)priceExists.GetType().GetProperty("exists").GetValue(priceExists, null))
                        {
                            #region "Price Already Exists"
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "Price Already Exists";
                            response.ObjectID = priceExists.GetType().GetProperty("prices").GetValue(priceExists, null);
                            return response;
                            #endregion
                        }
                        else
                        {
                            #region "Create"
                            tblPrices price = new tblPrices();
                            var priceUnitID = 0;
                            price.priceTypeID = model.PriceInfo_PriceType;
                            price.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                            price.itemID = model.PriceInfo_ItemID;
                            price.priceClasificationID = model.PriceInfo_PriceClasification;
                            price.price = model.PriceInfo_Price;
                            price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                            price.permanent_ = model.PriceInfo_IsPermanent;
                            price.fromDate = date != null ? date : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).Date == DateTime.Today ? DateTime.Now : DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                            price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            price.twPermanent_ = model.PriceInfo_TWPermanent;
                            price.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                            price.twToDate = model.PriceInfo_TWPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            price.terminalID = model.PriceInfo_Terminal;
                            price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                            price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                            price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                            price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                            price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                            price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                            price.useOnLine = model.PriceInfo_UseOnline;
                            price.useOnSite = model.PriceInfo_UseOnsite;
                            price.dateSaved = DateTime.Now;
                            price.savedByUserID = session.UserID;

                            if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                            {
                                foreach (var i in model.PriceInfo_PriceUnits)
                                {
                                    var unit = new tblPriceUnits();
                                    unit.unit = i.PriceUnitInfo_Unit;
                                    unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                    unit.min = i.PriceUnitInfo_Min;
                                    unit.max = i.PriceUnitInfo_Max;
                                    unit.culture = i.PriceUnitInfo_Culture;
                                    price.tblPriceUnits.Add(unit);
                                }
                            }
                            db.tblPrices.AddObject(price);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Price Saved";
                            response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                            return response;
                            #endregion
                        }
                    }
                    #endregion
                }
                else
                {
                    #region "save price not being activity nor transportation"
                    tblPrices price = new tblPrices();
                    price.priceTypeID = model.PriceInfo_PriceType;
                    price.sysItemTypeID = (from a in db.tblSysItemTypes where a.sysItemType == model.PriceInfo_ItemType select a.sysItemTypeID).Single();
                    price.itemID = model.PriceInfo_ItemID;
                    price.priceClasificationID = model.PriceInfo_PriceClasification;
                    price.price = model.PriceInfo_Price;
                    price.currencyID = (from a in db.tblCurrencies where a.currencyCode == model.PriceInfo_Currency select a.currencyID).Single();//model.PriceInfo_Currency;
                    price.permanent_ = model.PriceInfo_IsPermanent;
                    price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                    price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    price.twPermanent_ = model.PriceInfo_TWPermanent;
                    price.twFromDate = DateTime.Parse(model.PriceInfo_TWFromDate, CultureInfo.InvariantCulture);
                    price.twToDate = model.PriceInfo_TWPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_TWToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                    price.terminalID = model.PriceInfo_Terminal;
                    price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                    price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                    price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                    price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                    price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                    price.dateSaved = DateTime.Now;
                    price.savedByUserID = session.UserID;
                    //PriceUnit is used only by activities
                    var priceUnitID = 0;

                    db.tblPrices.AddObject(price);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Price Saved";
                    response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                    return response;
                    #endregion
                }
            }
        }

        public AttemptResponse _SavePrice(PriceInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PriceInfo_PriceID != 0)
            {
                #region "update"
                switch (PricesCatalogs.IsPriceInUse(model, null))
                {
                    case "try to close":
                        {
                            var prices = db.tblPurchaseServiceDetails.Where(m => (m.priceID == model.PriceInfo_PriceID || m.netPriceID == model.PriceInfo_PriceID));
                            var _toDate = model.PriceInfo_ToDate != null ? DateTime.Parse(model.PriceInfo_ToDate) : (DateTime?)null;
                            //var cosa = prices.Any(m => m.tblPurchases_Services.dateSaved > _toDate);
                            if (!model.PriceInfo_IsPermanent && _toDate != null && prices.Any(m => m.tblPurchases_Services.dateSaved > _toDate))
                            {
                                #region "return message of price being in use"
                                response.Type = Attempt_ResponseTypes.Warning;
                                response.Message = "Price NOT Updated, price is being used on a date beyond the closing date";
                                response.ObjectID = new { priceID = model.PriceInfo_PriceID };
                                return response;
                                #endregion
                            }
                            else
                            {
                                #region "update price since it is a try to close"
                                try
                                {
                                    tblPrices price = db.tblPrices.Single(m => m.priceID == model.PriceInfo_PriceID);
                                    price.priceTypeID = model.PriceInfo_PriceType;
                                    price.sysItemTypeID = (from a in db.tblSysItemTypes where a.sysItemType == model.PriceInfo_ItemType select a.sysItemTypeID).Single();
                                    price.itemID = model.PriceInfo_ItemID;
                                    price.priceClasificationID = model.PriceInfo_PriceClasification;
                                    price.price = model.PriceInfo_Price;
                                    price.currencyID = (from a in db.tblCurrencies where a.currencyCode == model.PriceInfo_Currency select a.currencyID).Single();
                                    price.permanent_ = model.PriceInfo_IsPermanent;
                                    price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                    price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                    price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                    price.terminalID = model.PriceInfo_Terminal;
                                    price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                    price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                    price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                    price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                    price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                    price.dateLastModification = DateTime.Now;
                                    price.modifiedByUserID = session.UserID;
                                    //PriceUnit
                                    var priceUnitID = 0;
                                    var dbUnits = price.tblPriceUnits.Select(m => m.priceUnitID).ToArray();

                                    if (model.PriceInfo_PriceUnits != null)
                                    {
                                        foreach (var i in model.PriceInfo_PriceUnits)
                                        {
                                            if (i.PriceUnitInfo_PriceUnitID != 0)
                                            {
                                                //pop element from array of units to delete
                                                dbUnits = dbUnits.Where(m => m != i.PriceUnitInfo_PriceUnitID).ToArray();
                                                var unit = db.tblPriceUnits.Single(m => m.priceUnitID == i.PriceUnitInfo_PriceUnitID);
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                            }
                                            else
                                            {
                                                var unit = new tblPriceUnits();
                                                unit.unit = i.PriceUnitInfo_Unit;
                                                unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                                unit.min = i.PriceUnitInfo_Min;
                                                unit.max = i.PriceUnitInfo_Max;
                                                unit.culture = i.PriceUnitInfo_Culture;
                                                price.tblPriceUnits.Add(unit);
                                            }
                                        }
                                    }

                                    if (dbUnits.Count() > 0)
                                    {
                                        foreach (var i in dbUnits)
                                        {
                                            db.DeleteObject(db.tblPriceUnits.Single(m => m.priceUnitID == i));
                                        }
                                    }
                                    db.SaveChanges();
                                    response.Type = Attempt_ResponseTypes.Ok;
                                    response.Message = "Price Updated";
                                    response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                                    return response;
                                }
                                catch (Exception ex)
                                {
                                    response.Type = Attempt_ResponseTypes.Error;
                                    response.Message = "Price NOT Updated";
                                    response.ObjectID = 0;
                                    response.Exception = ex;
                                    return response;
                                }
                                #endregion
                            }
                        }
                    case "in use":
                        {
                            #region "return message of price being in use"
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "Price NOT Updated, It is in use";
                            response.ObjectID = new { priceID = model.PriceInfo_PriceID };
                            return response;
                            #endregion
                        }
                    case "not in use":
                        {
                            #region "update price since it is not in use"
                            try
                            {
                                tblPrices price = db.tblPrices.Single(m => m.priceID == model.PriceInfo_PriceID);
                                price.priceTypeID = model.PriceInfo_PriceType;
                                price.sysItemTypeID = (from a in db.tblSysItemTypes where a.sysItemType == model.PriceInfo_ItemType select a.sysItemTypeID).Single();
                                price.itemID = model.PriceInfo_ItemID;
                                price.priceClasificationID = model.PriceInfo_PriceClasification;
                                price.price = model.PriceInfo_Price;
                                price.currencyID = (from a in db.tblCurrencies where a.currencyCode == model.PriceInfo_Currency select a.currencyID).Single();
                                price.permanent_ = model.PriceInfo_IsPermanent;
                                price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                price.terminalID = model.PriceInfo_Terminal;
                                price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                price.dateLastModification = DateTime.Now;
                                price.modifiedByUserID = session.UserID;
                                //PriceUnit
                                var priceUnitID = 0;
                                var dbUnits = price.tblPriceUnits.Select(m => m.priceUnitID).ToArray();

                                if (model.PriceInfo_PriceUnits != null)
                                {
                                    foreach (var i in model.PriceInfo_PriceUnits)
                                    {
                                        if (i.PriceUnitInfo_PriceUnitID != 0)
                                        {
                                            //pop element from array of units to delete
                                            dbUnits = dbUnits.Where(m => m != i.PriceUnitInfo_PriceUnitID).ToArray();
                                            var unit = db.tblPriceUnits.Single(m => m.priceUnitID == i.PriceUnitInfo_PriceUnitID);
                                            unit.unit = i.PriceUnitInfo_Unit;
                                            unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                            unit.min = i.PriceUnitInfo_Min;
                                            unit.max = i.PriceUnitInfo_Max;
                                            unit.culture = i.PriceUnitInfo_Culture;
                                        }
                                        else
                                        {
                                            var unit = new tblPriceUnits();
                                            unit.unit = i.PriceUnitInfo_Unit;
                                            unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                            unit.min = i.PriceUnitInfo_Min;
                                            unit.max = i.PriceUnitInfo_Max;
                                            unit.culture = i.PriceUnitInfo_Culture;
                                            price.tblPriceUnits.Add(unit);
                                        }
                                    }
                                }

                                if (dbUnits.Count() > 0)
                                {
                                    foreach (var i in dbUnits)
                                    {
                                        db.DeleteObject(db.tblPriceUnits.Single(m => m.priceUnitID == i));
                                    }
                                }
                                db.SaveChanges();
                                response.Type = Attempt_ResponseTypes.Ok;
                                response.Message = "Price Updated";
                                response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                                return response;
                            }
                            catch (Exception ex)
                            {
                                response.Type = Attempt_ResponseTypes.Error;
                                response.Message = "Price NOT Updated";
                                response.ObjectID = 0;
                                response.Exception = ex;
                                return response;
                            }
                            #endregion
                        }
                    default:
                        {
                            return response;
                        }
                }
                #endregion
            }
            else
            {
                #region "save"
                if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                {
                    #region "save price being activity or transportation"
                    var priceExists = PricesCatalogs.PriceAlreadyExists(model);
                    if (model.PriceInfo_PriceToReplace != null)
                    {
                        if (model.PriceInfo_PriceToReplace != "null")
                        {
                            #region "try to replace price with target defined"
                            try
                            {
                                var _priceToReplaceID = long.Parse(model.PriceInfo_PriceToReplace);
                                tblPrices price = new tblPrices();
                                tblPrices _price = db.tblPrices.Single(m => m.priceID == _priceToReplaceID);

                                #region "nuevo precio"

                                price.priceTypeID = model.PriceInfo_PriceType;
                                price.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                                price.itemID = model.PriceInfo_ItemID;
                                price.priceClasificationID = model.PriceInfo_PriceClasification;
                                price.price = model.PriceInfo_Price;
                                price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                                price.permanent_ = model.PriceInfo_IsPermanent;
                                price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                                price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                price.terminalID = model.PriceInfo_Terminal;
                                price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                price.dateSaved = DateTime.Now;
                                price.savedByUserID = session.UserID;
                                foreach (var i in model.PriceInfo_PriceUnits)
                                {
                                    var unit = new tblPriceUnits();
                                    unit.unit = i.PriceUnitInfo_Unit;
                                    unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                    unit.min = i.PriceUnitInfo_Min;
                                    unit.max = i.PriceUnitInfo_Max;
                                    unit.culture = i.PriceUnitInfo_Culture;
                                    price.tblPriceUnits.Add(unit);
                                }
                                db.tblPrices.AddObject(price);
                                #endregion

                                if (model.PriceInfo_ToDate != null)
                                {
                                    if (_price.permanent_ || (_price.toDate > DateTime.Parse(model.PriceInfo_ToDate).AddDays(1).AddSeconds(-1)))
                                    {
                                        #region "apertura de nuevo precio por el rango de tiempo restante"
                                        var _toDate = _price.toDate;
                                        var newPrice = new tblPrices();
                                        newPrice.priceTypeID = _price.priceTypeID;
                                        newPrice.sysItemTypeID = _price.sysItemTypeID;
                                        newPrice.itemID = _price.itemID;
                                        newPrice.priceClasificationID = _price.priceClasificationID;
                                        newPrice.price = _price.price;
                                        newPrice.currencyID = _price.currencyID;
                                        newPrice.permanent_ = _price.permanent_;
                                        newPrice.fromDate = DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1);
                                        newPrice.toDate = _toDate;
                                        newPrice.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                                        newPrice.terminalID = model.PriceInfo_Terminal;
                                        newPrice.taxesIncluded = model.PriceInfo_TaxesIncluded;
                                        newPrice.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                                        newPrice.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                                        newPrice.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                                        newPrice.urlCompare = model.PriceInfo_UrlCompare ?? "";
                                        newPrice.dateSaved = DateTime.Now;
                                        newPrice.savedByUserID = session.UserID;
                                        //foreach (var i in model.PriceInfo_PriceUnits)
                                        foreach (var i in _price.tblPriceUnits)
                                        {
                                            var unit = new tblPriceUnits();
                                            unit.unit = i.unit;
                                            unit.additionalInfo = i.additionalInfo;
                                            unit.min = i.min;
                                            unit.max = i.max;
                                            unit.culture = i.culture;
                                            newPrice.tblPriceUnits.Add(unit);
                                        }
                                        db.tblPrices.AddObject(newPrice);
                                        #endregion
                                    }
                                }

                                //cierre de precio existente
                                _price.permanent_ = false;
                                _price.toDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).AddSeconds(-1);
                                db.SaveChanges();

                                response.Type = Attempt_ResponseTypes.Ok;
                                response.Message = "Price(s) Created";
                                response.ObjectID = new { priceID = price.priceID };
                                return response;
                            }
                            catch (Exception ex)
                            {
                                response.Type = Attempt_ResponseTypes.Error;
                                response.Message = "Price NOT Created";
                                response.ObjectID = 0;
                                response.Exception = ex;
                                return response;
                            }
                            #endregion

                            #region "try to replace price with target defined (commented)"
                            //try
                            //{
                            //    tblPrices price = new tblPrices();
                            //    var _priceID = long.Parse(model.PriceInfo_PriceToReplace);
                            //    var _price = db.tblPrices.Single(m => m.priceID == _priceID);
                            //    //close found price on a day previous the start of the new one
                            //    _price.permanent_ = false;
                            //    _price.toDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture).AddSeconds(-1);

                            //    price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                            //    price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                            //    price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                            //    price.itemID = model.PriceInfo_ItemID;
                            //    price.permanent_ = model.PriceInfo_IsPermanent;
                            //    price.price = model.PriceInfo_Price;
                            //    price.priceClasificationID = model.PriceInfo_PriceClasification;
                            //    price.priceTypeID = model.PriceInfo_PriceType;
                            //    price.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                            //    price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                            //    price.terminalID = model.PriceInfo_Terminal;
                            //    price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            //    price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                            //    price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                            //    price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                            //    foreach (var i in model.PriceInfo_PriceUnits)
                            //    {
                            //        var unit = new tblPriceUnits();
                            //        unit.unit = i.PriceUnitInfo_Unit;
                            //        unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                            //        unit.min = i.PriceUnitInfo_Min;
                            //        unit.max = i.PriceUnitInfo_Max;
                            //        unit.culture = i.PriceUnitInfo_Culture;
                            //        price.tblPriceUnits.Add(unit);
                            //    }
                            //    db.tblPrices.AddObject(price);
                            //    db.SaveChanges();
                            //    response.Type = Attempt_ResponseTypes.Ok;
                            //    response.Message = "Price Created";
                            //    response.ObjectID = new { priceID = price.priceID };
                            //    return response;
                            //}
                            //catch (Exception ex)
                            //{
                            //    response.Type = Attempt_ResponseTypes.Error;
                            //    response.Message = "Price NOT Created";
                            //    response.ObjectID = 0;
                            //    response.Exception = ex;
                            //    return response;
                            //}
                            #endregion
                        }
                        else
                        {
                            #region "save price being activity or transportation"
                            tblPrices price = new tblPrices();
                            price.priceTypeID = model.PriceInfo_PriceType;
                            price.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == model.PriceInfo_ItemType).sysItemTypeID;
                            price.itemID = model.PriceInfo_ItemID;
                            price.priceClasificationID = model.PriceInfo_PriceClasification;
                            price.price = model.PriceInfo_Price;
                            price.currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PriceInfo_Currency).currencyID;
                            price.permanent_ = model.PriceInfo_IsPermanent;
                            price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                            price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                            price.terminalID = model.PriceInfo_Terminal;
                            price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                            price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                            price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                            price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                            price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                            price.dateSaved = DateTime.Now;
                            price.savedByUserID = session.UserID;
                            //PriceUnit is used only by activities
                            var priceUnitID = 0;
                            if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                            {
                                foreach (var i in model.PriceInfo_PriceUnits)
                                {
                                    var unit = new tblPriceUnits();
                                    unit.unit = i.PriceUnitInfo_Unit;
                                    unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                    unit.min = i.PriceUnitInfo_Min;
                                    unit.max = i.PriceUnitInfo_Max;
                                    unit.culture = i.PriceUnitInfo_Culture;
                                    price.tblPriceUnits.Add(unit);
                                }
                            }
                            db.tblPrices.AddObject(price);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Price Saved";
                            response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                            return response;
                            #endregion
                        }
                    }
                    else
                    {
                        if ((bool)priceExists.GetType().GetProperty("exists").GetValue(priceExists, null))
                        {
                            #region "try to replace price without target defined"
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "Price Already Exists";
                            response.ObjectID = priceExists.GetType().GetProperty("prices").GetValue(priceExists, null);
                            return response;
                            #endregion
                        }
                        else
                        {
                            #region "save price being activity or transportation"
                            tblPrices price = new tblPrices();
                            price.priceTypeID = model.PriceInfo_PriceType;
                            price.sysItemTypeID = (from a in db.tblSysItemTypes where a.sysItemType == model.PriceInfo_ItemType select a.sysItemTypeID).Single();
                            price.itemID = model.PriceInfo_ItemID;
                            price.priceClasificationID = model.PriceInfo_PriceClasification;
                            price.price = model.PriceInfo_Price;
                            price.currencyID = (from a in db.tblCurrencies where a.currencyCode == model.PriceInfo_Currency select a.currencyID).Single();//model.PriceInfo_Currency;
                            price.permanent_ = model.PriceInfo_IsPermanent;
                            price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                            price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                            price.terminalID = model.PriceInfo_Terminal;
                            price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                            price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                            price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                            price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                            price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                            price.dateSaved = DateTime.Now;
                            price.savedByUserID = session.UserID;
                            //PriceUnit is used only by activities
                            var priceUnitID = 0;
                            if (model.PriceInfo_ItemType == "Activities" || model.PriceInfo_ItemType == "Transportation")
                            {
                                foreach (var i in model.PriceInfo_PriceUnits)
                                {
                                    var unit = new tblPriceUnits();
                                    unit.unit = i.PriceUnitInfo_Unit;
                                    unit.additionalInfo = i.PriceUnitInfo_AdditionalInfo;
                                    unit.min = i.PriceUnitInfo_Min;
                                    unit.max = i.PriceUnitInfo_Max;
                                    unit.culture = i.PriceUnitInfo_Culture;
                                    price.tblPriceUnits.Add(unit);
                                }
                            }
                            db.tblPrices.AddObject(price);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Price Saved";
                            response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                            return response;
                            #endregion
                        }
                    }
                    #endregion
                }
                else
                {
                    #region "save price not being activity nor transportation"
                    tblPrices price = new tblPrices();
                    price.priceTypeID = model.PriceInfo_PriceType;
                    price.sysItemTypeID = (from a in db.tblSysItemTypes where a.sysItemType == model.PriceInfo_ItemType select a.sysItemTypeID).Single();
                    price.itemID = model.PriceInfo_ItemID;
                    price.priceClasificationID = model.PriceInfo_PriceClasification;
                    price.price = model.PriceInfo_Price;
                    price.currencyID = (from a in db.tblCurrencies where a.currencyCode == model.PriceInfo_Currency select a.currencyID).Single();//model.PriceInfo_Currency;
                    price.permanent_ = model.PriceInfo_IsPermanent;
                    price.fromDate = DateTime.Parse(model.PriceInfo_FromDate, CultureInfo.InvariantCulture);
                    price.toDate = model.PriceInfo_IsPermanent ? (DateTime?)null : DateTime.Parse(model.PriceInfo_ToDate, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    price.genericUnitID = model.PriceInfo_GenericUnit != "null" ? int.Parse(model.PriceInfo_GenericUnit) : (int?)null;
                    price.terminalID = model.PriceInfo_Terminal;
                    price.taxesIncluded = model.PriceInfo_TaxesIncluded;
                    price.fromTransportationZoneID = model.PriceInfo_FromTransportationZone != 0 ? model.PriceInfo_FromTransportationZone : (int?)null;
                    price.toTransportationZoneID = model.PriceInfo_ToTransportationZone != 0 ? model.PriceInfo_ToTransportationZone : (int?)null;
                    price.urlRedeem = model.PriceInfo_UrlRedeem ?? "";
                    price.urlCompare = model.PriceInfo_UrlCompare ?? "";
                    price.dateSaved = DateTime.Now;
                    price.savedByUserID = session.UserID;
                    //PriceUnit is used only by activities
                    var priceUnitID = 0;

                    db.tblPrices.AddObject(price);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Price Saved";
                    response.ObjectID = new { priceID = price.priceID, priceUnitID = priceUnitID };
                    return response;
                    #endregion
                }
                #endregion
            }
        }

        //public List<PriceInfoModel> GetPrices(string itemType, int itemID)
        //public PricesTableModel GetPrices(string itemType, long itemID, string culture)
        public PricesTableModel GetPrices(string itemType, long itemID, string culture, int? pos = null)
        {
            ePlatEntities db = new ePlatEntities();
            List<PriceInfoModel> list = new List<PriceInfoModel>();
            PricesTableModel model = new PricesTableModel();
            List<PriceRowModel> _list = new List<PriceRowModel>();
            //try
            //{
            if (itemType == "Activities" || itemType == "Transportation")
            {
                /////new code to change table layout
                //var _service = db.tblServices.Single(m => m.serviceID == itemID);
                var _terminal = db.tblServices.Single(m => m.serviceID == itemID).originalTerminalID;
                
                var query = PriceDataModel.GetComputedPrices((long)itemID, null, pos, _terminal, null, culture);

                model.PriceTypes = new ReportDataModel().GetListOfPriceTypes(_terminal, true, itemID);//this line was added as substitute of the below commented lines
                                                                                                      //model.PriceTypes = new ReportDataModel().GetListOfPriceTypes(_terminal, false, itemID);
                                                                                                      ////add cost price to priceTypes
                                                                                                      //var _costPriceType = db.tblServices_PriceTypes.Where(m => m.terminalID == _terminal && m.providerID == null && m.serviceID == null && m.tblPriceTypes.isCost).Select(m => new { m.priceTypeID, m.tblPriceTypes.priceType, m.tblPriceTypes.order_ });
                                                                                                      //foreach (var cost in _costPriceType)
                                                                                                      //{
                                                                                                      //    model.PriceTypes.Add(new PriceType()
                                                                                                      //    {
                                                                                                      //        Type = cost.priceType,
                                                                                                      //        PriceTypeID = cost.priceTypeID,
                                                                                                      //        Order = cost.order_
                                                                                                      //    });
                                                                                                      //}                    
                                                                                                      //
                                                                                                      //modificar terminal por prefijo en tabla
                model.Terminal = db.tblTerminals.Single(m => m.terminalID == _terminal).prefix;

                foreach (var _unit in query.Select(m => new { m.FullUnit, m.MinMax, m.CurrencyCode, m.PriceID }).Distinct())
                {
                    PriceRowModel priceRow = new PriceRowModel();
                    priceRow.Unit = _unit.FullUnit + " " + _unit.MinMax;

                    List<PricesModel.PriceDetail> pricesPerType = new List<PricesModel.PriceDetail>();
                    foreach (var _priceType in model.PriceTypes.OrderBy(m => m.Order))
                    {
                        var price = query.Where(m => m.FullUnit == _unit.FullUnit && m.MinMax == _unit.MinMax && m.CurrencyCode == _unit.CurrencyCode && m.PriceTypeID == _priceType.PriceTypeID && m.PriceID == _unit.PriceID).FirstOrDefault();
                        if (price != null)//mike
                        {
                            priceRow.PriceID = price.PriceID;
                            PricesModel.PriceDetail priceDetail = new PricesModel.PriceDetail();
                            if (price != null)
                            {
                                priceDetail.PriceTypeID = price.PriceTypeID;
                                priceDetail.PriceType = price.PriceType;
                                priceDetail.Amount = price.Price;
                                priceDetail.Currency = price.CurrencyCode;
                                priceDetail.Base = price.Base;
                                priceDetail.Active = PricesCatalogs.IsPriceActive(price.PriceID);
                                priceDetail.Rule = price.Rule;
                                priceDetail.IsCost = price.IsCost;
                                priceDetail.PromoID = price.PromoID;
                            }
                            pricesPerType.Add(priceDetail);
                            priceRow.Vigency = ((DateTime)price.FromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " / " + (price.ToDate != null ? ((DateTime)price.ToDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "Permanent");
                            priceRow.TWVigency = ((DateTime)price.TwFromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " / " + (price.TwToDate != null ? ((DateTime)price.TwToDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "Permanent");
                        }
                    }
                    if (pricesPerType != null)//mike
                    {
                        priceRow.PricesPerType = pricesPerType;
                        _list.Add(priceRow);
                    }
                }
                model.Prices = _list;
            }
            else
            {
                try
                {
                    var query = from price in db.tblPrices
                                where price.tblSysItemTypes.sysItemType == itemType
                                && price.itemID == itemID
                                select price;
                    foreach (var i in query)
                    {
                        var _priceUnit = i.tblPriceUnits.Any(m => m.priceID == i.priceID)
                            ? i.tblPriceUnits.Where(m => m.priceID == i.priceID).Select(m => new { m.unit, m.min, m.max })
                            : null;

                        list.Add(new PriceInfoModel()
                        {
                            PriceInfo_PriceID = int.Parse(i.priceID.ToString()),
                            PriceInfo_IsBase = false,
                            PriceInfo_Price = i.price,
                            PriceInfo_Currency = i.tblCurrencies.currencyCode,
                            PriceInfo_IsPermanent = i.permanent_,
                            PriceInfo_FromDate = ((DateTime)i.fromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            PriceInfo_ToDate = i.toDate != null ? ((DateTime)i.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                            //PriceInfo_TWPermanent = i.twPermanent_,
                            //PriceInfo_TWFromDate = ((DateTime)i.twFromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            //PriceInfo_TWToDate = i.twToDate != null ? ((DateTime)i.twToDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                            PriceInfo_TerminalName = i.tblTerminals.terminal,
                            PriceInfo_PriceTypeName = i.tblPriceTypes.priceType,
                            PriceInfo_PriceClasificationName = itemType == "Activities"
                            ? (_priceUnit != null
                                ? _priceUnit.FirstOrDefault().unit + " " + _priceUnit.FirstOrDefault().min + " - " + _priceUnit.FirstOrDefault().max
                                : i.tblPriceClasifications.priceClasification)
                            : itemType == "Transportation"
                                ? (_priceUnit != null ? _priceUnit.FirstOrDefault().unit + ", " + i.tblTransportationZones1.transportationZone : "")
                                : i.tblPriceClasifications.priceClasification

                        });
                    }
                    model.SearchResultsModel = list;
                }
                catch
                {
                }
            }
            //}
            //catch { }
            return model;
        }

        public PriceInfoModel GetPricePerID(int priceID)
        {
            ePlatEntities db = new ePlatEntities();
            PriceInfoModel model = new PriceInfoModel();
            var query = db.tblPrices.Single(m => m.priceID == priceID);

            model.PriceInfo_PriceID = int.Parse(query.priceID.ToString());
            model.PriceInfo_PriceType = query.priceTypeID;
            model.PriceInfo_PriceClasification = query.priceClasificationID;
            model.PriceInfo_Price = query.price;
            model.PriceInfo_Currency = query.tblCurrencies.currencyCode;
            model.PriceInfo_IsPermanent = query.permanent_;
            model.PriceInfo_FromDate = query.fromDate != null ? ((DateTime)query.fromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            model.PriceInfo_ToDate = query.toDate != null ? ((DateTime)query.toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            model.PriceInfo_TWFromDate = query.twFromDate != null ? ((DateTime)query.twFromDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            model.PriceInfo_TWToDate = query.twToDate != null ? ((DateTime)query.twToDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            model.PriceInfo_TWPermanent = query.twPermanent_;
            model.PriceInfo_GenericUnit = query.genericUnitID != null ? query.genericUnitID.ToString() : "null";
            model.PriceInfo_Terminal = int.Parse(query.terminalID.ToString());
            model.PriceInfo_TaxesIncluded = query.taxesIncluded;
            model.PriceInfo_FromTransportationZone = query.fromTransportationZoneID != null ? (int)query.fromTransportationZoneID : 0;
            model.PriceInfo_ToTransportationZone = query.toTransportationZoneID != null ? (int)query.toTransportationZoneID : 0;
            model.PriceInfo_UrlRedeem = query.urlRedeem;
            model.PriceInfo_UrlCompare = query.urlCompare;

            //PriceUnits
            var list = new List<PriceUnitInfoModel>();
            foreach (var i in db.tblPriceUnits.Where(m => m.priceID == priceID))
            {
                list.Add(new PriceUnitInfoModel()
                {
                    PriceUnitInfo_PriceUnitID = i.priceUnitID,
                    PriceUnitInfo_Culture = i.culture,
                    PriceUnitInfo_Unit = i.unit,
                    PriceUnitInfo_AdditionalInfo = i.additionalInfo,
                    PriceUnitInfo_Min = i.min,
                    PriceUnitInfo_Max = i.max
                });
            }
            model.PriceInfo_PriceUnits = list;
            //var priceUnitQuery = db.tblPriceUnits.Where(m => m.priceID == priceID).FirstOrDefault();
            //if (priceUnitQuery != null)
            //{
            //    model.PriceUnitInfo_PriceUnitID = priceUnitQuery.priceUnitID;
            //    model.PriceUnitInfo_PriceID = int.Parse(priceUnitQuery.priceID.ToString());
            //    model.PriceUnitInfo_Unit = priceUnitQuery.unit;
            //    model.PriceUnitInfo_Min = priceUnitQuery.min;
            //    model.PriceUnitInfo_Max = priceUnitQuery.max;
            //    model.PriceUnitInfo_Culture = priceUnitQuery.culture;
            //}
            //--
            return model;
        }

        public AttemptResponse DeletePrice(int priceID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var model = new PriceInfoModel();
                if (PricesCatalogs.IsPriceInUse(model, priceID) != "in use")
                {
                    var query = db.tblPrices.Single(m => m.priceID == priceID);
                    db.DeleteObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Price Deleted";
                    response.ObjectID = priceID;
                    return response;
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Price NOT Deleted, it is in use";
                    response.ObjectID = priceID;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Price NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        //public AttemptResponse SavePriceGroup(string sysItemType, string itemID, string priceGroupID, string prices)
        //{
        //    //priceTypes required: Online, Public, Cost;
        //    ePlatEntities db = new ePlatEntities();
        //    AttemptResponse response = new AttemptResponse();
        //    long[] modelPrices = prices.Split(',').Select(m => long.Parse(m)).OrderBy(m => m).ToArray();
        //    var isAnyPriceExpired = false;
        //    foreach (var i in modelPrices)
        //    {
        //        //if (PricesCatalogs.IsPriceOpen(i) == false)
        //        if (!PricesCatalogs.IsPriceOpen(i))
        //        {
        //            isAnyPriceExpired = true;
        //            break;
        //        }
        //    }
        //    if (isAnyPriceExpired == false)
        //    {
        //        #region "Save Or Update PriceGroup"
        //        if (priceGroupID != null && priceGroupID != "")
        //        {
        //            try
        //            {
        //                var priceGroup = long.Parse(priceGroupID);
        //                var queryPrices = db.tblRelatedPrices.Single(m => m.relationshipID == priceGroup);
        //                long?[] _prices = new long?[] { queryPrices.priceID1, queryPrices.priceID2, queryPrices.priceID3, queryPrices.priceID4, queryPrices.priceID5 };
        //                foreach (var i in _prices)
        //                {
        //                    //if (PricesCatalogs.IsPriceOpen(i) == false)
        //                    if (!PricesCatalogs.IsPriceOpen(i) || PricesCatalogs.IsPriceInUse(i))
        //                    {
        //                        isAnyPriceExpired = true;
        //                        break;
        //                    }
        //                }
        //                if (isAnyPriceExpired == false)
        //                {
        //                    if ((_prices.Length == modelPrices.Length) || (_prices.Where(m => m == null).Count() == 2 & modelPrices.Length >= 3) || (_prices.Where(m => m == null).Count() == 1 & modelPrices.Length >= 4))
        //                    {
        //                        queryPrices.priceID1 = modelPrices[0];
        //                        queryPrices.priceID2 = modelPrices[1];
        //                        queryPrices.priceID3 = modelPrices[2];
        //                        //queryPrices.priceID4 = modelPrices.Length > 3 ? modelPrices[3] : (long?)null;
        //                        //queryPrices.priceID5 = modelPrices.Length > 4 ? modelPrices[4] : (long?)null;
        //                        if (modelPrices.Length > 3)
        //                        {
        //                            queryPrices.priceID4 = modelPrices[3];
        //                        }
        //                        if (modelPrices.Length > 4)
        //                        {
        //                            queryPrices.priceID5 = modelPrices[4];
        //                        }
        //                        db.SaveChanges();
        //                        response.Type = Attempt_ResponseTypes.Ok;
        //                        response.Message = "Prices Group Updated";
        //                        response.ObjectID = queryPrices.relationshipID;
        //                        return response;
        //                    }
        //                    else
        //                    {
        //                        response.Type = Attempt_ResponseTypes.Warning;
        //                        response.Message = "Invalid Prices Group Modification";
        //                        response.ObjectID = 0;
        //                        return response;
        //                    }
        //                }
        //                else
        //                {
        //                    response.Type = Attempt_ResponseTypes.Warning;
        //                    response.Message = "Cannot Save or Edit a Group With Expired/Used Prices";
        //                    response.ObjectID = 0;
        //                    return response;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                response.Type = Attempt_ResponseTypes.Error;
        //                response.Message = "Prices Group NOT Updated";
        //                response.ObjectID = 0;
        //                response.Exception = ex;
        //                return response;
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                var relatedPrices = new tblRelatedPrices();
        //                relatedPrices.sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == sysItemType).sysItemTypeID;
        //                relatedPrices.itemID = long.Parse(itemID);
        //                relatedPrices.priceID1 = modelPrices[0];
        //                relatedPrices.priceID2 = modelPrices[1];
        //                relatedPrices.priceID3 = modelPrices[2];
        //                if (modelPrices.Length > 3)
        //                {
        //                    relatedPrices.priceID4 = modelPrices[3];
        //                }
        //                if (modelPrices.Length > 4)
        //                {
        //                    relatedPrices.priceID5 = modelPrices[4];
        //                }
        //                relatedPrices.createdByUserID = session.UserID;
        //                relatedPrices.dateSaved = DateTime.Now;
        //                db.AddObject("tblRelatedPrices", relatedPrices);
        //                db.SaveChanges();
        //                response.Type = Attempt_ResponseTypes.Ok;
        //                response.Message = "Prices Group Saved";
        //                response.ObjectID = relatedPrices.relationshipID;
        //                return response;
        //            }
        //            catch (Exception ex)
        //            {
        //                response.Type = Attempt_ResponseTypes.Error;
        //                response.Message = "Prices Group NOT Saved";
        //                response.ObjectID = 0;
        //                response.Exception = ex;
        //                return response;
        //            }
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        response.Type = Attempt_ResponseTypes.Warning;
        //        response.Message = "Cannot Save or Edit a Group With Expired Prices";
        //        response.ObjectID = 0;
        //        return response;
        //    }
        //}

        public string GetGroupPrices(string sysItemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            var sysItemTypeID = db.tblSysItemTypes.Single(m => m.sysItemType == sysItemType).sysItemTypeID;
            var list = new List<KeyValuePair<long, string>>();
            var item = long.Parse(itemID);
            var query = from price in db.tblRelatedPrices
                        where price.sysItemTypeID == sysItemTypeID
                        && price.itemID == item
                        select price;
            foreach (var i in query)
            {
                var flag = true;
                var datePrice1 = db.tblPrices.Single(m => m.priceID == i.priceID1).toDate;
                if (datePrice1 != null && datePrice1 < DateTime.Now)
                {
                    flag = false;
                }
                if (flag)
                {
                    var datePrice2 = db.tblPrices.Single(m => m.priceID == i.priceID2).toDate;
                    if (datePrice2 != null && datePrice2 < DateTime.Now)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    var datePrice3 = db.tblPrices.Single(m => m.priceID == i.priceID3).toDate;
                    if (datePrice3 != null && datePrice3 < DateTime.Now)
                    {
                        flag = false;
                    }
                }
                if (flag && i.priceID4 != null)
                {
                    var datePrice4 = db.tblPrices.Single(m => m.priceID == i.priceID4).toDate;
                    if (datePrice4 != null && datePrice4 < DateTime.Now)
                    {
                        flag = false;
                    }
                }
                if (flag && i.priceID5 != null)
                {
                    var datePrice5 = db.tblPrices.Single(m => m.priceID == i.priceID5).toDate;
                    if (datePrice5 != null && datePrice5 < DateTime.Now)
                    {
                        flag = false;
                    }
                }
                list.Add(new KeyValuePair<long, string>(i.relationshipID, flag.ToString() + '-' + i.priceID1 + ',' + i.priceID2 + ',' + i.priceID3 + ',' + (i.priceID4 != null ? i.priceID4.ToString() : "null") + ',' + (i.priceID5 != null ? i.priceID5.ToString() : "null")));
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(list);
            //var result = "";
            //foreach (var i in query)
            //{
            //    result += i.relationshipID + ",";
            //}
            //result = result.TrimEnd(',');
            //return result;
        }

        public string GetPricesPerGroup(string groupID)
        {
            ePlatEntities db = new ePlatEntities();
            var _groupID = long.Parse(groupID);
            var list = new List<string>();
            var query = db.tblRelatedPrices.Single(m => m.relationshipID == _groupID);
            var price4 = query.priceID4 != null ? "," + query.priceID4.ToString() : "";
            var price5 = query.priceID5 != null ? "," + query.priceID5.ToString() : "";
            var builder = query.priceID1 + "," + query.priceID2 + "," + query.priceID3 + price4 + price5;
            return builder;
        }

        public static tblPriceUnits GetUnit(long priceid, string culture)
        {
            ePlatEntities db = new ePlatEntities();
            tblPriceUnits unit = new tblPriceUnits();
            var unitQ = from u in db.tblPriceUnits
                        where u.priceID == priceid
                        && u.culture == culture
                        select u;

            if (unitQ.Count() > 0)
            {
                unit = unitQ.FirstOrDefault();
            }
            else
            {
                unitQ = from u in db.tblPriceUnits
                        where u.priceID == priceid
                        select u;

                if (unitQ.Count() > 0)
                {
                    unit = unitQ.FirstOrDefault();
                }
            }

            return unit;
        }

        public static List<PriceRuleModel> GetFutureRules(long serviceID, long? terminalID = null, DateTime? date = null)
        {
            ePlatEntities db = new ePlatEntities();
            List<PriceRuleModel> Rules = new List<PriceRuleModel>();

            //obtener el proveedor
            int providerID = db.tblServices.Single(x => x.serviceID == serviceID).providerID;

            //obtener fecha para consulta
            date = date ?? DateTime.Now;

            //obtener terminales
            long[] terminals = { };
            if (terminalID == null)
            {
                //es backend, obtener todas las terminales disponibles para el usuario
                terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            }
            else
            {
                terminals = new long[1] { (long)terminalID };
            }


            List<long> AvailablePromos = (from a in db.tblPromos_RelatedItems
                                          where a.itemID == serviceID
                                          && a.sysItemTypeID == 1
                                          select a.promoID).ToList();

            List<long?> NullableAvailablePromos = new List<long?>();

            foreach (var c in AvailablePromos)
            {
                NullableAvailablePromos.Add(c);
            }
            NullableAvailablePromos.Add(null);

            var TerminalRules = (from r in db.tblServices_PriceTypes
                                 join priceType in db.tblPriceTypes on r.priceTypeID equals priceType.priceTypeID
                                 where terminals.Contains(r.terminalID)
                                 && ((r.providerID == null && r.serviceID == null)
                                 || (r.providerID == providerID && r.serviceID == null)
                                 || (r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID == null)
                                 || (r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID != null)
                                 )
                                 //&& r.fromDate > _now && (r.toDate == null || r.toDate > _now)//fromFutureRules
                                 && r.fromDate > date && (r.toDate == null || r.toDate > date)
                                 //&& ((r.permanent_ == true && r.fromDate <= date) || (r.fromDate <= date && r.toDate >= date))
                                 && NullableAvailablePromos.Contains(priceType.promoID)
                                 select r).Distinct();

            foreach (var tr in TerminalRules.Where(r => r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID == null))//actividades 
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblServices.service;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                rule.ThanPriceTypeID = tr.thanPriceTypeID;
                rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                rule.Formula = tr.@base ? "" : tr.formula ?? GetFormula(tr.service_priceTypeID);
                rule.Level = "Activity";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                rule.FromDate = (DateTime)tr.fromDate;
                Rules.Add(rule);
            }

            int[] priceTypeIDsForRules1 = Rules.Select(x => x.PriceTypeID).ToArray();
            foreach (var tr in TerminalRules.Where(r => r.providerID == providerID && r.serviceID == null && !priceTypeIDsForRules1.Contains(r.priceTypeID)))//provider 
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblProviders.comercialName;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                rule.ThanPriceTypeID = tr.thanPriceTypeID;
                rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                rule.Formula = tr.@base ? "" : tr.formula ?? GetFormula(tr.service_priceTypeID);
                rule.Level = "Provider";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                rule.FromDate = (DateTime)tr.fromDate;
                Rules.Add(rule);
            }

            int[] priceTypeIDsForRules2 = Rules.Select(x => x.PriceTypeID).ToArray();
            foreach (var tr in TerminalRules.Where(r => r.providerID == null && r.serviceID == null && !priceTypeIDsForRules2.Contains(r.priceTypeID)))//terminal 
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblTerminals.terminal;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                rule.ThanPriceTypeID = tr.thanPriceTypeID;
                rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                rule.Formula = tr.@base ? "" : tr.formula ?? GetFormula(tr.service_priceTypeID);
                rule.Level = "Terminal";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                rule.FromDate = (DateTime)tr.fromDate;
                Rules.Add(rule);
            }

            foreach (var ur in TerminalRules.Where(r => r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID != null))
            {
                PriceRuleModel newRule = new PriceRuleModel();
                newRule.Service_PriceTypeID = ur.service_priceTypeID;
                newRule.TerminalID = ur.terminalID;
                newRule.ProviderID = ur.providerID;
                newRule.ServiceID = ur.serviceID;
                newRule.GenericUnitID = ur.genericUnitID;
                newRule.RuleFrom = ur.tblGenericUnits.unit;
                newRule.PriceTypeID = ur.priceTypeID;
                newRule.PromoID = ur.tblPriceTypes.promoID;
                newRule.IsCost = ur.tblPriceTypes.isCost;
                newRule.IsMinimal = ur.tblPriceTypes.isMinimal;
                newRule.RuleFor = ur.tblPriceTypes.priceType;
                newRule.IsBasePrice = false;
                newRule.Percentage = ur.percentage;
                newRule.MoreOrLess = ur.moreOrLess;
                newRule.ThanPriceTypeID = ur.thanPriceTypeID;
                newRule.ThanPriceType = (ur.thanPriceTypeID != null ? ur.tblPriceTypes1.priceType : "");
                newRule.Formula = ur.@base ? "" : ur.formula ?? GetFormula(ur.service_priceTypeID);
                newRule.Level = "Unit";
                newRule.SavedOn = ur.dateSaved;
                newRule.PriceTypeOrder = ur.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == ur.savedByUserID);
                newRule.SavedBy = user.firstName + " " + user.lastName;
                newRule.ToDate = ur.toDate;
                newRule.IsPermanent = ur.permanent_;
                newRule.FromDate = (DateTime)ur.fromDate;
                Rules.Add(newRule);
            }

            //proceso de orden
            List<PriceRuleModel> OrderedRules = new List<PriceRuleModel>();
            foreach (var rule in Rules)
            {
                if (rule.IsBasePrice)
                {
                    OrderedRules.Insert(0, rule);
                }
                else
                {
                    //no es base
                    if (OrderedRules.Count(x => x.PriceTypeID == rule.ThanPriceTypeID && x.GenericUnitID == rule.GenericUnitID) > 0)
                    {
                        //si la base ya está en orderedRules, agregar
                        int cIndex = OrderedRules.FindIndex(x => x.ThanPriceTypeID == rule.PriceTypeID);
                        if (cIndex >= 1)
                        {
                            if (cIndex == 1)
                            {
                                cIndex = 2;
                            }
                            OrderedRules.Insert(cIndex - 1, rule);
                        }
                        else
                        {
                            OrderedRules.Add(rule);
                        }
                    }
                    else
                    {
                        //si la base no está en orderedRules, insertar en 1
                        if (OrderedRules.Count() >= 2)
                        {
                            int cIndex = OrderedRules.FindIndex(x => x.ThanPriceTypeID == rule.PriceTypeID);
                            if (cIndex >= 0)
                            {
                                if (cIndex == 0)
                                {
                                    cIndex = 1;
                                }
                                OrderedRules.Insert(cIndex - 1, rule);
                            }
                            else
                            {
                                OrderedRules.Insert(1, rule);
                            }
                        }
                        else if (OrderedRules.Count() == 1)
                        {
                            OrderedRules.Insert(0, rule);
                        }
                        else
                        {
                            OrderedRules.Add(rule);
                        }
                    }
                }
            }
            Rules = OrderedRules;

            return Rules;
        }

        public static List<PriceRuleModel> _GetFutureRules(long serviceID, DateTime? date = null)
        {
            ePlatEntities db = new ePlatEntities();
            List<PriceRuleModel> Rules = new List<PriceRuleModel>();

            var _now = date ?? DateTime.Now;
            int providerID = db.tblServices.Single(m => m.serviceID == serviceID).providerID;
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            List<long> AvailablePromos = (from a in db.tblPromos_RelatedItems
                                          where a.itemID == serviceID
                                          && a.sysItemTypeID == 1
                                          select a.promoID).ToList();

            List<long?> NullableAvailablePromos = new List<long?>();

            foreach (var c in AvailablePromos)
            {
                NullableAvailablePromos.Add(c);
            }
            NullableAvailablePromos.Add(null);

            var TerminalRules = from r in db.tblServices_PriceTypes
                                where terminals.Contains(r.terminalID)
                                && r.providerID == null
                                && r.serviceID == null
                                //&& ((r.permanent_ == true && r.fromDate <= date) || (r.fromDate <= date && r.toDate >= date))
                                && r.fromDate > _now && (r.toDate == null || r.toDate > _now)
                                && NullableAvailablePromos.Contains(r.tblPriceTypes.promoID)
                                orderby r.terminalID ascending, r.@base descending, r.moreOrLess ascending, r.tblPriceTypes.order_
                                select r;

            foreach (var tr in TerminalRules)
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblTerminals.terminal;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                rule.ThanPriceTypeID = tr.thanPriceTypeID;
                rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                rule.Level = "Terminal";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                rule.FromDate = (DateTime)tr.fromDate;
                Rules.Add(rule);
            }

            var ProviderRules = from p in db.tblServices_PriceTypes
                                where terminals.Contains(p.terminalID)
                                && p.providerID == providerID
                                && p.serviceID == null
                                //&& ((p.permanent_ == true && p.fromDate <= date) || (p.fromDate <= date && p.toDate >= date))
                                && p.fromDate > _now && (p.toDate == null || p.toDate > _now)
                                select p;

            foreach (var pr in ProviderRules)
            {
                foreach (PriceRuleModel rule in Rules)
                {
                    if (rule.TerminalID == pr.terminalID && rule.PriceTypeID == pr.priceTypeID)
                    {
                        rule.Service_PriceTypeID = pr.service_priceTypeID;
                        rule.TerminalID = pr.terminalID;
                        rule.ProviderID = pr.providerID;
                        rule.ServiceID = null;
                        rule.GenericUnitID = null;
                        rule.RuleFrom = pr.tblProviders.comercialName;
                        rule.PriceTypeID = pr.priceTypeID;
                        rule.RuleFor = pr.tblPriceTypes.priceType;
                        rule.IsBasePrice = pr.@base;
                        rule.Percentage = pr.percentage;
                        rule.MoreOrLess = pr.moreOrLess;
                        rule.ThanPriceTypeID = pr.thanPriceTypeID;
                        rule.ThanPriceType = (pr.thanPriceTypeID != null ? pr.tblPriceTypes1.priceType : "");
                        rule.Level = "Provider";
                        rule.SavedOn = pr.dateSaved;
                        rule.PriceTypeOrder = pr.tblPriceTypes.order_;
                        tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == pr.savedByUserID);
                        rule.SavedBy = user.firstName + " " + user.lastName;
                        rule.ToDate = pr.toDate;
                        rule.IsPermanent = pr.permanent_;
                        rule.FromDate = (DateTime)pr.fromDate;
                    }
                }
            }

            var ServiceRules = from s in db.tblServices_PriceTypes
                               where terminals.Contains(s.terminalID)
                                && s.providerID == providerID
                                && s.serviceID == serviceID
                                && s.genericUnitID == null
                                //&& ((s.permanent_ == true && s.fromDate <= date) || (s.fromDate <= date && s.toDate >= date))
                                && s.fromDate > _now && (s.toDate == null || s.toDate > _now)
                               select s;

            foreach (var sr in ServiceRules)
            {
                foreach (PriceRuleModel rule in Rules)
                {
                    if (rule.TerminalID == sr.terminalID && rule.PriceTypeID == sr.priceTypeID)
                    {
                        rule.Service_PriceTypeID = sr.service_priceTypeID;
                        rule.TerminalID = sr.terminalID;
                        rule.ProviderID = sr.providerID;
                        rule.ServiceID = sr.serviceID;
                        rule.GenericUnitID = null;
                        rule.RuleFrom = sr.tblServices.service;
                        rule.PriceTypeID = sr.priceTypeID;
                        rule.RuleFor = sr.tblPriceTypes.priceType;
                        rule.IsBasePrice = sr.@base;
                        rule.Percentage = sr.percentage;
                        rule.MoreOrLess = sr.moreOrLess;
                        rule.ThanPriceTypeID = sr.thanPriceTypeID;
                        rule.ThanPriceType = (sr.thanPriceTypeID != null ? sr.tblPriceTypes1.priceType : "");
                        rule.Level = "Activity";
                        rule.SavedOn = sr.dateSaved;
                        rule.PriceTypeOrder = sr.tblPriceTypes.order_;
                        tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == sr.savedByUserID);
                        rule.SavedBy = user.firstName + " " + user.lastName;
                        rule.ToDate = sr.toDate;
                        rule.IsPermanent = sr.permanent_;
                        rule.FromDate = (DateTime)sr.fromDate;
                    }
                }
            }

            var UnitRules = from r in db.tblServices_PriceTypes
                            where terminals.Contains(r.terminalID)
                            && r.providerID == providerID
                            && r.serviceID == serviceID
                            && r.genericUnitID != null
                            //&& ((r.permanent_ == true && r.fromDate <= date) || (r.fromDate <= date && r.toDate >= date))
                            && r.fromDate > _now && (r.toDate == null || r.toDate > _now)
                            select r;


            foreach (var ur in UnitRules)
            {
                PriceRuleModel newRule = new PriceRuleModel();
                newRule.Service_PriceTypeID = ur.service_priceTypeID;
                newRule.TerminalID = ur.terminalID;
                newRule.ProviderID = ur.providerID;
                newRule.ServiceID = ur.serviceID;
                newRule.GenericUnitID = ur.genericUnitID;
                newRule.RuleFrom = ur.tblGenericUnits.unit;
                newRule.PriceTypeID = ur.priceTypeID;
                newRule.PromoID = ur.tblPriceTypes.promoID;
                newRule.IsCost = ur.tblPriceTypes.isCost;
                newRule.IsMinimal = ur.tblPriceTypes.isMinimal;
                newRule.RuleFor = ur.tblPriceTypes.priceType;
                newRule.IsBasePrice = false;
                newRule.Percentage = ur.percentage;
                newRule.MoreOrLess = ur.moreOrLess;
                newRule.ThanPriceTypeID = ur.thanPriceTypeID;
                newRule.ThanPriceType = (ur.thanPriceTypeID != null ? ur.tblPriceTypes1.priceType : "");
                newRule.Level = "Unit";
                newRule.SavedOn = ur.dateSaved;
                newRule.PriceTypeOrder = ur.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == ur.savedByUserID);
                newRule.SavedBy = user.firstName + " " + user.lastName;
                newRule.ToDate = ur.toDate;
                newRule.IsPermanent = ur.permanent_;
                newRule.FromDate = (DateTime)ur.fromDate;
                Rules.Add(newRule);
            }

            //proceso de orden
            List<PriceRuleModel> OrderedRules = new List<PriceRuleModel>();
            foreach (var rule in Rules)
            {
                if (rule.IsBasePrice)
                {
                    OrderedRules.Insert(0, rule);
                }
                else
                {
                    //no es base
                    //if(OrderedRules.Count(x => x.PriceTypeID == rule.ThanPriceTypeID) > 0) {
                    if (OrderedRules.Count(x => x.PriceTypeID == rule.ThanPriceTypeID && x.GenericUnitID == rule.GenericUnitID) > 0)
                    {
                        //si la base ya está en orderedRules, agregar
                        OrderedRules.Add(rule);
                    }
                    else
                    {
                        //si la base no está en orderedRules, insertar en 1
                        if (OrderedRules.Count() >= 2)
                        {
                            OrderedRules.Insert(1, rule);
                        }
                        else if (OrderedRules.Count() == 1)
                        {
                            OrderedRules.Insert(0, rule);
                        }
                        else
                        {
                            OrderedRules.Add(rule);
                        }
                    }
                }
            }
            Rules = OrderedRules;

            return Rules;
        }

        public static List<PriceRuleModel> GetRules(long serviceID, long? terminalID, DateTime? date)
        {
            ePlatEntities db = new ePlatEntities();
            List<PriceRuleModel> Rules = new List<PriceRuleModel>();

            //obtener el proveedor
            int providerID = db.tblServices.Single(x => x.serviceID == serviceID).providerID;

            //obtener fecha para consulta
            date = date ?? DateTime.Now;

            //obtener terminales
            long[] terminals = { };
            if (terminalID == null)
            {
                //es backend, obtener todas las terminales disponibles para el usuario
                terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            }
            else
            {
                terminals = new long[1] { (long)terminalID };
            }


            List<long> AvailablePromos = (from a in db.tblPromos_RelatedItems
                                          where a.itemID == serviceID
                                          && a.sysItemTypeID == 1
                                          select a.promoID).ToList();

            List<long?> NullableAvailablePromos = new List<long?>();

            foreach (var c in AvailablePromos)
            {
                NullableAvailablePromos.Add(c);
            }
            NullableAvailablePromos.Add(null);

            var PriceTypes = from p in db.tblPriceTypes
                             select p;

            var TerminalRules = (from r in db.tblServices_PriceTypes
                                 join priceType in db.tblPriceTypes on r.priceTypeID equals priceType.priceTypeID
                                 where terminals.Contains(r.terminalID)
                                 && ((r.providerID == null && r.serviceID == null)
                                 || (r.providerID == providerID && r.serviceID == null)
                                 || (r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID == null)
                                 || (r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID != null)
                                 )
                                 && ((r.permanent_ == true && r.fromDate <= date) || (r.fromDate <= date && r.toDate >= date))
                                 && NullableAvailablePromos.Contains(priceType.promoID)
                                 select r).Distinct();

            foreach (var tr in TerminalRules.Where(r => r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID == null))//actividades 
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblServices.service;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PriceType = tr.tblPriceTypes.priceType;//mike
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Formula = tr.formula;
                rule.FormulaText = tr.@base ? "" : tr.formula ?? GetFormula(tr.service_priceTypeID);
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                if (tr.thanPriceTypeID != null)
                {
                    rule.ThanPriceTypeID = tr.thanPriceTypeID;
                    rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                }
                else
                {
                    if (rule.Formula != null)
                    {
                        string formula = rule.Formula.Replace(" ", "").Replace("=", "");
                        bool isNumeric = false;
                        try
                        {
                            decimal dec = int.Parse(formula.Substring(formula.IndexOf("P") + 1, 2));
                            isNumeric = true;
                        }
                        catch (Exception e) { }
                        int formulaBasePriceTypeID = int.Parse(isNumeric ? formula.Substring(formula.IndexOf("P") + 1, 2) : formula.Substring(formula.IndexOf("P") + 1, 1));

                        rule.ThanPriceTypeID = formulaBasePriceTypeID;
                        rule.ThanPriceType = PriceTypes.FirstOrDefault(x => x.priceTypeID == formulaBasePriceTypeID).priceType;
                    }
                }
                rule.Level = "Activity";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.FromDate = (DateTime)tr.fromDate;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                Rules.Add(rule);
            }

            int[] priceTypeIDsForRules1 = Rules.Select(x => x.PriceTypeID).ToArray();
            foreach (var tr in TerminalRules.Where(r => r.providerID == providerID && r.serviceID == null && !priceTypeIDsForRules1.Contains(r.priceTypeID)))//provider 
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblProviders.comercialName;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PriceType = tr.tblPriceTypes.priceType;//mike
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Formula = tr.formula;
                rule.FormulaText = tr.@base ? "" : tr.formula ?? GetFormula(tr.service_priceTypeID);
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                if (tr.thanPriceTypeID != null)
                {
                    rule.ThanPriceTypeID = tr.thanPriceTypeID;
                    rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                }
                else
                {
                    if (rule.Formula != null)
                    {
                        string formula = rule.Formula.Replace(" ", "").Replace("=", "");
                        bool isNumeric = false;
                        try
                        {
                            decimal dec = int.Parse(formula.Substring(formula.IndexOf("P") + 1, 2));
                            isNumeric = true;
                        }
                        catch (Exception e) { }
                        int formulaBasePriceTypeID = int.Parse(isNumeric ? formula.Substring(formula.IndexOf("P") + 1, 2) : formula.Substring(formula.IndexOf("P") + 1, 1));

                        rule.ThanPriceTypeID = formulaBasePriceTypeID;
                        rule.ThanPriceType = PriceTypes.FirstOrDefault(x => x.priceTypeID == formulaBasePriceTypeID).priceType;
                    }
                }

                rule.Level = "Provider";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.FromDate = (DateTime)tr.fromDate;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                Rules.Add(rule);
            }

            int[] priceTypeIDsForRules2 = Rules.Select(x => x.PriceTypeID).ToArray();
            foreach (var tr in TerminalRules.Where(r => r.providerID == null && r.serviceID == null && !priceTypeIDsForRules2.Contains(r.priceTypeID)))//terminal 
            {
                PriceRuleModel rule = new PriceRuleModel();
                rule.Service_PriceTypeID = tr.service_priceTypeID;
                rule.TerminalID = tr.terminalID;
                rule.ProviderID = null;
                rule.ServiceID = null;
                rule.GenericUnitID = null;
                rule.RuleFrom = tr.tblTerminals.terminal;
                rule.PriceTypeID = tr.priceTypeID;
                rule.PriceType = tr.tblPriceTypes.priceType;//mike
                rule.PromoID = tr.tblPriceTypes.promoID;
                rule.IsCost = tr.tblPriceTypes.isCost;
                rule.IsMinimal = tr.tblPriceTypes.isMinimal;
                rule.RuleFor = tr.tblPriceTypes.priceType;
                rule.IsBasePrice = tr.@base;
                rule.Formula = tr.formula;
                rule.FormulaText = tr.@base ? "" : tr.formula ?? GetFormula(tr.service_priceTypeID);
                rule.Percentage = tr.percentage;
                rule.MoreOrLess = tr.moreOrLess;
                if (tr.thanPriceTypeID != null)
                {
                    rule.ThanPriceTypeID = tr.thanPriceTypeID;
                    rule.ThanPriceType = (tr.thanPriceTypeID != null ? tr.tblPriceTypes1.priceType : "");
                }
                else
                {
                    if (rule.Formula != null)
                    {
                        string formula = rule.Formula.Replace(" ", "").Replace("=", "");
                        bool isNumeric = false;
                        try
                        {
                            decimal dec = int.Parse(formula.Substring(formula.IndexOf("P") + 1, 2));
                            isNumeric = true;
                        }
                        catch (Exception e) { }
                        int formulaBasePriceTypeID = int.Parse(isNumeric ? formula.Substring(formula.IndexOf("P") + 1, 2) : formula.Substring(formula.IndexOf("P") + 1, 1));

                        rule.ThanPriceTypeID = formulaBasePriceTypeID;
                        rule.ThanPriceType = PriceTypes.FirstOrDefault(x => x.priceTypeID == formulaBasePriceTypeID).priceType;
                    }
                }
                rule.Level = "Terminal";
                rule.SavedOn = tr.dateSaved;
                rule.PriceTypeOrder = tr.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == tr.savedByUserID);
                rule.SavedBy = user.firstName + " " + user.lastName;
                rule.FromDate = (DateTime)tr.fromDate;
                rule.ToDate = tr.toDate;
                rule.IsPermanent = tr.permanent_;
                Rules.Add(rule);
            }

            foreach (var ur in TerminalRules.Where(r => r.providerID == providerID && r.serviceID == serviceID && r.genericUnitID != null))
            {
                PriceRuleModel newRule = new PriceRuleModel();
                newRule.Service_PriceTypeID = ur.service_priceTypeID;
                newRule.TerminalID = ur.terminalID;
                newRule.ProviderID = ur.providerID;
                newRule.ServiceID = ur.serviceID;
                newRule.GenericUnitID = ur.genericUnitID;
                newRule.RuleFrom = ur.tblGenericUnits.unit;
                newRule.PriceTypeID = ur.priceTypeID;
                newRule.PriceType = ur.tblPriceTypes.priceType;//mike
                newRule.PromoID = ur.tblPriceTypes.promoID;
                newRule.IsCost = ur.tblPriceTypes.isCost;
                newRule.IsMinimal = ur.tblPriceTypes.isMinimal;
                newRule.RuleFor = ur.tblPriceTypes.priceType;
                newRule.IsBasePrice = false;
                newRule.Formula = ur.formula;
                newRule.FormulaText = ur.@base ? "" : ur.formula ?? GetFormula(ur.service_priceTypeID);
                newRule.Percentage = ur.percentage;
                newRule.MoreOrLess = ur.moreOrLess;
                if (ur.thanPriceTypeID != null)
                {
                    newRule.ThanPriceTypeID = ur.thanPriceTypeID;
                    newRule.ThanPriceType = (ur.thanPriceTypeID != null ? ur.tblPriceTypes1.priceType : "");
                }
                else
                {
                    if (newRule.Formula != null)
                    {
                        string formula = newRule.Formula.Replace(" ", "").Replace("=", "");
                        bool isNumeric = false;
                        try
                        {
                            decimal dec = int.Parse(formula.Substring(formula.IndexOf("P") + 1, 2));
                            isNumeric = true;
                        }
                        catch (Exception e) { }
                        int formulaBasePriceTypeID = int.Parse(isNumeric ? formula.Substring(formula.IndexOf("P") + 1, 2) : formula.Substring(formula.IndexOf("P") + 1, 1));

                        newRule.ThanPriceTypeID = formulaBasePriceTypeID;
                        newRule.ThanPriceType = PriceTypes.FirstOrDefault(x => x.priceTypeID == formulaBasePriceTypeID).priceType;
                    }
                }
                newRule.Level = "Unit";
                newRule.SavedOn = ur.dateSaved;
                newRule.PriceTypeOrder = ur.tblPriceTypes.order_;
                tblUserProfiles user = db.tblUserProfiles.Single(u => u.userID == ur.savedByUserID);
                newRule.SavedBy = user.firstName + " " + user.lastName;
                newRule.FromDate = (DateTime)ur.fromDate;
                newRule.ToDate = ur.toDate;
                newRule.IsPermanent = ur.permanent_;
                Rules.Add(newRule);
            }

            //proceso de orden
            List<PriceRuleModel> OrderedRules = new List<PriceRuleModel>();

            foreach (var rule in Rules)
            {
                if (rule.IsBasePrice)
                {
                    OrderedRules.Insert(0, rule);
                }
                else
                {
                    //tiene reglas de unidad dependientes?
                    int indexA = -1;
                    indexA = OrderedRules.FindIndex(x => x.ThanPriceTypeID == rule.PriceTypeID && x.GenericUnitID == rule.GenericUnitID);
                    if (indexA >= 0)
                    {
                        OrderedRules.Insert(indexA, rule);
                    }
                    else
                    {
                        //si no, buscar si tiene reglas sin unidad, dependientes
                        indexA = OrderedRules.FindIndex(x => x.ThanPriceTypeID == rule.PriceTypeID && x.GenericUnitID == null);
                        if (indexA >= 0)
                        {
                            //si tiene reglas dependientes, insertar antes
                            OrderedRules.Insert(indexA, rule);
                        }
                        else
                        {
                            //si no tiene reglas dependientes, agregar al final
                            OrderedRules.Add(rule);
                        }
                    }
                }
            }

            Rules = OrderedRules;
            
            return Rules;
        }

        public static PriceExchangeRate GetExchangeRateToApply(long terminalID, int providerID, int pointOfSaleID, DateTime? bookingDate, int? exchangeRateTypeID)
        {
            ePlatEntities db = new ePlatEntities();
            PriceExchangeRate er = new PriceExchangeRate();

            //obtener el tipo de cambio del proveedor
            decimal pRate = 0;
            long pRateid = 0;

            var exchangeRateQ = from r in db.tblExchangeRates
                                where r.terminalID == terminalID
                                && r.providerID == providerID
                                && r.fromCurrencyID == 1
                                && r.toCurrencyID == 2
                                //&& (r.permanent_ == true && r.fromDate <= (bookingDate ?? DateTime.Now) || (r.fromDate <= (bookingDate ?? DateTime.Now) && r.toDate >= (bookingDate ?? DateTime.Now)))
                                && (r.fromDate <= (bookingDate ?? DateTime.Now) && (r.permanent_ == true || r.toDate >= (bookingDate ?? DateTime.Now)))
                                select new
                                {
                                    r.exchangeRate,
                                    r.exchangeRateID,
                                    r.exchangeRateTypeID,
                                    r.tblExchangeRateTypes.exchangeRateType,
                                    r.tblExchangeRates_PointsOfSales
                                };

            if (exchangeRateQ.Count() > 0)
            {
                switch (exchangeRateTypeID)
                {
                    case 6:
                        if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 6) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRateType;
                            er.TypeID = 6;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 3) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 3).exchangeRateType;
                            er.TypeID = 3;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 5:
                        if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 5) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 5).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 5).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 5).exchangeRateType;
                            er.TypeID = 5;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 2) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 2).exchangeRateType;
                            er.TypeID = 2;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 4:
                        if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 3:
                        if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 3) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 3).exchangeRateType;
                            er.TypeID = 3;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 2:
                        if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 2) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 2).exchangeRateType;
                            er.TypeID = 2;
                        }
                        else if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 1:
                        if (exchangeRateQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            pRate = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            pRateid = exchangeRateQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Provider - " + exchangeRateQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                }
            }

            //obtener el tipo de cambio de la terminal
            decimal tRate = 0;
            long tRateid = 0;

            var exchangeRateTQ = from r in db.tblExchangeRates
                                 where r.terminalID == terminalID
                                 && r.providerID == null
                                 && r.fromCurrencyID == 1
                                 && r.toCurrencyID == 2
                                 && (r.permanent_ == true && r.fromDate <= (bookingDate ?? DateTime.Now) || (r.fromDate <= (bookingDate ?? DateTime.Now) && r.toDate >= (bookingDate ?? DateTime.Now)))
                                 select new
                                 {
                                     r.exchangeRate,
                                     r.exchangeRateID,
                                     r.exchangeRateTypeID,
                                     r.tblExchangeRateTypes.exchangeRateType,
                                     r.tblExchangeRates_PointsOfSales
                                 };

            if (exchangeRateTQ.Count() > 0)
            {
                switch (exchangeRateTypeID)
                {
                    case 6:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 6) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 6).exchangeRateType;
                            er.TypeID = 6;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 3) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 3).exchangeRateType;
                            er.TypeID = 3;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 &&
                                r.tblExchangeRates_PointsOfSales.Count(
                                    p => p.pointOfSaleID == pointOfSaleID
                                    && p.dateAdded <= bookingDate
                                    && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                                ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 1;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1 && t.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateType;
                                er.TypeID = 1;
                            }
                        }
                        break;
                    case 5:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 5) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 5).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 5).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 5).exchangeRateType;
                            er.TypeID = 5;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 2) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 2).exchangeRateType;
                            er.TypeID = 2;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 &&
                                r.tblExchangeRates_PointsOfSales.Count(
                                    p => p.pointOfSaleID == pointOfSaleID
                                    && p.dateAdded <= bookingDate
                                    && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                                ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 1;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1 && t.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateType;
                                er.TypeID = 1;
                            }
                        }
                        break;
                    case 4:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4 &&
                               r.tblExchangeRates_PointsOfSales.Count(
                                   p => p.pointOfSaleID == pointOfSaleID
                                   && p.dateAdded <= bookingDate
                                   && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                               ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 4;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                                er.TypeID = 4;
                            }
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 &&
                                r.tblExchangeRates_PointsOfSales.Count(
                                    p => p.pointOfSaleID == pointOfSaleID
                                    && p.dateAdded <= bookingDate
                                    && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                                ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 1;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1 && t.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateType;
                                er.TypeID = 1;
                            }
                        }
                        break;
                    case 3:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 3) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 3).exchangeRateType;
                            er.TypeID = 3;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 &&
                                r.tblExchangeRates_PointsOfSales.Count(
                                    p => p.pointOfSaleID == pointOfSaleID
                                    && p.dateAdded <= bookingDate
                                    && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                                ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 1;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1 && t.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateType;
                                er.TypeID = 1;
                            }
                        }
                        break;
                    case 2:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 2) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 2).exchangeRateType;
                            er.TypeID = 2;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 &&
                                r.tblExchangeRates_PointsOfSales.Count(
                                    p => p.pointOfSaleID == pointOfSaleID
                                    && p.dateAdded <= bookingDate
                                    && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                                ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 1;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1 && t.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateType;
                                er.TypeID = 1;
                            }
                        }
                        break;
                    case 1:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            //point of sale
                            var ERPoS = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 &&
                                r.tblExchangeRates_PointsOfSales.Count(
                                    p => p.pointOfSaleID == pointOfSaleID
                                    && p.dateAdded <= bookingDate
                                    && (p.dateDeleted == null || p.dateDeleted > bookingDate)
                                ) > 0);
                            if (ERPoS != null)
                            {
                                tRate = ERPoS.exchangeRate;
                                tRateid = ERPoS.exchangeRateID;
                                er.Type = "Terminal - " + ERPoS.exchangeRateType;
                                er.TypeID = 1;
                            }
                            else
                            {
                                //general
                                tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRate;
                                tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1 && r.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateID;
                                er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1 && t.tblExchangeRates_PointsOfSales.Count() == 0).exchangeRateType;
                                er.TypeID = 1;
                            }
                        }
                        break;
                }
            }
            else
            {
                exchangeRateTQ = from r in db.tblExchangeRates
                                 where r.terminalID == terminalID
                                 && r.providerID == null
                                 && r.fromCurrencyID == 1
                                 && r.toCurrencyID == 2
                                 && r.toDate <= (bookingDate ?? DateTime.Now)
                                 orderby r.fromDate descending
                                 select new
                                 {
                                     r.exchangeRate,
                                     r.exchangeRateID,
                                     r.exchangeRateTypeID,
                                     r.tblExchangeRateTypes.exchangeRateType,
                                     r.tblExchangeRates_PointsOfSales
                                 };

                switch (exchangeRateTypeID)
                {
                    case 6:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 6) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 6).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 6).exchangeRateType;
                            er.TypeID = 6;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 3) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 3).exchangeRateType;
                            er.TypeID = 3;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 5:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 5) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 5).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 5).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 5).exchangeRateType;
                            er.TypeID = 5;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 2) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 2).exchangeRateType;
                            er.TypeID = 2;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 4:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 4) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 4).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 4).exchangeRateType;
                            er.TypeID = 4;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 3:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 3) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 3).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 3).exchangeRateType;
                            er.TypeID = 3;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 2:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 2) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 2).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 2).exchangeRateType;
                            er.TypeID = 2;
                        }
                        else if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                    case 1:
                        if (exchangeRateTQ.Count(r => r.exchangeRateTypeID == 1) > 0)
                        {
                            tRate = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRate;
                            tRateid = exchangeRateTQ.FirstOrDefault(r => r.exchangeRateTypeID == 1).exchangeRateID;
                            er.Type = "Terminal - " + exchangeRateTQ.FirstOrDefault(t => t.exchangeRateTypeID == 1).exchangeRateType;
                            er.TypeID = 1;
                        }
                        break;
                }
            }



            if (pRate > 0)
            {
                //prate
                er.RateID = pRateid;
                er.Rate = pRate;
                er.Level = "Provider";
            }
            else
            {
                //trate
                er.RateID = tRateid;
                er.Rate = tRate;
                er.Level = "Terminal";
            }

            return er;
        }

        public static string GetFormula(long service_pricetypeID)
        {
            string formula = string.Empty;
            ePlatEntities db = new ePlatEntities();

            var rule = (from r in db.tblServices_PriceTypes
                        where r.service_priceTypeID == service_pricetypeID
                        select r).FirstOrDefault();

            if (rule != null && rule.percentage != null)
            {
                formula = "=P" + rule.thanPriceTypeID + ((bool)rule.moreOrLess ? "+" : "-") + rule.percentage + "%";
            }

            return formula;
        }

        //public static List<ComputedPriceModel> GetComputedPrices(long serviceID, DateTime? date, long? terminalID = null, bool online = false, DateTime? bookingDate = null, string culture = "", DateTime? costDate = null) //exchangeRateTypeID
        public static List<ComputedPriceModel> GetComputedPrices(long serviceID, DateTime? date, int? pointOfSaleID, long? terminalID = null, DateTime? bookingDate = null, string culture = "", DateTime? costDate = null) //exchangeRateTypeID
        {
            ePlatEntities db = new ePlatEntities();
            List<ComputedPriceModel> Prices = new List<ComputedPriceModel>();
            bool? online = pointOfSaleID == null ? (bool?)null : pointOfSaleID == 0 ? false : pointOfSaleID < 99999 ? db.tblPointsOfSale.Single(m => m.pointOfSaleID == pointOfSaleID).online : true;
            pointOfSaleID = pointOfSaleID == null ? 99999 : pointOfSaleID;
            //var online = pointOfSaleID == 0 ? false : pointOfSaleID < 99999 ? db.tblPointsOfSale.Single(m => m.pointOfSaleID == pointOfSaleID).online : true;

            //obtener la moneda del proveedor
            //int? baseCurrencyID = db.tblServices.Single(x => x.serviceID == serviceID).tblProviders.tblContractsCurrencyHistory.Where(c => c.dateSaved <= (bookingDate ?? DateTime.Now)).OrderByDescending(o => o.dateSaved).FirstOrDefault().contractCurrencyID;
            int? baseCurrencyID = db.tblServices.Single(x => x.serviceID == serviceID).tblProviders.tblContractsCurrencyHistory.Count() > 0 ?
                db.tblServices.Single(x => x.serviceID == serviceID).tblProviders.tblContractsCurrencyHistory.OrderByDescending(o => o.dateSaved).FirstOrDefault().contractCurrencyID
                //db.tblServices.Single(x => x.serviceID == serviceID).tblProviders.tblContractsCurrencyHistory.Where(c => c.dateSaved <= (bookingDate ?? DateTime.Now)).OrderByDescending(o => o.dateSaved).FirstOrDefault().contractCurrencyID
                : (int?)null;

            if (baseCurrencyID == null && online != true)
            {
                baseCurrencyID = 1;
            }
            else if (baseCurrencyID == null && online == true)
            {
                if (culture == "en-US")
                {
                    baseCurrencyID = 1;
                }
                else
                {
                    baseCurrencyID = 2;
                }
            }

            //revisaar si la terminal aplica current exchange rate
            bool acer = db.tblServices.Single(x => x.serviceID == serviceID).tblTerminals.useCurrentCostER;

            int exchangeRateTypeID = 1;
            int costExchangeRateTypeID = 4;
            if (culture == "en-US")
            {
                exchangeRateTypeID = 2;
                costExchangeRateTypeID = 5;
            }
            else if (culture == "es-MX")
            {
                exchangeRateTypeID = 3;
                costExchangeRateTypeID = 6;
            }

            //obtener el proveedor
            int providerID = db.tblServices.Single(x => x.serviceID == serviceID).providerID;
            string provider = db.tblProviders.Single(x => x.providerID == providerID).comercialName;

            //obtener reglas de tipos de precio disponibles en terminal
            List<PriceRuleModel> Rules = GetRules(serviceID, terminalID, bookingDate ?? DateTime.Now);

            //obtener el tipo de precio base
            var baseRule = Rules.Where(r => r.IsBasePrice == true);
            int basePriceTypeID = (baseRule.Count() > 0 ? baseRule.FirstOrDefault().PriceTypeID : 1);

            //obtención de terminales activas
            long[] terminals = { };
            if (terminalID == null)
            {
                //es backend, obtener todas las terminales disponibles para el usuario
                terminals = session.Terminals.Split(',').Select(x => long.Parse(x)).ToArray();
            }
            else
            {
                terminals = new long[1] { (long)terminalID };
            }

            //revisar si el servicio es redondeable
            bool? avoidRounding = db.tblServices.FirstOrDefault(x => x.serviceID == serviceID).avoidRounding;

            //obtener precios base
            var PriceQ = from p in db.tblPrices
                         where (p.sysItemTypeID == 1 || p.sysItemTypeID == 3)
                         //&& ((p.useOnLine == true && p.useOnLine == online) || (p.useOnSite == true && p.useOnSite == !online))
                         && ((p.useOnLine == true && p.useOnLine == online) || (p.useOnSite == true && p.useOnSite == !online) || (online == null))
                         && p.itemID == serviceID
                         && p.priceTypeID == basePriceTypeID
                         && (p.currencyID == baseCurrencyID || baseCurrencyID == null)
                         && terminals.Contains(p.terminalID)
                         && (
                         (date != null &&
                         ((p.twPermanent_ == true && p.twFromDate <= date)
                         || (p.twPermanent_ == false && p.twFromDate <= date && p.twToDate >= date))
                         )
                         ||
                         (date == null &&
                         ((p.twPermanent_ == true)
                         || (p.twPermanent_ == false && p.twToDate >= DateTime.Today))
                         )
                         )
                         && (
                         (bookingDate != null &&
                         ((p.permanent_ == true && p.fromDate <= bookingDate)
                         || (p.permanent_ == false && p.fromDate <= bookingDate && p.toDate >= bookingDate))
                         )
                         ||
                         (bookingDate == null &&
                         ((p.permanent_ == true)
                         || (p.permanent_ == false && p.toDate >= DateTime.Now))
                         )
                         )
                         select p;

            if (PriceQ.Count() == 0)
            {
                if (baseCurrencyID == 1)
                {
                    baseCurrencyID = 2;
                }
                else
                {
                    baseCurrencyID = 1;
                }

                PriceQ = from p in db.tblPrices
                         where (p.sysItemTypeID == 1 || p.sysItemTypeID == 3)
                         //&& ((p.useOnLine == true && p.useOnLine == online) || (p.useOnSite == true && p.useOnSite == !online))
                         && ((p.useOnLine == true && p.useOnLine == online) || (p.useOnSite == true && p.useOnSite == !online) || (online == null))
                         && p.itemID == serviceID
                         && p.priceTypeID == basePriceTypeID
                         && (p.currencyID == baseCurrencyID || baseCurrencyID == null)
                         && terminals.Contains(p.terminalID)
                         && (
                         (date != null &&
                         ((p.twPermanent_ == true && p.twFromDate <= date)
                         || (p.twPermanent_ == false && p.twFromDate <= date && p.twToDate >= date))
                         )
                         ||
                         (date == null &&
                         ((p.twPermanent_ == true)
                         || (p.twPermanent_ == false && p.twToDate >= DateTime.Today))
                         )
                         )
                         && (
                         (bookingDate != null &&
                         ((p.permanent_ == true && p.fromDate <= bookingDate)
                         || (p.permanent_ == false && p.fromDate <= bookingDate && p.toDate >= bookingDate))
                         )
                         ||
                         (bookingDate == null &&
                         ((p.permanent_ == true)
                         || (p.permanent_ == false && p.toDate >= DateTime.Now))
                         )
                         )
                         select p;

            }

            foreach (var p in PriceQ)
            {
                ComputedPriceModel currentPrice = new ComputedPriceModel();
                currentPrice.PriceID = p.priceID;
                currentPrice.ServiceID = p.itemID;
                currentPrice.GenericUnitID = p.genericUnitID;
                currentPrice.SysItemTypeID = p.sysItemTypeID;
                currentPrice.PriceTypeID = p.priceTypeID;
                currentPrice.PriceType = p.tblPriceTypes.priceType;
                currentPrice.PromoID = p.tblPriceTypes.promoID;
                currentPrice.IsCost = p.tblPriceTypes.isCost;
                currentPrice.IsMinimal = p.tblPriceTypes.isMinimal;
                currentPrice.Price = p.price;
                currentPrice.CurrencyID = p.currencyID;
                currentPrice.CurrencyCode = p.tblCurrencies.currencyCode;
                currentPrice.Permanent = p.permanent_;
                currentPrice.FromDate = p.fromDate;
                currentPrice.ToDate = p.toDate;
                currentPrice.TwPermanent = p.twPermanent_;
                currentPrice.TwFromDate = p.twFromDate;
                currentPrice.TwToDate = p.twToDate;
                currentPrice.TerminalID = (int)p.terminalID;
                currentPrice.Terminal = p.tblTerminals.terminal;
                currentPrice.TaxesIncluded = p.taxesIncluded;
                currentPrice.FromTransportationZoneID = p.fromTransportationZoneID;
                currentPrice.ToTransportationZoneID = p.toTransportationZoneID;
                currentPrice.ToTransportationZone = (p.toTransportationZoneID != null ? p.tblTransportationZones1.transportationZone : "");

                if (culture == "")
                {
                    currentPrice.Culture = (currentPrice.CurrencyID == 1 ? "en-US" : "es-MX");
                }
                else
                {
                    currentPrice.Culture = culture;
                }
                tblPriceUnits unit = GetUnit(p.priceID, currentPrice.Culture);
                currentPrice.Unit = unit.unit + (currentPrice.SysItemTypeID == 3 ? " - " + currentPrice.ToTransportationZone : "");

                currentPrice.AdditionalInfo = unit.additionalInfo;
                currentPrice.FullUnit = currentPrice.Unit + " " + currentPrice.AdditionalInfo;
                currentPrice.Min = unit.min;
                currentPrice.Max = unit.max;
                currentPrice.MinMax = unit.min + " - " + unit.max;
                currentPrice.ExchangeRateID = null;
                currentPrice.Base = true;
                currentPrice.Rounding = "Not Rounded";
                currentPrice.Rule = "Base Price";
                currentPrice.DependingOnPriceID = p.dependingOnPriceID;
                currentPrice.DependingOnPriceQuantity = p.dependingOnPriceQuantity;
                currentPrice.Highlight = p.highlight;
                currentPrice.Service_PriceTypeID = baseRule.FirstOrDefault().Service_PriceTypeID;

                if (p.savedByUserID != null)
                {
                    currentPrice.CreatedBy = p.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName;
                    currentPrice.CreatedOn = p.dateSaved.Value.ToString("yyyy-MM-dd hh:mm:dd");
                }

                if (p.genericUnitID != null)
                {
                    currentPrice.GenericUnit = p.tblGenericUnits.unit;
                }
                currentPrice.OnSite = p.useOnSite;
                currentPrice.OnLine = p.useOnLine;

                Prices.Add(currentPrice);
            }

            //generar precios
            List<ComputedPriceModel> SecondaryPrices = new List<ComputedPriceModel>();
            //reglas de redondeo
            List<tblPriceTypesRounding> RoundingRules = new List<tblPriceTypesRounding>();
            date = date ?? DateTime.Today;
            foreach (ComputedPriceModel price in Prices)
            {
                //obtener las unidades para cada moneda
                tblPriceUnits usdUnit = GetUnit(price.PriceID, "en-US");
                tblPriceUnits mxnUnit = GetUnit(price.PriceID, "es-MX");

                //obtener el tipo de cambio aplicable
                //PriceExchangeRate exchangeRate = GetExchangeRateToApply(price.TerminalID, providerID, bookingDate, exchangeRateTypeID);
                PriceExchangeRate exchangeRate = GetExchangeRateToApply(price.TerminalID, providerID, (int)pointOfSaleID, bookingDate, exchangeRateTypeID);
                int costExchangeRateIDToApply = costExchangeRateTypeID;
                if (acer)//si en la terminal aplica current exchange rate
                {
                    if (costDate == null) //y no hay fecha de costo, aplicar ER general
                    {
                        costExchangeRateIDToApply = exchangeRateTypeID;
                    }
                }
                //PriceExchangeRate costExchangeRate = GetExchangeRateToApply(price.TerminalID, providerID, (acer ? (costDate != null ? costDate : DateTime.Now) : bookingDate), costExchangeRateIDToApply);
                PriceExchangeRate costExchangeRate = GetExchangeRateToApply(price.TerminalID, providerID, (int)pointOfSaleID, (acer ? (costDate != null ? costDate : DateTime.Now) : bookingDate), costExchangeRateIDToApply);

                //si la moneda base no es null (contrato en ambas monedas) generar el precio base de la moneda contraria
                if (baseCurrencyID != null)
                {
                    if (baseCurrencyID == 1 || baseCurrencyID == 2)
                    {
                        ComputedPriceModel pPrice = new ComputedPriceModel();
                        pPrice.PriceID = price.PriceID;
                        pPrice.ServiceID = price.ServiceID;
                        pPrice.GenericUnitID = price.GenericUnitID;
                        pPrice.SysItemTypeID = price.SysItemTypeID;
                        pPrice.PriceTypeID = price.PriceTypeID;
                        pPrice.PriceType = price.PriceType;
                        pPrice.PromoID = price.PromoID;
                        pPrice.IsCost = price.IsCost;
                        pPrice.IsMinimal = price.IsMinimal;
                        if (baseCurrencyID == 1)
                        {

                            pPrice.Price = price.Price * (price.IsCost ? costExchangeRate.Rate : exchangeRate.Rate);
                        }
                        else
                        {
                            pPrice.Price = price.Price / (price.IsCost ? costExchangeRate.Rate : exchangeRate.Rate);
                        }

                        decimal ceiling = Math.Ceiling(pPrice.Price);
                        string rounding = "";
                        if ((bookingDate ?? DateTime.Now) >= DateTime.Parse("2015-12-15"))
                        {
                            tblPriceTypesRounding roundingRule = new tblPriceTypesRounding();
                            if (RoundingRules.Count(x => x.priceTypeID == pPrice.PriceTypeID && x.terminalID == price.TerminalID) == 0)
                            {

                                DateTime cbookingDate = bookingDate ?? DateTime.Now;
                                var roundingRuleQ = (from r in db.tblPriceTypesRounding
                                                     where r.priceTypeID == pPrice.PriceTypeID
                                                     && r.terminalID == price.TerminalID
                                                     && ((r.fromDate <= cbookingDate && r.toDate == null)
                                                     || (r.fromDate <= cbookingDate && r.toDate >= cbookingDate))
                                                     select r).FirstOrDefault();

                                if (roundingRuleQ != null)
                                {
                                    RoundingRules.Add(roundingRuleQ);
                                }
                            }
                            roundingRule = RoundingRules.FirstOrDefault(x => x.priceTypeID == pPrice.PriceTypeID && x.terminalID == price.TerminalID);

                            if (roundingRule != null && (avoidRounding == null || avoidRounding == false))
                            {
                                if (roundingRule.roundUp && roundingRule.roundDown)
                                {
                                    //redondeo por cercanía a unidad
                                    pPrice.Price = (ceiling - pPrice.Price > decimal.Parse("0.5") ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                                    rounding = "Round Up & Round Down";
                                }
                                else if (roundingRule.roundUp && !roundingRule.roundDown)
                                {
                                    if (roundingRule.roundToFifty)
                                    {
                                        //redondeo a .50 y 1
                                        pPrice.Price = (ceiling - pPrice.Price > decimal.Parse("0.5") ? Math.Floor(pPrice.Price) + decimal.Parse(".50") : Math.Ceiling(pPrice.Price));
                                        rounding = "Round Up & Fifty";
                                    }
                                    else
                                    {
                                        //redondeo hacia arriba
                                        pPrice.Price = Math.Ceiling(pPrice.Price);
                                        rounding = "Round Up";
                                    }
                                }
                                else if (!roundingRule.roundUp && roundingRule.roundDown)
                                {
                                    //redondeo hacia abajo
                                    pPrice.Price = Math.Floor(pPrice.Price);
                                    rounding = "Round Down";
                                }
                                else
                                {
                                    rounding = "Not Rounded";
                                }
                            }
                            else
                            {
                                if (roundingRule == null)
                                {
                                    rounding = "No Rounding Rule";
                                }
                                else if (avoidRounding == true)
                                {
                                    rounding = "Avoid Rounding Activated";
                                }
                            }
                        }
                        else if (pPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-10-17"))
                        {
                            pPrice.Price = (ceiling - pPrice.Price > decimal.Parse("0.5") ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                        }
                        else if (pPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-09-12"))
                        {
                            pPrice.Price = (ceiling - pPrice.Price >= decimal.Parse("0.5") ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                        }
                        else if (pPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-04-14"))
                        {
                            pPrice.Price = (ceiling - pPrice.Price >= (1 / 2) ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                        }

                        pPrice.CurrencyID = (baseCurrencyID == 1 ? 2 : 1);
                        pPrice.CurrencyCode = (baseCurrencyID == 1 ? "MXN" : "USD");
                        pPrice.Permanent = price.Permanent;
                        pPrice.FromDate = price.FromDate;
                        pPrice.ToDate = price.ToDate;
                        pPrice.TwPermanent = price.TwPermanent;
                        pPrice.TwFromDate = price.TwFromDate;
                        pPrice.TwToDate = price.TwToDate;
                        pPrice.TerminalID = price.TerminalID;
                        pPrice.Terminal = price.Terminal;
                        pPrice.TaxesIncluded = price.TaxesIncluded;
                        pPrice.FromTransportationZoneID = price.FromTransportationZoneID;
                        pPrice.ToTransportationZoneID = price.ToTransportationZoneID;
                        pPrice.ToTransportationZone = price.ToTransportationZone;
                        if (culture == "")
                        {
                            pPrice.Culture = (pPrice.CurrencyID == 1 ? "en-US" : "es-MX"); ;
                        }
                        else
                        {
                            pPrice.Culture = culture;
                        }
                        tblPriceUnits unit = (pPrice.Culture == "es-MX" ? mxnUnit : usdUnit);
                        pPrice.Unit = unit.unit + (pPrice.SysItemTypeID == 3 ? " - " + pPrice.ToTransportationZone : "");
                        pPrice.AdditionalInfo = unit.additionalInfo;
                        pPrice.FullUnit = pPrice.Unit + " " + pPrice.AdditionalInfo;
                        pPrice.Min = unit.min;
                        pPrice.Max = unit.max;
                        pPrice.MinMax = unit.min + " - " + unit.max;
                        pPrice.ExchangeRateID = (pPrice.IsCost ? costExchangeRate.RateID : exchangeRate.RateID);
                        pPrice.Base = false;
                        pPrice.Rounding = rounding;
                        pPrice.Rule = "Based on " + (baseCurrencyID == 1 ? "USD" : "MXN") + " Price (";
                        if (pPrice.ExchangeRateID != null && pPrice.ExchangeRateID != 0)
                        {
                            tblExchangeRates rate = db.tblExchangeRates.SingleOrDefault(x => x.exchangeRateID == pPrice.ExchangeRateID);
                            pPrice.Rule += rate.exchangeRate + " " + (rate.providerID != null ? provider : pPrice.Terminal) + " - " + rate.tblExchangeRateTypes.exchangeRateType + ")";
                        }
                        pPrice.DependingOnPriceID = price.DependingOnPriceID;
                        pPrice.DependingOnPriceQuantity = price.DependingOnPriceQuantity;
                        pPrice.Highlight = price.Highlight;
                        pPrice.Service_PriceTypeID = null;
                        SecondaryPrices.Add(pPrice);
                    }
                }

                foreach (PriceRuleModel rule in Rules.Where(r => r.IsBasePrice != true))
                {
                    if ((price.GenericUnitID == null && rule.GenericUnitID == null)
                        || (price.GenericUnitID != null && Rules.Count(r => r.GenericUnitID == price.GenericUnitID && r.PriceTypeID == rule.PriceTypeID) == 0 && rule.GenericUnitID == null)
                        || (price.GenericUnitID != null && Rules.Count(r => r.GenericUnitID == price.GenericUnitID && r.PriceTypeID == rule.PriceTypeID) > 0 && rule.GenericUnitID == price.GenericUnitID))
                    {
                        decimal oPrice = price.Price;
                        ComputedPriceModel cPrice = new ComputedPriceModel();
                        cPrice.PriceID = price.PriceID;
                        cPrice.ServiceID = price.ServiceID;
                        cPrice.GenericUnitID = price.GenericUnitID;
                        cPrice.SysItemTypeID = price.SysItemTypeID;
                        cPrice.PriceTypeID = rule.PriceTypeID;
                        cPrice.PriceType = rule.PriceType;
                        cPrice.PromoID = rule.PromoID;
                        cPrice.IsCost = rule.IsCost;
                        cPrice.IsMinimal = rule.IsMinimal;

                        //agregar funcionalidad por fórmula
                        //if (rule.Service_PriceTypeID == 17542)
                        //{
                        //    rule.Formula = "((P1-P3)/2)+P3";
                        //}
                        if (rule.Formula != null)
                        {
                            string formula = rule.Formula.Replace(" ", "").Replace("=", "");

                            while (formula.IndexOf("P") > -1)
                            {
                                bool isNumeric = false;
                                try
                                {
                                    decimal dec = int.Parse(formula.Substring(formula.IndexOf("P") + 1, 2));
                                    isNumeric = true;
                                    //if (dec >= 0 && dec < 10)
                                    //{
                                    //    isNumeric = true;
                                    //}                                
                                }
                                catch (Exception e)
                                {

                                }

                                int formulaBasePriceTypeID = int.Parse(isNumeric ? formula.Substring(formula.IndexOf("P") + 1, 2) : formula.Substring(formula.IndexOf("P") + 1, 1));

                                char[] separators = { '+', '-', '(', ')', '/', '*' };
                                var segments = formula.Split(separators);
                                for (int i = 0; i < segments.Length; i++)
                                {
                                    string newSegment = segments[i];
                                    if (segments[i].EndsWith("%"))
                                    {
                                        newSegment = "(P" + formulaBasePriceTypeID + "*" + segments[i].Substring(0, segments[i].Length - 1) + "/100)";
                                    }
                                    if (newSegment.IndexOf("P") >= 0)
                                    {
                                        //newSegment = newSegment.Replace("P" + formulaBasePriceTypeID, (price.PriceTypeID == rule.ThanPriceTypeID ? oPrice.ToString() : SecondaryPrices.Where(x => x.PriceTypeID == rule.ThanPriceTypeID && x.CurrencyID == price.CurrencyID && x.PriceID == cPrice.PriceID).FirstOrDefault().Price.ToString()));
                                        newSegment = newSegment.Replace("P" + formulaBasePriceTypeID, (price.PriceTypeID == formulaBasePriceTypeID ? oPrice.ToString() : SecondaryPrices.Where(x => x.PriceTypeID == formulaBasePriceTypeID && x.CurrencyID == price.CurrencyID && x.PriceID == cPrice.PriceID).FirstOrDefault().Price.ToString()));
                                        formula = formula.Replace(segments[i], newSegment);
                                    }
                                }
                            }

                            NCalc.Expression exp = new NCalc.Expression(formula);
                            cPrice.Price = decimal.Parse(exp.Evaluate().ToString());

                        }
                        else
                        {
                            //mayor o menor que
                            if ((bool)rule.MoreOrLess)
                            {
                                //mayor que
                                if (price.PriceTypeID == rule.ThanPriceTypeID)
                                {
                                    cPrice.Price = oPrice + (oPrice * (decimal)rule.Percentage / 100);
                                }
                                else
                                {
                                    //buscar precio calculado como base
                                    var calcPrice = SecondaryPrices.Where(x => x.PriceTypeID == rule.ThanPriceTypeID && x.CurrencyID == price.CurrencyID && x.PriceID == cPrice.PriceID);
                                    if (calcPrice.Count() > 0)
                                    {
                                        cPrice.Price = calcPrice.FirstOrDefault().Price + (calcPrice.FirstOrDefault().Price * (decimal)rule.Percentage / 100);
                                    }
                                }
                            }
                            else
                            {
                                //menor que
                                if (price.PriceTypeID == rule.ThanPriceTypeID)
                                {
                                    cPrice.Price = oPrice - (oPrice * (decimal)rule.Percentage / 100);
                                }
                                else
                                {
                                    //buscar precio calculado como base
                                    var calcPrice = SecondaryPrices.Where(x => x.PriceTypeID == rule.ThanPriceTypeID && x.CurrencyID == price.CurrencyID && x.PriceID == cPrice.PriceID);
                                    if (calcPrice.Count() > 0)
                                    {
                                        cPrice.Price = calcPrice.FirstOrDefault().Price - (calcPrice.FirstOrDefault().Price * (decimal)rule.Percentage / 100);
                                    }
                                }
                            }
                        }

                        decimal ceiling = Math.Ceiling(cPrice.Price);
                        string rounding = "";
                        if ((bookingDate ?? DateTime.Now) >= DateTime.Parse("2015-12-15"))
                        {
                            tblPriceTypesRounding roundingRule = new tblPriceTypesRounding();
                            if (RoundingRules.Count(x => x.priceTypeID == cPrice.PriceTypeID && x.terminalID == price.TerminalID) == 0)
                            {
                                DateTime cbookingDate = bookingDate ?? DateTime.Now;
                                var roundingRuleQ = (from r in db.tblPriceTypesRounding
                                                     where r.priceTypeID == cPrice.PriceTypeID
                                                     && r.terminalID == price.TerminalID
                                                     && ((r.fromDate <= cbookingDate && r.toDate == null)
                                                     || (r.fromDate <= cbookingDate && r.toDate >= cbookingDate))
                                                     select r).FirstOrDefault();

                                if (roundingRuleQ != null)
                                {
                                    RoundingRules.Add(roundingRuleQ);
                                }
                            }
                            roundingRule = RoundingRules.FirstOrDefault(x => x.priceTypeID == cPrice.PriceTypeID && x.terminalID == price.TerminalID);

                            if (roundingRule != null && (avoidRounding == null || avoidRounding == false))
                            {
                                if (roundingRule.roundUp && roundingRule.roundDown)
                                {
                                    //redondeo por cercanía a unidad
                                    cPrice.Price = (ceiling - cPrice.Price > decimal.Parse("0.5") ? Math.Floor(cPrice.Price) : Math.Ceiling(cPrice.Price));
                                    rounding = "Round Up & Round Down";
                                }
                                else if (roundingRule.roundUp && !roundingRule.roundDown)
                                {
                                    if (roundingRule.roundToFifty)
                                    {
                                        //redondeo a .50 y 1
                                        cPrice.Price = (ceiling - cPrice.Price > decimal.Parse("0.5") ? Math.Floor(cPrice.Price) + decimal.Parse(".50") : Math.Ceiling(cPrice.Price));
                                        rounding = "Round Up & Fifty";
                                    }
                                    else
                                    {
                                        //redondeo hacia arriba
                                        cPrice.Price = Math.Ceiling(cPrice.Price);
                                        rounding = "Round Up";
                                    }
                                }
                                else if (!roundingRule.roundUp && roundingRule.roundDown)
                                {
                                    //redondeo hacia abajo
                                    cPrice.Price = Math.Floor(cPrice.Price);
                                    rounding = "Round Down";
                                }
                                else
                                {
                                    rounding = "Not Rounded";
                                }
                            }
                            else
                            {
                                if (roundingRule == null)
                                {
                                    rounding = "No Rounding Rule";
                                }
                                else if (avoidRounding == true)
                                {
                                    rounding = "Avoid Rounding Activated";
                                }
                            }
                        }
                        else if (cPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-10-17"))
                        {
                            cPrice.Price = (ceiling - cPrice.Price > decimal.Parse("0.5") ? Math.Floor(cPrice.Price) : Math.Ceiling(cPrice.Price));
                        }
                        else if (cPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-09-12"))
                        {
                            cPrice.Price = (ceiling - cPrice.Price >= decimal.Parse("0.5") ? Math.Floor(cPrice.Price) : Math.Ceiling(cPrice.Price));
                        }
                        else if (cPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-04-14"))
                        {
                            cPrice.Price = (ceiling - cPrice.Price >= (1 / 2) ? Math.Floor(cPrice.Price) : Math.Ceiling(cPrice.Price));
                        }

                        cPrice.CurrencyID = price.CurrencyID;
                        cPrice.CurrencyCode = price.CurrencyCode;
                        cPrice.Permanent = price.Permanent;
                        cPrice.FromDate = price.FromDate;
                        cPrice.ToDate = price.ToDate;
                        cPrice.TwPermanent = price.TwPermanent;
                        cPrice.TwFromDate = price.TwFromDate;
                        cPrice.TwToDate = price.TwToDate;
                        cPrice.TerminalID = price.TerminalID;
                        cPrice.Terminal = price.Terminal;
                        cPrice.TaxesIncluded = price.TaxesIncluded;
                        cPrice.FromTransportationZoneID = price.FromTransportationZoneID;
                        cPrice.ToTransportationZoneID = price.ToTransportationZoneID;
                        cPrice.ToTransportationZone = price.ToTransportationZone;
                        cPrice.Culture = price.Culture;
                        cPrice.Unit = price.Unit;
                        cPrice.AdditionalInfo = price.AdditionalInfo;
                        cPrice.FullUnit = cPrice.Unit + " " + cPrice.AdditionalInfo;
                        cPrice.Min = price.Min;
                        cPrice.Max = price.Max;
                        cPrice.MinMax = price.MinMax;
                        cPrice.ExchangeRateID = null;
                        cPrice.Base = false;
                        cPrice.Rounding = rounding;
                        if (rule.Formula != null)
                        {
                            cPrice.Rule = "Based on formula: " + rule.Formula + " (Level " + rule.Level + ")";
                        }
                        else
                        {
                            cPrice.Rule = "Based on " + rule.Percentage.ToString().Replace(".00", "") + "%" + ((bool)rule.MoreOrLess ? " > " : " < ") + rule.ThanPriceType + " (Level " + rule.Level + ")";
                        }
                        cPrice.DependingOnPriceID = price.DependingOnPriceID;
                        cPrice.DependingOnPriceQuantity = price.DependingOnPriceQuantity;
                        cPrice.Highlight = price.Highlight;
                        cPrice.Service_PriceTypeID = rule.Service_PriceTypeID;
                        SecondaryPrices.Add(cPrice);

                        if (baseCurrencyID != null)
                        {
                            if (baseCurrencyID == 1 || baseCurrencyID == 2)
                            {
                                ComputedPriceModel pPrice = new ComputedPriceModel();
                                pPrice.PriceID = price.PriceID;
                                pPrice.ServiceID = price.ServiceID;
                                pPrice.GenericUnitID = price.GenericUnitID;
                                pPrice.SysItemTypeID = price.SysItemTypeID;
                                pPrice.PriceTypeID = rule.PriceTypeID;
                                pPrice.PromoID = rule.PromoID;
                                pPrice.IsCost = rule.IsCost;
                                pPrice.IsMinimal = rule.IsMinimal;
                                if (baseCurrencyID == 1)
                                {
                                    pPrice.Price = cPrice.Price * (cPrice.IsCost ? costExchangeRate.Rate : exchangeRate.Rate);
                                }
                                else
                                {
                                    pPrice.Price = cPrice.Price / (cPrice.IsCost ? costExchangeRate.Rate : exchangeRate.Rate);
                                }

                                ceiling = Math.Ceiling(pPrice.Price);
                                if ((bookingDate ?? DateTime.Now) >= DateTime.Parse("2015-12-15"))
                                {
                                    tblPriceTypesRounding roundingRule = new tblPriceTypesRounding();
                                    if (RoundingRules.Count(x => x.priceTypeID == pPrice.PriceTypeID && x.terminalID == price.TerminalID) == 0)
                                    {

                                        var roundingRuleQ = (from r in db.tblPriceTypesRounding
                                                             where r.priceTypeID == pPrice.PriceTypeID
                                                             && r.terminalID == price.TerminalID
                                                             && ((r.fromDate <= bookingDate && r.toDate == null)
                                                     || (r.fromDate <= bookingDate && r.toDate >= r.toDate))
                                                             select r).FirstOrDefault();

                                        if (roundingRuleQ != null)
                                        {
                                            RoundingRules.Add(roundingRuleQ);
                                        }
                                    }
                                    roundingRule = RoundingRules.FirstOrDefault(x => x.priceTypeID == pPrice.PriceTypeID && x.terminalID == price.TerminalID);

                                    if (roundingRule != null && (avoidRounding == null || avoidRounding == false))
                                    {
                                        if (roundingRule.roundUp && roundingRule.roundDown)
                                        {
                                            //redondeo por cercanía a unidad
                                            pPrice.Price = (ceiling - pPrice.Price > decimal.Parse("0.5") ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                                            rounding = "Round Up & Round Down";
                                        }
                                        else if (roundingRule.roundUp && !roundingRule.roundDown)
                                        {
                                            if (roundingRule.roundToFifty)
                                            {
                                                //redondeo a .50 y 1
                                                pPrice.Price = (ceiling - pPrice.Price > decimal.Parse("0.5") ? Math.Floor(pPrice.Price) + decimal.Parse(".50") : Math.Ceiling(pPrice.Price));
                                                rounding = "Round Up & Fifty";
                                            }
                                            else
                                            {
                                                //redondeo hacia arriba
                                                pPrice.Price = Math.Ceiling(pPrice.Price);
                                                rounding = "Round Up";
                                            }
                                        }
                                        else if (!roundingRule.roundUp && roundingRule.roundDown)
                                        {
                                            //redondeo hacia abajo
                                            pPrice.Price = Math.Floor(pPrice.Price);
                                            rounding = "Round Down";
                                        }
                                        else
                                        {
                                            rounding = "Not Rounded";
                                        }
                                    }
                                    else
                                    {
                                        if (roundingRule == null)
                                        {
                                            rounding = "No Rounding Rule";
                                        }
                                        else if (avoidRounding == true)
                                        {
                                            rounding = "Avoid Rounding Activated";
                                        }
                                    }
                                }
                                else if (pPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-10-17"))
                                {
                                    pPrice.Price = (ceiling - pPrice.Price > decimal.Parse("0.5") ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                                }
                                else if (pPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-09-12"))
                                {
                                    pPrice.Price = (ceiling - pPrice.Price >= decimal.Parse("0.5") ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                                }
                                else if (pPrice.PriceTypeID != 3 && bookingDate >= DateTime.Parse("2015-04-14"))
                                {
                                    pPrice.Price = (ceiling - pPrice.Price >= (1 / 2) ? Math.Floor(pPrice.Price) : Math.Ceiling(pPrice.Price));
                                }

                                pPrice.CurrencyID = (baseCurrencyID == 1 ? 2 : 1);
                                pPrice.CurrencyCode = (baseCurrencyID == 1 ? "MXN" : "USD");
                                pPrice.Permanent = price.Permanent;
                                pPrice.FromDate = price.FromDate;
                                pPrice.ToDate = price.ToDate;
                                pPrice.TwPermanent = price.TwPermanent;
                                pPrice.TwFromDate = price.TwFromDate;
                                pPrice.TwToDate = price.TwToDate;
                                pPrice.TerminalID = price.TerminalID;
                                pPrice.Terminal = price.Terminal;
                                pPrice.TaxesIncluded = price.TaxesIncluded;
                                pPrice.FromTransportationZoneID = price.FromTransportationZoneID;
                                pPrice.ToTransportationZoneID = price.ToTransportationZoneID;
                                pPrice.ToTransportationZone = price.ToTransportationZone;
                                if (culture == "")
                                {
                                    pPrice.Culture = (pPrice.CurrencyID == 1 ? "en-US" : "es-MX");
                                }
                                else
                                {
                                    pPrice.Culture = culture;
                                }
                                tblPriceUnits unit = (pPrice.Culture == "es-MX" ? mxnUnit : usdUnit);
                                pPrice.Unit = unit.unit + (pPrice.SysItemTypeID == 3 ? " - " + pPrice.ToTransportationZone : "");
                                pPrice.AdditionalInfo = unit.additionalInfo;
                                pPrice.FullUnit = pPrice.Unit + " " + pPrice.AdditionalInfo;
                                pPrice.Min = unit.min;
                                pPrice.Max = unit.max;
                                pPrice.MinMax = unit.min + " - " + unit.max;
                                pPrice.ExchangeRateID = (pPrice.IsCost ? costExchangeRate.RateID : exchangeRate.RateID);
                                pPrice.Base = false;
                                pPrice.Rounding = rounding;
                                pPrice.Rule = "Based on " + (baseCurrencyID == 1 ? "USD" : "MXN") + " Price (";
                                if (pPrice.ExchangeRateID != null && pPrice.ExchangeRateID != 0)
                                {
                                    tblExchangeRates rate = db.tblExchangeRates.SingleOrDefault(x => x.exchangeRateID == pPrice.ExchangeRateID);
                                    pPrice.Rule += rate.exchangeRate + " " + (rate.providerID != null ? provider : pPrice.Terminal) + " - " + rate.tblExchangeRateTypes.exchangeRateType + ")";
                                }
                                pPrice.DependingOnPriceID = price.DependingOnPriceID;
                                pPrice.DependingOnPriceQuantity = price.DependingOnPriceQuantity;
                                pPrice.Highlight = price.Highlight;
                                pPrice.Service_PriceTypeID = null;
                                SecondaryPrices.Add(pPrice);
                            }
                        }
                    }
                }
            }

            //concatenar los precios obtenidos secundariamente con la lista original
            var NewPrices = Prices.Concat(SecondaryPrices);

            //regresar la colección
            return NewPrices.ToList();
        }

        public List<SelectListItem> GetDDLData(string itemType, string itemID)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();

            switch (itemType)
            {
                case "priceTypesPerSelectedTerminals":
                    {
                        var _itemID = itemID != null && itemID != "0" ? long.Parse(itemID) : (long?)null;
                        list = PricesCatalogs.FillDrpPriceTypes(_itemID);
                        list.Insert(0, ListItems.Default());
                        break;
                    }
            }
            return list;
        }
    }
}
