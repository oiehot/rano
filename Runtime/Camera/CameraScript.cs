// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

namespace Rano
{
    using UnityEngine;
    
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraScript : MonoBehaviour
    {
        Vector2 resolution;
        UnityEngine.Camera camera;
        public float pixelPerUnit = 100;

        void Awake()
        {
            camera = GetComponent<UnityEngine.Camera>();
            resolution = new Vector2(Screen.width, Screen.height);
            Debug.Log($"First Resolution: {resolution.x}x{resolution.y}");
            UpdateCamera();
        }
        
        void UpdateCamera()
        {
            resolution.x = Screen.width;
            resolution.y = Screen.height;
            camera.orthographicSize = resolution.y / pixelPerUnit / 2;            
            Debug.Log($"Set Camera Orthographicsize: {camera.orthographicSize}");
        }

        #if UNITY_EDITOR
        void Update()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                UpdateCamera();
            }
        }
        #endif
    }
}

#endif