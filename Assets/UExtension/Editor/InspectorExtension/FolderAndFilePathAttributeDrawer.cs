using UnityEngine;
using UnityEditor;

namespace UExtension
{
    /// <summary>
    /// FolderAttribute editor
    /// </summary>
	[CustomPropertyDrawer(typeof(FolderAttribute))]
    public class FolderAttributeDrawer : InspectorExtensionPropertyDrawer
    {
        protected override void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                this.PropertyDrawerError(position, property, typeof(string));
                return;
            }

            var rAttribute = this.attribute as FolderAttribute;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            const float ButtonSize = 18;

            position.xMax -= ButtonSize;

            var indent = EditorGUI.indentLevel;

            EditorGUI.indentLevel = 0;

            if (rAttribute.Editable)
                property.stringValue = EditorGUI.TextField(position, property.stringValue);
            else
                EditorGUI.LabelField(position, property.stringValue);

            var ButtonRect = position;
            ButtonRect.xMin = position.xMax;
            ButtonRect.xMax += ButtonSize;
            if (GUI.Button(ButtonRect, ".."))
            {
                string rNewPath = string.Empty;
                string rDefaultFolder = string.Empty;
                if (rAttribute.Type == PathType.AssetPath || rAttribute.Type == PathType.ResourcesPath)
                    rDefaultFolder = PathRoot.AssetPathRoot;
                else if (rAttribute.Type == PathType.ProjectPath)
                    rDefaultFolder = PathRoot.ProjectPathRoot;
                bool bCompleted = false;
                bool bCancel = false;
                do
                {
                    rNewPath = EditorUtility.OpenFolderPanel("select folder", rDefaultFolder, string.Empty);
                    if (string.IsNullOrEmpty(rNewPath))
                    {
                        bCancel = true;
                        break;
                    }

                    if (rAttribute.Type == PathType.AssetPath || rAttribute.Type == PathType.ProjectPath)
                    {
                        if (string.IsNullOrEmpty(rAttribute.Key) || rNewPath.Contains(rAttribute.Key))
                        {
                            bCompleted = rNewPath.Contains(rDefaultFolder);
                            rNewPath = rNewPath.Replace(rDefaultFolder, "");
                        }
                    }
                    else if (string.IsNullOrEmpty(rAttribute.Key) || rNewPath.Contains(rAttribute.Key) ||
                             string.IsNullOrEmpty(rNewPath))
                    {
                        bCompleted = true;
                    }
                }
                while (!bCompleted);

                if (!bCancel)
                    property.stringValue = rNewPath;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// FilePathAttribute editor
    /// </summary>
    [CustomPropertyDrawer(typeof(FilePathAttribute))]
    public class FilePathAttributeDrawer : InspectorExtensionPropertyDrawer
    {
        protected override void OnInspectorExtensionGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                this.PropertyDrawerError(position, property, typeof(string));
                return;
            }

            var rAttribute = this.attribute as FilePathAttribute;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            const float ButtonSize = 18;

            position.xMax -= ButtonSize;

            var indent = EditorGUI.indentLevel;

            EditorGUI.indentLevel = 0;

            if (rAttribute.Editable)
                property.stringValue = EditorGUI.TextField(position, property.stringValue);
            else
                EditorGUI.LabelField(position, property.stringValue);

            var ButtonRect = position;
            ButtonRect.xMin = position.xMax;
            ButtonRect.xMax += ButtonSize;
            if (GUI.Button(ButtonRect, ".."))
            {
                string rNewPath = string.Empty;
                string rDefaultFolder = string.Empty;
                if (rAttribute.Type == PathType.AssetPath || rAttribute.Type == PathType.ResourcesPath)
                    rDefaultFolder = PathRoot.AssetPathRoot;
                else if (rAttribute.Type == PathType.ProjectPath)
                    rDefaultFolder = PathRoot.ProjectPathRoot;
                bool bCompleted = false;
                bool bCancel = false;
                do
                {
                    rNewPath = EditorUtility.OpenFilePanelWithFilters("Select File", rDefaultFolder, rAttribute.Filters);
                    if (string.IsNullOrEmpty(rNewPath))
                    {
                        bCancel = true;
                        break;
                    }

                    if (rAttribute.Type == PathType.AssetPath || rAttribute.Type == PathType.ProjectPath)
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

                if (!bCancel)
                    property.stringValue = rNewPath;
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}