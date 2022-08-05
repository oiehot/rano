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