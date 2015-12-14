using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeocachingToolbox.GeocachingCom
{
	public class GCConnector : IGCConnector
	{
		private const string UrlPrefix = "http://www.geocaching.com/";
		private WebBrowserSimulator webBrowser;

		public string GetPage(string url)
		{
			var response = webBrowser.GetRequest(UrlPrefix + url);
			return response;
		}

		public string PostToPage(string url, IDictionary<string, string> parameters)
		{
			var response = webBrowser.PostRequest(UrlPrefix + url, parameters);
			return response;
		}

		public string Login(string login, string password)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>
			{
				{ "__EVENTTARGET", ""  },
				{ "__EVENTARGUMENT", "" },
				{ "ctl00$ContentBody$tbUsername", login },
				{ "ctl00$ContentBody$tbPassword", password },
				{ "ctl00$ContentBody$cbRememberMe", "on" },
				{ "ctl00$ContentBody$btnSignIn", "Sign In" }
			};

			webBrowser = new WebBrowserSimulator();
            var response = webBrowser.PostRequest(UrlPrefix + "login/default.aspx?RESETCOMPLETE=Y&redir=http%3a%2f%2fwww.geocaching.com%2fmy%2fdefault.aspx%3f", parameters);

			return response;
		}
	}
}
