using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ePlatBack.Models.ViewModels
{
    public class StructureViewModel
    {
        public List<UserItem> Users { get; set; }
    }

    public class UserItem {
        public Guid UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Destination { get; set; }
        public string WorkGroups { get; set; }
        public List<UserItem> Users { get; set; }
    }
}
