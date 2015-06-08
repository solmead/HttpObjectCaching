using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;

namespace HttpObjectCaching.Caches
{
    //public class RequestCache : DictionaryCache
    //{
    //    public RequestCache()
    //        : base(new DataCache(new RequestDataSource()), () => "request")
    //    {
    //        Area =CacheArea.Request;
    //        Name = "DefaultRequest";
    //    }
    //}
    public class RequestCache : DataCache
    {
        public RequestCache()
            : base(new RequestDataSource())
        {
            Area = CacheArea.Request;
            Name = "DefaultRequest";
        }
    }
}
