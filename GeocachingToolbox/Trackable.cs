using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
	public abstract class Trackable : ILoggable
	{
		public string TrackingCode { get; set; }
	}
}
