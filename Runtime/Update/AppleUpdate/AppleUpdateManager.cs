#nullable enable
 
using System;
using System.Threading.Tasks;
 
namespace Rano.Update.AppleUpdate
{
    public sealed class AppleUpdateManager : IUpdateManager
    {
        public bool IsInitialized => true;
        
        public void Initialize()
        {
            Log.Info("초기화 중...");
            Log.Info("초기화 완료");
        }
        
        public async Task<UpdateInfo> GetUpdateInfoAsync()
        {
            // TODO: Firebase.RemoteConfig를 이용하여 버젼을 얻고 UpdateInfo를 리턴.
            throw new NotImplementedException();
            await Task.Yield();
        }
 
        public async Task<bool> UpdateAsync()
        {
            // TODO: 앱스토어의 페이지로 연결시킴
            throw new NotImplementedException();
            await Task.Yield();
        }
    }
}