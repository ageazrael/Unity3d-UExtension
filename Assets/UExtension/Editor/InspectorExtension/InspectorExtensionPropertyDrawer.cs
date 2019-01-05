using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;

namespace UExtension
{
    [CustomPropertyDrawer(typeof(InspectorExtensionAttribute))]
    public abstract class InspectorExtensionPropertyDrawer : PropertyDrawer
    {
        protected bool GetControllerValue(SerializedProperty property, string rControllerValue)
        {
            const BindingFlags kFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            var rTargetObject   = this.GetTargetObject(property);
            var rType           = rTargetObject.GetType();

            var rFieldInfo = rType.GetField(rControllerValue, kFlags);
            if (rFieldInfo != null && rFieldInfo.FieldType == typeof(bool))
            {
                return (bool)rFieldInfo.GetValue(rTargetObject);
            }

            var rPropertyInfo = rType.GetProperty(rControllerValue, kFlags);
            if (rPropertyInfo != null && rPropertyInfo.PropertyType == typeof(bool))
            {
                return (bool)rPropertyInfo.GetValue(rTargetObject, null);
            }

            var rMethodInfo = rType.GetMethod(rControllerValue, kFlags);
            if (rMethodInfo != null && rMethodInfo.ReturnType == typeof(bool) && rMethodInfo.GetParameters().Length == 0)
            {
                return (bool)rMethodInfo.Invoke(rTargetObject, null);
            }

            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var rAttribute = this.attribute as InspectorExtensionAttribute;

            if (string.IsNullOrEmpty(rAttribute.IsVisibleControllerValue))
                return base.GetPropertyHeight(property, label);

            return this.GetControllerValue(property, rAttribute.IsVisibleControllerValue) ? 
                base.GetPropertyHeight(property, label) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rAttribute = this.attribute as InspectorExtensionAttribute;

            if (!string.IsNullOrEmpty(rAttribute.IsVisibleControllerValue) && !this.GetControllerValue(property, rAttribute.IsVisibleControllerValue))
                return;

            var bOldEnable = GUI.enabled;
            var bIsDisable = !string.IsNullOrEmpty(rAttribute.IsEnableControllerValue) && !this.GetControllerValue(property, rAttribute.IsEnableControllerValue);

            if (bIsDisable || rAttribute.Readonly)
                GUI.enabled = false;

            this.OnInspectorExtensionGUI(position, property, label);
            GUI.enabled = bOldEnable;
        }

        protected virtual void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        protected void PropertyDrawerError(Rect position, SerializedProperty property, params Type[] types)
        {
            var text = new StringBuilder();
            text.AppendFormat("property {0} use {1} attribute can be used only on", property.name, this.attribute.GetType().Name);
            foreach(var rType in types)
                text.AppendFormat(" {0}", rType.Name);
            EditorGUI.LabelField(position, text.ToString());
            Debug.LogError(text.ToString(), property.serializedObject.targetObject);
        }
        protected void PropertyDrawerError(Rect position, SerializedProperty property, string format, params object[] args)
        {
            var text = new StringBuilder();
            text.AppendFormat("property {0} use {1} attribute error: {2}", property.name, this.attribute.GetType().Name, string.Format(format, args));
            EditorGUI.LabelField(position, text.ToString());
            Debug.LogError(text.ToString(), property.serializedObject.targetObject);
        }
        protected object GetTargetObject(SerializedProperty property)
        {
            var rPropertyPath = property.propertyPath;
            if (rPropertyPath.Contains("." + property.name))
                rPropertyPath = rPropertyPath.Replace("." + property.name, string.Empty);
            else if (rPropertyPath == property.name)
                rPropertyPath = "";

            return this.GetBaseProperty(property.serializedObject.targetObject, rPropertyPath);
        }
        protected T GetBaseProperty<T>(SerializedProperty prop)
        {
            return (T)this.GetBaseProperty(prop.serializedObject.targetObject, prop.propertyPath);
        }
        protected object GetBaseProperty(object rReflectionTarget, string rPropertyPath)
        {
            // Separate the steps it takes to get to this property
            string[] rSeparatedPaths = rPropertyPath.Split('.');

            // Go down to the root of this serialized property
            // Walk down the path to get the target object
            foreach (var rPath in rSeparatedPaths)
            {
                if (string.IsNullOrEmpty(rPath) || rPath == "Array")
                    continue;

                if (typeof(System.Array).IsAssignableFrom(rReflectionTarget.GetType()))
                {
                    var nStartIndex = rPath.IndexOf('[');
                    var nEndIndex = rPath.LastIndexOf(']');
                    var nDataIndex = int.Parse(rPath.Substring(nStartIndex + 1, nEndIndex - nStartIndex - 1));

                    rReflectionTarget = ((System.Array)rReflectionTarget).GetValue(nDataIndex);
                }
                else
                {
                    var rFieldInfo = rReflectionTarget.GetType().GetField(rPath);
                    rReflectionTarget = rFieldInfo.GetValue(rReflectionTarget);
                }
                
            }
            return rReflectionTarget;
        }
    }

}