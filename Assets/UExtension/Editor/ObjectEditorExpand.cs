using UnityEngine;
using System.Collections;

using UnityEditor;

namespace UExtension
{
    public static class ObjectEditorExpand
    {
        public static string GetAssetGUID(this Object obj)
        {
            var rAssetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(rAssetPath))
                return string.Empty;

            return AssetDatabase.AssetPathToGUID(rAssetPath);
        }

        public static string GetAssetPath(this Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }
    }
}