using System;
using System.Collections.Generic;

namespace GeocachingToolbox.GeocachingCom
{
    public class GCConstants
    {
        public const string PATTERN_USERSESSION = "UserSession\\('([^']+)'";
        public const string PATTERN_SESSIONTOKEN = "sessionToken:'([^']+)'";
        public const string GC_URL = "https://www.geocaching.com/";
        /** Live Map */
        public const string URL_LIVE_MAP = GC_URL + "map/default.aspx";
        /** Live Map pop-up */
        public const string URL_LIVE_MAP_DETAILS = GC_URL + "map/map.details";
        /** Caches in a tile */
        public const string URL_MAP_INFO = GC_URL + "map/map.info";
        /** Tile itself */
        public const string URL_MAP_TILE = GC_URL + "map/map.png";
        public const String PREFERENCE_URL = "https://www.geocaching.com/myaccount/settings/preferences";
        public const String ENGLISH = "<a href=\"#\">English</a>";
        public const String LANGUAGE_CHANGE_URI = "https://www.geocaching.com/my/souvenirs.aspx";
        public const string PATTERN_LOGIN_NAME = "class=\"li-user-info\"[^>]*>\\s*?<span>(.*?)</span>";
        public const string PATTERN_VIEWSTATEFIELDCOUNT = "id=\"__VIEWSTATEFIELDCOUNT\"[^(Value)]+Value=\"(\\d+)\"[^>]+>";
        public const string PATTERN_VIEWSTATES = "id=\"__VIEWSTATE(\\d*)\"[^(Value)]+Value=\"([^\"]+)\"[^>]+>";
        public const string PATTERN_PREMIUMMEMBERS = "<p class=\"Warning NoBottomSpacing\"";
        public const string PATTERN_NAME = "<span id=\"ctl00_ContentBody_CacheName\">(.*?)</span>";
        public const string PATTERN_GEOCODE = "class=\"CoordInfoCode\">(GC[0-9A-Z&&\\[^ILOSU\\]]+)</span>";
        public const string PATTERN_TYPE = "<a href=\"/seek/nearest\\.aspx\\?tx=([0-9a-f-]+)";
        public const string STRING_CACHEDETAILS = "id=\"cacheDetails\"";
        public const string PATTERN_DIFFICULTY = "<span id=\"ctl00_ContentBody_uxLegendScale\"[^>]*>[^<]*<img src=\"[^\"]*/images/stars/stars([0-9_]+)\\.gif\"";
        public const string PATTERN_TERRAIN = "<span id=\"ctl00_ContentBody_Localize[\\d]+\"[^>]*>[^<]*<img src=\"[^\"]*/images/stars/stars([0-9_]+)\\.gif\"";
        public const string PATTERN_SIZE = "/icons/container/([a-z_]+)\\.";

        public const string PATTERN_OWNER_DISPLAYNAME =
            "<div id=\"ctl00_ContentBody_mcd1\">[^<]+<a href=\"[^\"]+\">([^<]+)</a>";

        public const string PATTERN_SHORTDESC = "<span id=\"ctl00_ContentBody_ShortDescription\">(.*?)</span>\\s*</div>";

        public const string PATTERN_DESC =
            "<span id=\"ctl00_ContentBody_LongDescription\">(.*?)</span>\\s*</div>\\s*<p>\\s*</p>\\s*<p id=\"ctl00_ContentBody_hints\">";

        public const string PATTERN_HINT = "<div id=\"div_hint\"[^>]*>(.*?)</div>";
        public const string PATTERN_STATUS = "<p class=\"OldWarning(.*?)<div";

        public static readonly List<String> STATUS_DISABLED = new List<string>
        {
            "This cache is temporarily unavailable",
            "Das Listing zu diesem Geocache ist momentan deaktiviert"
        };
        public static readonly List<String> STATUS_ARCHIVED = new List<string>
        {
           "This cache has been archived",
            "Dieser Geocache wurde archiviert"
        };

    }
}
