using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core.DataSources
{
    public class ThreadDataSource : IDataSource
    {
        public CachedEntry<tt> GetItem<tt>(string name)
        {
            object empty = default(tt);
            tt tObj = default(tt);
            var entry = Thread.GetData(Thread.GetNamedDataSlot(name.ToUpper())) as CachedEntry<tt>;
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = null;
                DeleteItem(name);
            }
            if (entry != null)
            {
                try
                {
                    tObj = (tt) (entry.Item);
                }
                catch (Exception)
                {
                    entry = null;
                    DeleteItem(name);
                }
            }
            return entry;
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {

            Thread.SetData(Thread.GetNamedDataSlot(item.Name.ToUpper()), item);
        }

        public void DeleteItem(string name)
        {
            Thread.SetData(Thread.GetNamedDataSlot(name.ToUpper()), null);
        }

        public void DeleteAll()
        {
            //throw new NotImplementedException();
        }
    }
}
