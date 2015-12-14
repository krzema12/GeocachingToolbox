using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public abstract class User
    {
        public string Name { get; protected set; }
        public int FoundGeocachesCount { get; protected set; }
    }
}
