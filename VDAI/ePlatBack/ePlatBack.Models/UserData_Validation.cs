using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models
{
    public partial class UserData_Validation
    {
        
        public int userProfileID { get; set; }
        public Guid userID { get; set; }
        [Required(ErrorMessage = "*")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "*")]
        public string lastName { get; set; }
        [Required(ErrorMessage = "*")]
        public string SPIUserName { get; set; }
    }
}