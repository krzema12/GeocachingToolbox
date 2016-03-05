using OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GeocachingToolbox.Opencaching
{
    public class OCConnector : IOCConnector
    {
        public bool UseSsl { get; set; }
        private string _installationUrl;
        private string _consumerKey = "";
        private string _consumerSecret = "";
        private string _token = "";
        private string _tokenSecret = "";

        public OCConnector(string installationUrl)
        {
            _installationUrl = installationUrl;
        }

        public void SetConsumerKeyAndSecret(string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        public void SetTokens(string token, string tokenSecret)
        {
            _token = token;
            _tokenSecret = tokenSecret;
        }

        public string GetURL(string method, Dictionary<string, string> args = null)
        {
            var oauth = new OAuthBase();

            if (args == null)
            {
                args = new Dictionary<string, string>();
            }

            var argPairsEncoded = new List<string>();

            foreach (var pair in args)
            {
                argPairsEncoded.Add(oauth.UrlEncode(pair.Key) + "=" + oauth.UrlEncode(pair.Value));
            }

            // Normalization of the installation URL.
            if (_installationUrl.IndexOf("http://") == -1)
            {
                _installationUrl = "http://" + _installationUrl;
            }

            if (_installationUrl[_installationUrl.Length - 1] != '/')
            {
                _installationUrl += "/okapi/";
            }

            string method_url = _installationUrl + method;

            if (UseSsl)
            {
                method_url = method_url.Replace("http://", "https://");
            }

            string url = method_url;
            string argsString = "";

            if (argPairsEncoded.Count > 0)
            {
                argsString += string.Join("&", argPairsEncoded);
                url += "?" + argsString;
            }

            if (_consumerKey == "")
            {
                return url;
            }

            string timestamp = oauth.GenerateTimeStamp();
            string nonce = oauth.GenerateNonce();
            string normalized_url;
            string normalized_params;
            string signature = oauth.GenerateSignature(new System.Uri(url), _consumerKey,
                _consumerSecret, _token, _tokenSecret, "GET", timestamp, nonce, out normalized_url,
                out normalized_params);

            url = method_url + "?" + normalized_params + "&oauth_signature=" + HttpUtility.UrlEncode(signature);

            return url;
        }

        public static string ReadResponse(WebResponse response)
        {
            if (response == null)
            {
                return "";
            }

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            var s = reader.ReadToEnd();
            return s;
        }

        public string GetResponse(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 15000;
                request.Proxy = null;

                using (WebResponse response = request.GetResponse())
                {
                    return ReadResponse(response);
                }
            }
            catch (UriFormatException)
            {
                throw new WebException("Check your installation URL.");
            }
        }
    }
}
