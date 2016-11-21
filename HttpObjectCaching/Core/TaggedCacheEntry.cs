using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpObjectCaching.Core
{
    public class TaggedCacheEntry
    {

        public CacheArea CacheArea { get; set; }
        public string Tags { get; set; }
        public string EntryName { get; set; }



    }
}
