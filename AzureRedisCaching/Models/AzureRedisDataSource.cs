using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureRedisCaching.Properties;
using HttpObjectCaching;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AzureRedisCaching.Models
{
    public class AzureRedisDataSource : IDataSource
    {

        public BaseCacheArea Area { get { return BaseCacheArea.Distributed; } }
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(Settings.Default.HostName + ",abortConnect=false,ssl=" + !Settings.Default.AllowNonSSL + ",password=" + Settings.Default.CacheKey);
        });

        private static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        private static IDatabase CacheDatabase { get { return Connection.GetDatabase(); } }

        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            try
            {
                
                var t = await CacheDatabase.StringGetAsync(name.ToUpper());
                //var t = Cache.GetItem<string>(CacheArea.Global,"TestDistributedCache_" + name, (string) null);
                if (!string.IsNullOrWhiteSpace(t))
                {
                    return BinarySerializer.Deserialize<CachedEntry<tt>>(t);
                }
            }
            catch
            {
                //throw;
            }
            return default(CachedEntry<tt>);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item);
                if (item.TimeOut.HasValue)
                {
                    await CacheDatabase.StringSetAsync(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    await CacheDatabase.StringSetAsync(item.Name.ToUpper(), s,
                        new TimeSpan(0, Settings.Default.DefaultTimeoutMinutes, 0));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task<CachedEntry<object>> GetItemAsync(string name, Type type)
        {
            try
            {
                var t = await CacheDatabase.StringGetAsync(name.ToUpper());
                //var t = Cache.GetItem<string>(CacheArea.Global,"TestDistributedCache_" + name, (string) null);
                if (!string.IsNullOrWhiteSpace(t))
                {
                    return BinarySerializer.Deserialize(t, type) as CachedEntry<object>;
                }
            }
            catch
            {
                //throw;
            }
            return null;
        }

        public async Task SetItemAsync(Type type, CachedEntry<object> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item, type);
                if (item.TimeOut.HasValue)
                {
                    await CacheDatabase.StringSetAsync(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    await CacheDatabase.StringSetAsync(item.Name.ToUpper(), s,
                        new TimeSpan(0, Settings.Default.DefaultTimeoutMinutes, 0));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task DeleteItemAsync(string name)
        {
            await CacheDatabase.KeyDeleteAsync(name.ToUpper());
            //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + name, null);
        }

        public async Task DeleteAllAsync()
        {
            foreach (var ep in Connection.GetEndPoints())
            {
                var server =  Connection.GetServer(ep);
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    Console.WriteLine("Removing Key {0} from cache", key.ToString());
                    await CacheDatabase.KeyDeleteAsync(key);
                }
            }
        }




        
        public CachedEntry<tt> GetItem<tt>(string name)
        {
            try
            {
                var t =  CacheDatabase.StringGet(name.ToUpper());
                //var t = Cache.GetItem<string>(CacheArea.Global,"TestDistributedCache_" + name, (string) null);
                if (!string.IsNullOrWhiteSpace(t))
                {
                    return BinarySerializer.Deserialize<CachedEntry<tt>>(t);
                }
            }
            catch
            {
                //throw;
            }
            return default(CachedEntry<tt>);
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item);
                if (item.TimeOut.HasValue)
                {
                     CacheDatabase.StringSet(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                     CacheDatabase.StringSet(item.Name.ToUpper(), s,
                        new TimeSpan(0, Settings.Default.DefaultTimeoutMinutes, 0));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                 DeleteItem(item.Name);
            }
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            try
            {
                var t =  CacheDatabase.StringGet(name.ToUpper());
                //var t = Cache.GetItem<string>(CacheArea.Global,"TestDistributedCache_" + name, (string) null);
                if (!string.IsNullOrWhiteSpace(t))
                {
                    return BinarySerializer.Deserialize(t, type) as CachedEntry<object>;
                }
            }
            catch
            {
                //throw;
            }
            return null;
        }

        public void SetItem(Type type, CachedEntry<object> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item, type);
                if (item.TimeOut.HasValue)
                {
                     CacheDatabase.StringSet(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                     CacheDatabase.StringSet(item.Name.ToUpper(), s,
                        new TimeSpan(0, Settings.Default.DefaultTimeoutMinutes, 0));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                 DeleteItem(item.Name);
            }
        }

        public void DeleteItem(string name)
        {
             CacheDatabase.KeyDelete(name.ToUpper());
            //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + name, null);
        }

        public void DeleteAll()
        {
            foreach (var ep in Connection.GetEndPoints())
            {
                var server =  Connection.GetServer(ep);
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    Console.WriteLine("Removing Key {0} from cache", key.ToString());
                     CacheDatabase.KeyDelete(key);
                }
            }
        }

        private string GetStringOfItem<tt>(tt item)
        {
            return JsonConvert.SerializeObject(item);
        }
        private tt GetItemOfString<tt>(string val)
        {
            return JsonConvert.DeserializeObject<tt>(val);
        }

        public List<tt> GetList<tt>(string name)
        {
            var lst = CacheDatabase.ListRange(name).ToList();
            return (from i in lst select GetItemOfString<tt>(i.ToString())).ToList();
        }

        public void AddToList<tt>(string name, tt item)
        {
            string v = GetStringOfItem<tt>(item);
            CacheDatabase.ListRightPush(name, v);
        }

        public void ClearList<tt>(string name)
        {
            CacheDatabase.ListTrim(name,0,0);
            CacheDatabase.ListLeftPop(name);
        }

        public void RemoveFromList<tt>(string name, tt item)
        {
            string v = GetStringOfItem<tt>(item);
            CacheDatabase.ListRemove(name, v);
        }

        public void RemoveFromListAt<tt>(string name, int index)
        {
            var v = CacheDatabase.ListGetByIndex(name, index);
            CacheDatabase.ListRemove(name, v);
        }

        public void InsertIntoList<tt>(string name, int index, tt item)
        {
            var vPivot = CacheDatabase.ListGetByIndex(name, index);
            string v = GetStringOfItem<tt>(item);
            CacheDatabase.ListInsertAfter(name, vPivot, v);
        }

        public void SetInList<tt>(string name, int index, tt item)
        {
            string v = GetStringOfItem<tt>(item);
            CacheDatabase.ListSetByIndex(name, index, v);
        }

        public void CopyToList<tt>(string name, tt[] array, int arrayIndex)
        {
            var lst = (from a in array select GetStringOfItem<tt>(a)).ToList();
            lst.Reverse();
            var vPivot = CacheDatabase.ListGetByIndex(name, arrayIndex);
            lst.ForEach((v) => CacheDatabase.ListInsertAfter(name, vPivot, v));
        }
    }
}
