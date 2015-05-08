using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Core;
using XmlCaching;
using XmlCaching.Properties;

namespace HttpObjectCaching.Caches
{
    public class XmlCache : DictionaryCache
    {

        public XmlCache()
            : base(new XmlCacheBase(), () => "Global")
        {
            Area = CacheArea.Permanent;
            Name = "XmlCacheCombined";
        }
    }
}
