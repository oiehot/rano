#if false

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rano;

namespace Rano.Localization
{
    public sealed class LocalizationManager : ManagerComponent
    {
        private LocalizationLanguage language;
        private Dictionary<string, string> dict;
        private Dictionary<LocalizationLanguage, LocalizationFont> fonts;

        protected override void Awake()
        {
            base.Awake();
            this.dict = new Dictionary<string, string>();
            this.fonts = new Dictionary<LocalizationLanguage, LocalizationFont>();
        }
        
        public void SetLanguage(LocalizationLanguage language)
        {
            if (this.language == language) return;

            if (this.HasFont(language))
            {   
                this.language = language;
            }
            else
            {
                Log.Warning($"폰트가 등록되지 않은 언어입니다: {language.ToString()}");
                Log.Warning("대체언어로 English를 사용합니다");
                this.language = new LocalizationLanguage(SystemLanguage.English);
            }

            Log.Info($"Set Language to: {this.language.ToString()}");
        }

        public LocalizationLanguage GetCurrentLanguage()
        {
            return this.language;
        }

        public LocalizationLanguage GetSystemLanguage()
        {
            return new LocalizationLanguage(Application.systemLanguage);
        }
        
        public void Load(LocalizationData data)
        {
            this.Clear();
            for (int i=0; i<data.items.Length; i++)
            {
                this.Add(data.items[i].key, data.items[i].value);
            }
        }

        public void Add(string key, string value)
        {
            this.dict[key] = value;
        }
        
        public string GetValue(string key)
        {
            if (this.dict.ContainsKey(key))
            {
                return this.dict[key];
            }
            else
            {
                throw new Exception($"No value found from key: {key}");
            }
        }

        public void Clear()
        {
            this.dict.Clear();
        }

        public void AddFont(LocalizationLanguage language, LocalizationFont font)
        {
            if (this.fonts.ContainsKey(language))
            {
                Log.Warning($"언어:{language.ToString()} 에 해당하는 폰트는 이미 로드되어 있습니다.");
                return;
            }
            this.fonts.Add(language, font);
        }

        public LocalizationFont GetFont(LocalizationLanguage language)
        {
            if (this.fonts.ContainsKey(language))
            {
                return this.fonts[language];
            }
            else
            {
                Log.Warning($"언어: {language.ToString()}에 맞는 폰트가 없음.");
                return null;
            }
        }

        public LocalizationFont GetCurrentFont()
        {
            LocalizationFont font = this.GetFont(this.language);
            return font;
        }

        public bool HasFont(LocalizationLanguage language)
        {
            if (this.fonts.ContainsKey(language)) return true;
            else return false;
        }

        /// <summary>씬 안의 모든 LocalizedText 컴포넌트를 리프레시한다</summary>
        public void Refresh()
        {
            LocalizedText[] texts = FindObjectsOfType<LocalizedText>();
            
            foreach (LocalizedText text in texts)
            {
                text.Refresh();
            }
        }
    }
}

#endif