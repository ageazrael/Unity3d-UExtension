using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

namespace UExtension
{
    /// <summary>
    /// FolderAttribute editor
    /// </summary>
	[CustomPropertyDrawer(typeof(FolderAttribute))]
	public class FolderAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
            var rAttribute = this.attribute as FolderAttribute;

			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            const float ButtonSize = 18;

            position.xMax -= ButtonSize;

			var indent = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;

            EditorGUI.LabelField(position, property.stringValue);

            var ButtonRect = position;
            ButtonRect.xMin = position.xMax; 
            ButtonRect.xMax += ButtonSize;
            if (GUI.Button(ButtonRect, ".."))
            {
                string rNewPath = string.Empty;
                string rDefaultFolder = string.Empty;
                if (rAttribute.Type == FolderType.AssetPath || rAttribute.Type == FolderType.ResourcesPath)
                    rDefaultFolder = FolderAttribute.AssetPathRoot;
                else if (rAttribute.Type == FolderType.ProjectPath)
                    rDefaultFolder = FolderAttribute.ProjectPathRoot;
                bool bCompleted = false;
                do
                {
                    rNewPath = EditorUtility.OpenFolderPanel("select folder", rDefaultFolder, string.Empty);

                    if (rAttribute.Type == FolderType.AssetPath || rAttribute.Type == FolderType.ProjectPath)
                    {
                        bCompleted = rNewPath.Contains(rDefaultFolder);

                        rNewPath = rNewPath.Replace(rDefaultFolder, "");
                    }
                    else
                    {
                        bCompleted = true;
                    }
                }
                while (!bCompleted);

                property.stringValue = rNewPath;
            }

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}

	/// <summary>
	/// Enum mask attribute drawer.
	/// </summary>
	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var rTargetEnum = GetBaseProperty<Enum> (property);

			EditorGUI.BeginProperty (position, label, property);

			if (rTargetEnum.GetType().IsDefined(typeof(FlagsAttribute), false))
			{
				var rNewEnumValue = EditorGUI.EnumMaskPopup (position, label, rTargetEnum);
				property.intValue = (int)Convert.ChangeType (rNewEnumValue, rTargetEnum.GetType ());
			}
			else
			{
				var rNewEnumValue = EditorGUI.EnumPopup (position, label, rTargetEnum);
				property.intValue = (int)Convert.ChangeType (rNewEnumValue, rTargetEnum.GetType ());
			}

			EditorGUI.EndProperty ();
		}

		static T GetBaseProperty<T>(SerializedProperty prop)
		{
			// Separate the steps it takes to get to this property
			string[] separatedPaths = prop.propertyPath.Split('.');

			// Go down to the root of this serialized property
			System.Object reflectionTarget = prop.serializedObject.targetObject as object;
			// Walk down the path to get the target object
			foreach (var path in separatedPaths)
			{
				FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
				reflectionTarget = fieldInfo.GetValue(reflectionTarget);
			}
			return (T) reflectionTarget;
		}
	}
}