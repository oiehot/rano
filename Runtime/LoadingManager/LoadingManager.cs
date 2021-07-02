// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine;

namespace Rano
{
    [AddComponentMenu("Rano/LoadingManager")]
    [RequireComponent(typeof(Fader))]
    public partial class LoadingManager : MonoSingleton<LoadingManager>
    {
        Fader fader;
        public Status status {get; private set;}
        public float percent {get; private set;}

        void Awake()
        {
            status = Status.None;
            percent = 0.0f;
            fader = GetComponent<Fader>();
            if (fader == null) throw new LoadingManagerException("Fader 컴포넌트가 없음");
        }

        void OnEnable()
        {
            Log.Info("LoadingManager Enabled");
            StopCoroutine(nameof(CoUpdate));
        }

        void OnDisable()
        {
            Log.Info("LoadingManager Disabled");
            StopCoroutine(nameof(CoUpdate));
        }

        IEnumerator CoUpdate()
        {
            while (true)
            {
                string statusText;
                string progressDot = new String('.', (int)(Time.realtimeSinceStartup % 4));
                
                switch (status)
                {
                    case Status.LoadingSceneFailed:
                    case Status.LoadingAssetFailed:
                    case Status.UnloadingSceneFailed:
                    case Status.UnloadingAssetFailed:
                        statusText = "Loading Failed";
                        break;
                    case Status.Downloading:
                        statusText = $"Downloading{progressDot} {percent.ToPercentString()}";
                        break;
                    case Status.DownloadFailed:
                        statusText = "Download Failed";
                        break;
                    default:
                        statusText = $"Loading{progressDot}";
                        break;
                }
                fader.text.text = statusText;

                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}