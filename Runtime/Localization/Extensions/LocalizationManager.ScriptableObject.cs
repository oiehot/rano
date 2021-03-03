// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    public static partial class LocalizationManagerExtensions
    {   
        public static void LoadFromScriptableObject(this LocalizationManager manager, string path)
        {
            LocalizationData data;
            try
            {
                data = Resources.Load<LocalizationData>(path);
            }
            catch
            {
                throw new Exception($"Unable to load resource: {path}");
            }

            if (!data)
            {
                throw new Exception($"Unable to find resource: {path}");
            }
            
            manager.Load(data);
        }
    }
}