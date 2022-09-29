#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Rano;
using Firebase.RemoteConfig;

namespace Rano.RemoteConfig.Firebase
{
    /// <summary>
    /// Firebase RemoteConfig 전반을 관리한다.
    /// </summary>
    /// <remarks>
    /// Fetch는 서버에서 값들을 가져온다.
    /// Activate는 가져온 값들을 실제로 적용한다.
    /// Fetch는 기본 12시간 기준으로 유지되며 여러번 요청해도 더 가져오지 않는다.
    /// </remarks>
    /// <seealso href="https://jslee-tech.tistory.com/23" alt="실행 타이밍 전략"/>
    /// <seealso href="https://fsd-jinss.tistory.com/152" alt="실시간 값이 필요하면 RealtimeDatabase를 사용 할 것은 추천"/>
    /// <seaalso href="https://medium.com/harrythegreat/android-remote-config-%EC%9E%98-%ED%99%9C%EC%9A%A9%ED%95%98%EA%B8%B0-f8b04ef2645a" alt="FCM 사용 제안" />
    public sealed class FirebaseRemoteConfigManager : ManagerComponent, IRemoteConfigManager
    {
        private const int AUTO_FETCH_PERIOD = 6000;
        private CancellationTokenSource? _updateCancelTokenSource;
        
