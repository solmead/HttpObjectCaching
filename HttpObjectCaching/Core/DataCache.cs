using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void ClearCache()
        {
            _dataSource.DeleteAll();
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            object empty = default(tt);
            tt tObj = default(tt);
            var entry = LoadItem<tt>(name, lifeSpanSeconds);
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
                    tObj = createMethod();
                    entry.Item = tObj;
                }
            }
            SaveItem(entry);
            return tObj;
        }

        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = _dataSource.GetItem<tt>(name);
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
        private void SaveItem<tt>(CachedEntry<tt> entry)
        {
            _dataSource.SetItem(entry);
        }
        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var entry = LoadItem<tt>(name, lifeSpanSeconds);
            entry.Item = obj;
            entry.Changed = DateTime.Now;
            if (lifeSpanSeconds.HasValue)
            {
                entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }
            SaveItem(entry);
        }
    }
}
