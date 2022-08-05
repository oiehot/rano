using System.Collections.Generic;
using UnityEngine;

namespace Rano.SoundSystem
{
    public sealed class SoundManager : BaseComponent
    {
        Dictionary<string, AudioClip> clips;
        Dictionary<string, SoundLayer> layers;
        AudioListener audioListener;

        public float masterVolume
        {
            get
            {
                return AudioListener.volume;
            }
            set
            {
                AudioListener.volume = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            clips = new Dictionary<string, AudioClip>();
            layers = new Dictionary<string, SoundLayer>();

            // 현재 씬에 AudioListener가 없다면(보통 카메라에 있음) 이 사운드 매니져에 장착한다.
            audioListener = UnityEngine.Object.FindObjectOfType<AudioListener>();
            if (audioListener == null)
            {
                audioListener = gameObject.AddComponent<AudioListener>();
            }
        }

        /// <summary>
        /// 사운드 레이어를 추가한다.
        /// </summary>
        /// <example><code>
        /// SoundManager.Instance.AddLayer("Music");
        /// </code></example>
        public SoundLayer AddLayer(string name, float volume=1.0f)
        {
            Log.Info($"Add SoundLayer: {name}");
            GameObject soundLayerGameObject = new GameObject(name);
            UnityEngine.Object.DontDestroyOnLoad(soundLayerGameObject);
            SoundLayer soundLayer = soundLayerGameObject.AddComponent<SoundLayer>();
            soundLayer.layerName = name;
            soundLayer.volume = volume;
            soundLayerGameObject.transform.parent = gameObject.transform;
            layers.Add(name, soundLayer);
            return soundLayer;
        }

        /// <summary>
        /// 로드된 오디오 클립을 찾는다.
        /// </summary>
        public AudioClip FindClip(string clipName)
        {
            if (clips.ContainsKey(clipName))
            {
                return clips[clipName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 리소스 폴더로 부터 사운드를 로드한다.
        /// </summary>
        /// <example><code>
        /// SoundManager.Instance.LoadInResources("BGM_GameMenu", "Sounds/BGM/BGM_GameMenu");
        /// SoundManager.Instance.LoadInResources("SFX_Bounce_01", "Sounds/SFX/SFX_Bounce_01");
        /// </code></example>
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

            if (audioClip != null)
            {
                clips.Add(clipName, audioClip);
                Log.Info($"AudioClip Loaded: {clipName} <= {path}");
            }
            else
            {
                Log.Warning($"Unable to load AudioClip: {clipName} <= {path}");
            }
        }

        /// <summary>
        /// 원하는 레이어에서 특정 클립을 플레이한다.
        /// </summary>
        /// <example><code>
        /// SoundManager.Instance.Play("Music", "BGM_GamePlay1", true);
        /// </code></example>
        public AudioSource Play(string soundLayerName, string clipName, bool loop)
        {
            AudioClip clip;
            clip = FindClip(clipName);
            if (!clip)
            {
                Log.Warning($"Not found AudioClip: {clipName}");
                return null;
            }

            if (layers.ContainsKey(soundLayerName))
            {
                return layers[soundLayerName].Play(clip, loop);
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
                return null;
            }
        }

        /// <summary>
        /// 특정 레이어에서 플레이중인 클립을 중지한다.
        /// </summary>
        /// <example><code>
        /// SoundManager.Instance.Stop("Music", "BGM_GamePlay1");
        /// </code></example>
        public void Stop(string soundLayerName, string clipName)
        {
            AudioClip clip;
            clip = FindClip(clipName);
            if (!clip) return;

            if (layers.ContainsKey(soundLayerName))
            {
                layers[soundLayerName].Stop(clip);
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
            }
        }

        /// <summary>
        /// 모든 레이어에서 출력중인 사운드를 중지한다.
        /// </summary>
        public void StopAll()
        {
            foreach(SoundLayer soundLayer in layers.Values)
            {
                soundLayer.StopAll();
            }
        }

        /// <summary>
        /// 모든 레이어의 출력을 멈춘다.
        /// </summary>
        public void Pause()
        {
            foreach(SoundLayer soundLayer in layers.Values)
            {
                soundLayer.Pause();
            }
        }

        /// <summary>
        /// 모든 레이어의 출력을 재개한다.
        /// </summary>
        public void Resume()
        {
            foreach(SoundLayer soundLayer in layers.Values)
            {
                soundLayer.Resume();
            }
        }

        /// <summary>
        /// 특정 레이어를 Mute 여부를 설정한다.
        /// </summary>
        /// <example><code>
        /// SoundManager.Instance.SetMute("Music", true);
        /// </code></example>
        public void SetMute(string soundLayerName, bool mute)
        {
            if (layers.ContainsKey(soundLayerName))
            {
                layers[soundLayerName].SetMute(mute);
            }
            else
            {
                Log.Warning($"Not found SoundLayer: {soundLayerName}");
            }
        }

        /// <summary>
        /// 특정 레이어의 뮤트값을 없는다.
        /// </summary>
        /// <returns>
        /// 뮤트 여부 bool 값.
        /// </returns>
        public bool GetMute(string soundLayerName)
        {
            if (layers.ContainsKey(soundLayerName))
            {
                return layers[soundLayerName].GetMute();
            }
            else
            {
                throw new NotFoundSoundLayerException($"Not found SoundLayer: {soundLayerName}");
            }
        }
    }
}
