// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if UNITY_IOS

using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Rano
{
    public class SocialManager : Singleton<SocialManager>, ISocialManager
    {
        void OnEnable()
        {
            Log.Important("GameCenter Enable");
            GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
        }

        void OnDisable()
        {
            Log.Info("GameCenter Disable");
        }

        public void SignIn()
        {
            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                    Log.Info("GameCenter SignIn Success");
                }
                else
                {
                    Log.Error("GameCenter SignIn Failed");
                }
            });
        }

        public void SignOut()
        {
            Log.Warning("GameCenter SignOut was Not Implemented");
        }

        public bool IsSigned
        {
            get
            {
                return Social.localUser.authenticated;
            }
        }

        public void ShowAchievementUI()
        {
            if (IsSigned)
            {
                Social.ShowAchievementsUI();
            }
            else
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    if (success)
                    {
                        Social.ShowAchievementsUI();
                    }
                    else
                    {
                        Log.Error("GameCenter ShowAchievementsUI Failed");
                    }
                });
            }
        }

        public void UnlockAchievement(string achievementID, float percent)
        {
            Social.ReportProgress(achievementID, percent, null); // null callback
            Log.Info($"GameCenter UnlockAchievement [{achievementID}, {percent}] Requested");
        }
    
        public void ShowLeaderboardUI(string leaderboardID)
        {
            if (IsSigned)
            {
                // AllTime - 전체 기간 순위표
                // Today - 오늘 순위표
                // Week - 이번 주 순위표
                GameCenterPlatform.ShowLeaderboardUI(leaderboardID, UnityEnginePlatforms.TimeScope.AllTime);
            }
            else
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    if (success)
                    {
                        GameCenterPlatform.ShowLeaderboardUI(leaderboardID, UnityEnginePlatforms.TimeScope.AllTime);
                    }
                    else
                    {
                        Log.Error("GameCenter ShowLeaderboardUI Failed");
                    }
                });
            }            
        }

        public void ReportScore(long score, string leaderboardID)
        {
            // ReportScore() 내부에서 플레이어의 기록된 점수와 Parameter로 날아온 점수의 높낮이를 비교하고 필터링함. 낮은 점수일 경우의 예외처리를 할 필요없다.
            Social.ReportScore(score, leaderboardID, (bool success) => {
                if (success)
                {
                    Log.Info($"GameCenter ReportScore [{leaderboardID}, {score}] Success");
                }
                else
                {
                    Log.Error($"GameCenter ReportScore [{leaderboardID}, {score}] Failed");
                }
            });
        }
    }
}

#endif