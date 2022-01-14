// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;

namespace Rano
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object Lock = new object();
                
        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null) _instance = new T();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Instance를 생성하기 위한 빈 메소드.
        /// </summary>
        public void EmptyMethod()
        {
        }
    }
}