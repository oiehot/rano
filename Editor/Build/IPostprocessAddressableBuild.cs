namespace RanoEditor.Build
{
    public interface IPostprocessAddressableBuild
    {
        int callbackOrder { get; }
        void OnPostprocessAddressableBuild();
    }
}