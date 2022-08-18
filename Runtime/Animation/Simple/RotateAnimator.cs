using UnityEngine;

namespace Rano.Animation
{
    public class RotateAnimator : SimpleAnimator
    {
        [SerializeField] private Vector3 _rotatePerSecond;
        
        protected override void HandleUpdate()
        {
            Vector3 moveRotate = _rotatePerSecond * Time.deltaTime;
            transform.Rotate(moveRotate, Space.Self);
        }
    }
}