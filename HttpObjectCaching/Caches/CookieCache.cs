using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
{
    public class CookieCache : DictionaryCache
    {
        public CookieCache()
            : base(BaseCacheArea.Permanent, CacheSystem.CookieId, 60*60*24*60)
        {
            Area =  CacheArea.Cookie;
            Name = "DefaultCookie";
        }
    }
}
