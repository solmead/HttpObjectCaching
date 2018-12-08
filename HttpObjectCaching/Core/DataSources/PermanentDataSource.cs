using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core.Extras;
using HttpObjectCaching.Helpers;
using HttpObjectCaching.Properties;

namespace HttpObjectCaching.Core.DataSources
{
    public class PermanentDataSource : IDataSource
    {

        public BaseCacheArea Area => BaseCacheArea.Permanent;

        public IPermanentRepository CacheDatabase { get; set; }

        public Action NeedCacheDatabase { get; set; }

        public PermanentDataSource()
        {
            
        }
        public PermanentDataSource(IPermanentRepository baseData)
        {
            CacheDatabase = baseData;
        }


        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to read from");
                }
            }
            try
            {

                var t = await CacheDatabase.GetAsync(name.ToUpper());
                return BinarySerializer.Deserialize<CachedEntry<tt>>(t);
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
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item);
                if (item.TimeOut.HasValue)
                {
                    await CacheDatabase.SetAsync(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    await CacheDatabase.SetAsync(item.Name.ToUpper(), s, null);
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task<CachedEntry<object>> GetItemAsync(string name, Type type)
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to read from");
                }
            }
            try
            {
                var t = await CacheDatabase.GetAsync(name.ToUpper());
                return BinarySerializer.Deserialize(t, type) as CachedEntry<object>;
            }
            catch
            {
                //throw;
            }
            return null;
        }

        public async Task SetItemAsync(Type type, CachedEntry<object> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to read from");
                }
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item, type);
                if (item.TimeOut.HasValue)
                {
                    await CacheDatabase.SetAsync(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                }
                else
                {
                    await CacheDatabase.SetAsync(item.Name.ToUpper(), s, null);
                }
            }
            else
            {
                await DeleteItemAsync(item.Name);
            }
        }

        public async Task DeleteItemAsync(string name)
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            await CacheDatabase.DeleteAsync(name.ToUpper());
        }

        public async Task DeleteAllAsync()
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            var keys = await CacheDatabase.GetKeysAsync();
            foreach (var key in keys)
            {
                Console.WriteLine("Removing Key {0} from cache", key.ToString());
                await CacheDatabase.DeleteAsync(key);
            }
        }





        public CachedEntry<tt> GetItem<tt>(string name)
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to read from");
                }
            }
            try
            {
                var t = CacheDatabase.Get(name.ToUpper());
                return BinarySerializer.Deserialize<CachedEntry<tt>>(t);
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
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            object comp = item.Item;
            object empty = default(tt);
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item);
                if (item.TimeOut.HasValue)
                {
                    CacheDatabase.Set(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                }
                else
                {
                    CacheDatabase.Set(item.Name.ToUpper(), s, null);
                }
            }
            else
            {
                DeleteItem(item.Name);
            }
        }

        public CachedEntry<object> GetItem(string name, Type type)
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to read from");
                }
            }
            try
            {
                var t = CacheDatabase.Get(name.ToUpper());
                return BinarySerializer.Deserialize(t, type) as CachedEntry<object>;
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
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            object comp = item.Item;
            object empty = null;
            if (comp != empty)
            {
                var s = BinarySerializer.Serialize(item, type);
                if (item.TimeOut.HasValue)
                {
                    CacheDatabase.Set(item.Name.ToUpper(), s, item.TimeOut.Value.Subtract(DateTime.Now));
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s,
                    //    item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds);
                }
                else
                {
                    CacheDatabase.Set(item.Name.ToUpper(), s, null);
                    //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + item.Name, s);
                }
            }
            else
            {
                DeleteItem(item.Name);
            }
        }

        public void DeleteItem(string name)
        {
            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            CacheDatabase.Delete(name.ToUpper());
            //Cache.SetItem<string>(CacheArea.Global, "TestDistributedCache_" + name, null);
        }

        public void DeleteAll()
        {

            if (CacheDatabase == null)
            {
                if (NeedCacheDatabase != null)
                {
                    NeedCacheDatabase();
                }
                else
                {
                    throw new Exception("No Cache Database to write to");
                }
            }
            var keys =  CacheDatabase.GetKeys();
            foreach (var key in keys)
            {
                Console.WriteLine("Removing Key {0} from cache", key.ToString());
                CacheDatabase.Delete(key);
            }
        }

        //private string GetStringOfItem<tt>(tt item)
        //{
        //    return JsonConvert.SerializeObject(item);
        //}

        //private tt GetItemOfString<tt>(string val)
        //{
        //    return JsonConvert.DeserializeObject<tt>(val);
        //}

        //private bool IsInList<tt>(tt item)
        //{
        //    string v = GetStringOfItem<tt>(item);
        //    CacheDatabase.L
        //}

        private void WriteLine(string msg)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString() + " : " + DateTime.Now.Millisecond + " - " + msg);
        }

        public List<tt> GetList<tt>(string name)
        {
            //WriteLine("AzureRedis GetList:" + name);

            //var lst = CacheDatabase.ListRange(name).ToList();
            //return (from i in lst select GetItemOfString<tt>(i.ToString())).ToList();
            throw new NotImplementedException();
        }

        public void AddToList<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis AddToList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListRightPush(name, v);
            throw new NotImplementedException();
        }

        public void ClearList<tt>(string name)
        {
            //WriteLine("AzureRedis ClearList:" + name);
            //CacheDatabase.ListTrim(name, 0, 0);
            //CacheDatabase.ListLeftPop(name);
            throw new NotImplementedException();
        }

        public void RemoveFromList<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis RemoveFromList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListRemove(name, v);
            throw new NotImplementedException();
        }

        public void RemoveFromListAt<tt>(string name, int index)
        {
            //WriteLine("AzureRedis RemoveFromListAt:" + name + " [" + index + "]");
            //var v = CacheDatabase.ListGetByIndex(name, index);
            //CacheDatabase.ListRemove(name, v);
            throw new NotImplementedException();
        }

        public void InsertIntoList<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis Insert into List:" + name + " - " + index + " [" + item.ToString() + "]");
            //var vPivot = CacheDatabase.ListGetByIndex(name, index);
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListInsertAfter(name, vPivot, v);
            throw new NotImplementedException();
        }

        public void SetInList<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis SetInList:" + name + " - " + index + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //CacheDatabase.ListSetByIndex(name, index, v);
            throw new NotImplementedException();
        }

        public async Task<List<tt>> GetListAsync<tt>(string name)
        {
            //WriteLine("AzureRedis GetList:" + name);

            //var lst = (await CacheDatabase.ListRangeAsync(name)).ToList();
            //return (from i in lst select GetItemOfString<tt>(i.ToString())).ToList();
            throw new NotImplementedException();
        }

        public async Task AddToListAsync<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis AddToList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListRightPushAsync(name, v);
            throw new NotImplementedException();
        }

        public async Task ClearListAsync<tt>(string name)
        {
            //WriteLine("AzureRedis ClearList:" + name);
            //await CacheDatabase.ListTrimAsync(name, 0, 0);
            //await CacheDatabase.ListLeftPopAsync(name);
            throw new NotImplementedException();
        }

        public async Task RemoveFromListAsync<tt>(string name, tt item)
        {
            //WriteLine("AzureRedis RemoveFromList:" + name + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListRemoveAsync(name, v);
            throw new NotImplementedException();
        }

        public async Task RemoveFromListAtAsync<tt>(string name, int index)
        {
            //WriteLine("AzureRedis RemoveFromListAt:" + name + " [" + index + "]");
            //var v = await CacheDatabase.ListGetByIndexAsync(name, index);
            //await CacheDatabase.ListRemoveAsync(name, v);
            throw new NotImplementedException();
        }

        public async Task InsertIntoListAsync<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis Insert into List:" + name + " - " + index + " [" + item.ToString() + "]");
            //var vPivot = await CacheDatabase.ListGetByIndexAsync(name, index);
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListInsertAfterAsync(name, vPivot, v);
            throw new NotImplementedException();
        }

        public async Task SetInListAsync<tt>(string name, int index, tt item)
        {
            //WriteLine("AzureRedis SetInList:" + name + " - " + index + " [" + item.ToString() + "]");
            //string v = GetStringOfItem<tt>(item);
            //await CacheDatabase.ListSetByIndexAsync(name, index, v);
            throw new NotImplementedException();
        }
    }
}
