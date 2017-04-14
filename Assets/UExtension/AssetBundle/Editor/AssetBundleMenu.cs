using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.IO;

namespace UExtension
{

    public class AssetBundleMenu
    {
		[MenuItem("Tools/UExtension/Build AssetBundle")]
		public static void MCBuildAssetBundle()
		{
			var rEditorWindow = EditorWindow.GetWindow<AssetBundleBuilderWindow>();
			rEditorWindow.titleContent = new GUIContent("Build AssetBundle");
			rEditorWindow.Show();
			rEditorWindow.Focus();
		}
		[MenuItem("Tools/UExtension/Exclud in AssetBundle asset")]
		static void ExcludeInAssetBundleAsset()
		{
			var rBuildSettings = AssetDatabase.FindAssets(AssetBundleSettingBase.FindAssetsFilter);
			for (int nIndex = 0; nIndex < rBuildSettings.Length; ++ nIndex)
			{
				rBuildSettings[nIndex] = AssetDatabase.GUIDToAssetPath(rBuildSettings[nIndex]);

				var rBuildSetting = AssetDatabase.LoadAssetAtPath<AssetBundleSettingBase>(rBuildSettings[nIndex]);
				if (!rBuildSetting.ExcludeInPlayer)
					continue;

				var rTargetPath     = rBuildSetting.ResourcesPath.Replace("/Resources", "/NotResources");
				var rNeedMoveAssets = new List<string>();
				foreach(var rBundle in rBuildSetting.GetAllBundle())
					rNeedMoveAssets.AddRange(rBundle.assetNames);
				foreach(var rAssetSourcePath in rNeedMoveAssets)
				{
					var rAssetTargetPath = rAssetSourcePath.Replace(rBuildSetting.ResourcesPath, rTargetPath);
					var rTargetDirectory = Path.GetDirectoryName(rAssetTargetPath);
					if (!Directory.Exists(rTargetDirectory))
					{
						Directory.CreateDirectory(rTargetDirectory);
						AssetDatabase.Refresh();
					}

					AssetDatabase.MoveAsset(rAssetSourcePath, rAssetTargetPath);
					Debug.LogFormat("MoveAsset({0} => {1})", rAssetSourcePath, rAssetTargetPath);
				}
			}
		}
		[MenuItem("Tools/UExtension/Restore in AssetBundle asset")]
        static void RestoreInAssetBundleAsset()
        {
            var rBuildSettings = AssetDatabase.FindAssets(AssetBundleSettingBase.FindAssetsFilter);
            for (int nIndex = 0; nIndex < rBuildSettings.Length; ++ nIndex)
            {
                rBuildSettings[nIndex] = AssetDatabase.GUIDToAssetPath(rBuildSettings[nIndex]);

                var rBuildSetting = AssetDatabase.LoadAssetAtPath<AssetBundleSettingBase>(rBuildSettings[nIndex]);
                if (!rBuildSetting.ExcludeInPlayer)
                    continue;

                var rTargetPath     = rBuildSetting.ResourcesPath.Replace("/Resources", "/NotResources");
                if (!Directory.Exists(rTargetPath))
                    continue;

                var rNeedMoveAssets = new List<string>();
                foreach(var rAssetGUID in AssetDatabase.FindAssets("t:Object", new string[] {rTargetPath }))
                {
                    var rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
                    if (Directory.Exists(rAssetPath))
                        continue;

                    if (!rNeedMoveAssets.Contains(rAssetPath))
                        rNeedMoveAssets.Add(rAssetPath);
                }
                foreach(var rAssetSourcePath in rNeedMoveAssets)
                {
                    var rAssetTargetPath = rAssetSourcePath.Replace(rTargetPath, rBuildSetting.ResourcesPath);
                    var rTargetDirectory = Path.GetDirectoryName(rAssetTargetPath);
                    if (!Directory.Exists(rTargetDirectory))
                        Directory.CreateDirectory(rTargetDirectory);

                    AssetDatabase.MoveAsset(rAssetSourcePath, rAssetTargetPath);
                    Debug.LogFormat("MoveAsset({0} => {1})", rAssetSourcePath, rAssetTargetPath);
                }
                AssetDatabase.DeleteAsset(rTargetPath);
            }
        }
    }

}