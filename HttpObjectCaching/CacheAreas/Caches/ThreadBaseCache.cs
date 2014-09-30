using System;
using System.Diagnostics;
using System.Threading;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.CacheAreas.Caches
{
    public class ThreadBaseCache : ICacheArea
    {
        public CacheArea Area { get { return CacheArea.Thread; } }
        public string Name { get { return "BaseThread"; } }
        public void ClearCache()
        {
            throw new NotImplementedException();
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            object empty = default(tt);
            tt tObj = default(tt);
            var entry = Thread.GetData(Thread.GetNamedDataSlot(name.ToUpper())) as CachedEntry;
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry();
            }
            try
            {
                tObj = (tt)(entry.Item);
            }
            catch (Exception)
            {

            }
            object comp = tObj;
            if (comp == empty)
            {
                if (createMethod != null)
                {
                    tObj = createMethod();
                }
            }
            SetItem(name, tObj, lifeSpanSeconds);
            return tObj;
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var entry = new CachedEntry()
            {
                Name = name,
                Changed = DateTime.Now,
                Created = DateTime.Now,
                Item = obj
            };
            if (lifeSpanSeconds.HasValue)
            {
                entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
            }

            Thread.SetData(Thread.GetNamedDataSlot(name.ToUpper()), entry);
        }
    }
}
