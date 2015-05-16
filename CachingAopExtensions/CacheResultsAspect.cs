using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CachingAopExtensions.Naming;
using HttpObjectCaching;
using HttpObjectCaching.Core;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Extensibility;

namespace CachingAopExtensions
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, PersistMetaData = true)]
    public class CachingAspect : OnMethodBoundaryAspect
    {
        public CacheArea CacheArea { get; set; }
        public string BaseName { get; set; }
        public double LifeSpanSeconds { get; set; }
        private string Name { get; set; }
        private ICacheEntryNamer _namer;
        public ICacheEntryNamer Namer
        {
            get
            {
                if (_namer == null)
                {
                    if (!string.IsNullOrWhiteSpace(Name))
                    {
                        _namer = new StringNamer(Name);
                    }
                    if (_namer == null)
                    {
                        _namer = new BaseNamer();
                    }
                }
                return _namer;
            }
        }

        //public IDataSource DataSource { get; set; }

        public CachingAspect()
        {
            ApplyToStateMachine = false;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var name = Namer.GetName(BaseName, args);
            var mthInfo = args.Method as MethodInfo;
            if (mthInfo != null)
            {
                var retType = mthInfo.ReturnType;
                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];
                }

                object cachedValue = Cache.GetItem(CacheArea, name, retType,lifeSpanSeconds: LifeSpanSeconds);

                if (cachedValue != null)
                {
                    Debug.WriteLine("CachingAspect:" + CacheArea.ToString() + " [" + name + "] - Has Value [" +
                                    cachedValue.ToString() + "]");
                    retType = mthInfo.ReturnType;
                    if (retType.IsGenericType && typeof (Task).IsAssignableFrom(retType))
                    {
                        args.ReturnValue = FromResult((dynamic) cachedValue);
                    }
                    else
                    {
                        args.ReturnValue = cachedValue;
                    }


                    args.FlowBehavior = FlowBehavior.Return;
                }
                else
                {
                    Debug.WriteLine("CachingAspect:" + CacheArea.ToString() + " [" + name + "] - is null");
                }

            }





        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            var name = Namer.GetName(BaseName, args);
            var mthInfo = args.Method as MethodInfo;
            if (mthInfo != null)
            {
                var retType = mthInfo.ReturnType;
                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];
                    //var task = (Task)args.ReturnValue;
                    var task = args.ReturnValue;
                    args.ReturnValue = SetContinuation((dynamic)task, name, retType);
                    //task.ContinueWith(
                    //    t =>
                    //    {

                    //        Cache.SetItem(CacheArea, name, retType , t.Result, LifeSpanSeconds);
                    //        //MemoryCache.Default[args.Method.Name] = t.Result;
                    //        return t.Result;
                    //    });
                }
                else
                {

                    Debug.WriteLine("CachingAspect:" + CacheArea.ToString() + " [" + name + "] - Storing Value [" +
                                    args.ReturnValue.ToString() + "] for " + LifeSpanSeconds);
                    Cache.SetItem(CacheArea, name, retType, args.ReturnValue, LifeSpanSeconds);
                }


            }
        }

        Task<TResult> FromResult<TResult>(TResult data)
        {
            return Task.FromResult(data);
        }

        Task<TResult> SetContinuation<TResult>(Task<TResult> task, string name, Type retType)
        {
            var syncContext = TaskScheduler.FromCurrentSynchronizationContext();
            return task.ContinueWith(
                t =>
                {
                    Debug.WriteLine("CachingAspect:" + CacheArea.ToString() + " [" + name + "] - Storing Value [" +
                                    t.Result.ToString() + "] for " + LifeSpanSeconds);
                    Cache.SetItem(CacheArea, name, retType, t.Result, LifeSpanSeconds);
                    return t.Result;
                }, syncContext);
        }
    }
}
