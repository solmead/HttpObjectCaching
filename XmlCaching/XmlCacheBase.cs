using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Helpers;
using XmlCaching.Helpers;
using XmlCaching.Models;
using XmlCaching.Properties;
using FileMode = XmlCaching.Helpers.FileMode;

namespace XmlCaching
{
    public class XmlCacheBase : ICacheArea
    {
        private object fileLock = new object(); 

        public XmlCacheBase()
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

        public CacheArea Area { get { return CacheArea.Permanent; } }
        public string Name { get { return "XmlCache"; } }
        public void ClearCache()
        {
            FileHandling.DeleteFiles(BaseDirectory, Name);
        }


        //private void DeleteEntry(string name)
        //{
        //    lock (fileLock)
        //    {
        //        var fi = getFile(name);
        //        if (fi.Exists)
        //        {
        //            fi.Delete();
        //        }
        //    }
        //}
        //private CachedEntry GetEntry(string name)
        //{
        //    var o = Cache.GetItem<CachedEntry>(CacheArea.Global,"xmlCacheBase_Entry_" + name);
        //    if (o == null)
        //    {
        //        o = loadFromFile<tt>(getFile(name));
        //        if (o == null)
        //        {
        //            o = new CachedEntry()
        //            {
        //                Created = DateTime.Now,
        //                Name = name,
        //                Changed = DateTime.Now
        //            };
        //        }
        //        Cache.SetItem(CacheArea.Global, "xmlCacheBase_Entry_" + name, o);
        //    }
        //    return o;
        //}

        //private void SetEntry(string name, CachedEntry item)
        //{
        //    saveToFile(item, getFile(name));
        //    Cache.SetItem(CacheArea.Global, "xmlCacheBase_Entry_" + name, item);
        //}

        //public DateTime? GetModifiedTime(string name)
        //{
        //    return GetEntry(name).Changed;
        //}
        //public DateTime? GetCreatedTime<tt>(string name)
        //{
        //    return GetEntry(name).Created;
        //}
        //public DateTime? GetTimeOut<tt>(string name)
        //{
        //    return GetEntry(name).TimeOut;
        //}

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
        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            SetItemToFile(name, obj, lifeSpanSeconds);
            Cache.SetItem<tt>(CacheArea.Global, "XmlCache_Item_" + name, obj, Settings.Default.SecondsInMemory);
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            return Cache.GetItem<tt>(CacheArea.Global, "XmlCache_Item_" + name, () => GetItemFromFile(name, createMethod, lifeSpanSeconds), Settings.Default.SecondsInMemory);
        }
        //public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        //{
        //    var o = Cache.GetItem<tt>(CacheArea.Global,"xmlCacheBase_Item_" + name) as object;
        //    if (o != null)
        //    {
        //        return (tt)o;
        //    }
        //    var xml = "";
        //    var itm = GetEntry(name);
        //    //
        //    object empty = default(tt);
        //    if (itm == null || itm.ItemObject == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
        //    {
        //        if (createMethod != null)
        //        {
        //            var t = createMethod();
        //            SetItem(name, t, lifeSpanSeconds);
        //            return t;
        //        }
        //    }
        //    else
        //    {
        //        return (tt)itm.Item;
        //    }
        //    return default(tt);
        //}

        //public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        //{
        //    var itm = GetEntry<tt>(name);
        //    if (obj == null)
        //    {
        //        if (itm != null)
        //        {
        //            DeleteEntry(name);
        //        }
        //    }
        //    else
        //    {
        //        if (itm == null)
        //        {
        //            itm = new CachedEntry<tt>()
        //            {
        //                Created = DateTime.Now,
        //                Name = name,
        //                Changed = DateTime.Now
        //            };
        //        }
        //        if (lifeSpanSeconds.HasValue)
        //        {
        //            itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
        //        }
        //        itm.Changed = DateTime.Now;
        //        itm.Item = obj;
        //        SetEntry(name, itm);
        //    }
        //    Cache.SetItem<tt>(CacheArea.Global, "xmlCacheBase_Item_" + name, obj);
        //}
    }
}
