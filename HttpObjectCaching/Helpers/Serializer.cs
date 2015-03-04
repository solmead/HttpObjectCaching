using System;
using System.Linq;

namespace HttpObjectCaching.Helpers
{
    public class Serializer
    {
        public static string Serialize<T>(T item, bool tryXml = true) 
        {
            var xml = "";
            if (tryXml)
            {
                try
                {
                    xml = XmlSerializer.Serialize(item);
                }
                catch (Exception)
                {
                    xml = "B" + BinarySerializer.Serialize(item);
                }
            }
            else
            {
                    xml = "B" + BinarySerializer.Serialize(item);
            }
            return xml;
            
        }

        public static T Deserialize<T>(string xmlString)
        {
            object o = null;
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                return default(T);
            }
            if (xmlString.First() != 'B')
            {
                o = XmlSerializer.Deserialize<T>(xmlString);
            }
            else
            {
                o = BinarySerializer.Deserialize<T>(xmlString.Substring(1));
            }
            return (T)o;
        }
    }
}
