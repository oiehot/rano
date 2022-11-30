#nullable enable

using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Linq;
using System.Globalization;

namespace Rano.Localization
{
    public sealed class LocalizationManager : ManagerComponent
    {
        public event Action<CultureInfo>? OnLocaleChanged;

        public CultureInfo CurrentCultureInfo => LocalizationSettings.SelectedLocale.Identifier.CultureInfo;
        public string CurrentLocaleCode => LocalizationSettings.SelectedLocale.Identifier.CultureInfo.Name;

        protected override void Awake()
        {
            base.Awake();
            Log.Sys($"CurrentLocale: {CurrentLocaleCode}");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
        }

        protected override void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;
        }

        private void HandleLocaleChanged(Locale locale)
        {
            Log.Important($"Locale Changed ({locale.Identifier.Code})");
            UpdateLocalizables(locale.Identifier.CultureInfo);
        }

        /// <summary>
        /// 모든 ILocalizable 구현 컴포넌트들을 업데이트한다.
        /// </summary>
        /// <param name="cultureInfo"></param>
        private void UpdateLocalizables(CultureInfo cultureInfo)
        {
            var localizables =
                GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ILocalizable>();
            
            foreach (ILocalizable localizable in localizables)
            {
                localizable.OnLocaleChanged(cultureInfo);
            }
            
            OnLocaleChanged?.Invoke(cultureInfo);
        }
        
        public void SetLocale(string code)
        {
            LocaleIdentifier localeIdentifier = new LocaleIdentifier(code);
            SetLocale(localeIdentifier);
        }
        
        public void SetLocale(CultureInfo cultureInfo)
        {
            LocaleIdentifier localeIdentifier = new LocaleIdentifier(cultureInfo);
            SetLocale(localeIdentifier);
        }

        private void SetLocale(LocaleIdentifier localeIdentifier)
        {
            Log.Sys($"Set Locale ({localeIdentifier.Code})");
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(localeIdentifier);
            LocalizationSettings.SelectedLocale = locale;
        }
    }
}