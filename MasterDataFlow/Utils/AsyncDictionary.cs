using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MasterDataFlow.Utils
{
    public class AsyncDictionary<TKey, TI> 
    {
        private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TI> _cache = new Dictionary<TKey, TI>(1000);

        #region Items Cache
        public virtual void AddItem(TKey itemId, TI item)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                TI cachedItem;
                if (_cache.TryGetValue(itemId, out cachedItem))
                {
                    throw new ArgumentException("Duplicate key");
                }
                else
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        _cache.Add(itemId, item);
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        public virtual void RemoveItem(TKey itemId)
        {
            _cacheLock.EnterUpgradeableReadLock();
            try
            {
                TI cachedItem;
                if (_cache.TryGetValue(itemId, out cachedItem))
                {
                    _cacheLock.EnterWriteLock();
                    try
                    {
                        _cache.Remove(itemId);
                    }
                    finally
                    {
                        _cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        public virtual TI GetItem(TKey itemId)
        {
            _cacheLock.EnterReadLock();
            try
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
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public virtual List<TI> GetItems()
        {
            _cacheLock.EnterReadLock();
            try
            {
                return _cache.Values.Select(t => t).ToList();
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }


        #endregion

    }
}
