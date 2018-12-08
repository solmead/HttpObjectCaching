using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core.Extras;
using HttpObjectCaching.Helpers;
using XmlCaching.Helpers;
using XmlCaching.Properties;

namespace XmlCaching.Models
{
    public class StoreInXML : IPermanentRepository
    {
        private string Name = "XmlCache";
        private object fileLock = new object();
        
        public StoreInXML()
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



        private void SetItemToFile(string name, byte[] obj, double? lifeSpanSeconds = null)
        {
            lock (fileLock)
            {
                var itm = FileHandling.LoadFromFile(BaseDirectory, Name, name);

                if (obj == null || obj?.Length == 0)
                {
                    if (itm != null)
                    {
                        FileHandling.DeleteFile(BaseDirectory, Name, name);
                    }
                }
                else
                {
                    var xml = Convert.ToBase64String(obj);
                    DateTime? timeOut = null;
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
        private byte[] GetItemFromFile(string name, double? lifeSpanSeconds = null)
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
                            var o = Convert.FromBase64String(xml);
                            return o;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
            return new byte[0];
        }






        public async Task<byte[]> GetAsync(string name)
        {
            return GetItemFromFile(name);
        }

        public async Task SetAsync(string name, byte[] value, TimeSpan? timeout)
        {
            SetItemToFile(name, value, timeout?.TotalSeconds);
        }

        public async Task DeleteAsync(string name)
        {
            SetItemToFile(name, null);
        }

        public async Task<List<string>> GetKeysAsync()
        {
            return GetKeys();
        }

        public byte[] Get(string name)
        {
            return GetItemFromFile(name);
        }

        public void Set(string name, byte[] value, TimeSpan? timeout)
        {
            SetItemToFile(name, value, timeout?.TotalSeconds);
        }

        public void Delete(string name)
        {
            SetItemToFile(name, null);
        }

        public List<string> GetKeys()
        {
            var list = FileHandling.GetFiles(BaseDirectory, Name);

            var lst = (from f in list select f.Name);


            var finList = new List<string>();
            foreach (var it in lst)
            {
                var t = it.Split('.', '-');
                finList.Add(t[1]);
            }
            return finList;
        }
    }
}
