using System;
using System.Collections;
using UnityEngine;
using Rano;
using Rano.App;

namespace Rano.PlatformServices.Store
{
    public enum ECheckUpdatesResult
    {
        UpdateRequired,
        AlreadyUpdated,
        ParseError,
        ConnectionError,
        UnknownError
    }
        
    public class AppStoreManager : MonoBehaviour
    {
        private IAppStore _appstore;
        public Action OnUpdateRequired { get; set; }
        public Action OnAlreadyUpdated { get; set; }
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
                    Log.Info("애플 앱스토어를 사용합니다");
                    _appstore = new AppleAppStore(Application.identifier);
                    break;

                case RuntimePlatform.Android:
                    Log.Info("구글 앱스토어를 사용합니다");
                    _appstore = new GoogleAppStore(Application.identifier);
                    break;
                    
                #if UNITY_EDITOR
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                    Log.Info("테스트 앱스토어를 사용합니다");
                    _appstore = new TestAppStore(Application.identifier);
                    break;
                #endif
                
                default:
                    Log.Error($"현재 플랫폼은 지원하지 않습니다 ({CurrentPlatform})");
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
        public IEnumerator CheckUpdatesCoroutine(Action<ECheckUpdatesResult> callback)
        {
            Log.Info("앱스토어에서 이 앱의 최신버젼을 파악하는중...");
            yield return _appstore.GetLatestVersionCoroutine((result, latestAppVersion) => {
                switch (result)
                {
                    case EGetVersionResult.Success:
                        if (latestAppVersion.HasValue == false)
                        {
                            Log.Warning($"성공했다는 결과를 받았지만 정작 버젼값이 비어있습니다.");
                            callback(ECheckUpdatesResult.ParseError);
                            OnParseFailed?.Invoke();
                            break;
                        }
                        if (CurrentAppVersion < latestAppVersion.Value)
                        {
                            Log.Warning($"최신버젼이 출시되었습니다. 업데이트가 필요합니다. (current:{CurrentAppVersion}, latest:{latestAppVersion})");
                            callback(ECheckUpdatesResult.UpdateRequired);
                            OnUpdateRequired?.Invoke();
                        }
                        else
                        {
                            Log.Info($"앱이 최신버젼입니다. ({CurrentAppVersion})");
                            callback(ECheckUpdatesResult.AlreadyUpdated);
                            OnAlreadyUpdated?.Invoke();
                        }
                        break;
                    
                    case EGetVersionResult.ConnectionError:
                        Log.Warning($"앱스토어 접속에 실패하여 최신버젼을 파악할 수 없습니다.");
                        callback(ECheckUpdatesResult.ConnectionError);
                        OnConnectionFailed?.Invoke();
                        break;
                    
                    case EGetVersionResult.ParseError:
                        Log.Warning($"앱스토어 페이지 파싱에 실패하여 최신버젼을 파악할 수 없습니다.");
                        callback(ECheckUpdatesResult.ParseError);
                        OnParseFailed?.Invoke();
                        break;
                    
                    default:
                        Log.Error($"발생해서는 안되는 결과 ({result})");
                        callback(ECheckUpdatesResult.UnknownError);
                        break;
                }
            });
        }
    }
}
