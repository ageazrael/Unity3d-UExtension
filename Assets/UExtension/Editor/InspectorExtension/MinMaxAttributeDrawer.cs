using UnityEngine;
using UnityEditor;

namespace UExtension
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxAttributeDrawer : InspectorExtensionPropertyDrawer
    {
        protected override void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rAttribute = this.attribute as MinMaxAttribute;

            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (rAttribute.IsSlider)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.IntSlider(position, property, (int)rAttribute.MinValue, (int)rAttribute.MaxValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.DoChangedCallMethod(property);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var newValue = EditorGUI.IntField(position, label, property.intValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.intValue = Mathf.Clamp(newValue, (int)rAttribute.MinValue, (int)rAttribute.MaxValue);
                        this.DoChangedCallMethod(property);
                    }
                }
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                if (rAttribute.IsSlider)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.Slider(position, property, rAttribute.MinValue, rAttribute.MaxValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.DoChangedCallMethod(property);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var newValue = EditorGUI.FloatField(position, label, property.floatValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.floatValue = Mathf.Clamp(newValue, rAttribute.MinValue, rAttribute.MaxValue);
                        this.DoChangedCallMethod(property);
                    }
                }
            }
            else if (property.propertyType == SerializedPropertyType.Vector2)
            {
                if (rAttribute.IsSlider)
                {
                    var sliderPosition = EditorGUI.PrefixLabel(position, label);
                    var leftFloatField = RectExtension.SplitByWidth(ref sliderPosition, true, EditorGUIUtility.fieldWidth, 5);
                    var rightFloatField = RectExtension.SplitByWidth(ref sliderPosition, false, EditorGUIUtility.fieldWidth, 5);

                    EditorGUI.BeginChangeCheck();
                    var sliderValue = property.vector2Value;
                    sliderValue.x = EditorGUI.FloatField(leftFloatField, sliderValue.x);
                    EditorGUI.MinMaxSlider(sliderPosition, ref sliderValue.x, ref sliderValue.y, rAttribute.MinValue, rAttribute.MaxValue);
                    sliderValue.y = EditorGUI.FloatField(rightFloatField, sliderValue.y);

                    sliderValue.x = Mathf.Clamp(sliderValue.x, rAttribute.MinValue, sliderValue.y);
                    sliderValue.y = Mathf.Clamp(sliderValue.y, sliderValue.x, rAttribute.MaxValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector2Value = sliderValue;
                        this.DoChangedCallMethod(property);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        vector2Value.x = Mathf.Clamp(vector2Value.x, rAttribute.MinValue, vector2Value.y);
                        vector2Value.y = Mathf.Clamp(vector2Value.y, vector2Value.x, rAttribute.MaxValue);
                        property.vector2Value = vector2Value;
                        this.DoChangedCallMethod(property);
                    }
                }
            }
            else
            {
                this.PropertyDrawerError(position, property, typeof(int), typeof(float), typeof(Vector2));
            }
            EditorGUI.EndProperty();
        }
    }
}