using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Rano.Editor
{
    public static class AssetDatabaseHelper
    {
        /// <summary>
        /// 하나의 스크립터블 오브젝트 에셋을 로드하여 얻습니다.
        /// </summary>
        /// <remarks>동일 타입의 스크립터블 오브젝트가 있으면 예외가 발생합니다</remarks>
        /// <returns>찾을 수 없으면 null이 리턴됩니다.</returns>
        public static T GetScriptableObject<T>() where T : ScriptableObject
        {
            var scriptableObjectName = typeof(T).FullName;
            var guids = AssetDatabase.FindAssets($"t:{scriptableObjectName}");
            if (guids.Length <= 0)
            {
                Debug.LogWarning($"{scriptableObjectName} 에셋을 찾을 수 없습니다");
                return null;
            }            
            if (guids.Length > 1)
            {
                Debug.LogWarning($"{scriptableObjectName} 에셋이 여러개 있어 한 개를 특정할 수 없습니다");
                return null;
            }
            var guid = guids[0];
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var scriptableObject = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
            if (scriptableObject == null)
            {
                Debug.LogWarning($"{scriptableObjectName} guid에 연결된 에셋이 없습니다");
                return null;
            }
            return scriptableObject;
        }

        /// <summary>
        /// 여러개의 스크립터블 오브젝트를 로드하여 얻습니다.
        /// </summary>
        /// <remarks>Missing된 guid는 드롭됩니다.</remarks>
        public static List<T> GetScriptableObjects<T>() where T : ScriptableObject
        {
            var scriptableObjectName = typeof(T).FullName;
            var guids = AssetDatabase.FindAssets($"t:{scriptableObjectName}");
            var assets = guids
                .Select(guid =>
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
                })
                .Where( asset => asset != null )
                .ToList();
            return assets;
        }  
    }
}