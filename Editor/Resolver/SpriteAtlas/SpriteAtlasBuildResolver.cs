#if false

using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.U2D;
using UnityEngine;

namespace Rano.Editor.Resolver
{
    /// <summary>
    /// 빌드시 모든 SpriteAtlas 에셋의 IncludeInBuild를 꺼두고 완료되면 원상복구한다.
    /// </summary>
    /// <remarks>
    /// SpriteAtlas의 IncludeInBuild를 꺼둔 상태에서 PlayModeScript를 UseAssetDatabase로 둔채 테스트를 하면,
    /// SpriteAtlasManager가 스프라이트를 찾을 수가 없어 Request를 해야함에도, 시도하지 않아
    /// 스프라이트가 로드되지 않은채 빈 상자로 나온다.
    /// 
    /// IncludeInBuild가 켜져 있어야 UseAssetDatabase 실행모드에서 문제없이 테스트 할 수 있었다.
    /// 하지만 IncludeInBuild가 켜져 있으면 불필요하게 빌드에 포함되므로, 빌드시에만 꺼둘 필요가 있다.
    /// </remarks> 
    public class SpriteAtlasBuildResolver : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>
        /// 빌드시 모든 SpriteAtlas에셋들에 설정될 IncludeInBuild값.
        /// </summary>
        private const bool DEFAULT_INCLUDE_IN_BUILD = false;
        
        /// <summary>
        /// 빌드 전, 모든 SpriteAtlas에셋들의 IncludeInBuild 값을 보관하는 곳.
        /// </summary>
        private Dictionary<string, bool> _flags = new Dictionary<string, bool>();
        
        public int callbackOrder => 0;
		
        /// <summary>
        /// 빌드 전, 모든 SpriteAtlas에셋들의 IncludeInBuild를 기억한 뒤, 기본값(false)으로 설정한다.
        /// </summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            Log.Info("모든 SpriteAtlas 에셋의 IncludeInBuild를 끕니다.");
            _flags.Clear();
            var assetPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
            foreach (var assetPath in assetPaths)
            {
                var spriteAtlas = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SpriteAtlas)) as SpriteAtlas;
                if (spriteAtlas == null) continue;
                
                // 스프라이트 아틀라스 IncludeInBuild를 기억해둔다.
                _flags[assetPath] = spriteAtlas.GetIncludeInBuild();
                
                // 스프라이트 아틀라스의 IncludeInBuild를 끈다.
                spriteAtlas.SetIncludeInBuild(DEFAULT_INCLUDE_IN_BUILD);
                Debug.Assert(spriteAtlas.GetIncludeInBuild() == DEFAULT_INCLUDE_IN_BUILD);
            }
        }

        /// <summary>
        /// 빌드 후, 모든 SpriteAtlas에셋들의 IncludeInBuild값을 빌드 시작전 값으로 복구한다.
        /// </summary>
        public void OnPostprocessBuild(BuildReport report)
        {
            Log.Info("모든 SpriteAtlas 에셋의 IncludeInBuild를 원상복구합니다.");
            foreach (var kv in _flags)
            {
                var assetPath = kv.Key;
                var originalIncludeInBuild = kv.Value;
                var spriteAtlas = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SpriteAtlas)) as SpriteAtlas;
                if (spriteAtlas != null)
                {
                    // 스프라이트 아틀라스의 IncludeInBuild를 복구한다.
                    spriteAtlas.SetIncludeInBuild(originalIncludeInBuild);
                    Debug.Assert(spriteAtlas.GetIncludeInBuild() == originalIncludeInBuild);
                }
                else
                {
                    throw new BuildFailedException("빌드전에 로드했던 스프라이트 아틀라스를 다시 로드할 수 없습니다.");
                }
            }
            _flags.Clear();
        }
    }
}

#endif