using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Web;

namespace HttpObjectCaching
{
    public enum CacheArea
    {
        Global,
        Session,
        Request,
        Thread
    }
    public static class Cache
    {

        public static tt GetItem<tt>(CacheArea area, string name)
        {
            if (area == CacheArea.Request)
            {
                return CacheSystem.Instance.GetFromRequest<tt>(name);
            }
            if (area == CacheArea.Session)
            {
                return CacheSystem.Instance.GetFromSession<tt>(name);
            }
            if (area == CacheArea.Global)
            {
                return CacheSystem.Instance.GetFromApplication<tt>(name);
            }
            //try
            //{
            //    var context = HttpContext.Current;
            //    if (context != null)
            //    {
            //        if (area == CacheArea.Request)
            //        {
            //            if (context.Items.Contains(name))
            //            {
            //                var ctx = (tt)context.Items[name];
            //                return ctx;
            //            }
            //        }
            //        else if (area == CacheArea.Session)
            //        {
            //            if (context.Session != null)
            //            {
            //                    var ctx = (tt) context.Session[name];
            //                    return ctx;
            //            } 
            //        }
            //        else if (area == CacheArea.Global)
            //        {
            //            if (context.Application != null)
            //            {
            //                var ctx = (tt)context.Application[name];
            //                return ctx;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //var app = System.AppDomain.CurrentDomain;
            //        //var app = Application.Current;

            //    }
                
            //}
            //catch (Exception)
            //{
            //    return default(tt);
            //}
            return default(tt);
        }
        public static void SetItem<tt>(CacheArea area, string name, tt obj)
        {

            if (area == CacheArea.Request)
            {
                CacheSystem.Instance.SetInRequest<tt>(name, obj);
            }
            if (area == CacheArea.Session)
            {
                CacheSystem.Instance.SetInSession<tt>(name, obj);
            }
            if (area == CacheArea.Global)
            {
                CacheSystem.Instance.SetInApplication<tt>(name, obj);
            }
            //var nullTT = default(tt);
            //var context = HttpContext.Current;
            //if (context != null)
            //{
            //    if (area == CacheArea.Request)
            //    {
            //        if (context.Items.Contains(name))
            //        {
            //            context.Items.Remove(name);
            //        }
            //        context.Items.Add(name, obj);
            //    } else if (area == CacheArea.Session)
            //    {
            //        if (context.Session != null)
            //        {
            //            var c = context.Session[name];
            //            if (c != null)
            //            {
            //                context.Session.Remove(name);
            //            }
            //            context.Session[name] = obj;
            //        }
            //    }
            //    else if (area == CacheArea.Global)
            //    {
            //        if (context.Application != null)
            //        {
            //            var ctx = context.Application[name];
            //            if (ctx != null)
            //            {
            //                context.Application.Remove(name);
            //            }
            //            context.Application[name] = obj;
            //        }
            //    }
            //}
    }

    }
}
