#nullable enable

using System.Collections.Generic;
using Rano.Auth;

namespace Rano.Analytics
{
    public interface IAnalyticsManager
    {
        public bool IsInitialized { get; }
        public void Initialize(IAuthManager authManager);
        // public void SetUserID(string userID);
        public void SetUserProperty(string key, string value);
        public void LogEvent(string eventName);
        public void LogEvent(string eventName, string parameterName, string parameterValue);
        public void LogEvent(string eventName, string parameterName, double parameterValue);
        public void LogEvent(string eventName, string parameterName, long parameterValue);
        public void LogEvent(string eventName, string parameterName, int parameterValue);
        public void LogEvent(string eventName, Dictionary<string, object> parameterDictionary);
    }
}