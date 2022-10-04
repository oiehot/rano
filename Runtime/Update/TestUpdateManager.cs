#nullable enable

using Rano.App;

namespace Rano.Update
{
    public sealed class TestUpdateManager : UpdateManager
    {
        protected override ECheckUpdateResult GetUpdateStatus()
        {
            Log.Info("TEST: 업데이트 검사 중...");
            
            if (IsInitialized == false)
            {
                Log.Warning("TEST: 업데이트 검사 실패 (초기화 되어있지 않음)");
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
                Log.Important($"TEST: 업데이트가 필요합니다 ({_currentVersion} => {latestVersion})");
                return ECheckUpdateResult.UpdateRequired;
            }
            else if (_currentVersion == latestVersion)
            {
                Log.Important($"TEST: 이미 최신 버전입니다 ({_currentVersion}) (Test)");
                return ECheckUpdateResult.UpdateAlready;
            }
            else // if (_currentVersion > latestVersion)
            {
                Log.Warning($"TEST: 현재 버전이 RemoteConfig에 설정된 버전보다 최신입니다 (current:{_currentVersion}, latest:{latestVersion}) (Test)");
                return ECheckUpdateResult.Error;
            }
        }
    }
}