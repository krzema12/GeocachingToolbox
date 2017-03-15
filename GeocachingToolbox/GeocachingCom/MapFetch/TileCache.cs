namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    internal class TileCache
    {
        private readonly LruCache<int, Tile> _tileCache = new LruCache<int, Tile>(64);

        public bool Contains(Tile tile)
        {
            return _tileCache.ContainsKey(tile.GetHashCode());
        }

        public Tile Get(Tile tile)
        {
            return _tileCache.Get(tile.GetHashCode());
        }

        public void Add(Tile tile)
        {
            _tileCache.Add(tile.GetHashCode(), tile);
        }
    }
}