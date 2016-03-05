using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
    public class GCUser : User
    {
        public GCUser(string nick, int foundCount)
        {
            Name = nick;
            FoundGeocachesCount = foundCount;
        }
    }
}
