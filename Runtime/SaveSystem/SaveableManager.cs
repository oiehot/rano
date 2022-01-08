// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using Rano;

namespace Rano.SaveSystem
{
    public sealed class SaveableManager : MonoSingleton<SaveableManager>
    {
        private InMemoryDatabase _db;
        public bool AutoSaveOnExit { get; private set; } = true;
        public bool IncludeInactive { get; private set; } = true;
        public Action OnExit { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _db = InMemoryDatabase.Instance;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            Log.Info("OnApplicationQuit");
            OnApplicationPauseOrQuit();
        }

        private void OnApplicationPauseOrQuit()
        {
            Log.Info("OnApplicationPauseOrQuit");
            if (AutoSaveOnExit)
            {
                Log.Info("AutoSaveOnExit가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                OnExit?.Invoke();
                CaptureAllSaveableEntities();
                _db.Save();
            }
            else
            {
                Log.Info("AutoSaveOnExit가 꺼져 있으므로 모든Saveable 상태를 InMemoryDB에 저장하지 않습니다");
            }
        }

        public void CaptureAllSaveableEntities()
        {
            foreach (var saveable in GameObject.FindObjectsOfType<SaveableEntity>(IncludeInactive))
            {
                // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
                string id = saveable.Id;
                Log.Info($"{id} 게임오브젝트 상태저장");
                var gameObjectState = saveable.CaptureState();
                _db.SetDictionary(id, gameObjectState);
            }
        }
    }
}