using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
	public class GCClient : Client
	{
		private IGCConnector _connector;

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

		public void Login(string login, string password)
		{
			try
			{
				var code = _connector.Login(login, password);

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

		public override IEnumerable<T> GetFoundGeocaches<T>()
		{
			var html = _connector.GetPage("my/logs.aspx?s=1&lt=2");
			var found = new List<GCLog>();

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var foundCachesRows = doc.DocumentNode.SelectNodes("//div[@id=\"ctl00_divContentMain\"]/table/tbody/tr");

			foreach (var row in foundCachesRows)
			{
				var nameCell = row.SelectSingleNode("td[4]");
				HtmlNode nameNode = nameCell.SelectSingleNode("span") ?? nameCell;
				nameNode = nameNode.SelectSingleNode("a[2]");

				var status = GeocacheStatus.Published;
				var statusSpan = nameNode.SelectSingleNode("span");

				if (statusSpan != null)
				{
					if (statusSpan.Attributes["class"].Value == "Strike")
					{
						status = GeocacheStatus.Disabled;
					}
					else if (statusSpan.Attributes["class"].Value == "Strike OldWarning")
					{
						status = GeocacheStatus.Archived;
					}

					nameNode = nameNode.SelectSingleNode("span");
				}

				var name = WebUtility.HtmlDecode(nameNode.InnerText);
				var detailsUrl = RemoveBeginningOfAddress(nameCell.SelectSingleNode(".//a").Attributes["href"].Value);
				var type = ToGeocacheType(nameCell.SelectSingleNode(".//img").Attributes["title"].Value);

				var isFavoriteNode = row.SelectSingleNode("td[2]/img");
				bool isFavorite = isFavoriteNode != null && isFavoriteNode.Attributes["title"].Value == "You have favorited this cache";

				var dateText = row.SelectSingleNode("td[3]").InnerText.Trim();
				var date = DateTime.ParseExact(dateText, "dd/MM/yyyy", CultureInfo.InvariantCulture);

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

		public override IEnumerable<T> GetNearestGeocaches<T>(Location location)
		{
			var url = string.Format("seek/nearest.aspx?lat={0}&lng={1}&ex=0",
				location.Latitude.ToString(CultureInfo.InvariantCulture),
				location.Longitude.ToString(CultureInfo.InvariantCulture));
			var html = _connector.GetPage(url);

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var nearest = new List<GCGeocache>();

			var nearestCachesRows = doc.DocumentNode.SelectNodes("//div[@id=\"ctl00_ContentBody_ResultsPanel\"]/table[@class=\"SearchResultsTable Table\"]/tr[position()>1]");

			foreach (var row in nearestCachesRows)
			{
				var nameAuthorCodeCell = row.SelectSingleNode("td[6]");
				var name = WebUtility.HtmlDecode(nameAuthorCodeCell.SelectSingleNode("a/span").InnerText);

				var authorAndCode = WebUtility.HtmlDecode(nameAuthorCodeCell.SelectSingleNode("span").InnerText.Trim());
				var authorAndCodeRegex = new Regex("by (.*)\\s+\\|\\s+([0-9A-Z]+)\\s+\\|\\s+.*");
				var authorAndCodeMatch = authorAndCodeRegex.Match(authorAndCode);
				var author = authorAndCodeMatch.Groups[1].Value.Trim();
				var code = authorAndCodeMatch.Groups[2].Value.Trim();

				var typeText = row.SelectSingleNode("td[5]/a/img").Attributes["title"].Value;
				var type = ToGeocacheType(typeText);

				var diffTerrainSizeNode = row.SelectSingleNode("td[8]");
				var diffTerrain = diffTerrainSizeNode.SelectSingleNode("span").InnerText.Split('/');
				var diff = float.Parse(diffTerrain[0], CultureInfo.InvariantCulture);
				var terrain = float.Parse(diffTerrain[1], CultureInfo.InvariantCulture);
				var sizeRawString = diffTerrainSizeNode.SelectSingleNode("img").Attributes["title"].Value;
				var sizeRegex = new Regex("Size: (.*)");
				var sizeMatch = sizeRegex.Match(sizeRawString);
				var size = ToGeocacheSize(sizeMatch.Groups[1].Value.ToLower());

				var isPremium = row.SelectSingleNode("td[7]/img[@title=\"Premium Member Only Cache\"]") != null;

				var dateText = row.SelectSingleNode("td[9]/span").InnerText;
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

		public override void PostGeocacheLog<T>(T geocache, GeocacheLogType logType, DateTime date, string description)
		{
			if (string.IsNullOrEmpty(geocache.Code) == false)
			{
				var address = "seek/log.aspx?wp=" + geocache.Code;
				var html = _connector.GetPage(address);

				var doc = new HtmlDocument();
				doc.LoadHtml(html);

				IDictionary<string, string> parameters = new Dictionary<string, string>
				{
					{ "ctl00$ContentBody$LogBookPanel1$ddLogType", GeocacheLogTypeToNumber(logType).ToString() },
					{ "ctl00$ContentBody$LogBookPanel1$uxDateVisited", date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) },
					{ "ctl00$ContentBody$LogBookPanel1$uxLogInfo", description },
					{ "ctl00$ContentBody$LogBookPanel1$btnSubmitLog", "Submit Log Entry" },
					{ "ctl00$ContentBody$uxVistOtherListingGC", "" },
				};

				var hiddenFormFields = doc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]");

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

		public override void PostTrackableLog<T>(T trackable, TrackableLogType logType, DateTime date, string description)
		{
			if (string.IsNullOrEmpty(trackable.TrackingCode) == false)
			{
				var detailsPageHtml = _connector.GetPage("track/details.aspx?tracker=" + trackable.TrackingCode);
				var detailsPageDoc = new HtmlDocument();
				detailsPageDoc.LoadHtml(detailsPageHtml);

				if (detailsPageDoc.GetElementbyId("ctl00_ContentBody_ErrorMessage") != null)
				{
					throw new TrackableNotFoundException("Trackable with the code " + trackable.TrackingCode + " does not exist in the system!");
				}

				var logPageAddress = detailsPageDoc.GetElementbyId("ctl00_ContentBody_LogLink").Attributes["href"].Value;
				logPageAddress = "track/" + WebUtility.HtmlDecode(RemoveBeginningOfAddress(logPageAddress));

				var loggingPageHtml = _connector.GetPage(logPageAddress);
				var loggingPageDoc = new HtmlDocument();
				loggingPageDoc.LoadHtml(loggingPageHtml);

				IDictionary<string, string> parameters = new Dictionary<string, string>
				{
					{ "ctl00$ContentBody$LogBookPanel1$ddLogType", TrackableLogTypeToNumber(logType).ToString() },
					{ "ctl00$ContentBody$LogBookPanel1$uxDateVisited", date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) },
					{ "ctl00$ContentBody$LogBookPanel1$tbCode", trackable.TrackingCode },
					{ "ctl00$ContentBody$LogBookPanel1$uxLogInfo", description },
					{ "ctl00$ContentBody$LogBookPanel1$btnSubmitLog", "Submit Log Entry" },
					{ "ctl00$ContentBody$uxVistOtherTrackableTB", "" }
				};

				var hiddenFormFields = loggingPageDoc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]");

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

		public override void GetGeocacheDetails<T>(T geocache)
		{
			string html = "";
			var gcGeocache = geocache as GCGeocache;

			if (string.IsNullOrEmpty(geocache.Code) == false)
			{
				html = _connector.GetPage("geocache/" + geocache.Code);
			}
			else if (string.IsNullOrEmpty(gcGeocache.DetailsUrl) == false)
			{
				html = _connector.GetPage(gcGeocache.DetailsUrl);
			}
			else
			{
				throw new Exception("No way to identify the cache!");
			}

			var doc = new HtmlDocument();
			doc.LoadHtml(html);

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

		public override void GetTrackableDetails<T>(T trackable)
		{
			if (string.IsNullOrEmpty(trackable.TrackingCode) == false)
			{
				var detailsPageHtml = _connector.GetPage("track/details.aspx?tracker=" + trackable.TrackingCode);
				var detailsPageDoc = new HtmlDocument();
				detailsPageDoc.LoadHtml(detailsPageHtml);
			}
		}

		private static string RemoveBeginningOfAddress(string address)
		{
			return address.Replace("http://www.geocaching.com/", "");
		}

		private void CheckIfPremium(GCGeocache gcGeocache, HtmlDocument doc)
		{
			gcGeocache.IsPremium = doc.GetElementbyId("ctl00_ContentBody_basicMemberMsg") != null;
		}

		private void ParseCode(GCGeocache gcGeocache, HtmlDocument doc)
		{
			if (gcGeocache.IsPremium)
			{
				var code = doc.DocumentNode.SelectSingleNode("//div[@id=\"ctl00_divContentMain\"]/h2")
					.InnerText;
				code = code.Substring(code.LastIndexOf('(') + 1).Trim();
				code = code.Substring(0, code.Length - 1);
				gcGeocache.Code = code;
			}
			else
			{
				gcGeocache.Code = doc.GetElementbyId("ctl00_ContentBody_CoordInfoLinkControl1_uxCoordInfoCode")
					.InnerText.Trim();
			}
		}

		private void ParseOwner(GCGeocache gcGeocache, HtmlDocument doc)
		{
			string ownerName;
			if (gcGeocache.IsPremium)
			{
				ownerName = doc.GetElementbyId("ctl00_ContentBody_uxCacheType").InnerText.Trim();
				ownerName = ownerName.Replace("A cache by ", "");
			}
			else
			{
				var ownerNode = doc.DocumentNode.SelectSingleNode("//div[@id=\"ctl00_ContentBody_mcd1\"]/a");
				ownerName = ownerNode.InnerText.Trim();
			}

			ownerName = WebUtility.HtmlDecode(ownerName);
			var owner = new GCUser(ownerName, -1);
			gcGeocache.Owner = owner;
		}

		private void ParseWaypoint(GCGeocache gcGeocache, HtmlDocument doc)
		{
			var coordsString = doc.GetElementbyId("uxLatLon").InnerText.Trim();
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

		private void ParseHiddenDate(GCGeocache gcGeocache, HtmlDocument doc)
		{
			var date = doc.GetElementbyId("ctl00_ContentBody_mcd2").InnerText;
			date = date.Substring(date.IndexOf(':') + 1).Trim();
			gcGeocache.DateHidden = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
		}

		private void ParseTerrain(GCGeocache gcGeocache, HtmlDocument doc)
		{
			string terrain;

			if (gcGeocache.IsPremium)
			{
				terrain = doc.DocumentNode.SelectSingleNode("//div[@id=\"ctl00_divContentMain\"]/p/img[3]")
					.Attributes["alt"].Value;
			}
			else
			{
				terrain = doc.DocumentNode.SelectSingleNode("//span[@id=\"ctl00_ContentBody_Localize12\"]/img")
				.Attributes["alt"].Value;
			}
			
			terrain = terrain.Substring(0, terrain.IndexOf(' '));
			gcGeocache.Terrain = float.Parse(terrain, CultureInfo.InvariantCulture);
		}

		private void ParseDifficulty(GCGeocache gcGeocache, HtmlDocument doc)
		{
			string difficulty;

			if (gcGeocache.IsPremium)
			{
				difficulty = doc.DocumentNode.SelectSingleNode("//div[@id=\"ctl00_divContentMain\"]/p/img[2]")
					.Attributes["alt"].Value;
			}
			else
			{
				difficulty = doc.DocumentNode.SelectSingleNode("//span[@id=\"ctl00_ContentBody_uxLegendScale\"]/img")
					.Attributes["alt"].Value;
			}

			difficulty = difficulty.Substring(0, difficulty.IndexOf(' '));
			gcGeocache.Difficulty = float.Parse(difficulty, CultureInfo.InvariantCulture);
		}

		private static void ParseName(GCGeocache gcGeocache, HtmlDocument doc)
		{
			if (gcGeocache.IsPremium)
			{
				var name = doc.DocumentNode.SelectSingleNode("//div[@id=\"ctl00_divContentMain\"]/h2")
					.InnerText;
				name = name.Substring(0, name.LastIndexOf('(')).Trim();
				gcGeocache.Name = name;
			}
			else
			{
				gcGeocache.Name = doc.GetElementbyId("ctl00_ContentBody_CacheName").InnerText;
			}
		}

		private static void ParseDescription(GCGeocache gcGeocache, HtmlDocument doc)
		{
			gcGeocache.Description = doc.GetElementbyId("ctl00_ContentBody_ShortDescription").InnerHtml.Trim()
				+ "<br />" + doc.GetElementbyId("ctl00_ContentBody_LongDescription").InnerHtml.Trim();
		}

		private void ParseHint(GCGeocache gcGeocache, HtmlDocument doc)
		{
			gcGeocache.Hint = ROT13Coder.Decode(doc.GetElementbyId("div_hint").InnerText).Trim();
		}

		private static void ParseType(GCGeocache gcGeocache, HtmlDocument doc)
		{
			string type;

			if (gcGeocache.IsPremium)
			{
				type = doc.GetElementbyId("ctl00_ContentBody_uxWptTypeImage").Attributes["src"].Value;
				type = type.Substring(type.LastIndexOf('/') + 1);
			}
			else
			{
				type = doc.DocumentNode.SelectSingleNode("//div[@id=\"cacheDetails\"]/p[@class=\"cacheImage\"]/a/img")
					.Attributes["title"].Value;
			}

			gcGeocache.Type = ToGeocacheType(type);
		}

		private void ParseSize(GCGeocache geocache, HtmlDocument doc)
		{
			string size;

			if (geocache.IsPremium)
			{
				size = doc.DocumentNode.SelectSingleNode("//div[@id=\"ctl00_divContentMain\"]/p/small")
					.InnerText.Trim();
			}
			else
			{
				size = doc.DocumentNode.SelectSingleNode("//span[@class=\"minorCacheDetails\"]/small")
					.InnerText.Trim();
			}

			size = size.Substring(1, size.Length - 2);
			size = size.ToLower();
			geocache.Size = ToGeocacheSize(size);
		}

		private static void ParseStatus(GCGeocache geocache, HtmlDocument doc)
		{
			var statusListItems = doc.DocumentNode.SelectNodes("//ul[@class=\"OldWarning\"]/li");

			// TODO: Handle other statuses
			if (statusListItems == null)
			{
				geocache.Status = GeocacheStatus.Published;
			}
			else if (statusListItems[0].InnerText.Trim()
				== "This cache is temporarily unavailable. Read the"
				+ " logs below to read the status for this cache.")
			{
				geocache.Status = GeocacheStatus.Disabled;
			}
			else if (statusListItems[0].InnerText.Trim()
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
			switch(size)
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
