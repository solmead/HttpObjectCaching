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
        Temp = 6,
        Request = 7,
        //Local = 8,
        //Thread = 7,
        None = 9
    }
    public enum BaseCacheArea
    {
        Other = 0,
        Permanent = 1,
        Distributed = 2,
        Global = 3,
        Temp = 6,
        Request = 7,
        //Local = 8,
        //Thread = 7,
        None = 9
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
        public static async Task<tt> GetItemAsync<tt>(ICacheArea area, string name, Func<Task<tt>> createMethod, double lifeSpanSeconds)
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
                if (area != null)
                {
                    name = area.Name + "_" + name;
                    //if (area.Area < CacheArea.Temp)
                    //{
                    //    return await GetItemAsync(CacheArea.Temp, name + "_Request_TempCache", async () =>
                    //    {
                    //        return await area.GetItemAsync<tt>(name, createMethod, lSS);
                    //    });
                    //}
                    //else
                    {
                        return await area.GetItemAsync<tt>(name, createMethod, lSS);
                    }
                }
                if (createMethod != null)
                {
                    return await createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message + " retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
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
        public static tt GetItem<tt>(ICacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds)
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
                        return  createMethod();
                    }
                    return default(tt);
                }

                if (area != null)
                {
                    name = area.Name + "_" + name;
                    //if (area.Area < CacheArea.Temp)
                    //{
                    //    return GetItem(CacheArea.Temp, name + "_Request_TempCache", () =>
                    //    {
                    //        return  area.GetItem<tt>(name, createMethod, lSS);
                    //    });
                    //}
                    //else
                    {
                        return  area.GetItem<tt>(name, createMethod, lSS);
                    }
                }
                if (createMethod != null)
                {
                    return  createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message + " retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
            }
            return default(tt);
        }

        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync<tt>(ICacheArea area, string name, tt obj, double lifeSpanSeconds)
        {
            double? lSS = lifeSpanSeconds;
            if (lSS == 0)
            {
                lSS = null;
            }
            name = area.Name + "_" + name;
            await area.SetItemAsync<tt>(name, obj, lSS);
            //if (area.Area < CacheArea.Temp)
            //{
            //    await SetItemAsync(CacheArea.Temp, name + "_Request_TempCache", obj);
            //}


        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem<tt>(ICacheArea area, string name, tt obj, double lifeSpanSeconds)
        {
            double? lSS = lifeSpanSeconds;
            if (lSS == 0)
            {
                lSS = null;
            }
            name = area.Name + "_" + name;
            area.SetItem<tt>(name, obj, lSS);
            //if (area.Area < CacheArea.Temp)
            //{
            //    SetItem(CacheArea.Temp, name + "_Request_TempCache", obj);
            //}
        }


        #region OverloadAreaSelect

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
            var ca = new DataCache(area);
            return await GetItemAsync<tt>(ca, name, createMethod, lifeSpanSeconds);
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
        public static tt GetItem<tt>(IDataSource area, string name, Func<tt> createMethod, double lifeSpanSeconds)
        {
            var ca = new DataCache(area);
            return GetItem<tt>(ca, name, createMethod, lifeSpanSeconds);
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
            var ca = CacheSystem.Instance.GetCacheArea(area);
            return await GetItemAsync(ca, name, createMethod, lifeSpanSeconds);
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
        public static tt GetItem<tt>(CacheArea area, string name, Func<tt> createMethod, double lifeSpanSeconds)
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            return GetItem(ca, name, createMethod, lifeSpanSeconds);
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
        public static object GetItemAsync(CacheArea area, string name, Type type, Func<Task<object>> createMethod, double lifeSpanSeconds)
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            try
            {

                double? lSS = lifeSpanSeconds;
                if (lSS == 0)
                {
                    lSS = null;
                }
                if (!name.Contains("CacheEnabled") && !CacheSystem.Instance.CacheEnabled)
                {
                    return createMethod?.Invoke();
                }
                if (ca != null)
                {
                    name = ca.Name + "_" + name;
                    return ca.GetItemAsync(name, createMethod, lSS);
                }
                if (createMethod != null)
                {
                    return createMethod();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error:" + ex.Message + " retrieving cache item [" + name + "] on cache [" + area.ToString() + "]", ex);
            }
            return null;
            //return await GetItemAsync(ca, name, createMethod, lifeSpanSeconds);
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
        public static object GetItem(CacheArea area, string name, Type type, Func<object> createMethod, double lifeSpanSeconds)
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            return GetItem(ca, name, createMethod, lifeSpanSeconds);
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
            await SetItemAsync<tt>(ca, name, obj, lifeSpanSeconds);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem<tt>(IDataSource area, string name, tt obj, double lifeSpanSeconds)
        {
            var ca = new DataCache(area);
            SetItem<tt>(ca, name, obj, lifeSpanSeconds);
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
            await SetItemAsync(ca, name, obj, lifeSpanSeconds);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static async Task SetItemAsync(CacheArea area, string name, Type type, object obj, double lifeSpanSeconds)
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            await SetItemAsync(ca, name, obj, lifeSpanSeconds);
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
            SetItem(ca, name, obj, lifeSpanSeconds);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        /// <param name="lifeSpanSeconds"></param>
        public static void SetItem(CacheArea area, string name, Type type, object obj, double lifeSpanSeconds)
        {
            var ca = CacheSystem.Instance.GetCacheArea(area);
            SetItem(ca, name, obj, lifeSpanSeconds);
        }


        #endregion

        #region OverloadedFunctions

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
        public static object GetItemAsync(CacheArea area, string name, Type type, Func<object> createMethod, double lifeSpanSeconds)
        {
            return GetItemAsync(area, name, type, async () => createMethod(), lifeSpanSeconds);
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
            return await GetItemAsync<tt>(area, name, async () => createMethod(), 0);
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
        public static object GetItemAsync(CacheArea area, string name, Type type, Func<object> createMethod)
        {
            return GetItemAsync(area, name, type, async () => createMethod(), 0);
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
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItemAsync(CacheArea area, string name, Type type, Func<Task<object>> createMethod)
        {
            return GetItemAsync(area, name, type, createMethod, 0);
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
        /// <returns></returns>
        public static object GetItemAsync(CacheArea area, string name, Type type)
        {
            return GetItemAsync(area, name, type, async () => null, 0);
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
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItemAsync(CacheArea area, string name, Type type, double lifeSpanSeconds)
        {
            return GetItemAsync(area, name, type, async () => null, lifeSpanSeconds);
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
        /// <returns></returns>
        public static object GetItemAsync(CacheArea area, string name, Type type, object defaultValue)
        {
            return GetItemAsync(area, name, type, async () => defaultValue, 0);
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
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static object GetItemAsync(CacheArea area, string name, Type type, object defaultValue, double lifeSpanSeconds)
        {
            return GetItemAsync(area, name, type, async () => defaultValue, lifeSpanSeconds);
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
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="createMethod">Function passed in that will return a new object of type tt if cache location and name doesnt exist yet.</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        public static tt GetItem<tt>(CacheArea area, string name, double lifeSpanSeconds)
        {
            return GetItem<tt>(area, name, () => default(tt), lifeSpanSeconds);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
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
        public static tt GetItem<tt>(CacheArea area, string name, tt defaultValue)
        {
            return  GetItem<tt>(area, name, () => defaultValue, 0);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static object GetItem(CacheArea area, string name, Type type, object defaultValue)
        {
            return  GetItem(area, name, type,  () => defaultValue, 0);
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
            return  GetItem<tt>(area, name,  () => defaultValue, lifeSpanSeconds);
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
        public static object GetItem(CacheArea area, string name, Type type, object defaultValue, double lifeSpanSeconds)
        {
            return  GetItem(area, name, type,  () => defaultValue, lifeSpanSeconds);
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
        public static tt GetItem<tt>(IDataSource area, string name, Func<tt> createMethod)
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
        public static tt GetItem<tt>(IDataSource area, string name)
        {
            return  GetItem<tt>(area, name,  () => default(tt), 0);
        }
        /// <summary>
        /// Pull an item from the cache, cache instance will be initialized to whatever the default for the type tt is.
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="lifeSpanSeconds"></param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, double lifeSpanSeconds)
        {
            return  GetItem<tt>(area, name,  () => default(tt), lifeSpanSeconds);
        }
        /// <summary>
        /// Pull an item from the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being requested</typeparam>
        /// <param name="area">What area object is stored in</param>
        /// <param name="name">Name of object</param>
        /// <param name="defaultValue">Value to initialize the cache location to if cache location and name doesnt exist yet.</param>
        /// <returns></returns>
        public static tt GetItem<tt>(IDataSource area, string name, tt defaultValue)
        {
            return  GetItem<tt>(area, name,  () => defaultValue, 0);
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
        public static tt GetItem<tt>(IDataSource area, string name, tt defaultValue, double lifeSpanSeconds)
        {
            return  GetItem<tt>(area, name,  () => defaultValue, lifeSpanSeconds);
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
            await SetItemAsync(area, name, obj, 0);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static async Task SetItemAsync(CacheArea area, string name, Type type, object obj)
        {
            await SetItemAsync(area, name, type, obj, 0);
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
            await SetItemAsync(area, name, obj, 0);
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

            SetItem(area, name, obj, 0);
        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static void SetItem(CacheArea area, string name, Type type, object obj)
        {

            SetItem(area, name, type, obj, 0);

        }
        /// <summary>
        /// Puts an item into the cache
        /// </summary>
        /// <typeparam name="tt">Type of object being stored</typeparam>
        /// <param name="area">What area to store object in</param>
        /// <param name="name">Name of object</param>
        /// <param name="obj">Object to store in cache location</param>
        public static void SetItem<tt>(IDataSource area, string name, tt obj)
        {
            SetItem<tt>(area, name, obj, 0);
        }

        #endregion
    }
}
