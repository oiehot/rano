using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Rano.UGUI
{
    public enum ScrollOrientation
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// 스크롤을 움직일 때만 스크롤 바가 나오고 스크롤이 멈추면 잠시 후 사라지게 한다.
    /// </summary>
    public class FadableScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [Header("Link")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Image _scrollHandleImage;

        [Header("Settings")]
        [SerializeField] private ScrollOrientation _scrollOrientation;
        [SerializeField] private float _autoHideDelay = 2.0f;
        [SerializeField] private float _stopVelocityThreshold = 50.0f;
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;

        private Color _defaultColor;
        private float _defaultAlpha;

        public bool IsScrolling { get; private set; }

        void Awake()
        {
            _defaultColor = _scrollHandleImage.color;
            _defaultAlpha = _scrollHandleImage.color.a;
        }

        void OnEnable()
        {
            var color = _scrollHandleImage.color;
            color.a = 0.0f;
            _scrollHandleImage.color = color;

            _scrollRect.onValueChanged.AddListener(CheckScroll);
        }

        void OnDisable()
        {
            var color = _scrollHandleImage.color;
            color.a = 1.0f;
            _scrollHandleImage.color = _defaultColor;

            _scrollRect.onValueChanged.RemoveListener(CheckScroll);
        }

        private void CheckScroll(Vector2 vector2)
        {
            switch (_scrollOrientation)
            {
                case ScrollOrientation.Horizontal:
                    if (Mathf.Abs(_scrollRect.velocity.x) <= _stopVelocityThreshold)
                    {
                        IsScrolling = false;
                    }
                    else
                    {
                        IsScrolling = true;
                    }
                    break;
                case ScrollOrientation.Vertical:
                    if (Mathf.Abs(_scrollRect.velocity.y) <= _stopVelocityThreshold)
                    {
                        IsScrolling = false;
                    }
                    else
                    {
                        IsScrolling = true;
                    }
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            _scrollHandleImage.DOKill();
            _scrollHandleImage.DOFade(_defaultAlpha, _fadeDuration).SetEase(_fadeEase);
        }

        public void OnEndDrag(PointerEventData data)
        {
            StartCoroutine(nameof(CoOnEndDrag));
        }

        private IEnumerator CoOnEndDrag()
        {
            yield return new WaitWhile(() => IsScrolling);
            _scrollHandleImage.DOFade(0.0f, _fadeDuration)
                .SetDelay(_autoHideDelay)
                .SetEase(_fadeEase);
        }
    }
}