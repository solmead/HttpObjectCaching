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
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name)
        {
            return GetItem<tt>(area, name, ()=> default(tt));
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue)
        {
            return GetItem<tt>(area, name, () => defaultValue);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
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
