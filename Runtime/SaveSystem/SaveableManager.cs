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
        public bool IncludeInactive { get; set; } = true;
        public bool AutoSaveOnPause { get; set; } = false;
        public bool AutoSaveOnFocusOut { get; set; } = false;
        public bool AutoSaveOnExit { get; set; } = false;
        public Action OnSave { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _db = InMemoryDatabase.Instance;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            if (AutoSaveOnExit == true)
            {
                Log.Info("OnApplicationQuit");
                Log.Info("AutoSaveOnExit가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                SaveAllSaveableEntities();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true && AutoSaveOnPause)
            {
                Log.Info($"OnApplicationPause({pause})");
                Log.Info("AutoSaveOnPause가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                SaveAllSaveableEntities();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false && AutoSaveOnFocusOut)
            {
                Log.Info($"OnApplicationFocus({focus})");
                Log.Info("OnApplicationFocus가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                SaveAllSaveableEntities();
            }
        }

        public void SaveAllSaveableEntities()
        {
            OnSave?.Invoke();
            CaptureAllSaveableEntities();
            _db.Save();
        }

        private void CaptureAllSaveableEntities()
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