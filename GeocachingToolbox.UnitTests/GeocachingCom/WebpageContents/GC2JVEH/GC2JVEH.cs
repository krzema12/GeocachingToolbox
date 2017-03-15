using System;

namespace GeocachingToolbox.UnitTests.GeocachingCom.WebpageContents.GC2JVEH
{
    class GC2JVEH : MockedGCCache
    {
        public GC2JVEH() : base("GC2JVEH")
        {
            Name = "Auf den Spuren des Indianer Jones Teil 1";
            Difficulty = 5;
            Terrain = 3;
            Code = "GC2JVEH";
            Size = GeocacheSize.Small;
            Type = GeocacheType.Mystery;
            ShortDescription =
                "Aufgabe zum Start: Finde die Schattenlinie. !!!Die Skizze mit den Zahlen solltest du mitnehmen!!! Du solltest den cache so beginnen, das du station 2 in der Zeit von mo- fr von 11-19 Uhr und sa von 11-16 Uhr erledigt hast. Achtung: Damit ihr die Zahlenpause in druckbarer Größe sehen könnt müsst ihr über die Bildergalerie gehen nicht über den unten zu sehenden link.....";
            DateHidden = new DateTime(2010, 11, 28);
            IsPremium = true;
            Status = GeocacheStatus.Archived;
            Description = @"<img src=""data:image/png; base64,77u/"">";
            Waypoint = new Location(52.3722500, 009.7353667);
            IsDetailed = true;
        }
    }
}
