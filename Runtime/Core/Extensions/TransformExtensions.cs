using UnityEngine;

namespace Rano
{    
    public static class TransformExtensions
    {
        /// <summary>
        /// Hierarchy상 경로를 돌려준다.
        /// </summary>
        public static string GetPath(this Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

        /// <summary>
        /// Transform 값을 초기화한다.
        /// </summary>
        public static void ResetTransformation(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(1, 1, 1);
        }

        // public static Vector2 ToRectTransformPosition(this Transform transform, Camera camera, RectTransform rectTransform)
        // {
        //     Vector2 viewportPos = camera.WorldToViewportPoint(transform.position);
        //     Vector2 result = new Vector2(
        //         ((viewportPos.x * rectTransform.sizeDelta.x) - (rectTransform.sizeDelta.x * 0.5f)),
        //         ((viewportPos.y * rectTransform.sizeDelta.y) - (rectTransform.sizeDelta.y * 0.5f))
        //     );
        //     return result;
        // }
    }
}