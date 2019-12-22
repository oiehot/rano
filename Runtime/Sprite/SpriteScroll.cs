using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano.Sprite
{   
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteScroll : MonoBehaviour
    {
        // TODO: Must required
        public float speed;
            
        SpriteRenderer spriteRenderer;
        GameObject o2;
        Vector3 startPosition;
        bool run;   
        float spriteWidth;
        float spriteHeight;
        float minX;
        
        void Awake()
        {  
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteWidth = spriteRenderer.size.x;
            spriteHeight = spriteRenderer.size.y;
        }
        
        void Start()
        {
            startPosition = transform.position;
            minX = startPosition.x - spriteWidth;
            Run();
        }
        
        // TODO: Run() Stop() 을 가지는 Interface정의
        public void Run()
        {
            run = true;
        }
        
        public void Stop()
        {
            run = false;
        }
        
        void Update()
        {
            if (run)
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                if (transform.position.x < minX)
                {
                    transform.position = startPosition;
                }
            }
        }
    }
}