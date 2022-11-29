#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.U2D;
using Rano.IO;

namespace Rano.Editor.Resolver
{   
    /// <summary>
    /// SpriteAtlas를 어드레서블로 사용시 UseAssetDatabase 모드로 플레이 하면
    /// 지연 로드시 요청 및 등록이 되지 않아 스프라이트 나오지 않는 문제가 발생한다.
    /// 이 문제를 해결하기 위해 플레이시 모든 SpriteAtlas 에셋의 IncludeInBuild를 켜고 종료되면 원상 복구한다.
    /// </summary>
    /// <remarks>
    /// SpriteAtlas의 IncludeInBuild를 꺼둔 상태에서 PlayModeScript를 UseAssetDatabase로 둔채 테스트를 하면,
    /// SpriteAtlasManager가 스프라이트를 찾을 수가 없어 Request를 해야함에도 시도하지 않아
    /// 스프라이트가 로드되지 않은채 빈 상자로 나올 때가 많다.
    /// IncludeInBuild가 켜져 있어야 UseAssetDatabase 모드에서 문제없이 테스트할 수 있다.
    /// </remarks>
    /// TODO: 추후 UseAssetDatabase 모드로 설정된 경우만 작동되도록 할 것.
    [InitializeOnLoad]
    public static class SpriteAtlasAddressableResolver
    {
        private static string _flagsPrefsKey = $"__{nameof(SpriteAtlasAddressableResolver)}_Flags";
        
        static SpriteAtlasAddressableResolver()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    OnBeforePlay();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    OnAfterPlay();
                    break;
                default:
                    throw new Exception($"지원하지 않는 {nameof(PlayModeStateChange)}");
            }
        }

        /// <summary>
        /// Play 모드로 들어가기 전, 모든 SpriteAtlas의 IncludeInBuild 값을 기억하고 켠다.
        /// </summary>
        /// <remarks>
        /// Use Asset Database로 플레이할 때 SpriteAtlas의 IncludeInBuild가 꺼져 있는 경우 지연 로딩 요청이 누락되어
        /// 스프라이트가 로드되지 않는 문제를 방지하고자 함. 에디터에서만 발생하는 것으로 보이고 특정 플랫폼 빌드시에는 발생하지 않았음.
        /// </remarks>
        private static void OnBeforePlay()
        {
            Log.Info("모든 SpriteAtlas의 IncludeInBuild 값을 켭니다");

            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            string[] guids = AssetDatabase.FindAssets ($"t:{nameof(SpriteAtlas)}", null);
            foreach (string guid in guids)
            {
                // 스프라이트 아틀라스 에셋의 경로를 얻고 로드한다. 
                string spriteAtlasAssetPath = (AssetDatabase.GUIDToAssetPath(guid));
                SpriteAtlas? spriteAtlas = AssetDatabase.LoadAssetAtPath(spriteAtlasAssetPath, typeof(SpriteAtlas)) as SpriteAtlas;
                if (spriteAtlas == null)
                {
                    Log.Warning($"스프라이트 에셋의 경로를 얻을 수 없습니다 ({guid})");
                    continue;
                }
                
                // 현재 IncludeInBuild를 기억해둔다.
                bool currentIncludeInBuild = spriteAtlas.IsIncludeInBuild();
                flags[spriteAtlasAssetPath] = currentIncludeInBuild;
                
                // IncludeInBuild를 켠다.
                // Log.Info($"스프라이트 아틀라스의 IncludeInBuild 를 켭니다 ({spriteAtlasAssetPath})");
                spriteAtlas.SetIncludeInBuild(true);
            }
            
            // Log.Info("스프라이트 아틀라스 플래그 데이터를 PlayerPrefs에 저장합니다");
            Prefs.SetObject(_flagsPrefsKey, flags);
        }

        /// <summary>
        /// Play 모드에서 나온 후, 모든 SpriteAtlas의 IncludeInBuild 값을 복원한다.
        /// </summary>
        /// <remarks>
        /// 복원 할 때 사용하는 IncludeInBuild는 Play 모드 시작 전 PlayerPrefs에 저장해둔 값을 사용한다.
        /// </remarks>
        private static void OnAfterPlay()
        {
            Log.Info("모든 SpriteAtlas의 IncludeInBuild 값을 원상복구 시킵니다");

            Dictionary<string, bool>? flags = Prefs.GetObject(_flagsPrefsKey) as Dictionary<string, bool>;
            if (flags == null)
            {
                Log.Warning("PlayerPrefs로 부터 스프라이트 아틀라스 플래그 데이터를 얻을 수 없음");
                return;
            }
            
            foreach (KeyValuePair<string,bool> kv in flags)
            {
                string spriteAtlasAssetPath = kv.Key;
                bool previousIncludeInBuild = kv.Value;

                SpriteAtlas? spriteAtlas;
                try
                {
                    spriteAtlas = AssetDatabase.LoadAssetAtPath(spriteAtlasAssetPath, typeof(SpriteAtlas)) as SpriteAtlas;
                }
                catch
                {
                    Log.Warning($"스프라이트 아틀라스를 얻을 수 없음 ({spriteAtlasAssetPath}, 예외 발생)");
                    continue;
                }
                
                if (spriteAtlas == null)
                {
                    Log.Warning($"스프라이트 아틀라스를 얻을 수 없음 ({spriteAtlasAssetPath}, 비어 있음)");
                    continue;
                }
                
                // Log.Info($"스프라이트 아틀라스의 IncludeInBuild 값을 복구합니다 (path:{spriteAtlasAssetPath}, includeInBuild:{previousIncludeInBuild})");
                spriteAtlas.SetIncludeInBuild(previousIncludeInBuild);
            }
            
            // PlayerPrefs 데이터를 제거한다.
            Prefs.DeleteKey(_flagsPrefsKey);
        }
    }
}