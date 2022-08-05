using System;
using UnityEngine;

namespace Rano.SaveSystem
{
    public sealed class SaveableManager : BaseComponent
    {
        public bool IncludeInactive { get; set; } = true;
        public bool AutoSaveOnPause { get; set; } = false;
        public bool AutoSaveOnFocusOut { get; set; } = false;
        public bool AutoSaveOnExit { get; set; } = true;
        public Action OnSave { get; set; }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            if (AutoSaveOnExit == true)
            {
                Log.Info("OnApplicationQuit");
                Log.Info("AutoSaveOnExit가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                Save();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true && AutoSaveOnPause)
            {
                Log.Info($"OnApplicationPause({pause})");
                Log.Info("AutoSaveOnPause가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                Save();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false && AutoSaveOnFocusOut)
            {
                Log.Info($"OnApplicationFocus({focus})");
                Log.Info("OnApplicationFocus가 켜져 있으므로 모든Saveable 상태를 InMemoryDB에 저장합니다");
                Save();
            }
        }
        
        public void Save()
        {
            OnSave?.Invoke();
            Capture();
            Game.Database.Save();
        }

        private void Capture()
        {
            foreach (var saveable in GameObject.FindObjectsOfType<SaveableEntity>(IncludeInactive))
            {
                // TODO: 같은 Id로 두번 저장하는 경우 경고처리.
                string id = saveable.Id;
                Log.Info($"{id} 게임오브젝트 상태저장");
                var gameObjectState = saveable.CaptureState();
                Game.Database.SetDictionary(id, gameObjectState);
            }
        }

        [ContextMenu(nameof(LogStatus))]
        public void LogStatus()
        {
            Log.Info($"{nameof(SaveableManager)}");
            Log.Info($"  IncludeInactive: {IncludeInactive}");
            Log.Info($"  AutoSaveOnPause: {AutoSaveOnPause}");
            Log.Info($"  AutoSaveOnFocusOut: {AutoSaveOnFocusOut}");
            Log.Info($"  AutoSaveOnExit: {AutoSaveOnExit}");
        }
    }
}