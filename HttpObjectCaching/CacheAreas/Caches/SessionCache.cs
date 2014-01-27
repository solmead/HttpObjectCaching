using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class SessionCache : ICacheArea, INameValueLister
    {
        private object sessionCreateLock = new object();
        private object sessionSetLock = new object();
        public CacheArea Area { get { return CacheArea.Session; } }
        public string Name { get { return "DefaultSession"; } }
        public void ClearCache()
        {
            Cache.SetItem<Dictionary<string, object>>(CacheArea.Global, "Session_" + CacheSystem.Instance.SessionId, null);
        }

        public Dictionary<string, object> DataDictionary
        {
            get { return Session; }
        }

        public Dictionary<string, object> Session
        {
            get
            {
                lock (sessionCreateLock)
                {
                    return Cache.GetItem(CacheArea.Global, "Session_" + CacheSystem.Instance.SessionId, () => new Dictionary<string, object>(), 30*60);
                }
            }
        }


        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            if (Session.ContainsKey(name.ToUpper()))
            {
                var t = (tt)Session[name.ToUpper()];
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
            lock (sessionSetLock)
            {
                if (Session.ContainsKey(name.ToUpper()))
                {
                    Session.Remove(name.ToUpper());
                }
                Session.Add(name.ToUpper(), obj);
            }
        }

    }
}
