
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

        /// <summary>앱스토어의 앱페이지 내용을 얻어 콜백호출시 전달한다.</summary>
        protected IEnumerator Lookup(string bundleId, System.Action<UnityWebRequest.Result, string> callback)
        {
            string output;
            UnityWebRequest request = UnityWebRequest.Get(GetPageUrl(bundleId));
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

        /// <summary>앱스토어의 앱페이지 내용을 얻은 뒤 파싱하여 버젼을 얻어내고, 콜백호출시 전달한다.</summary>
        public IEnumerator GetVersion(string bundleId, System.Action<Result, Version?> callback)
        {
            yield return Lookup(bundleId, (UnityWebRequest.Result result, string output) => {
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

        public void Open(string bundleId)
        {
            Application.OpenURL(GetBrowserUrl(bundleId));
        }

        /// <summary>앱페이지를 분석 할 때 GET하는 주소를 돌려준다.</summary>
        protected abstract string GetPageUrl(string bundleId);

        /// <summary>브라우져로 열 때 사용하는 주소를 돌려준다.</summary>
        protected abstract string GetBrowserUrl(string bundleId);

        /// <summary>GetVersion에서 사용하는 추상함수. TemplateMethodPattern</summary>
        protected abstract Version? ParseVersion(string output);
    }
}