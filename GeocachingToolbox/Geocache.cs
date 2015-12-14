using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public abstract class Geocache : ILoggable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Hint { get; set; }
		public GeocacheType Type { get; set; }
		public GeocacheStatus Status { get; set; }
		public GeocacheSize Size { get; set; }
		public float Difficulty { get; set; }
		public float Terrain { get; set; }
        public DateTime DateHidden { get; set; }
        public Location Waypoint { get; set; }
		public User Owner { get; set; }

		public override string ToString()
		{
			return string.Format("[{0}] {1} ({2}/{3})", Code, Name, Difficulty, Terrain);
		}
    }
}
