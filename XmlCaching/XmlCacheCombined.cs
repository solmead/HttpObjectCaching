using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;

namespace XmlCaching
{
    public class XmlCacheCombined : DictionaryCache
    {

        public XmlCacheCombined() : base(new XmlCacheBase(), () => "Global")
        {
            Area = CacheArea.Permanent;
            Name = "XmlCacheCombined";
        }
    }
}
