using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePlatBack.Models.DataModels
{
    public class ApiValidationDataModel
    {
        public static bool IsValid(string ip, string referrer, string username)
        {
            bool valid = false;
            if (ip == "::1")
            {
                valid = true;
            }
            else
            {
                //obtener userid
                ePlatEntities db = new ePlatEntities();
                var userID = (from u in db.AspNetUsers
                              where u.UserName == username
                              select u.Id).FirstOrDefault();

                if (userID != null)
                {
                    if (referrer == null)
                    {
                        // means api client is access from desktop 
                        var ValidIPs = (from p in db.tblApiAuthorizedIPs
                                        where p.identityUserID == userID
                                        && (p.ipv4 == ip || p.ipv4 == "*")
                                        select p.ipv4).Count();

                        if (ValidIPs > 0)
                        {
                            //ip válida
                            valid = true;
                        }
                    }
                    else
                    {
                        //access from browser
                        var ValidDomains = (from d in db.tblApiAuthorizedDomains
                                            where (d.domain == referrer || d.domain == "*")
                                            && d.identityUserID == userID
                                            select d.domain).Count();

                        if (ValidDomains > 0)
                        {
                            valid = true;
                        }
                    }
                }
            }            

            return valid;
        }
    }
}
