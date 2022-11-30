using UnityEngine;
using UnityEngine.UI;

namespace Rano.Animation
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageScroll : MonoBehaviour
    {
        private RawImage _rawImage;
        private float _currentOffsetX;
        private float _currentOffsetY;
        
        [SerializeField] private float _speedX = 0.1f;
        [SerializeField] private float _speedY;
        
        void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            _currentOffsetX = _rawImage.uvRect.x;
            _currentOffsetY = _rawImage.uvRect.y;
        }
        
        void Update()
        {
            _currentOffsetX += _speedX * Time.deltaTime;
            _currentOffsetY += _speedY * Time.deltaTime;
            _rawImage.uvRect = new Rect(_currentOffsetX, _currentOffsetY, 1.0f, 1.0f);
        }
    }
}