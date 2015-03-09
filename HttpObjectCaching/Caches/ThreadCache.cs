//using HttpObjectCaching.Core;
//using HttpObjectCaching.Core.DataSources;

//namespace HttpObjectCaching.Caches
//{
//    public class ThreadCache : DictionaryCache
//    {
//        public ThreadCache()
//            : base(new DataCache(new ThreadDataSource()),async () => "thread", 1)
//        {
//            Area = CacheArea.Thread;
//            Name = "DefaultThread";
//        }
//    }
//}
