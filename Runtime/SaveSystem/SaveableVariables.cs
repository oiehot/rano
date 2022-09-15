#nullable enable

using UnityEngine;
using Rano.Database;

namespace Rano.SaveSystem
{
    public class SaveableBool : MonoBehaviour
    {
        private ILocalDatabase _database;
        private string _key;
        
        public string Key => _key;

        public SaveableBool(ILocalDatabase database, string key)
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