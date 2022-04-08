#if false

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Linq;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using RanoEditor.Extensions;

namespace RanoEditor.Build
{
	/// <summary>
	/// 모든 스프라이트 아틀라스의 IncludeInBuild를 체크하고, 켜져(On) 있으면 빌드를 중단한다.
	/// </summary>
	public class SpriteAtlasChecker : IPreprocessBuildWithReport
	{
		public int callbackOrder => 0;	
		public void OnPreprocessBuild(BuildReport report)
		{
	        var assets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
	        var items = assets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(SpriteAtlas)) as SpriteAtlas)
		        .Where(item => item != null).ToArray();
	        foreach (var item in items)
	        {
		        if (item.GetIncludeInBuild() == true)
		        {
			        string assetPath = AssetDatabase.GetAssetPath(item);
			        throw new BuildFailedException($"{assetPath}의 IncludeInBuild가 꺼져 있어야 빌드를 진행할 수 있습니다.");			        
		        }
	        }
        }
	}
}
#endif