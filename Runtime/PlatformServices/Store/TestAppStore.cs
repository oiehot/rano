#if UNITY_EDITOR

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Rano.App;

namespace Rano.PlatformServices.Store
{
    public class TestAppStore : IAppStore
    {
        private string _bundleId;
        public string BundleId => _bundleId;
        
        public string PageUrl => $"https://play.google.com/store/apps/details?id={BundleId}";
        public string BrowserUrl => $"market://details?id={BundleId}";

        public TestAppStore(string bundleId)
        {
            _bundleId = bundleId;
        }
        
        private SVersion? GetVersionOrNullByPageText(string text)
        {
            // ...
            // <span class=“htlgb”>1.0.7</span>
            // ...
            Regex regex = new Regex("<span class=\"htlgb\">([0-9.]+)</span>");
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

#endif