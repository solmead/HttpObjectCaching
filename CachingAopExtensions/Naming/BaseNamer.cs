﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using PostSharp.Aspects;

namespace CachingAopExtensions.Naming
{
    [Serializable]
    public class BaseNamer : ICacheEntryNamer
    {
        public string GetName(string baseName, MethodInfo method, Dictionary<string, object> parameters)
        {

            var concatArguments = Serialize(parameters);
            //var concatArguments = string.Join("_", serializedArguments);
            var name = method.Name + "" + concatArguments;
            if (method.DeclaringType != null)
            {
                name = method.DeclaringType.FullName + "." + name;
            }
            name = baseName + "_" + name;
            return name;
        }
        public string GetName(string baseName, MethodExecutionArgs args)
        {

            var param = args.Method.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                dic.Add(p.Name, args.Arguments[p.Position]);
            }
            return GetName(baseName, args.Method as MethodInfo, dic);
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
