#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace Rano.RemoteConfig.Firebase
{
    public sealed partial class FirebaseRemoteConfigManager : ManagerComponent, IRemoteConfigManager
    {
        private const int AUTO_FETCH_PERIOD = 60 * 1000; // ms
        private CancellationTokenSource? _updateCancelTokenSource;

        
        /// <summary>
        /// 자동 Fetch를 시작한다.
        /// </summary>
        /// <remarks>
        /// Fetch가 가능한 상황이면 바로 Fetch받는다.
        /// </remarks>
        private async void StartAutoFetchAsync()
        {
            _updateCancelTokenSource = new CancellationTokenSource();
            CancellationToken token = _updateCancelTokenSource.Token;
            
            Log.Info($"자동 Fetch를 시작합니다 (period: {AUTO_FETCH_PERIOD}ms)");
            
            while (true)
            {
                if (token.IsCancellationRequested == true)
                {
                    Log.Info("토큰에 의해 AutoFetch를 취소합니다 (in while loop)");
                    break;
                }

                // 지금 Fetch가 가능한지 확인하고 Fetch합니다.
                if (IsFetchable() == true)
                {
                    bool fetchResult = await FetchAsync();
                    
                    // Fetch에 성공하면 즉시 적용합니다 (활성화, Activate)
                    if (fetchResult == true)
                    {
                        bool activateResult = await ActivateAsync();
                    }
                }

                // 잠시 뒤에 다시 Fetch 합니다.
                try
                {
                    await Task.Delay(AUTO_FETCH_PERIOD, token);
                }
                catch (TaskCanceledException)
                {
                    Log.Info("토큰에 의해 AutoFetch를 취소합니다 (in task delay)");
                    break;
                }
            }
        }
        
        /// <summary>
        /// 자동 Fetch를 종료한다.
        /// </summary>
        private void StopAutoFetch()
        {
            if (_updateCancelTokenSource == null)
            {
                Log.Info("자동 Fetch 중지 실패 (시작되지 않았음)");
                return;
            }
            
            if (_updateCancelTokenSource.IsCancellationRequested == true)
            {
                Log.Info("자동 Fetch 중지 실패 (이미 취소했음)");
                return;
            }
            
            Log.Info("자동 Fetch를 취소합니다");
            _updateCancelTokenSource.Cancel();
        }

        /// <summary>
        /// Fetch가 가능한 상황인지 체크한다.
        /// </summary>
        /// <remarks>
        /// Fetch가 가능한 상황은 다음 중 하나에 해당 될 때이다: 
        /// 1) 한번도 Fetch되지 않았을 때.
        /// 2) 마지막으로 Fetch한 시간에서 최소 FetchInterval시간이 지났을 때.
        /// </remarks>
        private bool IsFetchable()
        {
            // 초기화되지 않았으면 Fetch할 수 없다.
            if (IsInitialized == false) return false;
            Debug.Assert(_remoteConfig != null);

            ConfigInfo? info;
            try
            {
                info = _remoteConfig!.Info;
            }
            catch
            {
                // ConfigInfo를 얻는 중 예외가 발생하면 한번도 Fetch되지 않은 상황이라고 판단한다.
                // 이 경우는 Fetch할 수 있다.
                return true;
            }
            
            // ConfigInfo가 비어 있으면 한번도 Fetch되지 않은 상태이므로 Fetch할 수 있다.
            if (info == null) return true;

            // 마지막으로 Fetch된 시간과 최소 Fetch간격(ms)을 파악한다.
            DateTime latestFetchDateTime;
            TimeSpan minimumFetchInterval;
            try
            {
                latestFetchDateTime = info.FetchTime.ToUniversalTime(); // UTC
            }
            catch
            {
                // 마지막 Fetch시간을 얻는 중 예외가 발생하면 한번도 Fetch되지 않은 상황이라고 판단한다.
                return true;
            }

            try
            {
                minimumFetchInterval = new TimeSpan(0, 0, 0, 0, milliseconds: (int)_remoteConfig!.ConfigSettings.MinimumFetchInternalInMilliseconds);
            }
            catch
            {
                // 최소 Fetch간격을 얻는 중 에외가 발생하면 한번도 Fetch되지 않은 상황이라고 판단한다.
                return true;
            }

            // 마지막으로 Fetch된 시간에서 최소 Interval 시간이 경과됐다면, Fetch가 가능하다고 판단한다.
            if (DateTime.UtcNow >= (latestFetchDateTime + minimumFetchInterval))
            {
                return true;
            }

            // 마지막으로 Fetch된 시간으로 최소 Interval 시간이 경과되지 않았으므로 Fetch가 불가능하다.
            return false;
        }
    } 
}