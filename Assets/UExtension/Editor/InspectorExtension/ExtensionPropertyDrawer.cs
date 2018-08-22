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
        protected void PropertyDrawerError(Rect position, SerializedProperty property, string format, params object[] args)
        {
            var text = new StringBuilder();
            text.AppendFormat("property {0} use {1} attribute error: {2}", property.name, this.attribute.GetType().Name, string.Format(format, args));
            EditorGUI.LabelField(position, text.ToString());
            Debug.LogError(text.ToString(), property.serializedObject.targetObject);
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