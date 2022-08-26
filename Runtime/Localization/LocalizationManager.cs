using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Linq;

namespace Rano.Localization
{
    public sealed class LocalizationManager : ManagerComponent
    {
        public Action OnLocaleChanged { get; set; }
        
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
            Log.Important($"Locale Changed ({locale})");
            StartCoroutine(nameof(UpdateLocalizeStringCoroutine));
        }

        public IEnumerator UpdateLocalizeStringCoroutine()
        {
            var nodes = GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<ILocalizable>();
            foreach (ILocalizable node in nodes)
            {
                node.UpdateLocalizableString();
                yield return null;
            }
            OnLocaleChanged?.Invoke();
        }
    }
}