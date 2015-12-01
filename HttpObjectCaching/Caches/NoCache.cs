﻿using System;
using System.Threading.Tasks;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
{
    public class NoCache : ICacheArea
    {
        public CacheArea Area { get { return CacheArea.None; } }
        public string Name { get { return "DefaultNone"; } }
        public IDataSource DataSource { get; private set; }

        public void ClearCache()
        {
            
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            if (createMethod != null)
            {
                return createMethod();
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            
        }

        public object GetItem(string name, Type type, Func<object> createMethod = null, double? lifeSpanSeconds = null)
        {

            if (createMethod != null)
            {
                return createMethod();
            }
            return null;
        }

        public void SetItem(string name, Type type, object obj, double? lifeSpanSeconds = null)
        {
            
        }

        public async Task ClearCacheAsync()
        {
            
        }

        public async Task<tt> GetItemAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null)
        {
            if (createMethod != null)
            {
                return await createMethod();
            }
            return default(tt);
        }

        public async Task SetItemAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            
        }

        //public  object GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    if (createMethod != null)
        //    {
        //        return await createMethod();
        //    }
        //    return null;
        //}

        public async Task SetItemAsync(string name, Type type, object obj, double? lifeSpanSeconds = null)
        {
            
        }

        //async Task<object> ICacheArea.GetItemAsync(string name, Type type, Func<Task<object>> createMethod, double? lifeSpanSeconds)
        //{
        //    if (createMethod != null)
        //    {
        //        return await createMethod();
        //    }
        //    return null;
        //}
    }
}
