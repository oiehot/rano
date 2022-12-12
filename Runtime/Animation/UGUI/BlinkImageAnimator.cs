// #nullable enable

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Rano.Animation.UGUI
{
    public class BlinkImageAnimator : MonoBehaviour
    {
        private Image _image;
        private Sequence _sequence;
        private Color _initialColor;

        private Color FadeInColor
        {
            get
            {
                Color result = _initialColor;
                result.a = 1.0f;
                return result;
            }
        }

        private Color FadeOutColor
        {
            get
            {
                Color result = _initialColor;
                result.a = 0.0f;
                return result;
            }
        }
        
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private float _showDuration = 0.50f;
        [SerializeField] private float _hideDuration = 0.25f;
        [SerializeField] private Ease _fadeEase = Ease.Linear;
        
        [SerializeField] private bool _autoPlayOnStart = true;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _initialColor = _image.color;
            _image.color = FadeOutColor;
        }

        private void OnDisable()
        {
            // Image 컴포넌트가 삭제 된 상태에서
            // 트위닝 시퀀스를 실행하면 널 참조 에러가 발생하므로,
            // 트위닝 시퀀스를 종료해준다.
            Stop();
        }

        private void Start()
        {
            if (_autoPlayOnStart) Play();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (_sequence != null)
            {
                Log.Info("이미 실행 중이므로 재시작 합니다");
                Stop();
            }

            _sequence = DOTween.Sequence()
                .SetAutoKill(false) // 기본적으로 시퀀스는 1회용임. 재사용하기 위해 종료되어도 삭제되지 않게 만듬.
                .SetLoops(-1, LoopType.Restart)
                .Append(_image.DOFade(1.0f, _fadeDuration).SetEase(_fadeEase))
                .AppendInterval(_showDuration)
                .Append(_image.DOFade(0.0f, _fadeDuration).SetEase(_fadeEase))
                .AppendInterval(_hideDuration)
            ;
            
            // .Pause() // 자동실행 방지
            // _sequence.Restart(); // Play()로 하면 1회만 실행한다.
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;

                if (_image) _image.color = FadeOutColor;
            }
        }
    }
}