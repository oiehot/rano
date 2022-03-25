// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;

namespace Rano
{
    public static class ColorExtensions
    {
        /// <summary>
        /// 해당컬러에서 특정 알파로 바꾼 컬러를 리턴한다.
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return color * new Color(1f, 1f, 1f, alpha);
        }
    }
}