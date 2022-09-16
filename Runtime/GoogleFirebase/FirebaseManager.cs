#nullable enable

using System;
using System.Threading.Tasks;
using Firebase;

namespace Rano.GoogleFirebase
{
    public class FirebaseManager : ManagerComponent
    {
        private FirebaseApp? _app;
        public bool IsAvailable => _app != null;

        public async Task<bool> InitializeAsync()
        {
            Log.Sys("Firebase Initializing...");
            _app = null;
            
            DependencyStatus dependencyStatus; // enum
            try
            {
                dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Log.Warning("Firebase Check and Fix dependencies Error");
                Log.Warning("Firebase Initialize Failed");
                return false;
            }

            if (dependencyStatus == DependencyStatus.Available)
            {
                try
                {
                    _app = FirebaseApp.DefaultInstance;
                }
                catch (Exception e)
                {
                    Log.Warning("Can't get firebase app default instance");
                    Log.Warning("Firebase Initialize Failed");
                    return false;
                }
            }
            else
            {
                Log.Warning($"Could not resolve all Firebase dependencies ({dependencyStatus})");
                Log.Warning("Firebase Initialize Failed");
                return false;
            }
            
            Log.Sys("Firebase Initialized");
            return true;
        }
    }
}