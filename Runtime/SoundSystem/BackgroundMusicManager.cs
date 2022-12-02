#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Rano.Math;
using System.Linq;

namespace Rano.SoundSystem
{
    public sealed class BackgroundMusicManager : ManagerComponent
    {   
        private int _playIntervalMilliseconds = 5000;
        
        private SoundManager? _soundManager;
        private SoundLayer? _soundLayer;
        private PlayItem[] _playItems = Array.Empty<PlayItem>();
        
        private int _currentIndex;
        private bool _isShuffleMode = true;
        private bool _isRepeatMode = true;

        public bool IsInitialized => _soundManager && _soundLayer;
        public int MusicCount => _playItems.Length;
        public bool IsLoaded => _playItems.Length > 0;
        public bool IsShuffleMode => _isShuffleMode;
        public bool IsRepeatMode => _isRepeatMode;
        public bool IsPlaying => _soundLayer != null && _soundLayer.IsPlaying;
        public bool IsAllMusicPlayed
        {
            get
            {
                for (int i = 0; i < _playItems.Length; i++)
                {
                    if (_playItems[i].IsPlayed == false) return false;
                }
                return true;
            }
        }
        private PlayItem[] UnplayedItems => _playItems.Where(item => item.IsPlayed == false).ToArray();
        
        protected override void Awake()
        {
            base.Awake();
            
            Log.Info("초기화 중...");
            
            _soundManager = UnityEngine.Object.FindObjectOfType<SoundManager>();
            if (_soundManager)
            {
                _soundLayer = _soundManager.GetSoundLayer(ESoundLayerType.Background);
                if (!_soundLayer)
                {
                    Log.Warning($"사운드 레이어 연결 실패 ({_soundLayer})");
                }
            }
            else
            {
                Log.Warning($"사운드 매니져 연결 실패");
            }

            if (IsInitialized)
            {
                Log.Info("초기화 성공");
            }
            else
            {
                Log.Warning("초기화 실패");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_soundLayer != null)
            {
                _soundLayer.OnPlayFinished += OnPlayFinished;
            }
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            if (_soundLayer != null)
            {
                _soundLayer.OnPlayFinished -= OnPlayFinished;    
            }
        }

        private int GetRandomIndex_InPlayList()
        {
            return RandomUtils.GetRandomInt(0, _playItems.Length);
        }

        /// <summary>
        /// 플레이 되지 않은 곡 중 하나의 인덱스를 리턴한다.
        /// </summary>
        private (bool result, int index) GetRandomIndex_InUnplayedList()
        {
            List<int> unplayedItemIndexes = new List<int>();
            
            for (int i=0; i<_playItems.Length; i++)
            {
                if (_playItems[i].IsPlayed == false)
                {
                    unplayedItemIndexes.Add(i);
                }
            }

            if (unplayedItemIndexes.Count <= 0) return (false, 0);
            
            int randomIndex = RandomUtils.GetRandomInt(0, unplayedItemIndexes.Count);
            int itemIndex = unplayedItemIndexes[randomIndex];

            return (true, itemIndex);
        }
        
        public async Task LoadAsync(PlaylistSO playList)
        {
            Log.Info($"플레이 리스트 로딩 시작 ({playList})");
            
            if (IsLoaded) Unload();
            
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
                AudioClip? audioClip = _playItems[i].AudioClip;
                if (audioClip == null) continue;
                
                Addressables.Release<AudioClip>(audioClip);
                
                _playItems[i].AudioClip = null;
                _playItems[i].IsPlayed = false;
            }
            
            _playItems = Array.Empty<PlayItem>();
            
            Log.Info($"언로딩 완료");
        }

        private void ResetPlayState()
        {
            foreach (PlayItem musicItem in _playItems)
            {
                musicItem.IsPlayed = false;
            }
        }

        [ContextMenu("Play")]
        public async void Play()
        {
            if (MusicCount <= 0)
            {
                Log.Info("플레이 할 음악이 없음");
                return;
            }
            
            // 이전에 있는 플레이를 먼저 중지한다. (페이드에 시간이 걸림)
            await StopAsync();

            // 플레이 상태 체크를 리셋한다.
            ResetPlayState();
            
            // 음악 플레이
            PlayByIndex(IsShuffleMode ? GetRandomIndex_InPlayList() : 0);
        }

        [ContextMenu("Play Next")]
        public async void PlayNext()
        {
            int index = 0;
            
            await StopAsync();

            if (IsShuffleMode)
            {
                if (IsAllMusicPlayed)
                {
                    if (IsRepeatMode)
                    {
                        Log.Info("반복 모드가 켜져 있어 처음부터 다시 재생합니다");
                        ResetPlayState();
                        index = GetRandomIndex_InPlayList();
                    }
                    else
                    {
                        Log.Info("모든 곡이 플레이되었습니다");
                        return;
                    }
                }
                else
                {
                    (bool result, int index) resultTuple = GetRandomIndex_InUnplayedList();
                    if (resultTuple.result)
                    {
                        index = resultTuple.index;
                    }
                }
            }
            else
            {
                int nextIndex = _currentIndex + 1;
                if (nextIndex >= MusicCount)
                {
                    if (IsRepeatMode)
                    {
                        nextIndex = 0;
                    }
                    else
                    {
                        Log.Info("모든 곡이 플레이되었습니다");
                        _currentIndex = 0;
                        return;
                    }
                }
                index = nextIndex;
            }
            
            PlayByIndex(index);
        }

        private void PlayByIndex(int index)
        {
            if (!IsInitialized)
            {
                Log.Warning($"초기화 되지 않아 음악을 재생할 수 없습니다 (index:{index})");
                return;
            }

            if (index < 0 || index >= _playItems.Length)
            {
                Log.Warning($"플레이리스트 범위를 넘어선 곡을 플레이 할 수 없습니다 (index:{index})");
                return;
            }
            
            PlayItem playItem = _playItems[index];
            AudioClip? audioClip = playItem.AudioClip;
            
            if (audioClip == null)
            {
                Log.Warning($"음악 플레이 실패 (오디오 클립이 없음, index: {index})");
                return;
            }
            
            Log.Info($"음악 플레이 시작 (index: {index})");
            _soundLayer!.PlayOneShot(audioClip);
            
            playItem.IsPlayed = true;
            _currentIndex = index;
        }

        public void Pause()
        {
            if (!IsInitialized)
            {
                Log.Warning($"초기화 되지 않아 음악을 멈출 수 없습니다");
                return;
            }
            _soundLayer!.Pause();
        }

        public void Resume()
        {
            if (!IsInitialized)
            {
                Log.Warning($"초기화 되지 않아 음악을 재개할 수 없습니다");
                return;
            }
            _soundLayer!.Resume();
        }

        [ContextMenu("Stop")]
        public async Task StopAsync()
        {
            if (!IsInitialized)
            {
                Log.Warning($"초기화 되지 않아 음악을 멈출 수 없습니다");
                return;
            }
            await _soundLayer!.StopAsync();
        }

        private async void OnPlayFinished()
        {
            Log.Info("음악 한 곡이 종료됨");
            await Task.Delay(_playIntervalMilliseconds); // 잠시 무음 상태
            PlayNext();
        }
    }
}