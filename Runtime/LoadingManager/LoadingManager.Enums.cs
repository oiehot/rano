// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
    public partial class LoadingManager : MonoSingleton<LoadingManager>
    {
        public enum Status
        {
            None,
            Downloading,
            DownloadCompleted,
            DownloadFailed,
            LoadingScene,
            LoadingSceneCompleted,
            LoadingSceneFailed,
            LoadingAsset,
            LoadingAssetCompleted,
            LoadingAssetFailed,
            UnloadingScene,
            UnloadingSceneCompleted,
            UnloadingSceneFailed,
            UnloadingAsset,
            UnloadingAssetCompleted,
            UnloadingAssetFailed
        }
    }
}