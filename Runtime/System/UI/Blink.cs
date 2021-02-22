using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Text, Image, ...

namespace Rano
{
    public class Blink<T> : MonoBehaviour where T : MonoBehaviour
    {
        private T component;
        private bool hidden = false;    
        public float updateDelay = 1.0f;
        
        void Awake()
        {
            component = GetComponent<T>();
        }
        
        void OnEnable()
        {
            InvokeRepeating("UpdateBlink", 0.000001f, updateDelay);
        }
        
        void OnDisable()
        {
            CancelInvoke("UpdateBlink");
        }
        
        void UpdateBlink()
        {
            if (hidden)
            {
                component.enabled = false;
                hidden = false;
            }
            else
            {
                component.enabled = true;
                hidden = true;
            }
        }
    }
}