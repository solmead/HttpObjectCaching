using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using DatabaseCaching.Context;
using DatabaseCaching.Helpers;
using DatabaseCaching.Models;
using DatabaseCaching.Properties;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;

namespace DatabaseCaching
{
    public class DatabaseCache : ICacheArea
    {
        private static object _lock = new object();

        //private static DataContext database
        //{
        //    get { return DataContext.Current; }
        //}

        public DatabaseCache()
        {
            DataContext.UpgradeDB();
        }

        public CacheArea Area { get { return CacheArea.Permanent; } }
        public string Name { get { return "DatabaseDefault"; } }
        public void ClearCache()
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

        public DateTime? GetModifiedTime(string name)
        {
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    return
                        (from ce in database.CachedEntries where ce.Name == name select (DateTime?) ce.Changed)
                            .FirstOrDefault();
                }
            }
        }
        public DateTime? GetCreatedTime(string name)
        {
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    return
                        (from ce in database.CachedEntries where ce.Name == name select (DateTime?) ce.Created)
                            .FirstOrDefault();
                }
            }
        }
        public DateTime? GetTimeOut(string name)
        {
            lock (_lock)
            {
                using (var database = new DataContext())
                {
                    return
                        (from ce in database.CachedEntries where ce.Name == name select ce.TimeOut)
                            .FirstOrDefault();
                }
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

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            return Cache.GetItem<tt>(CacheArea.Global, "DbCache_Item_" + name, () => GetItemFromDb(name, createMethod, lifeSpanSeconds), Settings.Default.SecondsInMemory);
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
        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            SetItemToDb(name, obj, lifeSpanSeconds);
            Cache.SetItem<tt>(CacheArea.Global, "DbCache_Item_" + name, obj, Settings.Default.SecondsInMemory);
        }
    }
}
