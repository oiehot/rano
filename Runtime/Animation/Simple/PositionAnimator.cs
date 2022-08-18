using UnityEngine;
using UnityEngine.UI;

namespace Rano.Animation
{
    public class PositionAnimator : SimpleAnimator
    {
        [SerializeField] private Vector3 _startPos;
        [SerializeField] private Vector3 _endPos;

        protected override void HandleStart()
        { 
            Log.Todo("Dotween 으로 RectTransform를 시작지점과 끝지점으로 드래그 루프 애니메이션 시작하기");
        }

        protected override void HandleEnd()
        {
            Log.Todo("Dotween 으로 RectTransform를 시작지점과 끝지점으로 드래그 루프 애니메이션 종료하기");
        }
    }
}