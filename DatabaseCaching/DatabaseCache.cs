using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DatabaseCaching.Context;
using DatabaseCaching.Helpers;
using DatabaseCaching.Models;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;

namespace DatabaseCaching
{
    public class DatabaseCache : ICacheArea
    {
        public DatabaseCache()
        {
            DataContext.UpgradeDB();
        }

        public CacheArea Area { get { return CacheArea.Permanent; } }
        public string Name { get { return "DatabaseDefault"; } }
        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            var o = Cache.GetItem<tt>(CacheArea.Global, name) as object;
            if (o != null)
            {
                return (tt)o;
            }

            var itm = (from ce in DataContext.Current.CachedEntries where ce.Name == name select ce).FirstOrDefault();
            string xml = "";
            if (itm == null || (itm.TimeOut.HasValue && itm.TimeOut.Value<DateTime.Now) )
            {
                if (createMethod != null)
                {
                    var t = createMethod();
                    SetItem(name, t, lifeSpanSeconds);
                    return t;
                }
            }
            else
            {
                
                xml = itm.Object;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    o= XmlSerializer.Deserialize<tt>(xml);
                    Cache.SetItem<tt>(CacheArea.Global, name, (tt) o);
                    return (tt) o;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            if (createMethod != null)
            {
                var t = createMethod();
                SetItem(name, t, lifeSpanSeconds);
                return t;
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var itm = (from ce in DataContext.Current.CachedEntries where ce.Name == name select ce).FirstOrDefault();
            if (itm == null)
            {
                itm = new CachedEntry()
                {
                    Created = DateTime.Now,
                    Name = name,
                    Changed = DateTime.Now,
                    Object=""
                };
                DataContext.Current.CachedEntries.Add(itm);
            }
            if (lifeSpanSeconds.HasValue)
            {
                itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }
            itm.Changed = DateTime.Now;
            try
            {
                itm.Object = XmlSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
            Cache.SetItem<tt>(CacheArea.Global, name, obj);
            var lst =
                (from ce in DataContext.Current.CachedEntries
                    where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                    select ce).ToList();
            if (lst.Count > 0)
            {
                DataContext.Current.CachedEntries.RemoveRange(lst); 
            }
            DataContext.Current.SaveChanges();
        }
    }
}
