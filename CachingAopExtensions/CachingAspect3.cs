using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CachingAopExtensions.Naming;
using HttpObjectCaching;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace CachingAopExtensions
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, PersistMetaData = true)]
    public class CachingAspect3 : MethodInterceptionAspect
    {
        public CacheArea CacheArea { get; set; }
        public string BaseName { get; set; }
        public double LifeSpanSeconds { get; set; }
        public string Name { get; set; }
        public ICacheEntryNamer Namer { get; set; }

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                Namer = new StringNamer(Name);
            }
            if (Namer == null)
            {
                Namer = new BaseNamer();
            }
            var name = Namer.GetName(BaseName, args);
            var mthInfo = args.Method as MethodInfo;
            if (mthInfo != null)
            {
                var retType = mthInfo.ReturnType;


                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];

                    //args.ReturnValue = Cache.GetItemAsync(CacheArea, name, retType, async () =>
                    //{
                    //    args.Proceed();

                    //    return (dynamic)await TaskTranslate((dynamic)args.ReturnValue);
                    //});
                    var ret = Cache.GetItemAsync(CacheArea, name, retType, async () =>
                    {
                        base.OnInvoke(args);
                        return (dynamic)await TaskTranslate((dynamic)args.ReturnValue);
                    });
                    args.ReturnValue = ret;
                }
                else
                {
                    args.ReturnValue = Cache.GetItem(CacheArea, name, retType, () =>
                    {
                        base.OnInvoke(args);
                        return args.ReturnValue;
                    });
                    //args.ReturnValue = Cache.GetItem(CacheArea, name, retType, () =>
                    //{
                    //    args.Proceed();
                    //    return args.ReturnValue;
                    //});
                }
            }

        }

        Task<TResult> TaskTranslate<TResult>(Task<TResult> task)
        {
            return task;
        }
    }
}
