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
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] public string key;
        private int initialFontSize;
        private Text text;

        void Awake()
        {
            this.text = GetComponent<Text>();
            this.initialFontSize = text.fontSize;
        }
    
        void OnEnable()
        {
            this.Refresh();
        }

        public void Refresh()
        {
            // Font
            LocalizationFont localizationFont;
            localizationFont = LocalizationManager.Instance.GetCurrentFont();
            this.text.font = localizationFont.font;
            this.text.fontSize = (int)(this.initialFontSize * localizationFont.scale);

            // Text
            try
            {
                this.text.text = LocalizationManager.Instance.GetValue(this.key);
            }
            catch
            {
                Log.Warning($"사전에서 단어를 찾을 수 없음: {this.key}");
                this.text.text = "?";
            }
        }
    }
}
