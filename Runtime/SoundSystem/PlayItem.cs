#nullable enable

using UnityEngine;

namespace Rano.SoundSystem
{
    public sealed class PlayItem
    {
        public AudioClip? AudioClip { get; set; }
        public bool IsPlayed { get; set; }

        public PlayItem(AudioClip audioClip)
        {
            AudioClip = audioClip;
            IsPlayed = false;
        }
    }
}