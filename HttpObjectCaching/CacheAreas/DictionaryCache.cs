using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HttpObjectCaching.CacheAreas.Caches;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.CacheAreas
{
    public abstract class DictionaryCache : ICacheArea, INameValueLister
    {
        private object createLock = new object();
        private object setLock = new object();
        private ICacheArea _cache = null;

        private ICacheArea _baseTempCache = new ThreadBaseCache();

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
            BaseDictionary.Clear();
            BaseDictionary = null;
        }

        public IDictionary<string, object> DataDictionary
        {
            get
            {
                var dic = new Dictionary<string, object>();
                foreach (var ce in BaseDictionary)
                {
                    if (!dic.ContainsKey(ce.Name.ToUpper()))
                    {
                        dic.Add(ce.Name.ToUpper(), ce.TheObject);
                    }
                }
                return dic;
            }
        }

        private SerializableList<CachedEntryBase> BaseDictionary
        {
            get
            {
                lock (createLock)
                {
                    return _baseTempCache.GetItem(Name + "_DataDictionary_Thread_" + GetInstanceId(),
                        () =>
                            CacheTo.GetItem<SerializableList<CachedEntryBase>>(
                                Name + "_DataDictionary_Base_" + GetInstanceId(),
                                () => new SerializableList<CachedEntryBase>(), LifeSpanInSeconds), 1);
                }
            }
            set
            {
                lock (createLock)
                {
                    CacheTo.SetItem<SerializableList<CachedEntryBase>>(Name + "_DataDictionary_Base_" + GetInstanceId(),
                        value, LifeSpanInSeconds);
                    _baseTempCache.SetItem(Name + "_DataDictionary_Thread_" + GetInstanceId(), value,
                        1);
                }
            }
        }



        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            lock (setLock)
            {
                var dd = BaseDictionary;
                object empty = default(tt);
                var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
                if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
                {
                    if (createMethod != null)
                    {
                        var t = createMethod();
                        SetTheItem(name, t, lifeSpanSeconds);
                        return t;
                    }
                }
                else
                {
                    return (tt) itm.Item;
                }
                return default(tt);
            }
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            lock (setLock)
            {
                SetTheItem(name, obj, lifeSpanSeconds);
            }
        }

        private void SetTheItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var dd = BaseDictionary;
                var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
                if (itm == null)
                {
                    itm = new CachedEntry<tt>()
                    {
                        Created = DateTime.Now,
                        Name = name,
                        Changed = DateTime.Now
                    };
                    dd.Add(itm);
                }
                if (lifeSpanSeconds.HasValue)
                {
                    itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
                itm.Changed = DateTime.Now;
                itm.Item = obj;


                var lst =
                    (from ce in dd where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now select ce).ToList();
                if (lst.Count > 0)
                {
                    foreach (var dit in lst)
                    {
                        dd.Remove(dit);
                    }
                }
            BaseDictionary = dd;
        }
    }
}

