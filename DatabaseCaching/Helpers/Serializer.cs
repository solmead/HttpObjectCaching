using SqlCeDatabaseCaching.Properties;

namespace SqlCeDatabaseCaching.Helpers
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
