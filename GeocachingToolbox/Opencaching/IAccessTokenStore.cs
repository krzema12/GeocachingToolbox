using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeocachingToolbox.Opencaching
{
	public interface IAccessTokenStore
	{
		bool Populated { get; set; }
		string Token { get; set; }
		string TokenSecret { get; set; }

		void SetValues(string token, string tokenSecret);
	}
}
