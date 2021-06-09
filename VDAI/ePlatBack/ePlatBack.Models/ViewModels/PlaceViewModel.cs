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

namespace ePlatBack.Models.ViewModels
{
    public class PlaceViewModel
    {
        public PlacesSearchModel PlacesSearchModel { get; set; }

        public List<PlacesSearchResultsModel> SearchResults { get; set; }

        public PlaceInfoModel PlaceInfoModel { get; set; }

        public ZoneInfoModel ZoneInfoModel { get; set; }

        public PlaceTypeInfoModel PlaceTypeInfoModel { get; set; }

        public DestinationInfoModel DestinationInfoModel { get; set; }

        public PlaceClasificationInfoModel PlaceClasificationInfoModel { get; set; }

        public TransportationZoneInfoModel TransportationZoneInfoModel { get; set; }

        public ZonesSearchModel ZonesSearchModel { get; set; }

        public PlaceTypesSearchModel PlaceTypesSearchModel { get; set; }

        public DestinationsSearchModel DestinationsSearchModel { get; set; }

        public PlaceClasificationsSearchModel PlaceClasificationsSearchModel { get; set; }

        public TransportationZonesSearchModel TransportationZonesSearchModel { get; set; }
    }

    public class PlacesSearchResultsModel
    {
        public int PlaceID { get; set; }
        public string Place { get; set; }
        public string PlaceLabel { get; set; }
        public string Destination { get; set; }
        public string Zone { get; set; }
        public string PlaceType { get; set; }
    }

    public class PlacesSearchModel
    {
        [Display(Name = "Place")]
        public string Search_Place { get; set; }

