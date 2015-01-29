using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;
using HttpObjectCaching.Helpers;
using XmlCaching.Helpers;
using XmlCaching.Models;
using XmlCaching.Properties;
using FileMode = XmlCaching.Helpers.FileMode;

namespace HttpObjectCaching.Caches
{
    public class XmlCacheBase : DataCache
    {

        public XmlCacheBase() : base(new XmlDataSource())
        {
            
        }

        public override CacheArea Area { get { return CacheArea.Permanent; } }
        public override string Name { get { return "XmlCache"; } }
        
    }
}
