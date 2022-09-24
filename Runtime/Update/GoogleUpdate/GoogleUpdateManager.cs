#nullable enable
 
 using System;
 using System.Threading.Tasks;
 
 namespace Rano.Update.GoogleUpdate
 {
     public sealed class GoogleUpdateManager : IUpdateManager
     {
         public bool IsInitialized => true;
        
         public void Initialize()
         {
             Log.Info("초기화 중...");
             Log.Info("초기화 완료");
         }
         
         public async Task<UpdateInfo> GetUpdateInfoAsync()
         {
             // TODO: GooglePlay in-app update 플러그인을 사용하여 UpdateInfo 인스턴스 생성.
             throw new NotImplementedException();
             await Task.Yield();
         }
 
         public async Task<bool> UpdateAsync()
         {
             // TODO: GooglePlay in-app update 플러그인을 사용하여 즉시 업데이트 한다.
             throw new NotImplementedException();
             await Task.Yield();
         }
     }
 }