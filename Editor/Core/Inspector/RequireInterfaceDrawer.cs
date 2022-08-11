#if false

// Ref: https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/

using UnityEngine;
using UnityEditor;
using UnityEngine.Scripting;

namespace Rano.Editor.Inspector
{
    [CustomPropertyDrawer(typeof(RequiredInterfaceAttribute))]
    public class RequiredInterfaceAttribute : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var requiredAttribute = this.attribute as RequireInterfaceAttribute;
                EditorGUI.BeginProperty(position, label, property);
                property.objectReferenceValue = EditorGUI.ObjectField(
                    position,
                    label,
                    property.objectReferenceValue,
                    requiredAttribute.requiredType,
                    true
                );
                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;
                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
                GUI.color = previousColor;
            }
        }
    }
}

#endif