// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine;
using Rano.Addressable;

namespace Rano
{
    public partial class LoadingManager : MonoSingleton<LoadingManager>
    {
        /// <summary>
        /// 씬을 추가한다(로드)
        /// </summary>
        public IEnumerator CoAddScene(Address sceneAddress)
        {
            status = Status.LoadingScene;
            yield return SceneManager.Instance.AddSceneAsync(sceneAddress);
            status = Status.LoadingSceneCompleted;
        }

        /// <summary>
        /// 씬을 활성화한다.
        /// </summary>
        /// <remarks>
        /// Instantiate된 개체가 지정되는 씬 활성화를 말한다.
        /// </remarks>
        public void ActiveScene(Address sceneAddress)
        {
            SceneManager.Instance.ActivateScene(sceneAddress);
        }

        /// <summary>
        /// 씬을 제거한다(언로드)
        /// </summary>
        public IEnumerator CoRemoveScene(Address sceneAddress)
        {
            status = Status.UnloadingScene;
            yield return SceneManager.Instance.RemoveSceneAsync(sceneAddress);
            status = Status.UnloadingSceneCompleted;
        }

        /// <summary>
        /// 씬을 교체한다.
        /// </summary>
        /// <remark>
        /// 현재 씬은 언로드된다.
        /// </remark>
        // public void ChangeScene(Address sceneAddress, bool fadeOut=true, bool fadeIn=true, bool showProgress=false)
        // {
        //     StartCoroutine(CoChangeScene(sceneAddress, fadeOut, fadeIn, showProgress));
        // }

        // /// <remarks>
        // /// 이 코루틴은, 언로드될 씬에서 실행해서는 절대 안된다.
        // /// </remarks>
        // private IEnumerator CoChangeScene(Address sceneAddress, bool fadeOut, bool fadeIn, bool showProgress)
        // {
        //     // 페이드 아웃
        //     if (fadeOut) yield return StartCoroutine(nameof(fader.FadeOut));
        //     if (showProgress) yield return StartCoroutine(nameof(fader.ShowText));

        //     // 씬 교체
        //     status = Status.LoadingScene;
        //     yield return SceneManager.Instance.ChangeSceneAsync(sceneAddress);
        //     status = Status.LoadingSceneCompleted;

        //     // 페이드 인
        //     if (showProgress) yield return StartCoroutine(nameof(fader.HideText));
        //     if (fadeIn) yield return StartCoroutine(nameof(fader.FadeIn));
        // }

        /// <remarks>
        /// 이 코루틴은, 언로드될 씬에서 실행해서는 절대 안된다.
        /// </remarks>
        private IEnumerator CoChangeScene(Address sceneAddress, bool fadeOut, bool fadeIn, bool showProgress)
        {
            // 페이드 아웃
            if (fadeOut) yield return StartCoroutine(fader.FadeOut());
            if (showProgress) yield return StartCoroutine(fader.ShowText());

            // 씬 교체
            status = Status.LoadingScene;
            yield return SceneManager.Instance.ChangeSceneAsync(sceneAddress);
            status = Status.LoadingSceneCompleted;

            // 페이드 인
            if (showProgress) yield return StartCoroutine(fader.HideText());
            if (fadeIn) yield return StartCoroutine(fader.FadeIn());
        }
    }
}