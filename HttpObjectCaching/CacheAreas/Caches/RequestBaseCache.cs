using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class RequestBaseCache : ICacheArea
    {
        private object requestSetLock = new object();

        public CacheArea Area { get { return CacheArea.Request; } }
        public string Name { get { return "BaseRequest"; } }
        public void ClearCache()
        {
            //throw new NotImplementedException();
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        var t = (tt)context.Items[name.ToUpper()];

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
                }
            }
            else
            {
                return CacheSystem.Instance.GetCacheArea(CacheArea.Thread).GetItem<tt>(name.ToUpper(), createMethod, lifeSpanSeconds);
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        context.Items.Remove(name.ToUpper());
                    }
                    context.Items.Add(name.ToUpper(), obj);
                }
            }
            else
            {
                CacheSystem.Instance.GetCacheArea(CacheArea.Thread).SetItem<tt>(name.ToUpper(), obj, lifeSpanSeconds);
            }
        }
    }
}
