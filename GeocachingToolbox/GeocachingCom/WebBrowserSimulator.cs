using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace GeocachingToolbox.GeocachingCom
{
    public class WebBrowserSimulator
    {
        private CookieContainer m_CookieContainer = new CookieContainer();

        public string GetRequest(string url, IDictionary<string, string> getData = null)
        {
            return Request(url, HttpMethod.Get, getData);
        }

        public string PostRequest(string url, IDictionary<string, string> postData)
        {
            return Request(url, HttpMethod.Post, postData);
        }

        private string Request(string url, HttpMethod method, IDictionary<string, string> data = null)
        {
            if (method == HttpMethod.Get && data != null && data.Count > 0)
            {
                url += "?";

                foreach (var entry in data)
                {
                    url += Uri.EscapeDataString(entry.Key) + "="
                        + Uri.EscapeDataString(entry.Value) + "&";
                }
            }

            var handler = new HttpClientHandler { AllowAutoRedirect = true, CookieContainer = m_CookieContainer };
            var newClient = new HttpClient(handler);
            newClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0");

            HttpResponseMessage httpResponse = null;
            httpResponse = method == HttpMethod.Post ? newClient.PostAsync(url, new FormUrlEncodedContent(data)).Result : newClient.GetAsync(url).Result;
            m_CookieContainer = handler.CookieContainer;

            string responseString = httpResponse.Content.ReadAsStringAsync().Result;

            return responseString;
        }
    }
}
