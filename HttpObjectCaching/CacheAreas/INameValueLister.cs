using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.CacheAreas
{
    public interface INameValueLister
    {
        Dictionary<string, object> DataDictionary { get;  }
    }
}
