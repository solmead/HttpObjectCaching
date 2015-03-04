using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
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
