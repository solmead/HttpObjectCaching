using System;
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

        public CachedEntry<tt> GetItem<tt>(string name)
        {
            try
            {
                var t = CacheDatabase.StringGet(name.ToUpper());
                //var t = Cache.GetItem<string>(CacheArea.Global,"TestDistributedCache_" + name, (string) null);
                if (!string.IsNullOrWhiteSpace(t))
                {
                    return Serializer.Deserialize<CachedEntry<tt>>(t);
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
                var s = Serializer.Serialize(item, false);
                if (item.TimeOut.HasValue)
                {
                    CacheDatabase.StringSet(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    CacheDatabase.StringSet(item.Name.ToUpper(), s);
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
            //foreach (var ep in Connection.GetEndPoints())
            //{
            //    var server = Connection.GetServer(ep);
            //    server.FlushAllDatabases();
            //}
        }
    }
}
