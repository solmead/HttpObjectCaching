using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas
{
    public abstract class DictionaryCache : ICacheArea, INameValueLister
    {
        private object createLock = new object();
        private object setLock = new object();
        public BaseCacheArea CacheTo { get; set; }
        private Func<string> GetInstanceId = null;
        public double LifeSpanInSeconds { get; set; }

        public DictionaryCache(BaseCacheArea cacheTo, Func<string> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            CacheTo = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public CacheArea Area { get; protected set; }
        public string Name { get; protected set; }
        public void ClearCache()
        {
            Cache.SetItem<Dictionary<string, object>>((CacheArea) CacheTo, Name + "_" + GetInstanceId(), null);
        }

        public Dictionary<string, object> DataDictionary
        {
            get
            {
                lock (createLock)
                {
                    return Cache.GetItem(CacheArea.Thread, Name + "_DataDictionary_" + GetInstanceId(), () => Cache.GetItem((CacheArea)CacheTo, Name + "_" + GetInstanceId(), () => new Dictionary<string, object>(), LifeSpanInSeconds));
                }
            }
            private set
            {
                Cache.SetItem((CacheArea)CacheTo, Name + "_" + GetInstanceId(), value, LifeSpanInSeconds);
            }
        }


        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            var dd = DataDictionary;
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
            var dd = DataDictionary;
            lock (setLock)
            {
                if (dd.ContainsKey(name.ToUpper()))
                {
                    dd.Remove(name.ToUpper());
                }
                dd.Add(name.ToUpper(), obj);
            }
            DataDictionary = dd;
        }
    }
}
