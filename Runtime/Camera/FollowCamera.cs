namespace Rano.Camera
{   
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        private Camera _camera; // ! camera는 deprecated 되었다. 반면에 camera를 만들려고 하면 경고를 띄우므로 _을 붙임.
        private Vector3 velocity = Vector3.zero;
        private float errorToIgnore = 0.1f;
        
        public Transform target;
        public Vector3 targetDistance = new Vector3(0.5f, 0.5f, 10.0f); // 이 값 + 타겟 에 카메라가 놓이게 된다.
        public float smoothTime = 0.15f;
        
        private void Awake() {
            _camera = GetComponent<Camera>();
        }
        
        void Update()
        {
            if (target)
            {
                // public static Vector3 SmoothDamp(
                //   Vector3 current: 현재 위치
                //   Vector3 target: 도달하려는 위치
                //   ref Vector3 currentVelocity: 현재 속도, 레퍼런스로 이 변수는 해당 함수 안에서 업데이트된다.
                //   float smoothTime: 타겟에 도달하기 위한 대략적인 시간
                //   float maxSpeed: 선택적으로 최대속도를 고정할 수 있도록 함. 기본값은 Mathf.Infinity
                //   float deltaTime: 이 함수가 마지막으로 호출되고 나서의 경과 시간. 기본값은 Time.deltaTime.
                // );
                
                Vector3 delta = target.position - _camera.ViewportToWorldPoint(targetDistance);
                // Vector3.magnitude: 벡터의 길이
                if (delta.magnitude >= errorToIgnore) // 오차는 무시하고 움직이지 않는다. 이 코드가 없으면 카메라가 플레이어 중심에서 계속 떨린다.
                {
                    Vector3 destination = transform.position + delta;
                    transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smoothTime);
                }
            }
        }
    }
}