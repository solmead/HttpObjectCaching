using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CachingAopExtensions;
using CachingAopExtensions.Helpers;
using HttpObjectCaching;
using PostSharp.Aspects;

namespace HttpObjectCaching
{
    public class CacheAOP
    {


        public static void SetItem<t1, t2, t3, t4, tResult>(Func<t1, t2, t3, t4, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj);
        }
        public static void SetItem<t1, t2, t3, tResult>(Func<t1, t2, t3, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj);
        }
        public static void SetItem<t1, t2, tResult>(Func<t1, t2, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj);
        }
        public static void SetItem<t1, tResult>(Func<t1, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj);
        }
        public static void SetItem<tResult>(Func<tResult> method, tResult obj)
        {
            SetItem(method.Method, null, obj);
        }
        

        public static void SetItem<tt>(MethodInfo method, List<object> args, tt obj)
        {
            var mi = method;
            var caList = GetAttributes(mi);
            var param = mi.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                object v = null;
                if (args!=null && args.Count > p.Position)
                {
                    v = args[p.Position];
                }
                else if (p.HasDefaultValue)
                {
                    v = p.DefaultValue;
                }
                dic.Add(p.Name, v);
            }
            caList.ForEach((ca) =>
            {
                Cache.SetItem<object>(ca.CacheArea, ca.Namer.GetName(ca.BaseName, mi, dic), obj, ca.LifeSpanSeconds);
            });
        }


        public static async Task SetItemAsync<t1, t2, t3, t4, tResult>(Func<t1, t2, t3, t4, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetItemAsync(method.Method, args, obj);
        }
        public static async Task SetItemAsync<t1, t2, t3, tResult>(Func<t1, t2, t3, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetItemAsync(method.Method, args, obj);
        }
        public static async Task SetItemAsync<t1, t2, tResult>(Func<t1, t2, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetItemAsync(method.Method, args, obj);
        }
        public static async Task SetItemAsync<t1, tResult>(Func<t1, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetItemAsync(method.Method, args, obj);
        }
        public static async Task SetItemAsync<tResult>(Func<Task<tResult>> method, tResult obj)
        {
            await SetItemAsync(method.Method, null, obj);
        }
       

        public static async Task SetItemAsync<tt>(MethodInfo method, List<object> args, tt obj)
        {
            var mi = method;
            var caList = GetAttributes(mi);
            var param = mi.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                object v = null;
                if (args != null && args.Count > p.Position)
                {
                    v = args[p.Position];
                }
                else if (p.HasDefaultValue)
                {
                    v = p.DefaultValue;
                }
                dic.Add(p.Name, v);
            }
            await caList.ForEachAsync(async(ca) =>
            {
                await Cache.SetItemAsync<object>(ca.CacheArea, ca.Namer.GetName(ca.BaseName, mi, dic), obj, ca.LifeSpanSeconds);
            });
            
        }



        private static List<CachingAspect> GetAttributes(MethodInfo method)
        {
            var pInfo = method
                             .GetCustomAttributes(typeof(CachingAspect), false)
                             .Cast<CachingAspect>().ToList();

            return pInfo;
        }
    }
}
