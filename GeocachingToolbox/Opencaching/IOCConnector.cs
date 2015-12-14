using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
	public interface IOCConnector
	{
		bool UseSsl { get; set; }

		void SetConsumerKeyAndSecret(string consumerKey, string consumerSecret);
		void SetTokens(string token, string tokenSecret);
		string GetURL(string method, Dictionary<string, string> args = null);
		string GetResponse(string url);
	}
}
