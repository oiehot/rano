// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using UnityEngine;

namespace Rano
{
    public class FontManager : MonoBehaviour
    {
        public Font font;
        public FilterMode filterMode;
        
        void Start()
        {
            // FilterMode.Point 를 넣으면 Antialising 을 없앨 수 있다.
            font.material.mainTexture.filterMode = filterMode;
        }
    }
}

#endif