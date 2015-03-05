using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Caches;
using HttpObjectCaching.Core.DataSources;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core
{
    public abstract class DictionaryCache : ICacheArea, INameValueLister
    {
        private object createLock = new object();
        private object setLock = new object();
        private ICacheArea _cache = null;

        private ICacheArea _baseTempCache = new NoCache();

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
        private Func<Task<string>> GetInstanceId = null;
        public double LifeSpanInSeconds { get; set; }

        public DictionaryCache(BaseCacheArea cacheTo, Func<Task<string>> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            CacheToType = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public DictionaryCache(ICacheArea cacheTo, Func<Task<string>> getInstanceId, double lifeSpanInSeconds = 30*60)
        {
            LifeSpanInSeconds = lifeSpanInSeconds;
            _cache = cacheTo;
            GetInstanceId = getInstanceId;
        }

        public CacheArea Area { get; protected set; }
        public string Name { get; protected set; }


        //public IDictionary<string, object> DataDictionary
        //{
        //    get
        //    {
        //        return AsyncHelper.RunSync(DataDictionaryGet);
        //    }
        //}

        public async Task<IDictionary<string, object>> DataDictionaryGet()
        {
            //System.Collections.Concurrent.ConcurrentDictionary<>
            var dic = new Dictionary<string, object>();
            foreach (var ce in (await BaseDictionaryGetAsync()).Values)
            {
                if (!dic.ContainsKey(ce.Name.ToUpper()))
                {
                    dic.Add(ce.Name.ToUpper(), ce.TheObject);
                }
            }
            return dic;
        }

        private async Task<ConcurrentDictionary<string, CachedEntryBase>> BaseDictionaryGetAsync()
        {
            //lock (createLock)
            //{
                return await _baseTempCache.GetItemAsync(Name + "_DataDictionary_Thread_" + GetInstanceId(),
                    async () =>
                        await CacheTo.GetItemAsync<ConcurrentDictionary<string, CachedEntryBase>>(
                            Name + "_DataDictionary_Base_" + GetInstanceId(),
                            async () => new ConcurrentDictionary<string, CachedEntryBase>(), LifeSpanInSeconds), 1);
            //}
        }

        private async Task BaseDictionarySetAsync(ConcurrentDictionary<string, CachedEntryBase> value)
        {
            //lock (createLock)
            //{
            await CacheTo.SetItemAsync<ConcurrentDictionary<string, CachedEntryBase>>(Name + "_DataDictionary_Base_" + GetInstanceId(),
                    value, LifeSpanInSeconds);
                await _baseTempCache.SetItemAsync(Name + "_DataDictionary_Thread_" + GetInstanceId(), value,
                    1);
            //}
        }





        public async Task ClearCacheAsync()
        {
            (await BaseDictionaryGetAsync()).Clear();
            //await BaseDictionarySetAsync(null);
        }

        public async Task<tt> GetItemAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null)
        {
            var dd = await BaseDictionaryGetAsync();
            object empty = default(tt);
            CachedEntryBase itm2;
            CachedEntry<tt> itm;
            dd.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<tt>;
            //if (dd.TryGetValue())
            //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
            if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
            {
                if (createMethod != null)
                {
                    var t = await createMethod();
                    await SetItemAsync(name, t, lifeSpanSeconds);
                    return t;
                }
            }
            else
            {
                return (tt)itm.Item;
            }
            return default(tt);
        }

        public async Task SetItemAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var dd = await BaseDictionaryGetAsync();
            CachedEntryBase itm2 = null;
            CachedEntry<tt> itm = null;
            dd.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<tt>;
            //var itm = dd.getByName(name.ToUpper()) as CachedEntry<tt>;
            if (itm == null)
            {
                itm = new CachedEntry<tt>()
                {
                    Created = DateTime.Now,
                    Name = name,
                    Changed = DateTime.Now
                };
                if (!dd.TryAdd(name.ToUpper(), itm))
                {
                    dd.TryGetValue(name.ToUpper(), out itm2);
                    itm = itm2 as CachedEntry<tt>;
                }
            }
            if (itm != null)
            {
                if (lifeSpanSeconds.HasValue)
                {
                    itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
                itm.Changed = DateTime.Now;
                itm.Item = obj;
            }

            var lst =
                (from ce in dd.Values where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now select ce).ToList();
            if (lst.Count > 0)
            {
                foreach (var dit in lst)
                {
                    dd.TryRemove(dit.Name.ToUpper(), out itm2);
                }
            }
            await BaseDictionarySetAsync(dd);
        }


    }
}

