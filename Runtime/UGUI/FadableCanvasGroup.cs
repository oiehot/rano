using System;
using UnityEngine;
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
        
        public event Action OnShowCompleted;
        public event Action OnHideCompleted;

        public bool IsShowed => gameObject.activeSelf;
        
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        [ContextMenu("Show")]
        public void Show()
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1.0f;
            OnShowCompleted?.Invoke();
        }
        
        [ContextMenu("Hide")]
        public void Hide()
        {
            if (gameObject.activeSelf) return;
            _canvasGroup.alpha = 0.0f;
            gameObject.SetActive(false);
            OnHideCompleted?.Invoke();
        }

        [ContextMenu("Show Fade")]
        public void ShowFade()
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1.0f, _showDuration)
                .SetEase(_showEase)
                .OnComplete(() =>
                {
                    OnShowCompleted?.Invoke();
                });
        }
        
        [ContextMenu("Hide Fade")]
        public void HideFade() // Action onHideCompleted=null)
        {
            if (gameObject.activeSelf == false) return;
            _canvasGroup.DOFade(0.0f, _hideDuration)
                .SetEase(_hideEase)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnHideCompleted?.Invoke();
                    // onHideCompleted?.Invoke();
                });
        }
    }
}