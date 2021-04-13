// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;

namespace Rano
{
    // TODO: Screenshot 디렉토리가 없으면 캡쳐가 안됨
    public class ScreenshotController : MonoBehaviour
    {
        private string directory;
        
        private void Awake()
        {
            directory = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') ) + "/Screenshot" ;
        }
        
        private void Capture()
        {
            string date = DateTime.Now.ToString("yyyymmdd_HHmmss");
            string path = directory + '/' + date + ".png";
            ScreenCapture.CaptureScreenshot(path);
            Rano.Log.Important("Capture Screenshot to " + path);
        }
        
        // TODO: ENABLE_INPUT_SYSTEM
        #if UNITY_EDITOR && ENABLE_LEGACY_INPUT_MANAGER
        public void Update()
        {   
            if (
                (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                Input.GetKeyUp(KeyCode.P))
            {
                Capture();
            }
        }
        #endif
    }
}