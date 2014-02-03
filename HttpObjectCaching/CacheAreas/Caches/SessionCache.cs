using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class SessionCache : DictionaryCache
    {
        public SessionCache() : base(BaseCacheArea.Global, () => CacheSystem.Instance.SessionId)
        {
            Area =CacheArea.Session;
            Name = "DefaultSession";
        }
    }
}
