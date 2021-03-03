// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rano
{
    [RequireComponent (typeof(Text))]
    public class LocalizedFont : MonoBehaviour
    {
        private Text text;        
        private int initialFontSize;
        private float initialLineSpacing;

        void Start()
        {
            this.text = GetComponent<Text>();
            this.initialFontSize = text.fontSize;
            this.initialLineSpacing = text.lineSpacing;
            this.Refresh();
        }

        public void Refresh()
        {
            // Font
            LocalizationFont localizationFont;
            localizationFont = LocalizationManager.Instance.GetCurrentFont();
            this.text.font = localizationFont.font;
            this.text.fontSize = (int)(this.initialFontSize * localizationFont.scale);
            
            // Line Spacing Scale
            this.text.lineSpacing = this.initialLineSpacing * localizationFont.lineSpacingScale;
        }
    }
}
