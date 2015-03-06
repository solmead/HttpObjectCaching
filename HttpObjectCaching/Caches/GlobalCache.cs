using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;

namespace HttpObjectCaching.Caches
{
    public class GlobalCache : DictionaryCache
    {
        public GlobalCache()
            : base(new DataCache(new ApplicationDataSource()), async () => "global")
        {
            Area =CacheArea.Global;
            Name = "DefaultGlobal";
        }
    }
}
