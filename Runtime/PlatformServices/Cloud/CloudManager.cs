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
        public bool IsFeatureAvailable => CloudServices.IsAvailable();
        public bool IsInitialSynchornized { get; private set; }
        public bool IsSynchronizing { get; private set; }
        public bool IsAvailable
        {
            get
            {
                if (IsFeatureAvailable && GameServices.IsAuthenticated)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

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

        /// <summary>
        /// 로컬데이터에서 수정사항이 생기면 콜백된다.
        /// 동기화시 클라우드로 부터 최신데이터를 다운로드 받고 로컬카피본과 비교하여 차이점이 있으면 콜백됨.
        /// 이 콜백이 호출된 쯤이면 클라우드데이터가 로컬에 덮어씌어진 이후다.
        /// </summary>
        /// <param name="result"></param>
        private void OnSavedDataChange(CloudServicesSavedDataChangeResult result)
        {
            string[] changedKeys = result.ChangedKeys;

            Log.Info("OnSavedDataChange 이벤트 발생.");
            Log.Info($"총 바뀐 키: {changedKeys.Length}");
            foreach (string key in changedKeys)
            {
                Log.Info($"키: {key}");
            }

            CloudSavedDataChangeReasonCode reason = result.ChangeReason;
            switch (reason)
            {
                case CloudSavedDataChangeReasonCode.ServerChange:
                    Log.Info($"이유: 서버가 변경됨.");
                    break;
                case CloudSavedDataChangeReasonCode.InitialSyncChange:
                    Log.Info($"이유: 최초 동기화.");
                    break;
                case CloudSavedDataChangeReasonCode.QuotaViolationChange:
                    Log.Info($"이유: 쿼터 위반.");
                    break;
                case CloudSavedDataChangeReasonCode.AccountChange:
                    Log.Info($"이유: 계정 변경.");
                    break;
            }
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

        /// <summary>
        /// 로컬카피와 클라우드카피를 동기화한다.
        /// 클라우드에 있는 복사본이 로컬메모리로 덮어씌어진다.
        /// 차이점이 있으면 OnSavedDataChange이 콜백된다.
        /// </summary>
        /// <param name="onResult">완료 콜백</param>
        /// <returns></returns>
        public IEnumerator SynchronizeCoroutine(Action<bool> onResult=null)
        {
            // TODO: 이미 동기화 중이면 생략한다 or Queue 처리한다.
            bool syncResult = false;
            bool syncCompleted = false;

            if (IsSynchronizing == true)
            {
                Log.Info("클라우드 동기화가 요청되었으나 이미 진행중이므로 생략합니다.");
                onResult?.Invoke(false);
                yield break;
            }

            IsSynchronizing = true;
            Log.Info("클라우드 동기화 시작.");
            CloudServices.Synchronize((CloudServicesSynchronizeResult result) =>
            {
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
                if (IsInitialSynchornized == false) IsInitialSynchornized = true;
            });

            while (syncCompleted == false)
            {
                // TODO: 동기화 시간제한.
                yield return YieldCache.WaitForFixedUpdate;
            }

            IsSynchronizing = false;
            if (syncResult == true) onResult?.Invoke(true);
            else onResult?.Invoke(false);
        }

        #region 로컬카피 편집 메소드들

        public void SetBool(string key, bool value) => CloudServices.SetBool(key, value);
        public void SetByteArray(string key, byte[] value) => CloudServices.SetByteArray(key, value);
        public void SetDouble(string key, double value) => CloudServices.SetDouble(key, value);
        public void SetFloat(string key, float value) => CloudServices.SetFloat(key, value);
        public void SetInt(string key, int value) => CloudServices.SetInt(key, value);
        public void SetLong(string key, long value) => CloudServices.SetLong(key, value);
        public void SetString(string key, string value) => CloudServices.SetString(key, value);

        public bool GetBool(string key) => CloudServices.GetBool(key);
        public byte[] GetByteArray(string key) => CloudServices.GetByteArray(key);
        public double GetDouble(string key) => CloudServices.GetDouble(key);
        public float GetFloat(string key) => CloudServices.GetFloat(key);
        public int GetInt(string key) => CloudServices.GetInt(key);
        public long GetLong(string key) => CloudServices.GetLong(key);
        public string GetString(string key) => CloudServices.GetString(key);

        public void RemoveKey(string key) => CloudServices.RemoveKey(key);

        #endregion
    }
}
