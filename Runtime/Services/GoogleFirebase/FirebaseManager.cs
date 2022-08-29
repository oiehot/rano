#nullable enable

using System.Threading.Tasks;
using Firebase;

namespace Rano.Services.GoogleFirebase
{
    public class FirebaseManager : UnityEngine.MonoBehaviour
    {
        public enum EState
        {
            None,
            Initialized
        }

        private EState _status;
        private FirebaseApp? _app;
        public bool IsAvailable => (_app != null) && (_status == EState.Initialized);

        public async Task InitializeAsync()
        {
            Log.Sys("Firebase Initializing...");
            // 파이어베이스 의존성을 체크한다.
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available) {
                Log.Sys("Firebase Initialized");
                #if UNITY_EDITOR
                    // Log.Warning("FirebaseApp Create instance (for desktop workflow)");
                    // _app = FirebaseApp.Create();
                    _app = FirebaseApp.DefaultInstance;
                #else
                    _app = FirebaseApp.DefaultInstance;
                #endif
                _status = EState.Initialized;
            } else
            {
                Log.Warning("Firebase Initialize Failed");
                Log.Warning($"Could not resolve all Firebase dependencies ({dependencyStatus})");
                _app = null;
                _status = EState.None;
            }
        }
    }
}