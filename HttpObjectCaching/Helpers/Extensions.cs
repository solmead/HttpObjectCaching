using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;

namespace HttpObjectCaching.Helpers
{
    public static class Extensions
    {

        public static bool ContainsKey(this List<CachedEntryBase> dictionary, string name)
        {
            return (from ce in dictionary where ce.Name.ToUpper() == name select ce).Any();
        }
        public static CachedEntryBase getByName(this List<CachedEntryBase> dictionary, string name)
        {
            return (from ce in dictionary where ce.Name.ToUpper() == name select ce).FirstOrDefault();
        } 
    }
}
