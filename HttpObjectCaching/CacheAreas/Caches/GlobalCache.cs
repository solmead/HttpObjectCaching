using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class GlobalCache : DictionaryCache
    {
        public GlobalCache()
            : base(new ApplicationBaseCache(), () => "global")
        {
            Area =CacheArea.Global;
            Name = "DefaultGlobal";
        }
    }
}
