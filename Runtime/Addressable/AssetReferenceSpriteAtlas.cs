// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;

namespace Rano.Addressable
{
    [Serializable]
    public class AssetReferenceSpriteAtlas : AssetReferenceT<SpriteAtlas>
    {
        public AssetReferenceSpriteAtlas(string guid) : base(guid) {}
    }
}