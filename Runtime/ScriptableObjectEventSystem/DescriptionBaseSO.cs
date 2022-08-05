using UnityEngine;

namespace Rano.ScriptableObjectEventSystem
{
    public class DescriptionBaseSO : SerializableScriptableObject
    {
        [SerializeField, TextArea] private string _description;
    }
}