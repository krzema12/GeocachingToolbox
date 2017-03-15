using System;

namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    public class Viewport
    {
        public readonly Location bottomLeft;
        public readonly Location topRight;

        public Viewport(Location point1, Location point2)
        {
            bottomLeft = new Location(Math.Min(point1.Latitude, point2.Latitude), Math.Min(point1.Longitude, point2.Longitude));
            topRight = new Location(Math.Max(point1.Latitude, point2.Latitude), Math.Max(point1.Longitude, point2.Longitude));
        }

        /**
         * Check whether a point is contained in this viewport.
         *
         * @param point
         *            the coordinates to check
         * @return true if the point is contained in this viewport, false otherwise or if the point contains no coordinates
         */
        public bool Contains(Location coords)
        {
            return coords != null
                   && coords.Longitude >= bottomLeft.Longitude
                   && coords.Longitude <= topRight.Longitude
                   && coords.Latitude >= bottomLeft.Latitude
                   && coords.Latitude <= topRight.Latitude;
        }

    }
}