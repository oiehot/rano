#nullable enable

using System.Threading.Tasks;
using UnityEngine;
using Rano.App;
using Rano.Math;

namespace Rano.Update
{
    public sealed class TestUpdateManager : IUpdateManager
    {
        public bool IsInitialized => true;

        public void Initialize()
        {
            Log.Info("초기화 중... (Test)");
            Log.Info("초기화 완료 (Test)");
        }
        
        public async Task<UpdateInfo> GetUpdateInfoAsync()
        {
            SVersion currentVersion = new SVersion(Application.version);
            
            // 50% 확률로 현재 버젼에서 Minor 버젼을 1올린다.
            if (RandomUtils.GetRandomBool() == true)  currentVersion.minor++;

            UpdateInfo updateInfo = new UpdateInfo()
            {
                latestVersion = currentVersion
            };

            await Task.Delay(1000);
            
            return updateInfo;
        }
 
        public async Task<bool> UpdateAsync()
        {
            Log.Info("업데이트 준비 중... (Test)");
            
            SVersion currentVersion = new SVersion(Application.version);
            UpdateInfo updateInfo = await GetUpdateInfoAsync();
            
            if (updateInfo.IsUpdatable() == true)
            {
                Log.Info($"업데이트 중... ({currentVersion} => {updateInfo.latestVersion}) (Test)");
                await Task.Delay(RandomUtils.GetRandomInt(1000,3000));
                Log.Info($"업데이트 성공 ({updateInfo.latestVersion}) (Test)");
            }
            else
            {
                Log.Info($"이미 최신 버젼입니다 (ver:{updateInfo.latestVersion}) (Test)");
            }
            
            Log.Info("업데이트 종료 (Test)");
            return true;
        }
    }
}