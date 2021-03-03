// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

#if false

using UnityEngine;
using System.Net.Http;
using System.Threading; // for Thread.Sleep
using System.Threading.Tasks; // for Task.Run
using LitJson;

namespace Rano
{
    public static class GoogleAppstore
    {
        public static void Open(string id)
        {
            Application.OpenURL($"market://details?id={id}");
        }
    }
}

#endif