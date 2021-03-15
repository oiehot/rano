// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Rano.Addressable
{
    public partial class AssetManager : MonoBehaviour, IAssetManager
    {
        private Dictionary<Address, object> assets;
        
        void Awake()
        {
            this.assets = new Dictionary<Address, object>();
        }
    }
}