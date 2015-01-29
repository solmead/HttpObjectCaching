using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureRedisCaching.Models;
using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
{
    public class AzureRedisCache : DataCache
    {

        public AzureRedisCache()
            : base(new AzureRedisDataSource())
        {
            
        }

        public override CacheArea Area { get { return CacheArea.Distributed; } }
        public override string Name { get { return "AzureRedisCache"; } }
    }
}
