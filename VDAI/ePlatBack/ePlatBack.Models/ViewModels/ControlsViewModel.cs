using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Models.ViewModels
{
    public class FreeVacationViewModel
    {
        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Your_First_Name", ResourceType = typeof(Resources.Models.Controls.FreeVacationStrings))]
        public string FreeVacation_FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Your_Last_Name", ResourceType = typeof(Resources.Models.Controls.FreeVacationStrings))]
        public string FreeVacation_LastName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Your_Email", ResourceType = typeof(Resources.Models.Controls.FreeVacationStrings))]
        public string FreeVacation_Email { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Your_Phone", ResourceType = typeof(Resources.Models.Controls.FreeVacationStrings))]
        public string FreeVacation_Phone { get; set; }
    }

    public class GroupsFormViewModel
    {
        [Display(Name = "GroupsForm_Hotel", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_Hotel { get; set; }
        [Display(Name = "GroupsForm_NumberOfPassengers", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_NumberOfPassengers { get; set; }
        [Display(Name = "GroupsForm_FromDate", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_FromDate { get; set; }
        [Display(Name = "GroupsForm_ToDate", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_ToDate { get; set; }
        [Display(Name = "GroupsForm_Name", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_Name { get; set; }
        [Display(Name = "GroupsForm_Phone", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_Phone { get; set; }
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "GroupsForm_Email", ResourceType = typeof(Resources.Models.Controls.GroupsFormStrings))]
        public string GroupsForm_Email { get; set; }
    }

    public class ContactFormViewModel
    {
        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "First_Name", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Last_Name", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_LastName { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Phone", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_Phone { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Email", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_Email { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Confirm_Email", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_ConfirmEmail { get; set; }

        [Display(Name = "Time_To_Reach", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_TimeToReach { get; set; }

        [Required(ErrorMessageResourceName = "Required",
ErrorMessageResourceType = typeof(Resources.Models.Shared.ValidationStrings))]
        [Display(Name = "Comment", ResourceType = typeof(Resources.Models.Controls.ContactFormStrings))]
        public string Contact_Comment { get; set; }
    }

    public class FreeGetawayViewModel
    {
        [Display(Name = "Please type the next characters:")]
        public string FreeGetaway_ValidationString { get; set; }
        [Required(ErrorMessage = "This is important, to validate you are not a robot.")]
        public string FreeGetaway_ValidationStringResponse { get; set; }

        [Required(ErrorMessage = "Your First Name is Required.")]
        [Display(Name = "First Name")]
        public string FreeGetaway_FirstName { get; set; }

        [Required(ErrorMessage = "Your Last Name is Required.")]
        [Display(Name = "Last Name")]
        public string FreeGetaway_LastName { get; set; }

        [Required(ErrorMessage = "Your Day Phone Number is Required.")]
        [Display(Name = "Phone Day")]
        public string FreeGetaway_PhoneDay { get; set; }

        [Required(ErrorMessage = "Your Evening Phone Number is Required.")]
        [Display(Name = "Phone Evening")]
        public string FreeGetaway_PhoneEvening { get; set; }

        [Required(ErrorMessage = "Your Email Address is Required.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Address")]
        [Display(Name = "Email")]
        public string FreeGetaway_Email { get; set; }

        [Required(ErrorMessage = "Your Age is Required.")]
        [Display(Name = "Age")]
        public string FreeGetaway_Age { get; set; }
        public List<SelectListItem> AgeRanges
        {
            get
            {
                List<SelectListItem> ages = new List<SelectListItem>();
                ages.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "Select One"
                });
                ages.Add(new SelectListItem()
                {
                    Value = "Under 30",
                    Text = "Under 30"
                });
                ages.Add(new SelectListItem()
                {
                    Value = "30 to 70",
                    Text = "30 to 70"
                });
                ages.Add(new SelectListItem()
                {
                    Value = "70 +",
                    Text = "70 +"
                });

                return ages;
            }
        }

        [Required(ErrorMessage = "Your Marital Status is Required.")]
        [Display(Name = "Marital Status")]
        public string FreeGetaway_MaritalStatus { get; set; }
        public List<SelectListItem> MaritalStatus
        {
            get
            {
                List<SelectListItem> marital = new List<SelectListItem>();
                marital.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "Select One"
                });
                marital.Add(new SelectListItem()
                {
                    Value = "Cohabitating",
                    Text = "Cohabitating"
                });
                marital.Add(new SelectListItem()
                {
                    Value = "Divorced",
                    Text = "Divorced"
                });
                marital.Add(new SelectListItem()
                {
                    Value = "Married",
                    Text = "Married"
                });
                marital.Add(new SelectListItem()
                {
                    Value = "Single",
                    Text = "Single"
                });
                marital.Add(new SelectListItem()
                {
                    Value = "Widowed",
                    Text = "Widowed"
                });
                return marital;
            }
        }

        [Required(ErrorMessage = "A Security Question is needed.")]
        [Display(Name = "Security Question")]
        public string FreeGetaway_SecurityQuestion { get; set; }
        public List<SelectListItem> SecurityQuestions { 
            get {
                List<SelectListItem> questions = new List<SelectListItem>();
                questions.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "Select One"
                });
                questions.Add(new SelectListItem()
                {
                    Value = "1. Name of the street you grew up on?",
                    Text = "1. Name of the street you grew up on?"
                });
                questions.Add(new SelectListItem()
                {
                    Value = "2. Pet Name?",
                    Text = "2. Pet Name?"
                });
                questions.Add(new SelectListItem()
                {
                    Value = "3. Nickname?",
                    Text = "3. Nickname?"
                });
                questions.Add(new SelectListItem()
                {
                    Value = "4. Favourite Color?",
                    Text = "4. Favourite Color?"
                });
                questions.Add(new SelectListItem()
                {
                    Value = "5. Name of your favourite Superhero?",
                    Text = "5. Name of your favourite Superhero?"
                });

                return questions;
            } 
        }

        [Required(ErrorMessage = "A Security Answer is needed.")]
        [Display(Name = "Security Answer")]
        public string FreeGetaway_SecurityAnswer { get; set; }
    }

    public class RedeemMyPackageViewModel
    {
        [Required(ErrorMessage = "Your First Name is Required.")]
        [Display(Name = "First Name")]
        public string Redeem_FirstName { get; set; }

        [Required(ErrorMessage = "Your Last Name is Required.")]
        [Display(Name = "Last Name")]
        public string Redeem_LastName { get; set; }

        [Required(ErrorMessage = "Your Email Address is Required.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Address")]
        [Display(Name = "Email")]
        public string Redeem_Email { get; set; }

        [Required(ErrorMessage = "Your Phone Number is Required.")]
        [Display(Name = "Phone")]
        public string Redeem_Phone { get; set; }

        [Display(Name = "Certificate Number")]
        public string Redeem_CertificateNumber { get; set; }

        [Display(Name = "Comments")]
        public string Redeem_Comments { get; set; }
    }

    public class QuoteRequestViewModel
    {
        [Required(ErrorMessage = "Your First Name is Required.")]
        [Display(Name = "First Name")]
        public string QuoteRequest_FirstName { get; set; }

        [Required(ErrorMessage = "Your Last Name is Required.")]
        [Display(Name = "Last Name")]
        public string QuoteRequest_LastName { get; set; }

        [Required(ErrorMessage = "Your Email Address is Required.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Address")]
        [Display(Name = "Email")]
        public string QuoteRequest_Email { get; set; }

        [Required(ErrorMessage = "Your Phone Number is Required.")]
        [Display(Name = "Phone")]
        public string QuoteRequest_Phone { get; set; }

        [Display(Name = "Destination")]
        public string QuoteRequest_Destination { get; set; }
        public List<SelectListItem> Destinations
        {
            get
            {
                return ControlsDataModel.QuoteRequestDataModel.GetDestinationsListTextValue();
            }
        }

        [Display(Name = "Resort")]
        public string QuoteRequest_Resort { get; set; }
        public List<SelectListItem> Resorts
        {
            get
            {
                return ControlsDataModel.QuoteRequestDataModel.GetResortsListTextValue("");
            }
        }

        [Required(ErrorMessage = "Your Arrival Date is Required.")]
        [Display(Name = "Arrival Date")]
        public string QuoteRequest_Arrival { get; set; }

        [Required(ErrorMessage = "Your Departure Date is Required.")]
        [Display(Name = "Departure Date")]
        public string QuoteRequest_Departure { get; set; }

        [Required(ErrorMessage = "You need to specify the Number of Adults traveling.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "You need to specify the Number of Adults traveling.")]
        [Display(Name = "Number of Adults")]
        public int QuoteRequest_Adults { get; set; }

        [Required(ErrorMessage = "The Number of Children's field needs a value from 0 to n.")]
        [Display(Name = "Number of Children")]
        public int QuoteRequest_Children { get; set; }

        [Display(Name = "Time to be reached")]
        public string QuoteRequest_TimeToReach { get; set; }

        [Display(Name = "Comments")]
        public string QuoteRequest_Comments { get; set; }
    }

    public class SpecialPromoForCityViewModel
    {
        public string BannerAd { get; set; }
        public string PhoneDesktop { get; set; }
        public string PhoneMobile { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public class CategoriesViewModel
    {
        public string Banner { get; set; }
        IEnumerable<CategoryListItem> Categories { get; set; }
    }

    public class CategoryListItem
    {
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
    }

    public class BlockContentViewModel
    {
        public string Content { get; set; }
    }

    public class BlogContentViewModel
    {
        public string Content { get; set; }
    }
}
