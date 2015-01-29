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
    }
}
