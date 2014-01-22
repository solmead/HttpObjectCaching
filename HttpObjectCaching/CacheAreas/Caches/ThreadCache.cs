using System;
using System.Threading;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class ThreadCache : ICacheArea 
    {
        public CacheArea Area { get { return CacheArea.Thread; } }
        public string Name { get { return "DefaultThread"; } }
        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            try
            {
                var t = (tt)Thread.GetData(Thread.GetNamedDataSlot(name.ToUpper()));
                object comp = t;
                object empty = default(tt);
                if (comp == empty)
                {
                    if (createMethod != null)
                    {
                        t = createMethod();
                        SetItem(name, t, lifeSpanSeconds);
                    }
                }
                return t;
            }
            catch
            {
                throw;
            }
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            Thread.SetData(Thread.GetNamedDataSlot(name.ToUpper()), obj);
        }
    }
}
