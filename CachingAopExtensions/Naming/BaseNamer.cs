using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using PostSharp.Aspects;

namespace CachingAopExtensions.Naming
{
    public class BaseNamer : ICacheEntryNamer
    {
        public string GetName(string baseName, MethodExecutionArgs args)
        {

            var param = args.Method.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                dic.Add(p.Name, args.Arguments[p.Position]);
            }

            var concatArguments = Serialize(dic);
            //var concatArguments = string.Join("_", serializedArguments);
            var name = baseName + "_" + args.Method.Module.Name + "_" + args.Method.Name + "_" + concatArguments;
            //var name = baseName + "_" + args.Method.Module.Name + "_" + GetCacheKey(args);
            

            return name;
        }

        //private string GetCacheKey(MethodExecutionArgs args)
        //{
        //    var serializedArguments = Serialize(args.Arguments);
        //    var concatArguments = string.Join("_", serializedArguments);
        //    concatArguments = args.Method.Name + "_" + concatArguments;
        //    return concatArguments;
        //}

        //private string[] Serialize(Arguments arguments)
        //{
        //    var json = new JavaScriptSerializer();
        //    return arguments.Select(json.Serialize).ToArray();
        //}

        private string Serialize(Dictionary<string, object> arguments)
        {
            var json = new JavaScriptSerializer();
            return json.Serialize(arguments);
            //return arguments.Select(json.Serialize).ToArray();
        }
    }
}
