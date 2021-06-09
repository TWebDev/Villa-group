using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ePlatBack.Models.ViewModels
{
    public class PrivateLabelViewModel
    {
        public class Catalog
        {
            public List<ParentCategory> ParentCategories { get; set; }
        }

        public class ParentCategory
        {
            public long ParentCategoryID { get; set; }
            public string ParentCategoryName { get; set; }
            public List<Category> Categories { get; set; }
        }

        public class Category
        {
            public long CategoryID { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
