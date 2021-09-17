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
        Appstore appstore;
        string appID;

        public string appleStoreAppID;
        [ReadOnly] public RuntimePlatform currentPlatform; // TODO: [ShowOnly] or [ReadOnly]
        [ReadOnly] public Version currentVersion;
        [ReadOnly] public Version? lastestVersion;
        public UnityEvent onUpdateRequired;
        public UnityEvent onUpdateCheckFailed;

        void OnValidate()
        {
            Debug.Assert(appleStoreAppID != null && appleStoreAppID != "", "애플 앱스토어에서 사용하는 앱ID를 지정해주세요. 예) 1350067922");
        }

        void Awake()
        {
            lastestVersion = null;            
            currentPlatform = Application.platform;
            currentVersion = new Version(Application.version);

            switch (currentPlatform)
            {
                case RuntimePlatform.IPhonePlayer:
                    appstore = new AppleAppstore();
                    appID = appleStoreAppID;
                    break;

                case RuntimePlatform.Android:
                    appstore = new GoogleAppstore();
                    appID = Application.identifier;
                    break;
                
                default:
                    appstore = null;
                    appID = null;
                    break;
            }
        }

        IEnumerator Start()
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

        IEnumerator UpdateLastestVersion()
        {
            if (appstore != null)
            {
                yield return appstore.GetVersion(appID, (Result result, Version? version) => {
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
    }
}