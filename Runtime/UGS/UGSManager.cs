#nullable enable

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    #define ENABLE_UNITY_SERVICES_CORE_VERBOSE_LOGGING
#endif

using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace Rano.UGS
{
    public class UGSManager : ManagerComponent
    {
        private enum EState
        {
            None = 0,
            Initializing,
            Initialized,
        }

        private EState State => UnityServices.State switch
        {
            ServicesInitializationState.Uninitialized => EState.None,
            ServicesInitializationState.Initializing => EState.Initializing,
            ServicesInitializationState.Initialized => EState.Initialized,
            _ => EState.None,
        };
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const string ENVIRONMENT_NAME = "development";
#else
        private const string ENVIRONMENT_NAME = "production";
#endif

        public bool IsInitialized => State >= EState.Initialized;
        
        public async Task InitializeAsync()
        {
            Log.Info($"UGS 초기화 중... ({ENVIRONMENT_NAME})");
            try
            {
                InitializationOptions options = new InitializationOptions();
                options.SetEnvironmentName(ENVIRONMENT_NAME);
                await UnityServices.InitializeAsync(options);
            }
            catch (Exception e)
            {
                Log.Warning($"UGS 초기화 중 예외 발생");
                Log.Exception(e);
                return;
            }

            if (IsInitialized)
            {
                Log.Info($"UGS 초기화 완료 ({ENVIRONMENT_NAME})");
            }
            else
            {
                Log.Warning($"UGS 초기화 실패 (originalState:{UnityServices.State}, state:{State})");
            }
        }
    }
}