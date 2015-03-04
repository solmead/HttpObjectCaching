using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;

namespace TestDistributedCache.Models
{
    public class TestDataSource : IDataSource
    {


        public CachedEntry<tt> GetItem<tt>(string name)
        {
            try
            {
                var t = Cache.GetItem<string>(CacheArea.Global,"TestDistributedCache_" + name, (string) null);
                return Serializer.Deserialize<CachedEntry<tt>>(t);
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
                var s = Serializer.Serialize(item,false);
                if (item.TimeOut.HasValue)
                {
                    Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                        item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                DeleteItem(item.Name);
            }
        }

        public void DeleteItem(string name)
        {
            Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + name, null);
        }

        public void DeleteAll()
        {
            
        }
    }
}
