using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core;
using HttpObjectCaching.Helpers;
using XmlCaching.Helpers;
using XmlCaching.Models;
using XmlCaching.Properties;

namespace HttpObjectCaching.Core.DataSources
{
    public class XmlDataSource : IDataSource
    {
        private object fileLock = new object();
        private string Name = "XmlCache";

        public BaseCacheArea Area { get; } = BaseCacheArea.Permanent;
        public XmlDataSource()
        {
            BaseDirectory = InternalBaseDirectory;
        }
        private DirectoryInfo InternalBaseDirectory
        {
            get
            {
                return new DirectoryInfo(System.Web.Hosting.HostingEnvironment.MapPath(Settings.Default.CacheFileLocation));
            }
        }
        public DirectoryInfo BaseDirectory { get; set; }

        public CachedEntry<tt> GetItem<tt>(string name)
        {
            return Cache.GetItem<CachedEntry<tt>>(CacheArea.Global, "XmlCache_Item_" + name, () => GetItemFromFile<CachedEntry<tt>>(name), Settings.Default.SecondsInMemory);
        }

        public void SetItem<tt>(CachedEntry<tt> item)
        {
            SetItemToFile(item.Name, item, (item.TimeOut.HasValue ? (int?)item.TimeOut.Value.Subtract(DateTime.Now).TotalSeconds : null));
            Cache.SetItem<CachedEntry<tt>>(CacheArea.Global, "XmlCache_Item_" + item.Name, item, Settings.Default.SecondsInMemory);
        }

        public void DeleteItem(string name)
        {
            
        }

        public void DeleteAll()
        {
            FileHandling.DeleteFiles(BaseDirectory, Name);
        }

        public async Task<CachedEntry<tt>> GetItemAsync<tt>(string name)
        {
            return GetItem<tt>(name);
        }

        public async Task SetItemAsync<tt>(CachedEntry<tt> item)
        {
            SetItem<tt>(item);
        }

        public async Task DeleteItemAsync(string name)
        {
            DeleteItem(name);
        }

        public async Task DeleteAllAsync()
        {
            DeleteAll();
        }


        private void SetItemToFile<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            lock (fileLock)
            {
                object nObj = default(tt);
                object tObj = obj;
                var itm = FileHandling.LoadFromFile(BaseDirectory, Name, name);

                if (tObj == nObj)
                {
                    if (itm != null)
                    {
                        FileHandling.DeleteFile(BaseDirectory, Name, name);
                    }
                }
                else
                {
                    var xml = "";
                    DateTime? timeOut = null;
                    try
                    {
                        xml = Serializer.Serialize(obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        throw;
                    }
                    if (lifeSpanSeconds.HasValue)
                    {
                        timeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                    }
                    if (itm == null)
                    {
                        itm = new CachedEntry()
                        {
                            Created = DateTime.Now,
                            Name = name,
                            Object = ""
                        };
                    }
                    itm.TimeOut = timeOut;
                    itm.Changed = DateTime.Now;
                    itm.Object = xml;
                    FileHandling.SaveToFile(itm, BaseDirectory, Name, name);
                }

            }
        }
        private tt GetItemFromFile<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {

            lock (fileLock)
            {
                CachedEntry itm = FileHandling.LoadFromFile(BaseDirectory, Name, name);
                if (itm != null && itm.TimeOut.HasValue && itm.TimeOut.Value >= DateTime.Now)
                {
                    var xml = itm.Object;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(xml))
                        {
                            var o = Serializer.Deserialize<tt>(xml);
                            return o;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
            if (createMethod != null)
            {
                var t = createMethod();
                SetItemToFile(name, t, lifeSpanSeconds);
                return t;
            }
            return default(tt);
        }





        private CachedEntry<tt> LoadItem<tt>(string name, double? lifeSpanSeconds = null)
        {
            var entry = GetItem<tt>(name);
            if (entry == null || (entry.TimeOut.HasValue && entry.TimeOut.Value < DateTime.Now))
            {
                entry = new CachedEntry<tt>()
                {
                    Name = name,
                    Changed = DateTime.Now,
                    Created = DateTime.Now
                };
                if (lifeSpanSeconds.HasValue)
                {
                    entry.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
            }
            return entry;
        }
        public List<tt> GetList<tt>(string name)
        {
            var lstEntry = LoadItem<List<tt>>(name);
            if (lstEntry.Item == null)
            {
                lstEntry.Item = new List<tt>();
                SetItem(lstEntry);
            }
            return lstEntry.Item;
        }

        public void AddToList<tt>(string name, tt item)
        {
            GetList<tt>(name).Add(item);
        }

        public void ClearList<tt>(string name)
        {
            GetList<tt>(name).Clear();
        }

        public void RemoveFromList<tt>(string name, tt item)
        {
            GetList<tt>(name).Remove(item);
        }

        public void RemoveFromListAt<tt>(string name, int index)
        {
            GetList<tt>(name).RemoveAt(index);
        }

        public void InsertIntoList<tt>(string name, int index, tt item)
        {
            GetList<tt>(name).Insert(index, item);
        }

        public void SetInList<tt>(string name, int index, tt item)
        {
            GetList<tt>(name)[index] = item;
        }

        public void CopyToList<tt>(string name, tt[] array, int arrayIndex)
        {
            GetList<tt>(name).CopyTo(array, arrayIndex);
        }
        public async Task<List<tt>> GetListAsync<tt>(string name)
        {
            return GetList<tt>(name);
        }

        public async Task AddToListAsync<tt>(string name, tt item)
        {
            AddToList<tt>(name, item);
        }

        public async Task ClearListAsync<tt>(string name)
        {
            ClearList<tt>(name);
        }

        public async Task RemoveFromListAsync<tt>(string name, tt item)
        {
            RemoveFromList(name, item);
        }

        public async Task RemoveFromListAtAsync<tt>(string name, int index)
        {
            RemoveFromListAt<tt>(name, index);
        }

        public async Task InsertIntoListAsync<tt>(string name, int index, tt item)
        {
            InsertIntoList<tt>(name, index, item);
        }

        public async Task SetInListAsync<tt>(string name, int index, tt item)
        {
            SetInList(name, index, item);
        }

    }
}
