using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimerConsumerLib.DTOs
{
    public class Image
    {
        public string id { get; set; }
        public string title { get; set; }
        public string mime_type { get; set; }
        public string url { get; set; }
        public string alt_text { get; set; }
    }

    public class Article
    {
        public string urn { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public bool published { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string url { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string changed_date { get; set; }
        public bool date_hidden { get; set; }
        public List<Image> images { get; set; }
        public List<object> tags { get; set; }
    }

    public class ArticlesRootObject
    {
        public string status { get; set; }
        public List<Article> data { get; set; }
        public DateTime timestamp { get; set; }
    }
}
