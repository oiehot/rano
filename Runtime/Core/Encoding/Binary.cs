using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rano.Encoding
{
    public static class Binary
    {
        public static byte[] ConvertObjectToBinary(object obj)
        {
            byte[] bytes = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        public static object ConvertBinaryToObject(byte[] bytes)
        {
            var binaryFormatter = new BinaryFormatter();
            object obj;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                obj = binaryFormatter.Deserialize(memoryStream);
            }
            return obj;
        }
    }
}