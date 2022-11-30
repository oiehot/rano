using UnityEngine;

namespace Rano.Cinematics
{
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        private Camera _camera;
        private Vector3 _velocity = Vector3.zero;
        private float _threshold = 0.1f;
        
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Vector3 _targetDistance = new Vector3(0.5f, 0.5f, 10.0f); // 이 값 + 타겟 에 카메라가 놓이게 된다.
        [SerializeField] private float _smoothTime = 0.15f;
        
        void Awake()
        {
            _camera = GetComponent<Camera>();
        }
        
        void Update()
        {
            if (_targetTransform)
            {
                // public static Vector3 SmoothDamp(
                //   Vector3 current: 현재 위치
                //   Vector3 target: 도달하려는 위치
                //   ref Vector3 currentVelocity: 현재 속도, 레퍼런스로 이 변수는 해당 함수 안에서 업데이트된다.
                //   float smoothTime: 타겟에 도달하기 위한 대략적인 시간
                //   float maxSpeed: 선택적으로 최대속도를 고정할 수 있도록 함. 기본값은 Mathf.Infinity
                //   float deltaTime: 이 함수가 마지막으로 호출되고 나서의 경과 시간. 기본값은 Time.deltaTime.
                // );
                
                Vector3 delta = _targetTransform.position - _camera.ViewportToWorldPoint(_targetDistance);
                // Vector3.magnitude: 벡터의 길이
                if (delta.magnitude >= _threshold) // 오차는 무시하고 움직이지 않는다. 이 코드가 없으면 카메라가 플레이어 중심에서 계속 떨린다.
                {
                    Vector3 position = transform.position;
                    Vector3 destination = position + delta;
                    transform.position = Vector3.SmoothDamp(position, destination, ref _velocity, _smoothTime);
                }
            }
        }
    }
}