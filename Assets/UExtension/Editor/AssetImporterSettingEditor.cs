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
    }


    public class AssetImporterSettingPostProcesss : AssetPostprocessor
    {
        private Dictionary<string, List<AssetImporterSetting.ImporterSetting>> Settings;

        public AssetImporterSettingPostProcesss()
        {
            this.Settings = new Dictionary<string, List<AssetImporterSetting.ImporterSetting>>();
        }

        void UpdateSettings()
        {
            this.Settings.Clear();

            var rImporterSettings = new List<AssetImporterSetting>();
            var rAssetGuids = AssetDatabase.FindAssets("t:" + typeof(AssetImporterSetting));
            foreach (var rGuid in rAssetGuids)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGuid);
                Debug.Log($"Load {rAssetPath}");

                rImporterSettings.Add(AssetDatabase.LoadAssetAtPath<AssetImporterSetting>(rAssetPath));
            }

            rImporterSettings.Sort((a, b) => {
                var rAPath = AssetDatabase.GetAssetPath(a);
                var rBPath = AssetDatabase.GetAssetPath(b);

                return rAPath.CompareTo(rBPath);
            });

            foreach(var rProcessSetting in rImporterSettings)
            {
                foreach(var rSetting in rImporterSettings)
                {
                    if (rProcessSetting == rSetting)
                        continue;

                    var rProcessAssetPath = AssetDatabase.GetAssetPath(rProcessSetting);
                    var rSettingAssetPath = AssetDatabase.GetAssetPath(rSetting);

                    if (!this.Settings.TryGetValue(rProcessAssetPath, out var rSettingList))
                    {
                        rSettingList = new List<AssetImporterSetting.ImporterSetting>();
                        this.Settings.Add(rProcessAssetPath, rSettingList);
                    }
                }
            }
        }

        void OnPreprocessTexture()
        {
        }
    }
}