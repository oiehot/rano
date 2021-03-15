// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.SceneManagement;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.ResourceManagement.ResourceProviders;

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
            return $"{value.ToString()}";
        }        
        
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

    public struct Label
    {
        public string value;

        public Label(string label)
        {
            value = label;
        }

        public override string ToString()
        {
            return $"{value.ToString()}";
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}