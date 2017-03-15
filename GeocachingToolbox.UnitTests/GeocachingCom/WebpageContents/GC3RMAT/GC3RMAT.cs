using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.UnitTests.GeocachingCom.WebpageContents.GC3RMAT
{
    class GC3RMAT : MockedGCCache
    {
        public GC3RMAT() : base("GC3RMAT")
        {
            Name = "Ecoduc Grünhaut";
            Difficulty = 2;
            Terrain = 1.5f;
            Code = "GC3RMAT";
            Size = GeocacheSize.Micro;
            Type = GeocacheType.Traditional;
            ShortDescription = "";
            DateHidden = new DateTime(2012, 7, 31);
            IsPremium = false;
            Status = GeocacheStatus.Published;
            IsDetailed = true;
            Description = "<p align=\"center\"> </p>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <p align=\"left\"><span style=\"font-size:12.0pt;mso-bidi-font-size:9.0pt;font-family:&amp;amp;\"><img src=\"data:image/png; base64,77u/\" width=\"640\" height=\"480\" align=\"left\"></span></p>" +
                          Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <p align=\"left\"><span style=\"font-size:12.0pt;mso-bidi-font-size:9.0pt;font-family:&amp;amp;\"><img src=\"data:image/png; base64,77u/\" width=\"640\" height=\"480\" align=\"left\"></span></p>" +
                          Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>" + Environment.NewLine +
                          "                                <br>";
                //@"<img src=""data:image/png; base64,77u/"">";
            Waypoint = new Location(50, 39.006M, 5, 55.953M);

            /*
             *  Code="GC3RMAT",
                         Name = "Ecoduc Grünhaut",
                         Waypoint = new Location(50,39.006M,5,55.953M),
                         Found = false,
                         Type = GeocacheType.Traditional*/
        }
    }
}
