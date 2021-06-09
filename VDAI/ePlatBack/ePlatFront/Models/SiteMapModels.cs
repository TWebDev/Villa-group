using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Extensibility;
using ePlatBack.Models;

namespace ePlatBack.Models.DataModels
{
    public class SiteMap : DynamicNodeProviderBase
    {
        ePlatEntities db = new ePlatEntities();
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection()
        {
            var returnValue = new List<DynamicNode>();

            var terminalID = Utils.GeneralFunctions.GetTerminalID();
            var culture = Utils.GeneralFunctions.GetCulture();
            var SeoItems = from item in db.tblSeoItems
                           where item.terminalID == terminalID
                           && item.culture == culture
                           && item.friendlyUrl != "/"
                           orderby item.friendlyUrl
                           select item.friendlyUrl;

            foreach (string url in SeoItems)
            {
                DynamicNode dn = new DynamicNode();
                dn.Route = "Router";
                dn.Action = "Router";
                dn.RouteValues.Add("path", url.Substring(1));
                dn.LastModifiedDate = Convert.ToDateTime(DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + "01");
                dn.ChangeFrequency = ChangeFrequency.Monthly;
                int level = url.Substring(1).Split('/').Length ;
                if(level == 1){
                    dn.UpdatePriority = UpdatePriority.Absolute_090;
                }
                else if (level>= 2)
                {
                    dn.UpdatePriority = UpdatePriority.Absolute_080;
                }

                if (url != "/")
                {
                    returnValue.Add(dn);
                }
            }

            return returnValue;
        }

        public override CacheDescription GetCacheDescription()
        {
            return new CacheDescription("StoreDetailsDynamicNodeProvider")
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };
        }
    }
}
