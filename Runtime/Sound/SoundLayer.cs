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
    public class SoundLayer : MonoBehaviour
    {
        public static int classCount = 1;
        public string layerName;
        public float volume = 1.0f;
        AudioSource[] audioSources;
        string id
        {
            get
            {
                return $"SoundLayer/{layerName}";
            }
        }
        bool mute = false;

        void Awake()
        {
            layerName = $"Unknown{SoundLayer.classCount++}";
            audioSources = new AudioSource[5];
            for (int i=0; i<audioSources.Length; i++)
            {
                audioSources[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        /// <summary>
        /// 출력이 가능한 오디오 소스를 찾는다.
        /// </summary>
        AudioSource GetAudioSource()
        {
            for (int i=0; i<audioSources.Length; i++)
            {
                if (!audioSources[i].isPlaying)
                {
                    return audioSources[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 사운드를 플레이한다.
        /// </summary>
        public AudioSource Play(AudioClip clip, bool loop=false)
        {
            // 뮤트되어 있으면 플레이 할 수 없다.
            if (mute) return null;

            AudioSource audioSource;
            audioSource = GetAudioSource();
            if (audioSource)
            { 
                audioSource.clip = clip;
                audioSource.loop = loop;
                audioSource.volume = volume;
                audioSource.Play();
                return audioSource;
            }
            else
            {
                Log.Warning($"{id}: Audio source is full. Unable to play Sound");
                return null;
            }
        }

        /// <summary>
        /// 출력중인 특정 사운드를 중지한다.
        /// </summary>
        public void Stop(AudioClip clip)
        {
            for (int i=0; i<audioSources.Length; i++)
            {
                if (audioSources[i].isPlaying)
                {
                    if (audioSources[i].clip == clip)
                    {
                        audioSources[i].Stop();
                    }
                }
            }
        }

        /// <summary>
        /// 모든 출력중인 사운드를 중지한다.
        /// </summary>
        public void StopAll()
        {
            Log.Info($"{id}: Stop all sounds");
            for (int i=0; i<audioSources.Length; i++)
            {
                audioSources[i].Stop();
            }
        }

        /// <summary>
        /// 모든 사운드를 잠시 정지한다.
        /// </summary>
        public void Pause()
        {
            Log.Info($"{id}: Pause all sounds");
            for (int i=0; i<audioSources.Length; i++)
            {
                audioSources[i].Pause();
            }
        }

        /// <summary>
        /// 정지되었던 모든 사운드를 재개한다.
        /// </summary>
        public void Resume()
        {
            // 뮤트되지 않은 상태에서만 하용한다.
            if (!mute)
            {
                Log.Info($"{id}: Resume all sounds");
                for (int i=0; i<audioSources.Length; i++)
                {
                    audioSources[i].UnPause();
                }
            }
        }

        /// <summary>
        /// 사운드 Mute 여부를 설정한다.
        /// </summary>
        public void SetMute(bool mute)
        {
            this.mute = mute;
            if (mute)
            {
                Log.Info($"{id}: OFF");
                Pause();
            }
            else
            {
                Log.Info($"{id}: ON");
                Resume();
            }
        }

        /// <summary>
        /// 사운드 Mute 여부를 얻는다.
        /// </summary>
        public bool GetMute()
        {
            return mute;
        }
    }
}