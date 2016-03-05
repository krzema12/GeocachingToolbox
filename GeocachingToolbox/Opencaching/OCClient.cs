using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Web;

namespace GeocachingToolbox.Opencaching
{
    public class OCClient : Client
    {
        public bool NeedsAuthorization
        {
            get { return _tokenStore.Populated == false; }
        }

        protected IAccessTokenStore _tokenStore;
        protected IOCConnector _connector;
        protected ApiAccessKeys _apiAccesKeys = new ApiAccessKeysImpl();
        private Dictionary<string, string> _requestTokens;

        public OCClient(string installationUrl, IOCConnector connector = null, IAccessTokenStore tokenStore = null, ApiAccessKeys apiAccessKeys = null)
        {
            if (connector == null)
            {
                _connector = new OCConnector(installationUrl);
                _tokenStore = new AccessTokenStore();
            }
            else
            {
                _connector = connector;
                _tokenStore = tokenStore;
            }

            if (apiAccessKeys != null)
            {
                _apiAccesKeys = apiAccessKeys;
            }

            _connector.SetConsumerKeyAndSecret(_apiAccesKeys.ConsumerKey, _apiAccesKeys.ConsumerSecret);
            
            if (_tokenStore.Populated)
            {
                _connector.SetTokens(_tokenStore.Token, _tokenStore.TokenSecret);
            }
        }

        public string GetAuthorizationUrl()
        {
            var requestTokenArgs = new Dictionary<string, string>();
            requestTokenArgs.Add("oauth_callback", "oob");
            var requestTokenUrl = _connector.GetURL("services/oauth/request_token", requestTokenArgs);
            var requestTokenResponse = _connector.GetResponse(requestTokenUrl);

            _requestTokens = UrlParser.Parse(requestTokenResponse);

            _connector.SetConsumerKeyAndSecret("", "");

            var authorizeArgs = new Dictionary<string, string>();
            authorizeArgs.Add("oauth_token", _requestTokens["oauth_token"]);
            var authorizeUrl = _connector.GetURL("services/oauth/authorize", authorizeArgs);

            _connector.SetConsumerKeyAndSecret(_apiAccesKeys.ConsumerKey, _apiAccesKeys.ConsumerSecret);

            return authorizeUrl;
        }

        public void EnterAuthorizationPin(string pin)
        {
            var accessTokenArgs = new Dictionary<string, string>();
            accessTokenArgs.Add("oauth_verifier", pin);
            _connector.SetTokens(_requestTokens["oauth_token"], _requestTokens["oauth_token_secret"]);
            var accessTokenUrl = _connector.GetURL("services/oauth/access_token", accessTokenArgs);

            var accessTokenResponse = _connector.GetResponse(accessTokenUrl);
            var accessTokens = UrlParser.Parse(accessTokenResponse);

            _tokenStore.SetValues(accessTokens["oauth_token"], accessTokens["oauth_token_secret"]);
        }

        public void Connect()
        {
            GetUserInfo();
        }

        private void GetUserInfo()
        {
            var userInfoArgs = new Dictionary<string, string>();
            userInfoArgs.Add("fields", "uuid|username|caches_found");
            var userInfoUrl = _connector.GetURL("services/users/user", userInfoArgs);

            var userInfoResponse = _connector.GetResponse(userInfoUrl);
            var userInfoJson = JObject.Parse(userInfoResponse);

            User = new OCUser(userInfoJson["username"].Value<string>(),
                userInfoJson["caches_found"].Value<int>(),
                userInfoJson["uuid"].Value<string>());
        }

        public override IEnumerable<T> GetFoundGeocaches<T>()
        {
            var userLogsArgs = new Dictionary<string, string>();
            userLogsArgs.Add("user_uuid", ((OCUser)User).Uuid);
            var userLogsUrl = _connector.GetURL("services/logs/userlogs", userLogsArgs);

            var userLogsResponse = _connector.GetResponse(userLogsUrl);
            var found = new List<OCLog>();
            var userLogsJson = JArray.Parse(userLogsResponse);

            foreach (var jsonLog in userLogsJson)
            {
                var cache = new OCGeocache();
                cache.Code = jsonLog["cache_code"].Value<string>();

                var log = new OCLog()
                {
                    Thing = cache,
                    Uuid = jsonLog["uuid"].Value<string>(),
                    Comment = jsonLog["comment"].Value<string>(),
                    Date = DateTime.Parse(jsonLog["date"].Value<string>()),
                    LogType = StringToLogType(jsonLog["type"].Value<string>())
                };

                found.Add(log);
            }

            return (IEnumerable<T>)found;
        }

        private GeocacheLogType StringToLogType(string type)
        {
            switch(type)
            {
                case "Found it":
                    return GeocacheLogType.Found;
                case "Attended":
                    return GeocacheLogType.Attended;
                default:
                    return GeocacheLogType.Undefined;
            }
        }

