using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GeocachingToolbox.GeocachingCom
{
    public class WebBrowserSimulator
    {
        private CookieCollection cookies = new CookieCollection();

        public string GetRequest(string url, IDictionary<string, string> getData = null)
        {
            return Request(url, WebRequestMethods.Http.Get, getData);
        }

        public string PostRequest(string url, IDictionary<string, string> postData)
        {
            return Request(url, WebRequestMethods.Http.Post, postData);
        }

        private string Request(string url, string method, IDictionary<string, string> data = null)
        {
            if (method == WebRequestMethods.Http.Get && data != null && data.Count > 0)
            {
                url += "?";

                foreach (var entry in data)
                {
                    url += Uri.EscapeDataString(entry.Key) + "="
                        + Uri.EscapeDataString(entry.Value) + "&";
                }
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "User-Agent=Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) "
                + "Gecko/20100101 Firefox/37.0";
            request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);
            request.Method = method;
            request.KeepAlive = true;
            request.Headers.Add("Accept-Charset: utf-8,iso-8859-1;q=0.8,utf-16;q=0.8,*;q=0.7");
            request.Headers.Add("Accept-Language: en-US,*;q=0.9");
            request.AllowWriteStreamBuffering = true;
            request.ProtocolVersion = HttpVersion.Version11;
            request.AllowAutoRedirect = true;
            request.ContentType = "application/x-www-form-urlencoded";
            // TODO: handle compressed responses
            //request.Headers.Add("Accept-Encoding: gzip,deflate");

            if (method == WebRequestMethods.Http.Post)
            {
                string postDataAsString = "";

                foreach (var entry in data)
                {
                    // TODO remove this temporary workaround
                    postDataAsString += HttpUtility.UrlEncode(entry.Key) + "="
                        + HttpUtility.UrlEncode(entry.Value) + "&";
                }

                byte[] postDataAsByteArray = Encoding.ASCII.GetBytes(postDataAsString);
                request.ContentLength = postDataAsByteArray.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postDataAsByteArray, 0, postDataAsByteArray.Length);
                requestStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            cookies = response.Cookies;

            string responseString;

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseString = sr.ReadToEnd();
            }

            return responseString;
        }
    }
}
