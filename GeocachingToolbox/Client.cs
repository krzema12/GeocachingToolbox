using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public abstract class Client
    {
        public User User { get; protected set; }
        public abstract IEnumerable<T> GetFoundGeocaches<T>() where T : Log;
        public abstract void GetGeocacheDetails<T>(T geocache) where T : Geocache;
        public abstract void GetTrackableDetails<T>(T trackable) where T : Trackable;
        public abstract IEnumerable<T> GetNearestGeocaches<T>(Location location) where T : Geocache;
        public abstract void PostGeocacheLog<T>(T geocache, GeocacheLogType logType, DateTime date, string description) where T : Geocache;
        public abstract void PostTrackableLog<T>(T trackable, TrackableLogType logType, DateTime date, string description) where T : Trackable;
    }
}
