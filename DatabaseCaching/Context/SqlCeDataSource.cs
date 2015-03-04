using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;
using SqlCeDatabaseCaching.Context;
using SqlCeDatabaseCaching.Models;
using SqlCeDatabaseCaching.Properties;
using Serializer = SqlCeDatabaseCaching.Helpers.Serializer;

namespace HttpObjectCaching.Core.DataSources
{
    public class SqlCeDataSource : IDataSource
    {

        private static object _lock = new object();

        public SqlCeDataSource()
        {
            DataContext.UpgradeDB(); 
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
            return await Cache.GetItemAsync<CachedEntry<tt>>(CacheArea.Global, "SqlCeDbCache_Item_" + name,async () => await GetItemFromDbAsync<CachedEntry<tt>>(name), Settings.Default.SecondsInMemory);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            await SetItemToDbAsync(item.Name, item, (item.TimeOut.HasValue ? (int?) item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds : null));
            await Cache.SetItemAsync<CachedEntry<tt>>(CacheArea.Global, "SqlCeDbCache_Item_" + item.Name, item, Settings.Default.SecondsInMemory);
        }

        public async Task DeleteItemAsync(string name)
        {
            //lock (_lock)
            //{
                using (var database = new DataContext())
                {
                    var lst = await (from ce in database.CachedEntries where ce.Name==name select ce).ToListAsync();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                    }
                    await database.SaveChangesAsync();
                }
            //}
        }

        public async Task DeleteAllAsync()
        {
            //lock (_lock)
            //{
                using (var database = new DataContext())
                {
                    var lst = await (from ce in database.CachedEntries select ce).ToListAsync();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                    }
                    await database.SaveChangesAsync();
                }
            //}
        }


        private async Task CleanOutTimeOutValuesAsync()
        {
            using (var database = new DataContext())
            {
                var lst =await 
                    (from ce in database.CachedEntries
                     where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                     select ce).ToListAsync();
                if (lst.Count > 0)
                {
                    database.CachedEntries.RemoveRange(lst);
                }

                await database.SaveChangesAsync();
            }
        }

        private async Task SetItemToDbAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            //lock (_lock)
            {
                using (var database = new DataContext())
                {
                    object nObj = default(tt);
                    object tObj = obj;
                    var itm =
                        await (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefaultAsync();

                    if (tObj == nObj)
                    {
                        if (itm != null)
                        {
                            database.CachedEntries.Remove(itm);
                        }
                    }
                    else
                    {
                        var xml = "";
                        DateTime? timeOut = null;
                        try
                        {
                            xml = Serializer.Serialize(obj);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                            throw;
                        }
                        if (lifeSpanSeconds.HasValue)
                        {
                            timeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                        }
                        if (itm == null)
                        {
                            itm = new CachedEntry()
                            {
                                Created = DateTime.Now,
                                Name = name,
                                Object = ""
                            };
                            database.CachedEntries.Add(itm);
                        }
                        itm.TimeOut = timeOut;
                        itm.Changed = DateTime.Now;
                        itm.Object = xml;
                    }


                    await database.SaveChangesAsync();
                }

                await CleanOutTimeOutValuesAsync();
            }
        }
        private async Task<tt> GetItemFromDbAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null)
        {

            CachedEntry itm;
            //lock (_lock)
            {
                using (var database = new DataContext())
                {
                    itm = await (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefaultAsync();
                }
            }
            if (itm != null && itm.TimeOut.HasValue && itm.TimeOut.Value >= DateTime.Now)
            {
                var xml = itm.Object;
                try
                {
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var o = Serializer.Deserialize<tt>(xml);
                        return o;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            if (createMethod != null)
            {
                var t = await createMethod();
                await SetItemToDbAsync(name, t, lifeSpanSeconds);
                return t;
            }
            return default(tt);
        } 
    }
}
