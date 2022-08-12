#nullable enable

using System;
using DG.Tweening;
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
    public struct SSoundManagerData
    {
        public float masterVolume;
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
            
            // SaveableEntity 컴포넌트 부착한다.
            SaveableEntity saveableEntity = go.AddComponent<SaveableEntity>();
            saveableEntity.Id = $"{typeof(SoundManager)}.{soundLayerName}";
            
            // SoundLayer 상태를 복원한다.
            if (_saveableManager)
            {
                _saveableManager.Load(saveableEntity, true);
            }
            else
            {
                Log.Warning($"{nameof(SaveableManager)}가 없으므로 복원하지 않습니다 ({saveableEntity.Id})");
            }

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
            SSoundManagerData state = new SSoundManagerData
            {
                masterVolume = MasterVolume
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
        }
        
        public void RestoreState(object state)
        {
            var data = (SSoundManagerData)state;
            SetMasterVolume(data.masterVolume);
        }
        
        #endregion
    }
}
