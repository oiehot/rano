#nullable enable

using UnityEngine;

namespace Rano.SaveSystem
{
    public class SaveableBool : MonoBehaviour
    {
        private IDatabase _database;
        private string _key;
        
        public string Key => _key;

        public SaveableBool(IDatabase database, string key)
        {
            _database = database;
            _key = key;
        }

        public bool Value
        {
            get => _database.GetBool(_key);
            set => _database.SetBool(_key, value);
        }
    }
}