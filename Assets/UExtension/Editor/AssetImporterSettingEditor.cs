using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UExtension
{

    public class AssetImporterSettingPostProcesss : AssetPostprocessor
    {
        private Dictionary<string, List<AssetImporterSetting.ImporterFieldSetting>> ImporterSettings;
        private List<string> ImporterSettingPaths = new List<string>();

        public AssetImporterSettingPostProcesss()
        {
            this.ImporterSettings = new Dictionary<string, List<AssetImporterSetting.ImporterFieldSetting>>();
        }


        protected float mLastRefreshTime;
        private void RefreshSettings()
        {
            if (Time.realtimeSinceStartup - this.mLastRefreshTime < 2.0f)
                return;
            this.mLastRefreshTime = Time.realtimeSinceStartup;

            this.ImporterSettings.Clear();
            this.ImporterSettingPaths.Clear();

            var rImporterSettings = new List<AssetImporterSetting>();
            var rAssetGuids = AssetDatabase.FindAssets("t:" + typeof(AssetImporterSetting));
            foreach (var rGuid in rAssetGuids)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGuid);
                Debug.Log($"Load {rAssetPath}");

                rImporterSettings.Add(AssetDatabase.LoadAssetAtPath<AssetImporterSetting>(rAssetPath));

                this.ImporterSettingPaths.Add(rAssetPath);
            }

            rImporterSettings.Sort((a, b) => {
                var rAPath = AssetDatabase.GetAssetPath(a);
                var rBPath = AssetDatabase.GetAssetPath(b);

                return rAPath.CompareTo(rBPath);
            });

            foreach(var rProcessSetting in rImporterSettings)
            {
                var rProcessAssetPath = AssetDatabase.GetAssetPath(rProcessSetting);
                this.MergeSetting(Path.GetDirectoryName(rProcessAssetPath).Replace('\\', '/'), rProcessSetting);

                //foreach (var rSetting in rImporterSettings)
                //{
                //    if (rProcessSetting == rSetting)
                //        continue;
                    
                //    if (rProcessAssetPath.Contains(AssetDatabase.GetAssetPath(rSetting)))
                //        this.MergeSetting(rProcessAssetPath, rSetting);
                //}
            }
        }

        private void MergeSetting(string rAssetPath, AssetImporterSetting rSetting)
        {
            if (rSetting.ImporterFieldSettings == null)
                return;

            if (!this.ImporterSettings.TryGetValue(rAssetPath, out var rSettingList))
            {
                rSettingList = new List<AssetImporterSetting.ImporterFieldSetting>();
                this.ImporterSettings.Add(rAssetPath, rSettingList);
            }
            

            foreach(var rNewFieldSetting in rSetting.ImporterFieldSettings)
            {
                var bAdd = true;
                foreach(var rFieldSetting in rSettingList)
                {
                    if (rNewFieldSetting.ImporterType == rFieldSetting.ImporterType && 
                        rNewFieldSetting.FieldName == rFieldSetting.FieldName)
                    {
                        bAdd = false;

                        rFieldSetting.Contains   = rNewFieldSetting.Contains;
                        rFieldSetting.FieldValue = rNewFieldSetting.FieldValue;
                    }
                }
                if (bAdd)
                    rSettingList.Add(rNewFieldSetting);
            }
        }

        protected void DoPreprocessAsset()
        {
            this.RefreshSettings();
            if (this.ImporterSettingPaths.Contains(this.assetPath))
                return;

            var fStartTime = Time.realtimeSinceStartup;
            var bSettingChanged = false;
            foreach (var rSetting in this.ImporterSettings)
            {
                if (!this.assetPath.Contains(rSetting.Key))
                    continue;

                foreach(var rFieldSetting in rSetting.Value)
                {
                    if (!string.IsNullOrEmpty(rFieldSetting.Contains) && !this.assetPath.Contains(rFieldSetting.Contains))
                        continue;

                    if (rFieldSetting.ImportSettingsMissing && !this.assetImporter.importSettingsMissing)
                        continue;

                    if (string.IsNullOrEmpty(rFieldSetting.FieldName) || string.IsNullOrEmpty(rFieldSetting.FieldName))
                        continue;

                    var rType = rFieldSetting.GetImporterType();
                    if (rType == null)
                        continue;

                    if (!rType.IsAssignableFrom(this.assetImporter.GetType()))
                        continue;

                    var rPropInfo = rType.GetProperty(rFieldSetting.FieldName);
                    if (rPropInfo != null)
                    {
                        if (rPropInfo.PropertyType == typeof(string))
                        {
                            rPropInfo.SetValue(this.assetImporter, rFieldSetting.FieldValue);
                        }
                        else if (rPropInfo.PropertyType == typeof(int))
                        {
                            if (int.TryParse(rFieldSetting.FieldValue, out var nFeildValue))
                                rPropInfo.SetValue(this.assetImporter, nFeildValue);
                        }
                        else if (rPropInfo.PropertyType.IsEnum)
                        {
                            rPropInfo.SetValue(this.assetImporter, 
                                System.Enum.Parse(rPropInfo.PropertyType, rFieldSetting.FieldValue));
                        }

                        bSettingChanged = true;
                    }
                }
            }
            Debug.Log($"Importer({bSettingChanged}) {this.assetPath} {Time.realtimeSinceStartup - fStartTime}s");
        }

        private void OnPreprocessTexture() => this.DoPreprocessAsset();
        private void OnPreprocessModel() => this.DoPreprocessAsset();
        private void OnPreprocessAnimation() => this.DoPreprocessAsset();
        private void OnPreprocessAudio() => this.DoPreprocessAsset();
        private void OnPreprocessSpeedTree() => this.DoPreprocessAsset();
        private void OnPreprocessAsset() => this.DoPreprocessAsset();

    }
}