using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using Rano.App;

namespace Rano.Store
{
    public class GoogleAppStore : IAppStore
    {
        private string _bundleId;
        public string BundleId => _bundleId;
        
        /// <summary>
        /// 구글 앱 페이지 Url
        /// ex) https://play.google.com/store/apps/details?id=com.oiehot.afo2
        /// ex) https://play.google.com/store/apps/details?id=com.oiehot.afo2&hl=en_US&gl=US
        /// </summary>
        public string PageUrl => $"https://play.google.com/store/apps/details?id={BundleId}";
        public string BrowserUrl => $"market://details?id={BundleId}";

        /// <summary>
        /// 생성자 
        /// </summary>
        /// <param name="bundleId">앱 번들 Id 예)com.oiehot.afo2</param>
        public GoogleAppStore(string bundleId)
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
        
        public IEnumerator GetLatestVersionCoroutine(System.Action<EGetVersionResult, SVersion?> callback)
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
                            callback(EGetVersionResult.Success, version.Value);
                        }
                        else
                        {
                            callback(EGetVersionResult.ParseError, null);
                        }
                        break;                    
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                    default:
                        callback(EGetVersionResult.ConnectionError, null);
                        break;
                }
            }
        }
    }
}