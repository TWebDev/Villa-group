using System;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Reflection;
using System.Globalization;
using System.Configuration;
using System.Web.Security;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Data.EntityClient;
using ePlatBack.Models;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using ePlatBack.Models.Utils.Custom;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Cryptography.Xml;

namespace ePlatBack.Models.DataModels
{
    public class PreArrivalDataModel
    {
        public static UserSession session = new UserSession();
        public class PreArrivalCatalogs
        {
            public static List<SearchFields> GetFields()
            {
                ePlatEntities db = new ePlatEntities();
                PreArrivalSearchModel model = new PreArrivalSearchModel();
                var properties = model.GetType().GetProperties().Where(m => Reflection.HasCustomAttribute<DataBaseInfoAttribute>(m));
                var list = new List<SearchFields>();
                var tables = properties.Select(m => Reflection.GetCustomAttribute<DataBaseInfoAttribute>(m).Name);
                var fieldNames = properties.Select(m => Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name);

                var sysComponentID = 11597;

                var components = sysComponentID != 0 ? AdminDataModel.GetChildrenOfComponentID(new List<long>() { sysComponentID }) : new List<long>() { };

                var query = from c in db.tblSysComponents
                            join alias in db.tblSysComponentAliases on c.sysComponentID equals alias.sysComponentID
                            where alias.sysWorkGroupID == session.WorkGroupID
                            && tables.Contains(c.tableName)
                            && fieldNames.Contains(c.fieldName)
                            && (sysComponentID == 0 || components.Contains(c.sysComponentID))
                            select new
                            {
                                fieldID = c.sysComponentID,
                                component = c.sysComponent,
                                tableName = c.tableName,
                                fieldName = c.fieldName,
                                alias = alias.alias
                            };

                foreach (var field in query.OrderBy(m => m.fieldName))
                {
                    //var property = properties.FirstOrDefault(m => Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name == field.fieldName);
                    var property = properties.Count(m => Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name == field.fieldName) > 1 ? properties.FirstOrDefault(m => Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name == field.fieldName && m.Name == field.component) : properties.FirstOrDefault(m => Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name == field.fieldName);
                    var filterID = "";
                    if (field.fieldName.IndexOf("ID") > 0 && field.fieldName != "reservationID")
                    {
                        var attr = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(property);
                        var fieldToRequest = Reflection.GetCustomAttribute<FieldToRequestAttribute>(property).Name;
                        var name = Reflection.GetCustomAttribute<FieldInfoAttribute>(property).Name;
                        filterID = attr.PrimaryKeyDatabaseName + "." + fieldToRequest + "|" + attr.Name + "." + name + "=" + attr.PrimaryKeyDatabaseName + "." + attr.PrimaryKeyModelName;
                    }
                    else
                    {
                        var attr = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(property);
                        filterID = field.tableName + "." + field.fieldName + (attr.PrimaryKeyDatabaseName != null && attr.PrimaryKeyDatabaseName != "" ? "|" + attr.PrimaryKeyDatabaseName + "." + attr.PrimaryKeyModelName + "=" + attr.Name + "." + attr.ForeignKeyModelName : "");
                    }
                    list.Add(new SearchFields()
                    {
                        FieldID = field.fieldID.ToString(),
                        TableName = field.tableName,
                        FieldName = field.fieldName,
                        DisplayName = field.alias != null && field.alias != "" ? field.alias : Reflection.HasCustomAttribute<DisplayAttribute>(property) ? Reflection.GetCustomAttribute<DisplayAttribute>(property).Name : "",
                        FieldType = ReportDataModel.ReportsCatalogs.GetDataBaseTypeOfProperty(field.tableName, field.fieldName, null),
                        PropertyName = property.Name,
                        FilterID = filterID
                    });
                }

                return list;
            }

            //public static object _DefineMissingJoinsRecursive(string pkTable, ref List<string> list, ref List<KeyValueModel> joins)
            //{
            //    //Dictionary<string, string> joins = new Dictionary<string, string>();
            //    var alias = "";
            //    //var _sql = "";
            //    var parentTable = "";
            //    var parentFK = "";
            //    var parentPK = "";

            //    switch (pkTable)
            //    {
            //        case "tblTerminals":
            //            {
            //                alias = "tblTerminals_tblLeads_terminalID";
            //                parentTable = "tblLeads";
            //                parentPK = "tblLeads";
            //                break;
            //            }
            //        case "aspnet_Users":
            //            {
            //                alias = "aspnet_Users_tblLeads_terminalID";
            //                parentTable = "tblLeads";
            //                parentPK = "tblLeads";
            //                break;
            //            }
            //        case "tblReservations":
            //            {
            //                alias = "tblReservations_tblLeads_leadID";
            //                parentTable = "tblLeads";
            //                parentPK = "tblLeads";
            //                //parentFK = "leadID";
            //                break;
            //            }
            //        case "tblLeadEmails":
            //            {
            //                alias = "tblLeadEmails_tblLeads_leadID";
            //                parentTable = "tblLeads";
            //                parentPK = "tblLeads";
            //                break;
            //            }
            //        case "tblPhones":
            //            {
            //                alias = "tblPhones_tblLeads_leadID";
            //                parentTable = "tblLeads";
            //                parentPK = "tblLeads";
            //                break;
            //            }
            //        case "tblPresentations":
            //            {
            //                alias = "tblPresentations_tblReservations_reservationID";
            //                parentTable = "tblReservations";
            //                parentFK = "leadID";
            //                parentPK = "tblReservations_tblLeads_leadID";
            //                break;
            //            }
            //        case "tblContractsHistory":
            //            {
            //                alias = "tblContractsHistory_tblPresentations_presentationID";
            //                parentTable = "tblPresentations";
            //                parentFK = "reservationID";
            //                parentPK = "tblPresentations_tblReservations_reservationID";
            //                break;
            //            }
            //        //case "tblCallClasifications":
            //        //    {
            //        //        alias = "tblCallClasifications_tblLeads_callClasificationID";
            //        //        parentTable = "tblLeads";
            //        //        parentPK = "tblLeads";
            //        //        break;
            //        //    }
            //        //case "tblBookingStatus":
            //        //    {
            //        //        alias = "tblBookingStatus_tblLeads_bookingStatusID";
            //        //        parentTable = "tblLeads";
            //        //        parentPK = "tblLeads";
            //        //        break;
            //        //    }
            //        //case "tblLeadStatus":
            //        //    {
            //        //        alias = "tblLeadStatus_tblLeads_leadStatusID";
            //        //        parentTable = "tblLeads";
            //        //        parentPK = "tblLeads";
            //        //        break;
            //        //    }
            //        //case "tblLeadSources":
            //        //    {
            //        //        alias = "tblLeadSources_tblLeads_leadSourceID";
            //        //        parentTable = "tblLeads";
            //        //        parentPK = "tblLeads";
            //        //        break;
            //        //    }
            //        //case "tblTourStatus":
            //        //    {
            //        //        alias = "tblTourStatus_tblPresentations_tourStatusID";
            //        //        parentTable = "tblPresentations";
            //        //        parentFK = "reservationID";
            //        //        parentPK = "tblPresentations_tblReservations_tourStatusID";
            //        //        break;
            //        //    }
            //    }
            //    //joins.Add(pkTable, alias);
            //    joins.Add(new KeyValueModel() { Key = pkTable, Value = alias });
            //    //_sql += " LEFT OUTER JOIN " + pkTable + " AS " + alias + " ON " + parentPK + "." + alias.Split('_')[1] + " = " + alias + "." + alias.Split('_')[1];
            //    //_sql += " LEFT OUTER JOIN " + pkTable + " AS " + alias + " ON " + parentPK + "." + alias.Split('_')[2] + " = " + alias + "." + alias.Split('_')[2];
            //    //list.Add(" LEFT OUTER JOIN " + pkTable + " AS " + alias + " ON " + parentPK + "." + alias.Split('_')[2] + " = " + alias + "." + alias.Split('_')[2]);
            //    list.Add(" LEFT OUTER JOIN " + pkTable + " AS " + alias + " ON " + parentPK + "." + alias.Split('_').Last() + " = " + alias + "." + alias.Split('_').Last());
            //    if (parentTable != "tblLeads")
            //    {
            //        DefineMissingJoinsRecursive(parentTable, ref list, ref joins);
            //    }
            //    return new { sqlString = list, joins = joins };
            //}

            //public static object _DefineMissingJoins(string pkTable)
            //{
            //    //Dictionary<string, string> joins = new Dictionary<string, string>();
            //    List<KeyValueModel> joins = new List<KeyValueModel>();
            //    var _sql = "";
            //    var list = new List<string>();
            //    var references = PreArrivalCatalogs.DefineMissingJoinsRecursive(pkTable, ref list, ref joins);
            //    var _list = references.GetType().GetProperty("sqlString").GetValue(references, null) as List<string>;
            //    //var _joins = references.GetType().GetProperty("joins").GetValue(references, null) as Dictionary<string, string>;
            //    var _joins = references.GetType().GetProperty("joins").GetValue(references, null) as List<KeyValueModel>;
            //    _list.Reverse();
            //    _sql = string.Join(" ", _list);
            //    return new { sqlString = _sql, joins = joins };
            //    //return PreArrivalCatalogs.DefineMissingJoinsRecursive(pkTable, ref list, ref joins);
            //}

            public static DependantFields GetDependantFields()
            {
                ePlatEntities db = new ePlatEntities();
                DependantFields df = new DependantFields();

                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var now = DateTime.Now;
                df.Fields = new List<DependantFields.DependantField>();

                DependantFields.FieldValue valDefault = new DependantFields.FieldValue();
                valDefault.ParentValue = null;
                valDefault.Value = "";
                valDefault.Text = "--Select One--";

                #region "call clasifications"
                DependantFields.DependantField callClasifications = new DependantFields.DependantField();
                callClasifications.Field = "Info_CallClasification";
                callClasifications.ParentField = "Info_Terminal";
                callClasifications.Values = new List<DependantFields.FieldValue>();

                var queryCallClasifications = from cc in db.tblTerminals_CallClasifications
                                              where terminals.Contains(cc.terminalID)
                                              select new
                                              {
                                                  cc.terminalID,
                                                  cc.callClasificationID,
                                                  cc.tblCallClasifications.callClasification
                                              };

                foreach (var cc in queryCallClasifications)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = cc.terminalID;
                    val.Value = cc.callClasificationID.ToString();
                    val.Text = cc.callClasification;
                    callClasifications.Values.Add(val);
                }

                callClasifications.Values.Insert(0, valDefault);
                df.Fields.Add(callClasifications);
                #endregion

                #region "lead status"
                DependantFields.DependantField leadStatus = new DependantFields.DependantField();
                leadStatus.Field = "Info_LeadStatus";
                leadStatus.ParentField = "Info_Terminal";
                leadStatus.Values = new List<DependantFields.FieldValue>();
                //var queryLeadStatus = db.tblTerminals_LeadStatus.Where(m => terminals.Contains(m.terminalID)).Select(m => new { m.terminalID, m.tblLeadStatus.leadStatus, m.leadStatusID });
                var queryLeadStatus = from ls in db.tblTerminals_LeadStatus
                                      where terminals.Contains(ls.terminalID)
                                      select new
                                      {
                                          ls.terminalID,
                                          ls.leadStatusID,
                                          ls.tblLeadStatus.leadStatus
                                      };

                foreach (var ls in queryLeadStatus)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = ls.terminalID;
                    val.Value = ls.leadStatusID.ToString();
                    val.Text = ls.leadStatus;
                    leadStatus.Values.Add(val);
                }

                leadStatus.Values.Insert(0, valDefault);
                df.Fields.Add(leadStatus);
                #endregion

                #region "lead sources"
                DependantFields.DependantField leadSources = new DependantFields.DependantField();
                leadSources.Field = "Info_LeadSource";
                leadSources.ParentField = "Info_Terminal";
                leadSources.Values = new List<DependantFields.FieldValue>();
                var sources = LeadSourceDataModel.GetLeadSourcesByTerminal().Select(m => long.Parse(m.Value)).ToArray();
                var queryLeadSources = db.tblTerminals_LeadSources.Where(m => sources.Contains(m.leadSourceID) && terminals.Contains(m.terminalID)).Select(m => new { m.terminalID, m.leadSourceID, m.tblLeadSources.leadSource });

                foreach (var ls in queryLeadSources)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = ls.terminalID;
                    val.Value = ls.leadSourceID.ToString();
                    val.Text = ls.leadSource;
                    leadSources.Values.Add(val);
                }
                leadSources.Values.Insert(0, valDefault);
                df.Fields.Add(leadSources);
                #endregion

                #region "secondary booking status"
                DependantFields.DependantField bs = new DependantFields.DependantField();
                bs.Field = "Info_SecondaryBookingStatus";
                bs.ParentField = "Info_Terminal";
                bs.Values = new List<DependantFields.FieldValue>();
                var bookingStatus = BookingStatusDataModel.GetSecondaryBookingStatus().Select(m => int.Parse(m.Value)).ToArray();
                var queryBookingStatus = db.tblTerminals_BookingStatus.Where(m => bookingStatus.Contains(m.bookingStatusID) && terminals.Contains(m.terminalID) && m.isPrimary == false).Select(m => new { m.terminalID, m.bookingStatusID, m.tblBookingStatus.bookingStatus });

                foreach (var pbs in queryBookingStatus.Distinct())
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = pbs.terminalID;
                    val.Value = pbs.bookingStatusID.ToString();
                    val.Text = pbs.bookingStatus;
                    bs.Values.Add(val);
                }
                bs.Values.Insert(0, valDefault);
                df.Fields.Add(bs);
                #endregion

                #region "final booking status"
                DependantFields.DependantField fbs = new DependantFields.DependantField();
                fbs.Field = "PresentationInfo_FinalBookingStatus";
                fbs.ParentField = "Info_Terminal";
                fbs.Values = new List<DependantFields.FieldValue>();
                var fBookingStatus = BookingStatusDataModel.GetFinalBookingStatus().Select(m => int.Parse(m.Value)).ToArray();
                var queryFBookingStatus = db.tblTerminals_BookingStatus.Where(m => fBookingStatus.Contains(m.bookingStatusID) && terminals.Contains(m.terminalID) && m.isFinal == true).Select(m => new { m.terminalID, m.bookingStatusID, m.tblBookingStatus.bookingStatus });

                foreach (var pbs in queryFBookingStatus.Distinct())
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = pbs.terminalID;
                    val.Value = pbs.bookingStatusID.ToString();
                    val.Text = pbs.bookingStatus;
                    fbs.Values.Add(val);
                }
                fbs.Values.Insert(0, valDefault);
                df.Fields.Add(fbs);
                #endregion

                #region "destinations"
                DependantFields.DependantField destinations = new DependantFields.DependantField();
                destinations.Field = "ReservationInfo_Destination";
                destinations.ParentField = "Info_Terminal";
                destinations.Values = new List<DependantFields.FieldValue>();

                var queryDestinations = db.tblTerminals_Destinations.Where(m => terminals.Contains(m.terminalID)).Select(m => new { m.terminalID, m.destinationID, m.tblDestinations.destination });

                foreach (var ls in queryDestinations)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = ls.terminalID;
                    val.Value = ls.destinationID.ToString();
                    val.Text = ls.destination;
                    destinations.Values.Add(val);
                }

                destinations.Values.Insert(0, valDefault);
                df.Fields.Add(destinations);
                #endregion

                #region "resorts"
                DependantFields.DependantField resorts = new DependantFields.DependantField();
                resorts.Field = "ReservationInfo_Place";
                resorts.ParentField = "ReservationInfo_Destination";
                resorts.Values = new List<DependantFields.FieldValue>();
                var places = PlaceDataModel.GetResortsByProfile().Select(m => long.Parse(m.Value)).ToArray();
                var queryResorts = db.tblPlaces_Terminals.Where(m => places.Contains(m.placeID) && terminals.Contains(m.terminalID)).Select(m => new { m.terminalID, m.tblPlaces.destinationID, m.placeID, m.tblPlaces.place });

                foreach (var place in queryResorts.Distinct())
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = place.destinationID;
                    val.Value = place.placeID.ToString();
                    val.Text = place.place;
                    resorts.Values.Add(val);
                }
                resorts.Values.Insert(0, valDefault);
                df.Fields.Add(resorts);
                #endregion

                #region "room types"
                DependantFields.DependantField roomTypes = new DependantFields.DependantField();
                roomTypes.Field = "ReservationInfo_RoomType";
                roomTypes.ParentField = "ReservationInfo_Place";
                roomTypes.Values = new List<DependantFields.FieldValue>();
                var queryRoomTypes = db.tblRoomTypes.Select(m => new { m.tblPlaces.destinationID, m.placeID, m.roomTypeID, m.roomType });
                foreach (var rt in queryRoomTypes)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = rt.placeID;
                    val.Value = rt.roomTypeID.ToString();
                    val.Text = rt.roomType;
                    roomTypes.Values.Add(val);
                }
                roomTypes.Values.Insert(0, valDefault);
                df.Fields.Add(roomTypes);
                #endregion

                #region "options"
                DependantFields.DependantField options = new DependantFields.DependantField();
                options.Field = "OptionInfo_Option";
                options.ParentField = "OptionInfo_OptionType";
                options.GrandParentField = "ReservationInfo_Place";
                options.Values = new List<DependantFields.FieldValue>();

                var optionNames = from o in db.tblOptions_Places
                                  where (o.tblOptions.terminateDate > now || o.tblOptions.terminateDate == null)
                                  select new
                                  {
                                      o.optionID,
                                      o.placeID,
                                      o.tblOptions.optionTypeID,
                                      o.tblOptions.optionName,
                                      o.tblOptions.optionDescription
                                  };
                foreach (var i in optionNames.OrderBy(m => m.optionName).Distinct())
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.GrandParentValue = i.placeID;
                    val.ParentValue = i.optionTypeID;
                    val.Value = i.optionID.ToString();
                    val.Text = i.optionName;
                    options.Values.Add(val);
                }

                DependantFields.FieldValue _default = new DependantFields.FieldValue();
                _default.ParentValue = null;
                _default.Value = "";
                _default.Text = "--Select Type--";
                options.Values.Insert(0, _default);
                df.Fields.Add(options);
                #endregion

                #region "options descriptions"
                DependantFields.DependantField descriptions = new DependantFields.DependantField();
                descriptions.Field = "OptionInfo_OptionDescription";
                descriptions.ParentField = "OptionInfo_Option";
                descriptions.Values = new List<DependantFields.FieldValue>();

                foreach (var i in optionNames.OrderBy(m => m.optionName).Distinct())
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i.optionID;
                    val.Value = i.optionDescription;
                    val.Text = i.optionDescription;
                    descriptions.Values.Add(val);
                }
                df.Fields.Add(descriptions);
                #endregion

                #region "points redemption"
                DependantFields.DependantField points = new DependantFields.DependantField();
                points.Field = "OptionInfo_MaxRateRedemption";
                points.ParentField = "OptionInfo_OptionType";
                points.GrandParentField = "Info_Terminal";
                points.Values = new List<DependantFields.FieldValue>();

                var pointsRedemption = from p in db.tblMaxPointsPerOptions
                                       join r in db.tblPointsRedemptionRates on p.pointsRedemptionRateID equals r.pointsRedemptionRateID
                                       where (p.fromDate <= now && (p.toDate > now || p.toDate == null))
                                       select new
                                       {
                                           p.tblOptionTypes.terminalID,
                                           p.optionTypeID,
                                           p.maxPointsRate,

                                       };
                foreach (var i in pointsRedemption)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.GrandParentValue = i.terminalID.ToString();
                    val.ParentValue = i.optionTypeID;
                    val.Value = i.maxPointsRate.ToString();
                    val.Text = i.maxPointsRate.ToString();
                    points.Values.Add(val);
                }
                df.Fields.Add(points);
                //DependantFields.FieldValue _defaultPoints = new DependantFields.FieldValue();
                //_default.ParentValue = null;
                //_default.Value = "";
                //_default.Text = "--Select Type--";
                //options.Values.Insert(0, _defaultPoints);
                //df.Fields.Add(options);
                #endregion

                #region "option prices"
                DependantFields.DependantField prices = new DependantFields.DependantField();
                prices.Field = "OptionInfo_Price";
                prices.ParentField = "OptionInfo_Option";
                prices.Values = new List<DependantFields.FieldValue>();

                var optionPrices = from o in db.tblOptions
                                       //where (o.terminateDate <= now || o.terminateDate == null)
                                   where (o.terminateDate > now || o.terminateDate == null)
                                   select new
                                   {
                                       o.optionID,
                                       o.goldCardPrice
                                   };
                foreach (var i in optionPrices)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i.optionID;
                    val.Value = i.goldCardPrice.ToString();
                    val.Text = i.goldCardPrice.ToString();
                    prices.Values.Add(val);
                }

                DependantFields.FieldValue _defaultPrice = new DependantFields.FieldValue();
                _defaultPrice.ParentValue = null;
                _defaultPrice.Value = "";
                _defaultPrice.Text = "--Select Option--";
                prices.Values.Insert(0, _defaultPrice);
                df.Fields.Add(prices);
                #endregion

                #region "eligible for credit"
                DependantFields.DependantField eligible = new DependantFields.DependantField();
                eligible.Field = "OptionInfo_Eligible";
                eligible.ParentField = "OptionInfo_Option";
                eligible.Values = new List<DependantFields.FieldValue>();

                var eligibles = from o in db.tblOptions
                                    //where (o.terminateDate <= now || o.terminateDate == null)
                                where (o.terminateDate > now || o.terminateDate == null)
                                select new
                                {
                                    o.optionID,
                                    o.resortCredit
                                };

                foreach (var i in eligibles)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = i.optionID;
                    val.Value = "false";
                    val.Text = "False";
                    eligible.Values.Add(val);
                    if (i.resortCredit != null && i.resortCredit != 0)
                    {
                        DependantFields.FieldValue _val = new DependantFields.FieldValue();
                        _val.ParentValue = i.optionID;
                        _val.Value = "true";
                        _val.Text = "True";
                        eligible.Values.Add(_val);
                    }
                }

                DependantFields.FieldValue _defaultEligible = new DependantFields.FieldValue();
                _defaultEligible.ParentValue = null;
                _defaultEligible.Value = "null";
                _defaultEligible.Text = "--Select Option--";
                eligible.Values.Insert(0, _defaultEligible);
                df.Fields.Add(eligible);
                #endregion

                #region "credit amount"
                DependantFields.DependantField credits = new DependantFields.DependantField();
                credits.Field = "OptionInfo_CreditAmount";
                credits.ParentField = "OptionInfo_Eligible";
                credits.GrandParentField = "OptionInfo_Option";
                credits.Values = new List<DependantFields.FieldValue>();

                var optionCredits = from o in db.tblOptions
                                        //where (o.terminateDate <= now || o.terminateDate == null)
                                    where (o.terminateDate > now || o.terminateDate == null)
                                    select new
                                    {
                                        o.optionID,
                                        o.resortCredit,
                                        eligible = (o.resortCredit == null || o.resortCredit == 0 ? "false" : "true"),
                                        o.optionName
                                    };

                foreach (var i in optionCredits)
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.GrandParentValue = i.optionID;
                    val.ParentValue = i.eligible;
                    val.Value = i.eligible == "true" ? i.resortCredit.ToString() : "0";
                    val.Text = i.eligible == "true" ? i.resortCredit.ToString() : "0";
                    credits.Values.Add(val);

                    val = new DependantFields.FieldValue();
                    val.GrandParentValue = i.optionID;
                    val.ParentValue = i.eligible == "true" ? "false" : "true";
                    val.Value = i.eligible == "true" ? "0" : i.resortCredit.ToString();
                    val.Text = i.eligible == "true" ? "0" : i.resortCredit.ToString();
                    credits.Values.Add(val);

                }

                DependantFields.FieldValue _defaultCredit = new DependantFields.FieldValue();
                _defaultCredit.ParentValue = null;
                _defaultCredit.Value = "null";
                _defaultCredit.Text = "--Select If Eligible--";
                credits.Values.Insert(0, _defaultCredit);
                df.Fields.Add(credits);
                #endregion

                #region "spiTours"
                //DependantFields.DependantField tours = new DependantFields.DependantField();
                //tours.Field = "PresentationInfo_SpiTour";
                //tours.ParentField = "ReservationInfo_Destination";
                //tours.Values = new List<DependantFields.FieldValue>();
                //var ids = queryDestinations.Select(m => m.destinationID);

                //var regions = (from d in db.tblDestinations
                //               where ids.Contains(d.destinationID)
                //               select new { d.spiRegionID, d.destinationID }).Distinct();
                //var _ids = regions.Select(m => m.spiRegionID).ToArray();

                //var queryTours = from t in db.tblSPIManifest
                //                 where _ids.Contains(t.regionID)
                //                 && t.tourDate == now.Date
                //                 select t;

                //foreach (var qt in queryTours)
                //{
                //    DependantFields.FieldValue val = new DependantFields.FieldValue();
                //    val.ParentValue = regions.FirstOrDefault(m => m.spiRegionID == qt.regionID).destinationID;
                //    val.Value = qt.tourID.ToString();
                //    val.Text = qt.fullName;
                //    tours.Values.Add(val);
                //}
                //tours.Values.Insert(0, valDefault);
                //df.Fields.Add(tours);
                #endregion

                return df;
            }

            public static List<SelectListItem> GetAllChargeTypes()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblChargeTypes;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.chargeTypeID.ToString(),
                        Text = i.chargeType
                    });
                }

                return list;
            }

            public static List<SelectListItem> GetAllChargeDescriptions()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblChargeDescriptions;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.chargeDescriptionID.ToString(),
                        Text = i.chargeDescription
                    });
                }

                return list;
            }

            public static List<SelectListItem> GetFrontResorts()
            {
                ePlatEntities db = new ePlatEntities();

                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var query = from pt in db.tblPlaces_Terminals
                            join p in db.tblPlaces on pt.placeID equals p.placeID into pt_p
                            from p in pt_p.DefaultIfEmpty()
                            join d in db.tblDestinations on p.destinationID equals d.destinationID into p_d
                            from d in p_d.DefaultIfEmpty()
                            where terminals.Contains(pt.terminalID)
                            && p.frontOfficeResortID != null
                            select new
                            {
                                p.frontOfficeResortID,
                                p.place,
                                d.destination
                            };

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.frontOfficeResortID.ToString(),
                        Text = i.place + " " + i.destination
                    });
                }

                return list;
            }

            public static List<SelectListItem> GetReservationStatus()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

                var query = from rst in db.tblReservationStatus_Terminals
                            join rs in db.tblReservationStatus on rst.reservationStatusID equals rs.reservationStatusID
                            where terminals.Contains(rst.terminalID)
                            select new
                            {
                                rs.reservationStatusID,
                                rs.reservationStatus
                            };

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.reservationStatusID.ToString(),
                        Text = i.reservationStatus
                    });
                }

                return list;
            }

            public static long GetLeadSourceByProgram(string marketCode, int resortID)
            {
                ePlatEntities db = new ePlatEntities();
                long leadSourceID = 0;
                var query = db.tblMarketCodes.FirstOrDefault(m => m.marketCode == marketCode && m.frontOfficeResortID == resortID);
                var programID = query != null ? query.programID != null ? (int)query.programID : 0 : 0;

                if (programID == 3 || programID == 11)
                {
                    leadSourceID = 1;
                }
                else if (programID == 4 || programID == 12)
                {
                    leadSourceID = 45;
                }
                else if (programID == 5 || programID == 13)
                {
                    leadSourceID = 52;
                }
                else
                {
                    leadSourceID = 15;
                }

                return leadSourceID;
            }

            public static List<SelectListItem> GetDestinationsAllowed()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var destPerProfile = db.tblUsers_Destinations.Where(m => m.userID == session.UserID).Select(m => m.destinationID).ToList();
                var destPerTerminals = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();

                list = destPerTerminals.Where(m => destPerProfile.Contains(long.Parse(m.Value))).ToList();

                return list;
            }

            public static List<SelectListItem> GetPlanTypes()
            {
                ePlatEntities db = new ePlatEntities();
                var list = new List<SelectListItem>();

                var query = db.tblPlanTypes;

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.planTypeID.ToString(),
                        Text = i.planType
                    });
                }
                return list;
            }

            public static List<SelectListItem> GetManifestByDate(int place, string date = null)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                try
                {
                    var _date = date != null && date != "null" && date != "" ? DateTime.Parse(date) : DateTime.Today;
                    var fDate = _date.AddDays(1).AddSeconds(-1);
                    if (place != 0)
                    {
                        var region = (from p in db.tblPlaces
                                      where place == p.placeID
                                      select p.spiRegionID).FirstOrDefault();

                        var queryTours = (from t in db.tblSPIManifest
                                          where region == t.regionID
                                          && (t.tourDate >= _date && t.tourDate <= fDate)
                                          select new { t.tourID, t.fullName, t.firstName, t.lastName }).Distinct();

                        foreach (var qt in queryTours.OrderBy(m => m.lastName))
                        {
                            list.Add(new SelectListItem()
                            {
                                Value = qt.tourID.ToString(),
                                Text = qt.lastName + " " + qt.firstName
                            });
                        }
                    }
                }
                catch { }
                list.Insert(0, ListItems.Default());
                return list;
            }

            public static List<SelectListItem> GetPaymentTypesByWorkGroup()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var types = GeneralFunctions.PaymentTypes.ToList();
                var pTypes = db.tblTerminals_PaymentTypes.Where(m => terminals.Contains(m.terminalID)).Select(m => m.paymentTypeID).ToArray();

                if (pTypes.Count() > 0)
                {
                    types = types.Where(m => pTypes.Contains(int.Parse(m.Key))).ToList();
                }

                foreach (var i in types)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.Key,
                        Text = i.Value
                    });
                }
                return list;
            }

            public static object GetTourInfo(int tourID)
            {
                ePlatEntities db = new ePlatEntities();
                var response = new { realTourDate = "", tourStatus = 0, source = "", volumeSold = (decimal?)decimal.Parse("0") };
                if (tourID != 0)
                {
                    var tour = db.tblSPIManifest.Where(m => m.tourID == tourID).OrderByDescending(m => m.dateLastUpdate).FirstOrDefault();
                    //response.realTourDate = tour.tourDate.Value.ToString("yyyy-MM-dd HH:mm tt");
                    //response.tourStatus = Dictionaries.FrontTourStatus.FirstOrDefault(m => tour.tourStatus.Contains(m.Key)).Value;
                    response = new { realTourDate = tour.tourDate.Value.ToString("yyyy-MM-dd HH:mm tt"), tourStatus = Dictionaries.FrontTourStatus.FirstOrDefault(m => tour.tourStatus.Contains(m.Key)).Value, tour.source, volumeSold = tour.volume };
                }
                return response;
            }

            public static object GetPointsRedemptionRate(string id)
            {
                ePlatEntities db = new ePlatEntities();
                Guid reservationID = Guid.Parse(id);
                DateTime now = DateTime.Now;

                var options = db.tblOptionsSold.Where(m => m.reservationID == reservationID && m.deleted != true);
                var optionTypes = options.Select(m => m.optionTypeID);

                var rates = db.tblMaxPointsPerOptions.Where(m => m.fromDate <= now && (m.toDate == null || m.toDate >= now) && optionTypes.Contains(m.optionTypeID)).Where(m => m.tblPointsRedemptionRates.fromDate <= now && (m.tblPointsRedemptionRates.toDate == null || m.tblPointsRedemptionRates.toDate >= now));
                var _ratesTypes = rates.Select(x => (int?)x.optionTypeID).ToArray();
                var total = options.Where(m => _ratesTypes.Contains(m.optionTypeID) && m.pointsRedemption != null);

                var list = new List<object>();
                foreach (var rate in rates)
                {
                    list.Add(new { optiontType = rate.optionTypeID, rate = rate.tblPointsRedemptionRates.rate });
                }
                var cosa = total.Select(m => m.pointsRedemption).ToList();
                decimal _total = cosa.Sum(m => decimal.Parse(m));
                return new { maxAllowed = _total, rates = list };
            }

            public static List<SelectListItem> GetInvoicesToRefund(Guid? reservationID)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                var id = reservationID ?? Guid.Empty;

                var query = from p in db.tblPaymentDetails
                            join m in db.tblMoneyTransactions on p.moneyTransactionID equals m.moneyTransactionID
                            where p.reservationID == id
                            && m.transactionTypeID == 1
                            && (p.deleted == null || p.deleted == false)
                            && (m.authCode != null && m.authCode != "")
                            select new
                            {
                                m.moneyTransactionID,
                                m.authCode,
                                p.paymentDetailsID
                            };

                foreach (var i in query)
                {
                    list.Add(new SelectListItem()
                    {
                        //Value = i.moneyTransactionID.ToString(),
                        Value = i.authCode,
                        Text = i.authCode
                    });
                }

                return list;
            }

            public static DependantFields GetDependantResorts()
            {
                ePlatEntities db = new ePlatEntities();
                DependantFields df = new DependantFields();
                var terminals = GeneralFunctions.IsUserInRole("Administrator") ? new long[] { 10, 16, 80 } : session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                df.Fields = new List<DependantFields.DependantField>();
                DependantFields.FieldValue valDefault = new DependantFields.FieldValue();
                valDefault.ParentValue = null;
                valDefault.Value = "";
                valDefault.Text = "--Select One--";

                #region "resorts"
                DependantFields.DependantField resorts = new DependantFields.DependantField();
                resorts.Field = "PlaceID";
                resorts.ParentField = "TerminalID";
                resorts.Values = new List<DependantFields.FieldValue>();
                var places = PlaceDataModel.GetResortsByTerminals(string.Join(",", terminals)).Select(m => long.Parse(m.Value)).ToArray();
                var queryResorts = db.tblPlaces_Terminals.Where(m => places.Contains(m.placeID) && terminals.Contains(m.terminalID)).Select(m => new { m.terminalID, m.tblPlaces.tblDestinations.destination, m.placeID, m.tblPlaces.place });

                foreach (var place in queryResorts.Distinct())
                {
                    DependantFields.FieldValue val = new DependantFields.FieldValue();
                    val.ParentValue = place.terminalID;
                    val.Value = place.placeID.ToString();
                    val.Text = place.place + " " + place.destination;
                    resorts.Values.Add(val);
                }
                resorts.Values.Insert(0, valDefault);
                df.Fields.Add(resorts);
                #endregion

                return df;
            }
        }

        public SearchToReassignModel ReassignOptions(string id)
        {
            ePlatEntities db = new ePlatEntities();
            var reservationID = Guid.Parse(id);
            var r = db.tblReservations.FirstOrDefault(m => m.reservationID == reservationID);
            var model = new SearchToReassignModel();
            model.ReservationID = reservationID;
            model.ReservationToReassign = r.tblLeads.firstName + " " + r.tblLeads.lastName + " - " + r.hotelConfirmationNumber + " (" + r.frontOfficeCertificateNumber + ") " + r.tblPlaces.place + " " + r.tblPlaces.tblDestinations.destination;

            return model;
        }

        public static List<ReassignmentModel> ReservationsToReceive(SearchToReassignModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<ReassignmentModel>();

            var r = db.tblReservations.FirstOrDefault(m => m.reservationID == model.ReservationID);

            if (model.Search)
            {
                var iDate = model.ArrivalDate == null ? DateTime.Today.AddDays(-10) : (DateTime?)null;
                var fDate = model.ArrivalDate == null ? DateTime.Today.AddDays(20) : (DateTime?)null;

                var query = (from rsv in db.tblReservations
                             join l in db.tblLeads on rsv.leadID equals l.leadID
                             where l.terminalID == model.TerminalID
                             && rsv.placeID == model.PlaceID
                             && (rsv.reservationStatusID == null || rsv.reservationStatusID != 3)
                             && rsv.reservationID != model.ReservationID
                             && (((model.ConfirmationNumber == null && model.CRS == null) && rsv.arrivalDate == model.ArrivalDate)
                             || model.ConfirmationNumber == rsv.hotelConfirmationNumber || model.CRS == rsv.frontOfficeCertificateNumber)
                             select new
                             {
                                 l.firstName,
                                 l.lastName,
                                 rsv.reservationID,
                                 l.terminalID,
                                 rsv.placeID,
                                 rsv.arrivalDate,
                                 rsv.hotelConfirmationNumber,
                                 rsv.frontOfficeCertificateNumber
                             }).ToList();


                //var query = (from rsv in db.tblReservations
                //             join l in db.tblLeads on rsv.leadID equals l.leadID
                //             where (model.TerminalID == null || l.terminalID == model.TerminalID)
                //             && ((model.PlaceID == null && rsv.placeID == r.placeID) || rsv.placeID == model.PlaceID)
                //             && (model.ConfirmationNumber == null || rsv.hotelConfirmationNumber.Contains(model.ConfirmationNumber))
                //             && (model.CRS == null || rsv.frontOfficeCertificateNumber.Contains(model.CRS))
                //             && ((model.ArrivalDate == null && model.CRS == null && model.ConfirmationNumber == null && (iDate <= rsv.arrivalDate && rsv.arrivalDate <= fDate))
                //             || (model.ArrivalDate == null && (model.ConfirmationNumber != null || model.CRS != null))
                //             || (model.ArrivalDate != null && (model.ArrivalDate == rsv.arrivalDate)))
                //             && (rsv.reservationStatusID == null || rsv.reservationStatusID != 3)
                //             && rsv.reservationID != model.ReservationID
                //             select new 
                //             {
                //                 l.firstName,
                //                 l.lastName,
                //                 rsv.reservationID,
                //                 l.terminalID,
                //                 rsv.placeID,
                //                 rsv.arrivalDate,
                //                 rsv.hotelConfirmationNumber,
                //                 rsv.frontOfficeCertificateNumber
                //             }).ToList();

                foreach (var i in query)
                {
                    list.Add(new ReassignmentModel()
                    {
                        FirstName = i.firstName,
                        LastName = i.lastName,
                        ReservationID = i.reservationID,
                        TerminalID = i.terminalID,
                        PlaceID = i.placeID,
                        ArrivalDate = i.arrivalDate.Value.ToString("yyyy-MM-dd"),
                        ConfirmationNumber = i.hotelConfirmationNumber,
                        CRS = i.frontOfficeCertificateNumber
                    });
                }

                return list;
            }
            else
            {
                list = new JavaScriptSerializer().Deserialize<List<ReassignmentModel>>(model.Reservations);
                var item = list.FirstOrDefault(m => m.Options || m.Flights || m.Payments);
                try
                {
                    var target = db.tblReservations.FirstOrDefault(m => m.reservationID == item.ReservationID);

                    if (item.Options)
                    {
                        var options = r.tblOptionsSold.Select(m => m.optionSoldID).ToList();
                        foreach (var i in options)
                        {
                            db.tblOptionsSold.Single(m => m.optionSoldID == i).reservationID = target.reservationID;
                        }
                    }
                    if (item.Flights)
                    {
                        var flights = r.tblFlights.Select(m => m.flightID).ToList();
                        foreach (var i in flights)
                        {
                            db.tblFlights.Single(m => m.flightID == i).reservationID = target.reservationID;
                        }
                    }
                    if (item.Payments)
                    {
                        var billings = r.tblLeads.tblBillingInfo.Select(m => m.billingInfoID).ToList();
                        var payments = r.tblPaymentDetails.Select(m => m.paymentDetailsID).ToList();

                        foreach (var i in billings)
                        {
                            db.tblBillingInfo.Single(m => m.billingInfoID == i).leadID = target.leadID;
                        }
                        foreach (var i in payments)
                        {
                            db.tblPaymentDetails.Single(m => m.paymentDetailsID == i).reservationID = target.reservationID;
                        }
                    }

                    db.SaveChanges();
                    item.Status = true;
                }
                catch (Exception ex)
                {
                    item.Status = false;
                }
                return list;
            }
            //return query;
        }

        public dynamic GetModelToTrack(PropertyInfo[] properties, dynamic _model, Type type)
        {
            var model = Convert.ChangeType(_model, type);
            var _json = "{";
            foreach (var p in properties.Where(m => m.CanRead))
            {
                dynamic value = model.GetType().GetProperty(p.Name).GetValue(model, null);
                var pType = p.PropertyType;

                if (p.PropertyType.FullName.IndexOf("ePlatBack.Models.ViewModels") != -1)
                {

                    var props = Reflection.GetPropertiesWithoutAtribute<DoNotTrackChangesAttribute>(p);
                    ////_json += GetModelToTrack(Reflection.GetPropertiesWithoutAtribute<DoNotTrackChangesAttribute>(properties.Where(m => m.Name == p.Name).FirstOrDefault()), _model, type);
                    _json += GetModelToTrack(props, p, pType);
                }
                var sValue = Convert.ChangeType(value, pType);
                //_json += (_json == "{" ? "" : ",") + "\"" + p.Name.ToString() + "\":\"" + (value != null ? value.ToString() : "") + "\"";
                _json += (_json == "{" ? "" : ",") + "\"" + p.Name.ToString() + "\":\"" + (sValue != null ? sValue.ToString() : "") + "\"";
            }
            _json += "}";

            var json = new JavaScriptSerializer().Deserialize<dynamic>(_json);
            return json;
        }

        public static object GetHistory(string referenceID)
        {
            ePlatEntities db = new ePlatEntities();

            DateTime date = DateTime.Today;
            var counter = 2;

            var conn = ConfigurationManager.ConnectionStrings["ePlatEntities"].ConnectionString.ToString();
            conn = new EntityConnectionStringBuilder(conn).ProviderConnectionString;
            var json = "";
            using (SqlConnection sqlConn = new SqlConnection(conn))
            {
                sqlConn.Open();
                date = date.AddMonths(-2);
                while (counter >= 0)
                {
                    var table = "tblUserLogActivity" + date.ToString("yyyy") + date.ToString("MM");
                    var str = "select JsonModel from " + table
                        + " where referenceID = '" + referenceID + "'"
                        + "order by DateSaved asc";

                    var command = new System.Data.SqlClient.SqlCommand(str, sqlConn);
                    var reader = command.ExecuteReader();
                    var cosa = "";
                    while (reader.Read())
                    {
                        cosa = reader.GetString(0);
                        json += ((json == "" ? "" : ",") + reader["JsonModel"].ToString());//no lee el valor de la columna, posible por el indice
                    }
                    reader.Close();
                    date = date.AddMonths(1);
                    counter--;
                }
                sqlConn.Close();
            }
            json = "[" + json + "]";
            return new { data = json };
        }

        public static AttemptResponse ClickToCall(Guid leadID, int? phoneID)
        {
            return PhoneDataModel.DialLeadPhoneNumber(leadID, phoneID);
        }

        public static AttemptResponse UnloadLead(Guid leadID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                if (!GeneralFunctions.IsUserInRole("Administrator"))
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "You don't have enough privileges to perform this action";
                    response.ObjectID = leadID;
                    return response;
                }
                var query = db.tblLeads.Single(m => m.leadID == leadID);
                query.isTest = true;
                db.SaveChanges();

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Lead Successfully Unloaded";
                response.ObjectID = leadID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Lead Not Unloaded";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public static AttemptResponse UnloadReservation(Guid reservationID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                if (!GeneralFunctions.IsUserInRole("Administrator"))
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "You don't have enough privileges to perform this action";
                    response.ObjectID = reservationID;
                    return response;
                }
                var query = db.tblReservations.Single(m => m.reservationID == reservationID);
                query.isTest = true;
                db.SaveChanges();

                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Reservation Successfully Unloaded";
                response.ObjectID = reservationID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Reservation Not Unloaded";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public static AttemptResponse GetDataFromRC(Guid leadID)
        {
            ePlatEntities db = new ePlatEntities();
            resortConnectEntities dba = new resortConnectEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var lead = db.tblLeads.Single(m => m.leadID == leadID);
                var hc = lead.tblReservations.OrderByDescending(x => x.arrivalDate).FirstOrDefault().hotelConfirmationNumber;


                var rcReservation = (from r in dba.Reservation
                                     join rc in dba.Reservation_Contact on r.ReservationId equals rc.ReservationId
                                     join c in dba.Contact on rc.ContactId equals c.ContactId
                                     where r.ConfirmationNumber == hc
                                     select c).FirstOrDefault();
                if (rcReservation != null)
                {
                    lead.firstName = rcReservation.FirstName;
                    lead.lastName = rcReservation.LastName;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead Updated";
                    response.ObjectID = new { lead.firstName, lead.lastName };
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "No Data Found";
                    response.ObjectID = 0;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Lead NOT Updated";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveLayout(PreArrivalSearchModel model)
        {
            AttemptResponse response = new AttemptResponse();
            ePlatEntities db = new ePlatEntities();

            try
            {
                var query = new tblReportLayouts();
                if (model.Search_ReportLayout != null && model.Search_ReportLayout != 0)
                {
                    query = db.tblReportLayouts.Single(m => m.reportLayoutID == (long)model.Search_ReportLayout);

                    //if ((model.Search_LayoutName.IndexOf("Administrator") != -1 || model.Search_LayoutName.IndexOf("Administrador") != -1) && !GeneralFunctions.IsUserInRole("Administrator"))
                    if ((query.layout.IndexOf("Administrator") != -1 || query.layout.IndexOf("Administrador") != -1) && !GeneralFunctions.IsUserInRole("Administrator"))
                    {
                        response.Message = "Administrator layouts cannot be modified";
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = query.reportLayoutID;
                        return response;
                    }
                    //update

                    query.layout = model.Search_LayoutName.IndexOf("-") != -1 ? model.Search_LayoutName.Split('-')[0].Trim() : model.Search_LayoutName;
                    query.fields = model.Search_Fields;
                    query.public_ = model.Search_Public;
                    response.Message = "Layout Updated";
                }
                else
                {
                    //create
                    query.layout = model.Search_LayoutName;
                    query.fields = model.Search_Fields;
                    query.ownerUserID = session.UserID;
                    query.public_ = model.Search_Public;
                    query.sysComponentID = 11597;//prearrival module

                    var _query = new tblSysWorkGroups_ReportLayouts();
                    _query.sysWorkGroupID = (int)session.WorkGroupID;
                    query.tblSysWorkGroups_ReportLayouts.Add(_query);

                    db.tblReportLayouts.AddObject(query);
                    response.Message = "Layout Saved";
                }

                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = query.reportLayoutID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Changes NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse CopyLayout(long reportLayoutID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var model = new PreArrivalSearchModel();
            try
            {
                var query = db.tblReportLayouts.Single(m => m.reportLayoutID == reportLayoutID);

                model.Search_LayoutName = query.layout + "_copy";
                model.Search_Fields = query.fields;
                model.Search_Public = query.public_;

                response = SaveLayout(model);
                response.ObjectID = new { layoutID = response.ObjectID, fields = query.fields, layout = model.Search_LayoutName };
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Copy NOT Saved";
                response.Exception = ex;
                response.ObjectID = 0;
                return response;
            }
        }

        public AttemptResponse DeleteLayout(long reportLayoutID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var query = db.tblReportLayouts.Single(m => m.reportLayoutID == reportLayoutID);
                if ((query.layout.IndexOf("dministrator") != -1 || query.layout.IndexOf("dministrador") != -1) & !GeneralFunctions.IsUserInRole("Administrator"))
                {
                    response.Type = Utils.Attempt_ResponseTypes.Warning;
                    response.Message = "You CANNOT delete Administrator Layouts";
                    return response;
                }
                var q = db.tblSysWorkGroups_ReportLayouts.Where(m => m.reportLayoutID == reportLayoutID);
                foreach (var i in q)
                {
                    db.tblSysWorkGroups_ReportLayouts.DeleteObject(i);
                }
                db.tblReportLayouts.DeleteObject(query);
                db.SaveChanges();
                response.Type = Utils.Attempt_ResponseTypes.Ok;
                response.Message = "Layout Deleted";
                response.ObjectID = reportLayoutID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Utils.Attempt_ResponseTypes.Error;
                response.Message = "Layout NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public PreArrivalSearchResultsModel SearchPreArrival(PreArrivalSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var results = new PreArrivalSearchResultsModel();
            DataTable dataTable = new DataTable();
            List<string> headers = new List<string>() { "ID" };
            List<List<KeyValuePair<string, string>>> rows = new List<List<KeyValuePair<string, string>>>();
            var select = @"select distinct tblLeads.leadID";
            var from = " from tblLeads ";
            var where = " where ";
            var _aliases = new List<KeyValuePair<string, KeyValuePair<string, string>>>();
            var _alias = "";

            var propertiesForColumns = model.GetType().GetProperties().Where(m => Reflection.HasCustomAttribute<DataBaseInfoAttribute>(m));
            var propertiesWithValue = propertiesForColumns.Where(m => m.GetValue(model, null) != null);

            #region "limitation of query"
            var limit = false;
            if (session.WorkGroupID == 8)
            {
                var isAgent = db.aspnet_Roles.Single(m => m.RoleId == session.RoleID).RoleName == "Agent";
                if (isAgent)
                {
                    model.Search_AssignedToUser = new Guid[] { session.UserID };
                }
            }
            model.Search_Terminal = model.Search_Terminal ?? session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            //model.IsTest = false;

            if (!GeneralFunctions.IsUserInRole("Administrator"))
            {
                model.IsTest = false;
            }

            if (propertiesWithValue.Count(m => m.Name.IndexOf("_I_") != -1) == 0 && model.Search_ReservationID == null && model.Search_FirstName == null && model.Search_LastName == null && model.Search_HotelConfirmationNumber == null && model.Search_CertificateNumber == null)
            {
                limit = true;
                model.Search_I_ArrivalDate = model.Search_I_ArrivalDate ?? DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                model.Search_F_ArrivalDate = model.Search_F_ArrivalDate ?? DateTime.Today.AddDays(62).AddSeconds(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            #endregion

            foreach (var column in model.Search_Columns.Split(','))
            {
                #region "select & from"
                select += ",";
                var property = propertiesForColumns.Where(m => column.Contains(Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name) && column.Replace("+", " ").Contains(Reflection.GetCustomAttribute<DisplayAttribute>(m).Name)).FirstOrDefault();
                var dbInfo = property.GetCustomAttribute<DataBaseInfoAttribute>();
                var fieldName = property.GetCustomAttribute<FieldInfoAttribute>().Name;
                var fieldToRequest = property.GetCustomAttribute<FieldToRequestAttribute>();
                var isRelation = dbInfo.IsRelationShip;
                headers.Add(column.Split('/')[1]);
                _alias = "";

                if (dbInfo.Name == "tblLeads")
                {
                    if (fieldName != "leadID")
                    {
                        if (fieldName.IndexOf("ID") == -1)
                        {
                            select += dbInfo.Name + "." + fieldName;
                        }
                        else
                        {
                            _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;
                            select += dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName + "." + fieldToRequest.Name;

                            if (_aliases.Count(m => m.Key == _alias) == 0)
                            {
                                _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.Name + "." + fieldName + " = " + dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName)));
                                from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                    + " on " +
                                    (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                    + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                            }
                        }
                    }
                }
                else
                {
                    if (fieldName.IndexOf("ID") == -1 || fieldName == "reservationID")
                    {
                        _alias = dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName;
                        select += dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName + "." + fieldName;

                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.Name, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + dbInfo.ForeignKeyFieldName)));
                            from += " left outer join " + dbInfo.Name + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.PrimaryKeyTableName) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.PrimaryKeyTableName).Key : dbInfo.PrimaryKeyTableName)
                                + "." + dbInfo.PrimaryKeyFieldName + " = " + _alias + "." + dbInfo.ForeignKeyFieldName;
                        }
                    }
                    else
                    {
                        _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;
                        select += dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName + "." + fieldToRequest.Name;

                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + fieldName)));
                            from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                        }
                    }
                }
                #endregion
            }

            foreach (var property in propertiesWithValue)
            {
                #region "where"
                var dbInfo = property.GetCustomAttribute<DataBaseInfoAttribute>();
                var fieldName = property.GetCustomAttribute<FieldInfoAttribute>().Name;
                var fieldToRequest = property.GetCustomAttribute<FieldToRequestAttribute>();
                var isRelation = dbInfo.IsRelationShip;
                var type = property.PropertyType;
                dynamic value = property.GetValue(model, null);
                dynamic value2;
                var alias = "";

                if (fieldName == "reservationID" || fieldName == "leadID")
                {
                    string v = Convert.ChangeType(value, TypeCode.String);
                    value = v.Length >= 36 ? v.Substring((v.Length - 36)) : value;
                }

                _alias = "";
                #region "alias"
                if (dbInfo.Name == "tblLeads")
                {
                    if (fieldName != "leadID")
                    {
                        alias = dbInfo.Name + "." + fieldName;
                        if (fieldName.IndexOf("ID") == -1)
                        {
                            //alias = dbInfo.Name + "." + fieldName;
                        }
                        else
                        {
                            _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;

                            if (_aliases.Count(m => m.Key == _alias) == 0)
                            {
                                _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.Name + "." + fieldName + " = " + dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName)));
                                from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                    + " on " +
                                    (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                    + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                            }
                        }
                    }
                    else
                    {
                        alias = "tblLeads.leadID";
                    }
                }
                else
                {
                    if (fieldName.IndexOf("ID") == -1 || fieldName == "reservationID")
                    {
                        alias = dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName + "." + fieldName;
                        _alias = dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName;
                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.Name, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + fieldName)));
                            from += " left outer join " + dbInfo.Name + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.PrimaryKeyTableName) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.PrimaryKeyTableName).Key : dbInfo.PrimaryKeyTableName)
                                + "." + dbInfo.PrimaryKeyFieldName + " = " + _alias + "." + dbInfo.ForeignKeyFieldName;
                        }
                    }
                    else
                    {
                        alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName + "." + fieldName;
                        _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;
                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + fieldName)));
                            from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                        }
                    }
                }
                #endregion

                //get field type to infer data type and define condition structure (array, int, guid, range, etc)
                if (type.IsArray)
                {
                    var _type = type.FullName.Split('.')[1];
                    var _value = "";

                    foreach (var v in value)
                    {
                        _value += (_type == "Guid[]" ? "'" : "") + ReportDataModel.ReportsCatalogs.SafeDataBaseValue(v) + (_type == "Guid[]" ? "'," : ",");
                    }
                    _value = _value.TrimEnd(',');

                    //where += (where == " where " ? "" : " AND ") + alias + " IN(" + _value + ")";
                    where += (where == " where " ? "" : " AND ") + (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name) + "." + fieldName + " IN(" + _value + ")";
                }
                else if (property.PropertyType == typeof(Boolean))
                {
                    bool boolValue = Convert.ToBoolean(value);
                    int intValue = Convert.ToInt32(boolValue);
                    where += (where == " where " ? "" : " AND ") + alias + (intValue == 1 ? "=" + intValue : "!=1");
                }
                else
                {
                    if (property.Name.IndexOf("_I_") > 0)//Date range
                    {
                        var _property2 = property.Name.Replace("_I_", "_F_");
                        value2 = model.GetType().GetProperty(_property2).GetValue(model, null);
                        var iDate = DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        var tDate = DateTime.Parse(value2.ToString(), CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

                        where += (where == " where " ? "" : " AND ") + alias + " BETWEEN '" + iDate + "' AND '" + tDate + "'";
                    }
                    else
                    {
                        where += (where == " where " ? "" : " AND ") + alias + " LIKE '%" + value + "%'";
                    }
                }

                #endregion
            }

            #region "connection & iteration"
            string sqlQuery = select + from + where;

            //var connString = "server=162.252.82.226;database=ePlat;user ID=sa;Password=c53613b6-c8b8-400d-95c6-274e6e60a14a;";
            var connString = "server=66.165.226.106;database=ePlat;user ID=sa;Password=c53613b6-c8b8-400d-95c6-274e6e60a14a;";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(sqlQuery, conn);
                    command.CommandTimeout = 180;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var header = 0;
                        var columns = new List<KeyValuePair<string, string>>();

                        if (rows.Where(m => m.Count(x => x.Value == row[dataTable.Columns[0]].ToString()) > 0).Count() > 0)
                        {

                        }
                        else
                        {
                            foreach (DataColumn column in dataTable.Columns)
                            {

                                var col = row[column].GetType().Name == "DateTime" ? DateTime.Parse(row[column].ToString()).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : row[column].ToString();
                                col = col.Replace(",", ";");

                                if (column.ColumnName == "phone" || column.ColumnName == "email")//avoid display of contact information
                                {
                                    col = "";
                                }

                                columns.Add(new KeyValuePair<string, string>(headers[header], col));
                                header++;
                            }

                            rows.Add(new List<KeyValuePair<string, string>>(columns));
                        }
                    }
                }
                catch { }
                finally
                {
                    conn.Close();
                    dataTable.Dispose();
                }
            }
            //rows = rows.Distinct().ToList();
            results.Results = rows;
            #endregion

            return results;
        }

        public PreArrivalSearchResultsModel _currentSearchPreArrival(PreArrivalSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            var results = new PreArrivalSearchResultsModel();
            DataTable dataTable = new DataTable();
            List<string> headers = new List<string>() { "ID" };
            List<List<KeyValuePair<string, string>>> rows = new List<List<KeyValuePair<string, string>>>();
            var select = @"select distinct tblLeads.leadID";
            var from = " from tblLeads ";
            var where = " where ";
            var _aliases = new List<KeyValuePair<string, KeyValuePair<string, string>>>();
            var _alias = "";

            var propertiesForColumns = model.GetType().GetProperties().Where(m => Reflection.HasCustomAttribute<DataBaseInfoAttribute>(m));
            var propertiesWithValue = propertiesForColumns.Where(m => m.GetValue(model, null) != null);

            #region "limitation of query"
            var limit = false;
            if (session.WorkGroupID == 8)
            {
                var isAgent = db.aspnet_Roles.Single(m => m.RoleId == session.RoleID).RoleName == "Agent";
                if (isAgent)
                {
                    model.Search_AssignedToUser = new Guid[] { session.UserID };
                }
            }
            model.Search_Terminal = model.Search_Terminal ?? session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            //model.IsTest = false;

            if (!GeneralFunctions.IsUserInRole("Administrator"))
            {
                model.IsTest = false;
            }

            if (propertiesWithValue.Count(m => m.Name.IndexOf("_I_") != -1) == 0 && model.Search_ReservationID == null && model.Search_FirstName == null && model.Search_LastName == null && model.Search_HotelConfirmationNumber == null && model.Search_CertificateNumber == null)
            {
                limit = true;
                model.Search_I_ArrivalDate = model.Search_I_ArrivalDate ?? DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                model.Search_F_ArrivalDate = model.Search_F_ArrivalDate ?? DateTime.Today.AddDays(62).AddSeconds(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            #endregion

            foreach (var column in model.Search_Columns.Split(','))
            {
                #region "select & from"
                select += ",";
                var property = propertiesForColumns.Where(m => column.Contains(Reflection.GetCustomAttribute<FieldInfoAttribute>(m).Name) && column.Replace("+", " ").Contains(Reflection.GetCustomAttribute<DisplayAttribute>(m).Name)).FirstOrDefault();
                var dbInfo = property.GetCustomAttribute<DataBaseInfoAttribute>();
                var fieldName = property.GetCustomAttribute<FieldInfoAttribute>().Name;
                var fieldToRequest = property.GetCustomAttribute<FieldToRequestAttribute>();
                var isRelation = dbInfo.IsRelationShip;
                headers.Add(column.Split('/')[1]);
                _alias = "";

                if (dbInfo.Name == "tblLeads")
                {
                    if (fieldName != "leadID")
                    {
                        if (fieldName.IndexOf("ID") == -1)
                        {
                            select += dbInfo.Name + "." + fieldName;
                        }
                        else
                        {
                            _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;
                            select += dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName + "." + fieldToRequest.Name;

                            if (_aliases.Count(m => m.Key == _alias) == 0)
                            {
                                _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.Name + "." + fieldName + " = " + dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName)));
                                from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                    + " on " +
                                    (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                    + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                            }
                        }
                    }
                }
                else
                {
                    if (fieldName.IndexOf("ID") == -1 || fieldName == "reservationID")
                    {
                        _alias = dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName;
                        select += dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName + "." + fieldName;

                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.Name, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + dbInfo.ForeignKeyFieldName)));
                            from += " left outer join " + dbInfo.Name + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.PrimaryKeyTableName) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.PrimaryKeyTableName).Key : dbInfo.PrimaryKeyTableName)
                                + "." + dbInfo.PrimaryKeyFieldName + " = " + _alias + "." + dbInfo.ForeignKeyFieldName;
                        }
                    }
                    else
                    {
                        _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;
                        select += dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName + "." + fieldToRequest.Name;

                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + fieldName)));
                            from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                        }
                    }
                }
                #endregion
            }

            foreach (var property in propertiesWithValue)
            {
                #region "where"
                var dbInfo = property.GetCustomAttribute<DataBaseInfoAttribute>();
                var fieldName = property.GetCustomAttribute<FieldInfoAttribute>().Name;
                var fieldToRequest = property.GetCustomAttribute<FieldToRequestAttribute>();
                var isRelation = dbInfo.IsRelationShip;
                var type = property.PropertyType;
                dynamic value = property.GetValue(model, null);
                dynamic value2;
                var alias = "";

                if (fieldName == "reservationID" || fieldName == "leadID")
                {
                    string v = Convert.ChangeType(value, TypeCode.String);
                    value = v.Length >= 36 ? v.Substring((v.Length - 36)) : value;
                }

                _alias = "";
                #region "alias"
                if (dbInfo.Name == "tblLeads")
                {
                    if (fieldName != "leadID")
                    {
                        alias = dbInfo.Name + "." + fieldName;
                        if (fieldName.IndexOf("ID") == -1)
                        {
                            //alias = dbInfo.Name + "." + fieldName;
                        }
                        else
                        {
                            _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;

                            if (_aliases.Count(m => m.Key == _alias) == 0)
                            {
                                _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.Name + "." + fieldName + " = " + dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName)));
                                from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                    + " on " +
                                    (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                    + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                            }
                        }
                    }
                    else
                    {
                        alias = "tblLeads.leadID";
                    }
                }
                else
                {
                    if (fieldName.IndexOf("ID") == -1 || fieldName == "reservationID")
                    {
                        alias = dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName + "." + fieldName;
                        _alias = dbInfo.Name + "_" + dbInfo.PrimaryKeyTableName;
                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.Name, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + fieldName)));
                            from += " left outer join " + dbInfo.Name + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.PrimaryKeyTableName) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.PrimaryKeyTableName).Key : dbInfo.PrimaryKeyTableName)
                                + "." + dbInfo.PrimaryKeyFieldName + " = " + _alias + "." + dbInfo.ForeignKeyFieldName;
                        }
                    }
                    else
                    {
                        alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName + "." + fieldName;
                        _alias = dbInfo.PrimaryKeyTableName + "_" + dbInfo.Name + "_" + fieldName;
                        if (_aliases.Count(m => m.Key == _alias) == 0)
                        {
                            _aliases.Add(new KeyValuePair<string, KeyValuePair<string, string>>(_alias, new KeyValuePair<string, string>(dbInfo.PrimaryKeyTableName, dbInfo.PrimaryKeyTableName + "." + dbInfo.PrimaryKeyFieldName + " = " + dbInfo.Name + "." + fieldName)));
                            from += " left outer join " + dbInfo.PrimaryKeyTableName + " as " + _alias
                                + " on " +
                                (_aliases.Count(m => m.Value.Key == dbInfo.Name) > 0 ? _aliases.FirstOrDefault(m => m.Value.Key == dbInfo.Name).Key : dbInfo.Name)
                                + "." + fieldName + " = " + _alias + "." + dbInfo.PrimaryKeyFieldName;
                        }
                    }
                }
                #endregion

                //get field type to infer data type and define condition structure (array, int, guid, range, etc)
                //if (type.IsArray)
                if (type.IsArray)
                {
                    var _type = type.FullName.Split('.')[1];
                    var _value = "";

                    foreach (var v in value)
                    {
                        _value += (_type == "Guid[]" ? "'" : "") + ReportDataModel.ReportsCatalogs.SafeDataBaseValue(v) + (_type == "Guid[]" ? "'," : ",");
                    }
                    _value = _value.TrimEnd(',');

                    where += (where == " where " ? "" : " AND ") + alias + " IN(" + _value + ")";
                }
                else if (property.PropertyType == typeof(Boolean))
                {
                    bool boolValue = Convert.ToBoolean(value);
                    int intValue = Convert.ToInt32(boolValue);
                    where += (where == " where " ? "" : " AND ") + alias + (intValue == 1 ? "=" + intValue : "!=1");
                }
                else
                {
                    if (property.Name.IndexOf("_I_") > 0)//Date range
                    {
                        var _property2 = property.Name.Replace("_I_", "_F_");
                        value2 = model.GetType().GetProperty(_property2).GetValue(model, null);
                        var iDate = DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        var tDate = DateTime.Parse(value2.ToString(), CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

                        where += (where == " where " ? "" : " AND ") + alias + " BETWEEN '" + iDate + "' AND '" + tDate + "'";
                    }
                    else
                    {
                        where += (where == " where " ? "" : " AND ") + alias + " LIKE '%" + value + "%'";
                    }
                }

                #endregion
            }

            #region "connection & iteration"
            string sqlQuery = select + from + where;

            //var connString = "server=162.252.82.226;database=ePlat;user ID=sa;Password=c53613b6-c8b8-400d-95c6-274e6e60a14a;";
            var connString = "server=66.165.226.106;database=ePlat;user ID=sa;Password=c53613b6-c8b8-400d-95c6-274e6e60a14a;";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(sqlQuery, conn);
                    command.CommandTimeout = 180;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var header = 0;
                        var columns = new List<KeyValuePair<string, string>>();

                        if (rows.Where(m => m.Count(x => x.Value == row[dataTable.Columns[0]].ToString()) > 0).Count() > 0)
                        {

                        }
                        else
                        {
                            foreach (DataColumn column in dataTable.Columns)
                            {

                                var col = row[column].GetType().Name == "DateTime" ? DateTime.Parse(row[column].ToString()).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : row[column].ToString();
                                col = col.Replace(",", ";");

                                if (column.ColumnName == "phone" || column.ColumnName == "email")//avoid display of contact information
                                {
                                    col = "";
                                }

                                columns.Add(new KeyValuePair<string, string>(headers[header], col));
                                header++;
                            }

                            rows.Add(new List<KeyValuePair<string, string>>(columns));
                        }
                    }
                }
                catch { }
                finally
                {
                    conn.Close();
                    dataTable.Dispose();
                }
            }
            //rows = rows.Distinct().ToList();
            results.Results = rows;
            #endregion

            return results;
        }

        public object GetTransactionsHistory(int id)
        {
            ePlatEntities db = new ePlatEntities();

            var _query = (from m in db.tblMoneyTransactions
                            where m.billingInfoID == id
                            select new
                            {
                                m.moneyTransactionID,
                                m.authCode,
                                m.authDate,
                                m.errorCode,
                                transactionDate = m.transactionDate,
                                billingName = m.tblMerchantAccounts.merchantAccountBillingName
                            }).ToList();

            var query = (from m in _query
                        select new 
                        {
                            moneyTransactionID = m.moneyTransactionID,
                            authCode = m.authCode,
                            authDate = m.authDate != null ?m.authDate.Value.ToString("yyyy-MM-dd") : "",
                            errorCode = m.errorCode != null ? RescomDataModel.ApplyPayment_ErrorCodes.FirstOrDefault(x => x.Key == int.Parse(m.errorCode)).Value : "",
                            transactionDate = m.transactionDate.ToString("yyyy-MM-dd"),
                            billingName = m.billingName
                        }).ToList();

            return query;
        }

        public PreArrivalResultModel GetLead(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalResultModel response = new PreArrivalResultModel();
            var status = "";
            try
            {
                response.ListPreArrivalEmails = new List<PreArrivalEmailsInfoModel>();
                response.ListPreArrivalPhones = new List<PreArrivalPhonesInfoModel>();
                response.ListPreArrivalBillings = new List<BillingResultsModel>();
                response.PreArrivalInteractions = new List<InteractionResultsModel>();
                response.PreArrivalMemberInfoModel = new PreArrivalMemberInfoModel();
                response.ListPreArrivalReservations = new List<ReservationResultsModel>();

                var lead = (from _lead in db.tblLeads
                            join member in db.tblMemberInfo on _lead.leadID equals member.leadID into lead_member
                            from member in lead_member.DefaultIfEmpty()
                            where _lead.leadID == id
                            select new
                            {
                                _lead.leadID,
                                _lead.firstName,
                                _lead.lastName,
                                _lead.terminalID,
                                _lead.leadStatusID,
                                _lead.leadStatusDescription,
                                _lead.callClasificationID,
                                _lead.leadSourceID,
                                _lead.secondaryBookingStatusID,
                                _lead.timeZoneID,
                                _lead.address,
                                _lead.city,
                                _lead.state,
                                _lead.zipcode,
                                _lead.countryID,
                                _lead.frontOfficeResortID,
                                _lead.tblBillingInfo,
                                //_lead.tblLeadEmails,
                                //_lead.tblPhones,
                                contractNumber = member != null ? member.contractNumber : null,
                                coOwner = member != null ? member.coOwner : null,
                                clubType = member != null ? member.clubType : null,
                                memberName = member != null ? member.memberName : null,
                                memberNumber = member != null ? member.memberNumber : null,
                                _lead.tblReservations
                            }).ToList().FirstOrDefault();

                #region "General Info"
                response.Info_LeadID = lead.leadID.ToString();
                response.Info_FirstName = lead.firstName;
                response.Info_LastName = lead.lastName;
                response.Info_Terminal = lead.terminalID.ToString();
                response.Info_LeadStatus = lead.leadStatusID != null ? lead.leadStatusID.ToString() : "0";
                response.Info_LeadStatusDescription = lead.leadStatusDescription;
                response.Info_CallClasification = lead.callClasificationID != null ? lead.callClasificationID.ToString() : "0";
                response.Info_LeadSource = lead.leadSourceID != null ? lead.leadSourceID.ToString() : "0";
                response.Info_SecondaryBookingStatus = lead.secondaryBookingStatusID != null ? lead.secondaryBookingStatusID.ToString() : "0";
                response.Info_TimeZone = lead.timeZoneID != null ? lead.timeZoneID.ToString() : "0";
                response.Info_Address = lead.address;
                response.Info_City = lead.city;
                response.Info_State = lead.state;
                response.Info_ZipCode = lead.zipcode;
                response.Info_Country = lead.countryID != null ? lead.countryID.ToString() : "0";
                #endregion

                status = "True";// GetContactInfo(id);

                #region "Email Info"
                var emails = db.tblLeadEmails.Where(m => m.leadID == lead.leadID);
                if (emails.Count() > 0)
                {
                    foreach (var i in emails)
                    {
                        response.ListPreArrivalEmails.Add(new PreArrivalEmailsInfoModel()
                        {
                            EmailsInfo_LeadEmailID = i.emailID,
                            EmailsInfo_LeadID = i.leadID,
                            EmailsInfo_Email = GeneralFunctions.MaskEmail(i.email),
                            EmailsInfo_Main = i.main
                        });
                    }
                }
                #endregion

                #region "Phone Info"
                var phones = db.tblPhones.Where(m => m.leadID == lead.leadID);
                if (phones.Count() > 0)
                {
                    foreach (var i in phones)
                    {
                        response.ListPreArrivalPhones.Add(new PreArrivalPhonesInfoModel()
                        {
                            PhonesInfo_LeadID = i.leadID,
                            PhonesInfo_LeadPhoneID = i.phoneID,
                            PhonesInfo_PhoneTypeText = i.tblPhoneTypes.phoneType,
                            PhonesInfo_PhoneNumber = GeneralFunctions.MaskPhone(i.phone),
                            PhonesInfo_Main = i.main,
                            PhonesInfo_DoNotCall = i.doNotCall
                        });
                    }
                }
                #endregion

                #region "Billing Info"
                //var billingInfo = db.tblBillingInfo.Where(m => m.leadID == lead.leadID).ToList();
                var billingInfo = lead.tblBillingInfo;
                var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
                foreach (var billing in billingInfo)
                {
                    var cc = mexHash.mexHash.DecryptString(billing.cardNumber);
                    response.ListPreArrivalBillings.Add(new BillingResultsModel()
                    {
                        BillingInfo_BillingInfoID = billing.billingInfoID.ToString(),
                        //BillingInfo_LeadID = billing.leadID.ToString(),
                        BillingInfo_CardHolderName = billing.cardHolderName,
                        BillingInfo_CardType = billing.tblCardTypes.cardType,
                        BillingInfo_CardNumber = cc.Length >= 15 ? "************" + cc.Substring(cc.Length - 4) : cc,
                        BillingInfo_CardExpiry = billing.cardExpiry,
                        BillingInfo_CardCVV = billing.cardCVV != null && billing.cardCVV.Length >= 3 ? "**" + billing.cardCVV.Substring(billing.cardCVV.Length - 2) : billing.cardCVV,
                        BillingInfo_ShowInfo = lead.terminalID != 10 || isAdmin,
                        BillingInfo_IsAdmin = isAdmin
                    });
                }
                #endregion

                #region "Interactions Info"
                response.PreArrivalInteractions = new InteractionDataModel().GetInteractions(id);
                #endregion

                #region "Member Info"
                response.PreArrivalMemberInfoModel = new PreArrivalMemberInfoModel()
                {
                    MemberInfo_ContractNumber = lead.contractNumber,
                    MemberInfo_CoOwner = lead.coOwner,
                    MemberInfo_ClubType = lead.clubType,
                    MemberInfo_MemberName = lead.memberName,
                    MemberInfo_MemberNumber = lead.memberNumber
                };
                #endregion

                #region "Reservation Info"
                //linea comentada para que el guardado de cambios en la reservacion se aplique al registro de la base de datos.
                //var reservations = lead.tblReservations.Where(m => m.leadID == lead.leadID && m.isTest != true).ToList();
                var reservations = lead.tblReservations.Where(m => m.leadID == lead.leadID && m.isTest != true);
                foreach (var rsv in reservations.OrderByDescending(m => m.arrivalDate))
                {
                    //try
                    //{
                    //    var _existing = FrontOfficeDataModel.GetArrivals((int)rsv.tblPlaces.frontOfficeResortID, (DateTime)rsv.arrivalDate, ((DateTime)rsv.arrivalDate).AddDays(1).AddSeconds(-1)).FirstOrDefault(m => m.idresort == rsv.tblLeads.frontOfficeResortID && m.idReservacion == rsv.frontOfficeReservationID && (m.idroomlist == null || rsv.frontOfficeRoomListID == m.idroomlist));

                    //    #region "lead status update"
                    //    if (_existing != null && _existing.codigostatusreservacion != null)
                    //    {
                    //        if (rsv.reservationStatusID != Dictionaries.FrontReservationStatus.Single(m => m.Key == _existing.codigostatusreservacion.Trim()).Value)
                    //        {
                    //            rsv.reservationStatusID = Dictionaries.FrontReservationStatus.Single(m => m.Key == _existing.codigostatusreservacion.Trim()).Value;
                    //            rsv.reservationStatusDate = DateTime.Now;
                    //        }

                    //        if (_existing.codigostatusreservacion.Trim() == "CA")
                    //        {
                    //            if (rsv.tblLeads.tblReservations.Count(m => m.reservationStatusID == 3) == rsv.tblLeads.tblReservations.Count())
                    //            {
                    //                if (rsv.tblLeads.leadStatusID != 35)
                    //                {
                    //                    rsv.tblLeads.leadStatusID = 4;
                    //                }
                    //            }
                    //            else if (rsv.tblLeads.leadStatusID == 4)
                    //            {
                    //                rsv.tblLeads.leadStatusID = 2;
                    //            }
                    //            else
                    //            {
                    //                rsv.tblLeads.leadStatusID = rsv.tblLeads.leadStatusID;
                    //            }
                    //        }
                    //        else if (rsv.tblLeads.leadStatusID == 4)
                    //        {
                    //            rsv.tblLeads.leadStatusID = 2;
                    //        }
                    //        else
                    //        {
                    //            rsv.tblLeads.leadStatusID = rsv.tblLeads.leadStatusID;
                    //        }
                    //        db.SaveChanges();
                    //    }
                    //    #endregion
                    //}
                    //catch { };

                    var distinctives = lead.frontOfficeResortID != null && rsv.frontOfficeReservationID != null ? FrontOfficeDataModel.GetDistinctives(lead.frontOfficeResortID, rsv.frontOfficeReservationID) : null;
                    var strDistinctives = "";
                    if (distinctives != null)
                    {
                        foreach (var i in distinctives)
                        {
                            strDistinctives += "&bull; " + i.NombreDeDistintivos + "<br />";
                        }
                    }
                    response.ListPreArrivalReservations.Add(new ReservationResultsModel()
                    {
                        ReservationInfo_ReservationID = rsv.reservationID.ToString(),
                        ReservationInfo_HotelConfirmationNumber = rsv.hotelConfirmationNumber,
                        ReservationInfo_FrontCertificateNumber = rsv.frontOfficeCertificateNumber ?? "",
                        ReservationInfo_ArrivalDate = rsv.arrivalDate != null ? ((DateTime)rsv.arrivalDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "",
                        ReservationInfo_DestinationText = rsv.destinationID != null ? rsv.tblDestinations.destination : "",
                        ReservationInfo_FoundInFront = rsv.foundInFront,
                        ReservationInfo_Distinctives = strDistinctives,
                        ReservationInfo_ReservationStatusText = rsv.reservationStatusID != null ? rsv.tblReservationStatus.reservationStatus : "",
                        ReservationInfo_OptionsSold = rsv.tblOptionsSold.Count(m => m.deleted != true) > 0 ? true : lead.terminalID == 10 ? PublicDataModel.GetResortConnectOptionals(rsv).Count() > 0 : false
                        //ReservationInfo_OptionsSold = rsv.tblOptionsSold.Count(m => m.deleted != true) > 0
                    });
                }
                #endregion
                response.Info_Response = status;
                return response;
            }
            catch (Exception e)
            {
                response.Info_Response = status;
                return response;
            }
        }

        public static string GetContactInfo(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            try
            {
                var lead = db.tblLeads.Single(m => m.leadID == id);
                var emails = lead.tblLeadEmails;
                var phones = lead.tblPhones;
                var resortID = lead.frontOfficeResortID;
                var guestID = lead.frontOfficeGuestID;

                if (emails.Count() == 0 || phones.Count() == 0)
                {
                    var contactInfo = FrontOfficeDataModel.GetContactInfo((int)resortID, guestID);
                    if (emails.Count() == 0 && contactInfo.Email != null)
                    {
                        var email = new tblLeadEmails();
                        email.leadID = id;
                        email.main = true;
                        email.email = contactInfo.Email;
                        db.tblLeadEmails.AddObject(email);
                        db.SaveChanges();
                    }

                    if (phones.Count() == 0 && contactInfo.Phone != null)
                    {
                        var phone = new tblPhones();
                        phone.leadID = id;
                        phone.phoneTypeID = 1;
                        phone.phone = contactInfo.Phone;
                        phone.main = true;
                        phone.ext = null;
                        phone.doNotCall = false;
                        db.tblPhones.AddObject(phone);
                        db.SaveChanges();
                    }

                    //emails = lead.tblLeadEmails;
                    //phones = lead.tblPhones;

                    if (emails.Count() == 0 || phones.Count() == 0)//verificar si estas variables actualizan su contenido al guardar info en las tablas.
                    {
                        try
                        {
                            resortConnectEntities rce = new resortConnectEntities();
                            //var reservation = lead.tblReservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();
                            var reservations = lead.tblReservations.Where(m => m.reservationStatusID != 3);
                            var certificates = reservations.Select(m => m.frontOfficeCertificateNumber).ToList();
                            var reservation = reservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();
                            var rcResortID = reservation.tblPlaces.resortConnectResortID;
                            //var aDate = reservation.arrivalDate.Value.Date;
                            var aDate = reservations.OrderBy(m => m.arrivalDate).FirstOrDefault().arrivalDate.Value.Date;
                            //var dDate = reservation.departureDate != null ? reservation.departureDate.Value.Date : (DateTime?)null;
                            var dDate = reservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault().departureDate.Value.Date;

                            var rc = (from r in rce.Reservation
                                      join c in rce.Reservation_Contact on r.ReservationId equals c.ReservationId
                                      //where r.ConfirmationNumber.Contains(reservation.frontOfficeCertificateNumber)
                                      where certificates.Contains(r.ConfirmationNumber)
                                      && r.CheckInDate == aDate
                                      && (r.CheckOutDate == dDate || dDate == null)
                                      && (r.ResortNumber == rcResortID || rcResortID == null)
                                      select new
                                      {
                                          c.PhoneNumber,
                                          c.EmailAddress
                                      }).FirstOrDefault();

                            if (rc != null)
                            {
                                if (emails.Count() == 0 && rc.EmailAddress != null)
                                {
                                    var email = new tblLeadEmails();
                                    email.leadID = id;
                                    email.main = true;
                                    email.email = rc.EmailAddress;
                                    db.tblLeadEmails.AddObject(email);
                                    db.SaveChanges();
                                }
                                if (phones.Count() == 0 && rc.PhoneNumber != null)
                                {
                                    var phone = new tblPhones();
                                    phone.leadID = id;
                                    phone.phoneTypeID = 4;
                                    phone.phone = rc.PhoneNumber;
                                    phone.main = true;
                                    phone.ext = null;
                                    phone.doNotCall = false;
                                    db.tblPhones.AddObject(phone);
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message + " Inner Exception: " + e.InnerException ?? "");
                        }
                    }
                }
                return "True";
            }
            catch (Exception e) { return e.Message; }
        }

        public static FrontOfficeViewModel.ContactInfo GetContactInfo(ImportModel arrival)
        {
            ePlatEntities db = new ePlatEntities();
            var contactInfo = new FrontOfficeViewModel.ContactInfo();
            try
            {
                var resortID = arrival.idresort;
                var guestID = arrival.idhuesped;

                contactInfo = FrontOfficeDataModel.GetContactInfo((int)resortID, guestID);

                if (contactInfo.Email == null || contactInfo.Phone == null)
                {
                    resortConnectEntities rce = new resortConnectEntities();
                    var rcResortID = db.tblPlaces.FirstOrDefault(m => m.frontOfficeResortID == arrival.idresort).resortConnectResortID;
                    var rc = (from r in rce.Reservation
                              join c in rce.Reservation_Contact on r.ReservationId equals c.ReservationId
                              where r.ConfirmationNumber == arrival.CRS
                              && r.CheckInDate == arrival.llegada
                              && (r.CheckOutDate == arrival.salida || arrival.salida == null)
                              && (r.ResortNumber == rcResortID || rcResortID == null)
                              select new
                              {
                                  c.PhoneNumber,
                                  c.EmailAddress
                              }).FirstOrDefault();

                    if (rc != null)
                    {
                        if (contactInfo.Email == null)
                        {
                            if (rc.EmailAddress != null && Regex.IsMatch(rc.EmailAddress, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                            {
                                contactInfo.Email = rc.EmailAddress;
                            }
                        }
                        if (contactInfo.Phone == null)
                        {
                            if (rc.PhoneNumber != null && rc.PhoneNumber != "")
                            {
                                contactInfo.Phone = Regex.Replace(rc.PhoneNumber, @"[^\d]", "");
                            }
                        }
                    }
                }
            }
            catch { }
            return contactInfo;
        }

        public List<PreArrivalEmailsInfoModel> GetEmail(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            var listEmails = new List<PreArrivalEmailsInfoModel>();

            if (db.tblLeadEmails.Count(m => m.leadID == id) > 0)
            {
                foreach (var i in db.tblLeadEmails.Where(m => m.leadID == id))
                {
                    listEmails.Add(new PreArrivalEmailsInfoModel()
                    {
                        EmailsInfo_LeadEmailID = i.emailID,
                        EmailsInfo_Email = i.email,
                        EmailsInfo_LeadID = i.leadID,
                        EmailsInfo_Main = i.main
                    });
                }
            }
            else
            {
                var lead = db.tblLeads.Where(m => m.leadID == id).FirstOrDefault();
                if (lead.frontOfficeResortID == null)
                {
                    listEmails.Add(new PreArrivalEmailsInfoModel()
                    {
                        EmailsInfo_LeadEmailID = 0,
                        EmailsInfo_Email = "",
                        EmailsInfo_LeadID = Guid.Empty,
                        EmailsInfo_Main = false
                    });
                    return listEmails;
                }
                var info = FrontOfficeDataModel.GetContactInfo((int)lead.frontOfficeResortID, lead.frontOfficeGuestID);

                if (info.Email != null)
                {

                    var email = new tblLeadEmails();
                    email.leadID = id;
                    email.main = true;
                    email.email = info.Email;
                    db.tblLeadEmails.AddObject(email);

                    if (info.Phone != null)
                    {
                        var phone = new tblPhones();
                        phone.leadID = id;
                        phone.phoneTypeID = 1;
                        phone.phone = info.Phone;
                        phone.main = true;
                        phone.ext = null;
                        phone.doNotCall = false;
                        db.tblPhones.AddObject(phone);
                    }

                    db.SaveChanges();

                    listEmails.Add(new PreArrivalEmailsInfoModel()
                    {
                        EmailsInfo_LeadEmailID = email.emailID,
                        EmailsInfo_Email = email.email,
                        EmailsInfo_LeadID = email.leadID,
                        EmailsInfo_Main = email.main
                    });
                }
                else if (db.tblMemberContactInfo.Count(m => m.leadID == id) > 0)
                {
                    var contactInfo = db.tblMemberContactInfo.FirstOrDefault(m => m.leadID == id);
                    listEmails.Add(new PreArrivalEmailsInfoModel()
                    {
                        EmailsInfo_LeadEmailID = 0,
                        EmailsInfo_Email = mexHash.mexHash.DecryptString(contactInfo.email),
                        EmailsInfo_LeadID = id,
                        EmailsInfo_Main = true
                    });
                }
                else
                {
                    listEmails.Add(new PreArrivalEmailsInfoModel()
                    {
                        EmailsInfo_LeadEmailID = 0,
                        EmailsInfo_Email = "",
                        EmailsInfo_LeadID = Guid.Empty,
                        EmailsInfo_Main = false
                    });
                }
            }

            return listEmails;
        }

        public List<PreArrivalPhonesInfoModel> GetPhone(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            var listPhones = new List<PreArrivalPhonesInfoModel>();

            var show = AdminDataModel.GetViewPrivileges(12021);
            if (!show.FirstOrDefault().View)
            {
                return listPhones;
            }
            if (db.tblPhones.Count(m => m.leadID == id) > 0)
            {
                foreach (var i in db.tblPhones.Where(m => m.leadID == id))
                {
                    listPhones.Add(new PreArrivalPhonesInfoModel()
                    {
                        PhonesInfo_LeadPhoneID = i.phoneID,
                        PhonesInfo_LeadID = i.leadID,
                        PhonesInfo_PhoneType = i.phoneTypeID,
                        PhonesInfo_PhoneNumber = i.phone,
                        PhonesInfo_ExtensionNumber = i.ext,
                        PhonesInfo_DoNotCall = i.doNotCall,
                        PhonesInfo_Main = i.main
                    });
                }
            }
            else
            {
                var lead = db.tblLeads.Where(m => m.leadID == id).FirstOrDefault();
                var info = FrontOfficeDataModel.GetContactInfo((int)lead.frontOfficeResortID, lead.frontOfficeGuestID);

                if (info.Phone != null)
                {
                    if (info.Email != null)
                    {
                        var email = new tblLeadEmails();
                        email.leadID = id;
                        email.main = true;
                        email.email = info.Email;
                        db.tblLeadEmails.AddObject(email);
                    }

                    var phone = new tblPhones();
                    phone.leadID = id;
                    phone.phoneTypeID = 1;
                    phone.phone = info.Phone;
                    phone.main = true;
                    phone.ext = null;
                    phone.doNotCall = false;
                    db.tblPhones.AddObject(phone);

                    db.SaveChanges();

                    listPhones.Add(new PreArrivalPhonesInfoModel()
                    {
                        PhonesInfo_LeadPhoneID = phone.phoneID,
                        PhonesInfo_LeadID = phone.leadID,
                        PhonesInfo_PhoneType = phone.phoneTypeID,
                        PhonesInfo_PhoneNumber = phone.phone,
                        PhonesInfo_ExtensionNumber = phone.ext,
                        PhonesInfo_DoNotCall = phone.doNotCall,
                        PhonesInfo_Main = phone.main
                    });
                }
                else if (db.tblMemberContactInfo.Count(m => m.leadID == id) > 0)
                {
                    var contactInfo = db.tblMemberContactInfo.FirstOrDefault(m => m.leadID == id);
                    listPhones.Add(new PreArrivalPhonesInfoModel()
                    {
                        PhonesInfo_LeadPhoneID = 0,
                        PhonesInfo_LeadID = id,
                        PhonesInfo_PhoneType = contactInfo.phoneType != null && contactInfo.phoneType != "null" ? db.tblPhoneTypes.FirstOrDefault(m => m.phoneType == contactInfo.phoneType).phoneTypeID : 0,
                        PhonesInfo_PhoneNumber = mexHash.mexHash.DecryptString(contactInfo.phone),
                        PhonesInfo_ExtensionNumber = "",
                        PhonesInfo_DoNotCall = false,
                        PhonesInfo_Main = true
                    });
                }
                else
                {
                    listPhones.Add(new PreArrivalPhonesInfoModel()
                    {
                        PhonesInfo_LeadPhoneID = 0,
                        PhonesInfo_LeadID = Guid.Empty,
                        PhonesInfo_PhoneType = 0,
                        PhonesInfo_PhoneNumber = "",
                        PhonesInfo_ExtensionNumber = "",
                        PhonesInfo_DoNotCall = false,
                        PhonesInfo_Main = false
                    });
                }
            }

            return listPhones;
        }

        public PreArrivalBillingModel GetBilling(long id)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalBillingModel response = new PreArrivalBillingModel();
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");

            var query = (from f in db.tblBillingInfo
                         where f.billingInfoID == id
                         select f).FirstOrDefault();
            var cc = mexHash.mexHash.DecryptString(query.cardNumber);
            response.BillingInfo_BillingInfoID = query.billingInfoID;
            response.BillingInfo_LeadID = query.leadID;
            response.BillingInfo_FirstName = query.firstName;
            response.BillingInfo_LastName = query.lastName;
            response.BillingInfo_Address = query.address;
            response.BillingInfo_Country = (int)query.countryID;
            response.BillingInfo_ZipCode = query.zipcode;
            response.BillingInfo_CardHolderName = query.cardHolderName;
            //response.BillingInfo_CardNumber = cc != null && cc.Length >= 15 ? "************" + cc.Substring(cc.Length - 4) : cc;
            response.BillingInfo_CardNumber = cc != null && cc.Length >= 15 && !isAdmin ? "************" + cc.Substring(cc.Length - 4) : cc;
            response.BillingInfo_CardType = query.cardTypeID;
            response.BillingInfo_CardExpiry = query.cardExpiry;
            //response.BillingInfo_CardCVV = query.cardCVV != null && query.cardCVV.Length >= 3 ? "**" + query.cardCVV.Substring(query.cardCVV.Length - 2) : query.cardCVV;
            response.BillingInfo_CardCVV = query.cardCVV != null && query.cardCVV.Length >= 3 && !isAdmin ? "**" + query.cardCVV.Substring(query.cardCVV.Length - 2) : query.cardCVV;
            response.BillingInfo_Comments = query.billingComments;

            return response;
        }

        public AttemptResponse SaveLead(PreArrivalInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            Guid currentUser = session.UserID;
            DateTime now = DateTime.Now;

            ChangesTracking.ChangeItem changeItem = new ChangesTracking.ChangeItem();
            List<ChangesTracking.ChangeItem> listChanges = new List<ChangesTracking.ChangeItem>();

            if (model.Info_LeadID != Guid.Empty && model.Info_DuplicateLead != true)
            {
                #region "update"
                try
                {
                    var query = db.tblLeads.Single(m => m.leadID == model.Info_LeadID);
                    query.firstName = model.Info_FirstName;
                    query.lastName = model.Info_LastName;
                    query.terminalID = model.Info_Terminal;
                    query.leadStatusID = model.Info_LeadStatus != 0 ? model.Info_LeadStatus : (int?)null;
                    query.leadStatusDescription = model.Info_LeadStatusDescription;
                    query.leadSourceID = model.Info_LeadSource != 0 ? model.Info_LeadSource : (long?)null;
                    //query.secondaryBookingStatusID = model.Info_SecondaryBookingStatus != 0 ? model.Info_SecondaryBookingStatus : query.secondaryBookingStatusID;
                    query.callClasificationID = model.Info_CallClasification != 0 ? model.Info_CallClasification : (int?)null;
                    query.timeZoneID = model.Info_TimeZone != 0 ? model.Info_TimeZone : (int?)null;
                    query.address = model.Info_Address;
                    query.city = model.Info_City;
                    query.state = model.Info_State;
                    query.zipcode = model.Info_ZipCode;
                    query.countryID = model.Info_Country != 0 ? model.Info_Country : (int?)null;
                    query.modificationDate = now;
                    query.modifiedByUserID = session.UserID;

                    int counter;
                    if (model.PreArrivalEmails != null && model.PreArrivalEmails != "")
                    {
                        counter = 1;
                        List<PreArrivalEmailsInfoModel> modelEmails = new JavaScriptSerializer().Deserialize(model.PreArrivalEmails, typeof(List<PreArrivalEmailsInfoModel>)) as List<PreArrivalEmailsInfoModel>;
                        var savedEmails = query.tblLeadEmails.Select(m => new PreArrivalEmailsInfoModel { EmailsInfo_LeadEmailID = m.emailID, EmailsInfo_LeadID = m.leadID, EmailsInfo_Email = m.email, EmailsInfo_Main = m.main }).ToList();
                        if (modelEmails.Count(m => m.EmailsInfo_Main == true) > 0)
                        {
                            counter = 0;
                        }
                        foreach (var x in savedEmails)
                        {
                            if (counter == 0)
                            {
                                x.EmailsInfo_Main = false;
                            }
                            if (modelEmails.Count(m => m.EmailsInfo_LeadEmailID == x.EmailsInfo_LeadEmailID) == 0)
                            {
                                modelEmails.Add(x);
                            }
                        }
                        counter = 1;
                        foreach (var i in modelEmails.OrderByDescending(m => m.EmailsInfo_Main))
                        {
                            if (i.EmailsInfo_LeadEmailID == 0)
                            {
                                if (i.EmailsInfo_Email.IndexOf("&bull;") == -1 && Regex.IsMatch(i.EmailsInfo_Email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                                {
                                    var email = new tblLeadEmails();
                                    email.email = i.EmailsInfo_Email;
                                    email.main = counter == 1 ? true : false;
                                    email.bounced = false;
                                    email.dnc = false;
                                    query.tblLeadEmails.Add(email);
                                }
                            }
                            else
                            {
                                var email = query.tblLeadEmails.Single(m => m.emailID == i.EmailsInfo_LeadEmailID);
                                if (email.email != i.EmailsInfo_Email || email.main != i.EmailsInfo_Main)
                                {
                                    email.email = i.EmailsInfo_Email;
                                    email.main = counter == 1 ? true : false;
                                    email.dateLastModification = now;
                                    email.modifiedByUserID = session.UserID;
                                }
                            }
                            counter++;
                        }
                        query.bookingStatusID = query.bookingStatusID == 15 ? 10 : query.bookingStatusID;
                    }
                    if (model.PreArrivalPhones != null && model.PreArrivalPhones != "")
                    {
                        counter = 1;
                        List<PreArrivalPhonesInfoModel> phones = new JavaScriptSerializer().Deserialize(model.PreArrivalPhones, typeof(List<PreArrivalPhonesInfoModel>)) as List<PreArrivalPhonesInfoModel>;
                        var savedPhones = query.tblPhones.Select(m => new PreArrivalPhonesInfoModel { PhonesInfo_LeadPhoneID = m.phoneID, PhonesInfo_LeadID = m.leadID, PhonesInfo_PhoneType = m.phoneTypeID, PhonesInfo_PhoneNumber = m.phone, PhonesInfo_ExtensionNumber = m.ext, PhonesInfo_Main = m.main, PhonesInfo_DoNotCall = m.doNotCall }).ToList();
                        if (phones.Count(m => m.PhonesInfo_Main == true) > 0)
                        {
                            counter = 0;
                        }
                        foreach (var x in savedPhones)
                        {
                            if (counter == 0)
                            {
                                x.PhonesInfo_Main = false;
                            }
                            if (phones.Count(m => m.PhonesInfo_LeadPhoneID == x.PhonesInfo_LeadPhoneID) == 0)
                            {
                                phones.Add(x);
                            }
                        }
                        counter = 1;
                        foreach (var i in phones.OrderByDescending(m => m.PhonesInfo_Main))
                        {
                            i.PhonesInfo_PhoneNumber = Regex.Replace(i.PhonesInfo_PhoneNumber, @"[^\d]", "");
                            if (i.PhonesInfo_LeadPhoneID == 0)
                            {
                                if (i.PhonesInfo_PhoneNumber.IndexOf("&bull;") == -1 && i.PhonesInfo_PhoneNumber.Length >= 10)
                                {
                                    var phone = new tblPhones();
                                    phone.phone = i.PhonesInfo_PhoneNumber.Substring(i.PhonesInfo_PhoneNumber.Length - 10);
                                    phone.phoneTypeID = i.PhonesInfo_PhoneType;
                                    phone.ext = i.PhonesInfo_ExtensionNumber;
                                    phone.doNotCall = i.PhonesInfo_DoNotCall;
                                    phone.main = counter == 1 ? true : false;
                                    query.tblPhones.Add(phone);
                                }
                            }
                            else
                            {
                                var phone = query.tblPhones.Single(m => m.phoneID == i.PhonesInfo_LeadPhoneID);
                                if (phone.phone != i.PhonesInfo_PhoneNumber || phone.phoneTypeID != i.PhonesInfo_PhoneType || phone.ext != i.PhonesInfo_ExtensionNumber
                                    || phone.doNotCall != i.PhonesInfo_DoNotCall || phone.main != i.PhonesInfo_Main)
                                {
                                    phone.phone = i.PhonesInfo_PhoneNumber;
                                    phone.phoneTypeID = i.PhonesInfo_PhoneType;
                                    phone.ext = i.PhonesInfo_ExtensionNumber;
                                    phone.doNotCall = i.PhonesInfo_DoNotCall;
                                    phone.main = counter == 1 ? true : false;
                                    phone.dateLastModification = now;
                                    phone.modifiedByUserID = session.UserID;
                                }
                            }
                            counter++;
                        }
                        query.bookingStatusID = query.bookingStatusID == 15 ? 10 : query.bookingStatusID;
                    }

                    if (query.tblMemberInfo.Count() > 0)
                    {
                        var member = query.tblMemberInfo.FirstOrDefault();
                        member.clubType = model.PreArrivalMemberInfoModel.MemberInfo_ClubType;
                        member.coOwner = model.PreArrivalMemberInfoModel.MemberInfo_CoOwner;
                        member.memberNumber = model.PreArrivalMemberInfoModel.MemberInfo_MemberNumber;
                        member.memberName = model.PreArrivalMemberInfoModel.MemberInfo_MemberName;
                        member.contractNumber = model.PreArrivalMemberInfoModel.MemberInfo_ContractNumber;
                    }
                    else
                    {
                        var member = new tblMemberInfo();
                        member.clubType = model.PreArrivalMemberInfoModel.MemberInfo_ClubType;
                        member.coOwner = model.PreArrivalMemberInfoModel.MemberInfo_CoOwner;
                        member.memberNumber = model.PreArrivalMemberInfoModel.MemberInfo_MemberNumber;
                        member.memberName = model.PreArrivalMemberInfoModel.MemberInfo_MemberName;
                        member.contractNumber = model.PreArrivalMemberInfoModel.MemberInfo_ContractNumber;
                        query.tblMemberInfo.Add(member);
                    }

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead Updated";
                    response.ObjectID = new { query.leadID, duplicate = false };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                if (model.Info_DuplicateLead == true)
                {
                    try
                    {
                        var query = new tblLeads();
                        var lead = db.tblLeads.Single(m => m.leadID == model.Info_LeadID);
                        var leadID = Guid.NewGuid();
                        query.leadID = leadID;
                        query.firstName = lead.firstName;
                        query.lastName = lead.lastName;
                        query.terminalID = lead.terminalID;
                        query.leadStatusID = 10;//duplicate
                        query.bookingStatusID = lead.bookingStatusID;
                        query.leadStatusDescription = lead.leadStatusDescription;
                        query.leadSourceID = lead.leadSourceID;
                        query.secondaryBookingStatusID = lead.secondaryBookingStatusID;
                        query.callClasificationID = lead.callClasificationID;
                        query.timeZoneID = lead.timeZoneID;
                        query.address = lead.address;
                        query.city = lead.city;
                        query.state = lead.state;
                        query.zipcode = lead.zipcode;
                        query.countryID = lead.countryID;
                        query.inputMethodID = 4;//auto. temporary defined as method for duplicate
                        query.inputDateTime = now;
                        query.inputByUserID = session.UserID;
                        query.assignedToUserID = lead.assignedToUserID;
                        query.assignationDate = now;
                        query.leadComments = lead.leadComments;
                        query.confirmed = lead.confirmed;
                        query.submissionForm = lead.submissionForm;
                        query.activityCert = lead.activityCert;
                        query.optionsTourDiscount = lead.optionsTourDiscount;
                        query.frontOfficeGuestID = lead.frontOfficeGuestID;
                        query.frontOfficeResortID = lead.frontOfficeResortID;
                        query.isTest = false;
                        query.tags = lead.tags;

                        foreach (var x in lead.tblLeadEmails.OrderByDescending(m => m.main))
                        {
                            var email = new tblLeadEmails();
                            email.email = x.email;
                            email.main = x.main;
                            email.bounced = x.bounced;
                            email.dnc = x.dnc;
                            query.tblLeadEmails.Add(email);
                        }

                        foreach (var i in lead.tblPhones.OrderByDescending(m => m.main))
                        {
                            var phone = new tblPhones();
                            phone.phone = i.phone;
                            phone.phoneTypeID = i.phoneTypeID;
                            phone.main = i.main;
                            phone.doNotCall = i.doNotCall;
                            phone.ext = i.ext;
                            query.tblPhones.Add(phone);
                        }

                        if (lead.tblMemberInfo.Count() > 0)
                        {
                            var member = new tblMemberInfo();
                            var leadMember = lead.tblMemberInfo.FirstOrDefault();
                            member.leadID = leadID;
                            member.clubType = leadMember.clubType;
                            member.coOwner = leadMember.coOwner;
                            member.memberNumber = leadMember.memberNumber;
                            member.memberName = leadMember.memberName;
                            member.contractNumber = leadMember.contractNumber;
                            query.tblMemberInfo.Add(member);
                        }

                        foreach (var modelReservation in lead.tblReservations)
                        {
                            var reservation = new tblReservations();
                            var reservationID = Guid.NewGuid();
                            //var modelReservation = lead.tblReservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();

                            reservation.reservationID = reservationID;
                            reservation.destinationID = modelReservation.destinationID;
                            reservation.placeID = modelReservation.placeID;
                            reservation.roomTypeID = modelReservation.roomTypeID;
                            reservation.roomNumber = modelReservation.roomNumber;
                            reservation.hotelConfirmationNumber = modelReservation.hotelConfirmationNumber;
                            reservation.arrivalDate = modelReservation.arrivalDate;
                            reservation.departureDate = modelReservation.departureDate;
                            reservation.adults = modelReservation.adults;
                            reservation.children = modelReservation.children;
                            reservation.totalNights = modelReservation.totalNights;
                            reservation.totalPaid = modelReservation.totalPaid;
                            reservation.confirmedTotalPaid = modelReservation.confirmedTotalPaid;
                            reservation.diamanteTotalPaid = modelReservation.diamanteTotalPaid;
                            reservation.greetingRepID = modelReservation.greetingRepID;
                            reservation.isSpecialOcassion = modelReservation.isSpecialOcassion;
                            reservation.specialOcassionComments = modelReservation.specialOcassionComments;
                            reservation.conciergeComments = modelReservation.conciergeComments;
                            reservation.reservationComments = modelReservation.reservationComments;
                            reservation.reservationStatusID = modelReservation.reservationStatusID;
                            reservation.reservationStatusDate = modelReservation.reservationStatusDate;
                            reservation.guestsNames = modelReservation.guestsNames;
                            reservation.roomUpgraded = modelReservation.roomUpgraded;
                            reservation.preCheckIn = modelReservation.preCheckIn;
                            reservation.planTypeID = modelReservation.planTypeID;
                            reservation.dateSaved = now;
                            reservation.savedByUserID = session.UserID;
                            reservation.frontOfficeReservationID = modelReservation.frontOfficeReservationID;
                            reservation.frontOfficePlanType = modelReservation.frontOfficePlanType;
                            reservation.frontOfficeAgencyName = modelReservation.frontOfficeAgencyName;
                            reservation.frontOfficeMarketCode = modelReservation.frontOfficeMarketCode;
                            reservation.frontOfficeRoomListID = modelReservation.frontOfficeRoomListID;
                            reservation.frontOfficeContractNumber = modelReservation.frontOfficeContractNumber;
                            reservation.frontOfficeCertificateNumber = modelReservation.frontOfficeCertificateNumber;

                            var modelPresentation = modelReservation.tblPresentations.FirstOrDefault();
                            if (modelReservation.tblPresentations.Count() > 0)
                            {
                                var presentation = new tblPresentations();
                                presentation.leadID = leadID;
                                presentation.reservationID = reservationID;
                                presentation.finalBookingStatusID = modelPresentation.finalBookingStatusID;
                                presentation.finalTourStatusID = modelPresentation.finalTourStatusID;
                                presentation.hostessComments = modelPresentation.hostessComments;
                                presentation.realTourDate = modelPresentation.realTourDate;
                                presentation.datePresentation = modelPresentation.datePresentation;
                                presentation.timePresentation = modelPresentation.timePresentation;
                                presentation.tourStatusID = modelPresentation.tourStatusID;
                                presentation.dateSaved = now;
                                presentation.savedByUserID = session.UserID;
                                reservation.tblPresentations.Add(presentation);
                            }
                            var modelFlights = modelReservation.tblFlights;
                            foreach (var x in modelFlights)
                            {
                                var flight = new tblFlights();

                                flight.destinationID = x.destinationID;
                                flight.flightTypeID = x.flightTypeID;
                                flight.airLineID = x.airLineID;
                                flight.flightNumber = x.flightNumber;
                                flight.passengersNames = x.passengersNames;
                                flight.passengers = x.passengers;
                                flight.flightComments = x.flightComments;
                                flight.flightDateTime = x.flightDateTime;
                                flight.dateSaved = now;
                                flight.pickupTime = x.pickupTime;
                                reservation.tblFlights.Add(flight);
                            }

                            query.tblReservations.Add(reservation);
                        }
                        db.tblLeads.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Lead Duplicated";
                        response.ObjectID = new { query.leadID, duplicate = true };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Lead NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {
                    try
                    {
                        var query = new tblLeads();
                        var leadID = Guid.NewGuid();

                        query.leadID = leadID;
                        query.firstName = model.Info_FirstName;
                        query.lastName = model.Info_LastName;
                        query.terminalID = model.Info_Terminal;
                        query.leadStatusID = model.Info_LeadStatus != 0 ? model.Info_LeadStatus : (int?)null;
                        query.bookingStatusID = 10;//not contacted
                        query.leadStatusDescription = model.Info_LeadStatusDescription;
                        query.leadSourceID = model.Info_LeadSource != 0 ? model.Info_LeadSource : (long?)null;
                        //query.secondaryBookingStatusID = model.Info_SecondaryBookingStatus != 0 ? model.Info_SecondaryBookingStatus : (int?)null;
                        query.callClasificationID = model.Info_CallClasification != 0 ? model.Info_CallClasification : (int?)null;
                        query.timeZoneID = model.Info_TimeZone != 0 ? model.Info_TimeZone : (int?)null;
                        query.address = model.Info_Address;
                        query.city = model.Info_City;
                        query.state = model.Info_State;
                        query.zipcode = model.Info_ZipCode;
                        query.countryID = model.Info_Country != 0 ? model.Info_Country : (int?)null;
                        query.inputMethodID = 1;
                        query.inputDateTime = now;
                        query.inputByUserID = session.UserID;
                        query.assignedToUserID = session.UserID;
                        query.assignationDate = now;
                        query.leadComments = model.Info_LeadComments;
                        query.confirmed = model.Info_Confirmed;
                        query.submissionForm = model.Info_SubmissionForm;
                        query.activityCert = model.Info_ActivityCert;
                        query.optionsTourDiscount = model.Info_OptionsTourDiscount;

                        if (model.PreArrivalEmails != null && model.PreArrivalEmails != "")
                        {
                            List<PreArrivalEmailsInfoModel> modelEmails = new JavaScriptSerializer().Deserialize(model.PreArrivalEmails, typeof(List<PreArrivalEmailsInfoModel>)) as List<PreArrivalEmailsInfoModel>;
                            if (modelEmails.Count(m => m.EmailsInfo_Main == true) == 1)
                            {
                                foreach (var x in modelEmails.OrderByDescending(m => m.EmailsInfo_Main))
                                {
                                    var email = new tblLeadEmails();
                                    email.email = x.EmailsInfo_Email;
                                    email.main = x.EmailsInfo_Main;
                                    email.bounced = false;
                                    email.dnc = false;
                                    query.tblLeadEmails.Add(email);
                                }
                            }
                        }
                        if (model.PreArrivalPhones != null && model.PreArrivalPhones != "")
                        {
                            List<PreArrivalPhonesInfoModel> modelPhones = new JavaScriptSerializer().Deserialize(model.PreArrivalPhones, typeof(List<PreArrivalPhonesInfoModel>)) as List<PreArrivalPhonesInfoModel>;
                            if (modelPhones.Count(m => m.PhonesInfo_Main == true) == 1)
                            {
                                foreach (var i in modelPhones.OrderByDescending(m => m.PhonesInfo_Main))
                                {
                                    var phone = new tblPhones();
                                    phone.phone = i.PhonesInfo_PhoneNumber;
                                    phone.phoneTypeID = i.PhonesInfo_PhoneType;
                                    phone.main = i.PhonesInfo_Main;
                                    phone.doNotCall = i.PhonesInfo_DoNotCall;
                                    phone.ext = i.PhonesInfo_ExtensionNumber;
                                    query.tblPhones.Add(phone);
                                }
                            }
                        }

                        var member = new tblMemberInfo();
                        member.leadID = leadID;
                        member.clubType = model.PreArrivalMemberInfoModel.MemberInfo_ClubType;
                        member.coOwner = model.PreArrivalMemberInfoModel.MemberInfo_CoOwner;
                        member.memberNumber = model.PreArrivalMemberInfoModel.MemberInfo_MemberNumber;
                        member.memberName = model.PreArrivalMemberInfoModel.MemberInfo_MemberName;
                        member.contractNumber = model.PreArrivalMemberInfoModel.MemberInfo_ContractNumber;
                        query.tblMemberInfo.Add(member);
                        db.tblLeads.AddObject(query);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Lead Saved";
                        response.ObjectID = new { query.leadID, duplicate = false };
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Lead NOT Saved";
                        response.ObjectID = 0;
                        response.Exception = ex;
                        return response;
                    }
                }
            }
        }

        public AttemptResponse SaveBilling(PreArrivalBillingModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.BillingInfo_BillingInfoID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblBillingInfo.Single(m => m.billingInfoID == model.BillingInfo_BillingInfoID);
                    query.firstName = model.BillingInfo_FirstName;
                    query.lastName = model.BillingInfo_LastName;
                    query.address = model.BillingInfo_Address;
                    query.countryID = model.BillingInfo_Country != 0 ? model.BillingInfo_Country : (int?)null;
                    query.zipcode = model.BillingInfo_ZipCode;
                    query.cardHolderName = model.BillingInfo_CardHolderName;
                    model.BillingInfo_CardNumber = model.BillingInfo_CardNumber.Trim().Replace("-", "").Replace(" ", "");
                    query.cardNumber = model.BillingInfo_CardNumber.IndexOf("*") != -1 ? query.cardNumber : mexHash.mexHash.EncryptString(model.BillingInfo_CardNumber);
                    query.cardTypeID = model.BillingInfo_CardType;
                    query.cardExpiry = model.BillingInfo_CardExpiry;
                    query.cardCVV = model.BillingInfo_CardCVV.IndexOf("*") != -1 ? query.cardCVV : model.BillingInfo_CardCVV;
                    query.billingComments = model.BillingInfo_Comments;

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Billing Info Updated";
                    response.ObjectID = query.billingInfoID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Billing Info NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    var query = new tblBillingInfo();
                    query.leadID = model.BillingInfo_LeadID;
                    query.firstName = model.BillingInfo_FirstName;
                    query.lastName = model.BillingInfo_LastName;
                    query.address = model.BillingInfo_Address;
                    query.countryID = model.BillingInfo_Country != 0 ? model.BillingInfo_Country : (int?)null;
                    query.zipcode = model.BillingInfo_ZipCode;
                    query.cardHolderName = model.BillingInfo_CardHolderName;
                    model.BillingInfo_CardNumber = model.BillingInfo_CardNumber.Trim().Replace("-", "").Replace(" ", "");
                    query.cardNumber = mexHash.mexHash.EncryptString(model.BillingInfo_CardNumber);
                    query.cardTypeID = model.BillingInfo_CardType;
                    query.cardExpiry = model.BillingInfo_CardExpiry;
                    query.cardCVV = model.BillingInfo_CardCVV;
                    query.billingComments = model.BillingInfo_Comments;
                    query.dateSaved = DateTime.Now;
                    db.tblBillingInfo.AddObject(query);

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Billing Info Saved";
                    response.ObjectID = query.billingInfoID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Billing Info NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public ReservationResultModel GetReservation(Guid id)
        {
            ePlatEntities db = new ePlatEntities();
            resortConnectEntities dba = new resortConnectEntities();

            ReservationResultModel response = new ReservationResultModel();

            response.PreArrivalPresentationsModel = new PresentationResultModel();
            response.ListPreArrivalFlights = new List<FlightResultsModel>();
            response.ListPreArrivalPayments = new List<PaymentResultsModel>();
            response.ListPreArrivalOptions = new List<OptionsResultsModel>();
            response.ListPreArrivalRCOptions = new List<OptionsResultsModel>();

            var _rsv = (from r in db.tblReservations where r.reservationID == id select r).ToList().FirstOrDefault();
            var query = new
            {
                _rsv.leadID,
                _rsv.tblLeads.leadSourceID,
                _rsv.tblLeads.secondaryBookingStatusID,
                _rsv.reservationID,
                _rsv.destinationID,
                _rsv.placeID,
                _rsv.roomTypeID,
                _rsv.roomNumber,
                _rsv.reservationStatusID,
                _rsv.hotelConfirmationNumber,
                _rsv.tblPlaces.resortConnectResortID,
                _rsv.arrivalDate,
                _rsv.departureDate,
                _rsv.frontOfficeAgencyName,
                _rsv.planTypeID,
                _rsv.frontOfficePlanType,
                _rsv.frontOfficeContractNumber,
                _rsv.frontOfficeCertificateNumber,
                _rsv.adults,
                _rsv.children,
                _rsv.totalNights,
                _rsv.totalPaid,
                _rsv.confirmedTotalPaid,
                _rsv.diamanteTotalPaid,
                _rsv.greetingRepID,
                _rsv.isSpecialOcassion,
                _rsv.specialOcassionComments,
                _rsv.conciergeComments,
                _rsv.reservationComments,
                _rsv.guestsNames,
                _rsv.roomUpgraded,
                _rsv.preCheckIn,
                _rsv.foundInFront,
                _rsv.spiTourID
            };

            #region "General Info"
            response.ReservationInfo_ReservationID = query.reservationID.ToString();
            response.ReservationInfo_Destination = query.destinationID != null ? query.destinationID.ToString() : "0";
            response.ReservationInfo_Place = query.placeID != null ? query.placeID.ToString() : "0";
            response.ReservationInfo_RoomType = query.roomTypeID != null ? query.roomTypeID.ToString() : "0";
            response.ReservationInfo_RoomNumber = query.roomNumber;
            response.ReservationInfo_ReservationStatus = query.reservationStatusID.ToString();
            response.ReservationInfo_HotelConfirmationNumber = query.hotelConfirmationNumber;
            response.ReservationInfo_ArrivalDate = query.arrivalDate != null ? ((DateTime)query.arrivalDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            response.ReservationInfo_DepartureDate = query.departureDate != null ? ((DateTime)query.departureDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            response.ReservationInfo_NumberAdults = query.adults.ToString();
            response.ReservationInfo_NumberChildren = query.children.ToString();
            response.ReservationInfo_FrontOfficeAgencyName = query.frontOfficeAgencyName;
            response.ReservationInfo_PlanType = query.planTypeID != null ? query.planTypeID.ToString() : null;
            response.ReservationInfo_FrontPlanType = query.frontOfficePlanType;
            response.ReservationInfo_TotalNights = query.totalNights != null ? query.totalNights.ToString() : "0";
            response.ReservationInfo_TotalPaid = query.totalPaid.ToString();
            response.ReservationInfo_ConfirmedTotalPaid = query.confirmedTotalPaid.ToString();
            response.ReservationInfo_DiamanteTotalPaid = query.diamanteTotalPaid.ToString();
            response.ReservationInfo_GreetingRep = query.greetingRepID.ToString();
            response.ReservationInfo_IsSpecialOcassion = query.isSpecialOcassion;
            response.ReservationInfo_SpecialOcassionComments = query.specialOcassionComments;
            response.ReservationInfo_ConciergeComments = query.conciergeComments;
            response.ReservationInfo_FrontComments = query.reservationComments;

            if (session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray().Contains(10))
            {
                var hc = query.hotelConfirmationNumber.Trim();
                var rcResortID = query.resortConnectResortID;
                var aDate = query.arrivalDate.Value.Date;
                var dDate = query.departureDate != null ? query.departureDate.Value.Date : (DateTime?)null;
                try
                {
                    var _query = (from r in dba.Reservation
                                  join m in dba.MemberNote on r.ReservationId equals m.ReservationId into r_m
                                  from m in r_m.DefaultIfEmpty().OrderByDescending(x => x.NoteDate)
                                  where r.ConfirmationNumber.Contains(hc)
                                  && r.CheckInDate == aDate
                                  && (r.CheckOutDate == dDate || dDate == null)
                                  && (r.ResortNumber == rcResortID || rcResortID == null)
                                  select new
                                  {
                                      r.TourBookingStatus,
                                      r.TourBookingStatusDesc,
                                      r.LastUpdated,
                                      m.Note,
                                  }).ToList().OrderByDescending(m => m.LastUpdated).FirstOrDefault();

                    response.PreArrivalPresentationsModel.PresentationInfo_TourStatusText = _query != null ? _query.TourBookingStatusDesc : "";
                    response.ReservationInfo_ResortConnectReservationComments = _query != null ? _query.Note : "";
                }
                catch (Exception ex)
                {
                    response.PreArrivalPresentationsModel.PresentationInfo_TourStatusText = "Error when connecting with ResortConnect. Contact System Administrator";
                }
            }
            response.ReservationInfo_GuestsNames = query.guestsNames;
            response.ReservationInfo_RoomUpgraded = query.roomUpgraded ?? false;
            response.ReservationInfo_PreCheckIn = query.preCheckIn ?? false;
            response.ReservationInfo_FrontContractNumber = query.frontOfficeContractNumber;
            response.ReservationInfo_FrontCertificateNumber = query.frontOfficeCertificateNumber;
            response.ReservationInfo_FoundInFront = query.foundInFront;
            //response.ReservationInfo_SPITourID = query.spiTourID;
            response.ListAvailableLetters = GetAvailableLetters(id);

            #region "getEmailNotificationLogs"
            var listEmailsSent = MailingDataModel.GetSentEmails("reservation", id.ToString());

            if (listEmailsSent.Count() > 0)
            {
                foreach (var e in listEmailsSent)
                {
                    if (response.ListAvailableLetters.Count(m => m.Transaction == e.trackingID.ToString()) == 0)
                    {
                        var item = new AvailableLettersModel()
                        {
                            Transaction = e.trackingID.ToString(),
                            ID = e.emailNotificationID,
                            EmailID = e.emailID,
                            Subject = e.subject,
                            Description = e.description,
                            Sent = e.dateSent != null,
                            DateSent = e.dateSent.ToString("yyyy-MM-dd HH:mm"),
                            DateRead = e.lastOpen != null ? e.lastOpen.Value.ToString("yyyy-MM-dd HH:mm") : "",
                            DateSigned = "",
                            Read = e.lastOpen != null,
                            Signed = false,
                            EventID = e.sysEventID//trackingInfo.Count() > 0 ? e.trackingInfo.FirstOrDefault().sysEventID : 0
                        };
                        response.ListAvailableLetters.Insert(0, item);
                    }
                }
            }
            #endregion
            #endregion

            #region "Presentation Info"
            var tourInfo = query.spiTourID != null ? db.tblSPIManifest.Where(m => m.tourID == query.spiTourID).ToList().FirstOrDefault() : null;
            response.PreArrivalPresentationsModel.PresentationInfo_FinalTourStatus = tourInfo != null ? tourInfo.tourStatus : "";
            response.PreArrivalPresentationsModel.PresentationInfo_RealTourDate = tourInfo != null ? tourInfo.tourDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
            response.PreArrivalPresentationsModel.PresentationInfo_SPITourID = query.spiTourID;
            var presentations = db.tblPresentations.Where(m => m.reservationID == query.reservationID).ToList();
            if (presentations.Count() > 0)
            {
                response.PreArrivalPresentationsModel.PresentationInfo_PresentationID = presentations.FirstOrDefault().presentationID.ToString();
                response.PreArrivalPresentationsModel.PresentationInfo_SecondaryBookingStatus = query.secondaryBookingStatusID != null ? query.secondaryBookingStatusID.ToString() : null;
                response.PreArrivalPresentationsModel.PresentationInfo_FinalBookingStatus = presentations.FirstOrDefault().finalBookingStatusID != null ? presentations.FirstOrDefault().finalBookingStatusID.ToString() : "0";
                response.PreArrivalPresentationsModel.PresentationInfo_FinalTourStatus = presentations.FirstOrDefault().finalTourStatusID != null ? presentations.FirstOrDefault().finalTourStatusID.ToString() : "0";
                response.PreArrivalPresentationsModel.PresentationInfo_HostessComments = presentations.FirstOrDefault().hostessComments;
                response.PreArrivalPresentationsModel.PresentationInfo_RealTourDate = presentations.FirstOrDefault().realTourDate != null ? ((DateTime)presentations.FirstOrDefault().realTourDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                response.PreArrivalPresentationsModel.PresentationInfo_DatePresentation = presentations.FirstOrDefault().datePresentation != null ? ((DateTime)presentations.FirstOrDefault().datePresentation).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                response.PreArrivalPresentationsModel.PresentationInfo_TimePresentation = presentations.FirstOrDefault().timePresentation != null ? ((TimeSpan)presentations.FirstOrDefault().timePresentation).ToString(@"hh\:mm") : "";
                response.PreArrivalPresentationsModel.PresentationInfo_TourStatus = presentations.FirstOrDefault().tourStatusID != null ? presentations.FirstOrDefault().tourStatusID.ToString() : "0";
                response.PreArrivalPresentationsModel.PresentationInfo_SPITourID = query.spiTourID;

                response.PreArrivalPresentationsModel.PresentationInfo_TourStatus = presentations.FirstOrDefault().tourStatusID != null ? presentations.FirstOrDefault().tourStatusID.ToString() : "0";
            }
            #endregion

            #region "Flights Info"
            var flights = db.tblFlights.Where(m => m.reservationID == query.reservationID).ToList();
            if (flights.Count() > 0)
            {
                foreach (var x in flights)
                {
                    response.ListPreArrivalFlights.Add(new FlightResultsModel()
                    {
                        //FlightInfo_ReservationID = (Guid)x.reservationID,
                        FlightInfo_FlightID = x.flightID.ToString(),
                        FlightInfo_AirlineText = x.tblAirLines.airLine,
                        FlightInfo_FlightNumber = x.flightNumber,
                        FlightInfo_FlightTypeText = x.tblFlightTypes.flightType,
                        FlightInfo_PassengerNames = x.passengersNames,
                        FlightInfo_Passengers = x.passengers.ToString(),
                        FlightInfo_FlightComments = x.flightComments,
                        FlightInfo_FlightDateTime = x.flightDateTime.ToString("yyyy-MM-dd hh:mm tt")
                    });
                }
            }
            #endregion

            #region "Options Sold Info"
            var optionsSold = db.tblOptionsSold.Where(m => m.reservationID == query.reservationID).ToList();
            if (optionsSold.Count() > 0)
            {
                foreach (var i in optionsSold.Where(m => m.deleted != true))
                {
                    response.ListPreArrivalOptions.Add(new OptionsResultsModel()
                    {
                        OptionInfo_OptionSoldID = i.optionSoldID.ToString(),
                        OptionInfo_Option = i.tblOptions.optionName,
                        OptionInfo_Quantity = i.quantity.ToString(),
                        OptionInfo_Price = i.optionPrice.ToString(),
                        OptionInfo_GuestNames = i.guestName ?? "",
                        OptionInfo_Eligible = (i.eligibleForCredit ? "True" : "False"),
                        OptionInfo_CreditAmount = i.creditAmount ?? "",
                        OptionInfo_PointsRedemption = i.pointsRedemption ?? "",
                        OptionInfo_TotalPaid = i.totalPaid,
                        OptionInfo_Comments = i.comments ?? ""
                    });
                }
            }
            if (session.Terminals.Split(',').Select(m => long.Parse(m)).Contains(10))
            {
                var hc = query.hotelConfirmationNumber.Trim();
                var rcResortID = query.resortConnectResortID;
                var aDate = query.arrivalDate.Value.Date;
                var dDate = query.departureDate != null ? query.departureDate.Value.Date : (DateTime?)null;
                try
                {
                    var _query = PublicDataModel.GetResortConnectOptionals(db.tblReservations.Single(m => m.reservationID == query.reservationID));

                    foreach (var i in _query)
                    {
                        response.ListPreArrivalRCOptions.Add(new OptionsResultsModel()
                        {
                            OptionInfo_OptionSoldID = "",
                            OptionInfo_Option = i.ProductName ?? "",
                            OptionInfo_Quantity = i.Quantity != null ? i.Quantity.ToString() : "",
                            OptionInfo_Price = i.BaseCurrencyAmount ?? "",
                            OptionInfo_GuestNames = "",//i.FirstName + " " + i.LastName,
                            OptionInfo_Eligible = "False",
                            OptionInfo_CreditAmount = "",
                            OptionInfo_PointsRedemption = "",
                            OptionInfo_TotalPaid = ((decimal)i.Quantity * decimal.Parse(i.BaseCurrencyAmount)).ToString(),
                            OptionInfo_Comments = i.Note ?? ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    response.ListPreArrivalRCOptions.Add(new OptionsResultsModel()
                    {
                        OptionInfo_OptionSoldID = "",
                        OptionInfo_Option = "Error when connecting with ResortConnect. Contact System Administrator",
                        OptionInfo_Quantity = "",
                        OptionInfo_Price = "",
                        OptionInfo_GuestNames = "",//i.FirstName + " " + i.LastName,
                        OptionInfo_Eligible = "False",
                        OptionInfo_CreditAmount = "",
                        OptionInfo_PointsRedemption = "",
                        OptionInfo_TotalPaid = "",
                        OptionInfo_Comments = ""
                    });
                }
            }
            #endregion

            #region "Payments Info"
            var payments = db.tblPaymentDetails.Where(m => m.reservationID == query.reservationID).ToList();
            if (payments.Count(m => m.deleted != true) > 0)
            {
                foreach (var x in payments.Where(m => m.deleted != true))
                {
                    response.ListPreArrivalPayments.Add(new PaymentResultsModel()
                    {
                        PaymentInfo_PaymentDetailsID = x.paymentDetailsID.ToString(),
                        PaymentInfo_TransactionType = x.tblMoneyTransactions.transactionTypeID.ToString(),
                        PaymentInfo_TransactionTypeText = (x.tblMoneyTransactions.transactionTypeID == 2 ? "Refund" : "Payment"),
                        PaymentInfo_PaymentType = x.paymentType.ToString(),
                        PaymentInfo_PaymentTypeText = Utils.GeneralFunctions.PaymentTypes.Single(m => m.Key == x.paymentType.ToString()).Value,
                        PaymentInfo_ChargeTypeText = x.tblChargeTypes.chargeType,
                        PaymentInfo_Invoice = x.tblMoneyTransactions.authCode ?? "",
                        PaymentInfo_InvoiceToRefund = x.tblMoneyTransactions.authCodeRefunded ?? "",
                        PaymentInfo_Amount = x.amount.ToString(),
                        PaymentInfo_Currency = x.tblCurrencies.currencyCode,
                        PaymentInfo_PaymentComments = x.paymentComments ?? "",
                        PaymentInfo_Status = x.paymentType == 2 ? x.tblMoneyTransactions.errorCode != string.Empty ? RescomDataModel.ApplyPayment_ErrorCodes.FirstOrDefault(m => m.Key == int.Parse(x.tblMoneyTransactions.errorCode)).Value : "Pending" : x.tblMoneyTransactions.errorCode == "0" ? "Approved" : "",
                        PaymentInfo_DateSaved = x.dateSaved.ToString("yyyy-MM-dd HH:mm tt"),
                        PaymentInfo_PendingCharge = x.tblMoneyTransactions.transactionTypeID == 1 && x.paymentType == 2 && x.tblMoneyTransactions.authCode == string.Empty ? true : false
                    });
                }
            }
            #endregion

            return response;
        }

        public PreArrivalOptionsSoldModel GetOptionSold(long id)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalOptionsSoldModel response = new PreArrivalOptionsSoldModel();

            var query = db.tblOptionsSold.Single(m => m.optionSoldID == id);

            response.OptionInfo_OptionSoldID = query.optionSoldID;
            response.OptionInfo_ReservationID = query.reservationID;
            response.OptionInfo_OptionType = (int)query.optionTypeID;
            response.OptionInfo_Option = query.optionID;
            response.OptionInfo_Price = query.optionPrice;
            response.OptionInfo_Quantity = query.quantity;
            response.OptionInfo_DateTime = query.optionDateTime != null ? ((DateTime)query.optionDateTime).ToString("yyyy-MM-dd hh:mm tt") : "";
            response.OptionInfo_PointsRedemption = query.pointsRedemption ?? "";
            response.OptionInfo_TotalPaid = decimal.Parse(query.totalPaid);
            response.OptionInfo_GuestNames = query.guestName ?? "";
            response.OptionInfo_Eligible = query.eligibleForCredit != null ? query.eligibleForCredit.ToString().ToLower() : "null";
            response.OptionInfo_CreditAmount = query.creditAmount != null ? query.creditAmount : "null";
            response.OptionInfo_Comments = query.comments ?? "";
            return response;
        }

        public AttemptResponse DeleteOptionSold(long id)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            var query = db.tblOptionsSold.Where(m => m.optionSoldID == id);

            try
            {
                if (query.Count() > 0)
                {
                    query.FirstOrDefault().dateDeleted = DateTime.Now;
                    query.FirstOrDefault().deletedByUserID = session.UserID;
                    query.FirstOrDefault().deleted = true;
                    query.FirstOrDefault().totalPaid = "0.00";
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Option Sold Deleted";
                    response.ObjectID = id.ToString() + "," + query.FirstOrDefault().reservationID.ToString();
                    return response;
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "No Option Sold with ID = " + id + " found";
                    response.ObjectID = id;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Option Sold NOT Deleted";
                response.Exception = ex;
                response.ObjectID = 0;
                return response;
            }
        }

        public AttemptResponse DeleteTransaction(long id)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            var query = db.tblPaymentDetails.Where(m => m.paymentDetailsID == id);
            try
            {
                if (query.Count() > 0)
                {
                    query.FirstOrDefault().deleted = true;
                    query.FirstOrDefault().dateDeleted = DateTime.Now;
                    query.FirstOrDefault().deletedByUserID = session.UserID;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Transaction Deleted";
                    response.ObjectID = id.ToString() + "," + query.FirstOrDefault().reservationID.ToString();
                    return response;
                }
                else
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "No Transaction with ID = " + id + " found";
                    response.ObjectID = id;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Transaction NOT Deleted";
                response.Exception = ex;
                response.ObjectID = 0;
                return response;
            }
        }

        public PreArrivalFlightsModel GetFlight(long id)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalFlightsModel response = new PreArrivalFlightsModel();

            var query = (from f in db.tblFlights
                         where f.flightID == id
                         select f).FirstOrDefault();

            response.FlightInfo_ReservationID = (Guid)query.reservationID;
            response.FlightInfo_FlightID = query.flightID;
            response.FlightInfo_FlightType = query.flightTypeID;
            response.FlightInfo_Airline = query.airLineID;
            response.FlightInfo_FlightNumber = query.flightNumber;
            response.FlightInfo_PassengerNames = query.passengersNames;
            response.FlightInfo_Passengers = query.passengers;
            response.FlightInfo_Destination = query.destinationID;
            response.FlightInfo_FlightComments = query.flightComments;
            response.FlightInfo_FlightDateTime = ((DateTime)query.flightDateTime).ToString("yyyy-MM-dd hh:mm tt");
            response.FlightInfo_PickUpTime = query.pickupTime != null ? ((TimeSpan)query.pickupTime).ToString(@"hh\:mm", CultureInfo.InvariantCulture) : "";
            return response;
        }

        public AttemptResponse DeleteFlight(long id)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            try
            {
                var query = db.tblFlights.Single(m => m.flightID == id);

                db.tblFlights.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Flight Deleted";
                response.ObjectID = id.ToString() + "," + query.reservationID.ToString();
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Flight NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public PreArrivalPaymentsModel GetPayment(long id)
        {
            ePlatEntities db = new ePlatEntities();
            PreArrivalPaymentsModel response = new PreArrivalPaymentsModel();

            var query = (from p in db.tblPaymentDetails
                         where p.paymentDetailsID == id
                         select p).FirstOrDefault();

            response.PaymentInfo_ReservationID = (Guid)query.reservationID;
            response.PaymentInfo_PaymentDetailsID = query.paymentDetailsID;
            response.PaymentInfo_TransactionType = query.tblMoneyTransactions.transactionTypeID;
            response.PaymentInfo_PaymentType = (int)query.paymentType;
            response.PaymentInfo_BillingInfo = query.tblMoneyTransactions.billingInfoID ?? 0;
            response.PaymentInfo_CCReferenceNumber = query.ccReferenceNumber;
            response.PaymentInfo_CCType = query.ccType ?? 0;
            response.PaymentInfo_RefundAccount = query.refundAccount;
            response.PaymentInfo_ChargeType = query.chargeTypeID;
            response.PaymentInfo_ChargeDescription = query.chargeDescriptionID;
            response.PaymentInfo_Amount = query.amount;
            response.PaymentInfo_Currency = query.tblCurrencies.currencyCode;
            response.PaymentInfo_PaymentComments = query.paymentComments;
            response.PaymentInfo_MadeByEplat = query.tblMoneyTransactions.madeByEplat;
            response.PaymentInfo_Transaction = query.tblMoneyTransactions.authCode;
            response.PaymentInfo_DateSaved = query.dateSaved.ToString("yyyy-MM-dd");
            return response;
        }

        public AttemptResponse _SavePresentation(PreArrivalPresentationsModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            DateTime now = DateTime.Now;
            var tour = model.PresentationInfo_SpiTour != null && model.PresentationInfo_SpiTour != 0 ? db.tblSPIManifest.Where(m => m.tourID == model.PresentationInfo_SpiTour).OrderByDescending(m => m.dateLastUpdate).FirstOrDefault() : null;

            if (model.PresentationInfo_PresentationID == 0)
            {
                try
                {
                    var rsv = db.tblReservations.FirstOrDefault(m => m.reservationID == model.PresentationInfo_ReservationID);
                    var query = new tblPresentations();
                    query.leadID = model.PresentationInfo_LeadID;
                    query.reservationID = model.PresentationInfo_ReservationID;
                    query.finalTourStatusID = tour != null && tour.tourStatus != null ? Dictionaries.FrontTourStatus.FirstOrDefault(m => tour.tourStatus.Contains(m.Key)).Value : (int?)null;
                    query.hostessComments = model.PresentationInfo_HostessComments;
                    query.realTourDate = tour != null && tour.tourDate != null ? tour.tourDate : (DateTime?)null;
                    query.dateSaved = now;
                    query.savedByUserID = session.UserID;
                    rsv.spiTourID = tour != null ? tour.tourID : (int?)null;
                    rsv.tblLeads.leadSourceID = model.PresentationInfo_LeadSource != null && model.PresentationInfo_LeadSource != 0 ? model.PresentationInfo_LeadSource : rsv.tblLeads.leadSourceID;
                    db.tblPresentations.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Presentation Saved";
                    response.ObjectID = query.presentationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Presentation NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var rsv = db.tblReservations.FirstOrDefault(m => m.reservationID == model.PresentationInfo_ReservationID);
                    var query = rsv.tblPresentations.FirstOrDefault(m => m.presentationID == model.PresentationInfo_PresentationID);
                    query.finalTourStatusID = tour != null && tour.tourStatus != null ? Dictionaries.FrontTourStatus.FirstOrDefault(m => tour.tourStatus.Contains(m.Key)).Value : (int?)null;
                    query.hostessComments = model.PresentationInfo_HostessComments;
                    query.realTourDate = tour != null && tour.tourDate != null ? tour.tourDate : (DateTime?)null;
                    query.dateLastModification = now;
                    query.modifiedByUserID = session.UserID;
                    rsv.spiTourID = tour != null ? tour.tourID : (int?)null;
                    rsv.tblLeads.leadSourceID = model.PresentationInfo_LeadSource != null && model.PresentationInfo_LeadSource != 0 ? model.PresentationInfo_LeadSource : rsv.tblLeads.leadSourceID;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Presentation Updated";
                    response.ObjectID = query.presentationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Presentation NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;

                }
            }
        }

        public AttemptResponse SavePresentation(PreArrivalPresentationsModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var now = DateTime.Now;
            if (model.PresentationInfo_PresentationID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblPresentations.Single(m => m.presentationID == model.PresentationInfo_PresentationID);

                    query.finalBookingStatusID = model.PresentationInfo_FinalBookingStatus != 0 ? model.PresentationInfo_FinalBookingStatus : (int?)null;
                    query.finalTourStatusID = model.PresentationInfo_FinalTourStatus != 0 ? model.PresentationInfo_FinalTourStatus : (int?)null;
                    query.hostessComments = model.PresentationInfo_HostessComments;
                    query.realTourDate = model.PresentationInfo_SpiTourDate != null && model.PresentationInfo_SpiTourDate != "" ? DateTime.Parse(model.PresentationInfo_SpiTourDate, CultureInfo.InvariantCulture) : (DateTime?)null;
                    query.datePresentation = model.PresentationInfo_DatePresentation != null ? DateTime.Parse(model.PresentationInfo_DatePresentation) : (DateTime?)null;
                    query.timePresentation = model.PresentationInfo_TimePresentation != null ? TimeSpan.Parse(model.PresentationInfo_TimePresentation) : (TimeSpan?)null;
                    query.tblLeads.secondaryBookingStatusModifiedByUserID = query.tblLeads.secondaryBookingStatusID != model.PresentationInfo_SecondaryBookingStatus ? session.UserID : query.tblLeads.secondaryBookingStatusModifiedByUserID;
                    query.tblLeads.secondaryBookingStatusLastModificationDate = query.tblLeads.secondaryBookingStatusID != model.PresentationInfo_SecondaryBookingStatus ? (DateTime?)now : (query.tblLeads.secondaryBookingStatusLastModificationDate == null ? (DateTime?)null : query.tblLeads.secondaryBookingStatusLastModificationDate);
                    query.tblLeads.secondaryBookingStatusID = model.PresentationInfo_SecondaryBookingStatus != 0 ? model.PresentationInfo_SecondaryBookingStatus : query.tblLeads.secondaryBookingStatusID;
                    query.tourStatusID = model.PresentationInfo_TourStatus != 0 ? model.PresentationInfo_TourStatus : (int?)null;
                    query.tblReservations.spiTourID = model.PresentationInfo_SpiTour;
                    query.dateLastModification = now;
                    query.modifiedByUserID = session.UserID;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Presentation Updated";
                    response.ObjectID = query.presentationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Presentation NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    var query = new tblPresentations();
                    var lead = db.tblLeads.Single(m => m.leadID == model.PresentationInfo_LeadID);
                    query.leadID = model.PresentationInfo_LeadID;
                    query.reservationID = model.PresentationInfo_ReservationID;
                    query.finalBookingStatusID = model.PresentationInfo_FinalBookingStatus != 0 ? model.PresentationInfo_FinalBookingStatus : (int?)null;
                    query.finalTourStatusID = model.PresentationInfo_FinalTourStatus != 0 ? model.PresentationInfo_FinalTourStatus : (int?)null;
                    query.hostessComments = model.PresentationInfo_HostessComments;
                    query.realTourDate = model.PresentationInfo_SpiTourDate != null && model.PresentationInfo_SpiTourDate != "" ? DateTime.Parse(model.PresentationInfo_SpiTourDate) : (DateTime?)null;
                    query.datePresentation = model.PresentationInfo_DatePresentation != null ? DateTime.Parse(model.PresentationInfo_DatePresentation) : (DateTime?)null;
                    query.timePresentation = model.PresentationInfo_TimePresentation != null ? TimeSpan.Parse(model.PresentationInfo_TimePresentation) : (TimeSpan?)null;
                    lead.secondaryBookingStatusModifiedByUserID = session.UserID;
                    lead.secondaryBookingStatusLastModificationDate = (DateTime?)now;
                    lead.secondaryBookingStatusID = model.PresentationInfo_SecondaryBookingStatus != 0 ? model.PresentationInfo_SecondaryBookingStatus : lead.secondaryBookingStatusID;
                    query.tourStatusID = model.PresentationInfo_TourStatus != 0 ? model.PresentationInfo_TourStatus : (int?)null;
                    query.dateSaved = now;
                    query.dateLastModification = now;
                    query.savedByUserID = session.UserID;
                    db.tblPresentations.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Presentation Saved";
                    response.ObjectID = query.presentationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Presentation NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public AttemptResponse SaveReservation(PreArrivalReservationsModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.ReservationInfo_ReservationID != null && model.ReservationInfo_ReservationID != Guid.Empty)
            {
                #region "update"
                try
                {
                    var query = db.tblReservations.Single(m => m.reservationID == model.ReservationInfo_ReservationID);

                    query.leadID = model.ReservationInfo_LeadID;
                    query.destinationID = model.ReservationInfo_Destination != 0 ? model.ReservationInfo_Destination : (long?)null;
                    query.placeID = model.ReservationInfo_Place != 0 ? model.ReservationInfo_Place : (long?)null;
                    query.roomTypeID = model.ReservationInfo_RoomType != 0 ? model.ReservationInfo_RoomType : (long?)null;
                    query.roomNumber = model.ReservationInfo_RoomNumber;
                    query.hotelConfirmationNumber = model.ReservationInfo_HotelConfirmationNumber;
                    query.arrivalDate = model.ReservationInfo_ArrivalDate != null ? DateTime.Parse(model.ReservationInfo_ArrivalDate) : (DateTime?)null;
                    query.departureDate = model.ReservationInfo_DepartureDate != null ? DateTime.Parse(model.ReservationInfo_DepartureDate) : (DateTime?)null;
                    query.adults = int.Parse(model.ReservationInfo_NumberAdults ?? "0");
                    query.children = int.Parse(model.ReservationInfo_NumberChildren ?? "0");
                    query.totalNights = model.ReservationInfo_TotalNights;
                    query.totalPaid = model.ReservationInfo_TotalPaid;
                    query.confirmedTotalPaid = model.ReservationInfo_ConfirmedTotalPaid;
                    query.diamanteTotalPaid = model.ReservationInfo_DiamanteTotalPaid;
                    query.greetingRepID = model.ReservationInfo_GreetingRep != null && model.ReservationInfo_GreetingRep != 0 ? model.ReservationInfo_GreetingRep : (long?)null;
                    query.isSpecialOcassion = model.ReservationInfo_IsSpecialOcassion;
                    query.specialOcassionComments = model.ReservationInfo_SpecialOcassionComments;
                    query.conciergeComments = model.ReservationInfo_ConciergeComments;
                    query.planTypeID = model.ReservationInfo_PlanType != 0 ? model.ReservationInfo_PlanType : query.planTypeID;
                    var r = "";

                    if (model.ReservationInfo_ReservationComments != null && model.ReservationInfo_ReservationComments != "")
                    {
                        #region"new front comments"
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        var client = new WebClient();
                        client.UseDefaultCredentials = true;
                        client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                        #region "obtener info de reservacion"
                        //var _llegadas = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/Llegadas/" + query.tblLeads.frontOfficeResortID + "/" + query.arrivalDate.Value.ToString("yyyy-MM-dd") + "T00:00:00/" + query.arrivalDate.Value.ToString("yyyy-MM-dd") + "T00:00:00");
                        //_llegadas = _llegadas.Replace("{\"result\":[", "")
                        //            .Replace("]]}", "]");
                        //var llegadas = serializer.Deserialize<List<FrontOfficeViewModel.LlegadasResult>>(_llegadas);
                        //FrontOfficeViewModel.LlegadasResult llegada = (from a in llegadas
                        //                                               select new FrontOfficeViewModel.LlegadasResult()
                        //                                               {
                        //                                                   idReservacion = a.idReservacion,
                        //                                                   CuartosNoche = a.CuartosNoche,
                        //                                                   TipoHab = a.TipoHab,
                        //                                                   llegada = a.llegada,
                        //                                                   salida = a.salida,
                        //                                                   NumHab = a.NumHab,
                        //                                                   numconfirmacion = a.numconfirmacion,
                        //                                                   Procedencia = a.Procedencia,
                        //                                                   CodigoMerc = a.CodigoMerc,
                        //                                                   idresort = a.idresort,
                        //                                                   Split = a.Split,
                        //                                                   CRS = a.CRS,
                        //                                                   codeagencia = a.codeagencia,
                        //                                                   nameagencia = a.nameagencia,
                        //                                                   Huesped = a.Huesped,
                        //                                                   cuartos = a.cuartos,
                        //                                                   Adultos = a.Adultos,
                        //                                                   Ninos = a.Ninos,
                        //                                                   apellidopaterno = a.apellidopaterno,
                        //                                                   apellidomaterno = a.apellidomaterno,
                        //                                                   nombres = a.nombres,
                        //                                                   codepais = a.codepais,
                        //                                                   namepais = a.namepais,
                        //                                                   codigostatusreservacion = a.codigostatusreservacion,
                        //                                                   X = a.X,
                        //                                                   Titulo = a.Titulo,
                        //                                                   Infantes = a.Infantes,
                        //                                                   HLlegada = a.HLlegada,
                        //                                                   HSalida = a.HSalida,
                        //                                                   idhuesped = a.idhuesped,
                        //                                                   DistintivoPrecheckin = a.DistintivoPrecheckin,
                        //                                                   FechaHoraCheckin = a.FechaHoraCheckin,
                        //                                                   Contrato = a.Contrato,
                        //                                                   TipoPlan = a.TipoPlan,
                        //                                                   Comentario = a.Comentario,
                        //                                                   idroomlist = a.idroomlist,
                        //                                                   Tarifa = a.Tarifa,
                        //                                                   codetipodemoneda = a.codetipodemoneda
                        //                                               }).ToList().FirstOrDefault(m => m.idReservacion == query.frontOfficeReservationID);
                        #endregion

                        var comments = "";
                        //comments = (llegada != null ? llegada.Comentario : "") + "\r\n"
                        comments = "\r\n"
                                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + session.User + ":\r\n"
                                    + model.ReservationInfo_ReservationComments;
                        comments = comments.Replace("#", "n:").Replace("/", "-").Replace("?", "").Replace("\"", "--").Replace("'", "");
                        var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/ComentarioRsv/" + query.tblLeads.frontOfficeResortID + "/{\"idreservacion\":\"" + query.frontOfficeReservationID + "\",\"comentario\":\"" + comments + "\"}");
                        #endregion

                        #region "old front comments"
                        //var conn = "";
                        //var comments = "";


                        //switch (query.tblLeads.frontOfficeResortID)
                        //{
                        //    case 1: //Villa del Palmar Vallarta
                        //        FrontOfficeVDPVEntities frontVDPV = new FrontOfficeVDPVEntities();

                        //        break;
                        //    case 9: //Garza Blanca Vallarta
                        //        conn = ConfigurationManager.ConnectionStrings["FrontOfficeGBRVEntities"].ConnectionString.ToString();
                        //        break;
                        //    case 11: //Villa del Palmar Cancun
                        //        conn = ConfigurationManager.ConnectionStrings["FrontOfficeVDPVCancunEntities"].ConnectionString.ToString();
                        //        break;
                        //    case 13: //Mousai PV
                        //        conn = ConfigurationManager.ConnectionStrings["FrontOfficeHMPVEntities"].ConnectionString.ToString();
                        //        break;
                        //    case 15: //Garza Blanca Cabo
                        //        conn = ConfigurationManager.ConnectionStrings["FrontOfficeGBCEntities"].ConnectionString.ToString();
                        //        break;
                        //    case 4: //Villa del Palmar Cabo
                        //    case 3: //Villa del Palmar Flamingos
                        //    case 5: //Villa La Estancia Cabo
                        //    case 6: //Villa del Arco Cabo
                        //    case 7: //Villa La Estancia Nuevo Vallarta
                        //    case 12: //Villa del Palmar Loreto
                        //        comments = "\r\n"
                        //            + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + session.User + ":\r\n"
                        //            + model.ReservationInfo_ReservationComments;
                        //        var client = new WebClient();
                        //        client.UseDefaultCredentials = true;
                        //        client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                        //        var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/ComentarioRsv/" + query.tblLeads.frontOfficeResortID + "/{\"idreservacion\":\"" + query.frontOfficeReservationID + "\",\"comentario\":\"" + comments + "\"}");
                        //        break;
                        //}

                        //if (conn != "")
                        //{
                        //    conn = new EntityConnectionStringBuilder(conn).ProviderConnectionString;//converts ef connectionString to sql ConnectionString

                        //    using (SqlConnection sqlConn = new SqlConnection(conn))
                        //    {
                        //        sqlConn.Open();
                        //        var command = new System.Data.SqlClient.SqlCommand("select top(1) comentario from tbaComentarios where idreservacion=" + query.frontOfficeReservationID + " order by fechaalta desc", sqlConn);
                        //        var reader = command.ExecuteReader();
                        //        var frontComments = "";
                        //        while (reader.Read())
                        //            frontComments = reader.GetString(0);
                        //        reader.Close();

                        //        comments = frontComments + "\r\n"
                        //            + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + session.User + ":\r\n"
                        //            + model.ReservationInfo_ReservationComments;

                        //        System.Data.SqlClient.SqlCommand sqlcomm = new SqlCommand("spAgregaComentario", sqlConn);
                        //        sqlcomm.CommandType = CommandType.StoredProcedure;

                        //        sqlcomm.Parameters.AddWithValue("@V_idReservacion", query.frontOfficeReservationID).Direction = ParameterDirection.Input;
                        //        if (query.frontOfficeRoomListID != null)
                        //        {
                        //            sqlcomm.Parameters.AddWithValue("@V_idRoomList", query.frontOfficeRoomListID).Direction = ParameterDirection.Input;
                        //        }
                        //        else
                        //        {
                        //            sqlcomm.Parameters.AddWithValue("@V_idRoomList", DBNull.Value).Direction = ParameterDirection.Input;
                        //        }

                        //        sqlcomm.Parameters.AddWithValue("@V_Comentario", comments).Direction = ParameterDirection.Input;
                        //        sqlcomm.Parameters.AddWithValue("@V_Urgente", 0).Direction = ParameterDirection.Input;
                        //        sqlcomm.Parameters.Add("@V_RESULTADO", SqlDbType.Char, 2).Direction = ParameterDirection.Output;
                        //        sqlcomm.ExecuteNonQuery();
                        //        r = sqlcomm.Parameters["@V_RESULTADO"].Value.ToString();
                        //        r = r == "OK" ? "Reservation Comments Updated in Front" : "Reservation Comments NOT Updated in Front. Please advise";
                        //        sqlConn.Close();
                        //    }
                        //}
                        #endregion

                        //query.reservationComments = comments;
                        if (str.IndexOf("Agregado") != -1)
                        {
                            r = "Reservation Comments Updated in Front";
                            query.reservationComments += comments;
                        }
                        else
                        {
                            r = "Reservation Comments NOT Updated in Front nor ePlat. Please advise";
                        }
                    }
                    query.guestsNames = model.ReservationInfo_GuestsNames;
                    query.roomUpgraded = model.ReservationInfo_RoomUpgraded;
                    query.preCheckIn = model.ReservationInfo_PreCheckIn;

                    query.dateModified = DateTime.Now;
                    query.modifiedByUserID = session.UserID;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reservation Updated" + (r != "" ? "<br />" + r : r);
                    response.ObjectID = query.reservationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Reservation NOT Updated";
                    response.ObjectID = 0;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    var query = new tblReservations();
                    var now = DateTime.Now;
                    query.reservationID = Guid.NewGuid();
                    query.leadID = model.ReservationInfo_LeadID;
                    query.destinationID = model.ReservationInfo_Destination != 0 ? model.ReservationInfo_Destination : (long?)null;
                    query.placeID = model.ReservationInfo_Place != 0 ? model.ReservationInfo_Place : (long?)null;
                    query.roomTypeID = model.ReservationInfo_RoomType != 0 ? model.ReservationInfo_RoomType : (long?)null;
                    query.roomNumber = model.ReservationInfo_RoomNumber;
                    query.hotelConfirmationNumber = model.ReservationInfo_HotelConfirmationNumber;
                    query.arrivalDate = model.ReservationInfo_ArrivalDate != null ? DateTime.Parse(model.ReservationInfo_ArrivalDate) : (DateTime?)null;
                    query.departureDate = model.ReservationInfo_DepartureDate != null ? DateTime.Parse(model.ReservationInfo_DepartureDate) : (DateTime?)null;
                    query.adults = int.Parse(model.ReservationInfo_NumberAdults ?? "0");
                    query.children = int.Parse(model.ReservationInfo_NumberChildren ?? "0");
                    query.totalNights = model.ReservationInfo_TotalNights;
                    query.totalPaid = model.ReservationInfo_TotalPaid;
                    query.confirmedTotalPaid = model.ReservationInfo_ConfirmedTotalPaid;
                    query.diamanteTotalPaid = model.ReservationInfo_DiamanteTotalPaid;
                    query.greetingRepID = model.ReservationInfo_GreetingRep != null && model.ReservationInfo_GreetingRep != 0 ? model.ReservationInfo_GreetingRep : (long?)null;
                    query.isSpecialOcassion = model.ReservationInfo_IsSpecialOcassion;
                    query.specialOcassionComments = model.ReservationInfo_SpecialOcassionComments;
                    query.conciergeComments = model.ReservationInfo_ConciergeComments;
                    query.reservationComments = model.ReservationInfo_FrontComments + (model.ReservationInfo_ReservationComments != null && model.ReservationInfo_ReservationComments != ""
                        ? "<br />" + now.ToString("yyyy-MM-dd HH:mm:ss") + " " + session.User + ": " + model.ReservationInfo_ReservationComments : "");
                    query.guestsNames = model.ReservationInfo_GuestsNames;
                    query.roomUpgraded = model.ReservationInfo_RoomUpgraded;
                    query.preCheckIn = model.ReservationInfo_PreCheckIn;
                    query.planTypeID = model.ReservationInfo_PlanType != 0 ? model.ReservationInfo_PlanType : (int?)null;
                    query.dateSaved = now;
                    query.savedByUserID = session.UserID;
                    db.tblReservations.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Reservation Saved";
                    response.ObjectID = query.reservationID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Reservation NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }

            return response;
        }

        public AttemptResponse SaveOptionSold(PreArrivalOptionsSoldModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.OptionInfo_OptionSoldID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblOptionsSold.Single(m => m.optionSoldID == model.OptionInfo_OptionSoldID);
                    query.optionTypeID = model.OptionInfo_OptionType;
                    query.optionID = model.OptionInfo_Option;
                    query.quantity = model.OptionInfo_Quantity;
                    query.optionPrice = model.OptionInfo_Price;
                    query.optionDateTime = model.OptionInfo_DateTime != null ? DateTime.Parse(model.OptionInfo_DateTime) : (DateTime?)null;
                    query.pointsRedemption = model.OptionInfo_PointsRedemption;
                    query.totalPaid = model.OptionInfo_TotalPaid.ToString();
                    query.guestName = model.OptionInfo_GuestNames;
                    query.eligibleForCredit = model.OptionInfo_Eligible != "null" ? bool.Parse(model.OptionInfo_Eligible) : false;
                    query.creditAmount = model.OptionInfo_CreditAmount != "null" ? model.OptionInfo_CreditAmount : null;
                    query.comments = model.OptionInfo_Comments;

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Option Updated";
                    response.ObjectID = query.optionSoldID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Option NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    var query = new tblOptionsSold();

                    query.reservationID = model.OptionInfo_ReservationID;
                    query.optionTypeID = model.OptionInfo_OptionType;
                    query.optionID = model.OptionInfo_Option;
                    query.quantity = model.OptionInfo_Quantity;
                    query.optionPrice = model.OptionInfo_Price;
                    query.optionDateTime = model.OptionInfo_DateTime != null ? DateTime.Parse(model.OptionInfo_DateTime) : (DateTime?)null;
                    query.pointsRedemption = model.OptionInfo_PointsRedemption;
                    query.totalPaid = model.OptionInfo_TotalPaid.ToString();
                    query.guestName = model.OptionInfo_GuestNames;
                    query.eligibleForCredit = model.OptionInfo_Eligible != "null" ? bool.Parse(model.OptionInfo_Eligible) : false;
                    query.creditAmount = model.OptionInfo_CreditAmount != "null" ? model.OptionInfo_CreditAmount : null;
                    query.comments = model.OptionInfo_Comments;
                    query.savedByUserID = session.UserID;
                    query.dateSaved = DateTime.Now;
                    db.tblOptionsSold.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Option Saved";
                    response.ObjectID = query.optionSoldID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Option NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public AttemptResponse SaveFlight(PreArrivalFlightsModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.FlightInfo_FlightID != 0)
            {
                #region "update"
                try
                {
                    var query = db.tblFlights.Single(m => m.flightID == model.FlightInfo_FlightID);
                    query.destinationID = model.FlightInfo_Destination;
                    query.flightTypeID = model.FlightInfo_FlightType;
                    query.airLineID = model.FlightInfo_Airline;
                    query.flightNumber = model.FlightInfo_FlightNumber;
                    query.passengersNames = model.FlightInfo_PassengerNames;
                    query.passengers = model.FlightInfo_Passengers;
                    query.flightComments = model.FlightInfo_FlightComments;
                    query.flightDateTime = DateTime.Parse(model.FlightInfo_FlightDateTime);
                    query.pickupTime = model.FlightInfo_PickUpTime != null && model.FlightInfo_FlightType == 2 ? TimeSpan.Parse(model.FlightInfo_PickUpTime, CultureInfo.InvariantCulture) : (TimeSpan?)null;

                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Flight Updated";
                    response.ObjectID = query.flightID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Flight NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    var query = new tblFlights();

                    query.reservationID = model.FlightInfo_ReservationID;
                    query.destinationID = model.FlightInfo_Destination;
                    query.flightTypeID = model.FlightInfo_FlightType;
                    query.airLineID = model.FlightInfo_Airline;
                    query.flightNumber = model.FlightInfo_FlightNumber;
                    query.passengersNames = model.FlightInfo_PassengerNames;
                    query.passengers = model.FlightInfo_Passengers;
                    query.flightComments = model.FlightInfo_FlightComments;
                    query.flightDateTime = DateTime.Parse(model.FlightInfo_FlightDateTime);
                    query.dateSaved = DateTime.Now;
                    query.pickupTime = model.FlightInfo_PickUpTime != null && model.FlightInfo_FlightType == 2 ? TimeSpan.Parse(model.FlightInfo_PickUpTime, CultureInfo.InvariantCulture) : (TimeSpan?)null;
                    db.tblFlights.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Flight Saved";
                    response.ObjectID = query.flightID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Flight NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public AttemptResponse SavePayment(PreArrivalPaymentsModel model)
        {
            ePlatEntities db = new ePlatEntities();

            AttemptResponse response = new AttemptResponse();
            tblPaymentDetails payment;
            tblMoneyTransactions transaction;
            DateTime now = DateTime.Now;
            var reservation = db.tblReservations.Single(m => m.reservationID == model.PaymentInfo_ReservationID);
            var lead = reservation.tblLeads;
            var currencyID = db.tblCurrencies.Single(m => m.currencyCode == model.PaymentInfo_Currency).currencyID;
            string status = null;
            if (model.PaymentInfo_PaymentDetailsID != null && model.PaymentInfo_PaymentDetailsID != 0)
            {
                #region "update"
                try
                {
                    payment = db.tblPaymentDetails.Single(m => m.paymentDetailsID == model.PaymentInfo_PaymentDetailsID);
                    transaction = payment.tblMoneyTransactions;

                    if(transaction.madeByEplat && transaction.authCode != string.Empty)
                    {
                        response.Type = Attempt_ResponseTypes.Warning;
                        response.Message = "Payment NOT Updated";
                        response.ObjectID = 0;
                        return response;
                    }

                    //payment.paymentType = model.PaymentInfo_PaymentType;
                    payment.refundAccount = model.PaymentInfo_RefundAccount;
                    payment.chargeTypeID = model.PaymentInfo_ChargeType;
                    payment.chargeDescriptionID = model.PaymentInfo_ChargeDescription;
                    payment.amount = model.PaymentInfo_Amount;
                    payment.currencyID = currencyID;
                    payment.paymentComments = model.PaymentInfo_PaymentComments;
                    payment.lastDateModified = now;
                    payment.modifiedByUserID = session.UserID;
                    
                    if (model.PaymentInfo_TransactionType == 2)
                    {
                        payment.dateSaved = model.PaymentInfo_DateSaved != null ? DateTime.Parse(model.PaymentInfo_DateSaved).Date != now.Date ? DateTime.Parse(model.PaymentInfo_DateSaved).AddHours(12) : payment.dateSaved.Date == DateTime.Parse(model.PaymentInfo_DateSaved).Date ? payment.dateSaved : (DateTime.Parse(model.PaymentInfo_DateSaved).Date + payment.dateSaved.TimeOfDay) : payment.dateSaved;
                        transaction.authCodeRefunded = model.PaymentInfo_InvoiceToRefund != null ? model.PaymentInfo_InvoiceToRefund.ToString() : null;
                    }
                    transaction.terminalID = lead.terminalID;
                    transaction.transactionTypeID = model.PaymentInfo_TransactionType;

                    if (model.PaymentInfo_PaymentType == 2)//credit card
                    {
                        payment.ccReferenceNumber = model.PaymentInfo_BillingInfo <= 0 ? model.PaymentInfo_CCReferenceNumber : (int?)null;
                        payment.ccType = model.PaymentInfo_BillingInfo <= 0 ? model.PaymentInfo_CCType : (int?)null;
                        
                        //transaction.errorCode = model.PaymentInfo_MadeByEplat ? string.Empty : "0";
                        transaction.authCode = model.PaymentInfo_MadeByEplat == false && model.PaymentInfo_Transaction != null ? model.PaymentInfo_Transaction : string.Empty;
                        
                        transaction.authAmount = model.PaymentInfo_Amount;
                        transaction.billingInfoID = model.PaymentInfo_BillingInfo <= 0 ? (long?)null : model.PaymentInfo_BillingInfo;
                        transaction.reference = lead.tblTerminals.prefix + "-" + reservation.reservationID + (reservation.frontOfficeCertificateNumber != null ? "-" + reservation.frontOfficeCertificateNumber : "");//define structure
                        transaction.madeByEplat = model.PaymentInfo_MadeByEplat;
                        payment.tblMoneyTransactions = transaction;
                        status = payment.paymentType == 2 ? transaction.errorCode != string.Empty ? RescomDataModel.ApplyPayment_ErrorCodes.FirstOrDefault(m => m.Key == int.Parse(transaction.errorCode)).Value : "Pending" : transaction.errorCode == "0" ? "Approved" : "";
                    }
                    else//validate permitted payment types with Linda
                    {
                        transaction.authCode = string.Empty;
                        transaction.authAmount = model.PaymentInfo_Amount;
                        transaction.authDate = now;
                        transaction.errorCode = "0";
                        transaction.transactionDate = now;
                        transaction.reference = lead.tblTerminals.prefix + "-" + reservation.reservationID + (reservation.certificateNumber != null ? "-" + reservation.certificateNumber : "");//define structure
                        transaction.madeByEplat = false;//define criteria
                        transaction.billingInfoID = (long?)null;
                        status = "0";
                    }

                    payment.paymentType = model.PaymentInfo_PaymentType;

                    db.SaveChanges();
                    var account = MerchantAccountDataModel.GetMerchantAccount(payment.paymentDetailsID);
                    transaction.merchantAccountID = model.PaymentInfo_PaymentType == 2 ? account.MerchantAccountID : (int?)null;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Payment Updated";
                    response.ObjectID = new { id = payment.paymentDetailsID, status = status == "0" ? "Approved" : "Pending" };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Payment NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
            else
            {
                #region "save"
                try
                {
                    payment = new tblPaymentDetails();
                    transaction = new tblMoneyTransactions();

                    payment.reservationID = reservation.reservationID;
                    payment.paymentType = model.PaymentInfo_PaymentType;
                    payment.refundAccount = model.PaymentInfo_RefundAccount;
                    payment.chargeTypeID = model.PaymentInfo_ChargeType;
                    payment.chargeDescriptionID = model.PaymentInfo_ChargeDescription;
                    payment.amount = model.PaymentInfo_Amount;
                    payment.currencyID = currencyID;
                    payment.paymentComments = model.PaymentInfo_PaymentComments;
                    if (model.PaymentInfo_TransactionType == 1)
                    {
                        payment.dateSaved = now;
                    }
                    else
                    {
                        payment.dateSaved = model.PaymentInfo_DateSaved != null ? DateTime.Parse(model.PaymentInfo_DateSaved).Date != now.Date ? DateTime.Parse(model.PaymentInfo_DateSaved).AddHours(12) : now : now;
                        transaction.authCodeRefunded = model.PaymentInfo_InvoiceToRefund != null ? model.PaymentInfo_InvoiceToRefund.ToString() : null;
                    }
                    payment.savedByUserID = session.UserID;

                    transaction.terminalID = lead.terminalID;
                    transaction.transactionTypeID = model.PaymentInfo_TransactionType;

                    if (model.PaymentInfo_PaymentType == 2)//credit card
                    {

                        payment.ccReferenceNumber = model.PaymentInfo_BillingInfo <= 0 ? model.PaymentInfo_CCReferenceNumber : (int?)null;
                        payment.ccType = model.PaymentInfo_BillingInfo <= 0 ? model.PaymentInfo_CCType : (int?)null;
                        
                        transaction.billingInfoID = model.PaymentInfo_BillingInfo <= 0 ? (long?)null : model.PaymentInfo_BillingInfo;
                        transaction.authAmount = model.PaymentInfo_Amount;
                        transaction.authCode = model.PaymentInfo_MadeByEplat == false && model.PaymentInfo_Transaction != null ? model.PaymentInfo_Transaction : string.Empty;
                        transaction.authDate = model.PaymentInfo_MadeByEplat == false ? now : (DateTime?)null;
                        transaction.errorCode = model.PaymentInfo_MadeByEplat == false ? "0" : string.Empty;
                        transaction.madeByEplat = model.PaymentInfo_MadeByEplat;
                        transaction.transactionDate = now;
                        transaction.reference = lead.tblTerminals.prefix + "-" + payment.reservationID + (reservation.frontOfficeCertificateNumber != null ? "-" + reservation.frontOfficeCertificateNumber : "");
                        
                        status = model.PaymentInfo_MadeByEplat == false ? "0" : "Pending";
                    }
                    else if (model.PaymentInfo_PaymentType == 8)
                    {
                        ShoppingCart listItem = new ShoppingCart();
                        var redeemModel = new RedeemCredits();
                        //redeemModel.ResortID = long.Parse(reservation.placeID.ToString());
                        redeemModel.ResortID = lead.terminalID == 16 ? 11923 : lead.terminalID == 80 ? 11925 : 11927;// long.Parse(reservation.placeID.ToString());
                        redeemModel.Account = reservation.frontOfficeContractNumber != null ? reservation.frontOfficeContractNumber : reservation.tblLeads.tblMemberInfo.Count() > 0 ? reservation.tblLeads.tblMemberInfo.FirstOrDefault().memberNumber : null;

                        listItem.CreditsApplied = (int)model.PaymentInfo_Amount;
                        listItem.Title = "Optionals Transaction";
                        listItem.TransactionTypeID = 6;//PreArrival Optionals
                        redeemModel.ShoppingCart = new List<ShoppingCart>() { listItem };

                        var redeemed = PublicDataModel.RedeemCredits(redeemModel);
                        payment.referralPointsID = redeemed.ObjectID != null ? Guid.Parse(redeemed.ObjectID.ToString()) : (Guid?)null;
                        if (redeemed.Type != Attempt_ResponseTypes.Ok)
                        {
                            throw new Exception(redeemed.Message);
                        }
                        transaction.authCode = string.Empty;
                        transaction.authAmount = model.PaymentInfo_Amount;
                        transaction.authDate = now;
                        transaction.errorCode = "0";
                        transaction.transactionDate = now;
                        transaction.reference = lead.tblTerminals.prefix + "-" + reservation.reservationID + (reservation.certificateNumber != null ? "-" + reservation.certificateNumber : "");
                        transaction.madeByEplat = false;//define criteria
                        transaction.billingInfoID = (long?)null;
                        status = "0";
                    }
                    else//validate permitted payment types with Linda
                    {
                        transaction.authCode = string.Empty;
                        transaction.authAmount = model.PaymentInfo_Amount;
                        transaction.authDate = now;
                        transaction.errorCode = "0";
                        transaction.transactionDate = now;
                        transaction.reference = lead.tblTerminals.prefix + "-" + reservation.reservationID + (reservation.certificateNumber != null ? "-" + reservation.certificateNumber : "");
                        transaction.madeByEplat = false;//define criteria
                        transaction.billingInfoID = (long?)null;
                        //transaction.merchantAccountID = (int?)null;
                        status = "0";
                    }

                    payment.tblMoneyTransactions = transaction;
                    db.tblPaymentDetails.AddObject(payment);
                    db.SaveChanges();
                    var account = MerchantAccountDataModel.GetMerchantAccount(payment.paymentDetailsID);
                    transaction.merchantAccountID = model.PaymentInfo_PaymentType == 2 ? account.MerchantAccountID : (int?)null;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Payment Saved";
                    response.ObjectID = new { id = payment.paymentDetailsID, dateSaved = transaction.transactionDate.ToString("yyyy-MM-dd HH:mm tt"), status = status == "0" ? "Approved" : "Pending" };
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Payment NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
                #endregion
            }
        }

        public static List<SelectListItem> GetRecentLeadGroups()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();

            var today = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            var lastMonth = today.AddMonths(-1);

            var query = from lg in db.tblLeadGroups
                        where lg.dateSaved >= lastMonth && lg.dateSaved <= today
                        select lg;

            foreach (var i in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.leadGroupID.ToString(),
                    Text = i.leadGroup
                });
            }

            return list;
        }

        public AttemptResponse SaveLeadGroup(GenericListItem model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            var lgID = model.Property2 != null ? Guid.Parse(model.Property2) : (Guid?)null;
            var leadGroup = lgID != null ? db.tblLeadGroups.Single(m => m.leadGroupID == lgID) : new tblLeadGroups();
            var ids = model.Property3.Split(',').Select(m => Guid.Parse(m)).ToArray();
            var leads = db.tblLeads.Where(m => ids.Contains(m.leadID));

            if (model.Property2 == null)
            {
                leadGroup.leadGroupID = Guid.NewGuid();
                leadGroup.leadGroup = model.Property1;
                leadGroup.dateSaved = DateTime.Now;
                leadGroup.savedByUserID = session.UserID;
                db.tblLeadGroups.AddObject(leadGroup);
                db.SaveChanges();
            }


            foreach (var lead in leads)
            {
                lead.leadGroupID = leadGroup.leadGroupID;
            }

            db.SaveChanges();

            return response;
        }

        public AttemptResponse ApplyCharge(long id, decimal? amount = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            RescomDataModel.ApplyPayment_Data paymentData = new RescomDataModel.ApplyPayment_Data();
            RescomDataModel.ApplyPayment_Response paymentResponse = new RescomDataModel.ApplyPayment_Response();
            var now = DateTime.Now;
            try
            {
                var payment = db.tblPaymentDetails.Single(m => m.paymentDetailsID == id);
                var transaction = new tblMoneyTransactions(); 
                
                if(payment.tblMoneyTransactions.errorCode == null || payment.tblMoneyTransactions.errorCode == string.Empty)
                {
                    transaction = payment.tblMoneyTransactions;
                }
                else
                {
                    var lead = payment.tblReservations.tblLeads;
                    transaction.terminalID = lead.terminalID;
                    transaction.transactionDate = now;
                    transaction.transactionTypeID = 1;//payment
                    transaction.merchantAccountID = payment.tblMoneyTransactions.merchantAccountID;
                    transaction.reference = lead.tblTerminals.prefix + "-" + payment.reservationID.ToString() + (payment.tblReservations.frontOfficeCertificateNumber != null ? "-" + payment.tblReservations.frontOfficeCertificateNumber : "");
                    transaction.billingInfoID = payment.tblMoneyTransactions.billingInfoID;
                    transaction.tblMerchantAccounts = payment.tblMoneyTransactions.tblMerchantAccounts;
                }
                

                paymentData.UserName = transaction.tblMerchantAccounts.username;
                paymentData.Password = transaction.tblMerchantAccounts.password;
                paymentData.Card_Holder = transaction.tblBillingInfo.cardHolderName;
                paymentData.Card_Holder_Address = transaction.tblBillingInfo.address;
                paymentData.Card_Holder_Zip = transaction.tblBillingInfo.zipcode;
                paymentData.Card_Number = mexHash.mexHash.DecryptString(transaction.tblBillingInfo.cardNumber);
                paymentData.Card_Security_Number = transaction.tblBillingInfo.cardCVV;
                paymentData.Expiration_Date = transaction.tblBillingInfo.cardExpiry;
                paymentData.Reference_Code = TerminalDataModel.GetPrefix(transaction.terminalID) + "-" + payment.reservationID;
                paymentData.Transaction_Amount = amount != null ? (double)amount : (double)payment.amount;
                paymentData.CurrencyID = (int)payment.currencyID;

                paymentResponse = RescomDataModel.ApplyPayment(paymentData);
                transaction.authDate = paymentResponse.Authorization_Date;//preguntar qué representa esta fecha. fecha de transacción, de captura de informacion, etc
                transaction.authCode = paymentResponse.Auth_Code;
                transaction.errorCode = paymentResponse.Error_Code.ToString();
                transaction.authAmount = (decimal)paymentResponse.Authorization_Amount;
                transaction.madeByEplat = true;
                
                payment.tblMoneyTransactions = transaction;
                
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                var _status = RescomDataModel.ApplyPayment_ErrorCodes.FirstOrDefault(m => m.Key == paymentResponse.Error_Code).Value;
                response.Message = "Transaction done. Status: " + _status;
                response.ObjectID = new { id = payment.paymentDetailsID, status = _status, authCode = paymentResponse.Auth_Code };
                //if (paymentResponse.Error_Code == 0)
                //{
                //    var balance = db.tblPaymentDetails.Where(m => m.reservationID == payment.reservationID).Sum(m => (m.tblMoneyTransactions.transactionTypeID == 1 ? m.amount : (m.amount * -1)));
                //    if (balance == 0)
                //    {
                //        //crear interaccion
                //        var model = new PreArrivalInteractionsInfoModel();
                //        model.InteractionsInfo_LeadID = payment.tblReservations.leadID;
                //        model.InteractionsInfo_BookingStatus = 16;//sold
                //        model.InteractionsInfo_InteractionType = 13;//Note. preguntar si agregar interaccion "system"
                //        model.InteractionsInfo_InteractionComments = "Created by system. Purchase Paid.";
                //        model.InteractionsInfo_ParentInteraction = (long?)null;
                //        model.InteractionsInfo_InteractedWithUser = session.UserID;
                //        var interaction = new InteractionDataModel().SaveInteraction(model);
                //        response.Message += "<br />" + interaction.Message + " as Sold";
                //    }
                //}

                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Charge NOT Applied";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse ApplyPendingCharges(string id)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            DateTime now = DateTime.Now;
            var results = new List<SelectListItem>();
            //x.paymentType == 2 && x.tblMoneyTransactions.authCode == string.Empty && x.tblMoneyTransactions.errorCode == string.Empty ? true : false
            var charges = id.Split(',').Select(m => long.Parse(m)).ToArray();
            var payments = db.tblPaymentDetails.Where(m => charges.Contains(m.paymentDetailsID));
            var paymentsNotCharged = payments.Where(m => m.tblMoneyTransactions.authCode == string.Empty || m.tblMoneyTransactions.errorCode == string.Empty);
            //var paymentsNotCharged = payments.Where(m => m.tblMoneyTransactions.errorCode == null || m.tblMoneyTransactions.errorCode == string.Empty);
            if (payments.Select(m => m.paymentDetailsID).ToArray().Except(paymentsNotCharged.Select(m => m.paymentDetailsID).ToArray()).Count() == 0)
            {
                var ids = "";

                var groupsByBilling = payments.GroupBy(m => m.tblMoneyTransactions.billingInfoID);

                foreach (var billing in groupsByBilling)
                {
                    var groupsByAccount = billing.GroupBy(m => m.tblMoneyTransactions.merchantAccountID);

                    foreach (var account in groupsByAccount)
                    {
                        var amount = account.Sum(m => m.amount);
                        RescomDataModel.ApplyPayment_Data paymentData = new RescomDataModel.ApplyPayment_Data();
                        RescomDataModel.ApplyPayment_Response paymentResponse = new RescomDataModel.ApplyPayment_Response();

                        paymentData.UserName = account.FirstOrDefault().tblMoneyTransactions.tblMerchantAccounts.username;
                        paymentData.Password = account.FirstOrDefault().tblMoneyTransactions.tblMerchantAccounts.password;
                        paymentData.Card_Holder = account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.cardHolderName;
                        paymentData.Card_Holder_Address = account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.address;
                        paymentData.Card_Holder_Zip = account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.zipcode;
                        paymentData.Card_Number = mexHash.mexHash.DecryptString(account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.cardNumber);
                        paymentData.Card_Security_Number = account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.cardCVV;
                        paymentData.Expiration_Date = account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.cardExpiry;
                        paymentData.Reference_Code = TerminalDataModel.GetPrefix(account.FirstOrDefault().tblMoneyTransactions.terminalID) + "-" + account.FirstOrDefault().reservationID;
                        paymentData.Transaction_Amount = (double)amount;
                        paymentData.CurrencyID = (int)billing.FirstOrDefault().currencyID;
                        ids += (ids == "" ? "" : ",") + account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.cardHolderName + " - " + mexHash.mexHash.DecryptString(account.FirstOrDefault().tblMoneyTransactions.tblBillingInfo.cardNumber).Substring(12) + " - " + amount;
                        paymentResponse = RescomDataModel.ApplyPayment(paymentData);
                        var transactions = account.Select(m => m.tblMoneyTransactions);
                        foreach (var transaction in transactions)
                        {
                            //new mike
                            var payment = transaction.tblPaymentDetails.FirstOrDefault();
                            if (transaction.errorCode == null || transaction.errorCode == string.Empty)
                            {
                                transaction.authDate = paymentResponse.Authorization_Date;//preguntar qué representa esta fecha. fecha de transacción, de captura de informacion, etc
                                transaction.authCode = paymentResponse.Auth_Code;
                                transaction.errorCode = paymentResponse.Error_Code.ToString();
                                transaction.authAmount = (decimal)paymentResponse.Authorization_Amount;
                                transaction.madeByEplat = true;
                            }
                            else
                            {
                                var _transaction = new tblMoneyTransactions();
                                _transaction.terminalID = transaction.terminalID;
                                _transaction.transactionDate = now;
                                _transaction.transactionTypeID = 1;//payment
                                _transaction.merchantAccountID = transaction.merchantAccountID;
                                _transaction.reference = transaction.reference;
                                _transaction.billingInfoID = transaction.billingInfoID;
                                _transaction.tblMerchantAccounts = transaction.tblMerchantAccounts;
                                _transaction.authDate = paymentResponse.Authorization_Date;//preguntar qué representa esta fecha. fecha de transacción, de captura de informacion, etc
                                _transaction.authCode = paymentResponse.Auth_Code;
                                _transaction.errorCode = paymentResponse.Error_Code.ToString();
                                _transaction.authAmount = (decimal)paymentResponse.Authorization_Amount;
                                _transaction.madeByEplat = true;
                                
                                payment.tblMoneyTransactions = _transaction;
                                db.SaveChanges();
                            }
                            //end mike

                            //transaction.authDate = paymentResponse.Authorization_Date;//preguntar qué representa esta fecha. fecha de transacción, de captura de informacion, etc
                            //transaction.authCode = paymentResponse.Auth_Code;
                            //transaction.errorCode = paymentResponse.Error_Code.ToString();
                            //transaction.authAmount = (decimal)paymentResponse.Authorization_Amount;
                            //transaction.madeByEplat = true;
                            results.Add(new SelectListItem()
                            {
                                //Value = transaction.tblPaymentDetails.FirstOrDefault().paymentDetailsID.ToString(),
                                Value = payment.paymentDetailsID.ToString(),
                                Text = RescomDataModel.ApplyPayment_ErrorCodes.FirstOrDefault(m => m.Key == paymentResponse.Error_Code).Value,
                            });
                        }
                    }
                }
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Charges Applied";
                response.ObjectID = new { id = results };
                return response;
            }
            else
            {
                response.Type = Attempt_ResponseTypes.Warning;
                response.Message = "Charges NOT Applied<br />An attempt to duplicate charge has been detected. Contact System Administrator";
                //response.ObjectID = new { id = results };
                return response;
            }
        }

        public List<ImportResultsModel> GetArrivals(SearchToImportModel model)
        {
            ePlatEntities db = new ePlatEntities();
            List<List<FrontOfficeViewModel.LlegadasResult>> list = new List<List<FrontOfficeViewModel.LlegadasResult>>();
            List<ImportResultsModel> results = new List<ImportResultsModel>();

            var resorts = model.SearchToImport_ImportResort != null ? model.SearchToImport_ImportResort : PreArrivalCatalogs.GetFrontResorts().Select(m => int.Parse(m.Value)).ToArray();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var fDate = DateTime.Parse(model.Search_I_ImportArrivalDate);
            var tDate = DateTime.Parse(model.Search_F_ImportArrivalDate).AddDays(1).AddSeconds(-1);
            var now = DateTime.Now;
            var places = db.tblPlaces.Where(m => resorts.Contains((int)m.frontOfficeResortID));
            var placeIDs = places.Select(m => m.placeID).ToArray();
            var roomTypes = db.tblRoomTypes.Where(m => placeIDs.Contains(m.placeID)).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode }).GroupBy(m => m.placeID);
            IQueryable<tblUserProfiles> supervisorUser = null;
            List<KeyValuePair<int, SelectListItem>> listPerResort = new List<KeyValuePair<int, SelectListItem>>();

            foreach (var resort in resorts)
            {
                var _resortID = resort.ToString();
                //var resortName = model.Resorts.FirstOrDefault(m => int.Parse(m.Value) == resort).Text;
                var resortName = model.Resorts.FirstOrDefault(m => m.Value == _resortID).Text;
                var marketCodesPerUserResort = db.tblMarketCodes_Users.Where(m => terminals.Contains(m.terminalID) && m.tblMarketCodes.frontOfficeResortID == resort);
                var codes = marketCodesPerUserResort.Select(m => m.tblMarketCodes.marketCode.Trim()).OrderBy(m => m);
                var listItem = new ImportResultsModel();
                var _list = new List<KeyValuePair<string, SelectListItem>>();
                var arrivalsPerResort = FrontOfficeDataModel.GetArrivals(resort, fDate, tDate);
                listItem.lista = arrivalsPerResort;
                //listItem.Resort = resort;
                listItem.Resort = resortName;
                //new
                var a = "";
                var arrivalsPerCode = arrivalsPerResort.GroupBy(m => m.Procedencia.Trim());
                var bodyMail = "";
                var userName = "";
                var cInfoMail = "";
                foreach (var arrivals in arrivalsPerCode.OrderBy(m => m.Key))
                {
                    if (codes.Contains(arrivals.Key))
                    {
                        #region "code already assigned to user"
                        var listUserCounter = new Dictionary<Guid, int>();
                        var users = marketCodesPerUserResort.Where(m => m.tblMarketCodes.marketCode.ToLower().Trim() == arrivals.Key.ToLower().Trim()).Select(m => m.aspnet_Users.UserId);
                        foreach (var user in users)
                        {
                            listUserCounter.Add(user, 0);
                        }
                        var counter = 0;

                        //proceed with import
                        var leadsToImport = new tblLeads();
                        bodyMail += "<br /><br />Market Code: " + arrivals.Key;
                        foreach (var item in arrivals)
                        {
                            try
                            {
                                var userItem = listUserCounter.OrderBy(m => m.Value).FirstOrDefault();
                                var user = userItem.Key;
                                var profile = db.tblUserProfiles.FirstOrDefault(m => m.userID == user);
                                userName = profile.firstName + " " + profile.lastName;

                                var existingLead = db.tblLeads.Where(m => m.frontOfficeGuestID == item.idhuesped && m.frontOfficeResortID == item.idresort);
                                var status = "";
                                if (existingLead.Count() > 0)
                                {
                                    #region
                                    status = "Lead imported on: " + existingLead.OrderByDescending(m => m.inputDateTime).FirstOrDefault().inputDateTime.ToString("yyyy-MM-dd hh:mm") + ".";
                                    var existingReservation = existingLead.FirstOrDefault().tblReservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();
                                    DateTime departure = existingReservation.departureDate.Value;
                                    DateTime llegada = item.llegada.Value;
                                    var lead = existingLead.FirstOrDefault();
                                    var _placeID = item.idresort != null ? places.Count(m => m.frontOfficeResortID == item.idresort) > 0 ? places.Single(m => m.frontOfficeResortID == item.idresort).placeID : (long?)null : (long?)null;

                                    //if (existingReservation.frontOfficeReservationID != item.idReservacion && departure.Date != llegada.Date)//la llegada de la rsv a importar es distinta de la salida de la rsv importada
                                    //revisar si estos campos son iguales o no en las reservaciones con hotelconfirmationnumber duplicado
                                    if (existingReservation.frontOfficeReservationID != item.idReservacion && departure.Date != llegada.Date)//la llegada de la rsv a importar es distinta de la salida de la rsv importada
                                    {
                                        var rsv = new tblReservations();
                                        rsv.reservationID = Guid.NewGuid();
                                        rsv.arrivalDate = item.llegada;
                                        rsv.departureDate = item.salida;
                                        rsv.placeID = _placeID;
                                        rsv.roomTypeID = _placeID != null ? roomTypes.Count(m => m.Key == _placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).Count(m => m.roomTypeCode == item.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).FirstOrDefault(m => m.roomTypeCode == item.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                        rsv.roomNumber = item.NumHab;
                                        rsv.adults = item.Adultos ?? 0;
                                        rsv.children = item.Ninos ?? 0;
                                        //rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("meal plan") != -1 || item.TipoPlan.ToLower().IndexOf("mp") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.certificateNumber = item.CRS;
                                        rsv.destinationID = _placeID != null ? places.Single(m => m.placeID == _placeID).destinationID : (long?)null;
                                        rsv.hotelConfirmationNumber = item.numconfirmacion;
                                        rsv.totalNights = item.CuartosNoche;
                                        rsv.sysWorkGroupID = session.WorkGroupID;
                                        rsv.guestsNames = item.Huesped;
                                        rsv.reservationStatusID = item.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == item.codigostatusreservacion) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == item.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                        rsv.reservationComments = item.Comentario;
                                        rsv.dateSaved = now;
                                        rsv.frontOfficeReservationID = (long)item.idReservacion;
                                        rsv.frontOfficePlanType = item.TipoPlan;
                                        rsv.frontOfficeAgencyName = item.nameagencia;
                                        rsv.frontOfficeMarketCode = item.Procedencia;
                                        rsv.frontOfficeRoomListID = item.idroomlist;
                                        //search for guestHubID
                                        var guestHubID = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeGuestID == item.idhuesped && m.frontOfficeResortID == item.idresort);
                                        lead.guestHubID = guestHubID != null ? guestHubID.guestHubID : (Guid?)null;
                                        if (item.codigostatusreservacion != null && item.codigostatusreservacion == "CA")
                                        {
                                            lead.leadStatusID = 4;//canceled
                                        }
                                        lead.tblReservations.Add(rsv);
                                        status += " Reservation imported";
                                    }
                                    else
                                    {
                                        #region "Fields Mapping"
                                        var rsv = existingReservation;
                                        rsv.arrivalDate = item.llegada;
                                        rsv.departureDate = item.salida;
                                        rsv.placeID = _placeID;
                                        rsv.roomTypeID = _placeID != null ? roomTypes.Count(m => m.Key == _placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).Count(m => m.roomTypeCode == item.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).FirstOrDefault(m => m.roomTypeCode == item.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                        rsv.roomNumber = item.NumHab;
                                        rsv.adults = item.Adultos ?? 0;
                                        rsv.children = item.Ninos ?? 0;
                                        //rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("meal plan") != -1 || item.TipoPlan.ToLower().IndexOf("mp") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.certificateNumber = item.CRS;
                                        rsv.destinationID = _placeID != null ? places.Single(m => m.placeID == _placeID).destinationID : (long?)null;
                                        rsv.hotelConfirmationNumber = item.numconfirmacion;
                                        rsv.totalNights = item.CuartosNoche;
                                        rsv.sysWorkGroupID = session.WorkGroupID;
                                        rsv.guestsNames = item.Huesped;
                                        rsv.reservationComments = item.Comentario;
                                        rsv.reservationStatusID = item.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == item.codigostatusreservacion) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == item.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                        rsv.frontOfficeReservationID = (long)item.idReservacion;
                                        rsv.frontOfficePlanType = item.TipoPlan;
                                        rsv.frontOfficeAgencyName = item.nameagencia;
                                        rsv.frontOfficeMarketCode = item.Procedencia;
                                        rsv.frontOfficeRoomListID = item.idroomlist;
                                        #endregion
                                        if (item.codigostatusreservacion != null && item.codigostatusreservacion == "CA")
                                        {
                                            lead.leadStatusID = 4;//canceled
                                        }
                                        counter++;
                                        a += (a == "" ? "" : ",") + item.Procedencia;
                                        status += " Reservation updated";
                                    }

                                    if (lead.tblLeadEmails.Count() == 0 && lead.tblPhones.Count() == 0)
                                    {
                                        var info = FrontOfficeDataModel.GetContactInfo((int)item.idresort, item.idhuesped);
                                        if (info.Email != null)
                                        {
                                            var leadEmail = new tblLeadEmails();
                                            leadEmail.email = info.Email != "" ? info.Email : info.UnformattedEmail;
                                            leadEmail.main = true;
                                            lead.tblLeadEmails.Add(leadEmail);
                                            if (info.Email == "")
                                            {
                                                cInfoMail += item.numconfirmacion + "<br />";
                                            }
                                        }
                                        if (info.Phone != null)
                                        {
                                            var phone = new tblPhones();
                                            phone.phone = info.Phone;
                                            phone.phoneTypeID = 4;//unkown
                                            phone.doNotCall = false;
                                            phone.ext = null;
                                            phone.main = true;
                                            lead.tblPhones.Add(phone);
                                        }

                                        if (info.Email == null && info.Phone == null)
                                        {
                                            lead.bookingStatusID = 15;//not contactable
                                        }
                                        else
                                        {
                                            if (lead.bookingStatusID == 15)
                                            {
                                                lead.bookingStatusID = 10;//not contacted
                                            }
                                            //lead.bookingStatusID = 10;//not contacted
                                        }
                                    }

                                    #endregion
                                }
                                else //new lead
                                {
                                    #region "Fields Mapping"
                                    var lead = new tblLeads();
                                    lead.leadID = Guid.NewGuid();
                                    lead.initialTerminalID = terminals.FirstOrDefault();
                                    lead.terminalID = terminals.FirstOrDefault();
                                    lead.personalTitleID = item.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == item.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == item.Titulo).Value) : (int?)null : (int?)null;
                                    lead.firstName = item.nombres;
                                    lead.lastName = item.apellidopaterno + (item.apellidomaterno != "" && item.apellidomaterno != null ? " " + item.apellidomaterno : "");
                                    lead.countryID = item.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == item.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.Single(m => m.Key == item.codepais).Value) : (int?)null : (int?)null;
                                    lead.bookingStatusID = 10;//not contacted
                                    lead.leadSourceID = marketCodesPerUserResort.FirstOrDefault(m => m.tblMarketCodes.marketCode.Trim() == arrivals.Key.Trim()).tblMarketCodes.leadSourceID;
                                    lead.inputByUserID = session.UserID;
                                    lead.assignedToUserID = user;
                                    lead.inputDateTime = now;
                                    lead.assignationDate = now;
                                    lead.inputMethodID = 2;//import
                                    lead.leadStatusID = item.codigostatusreservacion != null && item.codigostatusreservacion == "CA" ? 4 : 2;//assigned
                                    lead.isTest = false;
                                    lead.frontOfficeGuestID = (int)item.idhuesped;
                                    lead.frontOfficeResortID = (int)item.idresort;

                                    listUserCounter[user] = userItem.Value + 1;

                                    var info = FrontOfficeDataModel.GetContactInfo((int)item.idresort, item.idhuesped);
                                    if (info.Email != null)
                                    {
                                        var leadEmail = new tblLeadEmails();
                                        leadEmail.email = info.Email != "" ? info.Email : info.UnformattedEmail;
                                        leadEmail.main = true;
                                        lead.tblLeadEmails.Add(leadEmail);

                                        if (info.Email == "")
                                        {
                                            cInfoMail += item.numconfirmacion + "<br />";
                                        }
                                    }

                                    if (info.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = info.Phone != "" ? info.Phone : info.UnformattedPhone;
                                        //phone.phone = info.Phone;
                                        phone.phoneTypeID = 4;//unknown
                                        phone.doNotCall = false;
                                        phone.ext = null;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                    }

                                    if (info.Email == null && info.Phone == null)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }

                                    #region "reservation info"
                                    var rsv = new tblReservations();
                                    var _placeID = item.idresort != null ? places.Count(m => m.frontOfficeResortID == item.idresort) > 0 ? places.Single(m => m.frontOfficeResortID == item.idresort).placeID : (long?)null : (long?)null;
                                    rsv.reservationID = Guid.NewGuid();
                                    rsv.arrivalDate = item.llegada;
                                    rsv.departureDate = item.salida;
                                    rsv.placeID = _placeID;
                                    rsv.roomTypeID = _placeID != null ? roomTypes.Count(m => m.Key == _placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).Count(m => m.roomTypeCode == item.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).FirstOrDefault(m => m.roomTypeCode == item.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                    rsv.roomNumber = item.NumHab;
                                    rsv.adults = item.Adultos ?? 0;
                                    rsv.children = item.Ninos ?? 0;
                                    //rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                    rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("meal plan") != -1 || item.TipoPlan.ToLower().IndexOf("mp") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                    rsv.certificateNumber = item.CRS;
                                    rsv.destinationID = _placeID != null ? places.Single(m => m.placeID == _placeID).destinationID : (long?)null;
                                    rsv.hotelConfirmationNumber = item.numconfirmacion;
                                    rsv.totalNights = item.CuartosNoche;
                                    rsv.sysWorkGroupID = session.WorkGroupID;
                                    rsv.guestsNames = item.Huesped;
                                    rsv.reservationComments = item.Comentario;
                                    rsv.reservationStatusID = item.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == item.codigostatusreservacion) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == item.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    rsv.dateSaved = now;
                                    rsv.frontOfficeReservationID = (long)item.idReservacion;
                                    rsv.frontOfficePlanType = item.TipoPlan;
                                    rsv.frontOfficeAgencyName = item.nameagencia;
                                    rsv.frontOfficeMarketCode = item.Procedencia;
                                    rsv.frontOfficeRoomListID = item.idroomlist;
                                    #endregion
                                    //search for guestHubID
                                    var guestHubID = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeGuestID == item.idhuesped && m.frontOfficeResortID == item.idresort);
                                    lead.guestHubID = guestHubID != null ? guestHubID.guestHubID : (Guid?)null;

                                    lead.tblReservations.Add(rsv);
                                    db.tblLeads.AddObject(lead);
                                    #endregion
                                    counter++;
                                    a += (a == "" ? "" : ",") + item.Procedencia;
                                    status = "Imported";
                                }

                                bodyMail += "<br />Guest ID: " + item.idhuesped + ", Reservation ID: " + item.idReservacion + ", Name: " + item.nombres + ", Hotel Confirmation Number: " + item.numconfirmacion + ", Status: " + status;
                            }
                            catch (Exception ex)
                            {
                                a += (a == "" ? "-" : ",-") + item.Procedencia + "Message: " + ex.Message + "<br />Exception: " + ex.InnerException;
                                bodyMail += "<br />Guest ID: " + item.idhuesped + ", Reservation ID: " + item.idReservacion + ", Name: " + item.nombres + ", Hotel Confirmation Number: " + item.numconfirmacion + ", Status: NOT Imported";
                                bodyMail += new JavaScriptSerializer().Serialize(item) + "<br />";
                                bodyMail += "<br />Eror Message: " + ex.Message + "<br />Exception: " + ex.InnerException;
                            }
                        }
                        db.SaveChanges();
                        _list.Add(new KeyValuePair<string, SelectListItem>(userName, new SelectListItem() { Value = arrivals.Key, Text = counter.ToString() }));
                        #endregion
                    }
                    else
                    {
                        #region "code not assigned to user yet"
                        //el marketCode no está asignado a ningun usuario, definir accion
                        //Guid roleID = Guid.Parse("6EE4BCC6-93A5-4BA6-A448-F0F0B76BB41C");//Supervisor
                        Guid roleID = Guid.Parse("65946EF0-A2CA-483C-8B84-66A6C74978CB");//Operation Manager
                        supervisorUser = from u in db.aspnet_Users
                                         join p in db.tblUsers_SysWorkGroups on u.UserId equals p.userID
                                         join up in db.tblUserProfiles on u.UserId equals up.userID
                                         where p.roleID == roleID
                                         && p.sysWorkGroupID == session.WorkGroupID
                                         && !u.aspnet_Membership.IsLockedOut
                                         select up;

                        userName = supervisorUser.FirstOrDefault().firstName + " " + supervisorUser.FirstOrDefault().lastName;
                        var counter = 0;
                        var leadsToImport = new tblLeads();
                        bodyMail += "<br /><br />Market Code Not Assigned: " + arrivals.Key;
                        foreach (var item in arrivals)
                        {
                            try
                            {
                                var existingLead = db.tblLeads.Where(m => m.frontOfficeGuestID == item.idhuesped && m.frontOfficeResortID == item.idresort);
                                var status = "";
                                if (existingLead.Count() > 0)
                                {
                                    status = "Lead imported on: " + existingLead.OrderByDescending(m => m.inputDateTime).FirstOrDefault().inputDateTime.ToString("yyyy-MM-dd hh:mm") + ".";
                                    var existingReservation = existingLead.FirstOrDefault().tblReservations.OrderByDescending(m => m.arrivalDate).FirstOrDefault();
                                    DateTime departure = existingReservation.departureDate.Value;
                                    DateTime llegada = item.llegada.Value;
                                    var lead = existingLead.FirstOrDefault();
                                    var _placeID = item.idresort != null ? places.Count(m => m.frontOfficeResortID == item.idresort) > 0 ? places.Single(m => m.frontOfficeResortID == item.idresort).placeID : (long?)null : (long?)null;
                                    if (existingReservation.frontOfficeReservationID != item.idReservacion && departure.Date != llegada.Date)//la llegada de la rsv a importar es distinta de la salida de la rsv importada
                                    {
                                        #region "Fields Mapping"

                                        var rsv = new tblReservations();
                                        rsv.reservationID = Guid.NewGuid();
                                        rsv.arrivalDate = item.llegada;
                                        rsv.departureDate = item.salida;
                                        rsv.placeID = _placeID;
                                        rsv.roomTypeID = _placeID != null ? roomTypes.Count(m => m.Key == _placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).Count(m => m.roomTypeCode == item.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).FirstOrDefault(m => m.roomTypeCode == item.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                        rsv.roomNumber = item.NumHab;
                                        rsv.adults = item.Adultos ?? 0;
                                        rsv.children = item.Ninos ?? 0;
                                        //rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("meal plan") != -1 || item.TipoPlan.ToLower().IndexOf("mp") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.certificateNumber = item.CRS;
                                        rsv.destinationID = _placeID != null ? places.Single(m => m.placeID == _placeID).destinationID : (long?)null;
                                        rsv.hotelConfirmationNumber = item.numconfirmacion;
                                        rsv.totalNights = item.CuartosNoche;
                                        rsv.sysWorkGroupID = session.WorkGroupID;
                                        rsv.guestsNames = item.Huesped;
                                        rsv.reservationComments = item.Comentario;
                                        rsv.reservationStatusID = item.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == item.codigostatusreservacion) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == item.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                        rsv.dateSaved = now;
                                        rsv.frontOfficeReservationID = (long)item.idReservacion;
                                        rsv.frontOfficePlanType = item.TipoPlan;
                                        rsv.frontOfficeAgencyName = item.nameagencia;
                                        rsv.frontOfficeMarketCode = item.Procedencia;
                                        rsv.frontOfficeRoomListID = item.idroomlist;
                                        //search for guestHubID
                                        var guestHubID = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeGuestID == item.idhuesped && m.frontOfficeResortID == item.idresort);
                                        lead.guestHubID = guestHubID != null ? guestHubID.guestHubID : (Guid?)null;
                                        if (item.codigostatusreservacion != null && item.codigostatusreservacion == "CA")
                                        {
                                            lead.leadStatusID = 4;//canceled
                                        }
                                        lead.tblReservations.Add(rsv);
                                        #endregion
                                        counter++;
                                        a += (a == "" ? "" : ",") + item.Procedencia;
                                        status += " Reservation imported";
                                    }
                                    else
                                    {
                                        #region "Fields Mapping"
                                        var rsv = existingReservation;
                                        rsv.arrivalDate = item.llegada;
                                        rsv.departureDate = item.salida;
                                        rsv.placeID = _placeID;
                                        rsv.roomTypeID = _placeID != null ? roomTypes.Count(m => m.Key == _placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).Count(m => m.roomTypeCode == item.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).FirstOrDefault(m => m.roomTypeCode == item.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                        rsv.roomNumber = item.NumHab;
                                        rsv.adults = item.Adultos ?? 0;
                                        rsv.children = item.Ninos ?? 0;
                                        // rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("meal plan") != -1 || item.TipoPlan.ToLower().IndexOf("mp") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                        rsv.certificateNumber = item.CRS;
                                        rsv.destinationID = _placeID != null ? places.Single(m => m.placeID == _placeID).destinationID : (long?)null;
                                        rsv.hotelConfirmationNumber = item.numconfirmacion;
                                        rsv.totalNights = item.CuartosNoche;
                                        rsv.sysWorkGroupID = session.WorkGroupID;
                                        rsv.guestsNames = item.Huesped;
                                        rsv.reservationComments = item.Comentario;
                                        rsv.reservationStatusID = item.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == item.codigostatusreservacion) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == item.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                        rsv.frontOfficeReservationID = (long)item.idReservacion;
                                        rsv.frontOfficePlanType = item.TipoPlan;
                                        rsv.frontOfficeAgencyName = item.nameagencia;
                                        rsv.frontOfficeMarketCode = item.Procedencia;
                                        rsv.frontOfficeRoomListID = item.idroomlist;
                                        #endregion
                                        if (item.codigostatusreservacion != null && item.codigostatusreservacion == "CA")
                                        {
                                            lead.leadStatusID = 4;//canceled
                                        }
                                        counter++;
                                        a += (a == "" ? "" : ",") + item.Procedencia;
                                        status += " Reservation updated";
                                    }

                                    if (lead.tblLeadEmails.Count() == 0 && lead.tblPhones.Count() == 0)
                                    {
                                        var info = FrontOfficeDataModel.GetContactInfo((int)item.idresort, item.idhuesped);
                                        if (info.Email != null)
                                        {
                                            var leadEmail = new tblLeadEmails();
                                            leadEmail.email = info.Email != "" ? info.Email : info.UnformattedEmail;
                                            leadEmail.main = true;
                                            lead.tblLeadEmails.Add(leadEmail);
                                            if (info.Email == "")
                                            {
                                                cInfoMail += item.numconfirmacion + "<br />";
                                            }
                                        }
                                        if (info.Phone != null)
                                        {
                                            var phone = new tblPhones();
                                            phone.phone = info.Phone;
                                            phone.phoneTypeID = 4;//unkown
                                            phone.doNotCall = false;
                                            phone.ext = null;
                                            phone.main = true;
                                            lead.tblPhones.Add(phone);
                                        }

                                        if (info.Email == null && info.Phone == null)
                                        {
                                            lead.bookingStatusID = 15;//not contactable
                                        }
                                        else
                                        {
                                            if (lead.bookingStatusID == 15)
                                            {
                                                lead.bookingStatusID = 10;//not contacted
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    #region "Fields Mapping"
                                    var lead = new tblLeads();
                                    lead.leadID = Guid.NewGuid();
                                    lead.initialTerminalID = terminals.FirstOrDefault();
                                    lead.terminalID = terminals.FirstOrDefault();
                                    lead.personalTitleID = item.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == item.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.Single(m => m.Key == item.Titulo).Value) : (int?)null : (int?)null;
                                    lead.firstName = item.nombres;
                                    lead.lastName = item.apellidopaterno + (item.apellidomaterno != "" && item.apellidomaterno != null ? " " + item.apellidomaterno : "");
                                    lead.countryID = item.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == item.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.Single(m => m.Key == item.codepais).Value) : (int?)null : (int?)null;
                                    lead.bookingStatusID = 10;//not contacted
                                    lead.inputByUserID = session.UserID;
                                    lead.assignedToUserID = supervisorUser.FirstOrDefault().userID;
                                    lead.inputDateTime = now;
                                    lead.assignationDate = now;
                                    lead.inputMethodID = 2;//import
                                    lead.leadStatusID = item.codigostatusreservacion != null && item.codigostatusreservacion == "CA" ? 4 : 2;//assigned
                                    lead.isTest = false;
                                    lead.frontOfficeGuestID = (int)item.idhuesped;
                                    lead.frontOfficeResortID = (int)item.idresort;

                                    var info = FrontOfficeDataModel.GetContactInfo((int)item.idresort, item.idhuesped);
                                    if (info.Email != null)
                                    {
                                        var leadEmail = new tblLeadEmails();
                                        leadEmail.email = info.Email != "" ? info.Email : info.UnformattedEmail;
                                        leadEmail.main = true;
                                        lead.tblLeadEmails.Add(leadEmail);
                                        if (info.Email == "")
                                        {
                                            cInfoMail += item.numconfirmacion + "<br />";
                                        }
                                    }
                                    if (info.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = info.Phone;
                                        phone.phoneTypeID = 4;//unkown
                                        phone.doNotCall = false;
                                        phone.ext = null;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                    }

                                    if (info.Email == null && info.Phone == null)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }

                                    var rsv = new tblReservations();
                                    var _placeID = item.idresort != null ? places.Count(m => m.frontOfficeResortID == item.idresort) > 0 ? places.Single(m => m.frontOfficeResortID == item.idresort).placeID : (long?)null : (long?)null;
                                    rsv.reservationID = Guid.NewGuid();
                                    rsv.arrivalDate = item.llegada;
                                    rsv.departureDate = item.salida;
                                    rsv.placeID = _placeID;
                                    rsv.roomTypeID = _placeID != null ? roomTypes.Count(m => m.Key == _placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).Count(m => m.roomTypeCode == item.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == _placeID).FirstOrDefault(m => m.roomTypeCode == item.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                    rsv.roomNumber = item.NumHab;
                                    rsv.adults = item.Adultos ?? 0;
                                    rsv.children = item.Ninos ?? 0;
                                    //rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                    rsv.planTypeID = item.TipoPlan.ToLower().IndexOf("all inclusive") != -1 || item.TipoPlan.ToLower().IndexOf("todo incluido") != -1 || item.TipoPlan.ToLower().IndexOf("meal plan") != -1 || item.TipoPlan.ToLower().IndexOf("mp") != -1 || item.TipoPlan.ToLower().IndexOf("ai") != -1 ? 1 : item.TipoPlan.ToLower().IndexOf("european") != -1 || item.TipoPlan.ToLower().IndexOf("europeo") != -1 || item.TipoPlan.ToLower().IndexOf(" ep ") != -1 ? 2 : (int?)null;
                                    rsv.certificateNumber = item.CRS;
                                    rsv.destinationID = _placeID != null ? places.Single(m => m.placeID == _placeID).destinationID : (long?)null;
                                    rsv.hotelConfirmationNumber = item.numconfirmacion;
                                    rsv.totalNights = item.CuartosNoche;
                                    rsv.sysWorkGroupID = session.WorkGroupID;
                                    rsv.guestsNames = item.Huesped;
                                    rsv.reservationComments = item.Comentario;
                                    rsv.reservationStatusID = item.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == item.codigostatusreservacion) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == item.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    rsv.dateSaved = now;
                                    rsv.frontOfficeReservationID = (long)item.idReservacion;
                                    rsv.frontOfficePlanType = item.TipoPlan;
                                    rsv.frontOfficeAgencyName = item.nameagencia;
                                    rsv.frontOfficeMarketCode = item.Procedencia;
                                    rsv.frontOfficeRoomListID = item.idroomlist;
                                    //search for guestHubID
                                    var guestHubID = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeGuestID == item.idhuesped && m.frontOfficeResortID == item.idresort);
                                    lead.guestHubID = guestHubID != null ? guestHubID.guestHubID : (Guid?)null;

                                    lead.tblReservations.Add(rsv);
                                    db.tblLeads.AddObject(lead);
                                    #endregion
                                    counter++;
                                    a += (a == "" ? "" : ",") + item.Procedencia;
                                    status = "Imported";
                                }
                                bodyMail += "<br />Guest ID: " + item.idhuesped + ", Reservation ID: " + item.idReservacion + ", Name: " + item.nombres + ", Hotel Confirmation Number: " + item.numconfirmacion + ", Status: " + status;
                            }
                            catch (Exception ex)
                            {
                                a += (a == "" ? "-" : ",-") + item.Procedencia + ", Message: " + ex.Message + "<br />Exception:" + ex.InnerException;
                                bodyMail += "<br />Guest ID: " + item.idhuesped + ", Reservation ID: " + item.idReservacion + ", Name: " + item.nombres + ", Hotel Confirmation Number: " + item.numconfirmacion + ", Status: NOT Imported";
                                bodyMail += "<br />Eror Message: " + ex.Message + "<br />Exception: " + ex.InnerException;
                            }
                        }
                        db.SaveChanges();
                        _list.Add(new KeyValuePair<string, SelectListItem>(userName, new SelectListItem() { Value = arrivals.Key, Text = counter.ToString() }));
                        #endregion
                    }
                }

                bodyMail += "<br /><br /><br />Leads with Market Codes Not Assigned to any user were assigned to the Supervisor " + (supervisorUser != null ? supervisorUser.FirstOrDefault().firstName + " " + supervisorUser.FirstOrDefault().lastName : "");
                bodyMail += "<br />Contact corresponing person in order to assign Market Codes and reassign leads if needed.";
                var email = EmailNotifications.GetSystemEmail(bodyMail);
                email.Subject = "Pre Arrival Importation By " + session.User + ", Resort: " + resortName + " Range: " + fDate.ToString("yyyy-MM-dd") + " - " + tDate.ToString("yyyy-MM-dd");
                //email.CC.Add(session.Email);
                //EmailNotifications.Send(email, false);
                EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                if (cInfoMail != "")
                {
                    cInfoMail = "There were found format issues on Contact Information importation. Please verify and correct.<br /><br />"
                    + "Hotel Confirmation Number(s) of Reservations with issues on contact information: <br /><br />" + cInfoMail;
                    var cEmail = EmailNotifications.GetSystemEmail(cInfoMail, session.Email);
                    cEmail.Subject = "ePlat - Issues on " + resortName + " Imported Leads Information";
                    cEmail.Bcc.Add("efalcon@villagroup.com");
                    //EmailNotifications.Send(cEmail, false);
                    EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = cEmail } });
                }
                //end new
                var r = _list.GroupBy(m => m.Key);
                var cosa = new List<KeyValuePair<string, List<SelectListItem>>>();
                foreach (var i in r)
                {
                    var l = new List<SelectListItem>();
                    foreach (var x in i)
                    {
                        l.Add(new SelectListItem() { Value = x.Value.Value, Text = x.Value.Text });
                    }
                    cosa.Add(new KeyValuePair<string, List<SelectListItem>>(i.Key, l));
                }
                listItem.Cosa = cosa;
                listItem.UserCodeCountList = _list;
                listItem.a = a;
                results.Add(listItem);
            }

            return results;
        }

        public List<ImportModel> GetArrivalsToImport(SearchToImportModel model)
        {
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = int.MaxValue;
            var list = new List<ImportModel>();
            var leadSources = LeadSourceDataModel.GetLeadSourcesByTerminal();
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator");
            try
            {
                var serializer = new JavaScriptSerializer();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                //var leadSources = LeadSourceDataModel.GetLeadSourcesByTerminal();
                var resorts = model.Resorts;
                var now = DateTime.Now;
                //var assignation = new List<object>();
                var frontOfficeResortID = model.SearchToImport_ImportResort.FirstOrDefault();
                var resort = db.tblPlaces.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID);
                var roomTypes = db.tblRoomTypes.Where(m => resort.placeID == m.placeID).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode, m.roomType }).ToList();
                var marketCodes = db.tblMarketCodes.Where(m => m.frontOfficeResortID == frontOfficeResortID && m.leadSourceID != null);
                var leadSourcesPerTerminal = db.tblTerminals_LeadSources.Where(m => m.terminalID == terminals.FirstOrDefault()).Select(m => (long?)m.leadSourceID).ToList();
                var marketCodesPerTerminal = marketCodes.Where(m => leadSourcesPerTerminal.Contains(m.leadSourceID)).ToList();
                var arrivals = FrontOfficeDataModel.GetArrivals(frontOfficeResortID, DateTime.Parse(model.Search_I_ImportArrivalDate), DateTime.Parse(model.Search_F_ImportArrivalDate).AddDays(1).AddSeconds(-1));
                arrivals = arrivals.Where(m => m.codigostatusreservacion != "CA").ToList();

                var counter = 0;

                #region"filter arrivals by date, depending on terminal"
                if (terminals.Contains(10))// Pre Arrival RC
                {
                    if (frontOfficeResortID != 12)// Villa del Palmar Loreto
                    {
                        arrivals = arrivals.Where(m => m.llegada.Value.Day % 2 == 0).ToList();
                    }
                }
                else if (terminals.Contains(80))// Pre Arrival VG
                {
                    arrivals = arrivals.Where(m => m.llegada.Value.Day % 2 != 0).ToList();
                }
                #endregion

                //new
                var contractsPermitted = db.tblContracts_Places.Where(m => terminals.Contains(m.terminalID) && m.frontOfficeResortID == frontOfficeResortID && m.fromDate <= now && (m.toDate == null || m.toDate > now));
                if (contractsPermitted.Count() > 0 && !isAdmin)
                {
                    var _arrivals = new List<FrontOfficeViewModel.LlegadasResult>();
                    foreach (var contract in contractsPermitted)
                    {
                        _arrivals = _arrivals.Concat(arrivals.Where(m => m.Contrato.IndexOf(contract.contractPrefix) != -1 || m.Contrato == "")).ToList();
                    }
                    arrivals = _arrivals;
                }
                //end new
                #region "users preassignation"

                var assignation = (from ul in db.tblUsers_LeadSources
                                   join u in db.aspnet_Membership on ul.userID equals u.UserId
                                   where terminals.FirstOrDefault() == ul.terminalID
                                   && marketCodes.Select(m => m.leadSourceID).Contains(ul.leadSourceID)
                                   && ul.frontOfficeResortID == frontOfficeResortID
                                   && ul.fromDate <= now
                                   && (ul.toDate == null || ul.toDate >= now)
                                   && !u.IsLockedOut
                                   select new { Key = ul.leadSourceID, Value = ul.userID }).ToList();

                //assignation = _leadSourcesPerUser.ToList();
                #endregion

                foreach (var arrival in arrivals)
                {
                    //if (((frontOfficeResortID == 5 || frontOfficeResortID == 7) && marketCodesPerTerminal.Select(m => m.marketCode).Contains(arrival.Procedencia))
                    if (((frontOfficeResortID == 5 || frontOfficeResortID == 7) && (marketCodesPerTerminal.Select(m => m.marketCode).Contains(arrival.Procedencia) || isAdmin))
                        || (frontOfficeResortID == 11 && (terminals.Contains(16) || marketCodesPerTerminal.Select(m => m.marketCode).Contains(arrival.Procedencia)))
                        || (frontOfficeResortID != 11 && frontOfficeResortID != 5 && frontOfficeResortID != 7))
                    {
                        var frontOfficeGuestID = arrival.idhuesped;
                        var frontOfficeReservationID = arrival.idReservacion;
                        var frontOfficeRoomListID = arrival.idroomlist != null ? arrival.idroomlist : (int?)null;
                        var roomType = roomTypes.Count(m => m.roomTypeCode == arrival.TipoHab || m.roomType == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab || m.roomType == arrival.TipoHab) : null;
                        if (roomType == null)
                        {
                            try
                            {
                                tblRoomTypes rt = new tblRoomTypes();
                                rt.roomType = arrival.TipoHab;
                                rt.roomTypeCode = arrival.TipoHab;
                                rt.placeID = resort.placeID;
                                rt.quantity = null;
                                rt.dateSaved = now;
                                rt.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                                db.tblRoomTypes.AddObject(rt);
                                db.SaveChanges();
                                roomTypes.Add(new { rt.placeID, rt.roomTypeID, rt.roomTypeCode, rt.roomType });
                                var email = EmailNotifications.GetSystemEmail("Room Type " + arrival.TipoHab + " created for " + resort.placeID, "efalcon@villagroup.com");
                                EmailNotifications.Send(email);
                                list.FirstOrDefault().Message = "New RoomTypes Created";
                            }
                            catch { }
                            //throw new Exception("room type out of range");
                        }

                        var exists = (from r in db.tblReservations
                                      join l in db.tblLeads on r.leadID equals l.leadID
                                      where l.frontOfficeResortID == frontOfficeResortID
                                      && r.frontOfficeReservationID == frontOfficeReservationID
                                      && ((r.frontOfficeRoomListID == null && frontOfficeRoomListID == null) || r.frontOfficeRoomListID == frontOfficeRoomListID)
                                      select new
                                      {
                                          r.reservationID,
                                          r.roomTypeID,
                                          l.leadSourceID,
                                          l.assignedToUserID
                                      }).ToList();

                        var _item = serializer.Serialize(arrival);
                        var item = serializer.Deserialize<ImportModel>(_item);

                        if (exists != null && exists.Count() > 0)
                        {
                            item.Status = true;
                            item.Message = "Already Imported";
                            item.LeadSource = exists.FirstOrDefault().leadSourceID.ToString();
                            item.AssignedToUserID = exists.FirstOrDefault().assignedToUserID;
                            //en teoría, no existirá habitación sin tipo definido
                        }
                        else
                        {
                            if (roomTypes.Count(m => m.roomType == arrival.TipoHab || m.roomTypeCode == arrival.TipoHab) == 0)
                            {
                                try
                                {
                                    tblRoomTypes rt = new tblRoomTypes();
                                    rt.roomType = item.TipoHab;
                                    rt.roomTypeCode = item.TipoHab;
                                    rt.placeID = resort.placeID;
                                    rt.quantity = null;
                                    rt.dateSaved = now;
                                    rt.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                                    db.tblRoomTypes.AddObject(rt);
                                    db.SaveChanges();
                                    roomTypes.Add(new { rt.placeID, rt.roomTypeID, rt.roomTypeCode, rt.roomType });
                                    var email = EmailNotifications.GetSystemEmail("Room Type " + item.TipoHab + " created for " + roomType.placeID, "efalcon@villagroup.com");
                                    EmailNotifications.Send(email);
                                    list.FirstOrDefault().Message = "New RoomTypes Created";
                                }
                                catch { }
                            }
                            item.LeadSource = marketCodesPerTerminal.Count(m => m.marketCode == item.Procedencia) > 0 ? marketCodesPerTerminal.FirstOrDefault(m => m.marketCode == item.Procedencia).leadSourceID.ToString() : null;

                            item.Phone = arrival.telefono != null && arrival.telefono != "" ? 1 : 0;
                            item.Correo = arrival.email != null && arrival.email != "" ? 1 : 0;
                        }
                        item.Comentario = item.Comentario.Replace("mailto:", "").Replace("<", "").Replace(">", "").Replace("'", "&apos;").Replace("&#39;", "");
                        item.Resort = resorts.Count(m => int.Parse(m.Value) == resort.frontOfficeResortID) > 0 ? resorts.FirstOrDefault(m => int.Parse(m.Value) == resort.frontOfficeResortID).Text : "";
                        item.Arrival = item.llegada != null ? item.llegada.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        item.Departure = item.salida != null ? item.salida.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        item.CheckIn = item.FechaHoraCheckin != null ? item.FechaHoraCheckin.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        item.llegada = (DateTime?)null;
                        item.salida = (DateTime?)null;
                        item.FechaHoraCheckin = (DateTime?)null;
                        item.ListAssignation = assignation.ToList<object>();
                        item.Index = counter;
                        list.Add(item);
                        counter++;
                    }
                }

                if (list.Count() > 0)
                {
                    leadSources.Insert(0, ListItems.Default("--None--", null));
                    leadSources.Insert(1, ListItems.Default("--All--", "", true));
                    list.FirstOrDefault().LeadSources = leadSources;
                    list.FirstOrDefault().Users = UserDataModel.GetUsersBySupervisor(null, true, false);
                }
            }
            catch (Exception ex)
            {
                list.FirstOrDefault().LeadSources = leadSources;
                list.FirstOrDefault().Users = UserDataModel.GetUsersBySupervisor(null, true, false);
            }

            return list;
        }

        public List<ImportModel> _GetArrivalsToImport(SearchToImportModel model)
        {
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = int.MaxValue;
            var list = new List<ImportModel>();

            try
            {
                var serializer = new JavaScriptSerializer();
                var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
                var leadSources = LeadSourceDataModel.GetLeadSourcesByTerminal();
                var resorts = model.Resorts;
                var now = DateTime.Now;
                var assignation = new List<KeyValuePair<long, Guid>>();
                var frontOfficeResortID = model.SearchToImport_ImportResort.FirstOrDefault();
                var resort = db.tblPlaces.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID);
                var roomTypes = db.tblRoomTypes.Where(m => resort.placeID == m.placeID).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode, m.roomType }).ToList();
                //var roomTypes = db.tblRoomTypes.Where(m => m.tblPlaces.frontOfficeResortID == resort).Select(m => new { m.placeID, m.roomType, m.roomTypeCode, m.roomTypeID }).ToList();
                var marketCodes = db.tblMarketCodes.Where(m => m.frontOfficeResortID == frontOfficeResortID && m.leadSourceID != null);
                var leadSourcesPerTerminal = db.tblTerminals_LeadSources.Where(m => m.terminalID == terminals.FirstOrDefault()).Select(m => (long?)m.leadSourceID).ToList();
                var marketCodesPerTerminal = marketCodes.Where(m => leadSourcesPerTerminal.Contains(m.leadSourceID)).ToList();
                var arrivals = FrontOfficeDataModel.GetArrivals(frontOfficeResortID, DateTime.Parse(model.Search_I_ImportArrivalDate), DateTime.Parse(model.Search_F_ImportArrivalDate));
                arrivals = arrivals.Where(m => m.codigostatusreservacion != "CA").ToList();
                var counter = 0;

                #region"filter arrivals by date, depending on terminal"
                if (terminals.Contains(10))// Pre Arrival RC
                {
                    if (frontOfficeResortID != 12)// Villa del Palmar Loreto
                    {
                        arrivals = arrivals.Where(m => m.llegada.Value.Day % 2 == 0).ToList();
                    }
                }
                else if (terminals.Contains(80))// Pre Arrival VG
                {
                    arrivals = arrivals.Where(m => m.llegada.Value.Day % 2 != 0).ToList();
                }
                #endregion

                //new
                var contractsPermitted = db.tblContracts_Places.Where(m => terminals.Contains(m.terminalID) && m.frontOfficeResortID == frontOfficeResortID && m.fromDate <= now && (m.toDate == null || m.toDate > now));
                if (contractsPermitted.Count() > 0 && !GeneralFunctions.IsUserInRole("Administrator"))
                {
                    var _arrivals = new List<FrontOfficeViewModel.LlegadasResult>();
                    foreach (var contract in contractsPermitted)
                    {
                        _arrivals = _arrivals.Concat(arrivals.Where(m => m.Contrato.IndexOf(contract.contractPrefix) != -1 || m.Contrato == "")).ToList();
                    }
                    arrivals = _arrivals;
                }
                //end new
                #region "users preassignation"
                var leadSourcesPerUser = (from ul in db.tblUsers_LeadSources
                                          join u in db.aspnet_Membership on ul.userID equals u.UserId
                                          where terminals.FirstOrDefault() == ul.terminalID
                                          && marketCodes.Select(m => m.leadSourceID).Contains(ul.leadSourceID)
                                          && ul.frontOfficeResortID == frontOfficeResortID
                                          && ul.fromDate <= now
                                          && (ul.toDate == null || ul.toDate >= now)
                                          && !u.IsLockedOut
                                          select ul).ToList();

                foreach (var i in leadSourcesPerUser)
                {
                    assignation.Add(new KeyValuePair<long, Guid>(i.leadSourceID, i.userID));
                }
                #endregion

                foreach (var arrival in arrivals)
                {
                    if (frontOfficeResortID != 11 || (frontOfficeResortID == 11 && (terminals.Contains(16) || marketCodesPerTerminal.Select(m => m.marketCode).Contains(arrival.Procedencia))))
                    {
                        var frontOfficeGuestID = arrival.idhuesped;
                        var frontOfficeReservationID = arrival.idReservacion;
                        //var roomType = roomTypes.FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab);
                        var roomType = roomTypes.FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab || m.roomType == arrival.TipoHab);

                        var exists = (from r in db.tblReservations
                                      join l in db.tblLeads on r.leadID equals l.leadID
                                      where l.frontOfficeResortID == frontOfficeResortID
                                      && l.frontOfficeGuestID == frontOfficeGuestID
                                      && r.frontOfficeReservationID == frontOfficeReservationID
                                      //&& r.roomTypeID == roomType.roomTypeID
                                      //&& (roomType != null && r.roomTypeID == roomType.roomTypeID)
                                      select new
                                      {
                                          r.reservationID,
                                          r.roomTypeID,
                                          l.leadSourceID,
                                          l.assignedToUserID
                                      }).ToList();

                        exists = roomType != null ? exists.Where(m => m.roomTypeID == roomType.roomTypeID).ToList() : null;

                        var _item = serializer.Serialize(arrival);
                        var item = serializer.Deserialize<ImportModel>(_item);

                        if (exists != null && exists.Count() > 0)
                        {
                            item.Status = true;
                            item.Message = "Already Imported";
                            item.LeadSource = exists.FirstOrDefault().leadSourceID.ToString();
                            item.AssignedToUserID = exists.FirstOrDefault().assignedToUserID;
                            //en teoría, no existirá habitación sin tipo definido
                        }
                        else
                        {
                            if (roomTypes.Count(m => m.roomType == arrival.TipoHab || m.roomTypeCode == arrival.TipoHab) == 0)
                            {
                                tblRoomTypes rt = new tblRoomTypes();
                                rt.roomType = item.TipoHab;
                                rt.roomTypeCode = item.TipoHab;
                                rt.placeID = resort.placeID;
                                rt.quantity = null;
                                rt.dateSaved = now;
                                rt.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                                db.tblRoomTypes.AddObject(rt);
                                db.SaveChanges();
                                roomTypes.Add(new { rt.placeID, rt.roomTypeID, rt.roomTypeCode, rt.roomType });
                                var email = EmailNotifications.GetSystemEmail("Room Type " + item.TipoHab + " created for " + roomType.placeID, "efalcon@villagroup.com");
                                EmailNotifications.Send(email);
                                list.FirstOrDefault().Message = "New RoomTypes Created";
                            }
                            item.LeadSource = marketCodesPerTerminal.Count(m => m.marketCode == item.Procedencia) > 0 ? marketCodesPerTerminal.FirstOrDefault(m => m.marketCode == item.Procedencia).leadSourceID.ToString() : null;
                            //else
                            //{
                            //    item.LeadSource = marketCodesPerTerminal.Count(m => m.marketCode == item.Procedencia) > 0 ? marketCodesPerTerminal.FirstOrDefault(m => m.marketCode == item.Procedencia).leadSourceID.ToString() : null;

                            //    item.Comentario = item.Comentario.Replace("mailto:", "").Replace("<", "").Replace(">", "").Replace("'", "&apos;").Replace("&#39;", "");
                            //    item.Resort = resorts.FirstOrDefault(m => int.Parse(m.Value) == resort.frontOfficeResortID).Text;
                            //    item.Arrival = item.llegada != null ? item.llegada.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                            //    item.Departure = item.salida != null ? item.salida.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                            //    item.CheckIn = item.FechaHoraCheckin != null ? item.FechaHoraCheckin.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                            //    item.llegada = (DateTime?)null;
                            //    item.salida = (DateTime?)null;
                            //    item.FechaHoraCheckin = (DateTime?)null;
                            //    item.ListAssignation = assignation;
                            //    item.Index = counter;
                            //    list.Add(item);
                            //    counter++;
                            //}
                        }
                        item.Comentario = item.Comentario.Replace("mailto:", "").Replace("<", "").Replace(">", "").Replace("'", "&apos;").Replace("&#39;", "");
                        item.Resort = resorts.FirstOrDefault(m => int.Parse(m.Value) == resort.frontOfficeResortID).Text;
                        item.Arrival = item.llegada != null ? item.llegada.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        item.Departure = item.salida != null ? item.salida.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        item.CheckIn = item.FechaHoraCheckin != null ? item.FechaHoraCheckin.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        item.llegada = (DateTime?)null;
                        item.salida = (DateTime?)null;
                        item.FechaHoraCheckin = (DateTime?)null;
                        //item.ListAssignation = assignation;
                        item.Index = counter;
                        list.Add(item);
                        counter++;
                        //else
                        //{
                        //    if (roomTypes.Count(m => m.roomType == arrival.TipoHab || m.roomTypeCode == arrival.TipoHab) == 0)
                        //    {
                        //        tblRoomTypes rt = new tblRoomTypes();
                        //        rt.roomType = item.TipoHab;
                        //        rt.roomTypeCode = item.TipoHab;
                        //        rt.placeID = resort.placeID;
                        //        rt.quantity = null;
                        //        rt.dateSaved = now;
                        //        rt.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                        //        db.tblRoomTypes.AddObject(rt);
                        //        db.SaveChanges();
                        //        roomTypes.Add(new { rt.placeID, rt.roomTypeID, rt.roomTypeCode, rt.roomType });
                        //    }
                        //    item.LeadSource = marketCodesPerTerminal.Count(m => m.marketCode == item.Procedencia) > 0 ? marketCodesPerTerminal.FirstOrDefault(m => m.marketCode == item.Procedencia).leadSourceID.ToString() : null;
                        //}
                        //item.Comentario = item.Comentario.Replace("mailto:", "").Replace("<", "").Replace(">", "").Replace("'", "&apos;").Replace("&#39;", "");
                        //item.Resort = resorts.FirstOrDefault(m => int.Parse(m.Value) == resort.frontOfficeResortID).Text;
                        //item.Arrival = item.llegada != null ? item.llegada.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        //item.Departure = item.salida != null ? item.salida.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        //item.CheckIn = item.FechaHoraCheckin != null ? item.FechaHoraCheckin.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                        //item.llegada = (DateTime?)null;
                        //item.salida = (DateTime?)null;
                        //item.FechaHoraCheckin = (DateTime?)null;
                        //item.ListAssignation = assignation;
                        //item.Index = counter;
                        //list.Add(item);
                        //counter++;
                    }
                }

                if (list.Count() > 0)
                {
                    leadSources.Insert(0, ListItems.Default("--None--", null));
                    leadSources.Insert(1, ListItems.Default("--All--", "", true));
                    list.FirstOrDefault().LeadSources = leadSources;
                    list.FirstOrDefault().Users = UserDataModel.GetUsersBySupervisor(null, true, false);
                }
            }
            catch { }

            return list;
        }

        public List<ImportModel> __GetArrivalsToImport(SearchToImportModel model)
        {
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = int.MaxValue;
            var list = new List<ImportModel>();
            var serializer = new JavaScriptSerializer();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var leadSources = LeadSourceDataModel.GetLeadSourcesByTerminal();
            var resorts = model.Resorts;
            var now = DateTime.Now;
            var assignation = new List<KeyValuePair<long, Guid>>();
            var resort = model.SearchToImport_ImportResort.FirstOrDefault();
            var roomTypes = db.tblRoomTypes.Where(m => m.tblPlaces.frontOfficeResortID == resort).Select(m => new { m.placeID, m.roomType, m.roomTypeCode, m.roomTypeID }).ToList();
            var arrivals = FrontOfficeDataModel.GetArrivals(resort, DateTime.Parse(model.Search_I_ImportArrivalDate), DateTime.Parse(model.Search_F_ImportArrivalDate));
            arrivals = arrivals.Where(m => m.codigostatusreservacion != "CA").ToList();

            #region"filter arrivals by date, depending on terminal"
            if (terminals.Contains(10))// Pre Arrival RC
            {
                if (resort != 12)// Villa del Palmar Loreto
                {
                    arrivals = arrivals.Where(m => m.llegada.Value.Day % 2 == 0).ToList();
                }
            }
            else if (terminals.Contains(80))// Pre Arrival VG
            {
                arrivals = arrivals.Where(m => m.llegada.Value.Day % 2 != 0).ToList();
            }
            #endregion

            var marketCodes = db.tblMarketCodes.Where(m => m.frontOfficeResortID == resort && m.leadSourceID != null);
            //new
            var contractsPermitted = db.tblContracts_Places.Where(m => terminals.Contains(m.terminalID) && m.frontOfficeResortID == resort && m.fromDate <= now && (m.toDate == null || m.toDate > now));
            if (contractsPermitted.Count() > 0 && !GeneralFunctions.IsUserInRole("Administrator"))
            {
                var _arrivals = new List<FrontOfficeViewModel.LlegadasResult>();
                foreach (var contract in contractsPermitted)
                {
                    _arrivals = _arrivals.Concat(arrivals.Where(m => m.Contrato.IndexOf(contract.contractPrefix) != -1 || m.Contrato == "")).ToList();
                }
                arrivals = _arrivals;
            }
            //end new


            var ids = arrivals.Select(m => m.idReservacion).ToArray();
            var existingArrivals = (from r in db.tblReservations
                                    join l in db.tblLeads on r.leadID equals l.leadID
                                    join rt in db.tblRoomTypes on r.roomTypeID equals rt.roomTypeID into r_rt
                                    from rt in r_rt.DefaultIfEmpty()
                                    where terminals.Contains(l.terminalID)
                                    && l.frontOfficeResortID == resort
                                    && ids.Contains(r.frontOfficeReservationID)
                                    select new
                                    {
                                        r.reservationID,
                                        r.frontOfficeReservationID,
                                        r.roomTypeID,
                                        l.leadSourceID,
                                        l.assignedToUserID,
                                        rt.roomTypeCode,
                                        rt.roomType
                                    }).ToList();

            var leadSourcesPerTerminal = db.tblTerminals_LeadSources.Where(m => m.terminalID == terminals.FirstOrDefault()).Select(m => (long?)m.leadSourceID).ToList();
            var marketCodesPerTerminal = marketCodes.Where(m => leadSourcesPerTerminal.Contains(m.leadSourceID)).ToList();
            //var leadSourcesPerUser = db.tblUsers_LeadSources.Where(m => terminals.FirstOrDefault() == m.terminalID && marketCodes.Select(x => x.leadSourceID).Contains(m.leadSourceID) && m.frontOfficeResortID == resort && m.fromDate <= now && (m.toDate == null || m.toDate >= now)).ToList();
            var leadSourcesPerUser = (from ul in db.tblUsers_LeadSources
                                      join u in db.aspnet_Membership on ul.userID equals u.UserId
                                      where terminals.FirstOrDefault() == ul.terminalID
                                      && marketCodes.Select(m => m.leadSourceID).Contains(ul.leadSourceID)
                                      && ul.frontOfficeResortID == resort
                                      && ul.fromDate <= now
                                      && (ul.toDate == null || ul.toDate >= now)
                                      && !u.IsLockedOut
                                      select ul).ToList();


            foreach (var i in leadSourcesPerUser)
            {
                assignation.Add(new KeyValuePair<long, Guid>(i.leadSourceID, i.userID));
            }
            var counter = 0;
            foreach (var item in arrivals)
            {
                if (resort != 11 || (resort == 11 && (terminals.FirstOrDefault() == 16 || marketCodesPerTerminal.Select(m => m.marketCode).Contains(item.Procedencia))))//en caso de que sea VDP Cancun, se mostrarán SOLO reservaciones de codigos relacionados a lead sources si la consulta no es de prearrival tafer.
                {
                    var existingArrival = existingArrivals.FirstOrDefault(m => m.frontOfficeReservationID == item.idReservacion && (m.roomTypeID == null || m.roomTypeCode == item.TipoHab || m.roomType == item.TipoHab));
                    var _str = serializer.Serialize(item);
                    var str = serializer.Deserialize<ImportModel>(_str);
                    if (existingArrival != null)
                    {
                        str.Status = true;
                        str.Message = "Already Imported";
                        str.LeadSource = existingArrival.leadSourceID.ToString();
                        str.AssignedToUserID = existingArrival.assignedToUserID;

                        if (existingArrival.roomTypeID == null && item.TipoHab != null && item.TipoHab != "")
                        {
                            db.tblReservations.Single(m => m.reservationID == existingArrival.reservationID).roomTypeID = roomTypes.FirstOrDefault(m => m.roomType == item.TipoHab || m.roomTypeCode == item.TipoHab).roomTypeID;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (roomTypes.Count(m => m.roomType == item.TipoHab || m.roomTypeCode == item.TipoHab) == 0)
                        {
                            tblRoomTypes roomType = new tblRoomTypes();
                            roomType.roomType = item.TipoHab;
                            roomType.roomTypeCode = item.TipoHab;
                            roomType.placeID = roomTypes.FirstOrDefault().placeID;
                            roomType.quantity = null;
                            roomType.dateSaved = now;
                            roomType.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");
                            db.tblRoomTypes.AddObject(roomType);
                            db.SaveChanges();
                            roomTypes.Add(new { roomType.placeID, roomType.roomType, roomType.roomTypeCode, roomType.roomTypeID });
                            var email = EmailNotifications.GetSystemEmail("Room Type " + item.TipoHab + " created for " + roomType.placeID, "efalcon@villagroup.com");
                            EmailNotifications.Send(email);
                            list.FirstOrDefault().Message = "New RoomTypes Created";
                        }
                        str.LeadSource = marketCodesPerTerminal.Count(m => m.marketCode == str.Procedencia) > 0 ? marketCodesPerTerminal.FirstOrDefault(m => m.marketCode == str.Procedencia).leadSourceID.ToString() : null;
                    }
                    str.Comentario = str.Comentario.Replace("mailto:", "").Replace("<", "").Replace(">", "").Replace("'", "&apos;").Replace("&#39;", "");
                    str.Resort = resorts.FirstOrDefault(m => int.Parse(m.Value) == resort).Text;
                    str.Arrival = str.llegada != null ? str.llegada.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                    str.Departure = str.salida != null ? str.salida.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                    str.CheckIn = str.FechaHoraCheckin != null ? str.FechaHoraCheckin.Value.ToString("yyyy-MM-dd hh:mm tt") : "";
                    str.llegada = (DateTime?)null;
                    str.salida = (DateTime?)null;
                    str.FechaHoraCheckin = (DateTime?)null;
                    //str.ListAssignation = assignation;
                    str.Index = counter;
                    list.Add(str);
                    counter++;
                }
            }

            if (list.Count() > 0)
            {
                leadSources.Insert(0, ListItems.Default("--None--", null));
                leadSources.Insert(1, ListItems.Default("--All--", "", true));

                list.FirstOrDefault().LeadSources = leadSources;
                list.FirstOrDefault().Users = UserDataModel.GetUsersBySupervisor(null, true, false);
            }
            return list;
        }

        public List<ImportModel> UpdateArrivals(List<ImportModel> data)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            db.CommandTimeout = int.MaxValue;
            serializer.MaxJsonLength = int.MaxValue;
            DateTime now = DateTime.Now;

            #region "filtracion de llegadas por importar"
            var arrivals = data;
            var _resort = (int)arrivals.FirstOrDefault().idresort;
            #endregion

            foreach (var arrival in arrivals.OrderBy(m => m.Arrival))
            {
                try
                {
                    int frontOfficeResortID = (int)arrival.idresort;
                    var frontOfficeReservationID = (long?)arrival.idReservacion;
                    var frontOfficeRoomListID = arrival.idroomlist != null ? arrival.idroomlist : (int?)null;

                    var frontOfficeGuestID = arrival.idhuesped;
                    var guestHub = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID && m.frontOfficeGuestID == frontOfficeGuestID);
                    var noContactable = true;
                    var isNew = false;
                    var contactInfo = GetContactInfo(arrival);

                    //new
                    var _existing = from r in db.tblReservations
                                    where r.tblLeads.frontOfficeResortID == frontOfficeResortID
                                    && r.frontOfficeReservationID == frontOfficeReservationID
                                    select r;

                    tblLeads lead = new tblLeads();

                    if (arrival.Contrato != null && arrival.Contrato != "" && arrival.Contrato.IndexOf("ending") == -1 && arrival.Contrato.IndexOf("xchange") == -1)
                    {
                        if (_existing.Count(x => x.frontOfficeContractNumber == arrival.Contrato) > 0)
                        {
                            var arrivalDate = DateTime.Parse(arrival.Arrival).Date;
                            if (arrival.Split == true || _existing.Count(m => EntityFunctions.TruncateTime(m.departureDate.Value) == arrivalDate) > 0)//split or extension
                            {
                                //same contract, split or extension.
                                lead = _existing.FirstOrDefault(m => m.frontOfficeContractNumber == arrival.Contrato).tblLeads;
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                        noContactable = false;
                                    }
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                        noContactable = false;
                                    }
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }

                                if (lead.tblMemberInfo != null && lead.tblMemberInfo.Count() > 0)
                                {
                                    var member = lead.tblMemberInfo.FirstOrDefault();
                                    member.memberNumber = arrival.Contrato;
                                    member.clubType = null;
                                    member.revenues = 0;
                                    member.isNational = lead.countryID == 3;
                                    member.presentationConfirmed = false;
                                    member.hasOptions = false;
                                    member.isVIP = false;
                                    member.pushedToOnSiteConcierge = false;
                                    member.isAllInclusive = false;
                                    member.coOwner = null;
                                    member.memberName = null;
                                    member.contractNumber = null;
                                }

                                if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                                {
                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    //item.placeID = resort.placeID;
                                    //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    //item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    //lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                            else
                            {
                                //same contract but not split, not extension. => guest of member
                                //condition defined to prevent saving of same information from distinct tab in browser
                                if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS || m.hotelConfirmationNumber == arrival.numconfirmacion) == 0)
                                {
                                    isNew = true;
                                    #region "lead info"
                                    //lead.leadID = Guid.NewGuid();
                                    //lead.initialTerminalID = (long)terminal;
                                    //lead.terminalID = (long)terminal;
                                    lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                    lead.firstName = arrival.nombres;
                                    lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                    lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                    lead.bookingStatusID = 10;//not contacted
                                    lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                    lead.inputByUserID = session.UserID;
                                    lead.inputDateTime = now;
                                    lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                    lead.assignationDate = now;
                                    lead.inputMethodID = 2;//import
                                    lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                    lead.isTest = false;
                                    lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                    lead.frontOfficeResortID = (int)frontOfficeResortID;
                                    lead.tags = arrival.Tags;
                                    #endregion

                                    #region "contact info and set of contactable"
                                    if (contactInfo.Email != null)
                                    {
                                        lead.tblLeadEmails.Add(new tblLeadEmails()
                                        {
                                            email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (contactInfo.Phone != null)
                                    {
                                        lead.tblPhones.Add(new tblPhones()
                                        {
                                            phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                            phoneTypeID = 4,
                                            doNotCall = false,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (noContactable)
                                    {
                                        if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                        {
                                            lead.bookingStatusID = 15;//not contactable
                                        }
                                    }
                                    #endregion

                                    #region "reservation info"
                                    var item = new tblReservations();
                                    //item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    //item.placeID = resort.placeID;
                                    //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    //item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    //lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            isNew = true;
                            #region "lead info"
                            //lead.leadID = Guid.NewGuid();
                            //lead.initialTerminalID = (long)terminal;
                            //lead.terminalID = (long)terminal;
                            lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                            lead.firstName = arrival.nombres;
                            lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                            lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                            lead.bookingStatusID = 10;//not contacted
                            lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                            lead.inputByUserID = session.UserID;
                            lead.inputDateTime = now;
                            lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                            lead.assignationDate = now;
                            lead.inputMethodID = 2;//import
                            lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                            lead.isTest = false;
                            lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                            lead.frontOfficeResortID = (int)frontOfficeResortID;
                            lead.tags = arrival.Tags;
                            #endregion

                            #region "contact info and set of contactable"
                            if (contactInfo.Email != null)
                            {
                                lead.tblLeadEmails.Add(new tblLeadEmails()
                                {
                                    email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (contactInfo.Phone != null)
                            {
                                lead.tblPhones.Add(new tblPhones()
                                {
                                    phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                    phoneTypeID = 4,
                                    doNotCall = false,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (noContactable)
                            {
                                if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                {
                                    lead.bookingStatusID = 15;//not contactable
                                }
                            }
                            #endregion

                            #region "member info"
                            lead.tblMemberInfo.Add(new tblMemberInfo()
                            {
                                memberNumber = arrival.Contrato,
                                clubType = null,
                                revenues = 0,
                                isNational = lead.countryID == 3,
                                presentationConfirmed = false,
                                hasOptions = false,
                                isVIP = false,
                                pushedToOnSiteConcierge = false,
                                isAllInclusive = false,
                                coOwner = null,
                                contractNumber = null
                            });
                            #endregion

                            #region "reservation info"
                            var item = new tblReservations();
                            item.reservationID = Guid.NewGuid();
                            item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                            item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                            //item.placeID = resort.placeID;
                            //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                            item.planTypeID = null;
                            item.certificateNumber = arrival.CRS;
                            //item.destinationID = resort.destinationID;
                            item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                            item.totalNights = arrival.CuartosNoche;
                            item.sysWorkGroupID = session.WorkGroupID;
                            item.guestsNames = arrival.Huesped;
                            item.reservationComments = arrival.Comentario;
                            item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                            item.adults = arrival.Adultos ?? 0;
                            item.children = arrival.Ninos ?? 0;
                            item.isTest = false;
                            item.dateSaved = now;
                            item.frontOfficeReservationID = (long)arrival.idReservacion;
                            item.frontOfficePlanType = arrival.TipoPlan;
                            item.frontOfficeAgencyName = arrival.nameagencia;
                            item.frontOfficeMarketCode = arrival.Procedencia;
                            item.frontOfficeRoomListID = arrival.idroomlist;
                            item.frontOfficeContractNumber = arrival.Contrato;
                            item.frontOfficeCertificateNumber = arrival.CRS;
                            lead.tblReservations.Add(item);
                            #endregion
                        }
                    }
                    else if (arrival.CRS != null && arrival.CRS != "")
                    {
                        if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS) > 0)
                        {
                            //new
                            var arrivalDate = DateTime.Parse(arrival.Arrival).Date;
                            if (arrival.Split == true || _existing.Count(m => EntityFunctions.TruncateTime(m.departureDate.Value) == arrivalDate) > 0)//split or extension
                            {
                                //split or extension without contract
                                lead = _existing.FirstOrDefault(m => m.frontOfficeCertificateNumber == arrival.CRS).tblLeads;
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                                if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                                {
                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    //item.placeID = resort.placeID;
                                    //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    //item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                            else
                            {
                                isNew = true;
                                #region "lead info"
                                //lead.leadID = Guid.NewGuid();
                                //lead.initialTerminalID = (long)terminal;
                                //lead.terminalID = (long)terminal;
                                lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                lead.firstName = arrival.nombres;
                                lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                lead.bookingStatusID = 10;//not contacted
                                lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                lead.inputByUserID = session.UserID;
                                lead.inputDateTime = now;
                                lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                lead.assignationDate = now;
                                lead.inputMethodID = 2;//import
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                lead.isTest = false;
                                lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                lead.frontOfficeResortID = (int)frontOfficeResortID;
                                lead.tags = arrival.Tags;
                                #endregion

                                #region "contact info"
                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                        noContactable = false;
                                    }
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                        noContactable = false;
                                    }
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }
                                #endregion

                                #region "reservation info"
                                var item = new tblReservations();
                                item.reservationID = Guid.NewGuid();
                                item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                //item.placeID = resort.placeID;
                                //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                item.planTypeID = null;
                                item.certificateNumber = arrival.CRS;
                                //item.destinationID = resort.destinationID;
                                item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                item.totalNights = arrival.CuartosNoche;
                                item.sysWorkGroupID = session.WorkGroupID;
                                item.guestsNames = arrival.Huesped;
                                item.reservationComments = arrival.Comentario;
                                item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                item.adults = arrival.Adultos ?? 0;
                                item.children = arrival.Ninos ?? 0;
                                item.isTest = false;
                                item.dateSaved = now;
                                item.frontOfficeReservationID = (long)arrival.idReservacion;
                                item.frontOfficePlanType = arrival.TipoPlan;
                                item.frontOfficeAgencyName = arrival.nameagencia;
                                item.frontOfficeMarketCode = arrival.Procedencia;
                                item.frontOfficeRoomListID = arrival.idroomlist;
                                item.frontOfficeContractNumber = arrival.Contrato;
                                item.frontOfficeCertificateNumber = arrival.CRS;
                                lead.tblReservations.Add(item);
                                #endregion
                            }
                            //
                        }
                        else if (arrival.numconfirmacion != null && arrival.numconfirmacion != "" && _existing.Count(m => m.hotelConfirmationNumber == arrival.numconfirmacion) > 0)
                        {
                            lead = _existing.FirstOrDefault(m => m.hotelConfirmationNumber == arrival.numconfirmacion).tblLeads;
                            lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                            lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                            #region "contact info"
                            if (lead.tblLeadEmails.Count() == 0)
                            {
                                if (contactInfo.Email != null)
                                {
                                    var email = new tblLeadEmails();
                                    email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                    email.main = true;
                                    lead.tblLeadEmails.Add(email);
                                    noContactable = false;
                                }
                            }

                            if (lead.tblPhones.Count() == 0)
                            {
                                if (contactInfo.Phone != null)
                                {
                                    var phone = new tblPhones();
                                    phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                    phone.phoneTypeID = 4;
                                    phone.doNotCall = false;
                                    phone.main = true;
                                    lead.tblPhones.Add(phone);
                                    noContactable = false;
                                }
                            }

                            if (noContactable)
                            {
                                if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                {
                                    lead.bookingStatusID = 15;//not contactable
                                }
                            }
                            #endregion

                            if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                            {
                                #region "reservation info"
                                var item = new tblReservations();
                                item.reservationID = Guid.NewGuid();
                                item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                //item.placeID = resort.placeID;
                                //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                item.planTypeID = null;
                                item.certificateNumber = arrival.CRS;
                                //item.destinationID = resort.destinationID;
                                item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                item.totalNights = arrival.CuartosNoche;
                                item.sysWorkGroupID = session.WorkGroupID;
                                item.guestsNames = arrival.Huesped;
                                item.reservationComments = arrival.Comentario;
                                item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                item.adults = arrival.Adultos ?? 0;
                                item.children = arrival.Ninos ?? 0;
                                item.isTest = false;
                                item.dateSaved = now;
                                item.frontOfficeReservationID = (long)arrival.idReservacion;
                                item.frontOfficePlanType = arrival.TipoPlan;
                                item.frontOfficeAgencyName = arrival.nameagencia;
                                item.frontOfficeMarketCode = arrival.Procedencia;
                                item.frontOfficeRoomListID = arrival.idroomlist;
                                item.frontOfficeContractNumber = arrival.Contrato;
                                item.frontOfficeCertificateNumber = arrival.CRS;
                                lead.tblReservations.Add(item);
                                #endregion
                            }
                        }
                        else
                        {
                            if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS || m.hotelConfirmationNumber == arrival.numconfirmacion) == 0)
                            {
                                isNew = true;
                                #region "lead info"
                                //lead.leadID = Guid.NewGuid();
                                //lead.initialTerminalID = (long)terminal;
                                //lead.terminalID = (long)terminal;
                                lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                lead.firstName = arrival.nombres;
                                lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                lead.bookingStatusID = 10;//not contacted
                                lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                lead.inputByUserID = session.UserID;
                                lead.inputDateTime = now;
                                lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                lead.assignationDate = now;
                                lead.inputMethodID = 2;//import
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                lead.isTest = false;
                                lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                lead.frontOfficeResortID = (int)frontOfficeResortID;
                                lead.tags = arrival.Tags;
                                #endregion

                                #region "contact info and set of contactable"
                                if (contactInfo.Email != null)
                                {
                                    lead.tblLeadEmails.Add(new tblLeadEmails()
                                    {
                                        email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                        main = true
                                    });
                                    noContactable = false;
                                }

                                if (contactInfo.Phone != null)
                                {
                                    lead.tblPhones.Add(new tblPhones()
                                    {
                                        phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                        phoneTypeID = 4,
                                        doNotCall = false,
                                        main = true
                                    });
                                    noContactable = false;
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }
                                #endregion

                                #region "reservation info"
                                var item = new tblReservations();
                                item.reservationID = Guid.NewGuid();
                                item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                //item.placeID = resort.placeID;
                                //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                item.planTypeID = null;
                                item.certificateNumber = arrival.CRS;
                                //item.destinationID = resort.destinationID;
                                item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                item.totalNights = arrival.CuartosNoche;
                                item.sysWorkGroupID = session.WorkGroupID;
                                item.guestsNames = arrival.Huesped;
                                item.reservationComments = arrival.Comentario;
                                item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                item.adults = arrival.Adultos ?? 0;
                                item.children = arrival.Ninos ?? 0;
                                item.isTest = false;
                                item.dateSaved = now;
                                item.frontOfficeReservationID = (long)arrival.idReservacion;
                                item.frontOfficePlanType = arrival.TipoPlan;
                                item.frontOfficeAgencyName = arrival.nameagencia;
                                item.frontOfficeMarketCode = arrival.Procedencia;
                                item.frontOfficeRoomListID = arrival.idroomlist;
                                item.frontOfficeContractNumber = arrival.Contrato;
                                item.frontOfficeCertificateNumber = arrival.CRS;
                                lead.tblReservations.Add(item);
                                #endregion
                            }
                        }
                    }
                    else if (arrival.numconfirmacion != null && arrival.numconfirmacion != "")
                    {
                        if (_existing.Count(m => m.hotelConfirmationNumber == arrival.numconfirmacion) > 0)
                        {
                            //new
                            var arrivalDate = DateTime.Parse(arrival.Arrival).Date;
                            if (arrival.Split == true || _existing.Count(m => EntityFunctions.TruncateTime(m.departureDate.Value) == arrivalDate) > 0)
                            {
                                lead = _existing.FirstOrDefault(m => m.frontOfficeCertificateNumber == arrival.CRS).tblLeads;
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                        noContactable = false;
                                    }
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                        noContactable = false;
                                    }
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }

                                if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                                {
                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    //item.placeID = resort.placeID;
                                    //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    //item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                            else
                            {
                                if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS || m.hotelConfirmationNumber == arrival.numconfirmacion) == 0)
                                {
                                    isNew = true;
                                    #region "lead info"
                                    //lead.leadID = Guid.NewGuid();
                                    //lead.initialTerminalID = (long)terminal;
                                    //lead.terminalID = (long)terminal;
                                    lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                    lead.firstName = arrival.nombres;
                                    lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                    lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                    lead.bookingStatusID = 10;//not contacted
                                    lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                    lead.inputByUserID = session.UserID;
                                    lead.inputDateTime = now;
                                    lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                    lead.assignationDate = now;
                                    lead.inputMethodID = 2;//import
                                    lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                    lead.isTest = false;
                                    lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                    lead.frontOfficeResortID = (int)frontOfficeResortID;
                                    lead.tags = arrival.Tags;
                                    #endregion

                                    #region "contact info and set of contactable"
                                    if (contactInfo.Email != null)
                                    {
                                        lead.tblLeadEmails.Add(new tblLeadEmails()
                                        {
                                            email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (contactInfo.Phone != null)
                                    {
                                        lead.tblPhones.Add(new tblPhones()
                                        {
                                            phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                            phoneTypeID = 4,
                                            doNotCall = false,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (noContactable)
                                    {
                                        if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                        {
                                            lead.bookingStatusID = 15;//not contactable
                                        }
                                    }
                                    #endregion

                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    //item.placeID = resort.placeID;
                                    //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    //item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            isNew = true;
                            #region "lead info"
                            //lead.leadID = Guid.NewGuid();
                            //lead.initialTerminalID = (long)terminal;
                            //lead.terminalID = (long)terminal;
                            lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                            lead.firstName = arrival.nombres;
                            lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                            lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                            lead.bookingStatusID = 10;//not contacted
                            lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                            lead.inputByUserID = session.UserID;
                            lead.inputDateTime = now;
                            lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                            lead.assignationDate = now;
                            lead.inputMethodID = 2;//import
                            lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion.Trim() == "CA" ? 4 : 1 : 1;
                            lead.isTest = false;
                            lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                            lead.frontOfficeResortID = (int)frontOfficeResortID;
                            lead.tags = arrival.Tags;
                            #endregion

                            #region "contact info and set of contactable"
                            if (contactInfo.Email != null)
                            {
                                lead.tblLeadEmails.Add(new tblLeadEmails()
                                {
                                    email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (contactInfo.Phone != null)
                            {
                                lead.tblPhones.Add(new tblPhones()
                                {
                                    phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                    phoneTypeID = 4,
                                    doNotCall = false,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (noContactable)
                            {
                                if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                {
                                    lead.bookingStatusID = 15;//not contactable
                                }
                            }
                            #endregion

                            #region "reservation info"
                            var item = new tblReservations();
                            item.reservationID = Guid.NewGuid();
                            item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                            item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                            //item.placeID = resort.placeID;
                            //item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                            item.planTypeID = null;
                            item.certificateNumber = arrival.CRS;
                            //item.destinationID = resort.destinationID;
                            item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                            item.totalNights = arrival.CuartosNoche;
                            item.sysWorkGroupID = session.WorkGroupID;
                            item.guestsNames = arrival.Huesped;
                            item.reservationComments = arrival.Comentario;
                            item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                            item.adults = arrival.Adultos ?? 0;
                            item.children = arrival.Ninos ?? 0;
                            item.isTest = false;
                            item.dateSaved = now;
                            item.frontOfficeReservationID = (long)arrival.idReservacion;
                            item.frontOfficePlanType = arrival.TipoPlan;
                            item.frontOfficeAgencyName = arrival.nameagencia;
                            item.frontOfficeMarketCode = arrival.Procedencia;
                            item.frontOfficeRoomListID = arrival.idroomlist;
                            item.frontOfficeContractNumber = arrival.Contrato;
                            item.frontOfficeCertificateNumber = arrival.CRS;
                            lead.tblReservations.Add(item);
                            #endregion
                        }
                    }

                    //end new


                    if (isNew)
                    {
                        db.tblLeads.AddObject(lead);
                    }
                    //db.SaveChanges();
                    arrival.Status = true;
                    arrival.Message = "Imported";
                    arrival.LeadID = lead.leadID;

                    #region "market codes relationships update"
                    var marketCode = db.tblMarketCodes.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID && m.marketCode == arrival.Procedencia);
                    if (marketCode != null)
                    {
                        marketCode.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                    }
                    else
                    {
                        var mk = new tblMarketCodes();
                        mk.marketCode = arrival.Procedencia;
                        mk.frontOfficeResortID = frontOfficeResortID;
                        mk.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                        db.tblMarketCodes.AddObject(mk);
                    }
                    #endregion

                    ///
                    ///No se puede actualizar una relación existente desde aquí.
                    ///Esta porción solo captura relaciones nuevas
                    ///
                    #region "lead source relationship update"
                    //if (lead.leadSourceID != null && arrival.AssignedToUserID != null)
                    //{
                    //    if (leadSources.Count(m => m.leadSourceID == lead.leadSourceID && m.userID == arrival.AssignedToUserID) == 0)
                    //    {
                    //        var ls = new tblUsers_LeadSources();
                    //        ls.leadSourceID = long.Parse(arrival.LeadSource);
                    //        ls.userID = (Guid)arrival.AssignedToUserID;
                    //        ls.terminalID = terminals.FirstOrDefault();
                    //        ls.frontOfficeResortID = frontOfficeResortID;
                    //        ls.fromDate = now;
                    //        ls.savedByUserID = session.UserID;
                    //        db.tblUsers_LeadSources.AddObject(ls);
                    //    }
                    //}
                    #endregion
                    //db.SaveChanges();
                }
                catch (Exception ex)
                {
                    arrival.Status = false;
                    arrival.Message = ex.Message + (ex.InnerException != null ? "<br />" + ex.InnerException.Message : "");
                    response.Exception = ex;
                }
            }

            response.Type = arrivals.Count(m => m.Import == true && m.Status != true) > 0 ? Attempt_ResponseTypes.Warning : Attempt_ResponseTypes.Ok;
            response.Message = response.Exception != null ? "There were some errors with Import" : "Import Successful";
            response.ObjectID = new { arrivals = serializer.Serialize(arrivals) };

            //return response;
            return arrivals;
        }

        public AttemptResponse Import(string __data)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            db.CommandTimeout = int.MaxValue;
            serializer.MaxJsonLength = int.MaxValue;
            DateTime now = DateTime.Now;
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var frontResorts = PreArrivalCatalogs.GetFrontResorts().Select(m => int.Parse(m.Value)).ToArray();
            var resorts = db.tblPlaces.Where(m => frontResorts.Contains((int)m.frontOfficeResortID));
            var roomTypes = db.tblRoomTypes.Where(m => resorts.Select(x => x.placeID).Contains(m.placeID)).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode }).GroupBy(m => m.placeID);

            #region "filtracion de llegadas por importar"
            var arrivals = serializer.Deserialize<List<ImportModel>>(__data);
            arrivals = arrivals.Count() == 0 ? new List<ImportModel>() { serializer.Deserialize<ImportModel>(__data) } : arrivals;
            arrivals = arrivals.Where(m => m.Import == true && m.Status != true).ToList<ImportModel>();

            var _resort = (int)arrivals.FirstOrDefault().idresort;
            #endregion

            var leadSources = db.tblUsers_LeadSources.Where(m => terminals.FirstOrDefault() == m.terminalID && m.frontOfficeResortID == _resort && (m.toDate == null || m.toDate > now)).ToList();

            foreach (var arrival in arrivals.OrderBy(m => m.Arrival))
            {
                try
                {
                    int frontOfficeResortID = (int)arrival.idresort;
                    var frontOfficeReservationID = (long?)arrival.idReservacion;
                    var frontOfficeRoomListID = arrival.idroomlist != null ? arrival.idroomlist : (int?)null;
                    var resort = resorts.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID);

                    var frontOfficeGuestID = arrival.idhuesped;
                    var guestHub = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID && m.frontOfficeGuestID == frontOfficeGuestID);
                    var noContactable = true;
                    var isNew = false;
                    var contactInfo = GetContactInfo(arrival);


                    //new
                    var _existing = from r in db.tblReservations
                                    where terminals.Contains(r.tblLeads.terminalID)
                                    && r.tblLeads.frontOfficeResortID == frontOfficeResortID
                                    && r.frontOfficeReservationID == frontOfficeReservationID
                                    //&& (frontOfficeRoomListID == null || r.frontOfficeRoomListID == frontOfficeRoomListID)
                                    select r;

                    tblLeads lead = new tblLeads();

                    if (arrival.Contrato != null && arrival.Contrato != "" && arrival.Contrato.IndexOf("ending") == -1 && arrival.Contrato.IndexOf("xchange") == -1)
                    {
                        if (_existing.Count(x => x.frontOfficeContractNumber == arrival.Contrato) > 0)
                        {
                            var arrivalDate = DateTime.Parse(arrival.Arrival).Date;
                            if (arrival.Split == true || _existing.Count(m => EntityFunctions.TruncateTime(m.departureDate.Value) == arrivalDate) > 0)//split or extension
                            {
                                //same contract, split or extension.
                                lead = _existing.FirstOrDefault(m => m.frontOfficeContractNumber == arrival.Contrato).tblLeads;
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                        noContactable = false;
                                    }
                                }
                                else
                                {
                                    noContactable = false;
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                        noContactable = false;
                                    }
                                }
                                else
                                {
                                    noContactable = false;
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }

                                if (lead.tblMemberInfo != null && lead.tblMemberInfo.Count() > 0)
                                {
                                    var member = lead.tblMemberInfo.FirstOrDefault();
                                    member.memberNumber = arrival.Contrato;
                                    member.clubType = null;
                                    member.revenues = 0;
                                    member.isNational = lead.countryID == 3;
                                    member.presentationConfirmed = false;
                                    member.hasOptions = false;
                                    member.isVIP = false;
                                    member.pushedToOnSiteConcierge = false;
                                    member.isAllInclusive = false;
                                    member.coOwner = null;
                                    member.memberName = null;
                                    member.contractNumber = null;
                                }

                                if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                                {
                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    item.placeID = resort.placeID;
                                    item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                            else
                            {
                                //same contract but not split, not extension. => guest of member
                                //condition defined to prevent saving of same information from distinct tab in browser
                                if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS || m.hotelConfirmationNumber == arrival.numconfirmacion) == 0)
                                {
                                    isNew = true;
                                    #region "lead info"
                                    lead.leadID = Guid.NewGuid();
                                    lead.initialTerminalID = terminals.FirstOrDefault();
                                    lead.terminalID = terminals.FirstOrDefault();
                                    lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                    lead.firstName = arrival.nombres;
                                    lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                    lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                    lead.bookingStatusID = 10;//not contacted
                                    lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                    lead.inputByUserID = session.UserID;
                                    lead.inputDateTime = now;
                                    lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                    lead.assignationDate = now;
                                    lead.inputMethodID = 2;//import
                                    lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                    lead.isTest = false;
                                    lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                    lead.frontOfficeResortID = (int)frontOfficeResortID;
                                    lead.tags = arrival.Tags;
                                    #endregion

                                    #region "contact info and set of contactable"
                                    if (contactInfo.Email != null)
                                    {
                                        lead.tblLeadEmails.Add(new tblLeadEmails()
                                        {
                                            email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (contactInfo.Phone != null)
                                    {
                                        lead.tblPhones.Add(new tblPhones()
                                        {
                                            phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                            phoneTypeID = 4,
                                            doNotCall = false,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (noContactable)
                                    {
                                        if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                        {
                                            lead.bookingStatusID = 15;//not contactable
                                        }
                                    }
                                    #endregion

                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    item.placeID = resort.placeID;
                                    item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            isNew = true;
                            #region "lead info"
                            lead.leadID = Guid.NewGuid();
                            lead.initialTerminalID = terminals.FirstOrDefault();
                            lead.terminalID = terminals.FirstOrDefault();
                            lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                            lead.firstName = arrival.nombres;
                            lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                            lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                            lead.bookingStatusID = 10;//not contacted
                            lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                            lead.inputByUserID = session.UserID;
                            lead.inputDateTime = now;
                            lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                            lead.assignationDate = now;
                            lead.inputMethodID = 2;//import
                            lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                            lead.isTest = false;
                            lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                            lead.frontOfficeResortID = (int)frontOfficeResortID;
                            lead.tags = arrival.Tags;
                            #endregion

                            #region "contact info and set of contactable"
                            if (contactInfo.Email != null)
                            {
                                lead.tblLeadEmails.Add(new tblLeadEmails()
                                {
                                    email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (contactInfo.Phone != null)
                            {
                                lead.tblPhones.Add(new tblPhones()
                                {
                                    phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                    phoneTypeID = 4,
                                    doNotCall = false,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (noContactable)
                            {
                                if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                {
                                    lead.bookingStatusID = 15;//not contactable
                                }
                            }
                            #endregion

                            #region "member info"
                            lead.tblMemberInfo.Add(new tblMemberInfo()
                            {
                                memberNumber = arrival.Contrato,
                                clubType = null,
                                revenues = 0,
                                isNational = lead.countryID == 3,
                                presentationConfirmed = false,
                                hasOptions = false,
                                isVIP = false,
                                pushedToOnSiteConcierge = false,
                                isAllInclusive = false,
                                coOwner = null,
                                contractNumber = null
                            });
                            #endregion

                            #region "reservation info"
                            var item = new tblReservations();
                            item.reservationID = Guid.NewGuid();
                            item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                            item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                            item.placeID = resort.placeID;
                            item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                            item.planTypeID = null;
                            item.certificateNumber = arrival.CRS;
                            item.destinationID = resort.destinationID;
                            item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                            item.totalNights = arrival.CuartosNoche;
                            item.sysWorkGroupID = session.WorkGroupID;
                            item.guestsNames = arrival.Huesped;
                            item.reservationComments = arrival.Comentario;
                            item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                            item.adults = arrival.Adultos ?? 0;
                            item.children = arrival.Ninos ?? 0;
                            item.isTest = false;
                            item.dateSaved = now;
                            item.frontOfficeReservationID = (long)arrival.idReservacion;
                            item.frontOfficePlanType = arrival.TipoPlan;
                            item.frontOfficeAgencyName = arrival.nameagencia;
                            item.frontOfficeMarketCode = arrival.Procedencia;
                            item.frontOfficeRoomListID = arrival.idroomlist;
                            item.frontOfficeContractNumber = arrival.Contrato;
                            item.frontOfficeCertificateNumber = arrival.CRS;
                            lead.tblReservations.Add(item);
                            #endregion
                        }
                    }
                    else if (arrival.CRS != null && arrival.CRS != "")
                    {
                        if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS) > 0)
                        {
                            //new
                            //lead = _existing.FirstOrDefault(m => m.frontOfficeCertificateNumber == arrival.CRS).tblLeads;
                            //lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                            //lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;
                            var arrivalDate = DateTime.Parse(arrival.Arrival).Date;
                            if (arrival.Split == true || _existing.Count(m => EntityFunctions.TruncateTime(m.departureDate.Value) == arrivalDate) > 0)//split or extension
                            {
                                //split or extension without contract
                                lead = _existing.FirstOrDefault(m => m.frontOfficeCertificateNumber == arrival.CRS).tblLeads;
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                        noContactable = false;
                                    }
                                }
                                else
                                {
                                    noContactable = false;
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                        noContactable = false;
                                    }
                                }
                                else
                                {
                                    noContactable = false;
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }

                                if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                                {
                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    item.placeID = resort.placeID;
                                    item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                            else
                            {
                                isNew = true;
                                #region "lead info"
                                lead.leadID = Guid.NewGuid();
                                lead.initialTerminalID = terminals.FirstOrDefault();
                                lead.terminalID = terminals.FirstOrDefault();
                                lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                lead.firstName = arrival.nombres;
                                lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                lead.bookingStatusID = 10;//not contacted
                                lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                lead.inputByUserID = session.UserID;
                                lead.inputDateTime = now;
                                lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                lead.assignationDate = now;
                                lead.inputMethodID = 2;//import
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                lead.isTest = false;
                                lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                lead.frontOfficeResortID = (int)frontOfficeResortID;
                                lead.tags = arrival.Tags;
                                #endregion

                                #region "contact info"
                                if (contactInfo.Email != null)
                                {
                                    lead.tblLeadEmails.Add(new tblLeadEmails()
                                    {
                                        email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                        main = true
                                    });
                                    noContactable = false;
                                }

                                if (contactInfo.Phone != null)
                                {
                                    lead.tblPhones.Add(new tblPhones()
                                    {
                                        phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                        phoneTypeID = 4,
                                        doNotCall = false,
                                        main = true
                                    });
                                    noContactable = false;
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }
                                #endregion

                                #region "reservation info"
                                var item = new tblReservations();
                                item.reservationID = Guid.NewGuid();
                                item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                item.placeID = resort.placeID;
                                item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                item.planTypeID = null;
                                item.certificateNumber = arrival.CRS;
                                item.destinationID = resort.destinationID;
                                item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                item.totalNights = arrival.CuartosNoche;
                                item.sysWorkGroupID = session.WorkGroupID;
                                item.guestsNames = arrival.Huesped;
                                item.reservationComments = arrival.Comentario;
                                item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                item.adults = arrival.Adultos ?? 0;
                                item.children = arrival.Ninos ?? 0;
                                item.isTest = false;
                                item.dateSaved = now;
                                item.frontOfficeReservationID = (long)arrival.idReservacion;
                                item.frontOfficePlanType = arrival.TipoPlan;
                                item.frontOfficeAgencyName = arrival.nameagencia;
                                item.frontOfficeMarketCode = arrival.Procedencia;
                                item.frontOfficeRoomListID = arrival.idroomlist;
                                item.frontOfficeContractNumber = arrival.Contrato;
                                item.frontOfficeCertificateNumber = arrival.CRS;
                                lead.tblReservations.Add(item);
                                #endregion
                            }
                        }
                        else if (arrival.numconfirmacion != null && arrival.numconfirmacion != "" && _existing.Count(m => m.hotelConfirmationNumber == arrival.numconfirmacion) > 0)
                        {
                            lead = _existing.FirstOrDefault(m => m.hotelConfirmationNumber == arrival.numconfirmacion).tblLeads;
                            lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                            lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                            #region "contact info"
                            if (lead.tblLeadEmails.Count() == 0)
                            {
                                if (contactInfo.Email != null)
                                {
                                    var email = new tblLeadEmails();
                                    email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                    email.main = true;
                                    lead.tblLeadEmails.Add(email);
                                    noContactable = false;
                                }
                            }
                            else
                            {
                                noContactable = false;
                            }

                            if (lead.tblPhones.Count() == 0)
                            {
                                if (contactInfo.Phone != null)
                                {
                                    var phone = new tblPhones();
                                    phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                    phone.phoneTypeID = 4;
                                    phone.doNotCall = false;
                                    phone.main = true;
                                    lead.tblPhones.Add(phone);
                                    noContactable = false;
                                }
                            }
                            else
                            {
                                noContactable = false;
                            }

                            if (noContactable)
                            {
                                if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                {
                                    lead.bookingStatusID = 15;//not contactable
                                }
                            }
                            #endregion

                            if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                            {
                                #region "reservation info"
                                var item = new tblReservations();
                                item.reservationID = Guid.NewGuid();
                                item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                item.placeID = resort.placeID;
                                item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                item.planTypeID = null;
                                item.certificateNumber = arrival.CRS;
                                item.destinationID = resort.destinationID;
                                item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                item.totalNights = arrival.CuartosNoche;
                                item.sysWorkGroupID = session.WorkGroupID;
                                item.guestsNames = arrival.Huesped;
                                item.reservationComments = arrival.Comentario;
                                item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                item.adults = arrival.Adultos ?? 0;
                                item.children = arrival.Ninos ?? 0;
                                item.isTest = false;
                                item.dateSaved = now;
                                item.frontOfficeReservationID = (long)arrival.idReservacion;
                                item.frontOfficePlanType = arrival.TipoPlan;
                                item.frontOfficeAgencyName = arrival.nameagencia;
                                item.frontOfficeMarketCode = arrival.Procedencia;
                                item.frontOfficeRoomListID = arrival.idroomlist;
                                item.frontOfficeContractNumber = arrival.Contrato;
                                item.frontOfficeCertificateNumber = arrival.CRS;
                                lead.tblReservations.Add(item);
                                #endregion
                            }
                        }
                        else
                        {
                            if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS || m.hotelConfirmationNumber == arrival.numconfirmacion) == 0)
                            {
                                isNew = true;
                                #region "lead info"
                                lead.leadID = Guid.NewGuid();
                                lead.initialTerminalID = terminals.FirstOrDefault();
                                lead.terminalID = terminals.FirstOrDefault();
                                lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                lead.firstName = arrival.nombres;
                                lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                lead.bookingStatusID = 10;//not contacted
                                lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                lead.inputByUserID = session.UserID;
                                lead.inputDateTime = now;
                                lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                lead.assignationDate = now;
                                lead.inputMethodID = 2;//import
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                lead.isTest = false;
                                lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                lead.frontOfficeResortID = (int)frontOfficeResortID;
                                lead.tags = arrival.Tags;
                                #endregion

                                #region "contact info and set of contactable"
                                if (contactInfo.Email != null)
                                {
                                    lead.tblLeadEmails.Add(new tblLeadEmails()
                                    {
                                        email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                        main = true
                                    });
                                    noContactable = false;
                                }

                                if (contactInfo.Phone != null)
                                {
                                    lead.tblPhones.Add(new tblPhones()
                                    {
                                        phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                        phoneTypeID = 4,
                                        doNotCall = false,
                                        main = true
                                    });
                                    noContactable = false;
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }
                                #endregion

                                #region "reservation info"
                                var item = new tblReservations();
                                item.reservationID = Guid.NewGuid();
                                item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                item.placeID = resort.placeID;
                                item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                item.planTypeID = null;
                                item.certificateNumber = arrival.CRS;
                                item.destinationID = resort.destinationID;
                                item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                item.totalNights = arrival.CuartosNoche;
                                item.sysWorkGroupID = session.WorkGroupID;
                                item.guestsNames = arrival.Huesped;
                                item.reservationComments = arrival.Comentario;
                                item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                item.adults = arrival.Adultos ?? 0;
                                item.children = arrival.Ninos ?? 0;
                                item.isTest = false;
                                item.dateSaved = now;
                                item.frontOfficeReservationID = (long)arrival.idReservacion;
                                item.frontOfficePlanType = arrival.TipoPlan;
                                item.frontOfficeAgencyName = arrival.nameagencia;
                                item.frontOfficeMarketCode = arrival.Procedencia;
                                item.frontOfficeRoomListID = arrival.idroomlist;
                                item.frontOfficeContractNumber = arrival.Contrato;
                                item.frontOfficeCertificateNumber = arrival.CRS;
                                lead.tblReservations.Add(item);
                                #endregion
                            }
                        }
                    }
                    else if (arrival.numconfirmacion != null && arrival.numconfirmacion != "")
                    {
                        if (_existing.Count(m => m.hotelConfirmationNumber == arrival.numconfirmacion) > 0)
                        {
                            //new
                            var arrivalDate = DateTime.Parse(arrival.Arrival).Date;
                            if (arrival.Split == true || _existing.Count(m => EntityFunctions.TruncateTime(m.departureDate.Value) == arrivalDate) > 0)
                            {
                                lead = _existing.FirstOrDefault(m => m.frontOfficeCertificateNumber == arrival.CRS).tblLeads;
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? lead.leadStatusID == 4 ? lead.leadStatusID : lead.leadStatusID : lead.leadStatusID == 4 ? 2 : lead.leadStatusID : 1;

                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                        noContactable = false;
                                    }
                                }
                                else
                                {
                                    noContactable = false;
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                        noContactable = false;
                                    }
                                }
                                else
                                {
                                    noContactable = false;
                                }

                                if (noContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }

                                if (_existing.Count(m => m.frontOfficeRoomListID == frontOfficeRoomListID) == 0)
                                {
                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    item.placeID = resort.placeID;
                                    item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                            else
                            {
                                if (_existing.Count(m => m.frontOfficeCertificateNumber == arrival.CRS || m.hotelConfirmationNumber == arrival.numconfirmacion) == 0)
                                {
                                    isNew = true;
                                    #region "lead info"
                                    lead.leadID = Guid.NewGuid();
                                    lead.initialTerminalID = terminals.FirstOrDefault();
                                    lead.terminalID = terminals.FirstOrDefault();
                                    lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                                    lead.firstName = arrival.nombres;
                                    lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                                    lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                                    lead.bookingStatusID = 10;//not contacted
                                    lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                                    lead.inputByUserID = session.UserID;
                                    lead.inputDateTime = now;
                                    lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                                    lead.assignationDate = now;
                                    lead.inputMethodID = 2;//import
                                    lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion == "CA" ? 4 : 1 : 1;
                                    lead.isTest = false;
                                    lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                                    lead.frontOfficeResortID = (int)frontOfficeResortID;
                                    lead.tags = arrival.Tags;
                                    #endregion

                                    #region "contact info and set of contactable"
                                    if (contactInfo.Email != null)
                                    {
                                        lead.tblLeadEmails.Add(new tblLeadEmails()
                                        {
                                            email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (contactInfo.Phone != null)
                                    {
                                        lead.tblPhones.Add(new tblPhones()
                                        {
                                            phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                            phoneTypeID = 4,
                                            doNotCall = false,
                                            main = true
                                        });
                                        noContactable = false;
                                    }

                                    if (noContactable)
                                    {
                                        if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                        {
                                            lead.bookingStatusID = 15;//not contactable
                                        }
                                    }
                                    #endregion

                                    #region "reservation info"
                                    var item = new tblReservations();
                                    item.reservationID = Guid.NewGuid();
                                    item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                                    item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                                    item.placeID = resort.placeID;
                                    item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                                    item.planTypeID = null;
                                    item.certificateNumber = arrival.CRS;
                                    item.destinationID = resort.destinationID;
                                    item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                                    item.totalNights = arrival.CuartosNoche;
                                    item.sysWorkGroupID = session.WorkGroupID;
                                    item.guestsNames = arrival.Huesped;
                                    item.reservationComments = arrival.Comentario;
                                    item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    item.adults = arrival.Adultos ?? 0;
                                    item.children = arrival.Ninos ?? 0;
                                    item.isTest = false;
                                    item.dateSaved = now;
                                    item.frontOfficeReservationID = (long)arrival.idReservacion;
                                    item.frontOfficePlanType = arrival.TipoPlan;
                                    item.frontOfficeAgencyName = arrival.nameagencia;
                                    item.frontOfficeMarketCode = arrival.Procedencia;
                                    item.frontOfficeRoomListID = arrival.idroomlist;
                                    item.frontOfficeContractNumber = arrival.Contrato;
                                    item.frontOfficeCertificateNumber = arrival.CRS;
                                    lead.tblReservations.Add(item);
                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            isNew = true;
                            #region "lead info"
                            lead.leadID = Guid.NewGuid();
                            lead.initialTerminalID = terminals.FirstOrDefault();
                            lead.terminalID = terminals.FirstOrDefault();
                            lead.personalTitleID = arrival.Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.Titulo).Value) : (int?)null : (int?)null;
                            lead.firstName = arrival.nombres;
                            lead.lastName = (arrival.apellidopaterno != null ? arrival.apellidopaterno + " " : "") + (arrival.apellidomaterno != null ? arrival.apellidomaterno : "");
                            lead.countryID = arrival.codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.codepais).Value) : (int?)null : (int?)null;
                            lead.bookingStatusID = 10;//not contacted
                            lead.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                            lead.inputByUserID = session.UserID;
                            lead.inputDateTime = now;
                            lead.assignedToUserID = arrival.AssignedToUserID ?? session.UserID;
                            lead.assignationDate = now;
                            lead.inputMethodID = 2;//import
                            lead.leadStatusID = arrival.codigostatusreservacion != null ? arrival.codigostatusreservacion.Trim() == "CA" ? 4 : 1 : 1;
                            lead.isTest = false;
                            lead.frontOfficeGuestID = (int)frontOfficeGuestID;
                            lead.frontOfficeResortID = (int)frontOfficeResortID;
                            lead.tags = arrival.Tags;
                            #endregion

                            #region "contact info and set of contactable"
                            if (contactInfo.Email != null)
                            {
                                lead.tblLeadEmails.Add(new tblLeadEmails()
                                {
                                    email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (contactInfo.Phone != null)
                            {
                                lead.tblPhones.Add(new tblPhones()
                                {
                                    phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone,
                                    phoneTypeID = 4,
                                    doNotCall = false,
                                    main = true
                                });
                                noContactable = false;
                            }

                            if (noContactable)
                            {
                                if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                {
                                    lead.bookingStatusID = 15;//not contactable
                                }
                            }
                            #endregion

                            #region "reservation info"
                            var item = new tblReservations();
                            item.reservationID = Guid.NewGuid();
                            item.arrivalDate = arrival.Arrival != null && arrival.Arrival != "" ? DateTime.Parse(arrival.Arrival) : (DateTime?)null;
                            item.departureDate = arrival.Departure != null && arrival.Departure != "" ? DateTime.Parse(arrival.Departure) : (DateTime?)null;
                            item.placeID = resort.placeID;
                            item.roomTypeID = roomTypes.Count(m => m.Key == resort.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).Count(m => m.roomTypeCode == arrival.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == resort.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.TipoHab).roomTypeID : (long?)null : (long?)null;
                            item.planTypeID = null;
                            item.certificateNumber = arrival.CRS;
                            item.destinationID = resort.destinationID;
                            item.hotelConfirmationNumber = arrival.numconfirmacion.Trim();
                            item.totalNights = arrival.CuartosNoche;
                            item.sysWorkGroupID = session.WorkGroupID;
                            item.guestsNames = arrival.Huesped;
                            item.reservationComments = arrival.Comentario;
                            item.reservationStatusID = arrival.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                            item.adults = arrival.Adultos ?? 0;
                            item.children = arrival.Ninos ?? 0;
                            item.isTest = false;
                            item.dateSaved = now;
                            item.frontOfficeReservationID = (long)arrival.idReservacion;
                            item.frontOfficePlanType = arrival.TipoPlan;
                            item.frontOfficeAgencyName = arrival.nameagencia;
                            item.frontOfficeMarketCode = arrival.Procedencia;
                            item.frontOfficeRoomListID = arrival.idroomlist;
                            item.frontOfficeContractNumber = arrival.Contrato;
                            item.frontOfficeCertificateNumber = arrival.CRS;
                            lead.tblReservations.Add(item);
                            #endregion
                        }
                    }
                    else
                    {
                        throw new Exception("Not enough data");
                    }

                    //end new


                    if (isNew)
                    {
                        db.tblLeads.AddObject(lead);
                    }
                    db.SaveChanges();
                    arrival.Status = true;
                    arrival.Message = "Imported";
                    arrival.LeadID = lead.leadID;

                    #region "market codes relationships update"
                    var marketCode = db.tblMarketCodes.FirstOrDefault(m => m.frontOfficeResortID == frontOfficeResortID && m.marketCode == arrival.Procedencia);
                    if (marketCode != null)
                    {
                        marketCode.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                    }
                    else
                    {
                        var mk = new tblMarketCodes();
                        mk.marketCode = arrival.Procedencia;
                        mk.frontOfficeResortID = frontOfficeResortID;
                        mk.leadSourceID = arrival.LeadSource != null && arrival.LeadSource != "" ? long.Parse(arrival.LeadSource) : (long?)null;
                        db.tblMarketCodes.AddObject(mk);
                    }
                    #endregion

                    ///
                    ///No se puede actualizar una relación existente desde aquí.
                    ///Esta porción solo captura relaciones nuevas
                    ///
                    #region "lead source relationship update"
                    if (lead.leadSourceID != null && arrival.AssignedToUserID != null)
                    {
                        if (leadSources.Count(m => m.leadSourceID == lead.leadSourceID && m.userID == arrival.AssignedToUserID) == 0)
                        {
                            var ls = new tblUsers_LeadSources();
                            ls.leadSourceID = long.Parse(arrival.LeadSource);
                            ls.userID = (Guid)arrival.AssignedToUserID;
                            ls.terminalID = terminals.FirstOrDefault();
                            ls.frontOfficeResortID = frontOfficeResortID;
                            ls.fromDate = now;
                            ls.savedByUserID = session.UserID;
                            db.tblUsers_LeadSources.AddObject(ls);
                        }
                    }
                    #endregion
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    arrival.Status = false;
                    arrival.Message = ex.Message + (ex.InnerException != null ? "<br />" + ex.InnerException.Message : "");
                    response.Exception = ex;
                }
            }

            response.Type = arrivals.Count(m => m.Import == true && m.Status != true) > 0 ? Attempt_ResponseTypes.Warning : Attempt_ResponseTypes.Ok;
            response.Message = response.Exception != null ? "There were some errors with Import" : "Import Successful";
            response.ObjectID = new { arrivals = serializer.Serialize(arrivals) };

            return response;
        }

        /// <summary>
        /// unused. It was used to import leads from file.
        /// call is in PublicDataModel
        /// </summary>
        /// <param name="__data"></param>
        /// <returns></returns>
        public AttemptResponse ImportArrivals(string __data)
        {
            ePlatEntities db = new ePlatEntities();
            db.CommandTimeout = int.MaxValue;
            AttemptResponse response = new AttemptResponse();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var counterNewImported = 0;
            var counterExistingImported = 0;
            var sources = db.tblLeadSources;
            var now = DateTime.Now;
            var resorts = PreArrivalCatalogs.GetFrontResorts();
            var terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var places = db.tblPlaces.Where(m => m.frontOfficeResortID != null);
            var roomTypes = db.tblRoomTypes.Where(m => places.Select(x => x.placeID).Contains(m.placeID)).Select(m => new { m.placeID, m.roomTypeID, m.roomTypeCode }).GroupBy(m => m.placeID);

            var arrivals = serializer.Deserialize<List<ImportModel>>(__data);
            arrivals = arrivals.Count() == 0 ? new List<ImportModel>() { serializer.Deserialize<ImportModel>(__data) } : arrivals;
            arrivals = arrivals.Where(m => m.Import == true).ToList<ImportModel>();//line to define what arrivals to import
            var groupPerResort = arrivals.GroupBy(m => m.idresort);
            var arrivalsPerConfirmation = arrivals.GroupBy(m => m.numconfirmacion);
            var contracts = new List<string> { };
            //posible sea necesario realizar una agrupación por contrato antes de por confirmación.
            foreach (var arrival in arrivalsPerConfirmation)
            {
                if (arrival.Count(m => m.Status != true) > 0)
                {
                    try
                    {
                        if (arrival.FirstOrDefault().Contrato == null || arrival.FirstOrDefault().Contrato.Trim() == "" || !contracts.Contains(arrival.FirstOrDefault().Contrato))
                        {
                            var idhuesped = arrival.FirstOrDefault().idhuesped.ToString();
                            var resort = arrival.FirstOrDefault().idresort;
                            var huesped = arrival.FirstOrDefault().idhuesped;
                            var procedencia = arrival.FirstOrDefault().Procedencia;

                            var existingLead = from l in db.tblLeads
                                               where terminals.Contains(l.terminalID)
                                               && l.frontOfficeResortID == resort
                                               && l.frontOfficeGuestID == huesped
                                               select l;

                            var lead = existingLead.Count() > 0 ? existingLead.FirstOrDefault() : new tblLeads();
                            var contactInfo = GetContactInfo(arrival.FirstOrDefault());

                            var isNonContactable = true;
                            var place = places.FirstOrDefault(m => m.frontOfficeResortID == (long)resort);
                            var guestHub = db.tblGuestHub_FrontOffice.FirstOrDefault(m => m.frontOfficeResortID == (int)resort && m.frontOfficeGuestID == huesped);
                            var marketCode = db.tblMarketCodes.Where(m => m.frontOfficeResortID == resort && m.marketCode == procedencia);
                            var leadSources = db.tblUsers_LeadSources.Where(m => terminals.FirstOrDefault() == m.terminalID && m.frontOfficeResortID == resort && (m.toDate == null || m.toDate > now)).ToList();

                            #region "lead assignation based on existence"
                            if (existingLead.Count() == 0)
                            {
                                #region "new lead"
                                counterNewImported++;
                                lead.leadID = Guid.NewGuid();
                                lead.initialTerminalID = terminals.FirstOrDefault();
                                lead.terminalID = terminals.FirstOrDefault();
                                lead.personalTitleID = arrival.FirstOrDefault().Titulo != null ? Dictionaries.FrontPersonalTitles.Count(m => m.Key == arrival.FirstOrDefault().Titulo) > 0 ? int.Parse(Dictionaries.FrontPersonalTitles.FirstOrDefault(m => m.Key == arrival.FirstOrDefault().Titulo).Value) : (int?)null : (int?)null;
                                lead.firstName = arrival.FirstOrDefault().nombres;
                                lead.lastName = (arrival.FirstOrDefault().apellidopaterno != null ? arrival.FirstOrDefault().apellidopaterno + " " : "") + (arrival.FirstOrDefault().apellidomaterno != null ? arrival.FirstOrDefault().apellidomaterno : "");
                                lead.countryID = arrival.FirstOrDefault().codepais != null ? Dictionaries.FrontCountries.Count(m => m.Key == arrival.FirstOrDefault().codepais) > 0 ? int.Parse(Dictionaries.FrontCountries.FirstOrDefault(m => m.Key == arrival.FirstOrDefault().codepais).Value) : (int?)null : (int?)null;
                                lead.bookingStatusID = 10;//not contacted
                                lead.leadSourceID = arrival.FirstOrDefault().LeadSource != null && arrival.FirstOrDefault().LeadSource != "" ? long.Parse(arrival.FirstOrDefault().LeadSource) : (long?)null;
                                lead.inputByUserID = session.UserID;
                                lead.inputDateTime = now;
                                lead.assignedToUserID = arrival.FirstOrDefault().AssignedToUserID ?? session.UserID;
                                lead.assignationDate = now;
                                lead.inputMethodID = 2;//import
                                lead.leadStatusID = arrival.Count(m => m.codigostatusreservacion != null) > 0 ? arrival.Count(m => m.codigostatusreservacion == "CA") == arrival.Count() ? 4 : 1 : 1;//new
                                lead.isTest = false;
                                lead.frontOfficeGuestID = (int)arrival.FirstOrDefault().idhuesped;
                                lead.frontOfficeResortID = (int)arrival.FirstOrDefault().idresort;
                                lead.tags = arrival.FirstOrDefault().Tags;

                                if (contactInfo.Email != null)
                                {
                                    var email = new tblLeadEmails();
                                    email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                    email.main = true;
                                    lead.tblLeadEmails.Add(email);
                                    isNonContactable = false;
                                }

                                if (contactInfo.Phone != null)
                                {
                                    var phone = new tblPhones();
                                    phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                    phone.phoneTypeID = 4;//unknown
                                    phone.doNotCall = false;
                                    phone.main = true;
                                    lead.tblPhones.Add(phone);
                                    isNonContactable = false;
                                }

                                if (isNonContactable)
                                {
                                    if (lead.leadSourceID != 1 && lead.leadSourceID != 54)
                                    {
                                        lead.bookingStatusID = 15;//not contactable
                                    }
                                }

                                if (lead.leadSourceID != null && (sources.Single(m => m.leadSourceID == lead.leadSourceID).leadSource.Trim() == "Member" || sources.Single(m => m.leadSourceID == lead.leadSourceID).leadSource.Trim() == "Clac Member"))
                                {
                                    var member = new tblMemberInfo();
                                    member.memberNumber = arrival.FirstOrDefault().Contrato;
                                    member.clubType = null;
                                    member.revenues = 0;
                                    member.isNational = lead.countryID == 3;
                                    member.presentationConfirmed = false;
                                    member.hasOptions = false;
                                    member.isVIP = false;
                                    member.pushedToOnSiteConcierge = false;
                                    member.isAllInclusive = false;
                                    member.coOwner = null;
                                    member.memberName = null;
                                    member.contractNumber = null;
                                    lead.tblMemberInfo.Add(member);
                                }

                                #endregion
                            }
                            else
                            {
                                #region "existing lead"
                                counterExistingImported++;
                                lead = existingLead.FirstOrDefault();
                                lead.guestHubID = guestHub != null ? guestHub.guestHubID : (Guid?)null;
                                lead.leadStatusID = arrival.Count(m => m.codigostatusreservacion != null) > 0 ? arrival.Count(m => m.codigostatusreservacion == "CA") == arrival.Count() ? 4 : lead.leadStatusID != 4 ? lead.leadStatusID : 2 : 1;//new
                                isNonContactable = false;

                                if (lead.tblLeadEmails.Count() == 0)
                                {
                                    if (contactInfo.Email != null)
                                    {
                                        var email = new tblLeadEmails();
                                        email.email = contactInfo.Email != "" ? contactInfo.Email : contactInfo.UnformattedEmail;
                                        email.main = true;
                                        lead.tblLeadEmails.Add(email);
                                    }
                                }

                                if (lead.tblPhones.Count() == 0)
                                {
                                    if (contactInfo.Phone != null)
                                    {
                                        var phone = new tblPhones();
                                        phone.phone = contactInfo.Phone != "" ? contactInfo.Phone : contactInfo.UnformattedPhone;
                                        phone.phoneTypeID = 4;//unknown
                                        phone.doNotCall = false;
                                        phone.main = true;
                                        lead.tblPhones.Add(phone);
                                    }
                                }

                                if (lead.tblMemberInfo != null && lead.tblMemberInfo.Count() > 0)
                                {
                                    if (lead.leadSourceID != null && (sources.Single(m => m.leadSourceID == lead.leadSourceID).leadSource.Trim() == "Member" || sources.Single(m => m.leadSourceID == lead.leadSourceID).leadSource.Trim() == "Clac Member"))
                                    {
                                        var member = lead.tblMemberInfo.FirstOrDefault();
                                        member.memberNumber = arrival.FirstOrDefault().Contrato;
                                        member.clubType = null;
                                        member.revenues = 0;
                                        member.isNational = lead.countryID == 3;
                                        member.presentationConfirmed = false;
                                        member.hasOptions = false;
                                        member.isVIP = false;
                                        member.pushedToOnSiteConcierge = false;
                                        member.isAllInclusive = false;
                                        member.coOwner = null;
                                        member.memberName = null;
                                        member.contractNumber = null;
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region "reservation(s) assignation based on splits"
                            if (arrival.Count() == arrival.Count(m => m.Split == true))
                            {
                                foreach (var reservation in arrival)
                                {
                                    #region "new reservation"
                                    var rsv = new tblReservations();
                                    rsv.reservationID = Guid.NewGuid();
                                    rsv.arrivalDate = reservation.Arrival != null && reservation.Arrival != "" ? DateTime.Parse(reservation.Arrival) : (DateTime?)null;
                                    rsv.departureDate = reservation.Departure != null && reservation.Departure != "" ? DateTime.Parse(reservation.Departure) : (DateTime?)null;
                                    rsv.placeID = place != null ? place.placeID : (long?)null;
                                    rsv.roomTypeID = place != null ? roomTypes.Count(m => m.Key == place.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == place.placeID).Count(m => m.roomTypeCode == reservation.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == place.placeID).FirstOrDefault(m => m.roomTypeCode == reservation.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                    rsv.planTypeID = null;
                                    rsv.certificateNumber = reservation.CRS;
                                    rsv.destinationID = place != null ? place.destinationID : (long?)null;
                                    rsv.hotelConfirmationNumber = reservation.numconfirmacion;
                                    rsv.totalNights = reservation.CuartosNoche;
                                    rsv.sysWorkGroupID = session.WorkGroupID;
                                    rsv.guestsNames = reservation.Huesped;
                                    rsv.reservationComments = reservation.Comentario;
                                    rsv.reservationStatusID = reservation.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == reservation.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == reservation.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    rsv.adults = reservation.Adultos ?? 0;
                                    rsv.children = reservation.Ninos ?? 0;
                                    rsv.isTest = false;
                                    rsv.dateSaved = now;
                                    rsv.frontOfficeReservationID = (long)reservation.idReservacion;
                                    rsv.frontOfficePlanType = reservation.TipoPlan;
                                    rsv.frontOfficeAgencyName = reservation.nameagencia;
                                    rsv.frontOfficeMarketCode = reservation.Procedencia;
                                    rsv.frontOfficeRoomListID = reservation.idroomlist;
                                    rsv.frontOfficeContractNumber = reservation.Contrato;
                                    rsv.frontOfficeCertificateNumber = reservation.CRS;
                                    lead.tblReservations.Add(rsv);
                                    reservation.Status = true;
                                    reservation.Message = "Imported";
                                    #endregion
                                }
                            }
                            else
                            {
                                if (arrival.FirstOrDefault().Contrato != null && arrival.FirstOrDefault().Contrato.Trim() != "")
                                {
                                    var arrivalsPerContract = arrivals.Where(m => m.Contrato.Trim() == arrival.FirstOrDefault().Contrato.Trim() && m.Contrato.IndexOf("xchange") == -1 && m.Contrato.IndexOf("ending") == -1);
                                    contracts.Add(arrival.FirstOrDefault().Contrato.Trim());
                                    foreach (var reservation in arrivalsPerContract)
                                    {
                                        #region "new reservation"
                                        var rsv = new tblReservations();
                                        rsv.reservationID = Guid.NewGuid();
                                        rsv.arrivalDate = reservation.Arrival != null && reservation.Arrival != "" ? DateTime.Parse(reservation.Arrival) : (DateTime?)null;
                                        rsv.departureDate = reservation.Departure != null && reservation.Departure != "" ? DateTime.Parse(reservation.Departure) : (DateTime?)null;
                                        rsv.placeID = place != null ? place.placeID : (long?)null;
                                        rsv.roomTypeID = place != null ? roomTypes.Count(m => m.Key == place.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == place.placeID).Count(m => m.roomTypeCode == reservation.TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == place.placeID).FirstOrDefault(m => m.roomTypeCode == reservation.TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                        //rsv.planTypeID = Dictionaries.PlanTypes.Count(m => m.Key == reservation.TipoPlan.ToLower()) > 0 ? Dictionaries.PlanTypes.FirstOrDefault(m => m.Key == reservation.TipoPlan.ToLower()).Value : (int?)null;
                                        rsv.planTypeID = null;
                                        rsv.certificateNumber = reservation.CRS;
                                        rsv.destinationID = place != null ? place.destinationID : (long?)null;
                                        rsv.hotelConfirmationNumber = reservation.numconfirmacion;
                                        rsv.totalNights = reservation.CuartosNoche;
                                        rsv.sysWorkGroupID = session.WorkGroupID;
                                        rsv.guestsNames = reservation.Huesped;
                                        rsv.reservationComments = reservation.Comentario;
                                        rsv.reservationStatusID = reservation.codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == reservation.codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == reservation.codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                        rsv.adults = reservation.Adultos ?? 0;
                                        rsv.children = reservation.Ninos ?? 0;
                                        rsv.isTest = false;
                                        rsv.dateSaved = now;
                                        rsv.frontOfficeReservationID = (long)reservation.idReservacion;
                                        rsv.frontOfficePlanType = reservation.TipoPlan;
                                        rsv.frontOfficeAgencyName = reservation.nameagencia;
                                        rsv.frontOfficeMarketCode = reservation.Procedencia;
                                        rsv.frontOfficeRoomListID = reservation.idroomlist;
                                        rsv.frontOfficeContractNumber = reservation.Contrato;
                                        rsv.frontOfficeCertificateNumber = reservation.CRS;
                                        lead.tblReservations.Add(rsv);
                                        reservation.Status = true;
                                        reservation.Message = "Imported";
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region "new reservation"
                                    var rsv = new tblReservations();
                                    rsv.reservationID = Guid.NewGuid();
                                    rsv.arrivalDate = arrival.FirstOrDefault().Arrival != null && arrival.FirstOrDefault().Arrival != "" ? DateTime.Parse(arrival.FirstOrDefault().Arrival) : (DateTime?)null;
                                    rsv.departureDate = arrival.FirstOrDefault().Departure != null && arrival.FirstOrDefault().Departure != "" ? DateTime.Parse(arrival.FirstOrDefault().Departure) : (DateTime?)null;
                                    rsv.placeID = place != null ? place.placeID : (long?)null;
                                    rsv.roomTypeID = place != null ? roomTypes.Count(m => m.Key == place.placeID) > 0 ? roomTypes.FirstOrDefault(m => m.Key == place.placeID).Count(m => m.roomTypeCode == arrival.FirstOrDefault().TipoHab) > 0 ? roomTypes.FirstOrDefault(m => m.Key == place.placeID).FirstOrDefault(m => m.roomTypeCode == arrival.FirstOrDefault().TipoHab).roomTypeID : (long?)null : (long?)null : (long?)null;
                                    //rsv.planTypeID = Dictionaries.PlanTypes.Count(m => m.Key == arrival.FirstOrDefault().TipoPlan.ToLower()) > 0 ? Dictionaries.PlanTypes.FirstOrDefault(m => m.Key == arrival.FirstOrDefault().TipoPlan.ToLower()).Value : (int?)null;
                                    rsv.planTypeID = null;
                                    rsv.certificateNumber = arrival.FirstOrDefault().CRS;
                                    rsv.destinationID = place != null ? place.destinationID : (long?)null;
                                    rsv.hotelConfirmationNumber = arrival.FirstOrDefault().numconfirmacion;
                                    rsv.totalNights = arrival.FirstOrDefault().CuartosNoche;
                                    rsv.sysWorkGroupID = session.WorkGroupID;
                                    rsv.guestsNames = arrival.FirstOrDefault().Huesped;
                                    rsv.reservationComments = arrival.FirstOrDefault().Comentario;
                                    rsv.reservationStatusID = arrival.FirstOrDefault().codigostatusreservacion != null ? Dictionaries.FrontReservationStatus.Count(m => m.Key == arrival.FirstOrDefault().codigostatusreservacion.Trim()) > 0 ? Dictionaries.FrontReservationStatus.FirstOrDefault(m => m.Key == arrival.FirstOrDefault().codigostatusreservacion.Trim()).Value : (int?)null : (int?)null;
                                    rsv.adults = arrival.FirstOrDefault().Adultos ?? 0;
                                    rsv.children = arrival.FirstOrDefault().Ninos ?? 0;
                                    rsv.isTest = false;
                                    rsv.dateSaved = now;
                                    rsv.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");//eplat.villagroup.com
                                    rsv.frontOfficeReservationID = (long)arrival.FirstOrDefault().idReservacion;
                                    rsv.frontOfficePlanType = arrival.FirstOrDefault().TipoPlan;
                                    rsv.frontOfficeAgencyName = arrival.FirstOrDefault().nameagencia;
                                    rsv.frontOfficeMarketCode = arrival.FirstOrDefault().Procedencia;
                                    rsv.frontOfficeRoomListID = arrival.FirstOrDefault().idroomlist;
                                    rsv.frontOfficeContractNumber = arrival.FirstOrDefault().Contrato;
                                    rsv.frontOfficeCertificateNumber = arrival.FirstOrDefault().CRS;
                                    lead.tblReservations.Add(rsv);
                                    arrival.FirstOrDefault().Status = true;
                                    arrival.FirstOrDefault().Message = "Imported";
                                    #endregion
                                }
                            }
                            #endregion

                            #region "set Not Contacted for existing lead"
                            if (existingLead.Count() > 0)
                            {
                                var existingReservations = existingLead.FirstOrDefault().tblReservations;
                                var arrivalDate = DateTime.Parse(arrival.FirstOrDefault().Arrival);

                                if (existingReservations.Count() == 0 || existingReservations.Count(m => arrivalDate <= m.departureDate) == 0)
                                {
                                    if (!isNonContactable)
                                    {
                                        lead.bookingStatusID = 10;//Not Contacted
                                        var interaction = new tblInteractions();
                                        interaction.bookingStatusID = 10;
                                        interaction.interactionTypeID = 13;//Note
                                        interaction.interactionComments = "Set by System Administrator due to a new reservation of an existing lead.";
                                        interaction.savedByUserID = Guid.Parse("C53613B6-C8B8-400D-95C6-274E6E60A14A");//System Administrator
                                        interaction.dateSaved = now;
                                        interaction.interactedWithUserID = session.UserID;
                                        interaction.parentInteractionID = null;
                                        interaction.totalSold = null;
                                        lead.tblInteractions.Add(interaction);
                                    }
                                }
                            }
                            #endregion

                            #region "MarketCode => LeadSource assignation (assignation update)"

                            if (marketCode.Count() > 0)
                            {
                                marketCode.FirstOrDefault().leadSourceID = arrival.FirstOrDefault().LeadSource != null && arrival.FirstOrDefault().LeadSource != "" ? long.Parse(arrival.FirstOrDefault().LeadSource) : (long?)null;
                            }
                            else
                            {
                                var mk = new tblMarketCodes();
                                mk.marketCode = arrival.FirstOrDefault().Procedencia;
                                mk.frontOfficeResortID = (int)arrival.FirstOrDefault().idresort;
                                mk.leadSourceID = arrival.FirstOrDefault().LeadSource != null && arrival.FirstOrDefault().LeadSource != "" ? long.Parse(arrival.FirstOrDefault().LeadSource) : (long?)null;
                                db.tblMarketCodes.AddObject(mk);
                            }
                            #endregion

                            #region "LeadSource => User assignation (only first assignation)"
                            if (leadSources.Count(m => m.leadSourceID == lead.leadSourceID) == 0)
                            {
                                if (arrival.FirstOrDefault().AssignedToUserID != null && arrival.FirstOrDefault().LeadSource != null)
                                {
                                    var ls = new tblUsers_LeadSources();
                                    ls.leadSourceID = long.Parse(arrival.FirstOrDefault().LeadSource);
                                    ls.userID = (Guid)arrival.FirstOrDefault().AssignedToUserID;
                                    ls.terminalID = terminals.FirstOrDefault();
                                    ls.frontOfficeResortID = (int)resort;
                                    ls.fromDate = now;
                                    db.tblUsers_LeadSources.AddObject(ls);
                                }
                            }
                            #endregion

                            if (existingLead.Count() == 0)
                            {
                                db.tblLeads.AddObject(lead);
                            }

                            db.SaveChanges();
                            arrival.FirstOrDefault().LeadID = lead.leadID;
                        }
                    }
                    catch (Exception ex)
                    {
                        foreach (var i in arrival)
                        {
                            i.Status = false;
                            i.Message = ex.Message + (ex.InnerException != null ? "<br />" + ex.InnerException.Message : "");
                        }
                        response.Exception = ex;
                    }
                }
            }

            response.Type = response.Exception != null ? Attempt_ResponseTypes.Warning : Attempt_ResponseTypes.Ok;
            response.Message = response.Exception != null ? "There were some errors with Import" : "Import Successful";
            response.ObjectID = new { arrivals = serializer.Serialize(arrivals) };
            return response;
        }

        public AttemptResponse MassUpdate(MassUpdate lvm)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            ChangesTracking.ChangeItem changeItem = new ChangesTracking.ChangeItem();
            List<ChangesTracking.ChangeItem> listChanges = new List<ChangesTracking.ChangeItem>();
            List<KeyValuePair<string, string>> listPropertiesValues = new List<KeyValuePair<string, string>>();
            var model = typeof(MassUpdate).GetProperties();
            var listTables = new List<string>();
            var changedFields = "";
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator") || GeneralFunctions.IsUserInRole("Operation Manager");

            foreach (var i in model)
            {
                var _a = i.GetCustomAttributes(typeof(DoNotUpdateAttribute), false);
                if (i.GetCustomAttributes(true).Count() != 0 && i.GetCustomAttributes(typeof(DoNotUpdateAttribute), false).Count() == 0)
                {
                    var property = Reflection.GetPropertyByName(lvm, i.Name);
                    var propertyValue = property.GetValue(lvm, null) != null && property.GetValue(lvm, null).ToString() != "0" ? property.GetValue(lvm, null) : null;
                    var propertyDbFieldName = Reflection.GetCustomAttribute<FieldInfoAttribute>(i).Name;
                    var propertyDbTableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(i).Name;
                    if (propertyValue != null)
                    {
                        listPropertiesValues.Add(new KeyValuePair<string, string>(propertyDbTableName + "." + propertyDbFieldName, propertyValue.ToString()));
                        changedFields += propertyDbFieldName + ",";
                        if (listTables.Where(m => m == propertyDbTableName).Count() == 0)
                        {
                            listTables.Add(propertyDbTableName);
                        }
                    }
                }
            }
            changedFields = changedFields.Substring(0, changedFields.Length - 1);

            if (lvm.MassUpdate_Coincidences != null)
            {
                try
                {
                    var _coindidences = lvm.MassUpdate_Coincidences;
                    var str = lvm.MassUpdate_Coincidences.Replace("'", "");
                    var leads = "(";
                    leads += _coindidences;
                    leads += ")";
                    var whereString = " WHERE tblLeads.leadID IN";
                    foreach (var i in listTables)//involve distinct tables 
                    {
                        var setString = "UPDATE " + i + " SET ";
                        foreach (var a in listPropertiesValues.Where(m => m.Key.Substring(0, m.Key.IndexOf('.')) == i))
                        {
                            #region "create change log"
                            foreach (var coincidence in str.Split(','))
                            {
                                Guid leadID = Guid.Parse(coincidence);
                                var lead = db.tblLeads.Single(m => m.leadID == leadID);
                                if (lead.bookingStatusID != 16 || isAdmin)//sold
                                {
                                    var currentPrimaryKey = Reflection.GetPrimaryKeyName(a.Key.Substring(0, a.Key.IndexOf('.')));
                                    var currentPrimaryKeyValue = lead.GetType().GetProperty(currentPrimaryKey).GetValue(lead, null) != null ? lead.GetType().GetProperty(currentPrimaryKey).GetValue(lead, null).ToString() : null;
                                    var field = a.Key.Substring((a.Key.IndexOf('.') + 1), (a.Key.Length - (a.Key.IndexOf('.') + 1)));
                                    var previousValue = lead.GetType().GetProperty(field).GetValue(lead, null) != null ? lead.GetType().GetProperty(field).GetValue(lead, null).ToString() : null;
                                    if (a.Value != "")
                                    {
                                        listChanges.Add(new ChangesTracking.ChangeItem()
                                        {
                                            SysComponentID = ChangesTracking.GetSysComponentIDs(i).Single(m => m.Key == field).Value,
                                            PreviousValue = previousValue,
                                            CurrentValue = a.Value,
                                            FullReferenceText = lead.firstName + " " + lead.lastName,
                                            ReferenceID = currentPrimaryKeyValue
                                        });
                                    }
                                }
                                else
                                {
                                    leads = leads.Replace("'" + coincidence + "'", "")
                                        .Replace("(,", "(")
                                        .Replace(",)", ")")
                                        .Replace(",,", ",");
                                }
                            }
                            #endregion
                            setString += a.Key + "='" + a.Value + "', ";
                        }
                        setString = setString.Substring(0, setString.Length - 2);//remove last comma

                        var sqlString = setString + whereString + leads;
                        if (!isAdmin)
                        {
                            var mail = EmailNotifications.GetSystemEmail("Intento de actualización masiva.<br />Hecho por: " + session.User + "<br />Fecha: " + DateTime.Now + "<br />query: " + sqlString, "efalcon@villagroup.com");
                            EmailNotifications.Send(mail);
                        }
                        if (leads != "()")
                        {
                            db.ExecuteStoreCommand(sqlString);
                        }
                        else
                        {
                            response.Type = Attempt_ResponseTypes.Warning;
                            response.Message = "No valid leads to update. <br />Please note that Sold Leads cannot be reassigned.";
                            response.ObjectID = 0;
                            return response;
                        }
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead(s) Updated";
                    response.ObjectID = 0;
                    ChangesTracking.LogChanges(listChanges);
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead(s) NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            return response;
        }

        public AttemptResponse MassInsert(MassUpdate lvm)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            ChangesTracking.ChangeItem changeItem = new ChangesTracking.ChangeItem();
            List<ChangesTracking.ChangeItem> listChanges = new List<ChangesTracking.ChangeItem>();
            List<KeyValuePair<string, string>> listPropertiesValues = new List<KeyValuePair<string, string>>();
            var model = typeof(MassUpdate).GetProperties();
            var listTables = new List<string>();

            foreach (var i in model)
            {
                var _a = i.GetCustomAttributes(typeof(DoNotUpdateAttribute), false);
                if (i.GetCustomAttributes(true).Count() != 0 && i.GetCustomAttributes(typeof(DoNotUpdateAttribute), false).Count() > 0)
                {
                    var property = Reflection.GetPropertyByName(lvm, i.Name);
                    var propertyValue = property.GetValue(lvm, null) != null && property.GetValue(lvm, null).ToString() != "0" ? property.GetValue(lvm, null) : null;
                    var propertyDbFieldName = Reflection.GetCustomAttribute<FieldInfoAttribute>(i).Name;
                    var propertyDbTableName = Reflection.GetCustomAttribute<DataBaseInfoAttribute>(i).Name;
                    if (propertyValue != null)
                    {
                        listPropertiesValues.Add(new KeyValuePair<string, string>(propertyDbTableName + "." + propertyDbFieldName, propertyValue.ToString()));
                        if (listTables.Where(m => m == propertyDbTableName).Count() == 0)
                        {
                            listTables.Add(propertyDbTableName);
                        }
                    }
                }
            }

            if (lvm.MassUpdate_Coincidences != null)
            {
                try
                {
                    var str = lvm.MassUpdate_Coincidences.Replace("'", "");
                    foreach (var _table in listTables)//involve distinct tables 
                    {
                        foreach (var _coincidence in str.Split(','))
                        {
                            var _sqlInsert = "INSERT INTO " + _table;
                            var _leadID = Guid.Parse(_coincidence);
                            var lead = db.tblLeads.Single(m => m.leadID == _leadID);
                            var _fields = "";
                            var _values = "";
                            foreach (var item in listPropertiesValues.Where(m => m.Key.Substring(0, m.Key.IndexOf('.')) == _table))
                            {
                                var _field = item.Key.Substring((item.Key.IndexOf('.') + 1), (item.Key.Length - (item.Key.IndexOf('.') + 1)));
                                var _value = item.Value;
                                if (_value != null)
                                {
                                    DateTime isDateTime;
                                    if (DateTime.TryParse(_value, out isDateTime))
                                    {
                                        _value = "'" + DateTime.Parse(_value).ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture) + "'";
                                    }
                                    else
                                    {
                                        _value = "'" + item.Value + "'";
                                    }
                                }
                                _fields += (_fields == "" ? "" : ",") + _field;
                                _values += (_values == "" ? "" : ",") + _value;

                                var currentPrimaryKey = Reflection.GetPrimaryKeyName(item.Key.Substring(0, item.Key.IndexOf('.')));
                                listChanges.Add(new ChangesTracking.ChangeItem()
                                {
                                    SysComponentID = ChangesTracking.GetSysComponentIDs(_table).Single(m => m.Key == _field).Value,
                                    PreviousValue = "",
                                    CurrentValue = item.Value,
                                    FullReferenceText = lead.firstName + " " + lead.lastName,
                                    ReferenceID = ""
                                });
                            }
                            _fields += (_fields == "" ? "" : ",") + "leadID";
                            _values += (_values == "" ? "" : ",") + "'" + _coincidence + "'";
                            _sqlInsert += " (" + _fields + ") VALUES(" + _values + ")";
                            db.ExecuteStoreCommand(_sqlInsert);
                        }
                    }
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Lead(s) info Saved";
                    response.ObjectID = 0;
                    ChangesTracking.LogChanges(listChanges);
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Lead(s) info NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            return response;
        }

        public AttemptResponse MassSending(MassUpdate model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            //var eventID = model.MassUpdate_SendingEvent;
            var leads = model.MassUpdate_Coincidences.Replace("'", "").Split(',').Select(m => Guid.Parse(m)).ToArray();
            var serializer = new JavaScriptSerializer();

            List<AttemptResponse> responses = new List<AttemptResponse>();

            foreach (var lead in leads)
            {
                //get resevationid, emailnotificationid and transactionid
                var reservationID = db.tblReservations.Where(m => m.leadID == lead).OrderByDescending(m => m.arrivalDate).FirstOrDefault().reservationID;
                var availableLetters = GetAvailableLetters(reservationID, model.MassUpdate_SendingEvent);
                var notification = availableLetters.FirstOrDefault();
                var preview = notification != null ? PreviewEmail(reservationID, notification.ID) : null;
                if (preview != null)
                {
                    var send = SendEmail(serializer.Serialize(preview));
                    responses.Add(send);
                }
            }

            response.Type = Utils.Attempt_ResponseTypes.Ok;
            response.Message = "Emails Sent: " + responses.Count(m => m.Type == Utils.Attempt_ResponseTypes.Ok)
                + "<br />Emails with Warning: " + string.Concat(responses.Where(m => m.Type == Utils.Attempt_ResponseTypes.Warning).Select(m => m.ObjectID))
                + "<br />Emails with Error: " + responses.Count(m => m.Type == Utils.Attempt_ResponseTypes.Error);
            response.ObjectID = 0;

            return response;
        }

        public AttemptResponse SendEmail(string model, bool sendOnly = false, System.Web.HttpContextBase context = null)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            var serializer = new JavaScriptSerializer();
            EmailPreViewModel item = serializer.Deserialize<EmailPreViewModel>(model);
            tblEmailNotificationLogs notificationLog = new tblEmailNotificationLogs();
            List<SurveyViewModel.FieldValue> list = new List<SurveyViewModel.FieldValue>();
            SurveyViewModel.SurveyValuesModel instance = new SurveyViewModel.SurveyValuesModel();
            Guid transactionID = Guid.NewGuid();
            var fieldGroupID = int.Parse(item.FieldGroup);
            var fieldGroup = db.tblFieldGroups.Single(m => m.fieldGroupID == fieldGroupID);
            var fields = fieldGroup.tblFields;
            var obj = new FieldValueModel();
            var notification = fieldGroup.tblEmailNotifications;
            var email = EmailNotifications.GetEmailByNotification(notification.emailNotificationID);

            email.From = new System.Net.Mail.MailAddress(item.FromAddress, item.FromAlias);
            if (item.To != null && item.To != "")
            {
                email.To.Add(item.To);
            }
            email.ReplyToList.Add(item.ReplyTo);
            email.Subject = item.Subject;
            if (item.CC != "")
            {
                var _cc = email.CC.Count() > 0 ? string.Join(",", email.CC) : "";
                item.CC = item.CC.Replace(" ", "");
                var cc = _cc + "," + item.CC;
                var ccArray = cc.Split(',').Distinct();
                email.CC.Clear();
                email.CC.Add(string.Join(",", ccArray));
            }
            if (item.BCC != "")
            {
                var cco = email.Bcc.Count() > 0 ? string.Join(",", email.Bcc) : "";
                item.BCC = item.BCC.Replace(" ", "");
                var bcc = cco + "," + item.BCC;
                var bccArray = bcc.Split(',').Distinct();
                email.Bcc.Clear();
                email.Bcc.Add(string.Join(",", bccArray));
            }

            //var anchor = Regex.Match(item.Body, @"<a.*?>.*?</a>", RegexOptions.Singleline);
            //if (anchor.Success)
            //{
            //    var aux = anchor.Groups[0].Value;
            //    if (anchor.Groups[0].Value.IndexOf("#") != -1)
            //    {
            //        aux = aux.Replace("href=\"", "href=\"https://eplat.villagroup.com/public/PaymentConfirmation?transaction=" + transactionID);
            //        item.Body = item.Body.Replace(anchor.Groups[0].Value, aux);
            //    }
            //}

            email.Body = email.Body.Replace("$TransactionID", transactionID.ToString());
            item.Body = item.Body.Replace("$TransactionID", transactionID.ToString());

            email.Body = EmailNotifications.InsertTracker(item.Body, transactionID.ToString());

            #region "save in tblEmailNotificationLogs"
            var itemID = item.ListFieldsValues.Count(m => m.Key.IndexOf("ItemID") != -1) > 0 ? Guid.Parse(item.ListFieldsValues.FirstOrDefault(m => m.Key.IndexOf("ItemID") != -1).Value) : (Guid?)null;
            var sentByUserID = HttpContext.Current.Request.IsAuthenticated && !HttpContext.Current.Request.IsLocal ? session.UserID : db.aspnet_Users.Single(m => m.UserName == item.FromAddress).UserId;

            notificationLog.dateSent = DateTime.Now;
            //notificationLog.sentByUserID = HttpContext.Current.Request.IsAuthenticated ? session.UserID : db.aspnet_Users.Single(m => m.UserName == item.FromAddress).UserId;
            notificationLog.sentByUserID = sentByUserID;
            notificationLog.reservationID = item.ListFieldsValues.Count(m => m.Key.IndexOf("ItemID") != -1) > 0 ? Guid.Parse(item.ListFieldsValues.FirstOrDefault(m => m.Key.IndexOf("ItemID") != -1).Value) : item.ReservationID != null ? Guid.Parse(item.ReservationID) : (Guid?)null;
            notificationLog.leadID = item.LeadID != null ? Guid.Parse(item.LeadID) : (Guid?)null;
            notificationLog.emailNotificationID = notification.emailNotificationID;
            notificationLog.trackingID = transactionID;
            notificationLog.subject = item.Subject;
            notificationLog.emailPreviewJson = serializer.Serialize(item);
            db.tblEmailNotificationLogs.AddObject(notificationLog);
            db.SaveChanges();
            #endregion

            #region "save in tblFieldsValues"
            foreach (var field in item.ListFieldsValues)
            {
                if (field.Key == "$SignatureLink" || field.Key == "$TransactionID")
                {
                    //var a = "<a href=\"https://eplat.villagroup.com/public/PaymentConfirmation?transaction=" + transactionID + "\">Please click on this link in order to e-sign this document. Your signature is required to finalize the process.</a>";
                    var a = transactionID.ToString();
                    list.Add(new SurveyViewModel.FieldValue()
                    {
                        FieldID = (int)fields.FirstOrDefault(m => m.field == field.Key).fieldID,
                        Value = a
                    });
                }
                else
                {
                    list.Add(new SurveyViewModel.FieldValue()
                    {
                        FieldID = (int)fields.FirstOrDefault(m => m.field == field.Key).fieldID,
                        Value = field.Value
                    });
                }
            }
            instance.SurveyID = (Guid)fieldGroup.fieldGroupGUID;
            instance.TransactionID = transactionID;
            instance.Fields = list;

            response = new NotificationsDataModel().SaveFieldValues(instance, context, email);
            if (response.Type == Attempt_ResponseTypes.Ok)
            {
                var _response = EmailNotifications.Send(new List<MailMessageResponse>() { new MailMessageResponse() { MailMessage = email } });
                if (!_response.FirstOrDefault().Sent.Value)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Email NOT Sent. Please report to System Administrator.<br />Key: " + transactionID;
                    var log = db.tblEmailNotificationLogs.FirstOrDefault(m => m.trackingID == transactionID);
                    var fv = db.tblFieldsValues.Where(m => m.transactionID == transactionID);

                }
            }
            #endregion

            return response;
        }

        public EmailPreViewModel PreviewEmail(object id, int emailNotificationID, string transactionID = null, System.Web.HttpContextBase context = null)
        {
            ePlatEntities db = new ePlatEntities();
            PublicDataModel pdm = new PublicDataModel();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var preview = new EmailPreViewModel();
            var list = new List<KeyValuePair<string, string>>();
            var _list = new List<KeyValue>();

            var fieldGroup = db.tblFieldGroups.Single(m => m.emailNotificationID == emailNotificationID);
            var notification = db.tblEmailNotifications.Single(m => m.emailNotificationID == emailNotificationID);
            var email = EmailNotifications.GetEmailByNotification(notification.emailNotificationID);
            var itemID = Guid.Parse(id.ToString());
            var query = db.tblReservations.Single(m => m.reservationID == itemID);
            var body = email.Body;

            try
            {
                if (transactionID != null && transactionID != "")
                {
                    var transaction = Guid.Parse(transactionID);
                    var fieldValues = db.tblFieldsValues.Where(m => m.transactionID == transaction);

                    if (db.tblEmailNotificationLogs.Count(m => m.trackingID == transaction) > 0)
                    {
                        var log = db.tblEmailNotificationLogs.FirstOrDefault(m => m.trackingID == transaction);
                        if (log.emailPreviewJson != null)
                        {
                            EmailPreViewModel response;
                            try
                            {
                                response = serializer.Deserialize<EmailPreViewModel>(log.emailPreviewJson);
                                return response;
                            }
                            catch { }
                        }
                    }

                    var oldTemplate = false;
                    foreach (var field in fieldGroup.tblFields)
                    {
                        var _value = fieldValues.Count(m => m.fieldID == field.fieldID) > 0 ? fieldValues.FirstOrDefault(m => m.fieldID == field.fieldID).value : "";
                        var value = _value;
                        if (field.field == "$PaymentInfo" && value == "")
                        {
                            oldTemplate = true;
                        }
                        if (field.field == "$OptionsSold")
                        {
                            if (_value.IndexOf("table") == -1)
                            {
                                var a = "<table style=\"font-family:verdana;text-align:center;font-size:10pt;border-collapse:separate;border-spacing:15px;\" align=\"center\">"
                                    + "<thead><tr><th>Qty</th><th>Option</th><th>Date</th><th>Total</th></tr></thead><tbody style=\"text-align:center;font-size:10pt;\">";
                                var _options = serializer.Deserialize<List<OptionalsSoldModel>>(_value);
                                foreach (var i in _options)
                                {
                                    a += "<tr><td>" + i.quantity + "</td><td>" + i.option + "</td><td>" + DateTime.Parse(i.date).ToString("MMM dd, yyyy hh:mm tt") + "</td><td>" + Decimal.Round((decimal.Parse(i.cost) * decimal.Parse(i.quantity)), 2) + "</td></tr>";
                                }
                                a += "</tbody></table>";
                                value = a;
                            }
                        }
                        list.Add(new KeyValuePair<string, string>(field.field, value));
                    }
                    var str = "";
                    if (oldTemplate)
                    {
                        var arr = list.Where(m => m.Key == "$PaymentType" || m.Key == "$Amount" || m.Key == "$PaymentDate" || m.Key == "$Invoice" || m.Key == "$CardHolder" || m.Key == "$CardType" || m.Key == "$CardNumber");
                        str += "<table align=\"center\"><tbody style=\"font - family: verdana; text - align: center; font - size: 10pt;\">";
                        foreach (var i in arr)
                        {
                            var header = list.Count(m => m.Key == ("$Header" + i.Key.Substring(1))) > 0 ? list.FirstOrDefault(m => m.Key == ("$Header" + i.Key.Substring(1))).Value : i.Key.Substring(1);
                            str += "<tr><td style=\"text-align:left;\">" + header + "</td><td  style=\"text-align:right;\">" + i.Value + "</td></tr>";
                        }
                        str += "</tbody></table>";
                        list[list.FindIndex(m => m.Key == "$PaymentInfo")] = new KeyValuePair<string, string>("$PaymentInfo", str);
                    }

                    //pendiente llenar la instancia de correo con la información usada en el envío para mostrar en el preview
                    Guid sentByUserID = Guid.Empty;
                    Guid.TryParse(fieldValues.Count(x => x.tblFields.field == "$SentByUserID") > 0 ? fieldValues.FirstOrDefault(x => x.tblFields.field == "$SentByUserID").value : Guid.Empty.ToString(), out sentByUserID);
                    var from = sentByUserID == Guid.Empty ? "" : db.aspnet_Membership.FirstOrDefault(m => m.UserId == sentByUserID).Email;
                    var fromName = fieldValues.Count(x => x.tblFields.field == "$SentByUser") > 0 ? fieldValues.FirstOrDefault(x => x.tblFields.field == "$SentByUser").value : Membership.GetUser(from).ProviderUserKey;
                    Guid fromID;
                    var profile = Guid.TryParse(fromName.ToString(), out fromID) ? db.tblUserProfiles.Where(m => m.userID == fromID).Select(m => new { username = m.firstName + " " + m.lastName }).FirstOrDefault().username : fromName.ToString();
                    email.From = new System.Net.Mail.MailAddress(from, profile);
                    var to = fieldValues.Count(x => x.tblFields.field == "$SentToAddress") > 0 ? fieldValues.FirstOrDefault(x => x.tblFields.field == "$SentToAddress").value : query.tblLeads.tblLeadEmails.FirstOrDefault(m => m.main == true && m.dnc != true).email;
                    var recipients = to.Split(',');
                    foreach (var r in recipients)
                    {
                        email.To.Add(r.Trim());
                    }

                    foreach (var i in list)
                    {
                        _list.Add(new KeyValue() { Key = i.Key, Value = i.Value });
                    }
                    preview = pdm.ReplaceFieldsValues(email, "en-US", _list, fieldGroup.fieldGroupID, transaction);
                    preview.SysEvent = notification.eventID.ToString();
                    return preview;
                }
                else
                {
                    var fields = fieldGroup.tblFields.Select(m => m.field).ToList();
                    PublicDataModel.ReplaceReservedWords(query, fields, ref _list, context);

                    if (email.From.Address == "empty@empty.com")
                    {
                        //if (HttpContext.Current.User.Identity.IsAuthenticated)
                        if (HttpContext.Current.User.Identity.IsAuthenticated && !HttpContext.Current.Request.IsLocal)
                        {
                            email.From = new System.Net.Mail.MailAddress(session.Email, session.User);
                        }
                        else
                        {
                            var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                            var profile = user != null ? user.tblUserProfiles.FirstOrDefault() : null;
                            email.From = user != null ? new System.Net.Mail.MailAddress(user.aspnet_Membership.Email, profile.firstName + " " + profile.lastName) : new System.Net.Mail.MailAddress("eplat@villagroup.com", "ePlat Administration");
                        }
                    }
                    if (notification.copyLead == true)
                    {
                        if (query.tblLeads.tblLeadEmails.Count() > 0)
                        {
                            var _addresses = query.tblLeads.tblLeadEmails.Where(m => m.main == true && m.dnc != true);
                            var mailList = _addresses.Count() > 0 ? _addresses.Select(m => m.email.Replace(" ", "")) : null;
                            if (mailList != null)
                            {
                                email.To.Add(string.Join(",", mailList));
                            }
                        }
                    }
                    if (notification.copySender == true)
                    {
                        var str = "";
                        if (HttpContext.Current.User.Identity.IsAuthenticated)
                        {
                            str = session.Email;
                        }
                        else
                        {
                            var user = db.aspnet_Users.FirstOrDefault(m => m.UserId == query.tblLeads.assignedToUserID);
                            str = user.aspnet_Membership.Email;
                        }
                        email.Bcc.Add(str);
                    }
                    email.ReplyToList.Add(notification.replyTo ?? email.From.Address);

                    preview = pdm.ReplaceFieldsValues(email, "en-US", _list, fieldGroup.fieldGroupID);
                    preview.SysEvent = notification.eventID.ToString();
                    preview.LeadID = query.leadID.ToString();
                    preview.ReservationID = query.reservationID.ToString();
                    return preview;
                }
            }
            catch (Exception ex)
            {
                var model = new PaymentConfirmationModel();
                model.Status = ex.Message + "<br />This email has no available preview. Please contact System Administrator";
                return model;
            }
        }

        public List<AvailableLettersModel> GetAvailableLetters(Guid reservationID)
        {
            return GetAvailableLetters(reservationID, 0);
        }

        public List<AvailableLettersModel> GetAvailableLetters(Guid reservationID, int eventID)
        {
            return GetAvailableLetters(reservationID, eventID, null);
        }

        public List<AvailableLettersModel> _GetAvailableLetters(Guid reservationID, int eventID, List<long?> listTerminals)
        {
            ePlatEntities db = new ePlatEntities();
            List<AvailableLettersModel> list = new List<AvailableLettersModel>();
            var terminals = listTerminals ?? session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToList();
            var reservation = db.tblReservations.Where(m => m.reservationID == reservationID).ToList().FirstOrDefault();
            //var query = db.tblEmailNotifications.Where(m => terminals.Contains(m.terminalID) && m.active == true && (eventID == 0 || m.eventID == eventID)).ToList();
            var query = db.tblEmailNotifications.Where(m => terminals.Contains(m.terminalID) && m.active == true && (eventID == 0 || m.eventID == eventID) && m.tblEmailNotifications_Destinations.Count(x => x.destinationID == reservation.destinationID) > 0).ToList();
            var rsvID = reservationID.ToString();

            List<tblEmailNotifications> newQuery = query.Where(m => m.eventID != 10 && m.eventID != 11).ToList();

            #region "terminal 10"
            //if (terminals.Contains(10))
            if (terminals.Contains(9999))
            {
                var options = PublicDataModel.GetResortConnectOptionals(reservation);
                if (options.Count() > 0)
                {
                    foreach (var i in query)
                    {
                        if (i.eventID == 10)
                        {
                            if (options.Where(m => m.ProductName.ToLower().Contains("one way") || m.ProductName.ToLower().Contains("arrival") || m.ProductName.ToLower().Contains("airport-resort")).Count() > 0)
                            {
                                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One"))).ToList();
                            }
                        }
                        else if (i.eventID == 44)
                        {
                            if (options.Count(m => m.ProductName.ToLower().Contains("departure")) > 0)
                            {
                                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return"))).ToList();
                            }
                        }
                        else
                        {
                            newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
                        }

                        if (i.requiredFields != null && i.requiredFields != "")
                        {
                            var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
                            var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

                            if (!allowed)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_BookingStatus.Count() > 0)
                        {
                            if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_SecondaryBookingStatus.Count() > 0)
                        {
                            if (i.tblEmailNotifications_SecondaryBookingStatus.Count(m => m.secondaryBookingStatusID == reservation.tblLeads.secondaryBookingStatusID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_Destinations.Count() > 0)
                        {
                            if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_LeadSources.Count() > 0)
                        {
                            if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_LeadStatus.Count() > 0)
                        {
                            if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_Places.Count() > 0)
                        {
                            if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_Roles.Count() > 0)
                        {
                            if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }
                    }
                }
                else
                {
                    foreach (var i in query)
                    {
                        #region
                        //ver cómo consultar opciones vendidas para filtrar notificaciones de transportacion
                        if (i.eventID == 9)//Activity Coupon Email
                        {
                            if (reservation.tblOptionsSold.Count() > 0)
                            {
                                if (reservation.tblOptionsSold.Count(m => !m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
                                {
                                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
                                }
                            }
                        }
                        else if (i.eventID == 10)//Transportation Coupon Email
                        {
                            if (reservation.tblOptionsSold.Count() > 0)
                            {
                                if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
                                {
                                    if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("Return") || m.tblOptions.optionName.Contains("Departure")) > 0)
                                    {
                                        newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return"))).ToList();
                                    }
                                    if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("One") || m.tblOptions.optionName.Contains("rrival")) > 0)
                                    {
                                        newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One"))).ToList();
                                    }

                                }
                            }
                        }
                        else if (i.eventID == 11)//Two Way Transportation Coupon
                        {
                            if (reservation.tblOptionsSold.Count() > 0)
                            {
                                if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation") && m.tblOptions.optionName.Contains("Round")) > 0 || reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.ToLower().Contains("rt ")) > 0)
                                {
                                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
                                }
                            }
                        }

                        if (i.requiredFields != null && i.requiredFields != "")
                        {
                            var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
                            var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

                            if (!allowed)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_BookingStatus.Count() > 0)
                        {
                            if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_SecondaryBookingStatus.Count() > 0)
                        {
                            if (i.tblEmailNotifications_SecondaryBookingStatus.Count(m => m.secondaryBookingStatusID == reservation.tblLeads.secondaryBookingStatusID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_Destinations.Count() > 0)
                        {
                            if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_LeadSources.Count() > 0)
                        {
                            if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_LeadStatus.Count() > 0)
                        {
                            if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_Places.Count() > 0)
                        {
                            if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        if (i.tblEmailNotifications_Roles.Count() > 0)
                        {
                            if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
                            {
                                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                            }
                        }

                        #endregion
                    }
                }
            }
            else
            #endregion
            {
                foreach (var i in query)
                {
                    #region
                    //ver cómo consultar opciones vendidas para filtrar notificaciones de transportacion
                    if (i.eventID == 9)//Activity Coupon Email
                    {
                        if (reservation.tblOptionsSold.Count() > 0)
                        {
                            if (reservation.tblOptionsSold.Count(m => !m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
                            {
                                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
                            }
                        }
                    }
                    else if (i.eventID == 10)//Transportation Coupon Email
                    {
                        if (reservation.tblOptionsSold.Count() > 0)
                        {
                            if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
                            {
                                if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("Return") || m.tblOptions.optionName.Contains("Departure")) > 0)
                                {
                                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return"))).ToList();
                                }
                                if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("One") || m.tblOptions.optionName.Contains("rrival")) > 0)
                                {
                                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One"))).ToList();
                                }

                            }
                        }
                    }
                    else if (i.eventID == 11)//Two Way Transportation Coupon
                    {
                        if (reservation.tblOptionsSold.Count() > 0)
                        {
                            if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation") && m.tblOptions.optionName.Contains("Round")) > 0 || reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.ToLower().Contains("rt ")) > 0)
                            {
                                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
                            }
                        }
                    }

                    if (i.requiredFields != null && i.requiredFields != "")
                    {
                        var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
                        var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

                        if (!allowed)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_BookingStatus.Count() > 0)
                    {
                        if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_SecondaryBookingStatus.Count() > 0)
                    {
                        if (i.tblEmailNotifications_SecondaryBookingStatus.Count(m => m.secondaryBookingStatusID == reservation.tblLeads.secondaryBookingStatusID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_Destinations.Count() > 0)
                    {
                        if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_LeadSources.Count() > 0)
                    {
                        if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_LeadStatus.Count() > 0)
                    {
                        if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_Places.Count() > 0)
                    {
                        if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    if (i.tblEmailNotifications_Roles.Count() > 0)
                    {
                        if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
                        {
                            newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
                        }
                    }

                    #endregion
                }
            }

            foreach (var i in newQuery.Distinct())
            {
                if (i.tblFieldGroups.Count() > 0)
                {
                    var fieldID = i.tblFieldGroups.FirstOrDefault().tblFields.Where(m => m.field == "$ItemID").Count() > 0 ? i.tblFieldGroups.FirstOrDefault().tblFields.Where(m => m.field == "$ItemID").FirstOrDefault().fieldID : 0;
                    var items = db.tblFieldsValues.Where(m => m.fieldID == fieldID && m.value == rsvID);
                    if (items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            var transaction = db.tblFieldsValues.Where(m => m.transactionID == item.transactionID);

                            list.Add(new AvailableLettersModel()
                            {
                                Transaction = item.transactionID.ToString(),
                                ID = i.emailNotificationID,
                                EmailID = i.emailID,
                                Subject = i.tblEmails.subject,
                                //Description = i.tblEmails.description,
                                Description = i.description,
                                Sent = transaction != null ? true : false,
                                DateSent = transaction.Count(m => m.tblFields.field == "$Sent") > 0 && transaction.FirstOrDefault(m => m.tblFields.field == "$Sent").value != null ? transaction.FirstOrDefault(m => m.tblFields.field == "$Sent").value.Replace("a.m.", "AM").Replace("p.m.", "PM") : "",
                                DateRead = transaction.Count(m => m.tblFields.field == "$Open") > 0 && transaction.FirstOrDefault(m => m.tblFields.field == "$Open").value != null ? transaction.FirstOrDefault(m => m.tblFields.field == "$Open").value.Replace("a.m.", "AM").Replace("p.m.", "PM") : "",
                                DateSigned = transaction.Count(m => m.tblFields.field == "$Signature" && m.value != null) > 0 ? transaction.FirstOrDefault(m => m.tblFields.field == "$Signature" && m.value != null).dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("a.m.", "AM").Replace("p.m.", "PM") : "",
                                Read = transaction != null ? transaction.Count(m => m.tblFields.field == "$Open" && m.value != null) > 0 ? true : false : false,
                                Signed = transaction != null ? transaction.Count(m => m.tblFields.field == "$Signature" && m.value != null) > 0 ? true : false : false,
                                EventID = i.eventID
                            });
                        }
                    }
                    list.Add(new AvailableLettersModel()
                    {
                        Transaction = "",
                        ID = i.emailNotificationID,
                        EmailID = i.emailID,
                        Subject = i.tblEmails.subject,
                        //Description = i.tblEmails.description,
                        Description = i.description,
                        Sent = false,
                        DateSent = "",
                        DateRead = "",
                        DateSigned = "",
                        Read = false,
                        Signed = false
                    });
                }
            }
            list = list.Count() > 0 ? list.Where(m => m.DateSent != "").OrderBy(m => DateTime.Parse(m.DateSent)).Concat(list.Where(m => m.DateSent == "")).ToList() : new List<AvailableLettersModel>();
            return list;
        }

        public List<AvailableLettersModel> GetAvailableLetters(Guid reservationID, int eventID, List<long?> listTerminals)
        {
            ePlatEntities db = new ePlatEntities();
            List<AvailableLettersModel> list = new List<AvailableLettersModel>();
            var terminals = listTerminals ?? session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToList();
            var reservation = db.tblReservations.Where(m => m.reservationID == reservationID).ToList().FirstOrDefault();
            var lead = reservation.tblLeads;
            var bookingStatus = lead.bookingStatusID;
            var leadSource = lead.leadSourceID;
            var leadStatus = lead.leadStatusID;
            var optionsSold = reservation.tblOptionsSold;
            List<tblEmailNotifications> notifications;

            notifications = (from n in db.tblEmailNotifications
                             join d in db.tblEmailNotifications_Destinations on n.emailNotificationID equals d.emailNotificationID into nd
                             from d in nd.DefaultIfEmpty()
                             join p in db.tblEmailNotifications_Places on n.emailNotificationID equals p.emailNotificationID into np
                             from p in np.DefaultIfEmpty()
                             join bs in db.tblEmailNotifications_BookingStatus on n.emailNotificationID equals bs.emailNotificationID into nbs
                             from bs in nbs.DefaultIfEmpty()
                             join ls in db.tblEmailNotifications_LeadSources on n.emailNotificationID equals ls.emailNotificationID into nls
                             from ls in nls.DefaultIfEmpty()
                             join lss in db.tblEmailNotifications_LeadStatus on n.emailNotificationID equals lss.emailNotificationID into nlss
                             from lss in nlss.DefaultIfEmpty()
                             where terminals.Contains(n.terminalID)
                             && (eventID == 0 || n.eventID == eventID)
                             && (n.tblEmailNotifications_Destinations.Count() == 0 || d.destinationID == reservation.destinationID)
                             && (n.tblEmailNotifications_Places.Count() == 0 || p.placeID == reservation.placeID)
                             && (n.tblEmailNotifications_BookingStatus.Count() == 0 || bs.bookingStatusID == bookingStatus)
                             && (n.tblEmailNotifications_LeadSources.Count() == 0 || ls.leadSourceID == leadSource)
                             && (n.tblEmailNotifications_LeadStatus.Count() == 0 || lss.leadStatusID == leadStatus)
                             && n.active == true
                             select n).ToList();

            if (optionsSold.Count(m => m.deleted != true) > 0)
            {
                var events = db.tblOptionTypes.Where(m => terminals.Contains(m.terminalID)).Select(m => m.eventID).ToList();
                var eventsInUse = optionsSold.Select(m => m.tblOptionTypes.eventID).Distinct().ToList();
                events = events.Where(m => !eventsInUse.Contains(m)).ToList();

                notifications = (from n in notifications
                                 where !events.Contains(n.eventID)
                                 select n).ToList();
            }

            var rsvID = reservationID.ToString();

            #region "terminal 10"
            //if (terminals.Contains(9999))
            //{
            //    var options = PublicDataModel.GetResortConnectOptionals(reservation);
            //    if (options.Count() > 0)
            //    {
            //        foreach (var i in query)
            //        {
            //            if (i.eventID == 10)
            //            {
            //                if (options.Where(m => m.ProductName.ToLower().Contains("one way") || m.ProductName.ToLower().Contains("arrival") || m.ProductName.ToLower().Contains("airport-resort")).Count() > 0)
            //                {
            //                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One"))).ToList();
            //                }
            //            }
            //            else if (i.eventID == 44)
            //            {
            //                if (options.Count(m => m.ProductName.ToLower().Contains("departure")) > 0)
            //                {
            //                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return"))).ToList();
            //                }
            //            }
            //            else
            //            {
            //                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
            //            }

            //            if (i.requiredFields != null && i.requiredFields != "")
            //            {
            //                var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
            //                var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

            //                if (!allowed)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_BookingStatus.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_SecondaryBookingStatus.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_SecondaryBookingStatus.Count(m => m.secondaryBookingStatusID == reservation.tblLeads.secondaryBookingStatusID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_Destinations.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_LeadSources.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_LeadStatus.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_Places.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_Roles.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        foreach (var i in query)
            //        {
            //            #region
            //            //ver cómo consultar opciones vendidas para filtrar notificaciones de transportacion
            //            if (i.eventID == 9)//Activity Coupon Email
            //            {
            //                if (reservation.tblOptionsSold.Count() > 0)
            //                {
            //                    if (reservation.tblOptionsSold.Count(m => !m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
            //                    {
            //                        newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
            //                    }
            //                }
            //            }
            //            else if (i.eventID == 10)//Transportation Coupon Email
            //            {
            //                if (reservation.tblOptionsSold.Count() > 0)
            //                {
            //                    if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
            //                    {
            //                        if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("Return") || m.tblOptions.optionName.Contains("Departure")) > 0)
            //                        {
            //                            newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return"))).ToList();
            //                        }
            //                        if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("One") || m.tblOptions.optionName.Contains("rrival")) > 0)
            //                        {
            //                            newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One"))).ToList();
            //                        }

            //                    }
            //                }
            //            }
            //            else if (i.eventID == 11)//Two Way Transportation Coupon
            //            {
            //                if (reservation.tblOptionsSold.Count() > 0)
            //                {
            //                    if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation") && m.tblOptions.optionName.Contains("Round")) > 0 || reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.ToLower().Contains("rt ")) > 0)
            //                    {
            //                        newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
            //                    }
            //                }
            //            }

            //            if (i.requiredFields != null && i.requiredFields != "")
            //            {
            //                var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
            //                var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

            //                if (!allowed)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_BookingStatus.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_SecondaryBookingStatus.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_SecondaryBookingStatus.Count(m => m.secondaryBookingStatusID == reservation.tblLeads.secondaryBookingStatusID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_Destinations.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_LeadSources.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_LeadStatus.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_Places.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            if (i.tblEmailNotifications_Roles.Count() > 0)
            //            {
            //                if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
            //                {
            //                    newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //                }
            //            }

            //            #endregion
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var i in query)
            //    {
            //        #region
            //        //ver cómo consultar opciones vendidas para filtrar notificaciones de transportacion
            //        if (i.eventID == 9)//Activity Coupon Email
            //        {
            //            if (reservation.tblOptionsSold.Count() > 0)
            //            {
            //                if (reservation.tblOptionsSold.Count(m => !m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
            //                {
            //                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
            //                }
            //            }
            //        }
            //        else if (i.eventID == 10)//Transportation Coupon Email
            //        {
            //            if (reservation.tblOptionsSold.Count() > 0)
            //            {
            //                if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
            //                {
            //                    if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("Return") || m.tblOptions.optionName.Contains("Departure")) > 0)
            //                    {
            //                        newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return"))).ToList();
            //                    }
            //                    if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("One") || m.tblOptions.optionName.Contains("rrival")) > 0)
            //                    {
            //                        newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One"))).ToList();
            //                    }

            //                }
            //            }
            //        }
            //        else if (i.eventID == 11)//Two Way Transportation Coupon
            //        {
            //            if (reservation.tblOptionsSold.Count() > 0)
            //            {
            //                if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation") && m.tblOptions.optionName.Contains("Round")) > 0 || reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.ToLower().Contains("rt ")) > 0)
            //                {
            //                    newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID)).ToList();
            //                }
            //            }
            //        }

            //        if (i.requiredFields != null && i.requiredFields != "")
            //        {
            //            var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
            //            var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

            //            if (!allowed)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_BookingStatus.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_SecondaryBookingStatus.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_SecondaryBookingStatus.Count(m => m.secondaryBookingStatusID == reservation.tblLeads.secondaryBookingStatusID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_Destinations.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_LeadSources.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_LeadStatus.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_Places.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        if (i.tblEmailNotifications_Roles.Count() > 0)
            //        {
            //            if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
            //            {
            //                newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID).ToList();
            //            }
            //        }

            //        #endregion
            //    }
            //}

            #endregion

            foreach (var i in notifications.Distinct())
            {
                if (i.tblFieldGroups.Count() > 0)
                {
                    var fields = i.tblFieldGroups.FirstOrDefault().tblFields;
                    var fieldID = fields.Count(m => m.field == "$ItemID") > 0 ? fields.FirstOrDefault(m => m.field == "$ItemID").fieldID : 0;
                    var items = db.tblFieldsValues.Where(m => terminals.Contains(m.terminalID) && m.fieldID == fieldID && m.value == rsvID);
                    if (items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            var transaction = db.tblFieldsValues.Where(m => m.transactionID == item.transactionID);

                            list.Add(new AvailableLettersModel()
                            {
                                Transaction = item.transactionID.ToString(),
                                ID = i.emailNotificationID,
                                EmailID = i.emailID,
                                Subject = i.tblEmails.subject,
                                Description = i.description,
                                Sent = transaction != null ? true : false,
                                DateSent = transaction.Count(m => m.tblFields.field == "$Sent") > 0 && transaction.FirstOrDefault(m => m.tblFields.field == "$Sent").value != null ? transaction.FirstOrDefault(m => m.tblFields.field == "$Sent").value.Replace("a.m.", "AM").Replace("p.m.", "PM") : "",
                                DateRead = transaction.Count(m => m.tblFields.field == "$Open") > 0 && transaction.FirstOrDefault(m => m.tblFields.field == "$Open").value != null ? transaction.FirstOrDefault(m => m.tblFields.field == "$Open").value.Replace("a.m.", "AM").Replace("p.m.", "PM") : "",
                                DateSigned = transaction.Count(m => m.tblFields.field == "$Signature" && m.value != null) > 0 ? transaction.FirstOrDefault(m => m.tblFields.field == "$Signature" && m.value != null).dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt").Replace("a.m.", "AM").Replace("p.m.", "PM") : "",
                                Read = transaction != null ? transaction.Count(m => m.tblFields.field == "$Open" && m.value != null) > 0 ? true : false : false,
                                Signed = transaction != null ? transaction.Count(m => m.tblFields.field == "$Signature" && m.value != null) > 0 ? true : false : false,
                                EventID = i.eventID
                            });
                        }
                    }
                    list.Add(new AvailableLettersModel()
                    {
                        Transaction = "",
                        ID = i.emailNotificationID,
                        EmailID = i.emailID,
                        Subject = i.tblEmails.subject,
                        Description = i.description,
                        Sent = false,
                        DateSent = "",
                        DateRead = "",
                        DateSigned = "",
                        Read = false,
                        Signed = false
                    });
                }
            }
            list = list.Count() > 0 ? list.Where(m => m.DateSent != "").OrderBy(m => DateTime.Parse(m.DateSent)).Concat(list.Where(m => m.DateSent == "")).ToList() : new List<AvailableLettersModel>();
            return list;
        }

        public AttemptResponse ResetAppVar()
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            System.Web.HttpContext.Current.Application.Lock();

            var query = (from ms in db.tblMailingSettings
                         where ms.userID == null
                         && ms.terminalID == null
                         && ms.popUsername.IndexOf("noreply") != -1
                         && ms.active != false
                         select ms).ToList();
            var first = query.OrderBy(m => m.dailyCounter).FirstOrDefault();
            System.Web.HttpContext.Current.Application["senderAccount"] = first.mailingSettingsID;
            db.SaveChanges();
            System.Web.HttpContext.Current.Application.UnLock();
            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "Variable Successfully Reseted";
            response.ObjectID = 0;
            return response;
        }

        public List<AvailableLettersModel> _GetAvailableLetters(Guid reservationID)
        {
            ePlatEntities db = new ePlatEntities();
            List<AvailableLettersModel> list = new List<AvailableLettersModel>();

            var terminals = session.Terminals.Split(',').Select(m => (long?)long.Parse(m)).ToArray();

            var reservation = db.tblReservations.Single(m => m.reservationID == reservationID);
            var query = db.tblEmailNotifications.Where(m => terminals.Contains(m.terminalID) && m.active == true);
            var rsvID = reservationID.ToString();
            IQueryable<tblEmailNotifications> newQuery = query.Where(m => m.eventID != 10 && m.eventID != 11);
            foreach (var i in query)
            {
                #region
                //ver cómo consultar opciones vendidas para filtrar notificaciones de transportacion
                if (i.eventID == 9)//Activity Coupon Email
                {
                    if (reservation.tblOptionsSold.Count() > 0)
                    {
                        if (reservation.tblOptionsSold.Count(m => !m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
                        {
                            newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID));
                        }
                    }
                }
                else if (i.eventID == 10)//Transportation Coupon Email
                {
                    if (reservation.tblOptionsSold.Count() > 0)
                    {
                        if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation")) > 0)
                        {
                            if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("One")) > 0)
                            {
                                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("One")));
                            }
                            else if (reservation.tblOptionsSold.Count(m => m.tblOptions.optionName.Contains("Return")) > 0)
                            {
                                newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID && m.tblFieldGroups.FirstOrDefault().description.Contains("Return")));
                            }
                        }
                    }
                }
                else if (i.eventID == 11)//Two Way Transportation Coupon
                {
                    if (reservation.tblOptionsSold.Count() > 0)
                    {
                        if (reservation.tblOptionsSold.Count(m => m.tblOptions.tblOptionTypes.optionType.Contains("Transportation") && m.tblOptions.optionName.Contains("Round")) > 0)
                        {
                            newQuery = newQuery.Concat(query.Where(m => m.emailNotificationID == i.emailNotificationID));
                        }
                    }
                }
                if (i.requiredFields != null && i.requiredFields != "")
                {
                    var requiredFields = GeneralFunctions.RequiredFields.Where(m => i.requiredFields.Split(',').ToArray().Contains(m.Key)).Select(m => m.Value).ToList();
                    var allowed = EmailsDataModel.EmailsCatalogs.CheckRequiredFields(reservation, requiredFields);

                    if (!allowed)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }

                if (i.tblEmailNotifications_BookingStatus.Count() > 0)
                {
                    if (i.tblEmailNotifications_BookingStatus.Count(m => m.bookingStatusID == reservation.tblLeads.bookingStatusID) == 0)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }
                if (i.tblEmailNotifications_Destinations.Count() > 0)
                {
                    if (i.tblEmailNotifications_Destinations.Count(m => m.destinationID == reservation.destinationID) == 0)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }
                if (i.tblEmailNotifications_LeadSources.Count() > 0)
                {
                    if (i.tblEmailNotifications_LeadSources.Count(m => m.leadSourceID == reservation.tblLeads.leadSourceID) == 0)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }
                if (i.tblEmailNotifications_LeadStatus.Count() > 0)
                {
                    if (i.tblEmailNotifications_LeadStatus.Count(m => m.leadStatusID == reservation.tblLeads.leadStatusID) == 0)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }
                if (i.tblEmailNotifications_Places.Count() > 0)
                {
                    if (i.tblEmailNotifications_Places.Count(m => m.placeID == reservation.placeID) == 0)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }
                if (i.tblEmailNotifications_Roles.Count() > 0)
                {
                    if (i.tblEmailNotifications_Roles.Count(m => m.roleID == session.RoleID) == 0)
                    {
                        newQuery = newQuery.Where(m => m.emailNotificationID != i.emailNotificationID);
                    }
                }

                #endregion
            }

            foreach (var i in newQuery.Distinct())
            {
                if (i.tblFieldGroups.Count() > 0)
                {
                    var fieldID = i.tblFieldGroups.FirstOrDefault().tblFields.Where(m => m.field == "$ItemID").Count() > 0 ? i.tblFieldGroups.FirstOrDefault().tblFields.Where(m => m.field == "$ItemID").FirstOrDefault().fieldID : 0;
                    var items = db.tblFieldsValues.Where(m => m.fieldID == fieldID && m.value == rsvID);
                    if (items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            var transaction = db.tblFieldsValues.Where(m => m.transactionID == item.transactionID);
                            list.Add(new AvailableLettersModel()
                            {
                                Transaction = item.transactionID.ToString(),
                                ID = i.emailNotificationID,
                                EmailID = i.emailID,
                                Subject = i.tblEmails.subject,
                                Description = i.tblEmails.description,
                                Sent = transaction != null ? true : false,
                                DateSent = transaction.Count(m => m.tblFields.field == "$Sent") > 0 ? transaction.FirstOrDefault(m => m.tblFields.field == "$Sent").value : "",
                                DateRead = transaction.Count(m => m.tblFields.field == "$Open") > 0 ? transaction.FirstOrDefault(m => m.tblFields.field == "$Open").value : "",
                                DateSigned = transaction.Count(m => m.tblFields.field == "$Signature" && m.value != null) > 0 ? transaction.FirstOrDefault(m => m.tblFields.field == "$Signature" && m.value != null).dateSaved.ToString("yyyy-MM-dd hh:mm:ss tt") : "",
                                Read = transaction != null ? transaction.Count(m => m.tblFields.field == "$Open" && m.value != null) > 0 ? true : false : false,
                                Signed = transaction != null ? transaction.Count(m => m.tblFields.field == "$Signature" && m.value != null) > 0 ? true : false : false,
                                EventID = i.eventID
                            });
                        }
                    }
                    list.Add(new AvailableLettersModel()
                    {
                        Transaction = "",
                        ID = i.emailNotificationID,
                        EmailID = i.emailID,
                        Subject = i.tblEmails.subject,
                        Description = i.tblEmails.description,
                        Sent = false,
                        DateSent = "",
                        DateRead = "",
                        DateSigned = "",
                        Read = false,
                        Signed = false
                    });
                }
            }
            list = list.Count() > 0 ? list.Where(m => m.DateSent != "").OrderBy(m => DateTime.Parse(m.DateSent)).Concat(list.Where(m => m.DateSent == "")).ToList() : new List<AvailableLettersModel>();
            return list;
        }

        public List<SelectListItem> GetDDLData(string itemType)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            switch (itemType)
            {
                case "callClasificationID":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpCallClasifications();
                        break;
                    }
                case "bookingStatusID":
                    {
                        list = BookingStatusDataModel.GetBookingStatusByCurrentWorkGroup();
                        break;
                    }
                case "secondaryBookingStatusID":
                    {
                        list = BookingStatusDataModel.GetSecondaryBookingStatus();
                        break;
                    }
                case "finalBookingStatusID":
                    {
                        list = BookingStatusDataModel.GetFinalBookingStatus();
                        break;
                    }
                case "leadSourceID":
                    {
                        list = LeadSourceDataModel.GetLeadSourcesByTerminal();
                        break;
                    }
                case "destinationID":
                    {
                        //list = CatalogsDataModel.Destinations.DestinationsCatalogs.FillDrpDestinationsPerCurrentTerminals();
                        list = PreArrivalCatalogs.GetDestinationsAllowed();
                        break;
                    }
                case "reservationStatusID":
                    {
                        list = ReservationStatusDataModel.GetallReservationStatus();
                        break;
                    }
                case "terminalID":
                    {
                        list = TerminalDataModel.GetCurrentUserTerminals();
                        break;
                    }
                case "assignedToUserID":
                case "inputByUserID":
                case "salesAgentUserID":
                case "modifiedByUserID":
                    {
                        if (GeneralFunctions.IsUserInRole("Administrator", (Guid)Membership.GetUser().ProviderUserKey))
                        {
                            list = UserDataModel.UserCatalogs.FillDrpUsersInWorkGroup();
                        }
                        else
                        {
                            list = UserDataModel.GetUsersBySupervisor(session.UserID);
                        }
                        //list = UserDataModel.GetUsersBySupervisor(session.UserID, false, false, true);
                        break;
                    }
                case "planTypeID":
                    {
                        list = ReportDataModel.ReportsCatalogs.FillDrpPlanTypes();
                        break;
                    }
                case "qualificationStatusID":
                    {
                        list = ReportDataModel.ReportsCatalogs.FillDrpQualificationStatus();
                        break;
                    }
                case "tourStatusID":
                case "finalTourStatusID":
                    {
                        //list = TourStatusDataModel.GetAlltourStatus();
                        list = TourStatusDataModel.GetTourStatusByCurrentWorkGroup();
                        break;
                    }
                case "placeID":
                    {
                        list = PlaceDataModel.GetResortsByProfile();
                        break;
                    }
                case "roomTypeID":
                    {
                        list = PlaceDataModel.GetAllRoomTypes();
                        break;
                    }
                case "pointOfSaleID":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpPointsOfSale();
                        break;
                    }
                case "opcID":
                    {
                        list = OpcDataModel.FillDrpOPCsPerCompany(null, null);
                        break;
                    }
                case "countryID":
                    {
                        list = CountryDataModel.GetAllCountries();
                        break;
                    }
                case "greetingRepID":
                    {
                        list = GreetingRepDataModel.GetAllGreetingReps();
                        break;
                    }
                case "chargeTypeID":
                    {
                        list = PreArrivalDataModel.PreArrivalCatalogs.GetAllChargeTypes();
                        break;
                    }
                case "chargeDescriptionID":
                    {
                        list = PreArrivalDataModel.PreArrivalCatalogs.GetAllChargeDescriptions();
                        break;
                    }
                case "leadStatusID":
                    {
                        list = LeadStatusDataModel.GetLeadStatusByTerminal();
                        break;
                    }
                case "airlines":
                    {
                        list = MasterChartDataModel.LeadsCatalogs.FillDrpAirlines();
                        break;
                    }
            }
            return list;
        }

    }
}
