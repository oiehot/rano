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
    public static class AssetManagerUtils
    {
        /// <summary>Get Path from AssetReference</summary>
        public static Path GetPath(AssetReference assetReference)
        {
            ResourceLocationMap map = AssetManagerUtils.GetResourceLocationMap();
            if (map != null)
            {
                string key = assetReference.RuntimeKey.ToString(); // Hash128 to String
                if (map.Locations.ContainsKey(key))
                {
                    IList<IResourceLocation> locations;
                    locations = map.Locations[key];
                    if (locations.Count >= 2)
                    {
                        throw new Exception($"GetPath 실패: 이 {key}(AssetReference) 는 여러개의 리소스 로케이션 데이터가 있음.");
                    }
                    
                    return new Path(locations[0].InternalId);
                }
                else
                {
                    throw new Exception($"GetPath 실패: 잘못된 키: {key}(AssetReference)");
                }
            }
            else
            {
                throw new Exception($"GetPath 실패: ResourceLocationMap을 찾을수 없음");
            }
        }

        /// <summary>어드레서블 리소스 로케이션 맵을 얻는다</summary>
        public static ResourceLocationMap GetResourceLocationMap()
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

        /// <summary>어드레서블 리소스 맵 리스트를 로그로 출력한다.</summary>
        public static void DebugResourceLocationMap()
        {
            ResourceLocationMap map = AssetManagerUtils.GetResourceLocationMap();
            if (map != null)
            {       
                foreach (KeyValuePair<object, IList<IResourceLocation>> item in map.Locations)
                {
                    Log.Info($"* key: {item.Key} ({item.Key.GetType()})");
                    foreach(var value in item.Value)
                    {
                        Log.Info($"    value: {value}");
                    }
                }
            }
        }
    }
}