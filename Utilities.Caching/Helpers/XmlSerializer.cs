using System.IO;
using System.Text;
using System.Xml;

namespace Utilities.Caching.Helpers
{
    public static class XmlSerializer
    {
        public static string Serialize<T>(T item)
        {
            var memStream = new MemoryStream();
            using (XmlTextWriter textWriter = new XmlTextWriter(memStream, Encoding.Unicode))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(textWriter, item);

                memStream = textWriter.BaseStream as MemoryStream;
            }
            if (memStream != null)
                return Encoding.Unicode.GetString(memStream.ToArray());
            else
                return null;
        }

        public static T Deserialize<T>(string xmlString)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
                return default(T);

            using (var memStream = new MemoryStream(Encoding.Unicode.GetBytes(xmlString)))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(memStream);
            }
        }
    }
}
