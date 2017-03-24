using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HttpObjectCaching.Helpers
{
    public static class BinarySerializer
    {
        

        public static T Deserialize<T>(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return default(T);
            byte[] b = Convert.FromBase64String(base64String);

            return Deserialize<T>(b);
        }

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
            if (data == null || data.Length == 0)
                return default(T);
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        public static byte[] Serialize(object item, Type type)
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

        public static object Deserialize(byte[] data, Type type)
        {
            if (data == null || data.Length == 0)
                return null;
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
}
