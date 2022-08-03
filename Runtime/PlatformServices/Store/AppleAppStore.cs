using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Rano.App;

namespace Rano.PlatformServices.Store
{
    public class AppleAppStore : IAppStore
    {
        private string _bundleId;
        public string BundleId => _bundleId;
        public string PageUrl => $"http://itunes.apple.com/lookup?bundleId={BundleId}";
        public string BrowserUrl => $"itms-apps://itunes.apple.com/app/id{BundleId}";

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="bundleId">앱 번들 Id 예)1350067922</param>
        public AppleAppStore(string bundleId)
        {
            _bundleId = bundleId;
        }

        private SVersion? GetVersionOrNullByPageText(string text)
        {
            // ...
            // "version":1.0.7
            // ...
            Regex regex = new Regex("\"version\":\"([0-9.]+)\"");
            Match m = regex.Match(text);
            if (m.Success)
            {
                SVersion result = new SVersion(m.Groups[1].Value);
                if (result != SVersion.Min) return result;
            }
            return null;
        }
        
        public IEnumerator GetLatestVersionCoroutine(System.Action<EAppStoreResult, SVersion?> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(PageUrl))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        var version = GetVersionOrNullByPageText(webRequest.downloadHandler.text);
                        if (version.HasValue)
                        {
                            callback(EAppStoreResult.Success, version.Value);
                        }
                        else
                        {
                            callback(EAppStoreResult.ParseError, null);
                        }
                        break;                    
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                    default:
                        callback(EAppStoreResult.ConnectionError, null);
                        break;
                }
            }
        }
    }
}