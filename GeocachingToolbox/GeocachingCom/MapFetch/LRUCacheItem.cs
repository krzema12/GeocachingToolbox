namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    internal class LRUCacheItem<K, V>
    {
        public K Key { get; }
        public V Value { get; }
        public LRUCacheItem(K k, V v)
        {
            Key = k;
            Value = v;
        }
    }
}