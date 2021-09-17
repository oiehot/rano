
// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Rano.App;

namespace Rano.Store
{
    public enum Result
    {
        Success,
        ParsingError,
        ConnectionError
    }

    public abstract class Appstore
    {
        protected Appstore() {}

        protected IEnumerator Lookup(string appId, System.Action<UnityWebRequest.Result, string> callback)
        {
            string output;
            UnityWebRequest request = UnityWebRequest.Get(GetPageUrl(appId));
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                output = request.downloadHandler.text;
                callback(request.result, output);
            }
            else
            {
                output = null;
                callback(request.result, output);
            }
        }

        public IEnumerator GetVersion(string appId, System.Action<Result, Version?> callback)
        {
            yield return Lookup(appId, (UnityWebRequest.Result result, string output) => {
                Version? version = null;

                if (result == UnityWebRequest.Result.Success)
                {
                    version = ParseVersion(output);
                    if (version.HasValue)
                    {
                        callback(Result.Success, version);
                    }
                    else
                    {
                        callback(Result.ParsingError, version);
                    }
                }
                else
                {
                    callback(Result.ConnectionError, version);
                }
            });
        }

        public void Open(string appId)
        {
            Application.OpenURL(GetBrowserUrl(appId));
        }

        /// <summary>앱페이지를 분석 할 때 GET하는 주소를 돌려준다.</summary>
        protected abstract string GetPageUrl(string appId);

        /// <summary>브라우져로 열 때 사용하는 주소를 돌려준다.</summary>
        protected abstract string GetBrowserUrl(string appId);

        /// <summary>GetVersion에서 사용하는 추상함수. TemplateMethodPattern</summary>
        protected abstract Version? ParseVersion(string output);
    }
}