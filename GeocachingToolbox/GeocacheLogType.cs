using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox
{
    public enum GeocacheLogType
    {
        Undefined,
        Found,
        DidNotFind,
        WriteNote,
        DisableListing,
        EnableListing,
        PublishListing,
        NeedsMaintenance,
        OwnerMaintenance,
        UpdateCoordinates,
        ReviewerNote,
        Attended
    }
}
