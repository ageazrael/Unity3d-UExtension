using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace UExtension
{
    [CreateAssetMenu()]
    public class NativeExtensionSetting : ScriptableObject
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class TargetAttribute : Attribute
        {
            public BuildTargetGroup Target;
            
            public TargetAttribute(BuildTargetGroup target)
            {
                this.Target = target;
            }
        }
		public enum FrameworkStatus
		{
			Required,
			Optional
		}
		[Serializable]
		public class Framework
		{
			[Folder(PathType.ProjectPath, ".framework", Editable = true)]
			public string Path;
			public FrameworkStatus Status;

			public bool IsWeak
			{
				get { return this.Status == FrameworkStatus.Optional; }
			}
		}
		[Serializable]
		public class Bundle
		{
			[Folder(PathType.ProjectPath, ".bundle", Editable = true)]
			public string Path;
		}
		[Serializable]
		public class Library
		{
			[FilePath(PathType.ProjectPath, "Static Library", "a,dylib", Editable = true)]
			public string File;
		}
		[Serializable]
		public class CompileFile
		{
			[FilePath(PathType.ProjectPath, "Source Files", "h,m,mm,cpp,c", Editable = true)]
			public string File;
			[Folder(PathType.ProjectPath, Editable = true)]
			public string HeadersDirectory;
			public string CompileFlags;
		}
		[Serializable]
		public class BuildProperty
		{
			public string Name;
			public string Value;
		}
        [Serializable]
        public class NativePlugin
        {
            public DefaultAsset Plugin;
            public BuildTarget  Target;
            public bool         AnyTarget;
        }

        [SerializeField]
		private bool            Enabled;
        public string           MacroSymbol;
        public NativePlugin[]   Plugins;


		public Framework[]      Frameworks;
		public Bundle[]         Bundles;
        public Library[]        Librarys;
		public CompileFile[]    CompileFiles;
		public Framework[]      DependentFrameworks;
		public Library[]        DependentLibrarys;
		public BuildProperty[]  BuildPropertys;
        public DefaultAsset[]   MergeInfoPlist;

        [MenuItem("CONTEXT/NativeExtensionSetting/Scan")]
        static void ScanMenuItem(MenuCommand rCommand)
        {
            (rCommand.context as NativeExtensionSetting).Scan();
        }
        public void Scan()
        {
            var rAssetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(rAssetPath))
                return;
            
            var rPlugins = new List<NativePlugin>();
            var rAssetDirectory = Path.GetDirectoryName(rAssetPath);
            var rAssetGuids = AssetDatabase.FindAssets("t:DefaultAsset", new string[] { rAssetDirectory });
            foreach(var rGuid in rAssetGuids)
            {
                var rImporterPath = AssetDatabase.GUIDToAssetPath(rGuid);
                var rImporter = AssetImporter.GetAtPath(rImporterPath);
                if (!rImporter)
                    continue;
                var rPluginImporter = rImporter as PluginImporter;
                if (!rPluginImporter)
                    continue;

                var rTarget = BuildTarget.NoTarget;
                try
                {
                    var rRelativePath = rImporterPath.Replace(rAssetDirectory + "/", string.Empty);
                    var nFindDivIndex = rRelativePath.IndexOf('/');
                    if (nFindDivIndex != -1)
                    {
                        rTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), rRelativePath.Substring(0, nFindDivIndex));
                    }
                }
                finally {}

                rPlugins.Add(new NativePlugin() {
                    Target = rTarget,
                    AnyTarget = rTarget == BuildTarget.NoTarget,
                    Plugin = AssetDatabase.LoadAssetAtPath<DefaultAsset>(rImporterPath)
                });
            }

            this.Plugins = rPlugins.ToArray();
        }
        public bool IsEnabled
        {
            get { return this.Enabled; }
            set { this.Enabled = value; this.OnValidate(); }
        }


        void OnValidate()
        {
            if (!string.IsNullOrEmpty(this.MacroSymbol))
                this.ApplyMacroAllBuildTargetGroup(this.MacroSymbol);

            foreach(var rNativePlugin in this.Plugins)
            {
                if (!rNativePlugin.Plugin)
                    continue;
                
                var rPluginImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(rNativePlugin.Plugin)) as PluginImporter;
                if (!rPluginImporter)
                    continue;
                bool bChanged = false;
                if (rPluginImporter.GetCompatibleWithPlatform(rNativePlugin.Target) != this.Enabled)
                {
                    rPluginImporter.SetCompatibleWithPlatform(rNativePlugin.Target, this.Enabled);
                    bChanged = true;
                }
                if (rPluginImporter.GetCompatibleWithAnyPlatform() != (rNativePlugin.AnyTarget && this.Enabled))
                {
                    rPluginImporter.SetCompatibleWithAnyPlatform(rNativePlugin.AnyTarget && this.Enabled);
                    bChanged = true;
                }
                if (bChanged)
                    rPluginImporter.SaveAndReimport();
            }




            // Checking valid plugin!
            if (null != this.Plugins)
            {
                for (var nIndex = 0; nIndex < this.Plugins.Length; ++nIndex)
                    this.Plugins[nIndex].Plugin = CheckPluginImporter(this.Plugins[nIndex].Plugin);
            }

            // Checking valid plist!
            if (null != MergeInfoPlist)
            {
                for (var nIndex = 0; nIndex < this.MergeInfoPlist.Length; ++nIndex)
                {
                    var rPlist = this.MergeInfoPlist[nIndex];
                    if (!rPlist)
                        continue;

                    var rPlistPath = AssetDatabase.GetAssetPath(rPlist);
                    if (Path.GetExtension(rPlistPath).ToLower() != ".plist")
                        this.MergeInfoPlist[nIndex] = null;
                }
            }
        }

        DefaultAsset CheckPluginImporter(DefaultAsset rPluginAsset)
        {
            if (!rPluginAsset)
                return null;

            return AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(rPluginAsset)) as PluginImporter ? rPluginAsset : null;
        }


		static BuildTargetGroup[] ValidBuildTargetGroups = {
			BuildTargetGroup.Unknown,
			BuildTargetGroup.Standalone,
			BuildTargetGroup.iOS,
			BuildTargetGroup.Android,
			BuildTargetGroup.WebGL,
			BuildTargetGroup.WSA,
			BuildTargetGroup.PSP2,
			BuildTargetGroup.PS4,
			BuildTargetGroup.XboxOne,
			BuildTargetGroup.N3DS,
			BuildTargetGroup.tvOS,
			BuildTargetGroup.Facebook,
			BuildTargetGroup.Switch,
		};
        void ApplyMacroAllBuildTargetGroup(string rMacroSymbol)
        {
			var rSettingMacros = rMacroSymbol.Split(';');
			foreach (var rBuildTargetGroup in ValidBuildTargetGroups)
			{
				if (rBuildTargetGroup == BuildTargetGroup.Unknown)
					continue;

                ApplyMacro(rBuildTargetGroup, rSettingMacros);
			}
        }
        void ApplyMacro(BuildTargetGroup rBuildTargetGroup, string[] rApplyMacros)
        {
            var rPlayerSettingMacrosText = PlayerSettings.GetScriptingDefineSymbolsForGroup(rBuildTargetGroup);
            var rPlayerSettingMacros = new List<string>(rPlayerSettingMacrosText.Split(';'));
			rPlayerSettingMacros.Sort();

			var bChanged = false;
			foreach (var rMacro in rApplyMacros)
			{
				if (string.IsNullOrEmpty(rMacro))
					continue;

				if (this.Enabled)
				{
					if (!rPlayerSettingMacros.Contains(rMacro))
					{
						rPlayerSettingMacros.Add(rMacro);
						bChanged = true;
					}
				}
				else
				{
					if (rPlayerSettingMacros.Contains(rMacro))
					{
						rPlayerSettingMacros.Remove(rMacro);
						bChanged = true;
					}
				}
			}
			if (bChanged)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(rBuildTargetGroup,
					string.Join(";", rPlayerSettingMacros.ToArray()));
			}
        }
    }
}