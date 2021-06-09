
using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.Data.Objects.SqlClient;

namespace ePlatBack.Models.DataModels
{
    public class PlaceDataModel
    {
        public static UserSession session = new UserSession();
        public class PlacesCatalogs
        {
            public static List<SelectListItem> FillDrpTransportationZones(int id)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (id != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    foreach (var i in (from a in db.tblTransportationZones where a.destinationID == id select a))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.transportationZoneID.ToString(),
                            Text = i.transportationZone.ToString()
                        });
                    }
                }
                else
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Destination--" });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpDestinations()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> d = new List<SelectListItem>();
                d.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblDestinations)
                {
                    d.Add(new SelectListItem()
                    {
                        Value = i.destinationID.ToString(),
                        Text = i.destination.ToString(),
                    });
                }
                return d;
            }

            public static List<SelectListItem> FillDrpCultures()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> c = new List<SelectListItem>();
                c.Add(new SelectListItem() { Value = "", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblLanguages)
                {
                    c.Add(new SelectListItem()
                    {
                        Value = i.culture.ToString(),
                        Text = i.language.ToString(),
                    });
                }
                return c;
            }

            public static List<SelectListItem> FillDrpPlaceClasifications(int id)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (id != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    foreach (var i in (from a in db.tblPlaceClasifications where a.placeTypeID == id select a))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.placeClasificationID.ToString(),
                            Text = i.placeClasification.ToString()
                        });
                    }
                }
                else
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Place Type--" });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpPlaceTypes()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();

                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPlaceTypes.Where(m => m.active == true).Select(m => m))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.placeTypeID.ToString(),
                        Text = i.placeType.ToString()
                    });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpZones(int id)
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                if (id != 0)
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                    foreach (var i in (from a in db.tblZones where a.destinationID == id select a))
                    {
                        list.Add(new SelectListItem()
                        {
                            Value = i.zoneID.ToString(),
                            Text = i.zone.ToString()
                        });
                    }
                }
                else
                {
                    list.Add(new SelectListItem() { Value = "0", Text = "--Select Destination--" });
                }
                return list;
            }

            public static List<SelectListItem> FillDrpTerminals()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> t = new List<SelectListItem>();
                t.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in TerminalDataModel.GetCurrentUserTerminals())
                {
                    t.Add(new SelectListItem()
                    {
                        Value = i.Value,
                        Text = i.Text
                    });
                }
                return t;
            }

            public static List<SelectListItem> FillDrpPlanTypes()
            {
                ePlatEntities db = new ePlatEntities();
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = "0", Text = "--Select One--", Selected = false });
                foreach (var i in db.tblPlanTypes)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = i.planTypeID.ToString(),
                        Text = i.planType
                    });
                }
                return list;
            }
        }

        public List<PlacesSearchResultsModel> SearchPlacesResults(PlacesSearchModel model)
        {
            ePlatEntities db = new ePlatEntities();
            List<PlacesSearchResultsModel> list = new List<PlacesSearchResultsModel>();
            List<Int64> placesPerTerminal = new List<Int64>();
            if (model.Search_Terminals != "")
            {
                var terminals = model.Search_Terminals.Split(',');
                foreach (var i in terminals)
                    placesPerTerminal.Add(Int64.Parse(i));
            }
            var queryTerminals = (from a in db.tblPlaces_Terminals
                                  where placesPerTerminal.Contains(a.terminalID)
                                  select a.placeID).Distinct().ToArray();

            var query = from places in db.tblPlaces
                        where (places.place.Contains(model.Search_Place) || model.Search_Place == null)
                        && (places.destinationID == model.Search_Destination || model.Search_Destination == 0)
                        && (places.placeTypeID == model.Search_PlaceType || model.Search_PlaceType == 0)
                        //&& queryTerminals.Contains(places.placeID)
                        select new
                        {
                            placeID = places.placeID,
                            place = places.place,
                            placeLabel = places.placeLabel,
                            destination = places.tblDestinations.destination,
                            zone = places.tblZones.zone,
                            placeType = places.tblPlaceTypes.placeType
                        };
            foreach (var i in query)
            {
                list.Add(new PlacesSearchResultsModel()
                {
                    PlaceID = int.Parse(i.placeID.ToString()),
                    Place = i.place,
                    PlaceLabel = i.placeLabel,
                    Destination = i.destination,
                    Zone = i.zone,
                    PlaceType = i.placeType
                });
            }
            return list;
        }

        public AttemptResponse SavePlace(PlaceInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PlaceInfo_PlaceID != 0 && model.PlaceInfo_PlaceID != null)
            {
                try
                {
                    var query = from terminal in db.tblPlaces_Terminals
                                where terminal.placeID == model.PlaceInfo_PlaceID
                                select terminal;
                    foreach (var i in query)
                        db.tblPlaces_Terminals.DeleteObject(i);
                    if (model.PlaceInfo_Terminal != null)
                    {
                        var terminals = model.PlaceInfo_Terminal.Split(',');
                        foreach (var i in terminals)
                        {
                            tblPlaces_Terminals terminal = new tblPlaces_Terminals();
                            terminal.placeID = Int64.Parse(model.PlaceInfo_PlaceID.ToString());
                            terminal.terminalID = Int64.Parse(i);
                            db.tblPlaces_Terminals.AddObject(terminal);
                        }
                    }
                    tblPlaces place = db.tblPlaces.Single(m => m.placeID == model.PlaceInfo_PlaceID);
                    place.place = model.PlaceInfo_Place;
                    place.placeLabel = model.PlaceInfo_PlaceLabel;
                    place.address = model.PlaceInfo_Address;
                    place.phone = model.PlaceInfo_Phone;
                    place.lat = model.PlaceInfo_Latitude;
                    place.lng = model.PlaceInfo_Longitude;
                    place.destinationID = int.Parse(model.PlaceInfo_Destination.ToString());
                    place.zoneID = int.Parse(model.PlaceInfo_Zone.ToString());
                    place.vg = model.PlaceInfo_IsVillaGroup;
                    place.placeTypeID = int.Parse(model.PlaceInfo_PlaceType.ToString());
                    if (model.PlaceInfo_PlaceClasification != 0)
                        place.placeClasificationID = int.Parse(model.PlaceInfo_PlaceClasification.ToString());
                    place.prospectation = model.PlaceInfo_Prospectation;
                    place.forSale = model.PlaceInfo_IsForSale;
                    place.checkInTime = model.PlaceInfo_CheckInTime;
                    place.checkOutTime = model.PlaceInfo_CheckOutTime;
                    place.taxesPercentage = model.PlaceInfo_TaxesPercentage;
                    if (model.PlaceInfo_TransportationZone != 0)
                        place.transportationZoneID = int.Parse(model.PlaceInfo_TransportationZone.ToString());
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = place.placeID;
                    response.Message = "Place Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place NOT Updated";
                    return response;
                }
            }
            else
            {
                if (db.tblPlaces.Where(m => m.place.ToLower() == model.PlaceInfo_Place.ToLower() && m.destinationID == model.PlaceInfo_Destination).Count() > 0)
                {
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.ObjectID = 0;
                    response.Message = "A Place With Same Name Already Exists";
                    return response;
                }
                else
                {
                    try
                    {
                        tblPlaces place = new tblPlaces();
                        place.place = model.PlaceInfo_Place;
                        place.placeLabel = model.PlaceInfo_PlaceLabel;
                        place.address = model.PlaceInfo_Address;
                        place.phone = model.PlaceInfo_Phone;
                        place.lat = model.PlaceInfo_Latitude;
                        place.lng = model.PlaceInfo_Longitude;
                        place.destinationID = int.Parse(model.PlaceInfo_Destination.ToString());
                        place.zoneID = int.Parse(model.PlaceInfo_Zone.ToString());
                        place.vg = model.PlaceInfo_IsVillaGroup;
                        if (model.PlaceInfo_PlaceType != 0)
                            place.placeTypeID = int.Parse(model.PlaceInfo_PlaceType.ToString());
                        if (model.PlaceInfo_PlaceClasification != 0)
                            place.placeClasificationID = int.Parse(model.PlaceInfo_PlaceClasification.ToString());
                        place.prospectation = model.PlaceInfo_Prospectation;
                        place.forSale = model.PlaceInfo_IsForSale;
                        place.checkInTime = model.PlaceInfo_CheckInTime;
                        place.checkOutTime = model.PlaceInfo_CheckOutTime;
                        place.taxesPercentage = model.PlaceInfo_TaxesPercentage;
                        if (model.PlaceInfo_TransportationZone != 0)
                            place.transportationZoneID = int.Parse(model.PlaceInfo_TransportationZone.ToString());
                        db.tblPlaces.AddObject(place);
                        db.SaveChanges();
                        if (model.PlaceInfo_Terminal != null)
                        {
                            var terminals = model.PlaceInfo_Terminal.Split(',');
                            foreach (var i in terminals)
                            {
                                tblPlaces_Terminals terminal = new tblPlaces_Terminals();
                                terminal.terminalID = Int64.Parse(i);
                                terminal.placeID = place.placeID;
                                db.tblPlaces_Terminals.AddObject(terminal);
                            }
                        }
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.ObjectID = place.placeID;
                        response.Message = "Place Saved";
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.ObjectID = 0;
                        response.Exception = ex;
                        response.Message = "Place NOT Saved";
                        return response;
                    }
                }
            }
        }

        public AttemptResponse SavePlaceDescription(PlaceDescriptionModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PlaceDescription_PlaceDescriptionID != 0)
            {
                try
                {
                    tblPlaceDescriptions description = db.tblPlaceDescriptions.Single(m => m.placeDescriptionID == model.PlaceDescription_PlaceDescriptionID);
                    description.culture = model.PlaceDescription_Culture;
                    description.shortDescription = model.PlaceDescription_ShortDescription;
                    description.fullDescription = model.PlaceDescription_FullDescription;
                    description.faq = model.PlaceDescription_FAQ;
                    description.amenities = model.PlaceDescription_Amenities;
                    description.allInclusive = model.PlaceDescription_AllInclusive;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = description.placeDescriptionID;
                    response.Message = "Place Description Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place Description NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPlaceDescriptions description = new tblPlaceDescriptions();
                    description.placeID = model.PlaceDescription_PlaceID;
                    description.culture = model.PlaceDescription_Culture;
                    description.shortDescription = model.PlaceDescription_ShortDescription;
                    description.fullDescription = model.PlaceDescription_FullDescription;
                    description.faq = model.PlaceDescription_FAQ;
                    description.amenities = model.PlaceDescription_Amenities;
                    description.allInclusive = model.PlaceDescription_AllInclusive;
                    db.AddTotblPlaceDescriptions(description);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = description.placeDescriptionID;
                    response.Message = "Place Description Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place Description NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SavePlacePlanType(PlacePlanTypeModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.PlacePlanType_PlacePlanTypeID != 0)
            {
                try
                {
                    var query = db.tblPlaces_PlanTypes.Single(m => m.place_PlanTypeID == model.PlacePlanType_PlacePlanTypeID);
                    query.planTypeID = model.PlacePlanType_PlanType;
                    query.terminalID = model.PlacePlanType_Terminal;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Plan Type Updated";
                    response.ObjectID = query.place_PlanTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Plan Type NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var query = new tblPlaces_PlanTypes();
                    query.placeID = model.PlacePlanType_PlaceID;
                    query.planTypeID = model.PlacePlanType_PlanType;
                    query.terminalID = model.PlacePlanType_Terminal;
                    db.tblPlaces_PlanTypes.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Plan Type Saved";
                    response.ObjectID = query.place_PlanTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Plan Type NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveRoomType(RoomTypeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.RoomTypeInfo_RoomTypeID != 0)
            {
                try
                {
                    var query = db.tblRoomTypes.Single(m => m.roomTypeID == model.RoomTypeInfo_RoomTypeID);
                    query.roomType = model.RoomTypeInfo_RoomType;
                    query.quantity = model.RoomTypeInfo_Quantity;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Room Type Updated";
                    response.ObjectID = query.roomTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Room Tye NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var query = new tblRoomTypes();
                    query.placeID = model.RoomTypeInfo_PlaceID;
                    query.roomType = model.RoomTypeInfo_RoomType;
                    query.quantity = model.RoomTypeInfo_Quantity;
                    query.dateSaved = DateTime.Now;
                    query.savedByUserID = session.UserID;
                    db.tblRoomTypes.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Room Type Saved";
                    response.ObjectID = query.roomTypeID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Room Type NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse SaveRoomTypeDescription(RoomTypeDescriptionModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (model.RoomTypeDescription_RoomTypeDescriptionID != 0)
            {
                try
                {
                    var query = db.tblRoomTypeDescriptions.Single(m => m.roomTypeDescriptionID == model.RoomTypeDescription_RoomTypeDescriptionID);
                    query.roomType = model.RoomTypeDescription_RoomType;
                    query.description = model.RoomTypeDescription_Description;
                    query.culture = model.RoomTypeDescription_Culture;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Room Type Description Updated";
                    response.ObjectID = query.roomTypeDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Room Type Description NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    var query = new tblRoomTypeDescriptions();
                    query.roomTypeID = model.RoomTypeDescription_RoomTypeID;
                    query.roomType = model.RoomTypeDescription_RoomType;
                    query.description = model.RoomTypeDescription_Description;
                    query.culture = model.RoomTypeDescription_Culture;
                    db.tblRoomTypeDescriptions.AddObject(query);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Room Type Description Saved";
                    response.ObjectID = query.roomTypeDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Room Type Description NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public PlaceInfoModel GetPlace(int placeID)
        {
            ePlatEntities db = new ePlatEntities();
            PlaceInfoModel model = new PlaceInfoModel();
            List<KeyValuePair<int, string>> listTerminals = new List<KeyValuePair<int, string>>();
            var query = db.tblPlaces.Single(m => m.placeID == placeID);
            var queryTerminals = db.tblPlaces_Terminals.Where(m => m.placeID == placeID).Select(m => m);
            var pc = 0;
            var tz = 0;
            foreach (var i in queryTerminals)
                listTerminals.Add(new KeyValuePair<int, string>(int.Parse(i.terminalID.ToString()), i.tblTerminals.terminal));
            if (query.placeClasificationID != null)
                pc = int.Parse(query.placeClasificationID.ToString());
            if (query.transportationZoneID != null)
                tz = int.Parse(query.transportationZoneID.ToString());
            model.PlaceInfo_Place = query.place;
            model.PlaceInfo_PlaceLabel = query.placeLabel != null ? query.placeLabel : "";
            model.PlaceInfo_Address = query.address;
            model.PlaceInfo_Phone = query.phone;
            model.PlaceInfo_Latitude = query.lat;
            model.PlaceInfo_Longitude = query.lng;
            model.PlaceInfo_Destination = int.Parse(query.destinationID.ToString());
            model.PlaceInfo_Zone = query.zoneID;
            model.PlaceInfo_IsVillaGroup = query.vg;
            model.PlaceInfo_PlaceType = query.placeTypeID;
            model.PlaceInfo_PlaceClasification = pc;
            model.PlaceInfo_Prospectation = query.prospectation;
            model.PlaceInfo_IsForSale = query.forSale;
            model.PlaceInfo_CheckInTime = query.checkInTime;
            model.PlaceInfo_CheckOutTime = query.checkOutTime;
            model.PlaceInfo_TaxesPercentage = query.taxesPercentage;
            model.PlaceInfo_TransportationZone = tz;
            model.PlaceInfo_ListTerminals = listTerminals;
            return model;
        }

        public List<PlaceDescriptionModel> GetPlaceDescriptions(int id)
        {
            ePlatEntities db = new ePlatEntities();
            List<PlaceDescriptionModel> list = new List<PlaceDescriptionModel>();
            try
            {
                var query = from description in db.tblPlaceDescriptions
                            where description.placeID == id
                            select description;
                foreach (var i in query)
                {
                    list.Add(new PlaceDescriptionModel()
                    {
                        PlaceDescription_PlaceDescriptionID = int.Parse(i.placeDescriptionID.ToString()),
                        PlaceDescription_Culture = i.culture,
                        PlaceDescription_ShortDescription = i.shortDescription,
                        PlaceDescription_FullDescription = i.fullDescription,
                        PlaceDescription_FAQ = i.faq,
                        PlaceDescription_Amenities = i.amenities,
                        PlaceDescription_AllInclusive = i.allInclusive
                    });
                }
            }
            catch
            {
            }
            return list;
        }

        public List<PlacePlanTypeModel> GetPlacePlanTypes(int id)
        {
            ePlatEntities db = new ePlatEntities();
            List<PlacePlanTypeModel> list = new List<PlacePlanTypeModel>();
            try
            {
                var query = from plan in db.tblPlaces_PlanTypes
                            where plan.placeID == id
                            select plan;
                foreach (var i in query)
                {
                    list.Add(new PlacePlanTypeModel()
                    {
                        PlacePlanType_PlacePlanTypeID = int.Parse(i.place_PlanTypeID.ToString()),
                        PlacePlanType_PlaceID = int.Parse(i.placeID.ToString()),
                        PlacePlanType_PlanType = i.planTypeID,
                        PlacePlanType_Terminal = int.Parse(i.terminalID.ToString())
                    });
                }
            }
            catch { }
            return list;
        }

        public static List<SelectListItem> GetRoomTypesByPlace(int placeID)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                var query = from type in db.tblRoomTypes
                            where ((placeID != 0 && type.placeID == placeID) || placeID == 0)
                            select type;
                foreach (var i in query)
                {
                    list.Add(new SelectListItem() { Value = i.roomTypeID.ToString(), Text = i.roomType });
                }
            }
            catch { }
            return list;
        }

        public List<RoomTypeInfoModel> GetRoomTypes(int placeID)
        {
            ePlatEntities db = new ePlatEntities();
            List<RoomTypeInfoModel> list = new List<RoomTypeInfoModel>();
            try
            {
                var query = from type in db.tblRoomTypes
                            where type.placeID == placeID
                            select type;
                foreach (var i in query)
                {
                    list.Add(new RoomTypeInfoModel()
                    {
                        RoomTypeInfo_RoomTypeID = int.Parse(i.roomTypeID.ToString()),
                        RoomTypeInfo_RoomType = i.roomType,
                        RoomTypeInfo_Quantity = i.quantity != null ? int.Parse(i.quantity.ToString()) : 0
                    });
                }
            }
            catch { }
            return list;
        }

        public List<RoomTypeDescriptionModel> GetRoomTypeDescriptions(int roomTypeID)
        {
            ePlatEntities db = new ePlatEntities();
            List<RoomTypeDescriptionModel> list = new List<RoomTypeDescriptionModel>();
            try
            {
                var query = from type in db.tblRoomTypeDescriptions
                            where type.roomTypeID == roomTypeID
                            select type;
                foreach (var i in query)
                {
                    list.Add(new RoomTypeDescriptionModel()
                    {
                        RoomTypeDescription_RoomTypeDescriptionID = int.Parse(i.roomTypeDescriptionID.ToString()),
                        RoomTypeDescription_RoomTypeID = int.Parse(i.roomTypeID.ToString()),
                        RoomTypeDescription_RoomType = i.roomType,
                        RoomTypeDescription_Description = i.description,
                        RoomTypeDescription_Culture = i.culture
                    });
                }
            }
            catch { }
            return list;
        }

        public PlaceDescriptionModel GetPlaceDescription(int placeDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            PlaceDescriptionModel model = new PlaceDescriptionModel();
            var query = db.tblPlaceDescriptions.Single(m => m.placeDescriptionID == placeDescriptionID);
            model.PlaceDescription_Culture = query.culture;
            model.PlaceDescription_ShortDescription = query.shortDescription;
            model.PlaceDescription_FullDescription = query.fullDescription;
            model.PlaceDescription_FAQ = query.faq;
            model.PlaceDescription_Amenities = query.amenities;
            model.PlaceDescription_AllInclusive = query.allInclusive;
            return model;
        }

        public PlacePlanTypeModel GetPlacePlanType(int placePlanTypeID)
        {
            ePlatEntities db = new ePlatEntities();
            PlacePlanTypeModel model = new PlacePlanTypeModel();
            var query = db.tblPlaces_PlanTypes.Single(m => m.place_PlanTypeID == placePlanTypeID);
            model.PlacePlanType_PlacePlanTypeID = int.Parse(query.place_PlanTypeID.ToString());
            model.PlacePlanType_PlaceID = int.Parse(query.placeID.ToString());
            model.PlacePlanType_PlanType = query.planTypeID;
            model.PlacePlanType_Terminal = int.Parse(query.terminalID.ToString());
            return model;
        }

        public RoomTypeInfoModel GetRoomType(int roomTypeID)
        {
            ePlatEntities db = new ePlatEntities();
            RoomTypeInfoModel model = new RoomTypeInfoModel();
            var query = db.tblRoomTypes.Single(m => m.roomTypeID == roomTypeID);
            model.RoomTypeInfo_RoomTypeID = int.Parse(query.roomTypeID.ToString());
            model.RoomTypeInfo_RoomType = query.roomType;
            model.RoomTypeInfo_Quantity = query.quantity != null ? int.Parse(query.quantity.ToString()) : 0;
            return model;
        }

        public RoomTypeDescriptionModel GetRoomTypeDescription(int roomTypeDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            RoomTypeDescriptionModel model = new RoomTypeDescriptionModel();
            var query = db.tblRoomTypeDescriptions.Single(m => m.roomTypeDescriptionID == roomTypeDescriptionID);
            model.RoomTypeDescription_RoomTypeID = int.Parse(query.roomTypeID.ToString());
            model.RoomTypeDescription_RoomType = query.roomType;
            model.RoomTypeDescription_Description = query.description;
            model.RoomTypeDescription_Culture = query.culture;
            return model;
        }

        public List<SelectListItem> GetDDLData(string path, int id)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            switch (path)
            {
                case "TransportationZone":
                    {
                        list = PlacesCatalogs.FillDrpTransportationZones(id);
                        break;
                    }
                case "PlaceClasification":
                    {
                        list = PlacesCatalogs.FillDrpPlaceClasifications(id);
                        break;
                    }
                case "Destination":
                    {
                        list = PlacesCatalogs.FillDrpDestinations();
                        break;
                    }
                case "PlaceType":
                    {
                        list = PlacesCatalogs.FillDrpPlaceTypes();
                        break;
                    }
                case "Zone":
                    {
                        list = PlacesCatalogs.FillDrpZones(id);
                        break;
                    }
            }
            return list;
        }

        public List<GenericListModel> SearchPlacesRelatedItems(string item, string path)
        {
            ePlatEntities db = new ePlatEntities();
            List<GenericListModel> list = new List<GenericListModel>();
            switch (item)
            {
                case "TransportationZones":
                    {
                        var query = from tz in db.tblTransportationZones
                                    where (path == null || tz.transportationZone.Contains(path))
                                    select tz;
                        foreach (var i in query)
                        {
                            list.Add(new GenericListModel()
                            {
                                ItemID = i.transportationZoneID,
                                ItemName = i.transportationZone
                            });
                        }
                        break;
                    }
                case "PlaceClasifications":
                    {
                        var query = from pc in db.tblPlaceClasifications
                                    where (path == null || pc.placeClasification.Contains(path))
                                    select pc;
                        foreach (var i in query)
                        {
                            list.Add(new GenericListModel()
                            {
                                ItemID = i.placeClasificationID,
                                ItemName = i.placeClasification
                            });
                        }
                        break;
                    }
                case "Destinations":
                    {
                        var query = from d in db.tblDestinations
                                    where (path == null || d.destination.Contains(path))
                                    select d;
                        foreach (var i in query)
                        {
                            list.Add(new GenericListModel()
                            {
                                ItemID = int.Parse(i.destinationID.ToString()),
                                ItemName = i.destination
                            });
                        }
                        break;
                    }
                case "PlaceTypes":
                    {
                        var query = from pt in db.tblPlaceTypes
                                    where (path == null || pt.placeType.Contains(path))
                                    select pt;
                        foreach (var i in query)
                        {
                            list.Add(new GenericListModel()
                            {
                                ItemID = i.placeTypeID,
                                ItemName = i.placeType
                            });
                        }
                        break;
                    }
                case "Zones":
                    {
                        var query = from z in db.tblZones
                                    where (path == null || z.zone.Contains(path))
                                    select z;
                        foreach (var i in query)
                        {
                            list.Add(new GenericListModel()
                            {
                                ItemID = i.zoneID,
                                ItemName = i.zone
                            });
                        }
                        break;
                    }
            }
            return list;
        }

        public AttemptResponse SaveZone(ZoneInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.ZoneInfo_ZoneID != 0)
            {
                try
                {
                    tblZones zone = db.tblZones.Single(m => m.zoneID == model.ZoneInfo_ZoneID);
                    zone.zone = model.ZoneInfo_Zone;
                    zone.destinationID = model.ZoneInfo_Destination;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = zone.zoneID;
                    response.Message = "Zone Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Zone NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblZones zone = new tblZones();
                    zone.zone = model.ZoneInfo_Zone;
                    zone.destinationID = model.ZoneInfo_Destination;
                    db.AddTotblZones(zone);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = zone.zoneID;
                    response.Message = "Zone Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Zone NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SavePlaceType(PlaceTypeInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.PlaceTypeInfo_PlaceTypeID != 0)
            {
                try
                {
                    tblPlaceTypes type = db.tblPlaceTypes.Single(m => m.placeTypeID == model.PlaceTypeInfo_PlaceTypeID);
                    type.placeType = model.PlaceTypeInfo_PlaceType;
                    type.active = model.PlaceTypeInfo_IsActive;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = type.placeTypeID;
                    response.Message = "Place Type Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place Type NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPlaceTypes type = new tblPlaceTypes();
                    type.placeType = model.PlaceTypeInfo_PlaceType;
                    type.active = model.PlaceTypeInfo_IsActive;
                    db.AddTotblPlaceTypes(type);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = type.placeTypeID;
                    response.Message = "Place Type Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place Type NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SaveDestination(DestinationInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.DestinationInfo_DestinationID != 0)
            {
                try
                {
                    tblDestinations destination = db.tblDestinations.Single(m => m.destinationID == model.DestinationInfo_DestinationID);
                    destination.destination = model.DestinationInfo_Destination;
                    destination.lat = model.DestinationInfo_Latitude;
                    destination.lng = model.DestinationInfo_Longitude;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = destination.destinationID;
                    response.Message = "Destination Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Destination NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblDestinations destination = new tblDestinations();
                    destination.destination = model.DestinationInfo_Destination;
                    destination.lat = model.DestinationInfo_Latitude;
                    destination.lng = model.DestinationInfo_Longitude;
                    db.AddTotblDestinations(destination);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = destination.destinationID;
                    response.Message = "Destination Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Destination NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SavePlaceClasification(PlaceClasificationInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.PlaceClasificationInfo_PlaceClasificationID != 0)
            {
                try
                {
                    tblPlaceClasifications clasification = db.tblPlaceClasifications.Single(m => m.placeClasificationID == model.PlaceClasificationInfo_PlaceClasificationID);
                    clasification.placeClasification = model.PlaceClasificationInfo_PlaceClasification;
                    clasification.placeTypeID = model.PlaceClasificationInfo_PlaceType;
                    clasification.hosting = model.PlaceClasificationInfo_Hosting;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = clasification.placeClasificationID;
                    response.Message = "Place Clasification Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place Clasification NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblPlaceClasifications clasification = new tblPlaceClasifications();
                    clasification.placeClasification = model.PlaceClasificationInfo_PlaceClasification;
                    clasification.placeTypeID = model.PlaceClasificationInfo_PlaceType;
                    clasification.hosting = model.PlaceClasificationInfo_Hosting;
                    db.AddTotblPlaceClasifications(clasification);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = clasification.placeClasificationID;
                    response.Message = "Place Clasification Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Place Clasification NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse SaveTransportationZone(TransportationZoneInfoModel model)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();

            if (model.TransportationZoneInfo_TransportationZoneID != 0)
            {
                try
                {
                    tblTransportationZones zone = db.tblTransportationZones.Single(m => m.transportationZoneID == model.TransportationZoneInfo_TransportationZoneID);
                    zone.transportationZone = model.TransportationZoneInfo_TransportationZone;
                    zone.destinationID = model.TransportationZoneInfo_Destination;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = zone.transportationZoneID;
                    response.Message = "Transportation Zone Updated";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Transportation Zone NOT Updated";
                    return response;
                }
            }
            else
            {
                try
                {
                    tblTransportationZones zone = new tblTransportationZones();
                    zone.transportationZone = model.TransportationZoneInfo_TransportationZone;
                    zone.destinationID = model.TransportationZoneInfo_Destination;
                    db.AddTotblTransportationZones(zone);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.ObjectID = zone.transportationZoneID;
                    response.Message = "Transportation Zone Saved";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.ObjectID = 0;
                    response.Exception = ex;
                    response.Message = "Transportation Zone NOT Saved";
                    return response;
                }
            }
        }

        public AttemptResponse DeletePlaceItem(int id, string path)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            switch (path)
            {
                case "TransportationZones":
                    {
                        var query = (from tz in db.tblTransportationZones
                                     where tz.transportationZoneID == id
                                     select tz).Single();
                        try
                        {
                            db.DeleteObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = id;
                            response.Message = "Transportation Zone Deleted";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Exception = ex;
                            response.ObjectID = 0;
                            response.Message = "Transportation Zone NOT Deleted";
                            return response;
                        }
                    }
                case "PlaceClasifications":
                    {
                        var query = (from pc in db.tblPlaceClasifications
                                     where pc.placeClasificationID == id
                                     select pc).Single();
                        try
                        {
                            db.DeleteObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = id;
                            response.Message = "Place Clasification Deleted";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Exception = ex;
                            response.ObjectID = 0;
                            response.Message = "Place Clasification NOT Deleted";
                            return response;
                        }
                    }
                case "Destinations":
                    {
                        var query = (from d in db.tblDestinations
                                     where d.destinationID == id
                                     select d).Single();
                        try
                        {
                            db.DeleteObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = id;
                            response.Message = "Destination Deleted";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Exception = ex;
                            response.ObjectID = 0;
                            response.Message = "Destination NOT Deleted";
                            return response;
                        }
                    }
                case "PlaceTypes":
                    {
                        var query = (from pt in db.tblPlaceTypes
                                     where pt.placeTypeID == id
                                     select pt).Single();
                        try
                        {
                            db.DeleteObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = id;
                            response.Message = "Place Type Deleted";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Exception = ex;
                            response.ObjectID = 0;
                            response.Message = "Place Type NOT Deleted";
                            return response;
                        }
                    }
                case "Zones":
                    {
                        var query = (from z in db.tblZones
                                     where z.zoneID == id
                                     select z).Single();
                        try
                        {
                            db.DeleteObject(query);
                            db.SaveChanges();
                            response.Type = Attempt_ResponseTypes.Ok;
                            response.ObjectID = id;
                            response.Message = "Zone Deleted";
                            return response;
                        }
                        catch (Exception ex)
                        {
                            response.Type = Attempt_ResponseTypes.Error;
                            response.Exception = ex;
                            response.ObjectID = 0;
                            response.Message = "Zone NOT Deleted";
                            return response;
                        }
                    }
                default:
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.ObjectID = 0;
                        response.Message = "Element NOT Found";
                        return response;
                    }
            }
        }

        public Object GetPlaceItem(int id, string path)
        {
            ePlatEntities db = new ePlatEntities();
            switch (path)
            {
                case "TransportationZones":
                    {
                        try
                        {
                            var query = (from tz in db.tblTransportationZones
                                         where tz.transportationZoneID == id
                                         select tz).Single();
                            return (new { firstItem = query.transportationZone, secondItem = query.destinationID });
                        }
                        catch
                        {
                            return (new { ok = false });
                        }
                    }
                case "PlaceClasifications":
                    {
                        try
                        {
                            var query = (from pc in db.tblPlaceClasifications
                                         where pc.placeClasificationID == id
                                         select pc).Single();
                            return (new { firstItem = query.placeClasification, secondItem = query.placeTypeID, thirdItem = query.hosting });
                        }
                        catch
                        {
                            return (new { ok = false });
                        }
                    }
                case "Destinations":
                    {
                        try
                        {
                            var query = (from d in db.tblDestinations
                                         where d.destinationID == id
                                         select d).Single();
                            return (new { firstItem = query.destination, secondItem = query.lat, thirdItem = query.lng });
                        }
                        catch
                        {
                            return (new { ok = false });
                        }
                    }
                case "PlaceTypes":
                    {
                        try
                        {
                            var query = (from pt in db.tblPlaceTypes
                                         where pt.placeTypeID == id
                                         select pt).Single();
                            return (new { firstItem = query.placeType, secondItem = query.active });
                        }
                        catch
                        {
                            return (new { ok = false });
                        }
                    }
                case "Zones":
                    {
                        try
                        {
                            var query = (from z in db.tblZones
                                         where z.zoneID == id
                                         select z).Single();
                            return (new { firstItem = query.zone, secondItem = query.destinationID });
                        }
                        catch
                        {
                            return (new { ok = false });
                        }
                    }
                default: return (new { ok = false });
            }
        }

        public AttemptResponse DeletePlace(int id)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                tblPlaces place = db.tblPlaces.Single(m => m.placeID == id);
                db.DeleteObject(place);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = 0;
                response.Message = "Place Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                response.Exception = ex;
                response.Message = "Place NOT Deleted";
                return response;
            }
        }

        public AttemptResponse DeletePlaceDescription(int placeDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = (from d in db.tblPlaceDescriptions
                             where d.placeDescriptionID == placeDescriptionID
                             select d).Single();
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.ObjectID = placeDescriptionID;
                response.Message = "Place Description Deleted";
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.ObjectID = 0;
                response.Exception = ex;
                response.Message = "Place Description NOT Deleted";
                return response;
            }
        }

        public AttemptResponse DeletePlacePlanType(int placePlanTypeID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblPlaces_PlanTypes.Single(m => m.place_PlanTypeID == placePlanTypeID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Plan Type Deleted";
                response.ObjectID = query.place_PlanTypeID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Plan Type NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteRoomType(int roomTypeID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblRoomTypes.Single(m => m.roomTypeID == roomTypeID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Room Type Deleted";
                response.ObjectID = roomTypeID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Room Type NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteRoomTypeDescription(int roomTypeDescriptionID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var query = db.tblRoomTypeDescriptions.Single(m => m.roomTypeDescriptionID == roomTypeDescriptionID);
                db.DeleteObject(query);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Room Type Description Deleted";
                response.ObjectID = roomTypeDescriptionID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Room Type Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public static List<SelectListItem> GetPlacesByType(int PlaceTypeID)
        {
            ePlatEntities db = new ePlatEntities();

            var places = from x in db.tblPlaces where x.placeTypeID == PlaceTypeID select new { x.placeID, x.place, x.tblDestinations.destination };

            List<SelectListItem> placesList = new List<SelectListItem>();

            foreach (var r in places)
            {
                placesList.Add(new SelectListItem() { Text = r.place + " " + r.destination, Value = r.placeID.ToString() });
            }

            return placesList;

        }

        public static List<SelectListItem> GetResortsByDestination(int DestinationID, bool concatenateDestination = false)
        {
            ePlatEntities db = new ePlatEntities();
            var _terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var resorts = from place in db.tblPlaces_Terminals
                        join destination in db.tblPlaces on place.placeID equals destination.placeID
                        where destination.placeTypeID == 1//placeType == resort
                        && _terminals.Contains(place.terminalID)
                        && destination.destinationID == DestinationID
                        select new
                        {
                            destination.placeID,
                            destination.place,
                            destination.tblDestinations.destination
                        };

            //var resorts = from x in db.tblPlaces where x.destinationID == DestinationID
            //              && x.placeTypeID == 1//hotels
            //              select new { x.placeID, x.place, x.tblDestinations.destination };
            //select new { x.placeID, x.place };

            List<SelectListItem> resortsList = new List<SelectListItem>();

            foreach (var r in resorts.OrderBy(m => m.destination).ThenBy(m => m.place))
            {
                resortsList.Add(new SelectListItem() { Text = r.place + (concatenateDestination ? " - " + r.destination : ""), Value = r.placeID.ToString() });
            }

            return resortsList;

        }

        public static List<SelectListItem> GetResortsByDestinations(long?[] destinations = null, bool concatenateDestination = false)
        {
            ePlatEntities db = new ePlatEntities();
            var _terminals = session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray();
            var _destinations = destinations != null ? destinations : GetDestinationsByCurrentTerminals().Select(m => (long?)long.Parse(m.Value)).ToArray();

            var resorts = from place in db.tblPlaces_Terminals
                          join destination in db.tblPlaces on place.placeID equals destination.placeID
                          where destination.placeTypeID == 1//placeType == resort
                          && _terminals.Contains(place.terminalID)
                          && _destinations.Contains(destination.destinationID)
                          select new
                          {
                              destination.placeID,
                              destination.place,
                              destination.tblDestinations.destination
                          };

            //var resorts = from x in db.tblPlaces where x.destinationID == DestinationID
            //              && x.placeTypeID == 1//hotels
            //              select new { x.placeID, x.place, x.tblDestinations.destination };
            //select new { x.placeID, x.place };

            List<SelectListItem> resortsList = new List<SelectListItem>();

            foreach (var r in resorts.OrderBy(m => m.destination).ThenBy(m => m.place))
            {
                resortsList.Add(new SelectListItem() { Text = r.place + (concatenateDestination ? " - " + r.destination : ""), Value = r.placeID.ToString() });
            }

            return resortsList;

        }

        public static SelectListItem GetResortByID(long PlaceID)
        {
            ePlatEntities db = new ePlatEntities();

            var r = (from x in db.tblPlaces
                     where x.placeID == PlaceID
                     select new { x.placeID, place = x.place + " - " + x.tblDestinations.destination }).Single();
            var sli = new SelectListItem() { Text = r.place, Value = r.placeID.ToString() };
            return sli;

        }

        public static List<SelectListItem> GetResortsByTerminals(string specifiedTerminals = "", bool concatenateDestination = false)
        {
            ePlatEntities db = new ePlatEntities();

            string _terminals = specifiedTerminals;

            if (_terminals == null || _terminals == "")
            {
                _terminals = session.Terminals;
            }

            long[] terminals = _terminals.Split(',').Select(x => long.Parse(x)).ToArray();

            var query = (from a in db.tblPlaces_Terminals
                         where terminals.Contains(a.terminalID)
                         && a.tblPlaces.placeTypeID == 1
                         select new SelectListItem
                         {
                             Text = a.tblPlaces.place + ((concatenateDestination) ? " - " + a.tblPlaces.tblDestinations.destination : ""),
                             Value = SqlFunctions.StringConvert((double)a.placeID).Trim()
                         }).Distinct().ToList();

            return query;
        }

        public static List<SelectListItem> GetResortsByTerminalDomains(string specifiedTerminals = "", bool concatenateDestination = false)
        {
            ePlatEntities db = new ePlatEntities();

            string _terminals = specifiedTerminals;

            if (_terminals == null || _terminals == "")
            {
                _terminals = session.Terminals;
            }

            long[] terminals = _terminals.Split(',').Select(x => long.Parse(x)).ToArray();

            var destinationsQ = from d in db.tblTerminals_Destinations
                                where terminals.Contains(d.terminalID)
                                select d.destinationID;

            var query = (from a in db.tblPlaces
                         where destinationsQ.Contains(a.destinationID)
                         && a.placeTypeID == 1
                         select new SelectListItem
                         {
                             Text = a.place + ((concatenateDestination) ? " - " + a.tblDestinations.destination : ""),
                             Value = SqlFunctions.StringConvert((double)a.placeID).Trim()
                         }).Distinct().ToList();

            return query;
        }

        public static List<SelectListItem> GetResortsByProfile(ePlatEntities dataContext = null)
        {
            ePlatEntities db = dataContext ?? new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            int workgroupID = int.Parse(session.WorkGroupID.ToString());
            Guid roleID = Guid.Parse(session.RoleID.ToString());
            var terminals = session.Terminals != "" ? session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray() : session.UserTerminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var placesByProfile = db.tblSysWorkGroups_Places.Where(m => m.sysWorkGroupID == workgroupID && m.roleID == roleID).Select(m => m.placeID).ToArray();
            //var places = (from place in db.tblSysWorkGroups_Places
            //              where place.sysWorkGroupID == workgroupID
            //              && place.roleID == roleID
            //              select place.placeID).ToList<Int64>();

            var places = (from place in db.tblPlaces_Terminals
                          where terminals.Contains(place.terminalID)
                          && placesByProfile.Contains(place.placeID)
                          select place.placeID).ToList<Int64>();

            var query = (from resort in db.tblPlaces
                         where places.Contains(resort.placeID)
                         && resort.placeTypeID == 1
                         orderby resort.place ascending
                         select new
                         {
                             resort.placeID,
                             resort.place,
                             resort.tblDestinations.destination
                         }).Distinct();

            foreach (var i in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.placeID.ToString(),
                    Text = i.place + " " + i.destination
                });
            }
            return list;
        }

        public static List<SelectListItem> GetResortsByUser()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            var isAdmin = GeneralFunctions.IsUserInRole("Administrator", session.UserID);

            var byTerminal = GetResortsByTerminals("", true).Select(m => long.Parse(m.Value)).ToArray();

            var query = from place in db.tblPlaces
                        join d in db.tblDestinations on place.destinationID equals d.destinationID into place_d
                        from d in place_d.DefaultIfEmpty()
                        join up in db.tblUsers_Places on place.placeID equals up.placeID into place_up
                        from up in place_up.DefaultIfEmpty()
                        where (up.userID == session.UserID || isAdmin)
                        && byTerminal.Contains(place.placeID)
                        select new
                        {
                            place.placeID,
                            place.place,
                            d.destination
                        };

            foreach(var i in query.Distinct())
            {
                list.Add(new SelectListItem()
                {
                    Value = i.placeID.ToString(),
                    Text = i.place + " " + i.destination
                });
            }

            return list;
        }

        public static List<SelectListItem> GetAllRoomTypes()
        {
            ePlatEntities db = new ePlatEntities();
            var list = new List<SelectListItem>();
            foreach (var i in db.tblRoomTypes)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.roomTypeID.ToString(),
                    Text = i.roomType
                });
            }
            return list;
        }

        public static List<SelectListItem> GetDestinationsByCurrentTerminals(string specifiedTerminals = "")
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var terminals = specifiedTerminals == "" ? session.Terminals.Split(',').Select(m => long.Parse(m)).ToArray() : specifiedTerminals.Split(',').Select(m => long.Parse(m)).ToArray();

            var query = (from d in db.tblTerminals_Destinations
                         where terminals.Contains(d.terminalID)
                         select new
                         {
                             d.destinationID,
                             d.tblDestinations.destination
                         }).Distinct();
            foreach (var i in query)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.destinationID.ToString(),
                    Text = i.destination
                });
            }
            return list;
        }

        public static List<SelectListItem> GetAllDestinations()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var i in db.tblDestinations)
            {
                list.Add(new SelectListItem()
                {
                    Value = i.destinationID.ToString(),
                    Text = i.destination
                });
            }
            return list;
        }

        public static int? GetTransportationZoneForPlace(long placeid)
        {
            ePlatEntities db = new ePlatEntities();

            var transportationZoneID = (from p in db.tblPlaces
                                        where p.placeID == placeid
                                        select p.transportationZoneID).Single();

            return transportationZoneID;
        }

        public static List<SelectListItem> GetPlacesByDestinationsPerTerminals(string specifiedTerminals = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _destinations = GetDestinationsByCurrentTerminals(specifiedTerminals).Select(m => int.Parse(m.Value)).ToArray();

            foreach (var i in _destinations)
            {
                var _resorts = GetResortsByDestination(i, true);
                foreach (var a in _resorts)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = a.Value,
                        Text = a.Text
                    });
                }
            }
            return list;
        }
    }
}
