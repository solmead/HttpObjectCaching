using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core.DataSources
{
    public class ApplicationDataSource : IDataSource
    {
        public CachedEntry<tt> GetItem<tt>(string name)
        {
            try
            {
                var t = (CachedEntry<tt>)HttpRuntime.Cache[name.ToUpper()];
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
                    HttpRuntime.Cache.Remove(item.Name.ToUpper());
                }
                catch (Exception)
                {

                }
                if (item.TimeOut.HasValue)
                {
                    var lifeSpanSeconds = item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds;
                    int totSeconds = (int)lifeSpanSeconds;
                    int ms = (int)((lifeSpanSeconds - (1.0 * totSeconds)) * 1000.0);
                    HttpRuntime.Cache.Insert(item.Name.ToUpper(), item, null,
                        System.Web.Caching.Cache.NoAbsoluteExpiration,
                        new TimeSpan(0, 0, 0, totSeconds, ms),
                        CacheItemPriority.Default, null);
                }
                else
                {
                    HttpRuntime.Cache[item.Name.ToUpper()] = item;
                }


            }
            else
            {
                HttpRuntime.Cache.Remove(item.Name.ToUpper());
            }
        }

        public void DeleteItem(string name)
        {
            try
            {
                HttpRuntime.Cache.Remove(name.ToUpper());
            }
            catch (Exception)
            {
                
            }
        }

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }
    }
}
