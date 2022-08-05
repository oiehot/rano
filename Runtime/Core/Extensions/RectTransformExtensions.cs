using UnityEngine;

namespace Rano
{
    public static class RectTransformExtensions
    {
        // Position

        public static void SetY(this RectTransform rectTransform, float y)
        {
            float x = rectTransform.anchoredPosition.x;
            rectTransform.anchoredPosition = new Vector2(x, y);
        }

        public static void MoveY(this RectTransform rectTransform, float offsetY)
        {
            float x = rectTransform.anchoredPosition.x;
            float y = rectTransform.anchoredPosition.y;
            rectTransform.anchoredPosition = new Vector2(x, y + offsetY);
        }

        // Width, Height

        public static float GetWidth(this RectTransform rectTransform)
        {
            //return rect.sizeDelta.x;
            return rectTransform.rect.width;
        }

        public static float GetHeight(this RectTransform rectTransform)
        {
            //return rect.sizeDelta.y;
            return rectTransform.rect.height;
        }

        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }
        
        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
        
        public static void SetSize(this RectTransform rectTransform, float width, float height)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        // Left, Right, Top, Bottom

        public static float GetLeft(this RectTransform rectTransform)
        {
            return rectTransform.offsetMin.x;
        }

        public static float GetRight(this RectTransform rectTransform)
        {
            return rectTransform.offsetMax.x;
        }

        public static void SetLeft(this RectTransform rectTransform, float left)
        {
            rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        }
            
        public static void SetRight(this RectTransform rectTransform, float right)
        {
            rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
        }
    
        public static void SetTop(this RectTransform rectTransform, float top)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
        }
    
        public static void SetBottom(this RectTransform rectTransform, float bottom)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
        }

        //  Padding

        public static void SetPadding(this RectTransform rectTransform, float horizontal, float vertical)
        {
            rectTransform.offsetMax = new Vector2(-horizontal, -vertical);
            rectTransform.offsetMin = new Vector2(horizontal, vertical);
        }

        public static void SetPadding(this RectTransform rectTransform, float left, float top, float right, float bottom)
        {
            rectTransform.offsetMax = new Vector2(-right, -top);
            rectTransform.offsetMin = new Vector2(left, bottom);
        }
    }
}