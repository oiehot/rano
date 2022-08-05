using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.Services.Gaming
{
    public sealed class LeaderboardManager : BaseComponent
    {
        public bool IsFeatureAvailable => GameServices.IsAvailable() && GameServices.IsAuthenticated;

        /// <summary>
        /// 리더보드에 점수를 업데이트한다.
        /// </summary>
        public void ReportScore(string leaderboardId, long score)
        {
            GameServices.ReportScore(leaderboardId, score, (Error error) =>
            {
                if (error == null)
                {
                    Log.Info($"리더보드({leaderboardId})에 점수({score}) 업데이트 성공");
                }
                else
                {
                    Log.Warning($"리더보드({leaderboardId})에 점수 업데이트 실패 ({error.Description})");
                }
            });
        }

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
            GameServices.ShowLeaderboards(timeScope.ToVoxelBusterEnum(), (GameServicesViewResult result, Error error) =>
            {
                switch (result.ResultCode)
                {
                    case GameServicesViewResultCode.Done:
                        Log.Info("리더보드가 성공적으로 닫힘");
                        break;
                    case GameServicesViewResultCode.Unknown:
                        Log.Info("리더보드의 닫힘상태를 알 수 없음");
                        break;
                }
            });
        }

        /// <summary>
        /// 특정 리더보드 출력
        /// </summary>
        public void ShowLeaderboard(string leaderboardId, TimeScope timeScope = TimeScope.AllTime)
        {
            GameServices.ShowLeaderboard(leaderboardId, timeScope.ToVoxelBusterEnum(), (GameServicesViewResult result, Error error) =>
            {
                switch (result.ResultCode)
                {
                    case GameServicesViewResultCode.Done:
                        Log.Info("리더보드가 성공적으로 닫힘");
                        break;
                    case GameServicesViewResultCode.Unknown:
                        Log.Info("리더보드의 닫힘상태를 알 수 없음");
                        break;
                }
            });
        }

        [ContextMenu(nameof(LogStatus))]
        public void LogStatus()
        {
            Log.Info("LeaderboardManager Status:");
            Log.Info($"  FeatureAvailable: {IsFeatureAvailable}");
            GameServices.LoadLeaderboards((GameServicesLoadLeaderboardsResult result, Error error) => {
                if (error == null)
                {
                    ILeaderboard[] items = result.Leaderboards;
                    Log.Info($"  Total Leaderboards ({items.Length})");
                    for (int i = 0; i < items.Length; i++)
                    {
                        ILeaderboard item = items[i];
                        Log.Info($"    [{i}] {item}");
                    }
                }
                else
                {
                    Log.Warning($"Load leaderboard failed ({error.Description})");
                }
            });
        }
    }
}