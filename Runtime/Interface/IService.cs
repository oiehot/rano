// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
    public enum EServiceState
    {
        None,
        Initialized,
        Running,
        Paused,
        Stopped
    }

    public interface IService
    {
        EServiceState state { get; }
        void Init();
        void Run();
        void Pause();
        void Resume();
        void Stop();
    }
}