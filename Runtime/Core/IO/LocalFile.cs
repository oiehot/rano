#nullable enable

using System;

namespace Rano.IO
{
    public static class LocalFile
    {
        public static bool WriteBytes(string filePath, byte[] bytes)
        {
            try
            {
                System.IO.File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            return true;
        }

        public static bool WriteString(string filePath, string str)
        {
            byte[] bytes;
            try
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(str);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
            return WriteBytes(filePath, bytes);
        }

        public static byte[]? ReadBytes(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false) return null;
            
            byte[] bytes;
            try
            {
                bytes = System.IO.File.ReadAllBytes(filePath);    
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
            return bytes;
        }

        public static string? ReadString(string filePath)
        {
            byte[]? bytes = ReadBytes(filePath);
            if (bytes == null) return null;

            string str;
            try
            {
                str = System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return null;
            }
            return str;
        }

        public static bool Move(string srcPath, string destPath, bool force)
        {
            bool exists;
            try
            {
                exists = System.IO.File.Exists(destPath);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }

            if (force == false && exists)
            {
                return false;
            }

            try
            {
                if (force && exists) System.IO.File.Delete(destPath);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }

            try
            {
                System.IO.File.Move(srcPath, destPath);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }

            return true;
        }
    }
}