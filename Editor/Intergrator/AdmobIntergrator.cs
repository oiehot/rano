using UnityEngine;
using UnityEditor;
using Rano;
using Rano.PlatformServices.Admob;
using RanoEditor.Helper;
using GoogleMobileAds.Editor;

namespace RanoEditor.Intergrator
{
    /// <summary>
    /// AdmobSettingsSO를 GoogleMobileAds Settings에 통합시킨다.
    /// </summary>
    /// <remarks>에디터에서 플레이 전후로 실행된다.</remarks>
    public static class AdmobIntergrator
    {
        [InitializeOnLoadMethod]
        public static void Intergrate()
        {
            Log.Info($"Intergrating {nameof(AdmobSettingsSO)} to {nameof(GoogleMobileAdsSettings)}...");
            var admobSettings = AssetDatabaseHelper.GetScriptableObject<AdmobSettingsSO>();
            if (admobSettings == null)
            {
                Log.Warning($"{typeof(AdmobSettingsSO).FullName}가 없습니다. 생성하고 설정하시기를 추천합니다. 설정통합을 생략합니다");
                return;
            }
            // WARN: GoogleMobileAdsSettings 클래스는 원래 internal class 였습니다.
            // 수정하기 위해서 public class로 수정했습니다. 추후 플러그인이 업데이트 되었을 때 이 부분에서 에러가 발생할 수 있습니다.
            var settings = GoogleMobileAdsSettings.Instance;
            settings.GoogleMobileAdsAndroidAppId = admobSettings.androidId;
            settings.GoogleMobileAdsIOSAppId = admobSettings.iosId;
            settings.DelayAppMeasurementInit = admobSettings.delayAppMeasurement;
        }
    }
}