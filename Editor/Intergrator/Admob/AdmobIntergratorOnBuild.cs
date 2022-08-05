using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Rano.Editor.Intergrator.Admob
{   
    /// <summary>
    /// 빌드 전에 AdmobIntergrator.Intergrate()를 호출한다.
    /// </summary>
    public class AdmobIntergratorOnBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            AdmobIntergrator.Intergrate();
        }
    }
}