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
        public abstract Task<IEnumerable<T>> GetFoundGeocachesAsync<T>() where T : Log;
        public abstract Task GetGeocacheDetailsAsync<T>(T geocache) where T : Geocache;
        public abstract Task GetTrackableDetailsAsync<T>(T trackable) where T : Trackable;
        public abstract Task<IEnumerable<T>> GetNearestGeocachesAsync<T>(Location location) where T : Geocache;
        public abstract Task PostGeocacheLogAsync<T>(T geocache, GeocacheLogType logType, DateTime date, string description) where T : Geocache;
        public abstract Task PostTrackableLogAsync<T>(T trackable, TrackableLogType logType, DateTime date, string description) where T : Trackable;
    }
}
