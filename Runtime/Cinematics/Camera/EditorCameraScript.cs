#if false

using UnityEngine;

namespace Rano.Cinematics
{    
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