#if false

using System;
using UnityEngine;

namespace Rano.Localization
{
    [System.Serializable]
    public class LocalizationItem
    {
        public string key;
        public string value;

        public LocalizationItem(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    
    [CreateAssetMenu(menuName="Rano/Localization Data", order=int.MaxValue)]
    [System.Serializable]
    public class LocalizationData : ScriptableObject
    {
        public LocalizationItem[] items;
    }
}

#endif