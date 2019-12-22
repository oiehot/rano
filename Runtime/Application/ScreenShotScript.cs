namespace Rano.Application
{
    using System;
    using UnityEngine;
    
    public class ScreenShotScript : MonoBehaviour
    {
        string dir;
        
        private void Awake()
        {
            dir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') ) + "/Screenshot" ;
        }
        
        private void Capture()
        {
            string date = DateTime.Now.ToString("yyyymmdd_HHmmss");
            string path = dir + '/' + date + ".png";
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Capture Screenshot to " + path);
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