using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public class IncorrectCredentialsException : Exception
    {
        public IncorrectCredentialsException(string message)
            : base(message)
        {
        }
    }

    public class ConnectionProblemException : Exception
    {
        public ConnectionProblemException(string message)
            : base(message)
        {
        }
    }

    public class TrackableNotFoundException : Exception
    {
        public TrackableNotFoundException(string message)
            : base(message)
        {
        }
    }
}
