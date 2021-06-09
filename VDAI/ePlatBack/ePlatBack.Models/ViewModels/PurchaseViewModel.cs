using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Models.ViewModels
{
    public class PurchaseProcessViewModel
    {
        public PageViewModel Page { get; set; }
        public Guid PurchaseID { get; set; }
        public string IP { get; set; }
        public string Browser { get; set; }
        public string OS { get; set; }
        public string PaymentsProviderName { get; set; }
        public string PaymentsProviderAccount { get; set; }
        public int PaymentsMerchantID { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "First_Name", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Last_Name", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Email", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Confirm_Email", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        [System.Web.Mvc.Compare("Email", ErrorMessageResourceName = "Not_Match", ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Address", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Zip_Code", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string ZipCode { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "City", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string City { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "State", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string State { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Country", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public int Country { get; set; }

        public List<SelectListItem> Countries
        {
            get
            {
                List<SelectListItem> countries = new List<SelectListItem>();
                countries.Add(new SelectListItem()
                {
                    Value = "",
                    Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Select_one
                });
                foreach (SelectListItem item in CountryDataModel.GetAllCountries())
                {
                    countries.Add(item);
                }
                return countries;
            }
        }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        [Display(Name = "Phone", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string Phone { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered mobile phone format is not valid.")]
        [Display(Name = "Mobile", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string Mobile { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Range(0, Int64.MaxValue, ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Staying_at_Place", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public long StayingAtPlaceID { get; set; }
        public List<SelectListItem> StayingAtPlaces
        {
            get
            {
                List<SelectListItem> places = new List<SelectListItem>();
                places.Add(new SelectListItem()
                {
                    Value = "",
                    Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Select_one,
                    Selected = true
                });
                places.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Other
                });
                long terminalid = Utils.GeneralFunctions.GetTerminalID();
                foreach (SelectListItem item in PlaceDataModel.GetResortsByTerminalDomains(terminalid.ToString()))
                {
                    places.Add(item);
                }
                
                return places;
            }
        }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Staying_at_Other", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string StayingAt { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "CC_Holder_Name", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string CCHolderName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Range(1, 10, ErrorMessageResourceName = "Select", ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "CC_Type", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public int CCType { get; set; }

        public List<SelectListItem> CCTypes
        {
            get
            {
                List<SelectListItem> creditcards = CreditCardsDataModel.GetAllCreditCardTypes();
                creditcards.Insert(0, new SelectListItem()
                {
                    Value = "",
                    Text = ePlatBack.Models.Resources.Models.Shared.SharedStrings.Select_one
                });
                return creditcards;
            }
        }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "CC_Number", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string CCNumber { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "CC_Expiration", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public int CCExpirationMonth { get; set; }     
        public int CCExpirationYear { get; set; }

        public List<SelectListItem> CCExpirationMonths
        {
            get
            {
                return new List<SelectListItem>(){
                    new SelectListItem(){
                        Value = "01",
                        Text = "01 | Jan"
                    },
                    new SelectListItem(){
                        Value = "02",
                        Text = "02 | Feb"
                    },
                    new SelectListItem(){
                        Value = "03",
                        Text = "03 | Mar"
                    },
                    new SelectListItem(){
                        Value = "04",
                        Text = "04 | Apr"
                    },
                    new SelectListItem(){
                        Value = "05",
                        Text = "05 | May"
                    },
                    new SelectListItem(){
                        Value = "06",
                        Text = "06 | Jun"
                    },
                    new SelectListItem(){
                        Value = "07",
                        Text = "07 | Jul"
                    },
                    new SelectListItem(){
                        Value = "08",
                        Text = "08 | Aug"
                    },
                    new SelectListItem(){
                        Value = "09",
                        Text  = "09 | Sep"
                    },
                    new SelectListItem(){
                        Value = "10",
                        Text = "10 | Oct"
                    },
                    new SelectListItem(){
                        Value = "11",
                        Text = "11 | Nov"
                    },
                    new SelectListItem(){
                        Value = "12",
                        Text = "12 | Dec"
                    }
                };
            }
        }

        public List<SelectListItem> CCExpirationYears
        {
            get
            {
                List <SelectListItem> years = new List<SelectListItem>();
                for (int i = 0; i < 10; i++)
                {
                    string year = DateTime.Now.AddYears(i).Year.ToString();
                    years.Add(new SelectListItem() {
                        Value = year,
                        Text = year
                    });
                }

                return years;
            }
        }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "CC_Security_Number", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string CCSecurityNumber { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public string Comments { get; set; }

        public bool isTrue { get { return true; } }

        [Required]
        [System.Web.Mvc.Compare("isTrue", ErrorMessageResourceName = "Accept",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Accept", ResourceType = typeof(Resources.Models.Shared.PurchaseProcess))]
        public bool Accept { get; set; }
        
        public List<PurchaseItem> Items { get; set; }
        public string ItemsJSON { get; set; }
        public decimal Total { get; set; }
        public decimal Savings { get; set; }
        public long PromoID { get; set; }
        public int CurrencyID { get; set; }
        public int PointOfSaleID { get; set; }
        public string CampaignTag { get; set; }
        public int TerminalID { get; set; }
        public string Terms
        {
            get
            {
                return PurchaseDataModel.GetTerms();
            }
        }
    }

    public class PurchaseItem
    {
        public long CartItemID { get; set; }
        public long ServiceID { get; set; }
        public string Service { get; set; }
        public long ServiceType { get; set; }
        public string ServiceDate { get; set; }
        public long? WeeklyAvailabilityID { get; set; }
        public string Schedule { get; set; }
        public string Airline { get; set; }
        public string Flight { get; set; }
        public long? HotelID { get; set; }
        public int? TransportationZoneID { get; set; }
        public bool? Round { get; set; }
        public string RoundAirline { get; set; }
        public string RoundFlightNumber { get; set; }
        public string RoundDate { get; set; }
        public string RoundMeetingTime { get; set; }
        public decimal Total { get; set; }
        public decimal PromoTotal { get; set; }
        public decimal Savings { get; set; }
        public decimal PromoSavings { get; set; }
        public string DateSaved { get; set; }
        public List<PurchaseItemDetail> Details { get; set; }
        public bool Deleted { get; set; }
        public long? PromoID { get; set; }
    }

    public class PurchaseItemDetail
    {
        public long PriceID { get; set; }
        public int PriceTypeID { get; set; }
        public long? ExchangeRateID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public bool Deleted { get; set; }
    }

    public class CartSettingsModel
    {
        public int PointOfSaleID { get; set; }
        public long TerminalID { get; set; }
        public int CatalogID { get; set; }
    }

    public class PaymentsProvider
    {
        public string ProviderName { get; set; }
        public string ProviderAccount { get; set; }
        public int? MerchantID { get; set; }
    }

    public class ProsaResponseModel
    {
        public string EM_Response { get; set; }
        public string EM_Total { get; set; }
        public string EM_OrderID { get; set; }
        public string EM_OrderIDX { get; set; }
        public string EM_Merchant { get; set; }
        public string EM_Store { get; set; }
        public string EM_Term { get; set; }
        public string EM_RefNum { get; set; }
        public string EM_Auth { get; set; }
        public string EM_Digest { get; set; }
        public string host { get; set; }
    }

    public class ConfirmationPageViewModel: PageViewModel
    {
        public Guid PurchaseID { get; set; }
        public string AuthCode { get; set; }
        public decimal? Total { get; set; }
        public string CurrencyCode { get; set; }
        public string MerchantName { get; set; }
    }

    public class SavePurchaseResponse
    {
        public Guid PurchaseID { get; set; }
        public string SessionID { get; set; }        
    }
}
