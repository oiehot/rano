using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rano;
using Rano.PlatformServices.Billing;
using Rano.PlatformServices.Gaming;
using RanoEditor.Helper;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.EssentialKit;

namespace RanoEditor.Intergrator
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
            var essentialKitSettings = AssetDatabaseHelper.GetScriptableObject<EssentialKitSettings>();
            if (essentialKitSettings == null)
            {
                Log.Info($"{typeof(EssentialKitSettings).FullName} 스크립터블 오브젝트가 없으므로 설정통합을 생략합니다.");
                return;
            }
            IntergrateInAppProducts(essentialKitSettings);
            IntergrateAchievementsAndLeaderboards(essentialKitSettings);
        }
        
        /// <summary>
        /// InAppProductSO 데이터를 EssentialKitSettings.BillingServicesSettings에 통합시킨다.
        /// </summary>
        private static void IntergrateInAppProducts(EssentialKitSettings settings)
        {
            Log.Info($"Intergrating {nameof(InAppProductSO)} to {nameof(VoxelBusters.EssentialKit)}...");
            var before = settings.BillingServicesSettings;
            var billingServiceSettings = new BillingServicesUnitySettings (
                before.IsEnabled,
                Get_Intergrated_BillingProductDefinitons().ToArray(),
                before.MaintainPurchaseHistory,
                before.AutoFinishTransactions,
                before.VerifyPaymentReceipts,
                before.IosProperties,
                before.AndroidProperties
            );
            settings.BillingServicesSettings = billingServiceSettings;
        }

        /// <summary>
        /// GpgsSettingsSO 데이터를 통합한 EssentialKitSettings.GameServicesSettings.AndroidProperties을 리턴한다.
        /// </summary>
        private static VoxelBusters.EssentialKit.GameServicesUnitySettings.AndroidPlatformProperties
            Get_GameServices_AndroidPlatformProperties_Intergrated(EssentialKitSettings settings)
        {
            var gpgsSettings = AssetDatabaseHelper.GetScriptableObject<GpgsSettingsSO>();
            if (gpgsSettings == null) return null;
            var before = settings.GameServicesSettings.AndroidProperties;
            var androidPlatformProperties =
                new VoxelBusters.EssentialKit.GameServicesUnitySettings.AndroidPlatformProperties(
                    gpgsSettings.gpgsApplicationId,
                    before.ServerClientId,
                    before.AchievedDescriptionFormats,
                    before.ShowErrorDialogs,
                    before.DisplayPopupsAtTop,
                    before.NeedsProfileScope,
                    before.NeedsEmailScope
                );
            return androidPlatformProperties;
        }

        /// <summary>
        /// AchievementSO, LeaderboardSO 데이터를 EssentialKitSettings.GameServicesSettings에 통합시킨다.
        /// </summary>
        private static void IntergrateAchievementsAndLeaderboards(EssentialKitSettings settings)
        {
            Log.Info($"Intergrating {nameof(AchievementSO)} and {nameof(LeaderboardSO)} to {nameof(VoxelBusters.EssentialKit)}...");
            var before = settings.GameServicesSettings;
            
            var androidPlatformProperties = Get_GameServices_AndroidPlatformProperties_Intergrated(settings);
            if (androidPlatformProperties == null)
            {
                Log.Warning($"{typeof(GpgsSettingsSO).FullName}가 없습니다. 생성하고 설정하시기를 추천합니다. 설정통합을 생략합니다");
                androidPlatformProperties = before.AndroidProperties;
            }
            
            var gameServicesSettings = new GameServicesUnitySettings(
                before.IsEnabled,
                true,
                Get_LeaderboardDefinitons_Intergrated().ToArray(),
                Get_AchievementDefinitons_Intergrated().ToArray(),
                before.ShowAchievementCompletionBanner,
                androidPlatformProperties
            );
            settings.GameServicesSettings = gameServicesSettings;
        }

        /// <summary>
        /// LeaderboardSO 데이터를 통합한 LeaderboardDefinition 리스트를 리턴한다.
        /// </summary>
        private static List<LeaderboardDefinition> Get_LeaderboardDefinitons_Intergrated()
        {
            var leaderboardList = new List<LeaderboardDefinition>();
            var leaderboards = AssetDatabaseHelper.GetScriptableObjects<LeaderboardSO>();
            foreach (var leaderboard in leaderboards)
            {
                if (!leaderboard.includeInBuild) continue;
                
                var platformIdOverrides = new NativePlatformConstantSet(
                    leaderboard.iosId,
                    leaderboard.tvosId,
                    leaderboard.androidId
                );                
                var product = new LeaderboardDefinition(
                        leaderboard.id,
                        null,
                        platformIdOverrides,
                        leaderboard.title,
                        null,
                        null
                );
                // Log.Info($"* {leaderboard.id} (Leaderboard)");
                leaderboardList.Add(product);
            }
            return leaderboardList;
        }
        
        /// <summary>
        /// AchievementSO 데이터를 통합한 AchievementDefinition 리스트를 리턴한다.
        /// </summary>
        private static List<AchievementDefinition> Get_AchievementDefinitons_Intergrated()
        {
            var achievementList = new List<AchievementDefinition>();
            var achievements = AssetDatabaseHelper.GetScriptableObjects<AchievementSO>();
            foreach (var achievement in achievements)
            {
                if (!achievement.includeInBuild) continue;
                
                var platformIdOverrides = new NativePlatformConstantSet(
                    achievement.iosId,
                    achievement.tvosId,
                    achievement.androidId
                );
                var product = new AchievementDefinition(
                    achievement.id,
                    null,
                    platformIdOverrides,
                    achievement.title,
                    achievement.numberOfStepsToUnlock,
                    null,
                    null
                );
                // Debug.Log($"* {achievement.id} (Achievement)");
                achievementList.Add(product);
            }
            return achievementList;
        }

        /// <summary>
        /// InAppProductSO 데이터를 통합한 BillingProductDefinition 리스트를 리턴한다. 
        /// </summary>
        private static List<BillingProductDefinition> Get_Intergrated_BillingProductDefinitons()
        {
            var productList = new List<BillingProductDefinition>();
            var iaps = AssetDatabaseHelper.GetScriptableObjects<InAppProductSO>();
            foreach (var iap in iaps)
            {
                var platformIdOverrides = new NativePlatformConstantSet(
                    null,
                    null,
                    null
                );
                BillingProductType billingProductType;
                switch (iap.productType)
                {
                    case InAppProductType.Consumable:
                        billingProductType = BillingProductType.Consumable;
                        break;
                    case InAppProductType.NonConsumable:
                        billingProductType = BillingProductType.NonConsumable;
                        break;
                    default:
                        throw new Exception($"Incompatible in-app product type ({iap.productType})");
                }
                
                var product = new BillingProductDefinition(
                    iap.id,
                    iap.id,
                    platformIdOverrides,
                    billingProductType,
                    iap.title,
                    iap.description,
                    null,
                    null
                );
                // Log.Info($"* {iap.id} (InAppProduct)");
                productList.Add(product);
            }
            return productList;
        }
    }
}