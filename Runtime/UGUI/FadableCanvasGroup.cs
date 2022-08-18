using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Rano.UGUI
{
    public class FadableCanvasGroup : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 0.25f;
        [SerializeField] private float _hideDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.Linear;
        [SerializeField] private Ease _hideEase = Ease.Linear;
        public Action OnShowCompleted { get; set; }
        public Action OnHideCompleted { get; set; }
        public bool IsShowed => gameObject.activeSelf;
        
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        [ContextMenu("Show")]
        public void Show()
        {
            if (gameObject.activeSelf == true) return;
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1.0f, _showDuration)
                .SetEase(_showEase)
                .OnComplete(() =>
                {
                    OnShowCompleted?.Invoke();
                });
        }
        
        [ContextMenu("Hide")]
        public void Hide(Action onHideCompleted=null)
        {
            if (gameObject.activeSelf == false) return;
            _canvasGroup.DOFade(0.0f, _hideDuration)
                .SetEase(_hideEase)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnHideCompleted?.Invoke();
                    onHideCompleted?.Invoke();
                });
        }
        
        // public IEnumerator ShowCoroutine()
        // {
        //     if (gameObject.activeSelf == true) yield break;
        //     gameObject.SetActive(true);
        //     yield return _canvasGroup.DOFade(1.0f, _showDuration)
        //         .SetEase(_showEase)
        //         .WaitForCompletion();
        //     OnShowCompleted?.Invoke();
        // }
        //
        // public IEnumerator HideCoroutine()
        // {
        //     if (gameObject.activeSelf == false) yield break;
        //     yield return _canvasGroup.DOFade(0.0f, _hideDuration)
        //         .SetEase(_hideEase)
        //         .WaitForCompletion();
        //     gameObject.SetActive(false);
        //     OnHideCompleted?.Invoke();
        // }
    }
}