using UnityEngine;

namespace Rano
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Hierarchy상 게임오브젝트의 경로를 돌려준다.
        /// </summary>
        /// <returns>게임 오브젝트의 경로 ex) "/A/B/GameObject"</returns>
        public static string GetPath(this GameObject gameObject)
        {
            string path = "/" + gameObject.name;
            while (gameObject.transform.parent != null)
            {
                gameObject = gameObject.transform.parent.gameObject;
                path = "/" + gameObject.name + path;
            }
            return path;
        }

        /// <summary>
        /// 게임오브젝트에 부착된 컴포넌트를 돌려준다. 없으면 예외를 발생시킨다.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <returns>찾은 컴포넌트</returns>
        public static T GetRequiredComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                return component;
            }
            else
            {
                throw new MissingComponentException($"{gameObject.GetPath()} 에 {nameof(T)} 컴포넌트가 없음.");
            }
        }

        /// <summary>
        /// 게임오브젝트에 부착된 컴포넌트를 돌려준다. 없으면 새로 생성해서 돌려준다.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <returns>찾은 컴포넌트</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component) == true)
            {
                return component;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }

        /// <summary>
        /// 특정 컴포넌트 부착여부를 리턴한다.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <returns>컴포넌트 부착 여부</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }
    }
}