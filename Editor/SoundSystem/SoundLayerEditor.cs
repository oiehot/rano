#if false
using Rano;
using UnityEngine;
using UnityEditor;
using Rano.SoundSystem;

namespace RanoEditor.Inspector
{
    [CustomEditor(typeof(SoundLayer))]
    public class SoundLayerEditor : Editor
    {
        private AudioSource _audioSource;
        private SoundLayer _soundLayer;

        private void OnEnable()
        {
            _soundLayer = target as SoundLayer;
            if (_soundLayer)
            {
                _audioSource = _soundLayer.gameObject.GetComponent<AudioSource>();
            }
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (_soundLayer && _audioSource)
            {
                _soundLayer.TargetVolume = _audioSource.volume;
                Log.Info($"SetTargetVolumeTo {_soundLayer.TargetVolume}");
            }
        }
    }
}
#endif