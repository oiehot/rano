using System;
using UnityEngine;

namespace Rano
{
    // TODO: Screenshot 디렉토리가 없으면 캡쳐가 안됨
    public class ScreenshotScript : MonoBehaviour
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
            Rano.SysLog.Important("Capture Screenshot to " + path);
        }
        
        #if UNITY_EDITOR
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