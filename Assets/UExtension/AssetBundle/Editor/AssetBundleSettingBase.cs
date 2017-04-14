using UnityEditor;
using UnityEngine;

namespace UExtension
{
    /// <summary>
    /// AssetBundleSettingBase
    /// </summary>
    [TSIgnore]
	public abstract class AssetBundleSettingBase : ScriptableObject
    {
        [Folder(FolderType.ProjectPath)]
        public string                   ExportPath;
        public string                   Postfix;
		[EnumMask]
        public BuildAssetBundleOptions  BuildOptions    = BuildAssetBundleOptions.None;
        public bool                     ExcludeInPlayer = true;

        public System.Action<AssetBundleSettingBase> OnChanged;
        public void OnValidate()
        {
            if (null != OnChanged)
                OnChanged(this);
        }

		public abstract string 				ResourcesPath { get; }
        public abstract AssetBundleBuild[]  GetAllBundle();
        public abstract string              AssetPathToLoaderName(string rAssetPath);

        public static string FindAssetsFilter
        {
            get { return string.Format("t:{0}", typeof(AssetBundleSettingBase).Name); }
        }
    }
}