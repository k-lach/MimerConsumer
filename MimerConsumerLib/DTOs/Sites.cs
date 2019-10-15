using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimerConsumerLib.DTOs
{
    public class SiteInfoCollection
    {
        public string title { get; set; }
        public string author { get; set; }
        public string body { get; set; }
    }

    public class Site
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public SiteInfoCollection site_info_collection { get; set; }

        public string GetTitle()
        {
            if (String.IsNullOrWhiteSpace(title))
                return name;
            return title;
        }
    }

    public class SitesRootObject
    {
        public string status { get; set; }
        public List<Site> data { get; set; }
        public DateTime timestamp { get; set; }
    }
}
