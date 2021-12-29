#if UNITY_EDITOR || DEVELOPMENT_BUILD

#define USE_ACCOUNT_DATA_TEST

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;
using UnityEngine;
using Rano;

namespace Rano.SaveSystem
{
    public class SaveableExample : MonoBehaviour, ISaveLoadable
    {
#if USE_ACCOUNT_DATA_TEST
        [SerializeField] private int age;
        [SerializeField] private string _name;
#endif

        [SerializeField] private int level;
        [SerializeField] private int exp;
        [SerializeField] private int extra;

        private Dictionary<string, object> dict;

#if USE_ACCOUNT_DATA_TEST
        [Serializable]
        private struct AccountData
        {
            public int age;
            public string name;
        }
#endif

        [Serializable]
        private struct PlayerData
        {
            public int level;
            public int exp;
            public int extra;
#if USE_ACCOUNT_DATA_TEST
            public AccountData account;
#endif
            public Dictionary<string, object> dict;
        }

        void Awake()
        {
            Log.Info("Awake");
            dict = new Dictionary<string, object>();
        }

        void OnEnable()
        {
            Log.Info("OnEnable");
        }

        void OnDisable()
        {
            Log.Info("OnDisable");
        }

        void Start()
        {
            Log.Info("Start");
        }

        public void ClearState()
        {
            level = 1;
            exp = 0;
            extra = 0;
#if USE_ACCOUNT_DATA_TEST
            age = 0;
            _name = null;
#endif
            dict.Clear();
        }

        public void DefaultState()
        {
            ClearState();
            level = 42;
            exp = 1234;
            extra = 7;
            dict["foo"] = 100;
            dict["bar"] = 3.141592f;
#if USE_ACCOUNT_DATA_TEST
            age = 39;
            _name = "Taewoo Lee";
            dict["baz"] = new AccountData
            {
                age = 1,
                name = "baz"
            };
            dict["baz2"] = new AccountData
            {
                age = 2,
                name = "baz2"
            };
#endif
        }

        public object CaptureState()
        {
            var state = new PlayerData();
            state.level = level;
            state.exp = exp;
            state.extra = extra;
#if USE_ACCOUNT_DATA_TEST
            state.account = new AccountData
            {
                age = age,
                name = _name
            };
#endif
            state.dict = dict;
            return state;
        }

        public void ValidateState(object state)
        {
            var data = (PlayerData)state;
            if (data.level < 1) throw new StateValidateException("level이 1미만일 수는 없음");
            if (data.exp < 0) throw new StateValidateException("exp가 1미만일 수는 없음");
            if (data.extra < 0) throw new StateValidateException("extra가 1미만일 수는 없음");
        }

        public void RestoreState(object state)
        {
            ClearState();
            var data = (PlayerData)state;
            level = data.level;
            exp = data.exp;
            extra = data.extra;
#if USE_ACCOUNT_DATA_TEST
            age = data.account.age;
            _name = data.account.name;
#endif
            dict = data.dict;
        }
    }
}

#endif