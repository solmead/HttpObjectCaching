﻿using System;
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

        public void SetItem<tt>(CachedEntry<tt> item)
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

        public CachedEntry<object> GetItem(string name, Type type)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                //lock (requestSetLock)
                {
                    if (context.Items.Contains(name.ToUpper()))
                    {
                        var t = (CachedEntry<object>)context.Items[name.ToUpper()];
                        return t;
                    }
                }
            }
            return default(CachedEntry<object>);
        }

        public void SetItem(Type type, CachedEntry<object> item)
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

        public void DeleteItem(string name)
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

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }
    }
}
