#if false

using System;
using UnityEngine;

namespace Rano.Localization
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

#endif