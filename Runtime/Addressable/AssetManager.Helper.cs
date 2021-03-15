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
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Rano.Addressable
{
    public partial class AssetManager : MonoBehaviour, IAssetManager
    {
        // TODO: Go Helper(Utils)
        public Address GetAddress(AssetReference assetReference)
        {
            ResourceLocationMap map = this.GetResourceLocationMap();
            if (map != null)
            {
                string key = assetReference.ToString();
                // assetReference를 String으로 반환하면 대괄호가 붙는다. 이를 제거해야 맵 매칭되는 로케이션을 얻을 수 있다.
                // ex) [12345...] => 12345...
                key = key.Substring(1, key.Length-2);             
                if (map.Locations.ContainsKey(key))
                {
                    IList<IResourceLocation> locations;
                    locations = map.Locations[key];
                    if (locations.Count >= 2)
                    {
                        throw new Exception($"이 에셋레퍼런스({key})는 여러개의 리소스 로케이션 데이터가 있음.");
                    }    
                    return new Address(locations[0].InternalId);
                }
                else
                {
                    throw new Exception($"GetAddress Failed: Invalid Key: {key}");
                }
            }
            else
            {
                throw new Exception($"GetAddress Failed: Not found ResourceLocationMap");
            }
        }

        // // TODO: Go Helper(Utils)
        // public List<Address> GetAddress(Label label)
        // {
        //     throw new NotImplementedException();

        // }

        // TODO: Go Helper(Utils)
        private ResourceLocationMap GetResourceLocationMap()
        {
            foreach(var locator in Addressables.ResourceLocators)
            {
                if (locator is ResourceLocationMap)
                {
                    ResourceLocationMap map = (ResourceLocationMap)locator;
                    return map;
                }
            }
            return null;
        }

        // TODO: Go Helper(Utils)
        public void DebugResourceLocationMap()
        {
            ResourceLocationMap map = this.GetResourceLocationMap();
            if (map != null)
            {       
                foreach (KeyValuePair<object, IList<IResourceLocation>> item in map.Locations)
                {
                    Log.Info($"* key: {item.Key} ({item.Key.GetType()})");
                    foreach(var value in item.Value)
                    {
                        Log.Info($"    value: {value}"); // ({value.GetType()})");
                    }
                }
            }
        }
    }
}