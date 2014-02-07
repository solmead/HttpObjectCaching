using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpObjectCaching.Helpers
{
    public static class Extensions
    {

        public static bool ContainsKey(this List<CachedEntry> dictionary, string name)
        {
            return (from ce in dictionary where ce.Name.ToUpper() == name select ce).Any();
        }
        public static CachedEntry getByName(this List<CachedEntry> dictionary, string name)
        {
            return (from ce in dictionary where ce.Name.ToUpper() == name select ce).FirstOrDefault();
        } 
    }
}
