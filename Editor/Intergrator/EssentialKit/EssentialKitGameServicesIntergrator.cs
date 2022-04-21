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

namespace RanoEditor.Intergrator.EssentialKit
{
    public static class EssentialKitGameServicesIntergrator
    {
        public static GameServicesSettingsSO GetGameServicesSettings()
        {
            GameServicesSettingsSO settings = AssetDatabaseHelper.GetScriptableObject<GameServicesSettingsSO>();
            return settings;
        }
        
        /// <summary>
        /// AchievementSO, LeaderboardSO 데이터를 사용해서
        /// EssentialKitSettings.GameServicesSettings에 통합시킨다.
        /// </summary>
        public static void Intergrate()
        {
            Log.Info($"Intergrating EssentialKit GamingServices...", caller: false);
            
            // EssentialKitSettings 를 얻는다.
            EssentialKitSettings essentialKitSettings = EssentialKitIntergrator.GetEssentialKitSettings();
            if (essentialKitSettings == null)
            {
                Log.Info($"{typeof(EssentialKitSettings).FullName} 스크립터블 오브젝트가 없으므로 설정통합을 생략합니다.", caller:false);
                return;
            }
            
            // GameServicesSettingsSO를 얻는다.
            GameServicesSettingsSO gameServicesSettings = GetGameServicesSettings();
            if (gameServicesSettings == null)
            {
                Log.Info($"{typeof(GameServicesSettingsSO).FullName} 스크립터블 오브젝트가 없으므로 설정통합을 생략합니다.", caller:false);
                return;
            }
            
            // 새 설정을 만든다.
            var beforeSettings = essentialKitSettings.GameServicesSettings;
            var newSettings = new GameServicesUnitySettings(
                beforeSettings.IsEnabled,
                true,
                GetLeaderboardDefinitions().ToArray(),
                GetAchievementDefinitions().ToArray(),
                beforeSettings.ShowAchievementCompletionBanner,
                GetAndroidPlatformProperties(essentialKitSettings, gameServicesSettings)
            );
            
            // 설정을 업데이트한다.
            essentialKitSettings.GameServicesSettings = newSettings;
        }
        
        /// <summary>
        /// GameServicesSettingsSO 데이터를 통합한 GameServicesSettings.AndroidProperties을 생성하여 리턴한다.
        /// </summary>
        private static GameServicesUnitySettings.AndroidPlatformProperties GetAndroidPlatformProperties(EssentialKitSettings essentialKitSettings, GameServicesSettingsSO gameServicesSettings)
        {   
            var before = essentialKitSettings.GameServicesSettings.AndroidProperties;
            var androidPlatformProperties =
                new GameServicesUnitySettings.AndroidPlatformProperties(
                    gameServicesSettings.gpgsApplicationId,
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
        /// LeaderboardSO 데이터를 통합한 LeaderboardDefinition 리스트를 리턴한다.
        /// </summary>
        private static List<LeaderboardDefinition> GetLeaderboardDefinitions()
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
                leaderboardList.Add(product);
            }
            return leaderboardList;
        }
        
        /// <summary>
        /// AchievementSO 데이터를 통합한 AchievementDefinition 리스트를 리턴한다.
        /// </summary>
        private static List<AchievementDefinition> GetAchievementDefinitions()
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
                achievementList.Add(product);
            }
            return achievementList;
        }

    }
}