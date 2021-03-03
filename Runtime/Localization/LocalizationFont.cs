// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;

namespace Rano
{
    public class LocalizationFont
    {
        public Font font;
        public float scale;
        public float lineSpacingScale;

        public LocalizationFont(Font font, float scale=1.0f, float lineSpacingScale=1.0f)
        {
            this.font = font;
            this.scale = scale;
            this.lineSpacingScale = lineSpacingScale;
        }
    }
}