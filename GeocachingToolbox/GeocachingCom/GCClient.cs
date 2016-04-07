using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace GeocachingToolbox.GeocachingCom
{
    public class GCClient : Client
    {
        private readonly IGCConnector _connector;

        public GCClient(IGCConnector connector = null)
        {
            if (connector == null)
            {
                _connector = new GCConnector();
            }
            else
            {
                _connector = connector;
            }
        }

        public async Task Login(string login, string password)
        {
            try
            {
                var code = await _connector.Login(login, password);

                if (code.Contains("Log out"))
                {
                    var nickRegex = new Regex("<span class=\"ProfileUsername\" title=\"(?<username>[^\"]+?)\">");
                    var nickMatch = nickRegex.Match(code);
                    var nick = nickMatch.Groups["username"].Value;

                    var foundCountRegex = new Regex("<span class=\"statcount\">\\s*(?<count>\\d+)?");
                    var foundCountMatch = foundCountRegex.Match(code);
                    var foundCount = int.Parse(foundCountMatch.Groups["count"].Value);

                    User = new GCUser(nick, foundCount);
                }

                if (code.Contains("Either your username or password is incorrect!")
                    || code.Contains("Log out") == false)
                {
                    throw new IncorrectCredentialsException("Incorrect credentials!");
                }
            }
            catch (WebException)
            {
                throw new ConnectionProblemException("A problem with Internet connection occured!");
            }
        }

        public override async Task<IEnumerable<T>> GetFoundGeocachesAsync<T>()
        {
            var html = await _connector.GetPage("my/logs.aspx?s=1&lt=2");
            var found = new List<GCLog>();

            var parser = new HtmlParser();
            var document = parser.Parse(html);
            // below div name is wrong (ctl00_divContentMain => should be replaced by divContentMain)
            var foundCachesRows = document.QuerySelectorAll("div[id=ctl00_divContentMain] table tr");

            foreach (var row in foundCachesRows)
            {
                var element = row.QuerySelector("td:nth-child(4) a:nth-child(2)");
                var name = WebUtility.HtmlDecode(element.TextContent);

                var status = GeocacheStatus.Published;
                element = row.QuerySelector("td:nth-child(4) span[class*=Strike]");
                if (!string.IsNullOrWhiteSpace(element?.ClassName))
                {
                    if (element.ClassName == "Strike")
                        status = GeocacheStatus.Disabled;
                    if (element.ClassName == "Strike OldWarning")
                        status = GeocacheStatus.Archived;
                }

                element = row.QuerySelector("td:nth-child(4) a:first-of-type");
                var detailsUrl = RemoveBeginningOfAddress(element.GetAttribute("href"));

                element = row.QuerySelector("td:nth-child(4) img:first-of-type");
                var type = ToGeocacheType(element.GetAttribute("title"));

                bool isFavorite = false;
                element = row.QuerySelector("td:nth-child(2) img");
                if (element != null)
                {
                    isFavorite = element.GetAttribute("title") == "You have favorited this cache";
                }

                element = row.QuerySelector("td:nth-child(3)");
                var date = DateTime.ParseExact(element.TextContent.Trim(), "dd/MM/yyyy", CultureInfo.CurrentCulture);

                var geocache = new GCGeocache
                {
                    Name = name,
                    DetailsUrl = detailsUrl,
                    Type = type,
                    Status = status
                };

                var log = new GCLog
                {
                    Thing = geocache,
                    Date = date,
                    LogType = GeocacheLogType.Found,
                    IsFavorite = isFavorite
                };

                found.Add(log);
            }

            return (IEnumerable<T>)found;
        }

        public override async Task<IEnumerable<T>> GetNearestGeocachesAsync<T>(Location location)
        {
            var url = string.Format("seek/nearest.aspx?lat={0}&lng={1}&ex=0",
                location.Latitude.ToString(CultureInfo.InvariantCulture),
                location.Longitude.ToString(CultureInfo.InvariantCulture));
            var html = await _connector.GetPage(url);

            var parser = new HtmlParser();

            var doc = parser.Parse(html);

            var nearest = new List<GCGeocache>();

            var nearestCachesRows = doc.QuerySelectorAll("div[id=ctl00_ContentBody_ResultsPanel] table[class=\"SearchResultsTable Table\"] tr:not(:first-child)");

            foreach (var row in nearestCachesRows)
            {
                var nameAuthorCodeCell = row.QuerySelector("td:nth-child(6)");
                var name = WebUtility.HtmlDecode(nameAuthorCodeCell.QuerySelector("a span").TextContent);

                var authorAndCode = WebUtility.HtmlDecode(nameAuthorCodeCell.QuerySelector("span[class=small]").TextContent.Trim());
                var authorAndCodeRegex = new Regex("by (.*)\\s+\\|\\s+([0-9A-Z]+)\\s+\\|\\s+.*");
                var authorAndCodeMatch = authorAndCodeRegex.Match(authorAndCode);
                var author = authorAndCodeMatch.Groups[1].Value.Trim();
                var code = authorAndCodeMatch.Groups[2].Value.Trim();

                var typeText = row.QuerySelector("td:nth-child(5) a img").Attributes["title"].Value;
                var type = ToGeocacheType(typeText);

                var diffTerrainSizeNode = row.QuerySelector("td:nth-child(8)");
                var diffTerrain = diffTerrainSizeNode.QuerySelector("span").TextContent.Split('/');
                var diff = float.Parse(diffTerrain[0], CultureInfo.InvariantCulture);
                var terrain = float.Parse(diffTerrain[1], CultureInfo.InvariantCulture);
                var sizeRawString = diffTerrainSizeNode.QuerySelector("img").Attributes["title"].Value;
                var sizeRegex = new Regex("Size: (.*)");
                var sizeMatch = sizeRegex.Match(sizeRawString);
                var size = ToGeocacheSize(sizeMatch.Groups[1].Value.ToLower());

                var isPremium = row.QuerySelector("td:nth-child(7) img[title=\"Premium Member Only Cache\"]") != null;

                var dateText = row.QuerySelector("td:nth-child(9) span").TextContent;
                var date = DateTime.ParseExact(dateText, "dd/MM/yyyy", CultureInfo.InvariantCulture);


                var geocache = new GCGeocache
                {
                    Name = name,
                    Code = code,
                    Type = type,
                    Status = GeocacheStatus.Published,
                    Difficulty = diff,
                    Terrain = terrain,
                    Size = size,
                    IsPremium = isPremium,
                    DateHidden = date,
                    Owner = new GCUser(author, -1)
                };

                nearest.Add(geocache);
            }

            return (IEnumerable<T>)nearest;
        }

        public override async Task PostGeocacheLogAsync<T>(T geocache, GeocacheLogType logType, DateTime date, string description)
        {
            if (string.IsNullOrEmpty(geocache.Code) == false)
            {
                var address = "seek/log.aspx?wp=" + geocache.Code;
                var html = await _connector.GetPage(address);

                var parser = new HtmlParser();
                var doc = parser.Parse(html);

                IDictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "ctl00$ContentBody$LogBookPanel1$ddLogType", GeocacheLogTypeToNumber(logType).ToString() },
                    { "ctl00$ContentBody$LogBookPanel1$uxDateVisited", date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) },
                    { "ctl00$ContentBody$LogBookPanel1$uxLogInfo", description },
                    { "ctl00$ContentBody$LogBookPanel1$btnSubmitLog", "Submit Log Entry" },
                    { "ctl00$ContentBody$uxVistOtherListingGC", "" },
                };

                var hiddenFormFields = doc.QuerySelectorAll("input[type=hidden]");

                foreach (var field in hiddenFormFields)
                {
                    parameters.Add(field.Attributes["name"].Value, field.Attributes["value"].Value);
                }

                var response = _connector.PostToPage(address, parameters);
            }
            else
            {
                throw new Exception("No way to identify the cache!");
            }
        }

        public override async Task PostTrackableLogAsync<T>(T trackable, TrackableLogType logType, DateTime date, string description)
        {
            if (string.IsNullOrEmpty(trackable.TrackingCode) == false)
            {
                var detailsPageHtml = await _connector.GetPage("track/details.aspx?tracker=" + trackable.TrackingCode);
                var parser = new HtmlParser();
                var detailsPageDoc = parser.Parse(detailsPageHtml);

                if (detailsPageDoc.GetElementById("ctl00_ContentBody_ErrorMessage") != null)
                {
                    throw new TrackableNotFoundException("Trackable with the code " + trackable.TrackingCode + " does not exist in the system!");
                }

                var logPageAddress = detailsPageDoc.GetElementById("ctl00_ContentBody_LogLink").Attributes["href"].Value;
                logPageAddress = "track/" + WebUtility.HtmlDecode(RemoveBeginningOfAddress(logPageAddress));

                var loggingPageHtml = await _connector.GetPage(logPageAddress);
                var loggingPageDoc = parser.Parse(loggingPageHtml);

                IDictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "ctl00$ContentBody$LogBookPanel1$ddLogType", TrackableLogTypeToNumber(logType).ToString() },
                    { "ctl00$ContentBody$LogBookPanel1$uxDateVisited", date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) },
                    { "ctl00$ContentBody$LogBookPanel1$tbCode", trackable.TrackingCode },
                    { "ctl00$ContentBody$LogBookPanel1$uxLogInfo", description },
                    { "ctl00$ContentBody$LogBookPanel1$btnSubmitLog", "Submit Log Entry" },
                    { "ctl00$ContentBody$uxVistOtherTrackableTB", "" }
                };

                var hiddenFormFields = loggingPageDoc.QuerySelectorAll("input[type=hidden]");

                foreach (var field in hiddenFormFields)
                {
                    parameters.Add(field.Attributes["name"].Value, field.Attributes["value"].Value);
                }

                var response = _connector.PostToPage(logPageAddress, parameters);
                // TODO: error handling
            }
            else
            {
                throw new Exception("No way to identify the trackable!");
            }
        }

        private static int TrackableLogTypeToNumber(TrackableLogType logType)
        {
            switch (logType)
            {
                case TrackableLogType.WriteNote:
                    return 4;
                case TrackableLogType.Discovered:
                    return 48;
                case TrackableLogType.Grab:
                    return 19;
                default:
                    throw new Exception("Unsupported type of log!");
            }
        }

        private static GeocacheType ToGeocacheType(string name)
        {
            switch (name)
            {
                case "Traditional Cache":
                case "Traditional Geocache":
                case "2.gif":
                    return GeocacheType.Traditional;
                case "Unknown Cache":
                case "Mystery Cache":
                case "8.gif":
                    return GeocacheType.Mystery;
                case "Multi-cache":
                case "3.gif":
                    return GeocacheType.Multicache;
                case "Wherigo Cache":
                    return GeocacheType.Whereigo;
                case "Letterbox Hybrid":
                case "5.gif":
                    return GeocacheType.LetterboxHybrid;
                case "Earthcache":
                    return GeocacheType.Earthcache;
                default:
                    return GeocacheType.Unknown;
            }
        }

        public override async Task GetGeocacheDetailsAsync<T>(T geocache)
        {
            string html = "";
            var gcGeocache = geocache as GCGeocache;

            if (string.IsNullOrEmpty(geocache.Code) == false)
            {
                html = await _connector.GetPage("geocache/" + geocache.Code);
            }
            else if (string.IsNullOrEmpty(gcGeocache.DetailsUrl) == false)
            {
                html = await _connector.GetPage(gcGeocache.DetailsUrl);
            }
            else
            {
                throw new Exception("No way to identify the cache!");
            }

            var parser = new HtmlParser();
            var doc = parser.Parse(html);

            CheckIfPremium(gcGeocache, doc);

            ParseName(gcGeocache, doc);
            ParseCode(gcGeocache, doc);
            ParseType(gcGeocache, doc);
            ParseDifficulty(gcGeocache, doc);
            ParseTerrain(gcGeocache, doc);
            ParseSize(gcGeocache, doc);
            ParseOwner(gcGeocache, doc);

            if (gcGeocache.IsPremium == false)
            {
                ParseDescription(gcGeocache, doc);
                ParseHint(gcGeocache, doc);
                ParseStatus(gcGeocache, doc);
                ParseHiddenDate(gcGeocache, doc);
                ParseWaypoint(gcGeocache, doc);
            }
        }

        public override async Task GetTrackableDetailsAsync<T>(T trackable)
        {
            if (string.IsNullOrEmpty(trackable.TrackingCode) == false)
            {
                var detailsPageHtml = await _connector.GetPage("track/details.aspx?tracker=" + trackable.TrackingCode);
                var parser = new HtmlParser();
                parser.Parse(detailsPageHtml);
            }
        }

        private static string RemoveBeginningOfAddress(string address)
        {
            return address.Replace("http://www.geocaching.com/", "");
        }

        private void CheckIfPremium(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            gcGeocache.IsPremium = doc.GetElementById("ctl00_ContentBody_basicMemberMsg") != null;
        }

        private void ParseCode(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            if (gcGeocache.IsPremium)
            {
                var code = doc.QuerySelector("div[id=ctl00_divContentMain] h2").TextContent;
                code = code.Substring(code.LastIndexOf('(') + 1).Trim();
                code = code.Substring(0, code.Length - 1);
                gcGeocache.Code = code;
            }
            else
            {
                gcGeocache.Code = doc.GetElementById("ctl00_ContentBody_CoordInfoLinkControl1_uxCoordInfoCode").TextContent.Trim();
            }
        }

        private void ParseOwner(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            string ownerName;
            if (gcGeocache.IsPremium)
            {
                ownerName = doc.GetElementById("ctl00_ContentBody_uxCacheType").TextContent.Trim();
                ownerName = ownerName.Replace("A cache by ", "");
            }
            else
            {
                var ownerNode = doc.QuerySelector("div[id=ctl00_ContentBody_mcd1] a");
                ownerName = ownerNode.TextContent.Trim();
            }

            ownerName = WebUtility.HtmlDecode(ownerName);
            var owner = new GCUser(ownerName, -1);
            gcGeocache.Owner = owner;
        }

        private void ParseWaypoint(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            var coordsString = doc.GetElementById("uxLatLon").TextContent.Trim();
            var coordsRegex = new Regex("(?<latDir>[NS]) (?<latDeg>\\d+)° (?<latMin>\\d+\\.\\d+) "
                + "(?<longDir>[WE]) (?<longDeg>\\d+)° (?<longMin>\\d+\\.\\d+)");
            var coords = coordsRegex.Match(coordsString);
            var latDirection = coords.Groups["latDir"].Value == "N" ? 1 : -1;
            var longDirection = coords.Groups["longDir"].Value == "E" ? 1 : -1;
            var location = new Location(
                int.Parse(coords.Groups["latDeg"].Value) * latDirection,
                decimal.Parse(coords.Groups["latMin"].Value, CultureInfo.InvariantCulture),
                int.Parse(coords.Groups["longDeg"].Value) * longDirection,
                decimal.Parse(coords.Groups["longMin"].Value, CultureInfo.InvariantCulture));
            gcGeocache.Waypoint = location;
        }

        private void ParseHiddenDate(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            var date = doc.GetElementById("ctl00_ContentBody_mcd2").TextContent;
            date = date.Substring(date.IndexOf(':') + 1).Trim();
            gcGeocache.DateHidden = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private void ParseTerrain(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            string terrain;

            if (gcGeocache.IsPremium)
            {
                terrain = doc.QuerySelector("div[id=ctl00_divContentMain] p img:nth-of-type(3)")
                   .Attributes["alt"].Value;
            }
            else
            {
                terrain = doc.QuerySelector("span[id=ctl00_ContentBody_Localize12] img")
                .Attributes["alt"].Value;
            }

            terrain = terrain.Substring(0, terrain.IndexOf(' '));
            gcGeocache.Terrain = float.Parse(terrain, CultureInfo.InvariantCulture);
        }

        private void ParseDifficulty(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            string difficulty;

            if (gcGeocache.IsPremium)
            {
                difficulty = doc.QuerySelector("div[id=ctl00_divContentMain] p img:nth-of-type(2)")
                    .Attributes["alt"].Value;
            }
            else
            {
                difficulty = doc.QuerySelector("span[id=ctl00_ContentBody_uxLegendScale] img")
                    .Attributes["alt"].Value;
            }

            difficulty = difficulty.Substring(0, difficulty.IndexOf(' '));
            gcGeocache.Difficulty = float.Parse(difficulty, CultureInfo.InvariantCulture);
        }

        private static void ParseName(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            if (gcGeocache.IsPremium)
            {
                var name = doc.QuerySelector("div[id=ctl00_divContentMain] h2").TextContent;
                name = name.Substring(0, name.LastIndexOf('(')).Trim();
                gcGeocache.Name = name;
            }
            else
            {
                gcGeocache.Name = doc.GetElementById("ctl00_ContentBody_CacheName").TextContent;
            }
        }

        private static void ParseDescription(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            gcGeocache.Description = doc.GetElementById("ctl00_ContentBody_ShortDescription").InnerHtml.Trim()
                 + "<br />" + doc.GetElementById("ctl00_ContentBody_LongDescription").InnerHtml.Trim();
        }

        private void ParseHint(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            gcGeocache.Hint = ROT13Coder.Decode(doc.GetElementById("div_hint").TextContent).Trim();
        }

        private static void ParseType(GCGeocache gcGeocache, IHtmlDocument doc)
        {
            string type;

            if (gcGeocache.IsPremium)
            {
                type = doc.GetElementById("ctl00_ContentBody_uxWptTypeImage").Attributes["src"].Value;
                type = type.Substring(type.LastIndexOf('/') + 1);
            }
            else
            {
                type = doc.QuerySelector("div[id=cacheDetails] p[class=cacheImage] a img").Attributes["title"].Value;
            }

            gcGeocache.Type = ToGeocacheType(type);
        }

        private void ParseSize(GCGeocache geocache, IHtmlDocument doc)
        {
            string size;

            if (geocache.IsPremium)
            {
                size = doc.QuerySelector("div[id=ctl00_divContentMain] p small").TextContent.Trim();
            }
            else
            {
                size = doc.QuerySelector("span[class=minorCacheDetails] small").TextContent.Trim();
            }

            size = size.Substring(1, size.Length - 2);
            size = size.ToLower();
            geocache.Size = ToGeocacheSize(size);
        }

        private static void ParseStatus(GCGeocache geocache, IHtmlDocument doc)
        {
            var statusListItems = doc.QuerySelectorAll("ul[class=OldWarning] li").ToArray();

            // TODO: Handle other statuses
            if (statusListItems.Length == 0)
            {
                geocache.Status = GeocacheStatus.Published;
            }
            else if (statusListItems[0].TextContent.Trim()
                == "This cache is temporarily unavailable. Read the"
                + " logs below to read the status for this cache.")
            {
                geocache.Status = GeocacheStatus.Disabled;
            }
            else if (statusListItems[0].TextContent.Trim()
                == "This cache has been archived, but is available"
                + " for viewing for archival purposes.")
            {
                geocache.Status = GeocacheStatus.Archived;
            }
            else
            {
                geocache.Status = GeocacheStatus.Unknown;
            }
        }

        private GeocacheSize ToGeocacheSize(string size)
        {
            switch (size)
            {
                case "micro":
                    return GeocacheSize.Micro;
                case "small":
                    return GeocacheSize.Small;
                case "regular":
                    return GeocacheSize.Regular;
                case "other":
                    return GeocacheSize.Other;
                default:
                    return GeocacheSize.Unknown;
            }
        }

        private static int GeocacheLogTypeToNumber(GeocacheLogType logType)
        {
            switch (logType)
            {
                case GeocacheLogType.Found:
                    return 2;
                case GeocacheLogType.DidNotFind:
                    return 3;
                case GeocacheLogType.WriteNote:
                    return 4;
                case GeocacheLogType.NeedsMaintenance:
                    return 45;
                default:
                    throw new Exception("Unsupported type of log!");
            }
        }

        public override string ToString()
        {
            return "Geocaching.com client, user: " + User.Name + ", found caches: " + User.FoundGeocachesCount;
        }
    }
}