        private FirebaseRemoteConfig? _remoteConfig;
        public bool IsInitialized => _remoteConfig != null;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsInitialized) StartAutoFetchAsync();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAutoFetch();
        }

        private async void StartAutoFetchAsync()
        {
            _updateCancelTokenSource = new CancellationTokenSource();
            CancellationToken token = _updateCancelTokenSource.Token;
            
            Log.Info($"자동 Fetch를 시작합니다 (period: {AUTO_FETCH_PERIOD}ms)");
            
            while (true)
            {
                if (token.IsCancellationRequested == true)
                {
                    break;
                }
                
                bool fetchResult = await FetchAsync();
                if (fetchResult == true)
                {
                    bool activateResult = await ActivateAsync();    
                }
                
                await Task.Delay(AUTO_FETCH_PERIOD, token);
            }
        }
        
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
        /// 초기화
        /// </summary>
        /// <param name="defaults">인앱 기본값들, 클라우드에 설정된 기본값이 아님</param>
        /// <remarks>Fetch와 Activate를 하지 않는다.</remarks>
        public async Task Initialize(Dictionary<string, object>? defaults=null)
        {
            Log.Info("초기화 중...");
            
            _remoteConfig = FirebaseRemoteConfig.DefaultInstance;

            // 디버용 설정
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ConfigSettings configSettings = new ConfigSettings
            {
                FetchTimeoutInMilliseconds = 3 * 1000, // default: 30,000 (30 seconds)
                MinimumFetchInternalInMilliseconds = 0 // default: 43,200,000 = 12*60*60*1000 (12hours)
            };
            
            Log.Info("설정:");
            Log.Info($"  Fetch 타임아웃 (ms): {configSettings.FetchTimeoutInMilliseconds.ToCommaString()}");
            Log.Info($"  최소 Fetch 간격 (ms): {configSettings.MinimumFetchInternalInMilliseconds.ToCommaString()}");
            
            Log.Info("설정 적용 중...");
            try
            {
                await _remoteConfig.SetConfigSettingsAsync(configSettings);
            }
            catch (Exception e)
            {
                Log.Warning("설정 적용 실패 (예외 발생)");
                Log.Exception(e);
            }
            Log.Info("설정 적용 완료");
#endif

            // 기본값 설정
            if (defaults != null)
            {
                Log.Info("기본값 적용 중...");
                try
                {
                    await _remoteConfig.SetDefaultsAsync(defaults);
                }
                catch (Exception e)
                {
                    Log.Warning("기본값 적용 실패 (예외 발생)");
                    Log.Exception(e);
                }
            }
            
            Log.Info("초기화 완료");
        }

        /// <summary>
        /// 데이터 가져오기
        /// </summary>
        /// <remarks>활성화(Activate)해야 적용된다.</remarks>
        public async Task<bool> FetchAsync()
        {
            Log.Info("데이터 가져오는 중...");
        
            if (IsInitialized == false)
            {
                Log.Warning("데이터 가져오기 실패 (초기화가 안되어 있음)");
                return false;
            }

            // Fetch 실행
            
            try
            {
                await _remoteConfig!.FetchAsync();    
            }
            catch (Exception e)
            {
                Log.Warning("데이터 가져오기 생략 (예외 발생)");
                Log.Exception(e);
                return false;
            }

            // Fetch 결과 체크
            
            ConfigInfo? info = _remoteConfig!.Info;
            if (_remoteConfig!.Info == null)
            {
                Log.Warning("데이터 가져오기 실패 (결과가 비어있음)");
                return false;
            }
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Log.Info("FetchResult:");
            Log.Info($"    FetchTime(LocalTime): {info.FetchTime.ToLocalTime()}");
            Log.Info($"    ThrottledEndTime(LocalTime): {info.ThrottledEndTime.ToLocalTime()}");            
            Log.Info($"    LastFetchStatus: {info.LastFetchStatus}");
            Log.Info($"    LastFetchFailureReason: {info.LastFetchFailureReason}");
#endif

            // Fetch 결과에 따른 메시지 출력 및 결과값 리턴
            
            LastFetchStatus fetchStatus = info.LastFetchStatus;

            if (fetchStatus == LastFetchStatus.Success)
            {
                Log.Info("데이터 가져오기 성공");
                return true;
            }
            else if (fetchStatus == LastFetchStatus.Pending)
            {
                Log.Info("데이터 가져오기 실패 (아직 완료되지 않음)");
                return false;
            }
            else if (fetchStatus == LastFetchStatus.Failure)
            {
                Log.Warning($"데이터 가져오기 실패 ({info.LastFetchFailureReason})");
                return false;
            }
            else
            {
                Log.Warning($"데이터 가져오기 실패 (알 수 없는 원인)");
                return false;
            }
        }
        
        /// <summary>
        /// 가져온 데이터 활성화
        /// </summary>
        /// <remarks>Fetch 후에 실행해야 한다.</remarks>        
        public async Task<bool> ActivateAsync()
        {
            Log.Info("데이터 활성화 중...");

            if (IsInitialized == false)
            {
                Log.Warning("데이터 활성화 실패 (초기화가 안되어 있음)");
                return false;
            }
            
            bool result;
            try
            {
                result = await _remoteConfig!.ActivateAsync();
            }
            catch (Exception e)
            {
                Log.Info("데이터 활성화 생략 (예외 발생)");
                Log.Exception(e);
                return false;
            }

            if (result == true)
            {
                Log.Info("데이터 활성화 성공");
            }
            else
            {
                Log.Info("데이터 활성화 생략 (변경사항 없음)");
            }

            return result;
        }
        
        private bool TryGet<T>(string key, out T? value, Func<string,T> func)
        {
            if (IsInitialized == false)
            {
                Log.Warning($"값을 얻을 수 없음 (key:{key}, reason:초기화가 안되어 있음)");
                value = default(T);
                return false;
            }
            try
            {
                value = func(key);
            }
            catch (Exception e)
            {
                Log.Warning($"값을 얻을 수 없음 (key:{key}, reason:예외 발생)");
                Log.Exception(e);
                value = default(T);
                return false;
            }
            return true;
        }

        public bool TryGetString(string key, out string? value)
        {
            return TryGet<string>(key, out value, (k) => _remoteConfig!.GetValue(k).StringValue);
        }
        
        public bool TryGetDouble(string key, out double value)
        {
            return TryGet<double>(key, out value, (k) => _remoteConfig!.GetValue(k).DoubleValue);
        }

        public bool TryGetLong(string key, out long value)
        {
            return TryGet<long>(key, out value, (k) => _remoteConfig!.GetValue(k).LongValue);
        }

        public bool TryGetBool(string key, out bool value)
        {
            return TryGet<bool>(key, out value, (k) => _remoteConfig!.GetValue(k).BooleanValue);
        }

        public void LogStatus()
        {
            if (IsInitialized == false)
            {
                Log.Warning($"상태 출력 실패 (초기화가 안되어 있음)");
                return;
            }

            Log.Info($"{nameof(FirebaseRemoteConfigManager)}:");
            Log.Info($"  Instance: {_remoteConfig}");
            Log.Info($"  IsInitialized: {IsInitialized}");
            
            Log.Info($"  Settings:");
            Log.Info($"    FetchTimeoutInMilliseconds: {_remoteConfig!.ConfigSettings.FetchTimeoutInMilliseconds}");
            Log.Info($"    MinimumFetchInternalInMilliseconds: {_remoteConfig!.ConfigSettings.MinimumFetchInternalInMilliseconds}");

            if (_remoteConfig!.Info != null)
            {
                Log.Info("  Info:");
                Log.Info($"    FetchTime(LocalTime): {_remoteConfig!.Info.FetchTime.ToLocalTime()}");
                Log.Info($"    ThrottledEndTime(LocalTime): {_remoteConfig!.Info.ThrottledEndTime.ToLocalTime()}");
                Log.Info($"    LastFetchStatus: {_remoteConfig!.Info.LastFetchStatus}");
                Log.Info($"    LastFetchFailureReason: {_remoteConfig!.Info.LastFetchFailureReason}");
            }
            else
            {
                Log.Info("  Info: (null)");
            }

            IDictionary<string,ConfigValue>? allValues = _remoteConfig!.AllValues;
            if (allValues.Count > 0)
            {
                Log.Info($"  Keys:");
                foreach (KeyValuePair<string, ConfigValue> kv in allValues)
                {
                    Log.Info($"  - {kv.Key} (source:{kv.Value.Source})");
                }
            }
            else
            {
                Log.Info($"  Keys: (none)");
            }
        }
    } 
}