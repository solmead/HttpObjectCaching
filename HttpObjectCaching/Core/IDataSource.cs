using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core
{
    public interface IDataSource
    {
        //CachedEntry<tt> GetItem<tt>(string name);
        //void SetItem<tt>(CachedEntry<tt> item);
        //void DeleteItem(string name);
        //void DeleteAll();
        Task<CachedEntry<tt>> GetItemAsync<tt>(string name);
        Task SetItemAsync<tt>(CachedEntry<tt> item);
        Task DeleteItemAsync(string name);
        Task DeleteAllAsync();
    }
}
