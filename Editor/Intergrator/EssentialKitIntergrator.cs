using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rano.PlatformServices.Billing;
using Rano.PlatformServices.Gaming;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.EssentialKit;

namespace RanoEditor.Intergrator
{
    /// <summary>
    /// InAppProduct, Achievement, Leaderboard 스크립터블 오브젝트 데이터를 모아서 EssentialKitSettings를 업데이트한다.
    /// </summary>
    /// <remarks>에디터에서 플레이 전후로 실행된다.</remarks>
    public static class EssentialKitIntergrator
    {
        [InitializeOnLoadMethod]
        private static void Intergrate()
        {
            Debug.Log("Intergrating VoxelBustersEssentialKit...");
            var settings = GetEssentialKitSettings();
            IntergrateInAppProducts(settings);
            IntergrateAchievementsAndLeaderboards(settings);
        }
        
        /// <summary>
        /// InAppProductSO 데이터를 EssentialKitSettings.BillingServicesSettings에 통합시킨다.
        /// </summary>
        private static void IntergrateInAppProducts(EssentialKitSettings settings)
        {
            Debug.Log("Intergrating InAppProducts to VoxelBustersEssentialKit...");
            
            var before = settings.BillingServicesSettings;
            var billingServiceSettings = new BillingServicesUnitySettings (
                before.IsEnabled,
                GetBillingProductDefinitons().ToArray(),
                before.MaintainPurchaseHistory,
                before.AutoFinishTransactions,
                before.VerifyPaymentReceipts,
                before.IosProperties,
                before.AndroidProperties
            );
            settings.BillingServicesSettings = billingServiceSettings;
        }

        /// <summary>
        /// AchievementSO, LeaderboardSO 데이터를 EssentialKitSettings.GameServicesSettings에 통합시킨다.
        /// </summary>
        private static void IntergrateAchievementsAndLeaderboards(EssentialKitSettings settings)
        {
            // TODO: 에드몹 ID 통합 필요
            Debug.LogWarning("Intergration Required: <b>AndroidProperties</b> (GoogleAdmob => EssentialKitSettings.GameServicesSettings, if null then err)");
            Debug.Log("Intergrating Achievements and Leaderboards to VoxelBustersEssentialKit...");
            var before = settings.GameServicesSettings;
            var gameServicesSettings = new GameServicesUnitySettings(
                before.IsEnabled,
                true,
                GetLeaderboardDefinitons().ToArray(),
                GetAchievementDefinitons().ToArray(),
                before.ShowAchievementCompletionBanner,
                before.AndroidProperties
            );
            settings.GameServicesSettings = gameServicesSettings;
        }        
        
        /// <summary>
        /// EssentialKitSettings 에셋을 로드해서 리턴한다.
        /// </summary>
        private static EssentialKitSettings GetEssentialKitSettings()
        {
            var scriptableObjectName = nameof(EssentialKitSettings);
            var guids = AssetDatabase.FindAssets($"t:{scriptableObjectName}");
            if (guids.Length > 1) throw new Exception($"{scriptableObjectName} 에셋이 1개를 초과할 수는 없습니다");
            var guid = guids[0];
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var settings = AssetDatabase.LoadAssetAtPath(path, typeof(EssentialKitSettings)) as EssentialKitSettings;
            return settings;
        }

        /// <summary>
        /// LeaderboardSO 에셋들을 모아서 LeaderboardDefinition 리스트로 변환한다.
        /// </summary>
        private static List<LeaderboardDefinition> GetLeaderboardDefinitons()
        {
            var scriptableObjectName = nameof(LeaderboardSO);
            var leaderboardList = new List<LeaderboardDefinition>();
            var guids = AssetDatabase.FindAssets($"t:{scriptableObjectName}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var leaderboard = AssetDatabase.LoadAssetAtPath(path, typeof(LeaderboardSO)) as LeaderboardSO;
                if (leaderboard == null) throw new Exception($"Missing {scriptableObjectName} ({path}, {guid}");
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
                Debug.Log($"* {leaderboard.id} (Leaderboard)");
                leaderboardList.Add(product);
            }
            return leaderboardList;
        }
        
        /// <summary>
        /// AchievementSO 에셋들을 모아서 AchievementDefinition 리스트로 변환한다.
        /// </summary>
        private static List<AchievementDefinition> GetAchievementDefinitons()
        {
            var scriptableObjectName = nameof(AchievementSO);
            var achievementList = new List<AchievementDefinition>();
            var guids = AssetDatabase.FindAssets($"t:{scriptableObjectName}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var achievement = AssetDatabase.LoadAssetAtPath(path, typeof(AchievementSO)) as AchievementSO;
                if (achievement == null) throw new Exception($"Missing {scriptableObjectName} ({path}, {guid}");
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
                Debug.Log($"* {achievement.id} (Achievement)");
                achievementList.Add(product);
            }
            return achievementList;
        }

        /// <summary>
        /// InAppProductSO 에셋들을 모아서 BillingProductDefinition 리스트로 변환한다.
        /// </summary>
        private static List<BillingProductDefinition> GetBillingProductDefinitons()
        {
            var scriptableObjectName = nameof(InAppProductSO);
            var productList = new List<BillingProductDefinition>();
            var guids = AssetDatabase.FindAssets($"t:{scriptableObjectName}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var iap = AssetDatabase.LoadAssetAtPath(path, typeof(InAppProductSO)) as InAppProductSO;
                if (iap == null)
                {
                    throw new Exception($"Missing {scriptableObjectName} ({path}, {guid}");
                }
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
                Debug.Log($"* {iap.id} (InAppProduct)");
                productList.Add(product);
            }
            return productList;
        }
    }
}