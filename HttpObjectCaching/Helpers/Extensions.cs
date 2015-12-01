using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using HttpObjectCaching.Core.Collections.List;

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


        public static IAsyncList<Tt> AsAsyncList<Tt>(this List<Tt> baselst)
        {
            return new AsyncList<Tt>(baselst);
        }
        public static IAsyncList<Tt> AsAsyncList<Tt>(this IEnumerable<Tt> baselst)
        {
            return new AsyncList<Tt>(baselst.ToList());
        }
        public static IAsyncList<Tt> AsAsyncList<Tt>(this IQueryable<Tt> baselst)
        {
            return new AsyncList<Tt>(baselst.ToList());
        }
    }
}
