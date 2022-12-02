#nullable enable

using System;

namespace Rano.SoundSystem
{
    [Serializable]
    public class SoundLayerData
    {
        public float volume;
        public bool mute;

        public SoundLayerData(float volume, bool mute)
        {
            this.volume = volume;
            this.mute = mute;
        }
    }
}