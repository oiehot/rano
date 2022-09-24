#nullable enable

using UnityEngine;
using Rano.App;

namespace Rano.Update
{
    public class UpdateInfo
    {
        public SVersion latestVersion;

        public bool IsUpdatable()
        {
            SVersion currentVersion = new SVersion(Application.version);
            if (latestVersion > currentVersion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}