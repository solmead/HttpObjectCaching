using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;

namespace HttpObjectCaching.Caches
{
    public class RequestCache : DictionaryCache
    {
        public RequestCache()
            : base(new DataCache(new RequestDataSource()), async () => "request")
        {
            Area =CacheArea.Request;
            Name = "DefaultRequest";
        }
    }
}
