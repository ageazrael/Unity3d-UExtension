﻿using System;
using UnityEngine;

namespace UExtension
{
    /// <summary>
    /// Folder Attribute
    /// </summary>
    public enum PathType
    {
        AbsolutePath,
        AssetPath,
        ResourcesPath,
        ProjectPath,
    }
    public static class PathRoot
    {
        public static string ProjectPathRoot = Application.dataPath.Replace("/Assets", string.Empty) + "/";
        public static string AssetPathRoot = Application.dataPath + "/";
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class FolderAttribute : InspectorExtensionAttribute
    {
        public PathType Type;
        public string Key;
        public bool Editable;

        public FolderAttribute(PathType type, string key = "")
        {
            this.Type = type;
            this.Key = key;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class FilePathAttribute : InspectorExtensionAttribute
    {
        public PathType Type;
        public string[] Filters;
        public bool Editable;

        public FilePathAttribute(PathType type, params string[] filters)
        {
            this.Type = type;
            this.Filters = filters;
        }
    }
}
