using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
    public class WebBrowserSimulator
    {
        private CookieContainer m_CookieContainer = new CookieContainer();

        public async Task<string> GetRequestAsString(string url, IDictionary<string, string> getData = null)
        {
            var data = await Request(url, HttpMethod.Get, getData);
            return await data.ReadAsStringAsync();
        }

        public async Task<HttpContent> GetRequest(string url, IDictionary<string, string> getData = null)
        {
            return await Request(url, HttpMethod.Get, getData);
        }

        public async Task<string> PostRequest(string url, IDictionary<string, string> postData)
        {
            var data = await Request(url, HttpMethod.Post, postData);
            return await data.ReadAsStringAsync();
        }

        private async Task<HttpContent> Request(string url, HttpMethod method, IDictionary<string, string> data = null)
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

            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                CookieContainer = m_CookieContainer
            };
            if (url.Contains("login"))
                httpHandler.AllowAutoRedirect = false;

            httpHandler.UseCookies = true;
            var httpClient = new HttpClient(httpHandler);

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/46.0");
            httpClient.DefaultRequestHeaders.Add("Accept-Charset", "utf-8,iso-8859-1;q=0.8,utf-16;q=0.8,*;q=0.7");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,*;q=0.9");

            var httpResponse = method == HttpMethod.Post
                ? await httpClient.PostAsync(url, new FormUrlEncodedContent(data))
                : await httpClient.GetAsync(url);

            //httpResponse.EnsureSuccessStatusCode();
            m_CookieContainer = httpHandler.CookieContainer;
            return httpResponse.Content;

            // return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
