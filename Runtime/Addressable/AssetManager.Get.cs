// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Rano.Addressable
{
    public partial class AssetManager : MonoBehaviour, IAssetManager
    {
        public T Get<T>(Address address)
        {
            if (this.assets.ContainsKey(address))
            {
                return (T)this.assets[address];
            }
            else
            {
                throw new Exception($"리소스가 로드되어 있지 않음: {address.value} (address)");
            }
        }

        public T Get<T>(AssetReference assetReference)
        {
            Address address = this.GetAddress(assetReference);
            return this.Get<T>(address);
        }

        public GameObject Instantiate(Address address)
        {
            throw new NotImplementedException();
        }
    }
}