using UnityEditor;
using VoxelBusters.EssentialKit;

namespace Rano.Editor.Intergrator.EssentialKit
{
    /// <summary>
    /// InAppProductSO, AchievementSO, LeaderboardSO 데이터를 모아서 EssentialKitSettings를 업데이트한다.
    /// </summary>
    /// <remarks>에디터에서 플레이 전후로 실행된다.</remarks>
    public static class EssentialKitIntergrator
    {
        [InitializeOnLoadMethod]
        public static void Intergrate()
        {
            Log.Info($"Intergrating EssentialKit...", caller: false);
            
            #if USE_ESSENTIAL_KIT_BILLING_SERVICES
            EssentialKitBillingServicesIntergrator.Intergrate();
            #endif
            
            EssentialKitGameServicesIntergrator.Intergrate();
        }

        public static EssentialKitSettings GetEssentialKitSettings()
        {
            EssentialKitSettings settings = AssetDatabaseHelper.GetScriptableObject<EssentialKitSettings>();
            return settings;
        }
    }
}