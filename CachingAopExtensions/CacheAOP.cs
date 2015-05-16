using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CachingAopExtensions;
using HttpObjectCaching;
using PostSharp.Aspects;

namespace HttpObjectCaching
{
    public class CacheAOP
    {


        public static void SetItem<t1, t2, t3, t4, tResult>(Func<t1, t2, t3, t4, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj, 0);
        }
        public static void SetItem<t1, t2, t3, tResult>(Func<t1, t2, t3, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj, 0);
        }
        public static void SetItem<t1, t2, tResult>(Func<t1, t2, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj, 0);
        }
        public static void SetItem<t1, tResult>(Func<t1, tResult> method, List<object> args, tResult obj)
        {
            SetItem(method.Method, args, obj, 0);
        }
        public static void SetItem<tResult>(Func<tResult> method, tResult obj)
        {
            SetItem(method.Method, null, obj, 0);
        }
        public static void SetItem<t1, t2, t3, t4, tResult>(Func<t1, t2, t3, t4, tResult> method, List<object> args, tResult obj, double lifeSpanSeconds)
        {
            SetItem(method.Method, args, obj, lifeSpanSeconds);
        }
        public static void SetItem<t1, t2, t3, tResult>(Func<t1, t2, t3, tResult> method, List<object> args, tResult obj, double lifeSpanSeconds)
        {
            SetItem(method.Method, args, obj, lifeSpanSeconds);
        }
        public static void SetItem<t1, t2, tResult>(Func<t1, t2, tResult> method, List<object> args, tResult obj, double lifeSpanSeconds)
        {
            SetItem(method.Method, args, obj, lifeSpanSeconds);
        }
        public static void SetItem<t1, tResult>(Func<t1, tResult> method, List<object> args, tResult obj, double lifeSpanSeconds)
        {
            SetItem(method.Method, args, obj, lifeSpanSeconds);
        }
        public static void SetItem<tResult>(Func<tResult> method, tResult obj, double lifeSpanSeconds)
        {
            SetItem(method.Method, null, obj, lifeSpanSeconds);
        }
        public static void SetItem<tt>(MethodInfo method, List<object> args, tt obj)
        {
            SetItem(method, args, obj, 0);
        }

        public static void SetItem<tt>(MethodInfo method, List<object> args, tt obj, double lifeSpanSeconds)
        {
            var mi = method;
            var ca = GetAttribute(mi);
            var param = mi.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                object v = null;
                if (args!=null && args.Count < p.Position)
                {
                    v = args[p.Position];
                }
                else if (p.HasDefaultValue)
                {
                    v = p.DefaultValue;
                }
                dic.Add(p.Name, v);
            }

            Cache.SetItem<object>(ca.CacheArea, ca.Namer.GetName(ca.BaseName, mi, dic), obj, lifeSpanSeconds);
        }




        private static CachingAspect GetAttribute(MethodInfo method)
        {
            var pInfo = method
                             .GetCustomAttributes(typeof(CachingAspect), false)
                             .Cast<CachingAspect>().FirstOrDefault();

            return pInfo;
        }
    }
}
