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
                }
            }
            await SaveItemAsync(entry);
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
    }
}
