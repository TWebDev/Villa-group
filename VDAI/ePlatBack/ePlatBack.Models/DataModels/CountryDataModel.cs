
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ePlatBack.Models.DataModels
{
    public class CountryDataModel
    {
        public static List<SelectListItem> GetAllCountries()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> list = new List<SelectListItem>();
            var query = from c in db.tblCountries
                        select new { c.countryID, c.country };
            foreach (var i in query)
                list.Add(new SelectListItem() { Value = i.countryID.ToString(), Text = i.country });            
            return list;
        }

    }

    public class CompanyTypeDataModel
    {
        public static List<SelectListItem>GetAllCompanyType()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> listTypeCompany = new List<SelectListItem>();
            var query = from Types in db.tblCompanyTypes
                        select new
                        {
                            Types.companyTypeID,
                            Types.companyType
                        }; 
            foreach(var x in query)
            {
                listTypeCompany.Add(new SelectListItem() { Value = x.companyTypeID.ToString(), Text = x.companyType });
            }

            return listTypeCompany;
        }
    }
}
