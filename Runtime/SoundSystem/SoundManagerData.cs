#nullable enable

using System;
using System.Collections.Generic;

namespace Rano.SoundSystem
{
    [Serializable]
    public class SoundManagerData
    {
        public float masterVolume;
        public Dictionary<String, SoundLayerData> soundLayers;

        public SoundManagerData(float masterVolume, Dictionary<string, SoundLayerData> soundLayers)
        {
            this.masterVolume = masterVolume;
            this.soundLayers = soundLayers;
        }
    }
}