using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public abstract class Client
    {
        public User User { get; protected set; }
        public abstract Task<IEnumerable<T>> GetFoundGeocachesAsync<T>(CancellationToken ct = default(CancellationToken)) where T : Log;
        public abstract Task GetGeocacheDetailsAsync<T>(T geocache, CancellationToken ct = default(CancellationToken)) where T : Geocache;
        public abstract Task<Geocache> GetGeocacheDetailsAsync(string cacheCode, CancellationToken ct = default(CancellationToken));
        public abstract Task GetTrackableDetailsAsync<T>(T trackable) where T : Trackable;
        public abstract Task<IEnumerable<T>> GetNearestGeocachesAsync<T>(Location location, CancellationToken ct = default(CancellationToken)) where T : Geocache;
        public abstract Task<IEnumerable<T>> GetGeocachesFromMap<T>(Location topLeft, Location bottomRight, CancellationToken cancellationToken = default(CancellationToken)) where T : Geocache;
        public abstract Task PostGeocacheLogAsync<T>(T geocache, GeocacheLogType logType, DateTime date, string description, CancellationToken ct = default(CancellationToken)) where T : Geocache;
        public abstract Task PostTrackableLogAsync<T>(T trackable, TrackableLogType logType, DateTime date, string description) where T : Trackable;
    }
}
