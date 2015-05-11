using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace CachingAopExtensions.Naming
{
   public  interface ICacheEntryNamer
   {
       string GetName(string baseName, MethodExecutionArgs args);
   }
}
