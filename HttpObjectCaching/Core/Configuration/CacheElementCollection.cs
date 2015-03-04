using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.Core.Configuration
{
    [ConfigurationCollection(typeof(CacheElement))]
    public class CacheElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CacheElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CacheElement)element).Area;
        }
    }
}
