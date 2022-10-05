#nullable enable

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Rano.Network
{
    public enum EHttpRequestResult
    {
        InProgress,
        Success,
        Error
    }

    public static class Http
    {
        // public static async Task<byte[]> GetBytesAsync(string url)
        // {
        //     await Task.FromResult(0);
        //     throw new NotImplementedException();
        // }

        // public static string GetString(string url)
        // {
        //     using (UnityWebRequest request = UnityWebRequest.Get(url))
        //     {
        //         UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
        //         
        //         while (!asyncOperation.isDone)
        //         {
        //         }
        //
        //         // var requestResult = request.result;
        //         if (request.error == null)
        //         {
        //             Log.Info($"GET Success ({url})");
        //             return request.downloadHandler.text;
        //         }
        //         else
        //         {
        //             Log.Info($"GET Failed ({url})");
        //             throw new HttpRequestException($"{request.error} ({url})");
        //         }
        //     }
        // }

        public static async Task<string?> GetStringAsync(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
                
                // Wait for done
                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                // var requestResult = request.result;
                if (request.error == null)
                {
                    Log.Info($"GET Success ({url})");
                    return request.downloadHandler.text;
                }
                else
                {
                    Log.Warning($"GET Failed (url:{url}, reason:{request.error})");
                    return null;
                }
            }
        }

        // public static IEnumerator GetStringCoroutine(string url, Action<EHttpRequestResult, string> onResult)
        // {
        //     using (UnityWebRequest request = UnityWebRequest.Get(url))
        //     {
        //         UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
        //         yield return asyncOperation; // 완료될 때까지 대기
        //
        //         var requestResult = request.result;
        //         switch (requestResult)
        //         {
        //             case UnityWebRequest.Result.ConnectionError:
        //             case UnityWebRequest.Result.DataProcessingError:
        //             case UnityWebRequest.Result.ProtocolError:
        //                 Log.Warning($"GET {url} => Failed ({request.error})");
        //                 onResult?.Invoke(EHttpRequestResult.Error, null);
        //                 break;
        //             case UnityWebRequest.Result.Success:
        //                 Log.Info($"GET {url} => Success");
        //                 DownloadHandler downloadHandler = request.downloadHandler;
        //                 string result = downloadHandler.text;
        //                 onResult?.Invoke(EHttpRequestResult.Success, result);
        //                 break;
        //             default:
        //                 throw new Exception($"Unprocessable Result ({requestResult})");
        //         }
        //     }
        // }
    }
}