#nullable enable

using UnityEngine;
    
namespace Rano.Animation
{      
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererScroll : MonoBehaviour
    {
        private SpriteRenderer? _spriteRenderer;
        private Vector3 startPosition;
        private float _spriteWidth;
        private float _spriteHeight;
        private float _minX;
        private bool _isRun;
        
        public float speed;
        
        void Awake()
        {  
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Bounds bounds = _spriteRenderer.bounds;
            _spriteWidth = bounds.max.x - bounds.min.x;
            _spriteHeight = bounds.max.y - bounds.min.y;
        }
        
        void Start()
        {
            startPosition = transform.position;
            _minX = startPosition.x - _spriteWidth;
            Run();
        }
        
        public void Run()
        {
            _isRun = true;
        }
        
        public void Stop()
        {
            _isRun = false;
        }
        
        void Update()
        {
            if (_isRun)
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime);
                if (transform.position.x < _minX)
                {
                    transform.position = startPosition;
                }
            }
        }
    }
}