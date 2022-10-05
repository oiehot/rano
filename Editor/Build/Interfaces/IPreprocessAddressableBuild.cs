#nullable enable

namespace Rano.Editor.Build
{
    public interface IPreprocessAddressableBuild
    {
        int callbackOrder { get; }
        void OnPreprocessAddressableBuild();
    }
}