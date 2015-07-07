using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace CachingAopExtensions.Naming
{
    [Serializable]
    public class StringNamer : ICacheEntryNamer
    {
        public StringNamer(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public string GetName(string baseName, MethodInfo method, Dictionary<string, object> parameters)
        {
            return Name;
            
        }
        public string GetName(string baseName, MethodExecutionArgs args)
        {
            return Name;
        }
    }
}
