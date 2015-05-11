//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using CachingAopExtensions.Naming;
//using HttpObjectCaching;
//using PostSharp.Aspects;

//namespace CachingAopExtensions
//{
//    [Serializable]
//    public class CachingAspect : MethodInterceptionAspect
//    {
//        public CacheArea CacheArea { get; set; }
//        public string BaseName { get; set; }
//        public double LifeSpanSeconds { get; set; }
//        public string Name { get; set; }
//        public ICacheEntryNamer Namer { get; set; }

//        //public CachingAspect(CacheArea cacheArea)
//        //{
//        //    CacheArea = cacheArea;
//        //}
//        //public CachingAspect(CacheArea cacheArea, double lifeSpanSeconds)
//        //{
//        //    CacheArea = cacheArea;
//        //    LifeSpanSeconds = lifeSpanSeconds;
//        //}

//        //public CachingAspect(CacheArea cacheArea, string name)
//        //{
//        //    CacheArea = cacheArea;
//        //    Name = name;
//        //}
//        //public CachingAspect(CacheArea cacheArea, string name, double lifeSpanSeconds)
//        //{
//        //    CacheArea = cacheArea;
//        //    Name = name;
//        //    LifeSpanSeconds = lifeSpanSeconds;
//        //}


//        //public CachingAspect(CacheArea cacheArea, ICacheEntryNamer namer)
//        //{
//        //    CacheArea = cacheArea;
//        //    Namer = namer;
//        //}
//        //public CachingAspect(CacheArea cacheArea, double lifeSpanSeconds, ICacheEntryNamer namer)
//        //{
//        //    CacheArea = cacheArea;
//        //    LifeSpanSeconds = lifeSpanSeconds;
//        //    Namer = namer;
//        //}


//        public override void OnInvoke(MethodInterceptionArgs args)
//        {
//            if (!string.IsNullOrWhiteSpace(Name))
//            {
//                Namer = new StringNamer(Name);
//            }
//            if (Namer == null)
//            {
//                Namer = new BaseNamer();
//            }
//            var name = Namer.GetName(BaseName, args);
//            var mthInfo = args.Method as MethodInfo;
//            if (mthInfo != null)
//            {
//                var retType = mthInfo.ReturnType;
//                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
//                {
//                    retType = retType.GetGenericArguments()[0];

//                    args.ReturnValue = Cache.GetItemAsync(CacheArea, name, retType, async () =>
//                    {
//                        args.Proceed();

//                        return (dynamic) await TaskTranslate((dynamic)args.ReturnValue);
//                    });

//                }
//                else
//                {

//                    args.ReturnValue = Cache.GetItem(CacheArea, name, retType, () =>
//                    {
//                        args.Proceed();
//                        return args.ReturnValue;
//                    });
//                }
//            }

//        }

//        Task<TResult> TaskTranslate<TResult>(Task<TResult> task)
//        {
//            return task;
//        }
//    }
//}
