using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Gaming
{
    public sealed class AchievementManager : MonoSingleton<AchievementManager>
    {
        public bool IsFeatureAvailable => GameServices.IsAvailable() && GameServices.IsAuthenticated;

        [ContextMenu("Show Achievements UI")]
        public void ShowAchievementsUI()
        {
            if (IsFeatureAvailable == false)
            {
                Log.Warning($"게임서비스를 사용할 수 없기 때문에 업적창을 띄울 수 없음");
                return;
            }

            GameServices.ShowAchievements((GameServicesViewResult result, Error error) =>
            {
                switch (result.ResultCode)
                {
                    case GameServicesViewResultCode.Done:
                        Log.Info("업적창이 성공적으로 닫힘");
                        break;
                    case GameServicesViewResultCode.Unknown:
                        Log.Info("업적창 닫힘상태를 알 수 없음");
                        break;
                }
            });
        }

        /// <summary>
        /// 업적 진척도 업데이트
        /// </summary>
        /// <param name="achievementId"></param>
        /// <param name="percent">0 ~ 100</param>
        public void ReportProgress(string achievementId, double percentCompleted)
        {
            GameServices.ReportAchievementProgress(achievementId, percentCompleted, (Error error) =>
            {
                if (error == null)
                {
                    Log.Info($"업적 진척도 업데이트 성공(Id:{achievementId}, 완료율:{percentCompleted}%)");
                }
                else
                {
                    Log.Warning($"업적 진척도 업데이트 실패 (Id:{achievementId}, 완료율:{percentCompleted}%) => {error}");
                }
            });
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [ContextMenu("Print Achievements")]
        private void PrintAchievements()
        {
            GameServices.LoadAchievements(callback: (GameServicesLoadAchievementsResult result, Error error) =>
            {
                if (error == null)
                {
                    IAchievement[] items = result.Achievements;
                    Log.Info($"전체 업적 수: {items.Length}");
                    for (int i = 0; i < items.Length; i++)
                    {
                        IAchievement item = items[i];
                        Log.Info($"* {item.Id}");
                        Log.Info($"\tPlatformId: {item.PlatformId}");
                        Log.Info($"\tLastReportedDate: {item.LastReportedDate}");
                        Log.Info($"\tIsCompleted: {item.IsCompleted}");
                        Log.Info($"\tPercentageCompleted: {item.PercentageCompleted}");
                    }
                }
                else
                {
                    Log.Warning($"업적들을 출력할 수 없음 ({error.ToString()})");
                }
            });
        }

        [ContextMenu("Print Achievements Description")]
        private void PrintAchievementsDescription()
        {
            GameServices.LoadAchievementDescriptions(callback: (result, error) =>
            {
                if (error == null)
                {
                    IAchievementDescription[] items = result.AchievementDescriptions;
                    Log.Info($"전체 업적 수: {items.Length}");
                    for (int i = 0; i < items.Length; i++)
                    {
                        IAchievementDescription item = items[i];
                        Log.Info($"* {item.Id}");
                        Log.Info($"\tPlatformId: {item.PlatformId}");
                        Log.Info($"\tTitle: {item.Title}");
                        Log.Info($"\tUnachievedDescription: {item.UnachievedDescription}");
                        Log.Info($"\tAchievedDescription: {item.AchievedDescription}");
                        Log.Info($"\tMaximumPoints: {item.MaximumPoints}");
                        Log.Info($"\tNumberOfStepsRequiredToUnlockAchievement: {item.NumberOfStepsRequiredToUnlockAchievement}");
                        Log.Info($"\tIsHidden: {item.IsHidden}");
                        Log.Info($"\tIsReplayable: {item.IsReplayable}");
                    }
                }
                else
                {
                    Log.Warning($"업적설명을 출력할 수 없음 ({error.ToString()})");
                }
            });
        }
#endif
    }
}