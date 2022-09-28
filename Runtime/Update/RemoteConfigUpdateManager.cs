#nullable enable

using System;
using System.Threading.Tasks;
using Rano.App;

namespace Rano.Update
{
    /// <summary>
    /// RemoteConfig를 사용해서 최신 버젼을 파악하여
    /// 업데이트가 필요하면 UI를 띄워 업데이트를 유도한다.
    /// </summary>
    public sealed class RemoteConfigUpdateManager : UpdateManager
    {
        /// <summary>
        /// 업데이트 여부를 리턴한다.
        /// </summary>
        protected override async Task<ECheckUpdateResult> GetUpdateStatusAsync()
        {
            Log.Info("업데이트 검사 중...");
            
            if (IsInitialized == false)
            {
                Log.Warning("업데이트 검사 실패 (초기화 되어있지 않음)");
                return ECheckUpdateResult.Error;
            }
            
            // Fetch를 성공하지 못하면 false가 리턴된다.
            await _remoteConfig!.FetchAsync();
            
            // Fetch를 하던 안 하던 Activate를 하여 최근에 Fetch된 데이터를 적용한다.
            // 수정된게 없으면 false가 리턴된다.
            await _remoteConfig!.ActivateAsync();

            if (_remoteConfig!.TryGetString(Constants.VERSION, out string? latestVersionStr) == false)
            {
                Log.Warning("업데이트 검사 실패 (마지막 버전 값을 얻을 수 없음)");
                return ECheckUpdateResult.Error;
            }
            
            if (String.IsNullOrEmpty(latestVersionStr) == true)
            {
                Log.Warning("업데이트 검사 실패 (마지막 버전 값이 비어 있음)");
                return ECheckUpdateResult.Error;
            }
            
            SVersion latestVersion = new SVersion(latestVersionStr);
            if (_currentVersion < latestVersion)
            {
                Log.Info($"업데이트가 필요합니다 ({_currentVersion} => {latestVersion})");
                return ECheckUpdateResult.UpdateRequired;
            }
            else if (_currentVersion == latestVersion)
            {
                Log.Info($"이미 최신 버전입니다 ({_currentVersion})");
                return ECheckUpdateResult.UpdateAlready;
            }
            else // if (_currentVersion > latestVersion)
            {
                Log.Warning($"현재 버전이 RemoteConfig에 설정된 버전보다 최신입니다 (current:{_currentVersion}, latest:{latestVersion})");
                return ECheckUpdateResult.Error;
            }
        }
    }
}