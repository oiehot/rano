#nullable enable

using UnityEngine;

namespace Rano.EventSystem
{
    public class DescriptionBaseSO : SerializableScriptableObject
    {
        [TextArea] public string description;
    }
}