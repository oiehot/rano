namespace RanoEditor.Build
{
    public interface IPreprocessAddressableBuild
    {
        int callbackOrder { get; }
        void OnPreprocessAddressableBuild();
    }
}