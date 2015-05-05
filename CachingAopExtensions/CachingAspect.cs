using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using PostSharp.Aspects;

namespace CachingAopExtensions
{
    [Serializable]
    public class CachingAspect : MethodInterceptionAspect
    {
        public CacheArea CacheArea { get; set; }
        public string BaseName { get; set; }
        public double LifeSpanSeconds { get; set; }


        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var name = BaseName + "_" + args.Method.Module.Name + "_" + args.Method.Name;
            for (int i = 0; i < args.Arguments.Count; i++)
            {
                name = name + "_" + args.Arguments[i];
            }
            var mthInfo = args.Method as MethodInfo;
            if (mthInfo != null)
            {
                var retType = mthInfo.ReflectedType;
                if (retType.IsGenericType && typeof (Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];
                }

                args.ReturnValue = Cache.GetItem(CacheArea, name, retType, ()=>
                {
                    args.Proceed();
                    return args.ReturnValue;
                });

            }

        }
    }
}
