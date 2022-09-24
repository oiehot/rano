#nullable enable

using System.Threading.Tasks;

namespace Rano.Update
{
    /// <summary>
    /// 더미 앱 업데이트 관리자
    /// </summary>
    /// <remarks>
    /// 아무런 액션을 취하지 않고 경고 메시만 출력한다.
    /// 앱 업데이트가 필요 없거나 준비되지 않은 플랫폼에서 사용하면 된다.
    /// </remarks>
    public sealed class DummyUpdateManager : IUpdateManager
    {
        public bool IsInitialized
        {
            get
            {
                Log.Warning("IsInitialized 요청은 더미 값인 false로 리턴합니다 (Dummy)");
                return false;
            }
        }
        
        public void Initialize()
        {
            Log.Warning("초기화 요청을 생략합니다 (Dummy)");
        }
        
        public async Task<UpdateInfo> GetUpdateInfoAsync()
        {
            Log.Warning("GetUpdateInfoAsync 요청을 생략하고 기본값을 리턴합니다 (Dummy)");
            await Task.Delay(0);
            return new UpdateInfo();
        }
 
        public async Task<bool> UpdateAsync()
        {
            Log.Warning("업데이트 요청을 생략합니다 (Dummy)");
            await Task.Delay(0);
            return false;
        }
    }
}