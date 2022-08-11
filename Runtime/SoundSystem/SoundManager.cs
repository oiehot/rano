using System;
using UnityEngine;

namespace Rano.SoundSystem
{
    public enum ESoundLayerType
    {
        Unknown = 0,
        Background,
        Prop,
        Character,
        FX,
        UI,
        System
    }
    
    public sealed class SoundManager : ManagerComponent
    {
        private SoundLayer[] _soundLayers;
        private AudioListener _audioListener;

        public float MasterVolume
        {
            get
            {
                return AudioListener.volume;
            }
            set
            {
                AudioListener.volume = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            int soundlayerSize = System.Enum.GetValues(typeof(ESoundLayerType)).Length;
            _soundLayers = new SoundLayer[soundlayerSize];

            // 현재 씬에 AudioListener가 없다면(보통 카메라에 있음) 이 사운드 매니져에 장착한다.
            _audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
            if (_audioListener == null)
            {
                Log.Warning($"{nameof(AudioListener)} 컴포넌트를 찾을 수 없어 생성합니다.");
                _audioListener = gameObject.AddComponent<AudioListener>();
            }
            
            // 사운드 레이어 생성 후 캐싱
            foreach (ESoundLayerType soundLayerType in Enum.GetValues(typeof(ESoundLayerType)))
            {
                SoundLayer soundLayer = CreateSoundLayer(soundLayerType.ToString());
                int idx = (int)soundLayerType;
                _soundLayers[idx] = soundLayer;
            }
        }

        private SoundLayer CreateSoundLayer(string soundLayerName, float volume=1.0f)
        {
            Log.Info($"Create a SoundLayer ({soundLayerName})");
            
            GameObject go = new GameObject(soundLayerName);
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.transform.parent = gameObject.transform;
            
            SoundLayer soundLayer = go.AddComponent<SoundLayer>();
            soundLayer.Name = soundLayerName;
            soundLayer.Volume = volume;
            
            return soundLayer;
        }

        public SoundLayer GetSoundLayer(ESoundLayerType soundLayerType)
        {
            return _soundLayers[(int)soundLayerType];
        }
        
        public void PlayOneShot(ESoundLayerType soundLayerType, AudioClip audioClip, float volume=1.0f)
        {
            GetSoundLayer(soundLayerType).PlayOneShot(audioClip, volume);
        }
        
        public void Play(ESoundLayerType soundLayerType, AudioClip audioClip, bool loop=false)
        {
            GetSoundLayer(soundLayerType).Play(audioClip, loop);
        }

        public void Pause(ESoundLayerType soundLayerType)
        {
            GetSoundLayer(soundLayerType).Pause();
        }
        
        public void Resume(ESoundLayerType soundLayerType)
        {
            GetSoundLayer(soundLayerType).Resume();
        }        

        public void Stop(ESoundLayerType soundLayerType)
        {
            GetSoundLayer(soundLayerType).Stop();
        }
        
        #region Method for All SoundLayers

        public void StopAll()
        {
            foreach(SoundLayer soundLayer in _soundLayers)
            {
                soundLayer.Stop();
            }
        }

        public void PauseAll()
        {
            foreach(SoundLayer soundLayer in _soundLayers)
            {
                soundLayer.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach(SoundLayer soundLayer in _soundLayers)
            {
                soundLayer.Resume();
            }
        }
        
        #endregion
    }
}
