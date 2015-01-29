using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core
{
    public interface IDataSource
    {
        CachedEntry<tt> GetItem<tt>(string name);
        void SetItem<tt>(CachedEntry<tt> item);
        void DeleteItem(string name);
        void DeleteAll();
    }
}
