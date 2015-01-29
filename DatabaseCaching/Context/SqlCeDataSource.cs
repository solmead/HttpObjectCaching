using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            return Cache.GetItem<CachedEntry<tt>>(CacheArea.Global, "SqlCeDbCache_Item_" + name, () => GetItemFromDb<CachedEntry<tt>>(name), Settings.Default.SecondsInMemory);
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            SetItemToDb(item.Name, item, (item.TimeOut.HasValue ? (int?) item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds : null));
            Cache.SetItem<CachedEntry<tt>>(CacheArea.Global, "SqlCeDbCache_Item_" + item.Name, item, Settings.Default.SecondsInMemory);
        }

        public void DeleteItem(string name)
        {
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    var lst = (from ce in database.CachedEntries where ce.Name==name select ce).ToList();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                    }
                    database.SaveChanges();
                }
            }
        }

        public void DeleteAll()
        {
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    var lst = (from ce in database.CachedEntries select ce).ToList();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                    }
                    database.SaveChanges();
                }
            }
        }


        private void CleanOutTimeOutValues()
        {
            using (var database = new DataContext())
            {
                var lst =
                    (from ce in database.CachedEntries
                     where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                     select ce).ToList();
                if (lst.Count > 0)
                {
                    database.CachedEntries.RemoveRange(lst);
                }

                database.SaveChanges();
            }
        }

        private void SetItemToDb<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    object nObj = default(tt);
                    object tObj = obj;
                    var itm =
                        (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefault();

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


                    database.SaveChanges();
                }

                CleanOutTimeOutValues();
            }
        }
        private tt GetItemFromDb<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {

            CachedEntry itm;
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    itm = (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefault();
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
                var t = createMethod();
                SetItemToDb(name, t, lifeSpanSeconds);
                return t;
            }
            return default(tt);
        } 
    }
}
