using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public abstract class Log
    {
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public ILoggable Thing { get; set; }
        public GeocacheLogType LogType { get; set; }

		public override string ToString()
		{
			return "Log from " + Date.ToString() + ": " + Comment;
		}
    }
}
