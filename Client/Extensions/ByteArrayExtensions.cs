using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Netsphere.Client.Extensions
{
    public static class ByteArrayExtensions
    {
        public static T Deserialize<T>(this byte[] data)
        {
            if (data is null)
            {
                return default;
            }

            using (var ms = new MemoryStream(data))
            {
                var bf = new BinaryFormatter();
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        public static byte[] Serialize<T>(this T obj)
        {
            if (obj is null)
            {
                return new byte[0];
            }

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