        [Display(Name = "Destination")]
        public int Search_Destination { get; set; }
        public List<SelectListItem> Search_DrpDestinations
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpDestinations();
            }
            }

        [Display(Name = "Place Type")]
        public int Search_PlaceType { get; set; }
        public List<SelectListItem> Search_DrpPlaceTypes {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpPlaceTypes();
            }
        }

        public string Search_Terminals { get; set; }
    }

    public class PlaceInfoModel
    {
        public int? PlaceInfo_PlaceID { get; set; }

        [Required(ErrorMessage = "Place is required")]
        [Display(Name = "Place")]
        public string PlaceInfo_Place { get; set; }

        [Display(Name = "Place Label")]
        public string PlaceInfo_PlaceLabel { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string PlaceInfo_Address { get; set; }

        //[Required(ErrorMessage = "Phone is required")]
        [StringLength(15, ErrorMessage = " Phone 15 characters maximum")]
        [Display(Name = "Phone")]
        public string PlaceInfo_Phone { get; set; }

        [Required(ErrorMessage = "Latitude is required")]
        [StringLength(50, ErrorMessage = "Latitude 50 characters maximum")]
        [Display(Name = "Latitude")]
        //[RegularExpression("^-?[0-9]{1,14}(\\.[0-9]{1,14})?$", ErrorMessage = "Format Is Not Correct")]
        public string PlaceInfo_Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [StringLength(50, ErrorMessage = "Longitude 50 characters maximum")]
        //[RegularExpression("^-?[0-9]{1,14}(\\.[0-9]{1,14})?$", ErrorMessage = "Format Is Not Correct")]
        [Display(Name = "Longitude")]
        public string PlaceInfo_Longitude { get; set; }

        [Display(Name = "Destination")]
        [Range(1, int.MaxValue, ErrorMessage = "Destination is required")]
        public int PlaceInfo_Destination { get; set; }
        public List<SelectListItem> PlaceInfo_DrpDestinations
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpDestinations();
            }
        }

        [Display(Name = "Zone")]
        [Range(1, int.MaxValue, ErrorMessage = "Zone is required")]
        public int PlaceInfo_Zone{get;set;}
        public List<SelectListItem> PlaceInfo_DrpZones
        {
            get{
                return PlaceDataModel.PlacesCatalogs.FillDrpZones(0);
            }
        }

        [Required(ErrorMessage = "Is VillaGroup is required")]
        [Display(Name = "Is VillaGroup")]
        public bool PlaceInfo_IsVillaGroup { get; set; }

        [Display(Name = "Place Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Place Type is required")]
        public int PlaceInfo_PlaceType { get; set; }
        public List<SelectListItem> PlaceInfo_DrpPlaceTypes
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpPlaceTypes();
            }
        }

        [Display(Name = "Place Clasification")]
        public int PlaceInfo_PlaceClasification { get; set; }
        public List<SelectListItem> PlaceInfo_DrpPlaceClasifications
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpPlaceClasifications(0);
            }
            }

        [Required(ErrorMessage = "Prospectation is required")]
        [Display(Name = "Prospectation")]
        public bool PlaceInfo_Prospectation { get; set; }

        [Required(ErrorMessage = "Is For Sale is required")]
        [Display(Name = "Is For Sale")]
        public bool PlaceInfo_IsForSale { get; set; }

        //[Required(ErrorMessage = "Check-In Time is required")]
        //pendiente validar formato
        [Display(Name = "Check-In Time")]
        public string PlaceInfo_CheckInTime { get; set; }

        //[Required(ErrorMessage = "Check-Out Time is required")]
        //pendiente validar formato
        [Display(Name = "Check-Out Time")]
        public string PlaceInfo_CheckOutTime { get; set; }

        [Required(ErrorMessage = "Taxes Percentage is required")]
        //pendiente validar formato
        [Display(Name = "Taxes Percentage")]
        public int PlaceInfo_TaxesPercentage { get; set; }

        [Display(Name = "Transportation Zone")]
        public int PlaceInfo_TransportationZone { get; set; }
        public List<SelectListItem> PlaceInfo_DrpTransportationZones
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpTransportationZones(0);
            }
        }

        [Required(ErrorMessage = "Terminals is required")]
        [Display(Name = "Terminals")]
        public string PlaceInfo_Terminal { get; set; }
        public List<SelectListItem> PlaceInfo_DrpTerminals
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpTerminals();
            }
        }

        public List<KeyValuePair<int, string>> PlaceInfo_ListTerminals { get; set; }

        public int[] PlaceInfo_AllowedTerminals
        {
            get
            {
                return TerminalDataModel.GetCurrentUserTerminals().Select(m => int.Parse(m.Value)).ToArray();
            }
        }

        public PlaceInfoModel(){
            PlaceInfo_IsVillaGroup = true;
            PlaceInfo_Prospectation = false;
            PlaceInfo_IsForSale = false;
        }

        public PlaceDescriptionModel PlaceDescriptionModel { get; set; }

        public PlacePlanTypeModel PlacePlanTypeModel { get; set; }

        public RoomTypeInfoModel RoomTypeInfoModel { get; set; }

        public PictureInfoModel PictureInfoModel { get; set; }
    }

    public class PlaceDescriptionModel
    {
        public int PlaceDescription_PlaceID { get; set; }

        public int PlaceDescription_PlaceDescriptionID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        [Display(Name = "Language")]
        public string PlaceDescription_Culture { get; set; }
        public List<SelectListItem> PlaceDescription_DrpCultures {
            get {
                return PlaceDataModel.PlacesCatalogs.FillDrpCultures();
            }
        }

        [Display(Name = "Short Description")]
        [AllowHtml]
        public string PlaceDescription_ShortDescription { get; set; }

        [Display(Name = "Full Description")]
        [AllowHtml]
        public string PlaceDescription_FullDescription { get; set; }

        [Display(Name = "FAQ")]
        [AllowHtml]
        public string PlaceDescription_FAQ { get; set; }

        [Display(Name = "Amenities")]
        [AllowHtml]
        public string PlaceDescription_Amenities { get; set; }

        [Display(Name = "AllInclusive")]
        [AllowHtml]
        public string PlaceDescription_AllInclusive { get; set; }

    }

    public class PlacePlanTypeModel
    {
        public int PlacePlanType_PlacePlanTypeID { get; set; }

        public int PlacePlanType_PlaceID { get; set; }

        [Display(Name = "Plan Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Plan Type is required")]
        public int PlacePlanType_PlanType { get; set; }
        public List<SelectListItem> PlacePlanType_DrpPlanTypes
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpPlanTypes();
            }
        }

        [Display(Name = "Terminal")]
        [Range(1, int.MaxValue, ErrorMessage = "Terminal is required")]
        public int PlacePlanType_Terminal { get; set; }
        public List<SelectListItem> PlacePlanType_DrpTerminals
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpTerminals();
            }
        }
    }

    public class RoomTypeInfoModel
    {
        public int RoomTypeInfo_PlaceID { get; set; }

        public int RoomTypeInfo_RoomTypeID { get; set; }

        [Display(Name = "Room Type")]
        [Required(ErrorMessage = "Room Type is required")]
        public string RoomTypeInfo_RoomType { get; set; }

        [Display(Name = "Quantity")]
        public int RoomTypeInfo_Quantity { get; set; }

        public string RoomTypeInfo_DateSaved { get; set; }

        public RoomTypeDescriptionModel RoomTypeDescriptionModel { get; set; }
    }

    public class RoomTypeDescriptionModel
    {
        public int RoomTypeDescription_RoomTypeDescriptionID { get; set; }

        public int RoomTypeDescription_RoomTypeID { get; set; }

        [Display(Name = "Room Type")]
        [Required(ErrorMessage = "Room Type is required")]
        public string RoomTypeDescription_RoomType { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string RoomTypeDescription_Description { get; set; }

        [Display(Name = "Language")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Language is required")]
        public string RoomTypeDescription_Culture { get; set; }
        public List<SelectListItem> RoomTypeDescription_DrpCultures
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpCultures();
            }
        }

    }

    public class TransportationZonesSearchModel
    {
        [Display(Name = "Transportation Zone")]
        public string Search_TransportationZones { get; set; }
    }

    public class TransportationZoneInfoModel
    {
        public int TransportationZoneInfo_TransportationZoneID { get; set; }

        [Required(ErrorMessage = "Transportation Zone is required")]
        [Display(Name = "Transportation Zone")]
        public string TransportationZoneInfo_TransportationZone { get; set; }

        [Display(Name = "Destination")]
        [Range(1, int.MaxValue, ErrorMessage = "Destination is required")]
        public int TransportationZoneInfo_Destination { get; set; }
        public List<SelectListItem> TransportationZoneInfo_DrpDestinations {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpDestinations();
            }
            }
    }

    public class PlaceClasificationsSearchModel
    {
        [Display(Name = "Place Clasification")]
        public string Search_PlaceClasifications { get; set; }
    }

    public class PlaceClasificationInfoModel
    {
        public int PlaceClasificationInfo_PlaceClasificationID { get; set; }

        [Required(ErrorMessage = "Place Clasification is required")]
        [Display(Name = "Place Clasification")]
        public string PlaceClasificationInfo_PlaceClasification { get; set; }

        [Range (1, int.MaxValue, ErrorMessage = "Place Type is required")]
        [Display(Name = "Place Type")]
        public int PlaceClasificationInfo_PlaceType { get; set; }
        public List<SelectListItem> PlaceClasificationInfo_DrpPlaceTypes
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpPlaceTypes();
            }
        }

        [Required(ErrorMessage = "Hosting is required")]
        [Display(Name = "Hosting")]
        public bool PlaceClasificationInfo_Hosting { get; set; }

        public PlaceClasificationInfoModel()
        {
            PlaceClasificationInfo_Hosting = true;
        }
    }

    public class DestinationsSearchModel
    {
        [Display(Name = "Destination")]
        public string Search_Destinations { get; set; }
    }

    public class DestinationInfoModel
    {
        public int DestinationInfo_DestinationID { get; set; }

        [Required(ErrorMessage = "Destination is required")]
        [Display(Name = "Destination")]
        public string DestinationInfo_Destination { get; set; }

        [Display(Name = "Latitude")]
        public string DestinationInfo_Latitude { get; set; }

        [Display(Name = "Longitude")]
        public string DestinationInfo_Longitude { get; set; }
    }

    public class PlaceTypesSearchModel
    {
        [Display(Name = "Place Type")]
        public string Search_PlaceTypes { get; set; }
    }

    public class PlaceTypeInfoModel
    {
        public int PlaceTypeInfo_PlaceTypeID { get; set; }

        [Required(ErrorMessage = "Place Type is required")]
        [Display(Name = "Place Type")]
        public string PlaceTypeInfo_PlaceType { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool PlaceTypeInfo_IsActive { get; set; }

        public PlaceTypeInfoModel()
        {
            PlaceTypeInfo_IsActive = true;
        }
    }

    public class ZonesSearchModel
    {
        [Display(Name = "Zone")]
        public string Search_Zones { get; set; }
    }

    public class ZoneInfoModel
    {
        public int ZoneInfo_ZoneID { get; set; }

        [Required(ErrorMessage = "Zone is required")]
        [Display(Name = "Zone")]
        public string ZoneInfo_Zone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Destination is required")]
        [Display(Name = "Destination")]
        public int ZoneInfo_Destination { get; set; }
        public List<SelectListItem> ZoneInfo_DrpDestinations
        {
            get
            {
                return PlaceDataModel.PlacesCatalogs.FillDrpDestinations();
            }
        }
    }
}
