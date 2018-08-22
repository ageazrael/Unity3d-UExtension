using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UExtension
{

    public static class AssetImporterSettingEditor
    {
        public static string GetSelectionPath()
        {
            var rObjects = Selection.GetFiltered<Object>(SelectionMode.Assets);
            if (rObjects.Length == 0)
                return string.Empty;

            if (rObjects[0] as DefaultAsset)
                return AssetDatabase.GetAssetPath(rObjects[0]);
            else
                return Path.GetDirectoryName(AssetDatabase.GetAssetPath(rObjects[0]));
        }

        [MenuItem("Assets/Create/UExtension/Asset Importer Setting")]
        public static void CreateAssetImporter()
        {
            var rSelectionPath = GetSelectionPath();
            var rAssetImporterSetting = ScriptableObject.CreateInstance<AssetImporterSetting>();
            AssetDatabase.CreateAsset(rAssetImporterSetting, PathExtension.Combine(rSelectionPath, "AssetImporterSetting.asset"));
            ProjectWindowUtil.ShowCreatedAsset(rAssetImporterSetting);
        }
        [MenuItem("Assets/Create/UExtension/Asset Importer Setting", true)]
        public static bool CreateAssetImporterVerify()
        {
            var rSelectionPath = GetSelectionPath();
            return !AssetDatabase.LoadAssetAtPath<AssetImporterSetting>(
                PathExtension.Combine(rSelectionPath, "AssetImporterSetting.asset")
            );
        }
    }


    public class AssetImporterSettingPostProcesss : AssetPostprocessor
    {
        private List<AssetImporterSetting> Settings = new List<AssetImporterSetting>();

        public AssetImporterSettingPostProcesss()
        {
        }

        void UpdateSettings()
        {
            this.Settings.Clear();
            var rAssetGuids = AssetDatabase.FindAssets("t:" + typeof(AssetImporterSetting));
            foreach (var rGuid in rAssetGuids)
            {
                this.Settings.Add(AssetDatabase.LoadAssetAtPath<AssetImporterSetting>(AssetDatabase.GUIDToAssetPath(rGuid)));
            }
        }

        void OnPreprocessTexture()
        {
        }
    }
}