using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
{
    public class SessionCache : DictionaryCache
    {
        public SessionCache() : base(BaseCacheArea.Global, CacheSystem.SessionId)
        {
            Area =CacheArea.Session;
            Name = "DefaultSession";
            LifeSpanInSeconds = 60*20;
        }
    }
}
