using System;
using System.Threading.Tasks;
using AzureRedisCaching.Properties;
using HttpObjectCaching;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;
using StackExchange.Redis;

namespace AzureRedisCaching.Models
{
    public class AzureRedisDataSource : IDataSource
    {

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
    }
}
