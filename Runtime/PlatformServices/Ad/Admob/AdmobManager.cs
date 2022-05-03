using UnityEngine;
using GoogleMobileAds.Api;

namespace Rano.PlatformServices.Ad.Admob
{
    public class AdmobManager : MonoBehaviour, IAdManager
    {
        public void Initialize()
        {
            Log.Info($"AdmobManager Initalize Start");
            MobileAds.Initialize(OnInitComplete);
        }
        
        /// <summary>
        /// 초기화 완료시 호출됨.
        /// </summary>
        /// <param name="status"></param>
        private void OnInitComplete(InitializationStatus status)
        {
            Log.Info($"AdmobManager Initialize Completed");
        }
    }
}