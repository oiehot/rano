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
    public partial class SoundLayer : MonoBehaviour
    {
        public static int classCount = 1;
        public float volume = 1.0f;
        
        AudioSource[] players;
        public string layerName;
        private string id
        {
            get
            {
                return $"SoundLayer/{this.layerName}";
            }
        }
        private bool mute = false;

        void Awake()
        {
            this.layerName = $"Unknown{SoundLayer.classCount++}";
            this.players = new AudioSource[5];
            for (int i=0; i<this.players.Length; i++)
            {
                this.players[i] = this.gameObject.AddComponent<AudioSource>();
            }
        }

        /// <summary>출력이 가능한 오디오 소스를 찾는다</summary>
        private AudioSource GetAudioSource()
        {
            for (int i=0; i<this.players.Length; i++)
            {
                if (!this.players[i].isPlaying)
                {
                    return this.players[i];
                }
            }
            return null;
        }

        /// <summary>사운드를 플레이한다</summary>
        public AudioSource Play(AudioClip clip, bool loop=false)
        {
            // 뮤트되어 있으면 플레이 할 수 없다.
            if (this.mute) return null;

            AudioSource audioSource;
            audioSource = this.GetAudioSource();
            if (audioSource)
            { 
                audioSource.clip = clip;
                audioSource.loop = loop;
                audioSource.volume = this.volume;
                audioSource.Play();
                return audioSource;
            }
            else
            {
                Log.Warning($"{this.id}: Audio source is full. Unable to play Sound");
                return null;
            }
        }

        /// <summary>출력중인 특정 사운드를 중지한다</summary>
        public void Stop(AudioClip clip)
        {
            for (int i=0; i<this.players.Length; i++)
            {
                if (this.players[i].isPlaying)
                {
                    if (this.players[i].clip == clip)
                    {
                        this.players[i].Stop();
                    }
                }
            }
        }

        /// <summary>모든 출력중인 사운드를 중지한다</summary>
        public void StopAll()
        {
            Log.Info($"{this.id}: Stop all sounds");
            for (int i=0; i<this.players.Length; i++)
            {
                this.players[i].Stop();
            }
        }

        /// <summary>모든 사운드를 잠시 정지한다.</summary>
        public void Pause()
        {
            Log.Info($"{this.id}: Pause all sounds");
            for (int i=0; i<this.players.Length; i++)
            {
                this.players[i].Pause();
            }
        }

        /// <summary>정지되었던 모든 사운드를 재개한다.</summary>
        public void Resume()
        {
            // 뮤트되지 않은 상태에서만 하용한다.
            if (!this.mute)
            {
                Log.Info($"{this.id}: Resume all sounds");
                for (int i=0; i<this.players.Length; i++)
                {
                    this.players[i].UnPause();
                }
            }
        }

        public void SetMute(bool mute)
        {
            this.mute = mute;
            if (mute)
            {
                Log.Info($"{this.id}: OFF");
                this.Pause();
            }
            else
            {
                Log.Info($"{this.id}: ON");
                this.Resume();
            }
        }

        public bool GetMute()
        {
            return this.mute;
        }
    }
}