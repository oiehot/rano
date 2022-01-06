// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using Rano.Addressable;

namespace Rano.LoadingSystem
{
    public struct LoadingManagerCommand
    {
        public enum Type
        {
            None,
            AddScene,
            RemoveScene,
            ActiveScene,
            EnableUI,
            DisableUI,
            FadeOut,
            FadeIn,
            ShowBodyText,
            HideBodyText
        }

        public Type type;
        public Address address;
        public float fadeSpeed;

        public LoadingManagerCommand(Type type)
        {
            this.type = type;
            this.address = new Address("");
            this.fadeSpeed = 0.0f;
        }

        public LoadingManagerCommand(Type type, Address address)
        {
            this.type = type;
            this.address = address;
            this.fadeSpeed = 0.0f;
        }

        public LoadingManagerCommand(Type type, float fadeSpeed)
        {
            this.type = type;
            this.address = new Address("");
            this.fadeSpeed = fadeSpeed;
        }
    }
}