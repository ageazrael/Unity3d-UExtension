using UnityEngine;
using UnityEditor;
using System;

namespace UExtension
{
    /// <summary>
    /// Enum mask attribute drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public class EnumMaskAttributeDrawer : InspectorExtensionPropertyDrawer
    {
        protected override void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                this.PropertyDrawerError(position, property, typeof(Enum));
                return;
            }

            var rTargetEnum = this.GetBaseProperty<Enum>(property);

            EditorGUI.BeginProperty(position, label, property);
            if (rTargetEnum.GetType().IsDefined(typeof(FlagsAttribute), false))
            {
                var rNewEnumValue = EditorGUI.EnumFlagsField(position, label, rTargetEnum);
                property.intValue = (int)Convert.ChangeType(rNewEnumValue, rTargetEnum.GetType());
            }
            else
            {
                var rNewEnumValue = EditorGUI.EnumPopup(position, label, rTargetEnum);
                property.intValue = (int)Convert.ChangeType(rNewEnumValue, rTargetEnum.GetType());
            }
            EditorGUI.EndProperty();
        }
    }
}