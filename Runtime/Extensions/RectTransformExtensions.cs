// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    /*
    RectTransform rectTransform;
    rectTransform.offsetMin.x; // Left
    rectTransform.offsetMax.x; // Right
    rectTransform.offsetMax.y; // Top
    rectTransform.offsetMin.y; // Bottom
    */
    public static class RectTransformExtensions
    {
        public static void SetRight(this RectTransform rect, float right)
        {
            rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
        }

        public static void SetPadding(this RectTransform rect, float horizontal, float vertical)
        {
            rect.offsetMax = new Vector2(-horizontal, -vertical);
            rect.offsetMin = new Vector2(horizontal, vertical);
        }

        public static void SetPadding(this RectTransform rect, float left, float top, float right, float bottom)
        {
            rect.offsetMax = new Vector2(-right, -top);
            rect.offsetMin = new Vector2(left, bottom);
        }
    }
}