// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Rano;

namespace Rano.Network
{
    public enum HttpRequestResult
    {
        InProgress,
        Success,
        Error
    }

    public static class Http
    {
        public static async Task<byte[]> GetBytesAsync(string url)
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        public static async Task<string> GetStringAsync(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

                // TODO: Wait for done
                while (!asyncOperation.isDone)
                {
                    await Task.Delay(100);
                }

                UnityWebRequest.Result requestResult;
                requestResult = request.result;
                if (request.error == null)
                {
                    Log.Info($"GET Success ({url})");
                    return request.downloadHandler.text;
                }
                else
                {
                    Log.Info($"GET Failed ({url})");
                    throw new HttpRequestException($"{request.error} ({url})");
                }
            }
        }

        public static IEnumerator CoGetString(string url, Action<HttpRequestResult, string> onResult)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
                yield return asyncOperation; // 완료될 때까지 대기

                UnityWebRequest.Result requestResult;
                requestResult = request.result;
                switch (requestResult)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        Log.Warning($"GET {url} => Failed ({request.error})");
                        onResult?.Invoke(HttpRequestResult.Error, null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Log.Info($"GET {url} => Success");
                        DownloadHandler downloadHandler = request.downloadHandler;
                        string result = downloadHandler.text;
                        onResult?.Invoke(HttpRequestResult.Success, result);
                        break;
                }
            }
        }
    }
}