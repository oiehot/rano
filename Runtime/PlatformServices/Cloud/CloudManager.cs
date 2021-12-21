// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;

namespace Rano.PlatformServices.Cloud
{
    public sealed class CloudManager : MonoSingleton<CloudManager>
    {
        public bool InitialSynchornized { get; private set; }

        public Action onUserChange;
        public Action onSavedDataChange;
        public Action onSynchronizeComplete;

        void OnEnable()
        {
            Log.Info("CloudManager Enabled");
            CloudServices.OnUserChange += OnUserChange;
            CloudServices.OnSavedDataChange += OnSavedDataChange;
            CloudServices.OnSynchronizeComplete += OnSynchronizeComplete;
        }

        void OnDisable()
        {
            Log.Info("CloudManager Disabled");
            CloudServices.OnUserChange -= OnUserChange;
            CloudServices.OnSavedDataChange -= OnSavedDataChange;
            CloudServices.OnSynchronizeComplete -= OnSynchronizeComplete;
        }

        private void OnUserChange(CloudServicesUserChangeResult result, Error error)
        {
            if (error != null)
            {
                Log.Warning($"클라우드 계정 로그인 실패 ({error})");
                return;
            }

            ICloudUser user = result.User;
            string userId = user.UserId;
            CloudUserAccountStatus accountStatus = user.AccountStatus;
            Log.Info($"클라우드 계정 로그인 성공 (Id:{userId}, Status:{accountStatus})");

            onUserChange?.Invoke();
        }

        private void OnSavedDataChange(CloudServicesSavedDataChangeResult result)
        {
            Log.Info("OnSavedDataChange 이벤트 발생.");
            Log.Info("결과: " + result.ToString());

            string[] changedKeys = result.ChangedKeys;
            Log.Info($"총 바뀐 키: {changedKeys.Length}");
            foreach (string key in changedKeys)
            {
                Log.Info($"키: {key}");
            }

            CloudSavedDataChangeReasonCode reason = result.ChangeReason;
            Log.Info($"이유: {reason.ToString()}");

            onSavedDataChange?.Invoke();
        }

        private void OnSynchronizeComplete(CloudServicesSynchronizeResult result)
        {
            if (result.Success == true)
            {
                Log.Info("클라우드 동기화 완료.");
            }
            else
            {
                Log.Warning("클라우드 동기화 실패.");
            }
            onSynchronizeComplete?.Invoke();
        }

        public IEnumerator SynchronizeCoroutine(Action<bool> callback = null)
        {
            // TODO: 이미 동기화 중이면 생략한다 or Queue 처리한다.
            bool syncResult = false;
            bool syncCompleted = false;

            Log.Info("클라우드 동기화 시작.");
            CloudServices.Synchronize((CloudServicesSynchronizeResult result) => {
                if (result.Success)
                {
                    syncResult = true;
                }
                else
                {
                    syncResult = false;
                }
                syncCompleted = true;

                // 최초로 싱크했다면 체크한다.
                if (InitialSynchornized == false) InitialSynchornized = true;
            });

            while (syncCompleted == false)
            {
                // TODO: 동기화 시간제한.
                yield return YieldCache.WaitForFixedUpdate;
            }

            callback?.Invoke(syncResult);
        }

        //public void Synchronize()
        //{
        //    Log.Info("클라우드 동기화 시작.");
        //    CloudServices.Synchronize();
        //}
        //public void Synchronize(Callback<CloudServicesSynchronizeResult> callback)
        //{
        //    Log.Info("클라우드 동기화 시작.");
        //    CloudServices.Synchronize(callback);
        //}

        public bool IsAvailable() => CloudServices.IsAvailable();

        public bool GetBool(string key) => CloudServices.GetBool(key);
        public byte[] GetByteArray(string key) => CloudServices.GetByteArray(key);
        public double GetDouble(string key) => CloudServices.GetDouble(key);
        public float GetFloat(string key) => CloudServices.GetFloat(key);
        public int GetInt(string key) => CloudServices.GetInt(key);
        public long GetLong(string key) => CloudServices.GetLong(key);
        public string GetString(string key) => CloudServices.GetString(key);

        public void SetBool(string key, bool value) => CloudServices.SetBool(key, value);
        public void SetByteArray(string key, byte[] value) => CloudServices.SetByteArray(key, value);
        public void SetDouble(string key, double value) => CloudServices.SetDouble(key, value);
        public void SetFloat(string key, float value) => CloudServices.SetFloat(key, value);
        public void SetInt(string key, int value) => CloudServices.SetInt(key, value);
        public void SetLong(string key, long value) => CloudServices.SetLong(key, value);
        public void SetString(string key, string value) => CloudServices.SetString(key, value);

        public void RemoveKey(string key) => CloudServices.RemoveKey(key);

        public override string ToString()
        {
            if (IsAvailable()) return $"CloudManager(Online)";
            else return $"CloudManager(Offline)";
        }
    }
}
