using System;
using System.Web;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class ApplicationCache :  ICacheArea
    {
        public CacheArea Area { get { return CacheArea.Global; } }
        public string Name { get { return "DefaultGlobal"; } }
        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            try
            {
                var t = (tt)HttpRuntime.Cache[name.ToUpper()];
                object comp = t;
                object empty = default(tt);
                if (comp == empty)
                {
                    if (createMethod != null)
                    {
                        t = createMethod();
                        SetItem(name, t,lifeSpanSeconds );
                    }
                }
                return t;
            }
            catch
            {
                throw;
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            //if (HttpRuntime.Cache.)

            if (obj != null)
            {
                HttpRuntime.Cache[name.ToUpper()] = obj;
            }
            else
            {
                HttpRuntime.Cache.Remove(name.ToUpper());
            }
        }
    }
}
