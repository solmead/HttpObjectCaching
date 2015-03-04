using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
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
