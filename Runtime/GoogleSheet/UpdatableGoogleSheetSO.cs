#nullable enable

using UnityEngine;

namespace Rano.GoogleSheet
{
    public abstract class UpdatableGoogleSheetSO<T> : ScriptableObject
    {
        [SerializeField] protected T[] _items = {};

        public T[] Items
        {
            get => _items;
            set => _items = value;
        }
    }
}