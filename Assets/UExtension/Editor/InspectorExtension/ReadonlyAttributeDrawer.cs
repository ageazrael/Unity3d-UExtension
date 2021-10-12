using UnityEngine;
using UnityEditor;

namespace UExtension
{
    [CustomPropertyDrawer(typeof(ReadonlyAttribute))]
    public class ReadonlyAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var bOldEnabled = GUI.enabled;
            GUI.enabled = false;
            //EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            //EditorGUI.EndProperty();
            GUI.enabled = bOldEnabled;
        }
    }
}