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
        CachedEntry<tt> GetItem<tt>(string name);
        void SetItem<tt>(CachedEntry<tt> item);
        CachedEntry<object> GetItem(string name, Type type);
        void SetItem(Type type, CachedEntry<object> item);
        void DeleteItem(string name);
        void DeleteAll();

        Task<CachedEntry<tt>> GetItemAsync<tt>(string name);
        Task SetItemAsync<tt>(CachedEntry<tt> item);
        Task<CachedEntry<object>> GetItemAsync(string name, Type type);
        Task SetItemAsync(Type type, CachedEntry<object> item);
        Task DeleteItemAsync(string name);
        Task DeleteAllAsync();
    }
}
