namespace Rano
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [RequireComponent(typeof(MeshRenderer))]
    public class SpriteScrollUV : MonoBehaviour
    {
        Material material;
        public float speed_x = 0.1f;
        public float speed_y = 0.0f;
        public float offset_x = 0.0f;
        public float offset_y = 0.0f;
        
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            offset_x += speed_x * Time.deltaTime;
            offset_y += speed_y * Time.deltaTime;

            material.mainTextureOffset = new Vector2(offset_x, offset_y);
        }
    }
}
