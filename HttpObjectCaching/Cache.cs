using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Web;

namespace HttpObjectCaching
{
    public enum CacheArea
    {
        Global,
        Session,
        Request,
        Thread
    }
    public static class Cache
    {
        
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod)
        {
            if (area == CacheArea.Thread)
            {
                return CacheSystem.Instance.GetFromThread<tt>(name, createMethod);
            }
            if (area == CacheArea.Request)
            {
                return CacheSystem.Instance.GetFromRequest<tt>(name, createMethod);
            }
            if (area == CacheArea.Session)
            {
                return CacheSystem.Instance.GetFromSession<tt>(name, createMethod);
            }
            if (area == CacheArea.Global)
            {
                return CacheSystem.Instance.GetFromApplication<tt>(name, createMethod);
            }

            return default(tt);
        }
        public static tt GetItem<tt>(CacheArea area, string name)
        {
            return GetItem<tt>(area, name, ()=> default(tt));
        }
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue)
        {
            return GetItem<tt>(area, name, () => defaultValue);
        }
        public static void SetItem<tt>(CacheArea area, string name, tt obj)
        {

            if (area == CacheArea.Thread )
            {
                CacheSystem.Instance.SetInThread<tt>(name, obj);
            }
            if (area == CacheArea.Request)
            {
                CacheSystem.Instance.SetInRequest<tt>(name, obj);
            }
            if (area == CacheArea.Session)
            {
                CacheSystem.Instance.SetInSession<tt>(name, obj);
            }
            if (area == CacheArea.Global)
            {
                CacheSystem.Instance.SetInApplication<tt>(name, obj);
            }
            
    }

    }
}
