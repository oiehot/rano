// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine.U2D;

namespace RanoEditor.Extensions
{
    public static class SpriteAtlasAssetExtensions
    {
        /// <summary>
        /// 스프라이트 아틀라스 에셋에서 Include In Build 값을 얻는다.
        /// 참고: https://forum.unity.com/threads/solved-including-spriteatlases-dependent-on-platform.508110/
        /// </summary>
        public static bool GetIncludeInBuild(this SpriteAtlas spriteAtlasAsset)
        {
            var obj = new UnityEditor.SerializedObject(spriteAtlasAsset);
            var iter = obj.GetIterator();
            while (iter.Next(true))
            {
                if (iter.name == "bindAsDefault")
                {
                    return iter.boolValue;
                }
            }
            throw new Exception("SpriteAtlasAsset 에서 IncludeInBuild 속성값을 찾을 수 없음");
        }
    }
}
