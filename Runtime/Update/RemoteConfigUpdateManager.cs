#nullable enable

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
        protected override ECheckUpdateResult GetUpdateStatus()
        {
            Log.Info("업데이트 검사 중...");
            
            if (IsInitialized == false)
            {
                Log.Warning("업데이트 검사 실패 (초기화 되어있지 않음)");
                return ECheckUpdateResult.Error;
            }

            if (RemoteConfig!.TryGetString(Constants.VERSION, out string? latestVersionStr) == false)
            {
                Log.Warning("업데이트 검사 실패 (마지막 버전 값을 얻을 수 없음)");
                return ECheckUpdateResult.Error;
            }
            
            if (string.IsNullOrEmpty(latestVersionStr))
            {
                Log.Warning("업데이트 검사 실패 (마지막 버전 값이 비어 있음)");
                return ECheckUpdateResult.Error;
            }
            
            SVersion latestVersion = new SVersion(latestVersionStr!);
            if (CurrentVersion < latestVersion)
            {
                Log.Info($"업데이트가 필요합니다 ({CurrentVersion} => {latestVersion})");
                return ECheckUpdateResult.UpdateRequired;
            }
            else if (CurrentVersion == latestVersion)
            {
                Log.Info($"이미 최신 버전입니다 ({CurrentVersion})");
                return ECheckUpdateResult.UpdateAlready;
            }
            else
            {
                Log.Info($"현재 버전이 RemoteConfig에 설정된 버전보다 최신입니다 (current:{CurrentVersion}, latest:{latestVersion})");
                return ECheckUpdateResult.UpdateAlready;
            }
        }
    }
}