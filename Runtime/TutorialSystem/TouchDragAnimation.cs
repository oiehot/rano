#if false

using UnityEngine;
using UnityEngine.UI;

namespace Rano.TutorialSystem
{
    public class TouchDragAnimation : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Vector3 _startPos;
        [SerializeField] private Vector3 _endPos;

        public void Play()
        {
            // TODO: Dotween 으로 RectTransform를 시작지점과 끝지점으로 드래그 루프 애니메이션
        }

        public void Stop()
        {
            // TODO: Dotween 으로 RectTransform의 시작지점과 끝지점으로 드래그 루프 루프 애니메이션 끄기
        }
    }
}

#endif