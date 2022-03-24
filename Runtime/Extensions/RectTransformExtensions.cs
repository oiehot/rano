// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    public static class RectTransformExtensions
    {
        // Position

        public static void SetY(this RectTransform rect, float y)
        {
            float x = rect.anchoredPosition.x;
            rect.anchoredPosition = new Vector2(x, y);
        }

        public static void MoveY(this RectTransform rect, float offsetY)
        {
            float x = rect.anchoredPosition.x;
            float y = rect.anchoredPosition.y;
            rect.anchoredPosition = new Vector2(x, y + offsetY);
        }

        // Width, Height

        /// <summary>
        /// RectTransform�� ���� ��´�. Awake�� Start�� �ٷ� ����ϸ� �������� ���� �ȳ��� �� �ִ�.
        /// </summary>
        public static float GetWidth(this RectTransform rect)
        {
            //return rect.sizeDelta.x;
            return rect.rect.width;
        }

        /// <summary>
        /// RectTransform�� ���̸� ��´�. Awake�� Start�� �ٷ� ����ϸ� �������� ���� �ȳ��� �� �ִ�.
        /// </summary>
        public static float GetHeight(this RectTransform rect)
        {
            //return rect.sizeDelta.y;
            return rect.rect.height;
        }

        public static void SetWidth(this RectTransform rect, float width)
        {
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }
        
        public static void SetHeight(this RectTransform rect, float height)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }
        
        public static void SetSize(this RectTransform rect, float width, float height)
        {
            rect.sizeDelta = new Vector2(width, height);
        }

        // Left, Right, Top, Bottom

        public static float GetLeft(this RectTransform rect)
        {
            return rect.offsetMin.x;
        }

        public static float GetRight(this RectTransform rect)
        {
            return rect.offsetMax.x;
        }

        public static void SetLeft(this RectTransform rect, float left)
        {
            rect.offsetMin = new Vector2(left, rect.offsetMin.y);
        }
            
        public static void SetRight(this RectTransform rect, float right)
        {
            rect.offsetMax = new Vector2(-right, rect.offsetMax.y);
        }
    
        public static void SetTop(this RectTransform rect, float top)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x, -top);
        }
    
        public static void SetBottom(this RectTransform rect, float bottom)
        {
            rect.offsetMin = new Vector2(rect.offsetMin.x, bottom);
        }

        //  Padding

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