using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace UExtension
{
	/// <summary>
	/// Build target path.
	/// </summary>
	public static class BuildTargetPath
	{
		public static string ToPath(BuildTarget rBuildTarget)
		{
			switch (rBuildTarget) {
			case BuildTarget.Android:
				return "Android";

			case BuildTarget.iOS:
				return "iOS";

			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				return "Linux";

			case BuildTarget.tvOS:
				return "tvOS";

			case BuildTarget.SamsungTV:
				return "SamsungTV";

			//case BuildTarget.Nintendo3DS:
			//	return "Nintendo3DS";

			//case BuildTarget.PS3:
			//	return "PS3";

			case BuildTarget.PS4:
				return "PS4";

			case BuildTarget.PSM:
				return "PSM";

			case BuildTarget.PSP2:
				return "PSP2";

			//case BuildTarget.XBOX360:
			//	return "XBOX360";

			case BuildTarget.XboxOne:
				return "XboxOne";

			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "OSX";

			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";

			case BuildTarget.Tizen:
				return "Tizen";

			case BuildTarget.WebGL:
				return "WebGL";

			case BuildTarget.WiiU:
				return "WiiU";

			case BuildTarget.WSAPlayer:
				return "WSA";

			default:
				throw new UnityException (string.Format ("Not support platform {0}", rBuildTarget.ToString()));
			}
		}
	}
	
    /// <summary>
    /// AssetBundleBuilder
    /// </summary>
    public static class AssetBundleBuilder
    {
        public static void Build(AssetBundleSettingBase rBuildSetting, BuildTarget rBuildTarget)
        {
            if (!rBuildSetting)
                return;

            var rExportPath = PathExtension.Combine(FolderAttribute.ProjectPathRoot,
				rBuildSetting.ExportPath, BuildTargetPath.ToPath(rBuildTarget));
            if (!Directory.Exists(rExportPath))
                Directory.CreateDirectory(rExportPath);

            var rBuildSettingPath   = AssetDatabase.GetAssetPath(rBuildSetting);
            var rAllBundles         = rBuildSetting.GetAllBundle();
            var rManifest           = BuildPipeline.BuildAssetBundles(rExportPath, rAllBundles,
                rBuildSetting.BuildOptions, rBuildTarget);

            rBuildSetting = AssetDatabase.LoadAssetAtPath<AssetBundleSettingBase>(rBuildSettingPath);

            var rAssetBundleInfo = new ABInfoArchive();
            if (rManifest)
            {
                foreach(var rBundle in rAllBundles)
                {
                    var rABEntity    = new ABInfoEntity();
                    var rBundleName  = rBundle.assetBundleName + "." + rBundle.assetBundleVariant;
                    
                    rABEntity.Depend = rManifest.GetAllDependencies(rBundleName);
                    rABEntity.Hash   = rManifest.GetAssetBundleHash(rBundleName).ToString();
                    foreach(var rAssetName in rBundle.assetNames)
                    {
                        var rLoaderName = rBuildSetting.AssetPathToLoaderName(rAssetName);
                        if (!rABEntity.LoadNameToFullName.ContainsKey(rLoaderName))
                            rABEntity.LoadNameToFullName.Add(rLoaderName, rAssetName);

						if (rAssetName.Contains(".unity"))
							rABEntity.SceneNames.Add(Path.GetFileNameWithoutExtension(rAssetName));
                    }

                    rAssetBundleInfo.Entitys.Add(rBundleName, rABEntity);
                }
            }
            rAssetBundleInfo.SaveArchive(PathExtension.Combine(rExportPath, ABInfoArchive.BinaryFileName));
        }
    }
    /// <summary>
    /// AssetBundleSettingPreview
    /// </summary>
    public class AssetBundleSettingPreview
    {
        Dictionary<string, string[]> AssetBundleInfos   = new Dictionary<string, string[]>();
        bool[]                       FadeGroupValue     = new bool[0];
        Vector2                      ScrollViewPosition = Vector2.zero;

        public void OnGUI()
        {
            ScrollViewPosition = EditorGUILayout.BeginScrollView(ScrollViewPosition);
            int nIndex = 0;
            foreach(var rInfoPair in AssetBundleInfos)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUI.indentLevel ++;
                FadeGroupValue[nIndex] = EditorGUILayout.Foldout(FadeGroupValue[nIndex], rInfoPair.Key);
                if (FadeGroupValue[nIndex])
                {
                    EditorGUI.indentLevel ++;
                    foreach (var rAssetPath in rInfoPair.Value)
                        EditorGUILayout.LabelField(rAssetPath);
                    EditorGUI.indentLevel --;
                }
                EditorGUI.indentLevel --;
                EditorGUILayout.EndVertical();
                ++nIndex;
            }
            EditorGUILayout.EndScrollView();
        }

        public void Refresh(AssetBundleSettingBase rBuildSetting)
        {
            AssetBundleInfos.Clear();

            var rAllBundles = rBuildSetting.GetAllBundle();
            foreach(var rBuildBundle in rAllBundles)
            {
                AssetBundleInfos.Add(rBuildBundle.assetBundleName + "." + rBuildBundle.assetBundleVariant,
                    rBuildBundle.assetNames);
            }
            FadeGroupValue = new bool[rAllBundles.Length];
        }
    }
    /// <summary>
    /// AssetBundleBuilderWindow
    /// </summary>
    public class AssetBundleBuilderWindow : EditorWindow
    {
        public string[]                     AssetBundleSettings;
        public int                          BuildSettingSelectedIndex;
        public AssetBundleSettingPreview    BuildSettingPreview = new AssetBundleSettingPreview();

        void OnEnable()
        {
            this.HandleRefreshBuildSetting();

            if (this.CheckBuildValid())
                this.HandleRefreshBuildSettingPreview();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            var nNewSelectedBuildSettingIndex = EditorGUILayout.Popup("Setting", 
                this.BuildSettingSelectedIndex, this.AssetBundleSettings);
            if (nNewSelectedBuildSettingIndex != this.BuildSettingSelectedIndex)
            {
                this.BuildSettingSelectedIndex = nNewSelectedBuildSettingIndex;
                this.HandleRefreshBuildSettingPreview();
            }
            if (GUILayout.Button("S", GUILayout.MaxWidth(24), GUILayout.MaxHeight(14)))
                this.HandleSelectedBuildSetting();
            EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField("Target Platform", EditorUserBuildSettings.activeBuildTarget.ToString());

            if (CheckBuildValid() && GUILayout.Button("Build"))
                this.HandleBuild();

            if (GUILayout.Button("Refresh 'Build Setting'"))
                this.HandleRefreshBuildSetting();

            this.BuildSettingPreview.OnGUI();
        }

        bool CheckBuildValid()
        {
            return this.BuildSettingSelectedIndex < this.AssetBundleSettings.Length && this.BuildSettingSelectedIndex >= 0;
        }

        void HandleSelectedBuildSetting()
        {
            var rBuildSettingPath = this.AssetBundleSettings[this.BuildSettingSelectedIndex].Replace('\\', '/');
            var rBuildSetting = AssetDatabase.LoadAssetAtPath<AssetBundleSettingBase>(rBuildSettingPath);
            if (rBuildSetting)
                Selection.activeObject = rBuildSetting;
        }

        void HandleRefreshBuildSettingPreview()
        {
            var rBuildSettingPath = this.AssetBundleSettings[this.BuildSettingSelectedIndex].Replace('\\', '/');
            var rBuildSetting = AssetDatabase.LoadAssetAtPath<AssetBundleSettingBase>(rBuildSettingPath);
            if(rBuildSetting)
                this.BuildSettingPreview.Refresh(rBuildSetting);
        }

        void HandleBuild()
        {
            if (!this.CheckBuildValid())
                return;

            var rBuildSettingPath = this.AssetBundleSettings[this.BuildSettingSelectedIndex].Replace('\\', '/');

            var rBuildSetting = AssetDatabase.LoadAssetAtPath<AssetBundleSettingBase>(rBuildSettingPath);
            if (!rBuildSetting)
                return;

			AssetBundleBuilder.Build(rBuildSetting, EditorUserBuildSettings.activeBuildTarget);
        }
        void HandleRefreshBuildSetting()
        {
            this.AssetBundleSettings = AssetDatabase.FindAssets(AssetBundleSettingBase.FindAssetsFilter);
            for (int nIndex = 0; nIndex < this.AssetBundleSettings.Length; ++ nIndex)
                this.AssetBundleSettings[nIndex] = AssetDatabase.GUIDToAssetPath(this.AssetBundleSettings[nIndex]).Replace("/", "\\");
        }
    }
}