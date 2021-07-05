// // Copyright (C) OIEHOT - All Rights Reserved
// // Unauthorized copying of this file, via any medium is strictly prohibited
// // Proprietary and confidential
// // Written by Taewoo Lee <oiehot@gmail.com>

// using System;
// using System.Collections;
// using UnityEngine;
// using Rano.Addressable;

// namespace Rano
// {
//     public partial class LoadingManager : MonoSingleton<LoadingManager>
//     {
//         public void QueueDownload(Label label)
//         {
//             AssetDownloader.Instance.QueueDownload(label);
//         }

//         public IEnumerator StartDownload()
//         {
//             status = Status.Downloading;
//             AssetDownloader.Instance.Run();
//             yield return null;

//             // TODO: 완료/실패 여부 판정
//             while (!AssetDownloader.Instance.IsDownloaded())
//             {
//                 percent = AssetDownloader.Instance.percent;
//                 yield return null;
//             }
//             status = Status.DownloadCompleted;
//         }
//     }
// }