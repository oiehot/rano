#nullable enable

namespace Rano.Editor.Build
{
    public interface IPostprocessAddressableBuild
    {
        public int CallbackOrder { get; }
        public void OnPostprocessAddressableBuild();
    }
}