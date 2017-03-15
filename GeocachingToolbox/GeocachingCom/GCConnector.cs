using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
    public class GCConnector : IGCConnector
    {
        private const string UrlPrefix = "https://www.geocaching.com/";
        private WebBrowserSimulator webBrowser;

        public Task<string> GetPage(string url)
        {
            var usedUrl = prefixUrlIfNeeded(url);
            var response = webBrowser.GetRequestAsString(usedUrl);
            return response;
        }

        private static string prefixUrlIfNeeded(string url)
        {
            string usedUrl = url;
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                usedUrl = UrlPrefix + url;
            }
            return usedUrl;
        }

        public Task<HttpContent> GetContent(string fullUrl, IDictionary<string, string> getData)
        {
            var usedUrl = prefixUrlIfNeeded(fullUrl);
            return webBrowser.GetRequest(usedUrl, getData);
        }

        public Task<string> PostToPage(string url, IDictionary<string, string> parameters)
        {
            var usedUrl = prefixUrlIfNeeded(url);
            var response = webBrowser.PostRequest(usedUrl, parameters);
            return response;
        }

        public async Task<string> Login(string login, string password)
        {

            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "__EVENTTARGET", ""  },
                { "__EVENTARGUMENT", "" },
                { "ctl00$ContentBody$tbUsername", login },
                { "ctl00$ContentBody$tbPassword", password },
                { "ctl00$ContentBody$cbRememberMe", "on" },
                { "ctl00$ContentBody$btnSignIn", "Login" }
            };
            webBrowser = new WebBrowserSimulator();

            await webBrowser.PostRequest(UrlPrefix + "login/default.aspx?RESETCOMPLETE=Y&redir=https%3a%2f%2fwww.geocaching.com%2fmy%2fdefault.aspx", parameters);
            var response = await webBrowser.GetRequestAsString("https://www.geocaching.com/my/default.aspx");

            //var response = await webBrowser.PostRequest(UrlPrefix + "account/login/?returnUrl=https%3a%2f%2fwww.geocaching.com/my/default.aspx", parameters);

            //response = await webBrowser.GetRequestAsString(UrlPrefix + "my/default.aspx");
            return response;
        }
    }
}
