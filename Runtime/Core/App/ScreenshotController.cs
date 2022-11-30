using System;
using UnityEngine;

namespace Rano.App
{
    // TODO: Screenshot 디렉토리가 없으면 캡쳐가 안됨.
    public class ScreenshotController : MonoBehaviour
    {
        private string _directory;
        
        private void Awake()
        {
            _directory = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') ) + "/Screenshot" ;
        }
        
        private void Capture()
        {
            string date = DateTime.UtcNow.ToLocalTime().ToString("yyyymmdd_HHmmss");
            string path = _directory + '/' + date + ".png";
            ScreenCapture.CaptureScreenshot(path);
            Log.Important("Capture Screenshot to " + path);
        }
        
        // TODO: ENABLE_INPUT_SYSTEM
        #if (UNITY_EDITOR && ENABLE_LEGACY_INPUT_MANAGER)
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