        public override void GetGeocacheDetails<T>(T geocache)
        {
            var ocGeocache = geocache as OCGeocache;

            var geocacheDetailsArgs = new Dictionary<string, string>();
            geocacheDetailsArgs.Add("cache_code", ocGeocache.Code);
            geocacheDetailsArgs.Add("fields", "code|name|location|type|status|size2|difficulty|"
                + "terrain|date_hidden|owner|description|hint2");
            var geocacheDetailsUrl = _connector.GetURL("services/caches/geocache", geocacheDetailsArgs);

            var geocacheDetailsResponse = _connector.GetResponse(geocacheDetailsUrl);
            var json = JObject.Parse(geocacheDetailsResponse);

            ocGeocache.Code = json["code"].Value<string>();
            ocGeocache.Name = json["name"].Value<string>().Trim();

            var location = json["location"].Value<string>();
            var latitudeLongitue = location.Split('|');

            ocGeocache.Waypoint = new Location(decimal.Parse(latitudeLongitue[0], CultureInfo.InvariantCulture),
                decimal.Parse(latitudeLongitue[1], CultureInfo.InvariantCulture));

            ocGeocache.Type = StringToGeocacheType(json["type"].Value<string>());
            ocGeocache.Size = StringToGeocacheSize(json["size2"].Value<string>());
            ocGeocache.Status = StringToGeocacheStatus(json["status"].Value<string>());
            ocGeocache.Difficulty = json["difficulty"].Value<float>();
            ocGeocache.Terrain = json["terrain"].Value<float>();
            ocGeocache.DateHidden = DateTime.Parse(json["date_hidden"].Value<string>());
            ocGeocache.Description = json["description"].Value<string>();
            ocGeocache.Hint = json["hint2"].Value<string>();
            ocGeocache.Owner = new OCUser(json["owner"]["username"].Value<string>(),
                -1, json["owner"]["uuid"].Value<string>());
        }

        private GeocacheType StringToGeocacheType(string type)
        {
            switch (type)
            {
                case "Traditional":
                    return GeocacheType.Traditional;
                case "Multi":
                    return GeocacheType.Multicache;
                case "Other":
                    return GeocacheType.Unknown;
                default:
                    return GeocacheType.Unknown;
            }
        }

        private GeocacheSize StringToGeocacheSize(string size)
        {
            switch (size)
            {
                case "micro":
                    return GeocacheSize.Micro;
                default:
                    return GeocacheSize.Unknown;
            }
        }

        private GeocacheStatus StringToGeocacheStatus(string status)
        {
            switch (status)
            {
                case "Available":
                    return GeocacheStatus.Published;
                case "Archived":
                    return GeocacheStatus.Archived;
                case "Temporarily unavailable":
                    return GeocacheStatus.Disabled;
                default:
                    return GeocacheStatus.Unknown;
            }
        }

        public override void PostGeocacheLog<T>(T geocache, GeocacheLogType logType, DateTime date, string description)
        {
            var ocGeocache = geocache as OCGeocache;

            var args = new Dictionary<string, string>();
            args.Add("cache_code", ocGeocache.Code);
            args.Add("logtype", LogTypeToString(logType));
            args.Add("comment", description);
            args.Add("comment_format", "plaintext");
            args.Add("when", date.ToString());
            var url = _connector.GetURL("services/logs/submit", args);

            var response = _connector.GetResponse(url);
            var json = JObject.Parse(response);

            if (json["success"] == null || json["success"].Value<string>() != "True")
            {
                throw new Exception("There was a problem sending a log to Opencaching service!");
            }
        }

        public override void GetTrackableDetails<T>(T trackable)
        {
            throw new NotImplementedException();
        }

        private string LogTypeToString(GeocacheLogType logType)
        {
            switch (logType)
            {
                case GeocacheLogType.Found:
                    return "Found it";
                case GeocacheLogType.DidNotFind:
                    return "Didn't find it";
                case GeocacheLogType.WriteNote:
                    return "Comment";
                case GeocacheLogType.Attended:
                    return "Attended";
                default:
                    return "Comment";
            }
        }

        public override void PostTrackableLog<T>(T trackable, TrackableLogType logType, DateTime date, string description)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<T> GetNearestGeocaches<T>(Location location)
        {
            var nearestGeocachesArgs = new Dictionary<string, string>();
            nearestGeocachesArgs.Add("search_method", "services/caches/search/nearest");
            nearestGeocachesArgs.Add("search_params", string.Format(@"{{""center"": ""{0}|{1}"", ""radius"": ""1.0""}}",
                location.Latitude.ToString(CultureInfo.InvariantCulture),
                location.Longitude.ToString(CultureInfo.InvariantCulture)));
            nearestGeocachesArgs.Add("retr_method", "services/caches/geocaches");
            nearestGeocachesArgs.Add("retr_params", @"{""fields"":""name|location|type""}");
            nearestGeocachesArgs.Add("wrap", "false");

            var nearestGeocachesUrl = _connector.GetURL("services/caches/shortcuts/search_and_retrieve", nearestGeocachesArgs);

            var nearestGeocachesResponse = _connector.GetResponse(nearestGeocachesUrl);
            var json = JObject.Parse(nearestGeocachesResponse);

            var list = new List<OCGeocache>();

            foreach (var cache in json.Children())
            {
                var newCache = new OCGeocache();

                newCache.Code = ((JProperty)cache).Name;
                newCache.Name = cache.First["name"].Value<string>();
                newCache.Type = StringToGeocacheType(cache.First["type"].Value<string>());

                var coordsRaw = cache.First["location"].Value<string>();
                var coords = coordsRaw.Split('|');
                newCache.Waypoint = new Location(decimal.Parse(coords[0], CultureInfo.InvariantCulture),
                    decimal.Parse(coords[1], CultureInfo.InvariantCulture));

                list.Add(newCache);
            }

            return (IEnumerable<T>)list;
        }

        public override string ToString()
        {
            return "Opencaching.com client, user: " + (User != null ? User.Name + ", found caches: "
                + User.FoundGeocachesCount : " noone logged in");
        }
    }
}
