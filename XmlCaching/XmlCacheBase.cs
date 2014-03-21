﻿using System;
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
            var list = BaseDirectory.GetFiles(Name + "_*.xml");
            foreach (var fi in list)
            {
                fi.Delete();
            }
        }

        private FileInfo getFile(string itemName)
        {
            var fi = new FileInfo(BaseDirectory.FullName + "/" + Name + "-" + itemName + "." + Settings.Default.WriteMode.ToString());
            if (fi.Exists && fi.Length > Settings.Default.MaxFileSizeMB * 1024 * 1024)
            {
                fi.Delete();
            }
            fi.Refresh();
            return fi;
        }

        private void saveToFile(CachedEntry item, FileInfo file)
        {
            var currentFile = new FileInfo(file.FullName);
            var newFile = new FileInfo(file.FullName + ".new");
            var backupFile = new FileInfo(file.FullName + ".backup");
            lock (fileLock)
            {
                if (newFile.Exists)
                {
                    newFile.Delete();
                }
                if (backupFile.Exists)
                {
                    backupFile.Delete();
                }
                if (!newFile.Directory.Exists)
                {
                    newFile.Directory.Create();
                }
                try
                {
                    var xml = "";
                    if (Settings.Default.WriteMode == FileMode.Xml)
                    {
                        xml = XmlSerializer.Serialize(item);
                    }
                    else
                    {
                        xml = BinarySerializer.Serialize(item);
                    }
                    var fw = new StreamWriter(newFile.OpenWrite());
                    fw.Write(xml);
                    fw.Flush();
                    fw.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    throw;
                }
                newFile.Refresh();
                file.Refresh();
                backupFile.Refresh();
                if (newFile.Exists)
                {
                    if (currentFile.Exists)
                    {
                        currentFile.MoveTo(backupFile.FullName);
                    }
                    newFile.MoveTo(file.FullName);
                    currentFile = new FileInfo(file.FullName);
                    newFile = new FileInfo(file.FullName + ".new");
                    backupFile = new FileInfo(file.FullName + ".backup");
                    if (currentFile.Exists && backupFile.Exists)
                    {
                        backupFile.Delete();
                    }
                }
            }
        }
        private CachedEntry loadFromFile(FileInfo file)
        {
            lock (fileLock)
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

                            CachedEntry o = null;
                            if (Settings.Default.WriteMode == FileMode.Xml)
                            {
                                o = XmlSerializer.Deserialize<CachedEntry>(xml);
                            }
                            else
                            {
                                o = BinarySerializer.Deserialize<CachedEntry>(xml);
                            }
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
        }

        private void DeleteEntry(string name)
        {
            lock (fileLock)
            {
                var fi = getFile(name);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
        }
        private CachedEntry GetEntry(string name)
        {
            var o = Cache.GetItem<CachedEntry>(CacheArea.Global,"xmlCacheBase_Entry_" + name);
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
                Cache.SetItem(CacheArea.Global, "xmlCacheBase_Entry_" + name, o);
            }
            return o;
        }

        private void SetEntry(string name, CachedEntry item)
        {
            saveToFile(item, getFile(name));
            Cache.SetItem(CacheArea.Global, "xmlCacheBase_Entry_" + name, item);
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
            var o = Cache.GetItem<tt>(CacheArea.Global,"xmlCacheBase_Item_" + name) as object;
            if (o != null)
            {
                return (tt)o;
            }
            var xml = "";
            var itm = GetEntry(name);
            //
            object empty = default(tt);
            if (itm == null || itm.Item == empty || (itm.TimeOut.HasValue && itm.TimeOut.Value < DateTime.Now))
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
                return (tt)itm.Item;
            }
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
                itm.Item = obj;
                SetEntry(name, itm);
            }
            Cache.SetItem<tt>(CacheArea.Global, "xmlCacheBase_Item_" + name, obj);
        }
    }
}