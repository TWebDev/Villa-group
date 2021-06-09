using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Globalization;
using ePlatBack.Models.Utils;
using ePlatBack.Models.ViewModels;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using ePlatBack.Models.eplatformDataModel;


namespace ePlatBack.Models.DataModels
{
    public class ActivityDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();

        public class ActivitiesCatalogs
        {
            public static List<SelectListItem> FillDrpCategories()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var catalogs = (from t in db.tblCatalogs_Terminals where terminals.Contains(t.terminalID) select new { t.catalogID, t.tblTerminals.prefix }).Distinct();
                if (catalogs.Count() > 0)
                {
                    foreach (var i in catalogs)
                    {
                        var categories = (from c in db.tblCategories
                                          where c.active == true
                                          && c.parentCategoryID != null
                                          && c.catalogID == i.catalogID
                                          select new
                                          {
                                              c.categoryID,
                                              c.category
                                          }).Distinct();
                        foreach (var c in categories.OrderBy(m => m.category))
                        {
                            list.Add(new SelectListItem()
                            {
                                Value = c.categoryID.ToString(),
                                Text = i.prefix + " - " + c.category
                            });
                        }
                    }
                    list.Insert(0, ListItems.Default());
                }

                return list;
            }

            public static List<SelectListItem> FillDrpTerminals()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in TerminalDataModel.GetCurrentUserTerminals().OrderBy(m => m.Text))
                    list.Add(new SelectListItem() { Value = i.Value, Text = i.Text });
                return list;
            }

            public static List<SelectListItem> FillDrpDestinations(int? terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var terminal = terminalID ?? null;
                if (terminal != null && terminal != 0)
                {
                    var destinations = db.tblTerminals_Destinations.Where(m => m.terminalID == terminal).Select(m => new { m.destinationID, m.tblDestinations.destination }).ToArray();
                    foreach (var i in destinations.OrderBy(m => m.destination))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.destinationID.ToString(),
                            Text = i.destination
                        });
                    }
                }
                else
                {
                    list = PlaceDataModel.GetDestinationsByCurrentTerminals();
                }
                return list;
            }

            public static List<SelectListItem> FillDrpDestinationsByTerminal(int terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (terminalID != 0)
                {
                    var destinations = db.tblTerminals_Destinations.Where(m => m.terminalID == terminalID).Select(m => new { m.destinationID, m.tblDestinations.destination }).ToArray();
                    foreach (var i in destinations.OrderBy(m => m.destination))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.destinationID.ToString(),
                            Text = i.destination
                        });
                    }
                    list.Insert(0, ListItems.Default());
                }
                else
                {
                    list.Insert(0, ListItems.Default("--Select Terminal--"));
                }
                return list;
            }
            public static List<SelectListItem> FillDrpDestinationsBySelectedTerminals()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                foreach (var i in db.tblTerminals_Destinations.Where(m => terminals.Contains(m.terminalID)).Select(m => new { m.destinationID, m.tblDestinations.destination }))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.destinationID.ToString(),
                        Text = i.destination
                    });
                }
                return list;
            }
            public static List<SelectListItem> FillDrpProvidersByDestination(long terminal, int destinationID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = terminal != 0 ? new long?[] { (long?)terminal } : session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();

                var regions = destinationID != 0 ? db.tblRegions_Destinations.Where(m => m.destinationID == destinationID).Select(m => m.regionID).Distinct().ToArray() : new int[] { };
                var destinations = regions.Count() > 0 ? db.tblRegions_Destinations.Where(m => regions.Contains(m.regionID)).Select(m => m.destinationID).Distinct().ToArray() : destinationID != 0 ? new long[] { destinationID } : new long[] { };

                foreach (var i in db.tblProviders.Where(m => (destinations.Count() == 0 || destinations.Contains(m.destinationID)) && m.isActive && terminals.Contains(m.terminalID)).OrderBy(m => m.comercialName))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.providerID.ToString(),
                        Text = i.comercialName + " - " + i.taxesName
                    });
                }

                return list;
            }

            public static List<SelectListItem> FillDrpZones(int destinationID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (destinationID != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    foreach (var i in db.tblZones.Where(m => m.destinationID == destinationID).Select(m => m).OrderBy(m => m.zone))
                        list.Add(new SelectListItem() { Value = i.zoneID.ToString(), Text = i.zone });
                }
                else
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Destination--", Selected = false });
                return list;
            }

            public static List<SelectListItem> FillDrpCultures()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblLanguages)
                    list.Add(new SelectListItem() { Value = i.culture.ToString(), Text = i.language });
                return list;
            }

            public static List<SelectListItem> FillDrpSeoItems(string itemType, int itemID)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                if (itemType != "")
                {
                    list = SeoItemDataModel.SeoItemCatalogs.FillDrpSeoItemsRelated(itemType, itemID);
                }
                list.Insert(0, ListItems.Default());
                return list;
            }

            public static List<SelectListItem> FillDrpCatalogsPerTerminal(int terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (terminalID != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    var query = from catalog in db.tblCatalogs_Terminals
                                where catalog.terminalID == terminalID
                                select catalog;
                    foreach (var i in query.OrderBy(m => m.tblCatalogs.catalog))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.catalogID.ToString(),
                            Text = i.tblCatalogs.catalog
                        });
                    }
                }
                else
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Terminal--", Selected = false });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpCategoriesPerCatalog(int catalogID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (catalogID != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    var query = from category in db.tblCategories
                                where category.catalogID == catalogID
                                && category.active == true
                                && (category.parentCategoryID != null || category.category == "Airport Transfer")
                                select new
                                {
                                    categoryID = category.categoryID,
                                    parentCategory = category.tblCategories2.category,
                                    categoryName = category.category
                                };

                    if (query.Count() == 0)
                    {
                        query = from category in db.tblCategories
                                where category.catalogID == catalogID
                                && category.active == true
                                select new
                                {
                                    categoryID = category.categoryID,
                                    parentCategory = category.tblCategories2.category,
                                    categoryName = category.category
                                };
                    }

                    foreach (var i in query.OrderBy(m => m.parentCategory).ThenBy(m => m.categoryName))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.categoryID.ToString(),
                            Text = (i.parentCategory != null ? i.parentCategory + " > " : "") + i.categoryName
                        });
                    }
                }
                else
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Catalog--", Selected = false });
                return list;
            }

            public static List<SelectListItem> FillDrpWeeklySchedule(int activityID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (activityID != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    foreach (var i in db.tblWeeklyAvailability.Where(m => m.serviceID == activityID).Select(m => m).OrderBy(m => m.hour))
                    {
                        var weekDays = "";
                        if (i.monday)
                            weekDays += "Monday, ";
                        if (i.tuesday)
                            weekDays += "Tuesday, ";
                        if (i.wednesday)
                            weekDays += "Wednesday, ";
                        if (i.thursday)
                            weekDays += "Thursday, ";
                        if (i.friday)
                            weekDays += "Friday, ";
                        if (i.saturday)
                            weekDays += "Saturday, ";
                        if (i.sunday)
                            weekDays += "Sunday, ";
                        weekDays = weekDays != "" ? weekDays.Substring(0, weekDays.Length - 2) : weekDays;
                        list.Add(new SelectListItem()
                        {
                            Value = i.weeklyAvailabilityID.ToString(),
                            Text = i.hour + " " + weekDays
                        });
                    }
                }
                return list;
            }

            public static List<SelectListItem> FillDrpPlaces(int terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (terminalID != 0)
                {
                    var destinations = db.tblTerminals_Destinations.Where(m => m.terminalID == terminalID).Select(m => m.destinationID).ToArray();

                    foreach (var i in db.tblPlaces.Where(m => destinations.Contains(m.destinationID)).OrderBy(m => m.place))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.placeID.ToString(),
                            Text = i.place.TrimEnd() + " " + i.tblDestinations.destination.TrimStart()
                        });
                    }
                }
                else
                {
                    list = ActivitiesCatalogs.FillDrpPlaces();
                }
                return list;
            }

            public static List<SelectListItem> FillDrpPlaces()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                foreach (var i in db.tblPlaces_Terminals.Where(m => terminals.Contains(m.terminalID)))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.placeID.ToString(),
                        Text = i.tblPlaces.place
                    });
                }
                list.Insert(0, ListItems.Default());
                return list;
            }

            //--
            public static List<SelectListItem> PrevFillDrpCategories()
            {
                ecommerceEntities dba = new ecommerceEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var query = from a in dba.tbaCategorias
                            where a.tbaCatalogos.catalogo == "Activities"
                            select a;
                foreach (var i in query.OrderBy(m => m.categoria))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.idCategoria.ToString(),
                        Text = i.categoria
                    });
                }
                return list;
            }

            public static List<SelectListItem> PrevFillDrpProviders()
            {
                ecommerceEntities dba = new ecommerceEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                foreach (var i in dba.tbaProveedores.OrderBy(m => m.razonComercial))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.idProveedor.ToString(),
                        Text = i.razonComercial
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpTimes()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                return list;
            }

            public static List<SelectListItem> FillDrpProviderTypes()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                foreach (var i in db.tblProviderTypes.OrderBy(m => m.providerType))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.providerTypeID.ToString(),
                        Text = i.providerType
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpPointsOfSale()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                foreach (var i in db.tblPointsOfSale.OrderBy(m => m.shortName))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.pointOfSaleID.ToString(),
                        Text = i.shortName + " - " + i.pointOfSale
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpAccountingAccounts()
            {
                return FillDrpAccountingAccounts(null);
            }

            public static List<SelectListItem> FillDrpAccountingAccounts(long? terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                var terminals = new List<long>();
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
                var companies = db.tblTerminals_Companies.Where(m => terminals.Contains(m.terminalID)).Select(m => (int)m.companyID).ToArray();
                var list = new List<SelectListItem>();

                var query = db.tblAccountingAccounts.Where(m => companies.Contains(m.companyID));

                foreach (var i in query.OrderBy(m => m.accountName).ThenBy(m => m.accountType).ThenBy(m => m.account))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.accountingAccountID.ToString(),
                        Text = i.account + " - " + i.accountName + " - " + (i.accountType ? "Income" : "Outcome") + (i.priceTypeID != null ? " - " + i.tblPriceTypes.priceType : "")
                    });
                }
                return list;
            }

            public static List<SelectListItem> GetUnitTypes()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var query = db.tblPriceUnits.Select(m => m.unit).Distinct();
                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i,
                        Text = i
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpRegions()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var destinations = PlaceDataModel.GetDestinationsByCurrentTerminals().Select(m => long.Parse(m.Value)).ToArray();
                var query = (from r in db.tblRegions_Destinations
                             where destinations.Contains(r.destinationID)
                             select new
                             {
                                 r.regionID,
                                 r.tblRegions.region
                             }).Distinct();
                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.regionID.ToString(),
                        Text = i.region
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpProvidersByRegion(int regionID)
            {
                ePlatEntities db = new ePlatEntities();
                var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                var list = new List<SelectListItem>();

                long?[] destinations = regionID != 0 ? db.tblRegions_Destinations.Where(m => m.regionID == regionID).Select(m => (long?)m.destinationID).ToArray() : new long?[] { };
                foreach (var i in db.tblProviders.Where(m => (destinations.Count() == 0 || destinations.Contains(m.destinationID)) && terminals.Contains(m.terminalID) && m.isActive).OrderBy(x => x.comercialName))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.providerID.ToString(),
                        Text = i.comercialName + " - " + i.taxesName
                    });
                }

                return list;
            }

            public static List<SelectListItem> FillDrpServicesPerProvider(int providerID)
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblServices.Where(m => m.isTest != true && m.providerID == providerID && m.deleted != true);

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.serviceID.ToString(),
                        Text = i.service
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpServicesPerTerminal(long? TerminalID)
            {
                ePlatEntities db = new ePlatEntities();
                var terminals = TerminalID != null ? new long?[] { TerminalID } : session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();
                var list = new List<SelectListItem>();
                var query = from t in db.tblServices
                            where terminals.Contains(t.originalTerminalID)
                            select t;
                foreach (var i in query.OrderBy(m => m.service))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.serviceID.ToString(),
                        Text = i.service
                    });
                }
                return list;
            }

            public static List<SelectListItem> GetServicesPerTerminalsActives()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                long[] currentTerminals = session.Terminals != "" ? (from m in session.Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray() : new long[] { };

                foreach (var i in db.tblServices.Where(m => currentTerminals.Contains(m.originalTerminalID)).OrderBy(m => m.service))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.serviceID.ToString(),
                        Text = i.service
                    });
                }
                return list;
            }

            public static bool HasBaseRule(long serviceID, string service, long? terminalID)
            {
                var rules = PriceDataModel.GetRules(serviceID, terminalID, null);
                var level = service == null ? "Provider" : "Activity";
                return rules.Where(m => m.Level == level && m.IsBasePrice).Count() > 0 ? true : false;
            }

            /// <summary>
            /// Check if rules with same properties exist and are active.
            /// </summary>
            /// <param name="model"></param>
            /// <param name="serviceID"></param>
            /// <param name="now"></param>
            /// <returns>Returns query with rules found.</returns>
            public static IQueryable<tblServices_PriceTypes> RuleExists(ref ePlatEntities db, PriceTypeRulesInfoModel model, long? serviceID, DateTime? now)
            {
                var query = db.tblServices_PriceTypes.Where(m => m.terminalID == model.PriceTypeRulesInfo_Terminal);//same terminal rules
                if (serviceID != null)//service level rules
                {
                    query = query.Where(m => m.serviceID == serviceID);
                }
                else//provider level rules
                {
                    var providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                    query = query.Where(m => m.providerID == providerID && m.serviceID == null);
                }
                query = query.Where(m => m.priceTypeID == model.PriceTypeRulesInfo_PriceType);
                query = query.Where(m => m.thanPriceTypeID == model.PriceTypeRulesInfo_ThanPriceType);
                query = query.Where(m => m.fromDate <= now);
                query = query.Where(m => (m.permanent_ == true || m.toDate >= now));

                if (model.PriceTypeRulesInfo_GenericUnit != null)
                {
                    query = query.Where(m => m.genericUnitID == model.PriceTypeRulesInfo_GenericUnit);
                }
                else
                {
                    query = query.Where(m => m.genericUnitID == null);
                }

                return query;
            }

            public static List<SelectListItem> FillDrpAvailabilityPerDate(long serviceID, DateTime date)
            {
                ActivityDataModel adm = new ActivityDataModel();
                return adm.GetSchedulesForDate(serviceID, date);
            }

            public static IEnumerable<PriceRuleModel> GetRelatedRules(PriceTypeRulesInfoModel model, long serviceID, DateTime date)
            {
                ePlatEntities db = new ePlatEntities();
                List<PriceRuleModel> _rules = PriceDataModel.GetRules(serviceID, model.PriceTypeRulesInfo_Terminal, date);
                _rules.Concat(PriceDataModel.GetFutureRules(serviceID)).ToList<PriceRuleModel>();
                var rules = _rules.Where(m => (m.PriceTypeID == model.PriceTypeRulesInfo_PriceType || m.ThanPriceTypeID == model.PriceTypeRulesInfo_PriceType) && (model.PriceTypeRulesInfo_GenericUnit != null ? m.GenericUnitID == model.PriceTypeRulesInfo_GenericUnit : model.PriceTypeRulesInfo_GenericUnit == null));

                return rules;
            }

            public static string ServicesSoldAfterDate(long serviceID, IEnumerable<PriceRuleModel> rules, DateTime date)
            {
                ePlatEntities db = new ePlatEntities();
                var _rules = "";
                foreach (var rule in rules)
                {
                    if (db.tblPurchases_Services.Where(m => m.tblPurchases.terminalID == rule.TerminalID
                        && m.serviceID == serviceID
                        && m.dateSaved >= date
                        && (rule.IsPermanent || date <= rule.ToDate)
                        && m.tblPurchaseServiceDetails.Select(x => (int)x.priceTypeID).Contains(rule.PriceTypeID)).Count() > 0)
                    {
                        _rules += (_rules == "" ? "" : ", ") + rule.RuleFor;
                        //return true;
                    }
                }
                //return false;
                return _rules;
            }

            public static List<PriceRuleModel> GetChildrenRules(int providerID, long? serviceID, int? genericUnit, DateTime date, IEnumerable<PriceRuleModel> relatedRules)
            {
                ePlatEntities db = new ePlatEntities();

                var _relatedRules = relatedRules.Select(m => m.PriceTypeID).ToArray();
                var _rules = db.tblServices_PriceTypes.Where(m => (m.toDate == null || (m.toDate >= date && m.fromDate <= date) || m.fromDate > date) && m.providerID == providerID && (serviceID == null ? m.serviceID != null : m.serviceID == serviceID && (genericUnit != null ? genericUnit == m.genericUnitID : m.genericUnitID != null)) && (_relatedRules.Contains(m.priceTypeID)));
                //var _rules = db.tblServices_PriceTypes.Where(m => (m.toDate == null || (m.toDate >= date && m.fromDate <= date)) && m.providerID == providerID && (serviceID == null ? m.serviceID != null : m.serviceID == serviceID && (genericUnit != null ? genericUnit == m.genericUnitID : m.genericUnitID != null)) && (_relatedRules.Contains(m.priceTypeID)));

                var childrenRules = new List<PriceRuleModel>();
                foreach (var rule in _rules)
                {
                    PriceRuleModel ruleModel = new PriceRuleModel();
                    ruleModel.Service_PriceTypeID = rule.service_priceTypeID;
                    ruleModel.TerminalID = rule.terminalID;
                    ruleModel.ProviderID = rule.providerID;
                    ruleModel.ServiceID = rule.serviceID;
                    ruleModel.GenericUnitID = rule.genericUnitID;
                    ruleModel.RuleFrom = rule.genericUnitID != null ? rule.tblGenericUnits.unit : rule.serviceID != null ? rule.tblServices.service : rule.providerID != null ? rule.tblProviders.comercialName : rule.tblTerminals.terminal;
                    ruleModel.PriceTypeID = rule.priceTypeID;
                    ruleModel.PromoID = rule.tblPriceTypes.promoID;
                    ruleModel.IsCost = rule.tblPriceTypes.isCost;
                    ruleModel.RuleFor = rule.tblPriceTypes.priceType;
                    ruleModel.IsBasePrice = rule.@base;
                    ruleModel.Percentage = rule.percentage;
                    ruleModel.MoreOrLess = rule.moreOrLess;
                    ruleModel.ThanPriceTypeID = rule.thanPriceTypeID;
                    ruleModel.ThanPriceType = rule.thanPriceTypeID != null ? rule.tblPriceTypes1.priceType : "";
                    ruleModel.Level = rule.genericUnitID != null ? "Unit" : rule.serviceID != null ? "Service" : rule.providerID != null ? "Provider" : "Terminal";
                    ruleModel.SavedOn = rule.dateSaved;
                    ruleModel.SavedBy = rule.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + rule.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName;
                    ruleModel.PriceTypeOrder = rule.tblPriceTypes.order_;
                    ruleModel.ToDate = rule.toDate;
                    ruleModel.IsPermanent = rule.permanent_;
                    childrenRules.Add(ruleModel);
                }
                return childrenRules;
            }

            public static List<SelectListItem> FillDrpItemTypes()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                var query = db.tblItemTypes;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.itemTypeID.ToString(),
                        Text = i.itemType
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpJobPositionsPerTerminalCommissions(long terminalID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                if (terminalID != 0)
                {
                    var query = (from jp in db.tblJobPositions
                                 join commission in db.tblCommissions on jp.jobPositionID equals commission.jobPositionID
                                 where commission.terminalID == terminalID
                                 select new
                                 {
                                     jp.jobPositionID,
                                     jp.jobPosition
                                 }).Distinct();

                    foreach (var i in query)
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.jobPositionID.ToString(),
                            Text = i.jobPosition
                        });
                    }
                }
                return list;
            }
        }

        private string GetCategoriesPerService(Int64 serviceID)
        {
            var categories = from a in db.tblCategories_Services
                             where a.serviceID == serviceID
                             select new
                             {
                                 catalog = a.tblCategories.tblCatalogs.catalog,
                                 parentCategory = a.tblCategories.tblCategories2.category != null ? a.tblCategories.tblCategories2.category + " > " : "",
                                 category = a.tblCategories.category
                             };

            var listCategories = "";
            var flag = 1;
            foreach (var i in categories)
            {
                listCategories += "<span class=\"block\" title=\"" + i.catalog + " > " + (i.parentCategory + " " + i.category) + "\">" + i.category + "</span>";
            }
            return listCategories;
        }

        public string RemoveTimeFromDate(DateTime dateString)
        {
            var year = dateString.Year.ToString();
            var month = dateString.Month.ToString().Length < 2 ? "0" + dateString.Month.ToString() : dateString.Month.ToString();
            var day = dateString.Day.ToString().Length < 2 ? "0" + dateString.Day.ToString() : dateString.Day.ToString();
            return year + "-" + month + "-" + day;
        }

        private bool HasAccountingAccounts(long serviceID)
        {
            if (db.tblServices_AccountingAccounts.Where(m => m.serviceID == serviceID).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetAccountingAccountsOfService(long serviceID)
        {
            ePlatEntities db = new ePlatEntities();
            var query = db.tblServices_AccountingAccounts.Where(m => m.serviceID == serviceID).Select(m => new { m.tblAccountingAccounts.account, m.tblAccountingAccounts.accountName, m.tblAccountingAccounts.accountType }).Distinct();
            var _str = query.Count().ToString();

            return _str;
        }

        public List<ActivitySearchResultsModel> SearchActivities(ActivitiesSearchModel model)
        {
            List<ActivitySearchResultsModel> list = new List<ActivitySearchResultsModel>();
            long[] currentTerminals = (from m in model.Search_Terminals.Split(',').Select(m => long.Parse(m)) select m).ToArray();

            if (model.Search_Category != 0)
            {
                var query = from a in db.tblCategories_Services
                            where currentTerminals.Contains(a.tblServices.originalTerminalID)
                            && (a.tblServices.service.Contains(model.Search_Activity) || model.Search_Activity == null)
                            && (model.Search_Provider == a.tblServices.providerID || model.Search_Provider == 0)
                            && a.categoryID == model.Search_Category
                            && (a.serviceID == model.Search_ActivityID || model.Search_ActivityID == null)
                            && a.tblServices.tblProviders.isActive
                            select a;

                if (model.Search_Destination != null)
                {
                    var _places = db.tblPlaces.Where(m => model.Search_Destination.Contains(m.destinationID)).Select(m => (long?)m.placeID).ToArray();
                    query = query.Where(m => db.tblMeetingPoints.Where(a => a.serviceID == m.serviceID && _places.Contains(a.placeID)).Count() > 0);
                }
                ReportDataModel rdm = new ReportDataModel();
                foreach (var i in query)
                {
                    var categories = GetCategoriesPerService(i.serviceID);
                    int images = db.tblPictures_SysItemTypes.Count(x => x.itemID == i.serviceID && x.sysItemTypeID == 1);
                    int seo = db.tblSeoItems.Count(x => x.itemID == i.serviceID && x.sysItemTypeID == 1);

                    List<PriceRuleModel> serviceRules = PriceDataModel.GetRules(i.serviceID, i.tblServices.originalTerminalID, DateTime.Now);
                    List<PriceType> priceTypes = rdm.GetListOfPriceTypes(i.tblServices.originalTerminalID, true, i.serviceID);
                    string rulesErrors = "";
                    bool validBase = true;
                    if (serviceRules.Count(x => x.IsBasePrice) > 1)
                    {
                        validBase = false;
                        rulesErrors = "There is more than 1 base price rule.";
                    }
                    else if (serviceRules.Count(x => x.IsBasePrice) < 1)
                    {
                        validBase = false;
                        rulesErrors = "1 base rule is needed at least.";
                    }
                    bool validPriceTypeRules = true;
                    foreach (var pt in priceTypes)
                    {
                        if (serviceRules.Count(x => x.PriceTypeID == pt.PriceTypeID && x.GenericUnitID == null) > 1)
                        {
                            validPriceTypeRules = false;
                            rulesErrors += (rulesErrors != "" ? ", " : "") + "There are duplicated rules for " + serviceRules.FirstOrDefault(x => x.PriceTypeID == pt.PriceTypeID).RuleFor;
                        }
                    }
                    list.Add(new ActivitySearchResultsModel()
                    {
                        ActivityID = int.Parse(i.tblServices.serviceID.ToString()),
                        Provider = i.tblServices.tblProviders.comercialName,
                        Activity = i.tblServices.service,
                        Terminal = i.tblServices.tblTerminals.terminal,
                        Categories = categories,
                        AccountingAccounts = GetAccountingAccountsOfService(i.tblServices.serviceID),
                        Published = (
                        i.tblServices.tblServiceDescriptions.Count(x => x.active) > 0
                        && i.tblServices.tblProviders.isActive
                        && i.tblServices.tblCategories_Services.Count(x => x.tblCategories.active && x.tblCategories.showOnWebsite) > 0
                        ? true : false),
                        NDescriptions = i.tblServices.tblServiceDescriptions.Count(),
                        NCategories = i.tblServices.tblCategories_Services.Count(),
                        NPublicCategories = i.tblServices.tblCategories_Services.Count(x => x.tblCategories.showOnWebsite == true),
                        NImages = images,
                        NSeoItems = seo,
                        NSchedules = i.tblServices.tblWeeklyAvailability.Count(x => x.permanent_ || x.toDate >= DateTime.Today),
                        NMeetingPoints = i.tblServices.tblMeetingPoints.Select(x => x.placeID).Distinct().Count(),
                        Rules = validBase && validPriceTypeRules,
                        RulesErrors = rulesErrors,
                        Deleted = i.tblServices.deleted,
                        DeletedByUser = i.tblServices.deletedByUserID != null ? i.tblServices.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + i.tblServices.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName : "",
                        DateDeleted = i.tblServices.dateDeleted != null ? i.tblServices.dateDeleted.Value.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) : ""
                    });
                }
            }
            else
            {
                var query = from a in db.tblServices
                            where currentTerminals.Contains(a.originalTerminalID)
                            && (a.service.Contains(model.Search_Activity) || model.Search_Activity == null)
                            && (model.Search_Provider == a.providerID || model.Search_Provider == 0)
                            && (a.serviceID == model.Search_ActivityID || model.Search_ActivityID == null)
                            && a.tblProviders.isActive
                            select a;

                if (model.Search_Destination != null)
                {
                    var _places = db.tblPlaces.Where(m => model.Search_Destination.Contains(m.destinationID)).Select(m => (long?)m.placeID).ToArray();
                    query = query.Where(m => db.tblMeetingPoints.Where(a => a.serviceID == m.serviceID && _places.Contains(a.placeID)).Count() > 0);
                }
                ReportDataModel rdm = new ReportDataModel();
                foreach (var i in query)
                {
                    var categories = GetCategoriesPerService(i.serviceID);
                    int images = db.tblPictures_SysItemTypes.Count(x => x.itemID == i.serviceID && x.sysItemTypeID == 1);
                    int seo = db.tblSeoItems.Count(x => x.itemID == i.serviceID && x.sysItemTypeID == 1);
                    List<PriceRuleModel> serviceRules = PriceDataModel.GetRules(i.serviceID, i.originalTerminalID, DateTime.Now);
                    List<PriceType> priceTypes = rdm.GetListOfPriceTypes(i.originalTerminalID, true, i.serviceID);
                    string rulesErrors = "";
                    bool validBase = true;
                    if (serviceRules.Count(x => x.IsBasePrice) > 1)
                    {
                        validBase = false;
                        rulesErrors = "There is more than 1 base price rule.";
                    }
                    else if (serviceRules.Count(x => x.IsBasePrice) < 1)
                    {
                        validBase = false;
                        rulesErrors = "1 base rule is needed at least.";
                    }
                    bool validPriceTypeRules = true;
                    foreach (var pt in priceTypes)
                    {
                        if (serviceRules.Count(x => x.PriceTypeID == pt.PriceTypeID && x.GenericUnitID == null) > 1)
                        {
                            validPriceTypeRules = false;
                            rulesErrors += (rulesErrors != "" ? ", " : "") + "There are duplicated rules for " + serviceRules.FirstOrDefault(x => x.PriceTypeID == pt.PriceTypeID).RuleFor;
                        }
                    }
                    list.Add(new ActivitySearchResultsModel()
                    {
                        ActivityID = int.Parse(i.serviceID.ToString()),
                        Provider = i.tblProviders.comercialName,
                        Activity = i.service,
                        Terminal = i.tblTerminals.terminal,
                        Categories = categories,
                        AccountingAccounts = GetAccountingAccountsOfService(i.serviceID),
                        Published = (
                        i.tblServiceDescriptions.Count(x => x.active) > 0
                        && i.tblProviders.isActive
                        && i.tblCategories_Services.Count(x => x.tblCategories.active && x.tblCategories.showOnWebsite) > 0
                        ? true : false),
                        NDescriptions = i.tblServiceDescriptions.Count(),
                        NCategories = i.tblCategories_Services.Count(),
                        NPublicCategories = i.tblCategories_Services.Count(x => x.tblCategories.showOnWebsite == true),
                        NImages = images,
                        NSeoItems = seo,
                        NSchedules = i.tblWeeklyAvailability.Count(x => x.permanent_ || x.toDate >= DateTime.Today),
                        NMeetingPoints = i.tblMeetingPoints.Select(x => x.placeID).Distinct().Count(),
                        Rules = validBase && validPriceTypeRules,
                        RulesErrors = rulesErrors,
                        Deleted = i.deleted,
                        DeletedByUser = i.deletedByUserID != null ? i.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + i.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName : "",
                        DateDeleted = i.dateDeleted != null ? i.dateDeleted.Value.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) : ""
                    });
                }
            }
            return list;
        }

        public ActivityInfoModel GetActivityInfo(int activityID)
        {
            ActivityInfoModel model = new ActivityInfoModel();
            List<KeyValuePair<int, string>> listCategories = new List<KeyValuePair<int, string>>();
            try
            {
                var _now = DateTime.Now;
                var query = db.tblServices.Single(m => m.serviceID == activityID);
                var queryVideo = db.tblVideos_SysItemTypes.Where(m => m.sysItemTypeID == 1 && m.itemID == activityID);
                var queryJPExclusions = db.tblCommissionExclusions.Where(m => m.serviceID == query.serviceID && m.fromDate <= _now && (m.permanent_ || m.toDate >= _now));
                var queryCategories = from c in db.tblCategories_Services
                                      where c.serviceID == activityID
                                      select new
                                      {
                                          categoryID = c.categoryID,
                                          parentCategory = c.tblCategories.tblCategories2.category,
                                          category = c.tblCategories.category
                                      };

                foreach (var i in queryCategories)
                {
                    listCategories.Add(new KeyValuePair<int, string>(int.Parse(i.categoryID.ToString()), i.parentCategory + " > " + i.category));
                }

                model.ActivityInfo_ActivityID = int.Parse(query.serviceID.ToString());
                model.ActivityInfo_ItemType = (int)query.itemTypeID;
                model.ActivityInfo_Activity = query.service;
                model.ActivityInfo_OriginalTerminal = int.Parse(query.originalTerminalID.ToString());
                model.ActivityInfo_Destination = int.Parse(query.destinationID.ToString());
                model.ActivityInfo_ApplyWholeStay = query.applyWholeStay;
                model.ActivityInfo_Provider = query.providerID;
                model.ActivityInfo_Length = query.length;
                model.ActivityInfo_Zone = query.zoneID != null ? int.Parse(query.zoneID.ToString()) : 0;
                model.ActivityInfo_TransportationService = query.transportationService;
                model.ActivityInfo_OffersRoundTrip = query.offersRoundTrip != null ? query.offersRoundTrip : false;
                model.ActivityInfo_MinimumAge = query.minimumAge != null ? query.minimumAge.ToString() : "";
                model.ActivityInfo_MinimumHeight = query.minimumHeight != null ? query.minimumHeight.ToString() : "";
                model.ActivityInfo_MaximumWeight = query.maximumWeight != null ? query.maximumWeight.ToString() : "";
                model.ActivityInfo_BabiesAllowed = query.babiesAllowed;
                model.ActivityInfo_ChildrenAllowed = query.childrenAllowed;
                model.ActivityInfo_AdultsAllowed = query.adultsAllowed;
                model.ActivityInfo_PregnantsAllowed = query.pregnantsAllowed;
                model.ActivityInfo_OldiesAllowed = query.oldiesAllowed;
                model.ActivityInfo_ExcludeForCommission = query.excludeForCommission ?? false;
                model.ActivityInfo_AvoidRounding = query.avoidRounding ?? false;
                model.ActivityInfo_ListCategories = listCategories;
                model.ActivityInfo_JobPositions = queryJPExclusions.Select(m => m.jobPositionID).ToArray();
                model.ActivityInfo_HasSpecialExchangeRate = query.tblProviders.contractCurrencyID == null || db.tblExchangeRates.Where(m => m.providerID == query.providerID && m.fromDate <= _now && (_now <= m.toDate || m.permanent_) && (m.exchangeRateTypeID == 2 || m.exchangeRateTypeID == 3)).Count() > 0;
                if (queryVideo.Count() > 0)
                {
                    model.ActivityInfo_Video = queryVideo.FirstOrDefault().tblVideos.video;
                    model.ActivityInfo_VideoURL = queryVideo.FirstOrDefault().tblVideos.url;
                }


            }
            catch { }
            return model;
        }

        public List<ActivityDescriptionInfoModel> GetActivityDescriptions(int activityID)
        {
            List<ActivityDescriptionInfoModel> list = new List<ActivityDescriptionInfoModel>();

            try
            {
                var query = from description in db.tblServiceDescriptions
                            where description.serviceID == activityID
                            select new
                            {
                                descriptionID = description.serviceDescriptionID,
                                activityID = description.serviceID,
                                activity = description.service,
                                shortDescription = description.shortDescription,
                                fullDescription = description.fullDescription,
                                itinerary = description.itinerary,
                                includes = description.includes,
                                notes = description.notes,
                                recommendations = description.recommendations,
                                policies = description.policies,
                                cancelationPolicies = description.cancelationPolicies,
                                culture = description.culture,
                                isActive = description.active
                            };
                foreach (var i in query)
                {
                    list.Add(new ActivityDescriptionInfoModel()
                    {
                        ActivityDescriptionInfo_ActivityDescriptionID = int.Parse(i.descriptionID.ToString()),
                        ActivityDescriptionInfo_ActivityID = int.Parse(i.activityID.ToString()),
                        ActivityDescriptionInfo_Activity = i.activity,
                        ActivityDescriptionInfo_ShortDescription = i.shortDescription,
                        ActivityDescriptionInfo_FullDescription = i.fullDescription,
                        ActivityDescriptionInfo_Itinerary = i.itinerary,
                        ActivityDescriptionInfo_Includes = i.includes,
                        ActivityDescriptionInfo_Notes = i.notes,
                        ActivityDescriptionInfo_Recommendations = i.recommendations,
                        ActivityDescriptionInfo_Policies = i.policies,
                        ActivityDescriptionInfo_CancelationPolicies = i.cancelationPolicies,
                        ActivityDescriptionInfo_Culture = i.culture,
                        ActivityDescriptionInfo_IsActive = i.isActive
                    });
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public List<ActivityScheduleInfoModel> GetActivitySchedules(int activityID)
        {
            List<ActivityScheduleInfoModel> list = new List<ActivityScheduleInfoModel>();
            try
            {
                var query = from schedule in db.tblWeeklyAvailability
                            where schedule.serviceID == activityID
                            select new
                            {
                                activityScheduleID = schedule.weeklyAvailabilityID,
                                activityID = schedule.serviceID,
                                monday = schedule.monday,
                                tuesday = schedule.tuesday,
                                wednesday = schedule.wednesday,
                                thursday = schedule.thursday,
                                friday = schedule.friday,
                                saturday = schedule.saturday,
                                sunday = schedule.sunday,
                                fromTime = schedule.hour,
                                range = schedule.range,
                                toTime = schedule.toHour
                            };
                foreach (var i in query)
                {
                    list.Add(new ActivityScheduleInfoModel()
                    {
                        ActivityScheduleInfo_ActivityScheduleID = int.Parse(i.activityScheduleID.ToString()),
                        ActivityScheduleInfo_ActivityID = int.Parse(i.activityID.ToString()),
                        ActivityScheduleInfo_Monday = i.monday,
                        ActivityScheduleInfo_Tuesday = i.tuesday,
                        ActivityScheduleInfo_Wednesday = i.wednesday,
                        ActivityScheduleInfo_Thursday = i.thursday,
                        ActivityScheduleInfo_Friday = i.friday,
                        ActivityScheduleInfo_Saturday = i.saturday,
                        ActivityScheduleInfo_Sunday = i.sunday,
                        ActivityScheduleInfo_FromTime = i.fromTime.ToString(),
                        ActivityScheduleInfo_IsSpecificTime = i.range ? false : true,
                        ActivityScheduleInfo_ToTime = i.toTime.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public List<ActivityMeetingPointInfoModel> GetActivityMeetingPoints(int activityID)
        {
            List<ActivityMeetingPointInfoModel> list = new List<ActivityMeetingPointInfoModel>();
            try
            {
                var query = from meetingPoint in db.tblMeetingPoints
                            join place in db.tblPlaces on meetingPoint.placeID equals place.placeID into mp_place
                            from place in mp_place.DefaultIfEmpty()
                            where meetingPoint.serviceID == activityID
                            select new
                            {
                                activityMeetingPointID = meetingPoint.meetingPointID,
                                activityID = meetingPoint.serviceID,
                                departureTime = meetingPoint.hour,
                                destination = meetingPoint.placeID != null ? place.tblDestinations.destination : "",
                                placeString = meetingPoint.placeID != null ? place.place : "At Your Hotel",
                                place = meetingPoint.placeID,
                                notes = meetingPoint.note,
                                atYourHotel = meetingPoint.atYourHotel,
                                active = meetingPoint.active
                            };
                foreach (var i in query)
                {
                    list.Add(new ActivityMeetingPointInfoModel()
                    {
                        ActivityMeetingPointInfo_ActivityMeetingPointID = int.Parse(i.activityMeetingPointID.ToString()),
                        ActivityMeetingPointInfo_ActivityID = int.Parse(i.activityID.ToString()),
                        ActivityMeetingPointInfo_DepartureTime = i.departureTime.ToString(),
                        //ActivityMeetingPointInfo_Destination = i.destination,
                        ActivityMeetingPointInfo_PlaceString = i.placeString + " " + i.destination,
                        ActivityMeetingPointInfo_Place = i.place != null ? int.Parse(i.place.ToString()) : 0,
                        ActivityMeetingPointInfo_Notes = i.notes != null ? i.notes : "",
                        ActivityMeetingPointInfo_AtYourHotel = i.atYourHotel,
                        ActivityMeetingPointInfo_IsActive = i.active,
                        ActivityMeetingPointInfo_DrpPlaces = new List<SelectListItem>()
                    });
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public List<ActivityAccountingAccountsModel> GetAccountingAccounts()
        {
            CatalogsDataModel.AccountingAccounts aam = new CatalogsDataModel.AccountingAccounts();
            var list = new List<ActivityAccountingAccountsModel>();
            try
            {
                foreach (var i in aam.SearchAccountingAccounts(new AccountingAccountsModel.SearchAccountingAccountsModel()))
                {
                    list.Add(new ActivityAccountingAccountsModel()
                    {
                        AccountingAccountInfo_AccountingAccountID = i.AccountingAccountInfo_AccountingAccountID,
                        AccountingAccountInfo_Account = i.AccountingAccountInfo_Account,
                        AccountingAccountInfo_AccountName = i.AccountingAccountInfo_AccountName,
                        AccountingAccountInfo_Company = i.AccountingAccountInfo_Company,
                        AccountingAccountInfo_CompanyText = i.AccountingAccountInfo_CompanyText,
                        AccountingAccountInfo_PriceType = i.AccountingAccountInfo_PriceType,
                        AccountingAccountInfo_PriceTypeText = i.AccountingAccountInfo_PriceTypeText
                    });
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public List<ActivityPointsOfSaleModel> GetPointsOfSale()
        {
            CatalogsDataModel.PointsOfSale psm = new CatalogsDataModel.PointsOfSale();
            var list = new List<ActivityPointsOfSaleModel>();
            try
            {
                foreach (var i in psm.SearchPointsOfSale(new PointsOfSaleModel.SearchPointsOfSaleModel()))
                {
                    list.Add(new ActivityPointsOfSaleModel()
                    {
                        PointsOfSaleInfo_PointOfSaleID = i.PointsOfSaleInfo_PointOfSaleID,
                        PointsOfSaleInfo_PointOfSale = i.PointsOfSaleInfo_PointOfSale,
                        PointsOfSaleInfo_ShortName = i.PointsOfSaleInfo_ShortName,
                        PointsOfSaleInfo_Place = i.PointsOfSaleInfo_Place,
                        PointsOfSaleInfo_PlaceText = i.PointsOfSaleInfo_PlaceText
                    });
                }
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public List<ActivityAccountingAccountInfoModel> GetActivityAccountingAccounts(int activityID)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<ActivityAccountingAccountInfoModel>();

            try
            {
                var query = from aa in db.tblServices_AccountingAccounts
                            where aa.serviceID == activityID
                            select new
                            {
                                aa.accountingAccountID,
                                aa.tblAccountingAccounts.account,
                                aa.tblAccountingAccounts.accountName,
                                aa.tblAccountingAccounts.tblPriceTypes.priceType,
                                aa.tblAccountingAccounts.accountType,
                                aa.tblPointsOfSale.pointOfSale
                            };
                foreach (var i in query)
                {
                    if (list.Count(m => m.ActivityAccountingAccountInfo_AccountingAccount[0] == i.accountingAccountID) > 0)
                    {
                        list.FirstOrDefault(m => m.ActivityAccountingAccountInfo_AccountingAccount[0] == i.accountingAccountID).ActivityAccountingAccountInfo_PointOfSaleString += "," + i.pointOfSale;
                    }
                    else
                    {
                        list.Add(new ActivityAccountingAccountInfoModel()
                        {
                            ActivityAccountingAccountInfo_AccountingAccount = new int[] { i.accountingAccountID },
                            ActivityAccountingAccountInfo_AccountingAccountString = i.account,
                            ActivityAccountingAccountInfo_AccountingAccountName = i.accountName,
                            ActivityAccountingAccountInfo_PointOfSaleString = i.pointOfSale,
                            ActivityAccountingAccountInfo_PriceType = i.priceType,
                            ActivityAccountingAccountInfo_AccountType = (i.accountType ? "Income" : "Outcome")
                        });
                    }
                }
                list = list.OrderBy(x => x.ActivityAccountingAccountInfo_AccountType).ThenBy(x => x.ActivityAccountingAccountInfo_PriceType).ToList();
            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public ActivityDescriptionInfoModel GetActivityDescription(int activityDescriptionID)
        {
            ActivityDescriptionInfoModel model = new ActivityDescriptionInfoModel();
            try
            {
                var query = db.tblServiceDescriptions.Single(m => m.serviceDescriptionID == activityDescriptionID);
                model.ActivityDescriptionInfo_ActivityDescriptionID = int.Parse(query.serviceDescriptionID.ToString());
                model.ActivityDescriptionInfo_ActivityID = int.Parse(query.serviceID.ToString());
                model.ActivityDescriptionInfo_Activity = query.service;
                model.ActivityDescriptionInfo_ShortDescription = query.shortDescription;
                model.ActivityDescriptionInfo_FullDescription = query.fullDescription;
                model.ActivityDescriptionInfo_Itinerary = query.itinerary;
                model.ActivityDescriptionInfo_Includes = query.includes;
                model.ActivityDescriptionInfo_Notes = query.notes;
                model.ActivityDescriptionInfo_Recommendations = query.recommendations;
                model.ActivityDescriptionInfo_Policies = query.policies;
                model.ActivityDescriptionInfo_CancelationPolicies = query.cancelationPolicies;
                model.ActivityDescriptionInfo_Culture = query.culture;
                model.ActivityDescriptionInfo_IsActive = query.active;
                model.ActivityDescriptionInfo_Tag = query.tag;
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public ActivityScheduleInfoModel GetActivitySchedule(int activityScheduleID)
        {
            ActivityScheduleInfoModel model = new ActivityScheduleInfoModel();
            try
            {
                var query = db.tblWeeklyAvailability.Single(m => m.weeklyAvailabilityID == activityScheduleID);
                model.ActivityScheduleInfo_ActivityScheduleID = int.Parse(query.weeklyAvailabilityID.ToString());
                model.ActivityScheduleInfo_ActivityID = int.Parse(query.serviceID.ToString());
                model.ActivityScheduleInfo_Monday = query.monday;
                model.ActivityScheduleInfo_Tuesday = query.tuesday;
                model.ActivityScheduleInfo_Wednesday = query.wednesday;
                model.ActivityScheduleInfo_Thursday = query.thursday;
                model.ActivityScheduleInfo_Friday = query.friday;
                model.ActivityScheduleInfo_Saturday = query.saturday;
                model.ActivityScheduleInfo_Sunday = query.sunday;
                model.ActivityScheduleInfo_IsPermanent = query.permanent_;
                model.ActivityScheduleInfo_FromDate = RemoveTimeFromDate(query.fromDate.Value);
                var toDate = query.toDate != null ? RemoveTimeFromDate(query.toDate.Value) : "";
                model.ActivityScheduleInfo_ToDate = toDate;
                model.ActivityScheduleInfo_IsSpecificTime = query.range ? false : true;
                model.ActivityScheduleInfo_FromTime = query.hour.ToString();
                var toTime = query.toHour != null ? query.toHour.ToString() : "";
                model.ActivityScheduleInfo_ToTime = toTime;
                model.ActivityScheduleInfo_IntervalTime = query.everyXMinutes;
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public ActivityMeetingPointInfoModel GetActivityMeetingPoint(int activityMeetingPointID)
        {
            ActivityMeetingPointInfoModel model = new ActivityMeetingPointInfoModel();
            try
            {
                var query = db.tblMeetingPoints.Single(m => m.meetingPointID == activityMeetingPointID);
                model.ActivityMeetingPointInfo_ActivityMeetingPointID = query.meetingPointID;
                model.ActivityMeetingPointInfo_ActivityID = query.serviceID;
                model.ActivityMeetingPointInfo_WeeklySchedule = query.weeklyAvailabilityID;
                model.ActivityMeetingPointInfo_DepartureTime = query.hour.ToString();
                model.ActivityMeetingPointInfo_PlaceString = query.placeID != null ? query.tblPlaces.place + " " + query.tblPlaces.tblDestinations.destination : "";
                model.ActivityMeetingPointInfo_Place = query.placeID != null ? int.Parse(query.placeID.ToString()) : 0;
                model.ActivityMeetingPointInfo_Notes = query.note;
                model.ActivityMeetingPointInfo_AtYourHotel = query.atYourHotel;
                model.ActivityMeetingPointInfo_IsActive = query.active;
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public ActivityAccountingAccountInfoModel GetActivityAccountingAccount(int accountingAccountID, long serviceID)
        {
            ePlatEntities db = new ePlatEntities();
            ActivityAccountingAccountInfoModel model = new ActivityAccountingAccountInfoModel();
            try
            {
                var query = db.tblServices_AccountingAccounts.Where(m => m.accountingAccountID == accountingAccountID && m.serviceID == serviceID);
                model.ActivityAccountingAccountInfo_ActivityAccountingAccountID = query.First().service_accountID;
                model.ActivityAccountingAccountInfo_ActivityID = query.First().serviceID;
                model.ActivityAccountingAccountInfo_AccountingAccount = new int[] { query.First().accountingAccountID };
                model.ActivityAccountingAccountInfo_AccountingAccountString = query.First().tblAccountingAccounts.account + "-" + query.First().tblAccountingAccounts.accountName;
                model.ActivityAccountingAccountInfo_PointOfSale = query.First().pointOfSaleID != null ? Array.ConvertAll(query.Select(m => m.pointOfSaleID).ToArray(), m => m.ToString()) : null;// list.ToArray();
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public ActivityAccountingAccountsModel GetAccountingAccount(int accountingAccountID)
        {
            ePlatEntities db = new ePlatEntities();
            var model = new ActivityAccountingAccountsModel();
            try
            {
                var query = db.tblAccountingAccounts.Single(m => m.accountingAccountID == accountingAccountID);
                model.AccountingAccountInfo_AccountingAccountID = query.accountingAccountID;
                model.AccountingAccountInfo_Account = query.account;
                model.AccountingAccountInfo_AccountName = query.accountName;
                model.AccountingAccountInfo_Company = query.companyID;
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public ActivityPointsOfSaleModel GetPointOfSale(int pointOfSaleID)
        {
            ePlatEntities db = new ePlatEntities();
            var model = new ActivityPointsOfSaleModel();

            try
            {
                var query = db.tblPointsOfSale.Single(m => m.pointOfSaleID == pointOfSaleID);
                model.PointsOfSaleInfo_PointOfSaleID = query.pointOfSaleID;
                model.PointsOfSaleInfo_PointOfSale = query.pointOfSale;
                model.PointsOfSaleInfo_ShortName = query.shortName;
                model.PointsOfSaleInfo_Place = query.placeID;
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public PriceRuleModel GetPriceTypeRule(long ruleID)
        {
            PriceRuleModel model = new PriceRuleModel();
            try
            {
                var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == ruleID);
                model.Service_PriceTypeID = query.service_priceTypeID;
                model.TerminalID = query.terminalID;
                model.ProviderID = query.providerID;
                model.ServiceID = query.serviceID;
                model.GenericUnitID = query.genericUnitID;
                model.PriceTypeID = query.priceTypeID;
                model.FromDate = (DateTime)query.fromDate;
                model.IsBasePrice = query.@base;
                model.Percentage = query.percentage;
                model.MoreOrLess = query.moreOrLess;
                model.Formula = query.formula;
                model.ThanPriceTypeID = query.thanPriceTypeID;
            }
            catch
            {
            }
            return model;
        }

        public AttemptResponse ClosePriceTypeRule(int priceTypeRuleID, DateTime? toDate)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == priceTypeRuleID);
                if (query.providerID == null && query.serviceID == null)
                {
                    throw new Exception("Terminal Rules Cannot Be Closed");
                }
                query.toDate = toDate != null ? (toDate.Value.Date == DateTime.Today.Date ? DateTime.Now : toDate.Value.AddDays(1).AddSeconds(-1)) : DateTime.Now;
                query.permanent_ = false;
                query.modifiedByUserID = session.UserID;
                query.dateLastModification = DateTime.Now;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Rule Closed";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Rule NOT Closed";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SavePriceTypeRule(PriceTypeRulesInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            var _now = DateTime.Now;

            if (model.PriceTypeRulesInfo_PriceTypeRuleID != null && model.PriceTypeRulesInfo_PriceTypeRuleID != 0)
            {
                #region "Rule edition"
                try
                {
                    var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == model.PriceTypeRulesInfo_PriceTypeRuleID);

                    query.terminalID = model.PriceTypeRulesInfo_Terminal;
                    query.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                    query.serviceID = model.PriceTypeRulesInfo_Service != null ? long.Parse(model.PriceTypeRulesInfo_Service[0]) : (long?)null;
                    query.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                    query.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                    query.@base = model.PriceTypeRulesInfo_Base;
                    if (model.PriceTypeRulesInfo_Base)
                    {
                        query.percentage = (decimal?)null;
                        query.moreOrLess = (bool?)null;
                        query.thanPriceTypeID = (int?)null;
                    }
                    else
                    {
                        if (model.PriceTypeRulesInfo_UsesFormula)
                        {
                            query.formula = model.PriceTypeRulesInfo_Formula;
                            query.percentage = (decimal?)null;
                            query.moreOrLess = (bool?)null;
                            query.thanPriceTypeID = (int?)null;
                        }
                        else
                        {
                            query.formula = null;
                            query.percentage = model.PriceTypeRulesInfo_Percentage;
                            query.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                            query.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                        }
                    }
                    query.fromDate = model.PriceTypeRulesInfo_FromDate != null && model.PriceTypeRulesInfo_FromDate != "" ? DateTime.Parse(model.PriceTypeRulesInfo_FromDate) : query.fromDate;
                    query.dateLastModification = DateTime.Now;
                    query.modifiedByUserID = session.UserID;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Rule Updated";
                    response.ObjectID = "";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Rule NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else if (model.PriceTypeRulesInfo_PriceTypeRules != null && model.PriceTypeRulesInfo_PriceTypeRules != "")
            {
                #region "Close children rules (for provider)"
                Dictionary<string, string> childrenRules = new JavaScriptSerializer().Deserialize(model.PriceTypeRulesInfo_PriceTypeRules, typeof(Dictionary<string, string>)) as Dictionary<string, string>;

                var date = DateTime.Parse(childrenRules.Where(m => m.Key == "date").Select(m => m.Value).FirstOrDefault());
                var _childrenRules = childrenRules.Where(m => m.Key == "children").Select(m => m.Value).FirstOrDefault() != "" ? childrenRules.Where(m => m.Key == "children").Select(m => m.Value).FirstOrDefault().Split(',').Select(m => long.Parse(m)).ToArray() : new long[] { };
                var query = db.tblServices_PriceTypes.Where(m => _childrenRules.Contains(m.service_priceTypeID));

                foreach (var item in query)
                {
                    item.toDate = date.AddSeconds(-1);
                    item.permanent_ = false;
                    item.dateLastModification = DateTime.Now;
                    item.modifiedByUserID = session.UserID;
                }

                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Rules Correctly Closed and Saved";
                response.ObjectID = 0;
                return response;
                #endregion
            }
            else
            {
                #region "Rule creation"
                if (model.PriceTypeRulesInfo_Service != null)
                {
                    #region "Service Level"
                    var childrenRules = new List<PriceRuleModel>();
                    foreach (var _service in model.PriceTypeRulesInfo_Service)
                    {
                        var hasBaseRule = model.PriceTypeRulesInfo_Base ? ActivitiesCatalogs.HasBaseRule(model.PriceTypeRulesInfo_SelectedService, _service, model.PriceTypeRulesInfo_Terminal) : false;

                        if (hasBaseRule)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = "Cannot save two base rules within same level";
                            response.ObjectID = 0;
                            return response;
                        }
                        else
                        {
                            _now = (model.PriceTypeRulesInfo_FromDate != null && model.PriceTypeRulesInfo_FromDate != "") ? DateTime.Parse(model.PriceTypeRulesInfo_FromDate).Date == DateTime.Today.Date ? _now : DateTime.Parse(model.PriceTypeRulesInfo_FromDate) : _now;
                            var existingRule = ActivitiesCatalogs.RuleExists(ref db, model, long.Parse(_service), _now);
                            var relatedRules = ActivitiesCatalogs.GetRelatedRules(model, long.Parse(_service), _now);
                            var servicesSold = ActivitiesCatalogs.ServicesSoldAfterDate(long.Parse(_service), relatedRules, _now);
                            childrenRules = childrenRules.Concat(ActivitiesCatalogs.GetChildrenRules(int.Parse(model.PriceTypeRulesInfo_Provider), long.Parse(_service), model.PriceTypeRulesInfo_GenericUnit, _now, relatedRules)).ToList<PriceRuleModel>();

                            if (servicesSold != "")
                            {
                                response.Type = Attempt_ResponseTypes.Error;
                                response.Message = "Price Type Rule(s) NOT saved because another rule exists in the same date range and it has being used.";
                                response.Message += "<br />Rules being used:<br />" + servicesSold.Replace(",", "<br />");
                                response.ObjectID = 0;
                                return response;
                            }

                            #region "new rule"
                            var rule = new tblServices_PriceTypes();
                            rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                            rule.providerID = model.PriceTypeRulesInfo_Provider != "null" ? int.Parse(model.PriceTypeRulesInfo_Provider) : (int?)null;
                            rule.serviceID = long.Parse(_service);
                            rule.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                            rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                            rule.@base = model.PriceTypeRulesInfo_Base;
                            if (model.PriceTypeRulesInfo_Base)
                            {
                                rule.percentage = (decimal?)null;
                                rule.moreOrLess = (bool?)null;
                                rule.thanPriceTypeID = (int?)null;
                            }
                            else
                            {
                                if (model.PriceTypeRulesInfo_UsesFormula)
                                {
                                    rule.formula = model.PriceTypeRulesInfo_Formula;
                                    rule.percentage = (decimal?)null;
                                    rule.moreOrLess = (bool?)null;
                                    rule.thanPriceTypeID = (int?)null;
                                }
                                else
                                {
                                    rule.formula = null;
                                    rule.percentage = model.PriceTypeRulesInfo_Percentage;
                                    rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                    rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                                }
                            }
                            rule.permanent_ = true;
                            rule.fromDate = _now;
                            rule.dateSaved = DateTime.Now;
                            rule.savedByUserID = session.UserID;
                            #endregion

                            if (existingRule.Count() != 0)
                            {
                                //closing of new rule with existing rule parameters
                                rule.permanent_ = existingRule.FirstOrDefault().permanent_;
                                rule.toDate = existingRule.FirstOrDefault().toDate;
                                //closing of existing rule
                                existingRule.FirstOrDefault().permanent_ = false;
                                existingRule.FirstOrDefault().toDate = _now.AddSeconds(-1);
                                existingRule.FirstOrDefault().dateLastModification = DateTime.Now;
                                existingRule.FirstOrDefault().modifiedByUserID = session.UserID;
                                childrenRules = childrenRules.Where(m => m.Service_PriceTypeID != existingRule.FirstOrDefault().service_priceTypeID).ToList<PriceRuleModel>();
                            }

                            db.tblServices_PriceTypes.AddObject(rule);
                        }
                    }
                    db.SaveChanges();

                    if (childrenRules.Count() > 0)
                    {
                        response.Type = Attempt_ResponseTypes.Warning;
                        response.Message = "Rule Saved.<br />Would you like to close active rules that interfer with this new rule?<br />Active Rules:<br />";
                        response.Message += "<ul id=\"ulRulesToClose\" style=\"max-height:300px; overflow:hidden; overflow-y:scroll;\">";
                        var _children = "";
                        foreach (var child in childrenRules)
                        {
                            response.Message += "<li><input type=\"checkbox\" id=\"" + child.Service_PriceTypeID + "\" checked=\"checked\">" + child.RuleFrom + (child.IsBasePrice ? "" : (": " + child.RuleFor + " " + child.Percentage + " " + ((bool)child.MoreOrLess ? "more " : "less ") + "than " + child.ThanPriceType)) + "</li>";
                            _children += (_children == "" ? "" : ",") + child.Service_PriceTypeID;
                        }
                        response.Message += "</ul>";
                        response.ObjectID = new JavaScriptSerializer().Serialize(new { children = _children, date = _now.ToString() });
                        return response;
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Price Type Rule(s) Saved";
                    response.ObjectID = 0;
                    return response;
                    #endregion
                }
                else
                {
                    #region "Povider Level"
                    var _servicesPerProvider = GetDDLData("servicesPerProvider", model.PriceTypeRulesInfo_Provider).Select(m => Convert.ToInt64(m.Value)).ToList();
                    var hasBaseRule = model.PriceTypeRulesInfo_Base ? ActivitiesCatalogs.HasBaseRule(model.PriceTypeRulesInfo_SelectedService, null, model.PriceTypeRulesInfo_Terminal) : false;

                    if (hasBaseRule)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Cannot save two base rules within same level";
                        response.ObjectID = 0;
                        return response;
                    }
                    else
                    {
                        _now = (model.PriceTypeRulesInfo_FromDate != null && model.PriceTypeRulesInfo_FromDate != "") ? DateTime.Parse(model.PriceTypeRulesInfo_FromDate).Date == DateTime.Today ? _now : DateTime.Parse(model.PriceTypeRulesInfo_FromDate) : _now;
                        var existingRule = ActivitiesCatalogs.RuleExists(ref db, model, (long?)null, _now);
                        var childrenRules = new List<PriceRuleModel>();

                        foreach (var serviceID in _servicesPerProvider)
                        {
                            var relatedRules = ActivitiesCatalogs.GetRelatedRules(model, serviceID, _now);
                            var servicesSold = ActivitiesCatalogs.ServicesSoldAfterDate(serviceID, relatedRules, _now);
                            childrenRules = childrenRules.Concat(ActivitiesCatalogs.GetChildrenRules(int.Parse(model.PriceTypeRulesInfo_Provider), null, model.PriceTypeRulesInfo_GenericUnit, _now, relatedRules)).ToList<PriceRuleModel>();
                            if (servicesSold != "")
                            {
                                response.Type = Attempt_ResponseTypes.Error;
                                response.Message = "Price Type Rule(s) NOT saved because another rule exists in the same date range and it has being used.";
                                response.Message += "<br />Rules being used:<br />" + servicesSold.Replace(",", "<br />");
                                response.ObjectID = 0;
                                return response;
                            }
                        }

                        #region "new rule"
                        var rule = new tblServices_PriceTypes();
                        rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                        rule.providerID = model.PriceTypeRulesInfo_Provider != "null" ? int.Parse(model.PriceTypeRulesInfo_Provider) : (int?)null;
                        rule.serviceID = (long?)null;
                        rule.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                        rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                        rule.@base = model.PriceTypeRulesInfo_Base;
                        if (model.PriceTypeRulesInfo_Base)
                        {
                            rule.percentage = (decimal?)null;
                            rule.moreOrLess = (bool?)null;
                            rule.thanPriceTypeID = (int?)null;
                        }
                        else
                        {
                            if (model.PriceTypeRulesInfo_UsesFormula)
                            {
                                rule.formula = model.PriceTypeRulesInfo_Formula;
                                rule.percentage = (decimal?)null;
                                rule.moreOrLess = (bool?)null;
                                rule.thanPriceTypeID = (int?)null;
                            }
                            else
                            {
                                rule.formula = null;
                                rule.percentage = model.PriceTypeRulesInfo_Percentage;
                                rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                            }
                        }
                        rule.permanent_ = true;
                        rule.fromDate = _now;
                        rule.dateSaved = DateTime.Now;
                        rule.savedByUserID = session.UserID;
                        #endregion

                        if (existingRule.Count() != 0)
                        {
                            //closing of new rule with existing rule parameters
                            rule.permanent_ = existingRule.FirstOrDefault().permanent_;
                            rule.toDate = existingRule.FirstOrDefault().toDate;
                            //closing of existing rule
                            existingRule.FirstOrDefault().permanent_ = false;
                            existingRule.FirstOrDefault().toDate = _now.AddSeconds(-1);
                            existingRule.FirstOrDefault().dateLastModification = DateTime.Now;
                            existingRule.FirstOrDefault().modifiedByUserID = session.UserID;
                            childrenRules = childrenRules.Where(m => m.Service_PriceTypeID != existingRule.FirstOrDefault().service_priceTypeID).ToList<PriceRuleModel>();
                        }

                        db.tblServices_PriceTypes.AddObject(rule);
                        db.SaveChanges();

                        if (childrenRules.Count() > 0)
                        {
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "Rule Saved.<br />Would you like to close active rules that interfer with this new rule?<br />Active Rules:<br />";
                            response.Message += "<ul id=\"ulRulesToClose\" style=\"max-height:300px; overflow:hidden; overflow-y:scroll;\">";
                            var _children = "";
                            foreach (var child in childrenRules)
                            {
                                response.Message += "<li><input type=\"checkbox\" id=\"" + child.Service_PriceTypeID + "\" checked=\"checked\">" + child.RuleFrom + (child.IsBasePrice ? "" : (": " + child.RuleFor + " " + child.Percentage + " " + ((bool)child.MoreOrLess ? "more " : "less ") + "than " + child.ThanPriceType)) + "</li>";
                                _children += (_children == "" ? "" : ",") + child.Service_PriceTypeID;
                            }
                            response.Message += "</ul>";
                            response.ObjectID = new JavaScriptSerializer().Serialize(new { children = _children, date = _now.ToString() });
                            return response;
                        }
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Price Type Rule(s) Saved";
                        response.ObjectID = 0;
                        return response;
                    }
                    #endregion
                }
                #endregion
            }
        }

        public List<StockTransactionsResults> SearchStockTransactions(StockInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<StockTransactionsResults>();

            IQueryable<tblStockTransactions> query;

            if (model.SearchStockTransactions_I_Date != null)
            {
                var iDate = DateTime.Parse(model.SearchStockTransactions_I_Date, CultureInfo.InvariantCulture);
                var fDate = DateTime.Parse(model.SearchStockTransactions_F_Date, CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);

                query = db.tblStockTransactions.Where(m => m.stockID == model.SearchStockTransactions_Stock && m.dateSaved >= iDate && m.dateSaved <= fDate);
                query = query.Where(m => model.SearchStockTransactions_Ingress ? model.SearchStockTransactions_Egress ? m.ingress != null : m.ingress : model.SearchStockTransactions_Egress ? !m.ingress : m.ingress != null);
            }
            else
            {
                query = db.tblStockTransactions.Where(m => m.stockID == model.SearchStockTransactions_Stock).OrderByDescending(m => m.dateSaved).Take(10);
            }
            foreach (var i in query)
            {
                list.Add(new StockTransactionsResults()
                {
                    Ingress = i.ingress ? "Ingress" : "Egress",
                    Quantity = i.quantity.ToString(),
                    DateSaved = i.dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt"),
                    SavedByUser = i.aspnet_Users.tblUserProfiles.FirstOrDefault().firstName + " " + i.aspnet_Users.tblUserProfiles.FirstOrDefault().lastName,
                    Description = i.transactionDescription,
                    PurchaseServiceDetail = i.purchaseServiceDetailID != null ? i.tblPurchaseServiceDetails.coupon : ""
                });
            }

            return list;
        }

        public AttemptResponse SaveStockTransaction(StockInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                if (model.StockTransactionInfo_Stock == 0)
                {
                    tblStocks stock = new tblStocks();
                    stock.serviceID = model.StockTransactionInfo_Service;
                    stock.quantity = model.StockTransactionInfo_Quantity;
                    stock.minimalStock = 0;
                    stock.dateModified = DateTime.Now;
                    stock.modifiedByUserID = session.UserID;
                    db.tblStocks.AddObject(stock);
                    db.SaveChanges();
                    model.StockTransactionInfo_Stock = stock.stockID;
                }
                else
                {
                    var stock = db.tblStocks.Single(m => m.stockID == model.StockTransactionInfo_Stock);
                    stock.quantity = model.StockTransactionInfo_Ingress ? (stock.quantity + model.StockTransactionInfo_Quantity) : (stock.quantity - model.StockTransactionInfo_Quantity);
                }

                tblStockTransactions query = new tblStockTransactions();
                query.stockID = model.StockTransactionInfo_Stock;
                query.purchaseServiceDetailID = model.StockTransactionInfo_PurchaseServiceDetail != 0 ? model.StockTransactionInfo_PurchaseServiceDetail : (long?)null;
                query.quantity = model.StockTransactionInfo_Quantity;
                query.dateSaved = DateTime.Now;
                query.savedByUserID = session.UserID;
                query.ingress = model.StockTransactionInfo_Ingress;
                query.transactionDescription = model.StockTransactionInfo_TransactionDescription;
                db.tblStockTransactions.AddObject(query);

                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Transaction Saved";
                response.ObjectID = new { stock = query.stockID, transaction = query.stockTransactionID };
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Transaction NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public object GetStockBalance(long serviceID)
        {
            ePlatEntities db = new ePlatEntities();
            try
            {
                var query = db.tblStocks.Where(m => m.serviceID == serviceID).FirstOrDefault();
                return new { stockID = query.stockID, quantity = query.quantity, stock = query.minimalStock };
            }
            catch
            {
                return new { stockID = 0, quantity = 0, stock = 0 };
            }

        }

        public AttemptResponse UpdateStock(StockInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var _now = DateTime.Now;
            var _stock = 0;
            try
            {
                var _stocks = db.tblStocks.Where(m => m.serviceID == model.StockInfo_Service);
                if (_stocks.Count() > 0)
                {
                    var query = db.tblStocks.Where(m => m.serviceID == model.StockInfo_Service).FirstOrDefault();
                    query.minimalStock = model.StockInfo_MinimalStock;
                    query.dateModified = _now;
                    query.modifiedByUserID = session.UserID;
                    db.SaveChanges();
                    response.ObjectID = query.stockID;
                }
                else
                {
                    var query = new tblStocks();
                    query.serviceID = model.StockInfo_Service;
                    query.quantity = 0;
                    query.minimalStock = model.StockInfo_MinimalStock;
                    query.dateModified = _now;
                    query.modifiedByUserID = session.UserID;
                    db.tblStocks.AddObject(query);
                    db.SaveChanges();
                    response.ObjectID = query.stockID;
                }
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Stock Updated";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Stock NOT Updated";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse _SavePriceTypeRule(PriceTypeRulesInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var _now = DateTime.Now;
            if (model.PriceTypeRulesInfo_PriceTypeRules != null && model.PriceTypeRulesInfo_PriceTypeRules != "")
            {
                #region "replacing of rules"
                //add services selected in model to pricetyperules
                var _services = model.PriceTypeRulesInfo_Service.Select(m => long.Parse(m)).ToArray();
                foreach (var _rule in model.PriceTypeRulesInfo_PriceTypeRules.Split(',').Select(m => long.Parse(m)).ToArray())
                {
                    var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == _rule);
                    //if (query.dateSaved.Date == DateTime.Today)
                    //{
                    //    #region "rule was created the same day as the modification"
                    //    query.terminalID = model.PriceTypeRulesInfo_Terminal;
                    //    query.providerID = model.PriceTypeRulesInfo_Provider != "null" ? int.Parse(model.PriceTypeRulesInfo_Provider) : (int?)null;
                    //    query.serviceID = query.serviceID;
                    //    query.genericUnitID = model.PriceTypeRulesInfo_GenericUnit; //model.PriceTypeRulesInfo_GenericUnit != "null" ? int.Parse(model.PriceTypeRulesInfo_GenericUnit) : (int?)null;
                    //    query.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                    //    query.@base = model.PriceTypeRulesInfo_Base;
                    //    if (model.PriceTypeRulesInfo_Base)
                    //    {
                    //        query.percentage = (decimal?)null;
                    //        query.moreOrLess = (bool?)null;
                    //        query.thanPriceTypeID = (int?)null;
                    //    }
                    //    else
                    //    {
                    //        query.percentage = model.PriceTypeRulesInfo_Percentage;
                    //        query.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                    //        query.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                    //    }
                    //    #endregion
                    //}
                    //else
                    //{
                    #region "rule was created in a different date"
                    var rule = new tblServices_PriceTypes();
                    rule.terminalID = query.terminalID;
                    rule.providerID = query.providerID;
                    rule.serviceID = query.serviceID;
                    rule.genericUnitID = query.genericUnitID;
                    rule.priceTypeID = query.priceTypeID;
                    rule.@base = query.@base;
                    rule.percentage = query.percentage;
                    rule.moreOrLess = query.moreOrLess;
                    rule.thanPriceTypeID = query.thanPriceTypeID;
                    rule.permanent_ = true;
                    rule.fromDate = _now;
                    rule.dateSaved = _now;
                    rule.savedByUserID = session.UserID;
                    db.tblServices_PriceTypes.AddObject(rule);
                    query.permanent_ = false;
                    query.toDate = _now.AddSeconds(-1);
                    #endregion
                    //}
                    _services = _services.Where(m => m != query.serviceID).ToArray();
                }
                foreach (var i in _services)
                {
                    var rule = new tblServices_PriceTypes();
                    rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                    rule.providerID = model.PriceTypeRulesInfo_Provider != "null" ? int.Parse(model.PriceTypeRulesInfo_Provider) : (int?)null;
                    rule.serviceID = i;
                    rule.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                    rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                    rule.@base = model.PriceTypeRulesInfo_Base;
                    rule.percentage = model.PriceTypeRulesInfo_Percentage;
                    rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                    rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                    rule.permanent_ = true;
                    rule.fromDate = _now;
                    rule.dateSaved = _now;
                    rule.savedByUserID = session.UserID;
                    db.tblServices_PriceTypes.AddObject(rule);
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Price Type Rule(s) Saved";
                response.ObjectID = 0;
                return response;
                #endregion
            }
            else
            {
                if (model.PriceTypeRulesInfo_PriceTypeRuleID != null && model.PriceTypeRulesInfo_PriceTypeRuleID != 0)
                {
                    #region "update rule"
                    try
                    {
                        if (model.PriceTypeRulesInfo_Service != null)
                        {
                            #region "if rule has activities"
                            foreach (var i in model.PriceTypeRulesInfo_Service)
                            {
                                var _service = long.Parse(i);
                                var ruleExists = ActivitiesCatalogs.RuleExists(ref db, model, _service, _now);
                                if (ruleExists.Count() > 0)
                                {
                                    var service_PriceTypeID = ruleExists.FirstOrDefault().service_priceTypeID;
                                    var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == service_PriceTypeID);

                                    //if (query.tblPriceTypes.isCost || query.thanPriceTypeID == 3) //!query.permanent_ && query.toDate < _now && 
                                    if (query.tblPriceTypes.isCost) //!query.permanent_ && query.toDate < _now && 
                                    {
                                        #region "existing rule will be updated"
                                        query.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                                        query.serviceID = _service;
                                        query.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                                        query.percentage = model.PriceTypeRulesInfo_Percentage;
                                        query.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                        query.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType != 0 ? model.PriceTypeRulesInfo_ThanPriceType : (int?)null;
                                        var message = "Only these fields were modified:<br /><ul>";
                                        message += "Only these fields were modified:<br /><ul>";
                                        message += "<li>Generic Unit</li>";
                                        message += "<li>Percentage</li>";
                                        message += "<li>> or <</li>";
                                        message += "<li>Than Price Type</li>";
                                        message += "</ul>";
                                        response.Message = message;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region "existing rule is not for cost nor expired
                                        var rule = new tblServices_PriceTypes();

                                        rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                                        rule.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                                        rule.serviceID = _service;
                                        rule.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;// model.PriceTypeRulesInfo_GenericUnit != "null" ? int.Parse(model.PriceTypeRulesInfo_GenericUnit) : (int?)null;
                                        rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                                        rule.@base = model.PriceTypeRulesInfo_Base;
                                        rule.percentage = model.PriceTypeRulesInfo_Percentage;
                                        rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                        rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType != 0 ? model.PriceTypeRulesInfo_ThanPriceType : (int?)null;
                                        rule.permanent_ = true;
                                        rule.fromDate = _now;
                                        rule.dateSaved = _now;
                                        rule.savedByUserID = session.UserID;
                                        db.tblServices_PriceTypes.AddObject(rule);
                                        query.permanent_ = false;
                                        query.toDate = _now.AddSeconds(-1);
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region "rule does not exist"
                                    var rule = new tblServices_PriceTypes();

                                    rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                                    rule.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                                    rule.serviceID = _service;
                                    rule.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;// model.PriceTypeRulesInfo_GenericUnit != "null" ? int.Parse(model.PriceTypeRulesInfo_GenericUnit) : (int?)null;
                                    rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                                    rule.@base = model.PriceTypeRulesInfo_Base;
                                    rule.percentage = model.PriceTypeRulesInfo_Percentage;
                                    rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                    rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType != 0 ? model.PriceTypeRulesInfo_ThanPriceType : (int?)null;
                                    rule.permanent_ = true;
                                    rule.fromDate = _now;
                                    rule.dateSaved = _now;
                                    rule.savedByUserID = session.UserID;
                                    db.tblServices_PriceTypes.AddObject(rule);
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region "if rule is of provider level"
                            var ruleExists = ActivitiesCatalogs.RuleExists(ref db, model, (long?)null, _now);
                            var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == model.PriceTypeRulesInfo_PriceTypeRuleID);
                            if (ruleExists.Count() > 0)
                            {
                                if (query.tblPriceTypes.isCost || query.thanPriceTypeID == 3) //!query.permanent_ && query.toDate < _now &&
                                {
                                    #region "rule will be updated"
                                    query.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                                    query.serviceID = (long?)null;
                                    query.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                                    query.percentage = model.PriceTypeRulesInfo_Percentage;
                                    query.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                    query.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType != 0 ? model.PriceTypeRulesInfo_ThanPriceType : (int?)null;
                                    var message = "Only these fields were modified:<br /><ul>";
                                    message += "Only these fields were modified:<br /><ul>";
                                    message += "<li>Generic Unit</li>";
                                    message += "<li>Percentage</li>";
                                    message += "<li>> or <</li>";
                                    message += "<li>Than Price Type</li>";
                                    message += "</ul>";
                                    response.Message = message;
                                    #endregion
                                }
                                else
                                {
                                    #region "rule is not expired or is not for cost"
                                    var rule = new tblServices_PriceTypes();

                                    rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                                    rule.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                                    rule.serviceID = (long?)null;
                                    rule.genericUnitID = (int?)null;
                                    rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                                    rule.@base = model.PriceTypeRulesInfo_Base;
                                    rule.percentage = model.PriceTypeRulesInfo_Percentage;
                                    rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                    rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType != 0 ? model.PriceTypeRulesInfo_ThanPriceType : (int?)null;
                                    rule.permanent_ = true;
                                    rule.fromDate = _now;
                                    rule.dateSaved = _now;
                                    rule.savedByUserID = session.UserID;
                                    db.tblServices_PriceTypes.AddObject(rule);
                                    query.permanent_ = false;
                                    query.toDate = _now.AddSeconds(-1);
                                    #endregion
                                }
                            }
                            else
                            {
                                #region "rule does not exist"
                                var rule = new tblServices_PriceTypes();

                                rule.terminalID = model.PriceTypeRulesInfo_Terminal;
                                rule.providerID = int.Parse(model.PriceTypeRulesInfo_Provider);
                                rule.serviceID = (long?)null;
                                rule.genericUnitID = (int?)null;
                                rule.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                                rule.@base = model.PriceTypeRulesInfo_Base;
                                rule.percentage = model.PriceTypeRulesInfo_Percentage;
                                rule.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                rule.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType != 0 ? model.PriceTypeRulesInfo_ThanPriceType : (int?)null;
                                rule.permanent_ = true;
                                rule.fromDate = _now;
                                rule.dateSaved = _now;
                                rule.savedByUserID = session.UserID;
                                db.tblServices_PriceTypes.AddObject(rule);
                                #endregion
                            }
                            #endregion
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Price Type Rule(s) Updated/Saved";
                        response.ObjectID = 0;//new { ruleID = model.PriceTypeRulesInfo_PriceTypeRuleID,  };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Price Type Rule NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
                else
                {
                    #region "new rule(s)"
                    try
                    {
                        if (model.PriceTypeRulesInfo_Service != null)
                        {
                            #region "save rule with service(s)"
                            var _message = "";
                            var _existingRules = "";
                            foreach (var _service in model.PriceTypeRulesInfo_Service)
                            {

                                var hasBase = false;
                                if (model.PriceTypeRulesInfo_Base)
                                {
                                    hasBase = ActivitiesCatalogs.HasBaseRule(model.PriceTypeRulesInfo_SelectedService, _service, model.PriceTypeRulesInfo_Terminal);
                                }

                                if (!hasBase)
                                {
                                    var ruleExists = ActivitiesCatalogs.RuleExists(ref db, model, long.Parse(_service), _now);
                                    if (ruleExists.Count() == 0)
                                    {
                                        #region "save of not existing rule"
                                        var query = new tblServices_PriceTypes();
                                        query.terminalID = model.PriceTypeRulesInfo_Terminal;
                                        query.providerID = model.PriceTypeRulesInfo_Provider != "null" ? int.Parse(model.PriceTypeRulesInfo_Provider) : (int?)null;
                                        query.serviceID = long.Parse(_service);
                                        query.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;
                                        query.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                                        query.@base = model.PriceTypeRulesInfo_Base;
                                        if (model.PriceTypeRulesInfo_Base)
                                        {
                                            query.percentage = (decimal?)null;
                                            query.moreOrLess = (bool?)null;
                                            query.thanPriceTypeID = (int?)null;
                                        }
                                        else
                                        {
                                            query.percentage = model.PriceTypeRulesInfo_Percentage;
                                            query.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                            query.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                                        }
                                        query.permanent_ = true;
                                        query.fromDate = _now;
                                        query.dateSaved = _now;
                                        query.savedByUserID = session.UserID;
                                        db.tblServices_PriceTypes.AddObject(query);
                                        #endregion
                                    }
                                    else
                                    {
                                        //return message of existing rule and ask for confirmation of overwritten
                                        var service_ = long.Parse(_service);
                                        _message += (_message != "" ? "<br><br>" : "") + "The Activity \"" + db.tblServices.Single(m => m.serviceID == service_).service
                                            + "\" has the rule: <br>" + ruleExists.FirstOrDefault().tblPriceTypes.priceType
                                            + " = " + ruleExists.FirstOrDefault().percentage + "% " + ((bool)ruleExists.FirstOrDefault().moreOrLess ? "More Than " : "Less Than ") + ruleExists.FirstOrDefault().tblPriceTypes1.priceType;
                                        _existingRules += (_existingRules != "" ? "," : "") + ruleExists.FirstOrDefault().service_priceTypeID;
                                    }
                                }
                                else
                                {
                                    response.Type = Attempt_ResponseTypes.Error;
                                    response.Message = "Cannot save two base rules within same level";
                                    response.ObjectID = 0;
                                    return response;
                                }
                            }
                            if (_message != "")
                            {
                                response.Type = Attempt_ResponseTypes.Warning;
                                response.Message = _message + "<br><br>" + "Do you want to overwrite this/these Rule(s)?";
                                response.ObjectID = _existingRules;
                                return response;
                            }
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message = "Price Type Rule(s) Saved";
                            response.ObjectID = 0;
                            return response;
                            #endregion
                        }
                        else
                        {
                            #region "save rule of provider level"
                            var hasBase = false;
                            if (model.PriceTypeRulesInfo_Base)
                            {
                                hasBase = ActivitiesCatalogs.HasBaseRule(model.PriceTypeRulesInfo_SelectedService, null, model.PriceTypeRulesInfo_Terminal);
                            }

                            if (!hasBase)
                            {
                                var query = new tblServices_PriceTypes();
                                query.terminalID = model.PriceTypeRulesInfo_Terminal;
                                query.providerID = model.PriceTypeRulesInfo_Provider != "null" ? int.Parse(model.PriceTypeRulesInfo_Provider) : (int?)null;
                                query.serviceID = (long?)null;
                                query.genericUnitID = model.PriceTypeRulesInfo_GenericUnit;// model.PriceTypeRulesInfo_GenericUnit != "null" ? int.Parse(model.PriceTypeRulesInfo_GenericUnit) : (int?)null;
                                query.priceTypeID = model.PriceTypeRulesInfo_PriceType;
                                query.@base = model.PriceTypeRulesInfo_Base;
                                if (model.PriceTypeRulesInfo_Base)
                                {
                                    query.percentage = (decimal?)null;
                                    query.moreOrLess = (bool?)null;
                                    query.thanPriceTypeID = (int?)null;
                                }
                                else
                                {
                                    query.percentage = model.PriceTypeRulesInfo_Percentage;
                                    query.moreOrLess = model.PriceTypeRulesInfo_MoreOrLess;
                                    query.thanPriceTypeID = model.PriceTypeRulesInfo_ThanPriceType;
                                }
                                query.permanent_ = true;
                                query.fromDate = _now;
                                query.dateSaved = _now;
                                query.savedByUserID = session.UserID;
                                db.tblServices_PriceTypes.AddObject(query);
                            }
                            else
                            {
                                response.Type = Attempt_ResponseTypes.Error;
                                response.Message = "Cannot save two base rules within same level";
                                response.ObjectID = 0;
                                return response;
                            }
                            #endregion
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Price Type Rule(s) Saved";
                        response.ObjectID = 0;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Price Type Rule NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
            }
        }

        public AttemptResponse RestoreActivity(long activityID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var query = db.tblServices.Single(m => m.serviceID == activityID);
                query.deleted = false;
                query.dateDeleted = (DateTime?)null;
                query.deletedByUserID = (Guid?)null;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Service Restored";
                response.ObjectID = query.serviceID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Service NOT Restored";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveActivity(ActivityInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            var _now = DateTime.Now;
            if (model.ActivityInfo_ActivityID != 0 && model.ActivityInfo_ActivityID != null)
            {
                if (db.tblServices.Where(m => m.service == model.ActivityInfo_Activity && m.serviceID != model.ActivityInfo_ActivityID && m.originalTerminalID == model.ActivityInfo_OriginalTerminal && m.deleted == false).Count() == 0)
                {
                    #region
                    try
                    {
                        tblServices activity = db.tblServices.Single(m => m.serviceID == model.ActivityInfo_ActivityID);
                        var videoLength = db.tblVideos_SysItemTypes.Where(m => m.itemID == model.ActivityInfo_ActivityID && m.sysItemTypeID == 1).Count();//Activites == 1
                        activity.service = model.ActivityInfo_Activity;
                        activity.itemTypeID = model.ActivityInfo_ItemType;
                        activity.originalTerminalID = model.ActivityInfo_OriginalTerminal;
                        activity.destinationID = model.ActivityInfo_Destination;
                        activity.providerID = model.ActivityInfo_Provider;
                        activity.applyWholeStay = model.ActivityInfo_ApplyWholeStay != null ? model.ActivityInfo_ApplyWholeStay : false;
                        activity.length = model.ActivityInfo_Length;
                        if (model.ActivityInfo_Zone != 0)
                        {
                            activity.zoneID = model.ActivityInfo_Zone;
                        }
                        else
                        {
                            activity.zoneID = null;
                        }
                        activity.transportationService = model.ActivityInfo_TransportationService;
                        activity.offersRoundTrip = model.ActivityInfo_TransportationService ? model.ActivityInfo_OffersRoundTrip : false;
                        if (model.ActivityInfo_MinimumAge != null)
                        {
                            activity.minimumAge = int.Parse(model.ActivityInfo_MinimumAge);
                        }
                        else
                        {
                            activity.minimumAge = null;
                        }
                        if (model.ActivityInfo_MinimumHeight != null)
                        {
                            activity.minimumHeight = decimal.Parse(model.ActivityInfo_MinimumHeight);
                        }
                        else
                        {
                            activity.minimumHeight = null;
                        }
                        if (model.ActivityInfo_MaximumWeight != null)
                        {
                            activity.maximumWeight = decimal.Parse(model.ActivityInfo_MaximumWeight);
                        }
                        else
                        {
                            activity.maximumWeight = null;
                        }
                        activity.babiesAllowed = model.ActivityInfo_BabiesAllowed;
                        activity.childrenAllowed = model.ActivityInfo_ChildrenAllowed;
                        activity.adultsAllowed = model.ActivityInfo_AdultsAllowed;
                        activity.pregnantsAllowed = model.ActivityInfo_PregnantsAllowed;
                        activity.oldiesAllowed = model.ActivityInfo_OldiesAllowed;
                        activity.excludeForCommission = model.ActivityInfo_ExcludeForCommission;
                        activity.avoidRounding = model.ActivityInfo_AvoidRounding;
                        if (videoLength > 0)
                        {
                            if (model.ActivityInfo_Video != "" && model.ActivityInfo_Video != null && model.ActivityInfo_VideoURL != "" && model.ActivityInfo_VideoURL != null)
                            {
                                //update
                                var video = db.tblVideos_SysItemTypes.Single(m => m.itemID == model.ActivityInfo_ActivityID && m.sysItemTypeID == 1);
                                video.tblVideos.video = model.ActivityInfo_Video;
                                video.tblVideos.url = model.ActivityInfo_VideoURL;
                            }
                            else
                            {
                                //delete video
                                var queryVideo = db.tblVideos_SysItemTypes.Where(m => m.itemID == model.ActivityInfo_ActivityID && m.sysItemTypeID == 1);
                                foreach (var i in queryVideo)
                                {
                                    db.tblVideos.DeleteObject(db.tblVideos.Single(m => m.videoID == i.videoID));
                                    db.tblVideos_SysItemTypes.DeleteObject(i);
                                }
                            }
                        }
                        else
                        {
                            if (model.ActivityInfo_Video != "" && model.ActivityInfo_Video != null && model.ActivityInfo_VideoURL != "" && model.ActivityInfo_VideoURL != null)
                            {
                                var video_item = new tblVideos_SysItemTypes();
                                var video = new tblVideos();
                                video_item.sysItemTypeID = 1;
                                video_item.itemID = activity.serviceID;
                                db.tblVideos_SysItemTypes.AddObject(video_item);
                                video.video = model.ActivityInfo_Video;
                                video.url = model.ActivityInfo_VideoURL;
                                db.tblVideos.AddObject(video);
                            }
                        }
                        //exclusions
                        var savedJP = activity.tblCommissionExclusions.Where(m => m.permanent_);
                        if (model.ActivityInfo_ExcludeForCommission == true)
                        {
                            if (savedJP.Count() > 0)
                            {
                                foreach (var i in savedJP)
                                {
                                    if (model.ActivityInfo_JobPositions != null && model.ActivityInfo_JobPositions.Contains(i.jobPositionID))
                                    {
                                        model.ActivityInfo_JobPositions = model.ActivityInfo_JobPositions.Where(m => m != i.jobPositionID).ToArray();
                                    }
                                    else
                                    {
                                        i.permanent_ = false;
                                        i.toDate = _now;
                                    }
                                }
                            }
                            if (model.ActivityInfo_JobPositions != null)
                            {
                                foreach (var i in model.ActivityInfo_JobPositions)
                                {
                                    var jp = new tblCommissionExclusions();
                                    jp.jobPositionID = i;
                                    jp.fromDate = _now;
                                    jp.permanent_ = true;
                                    activity.tblCommissionExclusions.Add(jp);
                                }
                            }
                        }
                        else
                        {
                            if (savedJP.Count() > 0)
                            {
                                foreach (var i in savedJP)
                                {
                                    i.permanent_ = false;
                                    i.toDate = _now;
                                }
                            }
                        }
                        //end exclusions
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Activity Updated";
                        response.ObjectID = activity.serviceID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Activity NOT Updated";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                    #endregion
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Activity NOT Saved <br />Activity Name Already Exists";
                    response.ObjectID = 0;
                    return response;
                }
            }
            else
            {
                if (db.tblServices.Where(m => m.service.ToLower() == model.ActivityInfo_Activity && m.originalTerminalID == model.ActivityInfo_OriginalTerminal && m.deleted == false).Count() == 0)
                {
                    try
                    {
                        var activity = new tblServices();
                        activity.service = model.ActivityInfo_Activity;
                        activity.itemTypeID = model.ActivityInfo_ItemType;
                        activity.originalTerminalID = model.ActivityInfo_OriginalTerminal;
                        activity.destinationID = model.ActivityInfo_Destination;
                        activity.providerID = model.ActivityInfo_Provider;
                        activity.applyWholeStay = model.ActivityInfo_ApplyWholeStay != null ? model.ActivityInfo_ApplyWholeStay : false;
                        activity.length = model.ActivityInfo_Length;
                        if (model.ActivityInfo_Zone != 0)
                        {
                            activity.zoneID = model.ActivityInfo_Zone;
                        }
                        activity.transportationService = model.ActivityInfo_TransportationService;
                        activity.offersRoundTrip = model.ActivityInfo_TransportationService ? model.ActivityInfo_OffersRoundTrip : false;
                        if (model.ActivityInfo_MinimumAge != null)
                        {
                            activity.minimumAge = int.Parse(model.ActivityInfo_MinimumAge);
                        }
                        if (model.ActivityInfo_MinimumHeight != null)
                        {
                            activity.minimumHeight = decimal.Parse(model.ActivityInfo_MinimumHeight);
                        }
                        if (model.ActivityInfo_MaximumWeight != null)
                        {
                            activity.maximumWeight = decimal.Parse(model.ActivityInfo_MaximumWeight);
                        }
                        activity.babiesAllowed = model.ActivityInfo_BabiesAllowed;
                        activity.childrenAllowed = model.ActivityInfo_ChildrenAllowed;
                        activity.adultsAllowed = model.ActivityInfo_AdultsAllowed;
                        activity.pregnantsAllowed = model.ActivityInfo_PregnantsAllowed;
                        activity.oldiesAllowed = model.ActivityInfo_OldiesAllowed;
                        activity.dateSaved = _now;
                        activity.savedByUserID = session.UserID;
                        activity.excludeForCommission = model.ActivityInfo_ExcludeForCommission;
                        activity.avoidRounding = model.ActivityInfo_AvoidRounding;
                        if (model.ActivityInfo_ItemType == 4)//product
                        {
                            tblStocks stock = new tblStocks();
                            stock.quantity = 0;
                            stock.minimalStock = 0;
                            stock.dateModified = _now;
                            stock.modifiedByUserID = session.UserID;
                            activity.tblStocks.Add(stock);
                        }
                        db.tblServices.AddObject(activity);
                        if (model.ActivityInfo_Video != null && model.ActivityInfo_Video != "" && model.ActivityInfo_VideoURL != null && model.ActivityInfo_VideoURL != "")
                        {
                            var video = new tblVideos();
                            video.video = model.ActivityInfo_Video;
                            video.url = model.ActivityInfo_VideoURL;
                            db.tblVideos.AddObject(video);
                            db.SaveChanges();
                            var video_activity = new tblVideos_SysItemTypes();
                            video_activity.videoID = video.videoID;
                            video_activity.video_SysItemTypeID = 1;
                            video_activity.itemID = activity.serviceID;
                            db.tblVideos_SysItemTypes.AddObject(video_activity);
                        }
                        if (model.ActivityInfo_ExcludeForCommission == true)
                        {
                            if (model.ActivityInfo_JobPositions.Count() > 0)
                            {
                                foreach (var i in model.ActivityInfo_JobPositions)
                                {
                                    var jp = new tblCommissionExclusions();
                                    jp.jobPositionID = i;
                                    jp.fromDate = _now;
                                    jp.permanent_ = true;
                                    activity.tblCommissionExclusions.Add(jp);
                                }
                            }
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Activity Saved";
                        response.ObjectID = activity.serviceID;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Activity NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Activity NOT Saved <br />Activity Name Already Exists";
                    response.ObjectID = 0;
                    return response;
                }
            }
        }

        public AttemptResponse SaveCategoriesToActivity(ActivityCategoryInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = from c in db.tblCategories_Services
                            where c.serviceID == model.ActivityCategoryInfo_ActivityID
                            select c;
                foreach (var i in query)
                {
                    db.tblCategories_Services.DeleteObject(i);
                }
                db.SaveChanges();
                response.Message = "Category(ies) Deleted From Activity";
                if (model.ActivityCategoryInfo_Categories != null)
                {
                    var categories = model.ActivityCategoryInfo_Categories.Split(',');
                    foreach (var i in categories)
                    {
                        tblCategories_Services categoryService = new tblCategories_Services();
                        categoryService.categoryID = Int64.Parse(i);
                        categoryService.serviceID = model.ActivityCategoryInfo_ActivityID;
                        db.tblCategories_Services.AddObject(categoryService);
                    }
                    db.SaveChanges();
                    response.Message = "Category(ies) Saved to Activity";
                }
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = model.ActivityCategoryInfo_ActivityID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                response.Message = "Category(ies) NOT Saved";
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveActivityDescription(ActivityDescriptionInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            if (model.ActivityDescriptionInfo_ActivityDescriptionID != 0)
            {
                try
                {
                    tblServiceDescriptions description = db.tblServiceDescriptions.Single(m => m.serviceDescriptionID == model.ActivityDescriptionInfo_ActivityDescriptionID);
                    description.serviceID = model.ActivityDescriptionInfo_ActivityID;
                    description.service = model.ActivityDescriptionInfo_Activity;
                    description.shortDescription = model.ActivityDescriptionInfo_ShortDescription;
                    description.fullDescription = model.ActivityDescriptionInfo_FullDescription;
                    description.itinerary = model.ActivityDescriptionInfo_Itinerary;
                    description.includes = model.ActivityDescriptionInfo_Includes;
                    description.notes = model.ActivityDescriptionInfo_Notes;
                    description.recommendations = model.ActivityDescriptionInfo_Recommendations;
                    description.policies = model.ActivityDescriptionInfo_Policies;
                    description.cancelationPolicies = model.ActivityDescriptionInfo_CancelationPolicies;
                    description.culture = model.ActivityDescriptionInfo_Culture;
                    description.active = model.ActivityDescriptionInfo_IsActive;
                    description.tag = model.ActivityDescriptionInfo_Tag;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Activity Description Updated";
                    response.ObjectID = description.serviceDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Message = "Activity Description NOT Updated";
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblServiceDescriptions description = new tblServiceDescriptions();
                    description.serviceID = model.ActivityDescriptionInfo_ActivityID;
                    description.service = model.ActivityDescriptionInfo_Activity;
                    description.shortDescription = model.ActivityDescriptionInfo_ShortDescription;
                    description.fullDescription = model.ActivityDescriptionInfo_FullDescription;
                    description.itinerary = model.ActivityDescriptionInfo_Itinerary;
                    description.includes = model.ActivityDescriptionInfo_Includes;
                    description.notes = model.ActivityDescriptionInfo_Notes;
                    description.recommendations = model.ActivityDescriptionInfo_Recommendations;
                    description.policies = model.ActivityDescriptionInfo_Policies;
                    description.cancelationPolicies = model.ActivityDescriptionInfo_CancelationPolicies;
                    description.culture = model.ActivityDescriptionInfo_Culture;
                    description.dateSaved = DateTime.Now;
                    description.savedByUserID = session.UserID;
                    description.active = model.ActivityDescriptionInfo_IsActive;
                    description.tag = model.ActivityDescriptionInfo_Tag;
                    db.tblServiceDescriptions.AddObject(description);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Activity Description Saved";
                    response.ObjectID = description.serviceDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Message = "Activity Description NOT Saved";
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveActivitySchedule(ActivityScheduleInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();
            if (model.ActivityScheduleInfo_ActivityScheduleID != 0)
            {
                try
                {
                    var schedule = db.tblWeeklyAvailability.Single(m => m.weeklyAvailabilityID == model.ActivityScheduleInfo_ActivityScheduleID);
                    schedule.serviceID = model.ActivityScheduleInfo_ActivityID;
                    schedule.monday = model.ActivityScheduleInfo_Monday;
                    schedule.tuesday = model.ActivityScheduleInfo_Tuesday;
                    schedule.wednesday = model.ActivityScheduleInfo_Wednesday;
                    schedule.thursday = model.ActivityScheduleInfo_Thursday;
                    schedule.friday = model.ActivityScheduleInfo_Friday;
                    schedule.saturday = model.ActivityScheduleInfo_Saturday;
                    schedule.sunday = model.ActivityScheduleInfo_Sunday;
                    schedule.permanent_ = model.ActivityScheduleInfo_IsPermanent;
                    schedule.fromDate = model.ActivityScheduleInfo_IsPermanent ? DateTime.Today : DateTime.Parse(model.ActivityScheduleInfo_FromDate, CultureInfo.InvariantCulture);
                    if (model.ActivityScheduleInfo_IsPermanent)
                        schedule.toDate = null;
                    else
                        schedule.toDate = DateTime.Parse(model.ActivityScheduleInfo_ToDate, CultureInfo.InvariantCulture);
                    schedule.range = model.ActivityScheduleInfo_IsSpecificTime ? false : true;
                    schedule.hour = TimeSpan.Parse(model.ActivityScheduleInfo_FromTime, CultureInfo.InvariantCulture);
                    if (model.ActivityScheduleInfo_IsSpecificTime)
                    {
                        schedule.toHour = null;
                        schedule.everyXMinutes = null;
                    }
                    else
                    {
                        schedule.toHour = TimeSpan.Parse(model.ActivityScheduleInfo_ToTime, CultureInfo.InvariantCulture);
                        schedule.everyXMinutes = model.ActivityScheduleInfo_IntervalTime;
                    }
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Schedule Updated";
                    response.ObjectID = schedule.weeklyAvailabilityID;
                    return response;

                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Message = "Schedule NOT Updated";
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var schedule = new tblWeeklyAvailability();
                    schedule.serviceID = model.ActivityScheduleInfo_ActivityID;
                    schedule.monday = model.ActivityScheduleInfo_Monday;
                    schedule.tuesday = model.ActivityScheduleInfo_Tuesday;
                    schedule.wednesday = model.ActivityScheduleInfo_Wednesday;
                    schedule.thursday = model.ActivityScheduleInfo_Thursday;
                    schedule.friday = model.ActivityScheduleInfo_Friday;
                    schedule.saturday = model.ActivityScheduleInfo_Saturday;
                    schedule.sunday = model.ActivityScheduleInfo_Sunday;
                    schedule.permanent_ = model.ActivityScheduleInfo_IsPermanent;
                    schedule.fromDate = model.ActivityScheduleInfo_IsPermanent ? DateTime.Today : DateTime.Parse(model.ActivityScheduleInfo_FromDate, CultureInfo.InvariantCulture);
                    if (model.ActivityScheduleInfo_IsPermanent)
                        schedule.toDate = null;
                    else
                        schedule.toDate = DateTime.Parse(model.ActivityScheduleInfo_ToDate, CultureInfo.InvariantCulture);
                    //schedule.range = model.ActivityScheduleInfo_IsSpecificTime;
                    schedule.range = model.ActivityScheduleInfo_IsSpecificTime ? false : true;
                    schedule.hour = TimeSpan.Parse(model.ActivityScheduleInfo_FromTime, CultureInfo.InvariantCulture);
                    if (model.ActivityScheduleInfo_IsSpecificTime)
                    {
                        schedule.toHour = null;
                        schedule.everyXMinutes = null;
                    }
                    else
                    {
                        schedule.toHour = TimeSpan.Parse(model.ActivityScheduleInfo_ToTime, CultureInfo.InvariantCulture);
                        schedule.everyXMinutes = model.ActivityScheduleInfo_IntervalTime;
                    }
                    db.tblWeeklyAvailability.AddObject(schedule);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Schedule Saved";
                    response.ObjectID = schedule.weeklyAvailabilityID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Message = "Schedule NOT Saved";
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveMeetingPoint(ActivityMeetingPointInfoModel model)
        {
            AttemptResponse response = new AttemptResponse();

            if (model.ActivityMeetingPointInfo_ActivityMeetingPointID != 0)
            {
                #region "Update Meeting Point"
                try
                {
                    var meetingPoint = db.tblMeetingPoints.Single(m => m.meetingPointID == model.ActivityMeetingPointInfo_ActivityMeetingPointID);
                    meetingPoint.hour = TimeSpan.Parse(model.ActivityMeetingPointInfo_DepartureTime);
                    meetingPoint.weeklyAvailabilityID = model.ActivityMeetingPointInfo_WeeklySchedule;
                    if (model.ActivityMeetingPointInfo_AtYourHotel)
                    {
                        meetingPoint.placeID = null;
                    }
                    else
                    {
                        meetingPoint.placeID = model.ActivityMeetingPointInfo_Place;
                    }
                    meetingPoint.atYourHotel = model.ActivityMeetingPointInfo_AtYourHotel;
                    meetingPoint.active = model.ActivityMeetingPointInfo_IsActive;
                    meetingPoint.note = model.ActivityMeetingPointInfo_Notes;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Meeting Point Updated";
                    response.ObjectID = meetingPoint.meetingPointID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Meeting Point NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "Save Meeting Point(s)"
                try
                {
                    List<long> meetingPointID = new List<long>();
                    var result = new List<object>();
                    model.ActivityMeetingPointInfo_DepartureTimes = model.ActivityMeetingPointInfo_DepartureTimes != null ? model.ActivityMeetingPointInfo_DepartureTimes.Replace('_', ':') : model.ActivityMeetingPointInfo_DepartureTimes;
                    var meetingPoints = model.ActivityMeetingPointInfo_DepartureTimes != null ? model.ActivityMeetingPointInfo_DepartureTimes.TrimEnd(',').Split(',').ToArray() : new string[] { "" };
                    foreach (var i in meetingPoints)
                    {
                        var mp = new tblMeetingPoints();
                        mp.serviceID = model.ActivityMeetingPointInfo_ActivityID;
                        mp.weeklyAvailabilityID = model.ActivityMeetingPointInfo_WeeklySchedule;
                        mp.hour = i != "" ? TimeSpan.Parse(i.Split('|')[0]) : TimeSpan.Parse(model.ActivityMeetingPointInfo_DepartureTime);
                        if (i != "")
                        {
                            if (i.Split('|')[1] != "null")
                            {
                                mp.placeID = long.Parse(i.Split('|')[1]);
                                mp.atYourHotel = false;
                            }
                            else
                            {
                                mp.placeID = (long?)null;
                                mp.atYourHotel = true;
                            }
                        }
                        else
                        {
                            mp.placeID = model.ActivityMeetingPointInfo_Place;
                            mp.atYourHotel = model.ActivityMeetingPointInfo_AtYourHotel;
                        }
                        mp.active = model.ActivityMeetingPointInfo_IsActive;
                        mp.note = model.ActivityMeetingPointInfo_Notes;
                        db.tblMeetingPoints.AddObject(mp);
                        db.SaveChanges();
                        meetingPointID.Add(mp.meetingPointID);
                        result.Add(new
                        {
                            meetingPointID = mp.meetingPointID,
                            time = mp.hour.ToString(),
                            place = (mp.placeID != null ? mp.tblPlaces.place + " " + mp.tblPlaces.tblDestinations.destination : "At Your Hotel"),
                            atYourHotel = mp.atYourHotel,
                            active = mp.active
                        });
                    }

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Meeting Point Saved";
                    response.ObjectID = result;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Meeting Point NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public AttemptResponse SaveActivityAccountingAccount(ActivityAccountingAccountInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var _terminalID = db.tblServices.Single(m => m.serviceID == model.ActivityAccountingAccountInfo_ActivityID).originalTerminalID;
            model.ActivityAccountingAccountInfo_AccountingAccount = model.ActivityAccountingAccountInfo_AccountingAccount ?? ActivitiesCatalogs.FillDrpAccountingAccounts(_terminalID).Select(m => int.Parse(m.Value)).ToArray();
            model.ActivityAccountingAccountInfo_PointOfSale = model.ActivityAccountingAccountInfo_PointOfSale ?? MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(_terminalID).Select(m => m.Value).ToArray();

            if (model.ActivityAccountingAccountInfo_ActivityAccountingAccountID != 0)
            {
                try
                {
                    var service_account = db.tblServices_AccountingAccounts.Single(m => m.service_accountID == model.ActivityAccountingAccountInfo_ActivityAccountingAccountID);
                    var currentAccounts = db.tblServices_AccountingAccounts.Where(m => m.accountingAccountID == service_account.accountingAccountID && m.serviceID == service_account.serviceID);

                    foreach (var pointOfSale in model.ActivityAccountingAccountInfo_PointOfSale.Select(m => int.Parse(m)).ToArray())
                    {
                        var existingAccount = currentAccounts.Where(m => m.accountingAccountID == service_account.accountingAccountID && m.pointOfSaleID == pointOfSale);
                        if (existingAccount.Count() > 0)
                        {
                            currentAccounts = currentAccounts.Where(m => m.service_accountID != existingAccount.FirstOrDefault().service_accountID);
                        }
                        else
                        {
                            var serviceAccount = new tblServices_AccountingAccounts();
                            serviceAccount.accountingAccountID = service_account.accountingAccountID;
                            serviceAccount.serviceID = service_account.serviceID;
                            serviceAccount.pointOfSaleID = pointOfSale;
                            serviceAccount.dateSaved = DateTime.Now;
                            serviceAccount.savedByUserID = session.UserID;
                            db.tblServices_AccountingAccounts.AddObject(serviceAccount);
                        }
                    }
                    if (currentAccounts.Count() > 0)
                    {
                        foreach (var i in currentAccounts)
                        {
                            db.DeleteObject(i);
                        }
                    }
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Activity Accounting Account(s) Updated";
                    response.ObjectID = new { accounts = GetAccountingAccountsOfService(model.ActivityAccountingAccountInfo_ActivityID) };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Activity Accounting Account(s) NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    foreach (var account in model.ActivityAccountingAccountInfo_AccountingAccount)
                    {
                        foreach (var pointOfSale in model.ActivityAccountingAccountInfo_PointOfSale)
                        {
                            var _PoS = int.Parse(pointOfSale);
                            if (db.tblServices_AccountingAccounts.Count(m => m.serviceID == model.ActivityAccountingAccountInfo_ActivityID && m.pointOfSaleID == _PoS && m.accountingAccountID == account) == 0)
                            {
                                var service_account = new tblServices_AccountingAccounts();
                                service_account.accountingAccountID = account;
                                service_account.serviceID = model.ActivityAccountingAccountInfo_ActivityID;
                                service_account.pointOfSaleID = _PoS;
                                service_account.dateSaved = DateTime.Now;
                                service_account.savedByUserID = session.UserID;
                                db.tblServices_AccountingAccounts.AddObject(service_account);
                            }
                        }
                    }
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Activity Accounting Account(s) Saved";
                    response.ObjectID = new { accounts = GetAccountingAccountsOfService(model.ActivityAccountingAccountInfo_ActivityID) };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Activity Accounting Account(s) NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveAccountingAccount(ActivityAccountingAccountsModel model)
        {
            CatalogsDataModel.AccountingAccounts aam = new CatalogsDataModel.AccountingAccounts();
            AttemptResponse response = aam.SaveAccountingAccount(model);
            return response;
        }

        public AttemptResponse SavePointOfSale(ActivityPointsOfSaleModel model)
        {
            CatalogsDataModel.PointsOfSale psm = new CatalogsDataModel.PointsOfSale();
            AttemptResponse response = psm.SavePointOfSale(model);
            return response;
        }

        public AttemptResponse DeleteActivity(int activityID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var activity = db.tblServices.Single(m => m.serviceID == activityID);
                activity.deleted = true;
                activity.deletedByUserID = session.UserID;
                activity.dateDeleted = DateTime.Now;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = activityID;
                response.Message = "Activity Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Activity NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteActivityDescription(int activityDescriptionID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var description = db.tblServiceDescriptions.Single(m => m.serviceDescriptionID == activityDescriptionID);
                db.DeleteObject(description);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = activityDescriptionID;
                response.Message = "Activity Description Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Activity Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteActivitySchedule(int activityScheduleID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var description = db.tblWeeklyAvailability.Single(m => m.weeklyAvailabilityID == activityScheduleID);
                db.DeleteObject(description);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = activityScheduleID;
                response.Message = "Activity Schedule Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Activity Schedule NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteActivityMeetingPoint(int activityMeetingPointID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var meetingPoint = db.tblMeetingPoints.Single(m => m.meetingPointID == activityMeetingPointID);
                db.DeleteObject(meetingPoint);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = activityMeetingPointID;
                response.Message = "Activity Meeting Point Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Activity Meeting Point NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteActivityAccountingAccount(int[] accountingAccount, long serviceID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var accounts = db.tblServices_AccountingAccounts.Where(m => accountingAccount.Contains(m.accountingAccountID) && m.serviceID == serviceID);
                foreach (var i in accounts)
                {
                    db.DeleteObject(i);
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                if (accountingAccount.Length == 1)
                {
                    response.ObjectID = new { accountID = accountingAccount[0].ToString(), accounts = GetAccountingAccountsOfService(serviceID) };
                }
                else
                {
                    response.ObjectID = new { accountID = String.Join(",", accountingAccount), accounts = "" };
                }
                response.Message = "Activity Accounting Account(s) Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Activity Accounting Account(s) NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteAccountingAccount(int targetID)
        {
            CatalogsDataModel.AccountingAccounts aam = new CatalogsDataModel.AccountingAccounts();
            AttemptResponse response = aam.DeleteAccountingAccount(targetID);
            return response;
        }

        public AttemptResponse DeletePointOfSale(int targetID)
        {
            CatalogsDataModel.PointsOfSale psm = new CatalogsDataModel.PointsOfSale();
            AttemptResponse response = psm.DeletePointOfSale(targetID);
            return response;
        }

        public AttemptResponse DeleteProvider(int targetID)
        {
            CatalogsDataModel.Providers pm = new CatalogsDataModel.Providers();
            AttemptResponse response = pm.DeleteProvider(targetID);
            return response;
        }

        public AttemptResponse DeletePriceTypeRule(int targetID)
        {
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblServices_PriceTypes.Single(m => m.service_priceTypeID == targetID);
                query.permanent_ = false;
                query.toDate = DateTime.Now;
                query.dateLastModification = DateTime.Now;
                query.modifiedByUserID = session.UserID;
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Price Type Rule Deleted";
                response.ObjectID = targetID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Price Type Rule NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public List<SelectListItem> GetDDLData(string itemType, string itemID)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            switch (itemType)
            {
                case "providers":
                    {
                        var _terminal = itemID.Split('|')[0];
                        var _destination = itemID.Split('|')[1];
                        list = ActivitiesCatalogs.FillDrpProvidersByDestination(long.Parse(_terminal), int.Parse(_destination));
                        break;
                    }
                case "zones":
                    {
                        list = ActivitiesCatalogs.FillDrpZones(int.Parse(itemID));
                        break;
                    }
                case "destinationsByTerminal":
                    {
                        list = ActivitiesCatalogs.FillDrpDestinationsByTerminal(int.Parse(itemID));
                        break;
                    }
                case "destinations":
                    {
                        list = ActivitiesCatalogs.FillDrpDestinations(0);
                        break;
                    }
                case "catalogs":
                    {
                        list = ActivitiesCatalogs.FillDrpCatalogsPerTerminal(int.Parse(itemID));
                        break;
                    }
                case "categories":
                    {
                        list = ActivitiesCatalogs.FillDrpCategoriesPerCatalog(int.Parse(itemID));
                        break;
                    }
                case "weeklyAvailabilities":
                    {
                        list = ActivitiesCatalogs.FillDrpWeeklySchedule(int.Parse(itemID));
                        break;
                    }
                case "places":
                    {
                        list = ActivitiesCatalogs.FillDrpPlaces(int.Parse(itemID));
                        break;
                    }
                case "destinationsPerSelectedTerminals":
                    {
                        list = ActivitiesCatalogs.FillDrpDestinationsBySelectedTerminals();
                        break;
                    }
                case "category":
                    {
                        list = ActivitiesCatalogs.FillDrpCategories();
                        break;
                    }
                case "units":
                    {
                        list = ActivitiesCatalogs.GetUnitTypes();
                        break;
                    }
                case "accountingAccount":
                    {
                        list = ActivitiesCatalogs.FillDrpAccountingAccounts();
                        break;
                    }
                case "pointOfSale":
                    {
                        var _itemID = itemID != null ? long.Parse(itemID) : (long?)null;
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale(_itemID);
                        break;
                    }
                case "provider":
                    {
                        list = ActivitiesCatalogs.FillDrpProvidersByRegion(int.Parse(itemID));
                        list.Insert(0, ListItems.Default((itemID != "0" ? "--Select One--" : "--Select Region / Provider--")));
                        break;
                    }
                case "service":
                    {
                        ePlatEntities db = new ePlatEntities();
                        var _itemID = int.Parse(itemID);
                        list.Add(new SelectListItem() { Value = itemID.ToString(), Text = db.tblServices.Single(m => m.serviceID == _itemID).service });
                        break;
                    }
                case "providersPerDestinations":
                    {
                        list = CatalogsDataModel.Providers.ProvidersCatalogs.FillDrpProvidersPerDestinations(long.Parse(itemID));
                        list.Insert(0, ListItems.NotSet("All Providers"));
                        break;
                    }
                case "region":
                    {
                        list = ActivitiesCatalogs.FillDrpRegions();
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "terminal":
                    {
                        list = TerminalDataModel.GetActiveTerminalsList();
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "currenciesPerProvider":
                    {
                        list = PriceDataModel.PricesCatalogs.FillDrpCurrencies(int.Parse(itemID));
                        list.Insert(0, ListItems.Default("--Select One--", ""));
                        break;
                    }
                case "servicesPerProvider":
                    {
                        list = ActivitiesCatalogs.FillDrpServicesPerProvider(int.Parse(itemID));
                        break;
                    }
                case "activeTerminals":
                    {
                        list = TerminalDataModel.GetActiveTerminalsList();
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "priceTypes":
                    {
                        list = PriceDataModel.PricesCatalogs.FillDrpPriceTypes(int.Parse(itemID));
                        list.Insert(0, ListItems.Default());
                        break;
                    }
                case "jpPerTerminalCommissions":
                    {
                        list = ActivitiesCatalogs.FillDrpJobPositionsPerTerminalCommissions(long.Parse(itemID));
                        break;
                    }
            }
            return list;
        }
        //--
        public List<ActivityImportSearchResultsModel> PrevSearchActivities(PrevActivitySearchModel model)
        {
            ecommerceEntities dba = new ecommerceEntities();
            List<ActivityImportSearchResultsModel> list = new List<ActivityImportSearchResultsModel>();
            var prevTerminals = "";
            long[] terminals;
            int[] oldTerminals;
            int[] activitiesImported;

            terminals = model.PrevSearch_Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            if (model.PrevSearch_Terminals != "")
            {
                foreach (var terminal in model.PrevSearch_Terminals.Split(','))
                {
                    //match terminals and oldTerminals ID's
                    #region
                    switch (int.Parse(terminal))
                    {
                        case 5://My Vallarta Experience
                            {
                                prevTerminals += "1,";
                                break;
                            }
                        case 6://My Cabo Experience
                            {
                                prevTerminals += "3,";
                                break;
                            }
                        case 7://My Cancun Experience
                            {
                                prevTerminals += "15,";
                                break;
                            }
                        case 8://My Loreto Experience
                            {
                                prevTerminals += "16,";
                                break;
                            }
                    }
                    #endregion
                }
            }
            oldTerminals = prevTerminals.TrimEnd(',').Split(',').Select(m => int.Parse(m)).ToArray();
            //get already imported services ID's
            #region
            //comentado para preguntar porqué la consulta se hace a tblCategories_Services en lugar de a tblServices

            var queryActivitiesImported = from a in db.tblServices
                                          where terminals.Contains(a.originalTerminalID)
                                          && a.importedFromID > 0
                                          && a.deleted == false
                                          select a.importedFromID;

            activitiesImported = queryActivitiesImported.Select(m => (int)m).ToArray();
            #endregion

            //get only active and non-imported activities 

            var query = from s in dba.tbaServicios
                        where (s.servicio.Contains(model.PrevActivitySearch_Activity) || model.PrevActivitySearch_Activity == null)
                        && (s.tbaServiciosCategoria.Where(m => m.idServicio == s.idServicio).Select(m => m.idCategoria).Contains(model.PrevActivitySearch_Category) || model.PrevActivitySearch_Category == 0)
                        && (s.idProveedor == model.PrevActivitySearch_Provider || model.PrevActivitySearch_Provider == 0)
                        && activitiesImported.Contains(s.idServicio) == false
                        && s.temporadaFinal > DateTime.Today
                        && s.tbaTours.FirstOrDefault().activo == true
                        && oldTerminals.Contains(s.tbaTerminales.idTerminal)
                        select s;

            foreach (var i in query)
            {
                list.Add(new ActivityImportSearchResultsModel()
                {
                    ActivityID = int.Parse(i.idServicio.ToString()),
                    Activity = i.servicio
                });
            }
            return list;
        }

        public ActivityImportInfoModel GetPrevActivitySettings(int activityID)
        {
            ecommerceEntities dba = new ecommerceEntities();
            ActivityImportInfoModel model = new ActivityImportInfoModel();
            List<SelectListItem> listDescriptions = new List<SelectListItem>();
            List<ActivityImportPricesModel> listPrices = new List<ActivityImportPricesModel>();
            List<ActivityImportAvailabilityModel> listSchedules = new List<ActivityImportAvailabilityModel>();

            model.ActivityImportInfo_PrevActivityID = activityID;
            //descriptions
            var queryDescriptions = from a in dba.tbaTours
                                    where a.idServicio == activityID
                                    && a.activo == true
                                    select a;
            foreach (var i in queryDescriptions)
            {
                listDescriptions.Add(new SelectListItem() { Value = i.idTour.ToString(), Text = i.cultura });
            }
            model.ActivityImportInfo_ListDescriptions = listDescriptions;
            //prices
            var queryPrices = from a in dba.tbaPrecios
                              where a.idServicio == activityID
                              && (a.permanente == true || a.fechaFinal > DateTime.Today)
                              select new
                              {
                                  priceID = a.idPrecio,
                                  price = a.precio,
                                  currency = a.tbaTiposdeMoneda.tipodeMoneda,
                                  unit = a.unidad
                              };
            foreach (var i in queryPrices)
            {
                listPrices.Add(new ActivityImportPricesModel()
                {
                    ActivityImportPrices_PriceID = i.priceID,
                    ActivityImportPrices_Price = i.price.ToString(),
                    ActivityImportPrices_Currency = i.currency,
                    ActivityImportPrices_Unit = i.unit
                });
            }
            model.ActivityImportInfo_ListPrices = listPrices;
            //schedules
            var querySchedules = from a in dba.tbaDisponibilidadSemanal
                                 where a.idServicio == activityID
                                 && a.hora != null
                                 && a.fechaFinal > DateTime.Today
                                 select a;
            foreach (var i in querySchedules)
            {
                var list = new List<ActivityImportMeetingPointModel>();
                var tempQuery = from a in dba.tbaPuntosHorarios
                                where a.idDisponibilidadServicio == i.idDisponibilidadServicio
                                select a;
                foreach (var b in tempQuery)
                {
                    list.Add(new ActivityImportMeetingPointModel()
                    {
                        ActivityImportMeetingPoint_MeetingPointID = b.idPuntoHorario,
                        ActivityImportMeetingPoint_Place = b.idLugar != null ? b.tbaLugares.lugar : "",
                        ActivityImportMeetingPoint_Time = b.hora
                    });
                }

                listSchedules.Add(new ActivityImportAvailabilityModel()
                {
                    ActivityImportAvailability_AvailabilityID = i.idDisponibilidadServicio,
                    ActivityImportAvailability_Monday = (bool)i.lunes,
                    ActivityImportAvailability_Tuesday = (bool)i.martes,
                    ActivityImportAvailability_Wednesday = (bool)i.miercoles,
                    ActivityImportAvailability_Thursday = (bool)i.jueves,
                    ActivityImportAvailability_Friday = (bool)i.viernes,
                    ActivityImportAvailability_Saturday = (bool)i.sabado,
                    ActivityImportAvailability_Sunday = (bool)i.domingo,
                    ActivityImportAvailability_FromTime = i.hora,
                    ActivityImportAvailability_MeetingPoint = list
                });
            }
            model.ActivityImportInfo_ListAvailability = listSchedules;
            return model;
        }

        private int GetTerminalFromPrevious(int terminalID)
        {
            int terminal = 0;
            switch (terminalID)
            {
                case 1://My Vallarta Experience
                    {
                        terminal = 5;
                        break;
                    }
                case 3://My Cabo Experience
                    {
                        terminal = 6;
                        break;
                    }
                case 15://My Cancun Experience
                    {
                        terminal = 7;
                        break;
                    }
                case 16://My Loreto Experience
                    {
                        terminal = 8;
                        break;
                    }
            }
            return terminal;
        }

        //evento temporal para la importación de precios
        //public AttemptResponse ImportPrevActivityPrices()
        //{
        //    ecommerceEntities dba = new ecommerceEntities("name=ecommerceEntities");
        //    AttemptResponse response = new AttemptResponse();
        //    var listActivities = "105,108,109,110,111,112,113,118,129";
        //    var activitiesNotImported = "";
        //    var pricesNotImported = "";
        //    try
        //    {
        //        foreach (var service in listActivities.Split(',').Select(m => long.Parse(m)))
        //        {
        //            var oldID = db.tblServices.Single(m => m.serviceID == service).importedFromID;
        //            var oldPrices = dba.tbaPrecios.Where(m => m.idServicio == oldID && (m.permanente == true || m.fechaFinal > DateTime.Today)).Select(m => m.idPrecio).ToArray();
        //            //prices
        //            #region
        //            if (oldPrices != null)
        //            {
        //                foreach (var i in oldPrices)
        //                {
        //                    var price = new tblPrices();
        //                    var id = i;
        //                    var prevPrice = dba.tbaPrecios.Single(m => m.idPrecio == id);
        //                    try
        //                    {
        //                        price.priceTypeID = prevPrice.idTipoPrecio != null ? (int)prevPrice.idTipoPrecio : 0;
        //                        price.sysItemTypeID = 1;//itemType activity
        //                        price.itemID = service;
        //                        price.price = prevPrice.precio != null ? (decimal)prevPrice.precio : 0;
        //                        price.currencyID = prevPrice.idTipodeMoneda != null ? (int)prevPrice.idTipodeMoneda : 0;
        //                        price.permanent_ = prevPrice.permanente != null ? (bool)prevPrice.permanente : false;
        //                        price.fromDate = prevPrice.fechaInicial;
        //                        price.toDate = prevPrice.fechaFinal;
        //                        price.terminalID = prevPrice.idTerminal != null ? GetTerminalFromPrevious((int)prevPrice.idTerminal) : GetTerminalFromPrevious((int)dba.tbaServicios.Single(m => m.idServicio == oldID).idTerminal);
        //                        price.taxesIncluded = prevPrice.incluyeImpuestos != null ? (bool)prevPrice.incluyeImpuestos : true;
        //                        price.fromTransportationZoneID = prevPrice.idZonaTransporte1;
        //                        price.toTransportationZoneID = prevPrice.idZonaTransporte2;
        //                        price.urlRedeem = prevPrice.urlRedimir;
        //                        price.urlCompare = prevPrice.urlComparar;
        //                        price.priceClasificationID = db.tblPriceClasifications.Single(m => m.priceClasification == "Per Unit").priceClasificationID;
        //                        db.AddObject("tblPrices", price);
        //                        db.SaveChanges();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        activitiesNotImported += service + ",";
        //                        pricesNotImported += prevPrice.idPrecio + ",";
        //                        throw new Exception(ex.Message);
        //                    }
        //                    try
        //                    {
        //                        if ((bool)prevPrice.servicio || (bool)prevPrice.adulto || (bool)prevPrice.nino)
        //                        {
        //                            tblPriceUnits unit = new tblPriceUnits();
        //                            unit.priceID = price.priceID;
        //                            if (prevPrice.adulto != null && (bool)prevPrice.adulto)
        //                            {
        //                                if (prevPrice.tbaTiposdeMoneda.tipodeMoneda == "USD")
        //                                {
        //                                    unit.unit = "Adult";
        //                                    unit.culture = "en-US";
        //                                }
        //                                else
        //                                {
        //                                    unit.unit = "Adulto";
        //                                    unit.culture = "es-MX";
        //                                }
        //                                unit.min = prevPrice.minEdadAdulto != null ? prevPrice.minEdadAdulto.ToString() : "";
        //                                unit.max = prevPrice.maxEdadAdulto != null ? prevPrice.maxEdadAdulto.ToString() : "";
        //                                db.tblPriceUnits.Add(unit);
        //                            }
        //                            if (prevPrice.nino != null && (bool)prevPrice.nino)
        //                            {
        //                                if (prevPrice.tbaTiposdeMoneda.tipodeMoneda == "USD")
        //                                {
        //                                    unit.unit = "Child";
        //                                    unit.culture = "en-US";
        //                                }
        //                                else
        //                                {
        //                                    unit.unit = "Niño";
        //                                    unit.culture = "es-MX";
        //                                }
        //                                unit.min = prevPrice.minEdadNino != null ? prevPrice.minEdadNino.ToString() : "";
        //                                unit.max = prevPrice.maxEdadNino != null ? prevPrice.maxEdadNino.ToString() : "";
        //                                db.tblPriceUnits.Add(unit);
        //                            }
        //                            if (prevPrice.servicio != null && (bool)prevPrice.servicio)
        //                            {
        //                                if (prevPrice.tbaTiposdeMoneda.tipodeMoneda == "USD")
        //                                {
        //                                    unit.culture = "en-US";
        //                                }
        //                                else
        //                                {
        //                                    unit.culture = "es-MX";
        //                                }
        //                                unit.unit = prevPrice.unidad;
        //                                unit.min = "";
        //                                unit.max = "";
        //                                db.tblPriceUnits.Add(unit);
        //                            }
        //                            db.SaveChanges();
        //                        }

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        pricesNotImported += i + ",";
        //                        throw new Exception(ex.Message);
        //                    }
        //                }
        //            }
        //            #endregion
        //        }
        //        response.Type = Attempt_ResponseTypes.Ok;
        //        response.Message = "Prices Imported";
        //        response.ObjectID = 0;
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Type = Attempt_ResponseTypes.Error;
        //        response.Message = "Prices NOT imported <br />Activities:" + activitiesNotImported + "<br />Prices:" + pricesNotImported;
        //        response.ObjectID = 0;
        //        response.Exception = ex;
        //        return response;
        //    }
        //}

        public AttemptResponse ImportPrevActivitySettings(ActivityImportInfoModel model)
        {
            ecommerceEntities dba = new ecommerceEntities();
            AttemptResponse response = new AttemptResponse();
            var prevActivity = dba.tbaServicios.Single(m => m.idServicio == model.ActivityImportInfo_PrevActivityID);
            var activity = new tblServices();
            var activityID = 0;
            var importedActivities = from a in db.tblServices
                                     where a.importedFromID != 0
                                     && a.deleted == false
                                     select a.importedFromID;
            if (!importedActivities.Contains(model.ActivityImportInfo_PrevActivityID))
            {
                try
                {
                    if (db.tblServices.Where(m => m.service == prevActivity.servicio && m.providerID == prevActivity.idProveedor).Count() == 0)
                    {
                        activity.originalTerminalID = GetTerminalFromPrevious((int)prevActivity.idTerminal);

                        #region "Destination Assignment
                        switch (prevActivity.tbaTerminales.tbaTerminalesDestinos.FirstOrDefault().tbaDestinos.destino)
                        {
                            case "Puerto Vallarta":
                                {
                                    activity.destinationID = 1;
                                    break;
                                }
                            case "Cabo San Lucas":
                                {
                                    activity.destinationID = 2;
                                    break;
                                }
                            case "Loreto":
                                {
                                    activity.destinationID = 3;
                                    break;
                                }
                            case "Riviera Nayarit":
                                {
                                    activity.destinationID = 4;
                                    break;
                                }
                            case "Cancun":
                                {
                                    activity.destinationID = 5;
                                    break;
                                }
                        }
                        #endregion

                        activity.service = prevActivity.servicio;
                        activity.providerID = prevActivity.idProveedor != null ? (int)prevActivity.idProveedor : 0;
                        activity.applyWholeStay = prevActivity.aplicaTodaLaEstancia != null ? (bool)prevActivity.aplicaTodaLaEstancia : false;
                        activity.length = prevActivity.duracion ?? 0;
                        activity.zoneID = prevActivity.idZona;
                        activity.transportationService = prevActivity.esTransportacion != null ? (bool)prevActivity.esTransportacion : false;
                        activity.deleted = false;
                        activity.isTest = false;
                        activity.importedFromID = prevActivity.idServicio;
                        activity.dateSaved = DateTime.Now;
                        db.tblServices.AddObject(activity);
                        db.SaveChanges();
                        activityID = (int)activity.serviceID;
                        try
                        {
                            #region "Schedules"
                            try
                            {
                                if (model.ActivityImportInfo_Availability != null)
                                {
                                    var schedules = model.ActivityImportInfo_Availability.Split(',');
                                    foreach (var i in schedules)
                                    {
                                        var schedule = i.IndexOf('_') != -1 ? i.Substring(0, i.IndexOf('_')) : i;
                                        var meetingPoints = i.IndexOf('_') != -1 ? i.Substring(i.IndexOf('_') + 1).Split('-') : new string[] { };
                                        var availability = new tblWeeklyAvailability();
                                        var id = int.Parse(schedule);
                                        var prevAvailability = dba.tbaDisponibilidadSemanal.Single(m => m.idDisponibilidadServicio == id);

                                        availability.serviceID = activity.serviceID;
                                        availability.monday = prevAvailability.lunes != null ? (bool)prevAvailability.lunes : false;
                                        availability.tuesday = prevAvailability.martes != null ? (bool)prevAvailability.martes : false;
                                        availability.wednesday = prevAvailability.miercoles != null ? (bool)prevAvailability.miercoles : false;
                                        availability.thursday = prevAvailability.jueves != null ? (bool)prevAvailability.jueves : false;
                                        availability.friday = prevAvailability.viernes != null ? (bool)prevAvailability.viernes : false;
                                        availability.saturday = prevAvailability.sabado != null ? (bool)prevAvailability.sabado : false;
                                        availability.sunday = prevAvailability.domingo != null ? (bool)prevAvailability.domingo : false;
                                        var hora = prevAvailability.hora.Substring(0, 2) + ":" + prevAvailability.hora.Substring(2, 2);
                                        availability.hour = TimeSpan.Parse(hora);
                                        availability.fromDate = prevAvailability.fechaInicio;
                                        availability.toDate = prevAvailability.fechaFinal;
                                        availability.permanent_ = prevAvailability.fechaFinal != null ? false : true;
                                        availability.range = prevAvailability.esRango != null ? (bool)prevAvailability.esRango : false;
                                        if (prevAvailability.esRango != null && (bool)prevAvailability.esRango)
                                        {
                                            availability.toHour = TimeSpan.Parse(prevAvailability.horaFinal.Substring(0, 2) + ":" + prevAvailability.horaFinal.Substring(2, 2));

                                        }
                                        availability.everyXMinutes = prevAvailability.cadaMinutos;
                                        db.tblWeeklyAvailability.AddObject(availability);
                                        db.SaveChanges();
                                        foreach (var a in meetingPoints)
                                        {
                                            var meetingPoint = new tblMeetingPoints();
                                            var mpID = int.Parse(a);
                                            var prevMeetingPoint = dba.tbaPuntosHorarios.Single(m => m.idPuntoHorario == mpID);
                                            meetingPoint.serviceID = activity.serviceID;
                                            meetingPoint.weeklyAvailabilityID = availability.weeklyAvailabilityID;
                                            if (prevMeetingPoint.idLugar != null)
                                            {
                                                if (prevMeetingPoint.idLugar != 16)
                                                {
                                                    var place = db.tblPlaces.Where(m => m.oldPlaceID == prevMeetingPoint.idLugar);
                                                    if (place.Count() > 0)
                                                    {
                                                        meetingPoint.placeID = db.tblPlaces.FirstOrDefault(m => m.oldPlaceID == prevMeetingPoint.idLugar).placeID;
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("<b>" + prevMeetingPoint.tbaLugares.lugar + "</b> Does Not Exist in ePlat.");
                                                    }
                                                }
                                                else
                                                {
                                                    meetingPoint.atYourHotel = true;
                                                }
                                            }
                                            var time = prevMeetingPoint.hora.Substring(0, 2) + ":" + prevMeetingPoint.hora.Substring(2, 2);
                                            meetingPoint.hour = TimeSpan.Parse(time);
                                            meetingPoint.atYourHotel = meetingPoint.atYourHotel != null ? meetingPoint.atYourHotel : prevMeetingPoint.suHotel != null ? (bool)prevMeetingPoint.suHotel : false;
                                            meetingPoint.note = prevMeetingPoint.comentario;
                                            meetingPoint.active = prevMeetingPoint.activo != null ? (bool)prevMeetingPoint.activo : false;
                                            db.tblMeetingPoints.AddObject(meetingPoint);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Schedules Importation Failed: " + ex.Message);
                            }
                            #endregion

                            #region "Descriptions"
                            try
                            {
                                if (model.ActivityImportInfo_Descriptions != null)
                                {
                                    var descriptions = model.ActivityImportInfo_Descriptions.Split(',');
                                    foreach (var i in descriptions)
                                    {
                                        var description = new tblServiceDescriptions();
                                        var id = int.Parse(i);
                                        var prevDescription = dba.tbaTours.Single(m => m.idTour == id);
                                        description.serviceID = activity.serviceID;
                                        description.service = prevDescription.nombre;
                                        description.shortDescription = prevDescription.descripcionCorta;
                                        description.fullDescription = prevDescription.descripcionLarga;
                                        description.itinerary = prevDescription.itinerario;
                                        description.includes = prevDescription.incluye;
                                        description.notes = prevDescription.nota;
                                        description.recommendations = prevDescription.recomendaciones;
                                        description.policies = prevDescription.politicas;
                                        description.culture = prevDescription.cultura;
                                        description.dateSaved = DateTime.Today;
                                        description.savedByUserID = session.UserID;
                                        description.active = true;
                                        db.tblServiceDescriptions.AddObject(description);
                                    }
                                    db.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Descriptions Importation Failed: " + ex.Message);
                            }
                            #endregion

                            #region "Prices"
                            try
                            {
                                if (model.ActivityImportInfo_Prices != null)
                                {
                                    var prices = model.ActivityImportInfo_Prices.Split(',');
                                    foreach (var i in prices)
                                    {
                                        var price = new tblPrices();
                                        var id = int.Parse(i);
                                        var prevPrice = dba.tbaPrecios.Single(m => m.idPrecio == id);
                                        price.priceTypeID = prevPrice.idTipoPrecio != null ? (int)prevPrice.idTipoPrecio : 0;
                                        price.sysItemTypeID = 1;//itemType activity
                                        price.itemID = activity.serviceID;
                                        price.price = prevPrice.precio != null ? (decimal)prevPrice.precio : 0;
                                        price.currencyID = prevPrice.idTipodeMoneda != null ? (int)prevPrice.idTipodeMoneda : 0;
                                        price.permanent_ = prevPrice.permanente != null ? (bool)prevPrice.permanente : false;
                                        price.fromDate = prevPrice.fechaInicial;
                                        price.toDate = prevPrice.fechaFinal;
                                        price.terminalID = prevPrice.idTerminal != null ? GetTerminalFromPrevious((int)prevPrice.idTerminal) : activity.originalTerminalID;
                                        price.taxesIncluded = prevPrice.incluyeImpuestos != null ? (bool)prevPrice.incluyeImpuestos : true;
                                        price.fromTransportationZoneID = prevPrice.idZonaTransporte1;
                                        price.toTransportationZoneID = prevPrice.idZonaTransporte2;
                                        price.urlRedeem = prevPrice.urlRedimir;
                                        price.urlCompare = prevPrice.urlComparar;
                                        price.priceClasificationID = db.tblPriceClasifications.Single(m => m.priceClasification == "Per Unit").priceClasificationID;
                                        db.tblPrices.AddObject(price);
                                        db.SaveChanges();
                                        if ((bool)prevPrice.servicio || (bool)prevPrice.adulto || (bool)prevPrice.nino)
                                        {
                                            //price.priceClasificationID = db.tblPriceClasifications.Single(m => m.priceClasification == "Per Unit").priceClasificationID;
                                            tblPriceUnits unit = new tblPriceUnits();
                                            unit.priceID = price.priceID;
                                            if ((bool)prevPrice.adulto)
                                            {
                                                if (prevPrice.tbaTiposdeMoneda.tipodeMoneda == "USD")
                                                {
                                                    unit.unit = "Adult";
                                                    unit.culture = "en-US";
                                                }
                                                else
                                                {
                                                    unit.unit = "Adulto";
                                                    unit.culture = "es-MX";
                                                }
                                                unit.min = prevPrice.minEdadAdulto != null ? prevPrice.minEdadAdulto.ToString() : "";
                                                unit.max = prevPrice.maxEdadAdulto != null ? prevPrice.maxEdadAdulto.ToString() : "";
                                                db.tblPriceUnits.AddObject(unit);
                                            }
                                            if ((bool)prevPrice.nino)
                                            {
                                                if (prevPrice.tbaTiposdeMoneda.tipodeMoneda == "USD")
                                                {
                                                    unit.unit = "Child";
                                                    unit.culture = "en-US";
                                                }
                                                else
                                                {
                                                    unit.unit = "Niño";
                                                    unit.culture = "es-MX";
                                                }
                                                unit.min = prevPrice.minEdadNino != null ? prevPrice.minEdadNino.ToString() : "";
                                                unit.max = prevPrice.maxEdadNino != null ? prevPrice.maxEdadNino.ToString() : "";
                                                db.tblPriceUnits.AddObject(unit);
                                            }
                                            if ((bool)prevPrice.servicio)
                                            {
                                                if (prevPrice.tbaTiposdeMoneda.tipodeMoneda == "USD")
                                                {
                                                    unit.culture = "en-US";
                                                }
                                                else
                                                {
                                                    unit.culture = "es-MX";
                                                }
                                                unit.unit = prevPrice.unidad;
                                                unit.min = "";
                                                unit.max = "";
                                                db.tblPriceUnits.AddObject(unit);
                                            }
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Prices Importation Failed: " + ex.Message);
                            }
                            #endregion
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.Message += "Service Imported";
                            response.ObjectID = new { id = activity.serviceID, activity = activity.service, terminal = activity.originalTerminalID };
                            return response;
                        }
                        catch (Exception ex)
                        {
                            db.Dispose();
                            ePlatEntities bd = new ePlatEntities();
                            var schedules = bd.tblWeeklyAvailability.Where(m => m.serviceID == activityID).ToArray();
                            var descriptions = bd.tblServiceDescriptions.Where(m => m.serviceID == activityID).ToArray();
                            var prices = bd.tblPrices.Where(m => m.itemID == activityID && m.sysItemTypeID == 1).ToArray();
                            #region "Schedules"
                            if (schedules.Count() > 0)
                            {
                                foreach (var schedule in schedules)
                                {

                                    var meetingPoints = bd.tblMeetingPoints.Where(m => m.weeklyAvailabilityID == schedule.weeklyAvailabilityID).ToArray();
                                    foreach (var meetingPoint in meetingPoints)
                                    {
                                        bd.DeleteObject(meetingPoint);
                                    }
                                    bd.SaveChanges();
                                    bd.DeleteObject(schedule);
                                }
                            }
                            #endregion

                            #region "Descriptions"
                            if (descriptions.Count() > 0)
                            {
                                foreach (var description in descriptions)
                                {
                                    bd.DeleteObject(description);
                                }
                            }
                            #endregion

                            #region "Prices"
                            if (prices.Count() > 0)
                            {
                                foreach (var price in prices)
                                {
                                    var priceUnits = db.tblPriceUnits.Where(m => m.priceID == price.priceID);
                                    foreach (var i in priceUnits)
                                    {
                                        db.DeleteObject(i);
                                    }
                                    bd.SaveChanges();
                                    bd.DeleteObject(price);
                                }
                            }
                            #endregion
                            bd.SaveChanges();
                            var query = bd.tblServices.Single(m => m.serviceID == activityID);
                            bd.DeleteObject(query);
                            bd.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Message = "Service NOT Imported";
                            response.ObjectID = 0;
                            response.Exception = ex;
                            return response;
                        }
                    }
                    else
                    {
                        throw new Exception("Activity Name Already Exists");
                    }
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Service NOT Imported";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Service already Imported";
                response.ObjectID = 0;
                return response;
            }
        }

        /*Activity Detail Front End*/
        public ActivityReviewsViewModel GetPurchaseReviews(Guid purchaseID)
        {
            ActivityReviewsViewModel model = new ActivityReviewsViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();

            var purchaseQuery = (from p in db.tblPurchases
                                 where p.purchaseID == purchaseID
                                 select new
                                 {
                                     p.tblLeads.firstName,
                                     p.tblLeads.lastName,
                                     services = p.tblPurchases_Services.Where(x => x.serviceStatusID == 3).Select(x => new
                                     {
                                         x.purchase_ServiceID,
                                         x.serviceID,
                                         x.tblServices.tblServiceDescriptions.FirstOrDefault(d => d.culture == culture).service,
                                         x.tblServices.tblProviders.comercialName,
                                         x.serviceDateTime
                                     })
                                 }).FirstOrDefault();

            if (purchaseQuery != null)
            {
                model.FirstName = purchaseQuery.firstName;
                model.LastName = purchaseQuery.lastName;
                model.PurchaseID = purchaseID;
                model.ServicesReviews = new List<ActivityReviewsViewModel.ServiceReviewItem>();
                foreach (var service in purchaseQuery.services)
                {
                    ActivityReviewsViewModel.ServiceReviewItem newServiceReview = new ActivityReviewsViewModel.ServiceReviewItem();
                    newServiceReview.PurchaseServiceID = service.purchase_ServiceID;
                    newServiceReview.ServiceID = service.serviceID;
                    newServiceReview.Service = service.service;
                    newServiceReview.Provider = service.comercialName;
                    newServiceReview.ServiceDateTime = service.serviceDateTime;
                    List<PictureListItem> Pictures = PictureDataModel.GetPictures(1, service.serviceID);
                    if (Pictures.Count() > 0)
                    {
                        newServiceReview.ServicePicture = Pictures.FirstOrDefault().Picture;
                    }

                    newServiceReview.Rating = 0;
                    IEnumerable<ReviewListItem> reviews = GetReviews(1, service.serviceID);
                    newServiceReview.NumberOfReviews = reviews.Count();
                    if (reviews.Count() > 0)
                    {
                        decimal points = 0;
                        foreach (ReviewListItem r in reviews)
                        {
                            points += r.Rating;
                        }
                        newServiceReview.Rating = points / reviews.Count();
                    }
                    else
                    {
                        newServiceReview.Rating = 0;
                    }

                    var ReviewQuery = (from r in db.tblReviews
                                       where r.purchase_ServiceID == service.purchase_ServiceID
                                       select r).FirstOrDefault();
                    if (ReviewQuery != null)
                    {
                        newServiceReview.Rating = ReviewQuery.rating;
                        newServiceReview.Review = ReviewQuery.review;
                        newServiceReview.Picture = ReviewQuery.picture;
                    }
                    model.ServicesReviews.Add(newServiceReview);
                }
            }

            //template and SEO
            model.Culture = culture.Substring(0, 2).ToLower();
            DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
            if (objMaster != null)
            {
                model.Scripts_Header = objMaster.Scripts_Header;
                model.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                model.Scripts_Footer = objMaster.Scripts_Footer;
                model.Template_Header = objMaster.Template_Header;
                model.Template_Footer = objMaster.Template_Footer;
                model.Template_Logo = objMaster.Template_Logo;
                model.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                model.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
            }

            model.Seo_Title = "Share your experience";
            model.Seo_Keywords = "";
            model.Seo_Description = "";
            model.Seo_Index = "noindex";
            model.Seo_Follow = "nofollow";
            model.CanonicalDomain = ePlatBack.Models.Utils.GeneralFunctions.GetCanonicalDomain();
            model.Seo_FriendlyUrl = "/activiries/reviews";

            //Submenus
            SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
            if (objSubmenus != null)
            {
                model.Submenu1 = objSubmenus.Submenu1;
                model.Submenu2 = objSubmenus.Submenu2;
                model.Submenu3 = objSubmenus.Submenu3;
            }

            return model;
        }

        public CategoryActivitiesViewModel GetCategoryActivities(long id, string z)
        {
            CategoryActivitiesViewModel category = new CategoryActivitiesViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            var CategoryQ = (from c in db.tblCategories
                             join description in db.tblCategoryDescriptions on c.categoryID equals description.categoryID into categoryDescription
                             from description in categoryDescription.DefaultIfEmpty()
                             where c.categoryID == id && (description.culture == culture || description == null)
                             select new
                             {
                                 CategoryName = description.category,
                                 CategoryDescription = description.description,
                                 Category = c.category
                             }).FirstOrDefault();

            if (CategoryQ != null)
            {
                if (CategoryQ.CategoryName != null)
                {
                    category.Category = CategoryQ.CategoryName;
                }
                else
                {
                    category.Category = CategoryQ.Category;
                }
                if (CategoryQ.CategoryDescription != null)
                {
                    category.Description = CategoryQ.CategoryDescription;
                }

            }

            //Activities
            category.Activities = GetActivitiesList(id, z).OrderBy(x => x.OfferPrice);

            //Zones
            category.Zones = new List<SelectListItem>();
            long terminalid = Utils.GeneralFunctions.GetTerminalID();

            List<int?> zoneIDs = (from x in db.tblCategories_Services
                                  join service in db.tblServices on x.serviceID equals service.serviceID
                                  join provider in db.tblProviders on service.providerID equals provider.providerID
                                  join description in db.tblServiceDescriptions on x.serviceID equals description.serviceID
                                  into serviceDescription
                                  from description in serviceDescription.DefaultIfEmpty()
                                  where x.categoryID == id
                                  && service.zoneID != null
                                  && service.deleted != true
                                  && description != null
                                  && description.active == true
                                  && provider.isActive == true
                                  select service.zoneID).Distinct().ToList();

            var zones = db.tblZones.Where(x => zoneIDs.Contains(x.zoneID)).OrderBy(x => x.zone);

            category.Zones.Add(new SelectListItem()
            {
                Value = "",
                Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Any_Zone
            });
            foreach (var zone in zones)
            {
                category.Zones.Add(new SelectListItem()
                {
                    Value = zone.zone.ToLower().Replace(" ", "-"),
                    Text = zone.zone
                });
            }

            //Template
            category.Culture = culture.Substring(0, 2).ToLower();
            DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
            if (objMaster != null)
            {
                category.Scripts_Header = objMaster.Scripts_Header;
                category.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                category.Scripts_Footer = objMaster.Scripts_Footer;
                category.Template_Header = objMaster.Template_Header;
                category.Template_Footer = objMaster.Template_Footer;
                category.Template_Logo = objMaster.Template_Logo;
                category.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                category.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
            }
            //SEO
            tblSeoItems objSeo = PageDataModel.GetSeo(8, id, culture);
            if (objSeo != null)
            {
                category.Seo_Title = objSeo.title;
                category.Seo_Keywords = objSeo.keywords;
                category.Seo_Description = objSeo.description;
                category.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                category.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");
            }
            //Submenus
            SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
            if (objSubmenus != null)
            {
                category.Submenu1 = objSubmenus.Submenu1;
                category.Submenu2 = objSubmenus.Submenu2;
                category.Submenu3 = objSubmenus.Submenu3;
            }

            return category;
        }

        public CategoryActivitiesViewModel GetCategoryActivitiesV2(long id, string z)
        {
            CategoryActivitiesViewModel category = new CategoryActivitiesViewModel();
            string culture = Utils.GeneralFunctions.GetCulture();
            var CategoryQ = (from c in db.tblCategories
                             join description in db.tblCategoryDescriptions on c.categoryID equals description.categoryID into categoryDescription
                             from description in categoryDescription.DefaultIfEmpty()
                             where c.categoryID == id && (description.culture == culture || description == null)
                             select new
                             {
                                 CategoryName = description.category,
                                 CategoryDescription = description.description,
                                 Category = c.category
                             }).FirstOrDefault();

            if (CategoryQ != null)
            {
                if (CategoryQ.CategoryName != null)
                {
                    category.Category = CategoryQ.CategoryName;
                }
                else
                {
                    category.Category = CategoryQ.Category;
                }
                if (CategoryQ.CategoryDescription != null)
                {
                    category.Description = CategoryQ.CategoryDescription;
                }

            }

            //Activities
            long terminalid = Utils.GeneralFunctions.GetTerminalID();
            category.Activities = GetActivitiesListWithCache(id, terminalid, z).OrderBy(x => x.OfferPrice);

            //Zones
            category.Zones = new List<SelectListItem>();

            List<int?> zoneIDs = (from x in db.tblCategories_Services
                                  join service in db.tblServices on x.serviceID equals service.serviceID
                                  join provider in db.tblProviders on service.providerID equals provider.providerID
                                  join description in db.tblServiceDescriptions on x.serviceID equals description.serviceID
                                  into serviceDescription
                                  from description in serviceDescription.DefaultIfEmpty()
                                  where x.categoryID == id
                                  && service.zoneID != null
                                  && service.deleted != true
                                  && description != null
                                  && description.active == true
                                  && provider.isActive == true
                                  select service.zoneID).Distinct().ToList();

            var zones = db.tblZones.Where(x => zoneIDs.Contains(x.zoneID)).OrderBy(x => x.zone);

            category.Zones.Add(new SelectListItem()
            {
                Value = "",
                Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Any_Zone
            });
            foreach (var zone in zones)
            {
                category.Zones.Add(new SelectListItem()
                {
                    Value = zone.zone.ToLower().Replace(" ", "-"),
                    Text = zone.zone
                });
            }

            //Template
            category.Culture = culture.Substring(0, 2).ToLower();
            DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
            if (objMaster != null)
            {
                category.Scripts_Header = objMaster.Scripts_Header;
                category.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                category.Scripts_Footer = objMaster.Scripts_Footer;
                category.Template_Header = objMaster.Template_Header;
                category.Template_Footer = objMaster.Template_Footer;
                category.Template_Logo = objMaster.Template_Logo;
                category.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                category.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
                category.CanonicalDomain = objMaster.CanonicalDomain;
            }
            //SEO
            tblSeoItems objSeo = PageDataModel.GetSeo(8, id, culture);
            if (objSeo != null)
            {
                category.Seo_Title = objSeo.title;
                category.Seo_Keywords = objSeo.keywords;
                category.Seo_Description = objSeo.description;
                category.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                category.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");
                category.Seo_FriendlyUrl = objSeo.friendlyUrl;
            }
            tblSeoItems objSeo2 = PageDataModel.GetSeo(8, id, (culture == "es-MX" ? "en-US" : "es-MX"));
            if (objSeo2 != null)
            {
                category.Seo_OppositeLanguageUrl = objSeo2.friendlyUrl;
            }
            //Submenus
            SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
            if (objSubmenus != null)
            {
                category.Submenu1 = objSubmenus.Submenu1;
                category.Submenu2 = objSubmenus.Submenu2;
                category.Submenu3 = objSubmenus.Submenu3;
            }

            return category;
        }

        public ActivityDetailViewModel GetActivityDetail(long id, string culture = "", long? terminalid = null)
        {
            ActivityDetailViewModel activity = new ActivityDetailViewModel();
            if (culture == "")
            {
                culture = Utils.GeneralFunctions.GetCulture();
            }
            var activityQ = (from a in db.tblServices
                             join description in db.tblServiceDescriptions on a.serviceID equals description.serviceID
                             into serviceDescription
                             from description in serviceDescription.DefaultIfEmpty()
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
                                 a.minimumAge,
                                 a.minimumHeight,
                                 a.babiesAllowed,
                                 a.childrenAllowed,
                                 a.adultsAllowed,
                                 a.pregnantsAllowed,
                                 a.oldiesAllowed,
                                 a.length,
                                 description.fullDescription,
                                 description.itinerary,
                                 description.includes,
                                 description.recommendations,
                                 description.notes,
                                 description.policies
                             }).FirstOrDefault();

            if (activityQ != null)
            {
                activity.ItemTypeID = 1;
                activity.ActivityID = id;
                activity.IsTransportation = activityQ.transportationService;
                if (activityQ.transportationService)
                {
                    activity.ItemTypeID = 3;
                }
                activity.Activity = activityQ.service;
                activity.Features_MinimumAge = activityQ.minimumAge;
                activity.Features_MinimumHeight = activityQ.minimumHeight;
                activity.Features_BabiesAllowed = activityQ.babiesAllowed;
                activity.Features_ChildrenAllowed = activityQ.childrenAllowed;
                activity.Features_AdultsAllowed = activityQ.adultsAllowed;
                activity.Features_PregnantsAllowed = activityQ.pregnantsAllowed;
                activity.Features_OldiesAllowed = activityQ.oldiesAllowed;
                activity.Length = (activityQ.length / 60) + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.hours + (activityQ.length % 60 > 0 ? " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.and + " " + activityQ.length % 60 + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.minutes : "");
                activity.Description = activityQ.fullDescription;
                //
                activity.Itinerary = activityQ.itinerary;
                activity.Included = activityQ.includes;
                activity.Recommendations = activityQ.recommendations;
                activity.Notes = activityQ.notes;
                activity.Restrictions = activityQ.policies;

                //promo, 
                PromoDataModel promoDM = new PromoDataModel();
                PromoViewModel promoObj = promoDM.GetPromoForItemID(id, activity.ItemTypeID);

                activity.Promo_MainTag = promoObj.MainTag;
                activity.Promo_TitleTag = promoObj.TitleTag;
                activity.Promo_Description = promoObj.Description;
                activity.Promo_Instructions = promoObj.Instructions;
                activity.Promo_TagColor = promoObj.TagColor;
                activity.Promo_TextColor = promoObj.TextColor;

                //video, 
                var VideoQ = (from v in db.tblVideos_SysItemTypes
                              where v.sysItemTypeID == 1
                              && v.itemID == id
                              select v.tblVideos.url).FirstOrDefault();

                if (VideoQ != null)
                {
                    activity.Video_Url = VideoQ;
                }

                //prices, 
                if (activityQ.transportationService != true)
                {
                    if (terminalid == null)
                    {
                        terminalid = activityQ.originalTerminalID;
                    }
                    DateTime nextActivePriceDate = GetNextActivePriceDate(id, terminalid);
                    activity.Prices = GetPrices(id, nextActivePriceDate, terminalid, culture);
                }
                else
                {
                    activity.Prices = new List<PriceListItem>();
                }

                //schedules, 
                activity.Schedules = GetSchedules(id);

                //meeting points, 
                activity.MeetingPoints = GetMeetingPoints(id);

                //alternatives, 
                activity.AlternativeActivities = GetAlternativeActivities(id);

                //pictures, 
                activity.Pictures = PictureDataModel.GetPictures(1, id);

                //reviews
                IEnumerable<ReviewListItem> reviews = GetReviews(1, id);
                activity.Reviews = reviews;
                if (reviews.Count() > 0)
                {
                    decimal points = 0;
                    foreach (ReviewListItem r in reviews)
                    {
                        points += r.Rating;
                    }
                    activity.Rating = points / reviews.Count();
                }
                else
                {
                    activity.Rating = 0;
                }
                activity.NewReview = new ReviewListItem();

                //template and SEO
                activity.Culture = culture.Substring(0, 2).ToLower();
                DomainSettingsViewModel objMaster = PageDataModel.GetMasterSettings();
                if (objMaster != null)
                {
                    activity.Scripts_Header = objMaster.Scripts_Header;
                    activity.Scripts_AfterBody = objMaster.Scripts_AfterBody;
                    activity.Scripts_Footer = objMaster.Scripts_Footer;
                    activity.Template_Header = objMaster.Template_Header;
                    activity.Template_Footer = objMaster.Template_Footer;
                    activity.Template_Logo = objMaster.Template_Logo;
                    activity.Template_Phone_Desktop = objMaster.Template_Phone_Desktop;
                    activity.Template_Phone_Mobile = objMaster.Template_Phone_Mobile;
                    activity.CanonicalDomain = objMaster.CanonicalDomain;
                }
                tblSeoItems objSeo = PageDataModel.GetSeo(1, id, culture);
                if (objSeo != null)
                {
                    activity.Seo_Title = objSeo.title;
                    activity.Seo_Keywords = objSeo.keywords;
                    activity.Seo_Description = objSeo.description;
                    activity.Seo_Index = (objSeo.index_ ? "index" : "noindex");
                    activity.Seo_Follow = (objSeo.follow ? "follow" : "nofollow");
                    activity.Seo_FriendlyUrl = objSeo.friendlyUrl;
                }
                tblSeoItems objSeo2 = PageDataModel.GetSeo(1, id, (culture == "es-MX" ? "en-US" : "es-MX"));
                if (objSeo2 != null)
                {
                    activity.Seo_OppositeLanguageUrl = objSeo2.friendlyUrl;
                }
                //Submenus
                SubmenusViewModel objSubmenus = PageDataModel.GetSubmenus();
                if (objSubmenus != null)
                {
                    activity.Submenu1 = objSubmenus.Submenu1;
                    activity.Submenu2 = objSubmenus.Submenu2;
                    activity.Submenu3 = objSubmenus.Submenu3;
                }

                return activity;
            }
            else
            {
                return null;
            }
        }

        public DateTime GetNextActivePriceDate(long id, long? terminalid = null)
        {
            if (terminalid == null)
            {
                terminalid = Utils.GeneralFunctions.GetTerminalID();
            }

            int? baseCurrencyID = (from service in db.tblServices
                                   join provider in db.tblProviders on service.providerID equals provider.providerID
                                   join currency in db.tblContractsCurrencyHistory on provider.providerID equals currency.providerID
                                   where service.serviceID == id
                                   && currency.dateSaved <= DateTime.Now
                                   orderby currency.dateSaved descending
                                   select currency.contractCurrencyID).FirstOrDefault();

            if (baseCurrencyID == null)
            {
                if (Utils.GeneralFunctions.GetCulture() == "es-MX")
                {
                    baseCurrencyID = 2;
                }
                else
                {
                    baseCurrencyID = 1;
                }
            }

            List<PriceRuleModel> Rules = PriceDataModel.GetRules(id, terminalid, DateTime.Now);
            var baseRule = Rules.Where(r => r.IsBasePrice == true);
            int basePriceTypeID = (baseRule.Count() > 0 ? baseRule.FirstOrDefault().PriceTypeID : 1);

            var ActivePrices = from p in db.tblPrices
                               where p.itemID == id
                               && (p.sysItemTypeID == 1 || p.sysItemTypeID == 3)
                               && p.fromDate <= DateTime.Now
                               && (p.permanent_ == true || p.toDate >= DateTime.Now)
                               && p.twFromDate <= DateTime.Now
                               && (p.twPermanent_ == true || p.twToDate >= DateTime.Now)
                               && p.currencyID == baseCurrencyID
                               && p.priceTypeID == basePriceTypeID
                               select p.priceID;

            if (ActivePrices.Count() > 0)
            {
                return DateTime.Today;
            }
            else
            {
                var NextDate = (from p in db.tblPrices
                                where p.itemID == id
                                && (p.sysItemTypeID == 1 || p.sysItemTypeID == 3)
                                && p.fromDate <= DateTime.Now
                                && (p.permanent_ == true || p.toDate >= DateTime.Now)
                                && p.twFromDate > DateTime.Now
                                && p.currencyID == baseCurrencyID
                                && p.priceTypeID == basePriceTypeID
                                select p.twFromDate).FirstOrDefault();
                if (NextDate != null)
                {
                    return NextDate;
                }
                else
                {
                    return DateTime.Today;
                }
            }
        }

        public IEnumerable<ReviewListItem> GetReviews(int itemTypeId, long id)
        {
            List<ReviewListItem> reviews = new List<ReviewListItem>();
            string culture = Utils.GeneralFunctions.GetCulture();
            var ReviewsQ = from r in db.tblReviews
                           where r.sysItemTypeID == itemTypeId
                           && r.itemID == id
                           && r.active == true
                           select r;

            foreach (var review in ReviewsQ)
            {
                reviews.Add(new ReviewListItem()
                {
                    Review = review.review,
                    Picture = review.picture,
                    Rating = review.rating,
                    Author = review.author,
                    From = review.from_,
                    Saved = String.Format("{0:MMM d, yyyy}", (DateTime)review.dateSaved)
                });
            }

            return reviews;
        }

        public IEnumerable<AlternativeListItem> GetAlternativeActivities(long id)
        {
            List<AlternativeListItem> activities = new List<AlternativeListItem>();
            string culture = Utils.GeneralFunctions.GetCulture();
            var ActivitiesQ = (from a in db.tblCategories_Services
                               where (from x in db.tblCategories_Services
                                      where x.serviceID == id
                                      select x.categoryID).Contains(a.categoryID)
                               select new
                               {
                                   a.serviceID,
                                   a.tblServices.tblServiceDescriptions.FirstOrDefault(d => d.culture == culture).service
                               }).Take(3);

            foreach (var activity in ActivitiesQ)
            {
                AlternativeListItem newActivity = new AlternativeListItem();
                newActivity.Activity = activity.service;
                //url
                string url = PageDataModel.GetUrl(1, activity.serviceID);
                if (url != "")
                {
                    newActivity.Url = url;
                }
                else
                {
                    newActivity.Url = "/activities/detail/" + id;
                }
                //picture
                newActivity.Picture = PictureDataModel.GetMainPicture(1, id);
            }

            return activities;
        }

        public IEnumerable<MeetingPointListItem> GetMeetingPoints(long id)
        {
            List<MeetingPointListItem> places = new List<MeetingPointListItem>();

            var PlacesQ = (from p in db.tblMeetingPoints
                           where p.serviceID == id
                           && p.active
                           && (p.tblWeeklyAvailability.permanent_
                           || (p.tblWeeklyAvailability.fromDate <= DateTime.Now && p.tblWeeklyAvailability.toDate > DateTime.Now))
                           select new { p.tblPlaces.place, p.tblPlaces.lat, p.tblPlaces.lng, p.atYourHotel, p.tblPlaces.address, p.tblPlaces.tblDestinations.destination }).Distinct();

            foreach (var place in PlacesQ)
            {
                if (place.atYourHotel)
                {
                    places.Add(new MeetingPointListItem()
                    {
                        Place = ePlatBack.Models.Resources.Models.Shared.SharedStrings.At_your_hotel,
                        Lat = "",
                        Lng = "",
                        Address = "",
                        City = ""
                    });
                }
                else
                {
                    places.Add(new MeetingPointListItem()
                    {
                        Place = place.place,
                        Lat = place.lat,
                        Lng = place.lng,
                        Address = place.address,
                        City = place.destination
                    });
                }
            }

            return places;
        }

        public IEnumerable<ScheduleListItem> GetSchedules(long id)
        {
            List<ScheduleListItem> schedules = new List<ScheduleListItem>();
            string monday = "";
            string tuesday = "";
            string wednesday = "";
            string thursday = "";
            string friday = "";
            string saturday = "";
            string sunday = "";
            string hour = "";
            string toHour = "";

            var SchedulesQ = from s in db.tblWeeklyAvailability
                             where s.serviceID == id
                             && (s.permanent_ == true || (s.fromDate <= DateTime.Now && s.toDate >= DateTime.Now))
                             orderby s.hour
                             select s;

            foreach (tblWeeklyAvailability schedule in SchedulesQ)
            {
                hour = Utils.GeneralFunctions.DateFormat.ToMeridianHour(schedule.hour.ToString());
                toHour = Utils.GeneralFunctions.DateFormat.ToMeridianHour(schedule.toHour.ToString());

                if (schedule.monday)
                {
                    monday += (monday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }
                if (schedule.tuesday)
                {
                    tuesday += (tuesday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }
                if (schedule.wednesday)
                {
                    wednesday += (wednesday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }
                if (schedule.thursday)
                {
                    thursday += (thursday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }
                if (schedule.friday)
                {
                    friday += (friday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }
                if (schedule.saturday)
                {
                    saturday += (saturday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }
                if (schedule.sunday)
                {
                    sunday += (sunday != "" ? ", " : "") + GetScheduleItem(schedule.range, hour, toHour);
                }

            }

            schedules.Add(new ScheduleListItem()
            {
                Day = "Monday",
                Time = monday
            });
            schedules.Add(new ScheduleListItem()
            {
                Day = "Tuesday",
                Time = tuesday
            });
            schedules.Add(new ScheduleListItem()
            {
                Day = "Wednesday",
                Time = wednesday
            });
            schedules.Add(new ScheduleListItem()
            {
                Day = "Thursday",
                Time = thursday
            });
            schedules.Add(new ScheduleListItem()
            {
                Day = "Friday",
                Time = friday
            });
            schedules.Add(new ScheduleListItem()
            {
                Day = "Saturday",
                Time = saturday
            });
            schedules.Add(new ScheduleListItem()
            {
                Day = "Sunday",
                Time = sunday
            });

            return schedules;
        }

        public string GetScheduleItem(bool range, string hour, string toHour)
        {
            string time = "";
            if (range)
            {
                time = ePlatBack.Models.Resources.Models.Shared.SharedStrings.From + " " + hour + " " + ePlatBack.Models.Resources.Models.Shared.SharedStrings.to + " " + toHour;
            }
            else
            {
                time = hour;
            }
            return time;
        }

        public long GetItemType(long id)
        {
            long itemType = 1;
            var isTransportation = (from t in db.tblServices
                                    where t.serviceID == id
                                    select t.transportationService).Single();

            if (isTransportation)
            {
                itemType = 3;
            }

            return itemType;
        }

        public bool OffersRoundTransportation(long id)
        {
            var offersRound = (from o in db.tblServices
                               where o.serviceID == id
                               select o.offersRoundTrip).FirstOrDefault();

            return offersRound;
        }

        public IEnumerable<PriceListItem> GetPrices(long id, DateTime activityDate, long? terminalid = null, string culture = "en-US")
        {
            return GetPrices(id, activityDate, null, null, terminalid, culture);
        }

        public IEnumerable<PriceListItem> GetPrices(long id, DateTime activityDate, long? zoneId, bool? round, long? terminalID = null, string culture = "en-US", bool orderAscending = false)
        {
            List<PriceListItem> prices = new List<PriceListItem>();
            int currencyID = 1;
            if (culture == "es-MX")
            {
                currencyID = 2;
            }
            if (terminalID == null)
            {
                terminalID = Utils.GeneralFunctions.GetTerminalID();
            }

            //obtener tipos de precio disponibles para el dominio
            string domain = System.Web.HttpContext.Current.Request.Url.Host;
            int? retailPriceTypeID = null;
            int? offerPriceTypeID = null;
            List<int> priceTypes = new List<int>();

            int pointOfSaleID = PurchaseDataModel.GetPointOfSaleID(terminalID);

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

            //var ComputedPrices = PriceDataModel.GetComputedPrices(id, activityDate, terminalID, true, DateTime.Now, culture);
            var ComputedPrices = PriceDataModel.GetComputedPrices(id, activityDate, 99999, terminalID, DateTime.Now, culture);
            var PricesQ = from p in ComputedPrices
                          where priceTypes.Contains(p.PriceTypeID)
                          //&& p.DependingOnPriceID == null
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

            if (zoneId != null && zoneId != 0)
            {
                PricesQ = PricesQ.Where(x => x.ToTransportationZoneID == zoneId);
            }

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

                    if (round == true)
                    {
                        if (culture == "en-US" && newPrice.Unit.IndexOf("ound") >= 0)
                        {
                            prices.Add(newPrice);
                        }
                        else if (culture == "es-MX" && newPrice.Unit.IndexOf("edondo") >= 0)
                        {
                            prices.Add(newPrice);
                        }
                    }
                    else if (round == false)
                    {
                        if ((culture == "en-US" && newPrice.Unit.IndexOf("ound") == -1) || (culture == "es-MX" && newPrice.Unit.IndexOf("edondo") == -1))
                        {
                            prices.Add(newPrice);
                        }
                    }
                    else
                    {
                        prices.Add(newPrice);
                    }
                    retail = 0;
                }
            }

            if (orderAscending)
            {
                prices = prices.OrderBy(x => x.OfferPrice).ToList();
            }
            else
            {
                prices = prices.OrderByDescending(x => x.OfferPrice).ToList();
            }

            return prices;
        }

        public decimal GetRating(int itemTypeID, long id)
        {
            decimal rating = 0;
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
            else
            {
                rating = 0;
            }

            return rating;
        }

        public void GetBestPricesFromCache(long serviceID, int currencyID, ref decimal retailPrice, ref decimal offerPrice, ref int savings, long? terminalID = null)
        {
            if (terminalID == null)
            {
                terminalID = Utils.GeneralFunctions.GetTerminalID();
            }
            string culture = Utils.GeneralFunctions.GetCulture();
            decimal offerCheapest = 0;
            decimal retailCheapest = 0;

            //obtener tipos de precio disponibles para el dominio
            string domain = System.Web.HttpContext.Current.Request.Url.Host;
            int? retailPriceTypeID = null;
            int? offerPriceTypeID = null;
            List<int> priceTypes = new List<int>();

            int pointOfSaleID = PurchaseDataModel.GetPointOfSaleID(terminalID);

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
                          where p.serviceID == serviceID
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
                foreach (var price in PricesQ)
                {
                    if (price.priceTypeID == offerPriceTypeID)
                    {
                        if (offerCheapest == 0 || price.price < offerCheapest)
                        {
                            offerPrice = price.price;
                            offerCheapest = offerPrice;
                        }
                    }
                    else if (price.priceTypeID == retailPriceTypeID)
                    {
                        if (retailCheapest == 0 || price.price < retailCheapest)
                        {
                            retailPrice = price.price;
                            retailCheapest = retailPrice;
                        }
                    }
                }
            }
            else
            {
                var PricesChildQ = (from p in db.tblServices_PricesCache
                                    where p.serviceID == serviceID
                                    && p.price > 0
                                    && priceTypes.Contains(p.priceTypeID)
                                    && p.currencyID == currencyID
                                    orderby p.price ascending, p.priceTypeID descending
                                    select new
                                    {
                                        p.price,
                                        p.priceTypeID
                                    }).Take(2);

                foreach (var price in PricesChildQ)
                {
                    if (price.priceTypeID == offerPriceTypeID)
                    {
                        if (offerCheapest == 0 || price.price < offerCheapest)
                        {
                            offerPrice = price.price;
                            offerCheapest = offerPrice;
                        }
                    }
                    else if (price.priceTypeID == retailPriceTypeID)
                    {
                        if (retailCheapest == 0 || price.price < retailCheapest)
                        {
                            retailPrice = price.price;
                            retailCheapest = retailPrice;
                        }
                    }
                }
            }

            savings = Convert.ToInt32(retailPrice > 0 ? (100 - (offerPrice * 100 / retailPrice)) : 0);

        }


        public void GetBestPrices(long serviceID, int currencyID, ref decimal retailPrice, ref decimal offerPrice, ref int savings, long? terminalID = null)
        {
            if (terminalID == null)
            {
                terminalID = Utils.GeneralFunctions.GetTerminalID();
            }
            string culture = Utils.GeneralFunctions.GetCulture();
            DateTime nextActivePriceDate = GetNextActivePriceDate(serviceID);

            var ComputedPrices = PriceDataModel.GetComputedPrices(serviceID, nextActivePriceDate, 99999, terminalID, DateTime.Now, culture);
            decimal offerCheapest = 0;
            decimal retailCheapest = 0;

            var PricesQ = from p in ComputedPrices
                          where p.Price > 0
                          && (p.PriceTypeID == 1 || p.PriceTypeID == 2)
                          && p.CurrencyID == currencyID
                          && !p.Unit.ToLower().Contains("child")
                          && !p.Unit.ToLower().Contains("kid")
                          && !p.Unit.ToLower().Contains("niño")
                          orderby p.PriceID, p.PriceTypeID descending
                          select new
                          {
                              p.Price,
                              p.PriceTypeID,
                              p.Unit
                          };

            if (PricesQ.Count() > 0)
            {
                foreach (var price in PricesQ)
                {
                    if (price.PriceTypeID == 2)
                    {
                        if (offerCheapest == 0 || price.Price < offerCheapest)
                        {
                            offerPrice = price.Price;
                            offerCheapest = offerPrice;
                        }
                    }
                    else if (price.PriceTypeID == 1)
                    {
                        if (retailCheapest == 0 || price.Price < retailCheapest)
                        {
                            retailPrice = price.Price;
                            retailCheapest = retailPrice;
                        }
                    }
                }
            }
            else
            {
                var PricesChildQ = (from p in ComputedPrices
                                    where p.Price > 0
                                    && (p.PriceTypeID == 1 || p.PriceTypeID == 2)
                                    && p.CurrencyID == currencyID
                                    orderby p.Price ascending, p.PriceTypeID descending
                                    select new
                                    {
                                        p.Price,
                                        p.PriceTypeID
                                    }).Take(2);

                foreach (var price in PricesChildQ)
                {
                    if (price.PriceTypeID == 2)
                    {
                        if (offerCheapest == 0 || price.Price < offerCheapest)
                        {
                            offerPrice = price.Price;
                            offerCheapest = offerPrice;
                        }
                    }
                    else if (price.PriceTypeID == 1)
                    {
                        if (retailCheapest == 0 || price.Price < retailCheapest)
                        {
                            retailPrice = price.Price;
                            retailCheapest = retailPrice;
                        }
                    }
                }
            }

            savings = Convert.ToInt32(retailPrice > 0 ? (100 - (offerPrice * 100 / retailPrice)) : 0);
        }

        public IEnumerable<ActivitiesByCategory> GetActivitiesForPromo(long promoid)
        {
            List<ActivitiesByCategory> Categories = new List<ActivitiesByCategory>();
            string culture = Utils.GeneralFunctions.GetCulture();

            int currencyID = 1;
            if (culture == "es-MX")
            {
                currencyID = 2;
            }

            List<long> PromoItems = (from p in db.tblPromos_RelatedItems
                                     where p.promoID == promoid
                                     select p.itemID).ToList();

            var ActivitiesQ = from a in db.tblPromos_RelatedItems
                              join service in db.tblServices on a.itemID equals service.serviceID
                              join promo in db.tblPromos on a.promoID equals promo.promoID
                              join provider in db.tblProviders on service.providerID equals provider.providerID
                              join szone in db.tblZones on service.zoneID equals szone.zoneID
                              into serviceZone
                              from szone in serviceZone.DefaultIfEmpty()
                              join description in db.tblServiceDescriptions on service.serviceID equals description.serviceID
                              into serviceDescription
                              from description in serviceDescription.DefaultIfEmpty()
                              where a.promoID == promoid
                              && provider.isActive == true
                              && service.deleted != true
                              && description.active && description.culture == culture
                              select new
                              {
                                  ActivityID = service.serviceID,
                                  ActivityName = description.service,
                                  ZoneID = service.zoneID,
                                  Zone = szone.zone,
                                  Tag = description.tag,
                                  TagColor = service.tagColor,
                                  TagColorProvider = provider.tagColor,
                                  PromoPercentage = promo.percentage,
                                  CategoryName = service.tblCategories_Services.FirstOrDefault().tblCategories.category,
                                  CategoryID = service.tblCategories_Services.FirstOrDefault().categoryID,
                                  TerminalID = service.originalTerminalID
                              };

            ActivitiesQ = ActivitiesQ.Distinct();

            foreach (var activity in ActivitiesQ)
            {

                if (activity.ActivityName != null)
                {
                    if (Categories.Count(x => x.CategoryID == activity.CategoryID) == 0)
                    {
                        ActivitiesByCategory newCategory = new ActivitiesByCategory();
                        newCategory.CategoryID = activity.CategoryID;
                        newCategory.Category = activity.CategoryName;
                        newCategory.Activities = new List<ActivityListItem>();
                        Categories.Add(newCategory);
                    }

                    ActivityListItem newActivity = new ActivityListItem();
                    newActivity.ActivityID = activity.ActivityID;
                    newActivity.Activity = activity.ActivityName;
                    newActivity.ZoneID = activity.ZoneID;
                    newActivity.Zone = activity.Zone;
                    string Url = PageDataModel.GetUrl(1, activity.ActivityID);
                    newActivity.Url = (Url != "" ? Url : "/activities/detail/" + activity.ActivityID);
                    newActivity.Pictures = PictureDataModel.GetPictures(1, activity.ActivityID);
                    newActivity.Rating = GetRating(1, activity.ActivityID);
                    decimal retailPrice = 0;
                    decimal offerPrice = 0;
                    int savings = 0;
                    GetBestPricesFromCache(activity.ActivityID, currencyID, ref retailPrice, ref offerPrice, ref savings, activity.TerminalID);
                    newActivity.RetailPrice = retailPrice;
                    newActivity.OfferPrice = offerPrice;
                    newActivity.Currency = (currencyID == 1 ? "USD" : "MXN");
                    if (activity.PromoPercentage != null)
                    {
                        newActivity.PromoPrice = decimal.Round(offerPrice - (offerPrice * (decimal)activity.PromoPercentage / 100), 2);
                        newActivity.PromoSavings = Convert.ToInt32(retailPrice > 0 ? (100 - (newActivity.PromoPrice * 100 / retailPrice)) : 0);
                    }
                    newActivity.Savings = savings;

                    newActivity.Tag = activity.Tag;
                    if (activity.TagColor != null && activity.TagColor != "")
                    {
                        newActivity.TagColor = activity.TagColor;
                    }
                    else
                    {
                        newActivity.TagColor = activity.TagColorProvider;
                    }
                    Categories.FirstOrDefault(x => x.CategoryID == activity.CategoryID).Activities.Add(newActivity);
                }
            }

            Categories = Categories.OrderBy(x => x.Category).ToList();

            foreach (var cat in Categories)
            {
                cat.Activities = cat.Activities.OrderBy(x => x.PromoPrice).ToList();
            }

            return Categories;
        }

        public IEnumerable<ActivityListItem> GetActivitiesListWithCache(long categoryid, long terminalid, string z)
        {
            return GetActivitiesListWithCache(categoryid, null, "", terminalid, z);
        }

        public IEnumerable<ActivityListItem> GetActivitiesListWithCache(long? categoryid, long? providerid, string culture = "", long? terminalid = null, string zoneName = null)
        {
            List<ActivityListItem> activities = new List<ActivityListItem>();
            if (culture == "")
            {
                culture = Utils.GeneralFunctions.GetCulture();
            }
            int currencyID = 1;
            if (culture == "es-MX")
            {
                currencyID = 2;
            }

            string zone = "";
            if (zoneName != null)
            {
                zone = zoneName.Replace("-", " ");
            }

            var ActivitiesQ = (from a in db.tblCategories_Services
                               join service in db.tblServices on a.serviceID equals service.serviceID
                               join provider in db.tblProviders on service.providerID equals provider.providerID
                               join szone in db.tblZones on service.zoneID equals szone.zoneID
                               into serviceZone
                               from szone in serviceZone.DefaultIfEmpty()
                               join description in db.tblServiceDescriptions on service.serviceID equals description.serviceID
                               into serviceDescription
                               from description in serviceDescription.DefaultIfEmpty()
                               where (a.categoryID == categoryid || categoryid == null)
                               && service.originalTerminalID == terminalid
                               && (service.providerID == providerid || providerid == null && provider.isActive == true)
                               && service.deleted != true
                               && (szone.zone == zone || zoneName == null || service.zoneID == null)
                               && description.active && description.culture == culture
                               select new
                               {
                                   ActivityID = a.serviceID,
                                   ActivityName = description.service,
                                   ZoneID = service.zoneID,
                                   Zone = szone.zone,
                                   Tag = description.tag,
                                   TagColor = service.tagColor,
                                   TagColorProvider = provider.tagColor,
                                   Provider = provider.comercialName
                               }).Distinct();

            foreach (var activity in ActivitiesQ)
            {
                if (activity.ActivityName != null)
                {
                    ActivityListItem newActivity = new ActivityListItem();
                    newActivity.ActivityID = activity.ActivityID;
                    newActivity.Activity = activity.ActivityName;
                    newActivity.ZoneID = activity.ZoneID;
                    newActivity.Zone = activity.Zone;
                    string Url = PageDataModel.GetUrl(1, activity.ActivityID);
                    newActivity.Url = (Url != "" ? Url : "/activities/detailv2/" + activity.ActivityID);
                    newActivity.Pictures = PictureDataModel.GetMainPictureItem(1, activity.ActivityID);
                    newActivity.Rating = GetRating(1, activity.ActivityID);
                    decimal retailPrice = 0;
                    decimal offerPrice = 0;
                    int savings = 0;
                    GetBestPricesFromCache(activity.ActivityID, currencyID, ref retailPrice, ref offerPrice, ref savings, terminalid);
                    newActivity.RetailPrice = retailPrice;
                    newActivity.OfferPrice = offerPrice;
                    newActivity.Currency = (currencyID == 1 ? "USD" : "MXN");
                    newActivity.Savings = savings;
                    newActivity.Tag = activity.Tag;
                    if (activity.TagColor != null && activity.TagColor != "")
                    {
                        newActivity.TagColor = activity.TagColor;
                    }
                    else
                    {
                        newActivity.TagColor = activity.TagColorProvider;
                    }
                    newActivity.Provider = activity.Provider;
                    activities.Add(newActivity);
                }
            }

            activities = activities.OrderBy(a => a.OfferPrice).ToList();

            return activities;
        }

        public IEnumerable<ActivityListItem> GetActivitiesList(long categoryid, string z)
        {
            return GetActivitiesList(categoryid, null, "", null, z);
        }

        public IEnumerable<ActivityListItem> GetActivitiesList(long? categoryid, long? providerid, string culture = "", long? terminalid = null, string zoneName = null)
        {
            List<ActivityListItem> activities = new List<ActivityListItem>();
            if (culture == "")
            {
                culture = Utils.GeneralFunctions.GetCulture();
            }
            int currencyID = 1;
            if (culture == "es-MX")
            {
                currencyID = 2;
            }

            string zone = "";
            if (zoneName != null)
            {
                zone = zoneName.Replace("-", " ");
            }

            var ActivitiesQ = (from a in db.tblCategories_Services
                               join service in db.tblServices on a.serviceID equals service.serviceID
                               join provider in db.tblProviders on service.providerID equals provider.providerID
                               join szone in db.tblZones on service.zoneID equals szone.zoneID
                               into serviceZone
                               from szone in serviceZone.DefaultIfEmpty()
                               join description in db.tblServiceDescriptions on service.serviceID equals description.serviceID
                               into serviceDescription
                               from description in serviceDescription.DefaultIfEmpty()
                               where (a.categoryID == categoryid || categoryid == null)
                               && (service.providerID == providerid || providerid == null && provider.isActive == true)
                               && service.deleted != true
                               && (szone.zone == zone || zoneName == null || service.zoneID == null)
                               && description.active && description.culture == culture
                               select new
                               {
                                   ActivityID = a.serviceID,
                                   ActivityName = description.service,
                                   ZoneID = service.zoneID,
                                   Zone = szone.zone,
                                   Tag = description.tag,
                                   TagColor = service.tagColor,
                                   TagColorProvider = provider.tagColor
                               }).Distinct();

            foreach (var activity in ActivitiesQ)
            {
                if (activity.ActivityName != null)
                {
                    ActivityListItem newActivity = new ActivityListItem();
                    newActivity.ActivityID = activity.ActivityID;
                    newActivity.Activity = activity.ActivityName;
                    newActivity.ZoneID = activity.ZoneID;
                    newActivity.Zone = activity.Zone;
                    string Url = PageDataModel.GetUrl(1, activity.ActivityID);
                    newActivity.Url = (Url != "" ? Url : "/activities/detail/" + activity.ActivityID);
                    newActivity.Pictures = PictureDataModel.GetPictures(1, activity.ActivityID);
                    newActivity.Rating = GetRating(1, activity.ActivityID);
                    decimal retailPrice = 0;
                    decimal offerPrice = 0;
                    int savings = 0;
                    GetBestPrices(activity.ActivityID, currencyID, ref retailPrice, ref offerPrice, ref savings, terminalid);
                    newActivity.RetailPrice = retailPrice;
                    newActivity.OfferPrice = offerPrice;
                    newActivity.Savings = savings;
                    newActivity.Tag = activity.Tag;
                    if (activity.TagColor != null && activity.TagColor != "")
                    {
                        newActivity.TagColor = activity.TagColor;
                    }
                    else
                    {
                        newActivity.TagColor = activity.TagColorProvider;
                    }
                    activities.Add(newActivity);
                }
            }

            activities = activities.OrderBy(a => a.OfferPrice).ToList();

            return activities;
        }

        /*Booking Engine Activity Methods*/
        public string GetAvailableDates(long id, int mes, int ano, long? terminalid = null)
        {
            string dates = "";
            bool[,] mesDisp = new bool[32, 13];
            bool[,] precioDisp = new bool[32, 13];

            for (int j = 1; j <= 12; j++)
            {
                for (int i = 0; i <= 31; i++)
                {
                    mesDisp[i, j] = false;
                    precioDisp[i, j] = false;
                }
            }

            DateTime fechaInicio = new DateTime();
            DateTime fechaFinal = new DateTime();
            DateTime fechaActual = new DateTime();
            DateTime now = DateTime.Now;

            //check by available price
            DateTime startingDate = new DateTime(ano, mes, 1);
            DateTime finishingDate = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

            if (terminalid == null)
            {
                terminalid = Utils.GeneralFunctions.GetTerminalID();
            }

            List<PriceRuleModel> Rules = PriceDataModel.GetRules(id, terminalid, startingDate);

            //obtener el tipo de precio base
            var baseRule = Rules.Where(r => r.IsBasePrice == true);
            int basePriceTypeID = (baseRule.Count() > 0 ? baseRule.FirstOrDefault().PriceTypeID : 1);
            int providerID = db.tblServices.Where(x => x.serviceID == id).Select(x => x.providerID).FirstOrDefault();
            int? contractCurrencyID = (from c in db.tblContractsCurrencyHistory
                                          where c.dateSaved <= finishingDate
                                       //where c.dateSaved <= now
                                       && c.providerID == providerID
                                       orderby c.dateSaved descending
                                       select c.contractCurrencyID).FirstOrDefault();


            if (contractCurrencyID == null)
            {
                string culture = Utils.GeneralFunctions.GetCulture();
                if (culture == "en-US")
                {
                    contractCurrencyID = 1;
                }
                else
                {
                    contractCurrencyID = 2;
                }
            }

            var dispPrecio = from p in db.tblPrices
                             where p.itemID == id
                             && p.currencyID == contractCurrencyID
                             && p.priceTypeID == basePriceTypeID
                             && (p.sysItemTypeID == 1 || p.sysItemTypeID == 3)
                             && (p.twPermanent_ == true && p.twFromDate < finishingDate
                             || (p.twToDate > startingDate && p.twFromDate < finishingDate))
                             && (p.permanent_ == true && p.fromDate <= DateTime.Now
                             || (p.toDate > DateTime.Now && p.fromDate <= DateTime.Now))
                             select p;

            foreach (tblPrices d2 in dispPrecio)
            {
                if (d2.twPermanent_)
                {
                    if (d2.twFromDate < new DateTime(ano, mes, 1))
                    {
                        fechaInicio = new DateTime(ano, mes, 1);
                    }
                    else
                    {
                        fechaInicio = (DateTime)d2.twFromDate;
                    }
                    fechaFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                }
                else
                {
                    if (d2.twFromDate < new DateTime(ano, mes, 1))
                    {
                        fechaInicio = new DateTime(ano, mes, 1);
                    }
                    else
                    {
                        fechaInicio = (DateTime)d2.twFromDate;
                    }
                    if (d2.twToDate > new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)))
                    {
                        fechaFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                    }
                    else
                    {
                        fechaFinal = (DateTime)d2.twToDate;
                    }
                }
                fechaActual = fechaInicio;
                while (fechaActual <= fechaFinal)
                {


                    precioDisp[fechaActual.Day, fechaActual.Month] = true;
                    fechaActual = fechaActual.AddDays(1);
                }
            }

            //checar por horario disponible y dia de la semana
            var disponibilidad = from d in db.tblWeeklyAvailability
                                 where (d.permanent_ == true
                                || (d.toDate > startingDate && d.fromDate <= finishingDate))
                                 && d.serviceID == id
                                 select d;

            if (disponibilidad.Count() > 0)
            {
                int i = 0;
                foreach (tblWeeklyAvailability d1 in disponibilidad)
                {
                    if (d1.permanent_)
                    {
                        fechaInicio = startingDate;
                        fechaFinal = finishingDate;
                    }
                    else
                    {
                        if (d1.fromDate < new DateTime(ano, mes, 1))
                        {
                            fechaInicio = new DateTime(ano, mes, 1);
                        }
                        else
                        {
                            fechaInicio = (DateTime)d1.fromDate;
                        }
                        if (d1.toDate > new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)))
                        {
                            fechaFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                        }
                        else
                        {
                            fechaFinal = (DateTime)d1.toDate;
                        }
                    }

                    fechaActual = fechaInicio;
                    while (fechaActual <= fechaFinal)
                    {
                        switch (fechaActual.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                if (d1.sunday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Monday:
                                if (d1.monday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Tuesday:
                                if (d1.tuesday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if (d1.wednesday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (d1.thursday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (d1.friday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Saturday:
                                if (d1.saturday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                        }
                        fechaActual = fechaActual.AddDays(1);
                    }
                    i++;
                }
            }
            else
            {
                disponibilidad = from d in db.tblWeeklyAvailability
                                 where d.serviceID == id
                                 select d;

                if (disponibilidad.Count() == 0)
                {
                    fechaInicio = startingDate;
                    fechaFinal = finishingDate;
                    fechaActual = fechaInicio;

                    while (fechaActual <= fechaFinal)
                    {
                        switch (fechaActual.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Monday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Tuesday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Saturday:
                                if (precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                        }
                        fechaActual = fechaActual.AddDays(1);
                    }
                }
            }

            //checar por disponibilidad limitada
            var limitada = from l in db.tblBlackOutDates
                           where l.itemID == id
                           && l.sysItemTypeID == 1
                           && l.date >= startingDate
                           && l.date <= finishingDate
                           select l.date;

            foreach (DateTime l1 in limitada)
            {
                if (mesDisp[l1.Day, l1.Month])
                {
                    mesDisp[l1.Day, l1.Month] = false;
                }
            }

            //generar respuesta
            fechaActual = new DateTime(ano, mes, 1); //fechaActual = fechaInicio;
            while (fechaActual <= new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)))
            {
                if (fechaActual >= DateTime.Today.AddDays(1) && !mesDisp[fechaActual.Day, fechaActual.Month])
                {
                    if (dates != "")
                    {
                        dates += ",";
                    }
                    dates += fechaActual.Year + "-" + fechaActual.Month + "-" + fechaActual.Day;
                }
                fechaActual = fechaActual.AddDays(1);
            }
            return dates;
        }

        public string GetAvailableDatesV2(long id, int mes, int ano, long terminalid)
        {
            string dates = "";
            bool[,] mesDisp = new bool[32, 13];
            bool[,] precioDisp = new bool[32, 13];

            for (int j = 1; j <= 12; j++)
            {
                for (int i = 0; i <= 31; i++)
                {
                    mesDisp[i, j] = false;
                    precioDisp[i, j] = false;
                }
            }

            DateTime fechaInicio = new DateTime();
            DateTime fechaFinal = new DateTime();
            DateTime fechaActual = new DateTime();

            //check by available price
            DateTime startingDate = new DateTime(ano, mes, 1);
            DateTime finishingDate = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

            if (terminalid == null)
            {
                terminalid = Utils.GeneralFunctions.GetTerminalID();
            }

            List<PriceRuleModel> Rules = PriceDataModel.GetRules(id, terminalid, startingDate);

            //obtener el tipo de precio base
            var baseRule = Rules.Where(r => r.IsBasePrice == true);
            int basePriceTypeID = (baseRule.Count() > 0 ? baseRule.FirstOrDefault().PriceTypeID : 1);
            int providerID = db.tblServices.Where(x => x.serviceID == id).Select(x => x.providerID).FirstOrDefault();
            int? contractCurrencyID = (from c in db.tblContractsCurrencyHistory
                                           //where c.dateSaved <= startingDate
                                       where c.dateSaved <= finishingDate
                                       && c.providerID == providerID
                                       orderby c.dateSaved descending
                                       select c.contractCurrencyID).FirstOrDefault();


            if (contractCurrencyID == null)
            {
                string culture = Utils.GeneralFunctions.GetCulture();
                if (culture == "en-US")
                {
                    contractCurrencyID = 1;
                }
                else
                {
                    contractCurrencyID = 2;
                }
            }

            var dispPrecio = from p in db.tblPrices
                             where p.itemID == id
                             && p.currencyID == contractCurrencyID
                             && p.priceTypeID == basePriceTypeID
                             && (p.sysItemTypeID == 1 || p.sysItemTypeID == 3)
                             && (p.twPermanent_ == true && p.twFromDate < finishingDate
                             || (p.twToDate > startingDate && p.twFromDate < finishingDate))
                             && (p.permanent_ == true && p.fromDate <= DateTime.Now
                             || (p.toDate > DateTime.Now && p.fromDate <= DateTime.Now))
                             select p;

            foreach (tblPrices d2 in dispPrecio)
            {
                if (d2.twPermanent_)
                {
                    if (d2.twFromDate < new DateTime(ano, mes, 1))
                    {
                        fechaInicio = new DateTime(ano, mes, 1);
                    }
                    else
                    {
                        fechaInicio = (DateTime)d2.twFromDate;
                    }
                    fechaFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                }
                else
                {
                    if (d2.twFromDate < new DateTime(ano, mes, 1))
                    {
                        fechaInicio = new DateTime(ano, mes, 1);
                    }
                    else
                    {
                        fechaInicio = (DateTime)d2.twFromDate;
                    }
                    if (d2.twToDate > new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)))
                    {
                        fechaFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                    }
                    else
                    {
                        fechaFinal = (DateTime)d2.twToDate;
                    }
                }
                fechaActual = fechaInicio;
                while (fechaActual <= fechaFinal)
                {


                    precioDisp[fechaActual.Day, fechaActual.Month] = true;
                    fechaActual = fechaActual.AddDays(1);
                }
            }

            //checar por horario disponible y dia de la semana
            var disponibilidad = from d in db.tblWeeklyAvailability
                                 where (d.permanent_ == true
                                || (d.toDate > startingDate && d.fromDate <= finishingDate))
                                 && d.serviceID == id
                                 select d;

            if (disponibilidad.Count() > 0)
            {
                int i = 0;
                foreach (tblWeeklyAvailability d1 in disponibilidad)
                {
                    if (d1.permanent_)
                    {
                        fechaInicio = startingDate;
                        fechaFinal = finishingDate;
                    }
                    else
                    {
                        if (d1.fromDate < new DateTime(ano, mes, 1))
                        {
                            fechaInicio = new DateTime(ano, mes, 1);
                        }
                        else
                        {
                            fechaInicio = (DateTime)d1.fromDate;
                        }
                        if (d1.toDate > new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)))
                        {
                            fechaFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
                        }
                        else
                        {
                            fechaFinal = (DateTime)d1.toDate;
                        }
                    }

                    fechaActual = fechaInicio;
                    while (fechaActual <= fechaFinal)
                    {
                        switch (fechaActual.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                if (d1.sunday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Monday:
                                if (d1.monday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Tuesday:
                                if (d1.tuesday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if (d1.wednesday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (d1.thursday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (d1.friday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                            case DayOfWeek.Saturday:
                                if (d1.saturday && precioDisp[fechaActual.Day, fechaActual.Month])
                                {
                                    mesDisp[fechaActual.Day, fechaActual.Month] = true;
                                }
                                break;
                        }
                        fechaActual = fechaActual.AddDays(1);
                    }
                    i++;
                }
            }

            //checar por disponibilidad limitada
            var limitada = from l in db.tblBlackOutDates
                           where l.itemID == id
                           && l.sysItemTypeID == 1
                           && l.date >= startingDate
                           && l.date <= finishingDate
                           select l.date;

            foreach (DateTime l1 in limitada)
            {
                if (mesDisp[l1.Day, l1.Month])
                {
                    mesDisp[l1.Day, l1.Month] = false;
                }
            }

            //generar respuesta
            fechaActual = new DateTime(ano, mes, 1); //fechaActual = fechaInicio;
            while (fechaActual <= new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)))
            {
                if (fechaActual <= DateTime.Today.AddDays(1) || !mesDisp[fechaActual.Day, fechaActual.Month])
                {
                    if (dates != "")
                    {
                        dates += ",";
                    }
                    dates += "[" + fechaActual.Year + "," + (fechaActual.Month - 1) + "," + fechaActual.Day + "]";
                }
                fechaActual = fechaActual.AddDays(1);
            }
            dates = "[" + dates + "]";
            return dates;
        }

        public static List<MarketPlaceViewModel.ItemSchedule> GetItemSchedules(long id, DateTime date)
        {
            List<MarketPlaceViewModel.ItemSchedule> schedules = new List<MarketPlaceViewModel.ItemSchedule>();
            ePlatEntities db = new ePlatEntities();

            var scheduleQ = from d in db.tblWeeklyAvailability
                            where (d.permanent_ || (d.fromDate <= date && d.toDate >= date))
                            && d.serviceID == id
                            select d;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    scheduleQ = scheduleQ.Where(x => x.sunday);
                    break;
                case DayOfWeek.Monday:
                    scheduleQ = scheduleQ.Where(x => x.monday);
                    break;
                case DayOfWeek.Tuesday:
                    scheduleQ = scheduleQ.Where(x => x.tuesday);
                    break;
                case DayOfWeek.Wednesday:
                    scheduleQ = scheduleQ.Where(x => x.wednesday);
                    break;
                case DayOfWeek.Thursday:
                    scheduleQ = scheduleQ.Where(x => x.thursday);
                    break;
                case DayOfWeek.Friday:
                    scheduleQ = scheduleQ.Where(x => x.friday);
                    break;
                case DayOfWeek.Saturday:
                    scheduleQ = scheduleQ.Where(x => x.saturday);
                    break;
            }

            if (scheduleQ.Count() > 0)
            {
                if (scheduleQ.First().range)
                {
                    if (scheduleQ.First().hour < scheduleQ.First().toHour)
                    {
                        for (TimeSpan hour = scheduleQ.First().hour; hour < scheduleQ.First().toHour; hour = hour.Add(TimeSpan.FromMinutes((double)scheduleQ.First().everyXMinutes)))
                        {
                            schedules.Add(new MarketPlaceViewModel.ItemSchedule()
                            {
                                WeeklyAvailabilityID = scheduleQ.First().weeklyAvailabilityID,
                                Schedule = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour.ToString())
                            });
                        }
                    }
                    else
                    {
                        //actividades nocturnas que terminan en la madrugada
                        //noche
                        for (TimeSpan hour = scheduleQ.First().hour; hour < TimeSpan.Parse("23:59:59"); hour = hour.Add(TimeSpan.FromMinutes((double)scheduleQ.First().everyXMinutes)))
                        {
                            schedules.Add(new MarketPlaceViewModel.ItemSchedule()
                            {
                                WeeklyAvailabilityID = scheduleQ.First().weeklyAvailabilityID,
                                Schedule = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour.ToString())
                            });
                        }

                        //madrugada
                        for (TimeSpan hour = TimeSpan.Parse("00:00:00"); hour < scheduleQ.First().toHour; hour = hour.Add(TimeSpan.FromMinutes((double)scheduleQ.First().everyXMinutes)))
                        {
                            schedules.Add(new MarketPlaceViewModel.ItemSchedule()
                            {
                                WeeklyAvailabilityID = scheduleQ.First().weeklyAvailabilityID,
                                Schedule = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour.ToString())
                            });
                        }
                    }
                }
                else
                {
                    foreach (var schedule in scheduleQ.OrderBy(x => x.hour))
                    {
                        schedules.Add(new MarketPlaceViewModel.ItemSchedule()
                        {
                            WeeklyAvailabilityID = schedule.weeklyAvailabilityID,
                            Schedule = Utils.GeneralFunctions.DateFormat.ToMeridianHour(schedule.hour.ToString())
                        });
                    }
                }
            }

            return schedules;
        }

        public List<SelectListItem> GetSchedulesForDate(long id, DateTime date)
        {
            List<SelectListItem> schedules = new List<SelectListItem>();
            var scheduleQ = from d in db.tblWeeklyAvailability
                            where (d.permanent_ || (d.fromDate <= date && d.toDate >= date))
                            && d.serviceID == id
                            select d;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    scheduleQ = scheduleQ.Where(x => x.sunday);
                    break;
                case DayOfWeek.Monday:
                    scheduleQ = scheduleQ.Where(x => x.monday);
                    break;
                case DayOfWeek.Tuesday:
                    scheduleQ = scheduleQ.Where(x => x.tuesday);
                    break;
                case DayOfWeek.Wednesday:
                    scheduleQ = scheduleQ.Where(x => x.wednesday);
                    break;
                case DayOfWeek.Thursday:
                    scheduleQ = scheduleQ.Where(x => x.thursday);
                    break;
                case DayOfWeek.Friday:
                    scheduleQ = scheduleQ.Where(x => x.friday);
                    break;
                case DayOfWeek.Saturday:
                    scheduleQ = scheduleQ.Where(x => x.saturday);
                    break;
            }

            if (scheduleQ.Count() > 0)
            {
                schedules.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Choose please"
                });
                if (scheduleQ.First().range)
                {
                    if (scheduleQ.First().hour < scheduleQ.First().toHour)
                    {
                        int? everyXMinutes = scheduleQ.First().everyXMinutes;
                        if(everyXMinutes == null)
                        {
                            everyXMinutes = 15;
                        }
                        for (TimeSpan hour = scheduleQ.First().hour; hour < scheduleQ.First().toHour; hour = hour.Add(TimeSpan.FromMinutes((double)everyXMinutes)))
                        {
                            schedules.Add(new SelectListItem()
                            {
                                Value = scheduleQ.First().weeklyAvailabilityID.ToString(),
                                Text = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour.ToString())
                            });
                        }
                    }
                    else
                    {
                        //actividades nocturnas que terminan en la madrugada
                        //noche
                        for (TimeSpan hour = scheduleQ.First().hour; hour < TimeSpan.Parse("23:59:59"); hour = hour.Add(TimeSpan.FromMinutes((double)scheduleQ.First().everyXMinutes)))
                        {
                            schedules.Add(new SelectListItem()
                            {
                                Value = scheduleQ.First().weeklyAvailabilityID.ToString(),
                                Text = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour.ToString())
                            });
                        }

                        //madrugada
                        for (TimeSpan hour = TimeSpan.Parse("00:00:00"); hour < scheduleQ.First().toHour; hour = hour.Add(TimeSpan.FromMinutes((double)scheduleQ.First().everyXMinutes)))
                        {
                            schedules.Add(new SelectListItem()
                            {
                                Value = scheduleQ.First().weeklyAvailabilityID.ToString(),
                                Text = Utils.GeneralFunctions.DateFormat.ToMeridianHour(hour.ToString())
                            });
                        }
                    }
                }
                else
                {
                    foreach (var schedule in scheduleQ.OrderBy(x => x.hour))
                    {
                        schedules.Add(new SelectListItem()
                        {
                            Value = schedule.weeklyAvailabilityID.ToString(),
                            Text = Utils.GeneralFunctions.DateFormat.ToMeridianHour(schedule.hour.ToString())
                        });
                    }
                }
            }
            else
            {
                schedules.Add(new SelectListItem()
                {
                    Value = "-1",
                    Text = "There is no availability on the date you selected"
                });
            }

            return schedules;
        }

        /* Features */
        public List<IndexListItem> GetActivitiesIndex(long terminalid, string culture)
        {
            List<IndexListItem> list = new List<IndexListItem>();
            ePlatEntities db = new ePlatEntities();
            var activitiesQ = from a in db.tblServiceDescriptions
                              where a.tblServices.originalTerminalID == terminalid
                              && a.culture == culture
                              && a.active == true
                              && a.tblServices.deleted != true
                              && a.tblServices.tblProviders.isActive == true
                              select new
                              {
                                  activityid = a.serviceID,
                                  name = a.service,
                                  content = a.fullDescription + a.itinerary + a.includes + a.notes + a.recommendations + a.policies,
                                  meetingPoints = a.tblServices.tblMeetingPoints.Select(x => x.tblPlaces.place),
                                  provider = a.tblServices.tblProviders.comercialName
                              };

            foreach (var activity in activitiesQ)
            {
                string url = PageDataModel.GetUrl(1, activity.activityid);
                string points = "";
                foreach (string place in activity.meetingPoints)
                {
                    if (points != "")
                    {
                        points += ",";
                    }
                    points += place;
                }
                List<PictureListItem> pictures = PictureDataModel.GetPictures(1, activity.activityid);
                list.Add(new IndexListItem()
                {
                    ActivityID = activity.activityid,
                    Url = (url != "" ? url : "/activities/detailv2/" + activity.activityid),
                    Picture = (pictures.Count() > 0 ? pictures.First().Picture : ""),
                    Name = activity.name,
                    Content = activity.content,
                    MeetingPoints = points,
                    Provider = activity.provider
                });
            }

            return list;
        }

        public int GenerateFriendlyURLs(string terminals)
        {
            int counter = 0;
            string services = "";
            List<long> terminalIDs = terminals.Split(',').Select(long.Parse).ToList();
            var ActivitiesToProcess = from s in db.tblServices
                                      join url in db.tblSeoItems on s.serviceID equals url.itemID
                                      into service_url
                                      from url in service_url.DefaultIfEmpty()
                                      where terminalIDs.Contains(s.originalTerminalID)
                                      && url == null
                                      && s.tblCategories_Services.Count(x => x.tblCategories.showOnWebsite == true) > 1
                                      && (s.tblServiceDescriptions.Count(z => z.active == true) == 2
                                      || s.tblServiceDescriptions.Count(z => z.active == true) == 1)
                                      select s.serviceID;

            foreach (var serviceID in ActivitiesToProcess)
            {
                services += "," + serviceID;
                GenerateFriendlyURL(serviceID);
                counter++;
            }

            return counter;
        }

        public static bool GenerateFriendlyURL(long serviceID)
        {
            bool done = true;
            ePlatEntities db = new ePlatEntities();

            var ServiceDescriptions = from s in db.tblServiceDescriptions
                                      where s.serviceID == serviceID
                                      select new
                                      {
                                          s.serviceID,
                                          s.tblServices.originalTerminalID,
                                          s.service,
                                          s.culture
                                      };

            if (ServiceDescriptions.Count() > 0)
            {
                long Category = (from c in db.tblCategories_Services
                                 where c.serviceID == serviceID
                                 select c.tblCategories.categoryID
                                  ).FirstOrDefault();

                var CategoriesSeo = from x in db.tblSeoItems
                                    where x.sysItemTypeID == 8
                                    && x.terminalID == ServiceDescriptions.FirstOrDefault().originalTerminalID
                                    && x.itemID == Category
                                    select x;

                if (CategoriesSeo.Count() > 0)
                {
                    foreach (var lan in ServiceDescriptions)
                    {
                        string category = CategoriesSeo.FirstOrDefault(x => x.culture == lan.culture).friendlyUrl;
                        string serviceName = FilterURL(lan.service);
                        string url = category + "/" + serviceName;

                        var SeoItems = from s in db.tblSeoItems
                                       where s.terminalID == lan.originalTerminalID
                                       && s.friendlyUrl == url
                                       select s.seoItemID;

                        if (SeoItems.Count() == 0)
                        {
                            tblSeoItems newSeo = new tblSeoItems();
                            newSeo.title = lan.service;
                            newSeo.keywords = "";
                            newSeo.description = "";
                            newSeo.friendlyUrl = url;
                            newSeo.url = "/activities/detailv2/" + lan.serviceID;
                            newSeo.culture = lan.culture;
                            newSeo.index_ = true;
                            newSeo.follow = true;
                            newSeo.terminalID = lan.originalTerminalID;
                            newSeo.sysItemTypeID = 1;
                            newSeo.itemID = lan.serviceID;

                            db.tblSeoItems.AddObject(newSeo);
                        }
                    }
                    db.SaveChanges();
                }
            }

            return done;
        }

        public static string FilterURL(string url)
        {
            url = url.ToLower();
            url = url.Replace(" ", "-");
            url = url.Replace("---", "-");
            url = url.Replace("--", "-");
            url = url.Replace("?", "");
            url = url.Replace("!", "");
            url = url.Replace(",", "");
            url = url.Replace("á", "a");
            url = url.Replace("é", "e");
            url = url.Replace("í", "i");
            url = url.Replace("ó", "o");
            url = url.Replace("ú", "u");
            url = url.Replace("&", "and");
            url = url.Replace("ñ", "n");
            url = url.Replace("=", "");
            url = url.Replace("'", "");
            url = url.Replace("(", "");
            url = url.Replace("+", "plus");
            url = url.Replace(")", "");
            url = url.Replace("/-", "/");
            url = url.Replace(".", "");
            if (url.Substring(url.Length - 1, 1) == "-")
            {
                url = url.Substring(0, url.Length - 1);
            }
            return url;
        }
    }
}