﻿using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Caching.Helpers;

namespace Utilities.Caching.Core.DataSources
{
    public class ApplicationDataSource : IDataSource
    {

        private IMemoryCache _memoryCache;

        public BaseCacheArea Area { get {return BaseCacheArea.Global;} }

        public ApplicationDataSource()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

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
            try
            {
                var t = _memoryCache.Get<CachedEntry<tt>>(name.ToUpper());
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
                try
                {
                    _memoryCache.Remove(item.Name.ToUpper());
                    //HttpRuntime.Cache.Remove(item.Name.ToUpper());
                }
                catch (Exception)
                {

                }
                if (item.TimeOut.HasValue && item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds>0)
                {
                    var lifeSpanSeconds = item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds;
                    
                    int totSeconds = (int)lifeSpanSeconds;
                    int ms = (int)((lifeSpanSeconds - (1.0 * totSeconds)) * 1000.0);
                    try
                    {
                        _memoryCache.Set(item.Name.ToUpper(), item, new TimeSpan(0, 0, 0, totSeconds, ms));
                        //HttpRuntime.Cache.Insert(item.Name.ToUpper(), item, null,
                        //    System.Web.Caching.Cache.NoAbsoluteExpiration,
                        //    new TimeSpan(0, 0, 0, totSeconds, ms),
                        //    CacheItemPriority.Default, null);
                    }
                    catch (Exception ex)
                    {
                        
                        throw;
                    }
                }
                else
                {
                    _memoryCache.Set(item.Name.ToUpper(), item);
                    //HttpRuntime.Cache[item.Name.ToUpper()] = item;
                }


            }
            else
            {
                _memoryCache.Remove(item.Name.ToUpper());
                //HttpRuntime.Cache.Remove(item.Name.ToUpper());
            }
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            try
            {

                var t = _memoryCache.Get<CachedEntry<object>>(name.ToUpper());

                //var t = (CachedEntry<object>)HttpRuntime.Cache[name.ToUpper()];
                return t;
            }
            catch
            {
                //throw;
            }
            return null;
        }

        public void SetItem(Type type, CachedEntry<object> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                try
                {
                    _memoryCache.Remove(item.Name.ToUpper());
                    //HttpRuntime.Cache.Remove(item.Name.ToUpper());
                }
                catch (Exception)
                {

                }
                if (item.TimeOut.HasValue)
                {
                    var lifeSpanSeconds = item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds;
                    int totSeconds = (int)lifeSpanSeconds;
                    int ms = (int)((lifeSpanSeconds - (1.0 * totSeconds)) * 1000.0);

                    _memoryCache.Set(item.Name.ToUpper(), item, new TimeSpan(0, 0, 0, totSeconds, ms));
                    //HttpRuntime.Cache.Insert(item.Name.ToUpper(), item, null,
                    //    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    //    new TimeSpan(0, 0, 0, totSeconds, ms),
                    //    CacheItemPriority.Default, null);
                }
                else
                {
                    _memoryCache.Set(item.Name.ToUpper(), item);
                    //HttpRuntime.Cache[item.Name.ToUpper()] = item;
                }


            }
            else
            {
                _memoryCache.Remove(item.Name.ToUpper());
                //HttpRuntime.Cache.Remove(item.Name.ToUpper());
            }
        }

        public void DeleteItem(string name)
        {
            try
            {
                _memoryCache.Remove(name.ToUpper());
                //HttpRuntime.Cache.Remove(name.ToUpper());
            }
            catch (Exception)
            {

            }
        }

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }




        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry =  GetItem<tt>(name);
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
        public List<tt> GetList<tt>(string name)
        {
            var lstEntry = LoadItem<List<tt>>(name);
            if (lstEntry.Item == null)
            {
                lstEntry.Item = new List<tt>();
                SetItem(lstEntry);
            }
            return lstEntry.Item;
        }

        public void AddToList<tt>(string name, tt item)
        {
            GetList<tt>(name).Add(item);
        }

        public void ClearList<tt>(string name)
        {
            GetList<tt>(name).Clear();
        }

        public void RemoveFromList<tt>(string name, tt item)
        {
            GetList<tt>(name).Remove(item);
        }

        public void RemoveFromListAt<tt>(string name, int index)
        {
            GetList<tt>(name).RemoveAt(index);
        }

        public void InsertIntoList<tt>(string name, int index, tt item)
        {
            GetList<tt>(name).Insert(index, item);
        }

        public void SetInList<tt>(string name, int index, tt item)
        {
            GetList<tt>(name)[index] = item;
        }

        public async Task<List<tt>> GetListAsync<tt>(string name)
        {
            return GetList<tt>(name);
        }

        public async Task AddToListAsync<tt>(string name, tt item)
        {
            AddToList<tt>(name, item);
        }

        public async Task ClearListAsync<tt>(string name)
        {
            ClearList<tt>(name);
        }

        public async Task RemoveFromListAsync<tt>(string name, tt item)
        {
            RemoveFromList(name, item);
        }

        public async Task RemoveFromListAtAsync<tt>(string name, int index)
        {
            RemoveFromListAt<tt>(name, index);
        }

        public async Task InsertIntoListAsync<tt>(string name, int index, tt item)
        {
            InsertIntoList<tt>(name, index, item);
        }

        public async Task SetInListAsync<tt>(string name, int index, tt item)
        {
            SetInList(name, index, item);
        }

        //public void CopyToList<tt>(string name, tt[] array, int arrayIndex)
        //{
        //    GetList<tt>(name).CopyTo(array, arrayIndex);
        //}
    }
}
