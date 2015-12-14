using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
	public interface ApiAccessKeys
	{
		string ConsumerKey { get; }
		string ConsumerSecret { get; }
	}
}
