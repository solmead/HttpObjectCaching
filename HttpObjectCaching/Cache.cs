using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Web;
using HttpObjectCaching.CacheAreas;

namespace HttpObjectCaching
{
    public enum CacheArea
    {
        Other,
        Permanent,
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
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds)
        {
            double? lSS = lifeSpanSeconds;
            if (lSS == 0)
            {
                lSS = null;
            }
            if (!name.Contains("CacheEnabled") && !CacheSystem.Instance.CacheEnabled)
            {
                if (createMethod != null)
                {
                    return createMethod();
                }
                return default(tt);
            }
            var ca = CacheSystem.Instance.GetCacheArea(area);
            if (ca != null)
            {
                return ca.GetItem<tt>(name, createMethod, lSS);
            }

            if (createMethod != null)
            {
                return createMethod();
            }
            return default(tt);
        }

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod)
        {
            return GetItem<tt>(area, name, createMethod, 0);
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
            return GetItem<tt>(area, name, () => default(tt), 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, double lifeSpanSeconds)
        {
            return GetItem<tt>(area, name, () => default(tt), lifeSpanSeconds);
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
            return GetItem<tt>(area, name, () => defaultValue, 0);
        }

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue, double lifeSpanSeconds)
        {
            return GetItem<tt>(area, name, () => defaultValue, lifeSpanSeconds);
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

            var ca = CacheSystem.Instance.GetCacheArea(area);
            if (ca != null)
            {
                ca.SetItem<tt>(name, obj);
            }

        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem<tt>(CacheArea area, string name, tt obj, double lifeSpanSeconds)
        {

            var ca = CacheSystem.Instance.GetCacheArea(area);
            if (ca != null)
            {
                ca.SetItem<tt>(name, obj, lifeSpanSeconds);
            }
            
        }

    }
}
