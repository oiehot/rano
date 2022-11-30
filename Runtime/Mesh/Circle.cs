using UnityEngine;

namespace Rano.Mesh
{
    [RequireComponent(typeof(LineRenderer))]
    public class Circle2 : MonoBehaviour
    {
        [Range(0,100)]
        public int segments = 50;
        [Range(0,1)]
        public float width = 0.02f;
        
        private float _radiusX = 1;
        private float _radiusY = 1;
        private LineRenderer _lineRenderer;

        void Start()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            _lineRenderer.positionCount = segments + 1;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            CreatePoints();
        }

        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        void Update()
        {
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
        }
        #endif
        
        private void CreatePoints()
        {
            float x, y, z=0.0f, angle=20.0f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin (Mathf.Deg2Rad * angle) * _radiusX;
                y = Mathf.Cos (Mathf.Deg2Rad * angle) * _radiusY;
                _lineRenderer.SetPosition (i,new Vector3(x,y,z) );
                angle += (360f / segments);
            }
        }
    }
}
