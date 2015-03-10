using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching
{
    public enum CacheArea
    {
        Other = 0,
        Permanent = 1,
        Distributed = 2,
        Global = 3,
        Cookie = 4,
        Session = 5,
        Request = 6,
        //Thread = 7,
        None = 8
    }

    public enum BaseCacheArea
    {
        Other = 0,
        Permanent = 1,
        Distributed = 2,
        Global = 3,
        Request = 6,
        //Thread = 7,
        None = 8
    }
    public static class Cache
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        private static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var ctx = HttpContext.Current;

            return MyTaskFactory.StartNew(() =>
            {
                HttpContext.Current = ctx;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }

        private static void RunSync(Func<Task> func)
        {
            var ctx = HttpContext.Current;
            MyTaskFactory.StartNew(() =>
            {
                HttpContext.Current = ctx;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }


        ///// <summary>
        ///// Pull an item from the cache
        ///// </summary>
        ///// <typeparam name="tt">Type of object being requested</typeparam>
        ///// <param name="area">What area object is stored in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        ///// <param name="lifeSpanSeconds"></param>
        ///// <returns></returns>
        //[Obsolete("Use Async Version")]
        //public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds)
        //{
        //    return RunSync(()=> GetItemAsync<tt>(area, name, async ()=>createMethod(), lifeSpanSeconds));
        //}

        ///// <summary>
        ///// Pull an item from the cache
        ///// </summary>
        ///// <typeparam name="tt">Type of object being requested</typeparam>
        ///// <param name="area">What area object is stored in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        ///// <param name="lifeSpanSeconds"></param>
        ///// <returns></returns>
        //[Obsolete("Use Async Version")]
        //public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod)
        //{
        //    return GetItem<tt>(area, name, createMethod, 0);
        //}

        ///// <summary>
        ///// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        ///// </summary>
        ///// <typeparam name="tt">Type of object being requested</typeparam>
        ///// <param name="area">What area object is stored in</param>
        ///// <param name="name">Name of object</param>
        ///// <returns></returns>
        //[Obsolete("Use Async Version")]
        //public static tt GetItem<tt>(CacheArea area, string name)
        //{
        //    return GetItem<tt>(area, name, () => default(tt), 0);
        //}

        ///// <summary>
        ///// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        ///// </summary>
        ///// <typeparam name="tt">Type of object being requested</typeparam>
        ///// <param name="area">What area object is stored in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="lifeSpanSeconds"></param>
        ///// <returns></returns>
        //[Obsolete("Use Async Version")]
        //public static tt GetItem<tt>(CacheArea area, string name, double lifeSpanSeconds)
        //{
        //    return GetItem<tt>(area, name, () => default(tt), lifeSpanSeconds);
        //}

        ///// <summary>
        ///// Pull an item from the cache
        ///// </summary>
        ///// <typeparam name="tt">Type of object being requested</typeparam>
        ///// <param name="area">What area object is stored in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        ///// <returns></returns>
        //[Obsolete("Use Async Version")]
        //public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue)
        //{
        //    return GetItem<tt>(area, name, () => defaultValue, 0);
        //}

        ///// <summary>
        ///// Pull an item from the cache
        ///// </summary>
        ///// <typeparam name="tt">Type of object being requested</typeparam>
        ///// <param name="area">What area object is stored in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        ///// <param name="lifeSpanSeconds"></param>
        ///// <returns></returns>
        //[Obsolete("Use Async Version")]
        //public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue, double lifeSpanSeconds)
        //{
        //    return GetItem<tt>(area, name, () => defaultValue, lifeSpanSeconds);
        //}

        ///// <summary>
        ///// Puts an item into the cache
        ///// </summary>
        ///// <typeparam name="tt">Type of object being stored</typeparam>
        ///// <param name="area">What area to store object in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="obj">Object to store in cache location</param>
        //[Obsolete("Use Async Version")]
        //public static void SetItem<tt>(CacheArea area, string name, tt obj)
        //{
        //    RunSync(() => SetItemAsync<tt>(area, name, obj));
        //}

        ///// <summary>
        ///// Puts an item into the cache
        ///// </summary>
        ///// <typeparam name="tt">Type of object being stored</typeparam>
        ///// <param name="area">What area to store object in</param>
        ///// <param name="name">Name of object</param>
        ///// <param name="obj">Object to store in cache location</param>
        ///// <param name="lifeSpanSeconds"></param>
        //[Obsolete("Use Async Version")]
        //public static void SetItem<tt>(CacheArea area, string name, tt obj, double lifeSpanSeconds)
        //{
        //    RunSync(() => SetItemAsync<tt>(area, name, obj, lifeSpanSeconds));
        //}





        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds)
        {
            return await GetItemAsync<tt>(area, name, async () => createMethod(), lifeSpanSeconds);
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
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<tt> createMethod)
        {
            return await GetItemAsync<tt>(area, name, async ()=> createMethod(), 0);
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
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<Task<tt>> createMethod, double lifeSpanSeconds)
        {
            try
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
                        return await createMethod();
                    }
                    return default(tt);
                }
                var ca = CacheSystem.Instance.GetCacheArea(area);
                if (ca != null)
                {
                    return await ca.GetItemAsync<tt>(name, createMethod, lSS);
                }

                if (createMethod != null)
                {
                    return await createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message +" retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
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
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, Func<Task<tt>> createMethod)
        {
            return await GetItemAsync<tt>(area, name, createMethod, 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name)
        {
            return await GetItemAsync<tt>(area, name, async ()=> default(tt), 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, double lifeSpanSeconds)
        {
            return await GetItemAsync<tt>(area, name, async () => default(tt), lifeSpanSeconds);
        }

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, tt defaultValue)
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, 0);
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
        public static async Task<tt> GetItemAsync<tt>(CacheArea area, string name, tt defaultValue, double lifeSpanSeconds)
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, lifeSpanSeconds);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static async Task SetItemAsync<tt>(CacheArea area, string name, tt obj)
        {

            var ca = CacheSystem.Instance.GetCacheArea(area);
            if (ca != null)
            {
                await ca.SetItemAsync<tt>(name, obj);
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
        public static async Task SetItemAsync<tt>(CacheArea area, string name, tt obj, double lifeSpanSeconds)
        {

            var ca = CacheSystem.Instance.GetCacheArea(area);
            if (ca != null)
            {
                await ca.SetItemAsync<tt>(name, obj, lifeSpanSeconds);
            }

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
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, Func<Task<tt>> createMethod, double lifeSpanSeconds)
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
                    return await createMethod();
                }
                return default(tt);
            }
            var ca = new DataCache(area);
            return await ca.GetItemAsync<tt>(name, createMethod, lSS);
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
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, Func<Task<tt>> createMethod)
        {
            return await GetItemAsync<tt>(area, name, createMethod, 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name)
        {
            return await GetItemAsync<tt>(area, name, async () => default(tt), 0);
        }

        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, double lifeSpanSeconds)
        {
            return await GetItemAsync<tt>(area, name, async () => default(tt), lifeSpanSeconds);
        }

        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, tt defaultValue)
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, 0);
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
        public static async Task<tt> GetItemAsync<tt>(IDataSource area, string name, tt defaultValue, double lifeSpanSeconds)
        {
            return await GetItemAsync<tt>(area, name, async () => defaultValue, lifeSpanSeconds);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static async Task SetItemAsync<tt>(IDataSource area, string name, tt obj)
        {
            var ca = new DataCache(area);
            await ca.SetItemAsync<tt>(name, obj);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync<tt>(IDataSource area, string name, tt obj, double lifeSpanSeconds)
        {
            var ca = new DataCache(area);
            await ca.SetItemAsync<tt>(name, obj, lifeSpanSeconds);
        }
    }
}
