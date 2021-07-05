// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rano.Addressable;

namespace Rano
{
    [AddComponentMenu("Rano/LoadingManager")]
    [RequireComponent(typeof(Fader))]
    public partial class LoadingManager : MonoSingleton<LoadingManager>
    {
        public enum Status
        {
            None,
            Downloading,
            DownloadCompleted,
            DownloadFailed,
            LoadingScene,
            LoadingSceneCompleted,
            LoadingSceneFailed,
            LoadingAsset,
            LoadingAssetCompleted,
            LoadingAssetFailed,
            UnloadingScene,
            UnloadingSceneCompleted,
            UnloadingSceneFailed,
            UnloadingAsset,
            UnloadingAssetCompleted,
            UnloadingAssetFailed
        }

        Queue<Command> _queue;
        Fader _fader;
        public Status status {get; private set;}

        void Awake()
        {
            status = Status.None;
            _queue = new Queue<Command>();
            _fader = GetComponent<Fader>();
            if (_fader == null) throw new LoadingManagerException("Fader 컴포넌트가 없음.");
        }

        void OnEnable()
        {
            Log.Info("LoadingManager Enabled");
            StartCoroutine(nameof(CoUpdate));
        }

        void OnDisable()
        {
            Log.Info("LoadingManager Disabled");
            StopCoroutine(nameof(CoUpdate));
        }

        void Add(Command command)
        {
            _queue.Enqueue(command);
        }

        IEnumerator CoUpdate()
        {
            while (true)
            {
                if (_queue.Count > 0)
                {
                    Command command = _queue.Dequeue();

                    switch (command.type)
                    {
                        case Command.Type.AddScene:
                            status = Status.LoadingScene;
                            yield return Rano.Addressable.SceneManager.Instance.AddSceneAsync(command.address);
                            // 마지막으로 Add된 Scene이 Instatiable Active Scene이 됨.
                            Rano.Addressable.SceneManager.Instance.ActivateScene(command.address);
                            status = Status.LoadingSceneCompleted;
                            break;

                        case Command.Type.RemoveScene:
                            status = Status.UnloadingScene;
                            yield return Rano.Addressable.SceneManager.Instance.RemoveSceneAsync(command.address);
                            status = Status.UnloadingSceneCompleted;
                            break;

                        case Command.Type.ActiveScene:
                            Rano.Addressable.SceneManager.Instance.ActivateScene(command.address);
                            break;

                        case Command.Type.FadeOut:
                            yield return _fader.FadeOut(command.fadeSpeed);
                            break;

                        case Command.Type.FadeIn:
                            yield return _fader.FadeIn(command.fadeSpeed);
                            break;

                        case Command.Type.ShowText:
                            yield return _fader.ShowText(command.fadeSpeed);
                            break;

                        case Command.Type.HideText:
                            yield return _fader.HideText(command.fadeSpeed);
                            break;
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// 씬을 추가한다(로드)
        /// </summary>
        public void QueueAddScene(Address address)
        {
            Command command = new Command(Command.Type.AddScene, address);
            this.Add(command);
        }

        public void QueueRemoveScene(Address address)
        {
            Command command = new Command(Command.Type.RemoveScene, address);
            this.Add(command);
        }

        public void QueueActiveScene(Address address)
        {
            Command command = new Command(Command.Type.ActiveScene, address);
            this.Add(command);
        }

        public void QueueFadeOut(float speed)
        {
            Command command = new Command(Command.Type.FadeOut, speed);
            this.Add(command);
        }

        public void QueueFadeIn(float speed)
        {
            Command command = new Command(Command.Type.FadeIn, speed);
            this.Add(command);
        }

        public void QueueShowText(float speed)
        {
            Command command = new Command(Command.Type.ShowText, speed);
            this.Add(command);
        }

        public void QueueHideText(float speed)
        {
            Command command = new Command(Command.Type.HideText, speed);
            this.Add(command);
        }

        // IEnumerator CoUpdate()
        // {
        //     while (true)
        //     {
        //         string statusText;
        //         string progressDot = new String('.', (int)(Time.realtimeSinceStartup % 4));

        //         switch (status)
        //         {
        //             case Status.LoadingSceneFailed:
        //             case Status.LoadingAssetFailed:
        //             case Status.UnloadingSceneFailed:
        //             case Status.UnloadingAssetFailed:
        //                 statusText = "Loading Failed";
        //                 break;
        //             case Status.Downloading:
        //                 statusText = $"Downloading{progressDot} {percent.ToPercentString()}";
        //                 break;
        //             case Status.DownloadFailed:
        //                 statusText = "Download Failed";
        //                 break;
        //             default:
        //                 statusText = $"Loading{progressDot}";
        //                 break;
        //         }
        //         _fader.text.text = statusText;

        //         yield return new WaitForSeconds(0.25f);
        //     }
        // }
    }
}