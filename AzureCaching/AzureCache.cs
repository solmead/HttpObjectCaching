using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureCaching.Models;
using HttpObjectCaching;
using HttpObjectCaching.Core;

namespace HttpObjectCaching.Caches
{
    public class AzureCache : DataCache
    {

        public AzureCache()
            : base(new AzureDataSource())
        {
            
        }

        public override CacheArea Area { get { return CacheArea.Distributed; } }
        public override string Name { get { return "AzureCache"; } }
    }
}
