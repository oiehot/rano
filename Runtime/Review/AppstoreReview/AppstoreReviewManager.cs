#nullable enable

using System.Threading.Tasks;
using UnityEngine.iOS;

namespace Rano.Review.AppstoreReview
{
    /// <summary>
    /// 애플 앱스토어 리뷰 전반을 관리한다.
    /// </summary>
    public sealed class AppstoreReviewManager : Rano.Review.ReviewManager
    {
        public override bool IsInitialized => true; // TODO: 개발 안됨
        
        public override bool Initialize()
        {
            Log.Info("초기화 중...");
            Log.Info("초기화 완료");
            return true;
        }

        public override async Task<bool> RequestReviewInternalAsync()
        {
            
            Log.Info("리뷰 요청 중...");
            bool result = Device.RequestStoreReview();
            if (result == false)
            {
                Log.Warning("리뷰 요청 실패 (RequestStoreReview Failed)");
                return false;
            }
            Log.Info("리뷰 요청 완료");
            return true;
        }
    }
}