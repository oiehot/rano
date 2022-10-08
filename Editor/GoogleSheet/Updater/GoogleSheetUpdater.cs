#nullable enable

using UnityEditor;
using Rano.GoogleSheet;
using System;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

namespace Rano.Editor.GoogleSheet
{
    public static class GoogleSheetUpdater
    {
        [MenuItem("Build/Update all GoogleSheets", false, 500)]
        public static async Task<bool> UpdateAll()
        {
            Log.Important("모든 구글 시트 업데이트 업데이트 중...");
            
            // UpdatableGoogleSheetSO 타입에서 파생된 모든 에셋 GUID를 얻는다.  
            Type baseClass = typeof(UpdatableSheetSO<>);
            string query = $"t:{typeof(UpdatableSheetSO<>)}";

            string[]? guids = null;
            guids = AssetDatabase.FindAssets(query);
            
            if (guids == null)
            {
                Log.Warning("구글 시트 에셋 검색 실패 (결과가 비어 있음)");
                return false;
            }
            
            if (guids.Length <= 0)
            {
                Log.Info("업데이트 할 구글 시트가 없음");
                return true;
            }
            
            foreach (string guid in guids)
            {
                // Guid로 에셋경로를 얻는다.
                string? assetPath;
                assetPath = AssetDatabase.GUIDToAssetPath(guid);
                
                if (string.IsNullOrEmpty(assetPath))
                {
                    Log.Warning($"아이템 업데이트 생략 ({guid}, Guid로 에셋 경로를 얻는데 실패)");
                    continue;
                }
                
                Log.Info($"구글 시트 업데이트 중... ({assetPath})");
                
                // 이 에셋의 타입을 얻는다. (ex: DecoSheetSO)
                Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                
                // 에셋을 로드한다.
                ScriptableObject? asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType) as ScriptableObject;
                if (asset == null)
                {
                    Log.Warning($"아이템 업데이트 생략 ({assetPath}, 에셋을 로드하는데 실패)");
                    continue;
                }
                
                // 업데이트 메서드를 얻는다.
                // MethodInfo updateMethod = assetType.GetMethod("UpdateFromGoogleSheetAsync", BindingFlags.Public);
                MethodInfo? runtimeUpdateMethod = assetType.GetRuntimeMethod("UpdateFromGoogleSheetAsync", new Type[] { });
                if (runtimeUpdateMethod == null)
                {
                    Log.Warning($"아이템 업데이트 생략 ({assetPath}, 클래스에서 업데이트 메서드를 찾을 수 없음)");
                    continue;
                }
                
                // 에셋의 업데이트 메서드를 실행한다.
                bool result = await (Task<bool>)runtimeUpdateMethod.Invoke(asset, new object[] { });
                if (result == false)
                {
                    Log.Warning($"구글 시트 업데이트 실패 ({assetPath})");
                    return false;
                }
            }
            
            Log.Important("모든 구글 시트 업데이트 완료");
            return true;
        }
    }
}