// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if (GPGS && UNITY_ANDROID)

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using GooglePlayGames; // PlayGamesPlatform
using GooglePlayGames.BasicApi; // PlayGamesClientConfiguration
using Rano;

namespace Rano
{
    public class SocialManager : Singleton<SocialManager>, ISocialManager
    {
        public UnityAction OnSignInSuccess;
        public UnityAction OnSignInFailed;
        private bool onSignInSuccess = false;
        private bool onSignInFailed = false;

        public bool IsSigned
        {
            get
            {
                return PlayGamesPlatform.Instance.IsAuthenticated();
            }
        }

        public void Init()
        {
            // TODO: 이미 Initialize 된 경우 생략하기
            Log.Info("GPGS Initialize");

            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                // .RequestServerAuthCode(false)
                // .RequestEmail()
                // .RequestIdToken()
                .Build();
            
            PlayGamesPlatform.InitializeInstance(config);

            #if DEVELOPMENT_BUILD
            PlayGamesPlatform.DebugLogEnabled = true;
            #else
            PlayGamesPlatform.DebugLogEnabled = false;
            #endif

            PlayGamesPlatform.Activate();
        }

        void Update()
        {
            if (this.onSignInSuccess == true)
            {
                Log.Info("GPGS SignIn Success");
                this.OnSignInSuccess();
                this.onSignInSuccess = false;
            }
            if (this.onSignInFailed == true)
            {
                Log.Warning("GPGS SignIn Failed");                
                this.OnSignInFailed();
                this.onSignInFailed = false;
            }
        }

        public void SignIn()
        {
            Log.Info("GPGS SignIn");

            if (this.IsSigned == false)
            {
                Log.Info("GPGS Try SignIn");
                Social.localUser.Authenticate((bool success) => {
                    // TODO: Check: 메인 스레드에서 실행되는것인가???
                    // TODO: 안전하게 메인 스레드에서 실행하도록 함.
                    if (success)
                    {
                        // this.OnSignInSuccess();
                        this.onSignInSuccess = true;
                    }
                    else
                    {
                        // this.OnSignInFailed();
                        this.onSignInFailed = true;
                    }
                });
            }
            else
            {
                Log.Info("GPGS Already Signed");
            }
        }

        public void SignOut()
        {
            Log.Info("GPGS SignOut");
            PlayGamesPlatform.Instance.SignOut();
        }

        public void ShowAchievementUI()
        {
            throw new NotImplementedException();
        }
        // {
        //     if (IsSigned)
        //     {
        //         Social.ShowAchievementsUI();
        //     }
        //     else
        //     {
        //         Social.localUser.Authenticate((bool success) =>
        //         {
        //             if (success)
        //             {
        //                 Social.ShowAchievementsUI();
        //             }
        //             else
        //             {
        //                 Log.Warning("GPGS ShowAchievementsUI Failed");
        //             }
        //         });
        //     }
        // }

        public void UnlockAchievement(string achievementID, float percent)
        {
            throw new NotImplementedException();
        }
        // {
        //     PlayGamesPlatform.Instance.ReportProgress(achievementID, percent, null); // null callback
        //     Log.Info($"GPGS UnlockAchievement [{achievementID}, {percent}] Requested");
        // }

        public void ShowLeaderboardUI(string iosLeaderboardID)
        {
            throw new NotImplementedException();
        }
        // {
        //     if (IsSigned)
        //     {
        //         PlayGamesPlatform.Instance.ShowLeaderboardUI();
        //     }
        //     else
        //     {
        //         Social.localUser.Authenticate((bool success) =>
        //         {
        //             if (success)
        //             {
        //                 PlayGamesPlatform.Instance.ShowLeaderboardUI();
        //             }
        //             else
        //             {
        //                 Log.Warning("GPGS ShowLeaderboardUI Failed");
        //             }
        //         });
        //     }            
        // }

        public void ReportScore(long score, string leaderboardID)
        {
            throw new NotImplementedException();
        }
        // {
        //     // ReportScore() 내부에서 플레이어의 기록된 점수와 Parameter로 날아온 점수의 높낮이를 비교하고 필터링함. 낮은 점수일 경우의 예외처리를 할 필요없다.
        //     PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, (bool success) =>
        //     {
        //         if (success)
        //         {
        //             Log.Info($"GPGS ReportScore [{leaderboardID}, {score}] Success");
        //         }
        //         else
        //         {
        //             Log.Warning($"GPGS ReportScore [{leaderboardID}, {score}] Failed");
        //         }
        //     });
        // }
    }
}

#endif