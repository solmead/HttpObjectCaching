using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class SessionCache : ICacheArea
    {
        private object sessionSetLock = new object();
        public CacheArea Area { get { return CacheArea.Session; } }
        public string Name { get { return "DefaultSession"; } }
        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            var Session = CacheSystem.Instance.Session;
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
                var Session = CacheSystem.Instance.Session;
                if (Session.ContainsKey(name.ToUpper()))
                {
                    Session.Remove(name.ToUpper());
                }
                Session.Add(name.ToUpper(), obj);
            }
        }
    }
}
