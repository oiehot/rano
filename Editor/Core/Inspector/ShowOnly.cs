#if false

using UnityEngine;
using UnityEditor;

namespace Rano.Editor.Inspector
{
    /// <summary>ShowOnly 속성이 적용된 프로퍼티를 인스펙터에서 그린다.</summary>
    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position, SerializedProperty prop,
            GUIContent label)
        {
            string value;

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    value = prop.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    value = prop.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    value = prop.floatValue.ToString("0.00000");
                    break;
                case SerializedPropertyType.String:
                    value = prop.stringValue;
                    break;
                default:
                    value = "(unsupported)";
                    break;
            }

            EditorGUI.LabelField(position, label.text, value);
        }
    }
}

#endif