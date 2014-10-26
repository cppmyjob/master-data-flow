using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MasterDataFlow.Utils
{
    public class AsyncDictionary<TKey, TI> 
    {
        private readonly object _lockObject = new object();
        private readonly Dictionary<TKey, TI> _cache = new Dictionary<TKey, TI>(1000);

        #region Items Cache
        public virtual void AddItem(TKey itemId, TI item)
        {
            lock (_lockObject)
            {
                _cache.Add(itemId, item);
            }
        }

        public virtual void RemoveItem(TKey itemId)
        {
            lock (_lockObject)
            {
                _cache.Remove(itemId);
            }
        }

        public virtual TI GetItem(TKey itemId)
        {
            lock (_lockObject)
            {
                TI cachedItem;
                if (_cache.TryGetValue(itemId, out cachedItem))
                {
                    return cachedItem;
                }
                else
                {
                    return default(TI);
                }
            }
        }

        public virtual List<TI> GetItems()
        {
            lock (_lockObject)
            {
                return _cache.Values.Select(t => t).ToList();
            }
        }

        #endregion
    }
}
