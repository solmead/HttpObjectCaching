using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class RequestCache : DictionaryCache
    {
        public RequestCache()
            : base(new RequestBaseCache(), () => "request")
        {
            Area =CacheArea.Request;
            Name = "DefaultRequest";
        }
    }
}
