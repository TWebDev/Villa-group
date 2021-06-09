using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ePlatBack.Models.ViewModels
{
    public class PromoViewModel
    {
        public string MainTag { get; set; }
        public string TitleTag { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string TagColor { get; set; }
        public string TextColor { get; set; }
    }

    public class ApplicablePromo
    {
        public long PromoID { get; set; }
        public string ServiceIDs { get; set; }
        public string Promo { get; set; }
        public int PromoTypeID { get; set; }
        public string PromoType { get; set; }
        public decimal Discount { get; set; }
        public string DiscountType { get; set; }
        public bool Apply { get; set; }
        public string Condition { get; set; }
        public bool ApplyOnPerson { get; set; }
        public string PromoCode { get; set; }
    }
}
