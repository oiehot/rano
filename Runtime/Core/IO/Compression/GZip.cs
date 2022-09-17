#nullable enable

using System;
using System.IO;
using System.IO.Compression;

namespace Rano.IO.Compression
{
    public static class GZip
    {
        public static byte[]? Compress(byte[] bytes)
        {
            byte[]? compressedBytes;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress))
                    {
                        gs.Write(bytes, 0, bytes.Length);
                    }
                    compressedBytes = ms.ToArray();
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
            return compressedBytes;
        }

        public static byte[]? Decompress(byte[] compressedBytes)
        {
            byte[]? bytes;
            try
            {
                using (MemoryStream byteStream = new MemoryStream())
                {
                    using (MemoryStream compressedByteStream = new MemoryStream(compressedBytes))
                    {
                        using (GZipStream decompressionStream =
                               new GZipStream(compressedByteStream, CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(byteStream);
                        }
                    }
                    bytes = byteStream.ToArray();
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
            return bytes;
        }
    }
}