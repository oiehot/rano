// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;

namespace Rano
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