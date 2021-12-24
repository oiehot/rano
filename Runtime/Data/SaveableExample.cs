#if UNITY_EDITOR

// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using UnityEngine;
using Rano;

namespace Rano.SaveSystem
{
    public class SaveableExample : MonoBehaviour, ISaveable
    {
        [SerializeField] private int level;
        [SerializeField] private int exp;
        [SerializeField] private int extra;

        [Serializable]
        private struct PlayerData
        {
            public int level;
            public int exp;
            public int extra;
        }

        public virtual void ClearState()
        {
            //base.ClearState();
        }

        public virtual void DefaultState()
        {
            //base.DefaultState();
            level = 1;
            exp = 1;
            extra = 0;
        }

        public virtual object CaptureState()
        {
            //var state = (PlayerData)base.CaptureState();
            var state = new PlayerData();
            state.level = level;
            state.exp = exp;
            state.extra = extra;
            return state;
        }

        public virtual void RestoreState(object state)
        {
            ClearState();
            //base.RestoreState(state);
            var data = (PlayerData)state;
            level = data.level;
            exp = data.exp;
            extra = data.extra;

            // TODO: Instantiate All Extras
            //for (int i = 0; i < extra; i++)
            //{
            //    GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //}
        }
    }
}

#endif