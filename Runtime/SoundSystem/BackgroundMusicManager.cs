#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Rano.SaveSystem;
using Rano;
using Rano.Math;
using Rano.Pattern;
using System.Linq;

namespace Rano.SoundSystem
{
    public sealed class PlayItem
    {
        public AudioClip? audioClip;
        public bool isPlayed;

        public PlayItem(AudioClip audioClip)
        {
            this.audioClip = audioClip;
            this.isPlayed = false;
        }
    }
    
    public sealed class BackgroundMusicManager : MonoSingleton<BackgroundMusicManager>
    {
        private int _playIntervalMilliseconds = 5000;
        private SoundManager _soundManager;
        private SoundLayer _soundLayer;
        
        private PlayItem[] _playItems = Array.Empty<PlayItem>();
        private PlayItem[] _unplayedItems => _playItems.Where(item => item.isPlayed == false).ToArray();
        
        private int _currentIndex;

        private bool _shuffleMode = false;
        private bool repeatMode = true;
        
        public int MusicCount => _playItems.Length;
        public bool IsShuffleMode => _shuffleMode;
        public bool IsRepeatMode => repeatMode;

        public bool IsPlaying => _soundLayer.IsPlaying;
        public bool IsAllMusicPlayed
        {
            get
            {
                for (int i = 0; i < _playItems.Length; i++)
                {
                    if (_playItems[i].isPlayed == false) return false;
                }
                return true;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            _soundManager = UnityEngine.Object.FindObjectOfType<SoundManager>();
            _soundLayer = _soundManager.GetSoundLayer(ESoundLayerType.Background);
            
            Log.Info($"사운드 매니져 연결 ({_soundManager})");
            Log.Info($"사운드 레이어 연결 ({_soundLayer})");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_soundLayer)
            {
                _soundLayer.OnPlayFinished += OnPlayFinished;
            }
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            if (_soundLayer)
            {
                _soundLayer.OnPlayFinished -= OnPlayFinished;    
            }
        }

        public async Task LoadAsync(PlaylistSO playList)
        {
            Log.Info($"플레이 리스트 로딩 시작 ({playList})");
            // TODO: IsLoaded() => Unload()
            Unload();
            
            _playItems = new PlayItem[playList.tracks.Length];
            
            // TODO: 병렬 로딩으로 성능 향상이 필요함.
            for (int i=0; i<playList.tracks.Length; i++)
            {
                AssetReference assetReference = playList.tracks[i];
                Log.Info($"오디오 클립 로딩 ({assetReference})");
                var aoh = Addressables.LoadAssetAsync<AudioClip>(assetReference);
                // var aoh = assetReference.LoadAssetAsync<AudioClip>();
                
                AudioClip? audioClip = await aoh.Task;
                if (audioClip == null)
                {
                    Log.Warning($"오디오 클립 로딩 실패 ({assetReference})");
                    continue;
                }

                _playItems[i] = new PlayItem(audioClip);
            }
            
            Log.Info($"플레이 리스트 로딩 완료 ({playList})");
        }

        public void Unload()
        {
            Log.Info($"언로딩 시작");
            
            for (int i=0; i<_playItems.Length; i++)
            {
                AudioClip? audioClip = _playItems[i].audioClip;
                if (audioClip == null) continue;
                
                Addressables.Release<AudioClip>(audioClip);
                
                _playItems[i].audioClip = null;
                _playItems[i].isPlayed = false;
            }
            _playItems = Array.Empty<PlayItem>();
            
            Log.Info($"언로딩 완료");
        }

        private void ResetPlayState()
        {
            foreach (PlayItem musicItem in _playItems)
            {
                musicItem.isPlayed = false;
            }
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (_playItems.Length <= 0)
            {
                Log.Info("플레이 할 음악이 없음");
                return;
            }
            
            ResetPlayState();
            
            int index = _shuffleMode ? RandomUtils.GetRandomInt(0,_playItems.Length) : 0;
            PlayByIndex(index);
        }

        private bool PlayByIndex(int index)
        {
            if (index < 0 || index >= _playItems.Length) return false;
            
            PlayItem playItem = _playItems[index];
            AudioClip? audioClip = playItem.audioClip;
            if (audioClip == null)
            {
                Log.Warning($"음악 플레이 실패 (오디오 클립이 없음, index: {index})");
                return false;
            }
            
            Log.Info($"음악 플레이 시작 (index: {index})");
            _soundLayer.PlayOneShot(audioClip);
            playItem.isPlayed = true;
            
            _currentIndex = index;
            return true;
        }

        [ContextMenu("Play Next")]
        public void PlayNext()
        {
            int index = 0;

            if (IsPlaying)
            {
                _soundLayer.Stop();
            }

            if (IsShuffleMode)
            {
                if (IsAllMusicPlayed)
                {
                    if (IsRepeatMode)
                    {
                        Log.Info("반복 모드가 켜져 있어 처음부터 다시 재생합니다");
                        ResetPlayState();
                    }
                    else
                    {
                        Log.Info("모든 곡이 플레이되었습니다");
                        return;
                    }
                }
                // TODO: 인덱스 일치 필요
                // TODO: 인덱스 일치 필요
                // TODO: 인덱스 일치 필요
                index = RandomUtils.GetRandomInt(0, _unplayedItems.Length);
            }
            else
            {
                if (IsRepeatMode)
                {
                    int nextIndex = _currentIndex + 1;
                    if (nextIndex >= MusicCount) nextIndex = 0;
                    index = nextIndex;
                }
                else
                {
                    Log.Info("모든 곡이 플레이되었습니다");
                    return;
                }
            }
            
            Log.Info("다음 곡으로 넘어갑니다");
            PlayByIndex(index);
        }

        private async void OnPlayFinished()
        {
            Log.Info("음악 한 곡이 종료됨");
            
            await Task.Delay(_playIntervalMilliseconds); // 잠시 무음 상태
            
            Log.Info("다음 곡으로 넘어갑니다");
            
            PlayNext();
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            _soundLayer.Pause();
        }

        [ContextMenu("Resume")]
        public void Resume()
        {
            _soundLayer.Resume();
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            _soundLayer.Stop();
        }
    }
}