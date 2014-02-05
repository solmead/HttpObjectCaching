using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;
using XmlCaching.Helpers;
using XmlCaching.Models;
using XmlCaching.Properties;

namespace XmlCaching
{
    public class XmlCache : ICacheArea
    {


        public XmlCache()
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
        public string Name { get { return "XmlDefault"; } }
        public void ClearCache()
        {
            var list = BaseDirectory.GetFiles(Name + "_*.xml");
            foreach (var fi in list)
            {
                fi.Delete();
            }
        }

        private FileInfo getFile(string itemName)
        {
            return new FileInfo(BaseDirectory.FullName + "/" + Name + "_" + itemName + ".xml");
        }

        private void saveToFile(CachedEntry item, FileInfo file)
        {
            try
            {
                if (file.Exists)
                {
                    file.Delete();
                }
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }
                var xml = XmlSerializer.Serialize(item);
                var fw = new StreamWriter(file.OpenWrite());
                fw.Write(xml);
                fw.Flush();
                fw.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }
        private CachedEntry loadFromFile(FileInfo file)
        {
            try
            {
                if (file.Exists)
                {
                    var xml = "";
                    var fw = new StreamReader(file.OpenRead());
                    xml = fw.ReadToEnd();
                    fw.Close();
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var o = XmlSerializer.Deserialize<CachedEntry>(xml);
                        return o;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }

        private void DeleteEntry(string name)
        {
            var fi = getFile(name);
            if (fi.Exists)
            {
                fi.Delete();
            }
        }
        private CachedEntry GetEntry(string name)
        {
            var o = Cache.GetItem<CachedEntry>(CacheArea.Global,"xmlCache_" + name);
            if (o == null)
            {
                o = loadFromFile(getFile(name));
                if (o == null)
                {
                    o = new CachedEntry()
                    {
                        Created = DateTime.Now,
                        Name = name,
                        Changed = DateTime.Now
                    };
                }
                Cache.SetItem(CacheArea.Global, "xmlCache_" + name, o);
            }
            return o;
        }

        private void SetEntry(string name, CachedEntry item)
        {
            saveToFile(item, getFile(name));
            Cache.SetItem(CacheArea.Global, "xmlCache_" + name, item);
        }

        public DateTime? GetModifiedTime(string name)
        {
            return GetEntry(name).Changed;
        }
        public DateTime? GetCreatedTime(string name)
        {
            return GetEntry(name).Created;
        }
        public DateTime? GetTimeOut(string name)
        {
            return GetEntry(name).TimeOut;
        }

        public tt GetItem<tt>(string name, Func<tt> createMethod = null, double? lifeSpanSeconds = null)
        {
            var o = Cache.GetItem<tt>(CacheArea.Global, name) as object;
            if (o != null)
            {
                return (tt)o;
            }
            var xml = "";
            var itm = GetEntry(name);
            if (itm == null || string.IsNullOrWhiteSpace(itm.Object) || (itm.TimeOut.HasValue && itm.TimeOut.Value<DateTime.Now)) 
            {
                if (createMethod != null)
                {
                    var t = createMethod();
                    SetItem(name, t, lifeSpanSeconds);
                    return t;
                }
            }
            else
            {

                xml = itm.Object;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    o = XmlSerializer.Deserialize<tt>(xml);
                    Cache.SetItem<tt>(CacheArea.Global, name, (tt)o);
                    return (tt)o;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            
            //if (createMethod != null)
            //{
            //    var t = createMethod();
            //    SetItem(name, t, lifeSpanSeconds);
            //    return t;
            //}
            return default(tt);
        }

        public void SetItem<tt>(string name, tt obj, double? lifeSpanSeconds = null)
        {
            var itm = GetEntry(name);
            if (obj == null)
            {
                if (itm != null)
                {
                    DeleteEntry(name);
                }
            }
            else
            {
                if (itm == null)
                {
                    itm = new CachedEntry()
                    {
                        Created = DateTime.Now,
                        Name = name,
                        Changed = DateTime.Now
                    };
                }
                if (lifeSpanSeconds.HasValue)
                {
                    itm.TimeOut = DateTime.Now.AddSeconds(lifeSpanSeconds.Value);
                }
                itm.Changed = DateTime.Now;
                try
                {
                    itm.Object = XmlSerializer.Serialize(obj);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    throw;
                }
                SetEntry(name, itm);
            }
            Cache.SetItem<tt>(CacheArea.Global, name, obj);
            //var lst =
            //    (from ce in DataContext.Current.CachedEntries
            //        where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
            //        select ce).ToList();
            //if (lst.Count > 0)
            //{
            //    DataContext.Current.CachedEntries.RemoveRange(lst); 
            //}
            //DataContext.Current.SaveChanges();
        }
    }
}
