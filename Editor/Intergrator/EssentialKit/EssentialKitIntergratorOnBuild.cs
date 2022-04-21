using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace RanoEditor.Intergrator.EssentialKit
{
    /// <summary>
    /// 빌드 전에 EssentialKitIntergrator.Intergrate()를 호출한다.
    /// </summary>
    public class EssentialKitIntergratorOnBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            EssentialKitIntergrator.Intergrate();
        }
    }
}