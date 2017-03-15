using System.Collections.Generic;
using System.Linq;

namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    public class LruCache<K, V>
    {
        private readonly object Lock = new object();
        readonly int capacity;

        readonly Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>> cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem<K, V>>>();
        readonly LinkedList<LRUCacheItem<K, V>> lruList = new LinkedList<LRUCacheItem<K, V>>();

        public LruCache(int capacity)
        {
            this.capacity = capacity;
        }


        public bool ContainsKey(K key)
        {
            return cacheMap.ContainsKey(key);
        }


        public V Get(K key)
        {
            //lock (Lock)
            {
                LinkedListNode<LRUCacheItem<K, V>> node;
                if (cacheMap.TryGetValue(key, out node))
                {
                    var value = node.Value.Value;

                    lruList.Remove(node);
                    lruList.AddLast(node);
                    return value;
                }
                return default(V);
            }
        }


        public void Add(K key, V val)
        {
            lock (Lock)
            {
                if (cacheMap.Count >= capacity)
                {
                    RemoveFirst();
                }
                var cacheItem = new LRUCacheItem<K, V>(key, val);
                var node = new LinkedListNode<LRUCacheItem<K, V>>(cacheItem);
                lruList.AddLast(node);
                if (!cacheMap.ContainsKey(key))
                {
                    cacheMap.Add(key, node);
                }
            }
        }

        public IEnumerable<V> GetValues()
        {
            return cacheMap.Values.Select(v => v.Value.Value).ToList();
        }

        protected void RemoveFirst()
        {
            // Remove from LRUPriority
            var node = lruList.First;
            lruList.RemoveFirst();
            // Remove from cache

            cacheMap.Remove(node.Value.Key);
        }



        public void Clear()
        {
            cacheMap.Clear();
            lruList.Clear();
        }
    }
}