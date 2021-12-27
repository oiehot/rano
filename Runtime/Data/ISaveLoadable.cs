// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano.SaveSystem
{
    public interface ISaveLoadable
    {
        void ClearState();
        void DefaultState();
        object CaptureState();
        void RestoreState(object state);
    }
}