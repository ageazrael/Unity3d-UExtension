using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UExtension
{
    [CustomPropertyDrawer(typeof(ResizableTextAreaAttribute))]
    public class ResizableTextAreaAttributeDrawer : InspectorExtensionPropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (base.GetPropertyHeight(property, label) <= 0)
                return 0;

            var lineCount = 0;
            if (property.propertyType == SerializedPropertyType.String)
            {
                var str = property.stringValue;
                for (var nIndex = 0; nIndex < str.Length; ++ nIndex)
                {
                    if (str[nIndex] == '\n')
                        lineCount++;
                }
            }
            return EditorGUIUtility.singleLineHeight * 2 + EditorStyles.textArea.lineHeight * lineCount;
        }
        protected override void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.LabelField(RectExtension.SplitByHeight(ref position, true, EditorGUIUtility.singleLineHeight, 0), label);
                EditorGUI.BeginChangeCheck();
                var newText = EditorGUI.TextArea(position, property.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = newText;
                }
            }
            else
            {
                this.PropertyDrawerError(position, property, typeof(string));
            }
            EditorGUI.EndProperty();
        }
    }
}