using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core.DataSources
{
    public class ThreadDataSource : IDataSource
    {
        public BaseCacheArea Area { get; }

        public CachedEntry<tt> GetItem<tt>(string name)
        {

            object empty = default(tt);
            tt tObj = default(tt);
            var entry = Thread.GetData(Thread.GetNamedDataSlot(name.ToUpper())) as CachedEntry<tt>;
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = null;
                DeleteItem(name);
            }
            if (entry != null)
            {
                try
                {
                    tObj = (tt)(entry.Item);
                }
                catch (Exception)
                {
                    entry = null;
                    DeleteItem(name);
                }
            }
            return entry;
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            Thread.SetData(Thread.GetNamedDataSlot(item.Name.ToUpper()), item);
        }

        public void DeleteItem(string name)
        {
            Thread.SetData(Thread.GetNamedDataSlot(name.ToUpper()), null);
        }

        public void DeleteAll()
        {
            
        }
        
        

        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            return GetItem<tt>(name);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            SetItem(item);
        }

        //public async Task<CachedEntry<object>> GetItemAsync(string name, Type type)
        //{
        //    return GetItem(name, type);
        //}

        //public async Task SetItemAsync(Type type, CachedEntry<object> item)
        //{
        //    SetItem(type, item);
        //}

        public async Task DeleteItemAsync(string name)
        {
            DeleteItem(name);
        }

        public async Task DeleteAllAsync()
        {
            DeleteAll();
        }

        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = GetItem<tt>(name);
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

        public void CopyToList<tt>(string name, tt[] array, int arrayIndex)
        {
            GetList<tt>(name).CopyTo(array, arrayIndex);
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
    }
}
