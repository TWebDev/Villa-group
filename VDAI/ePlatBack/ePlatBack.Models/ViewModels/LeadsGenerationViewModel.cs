using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace ePlatBack.Models.ViewModels
{
    public class LeadsGenerationViewModel
    {
        public Guid? TransactionID { get; set; }
        [Required(ErrorMessage = "Please type your First Name.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please type your Last Name.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please type your Email.")]
        [RegularExpression("^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Invalid Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        [Required(ErrorMessage = "Please type your Mobile Phone.")]
        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }
        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }
        [Display(Name = "Time to Reach")]
        public string TimeToReach { get; set; }
        public List<SelectListItem> Countries { get; set; }
    }

    public class AssignationItem
    {
        public Guid UserID { get; set; }
        public int Assigned { get; set; }
    }
}
