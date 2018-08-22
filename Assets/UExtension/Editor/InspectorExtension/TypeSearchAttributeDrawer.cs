using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using System;

namespace UExtension
{
	[CustomPropertyDrawer(typeof(TypeSearchAttribute))]
	public class TypeSearchAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			List<string> PopupNames = (this.attribute as TypeSearchAttribute).TypeFullNames;
			int nSelectinIndex = PopupNames.FindIndex( (arg) => { return arg == property.stringValue; });

			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var indent = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;

			int nNewSelectionIndex = EditorGUI.Popup(position, nSelectinIndex, PopupNames.ToArray());
			if (nNewSelectionIndex != nSelectinIndex)
				property.stringValue = PopupNames[nNewSelectionIndex];

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}

}