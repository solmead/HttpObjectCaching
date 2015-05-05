using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core
{
    public class DataCache : ICacheArea
    {
        private IDataSource _dataSource = null;

        public DataCache(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }


        public virtual CacheArea Area { get; private set; }
        public virtual string Name { get; private set; }
        
        public async Task ClearCacheAsync()
        {
            await _dataSource.DeleteAllAsync();
        }

        public async Task<tt> GetItemAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null)
        {
            object empty = default(tt);
            tt tObj = default(tt);
            var entry = await LoadItemAsync<tt>(name, lifeSpanSeconds);
            try
            {
                tObj = (tt)(entry.Item);
            }
            catch (Exception)
            {

            }
            object comp = tObj;
            if (comp == empty)
            {
                if (createMethod != null)
                {
                    tObj = await createMethod();
                    entry.Item = tObj;
                    await SaveItemAsync(entry);
                }
            }
            return tObj;
        }

        private async Task<CachedEntry<tt>> LoadItemAsync<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = await _dataSource.GetItemAsync<tt>(name);
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry<tt>()
                {
                    Name = name,
                    Changed = DateTime.Now,
                    Created = DateTime.Now
                };
                if (lifeSpanSeconds.HasValue)
                {
                    entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
            }
            return entry;
        }
        private async Task<CachedEntry<object>> LoadItemAsync(string name, Type type, double? lifeSpanSeconds = null)
        {
            var entry = await _dataSource.GetItemAsync(name, type);
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry<object>()
                {
                    Name = name,
                    Changed = DateTime.Now,
                    Created = DateTime.Now
                };
                if (lifeSpanSeconds.HasValue)
                {
                    entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
            }
            return entry;
        }
        private async Task SaveItemAsync<tt>(CachedEntry<tt> entry)
        {
            await _dataSource.SetItemAsync(entry);
        }
        public async Task SetItemAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var entry = await LoadItemAsync<tt>(name, lifeSpanSeconds);
            entry.Item = obj;
            entry.Changed = DateTime.Now;
            if (lifeSpanSeconds.HasValue)
            {
                entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }
            await SaveItemAsync(entry);
        }

        public async Task<object> GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null)
        {
            //object empty = default(tt);
            object tObj = null;
            CachedEntry<object> entry = await LoadItemAsync(name, type, lifeSpanSeconds);

            try
            {
                object itm = entry.Item;
                if (type.IsInstanceOfType(itm))
                {
                    tObj = itm;
                }
            }
            catch (Exception)
            {

            }
            object comp = tObj;
            if (comp == null)
            {
                if (createMethod != null)
                {
                    tObj = await createMethod();
                    entry.Item = tObj;
                    //await SaveItemAsync(entry);
                    await _dataSource.SetItemAsync(type, entry);
                }
            }
            return tObj;
        }

        public async Task SetItemAsync(string name, Type type, object obj, double? lifeSpanSeconds = null)
        {
            var entry = await LoadItemAsync(name, type, lifeSpanSeconds);
            entry.Item = obj;
            entry.Changed = DateTime.Now;
            if (lifeSpanSeconds.HasValue)
            {
                entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }
            await _dataSource.SetItemAsync(type, entry);
        }
    }
}
