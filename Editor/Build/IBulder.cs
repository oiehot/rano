using UnityEditor.Build.Reporting;

namespace Rano.Editor.Build
{
    public interface IBuilder
    {
        BuildReport Build();
    }
}