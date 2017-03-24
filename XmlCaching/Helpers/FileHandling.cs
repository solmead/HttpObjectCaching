using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Helpers;
using XmlCaching.Models;
using XmlCaching.Properties;

namespace XmlCaching.Helpers
{
    internal static class FileHandling
    {
        static object fileLock = new object();

        public static List<FileInfo> GetFiles(DirectoryInfo baseDirectory, string baseName)
        {
            var di = new DirectoryInfo(baseDirectory.FullName + "/");
            return di.GetFiles(baseName + "-*." + Settings.Default.WriteMode.ToString()).ToList();
        }

        public static FileInfo GetFile(DirectoryInfo baseDirectory, string baseName, string itemName)
        {
            var fi = new FileInfo(baseDirectory.FullName + "/" + baseName + "-" + itemName + "." + Settings.Default.WriteMode.ToString());
            if (fi.Exists && fi.Length > Settings.Default.MaxFileSizeMB * 1024 * 1024)
            {
                fi.Delete();
            }
            fi.Refresh();
            return fi;
        }

        public static void DeleteFiles(DirectoryInfo baseDirectory, string baseName)
        {
            var list = baseDirectory.GetFiles(baseName + "_*.xml");
            foreach (var fi in list)
            {
                fi.Delete();
            }
        }
        public static void DeleteFile(DirectoryInfo baseDirectory, string baseName, string itemName)
        {
            
            var file = GetFile(baseDirectory, baseName, itemName);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        public static void SaveToFile(CachedEntry item, DirectoryInfo baseDirectory, string baseName, string itemName)
        {
            var file = GetFile(baseDirectory, baseName, itemName);
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
                    var xml = Serializer.Serialize(item);
                    File.WriteAllText(file.FullName, xml);
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
        public static CachedEntry LoadFromFile(DirectoryInfo baseDirectory, string baseName, string itemName)
        {
            var file = GetFile(baseDirectory, baseName, itemName);
            lock (fileLock)
            {
                try
                {
                    if (file.Exists)
                    {
                        var xml = File.ReadAllText(file.FullName);
                        if (!string.IsNullOrWhiteSpace(xml))
                        {
                            var o = Serializer.Deserialize<CachedEntry>(xml);
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
    }
}
