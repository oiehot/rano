#if false

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using System.Collections.Generic;
using Rano;
using Rano.IO;
using Rano.PlatformServices.Cloud;
using UnityEngine;
using Newtonsoft.Json;

namespace Rano.SaveSystem
{
    public sealed class SaveManager : MonoSingleton<SaveManager>
    {
        public enum DataSource
        {
            Local,
            Cloud,
        }

        public string LastModifiedDateField { get; private set; }
        public bool IncludeInactive { get; private set; } = true;

        protected override void Awake()
        {
            base.Awake();
            LastModifiedDateField = "__LastModifiedDate";
        }

        /// <summary>
        /// ISaveable 게임오브젝트들의 정보를 메모리DB, 클라우드 로컬카피로 저장한다.
        /// </summary>
        /// <param name="includeInactive">켜면 비활성화된 게임오브젝트, 컴포넌트들도 포함된다.</param>
        [ContextMenu("Save", false, 1101)]
        public void Save()
        {
            bool isCloudFeature = CloudManager.Instance.IsFeatureAvailable;
            string lastModifiedDate = DateTime.Now.ToString();

            // 1. 마지막 수정일자 업데이트.
            InMemoryDatabase.Instance.SetString(LastModifiedDateField, lastModifiedDate);
            if (isCloudFeature) CloudManager.Instance.SetString(LastModifiedDateField, lastModifiedDate);

            // 2. 게임오브젝트(ISaveable>SaveableEntity)들의 정보를 메모리DB, 클라우드-로컬카피에 Set하기.
            foreach (var saveable in FindObjectsOfType<SaveableEntity>(IncludeInactive))
            {
                // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
                string id = saveable.Id;
                Log.Info($"{id} 게임오브젝트 상태저장");

                // (1) 메모리DB에 Set
                InMemoryDatabase.Instance.Dict[id] = saveable.CaptureToDict();

                // (2) 클라우드-로컬카피에 Set
                if (isCloudFeature)
                {
                    byte[] tmpBytes = saveable.CaptureToBinary();
                    CloudManager.Instance.SetByteArray(id, tmpBytes);
                }
            }

            // 3. Set된 메모리DB를 파일로 저장하고, 클라우드-로컬카피를 동기화하여 실제 클라우드로 업로드하기.
            // (1) 메모리DB를 로컬파일에 저장한다.
            InMemoryDatabase.Instance.Save();

            // (2) 클라우드를 동기화하기.
            if (CloudManager.Instance.IsAvailable)
            {
                CloudManager.Instance.Synchronize();
            }
        }

        /// <summary>
        /// Database에 있는 데이터들로 게임오브젝트들을 업데이트한다.
        /// 로컬메모리, 클라우드 로컬카피 둘중 최신의 정보를 사용한다.
        /// </summary>
        /// <param name="includeInactive">켜면 비활성화된 게임오브젝트, 컴포넌트들도 포함된다.</param>
        [ContextMenu("Load", false, 1102)]
        public void Load()
        {
            string localStr;
            string cloudStr;
            DateTime localDateTime;
            DateTime cloudDateTime;

            bool isMemory = InMemoryDatabase.Instance.Load();
            bool isCloud = CloudManager.Instance.IsFeatureAvailable;

            // TODO:
            //CloudManager.Instance.CoSynchronize();

            DataSource dataSource;
            if (isMemory && isCloud)
            {
                // 수정날짜를 얻고 비교한다.
                localStr = InMemoryDatabase.Instance.GetString(LastModifiedDateField);
                cloudStr = CloudManager.Instance.GetString(LastModifiedDateField);
                localDateTime = localStr.ToDateTime();
                cloudDateTime = cloudStr.ToDateTime();

                // 로컬인메모리, 클라우드 로컬카피 둘중 최신본을 사용한다.

                if (localDateTime >= cloudDateTime)
                {
                    Log.Info("더 최신인 InMemoryDatabase를 사용해서 로드합니다.");
                    dataSource = DataSource.Local;
                }
                else
                {
                    Log.Info("더 최신인 CloudLocalCopy를 사용해서 로드합니다.");
                    dataSource = DataSource.Cloud;
                }
            }
            else if (isMemory == true)
            {
                Log.Info("InMemoryDatabase를 사용해서 로드합니다.");
                dataSource = DataSource.Local;
            }
            else if (isCloud == true)
            {
                Log.Info("CloudLocalCopy를 사용해서 로드합니다.");
                dataSource = DataSource.Cloud;
            }
            else
            {
                Log.Warning("InMemoryDatabase, CloudLocalCopy 모두 사용할 수 없어 로할 수 없습니다.");
                return;
            }

            // 모든 SaveableEntity 게임오브젝트들의 상태를 복구한다.
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>(IncludeInactive))
            {
                string id = saveable.Id;
                Log.Info($"{id} 게임오브젝트 상태복구");
                switch (dataSource)
                {
                    case DataSource.Local:
                        if (InMemoryDatabase.Instance.Dict.ContainsKey(id) == true)
                        {
                            saveable.RestoreFromDict(InMemoryDatabase.Instance.Dict[id]);
                        }
                        break;

                    case DataSource.Cloud:
                        byte[] bytes;
                        if (CloudManager.Instance.TryGetByteArray(id, out bytes))
                        {
                            saveable.RestoreFromBinary(bytes);
                        }
                        else
                        {
                            Log.Warning($"CloudLocalCopy에서 키:{id} 데이터를 찾을 수 없음.");
                        }
                        break;
                }
            }
        }

        [ContextMenu("Delete", false, 1103)]
        public void DeleteSaveFile()
        {
            if (System.IO.File.Exists(InMemoryDatabase.Instance.SavePath) == true)
            {
                Log.Info($"파일삭제 ({InMemoryDatabase.Instance.SavePath}");
                System.IO.File.Delete(InMemoryDatabase.Instance.SavePath);
            }
        }
    }
}

#endif