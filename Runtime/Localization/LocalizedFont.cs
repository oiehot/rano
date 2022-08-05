#if false

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rano.Localization
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

#endif