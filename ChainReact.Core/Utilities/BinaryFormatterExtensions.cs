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
        public static byte[] DeepSerialize<T>(this BinaryFormatter formatter, T obj)
        {
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }

        public static object DeepDeserialize(this BinaryFormatter formatter, byte[] serialized)
        {
            using (var ms = new MemoryStream(serialized))
            {
                ms.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(ms);
            }
        }
    }
}
