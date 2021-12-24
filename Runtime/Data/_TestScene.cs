#if RANO_DATA_EXAMPLE

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Rano;

namespace Rano.SaveSystem
{
    public class TestScene : MonoBehaviour
    {
        private string SavePath => $"{Application.persistentDataPath}/TestScene";

        [ContextMenu("Save")]
        private void Save()
        {
            Dictionary<string, object> gameObjectStates;

            // 1. Load Previous Data
            gameObjectStates = LoadFile();

            // 2. Update Data
            foreach (var gameObjectState in FindObjectsOfType<SaveableEntity>())
            {
                // TODO: 이미 같은 이름의 게임오브젝트로 저장하는 경우 경고처리 하고 덮어씌움.
                Log.Info($"{gameObjectState.Id} 게임오브젝트 상태저장");
                gameObjectStates[gameObjectState.Id] = gameObjectState.CaptureComponentStates();
            }

            // 3. Save Data
            SaveFile(gameObjectStates);
        }

        [ContextMenu("Load")]
        private void Load()
        {
            // 1. Load Previous Data
            Dictionary<string, object> gameObjectStates = LoadFile();

            // 2. Load Data
            foreach (var gameObjectState in FindObjectsOfType<SaveableEntity>())
            {
                if (gameObjectStates.TryGetValue(gameObjectState.Id, out object state))
                {
                    Log.Info($"{gameObjectState.Id} 게임오브젝트 상태복원");
                    gameObjectState.RestoreComponentStates(state);
                }
            }
        }

        private void SaveFile(object state)
        {
            using (var stream = File.Open(SavePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile()
        {
            if (!File.Exists(SavePath))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(SavePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }
    }
}

#endif