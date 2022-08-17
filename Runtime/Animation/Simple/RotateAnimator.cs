using UnityEngine;

namespace Rano.Animation
{
    public class RotateAnimator : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotatePerSecond;

        void Update()
        {
            Vector3 moveRotate = _rotatePerSecond * Time.deltaTime;
            transform.Rotate(moveRotate, Space.Self);
        }
    }
}