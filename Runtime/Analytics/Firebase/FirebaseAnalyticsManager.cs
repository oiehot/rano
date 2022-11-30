#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Analytics;
using Rano.Auth;

namespace Rano.Analytics.Firebase
{
    /// <summary>
    /// 분석 이벤트 로깅을 관리한다.
    /// </summary>
    /// <remarks>사용자ID를 인증ID로 사용한다.</remarks>
    public class FirebaseAnalyticsManager : ManagerComponent, IAnalyticsManager
    {
        private const string GUEST_USER_ID = "guest";
        private IAuthManager? _auth;

        public bool IsInitialized => _auth != null;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_auth != null)
            {
                _auth.OnSignIn += OnAuthUserChanged;
                _auth.OnSignOut += OnAuthUserChanged;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_auth != null)
            {
                _auth.OnSignIn -= OnAuthUserChanged;
                _auth.OnSignOut -= OnAuthUserChanged;
            }
        }

        private void OnAuthUserChanged()
        {
            if (_auth == null) return;
            
            // 인증이 안된 상태면 null값으로 설정되고 SetUserID에서 "guest"로 지정된다.
            if (string.IsNullOrEmpty(_auth.UserID))
            {
                SetUserID(GUEST_USER_ID);
            }
            else
            {
                SetUserID(_auth.UserID!);
            }
        }

        /// <summary>
        /// 초기화 한다.
        /// </summary>
        /// <remarks>사용자ID를 인증ID로 지정하기 위해서 인증괸리자를 사용한다.</remarks>
        public void Initialize(IAuthManager authManager)
        {
            Log.Info("초기화 중...");
            _auth = authManager;
            OnAuthUserChanged();
            Log.Info("초기화 완료");
#if UNITY_EDITOR
            Log.Warning("FirebaseAnalytics는 데스크톱이나 에디터 환경을 지원하지 않습니다");
#endif
        }

        private void SetUserID(string userID)
        {
            UnityEngine.Debug.Assert(string.IsNullOrEmpty(userID) == false);
            try
            {
                Log.Info($"로깅 이벤트 사용자ID를 설정 (userID:{userID})");
                FirebaseAnalytics.SetUserId(userID);
            }
            catch (Exception e)
            {
                Log.Warning($"로깅 이벤트 사용자ID 설정 실패 (예외발생, userID:{userID})");
                Log.Exception(e);
            }
        }

        public void SetUserProperty(string key, string value)
        {
            Log.Info($"사용자 프로퍼티 설정 (key:{key}, value:{value})");
            try
            {
                FirebaseAnalytics.SetUserProperty(key, value);
            }
            catch (Exception e)
            {
                Log.Warning($"사용자 프로퍼티 설정 실패 (예외발생, key:{key}, value:{value})");
                Log.Exception(e);
            }
        }

        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        public void LogEvent(string eventName)
        {
            Log.Info($"이벤트 로깅 (event:{eventName})");
            try
            {
                FirebaseAnalytics.LogEvent(eventName);
            }
            catch (Exception e)
            {
                Log.Warning($"이벤트 로깅 실패 (예외발생, event:{eventName})");
                Log.Exception(e);
            }
        }

        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        public void LogEvent(string eventName, string parameterName, string parameterValue)
        {
            Log.Info($"이벤트 로깅 (event:{eventName}, key:{parameterName}, value:{parameterValue})");
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
            }
            catch (Exception e)
            {
                Log.Warning($"이벤트 로깅 실패 (예외발생, event:{eventName}, key:{parameterName}, value:{parameterValue})");
                Log.Exception(e);
            }
        }
        
        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        public void LogEvent(string eventName, string parameterName, double parameterValue)
        {
            Log.Info($"이벤트 로깅 (event:{eventName}, key:{parameterName}, value:{parameterValue})");
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
            }
            catch (Exception e)
            {
                Log.Warning($"이벤트 로깅 실패 (예외발생, event:{eventName}, key:{parameterName}, value:{parameterValue})");
                Log.Exception(e);
            }
        }
        
        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        public void LogEvent(string eventName, string parameterName, long parameterValue)
        {
            Log.Info($"이벤트 로깅 (event:{eventName}, key:{parameterName}, value:{parameterValue})");
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
            }
            catch (Exception e)
            {
                Log.Warning($"이벤트 로깅 실패 (예외발생, event:{eventName}, key:{parameterName}, value:{parameterValue})");
                Log.Exception(e);
            }
        }

        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        public void LogEvent(string eventName, string parameterName, int parameterValue)
        {
            Log.Info($"이벤트 로깅 (event:{eventName}, key:{parameterName}, value:{parameterValue})");
            try
            {
                FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
            }
            catch (Exception e)
            {
                Log.Warning($"이벤트 로깅 실패 (예외발생, event:{eventName}, key:{parameterName}, value:{parameterValue})");
                Log.Exception(e);
            }
        }

        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        private void LogEventByParameters(string eventName, SParameter[] parameters)
        {
            Log.Info($"이벤트 로깅 (event:{eventName}, parametersCount:{parameters.Length})");
            Parameter[] internalParams = new Parameter[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                string parameterName = parameters[i].name;
                object parameterValue = parameters[i].value;
                try
                {
                    Type type = parameterValue.GetType();
                    if (type == typeof(int))
                    {
                        internalParams[i] = new Parameter(parameterName, (int)parameterValue);
                    }
                    else if (type == typeof(string))
                    {
                        internalParams[i] = new Parameter(parameterName, (string)parameterValue);
                    }
                    else if (type == typeof(double))
                    {
                        internalParams[i] = new Parameter(parameterName, (double)parameterValue);
                    }
                    else if (type == typeof(long))
                    {
                        internalParams[i] = new Parameter(parameterName, (long)parameterValue);
                    }
                    else if (type == typeof(bool))
                    {
                        internalParams[i] = new Parameter(parameterName, ((bool)parameterValue).ToString());
                    }
                    else
                    {
                        Log.Warning($"이벤트 로깅 실패 (지원하지 않는 파라미터 타입, event:{eventName}, key:{parameterName}, type:{type})");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Log.Warning($"이벤트 로깅 실패 (예외 발생, event:{eventName}, key:{parameterName}, parameterValue:{parameterValue})");
                    Log.Exception(e);
                    return;
                }
            }
            try
            {
                FirebaseAnalytics.LogEvent(eventName, internalParams);
            }
            catch (Exception e)
            {
                Log.Warning($"이벤트 로깅 실패 (예외발생, event:{eventName}, parametersCount:{parameters.Length})");
                Log.Exception(e);
            }
        }
        
        /// <summary>
        /// 이벤트를 로깅한다.
        /// </summary>
        /// <remarks>Firebase 유니티 플러그인 한계로 데스크톱이나 에디터 환경에서는 사용할 수 없다.</remarks>
        public void LogEvent(string eventName, Dictionary<string, object> parameterDictionary)
        {
            SParameter[] parameters = parameterDictionary.Select(kv => new SParameter(kv.Key, kv.Value)).ToArray();
            LogEventByParameters(eventName, parameters);
        }
    }
}