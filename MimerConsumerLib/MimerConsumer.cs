using MimerConsumerLib.DTOs;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimerConsumerLib
{
	/// <summary>
	/// https://www.dr.dk/tjenester/mimer/
	/// https://developer.dr.dk/docs/apis/mimer.html
	/// https://www.dr.dk/tjenester/mimer/help/resources/v3
	/// </summary>
	public class MimerConsumer
    {
        private const string ApiKey = "MimerConsumerConsoleApplication";
        private const string BaseUrl = "https://www.dr.dk";
        private const string ArticlesUrl = "tjenester/mimer/api/v3/articles/latest.json";
        private const string SitesUrl = "tjenester/mimer/api/v3/sites.json";
        private const string FirstPageEditorsBody = "forside@dr.dk";

        private readonly RestClient Client;

        public MimerConsumer()
        {
           Client = new RestClient(BaseUrl);
        }

        public List<Article> GetLatestArticles()
        {
            RestRequest articlesRequest = new RestRequest(ArticlesUrl, Method.GET);
            articlesRequest.AddHeader("X-Application-Name", String.Format("MimerConsumer: {0}", ApiKey));
            RestSharp.IRestResponse<ArticlesRootObject> response = Client.Execute<ArticlesRootObject>(articlesRequest);
            ArticlesRootObject articlesRoot = response.Data;
            return articlesRoot?.data ?? new List<Article>();
        }

        public List<Article> GetNLatestArticles(int n)
        {
            if (n <= 0)
                return new List<Article>();

            List<Article> latestArticles = GetLatestArticles();
            return latestArticles
                .Take(n)
                .ToList();
        }

        public List<Site> GetSites()
        {
            RestRequest sitesRequest = new RestRequest(SitesUrl, Method.GET);
            sitesRequest.AddHeader("X-Application-Name", String.Format("MimerConsumer: {0}", ApiKey));
            RestSharp.IRestResponse<SitesRootObject> response = Client.Execute<SitesRootObject>(sitesRequest);
            SitesRootObject sitesRoot = response.Data;
            return sitesRoot?.data ?? new List<Site>();
        }

        public List<Site> GetFrontPageEditorsSites()
        {
            List<Site> sites = GetSites();
            List<Site> frontPageSites = sites
                .Where(site => site.site_info_collection.body == FirstPageEditorsBody)
                .ToList();
            return frontPageSites;
        }
    }
}
