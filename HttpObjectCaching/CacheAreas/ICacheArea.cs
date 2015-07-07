using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core;

namespace HttpObjectCaching.CacheAreas
{
    public interface ICacheArea
    {
        CacheArea Area { get; }
        string Name { get; }

        IDataSource DataSource { get; }


        void ClearCache();
        tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null);
        void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null);
        //object GetItem(string name, Type type, Func<object> createMethod = null, double? lifeSpanSeconds = null);
        //void SetItem(string name, Type type, object obj, double? lifeSpanSeconds = null);



        Task ClearCacheAsync();
        Task<tt> GetItemAsync<tt>(string name, Func<Task<tt>> createMethod = null, double? lifeSpanSeconds = null);
        Task SetItemAsync<tt>(string name, tt obj, double? lifeSpanSeconds = null);
        //Task<object> GetItemAsync(string name, Type type, Func<Task<object>> createMethod = null, double? lifeSpanSeconds = null);
        //Task SetItemAsync(string name, Type type, object obj, double? lifeSpanSeconds = null);
    }
}
