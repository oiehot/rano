using System;
using System.Collections;
using UnityEngine;
using Rano;
using Rano.App;

namespace Rano.PlatformServices.Store
{
    public class AppStoreManager : MonoBehaviour
    {
        private IAppStore _appstore;
        public Action OnUpdateRequired { get; set; }
        public Action OnConnectionFailed { get; set; }
        public Action OnParseFailed { get; set; }
        private SVersion CurrentAppVersion => new SVersion(Application.version);
        private RuntimePlatform CurrentPlatform => Application.platform;
        
        void Awake()
        {
            switch (CurrentPlatform)
            {
                case RuntimePlatform.IPhonePlayer:
                    Log.Warning("TODO: 애플번들Id를 설정에서 얻도록 수정할것.");
                    _appstore = new AppleAppStore(Application.identifier);
                    break;

                case RuntimePlatform.Android:
                    _appstore = new GoogleAppStore(Application.identifier);
                    break;
                
                #if UNITY_EDITOR
                case RuntimePlatform.WindowsEditor:
                    _appstore = new TestAppStore(Application.identifier);
                    break;
                #endif
                
                default:
                    Log.Error($"현재 플랫폼은 사용이 불가능합니다 ({CurrentPlatform})");
                    break;
            }
        }

        /// <summary>
        /// 브라우져용 페이지를 연다.
        /// </summary>
        public void OpenBrowserPage()
        {
            Application.OpenURL(_appstore.BrowserUrl);
            
        }
        /// <summary>
        /// 업데이트가 필요하면 콜백을 호출한다.
        /// 최신버젼값을 얻고 현재버젼과 비교한다.
        /// </summary>
        public IEnumerator CheckUpdatesCoroutine()
        {
            yield return _appstore.GetLatestVersionCoroutine((result, latestAppVersion) => {
                switch (result)
                {
                    case EAppStoreResult.Success:
                        if (latestAppVersion.HasValue == false)
                        {
                            Log.Warning($"성공했다는 결과를 받았지만 정작 버젼값이 비어있습니다.");
                            OnParseFailed?.Invoke();
                            break;
                        }
                        if (CurrentAppVersion < latestAppVersion.Value)
                        {
                            Log.Warning($"앱 업데이트가 필요합니다. (current:{CurrentAppVersion}, latest:{latestAppVersion})");
                            OnUpdateRequired?.Invoke();
                        }
                        else
                        {
                            Log.Info($"앱이 최신버젼입니다. ({CurrentAppVersion})");
                        }
                        break;
                    
                    case EAppStoreResult.ConnectionError:
                        Log.Warning($"접속불량으로 최신버젼을 파악할 수 없습니다.");
                        OnConnectionFailed?.Invoke();
                        break;
                    
                    case EAppStoreResult.ParseError:
                        Log.Warning($"페이지 파싱에 실패하여 최신버젼을 파악할 수 없습니다.");
                        OnParseFailed?.Invoke();
                        break;
                    
                    default:
                        Log.Error($"알수없는 결과 ({result})");
                        break;
                }
            });
        }
    }
}
