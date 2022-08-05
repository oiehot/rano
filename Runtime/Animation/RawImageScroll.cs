using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rano.Animation
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageScroll : MonoBehaviour
    {
        RawImage rawImage;
        public float speedX = 0.1f;
        public float speedY = 0.0f;
        private float currentOffsetX;
        private float currentOffsetY;
        
        void Awake()
        {
            this.rawImage = GetComponent<RawImage>();
            this.currentOffsetX = this.rawImage.uvRect.x;
            this.currentOffsetY = this.rawImage.uvRect.y;
        }
        
        void Update()
        {
            this.currentOffsetX += speedX * Time.deltaTime;
            this.currentOffsetY += speedY * Time.deltaTime;
            this.rawImage.uvRect = new Rect(currentOffsetX, currentOffsetY, 1.0f, 1.0f);
        }
    }
}