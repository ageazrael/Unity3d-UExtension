using System;
using UnityEngine;

namespace UExtension
{
	/// <summary>
	/// Folder Attribute
	/// </summary>
    public enum FolderType
    {
        AbsolutePath,
        AssetPath,
        ResourcesPath,
        ProjectPath,
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class FolderAttribute : PropertyAttribute
    {
        public static string ProjectPathRoot = Application.dataPath.Replace("/Assets", string.Empty);
        public static string AssetPathRoot   = Application.dataPath;

        public FolderType   Type;

        public FolderAttribute(FolderType type)
        {
            this.Type = type;
        }
    }

	[AttributeUsage(AttributeTargets.Field)]
	public class EnumMaskAttribute : PropertyAttribute {}
}
