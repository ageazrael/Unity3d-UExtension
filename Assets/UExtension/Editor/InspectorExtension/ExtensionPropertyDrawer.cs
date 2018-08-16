using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;

namespace UExtension
{

    public abstract class ExtensionPropertyDrawer : PropertyDrawer
    {
        protected void PropertyDrawerError(Rect position, SerializedProperty property, params Type[] types)
        {
            var text = new StringBuilder();
            text.AppendFormat("property {0} use {1} attribute can be used only on", property.name, this.attribute.GetType().Name);
            foreach(var rType in types)
                text.AppendFormat(" {0}", rType.Name);
            EditorGUI.LabelField(position, text.ToString());
            Debug.LogError(text.ToString(), property.serializedObject.targetObject);
        }
        protected T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                var fieldInfo = reflectionTarget.GetType().GetField(path);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
            return (T)reflectionTarget;
        }
    }

}