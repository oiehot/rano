#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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
    public sealed partial class FirebaseRemoteConfigManager : ManagerComponent, IRemoteConfigManager
    {
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

        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="defaults">인앱 기본값들, 클라우드에 설정된 기본값이 아님</param>
        /// <remarks>Fetch와 Activate를 하지 않는다.</remarks>
        public async Task Initialize(Dictionary<string, object>? defaults=null)
        {
            Log.Info("초기화 중...");
            
            _remoteConfig = FirebaseRemoteConfig.DefaultInstance;

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            await ApplyDevelopmentSettings();
#endif

            // 데이터 기본값 설정
            if (defaults != null)
            {
                Log.Info("기본 데이터 적용 중...");
                try
                {
                    await _remoteConfig.SetDefaultsAsync(defaults);
                }
                catch (Exception e)
                {
                    Log.Warning("기본 데이터 적용 실패 (예외 발생)");
                    Log.Exception(e);
                }
            }

            Log.Info("초기화 완료");
            
            Log.Info("최근 Fetch된 데이터 중 미반영된 데이터가 있으면 적용합니다.");
            await ActivateAsync();
            
            // 위 코드로 인해서 최근에 받았던 데이터가 적용된 상태다.
            // 하지만 이 데이터가 최신 데이터가 아닐수도 있으므로,
            // 자동 Fetch를 시작하고 Fetch가 가능한 상황이면 바로 Fetch받는다.
            Log.Info("최근 데이터는 적용 되었지만 서버 설정이 업데이트 되었을 수도 있습니다");
            Log.Info("따라서 주기적으로 최신 데이터를 얻기 위해 자동Fetch를 시작하고, 필요시 최신 데이터를 Fetch받습니다");
            Log.Info("최신 데이터를 Fetch 받는데 성공하면 즉시 적용(활성화, Activation) 됩니다.");
            StartAutoFetchAsync();
        }

        
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        private async Task ApplyDevelopmentSettings()
        {
            if (IsInitialized == false)
            {
                Log.Warning("개발자 설정 적용 실패 (초기화가 안되어 있음)");
                return;
            }
            
            Log.Info("개발용 설정 적용 중...");
            
            ConfigSettings configSettings = new ConfigSettings
            {
                FetchTimeoutInMilliseconds = 3 * 1000, // default: 30,000 (30 seconds)
                // MinimumFetchInternalInMilliseconds = 0 // default: 43,200,000 = 12*60*60*1000 (12hours)
                MinimumFetchInternalInMilliseconds = 5 * 60 * 1000
            };
            
            Log.Info("개발용 설정값:");
            Log.Info($"  Fetch 타임아웃 (ms): {configSettings.FetchTimeoutInMilliseconds.ToCommaString()}");
            Log.Info($"  최소 Fetch 간격 (ms): {configSettings.MinimumFetchInternalInMilliseconds.ToCommaString()}");

            try
            {
                await _remoteConfig!.SetConfigSettingsAsync(configSettings);
            }
            catch (Exception e)
            {
                Log.Warning("개발용 설정 적용 실패 (예외 발생)");
                Log.Exception(e);
            }
            Log.Info("개발용 설정 적용 완료");
        }
#endif

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
#if UNITY_EDITOR
            Log.Info("유니티 에디터 환경에서는 MinimumFetchInternalInMilliseconds 값이 너무 낮게 설정되어 있으면 Fetch가 실패합니다");
            Log.Info("정확한 테스트를 위해서 Android나 iOS 플랫픔으로 빌드해서 테스트하시기 바랍니다");
#endif
            try
            {
                await _remoteConfig!.FetchAsync();    
            }
            catch // (Exception e)
            {
                Log.Warning("데이터 가져오기 생략 (예외 발생)");
                // Log.Exception(e);
                return false;
            }

            // Fetch 결과 체크
            
            ConfigInfo? info = _remoteConfig!.Info;
            if (_remoteConfig!.Info == null)
            {
                Log.Warning("데이터 가져오기 실패 (결과가 비어있음)");
                return false;
            }
            
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Log.Info("데이터 가져오기 결과:");
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

            if (result)
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
                // TODO: FetchTime을 엑세스 하는 중 예외가 발생함을 확인함 (in android build)
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