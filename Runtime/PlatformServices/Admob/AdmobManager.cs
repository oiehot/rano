using GoogleMobileAds.Api;

namespace Rano.PlatformServices.Admob
{
    /// <summary>
    /// Admob를 초기화하고 관리하는 클래스.
    /// </summary>
    public sealed class AdmobManager : MonoSingleton<AdmobManager>
    {
        protected override void Awake()
        {
            base.Awake();
            Log.Info($"Admob Initalize Start");
            MobileAds.Initialize(OnInitComplete);
        }

        /// <summary>
        /// 초기화 완료시 호출됨.
        /// </summary>
        /// <param name="status"></param>
        private void OnInitComplete(InitializationStatus status)
        {
            Log.Info($"Admob Initialize Completed");
        }

    }
}