using System;
using System.Collections.Generic;
using GeocachingToolbox.GeocachingCom;
using GeocachingToolbox.GeocachingCom.MapFetch;

namespace GeocachingToolbox
{
    public abstract class Geocache : ILoggable
    {
        private UncertainProperty<Location> m_uncertainWaypoint = new UncertainProperty<Location>(new Location(0d, 0d), 0);

        private UncertainProperty<GeocacheType> m_uncertainGeocacheType =
            new UncertainProperty<GeocacheType>(GeocacheType.Unknown, 0);
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Code { get; set; }
        public string Hint { get; set; }

        public bool Own { get; set; }

        public Geocache()
        {
            Name = "";
            Description = "";
            ShortDescription = "";
            Code = "";
            Hint = "";
        }

        public GeocacheType Type
        {
            get { return m_uncertainGeocacheType.getValue(); }
            set { m_uncertainGeocacheType = new UncertainProperty<GeocacheType>(value, 999); }
        }

        public void SetGeocacheType(GeocacheType type, int certainty = 999)
        {
            m_uncertainGeocacheType = new UncertainProperty<GeocacheType>(type, certainty);
        }

        public GeocacheStatus Status { get; set; }
        public GeocacheSize Size { get; set; }
        public float Difficulty { get; set; }
        public float Terrain { get; set; }
        public DateTime DateHidden { get; set; }

        public bool Found { get; set; }

        public void SetWaypoint(Location location, int certainty = 999)
        {
            m_uncertainWaypoint = new UncertainProperty<Location>(location, certainty);
        }


        public Location Waypoint
        {
            get { return m_uncertainWaypoint.getValue(); }
            set { m_uncertainWaypoint = new UncertainProperty<Location>(value, 999); }
        }

        public User Owner { get; set; }

        public bool IsDetailed { get; set; }

        public List<Log> Logs { get; set; }

        public void FillMissingDataFrom(Geocache geocache)
        {
            m_uncertainWaypoint = UncertainProperty<Location>.getMergedProperty(geocache.m_uncertainWaypoint, m_uncertainWaypoint);
        }

        public override string ToString()
        {
            return $"[{Code}] {Name} ({Difficulty}/{Terrain})";
        }
    }
}
