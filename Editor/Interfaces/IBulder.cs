// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEditor.Build.Reporting;

namespace RanoEditor.Build
{
    public interface IBuilder
    {
        BuildReport Build();
    }
}