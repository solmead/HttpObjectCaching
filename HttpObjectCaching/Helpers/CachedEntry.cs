using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.Helpers
{
    [Serializable]
    public class CachedEntry
    {
        public string Name { get; set; }
        public object Item { get; set; } 
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}
