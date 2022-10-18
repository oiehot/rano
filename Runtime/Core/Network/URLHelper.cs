#nullable enable

namespace Rano.Network
{
    public static class URLHelper
    {
        public static string EscapeURL(string url)
        {
            string result;
            result = UnityEngine.Networking.UnityWebRequest.EscapeURL(url);
            result = result.Replace("+", "%20");
            return result;
        }
    }
}