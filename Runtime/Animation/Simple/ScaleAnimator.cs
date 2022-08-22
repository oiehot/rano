using UnityEditor.UI;
using UnityEngine;

namespace Rano.Animation
{
    public class ScaleAnimator : SimpleAnimator
    {
        [SerializeField] private Vector3 _startPos;
        [SerializeField] private Vector3 _maxScale;

        protected override void HandleStart()
        {
            Log.Todo("Dotween 으로 RectTransform의 스케일을 루프 애니메이션 시작하기");
            
        }

        protected override void HandleEnd()
        {
            Log.Todo("Dotween 으로 RectTransform의 스케일을 루프 애니메이션 종료하기");
        }
    }
}