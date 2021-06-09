using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ePlatBack.Models;

namespace ePlatFront.Models
{
    public class UrlModels
    {
        public static string GetFriendlyUrl()
        {
            string url = HttpContext.Current.Request.RawUrl;
            if (url.LastIndexOf('?') >= 0)
            {
                url = url.Remove(url.LastIndexOf('?'));
            }
            return url;
        }

        public static string GetDomain()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        public static string GetDefaultPage()
        {
            ePlatEntities db = new ePlatEntities();
            string domain = GetDomain();
            var terminalDomain = from d in db.tblTerminalDomains 
                                 where d.domain == domain
                                 select d;

            if (terminalDomain.Count() > 0)
            {
                return GetRealUrl(terminalDomain.First().defaultPage, GetVirtualParams());
            }
            else
            {
                return "/Home";
            }
        }

        public static string GetVirtualParams()
        {
            return HttpContext.Current.Request.QueryString.ToString();
        }

        public static string GetRealUrl(string virtualUrl, string parameters)
        {
            ePlatEntities db = new ePlatEntities();
            string url = "/";

            if (virtualUrl.IndexOf("sitemap.xml") >= 0)
            {
                url = "/sitemap.xml";
            }
            else
            {
                //buscar la ruta en la base de datos
                UrlModels urlM = new UrlModels();
                url = urlM.FindUrl(virtualUrl, parameters);
            }
            return url;
        }

        public string FindUrl(string virtualUrl, string parameters)
        {
            string path = "";
            ePlatEntities db = new ePlatEntities();
            string domain = GetDomain();
            var seoUrl = from s in db.tblSeoItems
                          where s.friendlyUrl == virtualUrl && (from x in db.tblTerminalDomains where x.domain == domain select x.terminalID).Contains(s.terminalID)
                          select s.url;


            if (seoUrl.Count() > 0)
            {
                string realUrl = seoUrl.First();
                string realParam = seoUrl.First().ToString().Substring(seoUrl.First().ToString().IndexOf('?') + 1);

                string[] s = parameters.Split('&');
                string[] r = realParam.Split('&');
                string virtualParams = "";
                for (int i = 0; i < s.Length; i++)
                {
                    bool exists = false;
                    for (int j = 0; j < r.Length; j++)
                    {
                        if (s[i] == r[j])
                        {
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        if (virtualParams != string.Empty)
                        {
                            virtualParams += "&" + s[i];
                        }
                        else
                        {
                            virtualParams += s[i];
                        }
                    }
                }
                if (virtualParams != string.Empty)
                {
                    if (seoUrl.First().ToString().IndexOf("?") >= 0)
                    {
                        if (realUrl.IndexOf(virtualParams) < 0)
                        {
                            if (virtualParams.IndexOf("&") != 0)
                            {
                                realUrl += "&" + virtualParams;
                            }
                            else
                            {
                                realUrl += virtualParams;
                            }
                        }
                    }
                    else
                    {
                        realUrl += "?" + virtualParams;
                    }
                }
                path = realUrl;
            }
            else // si no existe transferir la misma ruta
            {
                virtualUrl = "/Not-Found";
                if (parameters != string.Empty)
                {
                    path = virtualUrl + "?" + parameters;
                }
                else
                {
                    path = virtualUrl;
                }
            }

            return path;
        }

        public static string GetMasterPageUrl()
        {
            string domain = GetDomain();
            ePlatEntities db = new ePlatEntities();
            var masterUrl = from p in db.tblTerminalDomains
                             where p.domain == domain
                             select p.defaultMasterPage;

            if (masterUrl == null)
            {
                return "~/Views/Shared/_LayoutEfe.cshtml";
            }
            else
            {
                if (masterUrl.FirstOrDefault() != null && masterUrl.FirstOrDefault() != "")
                {
                    return "~" + masterUrl.First();
                }
                else
                {
                    return "~/Views/Shared/_LayoutEfe.cshtml";
                }                
            }           
        }

        public static string RedirectionUrlRequired(string friendlyUrl)
        {
            ePlatEntities db = new ePlatEntities();
            var SeoItem = (from s in db.tblSeoItems
                          where s.friendlyUrl == friendlyUrl
                          select s).FirstOrDefault();
            if (SeoItem != null)
            {
                if (SeoItem.url.IndexOf("http") >= 0)
                {
                    return SeoItem.url;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static Boolean IsWWW(string path)
        {
            if (path.IndexOf("/") >= 0)
            {
                path = path.Remove(path.IndexOf('/'));
            }
            Int32 pointsCounter = 0;
            if (path.IndexOf('.') >= 0)
            {
                pointsCounter += 1;
                path = path.Remove(path.LastIndexOf('.'));
            }
            if (path.IndexOf('.') >= 0)
            {
                pointsCounter += 1;
            }
            if (pointsCounter <= 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}