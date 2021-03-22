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
    public struct Address
    {
        public string value;
        
        public Address(string address)
        {
            value = address;
        }

        public override string ToString()
        {
            return $"{value.ToString()}(Address)";
        }        
        
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public Path GetPath()
        {
            ResourceLocationMap map = AssetManagerUtils.GetResourceLocationMap();
            if (map != null)
            {
                string key = value;
                if (map.Locations.ContainsKey(key))
                {
                    IList<IResourceLocation> locations;
                    locations = map.Locations[key];
                    if (locations.Count >= 2)
                    {
                        throw new Exception($"GetPath Failed: 이 {key}(Address) 는 여러개의 리소스 로케이션 데이터가 있음.");
                    }
                    
                    return new Path(locations[0].InternalId);
                }
                else
                {
                    throw new Exception($"GetPath Failed: Invalid Key: {key}(AssetReference)");
                }
            }
            else
            {
                throw new Exception($"GetPath Failed: Not found ResourceLocationMap");
            }
        }

        public static explicit operator Path(Address address)
        {
            return address.GetPath();
        }
    }
}