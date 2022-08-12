using System;
using UnityEngine;
using DG.Tweening;
using Rano.SaveSystem;

namespace Rano.SoundSystem
{   
    [Serializable]
    public struct SSoundLayerData
    {
        public float volume;
        public bool mute;
    }
    
    public class SoundLayer : MonoBehaviour, ISaveLoadable
    {
        private const float DEFAULT_VOLUME = 1.0f;
        private const float DEFAULT_PITCH = 1.0f;
        private static int ClassCount = 1;
        
        private AudioSource _audioSource;
        private float _targetVolume = DEFAULT_VOLUME;
        private bool _isMute;
        
        [SerializeField] private string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public float CurrentVolume
        {
            get => _audioSource.volume;
            private set => _audioSource.volume = value;
        }
        
        public bool IsPlaying => _audioSource.isPlaying;
        public bool IsMute => _isMute;
        
        void Awake()
        {
            if (String.IsNullOrEmpty(_name))
            {
                _name = $"UnknownSoundLayer{SoundLayer.ClassCount++}";
            }
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.volume = _targetVolume;
            _audioSource.pitch = DEFAULT_PITCH;
            _audioSource.loop = false;
        }

        public void Play(AudioClip audioClip, bool loop=false)
        {
            if (audioClip == null) return;
            _audioSource.clip = audioClip;
            _audioSource.loop = loop;
            _audioSource.Play();
        }

        public void PlayOneShot(AudioClip audioClip, float volume = 1.0f)
        {
            if (audioClip == null) return;
            _audioSource.PlayOneShot(audioClip, volume);
        }

        public void Stop(float fadeDuration=0.25f)
        {
            Log.Info($"Stop SoundLayer ({Name})");
            _audioSource.DOFade(0f, fadeDuration)
                .OnComplete(() =>
                {
                    _audioSource.Stop();
                });
        }

        public void Pause(float fadeDuration=0.25f)
        {
            Log.Info($"Pause SoundLayer ({Name})");
            _audioSource.DOKill();
            _audioSource.DOFade(0f, fadeDuration)
                .OnComplete(() =>
                {
                    _audioSource.Pause();
                });
        }

        public void Resume(float fadeDuration=0.25f)
        {
            Log.Info($"Resume SoundLayer ({Name})");
            _audioSource.UnPause();
            _audioSource.DOFade(_targetVolume, fadeDuration);
        }

        public void SetVolume(float volume)
        {
            if (_isMute == true)
            {
                Log.Warning("뮤트중에는 볼륨을 설정할 수 없습니다");
                return;
            }
            _targetVolume = volume;
            _audioSource.volume = volume;
        }
        
        public void SetVolumeFade(float volume, float fadeDuration)
        {
            if (_isMute == true)
            {
                Log.Warning("뮤트중에는 볼륨을 설정할 수 없습니다");
                return;
            }
            _targetVolume = volume;
            _audioSource.DOFade(volume, fadeDuration);
        }

        public void SetMute(bool value)
        {
            if (_isMute == value) return;
            
            // 뮤트를 켜서 소리를 없애는 경우:
            if (value == true)
            {
                _isMute = true;
                _audioSource.volume = 0f;
                // TODO: _audioSource.mute = true;
                // TODO: _audioSource.mute = true;
                // TODO: _audioSource.mute = true;
                // TODO: _audioSource.mute = true;
                // TODO: _audioSource.mute = true;
            }
            // 뮤트를 끄고 소리를 나오게 하는 경우:
            else
            {
                _isMute = false;
                _audioSource.volume = _targetVolume;
            }         
        }
        
        public void SetMuteFade(bool value, float fadeDuration)
        {
            if (_isMute == value) return;
            
            // 뮤트를 켜서 소리를 없애는 경우:
            if (value == true)
            {
                _isMute = true;
                _audioSource.DOKill();
                _audioSource.DOFade(0f, fadeDuration);
            }
            // 뮤트를 끄고 소리를 나오게 하는 경우:
            else
            {
                _isMute = false;
                _audioSource.DOKill();
                _audioSource.DOFade(_targetVolume, fadeDuration);
            }         
        }
                
        #region Implementation of ISaveLoadable

        public void ClearState()
        {
            SetVolume(DEFAULT_VOLUME);
            SetMute(false);
        }
        
        public void DefaultState()
        {
            SetVolume(DEFAULT_VOLUME);
            SetMute(false);
        }
        
        public object CaptureState()
        {
            SSoundLayerData state = new SSoundLayerData
            {
                volume = _targetVolume,
                mute = _isMute
            };
            return state;
        }
        
        public void ValidateState(object state)
        {
            SSoundLayerData data = (SSoundLayerData)state;
            if (data.volume < 0f)
            {
                throw new StateValidateException($"{nameof(data.volume)}은 0이상이어야 합니다.");
            }
        }
        
        public void RestoreState(object state)
        {
            var data = (SSoundLayerData)state;
            SetVolume(data.volume);
            SetMute(data.mute);
        }
        
        #endregion
    }
}