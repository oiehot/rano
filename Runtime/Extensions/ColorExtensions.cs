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
        /// �ش��÷����� Ư�� ���ķ� �ٲ� �÷��� �����Ѵ�.
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return color * new Color(1f, 1f, 1f, alpha);
        }
    }
}