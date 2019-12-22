namespace Rano.Font
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class DigitNumber : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        public UnityEngine.Sprite[] fontSprites;
        
        private int val;
        public int Value
        {
            get
            {
                return val;
            }
            set
            {
                val = value % 10;
                spriteRenderer.sprite = fontSprites[val];
            }
        }
        
        // public float width
        // {
        //     get
        //     {
        //         return spriteRenderer.size.x;
        //     }
        // }
        
        // public float height
        // {
        //     get
        //     {
        //         return spriteRenderer.size.y;
        //     }
        // }
            
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            val = 0;
        }
        
        void Start()
        {
            // Debug.Log(width);
            // Debug.Log(height);
        }
    }
}