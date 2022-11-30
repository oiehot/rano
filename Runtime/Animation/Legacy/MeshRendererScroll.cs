using UnityEngine;

namespace Rano.Animation
{   
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshRendererScroll : MonoBehaviour
    {
        private Material _material;
        [SerializeField] private float _speedX = 0.1f;
        [SerializeField] private float _speedY;
        [SerializeField] private float _offsetX;
        [SerializeField] private float _offsetY;
        
        void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            _offsetX += _speedX * Time.deltaTime;
            _offsetY += _speedY * Time.deltaTime;
            _material.mainTextureOffset = new Vector2(_offsetX, _offsetY);
        }
    }
}
