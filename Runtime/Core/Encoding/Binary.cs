#nullable enable

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rano.Encoding
{
    public static class Binary
    {
        public static byte[]? ConvertObjectToBinary(object obj)
        {
            byte[] bytes;
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, obj);
                    bytes = memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
            
            return bytes;
        }

        public static object? ConvertBinaryToObject(byte[] bytes)
        {
            object obj;
            try
            {
                var binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    obj = binaryFormatter.Deserialize(memoryStream);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
            return obj;
        }
    }
}