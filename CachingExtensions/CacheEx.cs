using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HttpObjectCaching
{
    public static class CacheEx
    {
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return Nito.AsyncEx.AsyncContext.Run(func);
        }

        public static void RunSync(Func<Task> func)
        {
            Nito.AsyncEx.AsyncContext.Run(func);
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
        //[Obsolete("Use Async Version")]
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds)
        {
            return RunSync(() => Cache.GetItemAsync<tt>(area, name, async () => createMethod(), lifeSpanSeconds));
        }
        public static object GetItem(CacheArea area, string name, Type type, Func<object> createMethod, double lifeSpanSeconds)
        {
            return RunSync(() => Cache.GetItemAsync(area, name, type, async () => createMethod(), lifeSpanSeconds));
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
        //[Obsolete("Use Async Version")]
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod)
        {
            return GetItem<tt>(area, name, createMethod, 0);
        }
        public static object GetItem(CacheArea area, string name, Type type, Func<object> createMethod)
        {
            return GetItem(area, name, type, createMethod, 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        //[Obsolete("Use Async Version")]
        public static tt GetItem<tt>(CacheArea area, string name)
        {
            return GetItem<tt>(area, name, () => default(tt), 0);
        }
        public static object GetItem(CacheArea area, string name, Type type)
        {
            return GetItem(area, name, type, () => null, 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        //[Obsolete("Use Async Version")]
        public static tt GetItem<tt>(CacheArea area, string name, double lifeSpanSeconds)
        {
            return GetItem<tt>(area, name, () => default(tt), lifeSpanSeconds);
        }
        public static object GetItem(CacheArea area, string name, Type type, double lifeSpanSeconds)
        {
            return GetItem(area, name, type, () => null, lifeSpanSeconds);
        }

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        //[Obsolete("Use Async Version")]
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue)
        {
            return GetItem<tt>(area, name, () => defaultValue, 0);
        }
        public static object GetItem(CacheArea area, string name, Type type, object defaultValue)
        {
            return GetItem(area, name, type, () => defaultValue, 0);
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
        //[Obsolete("Use Async Version")]
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue, double lifeSpanSeconds)
        {
            return GetItem<tt>(area, name, () => defaultValue, lifeSpanSeconds);
        }
        public static object GetItem(CacheArea area, string name, Type type, object defaultValue, double lifeSpanSeconds)
        {
            return GetItem(area, name, type, () => defaultValue, lifeSpanSeconds);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        //[Obsolete("Use Async Version")]
        public static void SetItem<tt>(CacheArea area, string name, tt obj)
        {
            RunSync(() => Cache.SetItemAsync<tt>(area, name, obj));
        }
        public static void SetItem(CacheArea area, string name, Type type, object obj)
        {
            RunSync(() => Cache.SetItemAsync(area, name, type, obj));
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        //[Obsolete("Use Async Version")]
        public static void SetItem<tt>(CacheArea area, string name, tt obj, double lifeSpanSeconds)
        {
            RunSync(() => Cache.SetItemAsync<tt>(area, name, obj, lifeSpanSeconds));
        }
        public static void SetItem(CacheArea area, string name, Type type, object obj, double lifeSpanSeconds)
        {
            RunSync(() => Cache.SetItemAsync(area, name, type, obj, lifeSpanSeconds));
        }


    }
}
