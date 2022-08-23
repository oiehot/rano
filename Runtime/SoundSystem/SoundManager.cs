#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using Rano.SaveSystem;

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

    [Serializable]
    public struct SSoundLayerData
    {
        public float volume;
        public bool mute;
    }
    
    [Serializable]
    public struct SSoundManagerData
    {
        public float masterVolume;
        public Dictionary<String, SSoundLayerData> soundLayers;
    }

    public sealed class SoundManager : ManagerComponent, ISaveLoadable
    {
        private const float DEFAULT_MASTER_VOLUME = 1.0f;
        private SoundLayer[] _soundLayers;
        private AudioListener? _audioListener;
        private SaveableManager? _saveableManager;

        public float MasterVolume => AudioListener.volume;

        protected override void Awake()
        {
            base.Awake();

            _saveableManager = GameObject.FindObjectOfType<SaveableManager>();

            SetMasterVolume(DEFAULT_MASTER_VOLUME);
            
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

        private SoundLayer CreateSoundLayer(string soundLayerName)
        {   
            Log.Info($"Create a SoundLayer ({soundLayerName})");
            
            // GameObject를 생성한다.
            GameObject go = new GameObject(soundLayerName);
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.transform.parent = gameObject.transform;
            
            // SoundLayer 컴포넌트 부착한다.
            SoundLayer soundLayer = go.AddComponent<SoundLayer>();
            soundLayer.Name = soundLayerName;

            return soundLayer;
        }

        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = volume;
        }

        public void SetMasterVolumeFade(float volume, float fadeDuration = 0.25f)
        {
            throw new NotImplementedException();
        }

        public SoundLayer? GetSoundLayer(ESoundLayerType soundLayerType)
        {
            return _soundLayers[(int)soundLayerType];
        }
        
        public void PlayOneShot(ESoundLayerType soundLayerType, AudioClip audioClip, float volume=1.0f)
        {
            GetSoundLayer(soundLayerType)?.PlayOneShot(audioClip, volume);
        }
        
        public void Play(ESoundLayerType soundLayerType, AudioClip audioClip, bool loop=false)
        {
            GetSoundLayer(soundLayerType)?.Play(audioClip, loop);
        }

        public void Pause(ESoundLayerType soundLayerType)
        {
            GetSoundLayer(soundLayerType)?.Pause();
        }
        
        public void Resume(ESoundLayerType soundLayerType)
        {
            GetSoundLayer(soundLayerType)?.Resume();
        }        

        public void Stop(ESoundLayerType soundLayerType)
        {
            GetSoundLayer(soundLayerType)?.Stop();
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

        public bool IsAllMuted(ESoundLayerType[] soundLayerTypes)
        {
            foreach (ESoundLayerType soundLayerType in soundLayerTypes)
            {
                if (GetSoundLayer(soundLayerType)?.IsMute == false) return false;
            }
            return true;
        }
        
        #endregion
        
        #region Implementation of ISaveLoadable

        public void ClearState()
        {
            SetMasterVolume(DEFAULT_MASTER_VOLUME);
        }
        
        public void DefaultState()
        {
            SetMasterVolume(DEFAULT_MASTER_VOLUME);
        }
        
        public object CaptureState()
        {
            // SoundLayer 들의 상태를 준비한다.
            Dictionary<String, SSoundLayerData> soundLayerDatas = new Dictionary<string, SSoundLayerData>();
            
            for (int i=0; i < _soundLayers.Length; i++)
            {
                ESoundLayerType soundLayerType = (ESoundLayerType)i;
                SSoundLayerData soundLayerData = new SSoundLayerData
                {
                    mute = _soundLayers[i].IsMute,
                    volume = _soundLayers[i].TargetVolume
                };
                soundLayerDatas.Add(soundLayerType.ToString(), soundLayerData);
            }
            
            // SoundManager의 상태를 준비한다.
            SSoundManagerData state = new SSoundManagerData
            {
                masterVolume = MasterVolume,
                soundLayers = soundLayerDatas
            };
            
            return state;
        }
        
        public void ValidateState(object state)
        {
            SSoundManagerData data = (SSoundManagerData) state;
            if (data.masterVolume < 0f || data.masterVolume > 1f)
            {
                throw new StateValidateException("masterVolume은 0이상 1이하여야 합니다.");
            }

            foreach (KeyValuePair<string, SSoundLayerData> kv in data.soundLayers)
            {
                string soundLayerName = kv.Key; 
                SSoundLayerData soundLayerData = kv.Value;
                float volume = soundLayerData.volume;
                if (volume < 0f || volume > 1f)
                {
                    throw new StateValidateException($"사운드레이어의 볼륨은 0이상 1이하여야 합니다 (soundLayer: {soundLayerName})");
                }
            }
        }
        
        public void RestoreState(object state)
        {
            var data = (SSoundManagerData)state;

            // SoundManager 컴포넌트의 복구
            SetMasterVolume(data.masterVolume);
            
            // SoundLayer들의 복구
            foreach (KeyValuePair<string, SSoundLayerData> soundLayerDataKv in data.soundLayers)
            {
                string soundLayerName = soundLayerDataKv.Key;
                SSoundLayerData soundLayerData = soundLayerDataKv.Value;

                ESoundLayerType soundLayerType;
                if (ESoundLayerType.TryParse(soundLayerName, out soundLayerType) == true)
                {
                    int idx = (int)soundLayerType;
                    _soundLayers[idx].SetVolume(soundLayerData.volume);
                    _soundLayers[idx].SetMute(soundLayerData.mute);
                }
            }
        }
        
        #endregion
    }
}
