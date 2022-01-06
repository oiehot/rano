// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rano.Addressable;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

namespace Rano.LoadingSystem
{
    [DisallowMultipleComponent]
    // TODO: 코루틴이 아닌 async로 변경해야함.
    // TODO: 씬unload 실패시 예외처리.
    public sealed class LoadingManager : MonoSingleton<LoadingManager>
    {
        private Queue<LoadingManagerCommand> _queue;
        private LoadingManagerUI _ui;
        public LoadingManagerStatus Status { get; private set; }
        public LoadingManagerFadeStatus FadeStatus { get; private set; }

        protected override void Awake() 
        {
            base.Awake();
            _queue = new Queue<LoadingManagerCommand>();
            _ui = this.GetRequiredComponent<LoadingManagerUI>();
            Status = LoadingManagerStatus.None;
            FadeStatus = LoadingManagerFadeStatus.FadeOut;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(nameof(CoUpdate));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopCoroutine(nameof(CoUpdate));
        }

        IEnumerator CoUpdate()
        {
            while (true)
            {
                // TODO: _autoDisableCanvas이 켜져있으면: 특정 초 이상동안 Command가 없는 경우, 자동으로 Canvas를 Disable시킴.
                // TODO:                              Canvas가 Disable되어있는 상태에서 명령이 Dequeue되었다면, Canvas를 켬.
                if (_queue.Count > 0)
                {
                    LoadingManagerCommand command = _queue.Dequeue();

                    switch (command.type)
                    {
                        case LoadingManagerCommand.Type.AddScene:
                            Log.Info($"씬 추가 {command.address}");
                            Status = LoadingManagerStatus.LoadingScene;
                            yield return Rano.Addressable.AddressableSceneManager.Instance.AddSceneAsync(command.address);
                            // 마지막으로 Add된 Scene이 Instatiable Active Scene이 됨.
                            Rano.Addressable.AddressableSceneManager.Instance.ActivateScene(command.address);
                            Status = LoadingManagerStatus.LoadingSceneCompleted;
                            break;

                        case LoadingManagerCommand.Type.RemoveScene:
                            Log.Info($"씬 삭제 {command.address}");
                            Status = LoadingManagerStatus.UnloadingScene;
                            yield return Rano.Addressable.AddressableSceneManager.Instance.RemoveSceneAsync(command.address);
                            Status = LoadingManagerStatus.UnloadingSceneCompleted;
                            break;

                        case LoadingManagerCommand.Type.ActiveScene:
                            Log.Info($"씬 활성화 {command.address}");
                            Rano.Addressable.AddressableSceneManager.Instance.ActivateScene(command.address);
                            break;

                        case LoadingManagerCommand.Type.EnableUI:
                            Log.Info($"로딩UI 켜기");
                            _ui.EnableUI();
                            break;

                        case LoadingManagerCommand.Type.DisableUI:
                            Log.Info($"로딩UI 끄기");
                            _ui.DisableUI();
                            break;

                        case LoadingManagerCommand.Type.FadeOut:
                            Log.Info($"페이드 아웃");
                            yield return FadeOut(command.fadeSpeed);
                            break;

                        case LoadingManagerCommand.Type.FadeIn:
                            Log.Info($"페이드 인");
                            yield return FadeIn(command.fadeSpeed);
                            break;

                        case LoadingManagerCommand.Type.ShowBodyText:
                            Log.Info($"Body텍스트 보이기");
                            yield return ShowBodyText(command.fadeSpeed);
                            break;

                        case LoadingManagerCommand.Type.HideBodyText:
                            Log.Info($"Body텍스트 숨기기");
                            yield return HideBodyText(command.fadeSpeed);
                            break;
                    }
                }
                yield return null;
            }
        }

        public IEnumerator FadeOut(float speed)
        {
            // 이미 페이드아웃 되어 있다면 실행하지 않는다.
            if (FadeStatus == LoadingManagerFadeStatus.FadeOut)
            {
                Log.Warning("페이드아웃 생략: 이중 페이드아웃 실행");
                yield break;
            }
            yield return _ui.FadeOut(speed);
            FadeStatus = LoadingManagerFadeStatus.FadeOut;
        }

        public IEnumerator FadeIn(float speed)
        {
            // 이미 페이드인 되어 있다면 실행하지 않는다.
            if (FadeStatus == LoadingManagerFadeStatus.FadeIn)
            {
                Log.Warning("페이드인 생략: 이중 페이드인 실행");
                yield break;
            }

            yield return _ui.FadeIn(speed);
            FadeStatus = LoadingManagerFadeStatus.FadeIn;
        }
        
        public IEnumerator ShowBodyText(float speed)
        {
            yield return _ui.ShowBodyText(speed);
        }

        public IEnumerator HideBodyText(float speed)
        {
            yield return _ui.HideBodyText(speed);
        }

        private void AddCommand(LoadingManagerCommand command)
        {
            _queue.Enqueue(command);
        }

        public void QueueAddScene(Address address)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.AddScene, address);
            AddCommand(command);
        }

        public void QueueRemoveScene(Address address)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.RemoveScene, address);
            AddCommand(command);
        }

        public void QueueActiveScene(Address address)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.ActiveScene, address);
            AddCommand(command);
        }

        public void QueueFadeOut(float speed=1.0f)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.FadeOut, speed);
            AddCommand(command);
        }

        public void QueueFadeIn(float speed=1.0f)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.FadeIn, speed);
            AddCommand(command);
        }

        public void QueueShowBodyText(float speed=1.0f)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.ShowBodyText, speed);
            AddCommand(command);
        }

        public void QueueHideBodyText(float speed=1.0f)
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.HideBodyText, speed);
            AddCommand(command);
        }

        public void QueueEnableUI()
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.EnableUI);
            AddCommand(command);
        }

        public void QueueDisableUI()
        {
            LoadingManagerCommand command = new LoadingManagerCommand(LoadingManagerCommand.Type.DisableUI);
            AddCommand(command);
        }
    }
}