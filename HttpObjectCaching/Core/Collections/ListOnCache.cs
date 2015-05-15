using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.CacheAreas;
using Generic = System.Collections.Generic;

namespace HttpObjectCaching.Core.Collections
{
    public class ListOnCache<TT> : IList<TT>
    {
        private readonly IDataSource _dataSource = null;

        public string Name { get; private set; }
        //public double LifeSpanInSeconds { get; set; }
        public IDataSource DataSource { get { return _dataSource; } }

        public ListOnCache(string name, CacheArea cache)
        {
            Name = name;
            _dataSource = CacheSystem.Instance.GetCacheArea(cache).DataSource;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }
        public ListOnCache(string name, ICacheArea cache)
        {
            Name = name;
            _dataSource = cache.DataSource;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }
        public ListOnCache(string name, IDataSource cache)
        {
            Name = name;
            _dataSource = cache;
            //LifeSpanInSeconds = lifeSpanInSeconds;
        }

        private Generic.List<TT> BaseList
        {
            get { return _dataSource.GetList<TT>(Name); }
        }


        private Generic.List<TT> LocalList
        {
            get
            {
                return Cache.GetItem<Generic.List<TT>>(CacheArea.Request,
                    Name + "_localList",
                    ()=>BaseList);
            }
        }



        public IEnumerator<TT> GetEnumerator()
        {
            return LocalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TT item)
        {
            LocalList.Add(item);
            _dataSource.AddToList(Name, item);
        }

        public void Clear()
        {
            LocalList.Clear();
            _dataSource.ClearList<TT>(Name);
        }

        public bool Contains(TT item)
        {
            return LocalList.Contains(item);
        }

        public void CopyTo(TT[] array, int arrayIndex)
        {
            LocalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(TT item)
        {
            _dataSource.RemoveFromList(Name, item);
            return LocalList.Remove(item);
        }

        public int Count { get { return LocalList.Count; }  }
        public bool IsReadOnly { get { return false; }  }
        public int IndexOf(TT item)
        {
            return LocalList.IndexOf(item);
        }

        public void Insert(int index, TT item)
        {
            LocalList.Insert(index,item);
            _dataSource.InsertIntoList(Name, index, item);
        }

        public void RemoveAt(int index)
        {
            LocalList.RemoveAt(index);
            _dataSource.RemoveFromListAt<TT>(Name, index);
        }

        public TT this[int index]
        {
            get { return LocalList[index]; }
            set
            {
                LocalList[index] = value;
                _dataSource.SetInList(Name, index, value);
            }
        }
    }
}
