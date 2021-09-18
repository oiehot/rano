// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Rano;
using Rano.Store;

namespace Rano.App
{
    /// <summary>앱이 최신버젼인지 알아내고 업데이트가 필요하면 이벤트를 발생시킨다.</summary>
    [AddComponentMenu("Rano/App/Update Checker")]
    public class UpdateChecker : MonoBehaviour
    {
        Appstore appStore;
        [SerializeField] string bundleId;

        [ReadOnly] public RuntimePlatform currentPlatform;
        [ReadOnly] public Version currentVersion;
        [ReadOnly] public Version? lastestVersion;
        public UnityEvent onUpdateRequired;
        public UnityEvent onUpdateCheckFailed;

        void OnValidate()
        {
        }

        void Awake()
        {
            lastestVersion = null;
            Initialize();
        }

        /// <summary>초기화</summary>
        public void Initialize(string bundleId=null, RuntimePlatform? platform=null, Version? version=null)
        {
            this.bundleId = bundleId ?? Application.identifier;
            currentPlatform = platform ?? Application.platform;
            currentVersion = version ?? new Version(Application.version);

            switch (currentPlatform)
            {
                case RuntimePlatform.IPhonePlayer:
                    appStore = new AppleAppstore();
                    break;

                case RuntimePlatform.Android:
                    appStore = new GoogleAppstore();
                    break;
                
                default:
                    appStore = null;
                    break;
            }            
        }

        /// <summary>스토어로 부터 최신버젼값을 얻어 저장한다. 실패하면 콜백를 호출한다.</summary>
        public IEnumerator UpdateLastestVersion()
        {
            if (appStore != null)
            {
                yield return appStore.GetVersion(bundleId, (Result result, Version? version) => {
                    switch (result)
                    {
                        case Result.Success:
                            lastestVersion = version;
                            break;
                        case Result.ParsingError:
                            Log.Warning("앱스토어의 앱페이지에서 버젼정보를 파싱하는데 실패했습니다.");
                            onUpdateCheckFailed.Invoke();
                            break;
                        case Result.ConnectionError:
                            Log.Warning("앱스토어에 연결하는데 실패했습니다.");
                            onUpdateCheckFailed.Invoke();
                            break;
                    }
                });
            }
            else
            {
                Log.Warning("현재 플랫폼에 해당되는 앱스토어가 없으므로 앱버젼정보를 가져올 수 없습니다.");
            }
        }

        /// <summary>최신버젼값을 얻고 현재버젼과 비교하여 업데이트가 필요하면 콜백을 호출한다.</summary>
        public IEnumerator CheckUpdate()
        {
            yield return UpdateLastestVersion();

            if (lastestVersion.HasValue)
            {
                if (currentVersion < lastestVersion)
                {
                    Log.Warning($"앱 업데이트가 필요합니다. ({currentVersion} => {lastestVersion})");
                    onUpdateRequired.Invoke();
                }
                else
                {
                    Log.Important($"앱이 최신버젼입니다. ({currentVersion})");
                }
            }
            else
            {
                Log.Warning($"앱이 최신버젼인지 알 수 없습니다. ({currentVersion})");
            }
        }
    }
}