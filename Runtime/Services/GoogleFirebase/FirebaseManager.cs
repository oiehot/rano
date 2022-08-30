#nullable enable

using System.Threading.Tasks;
using Firebase;

namespace Rano.Services.GoogleFirebase
{
    public class FirebaseManager : ManagerComponent
    {
        private FirebaseApp? _app;
        public bool IsAvailable => _app != null;

        public async Task<bool> InitializeAsync()
        {
            Log.Sys("Firebase Initializing...");
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                Log.Sys("Firebase Initialized");
                _app = FirebaseApp.DefaultInstance;
                return true;
            }
            else
            {
                Log.Warning("Firebase Initialize Failed");
                Log.Warning($"Could not resolve all Firebase dependencies ({dependencyStatus})");
                _app = null;
                return false;
            }
        }
    }
}