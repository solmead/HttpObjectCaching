using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class CookieCache : DictionaryCache
    {
        public CookieCache()
            : base(BaseCacheArea.Permanent, () => CacheSystem.Instance.CookieId)
        {
            Area =  CacheArea.Cookie;
            Name = "DefaultCookie";
        }
    }
}
