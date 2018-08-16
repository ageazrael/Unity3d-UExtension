using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;

using System.IO;
using UExtension.XcodeAPI;


namespace UExtension
{
    public class ProcessNativeExtension : IPostprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder
        {
            get { return 0; }
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            NativeExtension.ProcessNativeExtension(report.summary.outputPath);
        }
    }

    public static class NativeExtension
    {
        public static void ProcessNativeExtension(string path)
        {
            var rAssetGUIDs = AssetDatabase.FindAssets("t:NativeExtensionSetting");
            var rSettings = new List<NativeExtensionSetting>(rAssetGUIDs.Length);
            foreach (var rGUID in rAssetGUIDs)
            {
                var rSetting = AssetDatabase.LoadAssetAtPath<NativeExtensionSetting>(
                    AssetDatabase.GUIDToAssetPath(rGUID));
                if (rSetting && rSetting.IsEnabled)
                {
                    rSettings.Add(rSetting);
                }
            }

            ApplyXCodeProject(rSettings, path);
        }

        private static string ConvertPathRootAndCopyFiles(string rOutputPath, string rFilesPath)
        {
			var rSourceFrameworkFullPath = Path.Combine(PathRoot.ProjectPathRoot, rFilesPath);
			PathExtension.Copy(rSourceFrameworkFullPath, PathExtension.Combine(rOutputPath, rFilesPath), true);

			return Path.Combine(rOutputPath, rFilesPath);
        }
        public static void ApplyXCodeProject(List<NativeExtensionSetting> rSettings, string path)
        {
            var rInfoPlist = new PlistDocument();
            rInfoPlist.ReadFromFile(PathExtension.Combine(path, "Info.plist"));

            var rPBXProject = new PBXProject();
            rPBXProject.ReadFromFile(PBXProject.GetPBXProjectPath(path));

            var rTargetGUID = rPBXProject.TargetGuidByName("Unity-iPhone");
            foreach (var rSetting in rSettings)
            {
                foreach (var rFramework in rSetting.Frameworks)
                {
                    if (string.IsNullOrEmpty(rFramework.Path))
                        continue;

                    var rFrameworkFullPath = ConvertPathRootAndCopyFiles(path, rFramework.Path);
                    var rFrameworkGuid = rPBXProject.AddFile(rFrameworkFullPath, rSetting.name + "/" + rFramework.Path, PBXSourceTree.Sdk);
                    rPBXProject.AddFrameworkToProjectByFileGuid(rTargetGUID, rFrameworkGuid, rFramework.IsWeak);
                    rPBXProject.AddBuildProperty(rTargetGUID, "FRAMEWORK_SEARCH_PATHS", Path.GetDirectoryName(rFrameworkFullPath) + "/**");
                }
                foreach (var rBundle in rSetting.Bundles)
                {
                    if (string.IsNullOrEmpty(rBundle.Path))
                        continue;

                    var rFrameworkFullPath = ConvertPathRootAndCopyFiles(path, rBundle.Path);
                    var rFrameworkGuid = rPBXProject.AddFile(rFrameworkFullPath, rSetting.name + "/" + rBundle.Path, PBXSourceTree.Sdk);
                    rPBXProject.AddFrameworkToProject(rTargetGUID, rFrameworkGuid, false);
                }
                foreach (var rLibrary in rSetting.Librarys)
                {
                    if (string.IsNullOrEmpty(rLibrary.File))
                        continue;

                    var rLibraryFullPath = ConvertPathRootAndCopyFiles(path, rLibrary.File);
                    var rLibraryGuid = rPBXProject.AddFile(rLibraryFullPath, rSetting.name + "/" + rLibrary.File, PBXSourceTree.Sdk);
                    rPBXProject.AddFileToBuild(rTargetGUID, rLibraryGuid);
                    rPBXProject.AddBuildProperty(rTargetGUID, "LIBRARY_SEARCH_PATHS", Path.GetDirectoryName(rLibraryFullPath) + "/**");
                }
                foreach (var rCompileFile in rSetting.CompileFiles)
                {
                    if (string.IsNullOrEmpty(rCompileFile.File))
                        continue;

                    var rCompileFileFullPath = ConvertPathRootAndCopyFiles(path, rCompileFile.File);
                    var rFileGUID = rPBXProject.AddFile(rCompileFileFullPath, rSetting.name + "/" + rCompileFile.File, PBXSourceTree.Source);
                    rPBXProject.AddFileToBuildWithFlags(rTargetGUID, rFileGUID, rCompileFile.CompileFlags);

                    if (!string.IsNullOrEmpty(rCompileFile.HeadersDirectory))
                        rPBXProject.AddBuildProperty(rTargetGUID, "HEADER_SEARCH_PATHS", Path.GetDirectoryName(rCompileFileFullPath) + "/**");
                }


                var rDependentGroupName = rSetting.name + "/Dependent/";
                foreach (var rFramework in rSetting.DependentFrameworks)
                {
                    if (string.IsNullOrEmpty(rFramework.Path))
                        continue;

                    var rFrameworkGuid = rPBXProject.AddFile("System/Library/Frameworks/" + rFramework.Path,
                        rDependentGroupName + rFramework.Path, PBXSourceTree.Sdk);
                    rPBXProject.AddFrameworkToProjectByFileGuid(rTargetGUID, rFrameworkGuid, rFramework.IsWeak);
                }
                foreach (var rLibrary in rSetting.DependentLibrarys)
                {
                    if (string.IsNullOrEmpty(rLibrary.File))
                        continue;

                    rPBXProject.AddFileToBuild(rTargetGUID, rPBXProject.AddFile("usr/lib/" + rLibrary.File,
                        rDependentGroupName + rLibrary.File, PBXSourceTree.Sdk));
                }
                foreach (var rBuildProperty in rSetting.BuildPropertys)
                {
                    if (string.IsNullOrEmpty(rBuildProperty.Name) || string.IsNullOrEmpty(rBuildProperty.Value))
                        continue;

                    rPBXProject.AddBuildProperty(rTargetGUID, rBuildProperty.Name, rBuildProperty.Value);
                }

                foreach (var rPlistAsset in rSetting.MergeInfoPlist)
                {
                    if (!rPlistAsset)
                        continue;

                    var rMergePlist = new PlistDocument();
                    try
                    {
                        rMergePlist.ReadFromFile(AssetDatabase.GetAssetPath(rPlistAsset));
                    }
                    finally
                    {
                        rInfoPlist.root.MergeFrom(rMergePlist.root);
                    }
                }
            }

            rPBXProject.WriteToFile(PBXProject.GetPBXProjectPath(path));
            rInfoPlist.WriteToFile(PathExtension.Combine(path, "Info.plist"));
        }
    }
}