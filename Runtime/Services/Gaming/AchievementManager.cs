using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.Services.Gaming
{
    public sealed class AchievementManager : BaseComponent
    {
        private readonly Dictionary<string, Achievement> _achievements = new Dictionary<string,Achievement>();
        private readonly Dictionary<string, AchievementInfo> _achievementInfos  = new Dictionary<string,AchievementInfo>();
        public Dictionary<string, Achievement> Achievements => _achievements;
        public Dictionary<string, AchievementInfo> AchievementInfos => _achievementInfos;

        /// <summary>
        /// 업적이 보고되었을 때 호출됨.
        /// </summary>
        public Action<string, double> OnAchievementReported { get; set; }
        
        /// <summary>
        /// 업적이 달성되었을 때 호출됨.
        /// </summary>
        public Action<string> OnAchievementCompleted { get; set; }

        /// <summary>
        /// 게임 서비스가 켜져 있고, 로그인이 되어있는 경우 true. 
        /// </summary>
        public bool IsFeatureAvailable => GameServices.IsAvailable() && GameServices.IsAuthenticated;

        /// <summary>
        /// 서버로부터 현재 업적들을 가져온다.
        /// </summary>
        public async Task UpdateAchievementsAsync()
        {
            if (!IsFeatureAvailable) return;
            
            VoxelBusters.EssentialKit.GameServicesLoadAchievementsResult result = null;
            VoxelBusters.CoreLibrary.Error error = null;
            
            GameServices.LoadAchievements(callback: (GameServicesLoadAchievementsResult _result, Error _error) =>
            {
                result = _result;
                error = _error;
            });

            while ( !(result != null || error != null) )
            {
                await Task.Delay(25);
            }
            
            if (error == null)
            {
                IAchievement[] items = result.Achievements;
                _achievements.Clear();
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    var achievement = new Achievement();
                    achievement.id = item.Id;
                    achievement.platformId = item.PlatformId;
                    achievement.lastReportedDate = item.LastReportedDate;
                    achievement.percentage = item.PercentageCompleted;
                    achievement.completed = item.IsCompleted;
                    _achievements[achievement.id] = achievement;
                }
                Log.Info($"서버에서 업적들을 가져왔습니다. (count: {_achievements.Count})");
            }
            else
            {
                Log.Warning($"서버에서 업적들을 가져올 수 없습니다. ({error.ToString()})");
                // _achievements.Clear();
            }
        }
        
        /// <summary>
        /// 서버로부터 모든 업적정보를 가져온다.
        /// </summary>
        public async Task UpdateAchievementInfosAsync()
        {
            if (!IsFeatureAvailable) return;
            
            VoxelBusters.EssentialKit.GameServicesLoadAchievementDescriptionsResult result = null;
            VoxelBusters.CoreLibrary.Error error = null;
            
            GameServices.LoadAchievementDescriptions(callback: (_result, _error) =>
            {
                result = _result;
                error = _error;
            });

            while ( !(result != null || error != null) )
            {
                await Task.Delay(25);
            }
            
            if (error == null)
            {
                IAchievementDescription[] items = result.AchievementDescriptions;
                _achievementInfos.Clear();
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    var achievementInfo = new AchievementInfo();
                    achievementInfo.id = item.Id;
                    achievementInfo.platformId = item.PlatformId;
                    achievementInfo.title = item.Title;
                    achievementInfo.unachievedDescription = item.UnachievedDescription;
                    achievementInfo.achievedDescription = item.AchievedDescription;
                    achievementInfo.maximumPoints = item.MaximumPoints;
                    achievementInfo.totalSteps = item.NumberOfStepsRequiredToUnlockAchievement;
                    achievementInfo.hidden = item.IsHidden;
                    achievementInfo.replayable = item.IsReplayable;
                    _achievementInfos[achievementInfo.id] = achievementInfo;
                }
                Log.Info($"서버에서 업적정보들을 가져왔습니다. (count: {_achievementInfos.Count})");
            }
            else
            {
                Log.Warning($"서버에서 업적정보들을 가져올 수 없습니다. ({error.ToString()})");
                // _achievementInfos.Clear();
            }
        }

        /// <summary>
        /// 업적 진척도 업데이트
        /// </summary>
        /// <param name="achievementId">업적 Id (플랫폼 독립적 내부 Id)</param>
        /// <param name="percent">업적 달성율 (0 ~ 100)</param>
        public void ReportAchievementPercent(string achievementId, double percentCompleted)
        {
            UnityEngine.Debug.Assert(percentCompleted is > 0 and <= 100, $"보고할 업적 진척도는 0초과 및 100이하여야 합니다. ({percentCompleted})");
            
            GameServices.ReportAchievementProgress(achievementId, percentCompleted, (Error error) =>
            {
                if (error == null)
                {
                    Log.Info($"Report AchievementProgress Success (id:{achievementId}, percent:{percentCompleted}%)");
                    OnAchievementReported?.Invoke(achievementId, percentCompleted);
                    if (percentCompleted >= 100) OnAchievementCompleted?.Invoke(achievementId);
                    UpdateAchievementsAsync(); // 서버에 업적을 리포팅했으면 로컬상태도 업데이트 한다.                    
                }
                else
                {
                    Log.Warning($"Report AchievementProgress Failed (id:{achievementId}, percent:{percentCompleted}%) => {error}");
                }
            });
        }

        /// <summary>
        /// 업적 단계 업데이트
        /// </summary>
        /// <param name="achievementId">업적 Id (플랫폼 독립적 내부 Id)</param>
        /// <param name="steps">업적 단계 (1 ~ AchivementInfo.totalSteps)</param>
        public void ReportAchievementSteps(string achievementId, int steps)
        {;
            AchievementInfo achievementInfo;
            double percent;
            
            if (_achievementInfos.TryGetValue(achievementId, out achievementInfo))
            {
                percent = (steps / (achievementInfo.totalSteps * 1.0)) * 100.0;
                ReportAchievementPercent(achievementId, percent);
            }
            else
            {
                Log.Warning($"Report AchievementProgress Failed (id:{achievementId}, steps:{steps}, err: InvalidAchievementId)");
            }
        }

        [ContextMenu(nameof(ShowAchievementsUI))]
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
        
        [ContextMenu(nameof(LogStatus))]
        public void LogStatus()
        {
            Log.Info("AchievementManager Status:");
            Log.Info($"  FeatureAvailable: {IsFeatureAvailable}");
        }
        
        [ContextMenu(nameof(LogLocalAchievements))]
        public void LogLocalAchievements()
        {
            Log.Info("Achievements (local):");
            foreach (var item in _achievements.Values)
            {
                Log.Info($"* {item.id} (Achievement)");
                Log.Info($"  id: {item.id}");
                Log.Info($"  platformId: {item.platformId}");
                Log.Info($"  lastReportedDate: {item.lastReportedDate}");
                Log.Info($"  completed: {item.completed}");
                Log.Info($"  percentage: {item.percentage}");
            }
        }

        [ContextMenu(nameof(LogLocalAchievementInfos))]
        public void LogLocalAchievementInfos()
        {
            Log.Info("AchievementInfos (local):");
            foreach (var item in _achievementInfos.Values)
            {
                Log.Info($"* {item.id} (AchievementInfo)");
                Log.Info($"  id: {item.id}");
                Log.Info($"  platformId: {item.platformId}");
                Log.Info($"  title: {item.title}");
                Log.Info($"  unachievedDescription: {item.unachievedDescription}");
                Log.Info($"  achievedDescription: {item.achievedDescription}");
                Log.Info($"  maximumPoints: {item.maximumPoints}");
                Log.Info($"  numberOfStepsRequiredToUnlockAchievement: {item.totalSteps}");
                Log.Info($"  hidden: {item.hidden}");
                Log.Info($"  replayable: {item.replayable}");
            }
        }
    }
}