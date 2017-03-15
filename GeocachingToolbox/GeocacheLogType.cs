namespace GeocachingToolbox
{
    public enum GeocacheLogType
    {
        [Description("unknown")]
        Undefined,
        [Description("Found it")]
        Found,
        [Description("Didn't find it")]
        DidNotFind,
        [Description("Write note")]
        WriteNote,
        [Description("temporarily disable listing")]
        DisableListing,
        [Description("Enable listing")]
        EnableListing,
        [Description("Publish listing")]
        PublishListing,
        [Description("needs maintenance")]
        NeedsMaintenance,
        [Description("owner maintenance")]
        OwnerMaintenance,
        [Description("update coordinates")]
        UpdateCoordinates,
        [Description("post reviewer note")]
        ReviewerNote,
        [Description("attended")]
        Attended
    }
}
