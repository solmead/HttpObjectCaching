using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCaching.Properties;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;
using Microsoft.ApplicationServer.Caching;

namespace AzureCaching.Models
{
    public class AzureDataSource : IDataSource
    {

        private Microsoft.ApplicationServer.Caching.DataCache _cache;

        public AzureDataSource()
        {
            _cache = new Microsoft.ApplicationServer.Caching.DataCache(Settings.Default.DataCacheName);
        }

        public CachedEntry<tt> GetItem<tt>(string name)
        {
            try
            {
                var t = (CachedEntry<tt>)_cache.Get(name.ToUpper());
                return t;
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
                if (item.TimeOut.HasValue)
                {
                    _cache.Put(item.Name.ToUpper(), item, item.TimeOut.Value.Subtract(DateTime.Now));
                }
                else
                {
                    _cache.Put(item.Name.ToUpper(), item); 
                }
            }
            else
            {
                DeleteItem(item.Name);
            }
        }

        public void DeleteItem(string name)
        {
            _cache.Remove(name.ToUpper());
        }

        public void DeleteAll()
        {
            _cache.Clear();
        }
    }
}
