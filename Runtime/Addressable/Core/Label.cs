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
    public struct Label
    {
        public string value;

        public Label(string label)
        {
            value = label;
        }

        public Path[] GetPath()
        {
            ResourceLocationMap map = AssetManagerUtils.GetResourceLocationMap();
            if (map != null)
            {
                string key = value;            
                if (map.Locations.ContainsKey(key))
                {
                    IList<IResourceLocation> locations;
                    locations = map.Locations[key];

                    Path[] pathArray = new Path[locations.Count];
                    for (int i=0; i<locations.Count; i++)
                    {
                        pathArray[i] = new Path(locations[i].InternalId);
                    }
                    return pathArray;
                }
                else
                {
                    throw new Exception($"GetPath Failed: Invalid Label Key: {value}");
                }
            }
            else
            {
                throw new Exception($"GetPath Failed: Not found ResourceLocationMap");
            }
        }

        public override string ToString()
        {
            return $"{value.ToString()}(Label)";
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }        
    }
}