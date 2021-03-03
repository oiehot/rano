// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
    public interface ILoadSave
    {
        void Load(string filePath);
        void Save(string filePath, byte[] bytes);
    }
}