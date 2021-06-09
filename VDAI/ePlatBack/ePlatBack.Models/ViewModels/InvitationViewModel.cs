using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.ViewModels
{
    public class InvitationViewModel
    {
        public InvitationInfoView InvitationForm { get; set; }
        public string Terminal { get; set; }

        public class InvitationsResponse
        {
            public List<InvitationInfoModel> Invitations { get; set; }
        }

        public class InvitationInfoModel
        {
            public Guid? InvitationID { get; set; }
            public string PresentationDate { get; set; }
            public string Guest { get; set; }
            public int? CountryID { get; set; }
            public string Country { get; set; }
            public string State { get; set; }
            public long? DestinationID { get; set; }
            public long? PlaceID { get; set; }
            public string ResortOther { get; set; }
            public string Resort { get; set; }
            public string Deposit { get; set; }
            public int DepositOther { get; set; }
            public int? DepositCurrencyID { get; set; }
            public string PickUpTimeHour { get; set; }
            public string PickUpTimeMinute { get; set; }
            public string PickUpTimeMeridian { get; set; }
            public string PresentationTimeHour { get; set; }
            public string PresentationTimeMinute { get; set; }
            public string PresentationTimeMeridian { get; set; }
            public string GuestPhone { get; set; }
            public string GuestEmail { get; set; }
            public string InvitationNumber { get; set; }
            public int? ProgramID { get; set; }
            public string Program { get; set; }
            public string Culture { get; set; }
            public bool Confirmed { get; set; }
            public string PickUpNotes { get; set; }
            public string Gifts { get; set; }
            public int? LocationID { get; set; }
            public int? PromotionTeamID { get; set; }
            public long? OPCID { get; set; }
            public string OPCOther { get; set; }
            public string OPC { get; set; }
            public string SavedOn { get; set; }
            public string SavedBy { get; set; }
            public bool ShowNoShow { get; set; }
            public string WelcomeLetterSentOn { get; set; }
            public string WelcomeLetterSentBy { get; set; }
            public long? SPICustomerID { get; set; }
            public long? TerminalID { get; set; }
        }

        public class InvitationInfoView
        {
            public Guid? InvitationID { get; set; }

            [Required]
            [Display(Name = "Presentation Date")]
            public string PresentationDate { get; set; }

            [Required]
            [Display(Name = "VIP Guest")]
            public string Guest { get; set; }

            [Required]
            [Display(Name = "Visiting from")]
            public int CountryID { get; set; }
            public List<SelectListItem> Countries
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCountriesDefaultNull();
                }
            }

            [Required]
            [Display(Name = "State")]
            public string State { get; set; }

            [Required]
            [Display(Name = "Destination")]
            public long DestinationID { get; set; }
            public List<SelectListItem> Destinations
            {
                get
                {
                    List<SelectListItem> destinations = MasterChartDataModel.LeadsCatalogs.FillDrpDestinations();
                    destinations.Insert(0, ListItems.Default("--Select One--",""));
                    return destinations;
                }
            }

            [Required]
            [Display(Name = "Hotel")]
            public long PlaceID { get; set; }
            public List<SelectListItem> Resorts
            {
                get
                {
                    List<SelectListItem> resorts = new List<SelectListItem>();
                    resorts = PlaceDataModel.GetResortsByTerminalDomains();
                    resorts.Add(new SelectListItem()
                    {
                        Value = "-1",
                        Text = "Other"
                    });
                    resorts.Insert(0, ListItems.Default("--Select One--", ""));
                    return resorts;
                }
            }
            public string ResortOther { get; set; }

            [Required]
            [Display(Name = "Deposit")]
            public string Deposit { get; set; }
            public List<SelectListItem> DepositOptions
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpDepositOptions();
                }
            }
            public decimal? DepositOther { get; set; }
            public int? DepositCurrencyID { get; set; }
            public List<SelectListItem> Currencies
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpCurrenciesIntID();
                }
            }

            [Required]
            [Display(Name = "Pick Up Time")]
            public string PickUpTimeHour { get; set; }
            [Required]
            public string PickUpTimeMinute { get; set; }
            [Required]
            public string PickUpTimeMeridian { get; set; }


            [Required]
            [Display(Name = "Presentation Time")]
            public string PresentationTimeHour { get; set; }
            [Required]
            public string PresentationTimeMinute { get; set; }
            [Required]
            public string PresentationTimeMeridian { get; set; }

            public List<SelectListItem> Hours
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpHours();
                }
            }
            public List<SelectListItem> Minutes
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpMinutes();
                }
            }

            public List<SelectListItem> Meridians
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpMeridians();
                }
            }

            [Display(Name = "Guest Phone")]
            public string GuestPhone { get; set; }

            [Required]
            [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
            [Display(Name = "Guest Email")]
            public string GuestEmail { get; set; }

            [Display(Name = "Invitation Number")]
            public string InvitationNumber { get; set; }

            [Display(Name = "Pick Up Notes")]
            public string PickUpNotes { get; set; }

            [Required]
            [Display(Name = "Gifts")]
            public string Gifts { get; set; }

            [Required]
            [Display(Name = "Department")]
            public string ProgramID { get; set; }
            public List<SelectListItem> Programs
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpProgramsDefaultNull();
                }
            }

            [Required]
            [Display(Name = "Language")]
            public string Culture { get; set; }
            public List<SelectListItem> Cultures
            {
                get
                {
                    return PageDataModel.PageCatalogs.FillDrpCultures();
                }
            }

            [Required]
            [Display(Name = "Confirmation")]
            public bool? Confirmed { get; set; }
            public List<SelectListItem> ConfirmationStatuses
            {
                get
                {
                    return InvitationDataModel.InvitationCatalogs.FillDrpConfirmationStatuses();
                }
            }

            [Display(Name = "Location")]
            public int LocationID { get; set; }
            public List<SelectListItem> Locations
            {
                get
                {
                    List<SelectListItem> locations = MasterChartDataModel.LeadsCatalogs.FillDrpLocationsForCompaniesGroup();
                    locations.Insert(0, ListItems.Default("--Select One--",""));
                    return locations;
                }
            }

            [Display(Name = "Team")]
            public int PromotionTeamID { get; set; }
            public List<SelectListItem> PromotionTeams
            {
                get
                {
                    return new List<SelectListItem>();
                }
            }

            [Display(Name = "OPC")]
            public long OPCID { get; set; }
            public string OPCOther { get; set; }
            public List<SelectListItem> OPCs
            {
                get{
                    return new List<SelectListItem>();
                }                
            }
            [Display(Name = "SPI Customer ID")]
            public long? SPICustomerID { get; set; }
        }
    }

    public class spiInvitation
    {
        public searchInvitation Invitation { get; set; }
      //  public List<SysComponentsPrivilegesModel> Privileges { get; set; }

        public class searchResults
        {
            public List<invitationModelTable> result { get; set; }
        }

        public class searchInvitation
        {
            [Display(Name = "Invitation Date" )]
            public DateTime? searchInvitationByDate { get; set; }
            [Display(Name ="Presentation Date")]
            public DateTime? searchPresentationDate { get; set; }

            [Display(Name = "Manifested")]
            public bool? searchInvitationManifested { get; set; }           
            [Display(Name = "Hotels")]
            public int? searchInvitationByHotel { get; set; }
            public List<SelectListItem> hotelList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.fillDrpHotelSPI();
                }
            }
            [Display(Name = "Languages")]
            public int? searchInvitationLanguage { get; set; }
            public List<SelectListItem> languageList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.fillDrpLanguageSPI();
                }
            }
            [Display(Name = "Groups")]
            public int? searchInvitationGroups { get; set; }
            public List<SelectListItem> groupList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.fillDrpGroupsSPI();
                }
            }
            [Display(Name = "Teams")]
            public int? searchInvitationByTeam { get; set; }
            public List<SysComponentsPrivilegesModel> Privileges { get; set; }

            //salesRoom in VerifyInformation
            [Display(Name = "Sales Rooms")]
            public int spiSalesRoomID { get; set; }
            public List<SelectListItem> salesRoomsList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.fillDrpSalesRoomSPI();
                }
            }
            [Display(Name = "Invitation Number")]
            public string searchInvitationNumber { get; set; }
        }
        public class invitationModelTable
        {
            public Guid invitationID { get; set; }        
            [Display(Name = "Presentation Date")]
            public DateTime? presentationDateTime { get; set; }
            public string presentationDateTimeFormat { get; set; }
            [Display(Name="Guest")]
            public string guest { get; set; }
            [Display(Name = "Spouse")]
            public string  spouse { get; set; }
            [Display(Name = "State")]          
            public string state { get; set; }
            [Required]
            [Display(Name = "Deposit Amount")]
            public int depositAmount { get; set; }
            [Required]
            [Display(Name="Deposit Currency")]
            public string depositCurrencyCode { get; set; }
            [Display(Name="Pick Up Time ")]
            public string pickUpTime { get; set; }
            public DateTime pickUpTime_Date { get; set; }
            public string pickUpTimeFormat { get; set; }
            [Display(Name ="Guest Phone")]
            public string guestPhone { get; set; }
            [Display(Name ="Guest Email")]
            public string guestEmail { get; set; }
            [Display(Name = "Invitation Number")]
            public string invitationNumber { get; set; }
            [Display(Name = "Additional Pick Up Notes")]
            public string pickUpNotes { get; set; }
            [Display(Name = "Gifts")]
            public string gift { get; set; }
            [Required]
            [Display(Name = "Confirmed")]
            public bool confirmed { get; set; }
            [Required]
            [Display(Name = "OPC")]
            public long? opcID { get; set; }
            public string spiOpcID { get; set; }
            [Display(Name = "Date Saved")]
            public DateTime dateSaved { get; set; }
            [Display(Name = "Saved By UserID")]
            public string savedByUser { get; set; }
            public Guid savedByUserID { get; set; }
            [Display(Name = "Last Modification")]
            public DateTime? dateModified { get; set; }
            [Display(Name = "Last Modification User")]
            public Guid? modifiedByUserID { get; set; }
            public string  modifiedByUser { get; set; }
            [Display(Name = "Terminal")]
            public int terminalID { get; set; }
            public string terminal { get; set; }
            [Display(Name = "Custumer")]
            public int? spiCustumerID { get; set; }
            [Required]
            [Display(Name = "Country")]
            public int? spiCountryID { get; set; }
            public string spiCountry { get; set; }
            [Required]
            [Display(Name = "Team")]
            public int? spiTeamID { get; set; }
            public string spiTeam { get; set; }
            [Required]
            [Display(Name = "Group")]
            public int? spiGroupID { get; set; }
            public string spiGroup { get; set; }
            [Required]
            [Display(Name="Category")]
            public int? spiCategoryID { get; set; }
            public string spiCategory { get; set; }

            [Display(Name = "Language")]
            public string spiLanguageID { get; set; }
            public string spiLanguange { get; set; }
            [Required]
            [Display(Name = "Hotel Pick Up")]
            public int? spiHotelID { get; set; }
            public string spiHotel { get; set; }
            [Required]
            [Display(Name="Sales Rooms")]
            public int? spiSalesRoomID { get; set; }
            public string spiSalesRoom { get; set; }
            [Display(Name = "Hotel Pick Up")]
            public Guid? spihotelPickUPID { get; set; }
            [Required]
            [Display(Name = "Location")]
            public int? spiLocationID { get; set; }
            [Required]
            [Display(Name = "First Name")]
            public string firstName { get; set; }
            [Display(Name = "Second Name")]
            public string secondName { get; set; }
            [Required]
            [Display(Name = "Last Name")]
            public string lastName { get; set; }
            [Display(Name = "Spouse First Name")]
            public string spouseFirstName { get; set; }
            [Display(Name = "Spouse Second Name")]
            public string spouseSecondName { get; set; }
            [Display(Name = "Spouse Last Name ")]
            public string spouseLastName { get; set; }
            [Display(Name = "Jalador")]
            public long? jaladorOpcID { get; set; }
            [Required]
            [Display(Name = "Program")]
            public int? programID { get; set; }
            [Required]
            [Display(Name = "Presentation Place")]
            public long? presentationPlaceID { get; set; }
            public string presentationPlace { get; set; }
            public string saved { get; set; }
            public string modified { get; set; }
            public bool premanifest { get; set; }
            public bool sendEmail { get; set; }
            public int emailLog { get; set; }
            [Required]
            [Display(Name = "Pick Up Type")]
            public int pickUpTypeID { get; set; }
            public List<SelectListItem>pickUpTypeList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.pickUpTypeList(); ;
                }
            }
            [Required]
            [Display(Name ="Deposit Pick Up")]
            public int depositPickUpID { get; set; }
            public List<SelectListItem> depositPickUpNotesList
            {
                get
                {
                    var list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Select One--"
                    });
                    list.Add(new SelectListItem()
                    { 
                        Value = "1",
                        Text = "Open"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Dont Manifest"
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "3",
                        Text = "Confirmed"
                    });
                    return list;
                }
            }
            [Required]
            [Display(Name="Shift")]
            public int shiftID { get; set; }
            public List<SelectListItem> shiftList
            {
                get
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Select One--",
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Morning",
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Afternoon",
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "3",
                        Text = "Night",
                    });
                    return list;
                }
            }
            [Display(Name="Manifest Folio")]
            public long manifestFolio { get; set; }
            [Display(Name="Adults")]
            public int adults { get; set; }
            [Display(Name="Childs")]
            public int? childs { get; set; }
            public List<invitationDeposits> invitationDeposits { get; set; }
            public List<SelectListItem> fillDrpPresentationPlaces { get; set; }
            public List<SelectListItem> fillDrpPrograms { get; set; }//
            public List<SelectListItem> fillDrpCurrencies { get; set; }//
            public List<SelectListItem> fillDrpOPC { get; set; }//
            public List<SelectListItem> fillDrpSPILanguage { get; set; }//
            public List<SelectListItem> fillDrpSPIGroups { get; set; }//
            public List<SelectListItem> fillDrpSPILocation { get; set; }//
            public List<SelectListItem> fillDrpSPIHotel  { get; set; }//
            public List<SelectListItem> fillDrpSPICountries { get; set; }//
            public List<SelectListItem> fillDrpSPISalesRooms { get; set; }//
            public List<SelectListItem> fillDrpJalador { get; set; }//
            public List<SelectListItem> fillDrpCategories
            {
                get
                {
                    //  return spiInvitationDataModel.spiInvitationCatalog.categoriesList();
                    return new List<SelectListItem>();
                }
            }
            //deposit Invitations
            public List<SelectListItem> currencieList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.currenciesList().OrderBy(z => z.Value).ToList();
                }
            }
            public List<SelectListItem> paymentsTypeList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.paymentTypeList().OrderBy(z => z.Value).ToList();
                }
            }
            public List<SysComponentsPrivilegesModel> Privileges { get; set; }
        }
        public class invitationDeposits
        {
                public long? invitationDepositID { get; set; }
                public decimal amount { get; set; }
                public int currencyID { get; set; }
                public string currency { get; set; }
                public int paymentTypeID { get; set; }
                public string paymentType { get; set; }
                public string ccReferenceNumber { get; set; }//ultimos 4 digitos de la tarjeta de credito
                public bool received { get; set; }
                public DateTime dateSaved { get; set; }
                public Guid? savedByUserID { get; set; }
                public string saveUser { get; set; }
                public DateTime? dateLastModification { get; set; }
                public Guid? modifiedByUserID { get; set; }
                public string modifiedByUser { get; set; }
                public bool? deleted { get; set; }
                public Guid? deletedByUserID { get; set; }
                public DateTime? deletedDateTime { get; set; }
                public string deleteByUser { get; set; }
                public Guid? invitationID { get; set; }
                public bool editDeposit { get; set; }
        }
        public class invitationModel
        {
            public Guid invitationIDModal { get; set; }
            [Display(Name = "Custumer ID")]
            public int? custumerIDModal { get; set; }                     
            [Display(Name = "First Name")]
            public string firstNameModal { get; set; }
            [Display(Name = "Second Name")]
            public string secondNameModal { get; set; }           
            [Display(Name = "Last Name")]
            public string lastNameModal { get; set; }
            [Display(Name = "Spouse First Name")]
            public string spouseFirstNameModal { get; set; }
            [Display(Name = "Spouse Second Name")]
            public string spouseSecondNameModal { get; set; }
            [Display(Name = "Spouse Last Name")]
            public string spouseLastNameModal { get; set; }

            [Display(Name = "Guest")]
            public string guestModal { get; set; }
            [Display(Name = "Spouse Name")]
            public string spouseNameModal { get; set; }
           
            [Display(Name = "Email")]
            public string guestEmailModal { get; set; }
            [Display(Name="Guest Phone")]
            public string guestPhoneModal { get; set; }
            [Display(Name= "State")]
            public string stateModal { get; set; }
        }
        public class guestToMatch
        {
            public int guestToMatchID { get; set; }
            public string customerName1 { get; set; }
            public string customerName2 { get; set; }
            public string city { get; set; }
            public string st { get; set; }
            public string country { get; set; }
            public DateTime? tourDate { get; set; }
            public string tourDateFormat { get; set; }
            public int tour { get; set; }
            public string salesCenter { get; set; }

            public string source { get; set; }
            public decimal? volumen { get; set; }
            public string match { get; set; }
            public string contractType { get; set; }
            public string contractStatus { get; set; }
            public string acountNo { get; set; }
            public bool matchUser { get; set; }
        }
        public class pickUpHotelInfo
        {
            public Guid hotelPickUpID { get; set; }
            public string description { get; set; }
            public string picture { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
        }       
        public class emailPreview
        {
            public int emailID { get; set; }
            public int programID { get; set; }
            public string emailTemplate { get; set; }
            public string email { get; set; }
        }
        public class PaymentTypes
        {
            public int paymenTypeID { get; set; }
            public string paymentType { get; set; }
        }
        public class DestinationName
        {
            public long terminalID { get; set; }
            public long destinationID { get; set; }
            public string destination { get; set; }
        }
    }

    public class SPIInvitationReport
    {
        public class searchInvitationModel
        {
            [Display(Name = "From Date")]
            public DateTime? fromDate { get; set; }
            [Display(Name = "To Date")]
            public DateTime? toDate { get; set; }
            [Display(Name="Shift")]
            public int? shift { get; set; }
            [Display(Name ="Presentation Place")]
            public int presentationPlaceID { get; set; }
            [Display(Name = "Program")]
            public int programID { get; set; }

            public List<SelectListItem> shiftList
            {
                get
                {

                    List<SelectListItem> list = new List<SelectListItem>();
                    list.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "--Select One--",
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "Morning",
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "2",
                        Text = "Afternoon",
                    });
                    list.Add(new SelectListItem()
                    {
                        Value = "3",
                        Text = "Night",
                    });
                    return list;
                }
            }
            public List<SelectListItem> presentationPlaceList
            {
                get
                {
                    return spiInvitationDataModel.spiInvitationCatalog.fillDrpPresentationPlaces();
                }
            }
            public List<SelectListItem> programList
            {
                get
                {
                    return MasterChartDataModel.LeadsCatalogs.FillDrpPrograms();
                }
            }
        }
        public class invitationReportResult
        {
            public List<invitationsReportModel> Result { get; set; }
            public List<spiInvitation.invitationDeposits> deposits { get; set; }
            [Display(Name = "Premanifest Date:")]
            public string premanifestDate { get; set; }
            [Display(Name = "Shift:")]
            public string shift { get; set; }
            [Display(Name = "User:")]
            public string premanifestUser { get; set; }
            public List<TotalByPaymentType> totalP { get; set; }
            public List<TotalByCurrencies> totalC { get; set; }
            //public decimal credit { get; set; }
            //public decimal cash { get; set; }
            //public List<Money> currencies { get; set; }
        }
        public class invitationsReportModel
        {
            public Guid invitationID { get; set; }
            public long? presentationPlaceID { get; set; }
            public string presentationPlace { get; set; }
            public int? programID { get; set; }
            public string program { get; set; }
            public int? groupID { get; set; }
            public string group { get; set; }
            public int? teamID { get; set; }
            public string team { get; set; }
            public string invitationFolio { get; set; }
            public string guest { get; set; }
            public int? spiHotelID { get; set; }
            public string spiHotel { get; set; }
            public long? opcID { get; set; }
            public string opc { get; set; }
            public string pickUpTime { get; set; }
            public int pickUpTypeID { get; set; }
            public string pickUpType { get; set; }
            public int depositPickUpID { get; set; }
            public string comments { get; set; }
            public string state { get; set; }
            public long jaladorID { get; set; }
            public string jalador { get; set; }
            public long manifestFolio { get; set; }
            public double pax { get; set; }
            public string dateSaved { get; set; }
            public string savedByUser { get; set; }
            public string dateModified { get; set; }
            public string modifiedByUser { get; set; }
            public List<spiInvitation.invitationDeposits> deposits { get; set; }

        }

        public class TotalByPaymentType
        {
            public int paymentTypeID { get; set; }
            public string paymentType { get; set; }
            public decimal amount { get; set; }
            public int currencyID { get; set; }
            public string currency { get; set; }
        }
        public class TotalByCurrencies
        {
            public decimal amount { get; set; }
            public int currencyID { get; set; }
            public string currency { get; set; }
        }
    }
}
