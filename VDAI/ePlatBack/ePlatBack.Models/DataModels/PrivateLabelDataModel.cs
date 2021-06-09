using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.DataModels;

namespace ePlatBack.Models.DataModels
{
    public class PrivateLabelDataModel
    {
        public static PrivateLabelViewModel.Catalog GetCatalog(int id, string culture)
        {
            ePlatEntities db =  new ePlatEntities();
            PrivateLabelViewModel.Catalog catalog = new PrivateLabelViewModel.Catalog() { 
                ParentCategories = new List<PrivateLabelViewModel.ParentCategory>()
            };
            ActivityDataModel adm = new ActivityDataModel();

            var CatalogQ = from c in db.tblCategories
                           where c.catalogID == id
                           && c.active == true
                           && c.parentCategoryID == null
                           && c.showOnWebsite == true
                           && !c.category.Contains("trans")
                           select new {
                               categoryID = c.categoryID,
                               category = c.tblCategoryDescriptions.FirstOrDefault(x => x.culture == culture).category
                           };

            foreach(var parentCategory in CatalogQ)
            {
                PrivateLabelViewModel.ParentCategory npc = new PrivateLabelViewModel.ParentCategory();
                npc.ParentCategoryID = parentCategory.categoryID;
                npc.ParentCategoryName = parentCategory.category;
                npc.Categories = new List<PrivateLabelViewModel.Category>();

                //get categories
                var CategoriesQ = from c in db.tblCategories
                                  where c.parentCategoryID == parentCategory.categoryID
                                  && c.active == true
                                  && c.showOnWebsite == true
                                  && c.tblCategories_Services.Count(
                                        x => x.tblServices.tblServiceDescriptions.FirstOrDefault(o => o.culture == culture).active == true
                                        && x.tblServices.tblProviders.isActive == true
                                        ) > 0
                                  orderby c.tblCategoryDescriptions.FirstOrDefault(x => x.culture == culture).category
                                  select new
                                  {
                                      categoryID = c.categoryID,
                                      category = c.tblCategoryDescriptions
                                      .FirstOrDefault(x => x.culture == culture).category
                                  };
                foreach(var category in CategoriesQ)
                {
                    PrivateLabelViewModel.Category nc = new PrivateLabelViewModel.Category();
                    nc.CategoryID = category.categoryID;
                    nc.CategoryName = category.category;
                    npc.Categories.Add(nc);
                }

                catalog.ParentCategories.Add(npc);
            }

            return catalog;
        }

        public static List<ActivityListItem>GetActivitiesForCategory(long id, string culture, long terminalid) {
            ActivityDataModel adm =  new ActivityDataModel();
            return adm.GetActivitiesList(id, null, culture, terminalid).OrderBy(x => x.OfferPrice).ToList(); 
        }
    }
}


//nc.Activities = 