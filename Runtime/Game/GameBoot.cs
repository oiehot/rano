// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Rano;

namespace YOUR_PROJECT
{
    public class GameBoot
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void GB001_GameBoot()
        {
            Rano.Log.Important("Start GameBoot");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void GB002_SetupEventSystem()
        {
            Rano.Log.Important("Setup EventSystem");
            EventSystem sceneEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (sceneEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                UnityEngine.Object.DontDestroyOnLoad(eventSystem);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void GB003_SetupGameManager()
        {
            Rano.Log.Important("Setup GameManager");
            GameManager sceneGameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
            if (sceneGameManager == null)
            {
                GameObject gameObject = new GameObject("GameManager");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<GameManager>();
            }
        }
    }
}

#endif