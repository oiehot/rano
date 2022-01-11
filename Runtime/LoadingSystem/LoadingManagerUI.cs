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
    public sealed class LoadingManagerUI : MonoBehaviour
    {
        // Canvas
        [Header("Base")]
        [SerializeField] private Canvas _canvas;
        private GameObject _canvasGameObject;

        // Fade
        [Header("Fade In/Out")]
        public Image fadeOutImage;
        public Ease fadeEase = Ease.OutQuad;
        public GameObject FadeOutGameObject { get; private set; }

        // Text
        [Header("Text UI")]
        public Text headerText;
        public Text bodyText;
        public Text footerText;
        public GameObject HeaderTextGameObject { get; private set; }
        public GameObject BodyTextGameObject { get; private set; }
        public GameObject FooterTextGameObject { get; private set; }

        private void Awake()
        {
            Log.Sys($"{typeof(LoadingManagerUI).ToString()}: Awake", caller: false);

            _canvasGameObject = _canvas.gameObject;

            fadeOutImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            FadeOutGameObject = fadeOutImage.gameObject;
            FadeOutGameObject.SetActive(true);

            HeaderTextGameObject = headerText.gameObject;
            HeaderTextGameObject.SetActive(false);
            headerText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            headerText.text = "";

            BodyTextGameObject = bodyText.gameObject;
            BodyTextGameObject.SetActive(false);
            bodyText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            bodyText.text = "LOADING...";

            FooterTextGameObject = footerText.gameObject;
            FooterTextGameObject.SetActive(false);
            footerText.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            footerText.text = "";
        }

        public void EnableUI()
        {
            _canvasGameObject.SetActive(true);
        }

        public void DisableUI()
        {
            _canvasGameObject.SetActive(false);
        }

        public IEnumerator CoFadeOut(float speed)
        {
            FadeOutGameObject.SetActive(true);
            yield return fadeOutImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), speed)
                .SetEase(fadeEase)
                .WaitForCompletion();
        }

        public IEnumerator CoFadeIn(float speed)
        {
            FadeOutGameObject.SetActive(true);
            yield return fadeOutImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), speed)
                .SetEase(fadeEase)
                .WaitForCompletion();
            FadeOutGameObject.SetActive(false);
        }

        public IEnumerator ShowBodyText(float speed)
        {
            BodyTextGameObject.SetActive(true);
            bodyText.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), speed);
            yield break;
        }

        public IEnumerator HideBodyText(float speed)
        {
            yield return bodyText.DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), speed)
                .WaitForCompletion();
            BodyTextGameObject.SetActive(false);
        }
    }
}