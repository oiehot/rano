// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using Rano;
using Rano.App;
using System.Reflection;
using UnityEditor.AddressableAssets;

namespace RanoEditor.Build
{
    public interface IPreprocessAddressableBuild
    {
        int callbackOrder { get; }
        void OnPreprocessAddressableBuild();
    }

    public interface IPostprocessAddressableBuild
    {
        int callbackOrder { get; }
        void OnPostprocessAddressableBuild();
    }
}