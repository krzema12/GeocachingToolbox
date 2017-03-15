//#define SAVETILES
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageTools;
using ImageTools.IO.Png;
using Newtonsoft.Json;
#if SAVETILES
using PCLStorage;
#endif

namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    public class Tile
    {
        private const int ZoomlevelMax = 18;
        private const int ZoomlevelMin = 0;

        private const int TileSize = 256;
        private static int TileRequestsCounter = 1;
        private const int MaxCachesOnTile = 100;

        private static readonly int[] NumberOfTiles = new int[ZoomlevelMax - ZoomlevelMin + 1];
        private static readonly int[] NumberOfPixels = new int[ZoomlevelMax - ZoomlevelMin + 1];

        public int TileX { get; }
        public int TileY { get; }
        public int Zoomlevel { get; }

        private Viewport Viewport { get; }

        public List<GCGeocache> Caches { get; set; }

        static Tile()
        {
            for (var z = ZoomlevelMin; z <= ZoomlevelMax; z++)
            {
                NumberOfTiles[z] = 1 << z;
                NumberOfPixels[z] = TileSize * 1 << z;
            }
        }

        private Tile(int tileX, int tileY, int zoomlevel)
        {
            Zoomlevel = zoomlevel;

            TileX = tileX;
            TileY = tileY;

            Viewport = new Viewport(GetCoord(new UTFGridPosition(0, 0)), GetCoord(new UTFGridPosition(63, 63)));
        }


        public Tile(Location origin, int zoomlevel)
        {
            Zoomlevel = Math.Max(Math.Min(zoomlevel, ZoomlevelMax), ZoomlevelMin);

            TileX = CalcX(origin);
            TileY = CalcY(origin);

            Viewport = new Viewport(GetCoord(new UTFGridPosition(0, 0)), GetCoord(new UTFGridPosition(63, 63)));
        }

        /**
         * Calculate latitude/longitude for a given x/y position in this tile.
         * 
         * @see <a
         *      href="http://developers.cloudmade.com/projects/tiles/examples/convert-coordinates-to-tile-numbers">Cloudmade</a>
         */
        private Location GetCoord(UTFGridPosition pos)
        {

            double pixX = TileX * TileSize + pos.x * 4;
            double pixY = TileY * TileSize + pos.y * 4;

            decimal lonDeg = (decimal)(((360.0 * pixX) / NumberOfPixels[Zoomlevel]) - 180.0);
            double latRad = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * pixY / NumberOfPixels[Zoomlevel])));
            return new Location(ConvertToDegrees(latRad), lonDeg);
        }

        /**
         * Calculate the tile for a Geopoint based on the Spherical Mercator.
         *
         * @see <a
         *      href="http://developers.cloudmade.com/projects/tiles/examples/convert-coordinates-to-tile-numbers">Cloudmade</a>
         */
        private int CalcX(Location origin)
        {
            return (int)((origin.Longitude + (decimal)180.0) / (decimal)360.0 * NumberOfTiles[Zoomlevel]);
        }

        /**
         * Calculate the tile for a Geopoint based on the Spherical Mercator.
         *
         */
        private int CalcY(Location origin)
        {
            // Optimization from Bing
            var sinLatRad = Math.Sin(ConvertToRadians((double)origin.Latitude));
            return (int)((0.5 - Math.Log((1 + sinLatRad) / (1 - sinLatRad)) / (4 * Math.PI)) * NumberOfTiles[Zoomlevel]);
        }

        private double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        private static decimal ConvertToDegrees(double angrad)
        {
            return (decimal)(angrad * 180.0 / Math.PI);
        }


        /**
         * Calculates the inverted hyperbolic sine
         * (after Bronstein, Semendjajew: Taschenbuch der Mathematik
         *
         * @param x
         * @return
         */
        private static double asinh(double x)
        {
            return Math.Log(x + Math.Sqrt(x * x + 1.0));
        }

        private static double tanGrad(double angleGrad)
        {
            return Math.Tan(angleGrad / 180.0 * Math.PI);
        }

        /**
         * Calculates the maximum possible zoom level where the supplied points
         * are covered by adjacent tiles on the east/west axis.
         * The order of the points (left/right) is irrelevant.
         *
         * @param left
         *            First point
         * @param right
         *            Second point
         * @return
         */
        public static int CalcZoomLon(Location left, Location right)
        {

            int zoom = (int)Math.Floor(
                Math.Log(360.0 / (double)Math.Abs(left.Longitude - right.Longitude))
                / Math.Log(2)
                );

            Tile tileLeft = new Tile(left, zoom);
            Tile tileRight = new Tile(right, zoom);

            if (tileLeft.TileX == tileRight.TileX)
            {
                zoom += 1;
            }

            return Math.Min(zoom, ZoomlevelMax);
        }

        /**
         * Calculates the maximum possible zoom level where the supplied points
         * are covered by adjacent tiles on the north/south axis.
         * The order of the points (bottom/top) is irrelevant.
         *
         * @param bottom
         *            First point
         * @param top
         *            Second point
         * @return
         */
        public static int CalcZoomLat(Location bottom, Location top)
        {

            int zoom = (int)Math.Ceiling(
                Math.Log(2.0 * Math.PI /
                         Math.Abs(
                             asinh(tanGrad((double)bottom.Latitude))
                             - asinh(tanGrad((double)top.Latitude))
                             )
                    ) / Math.Log(2)
                );

            Tile tileBottom = new Tile(bottom, zoom);
            Tile tileTop = new Tile(top, zoom);

            if (Math.Abs(tileBottom.TileY - tileTop.TileY) > 1)
            {
                zoom -= 1;
            }

            return Math.Min(zoom, ZoomlevelMax);
        }

        private static IEnumerable<Tile> GetTilesForViewport(Location bottomLeft, Location topRight)
        {
            var tiles = new HashSet<Tile>();
            int zoom = Math.Max(CalcZoomLon(bottomLeft, topRight), CalcZoomLat(bottomLeft, topRight));

            Tile tileBottomLeft = new Tile(bottomLeft, zoom);
            Tile tileTopRight = new Tile(topRight, zoom);

            int xLow = Math.Min(tileBottomLeft.TileX, tileTopRight.TileX);
            int xHigh = Math.Max(tileBottomLeft.TileX, tileTopRight.TileX);

            int yLow = Math.Min(tileBottomLeft.TileY, tileTopRight.TileY);
            int yHigh = Math.Max(tileBottomLeft.TileY, tileTopRight.TileY);

            for (int xNum = xLow; xNum <= xHigh; xNum++)
            {
                for (int yNum = yLow; yNum <= yHigh; yNum++)
                {
                    tiles.Add(new Tile(xNum, yNum, zoom));
                }
            }
            return tiles;
        }

        public static IEnumerable<Tile> GetTilesForViewport(Viewport viewport)
        {
            return GetTilesForViewport(viewport.bottomLeft, viewport.topRight);
        }

        public override string ToString()
        {
            return $"({TileX}/{TileY}), zoom={Zoomlevel}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private static string FormUrl(string baseUrl, Dictionary<string, string> parameters)
        {
            var urlString = baseUrl;
            var firstParameter = true;
            foreach (var k in parameters.Keys)
            {
                if (firstParameter)
                {
                    urlString += "?" + k + "=" + parameters[k];
                    firstParameter = false;
                }
                else
                {
                    urlString += "&" + k + "=" + parameters[k];
                }
            }
            return urlString;
        }



        public static async Task<int[]> RequestMapTile(GCMapToken token, Dictionary<string, string> parameters, IGCConnector gcConnector, CancellationToken ct)
        {
            //m_Token = token;
            //var parameters = new Dictionary<string, string>
            //{
            //        {"x", TileX + ""},
            //        {"y", TileY + ""},
            //        {"z", Zoomlevel + ""},
            //        {"ep", "1"},
            //};

            if (token != null)
            {
                parameters.Add("k", token.UserSession);
                parameters.Add("st", token.SessionToken);
            }


            //if (Zoomlevel != 14)
            //    parameters.Add("_", Environment.TickCount.ToString());

            int tileServerNb = ++TileRequestsCounter;
            tileServerNb = tileServerNb % 4 + 1;

            var urlString = FormUrl("http://tiles01.geocaching.com/map/map.png", parameters);
            Debug.WriteLine("Tile download url :" + urlString);

            //var urlString = FormUrl("http://tiles0" + tileServerNb + ".geocaching.com/map/map.png", parameters);

            HttpContent content = await gcConnector.GetContent(urlString, null);//, HttpMethod.Get);
            byte[] tileBytes = await content.ReadAsByteArrayAsync();
            ct.ThrowIfCancellationRequested();

#if SAVETILES
            IFolder rootFolfer = await FileSystem.Current.GetFolderFromPathAsync("d:\\");
            IFolder geoFolder = await rootFolfer.CreateFolderAsync("geo", CreationCollisionOption.OpenIfExists);
            string mapImageTileName = $"map_{parameters["x"]}_{parameters["y"]}_{parameters["z"]}.png";
            IFile file = await geoFolder.CreateFileAsync(mapImageTileName, CreationCollisionOption.ReplaceExisting);
            using (System.IO.Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                stream.Write(tileBytes, 0, tileBytes.Length);
            }

#endif

            ExtendedImage img = new ExtendedImage();
            PngDecoder pngDecoder = new PngDecoder();
            using (MemoryStream ms = new MemoryStream(tileBytes))
            {
                pngDecoder.Decode(img, ms);
            }
            
            ct.ThrowIfCancellationRequested();
            if (img.PixelWidth == 1)
                throw new ImageFormatException("RequestMapTile - Invalid tile image");
            int[] imgPixelsInt = new int[256 * 256];
            for (int i = 0; i < 256 * 256; i++)
            {
                int R = img.Pixels[i * 4];
                int G = img.Pixels[i * 4 + 1];
                int B = img.Pixels[i * 4 + 2];
                int A = img.Pixels[i * 4 + 3];
                imgPixelsInt[i] = (A << 24) + (R << 16) + (G << 8) + B;
            }

            return imgPixelsInt;
        }

        private int m_UrlMapInfoServerNumber = 1;
        public async Task<List<GCGeocache>> RequestMapInfo(IGCConnector gcConnecter, string url, Dictionary<string, string> parameters, string referer, int[] tilePixels, int zoomLevel, User currentUser, CancellationToken cancellationToken)
        {
            string urlString = string.Format("http://tiles01.geocaching.com/map/map.info", m_UrlMapInfoServerNumber);
            var urlString2 = FormUrl(urlString, parameters);
            Debug.WriteLine("Tile info url :" + urlString2);
            m_UrlMapInfoServerNumber = (m_UrlMapInfoServerNumber + 1) % 4 + 1;

            string data = await gcConnecter.GetPage(urlString2);
            cancellationToken.ThrowIfCancellationRequested();

#if SAVETILES
            IFolder rootFolfer = await FileSystem.Current.GetFolderFromPathAsync("d:\\");
            IFolder geoFolder = await rootFolfer.CreateFolderAsync("geo", CreationCollisionOption.OpenIfExists);
            string mapImageTileName = $"map_{parameters["x"]}_{parameters["y"]}_{parameters["z"]}.json";
            IFile file = await geoFolder.CreateFileAsync(mapImageTileName, CreationCollisionOption.ReplaceExisting);
            using (System.IO.Stream stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                using (TextWriter tw = new StreamWriter(stream))
                {
                    tw.Write(data);
                }
                //stream.Write(tileBytes,0,tileBytes.Length);
            }

#endif

            return ParseMapInfos(data, tilePixels, cancellationToken, zoomLevel, currentUser);
        }

        private List<GCGeocache> ParseMapInfos(string jsonResult, int[] TilePixels, CancellationToken ct, int zoomLevel, User currentUser)
        {
            if (!string.IsNullOrWhiteSpace(jsonResult))
            {
                var nameCache = new Dictionary<string, string>(); // JSON id, cache name

                var parsedData =
                    (GeocachingComApiCaches)
                        JsonConvert.DeserializeObject(jsonResult, typeof(GeocachingComApiCaches));

                var keys = parsedData.keys;

                var positions = new Dictionary<string, List<UTFGridPosition>>();

                int startTimer = Environment.TickCount;

                Debug.WriteLine($"JSon Keys = {keys.Length}");

                // JSON id as Key
                for (var i = 0; i < keys.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    // index 0 is empty
                    var key = keys[i];
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        var pos = UTFGridPosition.FromString(key);

                        var dataForKey = parsedData.data[key];
                        foreach (var c in dataForKey)
                        {
                            ct.ThrowIfCancellationRequested();
                            var id = c.i;
                            if (!nameCache.ContainsKey(id))
                            {
                                nameCache.Add(id, c.n);
                            }

                            if (!positions.ContainsKey(id))
                            {
                                positions.Add(id, new List<UTFGridPosition>());
                            }
                            if (positions.Count > MaxCachesOnTile)
                            {
                                //Debug.WriteLine("Tile contains " + positions.Keys.Count + " caches, too much!");
                                //ContainsTooManyCaches = true;
                                //break;
                                //ContainsTooManyCaches = true;
                                //throw new Exception("Too many caches on tile");
                            }
                            positions[id].Add(pos);
                        }
                    }
                    //if (ContainsTooManyCaches)
                    //    break;
                }
                Debug.WriteLine("Parsing positions : " + (Environment.TickCount - startTimer) + " ms");


                //if (positions.Keys.Count > MaxCachesOnTile)
                //{
                //    Debug.WriteLine("Tile contains " + positions.Keys.Count + " caches, too much!");
                //    ContainsTooManyCaches = true;
                //    throw new TooManyCachesOnTileException();
                //}

                startTimer = Environment.TickCount;
                var caches = new List<GCGeocache>();

                foreach (var id in positions.Keys)
                {
                    ct.ThrowIfCancellationRequested();

                    var pos = positions[id];
                    var xy = UTFGrid.GetPositionInGrid(pos);
                    var cache = new GCGeocache()
                    {
                        Name = nameCache[id],
                        Code = id
                    };
                    caches.Add(cache);
                    cache.SetWaypoint(GetCoord(xy), zoomLevel);
                    cache.SetGeocacheType(GeocacheType.Unknown, zoomLevel);
                    cache.Status = GeocacheStatus.Published;
                    foreach (var singlepos in positions[id])
                    {
                        ct.ThrowIfCancellationRequested();
                        if (IconDecoder.parseMapPNG(cache, TilePixels, singlepos, Zoomlevel, currentUser))
                            break;
                    }
                }
                Debug.WriteLine("Parsing images : " + (Environment.TickCount - startTimer) + " ms");

                return caches;
            }
            return null;
        }
    }
}