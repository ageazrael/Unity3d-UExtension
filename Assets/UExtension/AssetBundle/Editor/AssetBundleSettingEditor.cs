using UnityEngine;
using UnityEditor;
using System.IO;

namespace UExtension
{

    [CustomEditor(typeof(AssetBundleSettingBase), true)]
    public class AssetBundleSettingEditor : Editor
    {
        AssetBundleSettingPreview  mBuildSettingPreview = new AssetBundleSettingPreview();
        bool                       mAutoRefreshPreview  = true;
        AssetBundleSettingBase     mAssetBundleSetting  = null;

        public void OnEnable()
        {
            mAssetBundleSetting = this.target as AssetBundleSettingBase;

            if (mAutoRefreshPreview)
                mAssetBundleSetting.OnChanged += mBuildSettingPreview.Refresh;
            else
                mAssetBundleSetting.OnChanged -= mBuildSettingPreview.Refresh;

            mBuildSettingPreview.Refresh(mAssetBundleSetting);

			if (!string.IsNullOrEmpty (mAssetBundleSetting.ResourcesPath) &&
			    !Directory.Exists (mAssetBundleSetting.ResourcesPath))
			{
				Directory.CreateDirectory (mAssetBundleSetting.ResourcesPath);
				AssetDatabase.Refresh ();
			}
        }
        public void OnDisable()
        {
            if (mAssetBundleSetting)
                mAssetBundleSetting.OnChanged -= mBuildSettingPreview.Refresh;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button(string.Format("Build AssetBundle({0})", EditorUserBuildSettings.activeBuildTarget.ToString())))
                AssetBundleBuilder.Build(mAssetBundleSetting, EditorUserBuildSettings.activeBuildTarget);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(string.Format("Preview AssetBundle{0}", mAutoRefreshPreview ? "(Auto Update Preview)" : string.Empty)))
                mBuildSettingPreview.Refresh(mAssetBundleSetting);

            var rAuotRefresh = EditorGUILayout.Toggle(this.mAutoRefreshPreview, GUILayout.MaxWidth(18));
            if (rAuotRefresh != mAutoRefreshPreview)
            {
                mAutoRefreshPreview = rAuotRefresh;

                if (mAutoRefreshPreview)
                    mAssetBundleSetting.OnChanged += mBuildSettingPreview.Refresh;
                else
                    mAssetBundleSetting.OnChanged -= mBuildSettingPreview.Refresh;
            }

            EditorGUILayout.EndHorizontal();

            mBuildSettingPreview.OnGUI();
        }
    }

}