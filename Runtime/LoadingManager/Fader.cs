// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Rano
{
    [AddComponentMenu("Rano/Fader")]
    public class Fader : MonoBehaviour
    {
        public enum Status
        {
            None,
            FadeOut,
            FadeIn
        }

        public Status status {get; private set;}

        [Header("Fade In/Out")]
        public Image fadeImage;
        public Ease fadeEase = Ease.OutQuad;
        public float fadeSpeed = 0.5f;

        [Header("Progress")]
        public Text text;
        public float fadeTextSpeed = 0.1f;

        // Cache
        GameObject fadeGameObject;
        GameObject textGameObject;

        void Awake()
        {
            fadeImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f); // Black
            fadeGameObject = fadeImage.gameObject;
            fadeGameObject.SetActive(false);
            // status = Status.FadeOut;
            status = Status.None;

            textGameObject = text.gameObject;
            textGameObject.SetActive(false);
            text.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        public IEnumerator FadeOut()
        {
            // 이미 페이드아웃 되어 있다면 실행하지 않는다.
            if (status == Status.FadeOut)
            {
                Log.Warning("페이드아웃 생략: 이중 페이드아웃 실행");
                yield break;
            }

            fadeGameObject.SetActive(true);
            yield return fadeImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), fadeSpeed)
                .SetEase(fadeEase)
                .WaitForCompletion();
            status = Status.FadeOut;
        }

        public IEnumerator FadeIn()
        {
            // 이미 페이드인 되어 있다면 실행하지 않는다.
            if (status == Status.FadeIn)
            {
                Log.Warning("페이드인 생략: 이중 페이드인 실행");
                yield break;
            }

            fadeGameObject.SetActive(true);
            yield return fadeImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), fadeSpeed)
                .SetEase(fadeEase)
                .WaitForCompletion();
            fadeGameObject.SetActive(false);
            status = Status.FadeIn;
        }

        public IEnumerator ShowText()
        {
            textGameObject.SetActive(true);
            text.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), fadeTextSpeed);
            yield break;
        }

        public IEnumerator HideText()
        {
            yield return text.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), fadeTextSpeed)
                .WaitForCompletion();
            textGameObject.SetActive(false);
        }
    }
}