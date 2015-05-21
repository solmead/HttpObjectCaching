using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching.Core
{
    public interface IDataSource
    {
        BaseCacheArea Area { get; }


        CachedEntry<tt> GetItem<tt>(string name);
        void SetItem<tt>(CachedEntry<tt> item);
        //CachedEntry<object> GetItem(string name, Type type);
        //void SetItem(Type type, CachedEntry<object> item);
        void DeleteItem(string name);
        void DeleteAll();

        Task<CachedEntry<tt>> GetItemAsync<tt>(string name);
        Task SetItemAsync<tt>(CachedEntry<tt> item);
        //Task<CachedEntry<object>> GetItemAsync(string name, Type type);
        //Task SetItemAsync(Type type, CachedEntry<object> item);
        Task DeleteItemAsync(string name);
        Task DeleteAllAsync();



        List<tt> GetList<tt>(string name);
        void AddToList<tt>(string name, tt item);
        void ClearList<tt>(string name);
        void RemoveFromList<tt>(string name, tt item);
        void RemoveFromListAt<tt>(string name, int index);
        void InsertIntoList<tt>(string name, int index, tt item);
        void SetInList<tt>(string name, int index, tt item);
        //void CopyToList<tt>(string name, tt[] array, int arrayIndex);


    }
}
