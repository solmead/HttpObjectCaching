using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core;
using TestDistributedCache.Models;

namespace HttpObjectCaching.Caches
{
    public class TestCache : DataCache
    {

        public TestCache()
            : base(new TestDataSource())
        {
            
        }

        public override CacheArea Area { get { return CacheArea.Distributed; } }
        public override string Name { get { return "TestCache"; } }
    }
}
