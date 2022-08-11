using System;
using UnityEngine;

namespace Rano.SoundSystem
{
    public class SoundLayer : MonoBehaviour
    {
        private static int ClassCount= 1;
        private bool _isMute;
        private AudioSource _audioSource;
        [SerializeField] private string _name;
        
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public float Volume
        {
            get
            {
                return _audioSource.volume;
            }
            set
            {
                _audioSource.volume = value;
            }
        }
        
        public bool IsMute
        {
            get
            {
                return _isMute;
            }
            set
            {
                _isMute = value;
                if (_isMute)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
        }

        void Awake()
        {
            if (String.IsNullOrEmpty(_name))
            {
                _name = $"UnknownSoundLayer{SoundLayer.ClassCount++}";
            }
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
        }

        public void Play(AudioClip audioClip, bool loop=false)
        {
            if (IsMute) return;
            _audioSource.clip = audioClip;
            _audioSource.loop = loop;
            _audioSource.Play();
        }

        public void PlayOneShot(AudioClip audioClip, float volume = 1.0f)
        {
            _audioSource.PlayOneShot(audioClip, volume);
        }

        public void Stop()
        {
            Log.Info($"Stop SoundLayer ({Name})");
            _audioSource.Stop();
        }

        public void Pause()
        {
            Log.Info($"Pause SoundLayer ({Name})");
            _audioSource.Pause();
        }

        public void Resume()
        {
            // 뮤트되지 않은 상태에서만 하용한다.
            if (!IsMute)
            {
                Log.Info($"Resume SoundLayer ({Name})");
                _audioSource.UnPause();
            }
        }
    }
}