using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Helpers;
using XmlCaching.Properties;

namespace XmlCaching.Helpers
{
    public class Serializer
    {
        public static string Serialize<T>(T item) 
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
            return xml;
        }

        public static T Deserialize<T>(string xmlString)
        {
            object o = null;
            if (Settings.Default.WriteMode == FileMode.Xml)
            {
                o = XmlSerializer.Deserialize<T>(xmlString);
            }
            else
            {
                o = BinarySerializer.Deserialize<T>(xmlString);
            }
            return (T)o;
        }
    }
}
