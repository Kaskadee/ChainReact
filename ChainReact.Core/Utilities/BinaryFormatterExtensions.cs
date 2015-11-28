using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChainReact.Core.Utilities
{
    public static class BinaryFormatterExtensions
    {
        public static string DeepSerialize<T>(this BinaryFormatter formatter, T obj)
        {
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static object DeepDeserialize(this BinaryFormatter formatter, string serialized)
        {
            var value = Convert.FromBase64String(serialized);
            using (var ms = new MemoryStream(value))
            {
                ms.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(ms);
            }
        }
    }
}
