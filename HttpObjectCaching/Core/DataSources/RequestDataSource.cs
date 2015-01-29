using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core.DataSources
{
    public class RequestDataSource : IDataSource
    {
        private object requestSetLock = new object();

        public CachedEntry<tt> GetItem<tt>(string name)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        var t = (CachedEntry<tt>)context.Items[name.ToUpper()];
                        return t;
                    }
                }
            }
            else
            {
                return (new ThreadDataSource()).GetItem<tt>(name);
            }
            return default(CachedEntry<tt>);
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                lock (requestSetLock)
                {
                    if (context.Items.Contains(item.Name.ToUpper()))
                    {
                        context.Items.Remove(item.Name.ToUpper());
                    }
                    context.Items.Add(item.Name.ToUpper(), item);
                }
            }
            else
            {
                (new ThreadDataSource()).SetItem<tt>(item);
            }
        }

        public void DeleteItem(string name)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        context.Items.Remove(name.ToUpper());
                    }
                }
            }
            else
            {
                (new ThreadDataSource()).DeleteItem(name);
            }
        }

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }
    }
}
