using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;

namespace HttpObjectCaching.Caches
{
    public class RequestCache : DictionaryCache
    {
        public RequestCache()
            : base(new DataCache(new RequestDataSource()), () => "request")
        {
            Area =CacheArea.Request;
            Name = "DefaultRequest";
        }
    }
}
