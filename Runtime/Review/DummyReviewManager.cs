#nullable enable

using System.Threading.Tasks;

namespace Rano.Review
{
    /// <summary>
    /// 더미 리뷰 관리자
    /// </summary>
    /// <remarks>
    /// 초기화와 리뷰 요청은 비어 있으며, 아무런 액션을 취하지 않고 경고 메시만 출력한다.
    /// 리뷰 요청이 필요 없거나 준비되지 않은 플랫폼에서 사용하면 된다.
    /// </remarks>
    public sealed class DummyReviewManager : ReviewManager
    {
        private bool _initialized = false;
        public override bool IsInitialized => _initialized;
        
        public override bool Initialize()
        {
            Log.Warning("초기화를 생략합니다 (Dummy)");
            return true;
        }

        public override async Task<bool> RequestReviewInternalAsync()
        {
            Log.Warning("리뷰 요청을 생략합니다 (Dummy)");
            await Task.Delay(0);
            return false;
        }
        
        public override string GetWebPageUrl(string id)
        {
            id = Rano.Network.URLHelper.EscapeURL(id);
            return $"https://play.google.com/store/apps/details?id={id}";
        }
        
        public override string GetAppStoreUrl(string id)
        {
            id = Rano.Network.URLHelper.EscapeURL(id);
            return $"https://play.google.com/store/apps/details?id={id}";
        }
    }
}