using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageTools;

namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    public class MapFetcher
    {
        private readonly TileCache _tileCache = new TileCache();
        private static double MilliTimeStamp()
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = DateTime.UtcNow;
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }

        readonly Object LockObject = new object();
        public async Task<List<GCGeocache>> FetchCaches(IGCConnector gcConnector, GCMapToken mapToken, Location topLeft, Location bottomRight, User currentUser, CancellationToken cancellationToken)
        {
            var viewport = new Viewport(topLeft, bottomRight);

            var tiles = Tile.GetTilesForViewport(viewport);
            var caches = new List<GCGeocache>();
            var tsk = new List<Task>();
            foreach (Tile tile in tiles)
            {
                var t = Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!_tileCache.Contains(tile))
                    {
                        var parameters = new Dictionary<string, string>()
                        {
                            {"x", tile.TileX + ""},
                            {"y", tile.TileY + ""},
                            {"z", tile.Zoomlevel + ""},
                            {"ep", "1"},
                        };

                        if (tile.Zoomlevel != 14)
                        {
                            parameters.Add("_", MilliTimeStamp() + "");
                        }

                        var currentTile = tile;
                        try
                        {
                            var tilePixels = await Tile.RequestMapTile(mapToken, parameters, gcConnector, cancellationToken);
                            cancellationToken.ThrowIfCancellationRequested();
                            var c =
                                await
                                    currentTile.RequestMapInfo(gcConnector, GCConstants.URL_MAP_INFO, parameters,
                                        GCConstants.URL_LIVE_MAP, tilePixels, currentTile.Zoomlevel, currentUser, cancellationToken);
                            currentTile.Caches = c;
                            lock (LockObject)
                            {
                                caches.AddRange(c);
                            }
                            cancellationToken.ThrowIfCancellationRequested();
                            _tileCache.Add(currentTile);
                        }
                        catch (ImageFormatException)
                        {

                        }
                    }
                    else
                    {
                        Debug.WriteLine("Reusing tile");
                        var cachedTile = _tileCache.Get(tile);
                        lock (LockObject)
                        {
                            caches.AddRange(cachedTile.Caches);
                        }
                    }
                    //}
                });
                tsk.Add(t);
            }
            await Task.WhenAll(tsk);
            caches.RemoveAll(c => !viewport.Contains(c.Waypoint));
            return caches.GroupBy(item => item.Code).Select(group => group.First()).ToList();
            // return caches;
        }
    }
}
