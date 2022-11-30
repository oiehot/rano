using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rano.SoundSystem
{
    [CreateAssetMenu(fileName = "Playlist", menuName = "Rano/Sound/Playlist")]
    public sealed class PlaylistSO : ScriptableObject
    {
        public AssetReference[] tracks = Array.Empty<AssetReference>();
    }
}