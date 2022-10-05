#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Rano.Editor
{
    public static class AssetDatabaseHelper
    {
        public static T[] LoadSubAssetsFromName<T>(string subAssetName, string[]? searchInFolders=null) where T : Object
        {
            List<T> objects = new List<T>();
            if (searchInFolders == null) searchInFolders = new string[]{};
            
            // 이름으로 에셋을 찾는다. 서브 에셋인 경우 대표 에셋 경로를 얻는다.
            string typeName = typeof(T).Name;
            string[] guids = AssetDatabase.FindAssets($"t:{typeName} {subAssetName}", searchInFolders);
            if (guids.Length <= 0) return new T[]{};
            string[] assetPaths = Array.ConvertAll(guids, guid => {
                return AssetDatabase.GUIDToAssetPath(guid);
            });
            
            // 에셋을 찾았다면 이름이 동일한 서브 에셋을 모은다.
            foreach (string assetPath in assetPaths)
            {
                T[] subAssets = LoadSubAssetsFromAssetPath<T>(assetPath, subAssetName);
                if (subAssets.Length > 0)
                {
                    objects = objects.Concat(subAssets).ToList();
                }
            }
            
            return objects.ToArray();
        }
        
        public static T? LoadSubAssetFromName<T>(string subAssetName, string[]? searchInFolders=null) where T : Object
        {
            T[] objects = LoadSubAssetsFromName<T>(subAssetName, searchInFolders);
            if (objects.Length > 0) return objects[0];
            else return null;
        }
        
        public static T[] LoadSubAssetsFromAssetPath<T>(string assetPath, string subAssetName) where T : Object
        {
            Object[] datas = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            List<T> results = new List<T>();
            foreach (Object data in datas)
            {
                if (data.GetType() != typeof(T)) continue;
                if (data.name != subAssetName) continue;
                T result = data as T; 
                results.Add(result);
            }
            return results.ToArray();
        }
        
        public static T? LoadSubAssetFromAssetPath<T>(string assetPath, string subAssetName) where T : Object
        {
            T[] results = LoadSubAssetsFromAssetPath<T>(assetPath, subAssetName);
            if (results.Length > 0) return results[0];
            else return null;
        }
        
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