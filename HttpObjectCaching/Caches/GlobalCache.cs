using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;

namespace HttpObjectCaching.Caches
{
    //public class GlobalCache : DictionaryCache
    //{
    //    public GlobalCache()
    //        : base(new DataCache(new ApplicationDataSource()), () => "global")
    //    {
    //        Area = CacheArea.Global;
    //        Name = "DefaultGlobal";
    //    }
    //}
    public class GlobalCache : DataCache
    {
        public GlobalCache()
            : base(new ApplicationDataSource())
        {
            Area = CacheArea.Global;
            Name = "DefaultGlobal";
        }
    }
}
