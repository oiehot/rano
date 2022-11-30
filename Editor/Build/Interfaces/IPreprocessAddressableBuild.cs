#nullable enable

namespace Rano.Editor.Build
{
    public interface IPreprocessAddressableBuild
    {
        public int CallbackOrder { get; }
        public void OnPreprocessAddressableBuild();
    }
}