using UnityEngine;
using System.Collections;
using System.IO;

using UnityEditor;

namespace UExtension
{
    public static class EditorExtension
    {
        public static string CreateNewAssetName(string rAssetName)
        {
            string rCreatePath = string.Empty;
            if (Selection.activeObject)
            {
                if (Selection.activeObject.GetType().Name == "DefaultAsset") // Folder
                    rCreatePath = AssetDatabase.GetAssetPath(Selection.activeObject);
                else
                    rCreatePath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
            }
            else
            {
                rCreatePath = "Assets/";
            }

            if (rCreatePath[rCreatePath.Length - 1] != '/')
                rCreatePath += "/";

            var rFileName       = Path.GetFileNameWithoutExtension(rAssetName);
            var rFileExtension  = Path.GetExtension(rAssetName);

            int     nIndex       = 0;
            string  rNewFileName = string.Empty;
            do
            {
                if (nIndex == 0)
                    rNewFileName = rCreatePath + rAssetName;
                else
                    rNewFileName = string.Format("{0}{1} {2}{3}", rCreatePath, rFileName, nIndex, rFileExtension);

                if (!File.Exists(rNewFileName))
                    return rNewFileName;

            } while (++nIndex < 999);

            return string.Empty;
        }
    }
}