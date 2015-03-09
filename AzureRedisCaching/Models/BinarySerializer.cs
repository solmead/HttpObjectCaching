using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AzureRedisCaching.Models
{
    public static class BinarySerializer
    {
        public static byte[] Serialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Flush();
                stream.Position = 0;
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length==0)
                return default(T);
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
