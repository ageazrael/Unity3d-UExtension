using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UExtension
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownAttributeDrawer : InspectorExtensionPropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var rAttribute = this.attribute as DropdownAttribute;
            return base.GetPropertyHeight(property, label) * (rAttribute.PreviewValue ? 2 : 1);
        }

        protected override void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rAttribute = this.attribute as DropdownAttribute;

            var rSelectionPosition = position;
            if (rAttribute.PreviewValue)
                rSelectionPosition = RectExtension.SplitByHeight(ref position, true, base.GetPropertyHeight(property, label), 0);
            if (rAttribute.MappingValues != null)
                this.OnGUIDictionaryMappingValues(rSelectionPosition, property, label);
            else
                this.OnGUIMappingValueName(rSelectionPosition, property, label);

            if (rAttribute.PreviewValue)
            {
                var bOldEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, new GUIContent("\tPreview"));
                GUI.enabled = bOldEnabled;
            }
        }

        private void OnGUIDictionaryMappingValues(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                string[] rKeys;
                int[] rValues;
                var nSelectionIndex = this.GetKeyValuesInteger(out rKeys, out rValues, property);

                EditorGUI.BeginChangeCheck();
                var nNewSelectionIndex = EditorGUI.Popup(position, label.text, nSelectionIndex, rKeys);
                if (EditorGUI.EndChangeCheck())
                    property.intValue = rValues[nNewSelectionIndex];
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                string[] rKeys;
                float[] rValues;
                var nSelectionIndex = this.GetKeyValuesFloat(out rKeys, out rValues, property);

                EditorGUI.BeginChangeCheck();
                var nNewSelectionIndex = EditorGUI.Popup(position, label.text, nSelectionIndex, rKeys);
                if (EditorGUI.EndChangeCheck())
                    property.floatValue = rValues[nNewSelectionIndex];
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                string[] rKeys;
                string[] rValues;
                var nSelectionIndex = this.GetKeyValuesString(out rKeys, out rValues, property);

                EditorGUI.BeginChangeCheck();
                var nNewSelectionIndex = EditorGUI.Popup(position, label.text, nSelectionIndex, rKeys);
                if (EditorGUI.EndChangeCheck())
                    property.stringValue = rValues[nNewSelectionIndex];
            }
            else
            {
                this.PropertyDrawerError(position, property, typeof(int), typeof(float), typeof(string));
            }
        }
        private int  GetKeyValuesInteger(out string[] rKeys, out int[] rValues, SerializedProperty property)
        {
            var rAttribute = this.attribute as DropdownAttribute;

            var nIndex = 0;
            var nSelectionIndex = -1;
            rKeys = new string[rAttribute.MappingValues.Count];
            rValues = new int[rAttribute.MappingValues.Count];
            foreach (var rPair in rAttribute.MappingValues)
            {
                rKeys[nIndex] = rPair.Key;
                if (rPair.Value is int)
                    rValues[nIndex] = (int)rPair.Value;
                else if (rPair.Value is float)
                    rValues[nIndex] = System.Convert.ToInt32((float)rPair.Value);
                else if (rPair.Value is string)
                    rValues[nIndex] = System.Convert.ToInt32((string)rPair.Value);

                if (rValues[nIndex] == property.intValue)
                    nSelectionIndex = nIndex;

                nIndex++;
            }
            return nSelectionIndex;
        }
        private int GetKeyValuesFloat(out string[] rKeys, out float[] rValues, SerializedProperty property)
        {
            var rAttribute = this.attribute as DropdownAttribute;

            var nIndex = 0;
            var nSelectionIndex = -1;
            rKeys = new string[rAttribute.MappingValues.Count];
            rValues = new float[rAttribute.MappingValues.Count];
            foreach (var rPair in rAttribute.MappingValues)
            {
                rKeys[nIndex] = rPair.Key;
                if (rPair.Value is float)
                    rValues[nIndex] = (float)rPair.Value;
                else if (rPair.Value is int)
                    rValues[nIndex] = System.Convert.ToSingle((int)rPair.Value);
                else if (rPair.Value is string)
                    rValues[nIndex] = System.Convert.ToSingle((string)rPair.Value);

                if (System.Math.Abs(rValues[nIndex] - property.floatValue) < float.Epsilon)
                    nSelectionIndex = nIndex;

                nIndex++;
            }
            return nSelectionIndex;
        }
        private int GetKeyValuesString(out string[] rKeys, out string[] rValues, SerializedProperty property)
        {
            var rAttribute = this.attribute as DropdownAttribute;

            var nIndex = 0;
            var nSelectionIndex = -1;
            rKeys = new string[rAttribute.MappingValues.Count];
            rValues = new string[rAttribute.MappingValues.Count];
            foreach (var rPair in rAttribute.MappingValues)
            {
                rKeys[nIndex] = rPair.Key;
                if (rPair.Value is string)
                    rValues[nIndex] = (string)rPair.Value;
                else if (rPair.Value is int)
                    rValues[nIndex] = System.Convert.ToString((int)rPair.Value);
                else if (rPair.Value is float)
                    rValues[nIndex] = System.Convert.ToString((float)rPair.Value);

                if (rValues[nIndex] == property.stringValue)
                    nSelectionIndex = nIndex;

                nIndex++;
            }
            return nSelectionIndex;
        }
        private void OnGUIMappingValueName(Rect position, SerializedProperty property, GUIContent label)
        {
            var rAttribute = this.attribute as DropdownAttribute;
            if (string.IsNullOrEmpty(rAttribute.MappingValueName))
            {
                this.PropertyDrawerError(position, property, "MappingValueName is empty!");
                return;
            }

            var rTargetObject = this.GetTargetObject(property);

            var nBindingFlags = BindingFlags.Instance|BindingFlags.Static|BindingFlags.GetField|BindingFlags.GetProperty|BindingFlags.NonPublic|BindingFlags.Public;
            var rMemberInfoes = rTargetObject.GetType().GetMember(rAttribute.MappingValueName, nBindingFlags);
            var rFindMemberInfo = default(MemberInfo);
            foreach (var rInfo in rMemberInfoes)
            {
                var rMemberDataType = rInfo.GetMemberDataType();
                if (!typeof(IDictionary).IsAssignableFrom(rMemberDataType) ||
                    rMemberDataType.GetGenericArguments()[0] != typeof(string))
                    continue;

                var rValueType = rMemberDataType.GetGenericArguments()[1];

                if ((property.propertyType == SerializedPropertyType.Integer         && rValueType == typeof(int)) ||
                    (property.propertyType == SerializedPropertyType.Float           && rValueType == typeof(float)) ||
                    (property.propertyType == SerializedPropertyType.String          && rValueType == typeof(string)) ||
                    (property.propertyType == SerializedPropertyType.Color           && rValueType == typeof(Color)) ||
                    (property.propertyType == SerializedPropertyType.ObjectReference && typeof(Object).IsAssignableFrom(rValueType)) || 
                    (property.propertyType == SerializedPropertyType.LayerMask       && rValueType == typeof(LayerMask)) ||
                    (property.propertyType == SerializedPropertyType.Enum            && rValueType == typeof(System.Enum)) ||
                    (property.propertyType == SerializedPropertyType.Vector2         && rValueType == typeof(Vector2)) ||
                    (property.propertyType == SerializedPropertyType.Vector2Int      && rValueType == typeof(Vector2Int)) ||
                    (property.propertyType == SerializedPropertyType.Vector3         && rValueType == typeof(Vector3)) ||
                    (property.propertyType == SerializedPropertyType.Vector3Int      && rValueType == typeof(Vector3Int)) ||
                    (property.propertyType == SerializedPropertyType.Vector4         && rValueType == typeof(Vector4)) ||
                    (property.propertyType == SerializedPropertyType.Rect            && rValueType == typeof(Rect)) ||
                    (property.propertyType == SerializedPropertyType.RectInt         && rValueType == typeof(RectInt)) ||
                    (property.propertyType == SerializedPropertyType.AnimationCurve  && rValueType == typeof(AnimationCurve)) ||
                    (property.propertyType == SerializedPropertyType.Bounds          && rValueType == typeof(Bounds)) ||
                    (property.propertyType == SerializedPropertyType.BoundsInt       && rValueType == typeof(BoundsInt)) ||
                    (property.propertyType == SerializedPropertyType.Gradient        && rValueType == typeof(Gradient)) ||
                    (property.propertyType == SerializedPropertyType.Quaternion      && rValueType == typeof(Quaternion)))
                {
                    rFindMemberInfo = rInfo;
                    break;
                }
            }
            if (rFindMemberInfo == null)
            {
                this.PropertyDrawerError(position, property, "MappingValueName:{0} typeof Dictionary<string,{1}>",
                    rAttribute.MappingValueName, property.propertyType.ToString());
                return;
            }

            var rMappingDict = rFindMemberInfo.GetMemberDataValue<IDictionary>(rTargetObject);
            var rKeys = new string[rMappingDict.Count];
            var rValues = new object[rMappingDict.Count];
            var nIndex = 0;
            var nSelectionIndex = -1;
            var rEnumerator = rMappingDict.GetEnumerator();
            while (rEnumerator.MoveNext())
            {
                var rPair = rEnumerator.Entry;
                rKeys[nIndex] = (string)rPair.Key;
                rValues[nIndex] = rPair.Value;

                if ((property.propertyType == SerializedPropertyType.Integer         && (int)rPair.Value == property.intValue) ||
                    (property.propertyType == SerializedPropertyType.Float           && System.Math.Abs((float)rPair.Value - property.floatValue) < float.Epsilon) ||
                    (property.propertyType == SerializedPropertyType.String          && (string)rPair.Value == property.stringValue) ||
                    (property.propertyType == SerializedPropertyType.Color           && (Color)rPair.Value == property.colorValue) ||
                    (property.propertyType == SerializedPropertyType.ObjectReference && (Object)rPair.Value == property.objectReferenceValue) || 
                    (property.propertyType == SerializedPropertyType.Vector2         && (Vector2)rPair.Value == property.vector2Value) ||
                    (property.propertyType == SerializedPropertyType.Vector2Int      && (Vector2Int)rPair.Value == property.vector2IntValue) ||
                    (property.propertyType == SerializedPropertyType.Vector3         && (Vector3)rPair.Value == property.vector3Value) ||
                    (property.propertyType == SerializedPropertyType.Vector3Int      && (Vector3Int)rPair.Value == property.vector3IntValue) ||
                    (property.propertyType == SerializedPropertyType.Vector4         && (Vector4)rPair.Value == property.vector4Value) ||
                    (property.propertyType == SerializedPropertyType.Rect            && (Rect)rPair.Value == property.rectValue) ||
                    (property.propertyType == SerializedPropertyType.RectInt         && ((RectInt)rPair.Value).position == property.rectIntValue.position && ((RectInt)rPair.Value).size == property.rectIntValue.size) ||
                    (property.propertyType == SerializedPropertyType.AnimationCurve  && this.Equal((AnimationCurve)rPair.Value, property.animationCurveValue)) ||
                    (property.propertyType == SerializedPropertyType.Bounds          && (Bounds)rPair.Value == property.boundsValue) ||
                    (property.propertyType == SerializedPropertyType.BoundsInt       && (BoundsInt)rPair.Value == property.boundsIntValue) ||
                    (property.propertyType == SerializedPropertyType.Quaternion      && (Quaternion)rPair.Value == property.quaternionValue))
                {
                    nSelectionIndex = nIndex;
                }

                ++nIndex;
            }

            EditorGUI.BeginChangeCheck();
            var nNewSelectionIndex = EditorGUI.Popup(position, label.text, nSelectionIndex, rKeys);
            if (EditorGUI.EndChangeCheck())
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                    property.intValue = (int)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Float)
                    property.floatValue = (float)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.String)
                    property.stringValue = (string)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Color)
                    property.colorValue = (Color)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.ObjectReference)
                    property.objectReferenceValue = (Object)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Vector2)
                    property.vector2Value = (Vector2)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                    property.vector2IntValue = (Vector2Int)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Vector3)
                    property.vector3Value = (Vector3)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Vector3Int)
                    property.vector3IntValue = (Vector3Int)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Vector4)
                    property.vector4Value = (Vector4)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Rect)
                    property.rectValue = (Rect)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.RectInt)
                    property.rectIntValue = (RectInt)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.AnimationCurve)
                    property.animationCurveValue = (AnimationCurve)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Bounds)
                    property.boundsValue = (Bounds)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.BoundsInt)
                    property.boundsIntValue = (BoundsInt)rValues[nNewSelectionIndex];
                else if (property.propertyType == SerializedPropertyType.Quaternion)
                    property.quaternionValue = (Quaternion)rValues[nNewSelectionIndex];
            }
        }

        private bool Equal(AnimationCurve a, AnimationCurve b)
        {
            if (a.Equals(b))
                return true;

            if (a.postWrapMode != b.postWrapMode)
                return false;
            if (a.preWrapMode != b.preWrapMode)
                return false;
            if (a.length != b.length)
                return false;
            for (var nIndex = 0; nIndex < a.length; ++ nIndex)
            {
                if (System.Math.Abs(a.keys[nIndex].inTangent - b.keys[nIndex].inTangent) > float.Epsilon)
                    return false;
                if (System.Math.Abs(a.keys[nIndex].outTangent - b.keys[nIndex].outTangent) > float.Epsilon)
                    return false;
                if (System.Math.Abs(a.keys[nIndex].inWeight - b.keys[nIndex].inWeight) > float.Epsilon)
                    return false;
                if (System.Math.Abs(a.keys[nIndex].outWeight - b.keys[nIndex].outWeight) > float.Epsilon)
                    return false;
                if (a.keys[nIndex].weightedMode != b.keys[nIndex].weightedMode)
                    return false;
                if (System.Math.Abs(a.keys[nIndex].value - b.keys[nIndex].value) > float.Epsilon)
                    return false;
                if (System.Math.Abs(a.keys[nIndex].time - b.keys[nIndex].time) > float.Epsilon)
                    return false;
            }
            return true;
        }
    }
}