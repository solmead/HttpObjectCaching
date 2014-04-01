using System;
using System.Web;
using System.Web.Caching;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class ApplicationBaseCache :  ICacheArea
    {
        public CacheArea Area { get { return CacheArea.Global; } }
        public string Name { get { return "BaseGlobal"; } }
        public void ClearCache()
        {
            throw new NotImplementedException();
        }

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
                        SetItem(name, t, lifeSpanSeconds );
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
                try
                {
                    HttpRuntime.Cache.Remove(name.ToUpper());
                }
                catch (Exception)
                {
                    
                }
                if (lifeSpanSeconds.HasValue)
                {
                    int totSeconds = (int) (lifeSpanSeconds.Value);
                    int ms = (int) ((lifeSpanSeconds.Value - (1.0*totSeconds))*1000.0);
                    HttpRuntime.Cache.Insert(name.ToUpper(), obj, null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration,
                        new TimeSpan(0,0,0,totSeconds,ms),
                        CacheItemPriority.Default, null);
                }
                else
                {
                    HttpRuntime.Cache[name.ToUpper()] = obj; 
                }


            }
            else
            {
                HttpRuntime.Cache.Remove(name.ToUpper());
            }
        }
    }
}
