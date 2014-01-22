using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas
{
    public interface ICacheArea
    {
        CacheArea Area { get; }
        string Name { get; }
        tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null);
        void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null);
    }
}
