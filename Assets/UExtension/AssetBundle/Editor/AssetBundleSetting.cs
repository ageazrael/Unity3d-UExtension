using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.IO;

namespace UExtension
{
    [CreateAssetMenu(menuName = "UExtension/Asset Bundle Setting")]
    public class AssetBundleSetting : AssetBundleSettingBase
    {
        public enum PackageType
        {
            Folder,
            Object,
        }

        [System.Serializable]
        public class SettingItem
        {
            [Folder(PathType.AssetPath)]
            public string       SearchPath;
            public PackageType  PackageType;
            public string       SearchFilter;
            public string       OutputPath;
        }

        public SettingItem[]    Settings;

		public override string ResourcesPath
		{
			get {
                var rSettingPath = AssetDatabase.GetAssetPath(this);
                if (string.IsNullOrEmpty(rSettingPath))
					return string.Empty;
				return PathExtension.GetPathWithoutExtension(rSettingPath) + "/Resources";
			} 
		}

        public override AssetBundleBuild[] GetAllBundle()
        {
            var rAllBundles = new List<AssetBundleBuild>(); 

			if (null != this.Settings)
			{
				foreach (var rItem in this.Settings)
					AddAssetBundleBuild (rAllBundles, rItem);
			}

            return rAllBundles.ToArray();
        }
        public override string AssetPathToLoaderName(string rAssetPath)
        {
            var rExclusivePostfix = PathExtension.GetPathWithoutExtension(rAssetPath);
            if (rExclusivePostfix.Contains(this.ResourcesPath + "/"))
                return rExclusivePostfix.Replace(this.ResourcesPath + "/", string.Empty);
            return rExclusivePostfix;
        }

        void AddAssetBundleBuild(List<AssetBundleBuild> rBuildList, SettingItem rItem)
        {
            if (rItem.PackageType == PackageType.Object)
                AddAssetBundleBuildObject(rBuildList, rItem.SearchFilter, PathExtension.Combine("Assets", rItem.SearchPath), rItem.OutputPath);
            else if (rItem.PackageType == PackageType.Folder)
                AddAssetBundleBuildFolder(rBuildList, rItem.SearchFilter, PathExtension.Combine("Assets", rItem.SearchPath), rItem.OutputPath);
        }
        void AddAssetBundleBuildObject(List<AssetBundleBuild> rBuildList, string rSearchFilter, string rSearchPath, string rOutputPath)
        {
            foreach(var rAssetGUID in AssetDatabase.FindAssets(rSearchFilter, new string[] { rSearchPath }))
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rAssetGUID);
                if(!rAssetPath.Contains(rSearchPath))
                    continue;

                var rABBuild = new AssetBundleBuild();

                var rAssetBundlePath = rAssetPath.Replace(rSearchPath + "/", "");
                rAssetBundlePath = PathExtension.Combine(rOutputPath, Path.GetDirectoryName(rAssetBundlePath), Path.GetFileNameWithoutExtension(rAssetBundlePath));

                rABBuild.assetBundleName = rAssetBundlePath;
                rABBuild.assetBundleVariant = this.Postfix;
                rABBuild.assetNames = new string[] { rAssetPath };

                rBuildList.Add(rABBuild);
            }
        }
        void AddAssetBundleBuildFolder(List<AssetBundleBuild> rBuildList, string rSearchFilter, string rSearchPath, string rOutputPath)
        {
            if (!Directory.Exists(rSearchPath))
                return; // invalid folder

            foreach(var rPackageFolder in Directory.GetDirectories(rSearchPath))
            {
                var rFolder = rPackageFolder.Replace('\\', '/').Replace(
                    System.Environment.CurrentDirectory.Replace('\\', '/') + "/", string.Empty);

                var rAssets = AssetDatabase.FindAssets(rSearchFilter, new string[] { rFolder });
                for (int nIndex = 0; nIndex < rAssets.Length; ++ nIndex)
                    rAssets[nIndex] = AssetDatabase.GUIDToAssetPath(rAssets[nIndex]);

				if (rAssets.Length <= 0)
					continue;

                var rABBuild = new AssetBundleBuild();
                rABBuild.assetBundleName    = PathExtension.Combine(rOutputPath, rFolder.Replace(this.ResourcesPath + "/", string.Empty));
                rABBuild.assetBundleVariant = this.Postfix;
                rABBuild.assetNames         = rAssets;

                rBuildList.Add(rABBuild);
            }
        }
    }
}
