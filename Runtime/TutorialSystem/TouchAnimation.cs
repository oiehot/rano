#if false

using UnityEngine;
using UnityEngine.UI;

namespace Rano.TutorialSystem
{
    public class TouchAnimation : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Vector3 _startPos;
        [SerializeField] private Vector3 _maxScale;

        public void Play()
        {
            // TODO: Dotween 으로 RectTransform의 스케일을 루프 애니메이션
        }

        public void Stop()
        {
            // TODO: Dotween 으로 RectTransform의 스케일을 루프 애니메이션 끄기
        }
    }
}

#endif