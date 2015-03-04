using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core.DataSources
{
    public class RequestDataSource : IDataSource
    {
        //private object requestSetLock = new object();
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
            var context = HttpContext.Current;
            if (context != null)
            {
                //lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        var t = (CachedEntry<tt>)context.Items[name.ToUpper()];
                        return t;
                    }
                }
            }
            return default(CachedEntry<tt>);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                //lock (requestSetLock)
                {
                    if (context.Items.Contains(item.Name.ToUpper()))
                    {
                        context.Items.Remove(item.Name.ToUpper());
                    }
                    context.Items.Add(item.Name.ToUpper(), item);
                }
            }
        }

        public async Task DeleteItemAsync(string name)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                //lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        context.Items.Remove(name.ToUpper());
                    }
                }
            }
        }

        public async Task DeleteAllAsync()
        {
            //throw new NotImplementedException();
        }
    }
}
