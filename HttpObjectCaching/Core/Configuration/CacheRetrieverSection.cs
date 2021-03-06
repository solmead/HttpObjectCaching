﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.Core.Configuration
{
    public class CacheRetrieverSection : ConfigurationSection
    {
        [ConfigurationProperty("entries", IsDefaultCollection = true)]
        public CacheElementCollection Entries
        {
            get { return (CacheElementCollection)this["entries"]; }
            set { this["entries"] = value; }
        }
        [ConfigurationProperty("permanent_source")]
        public PermanentElement PermanentElement
        {
            get { return (PermanentElement)this["permanent_source"]; }
            set { this["permanent_source"] = value; }
        }
    }
}
