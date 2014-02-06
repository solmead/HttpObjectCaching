using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.CacheAreas
{
    public abstract class DictionaryCache : ICacheArea, INameValueLister
    {
        private object createLock = new object();
        private object setLock = new object();
        private ICacheArea _cache = null;

        public ICacheArea CacheTo
        {
            get
            {
                if (_cache != null)
                {
                    return _cache;
                }
                return CacheSystem.Instance.GetCacheArea((CacheArea) CacheToType);
            }
        }

        public BaseCacheArea CacheToType { get; set; }
        private Func<string> GetInstanceId = null;
        public double LifeSpanInSeconds { get; set; }

        public DictionaryCache(BaseCacheArea cacheTo, Func<string> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            CacheToType = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public DictionaryCache(ICacheArea cacheTo, Func<string> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            _cache = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public CacheArea Area { get; protected set; }
        public string Name { get; protected set; }
        public void ClearCache()
        {
            CacheTo.SetItem<SerializableDictionary<string, object>>(Name + "_" + GetInstanceId(), null);
        }

        public IDictionary<string, object> DataDictionary { get { return BaseDictionary; } }

        private SerializableDictionary<string, object> BaseDictionary
        {
            get
            {
                lock (createLock)
                {
                    return Cache.GetItem(CacheArea.Thread, Name + "_DataDictionary_" + GetInstanceId(), () => CacheTo.GetItem<SerializableDictionary<string, object>>(Name + "_" + GetInstanceId(), () => new SerializableDictionary<string, object>(), LifeSpanInSeconds));
                }
            }
             set
            {

                CacheTo.SetItem<SerializableDictionary<string, object>>(Name + "_" + GetInstanceId(), value, LifeSpanInSeconds);
            }
        }


        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            var dd = BaseDictionary;
            if (dd.ContainsKey(name.ToUpper()))
            {
                var t = (tt)dd[name.ToUpper()];
                object comp = t;
                object empty = default(tt);
                if (comp == empty)
                {
                    if (createMethod != null)
                    {
                        t = createMethod();
                        SetItem(name, t, lifeSpanSeconds);
                    }
                }
                return t;
            }
            else
            {
                if (createMethod != null)
                {
                    var t = createMethod();
                    SetItem(name, t, lifeSpanSeconds);
                    return t;
                }
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var dd = BaseDictionary;
            lock (setLock)
            {
                if (dd.ContainsKey(name.ToUpper()))
                {
                    dd.Remove(name.ToUpper());
                }
                dd.Add(name.ToUpper(), obj);
            }
            BaseDictionary = dd;
        }
    }
}
