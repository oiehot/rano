using UnityEngine;

namespace Rano
{
    public class DescriptionBaseSO : SerializableScriptableObject
    {
        [SerializeField, TextArea] private string _description;
    }
}