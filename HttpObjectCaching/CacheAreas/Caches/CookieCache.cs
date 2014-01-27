using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class CookieCache : ICacheArea, INameValueLister
    {
        private object createLock = new object();
        private object setLock = new object();
        public CacheArea Area { get { return CacheArea.Cookie ; } }
        public string Name { get { return "DefaultCookie"; } }
        public void ClearCache()
        {
            Cache.SetItem<Dictionary<string, object>>(CacheArea.Permanent, "Cookie_" + CacheSystem.Instance.CookieId, null);
        }

        public Dictionary<string, object> DataDictionary
        {
            get { return Cookie; }
        }
        public Dictionary<string, object> Cookie
        {
            get
            {
                lock (createLock)
                {
                    return Cache.GetItem(CacheArea.Permanent, "Cookie_" + CacheSystem.Instance.CookieId, () => new Dictionary<string, object>());
                }
            }
        }


        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            if (Cookie.ContainsKey(name.ToUpper()))
            {
                var t = (tt)Cookie[name.ToUpper()];
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
            lock (setLock)
            {
                if (Cookie.ContainsKey(name.ToUpper()))
                {
                    Cookie.Remove(name.ToUpper());
                }
                Cookie.Add(name.ToUpper(), obj);
            }
        }
    }
}
