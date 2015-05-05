using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;

namespace CachingAopExtensions
{
    [Serializable]
    public class CacheResultsAspect : OnMethodBoundaryAspect
    {
        public CacheArea CacheArea { get; set; }
        public string BaseName { get; set; }
        public double LifeSpanSeconds { get; set; }

        public CacheResultsAspect()
        {
            ApplyToStateMachine = false;
        }

        public override void OnEntry(MethodExecutionArgs args)
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
                if (retType.IsGenericType && typeof(Task).IsAssignableFrom(retType))
                {
                    retType = retType.GetGenericArguments()[0];
                }

                object cachedValue = CacheEx.GetItem(CacheArea, name, retType, null, LifeSpanSeconds);

                if (cachedValue != null)
                {
                    args.ReturnValue = Task.FromResult(cachedValue);
                    args.FlowBehavior = FlowBehavior.Return;
                }

            }




            
        }

        public override void OnSuccess(MethodExecutionArgs args)
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
                    var task = (Task<object>) args.ReturnValue;
                    args.ReturnValue =
                        task.ContinueWith(
                            t =>
                            {
                                CacheEx.SetItem(CacheArea, name, retType, t.Result, LifeSpanSeconds);
                                //MemoryCache.Default[args.Method.Name] = t.Result;
                                return t.Result;
                            });
                }
                else
                {

                    CacheEx.SetItem(CacheArea, name, retType, args.ReturnValue, LifeSpanSeconds); 
                }


            }
        }
    }
}
