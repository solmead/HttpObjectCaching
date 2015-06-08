using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpObjectCaching.Core.Collections
{
    interface IListAsync<TT>
    {
        Task<IEnumerator<TT>> GetEnumeratorAsync();

        Task Add(TT item);

        Task Clear();

        Task<bool> Contains(TT item);

        Task CopyTo(TT[] array, int arrayIndex);

        Task<bool> Remove(TT item);

        Task<int> Count { get; }
        Task<bool> IsReadOnly { get; }
        Task<int> IndexOf(TT item);

        Task Insert(int index, TT item);

        Task RemoveAt(int index);

        Task<TT> this[int index] { get; set; }
    }
}
