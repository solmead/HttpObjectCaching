using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DatabaseCaching.Helpers
{
    public static class BinarySerializer
    {
        public static string Serialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static T Deserialize<T>(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return default(T);
            byte[] b = Convert.FromBase64String(base64String);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
