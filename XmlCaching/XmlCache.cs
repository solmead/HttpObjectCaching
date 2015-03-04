using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;
using XmlCaching;
using XmlCaching.Properties;

namespace HttpObjectCaching.Caches
{
    public class XmlCache : ICacheArea, INameValueLister
    {
        private ICacheArea baseCache = null;

        public XmlCache()
        {
            if (Settings.Default.UseOneFile)
            {
                baseCache = new XmlCacheCombined();
            }
            else
            {
                baseCache = new XmlCacheBase();
            }
        }

        public CacheArea Area { get { return baseCache.Area; } }
        public string Name { get { return "XmlDefault"; } }
        public void ClearCache()
        {
            baseCache.ClearCache();
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            return baseCache.GetItem<tt>(name, createMethod, lifeSpanSeconds);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            baseCache.SetItem<tt>(name, obj, lifeSpanSeconds);
        }

        public IDictionary<string, object> DataDictionary
        {
            get
            {
                var bdd = baseCache as INameValueLister;
                if (bdd != null)
                {
                    return bdd.DataDictionary;
                }
                return null;
            }
        }
    }
}
