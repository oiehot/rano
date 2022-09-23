#nullable enable

using System.Threading.Tasks;

namespace Rano.Review
{
    /// <summary>
    /// 테스트 리뷰 관리자
    /// </summary>
    /// <remarks>초기화에서는 아무런 액션을 취하지 않으며, 리뷰 요청은 항상 성공한다.</remarks>
    public sealed class TestReviewManager : ReviewManager
    {
        private bool _initialized = false;
        public override bool IsInitialized => _initialized;
        
        public override bool Initialize()
        {
            Log.Info("초기화 중... (Test)");
            _initialized = true;
            Log.Info("초기화 완료... (Test)");
            return true;
        }

        public override async Task<bool> RequestReviewInternalAsync()
        {
            Log.Info("리뷰 요청 중... (Test)");
            await Task.Delay(3000);
            Log.Info("리뷰 요청 완료 (Test)");            
            return true;
        }
    }
}