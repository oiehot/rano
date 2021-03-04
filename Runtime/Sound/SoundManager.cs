// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rano;

namespace Rano
{
    /// <todo>
    /// TODO: PlayOneShot 의 사용
    /// </todo>
    public partial class SoundManager : Singleton<SoundManager>
    {
        public Dictionary<string, AudioClip> clips;        
        public Dictionary<string, SoundLayer> layers;
        public AudioListener audioListener;
        public float masterVolume {
            get
            {
                return AudioListener.volume;
            }
            set
            {
                AudioListener.volume = value;
            }
        }

        void Awake()
        {
            this.clips = new Dictionary<string, AudioClip>();
            this.layers = new Dictionary<string, SoundLayer>();

            // 현재 씬에 AudioListener가 없다면(보통 카메라에 있음) 이 사운드 매니져에 장착한다.
            this.audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
            if (this.audioListener == null)
            {
                this.audioListener = this.gameObject.AddComponent<AudioListener>();
            }
        }

        void OnEnable()
        {
            Log.Info("SoundManager Enabled");
        }

        void OnDisable()
        {
            Log.Info("SoundManager Disabled");
        }
        
        /// <example>
        /// SoundManager.Instance.AddLayer("Music");
        /// <example>
        public SoundLayer AddLayer(string name, float volume=1.0f)
        {
            Log.Info($"Add SoundLayer: {name}");
            GameObject soundLayerGameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(soundLayerGameObject);
            SoundLayer soundLayer = soundLayerGameObject.AddComponent<SoundLayer>();
            soundLayer.layerName = name;
            soundLayer.volume = volume;
            soundLayerGameObject.transform.parent = this.gameObject.transform;
            this.layers.Add(name, soundLayer);
            return soundLayer;
        }

        /// <summary>로드된 오디오 클립을 찾는다</summary>
        public AudioClip FindClip(string clipName)
        {
            if (this.clips.ContainsKey(clipName))
            {
                return this.clips[clipName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>리소스 폴더로 부터 사운드를 로드한다.</summary>
        public void LoadInResources(string clipName, string path)
        {
            AudioClip audioClip;
            try
            {
                audioClip = Resources.Load<AudioClip>(path);
            }
            catch
            {
                Log.Warning($"Unable to load resource: {path}");
                return;
            }

            if (audioClip)
            {
                this.clips.Add(clipName, audioClip);
                Log.Info($"AudioClip Loaded: {clipName} <= {path}");
            }
            else
            {
                Log.Warning($"Unable to load AudioClip: {clipName} <= {path}");
            }
        }

        /// <example>
        /// SoundManager.Instance.Play("Music", "BGM_GamePlay1", true);
        /// </example>
        public AudioSource Play(string soundLayerName, string clipName, bool loop=false)
        {
            AudioClip clip;
            clip = this.FindClip(clipName);
            if (!clip)
            {
                Log.Warning($"Not found AudioClip: {clipName}");
                return null;
            }

            if (this.layers.ContainsKey(soundLayerName))
            {
                return this.layers[soundLayerName].Play(clip, loop);
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
                return null;
            }
        }

        /// <example>
        /// SoundManager.Instance.Stop("Music", "BGM_GamePlay1");
        /// </example>
        public void Stop(string soundLayerName, string clipName)
        {
            AudioClip clip;
            clip = this.FindClip(clipName);
            if (!clip) return;

            if (this.layers.ContainsKey(soundLayerName))
            {
                this.layers[soundLayerName].Stop(clip);
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
            }            
        }

        /// <summary>모든 레이어에서 출력중인 사운드를 중지한다</summary>
        public void StopAll()
        {
            foreach(SoundLayer soundLayer in this.layers.Values)
            {
                soundLayer.StopAll();
            }            
        }

        /// <summary>모든 레이어의 출력을 멈춘다</summary>
        public void Pause()
        {
            foreach(SoundLayer soundLayer in this.layers.Values)
            {
                soundLayer.Pause();
            }
        }

        /// <summary>모든 레이어의 출력을 재개한다</summary>
        public void Resume()
        {
            foreach(SoundLayer soundLayer in this.layers.Values)
            {
                soundLayer.Resume();
            }
        }

        /// <summary>특정 레이어를 Mute한다</summary>
        /// <example>
        /// SoundManager.Instance.SetMute("Music", true);
        /// </example>
        public void SetMute(string soundLayerName, bool mute)
        {
            if (this.layers.ContainsKey(soundLayerName))
            {
                this.layers[soundLayerName].SetMute(mute);
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
            }            
        }

        public bool GetMute(string soundLayerName)
        {
            if (this.layers.ContainsKey(soundLayerName))
            {
                return this.layers[soundLayerName].GetMute();
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
                return false; // TODO: Check                
            }
        }
    }
}
