// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Rano;

namespace Rano.PlatformServices.Gaming
{
    public sealed class LeaderboardManager : MonoSingleton<LeaderboardManager>
    {
        public bool IsFeatureAvailable => GameServices.IsAvailable() && GameServices.IsAuthenticated;
        public Action OnShowFailed { get; set; }

        /// <summary>
        /// 전체시간대의 모든 리더보드 출력
        /// </summary>
        [ContextMenu("Show Leaderboards (AllTime)")]
        public void ShowLeaderboards()
        {
            ShowLeaderboards(TimeScope.AllTime);
        }

        /// <summary>
        /// 특정 시간범위의 모든 리더보드 출력
        /// </summary>
        public void ShowLeaderboards(TimeScope timeScope)
        {
            if (IsFeatureAvailable == false)
            {
                Log.Warning($"게임서비스를 사용할 수 없기 때문에 리더보드창을 띄울 수 없음");
                return;
            }

            GameServices.ShowLeaderboards(timeScope.ToVoxelBusterEnum(), (GameServicesViewResult result, Error error) =>
            {
                if (error != null)
                {
                    Log.Warning($"리더보드 출력 실패 ({error.ToString()})");
                    OnShowFailed?.Invoke();
                    return;
                }

                switch (result.ResultCode)
                {
                    case GameServicesViewResultCode.Done:
                        Log.Info("리더보드가 성공적으로 닫힘");
                        break;
                    case GameServicesViewResultCode.Unknown:
                        Log.Warning("리더보드의 닫힘상태를 알 수 없음");
                        break;
                }
            });
        }

        /// <summary>
        /// 특정 리더보드 출력
        /// </summary>
        public void ShowLeaderboard(string leaderboardId, TimeScope timeScope = TimeScope.AllTime)
        {
            if (IsFeatureAvailable == false)
            {
                Log.Warning($"게임서비스를 사용할 수 없기 때문에 리더보드창을 띄울 수 없음");
                return;
            }
            GameServices.ShowLeaderboard(leaderboardId, timeScope.ToVoxelBusterEnum(), (GameServicesViewResult result, Error error) =>
            {
                if (error != null)
                {
                    Log.Warning($"리더보드 출력 실패 ({error.ToString()})");
                    OnShowFailed?.Invoke();
                    return;
                }

                switch (result.ResultCode)
                {
                    case GameServicesViewResultCode.Done:
                        Log.Info("리더보드가 성공적으로 닫힘");
                        break;
                    case GameServicesViewResultCode.Unknown:
                        Log.Warning("리더보드의 닫힘상태를 알 수 없음");
                        break;
                }
            });
        }
    }
}