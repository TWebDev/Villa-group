using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ePlatBack.Models.ViewModels
{
    public class WikiFieldViewModel
    {
        public string FieldName { get; set; }
        public long SysComponentID { get; set; }
        public List<WikiCulture> Wikis { get; set; }
        public List<Culture> Cultures { 
            get {
                List<Culture> cultures = new List<Culture>();
                cultures.Add(new Culture()
                {
                    CultureCode = "en-US",
                    CultureName = "English"
                });
                cultures.Add(new Culture()
                {
                    CultureCode = "es-MX",
                    CultureName = "Español"
                });

                return cultures;
            }
        }

        public class Culture
        {
            public string CultureCode { get; set; }
            public string CultureName { get; set; }
        }

        public class WikiCulture 
        {
            public string Culture { get; set; }
            public List<Post> HowTo { get; set; }
            public List<Post> Glosary { get; set; }
        }

        public class Post
        {
            public long WikiFieldID { get; set; }
            public int Type { get; set; }
            public string Content { get; set; }
            public string User { get; set; }
            public DateTime DateTime { get; set; }
            public string Culture { get; set; }
            public long SysComponentID { get; set; }
            public bool Own { get; set; }
        }
    }
}
