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
            return AsyncHelper.RunSync(() => GetItemAsync<tt>(name));
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            AsyncHelper.RunSync(() => SetItemAsync<tt>(item));
        }

        public void DeleteItem(string name)
        {
            AsyncHelper.RunSync(() => DeleteItemAsync(name));
        }

        public void DeleteAll()
        {
            AsyncHelper.RunSync(DeleteAllAsync);
        }


        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
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
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task DeleteItemAsync(string name)
        {
            _cache.Remove(name.ToUpper());
        }

        public async Task DeleteAllAsync()
        {
            _cache.Clear();
        }
    }
}
