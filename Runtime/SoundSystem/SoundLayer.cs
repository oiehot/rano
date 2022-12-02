using System;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Rano.SoundSystem
{
    public class SoundLayer : MonoBehaviour
    {
        private const float DEFAULT_VOLUME = 1.0f;
        private const float DEFAULT_PITCH = 1.0f;
        
        private static int _classCount = 1;
        
        private AudioSource _audioSource;
        private float _targetVolume = DEFAULT_VOLUME;
        private bool _isMute;
        
        [SerializeField] private string _name;

        public event Action OnPlayFinished;

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

        public float TargetVolume
        {
            get => _targetVolume;
            set => _targetVolume = value;
        }

        private bool _latestPlayState;
        public bool IsPlaying => _audioSource.isPlaying;
        public bool IsMute => _isMute;
        
        void Awake()
        {
            if (String.IsNullOrEmpty(_name))
            {
                _name = $"UnknownSoundLayer{_classCount++}";
            }
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.volume = _targetVolume;
            _audioSource.pitch = DEFAULT_PITCH;
            _audioSource.loop = false;
        }

        void OnEnable()
        {
            StartCoroutine(nameof(UpdateCoroutine));
        }

        void OnDisable()
        {
            StopCoroutine(nameof(UpdateCoroutine));
        }

        private IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                if (_latestPlayState && IsPlaying == false)
                {
                    OnPlayFinished?.Invoke();
                    _latestPlayState = false;
                }
                yield return null;
            }
        }

        public void Play(AudioClip audioClip, bool loop=false)
        {
            if (audioClip == null) return;
            _audioSource.clip = audioClip;
            _audioSource.loop = loop;
            _audioSource.Play();
            _latestPlayState = true;
        }

        public void PlayOneShot(AudioClip audioClip, float volume = 1.0f)
        {
            if (!audioClip) return;
            _audioSource.PlayOneShot(audioClip, volume);
            _latestPlayState = true; // TODO: 여기 들어가는게 맞는가?
        }

        // public void Stop(float fadeDuration=0.25f)
        // {
        //     float previousVolume = _audioSource.volume;
        //     Log.Info($"Stop SoundLayer ({Name})");
        //     _audioSource.DOFade(0f, fadeDuration)
        //         .OnComplete(() =>
        //         {
        //             _audioSource.Stop();
        //             _audioSource.volume = previousVolume; // 볼륨 복구
        //             _latestPlayState = false; // TODO: 여기 들어가는게 맞는가?
        //         });
        // }
        
        public async Task StopAsync(float fadeDuration=0.25f)
        {
            if (_audioSource.isPlaying == false)
            {
                return;
            }
                
            Log.Info($"Stop SoundLayer ({Name})");
            
            float previousVolume = _audioSource.volume;

            // 볼륨이 0이 될 때까지 대기
            await _audioSource.DOFade(0f, fadeDuration).AsyncWaitForCompletion();

            // 볼륨이 0이 되면 플레이를 종료한다.
            _audioSource.Stop();
            
            // 0으로 된 볼륨을 복구한다.
            _audioSource.volume = previousVolume;
            
            _latestPlayState = false;
        }        

        public void Pause(float fadeDuration=0.25f)
        {
            Log.Info($"Pause SoundLayer ({Name})");
            _audioSource.DOKill();
            _audioSource.DOFade(0f, fadeDuration)
                .OnComplete(() =>
                {
                    _audioSource.Pause();
                    _latestPlayState = false; // TODO: 여기 들어가는게 맞는가?
                });
        }

        public void Resume(float fadeDuration=0.25f)
        {
            Log.Info($"Resume SoundLayer ({Name})");
            _audioSource.UnPause();
            _latestPlayState = true; // TODO: 여기 들어가는게 맞는가?
            _audioSource.DOFade(_targetVolume, fadeDuration);
        }

        public void SetVolume(float volume)
        {
            if (_isMute)
            {
                Log.Warning("뮤트중에는 볼륨을 설정할 수 없습니다");
                return;
            }
            _targetVolume = volume;
            _audioSource.volume = volume;
        }

        public void SetVolumeFade(float volume, float fadeDuration)
        {
            if (_isMute)
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
            if (value)
            {
                _isMute = true;
                _audioSource.mute = true;
                _audioSource.volume = 0f;
                Log.Info($"{_name} Muted");
            }
            // 뮤트를 끄고 소리를 나오게 하는 경우:
            else
            {
                _isMute = false;
                _audioSource.mute = false;
                _audioSource.volume = _targetVolume;
                Log.Info($"{_name} Un-Muted");
            }
        }
        
        public void SetMuteFade(bool value, float fadeDuration)
        {
            if (_isMute == value) return;
            
            // 뮤트를 켜서 소리를 없애는 경우:
            if (value)
            {
                _isMute = true;
                _audioSource.DOKill();
                _audioSource.DOFade(0f, fadeDuration)
                    .OnComplete(
                        () =>
                        {
                            _audioSource.mute = true;
                            Log.Info($"{_name} Muted");
                        }
                    );
            }
            // 뮤트를 끄고 소리를 나오게 하는 경우:
            else
            {
                _isMute = false;
                _audioSource.DOKill();
                _audioSource.DOFade(_targetVolume, fadeDuration)
                    .OnComplete(
                        () =>
                        {
                            _audioSource.mute = false;
                            Log.Info($"{_name} Un-Muted");
                        }
                    );                    
            }         
        }
    }
}