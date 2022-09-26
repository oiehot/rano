#nullable enable

using System;
using System.Threading.Tasks;
using UnityEngine;
using Rano.App;
using Rano.RemoteConfig;

namespace Rano.Update
{
    public enum ECheckUpdateResult
    {
        Error = 0,
        UpdateRequired,
        UpdateAlready
    }
    
    public abstract class UpdateManager : ManagerComponent
    {
        protected SVersion _currentVersion;
        protected IRemoteConfigManager? _remoteConfig;
        public bool IsInitialized => _remoteConfig != null && _remoteConfig.IsInitialized;
        public Action OnUpdateRequired { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            Log.Todo("부팅시 혹은 주기적으로 CheckUpdate를 호출한다. UpdateUI는 DontDestroyGameObject여야 한다.");
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        
        /// <summary>
        /// 초기화한다.
        /// </summary>
        public void Initialize(IRemoteConfigManager remoteConfigManager)
        {
            Log.Info("초기화 중...");
            
            _currentVersion = new SVersion(Application.version);
            _remoteConfig = remoteConfigManager;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Log.Info($"현재 버젼: {_currentVersion}");
#endif
            
            Log.Info("초기화 완료");
        }
        
        /// <summary>
        /// 업데이트 상태를 알아낸다.
        /// </summary>
        protected abstract Task<ECheckUpdateResult> GetUpdateStatusAsync();

        /// <summary>
        /// 업데이트 상태를 체크하고
        /// 업데이트가 필요하면 이벤트를 발생시킨다.
        /// </summary>
        public async void CheckUpdate()
        {
            ECheckUpdateResult status = await GetUpdateStatusAsync();
            if (status == ECheckUpdateResult.UpdateRequired)
            {
                OnUpdateRequired?.Invoke();
            }
        }

        /// <summary>
        /// 업데이트를 시작한다.
        /// </summary>
        public void StartUpdate()
        {
            Log.Todo("Start Update");
        }
    }
}