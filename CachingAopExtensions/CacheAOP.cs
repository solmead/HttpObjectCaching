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


        public static void SetItem<tt>(Delegate function, tt obj, List<object> args)
        {
            SetItem(function, obj, 0, args);
        }

        public static void SetItem<tt>(Delegate function, tt obj, double lifeSpanSeconds, List<object> args)
        {
            var mi = function.Method;
            var ca = GetAttribute(mi);
            var param = mi.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                object v = null;
                if (args.Count < p.Position)
                {
                    v = args[p.Position];
                }
                dic.Add(p.Name, v);
            }

            Cache.SetItem<tt>(ca.CacheArea, ca.Namer.GetName(ca.BaseName, mi, dic), obj, lifeSpanSeconds);
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
