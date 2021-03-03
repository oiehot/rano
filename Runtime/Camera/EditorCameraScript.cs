// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

namespace Rano
{    
    using UnityEngine;
    
    [ExecuteInEditMode]
    public class FixOrthographicSize : MonoBehaviour
    {
        public float pixelPerUnit = 100;
        void Start()
        {
            GetComponent<UnityEngine.Camera>().orthographicSize = Screen.height / pixelPerUnit / 2;    
        }
        void Update()
        {
            GetComponent<UnityEngine.Camera>().orthographicSize = Screen.height / pixelPerUnit / 2;    
        }
    }
}

#endif