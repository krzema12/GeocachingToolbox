using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeocachingToolbox
{
    public class Location
    {
        public decimal Latitude { get; private set; }
		public decimal Longitude { get; private set; }

		public Location(decimal latitude, decimal longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public Location(int latitude, decimal latitudeMinutes,
			int longitude, decimal longitudeMinutes)
		{
			Latitude = (decimal)latitude + latitudeMinutes/60;
			Longitude = (decimal)longitude + longitudeMinutes/60;
		}

		public double DistanceInMetersTo(Location other)
		{
			// http://www.movable-type.co.uk/scripts/latlong.html
			var R = 6371000; // metres
			var φ1 = ToRadians((double)Latitude);
			var φ2 = ToRadians((double)other.Latitude);
			var Δφ = ToRadians((double)(other.Latitude - Latitude));
			var Δλ = ToRadians((double)(other.Longitude - Longitude));

			var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
					Math.Cos(φ1) * Math.Cos(φ2) *
					Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			var d = R * c;

			return d;
		}

		private double ToRadians(double degrees)
		{
			return degrees * Math.PI / 180.0;
		}
    }
}
