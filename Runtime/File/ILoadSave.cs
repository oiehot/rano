namespace Rano.File
{
    public interface ILoadSave
    {
        void Load(string filePath);
        void Save(string filePath, byte[] bytes);
    }
}