#nullable enable

using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Rano.App;
using Rano.RemoteConfig;
using Rano.EventSystem;
using Rano.Store;

namespace Rano.Update
{
    public enum ECheckUpdateResult
    {
        Error = 0,
        UpdateRequired,
        UpdateAlready
    }
    
    public abstract class UpdateManager: ManagerComponent
    {
        private VoidEventChannelSO? _updateRequiredEventChannel;
        protected IRemoteConfigManager? _remoteConfig;
        protected SVersion _currentVersion;
        public bool IsInitialized => _remoteConfig != null && _remoteConfig.IsInitialized;

        
        /// <summary>
        /// 초기화한다.
        /// </summary>
        public void Initialize(IRemoteConfigManager remoteConfigManager, VoidEventChannelSO updateRequiredEventChannel)
        {
            Log.Info("초기화 중...");
            _currentVersion = new SVersion(Application.version);
            _remoteConfig = remoteConfigManager;
            _updateRequiredEventChannel = updateRequiredEventChannel;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Log.Info($"현재 버젼: {_currentVersion}");
#endif
            Log.Info("초기화 완료");
        }

        /// <summary>
        /// 업데이트 상태를 체크하고
        /// 업데이트가 필요하면 이벤트를 발생시킨다.
        /// </summary>
        public async Task<ECheckUpdateResult> CheckUpdate()
        {
            ECheckUpdateResult status = await GetUpdateStatusAsync();
            if (status == ECheckUpdateResult.UpdateRequired)
            {
                Debug.Assert(_updateRequiredEventChannel != null);
                _updateRequiredEventChannel!.RaiseEvent();
            }
            return status;
        }
        
        /// <summary>
        /// 업데이트 상태를 알아낸다.
        /// </summary>
        protected abstract Task<ECheckUpdateResult> GetUpdateStatusAsync();
        
        /// <summary>
        /// 업데이트를 시작한다.
        /// </summary>
        public void StartUpdate()
        {
            Log.Info("업데이트 시작");
            
            IAppStore store;
#if UNITY_EDITOR
            store = new Rano.Store.TestAppStore();
#elif UNITY_ANDROID
            store = new Rano.Store.GoogleAppStore();
#elif UNITY_IOS
            store = new Rano.Store.AppleAppStore();
#else
            Log.Warning("업데이트 실패 (지원하지 않는 플랫폼입니다)");
            return;
#endif
            
            string bundleId = Application.identifier;
            string storeUrl = store.GetStoreUrl(bundleId);
            Application.OpenURL(storeUrl);
        }
    }
}