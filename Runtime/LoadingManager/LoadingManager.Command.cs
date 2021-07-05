// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using Rano.Addressable;

namespace Rano
{
    public partial class LoadingManager : MonoSingleton<LoadingManager>
    {    
        public struct Command
        {
            public enum Type
            {
                AddScene,
                RemoveScene,
                ActiveScene,
                FadeOut,
                FadeIn,
                ShowText,
                HideText
            }

            public Type type;
            public Address address;
            public float fadeSpeed;

            public Command(Type type, Address address)
            {
                this.type = type;
                this.address = address;
                this.fadeSpeed = 0.0f;
            }

            public Command(Type type, float fadeSpeed)
            {
                this.type = type;
                this.address = new Address("");
                this.fadeSpeed = fadeSpeed;
            }
        }
    }
}