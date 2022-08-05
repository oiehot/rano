using UnityEngine;

namespace Rano
{    
    public static class TransformExtensions
    {
        /// <summary>
        /// Hierarchy상 게임오브젝트의 경로를 돌려준다.
        /// </summary>
        /// <param name="gameObject"></param>
        /// </example>
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
        /// <param name="trans"></param>
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}