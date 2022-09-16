#if UNITY_EDITOR

using System.Collections;
using UnityEngine;
using Rano.App;

namespace Rano.Store
{
    /// <summary>
    /// 테스트용 앱스토어
    /// 
    /// 테스트모드를 변경하여 원하는 테스트를 실행할 수 있다.
    /// </summary>
    public class TestAppStore : IAppStore
    {
        public enum ETestMode
        {
            UpdateRequiredTest,
            AlreadyUpdatedTest,
            ParseErrorTest,
            ConnectionErrorTest
        }
        
        private ETestMode _testMode = ETestMode.UpdateRequiredTest;
        
        private string _bundleId;
        public string BundleId => _bundleId;
        
        public string PageUrl => $"https://play.google.com/store/apps/details?id={BundleId}";
        public string BrowserUrl => $"market://details?id={BundleId}";

        public TestAppStore(string bundleId)
        {
            _bundleId = bundleId;
        }
        
        public IEnumerator GetLatestVersionCoroutine(System.Action<EGetVersionResult, SVersion?> callback)
        {
            SVersion currentVersion = new SVersion(Application.version);
            SVersion futureVersion = new SVersion(currentVersion.major, currentVersion.minor+1, currentVersion.build);
            
            yield return null;
            
            // 테스트 모드에 따라 결과와 버젼객체를 콜백에 전달한다.
            switch (_testMode)
            {
                case ETestMode.UpdateRequiredTest:
                    callback(EGetVersionResult.Success, futureVersion);
                    break;
                
                case ETestMode.AlreadyUpdatedTest:
                    callback(EGetVersionResult.Success, currentVersion);
                    break;
                
                case ETestMode.ParseErrorTest:
                    callback(EGetVersionResult.ParseError, null);
                    break;
                
                case ETestMode.ConnectionErrorTest:
                    callback(EGetVersionResult.ConnectionError, null);
                    break;
                
                default:
                    Log.Error($"발생될 수 없는 테스트모드 ({_testMode})");
                    break;
            }
        }
    }
}

#endif