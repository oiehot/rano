#nullable enable

namespace Rano.Editor.Build
{
    public interface IPostprocessAddressableBuild
    {
        int callbackOrder { get; }
        void OnPostprocessAddressableBuild();
    }
}