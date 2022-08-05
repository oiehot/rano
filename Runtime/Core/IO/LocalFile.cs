namespace Rano.IO
{
    public static class LocalFile
    {
        public static void WriteBytes(string filePath, byte[] bytes)
        {
            System.IO.File.WriteAllBytes(filePath, bytes);
        }

        public static void WriteString(string filePath, string str)
        {
            WriteBytes(filePath, System.Text.Encoding.UTF8.GetBytes(str));
        }

        public static byte[] ReadBytes(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"파일을 찾지 못했습니다 ({filePath})");
            }
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            return bytes;
        }

        public static string ReadString(string filePath)
        {
            byte[] bytes = ReadBytes(filePath);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static void Move(string srcPath, string destPath, bool overwrite)
        {
            if(overwrite == false && System.IO.File.Exists(destPath))
            {
                return;
            }

            if(overwrite == true && System.IO.File.Exists(destPath))
            {
                System.IO.File.Delete(destPath);
            }
            System.IO.File.Move(srcPath, destPath);
            return;
        }
    }
}