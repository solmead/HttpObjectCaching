using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core.DataSources
{
    public class LocalDataSource : IDataSource
    {
        private ConcurrentDictionary<string, CachedEntryBase> _baseDictionary = new ConcurrentDictionary<string, CachedEntryBase>();

        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            return GetItem<tt>(name);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            SetItem(item);
        }

        public async Task<CachedEntry<object>> GetItemAsync(string name, Type type)
        {
            return GetItem(name, type);
        }

        public async Task SetItemAsync(Type type, CachedEntry<object> item)
        {
            SetItem(type, item);
        }

        public async Task DeleteItemAsync(string name)
        {
            DeleteItem(name);
        }

        public async Task DeleteAllAsync()
        {
            DeleteAll();
        }



        public CachedEntry<tt> GetItem<tt>(string name)
        {
            CachedEntryBase itm2;
            CachedEntry<tt> itm;
            _baseDictionary.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<tt>;
            return itm;
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            CachedEntryBase itm2;
            _baseDictionary.TryRemove(item.Name.ToUpper(), out itm2);
            _baseDictionary.TryAdd(item.Name.ToUpper(), item);
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            CachedEntryBase itm2;
            CachedEntry<object> itm;
            _baseDictionary.TryGetValue(name.ToUpper(), out itm2);
            itm = itm2 as CachedEntry<object>;
            return itm;
        }

        public void SetItem(Type type, CachedEntry<object> item)
        {
            CachedEntryBase itm2;
            _baseDictionary.TryRemove(item.Name.ToUpper(), out itm2);
            _baseDictionary.TryAdd(item.Name.ToUpper(), item);
        }

        public void DeleteItem(string name)
        {
            CachedEntryBase itm2;
            _baseDictionary.TryRemove(name.ToUpper(), out itm2);
        }

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }
    }
}
