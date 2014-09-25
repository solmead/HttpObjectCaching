using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class NoCache : ICacheArea
    {
        public CacheArea Area { get { return CacheArea.None; } }
        public string Name { get { return "DefaultNone"; } }
        public void ClearCache()
        {
            
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            if (createMethod != null)
            {
                return createMethod();
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            
        }
    }
}
