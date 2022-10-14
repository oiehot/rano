#nullable enable

using Rano.App;

namespace Rano.Update
{
    public sealed class TestUpdateManager : UpdateManager
    {
        protected override ECheckUpdateResult GetUpdateStatus()
        {
            Log.Info("업데이트 검사 중... (TEST MODE)");
            
            if (IsInitialized == false)
            {
                Log.Warning("업데이트 검사 실패 (초기화 되어있지 않음) (TEST MODE)");
                return ECheckUpdateResult.Error;
            }

            SVersion[] versions =
            {
                new SVersion("0.0.1"),
                _currentVersion,
                new SVersion("9.9.9")
            };
            
            SVersion latestVersion =
                Rano.Math.RandomUtils.GetRandomValueInArray<SVersion>(versions);
            
            if (_currentVersion < latestVersion)
            {
                Log.Important($"업데이트가 필요합니다 ({_currentVersion} => {latestVersion}) (TEST MODE)");
                return ECheckUpdateResult.UpdateRequired;
            }
            else if (_currentVersion == latestVersion)
            {
                Log.Important($"이미 최신 버전입니다 ({_currentVersion}) (TEST MODE)");
                return ECheckUpdateResult.UpdateAlready;
            }
            else // if (_currentVersion > latestVersion)
            {
                Log.Warning($"현재 버전이 RemoteConfig에 설정된 버전보다 최신입니다 (current:{_currentVersion}, latest:{latestVersion}) (TEST MODE)");
                return ECheckUpdateResult.Error;
            }
        }
    }
}