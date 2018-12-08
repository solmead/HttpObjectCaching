using Utilities.Caching.Core;
using Utilities.Caching.Core.DataSources;

namespace Utilities.Caching.Caches
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
