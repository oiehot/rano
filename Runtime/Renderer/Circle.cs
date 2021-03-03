// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;
using System.Collections;

namespace Rano.Renderer
{
    [RequireComponent(typeof(LineRenderer))]
    public class Circle : MonoBehaviour
    {
        [Range(0,100)]
        public int segments = 50;
        [Range(0,1)]
        public float width = 0.02f;
        private float radiusX = 1;
        private float radiusY = 1;
        private LineRenderer line;

        void Start()
        {
            line = gameObject.GetComponent<LineRenderer>();
            line.positionCount = segments + 1;
            line.useWorldSpace = false;
            line.startWidth = width;
            line.endWidth = width;
            CreatePoints();
        }

        void CreatePoints()
        {
            float x=0.0f, y=0.0f, z=0.0f, angle=20.0f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin (Mathf.Deg2Rad * angle) * radiusX;
                y = Mathf.Cos (Mathf.Deg2Rad * angle) * radiusY;
                line.SetPosition (i,new Vector3(x,y,z) );
                angle += (360f / segments);
            }
        }

        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        void Update()
        {
            line.startWidth = width;
            line.endWidth = width;
        }
        #endif
    }
}
