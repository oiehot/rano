using System;
using System.Collections;
using Rano.App;
using UnityEngine;

namespace Rano.PlatformServices.Store
{
    public enum EAppStoreResult
    {
        Success,
        ParseError,
        ConnectionError
    }

    public interface IAppStore
    {
        /// <summary>
        /// 앱스토어에서 사용할 수 있는 이 앱의 번들 Id.
        /// </summary>
        public string BundleId { get; }
        
        /// <summary>앱페이지를 분석 할 때 GET하는 주소를 돌려준다.</summary>
        public string PageUrl { get; }

        /// <summary>브라우져로 열 때 사용하는 주소를 돌려준다.</summary>
        public string BrowserUrl { get; }
        
        /// <summary>
        /// 앱스토어에서 이 앱의 마지막 버젼을 얻는다.
        /// </summary>
        public IEnumerator GetLatestVersionCoroutine(
            System.Action<EAppStoreResult, SVersion?> callback
        );
    }
}
